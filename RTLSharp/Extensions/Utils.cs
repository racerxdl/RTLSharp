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

using System.Runtime.InteropServices;

namespace RTLSharp.Extensions {
  /// <summary>
  /// Utils
  /// </summary>
  public static class Utils {
    /// <summary>
    /// Map for memcpy syscall.
    /// </summary>
    /// <param name="dest">Destination Memory Pointer</param>
    /// <param name="src">Source Memory Pointer</param>
    /// <param name="len">Length to copy</param>
    /// <returns></returns>
    [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe void* memcpy(void* dest, void* src, int len);
    /// <summary>
    /// Returns true if x is a power of two number
    /// </summary>
    /// <param name="x">true if x is power of two (x = 2 ^ y)</param>
    /// <returns></returns>
    public static bool IsPowerOfTwo(ulong x) {
      return (x & (x - 1)) == 0;
    }
  }
}
