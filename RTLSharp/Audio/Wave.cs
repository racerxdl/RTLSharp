using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLSharp.Audio {
  public class Wave {
    public uint SampleRate;
    public uint Frequency;
    public DateTime DateTime;
    public WaveStream Data;
  }
}
