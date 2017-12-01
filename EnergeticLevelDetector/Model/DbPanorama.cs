using System;

namespace EnergeticLevelDetector.Model
{
    public class DbPanorama
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong RfFrom { get; set; }
        public ulong RfTo { get; set; }
        public int FftSize { get; set; }
        public byte[] BytePanorama { get; set; }
        public uint DownConverter { get; set; }
        public int LO { get; set; }
        public DateTime DateTime { get; set; }
    }
}
