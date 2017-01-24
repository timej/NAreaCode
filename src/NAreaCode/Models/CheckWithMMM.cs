using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NAreaCode.Models
{
    public class MMMData
    {
        public int JisCode1 { get; set; }
        public string PName1 { get; set; }
        public string GName1 { get; set; }
        public string CName1 { get; set; }
        public int JisCode2 { get; set; }
        public string PName2 { get; set; }
        public string GName2 { get; set; }
        public string CName2 { get; set; }

        public static List<MMMData> LoadMMMData(string dataPath)
        {
            string path = Path.Combine(dataPath, "codelist_19701001and20151001.tsv");
            var mmmList = new List<MMMData>(); 
            using (var sr = new StreamReader(File.OpenRead(path)))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    string[] data = s.Split('\t');
                    mmmList.Add(new MMMData
                    {
                        JisCode1 = int.Parse(data[2]),
                        PName1 = data[3],
                        GName1 = data[4],
                        CName1 = data[5],
                        JisCode2 = int.Parse(data[8]),
                        PName2 = data[9],
                        GName2 = data[10],
                        CName2 = data[11],
                    });
                }
            }
            return mmmList;
        }
    }
}
