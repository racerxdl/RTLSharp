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
using System;
using System.Collections.Generic;

namespace RTLSharp.Types {
  public class ComplexFifo : IDisposable {
    #region Constants
    private const int BlockSize = 65536 / 8;
    private const int MaxBlocksInCache = (4 * 1024 * 1024) / BlockSize;
    #endregion
    #region Readonly Data
    private readonly int _maxSize;
    private readonly Locker _readLocker;
    private readonly Stack<DataBuffer> _stackCache = new Stack<DataBuffer>();
    private readonly List<DataBuffer> _blockBuffer = new List<DataBuffer>();
    #endregion
    #region FIFO Data
    private int _size = 0;
    private int _readPos = 0;
    private int _writePos = 0;
    private bool _terminated = false;
    #endregion
    #region Constructors
    public ComplexFifo(int maxSize) {
      _readLocker = new Locker();
      _maxSize = maxSize;
    }
    ~ComplexFifo() {
      Dispose();
    }
    #endregion
    #region Methods
    public void Dispose() {
      Close();
      GC.SuppressFinalize(this);
    }
    public void Open() {
      _terminated = false;
    }
    public void Close() {
      _terminated = true;
      Flush();
    }
    public void Flush() {
      lock (this) {
        _blockBuffer.ForEach(a => FreeBlock(a));
        _blockBuffer.Clear();
        _readPos = 0;
        _writePos = 0;
        _size = 0;
      }
      if (_readLocker != null) {
        _readLocker.Unlock();
      }
    }
    public unsafe int Read(Complex* buf, int ofs, int count) {
      if (_readLocker != null) {
        while (_size == 0 && !_terminated) {
          _readLocker.Wait();
        }
        if (_terminated) {
          return 0;
        }
      }

      int result = 0;

      lock (this) {
        result = ReadData(buf, ofs, count);
        UpdateBlockPosition(result);
      }

      return result;
    }
    public unsafe void Write(Complex* buffer, int bufOffset, int length) {
      lock (this) {
        int bytesLeft = length;
        while (bytesLeft > 0) {
          int toWrite = Math.Min(BlockSize - _writePos, bytesLeft);
          DataBuffer block = GetBlock();
          Complex* blockPtr = (Complex*)block;
          Utils.memcpy(blockPtr + _writePos, buffer + bufOffset + length - bytesLeft, toWrite * sizeof(Complex));
          _writePos += toWrite;
          bytesLeft -= toWrite;
        }
        _size += length;
      }

      if (_readLocker != null) {
        _readLocker.Unlock();
      }
    }
    public unsafe int Read(Complex* buf, int count) => Read(buf, 0, count);
    public unsafe void Write(Complex* buf, int count) => Write(buf, 0, count);
    #endregion
    #region Properties
    public int Length {
      get { return _size; }
    }
    #endregion
    #region Private Methods
    private unsafe DataBuffer AllocBlock() {
      var result = _stackCache.Count > 0 ? _stackCache.Pop() : DataBuffer.Create(BlockSize, sizeof(Complex));
      return result;
    }
    private void FreeBlock(DataBuffer block) {
      if (_stackCache.Count < MaxBlocksInCache) {
        _stackCache.Push(block);
      }
    }
    private DataBuffer GetBlock() {
      DataBuffer result;
      if (_writePos < BlockSize && _blockBuffer.Count > 0) {
        result = _blockBuffer[_blockBuffer.Count - 1];
      } else {
        result = AllocBlock();
        _blockBuffer.Add(result);
        _writePos = 0;
      }
      return result;
    }
    private int UpdateBlockPosition(int count) {
      int sizeLeft = count;
      while (sizeLeft > 0 && _size > 0) {
        if (_readPos == BlockSize) {
          _readPos = 0;
          FreeBlock(_blockBuffer[0]);
          _blockBuffer.RemoveAt(0);
        }
        var toFeed = _blockBuffer.Count == 1 ? Math.Min(_writePos - _readPos, sizeLeft) : Math.Min(BlockSize - _readPos, sizeLeft);
        _readPos += toFeed;
        sizeLeft -= toFeed;
        _size -= toFeed;
      }
      return count - sizeLeft;
    }
    private unsafe int ReadData(Complex* buf, int ofs, int count) {
      var sizeLeft = count;
      var tempBlockPos = _readPos;
      var tempSize = _size;

      var currentBlock = 0;
      while (sizeLeft > 0 && tempSize > 0) {
        if (tempBlockPos == BlockSize) {
          tempBlockPos = 0;
          currentBlock++;
        }
        var upper = currentBlock < _blockBuffer.Count - 1 ? BlockSize : _writePos;
        var toFeed = Math.Min(upper - tempBlockPos, sizeLeft);
        var block = _blockBuffer[currentBlock];
        var blockPtr = (Complex*)block;
        Utils.memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(Complex));
        sizeLeft -= toFeed;
        tempBlockPos += toFeed;
        tempSize -= toFeed;
      }
      return count - sizeLeft;
    }
    #endregion
  }
}
