using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EnergeticLevelDetector.Model
{
    public struct WavHeader
    {
        public byte[] chunkID; // "riff"
        public uint chunkSize;  // number of bytes in data
        public byte[] format;  // "WAVE"
        public byte[] subChunk1ID;  // "fmt "
        public uint subchunk1Size;
        public ushort audioFormat;
        public ushort numChannels; // number of channels. Can be 4 or 6 depending on smth. The thing is, if numChannels==4, then first is the main, tha second is max, third - min and the last one - colorCode
        public uint sampleRate;
        public uint byteRate;
        public ushort blockAlign;
        public ushort bitsPerSample;  //number of bits per one sample
        public byte[] subchunk2ID; // "data"
        public uint subchunk2Size;

        public static Panorama GetSpectrumFromFile(string path)
        {
            Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return GetDataFromStream(fileStream);
        }
        public static Panorama GetSpectrumFromByteArray(byte[] array)
        {
            if (array != null)
            {
                Stream stream = new MemoryStream(array);
                return GetDataFromStream(stream);
            }
            return null;
        }

        #region Private
        private static Panorama GetDataFromStream(Stream stream)
        {
            WavHeader Header = new WavHeader();
            List<double> middleList = new List<double>();
            List<double> topList = new List<double>();
            List<double> bottomList = new List<double>();
            uint leftBound = 0, rightBound = 0;
            int L0 = 0;
            int deltaL0 = 0;

            using (stream)
            using (BinaryReader binaryReader = new BinaryReader(stream))
            {
                Header.chunkID = binaryReader.ReadBytes(4);
                var riff = Encoding.Default.GetString(Header.chunkID);
                Header.chunkSize = binaryReader.ReadUInt32();
                Header.format = binaryReader.ReadBytes(4);
                var wav = Encoding.Default.GetString(Header.format);
                Header.subChunk1ID = binaryReader.ReadBytes(4);
                var id = Encoding.Default.GetString(Header.chunkID);
                Header.subchunk1Size = binaryReader.ReadUInt32();
                Header.audioFormat = binaryReader.ReadUInt16();
                Header.numChannels = binaryReader.ReadUInt16();
                Header.sampleRate = binaryReader.ReadUInt32();
                Header.byteRate = binaryReader.ReadUInt32();
                Header.blockAlign = binaryReader.ReadUInt16();
                Header.bitsPerSample = binaryReader.ReadUInt16();//20
                L0 = binaryReader.ReadInt32();
                deltaL0 = binaryReader.ReadInt32();
                leftBound = binaryReader.ReadUInt32();
                rightBound = binaryReader.ReadUInt32();

                var toSkip = (int)Header.subchunk1Size + 20 - 52;
                binaryReader.ReadBytes(toSkip);

                Header.subchunk2ID = binaryReader.ReadBytes(4);
                var id2 = Encoding.Default.GetString(Header.subchunk2ID);
                Header.subchunk2Size = binaryReader.ReadUInt32();
                // Some data is left. 16 bytes. It must be some additional information about satelites

                int i = 0;

                //while (binaryReader.BaseStream.Length > binaryReader.BaseStream.Position)
                while (i < Header.subchunk2Size)
                {
                    try
                    {
                        middleList.Add((short)binaryReader.ReadUInt16() / 100.0);
                        i = i + 2;
                        if (Header.numChannels > 1)
                        {
                            topList.Add((short)binaryReader.ReadUInt16() / 100.0);
                            i = i + 2;
                        }
                        if (Header.numChannels > 2)
                        {
                            bottomList.Add((short)binaryReader.ReadUInt16() / 100.0);
                            i = i + 2;
                            for (int j = 0; j < Header.numChannels - 3; j++)
                            {
                                binaryReader.ReadUInt16();
                                i = i + 2;
                            }
                        }
                    }
                    catch (EndOfStreamException)
                    {
                    }
                }
            }
            long L0inHz = (long)L0 * 1000000;
            return new Panorama(middleList, bottomList, topList, (ulong)(leftBound + L0inHz), (ulong)(rightBound + L0inHz), L0, deltaL0);
        }
        #endregion
    }
}
