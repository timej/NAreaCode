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
        public static void Main(string[] args)
        {
            ApplicationEnvironment env = PlatformServices.Default.Application;

            var startup = new Startup(env);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string dataPath = Path.Combine(env.ApplicationBasePath, "data");

            if (args.Length > 0)
            {
                if (args[0] == "new")
                {
                    New(dataPath);
                    return;
                }
                else if (args[0] == "update")
                {
                    Update(dataPath);
                    return;
                }
                else if (args[0] == "pref")
                {
                    //Prefecture(dataPath);
                    //return;
                }
                else
                {
                    var areaCodeClass = new NAreaCodeClass(dataPath);
                    if (args[0] == "list")
                    {
                        List(areaCodeClass);
                        return;
                    }
                    else if (args[0] == "info")
                    {
                        //Info(areaCodeClass);
                        //return;
                    }
                }
            }
            Comment();
        }

        private static void New(string dataPath)
        {
            var eStatAreaCode = new EStatAreaCode(false, dataPath);
        }

        private static void Update(string dataPath)
        {
            var eStatAreaCode = new EStatAreaCode(true, dataPath);
        }

        private static void Prefecture(string dataPath)
        {
            string path = Path.Combine(dataPath, "pref.txt");
            using (var sw = new StreamWriter(File.OpenWrite(path)))
            {
                for (int n = 1; n < 48; n++)
                {
                    var pref = new Prefecture{Id = n};
                    EStatAreaCode.GetPrefectureData(pref);
                    sw.WriteLine($"new Prefecture {{Id = {pref.Id}, 名称 = \"{pref.名称}\", ふりがな = \"{pref.ふりがな}\", 英語名 = \"{pref.英語名}\"}},");
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

        private static void List(NAreaCodeClass areaCodeClass)
        {
            var areaCodes = areaCodeClass.GetAreaCode(0, DateTime.Today);
            foreach (var code in areaCodes)
            {
                Console.WriteLine($"{code.Id}\t{code.名称}");
            }
        }
    }
}
