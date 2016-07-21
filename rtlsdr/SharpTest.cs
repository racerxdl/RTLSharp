using RTLSharp.Callbacks;
using RTLSharp.Extensions;
using RTLSharp.Modules;
using RTLSharp.PortAudio;
using RTLSharp.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace rtlsdr {
  public partial class SharpTest : Form {
    private int maxIqSamples = 16 * 1024;
    private uint frequency = 106300000;
    private Device d;
    private DataBuffer powerBuffer;
    private DataBuffer powerBufferDecimator;
    private DataBuffer fftBuffer;
    private DataBuffer fftDecimatorBuffer;
    private unsafe float* powerPtr;
    private unsafe float* powerDecimatorPtr;
    private unsafe Complex* fftBufferPtr;
    private unsafe Complex* fftBufferDecimatorPtr;
    private int fftBins = 16384;
    private ComplexFifo _fftStream;
    private ComplexFifo _fftStreamDecimator;
    private DataBuffer audioBuffer;
    private unsafe float* audioPtr;
    private FloatFifo _audioFifo;
    private object lockObj = new object();
    private float _fftOffset = -40f;
    private Decimator decimator;
    private InlineFloatDecimator audioDecimator;

    private Thread _dspThread;
    private AudioPlayer _audioPlayer;

    private object locker = new object();
    private volatile bool dspHit = true;

    private int audioSampleRate = 192000;
    private int audioBufferInMs = 100;

    private FMDemodulator fmDemod;
    private int decimatorRatio = 8;

    private DataBuffer tmpBuffer;
    private unsafe float* tmpBufferPtr;

    public unsafe SharpTest() {
      InitializeComponent();
      powerBuffer = DataBuffer.Create(fftBins, sizeof(float));
      powerBufferDecimator = DataBuffer.Create(fftBins, sizeof(float));
      fftBuffer = DataBuffer.Create(fftBins, sizeof(Complex));
      fftDecimatorBuffer = DataBuffer.Create(fftBins, sizeof(Complex));

      powerPtr = (float*)powerBuffer;
      powerDecimatorPtr = (float*)powerBufferDecimator;
      fftBufferPtr = (Complex*)fftBuffer;
      fftBufferDecimatorPtr = (Complex*)fftDecimatorBuffer;


      offsetTrackBar.Value = spectrumAnalyzer1.DisplayOffset;
      trackBar3.Value = spectrumAnalyzer1.DisplayRange;
      fmDemod = new FMDemodulator();
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

    private void dspWork() {
      while (dspHit) {
        if (_fftStream.Length > 0) {
          process();
        }
      }
      Console.WriteLine("DSP Thread stopped.");
    }

    private unsafe void updateSpectrum(int length) {
      spectrumAnalyzer1.RenderSpectrum(powerPtr, length);
    }

    private unsafe void button1_Click(object sender, EventArgs e) {

      d = new Device(0);
      d.Frequency = frequency;
      d.SamplesAvailable += D_SamplesAvailable;
      spectrumAnalyzer1.Frequency = d.Frequency;
      spectrumAnalyzer1.SampleRate = d.SampleRate;

      maxIqSamples = (int)(audioBufferInMs * d.SampleRate / 1000);

      _fftStream = new ComplexFifo(maxIqSamples);
      _fftStreamDecimator = new ComplexFifo(maxIqSamples);
      _fftStream.Open();
      _fftStreamDecimator.Open();

      decimator = new Decimator(d.SampleRate, (uint)decimatorRatio, (uint)fftBins);
      audioDecimator = new InlineFloatDecimator(d.SampleRate / decimatorRatio, (uint)decimatorRatio);
      audioSampleRate = (int)((d.SampleRate / decimatorRatio) / decimatorRatio);
      audioBuffer = DataBuffer.Create((audioBufferInMs * audioSampleRate / 1000), sizeof(float));
      audioPtr = (float*)audioBuffer;
      tmpBuffer = DataBuffer.Create((audioBufferInMs * audioSampleRate / 1000), sizeof(float));
      tmpBufferPtr = (float*)tmpBuffer;
      fmLpf = new FloatLPF(audioSampleRate, 22050, 127);
      _audioFifo = new FloatFifo(16 * audioBufferInMs * audioSampleRate / 1000);
      _audioFifo.Open();
      _audioPlayer = new AudioPlayer(3, audioSampleRate, (uint)(audioBufferInMs * audioSampleRate / 1000), audioBufferNeeded);


      spectrumAnalyzer2.Frequency = d.Frequency;
      spectrumAnalyzer2.SampleRate = decimator.OutputSampleRate;
      decimator.SamplesAvailable += Decimator_SamplesAvailable;
      d.UseRtlAGC = false;
      d.UseTunerAGC = false;
      d.VGAGain = vgaGain.Value = 3;
      d.LNAGain = lnaGain.Value = 7;
      d.MixerGain = mixerGain.Value = 3;
      d.Start();
      dspHit = true;
      _dspThread = new Thread(dspWork);
      _dspThread.Start();

      Console.WriteLine("Receiving Samples now.");
    }

    private FloatLPF fmLpf;

    private unsafe void Decimator_SamplesAvailable(SamplesEventArgs e) {
      float num7 = (float)(10.0 * Math.Log10(fftBins / 16.0));
      float offset = (24f - num7) + _fftOffset;

      if (e.IsComplex) {

        int audioLength = (int)(e.Length / audioDecimator.DecimationFactor);
        if (tmpBuffer.Length < e.Length) {
          tmpBuffer = DataBuffer.Create(e.Length, sizeof(float));
          tmpBufferPtr = (float*)tmpBuffer;
        }
        fmDemod.process(e.ComplexBuffer, tmpBufferPtr, e.Length);
        fmLpf.Process(tmpBufferPtr, e.Length);
        audioDecimator.process(tmpBufferPtr, tmpBufferPtr, e.Length);
        _audioFifo.Write(tmpBufferPtr, (int)(e.Length / audioDecimator.DecimationFactor));

        FFT.ForwardTransform(e.ComplexBuffer, e.Length);
        FFT.SpectrumPower(e.ComplexBuffer, powerDecimatorPtr, e.Length, offset);
        spectrumAnalyzer2.RenderSpectrum(powerDecimatorPtr, e.Length);
      }
    }

    private unsafe void D_SamplesAvailable(SamplesEventArgs e) {
      if (_fftStream.Length < maxIqSamples) {
        if (e.IsComplex) {
          _fftStream.Write(e.ComplexBuffer, e.Length);
        }
      }
    }

    private unsafe void process() {
      if (_fftStream.Length >= fftBins) {
        int samplesRead = _fftStream.Read(fftBufferPtr, fftBins);

        float bins = (float)(10.0 * Math.Log10(fftBins / 2.0));
        float offset = (24f - bins) + _fftOffset;
        decimator.Process(fftBufferPtr, samplesRead);

        FFT.ForwardTransform(fftBufferPtr, samplesRead);
        FFT.SpectrumPower(fftBufferPtr, powerPtr, samplesRead, offset);
        if (spectrumAnalyzer1.InvokeRequired) {
          spectrumAnalyzer1.Invoke(new MethodInvoker(() => {
            updateSpectrum(samplesRead);
          }));
        } else {
          updateSpectrum(samplesRead);
        }
      }
    }

    private void button2_Click(object sender, EventArgs e) {
      if (d != null) {
        _fftStream.Close();
        _fftStreamDecimator.Close();
        dspHit = false;
        d.Stop();
        d = null;
        Console.WriteLine("Stopped Receiving Samples now.");
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
      if (d != null) {
        _fftStream.Close();
        _fftStreamDecimator.Close();
        dspHit = false;
        d.Stop();
      }
    }

    private void trackBar1_ValueChanged(object sender, EventArgs e) {
      spectrumAnalyzer1.SpectrumScale = trackBar1.Value / 10f;
      spectrumAnalyzer2.SpectrumScale = trackBar1.Value / 10f;
    }

    private void trackBar4_ValueChanged(object sender, EventArgs e) {
      spectrumAnalyzer1.DisplayOffset = -offsetTrackBar.Value * 10;
      spectrumAnalyzer2.DisplayOffset = -offsetTrackBar.Value * 10;
    }

    private void trackBar3_Scroll(object sender, EventArgs e) {
      spectrumAnalyzer1.DisplayRange = trackBar3.Value;
      spectrumAnalyzer2.DisplayRange = trackBar3.Value;
    }
    
    private void lnaGain_Scroll(object sender, EventArgs e) {
      if (d != null) {
        d.LNAGain = lnaGain.Value;
      }
    }

    private void mixerGain_Scroll(object sender, EventArgs e) {
      if (d != null) {
        d.MixerGain = mixerGain.Value;
      }
    }

    private void vgaGain_Scroll(object sender, EventArgs e) {
      if (d != null) {
        d.VGAGain = vgaGain.Value;
      }
    }

    private void trackBar5_ValueChanged(object sender, EventArgs e) {
      frequency = (UInt32)trackBar5.Value * 10000;
      label1.Text = "F: " + frequency;
      spectrumAnalyzer1.Frequency = frequency;
      spectrumAnalyzer2.Frequency = frequency;
      if (d != null) {
        d.Frequency = frequency;
      }
    }
  }
}
