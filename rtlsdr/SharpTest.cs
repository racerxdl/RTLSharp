using RTLSharp.Callbacks;
using RTLSharp.Extensions;
using RTLSharp.Modules;
using RTLSharp.PortAudio;
using RTLSharp.Types;
using RTLSharp.fftw;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using RTLSharp;

namespace rtlsdr {
  public partial class SharpTest : Form {
    #region Fields
    private int maxIqSamples = 16 * 1024;
    private uint frequency;
    private Device rtlDevice;
    private DataBuffer powerBuffer;
    private DataBuffer powerBufferDecimator;
    private DataBuffer fftBuffer;
    private unsafe float* powerPtr;
    private unsafe float* powerDecimatorPtr;
    private unsafe Complex* fftBufferPtr;
    private int fftBins = 16384;
    private ComplexFifo _fftStream;
    private ComplexFifo _fftStreamDecimator;
    private DataBuffer audioBuffer;
    private unsafe float* audioPtr;
    private FloatFifo _audioFifo;
    private float _fftOffset = -40f;
    private Decimator decimator;
    private InlineFloatDecimator audioDecimator;

    private Thread _dspThread;
    private AudioPlayer _audioPlayer;
    private volatile bool dspHit = true;

    private int audioSampleRate = 192000;
    private int audioBufferInMs = 100;

    private FMDemodulator fmDemod;
    private int decimatorRatio = 8;

    private DataBuffer tmpBuffer;
    private unsafe float* tmpBufferPtr;
    private float[] hammingWindow;
    private FloatLPF fmLpf;
    private int audioDeviceIdx = 0;

    private SFFT sFFT;
    private SFFT sFFTDecimator;

    #endregion
    #region Constructor / Destructor
    public unsafe SharpTest() {
      InitializeComponent();
      powerBuffer = DataBuffer.Create(fftBins, sizeof(float));
      powerBufferDecimator = DataBuffer.Create(fftBins, sizeof(float));
      fftBuffer = DataBuffer.Create(fftBins, sizeof(Complex));

      powerPtr = (float*)powerBuffer;
      powerDecimatorPtr = (float*)powerBufferDecimator;
      fftBufferPtr = (Complex*)fftBuffer;

      hammingWindow = Filters.generateHammingWindow(fftBins);

      offsetTrackBar.Value = mainSpectrumAnalyzer.DisplayOffset;
      displayRangeBar.Value = mainSpectrumAnalyzer.DisplayRange;
      fmDemod = new FMDemodulator();
      List<AudioDevice> devices = AudioDevice.GetDevices(AudioDeviceDirection.Output);
      foreach (AudioDevice d in devices) {
        if (d.IsDefault) {
          audioDeviceIdx = d.Index;
          break;
        }
      }
      updateFrequency((uint)(105.7 * 1000 * 1000));
      sFFT = new SFFT(fftBins);
    }
    #endregion
    #region Event Callbacks
    private unsafe void Decimator_SamplesAvailable(SamplesEventArgs e) {
      // http://www.designnews.com/author.asp?section_id=1419&doc_id=236273&piddl_msgid=522392
      float fftGain = (float)(10.0 * Math.Log10(fftBins / (2.0 * audioDecimator.DecimationFactor)));
      float offset = 24.0f - fftGain + _fftOffset;

      if (e.IsComplex) {
        if (tmpBuffer == null || tmpBuffer.Length < e.Length) {
          tmpBuffer = DataBuffer.Create(e.Length, sizeof(float));
          tmpBufferPtr = (float*)tmpBuffer;
        }
        fmDemod.process(e.ComplexBuffer, tmpBufferPtr, e.Length);
        fmLpf.Process(tmpBufferPtr, e.Length);
        audioDecimator.process(tmpBufferPtr, tmpBufferPtr, e.Length);
        _audioFifo.Write(tmpBufferPtr, (int)(e.Length / audioDecimator.DecimationFactor));

        sFFTDecimator.updateData(e.ComplexBuffer, e.Length);
        sFFTDecimator.execute();
        sFFTDecimator.copyOutput(e.ComplexBuffer, e.Length);

        lock (powerBuffer) {
          FFT.SpectrumPower(e.ComplexBuffer, powerDecimatorPtr, e.Length, offset);
        }
      }
    }
    private unsafe void D_SamplesAvailable(SamplesEventArgs e) {
      if (_fftStream.Length < maxIqSamples) {
        if (e.IsComplex) {
          _fftStream.Write(e.ComplexBuffer, e.Length);
        } else {
          Console.WriteLine("Weird, the buffer should be Complex");
        }
      } else {
        Console.WriteLine("Buffer is full!");
      }
    }
    private unsafe void audioBufferNeeded(SamplesEventArgs e) {
      float* audio = e.FloatBuffer;
      int length = e.Length;

      if (_audioFifo.Length >= length) {
        _audioFifo.Read(audioPtr, length);
        for (int i = 0; i < length; i++) {
          audio[i * 2] = audioPtr[i] * 50000;
          audio[i * 2 + 1] = audioPtr[i] * 50000;
        }
      } else {
        for (int i = 0; i < length; i++) {
          audio[i * 2] = 0;
          audio[i * 2 + 1] = 0;
        }
      }
    }
    #endregion
    #region DSP Methods
    private void dspWork() {
      while (dspHit) {
        if (_fftStream.Length > 0) {
          process();
        }
      }
      Console.WriteLine("DSP Thread stopped.");
    }

    private unsafe void process() {
      if (_fftStream.Length >= fftBins) {
        int samplesRead = _fftStream.Read(fftBufferPtr, fftBins);

        // http://www.designnews.com/author.asp?section_id=1419&doc_id=236273&piddl_msgid=522392
        float fftGain = (float)(10.0 * Math.Log10(fftBins / 2.0));
        float offset = 24.0f - fftGain + _fftOffset;

        decimator.Process(fftBufferPtr, samplesRead);
        if (checkBox1.Checked) {
          fixed (float* w = hammingWindow)
          {
            FFT.ApplyWindow(fftBufferPtr, w, samplesRead);
          }
        }
        
        sFFT.updateData(fftBufferPtr, samplesRead);
        sFFT.execute();
        sFFT.copyOutput(fftBufferPtr, samplesRead);

        lock (powerBuffer) {
          FFT.SpectrumPower(fftBufferPtr, powerPtr, samplesRead, offset);
        }
      }
    }
    private unsafe void updateSpectrum(int length) {
      lock (powerBuffer) {
        mainSpectrumAnalyzer.RenderSpectrum(powerPtr, length);
        if (tmpBuffer != null) {
          ifSpectrumAnalizer.RenderSpectrum(powerDecimatorPtr, tmpBuffer.Length);
        }
      }
    }
    #endregion
    #region Component Methods
    private unsafe void startButtonClick(object sender, EventArgs e) {
      rtlDevice = new Device(0);
      rtlDevice.Frequency = frequency;
      rtlDevice.SamplesAvailable += D_SamplesAvailable;
      mainSpectrumAnalyzer.Frequency = rtlDevice.Frequency;
      mainSpectrumAnalyzer.SampleRate = rtlDevice.SampleRate;

      maxIqSamples = (int)(audioBufferInMs * rtlDevice.SampleRate / 1000);

      _fftStream = new ComplexFifo(maxIqSamples);
      _fftStreamDecimator = new ComplexFifo(maxIqSamples);
      _fftStream.Open();
      _fftStreamDecimator.Open();

      decimator = new Decimator(rtlDevice.SampleRate, (uint)decimatorRatio, (uint)fftBins);
      audioDecimator = new InlineFloatDecimator(rtlDevice.SampleRate / decimatorRatio, (uint)decimatorRatio);
      audioSampleRate = (int)((rtlDevice.SampleRate / decimatorRatio) / decimatorRatio);
      audioBuffer = DataBuffer.Create((audioBufferInMs * audioSampleRate / 1000), sizeof(float));
      audioPtr = (float*)audioBuffer;
      fmLpf = new FloatLPF(audioSampleRate, 22050, 127);
      _audioFifo = new FloatFifo(16 * audioBufferInMs * audioSampleRate / 1000);
      _audioFifo.Open();
      _audioPlayer = new AudioPlayer(audioDeviceIdx, audioSampleRate, (uint)(audioBufferInMs * audioSampleRate / 1000), audioBufferNeeded);

      sFFTDecimator = new SFFT((int)(fftBins / audioDecimator.DecimationFactor));

      ifSpectrumAnalizer.Frequency = rtlDevice.Frequency;
      ifSpectrumAnalizer.SampleRate = decimator.OutputSampleRate;
      decimator.SamplesAvailable += Decimator_SamplesAvailable;
      rtlDevice.UseRtlAGC = false;
      rtlDevice.UseTunerAGC = false;
      rtlDevice.VGAGain = vgaGain.Value = 3;
      rtlDevice.LNAGain = lnaGain.Value = 7;
      rtlDevice.MixerGain = mixerGain.Value = 3;
      rtlDevice.Start();
      dspHit = true;
      _dspThread = new Thread(dspWork);
      _dspThread.Start();
      fftTimer.Enabled = true;
      startButton.Enabled = false;
      stopButton.Enabled = true;

      Console.WriteLine("Receiving Samples now.");
    }
    private void stopButtonClick(object sender, EventArgs e) {
      if (rtlDevice != null) {
        _fftStream.Close();
        _fftStreamDecimator.Close();
        dspHit = false;
        rtlDevice.Stop();
        rtlDevice = null;
        Console.WriteLine("Stopped Receiving Samples now.");
        startButton.Enabled = true;
        stopButton.Enabled = false;
      }
    }
    private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
      if (rtlDevice != null) {
        _fftStream.Close();
        _fftStreamDecimator.Close();
        dspHit = false;
        rtlDevice.Stop();
      }
    }
    private void spectrumScaleBar_ValueChanged(object sender, EventArgs e) {
      mainSpectrumAnalyzer.SpectrumScale = spectrumScaleBar.Value / 10f;
      ifSpectrumAnalizer.SpectrumScale = spectrumScaleBar.Value / 10f;
    }
    private void offsetTrackBar_ValueChanged(object sender, EventArgs e) {
      mainSpectrumAnalyzer.DisplayOffset = -offsetTrackBar.Value * 10;
      ifSpectrumAnalizer.DisplayOffset = -offsetTrackBar.Value * 10;
    }
    private void displayRangeBar_Scroll(object sender, EventArgs e) {
      mainSpectrumAnalyzer.DisplayRange = displayRangeBar.Value;
      ifSpectrumAnalizer.DisplayRange = displayRangeBar.Value;
    }
    private void lnaGain_Scroll(object sender, EventArgs e) {
      if (rtlDevice != null) {
        rtlDevice.LNAGain = lnaGain.Value;
      }
    }
    private void mixerGain_Scroll(object sender, EventArgs e) {
      if (rtlDevice != null) {
        rtlDevice.MixerGain = mixerGain.Value;
      }
    }
    private void vgaGain_Scroll(object sender, EventArgs e) {
      if (rtlDevice != null) {
        rtlDevice.VGAGain = vgaGain.Value;
      }
    }
    private void frequencyTrackBar_ValueChanged(object sender, EventArgs e) {
      updateFrequency((UInt32)frequencyTrackBar.Value * 10000);
    }
    private void fftTimer_Tick(object sender, EventArgs e) {
      updateSpectrum(fftBins);
    }
    #endregion
    #region Other Methods
    private void updateFrequency(uint newFreq) {
      if (newFreq != frequency) {
        frequency = newFreq;
        if (rtlDevice != null) {
          rtlDevice.Frequency = frequency;
          frequency = rtlDevice.Frequency;
        }

        frequencyTrackBar.Value = (int)(frequency / 10000);
        mainSpectrumAnalyzer.Frequency = frequency;
        ifSpectrumAnalizer.Frequency = frequency;
        mainSpectrumAnalyzer.Title = getMainSpectrumTitle();
        textBox1.Text = (frequency / 1000).ToString();
      }
    }
    private string getMainSpectrumTitle() {
      return string.Format("Radio Spectrum - {0:0.000} MHz", frequency / 1000000f);
    }
    #endregion

    private void textBox1_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        uint val = rtlDevice.Frequency;
        uint.TryParse(textBox1.Text, out val);
        val *= 1000;
        updateFrequency(val);
      }
    }

    private void spectrumScaleBar_Scroll(object sender, EventArgs e) {

    }
  }
}
