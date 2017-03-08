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

        public IEnumerable<(int id, string 名称)> GetAreaCode(DateTime date) => AreaCodeList
                .Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.所属 != 99)
                .OrderBy(x => x.Id)
                .Select(x => (id: x.Id, 名称: x.名称));

        public IEnumerable<(int id, string 名称)> GetAreaCode(int pref, DateTime date) => AreaCodeList
            .Where(x => x.Id / 1000 == pref && x.施行年月日 <= date && x.廃止年月日 > date && x.所属 != 99)
            .Select(x => (id: x.Id, 名称: x.名称))
            .OrderBy(x => x.id);

        public List<StandardAreaCode> GetStandardAreaCode(int pref, DateTime date, bool includeWard)
        {
            if(pref == 0 && includeWard)
                return AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date).OrderBy(x => x.Id).ToList();
            if(pref > 0 && pref < 48 && includeWard)
                return AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.Id / 1000 == pref).OrderBy(x => x.Id).ToList();
            if (pref == 0)
                return AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.種別 != 自治体種別.Ward).OrderBy(x => x.Id).ToList();
            if (pref > 0 && pref < 48)
                return AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.Id / 1000 == pref && x.種別 != 自治体種別.Ward).OrderBy(x => x.Id).ToList();
            return null;
        }

        //date日現在の市町村数
        public IEnumerable<(自治体種別 種別, int 計)> GetNumberOfMunicipalities(DateTime date) =>
            AreaCodeList
                .Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.所属 != 99)
                .GroupBy(x => x.種別)
                .Select(x => (種別: x.Key, 計: x.Count()))
                .OrderBy(x => x.種別);

        //date日現在の市町村数
        public IEnumerable<(自治体種別 種別, int 計)> GetNumberOfMunicipalities(int pref, DateTime date) =>
            AreaCodeList
                .Where(x => x.Id / 1000 == pref && x.施行年月日 <= date && x.廃止年月日 > date && x.所属 != 99)
                .GroupBy(x => x.種別)
                .Select(x => (種別: x.Key, 計: x.Count()))
                .OrderBy(x => x.種別);

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
