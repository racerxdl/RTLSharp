using System.Runtime.InteropServices;

namespace RTLSharp.FFTW {
  [StructLayout(LayoutKind.Sequential)]
  public struct FFTWComplex {
    #region Fields
    public double real;
    public double imag;
    #endregion
  }
}
