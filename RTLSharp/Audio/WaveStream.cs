using RTLSharp.Audio.Structs;
using RTLSharp.Types;
using System;
using System.IO;

namespace RTLSharp.Audio {
  public class WaveStream {
    private BinaryReader _reader;
    private string _fileName;
    private long _readPosition;
    private long _readSize;
    private FormatChunkHeader _format;

    internal WaveStream(string fileName, long readPosition, long readSize, FormatChunkHeader format) {
      _readPosition = readPosition;
      _readSize = readSize;
      _fileName = fileName;
      _format = format;
    }

    public void Open() {
      if (File.Exists(FileName)) {
        _reader = new BinaryReader(File.Open(FileName, FileMode.Open));
        _reader.BaseStream.Position = _readPosition;
      }
    }

    public void Close() {
      if (_reader != null) {
        _reader.Close();
      }
    }
    public float readFloat() {
      return (_reader.BaseStream.Position < _readPosition + _readSize) ? _reader.ReadSingle() : 0.0f;
    }

    public Int16 readInt16() {
      return (_reader.BaseStream.Position < _readPosition + _readSize) ? _reader.ReadInt16() : (Int16) 0;
    }

    public byte readByte() {
      return (_reader.BaseStream.Position < _readPosition + _readSize) ? _reader.ReadByte() : (byte)0;
    }

    public Complex readComplex() {
      float real = 0.0f, imag = 0.0f;

      switch (_format.Format) {
        case WavType.FLOAT:
          real = readFloat();
          imag = (_format.Channels == 2) ? readFloat() : 0.0f;
          break;
        case WavType.PCM:
          if (_format.Size == 8) {
            real = (readByte() - 128) / 127.0f;
            imag = (_format.Channels == 2) ? (readByte() - 128) / 127.0f : 0.0f;
          } else if (_format.Size == 16) {
            real = readInt16() / 16384.0f;
            imag = (_format.Channels == 2) ? readInt16() / 16384.0f : 0.0f;
          }
          break;
      }

      return new Complex(real, imag);
    } 

    public void readComplexes(ref Complex[] cs, uint size) {
      for (int i=0; i<size; i++) {
        cs[i] = readComplex();
      }
    }

    public unsafe void readComplexes(Complex *cs, uint size) {
      for (int i = 0; i < size; i++) {
        cs[i] = readComplex();
      }
    }

    public string FileName {
      get { return _fileName; }
    }
  }
}
