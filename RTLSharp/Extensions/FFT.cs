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
    #region Methods
 
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
        float m = (buffer[i].real * buffer[i].real) + (buffer[i].imag * buffer[i].imag);
        power[i] = (float)(10.0 * Math.Log10(1e-60 + m)) + offset;
      }
    }
    public static unsafe void ApplyWindow(Complex* buffer, float* window, int length) {
      for (var i = 0; i < length; i++) {
        buffer[i].real *= window[i];
        buffer[i].imag *= window[i];
      }
    }
    #endregion
  }
}
