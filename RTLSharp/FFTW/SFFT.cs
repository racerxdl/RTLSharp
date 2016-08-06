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
      length = length > _bins ? _bins : length;
      for (int i=0; i<length;i++) {
        id[i].imag = c[i].imag;
        id[i].real = c[i].real;
      }
      for (int i=length;i<_bins;i++) {
        id[i].real = 0;
        id[i].imag = 0;
      }
    }

    public unsafe void copyOutput(Complex *c, int length) {
      FFTWComplex* od = (FFTWComplex*)_output;
      for (int i = _bins; i < length; i++) {
        c[i].imag = 0;
        c[i].real = 0;
      }
      length = length > _bins ? _bins : length;
      for (int i=0;i<length;i++) {
        c[i].imag = (float)od[i].imag;
        c[i].real = (float)od[i].real;
      }
    }

    public void execute() {
      NativeFFTW.fftw_execute(_plan);
    }

    ~SFFT() {
      NativeFFTW.fftw_destroy_plan(_plan);
      NativeFFTW.fftw_free(_input);
      NativeFFTW.fftw_free(_output);
    }
  }
}
