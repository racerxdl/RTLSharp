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

using System;

namespace RTLSharp.Extensions {
  /// <summary>
  /// Filter Generators
  /// </summary>
  public static class Filters {
    /// <summary>
    /// Calculates the FIR Coeficients for a Kaiser-Bessel Filter
    /// </summary>
    /// <param name="sampleRate">The sample rate</param>
    /// <param name="lowFrequency">The Low Frequency (Fa)</param>
    /// <param name="highFrequency">The High Frequency (Fb)</param>
    /// <param name="length">The length of the filter (odd number)</param>
    /// <param name="attenuation">The attenuation in dB</param>
    /// <returns>FIR Taps</returns>
    public static float[] kaiserBesselFilter(float sampleRate, float lowFrequency, float highFrequency, int length, float attenuation = 30) {
      float[] taps = new float[length];
      int Np = (length - 1) / 2;
      float[] inpRes = new float[Np+1];
      float[] window = new float[Np+1];
      float alpha;

      if (attenuation > 50) {
        alpha = 0.1102f * (attenuation - 8.7f);
      } else if (attenuation >= 21 ) {
        alpha = (float)(0.5842 * Math.Pow(attenuation - 21, 0.4f) + 0.07886 * (attenuation - 21));
      } else {
        alpha = 0f;
      }

      // The filter is symmetric, so we only need to calculate half of it.
      for (int i=0; i<=Np; i++) {
        // Window
        float i0a = besselI0(alpha);
        float npSq = (float)(i * i / (Np * Np));
        window[i] = besselI0((float) (alpha * Math.Sqrt(1 - npSq)) ) / i0a;

        // Impulse Response
        if (i > 0) {
          inpRes[i] = (float) ((Math.Sin((2 * i * Math.PI * highFrequency) / sampleRate) - Math.Sin((2 * i * Math.PI * lowFrequency) / sampleRate)) / (i * Math.PI));
        } else {
          inpRes[i] = 2 * (highFrequency - lowFrequency) / sampleRate;
        }

        // Taps
        taps[Np + i] = window[i] * inpRes[i];
      }

      // Calculate the other end of the filter
      for (int i=0; i<Np; i++) {
        taps[i] = taps[length - 1 - i];
      }

      return taps;
    }

    /// <summary>
    /// Calculates the 0th order Bessel Function
    /// </summary>
    /// <param name="x">x</param>
    /// <returns>f(x)</returns>
    private static float besselI0(float x) {
      float d = 0, ds = 1, s = 1;
      do {
        d += 2;
        ds *= x * x / (d * d);
        s += ds;
      } while (ds > s * 1e-6);
      return s;
    }
    /// <summary>
    /// Simple Low Pass Filter
    /// </summary>
    /// <param name="sampleRate">The sampleRate</param>
    /// <param name="cutFreq">The Cut Frequency</param>
    /// <param name="length">Length of the Filter (Odd Number)</param>
    /// <returns></returns>
    public static float[] simpleLowPass(float sampleRate, float cutFreq, int length) {
      length += (length + 1) % 2;
      float[] taps = new float[length];
      var freq = cutFreq / sampleRate;
      var center = Math.Floor(length / 2.0);
      var sum = 0.0;

      for (var i = 0; i < length; ++i) {
        double val;
        if (i == center) {
          val = 2 * Math.PI * freq;
        } else {
          var angle = 2 * Math.PI * (i + 1) / (length + 1);
          val = Math.Sin(2 * Math.PI * freq * (i - center)) / (i - center);
          val *= 0.42 - 0.5 * Math.Cos(angle) + 0.08 * Math.Cos(2 * angle);
        }
        sum += val;
        taps[i] = (float)val;
      }

      for (var i = 0; i < length; ++i) {
        taps[i] /= (float)sum;
      }

      return taps;
    }
  }
}
