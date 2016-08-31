using RTLSharp.Base;
using RTLSharp.Base.Callbacks;
using RTLSharp.Types;

namespace RTLSharp.RTL {
  public class RTLFrontend : Frontend {
    private Device _device;

    public event ManagedCallbacks.SamplesAvailableEvent SamplesAvailable;

    public RTLFrontend(Device d) {
      _device = d;
      _device.SamplesAvailable += _device_SamplesAvailable;
    }

    private void _device_SamplesAvailable(SamplesEventArgs e) {
      SamplesAvailable?.Invoke(e);
    }

    public uint Frequency {
      get { return _device.Frequency; }
      set { _device.Frequency = value; }
    }

    public string Name {
      get { return "RTLSDR (" + _device.TunerType.ToString() + ")"; }
    }

    public uint SampleRate {
      get { return _device.SampleRate; }
    }

    public void Start() {
      _device.Start();
    }

    public void Stop() {
      _device.Stop();
    }

    public Device Device {
      get { return _device; }
    }
  }
}
