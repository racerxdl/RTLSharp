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
using System.Runtime.InteropServices;

namespace RTLSharp.Types {
  /// <summary>
  /// Raw Data Buffer for operating with raw pointers.
  /// </summary>
  public class DataBuffer {
    #region Fields
    private Array _buffer;
    private readonly GCHandle _handle;
    private int _length;
    private unsafe void* _ptr;
    #endregion
    #region Constructors
    private unsafe DataBuffer(Array buffer, int realLength) {
      _buffer = buffer;
      _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
      _ptr = (void*)_handle.AddrOfPinnedObject();
      _length = realLength;
    }
    ~DataBuffer() {
      Dispose();
    }
    #endregion
    #region Methods
    public static DataBuffer Create(int length, int sizeOfElement) {
      return new DataBuffer(new byte[length * sizeOfElement], length);
    }
    public unsafe void Dispose() {
      if (_handle.IsAllocated) {
        _handle.Free();
      }
      _buffer = null;
      _ptr = null;
      _length = 0;
      GC.SuppressFinalize(this);
    }
    public static unsafe implicit operator void* (DataBuffer unsafeBuffer) {
      return unsafeBuffer.Address;
    }
    #endregion
    #region Properties
    public unsafe void* Address {
      get { return _ptr; }
    }
    public int Length {
      get { return _length; }
    }
    #endregion
  }
}
