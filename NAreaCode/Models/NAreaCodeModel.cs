using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NAreaCode.Models
{
    class NAreaCodeClass
    {
        public List<StandardAreaCode> AreaCodeList;
        public List<ChangeEvent> ChangeEventList { get; set; }
        public List<District> DistrictList { get; set; }

        public DateTime StartDate { get; } = new DateTime(1970, 4, 1);


        public NAreaCodeClass(string path)
        {
            string datapath = Path.Combine(path, "StandardAreaCodeList.json");
            if (File.Exists(datapath))
                AreaCodeList = JsonUtils.LoadFromJson<List<StandardAreaCode>>(datapath);
            else
            {
                Console.WriteLine("地域コードのデータがありません。");
                return;
            }

            datapath = Path.Combine(path, "ChangeEventList.json");

            if (File.Exists(datapath))
                ChangeEventList = JsonUtils.LoadFromJson<List<ChangeEvent>>(datapath);
            else
            {
                Console.WriteLine("変更事由のデータがありません。");
                return;
            }

            datapath = Path.Combine(path, "District.json");
            if (File.Exists(datapath))
            {
                DistrictList = JsonUtils.LoadFromJson<List<District>>(datapath);
            }
            else
            {
                Console.WriteLine("郡のデータがありません。");
                return;
            }
        }

        public List<AreaCode> GetAreaCode(int pref, DateTime date)
        {
            var areaCodeList = new List<AreaCode>();
            var areaLods = AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.郡支庁 != 99).OrderBy(x => x.Id);

            foreach (var area in areaLods)
            {
                areaCodeList.Add(new AreaCode
                {
                    Id = area.Id,
                    名称 = area.名称
                });
            }
            return areaCodeList;
        }

        public List<StandardAreaCode> GetStandardAreaCode(int pref, DateTime date)
        {
            if(pref == 0)
            return AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date).OrderBy(x => x.Id).ToList();
            if(pref > 0 && pref < 48)
                return AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.Id / 1000 == pref).OrderBy(x => x.Id).ToList();
            return null;
        }

        //date日現在の市町村数
        public string GetNumber(DateTime date)
        {
            var areaLods = AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.郡支庁 != 99);

            int city = 0;
            int town = 0;
            int villege = 0;

            foreach (var area in areaLods)
            {
                if (area.名称.EndsWith("市"))
                    city++;
                if (area.名称.EndsWith("町"))
                    town++;
                if (area.名称.EndsWith("村"))
                    villege++;
            }
            return $"市 {city}、町 {town}、村 {villege}";
        }

        //市町村コードの対応を計算するプログラム
        //areaCode: 市町村コード 
        //date1: 変換前の日付
        //date2: 変換後の日付
        public List<int> GetNewCode(int areaCode, DateTime date1, DateTime date2)
        {
            var areaData = AreaCodeList.FirstOrDefault(x => x.Id == areaCode && x.施行年月日 <= date1 && date1 < x.廃止年月日);
            if (date2 < areaData.廃止年月日)
                return new List<int> { areaCode };

            var results = new List<int>();
            foreach (var suc in areaData.廃止データ)
            {
                var ev = ChangeEventList.First(x => x.Id == suc);
                foreach (var area in ev.変更後地域)
                {
                    results.AddRange(GetNewCode(area, ev.施行年月日, date2));
                }
            }
            return results;
        }
    }
}
