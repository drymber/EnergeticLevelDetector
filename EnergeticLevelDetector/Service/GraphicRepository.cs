using EnergeticLevelDetector.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EnergeticLevelDetector.Service
{
    [Serializable]
    public class GraphicRepository
    {
        public void GetAll()
        {
            var graphics = new List<double[]>();
            var neededDir = $"{AssemblyDirectory}Resources";
            foreach (var file in Directory.GetFiles(neededDir))
            {
                var panorama = WavHeader.GetSpectrumFromFile(file);
                graphics.Add(panorama.CenterData.ToArray());
            }
            Graphics = graphics;
        }

        public IEnumerable<double[]> Graphics { get; set; }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
       
        public void Serialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    formatter.Serialize(fs, Graphics);
                }
                catch (SerializationException e)
                {
                    throw;
                }
            }
        }
        public void Deserialize(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Graphics = (IEnumerable<double[]>)formatter.Deserialize(fs);
                }
                catch (SerializationException e)
                {
                    throw;
                }
            }
        }
    }
}
