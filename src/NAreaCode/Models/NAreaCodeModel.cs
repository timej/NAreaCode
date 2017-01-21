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
            var areaLods = AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.支庁 != 99).OrderBy(x => x.Id);

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

        //date日現在の市町村数
        public string GetNumber(DateTime date)
        {
            var areaLods = AreaCodeList.Where(x => x.施行年月日 <= date && x.廃止年月日 > date && x.支庁 != 99);

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
    }
}
