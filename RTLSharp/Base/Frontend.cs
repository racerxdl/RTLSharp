using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RTLSharp.Base.Callbacks.ManagedCallbacks;

namespace RTLSharp.Base {
  public interface Frontend {
    event SamplesAvailableEvent SamplesAvailable;
    string Name { get; }
    uint SampleRate { get; }
    uint Frequency { get; set; }
    void Start();
    void Stop();
  }
}
