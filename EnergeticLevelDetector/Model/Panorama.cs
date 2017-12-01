using System;
using System.Collections.Generic;

namespace EnergeticLevelDetector.Model
{
    public class Panorama : ICloneable
    {
        private const int _million = 1000000;
        private bool _areBoundsInMhz;

        public Panorama() { }
        public Panorama(List<double> center, List<double> bottom, List<double> top, ulong leftBound, ulong rightBound, int l0, int deltaL0)
        {
            CenterData = center;
            TopData = top;
            BottomData = bottom;
            LeftBound = leftBound;
            RightBound = rightBound;
            Step = (double)(RightBound - LeftBound) / (double)center.Count;
            L0 = l0;
            DeltaL0 = deltaL0;
        }

        #region Properties
        public uint Id { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public int FftSize { get; set; }
        public List<double> CenterData { get; set; }
        public List<double> TopData { get; set; }
        public List<double> BottomData { get; set; }
        public double Step { get; set; }
        public int L0 { get; set; }
        public int DeltaL0 { get; set; }
        public ulong LeftBound { get; set; }
        public ulong RightBound { get; set; } 
        #endregion

        public static explicit operator Panorama(DbPanorama dbPanorama)
        {
            var panorama = new Panorama();
            panorama.Id = (uint)dbPanorama.Id;
            panorama.Name = dbPanorama.Name;
            panorama.DateTime = dbPanorama.DateTime;
            panorama.LeftBound = dbPanorama.RfFrom;
            panorama.RightBound = dbPanorama.RfTo;
            panorama.FftSize = dbPanorama.FftSize;
            return panorama;
        }
        
        public void ConvertBoundsToMhz()
        {
            LeftBound /= _million;
            RightBound /= _million;
            _areBoundsInMhz = true;
        }

        public object Clone()
        {
            return new Panorama(CenterData, BottomData, TopData, LeftBound, RightBound, L0, DeltaL0);
        }
    }
}
