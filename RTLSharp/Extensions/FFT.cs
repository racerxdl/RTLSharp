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

using RTLSharp.Types;
using System;

namespace RTLSharp.Extensions {
  /// <summary>
  /// TODO: Rewrite this shitty code.
  /// </summary>
  public static class FFT {
    #region Constants
    private const int MAX_LUT_BITS = 16;
    private const int MAX_LUT_BINS = 1 << MAX_LUT_BITS;
    private const int LUT_SIZE = MAX_LUT_BINS / 2;
    private const double TWO_PI = 2.0 * Math.PI;
    #endregion
    #region Fields
    private unsafe static DataBuffer _lutBuffer = DataBuffer.Create(LUT_SIZE, sizeof(Complex));
    private unsafe static Complex* _lut = ((Complex*)_lutBuffer);
    #endregion
    #region Methods
    static unsafe FFT() {
      for (int i = 0; i < LUT_SIZE; i++) {
        _lut[i] = Complex.FromAngle(TWO_PI * i / MAX_LUT_BINS).Conjugate();
      }
    }
    public static unsafe void ForwardTransform(Complex* buffer, int length) {
      if (length <= MAX_LUT_BINS) {
        ForwardTransformLut(buffer, length);
      } else {
        ForwardTransformNormal(buffer, length);
      }
    }
    public static unsafe void InverseTransform(Complex* samples, int length) {
      for (int i = 0; i < length; i++) {
        samples[i].imag = -samples[i].imag;
      }
      ForwardTransform(samples, length);
      float a = 1f / length;
      for (int j = 0; j < length; j++) {
        samples[j].real *= a;
        samples[j].imag = -samples[j].imag * a;
      }
    }
    public static unsafe void Scale(float* src, byte* dest, int length, float minPower, float maxPower) {
      float a = 255f / (maxPower - minPower);
      for (int i = 0; i < length; i++) {
        float val = src[i];
        if (val < minPower) {
          val = minPower;
        } else if (val > maxPower) {
          val = maxPower;
        }
        dest[i] = (byte)((val - minPower) * a);
      }
    }
    public static unsafe void Smooth(float* srcPtr, float* dstPtr, int sourceLength, int destinationLength, float zoom = 1f, float offset = 0f) {
      zoom = zoom < 1f ? 1f : zoom;
      float zoomTarget = (sourceLength) / (zoom * destinationLength);
      float a = sourceLength * (offset + (0.5f * (1f - (1f / zoom))));
      if (zoomTarget > 1f) {
        int baseZoomTarget = (int)Math.Ceiling(zoomTarget);
        for (int i = 0; i < destinationLength; i++) {
          int n = (int)((zoomTarget * (i - 0.5f)) + a);
          float off = -600f;
          for (int j = 0; j < baseZoomTarget; j++) {
            int s = n + j;
            if (((s >= 0) && (s < sourceLength)) && (off < srcPtr[s])) {
              off = srcPtr[s];
            }
          }
          dstPtr[i] = off;
        }
      } else {
        for (int k = 0; k < destinationLength; k++) {
          int p = (int)((zoomTarget * k) + a);
          if ((p >= 0) && (p < sourceLength)) {
            dstPtr[k] = srcPtr[p];
          }
        }
      }
    }
    public unsafe static void SpectrumPower(Complex* buffer, float* power, int length, float offset) {
      for (var i = 0; i < length; i++) {
        float m = buffer[i].real * buffer[i].real + buffer[i].imag * buffer[i].imag;
        power[i] = (float)(10.0 * Math.Log10(1e-60 + m)) + offset;
      }
    }
    #endregion
    #region Private Methods
    private static unsafe void ForwardTransformLut(Complex* buffer, int length) {
      int a;
      Complex cplx;
      int b = length - 1;
      int d = length / 2;
      int c = 0;
      for (a = length; a > 1; a = a >> 1) {
        c++;
      }
      int index = d;
      a = 1;
      while (a < b) {
        if (a < index) {
          cplx = buffer[index];
          buffer[index] = buffer[a];
          buffer[a] = cplx;
        }
        int e = d;
        while (e <= index) {
          index -= e;
          e /= 2;
        }
        index += e;
        a++;
      }
      for (int i = 1; i <= c; i++) {
        int s = 1 << i;
        int t = s / 2;
        int lutPos = MAX_LUT_BITS - i;
        for (index = 1; index <= t; index++) {
          int pos = index - 1;
          Complex complex = _lut[pos << lutPos];
          for (a = pos; a <= b; a += s) {
            int v = a + t;
            cplx = complex * buffer[v];
            buffer[v] = buffer[a] - cplx;
            Complex* cPtr = buffer + a;
            cPtr[0] += cplx;
          }
        }
      }
    }
    private static unsafe void ForwardTransformNormal(Complex* buffer, int length) {
      int a;
      Complex cplx;
      int b = length - 1;
      int d = length / 2;
      int c = 0;
      for (a = length; a > 1; a = a >> 1) {
        c++;
      }
      int index = d;
      a = 1;
      while (a < b) {
        if (a < index) {
          cplx = buffer[index];
          buffer[index] = buffer[a];
          buffer[a] = cplx;
        }
        int e = d;
        while (e <= index) {
          index -= e;
          e /= 2;
        }
        index += e;
        a++;
      }
      for (int i = 1; i <= c; i++) {
        int s = 1 << i;
        int t = s / 2;
        double k = Math.PI / t;
        for (index = 1; index <= t; index++) {
          int pos = index - 1;
          Complex complex = Complex.FromAngle(k * pos).Conjugate();
          for (a = pos; a <= b; a += s) {
            int v = a + t;
            cplx = complex * buffer[v];
            buffer[v] = buffer[a] - cplx;
            Complex* cPtr = buffer + a;
            cPtr[0] += cplx;
          }
        }
      }
    }
    #endregion
  }
}
