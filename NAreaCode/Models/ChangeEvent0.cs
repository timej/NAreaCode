using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NAreaCode.Models
{
    //変更データ 
    class ChangeEvent0
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        //施行年月日
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        //変更理由
        // 政令指定都市施行 sacr:shiftToDesignatedCity
        // 境界変更sacr:changesOfBoundaries
        // 区の新設sacr:establishmentOfWard
        // 分離 sacr:separation
        // 編入合併sacr:absorption
        // 都市種別変更 sacr:shiftTtoAnotherKindOfCity
        // 新設合併sacr:establishmentOfNewMunicipalityByMmerging
        // 市制施行sacr:changesToCity
        // 名称変更sacr:changesOfNames
        // 町制施行sacr:changesToTown
        // 郡の区域変更 sacr:changesOfBoundariesOfDistrict
        // 分割 sacr:divisionIntoSeveralMunicipalities
        // 郡の新設sacr:establishmentOfNewDistrict
        // 郡の廃止sacr:abolishmentOfDistrict
        // その他sacr:others
        [JsonProperty("reasonForChange")]
        public string ReasonForChange { get; set; }
        //変更前の期間つき標準地域コード
        [JsonProperty("original")]
        public List<string> Original { get; set; }
        //変更後の期間つき標準地域コード
        [JsonProperty("resulting")]
        public List<string> Resulting { get; set; }
        //変更事由の詳細
        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
