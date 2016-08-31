using System;

namespace RTLSharp.Audio.Structs {
  struct ChunkHeader {
    public string Id;
    public UInt32 Size;
    public string Format;
  }

  struct FormatChunkHeader {
    public string Id;
    public UInt32 Size;
    public WavType Format;
    public UInt16 Channels;
    public UInt32 SampleRate;
    public UInt32 ByteRate;
    public UInt16 BlockAlign;
    public UInt16 BitsPerSample;
  }

  struct DataChunkHeader {
    public string Id;
    public UInt32 Size;
  }
}
