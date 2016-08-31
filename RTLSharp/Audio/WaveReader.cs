using System;
using System.IO;
using System.Text;
using RTLSharp.Audio.Structs;
using System.Text.RegularExpressions;
using System.Globalization;

namespace RTLSharp.Audio {
  public class WaveReader {
    private readonly static Regex SDRSharpRegex = new Regex(@"SDRSharp_([0-9]+)_([0-9]+)._([0-9]+)Hz_(.+)\.wav", RegexOptions.IgnoreCase);

    private static bool isSDRSharpFile(string fileName) {
      return SDRSharpRegex.IsMatch(fileName);
    }
    public static Wave readFromFile(string fileName) {
      if (File.Exists(fileName)) {
        ChunkHeader header = new ChunkHeader();
        FormatChunkHeader subHeader = new FormatChunkHeader();
        DataChunkHeader dataHeader = new DataChunkHeader();
        Wave wave = new Wave();
        string basename = Path.GetFileName(fileName);
        if (isSDRSharpFile(basename)) {
          MatchCollection matches = SDRSharpRegex.Matches(basename);
          GroupCollection groups = matches[0].Groups;
          uint.TryParse(groups[3].Value, out wave.Frequency);
          wave.DateTime = DateTime.ParseExact(groups[1].Value + groups[2].Value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open))) {
          subHeader.Format = WavType.UNKNOWN;

          header.Id = Encoding.UTF8.GetString(reader.ReadBytes(4));
          header.Size = reader.ReadUInt32();
          header.Format = Encoding.UTF8.GetString(reader.ReadBytes(4));

          if (!header.Id.Equals("RIFF")) {
            Console.WriteLine("Non Wave file!");
          }

          Console.WriteLine("ID: {0}\nSize: {1}\nFormat: {2}", header.Id, header.Size, header.Format);

          bool cont = true;
          while (reader.BaseStream.Position != reader.BaseStream.Length && cont) {
            string chunkId = Encoding.UTF8.GetString(reader.ReadBytes(4)).Trim();
            switch (chunkId) {
              case "fmt":
                subHeader.Id = chunkId;
                subHeader.Size = reader.ReadUInt32();
                subHeader.Format = (WavType)reader.ReadUInt16();
                subHeader.Channels = reader.ReadUInt16();
                subHeader.SampleRate = reader.ReadUInt32();
                subHeader.ByteRate = reader.ReadUInt32();
                subHeader.BlockAlign = reader.ReadUInt16();
                subHeader.BitsPerSample = reader.ReadUInt16();

                wave.SampleRate = subHeader.SampleRate;

                Console.WriteLine("Format Chunk 0:\n\tId: {0}\n\tSize: {1}\n\tFormat: {2}\n\tChannels: {3}\n\tSample Rate: {4}\n\tByte Rate: {5}\n\tBlock Align: {6}\n\tBitsPerSample: {7}", subHeader.Id, subHeader.Size, subHeader.Format, subHeader.Channels, subHeader.SampleRate, subHeader.ByteRate, subHeader.BlockAlign, subHeader.BitsPerSample);
                break;
              case "data":
                dataHeader.Id = chunkId;
                dataHeader.Size = reader.ReadUInt32();
                wave.Data = new WaveStream(fileName, reader.BaseStream.Position, dataHeader.Size, subHeader);
                
                Console.WriteLine("Data Chunk 0:\n\tId: {0}\n\tSize: {1}", dataHeader.Id, dataHeader.Size);
                cont = false;
                break;
              default:
                Console.WriteLine("Not recognized: {0}", chunkId);
                cont = false;
                break;
            }
          }
        }
        return wave;
      } else {
        Console.WriteLine("File does not exists!");
        return null;
      }
    }
  }
}
