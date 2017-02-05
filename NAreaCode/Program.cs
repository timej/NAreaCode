using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using NAreaCode.Models;

//コマンドラインの解析
//https://msdn.microsoft.com/ja-jp/magazine/mt763239.aspx

namespace NAreaCode
{
    public class Program
    {
        private static string _dataPath;
        public static void Main(string[] args)
        {
            ApplicationEnvironment env = PlatformServices.Default.Application;

            var startup = new Startup(env);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _dataPath = Path.Combine(env.ApplicationBasePath, "data");

            if (args.Length > 0)
            {
                if (args[0] == "new")
                {
                    New();
                    return;
                }
                else if (args[0] == "update")
                {
                    Update();
                    return;
                }
                else if (args[0] == "pref")
                {
                    //Prefecture(dataPath);
                    //return;
                }
                    else if (args[0] == "list")
                {
                    List();
                    return;
                }
                else if (args[0] == "test")
                {
                    Test();
                    return;
                }
                else if (args[0] == "info")
                {
                    //Info(areaCodeClass);
                    //return;
                }
            }
            Comment();
        }

        private static void New()
        {
            var eStatAreaCode = new EStatAreaCode(false, _dataPath);
        }

        private static void Update()
        {
            var eStatAreaCode = new EStatAreaCode(true, _dataPath);
        }

        private static void Prefecture()
        {
            string path = Path.Combine(_dataPath, "pref.txt");
            using (var sw = new StreamWriter(File.OpenWrite(path)))
            {
                for (int n = 1; n < 48; n++)
                {
                    var pref = new Prefecture {Id = n};
                    EStatAreaCode.GetPrefectureData(pref);
                    sw.WriteLine(
                        $"new Prefecture {{Id = {pref.Id}, 名称 = \"{pref.名称}\", ふりがな = \"{pref.ふりがな}\", 英語名 = \"{pref.英語名}\"}},");
                }
            }
        }

        private static void Comment()
        {
            Console.WriteLine("NAreaCode - 地域コードツール");
            Console.WriteLine("使用方法: NAreaCodo [command] [argument]");
            Console.WriteLine("new: 地域コードデータの新規作成");
            Console.WriteLine("update: 地域コードデータの更新");
            Console.WriteLine("list: 市町村コードの一覧表示");
        }

        private static void List()
        {
            var areaCodeClass = new NAreaCodeClass(_dataPath);
            var areaCodes = areaCodeClass.GetAreaCode(0, DateTime.Today);
            foreach (var code in areaCodes)
            {
                Console.WriteLine($"{code.Id}\t{code.名称}");
            }
        }

        //Municipality Map Maker ウェブ版
        //http://www.tkirimura.com/mmm/
        //任意の2時点間の市区町村コードの対応表とのチェック
        //
        private static void Test()
        {
            //Municipality Map Maker ウェブ版のデータの読込
            var mmmDataList = MMMData.LoadMMMData(_dataPath);

            var areaCodeClass = new NAreaCodeClass(_dataPath);
            var areaCodes = areaCodeClass.GetStandardAreaCode(0, new DateTime(1970, 4, 1));
            areaCodes.Sort((x, y) => x.Id - y.Id);

            int m = 0;
            int n = 0;
            int prev = 0;

            while (m < mmmDataList.Count && n < areaCodes.Count)
            {
                if (mmmDataList[m].JisCode1 == prev)
                {
                    m++;
                    continue;
                }
                if (mmmDataList[m].JisCode1 != areaCodes[n].Id)
                {
                    if(mmmDataList[m].JisCode1 < areaCodes[n].Id)
                    {
                        Console.WriteLine($"{mmmDataList[m].JisCode1} {mmmDataList[m].PName1}{mmmDataList[m].GName1}{mmmDataList[m].CName1}は、MMMにだけあります。");
                        m++;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"{areaCodes[n].Id} {areaCodes[n].名称}は、NAreaCodeにだけあります。");
                        n++;
                        continue;
                    }
                }
                prev = mmmDataList[m].JisCode1;
                m++;
                n++;
            }
        }
    }
}
