///////////////////////////////////////////////////////////////////////////////////
///    C# RTLSDR                                                                ///
///    Copyright(C) 2016 Lucas Teske                                            ///
///                                                                             ///
///    This program is free software: you can redistribute it and/or modify     ///
///    it under the terms of the GNU General Public License as published by     ///
///    the Free Software Foundation, either version 3 of the License, or        ///
///    any later version.                                                       ///
///                                                                             ///
///    This program is distributed in the hope that it will be useful,          ///
///    but WITHOUT ANY WARRANTY; without even the implied warranty of           ///
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the              ///
///    GNU General Public License for more details.                             ///
///                                                                             ///
///    You should have received a copy of the GNU General Public license        ///
///    along with this program.If not, see<http://www.gnu.org/licenses/>.       ///
///////////////////////////////////////////////////////////////////////////////////

using RTLSharp.Callbacks;
using RTLSharp.Exceptions;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using static RTLSharp.Callbacks.ManagedCallbacks;
using static RTLSharp.Callbacks.NativeCallbacks;

namespace RTLSharp.Types {
  /// <summary>
  /// RTLSDR Device Class
  /// </summary>
  public sealed class Device : IDisposable {
    #region Constants
    private const uint DefaultFrequency = 106300000;
    private const int DefaultSamplerate = 2560000;
    #endregion
    #region Static Fields
    private static readonly float[] _lut = new float[256];
    private static readonly uint _readLength = 16384;
    private unsafe static readonly RtlSdrReadAsyncCallback _rtlCallback = new RtlSdrReadAsyncCallback(rtlsdrSamplesAvailableCallback);
    #endregion
    #region Readonly Fields
    private readonly SamplesEventArgs _eventArgs = new SamplesEventArgs();
    private readonly uint _index;
    private readonly string _name;
    private readonly int[] _supportedGains;
    private readonly bool _supportsOffsetTuning;
    #endregion
    #region Fields
    private uint _centerFrequency = DefaultFrequency;
    private IntPtr _dev;
    private int _frequencyCorrection;
    private GCHandle _gcHandle;
    private DataBuffer _buffer;
    private unsafe Complex* _iqPtr;
    private uint _sampleRate = DefaultSamplerate;
    private SamplingMode _samplingMode;
    private bool _useOffsetTuning;
    private bool _useRtlAGC;
    private bool _useTunerAGC = true;
    private Thread _worker;
    private int _lnaGain = 0;
    private int _mixerGain = 0;
    private int _vgaGain = 0;
    private float _alpha;
    private float _iavg;
    private float _qavg;
    #endregion
    #region Events
    public event SamplesAvailableEvent SamplesAvailable;
    #endregion
    #region Constructors / Destructors
    public Device(uint index) {
      _index = index;
      if (NativeMethods.rtlsdr_open(out _dev, _index) != 0) {
        throw new RTLSharpException("Cannot open RTL device. Is the device locked somewhere?");
      }

      int tunerGains = (_dev == IntPtr.Zero) ? 0 : NativeMethods.rtlsdr_get_tuner_gains(_dev, null);
      if (tunerGains < 0) {
        tunerGains = 0;
      }
      _useTunerAGC = true;

      _supportsOffsetTuning = NativeMethods.rtlsdr_set_offset_tuning(_dev, false) != -2;
      _supportedGains = new int[tunerGains];
      if (tunerGains >= 0) {
        NativeMethods.rtlsdr_get_tuner_gains(_dev, _supportedGains);
      }

      _name = NativeMethods.rtlsdr_get_device_name(_index);
      _gcHandle = GCHandle.Alloc(this);
    }
    public void Dispose() {
      Console.WriteLine("Device Disposed");
      Stop();
      NativeMethods.rtlsdr_close(_dev);
      if (_gcHandle.IsAllocated) {
        _gcHandle.Free();
      }
      _dev = IntPtr.Zero;
      GC.SuppressFinalize(this);
    }
    #endregion
    #region Methods
    /// <summary>
    /// Initializes a lookup table to fast conversion from the byte source to float target.
    /// </summary>
    static Device() {
      for (int i = 0; i < 256; i++) {
        _lut[i] = (i - 128) * 0.007874016f;
      }
    }
    /// <summary>
    /// Called by rtlsdrSamplesAvailableCallback for correcting the IQ samples 
    /// and calling the event handler
    /// </summary>
    /// <param name="buffer">Complex Numbers Buffer</param>
    /// <param name="length">Length of Complex Numbers Buffer</param>
    private unsafe void complexSamplesAvailableCallback(Complex* buffer, int length) {
      if (SamplesAvailable != null) {
        for (int i = 0; i < length; i++) {
          // Correcting the IQ Samples to remove DC Offset
          _iavg += _alpha * (buffer[i].real - _iavg);
          _qavg += _alpha * (buffer[i].imag - _qavg);
          buffer[i].real -= _iavg;
          buffer[i].imag -= _qavg;
        }
        _eventArgs.IsComplex = true;
        _eventArgs.ComplexBuffer = buffer;
        _eventArgs.Length = length;
        SamplesAvailable(_eventArgs);
      }
    }
    /// <summary>
    /// Callback from the librtlsdr when samples are available.
    /// <para>Calls complexSamplesAvailableCallback with a complex buffer</para>
    /// </summary>
    /// <param name="buf">Sample buffer</param>
    /// <param name="len">Length of sample buffer</param>
    /// <param name="ctx">Context</param>
    private static unsafe void rtlsdrSamplesAvailableCallback(byte* buf, uint len, IntPtr ctx) {
      GCHandle handle = GCHandle.FromIntPtr(ctx);
      if (handle.IsAllocated) {
        Device target = (Device)handle.Target;
        int length = (int)(len / 2);
        if ((target._buffer == null) || (target._buffer.Length != length)) {
          target._buffer = DataBuffer.Create(length, sizeof(Complex));
          target._iqPtr = (Complex*)target._buffer;
        }
        Complex* complexPtr = target._iqPtr;
        for (int i = 0; i < length; i++) {
          buf++;
          complexPtr->imag = _lut[buf[0]];
          buf++;
          complexPtr->real = _lut[buf[0]];
          complexPtr++;
        }
        target.complexSamplesAvailableCallback(target._iqPtr, target._buffer.Length);
      }
    }
    /// <summary>
    /// Starts the working thread
    /// </summary>
    public void Start() {
      _alpha = (float)(1.0 - Math.Exp(-1.0 / (SampleRate * 0.05f)));
      _iavg = 0;
      _qavg = 0;

      if (_worker != null) {
        throw new DeviceWorkerAlreadyRunning("This device worker is already running!");
      }

      if (NativeMethods.rtlsdr_set_sample_rate(_dev, _sampleRate) != 0) {
        throw new SetParametersException("Cannot set device sample rate!");
      }

      if (NativeMethods.rtlsdr_set_center_freq(_dev, _centerFrequency) != 0) {
        throw new SetParametersException("Cannot set device center frequency!");
      }

      if (NativeMethods.rtlsdr_set_tuner_gain_mode(_dev, !_useTunerAGC) != 0) {
        throw new SetParametersException("Cannot set Gain Mode");
      }

      if (NativeMethods.rtlsdr_set_tuner_gain_ext(_dev, _lnaGain, _mixerGain, _vgaGain) != 0) {
        throw new SetParametersException("Cannot set gains");
      }

      if (NativeMethods.rtlsdr_reset_buffer(_dev) != 0) {
        throw new SetParametersException("Cannot reset rtlsdr buffer");
      }

      _worker = new Thread(new ThreadStart(WorkerMethod));
      _worker.Priority = ThreadPriority.Highest;
      _worker.Start();
    }
    /// <summary>
    /// Stops the Working Thread
    /// </summary>
    public void Stop() {
      if (_worker != null) {
        NativeMethods.rtlsdr_cancel_async(_dev);
        if (_worker.ThreadState == ThreadState.Running) {
          _worker.Join();
        }
        _worker = null;
      }
    }
    /// <summary>
    /// Refresh the Gains
    /// </summary>
    private void RefreshGains() {
      if (_dev != IntPtr.Zero) {
        NativeMethods.rtlsdr_set_tuner_gain_ext(_dev, _lnaGain, _mixerGain, _vgaGain);
      }
    }
    /// <summary>
    /// Internal Stream Processor: Calls the RTLSDR Library ReadAsync
    /// </summary>
    private void WorkerMethod() {
      NativeMethods.rtlsdr_read_async(_dev, _rtlCallback, (IntPtr)_gcHandle, 0, _readLength);
    }
    #endregion
    #region Properties
    /// <summary>
    /// Device Tuned Frequency
    /// </summary>
    public uint Frequency {
      get { return _centerFrequency; }
      set {
        _centerFrequency = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_center_freq(_dev, _centerFrequency);
          _centerFrequency = NativeMethods.rtlsdr_get_center_freq(_dev);
        }
      }
    }
    /// <summary>
    /// Device Frequency Correction (PPM)
    /// </summary>
    public int FrequencyCorrection {
      get { return _frequencyCorrection; }
      set {
        _frequencyCorrection = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_freq_correction(_dev, _frequencyCorrection);
        }
      }
    }
    /// <summary>
    /// R820T LNA Gain
    /// </summary>
    public int LNAGain {
      get { return _lnaGain; }
      set {
        _lnaGain = value;
        RefreshGains();
      }
    }
    /// <summary>
    /// R820T Mixer Gain
    /// </summary>
    public int MixerGain {
      get { return _mixerGain; }
      set {
        _mixerGain = value;
        RefreshGains();
      }
    }
    /// <summary>
    /// R820T VGA Gain
    /// </summary>
    public int VGAGain {
      get { return _vgaGain; }
      set {
        _vgaGain = value;
        RefreshGains();
      }
    }
    /// <summary>
    /// Device Index
    /// </summary>
    public uint Index => _index;
    /// <summary>
    /// Running State
    /// </summary>
    public bool IsRunning => _worker != null;
    /// <summary>
    /// Device Name
    /// </summary>
    public string Name => _name;
    /// <summary>
    /// Device Sample Rate
    /// </summary>
    public uint SampleRate {
      get { return _sampleRate; }
      set {
        _sampleRate = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_sample_rate(_dev, _sampleRate);
        }
      }
    }
    /// <summary>
    /// Device Sampling Mode
    /// </summary>
    public SamplingMode SamplingMode {
      get { return _samplingMode; }
      set {
        _samplingMode = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_direct_sampling(_dev, (int)_samplingMode);
        }
      }
    }
    /// <summary>
    /// Device supports offset tunning
    /// </summary>
    public bool SupportsOffsetTuning => _supportsOffsetTuning;

    /// <summary>
    /// Device Tuner Type
    /// </summary>
    public TunerType TunerType {
      get {
        if (!(_dev == IntPtr.Zero)) {
          return NativeMethods.rtlsdr_get_tuner_type(_dev);
        }
        return TunerType.Unknown;
      }
    }
    /// <summary>
    /// Device use offset tunning
    /// </summary>
    public bool UseOffsetTuning {
      get { return _useOffsetTuning; }
      set {
        _useOffsetTuning = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_offset_tuning(_dev, _useOffsetTuning);
        }
      }
    }

    /// <summary>
    /// Use Device RTL Automatic Gain Control
    /// </summary>
    public bool UseRtlAGC {
      get { return _useRtlAGC; }
      set {
        _useRtlAGC = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_agc_mode(_dev, _useRtlAGC);
        }
      }
    }
    /// <summary>
    /// Use Device Tuner Automatic Gain Control
    /// </summary>
    public bool UseTunerAGC {
      get { return _useTunerAGC; }
      set {
        _useTunerAGC = value;
        if (_dev != IntPtr.Zero) {
          NativeMethods.rtlsdr_set_tuner_gain_mode(_dev, _useTunerAGC);
        }
      }
    }
    #endregion
  }
}
