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

namespace RTLSharp.Modules {
  /// <summary>
  /// Float Array Low Pass Filter
  /// </summary>
  public class FloatLPF {
    #region Constants
    private const int TAPS = 63;
    private const int ATTENUATION = 150;
    #endregion
    #region Fields
    private float[] _taps;
    private uint _decimationFactor;
    #endregion
    #region Constructors
    public FloatLPF(float sampleRate, float cutFrequency, int length = TAPS, float attenuation = ATTENUATION, uint decimationFactor = 1) {
      _taps = Filters.kaiserBesselFilter(sampleRate, 0, cutFrequency, length, attenuation);
      _decimationFactor = decimationFactor;
    }
    #endregion
    #region Methods
    public unsafe void Process(float* data, int length) {
      int s = _taps.Length;
      float acc;
      for (uint i = 0; i < length; i+= _decimationFactor) {
        acc = 0;
        for (int j = 0; j < s; j++) {
          if (i + j >= length) {
            break;
          }
          acc += data[i + j] * _taps[j];
        }
        data[i] = acc;
      }
    }
    #endregion
    #region Properties
    public uint DecimationFactor {
      get { return _decimationFactor; }
    }
    #endregion
  }
}
