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

namespace RTLSharp.Modules {
  /// <summary>
  /// Simple IQ FM Demodulator
  /// </summary>
  public class FMDemodulator {
    #region Fields
    private Complex _lastIq;
    private float _gain = 1E-05f;
    #endregion
    #region Constructors
    public FMDemodulator() { }
    public FMDemodulator(float gain) {
      _gain = gain;
    }
    #endregion
    #region Methods
    public unsafe void process(Complex* input, float* output, int length) {
      for (int i = 0; i < length; i++) {
        Complex complex = input[i] * _lastIq.Conjugate();
        float mod = complex.Modulus();
        if (mod > 0f) {
          complex /= mod;
        }
        float argument = complex.Argument();
        output[i] = argument * _gain;
        _lastIq = input[i];
      }
    }
    #endregion
  }
}
