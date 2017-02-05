using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NAreaCode.Models
{
    class StandardAreaCode
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string 名称 { get; set; }
        public string ふりがな { get; set; }
        public string 英語名 { get; set; }
        public int 郡支庁 { get; set; }
        //北海道及び長崎県対馬の市町村のみ
        public string 郡名称 { get; set; }
        public string 郡ふりがな { get; set; }
        public DateTime 施行年月日 { get; set; }
        public DateTime 廃止年月日 { get; set; }
        public List<int> 施行データ { get; set; }
        public List<int> 廃止データ { get; set; }
    }
}
