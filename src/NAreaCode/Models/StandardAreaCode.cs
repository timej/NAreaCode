using System;
using System.Collections.Generic;

namespace NAreaCode.Models
{
    class StandardAreaCode
    {
        public int Id { get; set; }
        public string 名称 { get; set; }
        public string ふりがな { get; set; }
        public string 英語名 { get; set; }
        //北海道のみ
        public int 支庁 { get; set; }
        //市町村
        public string 郡 { get; set; }
        public DateTime 施行年月日 { get; set; }
        public DateTime 廃止年月日 { get; set; }
        public List<int> 施行データ { get; set; }
        public List<int> 廃止データ { get; set; }
    }
}
