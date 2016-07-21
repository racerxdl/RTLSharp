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

using RTLSharp.Exceptions;
using RTLSharp.Extensions;
using RTLSharp.Types;
using System;
using RTLSharp.Callbacks;
using static RTLSharp.Callbacks.ManagedCallbacks;

namespace RTLSharp.Modules {
  /// <summary>
  /// Complex Number Decimator
  /// </summary>
  public class Decimator : IDisposable {
    #region Constants
    private const int TAPS = 63;
    private const int ATTENUATION = 150;
    #endregion
    #region Fields
    private DataBuffer _complexInputBuffer;
    private DataBuffer _complexOutputBuffer;
    private readonly SamplesEventArgs _eventArgs = new SamplesEventArgs();
    private uint _decimationFactor;
    private long _sampleRate;
    private long _outSampleRate;
    private ComplexLPF _lpf;
    #endregion
    #region Events
    public event SamplesAvailableEvent SamplesAvailable;
    #endregion
    #region Constructors
    public unsafe Decimator(long sampleRate, uint decimationRatio = 1, uint buffSize = 16384) {
      if (decimationRatio == 0 || (decimationRatio > 1 && !Utils.IsPowerOfTwo(decimationRatio))) {
        throw new InvalidArgumentException("The decimationRatio needs to be 1 or a power of two number.");
      }

      _complexInputBuffer = DataBuffer.Create((int)buffSize, sizeof(Complex));
      _complexOutputBuffer = DataBuffer.Create((int)(buffSize / decimationRatio), sizeof(Complex));
      _sampleRate = sampleRate;
      _outSampleRate = sampleRate / decimationRatio;
      _decimationFactor = decimationRatio;

      _lpf = new ComplexLPF(_sampleRate, _outSampleRate / 2, TAPS, ATTENUATION, _decimationFactor);
    }
    #endregion
    #region Methods
    public unsafe void Process(Complex* input, int inputLen) {
      if (inputLen > _complexInputBuffer.Length) {
        resizeBuffer(inputLen);
      }

      Complex* output = (Complex *)_complexOutputBuffer;
      Complex* inputBuff = (Complex*)_complexInputBuffer;

      Utils.memcpy((void *)inputBuff, (void*)input, inputLen * sizeof(Complex));

      int outSize = (int) (inputLen / _decimationFactor);
      _lpf.Process(inputBuff, inputLen);
      for (int i = 0; i < outSize; i++) {
        output[i] = inputBuff[i * _decimationFactor];
      }

      _eventArgs.IsComplex = true;
      _eventArgs.ComplexBuffer = output;
      _eventArgs.Length = outSize;
      SamplesAvailable(_eventArgs);
    }
    public void Dispose() {
      _complexInputBuffer.Dispose();
      _complexOutputBuffer.Dispose();
      _complexInputBuffer = null;
      _complexOutputBuffer = null;
      GC.SuppressFinalize(this);
    }
    #endregion
    #region Private Methods
    private unsafe void resizeBuffer(int newSize) {
      _complexInputBuffer = DataBuffer.Create(newSize, sizeof(Complex));
      _complexOutputBuffer = DataBuffer.Create((int)(newSize / _decimationFactor), sizeof(Complex));
    }
    #endregion
    #region Properties
    public long InputSampleRate {
      get { return _sampleRate; }
      set {
        if (_sampleRate != value) {
          _sampleRate = value;
          _outSampleRate = _sampleRate / _decimationFactor;
          _lpf = new ComplexLPF(_sampleRate, _outSampleRate / 2, TAPS, ATTENUATION, _decimationFactor);
        }
      }
    }
    public long OutputSampleRate {
      get { return _outSampleRate; }
    }
    public uint DecimationFactor {
      get { return _decimationFactor; }
    }
    #endregion
  }
}
