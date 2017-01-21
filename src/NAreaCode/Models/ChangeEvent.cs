using System;
using System.Collections.Generic;

namespace NAreaCode.Models
{
    enum 変更事由 { その他, 新設合併, 編入合併, 政令指定都市施行, 市制施行, 郡の区域変更, 町制施行, 名称変更, 分離, 分割, 区の新設, 市制施行名称変更 };
    class ChangeEvent
    {
        //変更データID
        public int Id { get; set; }
        public DateTime 施行年月日 { get; set; }
        public 変更事由 変更事由 { get; set; }
        public List<int> 変更前地域 { get; set; }
        public List<int> 変更後地域 { get; set; }
        public string 変更事由の詳細 { get; set; }

    }
}
