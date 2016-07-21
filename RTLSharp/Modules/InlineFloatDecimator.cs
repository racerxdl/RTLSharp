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
  /// Inline float decimator
  /// </summary>
  public class InlineFloatDecimator {
    #region Constants
    private const int TAPS = 63;
    private const int ATTENUATION = 150;
    #endregion
    #region Fields
    private FloatLPF _lpf;
    private uint _decimationFactor;
    #endregion
    #region Constructors
    public InlineFloatDecimator(long sampleRate, uint decimationFactor) {
      _lpf = new FloatLPF(sampleRate, sampleRate / (2 * decimationFactor), TAPS, ATTENUATION, decimationFactor);
      _decimationFactor = decimationFactor;
    }
    #endregion
    #region Methods
    public unsafe void process(float *input, float* output, int length) {
      _lpf.Process(input, length);
      int targetLen = (int) ( length / _decimationFactor);
      for (int i=0; i<targetLen;i++) {
        output[i] = input[i * _decimationFactor];
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
