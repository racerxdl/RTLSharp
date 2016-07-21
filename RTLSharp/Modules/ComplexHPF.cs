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

using RTLSharp.Extensions;
using RTLSharp.Types;

namespace RTLSharp.Modules {
  /// <summary>
  /// Complex Array High Pass Filter
  /// </summary>
  public class ComplexHPF {
    #region Constants
    private const int TAPS = 63;
    private const int ATTENUATION = 150;
    #endregion
    #region Fields
    private float[] _taps;
    #endregion
    #region Constructors
    public ComplexHPF(float sampleRate, float cutFrequency, int length = TAPS, float attenuation = ATTENUATION) {
      _taps = Filters.kaiserBesselFilter(sampleRate, cutFrequency, sampleRate / 2f, length, attenuation);
    }
    #endregion
    #region Methods
    public unsafe void Process(Complex* data, int length) {
      int s = _taps.Length;
      float accI;
      float accQ;
      for (int i = 0; i < length; i++) {
        accI = 0;
        accQ = 0;
        for (int j = 0; j < s; j++) {
          if (i + j >= length) {
            break;
          }
          accI += data[i + j].real * _taps[j];
          accQ += data[i + j].imag * _taps[j];
        }
        data[i].real = accI;
        data[i].imag = accQ;
      }
    }
    #endregion
  }
}
