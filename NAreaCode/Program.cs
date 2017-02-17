using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.CommandLineUtils;
using NAreaCode.Models;

//コマンドラインの解析
//https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34
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

            //Windows のコマンドプロンプトが既定ではSift_Jisコードのための対応
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _dataPath = Path.Combine(env.ApplicationBasePath, "data");

            var app = new CommandLineApplication();
            app.Name = "nacode";
            app.HelpOption("-?|-h|--help");

            app.Command("new", (command) =>
            {
                command.Description = "地域コードデータの更新";
                command.OnExecute(() =>
                {
                    New();
                    return 0;
                });
            });

            app.Command("update", (command) =>
            {
                command.Description = "地域コードデータの更新";
                command.OnExecute(() =>
                {
                    Update();
                    return 0;
                });
            });

            app.Command("list", (command) =>
            {
                command.Description = "市区町村コードの一覧表示";
                command.HelpOption("-?|-h|--help");
                var pref = command.Option(
                   "-p |--pref <都道府県コード>",
                    "都道府県コードは、1から47までの数字で入力",
                    CommandOptionType.SingleValue);
                var date = command.Option(
                    "-d |--date <日付>",
                    "その日付の市区町村の一覧を表示。指定がない場合は今日。",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    DateTime dt;
                    if (date.HasValue())
                    { 
                        if (!DateTime.TryParse(date.Value(), out dt))
                        {
                            Console.WriteLine("日付を正しい形式で入力してください。");
                            return 0;
                        }
                    }
                    else
                        dt = DateTime.Today;
                    if (pref.HasValue())
                    {
                        if (int.TryParse(pref.Value(), out int r))
                            List(r, dt);
                        else
                            Console.WriteLine("-pオプションは、都道府県をコード1～47で指定してください");
                    }
                    else
                        List(dt);
                    return 0;
                });
            });

            app.Command("number", (command) =>
            {
                command.Description = "市町村数を表示";
                command.HelpOption("-?|-h|--help");
                var pref = command.Option(
                   "-p |--pref <都道府県コード>",
                    "都道府県コードは、1から47までの数字で入力",
                    CommandOptionType.SingleValue);
                var date = command.Option(
                    "-d |--date <日付>",
                    "その日付の市区町村の一覧を表示。指定がない場合は今日。",
                    CommandOptionType.SingleValue);
                command.OnExecute(() =>
                {
                    DateTime dt;
                    if (date.HasValue())
                    {
                        if (!DateTime.TryParse(date.Value(), out dt))
                        {
                            Console.WriteLine("日付を正しい形式で入力してください。");
                            return 0;
                        }
                    }
                    else
                        dt = DateTime.Today;
                    if (pref.HasValue())
                    {
                        if (int.TryParse(pref.Value(), out int r))
                            List(r, dt);
                        else
                            Console.WriteLine("-pオプションは、都道府県をコード1～47で指定してください");
                    }
                    else
                        List(dt);
                    Number(dt);
                    return 0;
                });
            });

            app.Command("test", (command) =>
            {
                command.Description = "MMMウェブ版の対応表とのチェック";
                command.OnExecute(() =>
                {
                    Test();
                    return 0;
                });
            });

            app.Execute(args);
        }

        private static void New()
        {
            var eStatAreaCode = new EStatAreaCode(false, _dataPath);
        }

        private static void Update()
        {
            var eStatAreaCode = new EStatAreaCode(true, _dataPath);
        }


        private static void List(DateTime dt)
        {
            var areaCodeClass = new NAreaCodeClass(_dataPath);
            var areaCodes = areaCodeClass.GetAreaCode(dt);
            foreach (var code in areaCodes)
            {
                Console.WriteLine($"{code.id}\t{code.名称}");
            }
        }

        private static void List(int pref, DateTime dt)
        {
            var areaCodeClass = new NAreaCodeClass(_dataPath);
            var areaCodes = areaCodeClass.GetAreaCode(pref, dt);
            if (areaCodes.Any())
            {
                foreach (var code in areaCodes)
                {
                    Console.WriteLine($"{code.id}\t{code.名称}");
                }
            }
            else
                Console.WriteLine("-pオプションは、都道府県をコード1～47で指定してください");
        }

        private static void Number(DateTime dt)
        {
            var areaCodeClass = new NAreaCodeClass(_dataPath);
            var number = areaCodeClass.GetNumberOfMunicipalities(dt);
            foreach(var num in number)
            {
                Console.WriteLine($"{num.種別}\t{num.計}");
            }
        }

        private static void Number(int pref, DateTime dt)
        {
            var areaCodeClass = new NAreaCodeClass(_dataPath);
            var number = areaCodeClass.GetNumberOfMunicipalities(pref, dt);
            foreach (var num in number)
            {
                Console.WriteLine($"{num.種別}\t{num.計}");
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
            var areaCodes = areaCodeClass.GetStandardAreaCode(0, new DateTime(1970, 4, 1), false);
            areaCodes.Sort((x, y) => x.Id - y.Id);
            var districtList = areaCodeClass.DistrictList;

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

                //名称のチェック
                //政令市
                if(areaCodes[n].Id/100%10 == 1 && areaCodes[n].名称.EndsWith("市"))
                {
                    if (areaCodes[n].名称 != mmmDataList[m].GName1)
                        Console.WriteLine($"名称相違 {areaCodes[n].Id} : NAreaCode {areaCodes[n].名称} | MMM {mmmDataList[m].GName1}");
                }
                else if(areaCodes[n].名称.EndsWith("市") || areaCodes[n].名称.EndsWith("区"))
                {
                    if(areaCodes[n].名称 != mmmDataList[m].CName1)
                        Console.WriteLine($"名称相違 {areaCodes[n].Id} : NAreaCode {areaCodes[n].名称} | MMM {mmmDataList[m].CName1}");
                }
                else
                {
                    if (areaCodes[n].名称 != mmmDataList[m].CName1)
                        Console.WriteLine($"名称相違 {areaCodes[n].Id} : NAreaCode {areaCodes[n].名称} | MMM {mmmDataList[m].CName1}");
                    if (areaCodes[n].所属 < 100)
                    {
                        var subpref = SubPrefecture.Hokkaido.FirstOrDefault(x => x.Id == areaCodes[n].所属);
                        if (subpref.支庁名 != mmmDataList[m].GName1)
                            Console.WriteLine($"郡名称相違 {areaCodes[n].Id} : NAreaCode {subpref.支庁名}{areaCodes[n].名称} | MMM {mmmDataList[m].GName1}{mmmDataList[m].CName1}");
                    }
                    else
                    {
                        var district = districtList.FirstOrDefault(x => x.Id == areaCodes[n].所属);
                        if (district == null)
                        {
                            Console.WriteLine($"郡名称相違 {areaCodes[n].Id} : NAreaCode 郡名なし {areaCodes[n].名称} | MMM {mmmDataList[m].GName1}{mmmDataList[m].CName1}");
                        }
                        else if (district.名称 != mmmDataList[m].GName1)
                            Console.WriteLine($"郡名称相違 {areaCodes[n].Id} : NAreaCode {district.名称}{areaCodes[n].名称} | MMM {mmmDataList[m].GName1}{mmmDataList[m].CName1}");
                    }
                }

                prev = mmmDataList[m].JisCode1;
                m++;
                n++;
            }
        }
    }
}
