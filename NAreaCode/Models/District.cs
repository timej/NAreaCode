using Newtonsoft.Json;

namespace NAreaCode.Models
{
    //郡コード
    //郡での集計はしない
    //郡の名称の変更は"囎唹郡(46460)が曽於郡に名称変更"しかないようなので、
    //5桁のコードで対応。
    class District
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        public string 名称 { get; set; }
        public string ふりがな { get; set; }
    }
}
