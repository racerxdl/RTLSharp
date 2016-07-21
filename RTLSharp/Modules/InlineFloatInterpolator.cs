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

namespace RTLSharp.Modules {
  /// <summary>
  /// Inline Float Array Interpolator
  /// </summary>
  public class InlineFloatInterpolator {
    #region Constants
    private const int TAPS = 63;
    private const int ATTENUATION = 150;
    #endregion
    #region Fields
    private FloatLPF _lpf;
    private uint _interpolationFactor;
    #endregion
    #region Constructors
    public InlineFloatInterpolator(long sampleRate, uint interpolationFactor) {
      _lpf = new FloatLPF(sampleRate, (sampleRate * interpolationFactor) / 2, TAPS, ATTENUATION);
      _interpolationFactor = interpolationFactor;
    }
    #endregion
    #region Methods
    public unsafe void Process(float* input, float* output, int length) {
      int targetLen = (int)(length * _interpolationFactor);
      uint idx;
      for (uint i = 0; i < length; i++) {
        idx = i * _interpolationFactor;
        output[idx] = input[i];
        for (int j=1;j<_interpolationFactor;j++) {
          output[idx + j] = 0;
        }
      }
      _lpf.Process(output, targetLen);
    }
    #endregion
    #region Properties
    public uint InterpolationFactor {
      get { return _interpolationFactor; }
    }
    #endregion
  }
}
