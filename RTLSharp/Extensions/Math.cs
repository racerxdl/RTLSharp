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

namespace RTLSharp.Extensions {
  /// <summary>
  /// Math Helper Class
  /// </summary>
  public class RtlMath {
    public static uint GCD(uint x1, uint x2) {
      uint a, b, g, z;

      if (x1 > x2) {
        a = x1;
        b = x2;
      } else {
        a = x2;
        b = x1;
      }

      if (b == 0)
        return 0;

      g = b;
      while (g != 0) {
        z = a % g;
        a = g;
        g = z;
      }
      return a;
    }
  }
}
