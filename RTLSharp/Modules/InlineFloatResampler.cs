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
  /// Inline Float Array Resampler
  /// </summary>
  public class InlineFloatResampler {
    #region Fields
    private DataBuffer _midBuffer;
    private InlineFloatDecimator _decimator;
    private InlineFloatInterpolator _interpolator;

    private uint _interpolationRatio;
    private uint _decimationRatio;
    #endregion
    #region Constructors
    public InlineFloatResampler(long sampleRate, uint interpolationRatio, uint decimationRatio, int buffSize = 16384) {
      uint gcd = RtlMath.GCD(interpolationRatio, decimationRatio);
      if (gcd == 0) {
        _interpolationRatio = interpolationRatio;
        _decimationRatio = decimationRatio;
      } else {
        _interpolationRatio = interpolationRatio / gcd;
        _decimationRatio = decimationRatio / gcd;
      }

      _decimator = new InlineFloatDecimator(sampleRate, _decimationRatio);
      _interpolator = new InlineFloatInterpolator(sampleRate, _interpolationRatio);

      _midBuffer = DataBuffer.Create(buffSize, sizeof(float));
    }
    #endregion
    #region Methods
    public int NeededOutputBufferSize(int length) {
      return (int)((((float)_interpolationRatio) * length) / _decimationRatio);
    }
    public unsafe void process(float* input, float* output, int length) {
      int midSize = NeededOutputBufferSize(length);
      if (midSize > _midBuffer.Length) {
        resizeBuffer(midSize);
      }
      _interpolator.Process(input, (float *)_midBuffer, length);
      _decimator.process((float*)_midBuffer, output, midSize);
    }
    #endregion
    #region Private Methods
    private void resizeBuffer(int length) {
      _midBuffer.Dispose();
      _midBuffer = DataBuffer.Create(length, sizeof(float));
    }
    #endregion
  }
}
