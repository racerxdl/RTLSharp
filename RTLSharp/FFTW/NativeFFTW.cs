using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RTLSharp.fftw {
  public class NativeFFTW {

    private const string LibFFTW = "libfftw3-3";

    [DllImport(LibFFTW, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr fftw_malloc(int n);
    [DllImport(LibFFTW, CallingConvention = CallingConvention.Cdecl)]
    public static extern void fftw_free(IntPtr p);
    [DllImport(LibFFTW, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr fftw_plan_dft_1d(int n, IntPtr input, IntPtr output, int sign, uint flags);
    [DllImport(LibFFTW, CallingConvention = CallingConvention.Cdecl)]
    public static extern void fftw_destroy_plan(IntPtr plan);
    [DllImport(LibFFTW, CallingConvention = CallingConvention.Cdecl)]
    public static extern void fftw_execute(IntPtr plan);
  }
}
