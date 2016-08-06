using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RTLSharp.fftw {
  [StructLayout(LayoutKind.Sequential)]
  public struct FFTWComplex {
    #region Fields
    public double real;
    public double imag;
    #endregion
  }
}
