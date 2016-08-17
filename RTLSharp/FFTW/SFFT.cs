using RTLSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTLSharp.fftw {
  public class SFFT {
    private IntPtr _input;
    private IntPtr _output;
    private IntPtr _plan;
    private int _bins;
    public unsafe SFFT(int bins) {
      _input = NativeFFTW.fftw_malloc(sizeof(FFTWComplex) * bins);
      _output = NativeFFTW.fftw_malloc(sizeof(FFTWComplex) * bins);
      _plan = NativeFFTW.fftw_plan_dft_1d(bins, _input, _output, FFTW.FFTW_FORWARD, FFTW.FFTW_ESTIMATE);
      _bins = bins;
    }

    public unsafe void updateData(Complex *c, int length) {
      FFTWComplex* id = (FFTWComplex *)_input;
      if (length != _bins) {
        Console.WriteLine("Warning! Reallocing SFFT!");
        realloc(length);
      }
      for (int i=0; i<length;i++) {
        id[i].imag = c[i].imag;
        id[i].real = c[i].real;
      }
    }

    public unsafe void copyOutput(Complex *c, int length) {
      FFTWComplex* od = (FFTWComplex*)_output;
      if (length != _bins) {
        Console.WriteLine("Warning! Reallocing SFFT!");
        realloc(length);
      }
      int middle = _bins / 2;
      // FFTW have DC (Zero-Frequency) at the left corner (od[0]). But the SDR Expects a symmetric FFT with the DC in the center.
      for (int i=0; i< middle; i++) {
        // Lower
        c[i].imag = (float)od[middle + i].imag;
        c[i].real = (float)od[middle + i].real;

        // Upper
        c[middle + i].imag = (float)od[i].imag;
        c[middle + i].real = (float)od[i].real;
      }
    }

    private void destroy() {
      NativeFFTW.fftw_destroy_plan(_plan);
      NativeFFTW.fftw_free(_input);
      NativeFFTW.fftw_free(_output);
    }

    private unsafe void realloc(int bins) {
      destroy();
      _input = NativeFFTW.fftw_malloc(sizeof(FFTWComplex) * bins);
      _output = NativeFFTW.fftw_malloc(sizeof(FFTWComplex) * bins);
      _plan = NativeFFTW.fftw_plan_dft_1d(bins, _input, _output, FFTW.FFTW_FORWARD, FFTW.FFTW_ESTIMATE);
      _bins = bins;
    }

    public void execute() {
      NativeFFTW.fftw_execute(_plan);
    }

    ~SFFT() {
      destroy();
    }
  }
}
