using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NAreaCode.Models
{
    public enum 自治体種別
    {
        Prefecture,
        DesignatedCity,
        CoreCity,
        SpecialCity,
        City,
        Town,
        Village,
        SpecialWard,
        Ward
    }

    public class StandardAreaCode
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string 名称 { get; set; }
        public string ふりがな { get; set; }
        public string 英語名 { get; set; }
        public 自治体種別 種別 { get; set;}
        //北海道、東京都島嶼部、長崎県対馬（旧）は支庁・振興局、その他の町村は郡、区は政令指定都市のコード
        public int 所属 { get; set; }
        //北海道及び旧長崎県対馬支庁の町村のみ
        public string 郡名称 { get; set; }
        public string 郡ふりがな { get; set; }
        public DateTime 施行年月日 { get; set; }
        public DateTime 廃止年月日 { get; set; }
        public List<int> 施行データ { get; set; }
        public List<int> 廃止データ { get; set; }
    }
}
