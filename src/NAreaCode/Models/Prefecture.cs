using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NAreaCode.Models
{
    public class Prefecture
    {
        public int Id { get; set; }
        public string 名称 { get; set; }
        public string ふりがな { get; set; }
        public string 英語名 { get; set; }

        public static readonly List<Prefecture> JpPrefectures = new List<Prefecture>
        {
            new Prefecture {Id = 1, 名称 = "北海道", ふりがな = "ほっかいどう", 英語名 = "Hokkaido"},
            new Prefecture {Id = 2, 名称 = "青森県", ふりがな = "あおもりけん", 英語名 = "Aomori-ken"},
            new Prefecture {Id = 3, 名称 = "岩手県", ふりがな = "いわてけん", 英語名 = "Iwate-ken"},
            new Prefecture {Id = 4, 名称 = "宮城県", ふりがな = "みやぎけん", 英語名 = "Miyagi-ken"},
            new Prefecture {Id = 5, 名称 = "秋田県", ふりがな = "あきたけん", 英語名 = "Akita-ken"},
            new Prefecture {Id = 6, 名称 = "山形県", ふりがな = "やまがたけん", 英語名 = "Yamagata-ken"},
            new Prefecture {Id = 7, 名称 = "福島県", ふりがな = "ふくしまけん", 英語名 = "Fukushima-ken"},
            new Prefecture {Id = 8, 名称 = "茨城県", ふりがな = "いばらきけん", 英語名 = "Ibaraki-ken"},
            new Prefecture {Id = 9, 名称 = "栃木県", ふりがな = "とちぎけん", 英語名 = "Tochigi-ken"},
            new Prefecture {Id = 10, 名称 = "群馬県", ふりがな = "ぐんまけん", 英語名 = "Gumma-ken"},
            new Prefecture {Id = 11, 名称 = "埼玉県", ふりがな = "さいたまけん", 英語名 = "Saitama-ken"},
            new Prefecture {Id = 12, 名称 = "千葉県", ふりがな = "ちばけん", 英語名 = "Chiba-ken"},
            new Prefecture {Id = 13, 名称 = "東京都", ふりがな = "とうきょうと", 英語名 = "Tokyo-to"},
            new Prefecture {Id = 14, 名称 = "神奈川県", ふりがな = "かながわけん", 英語名 = "Kanagawa-ken"},
            new Prefecture {Id = 15, 名称 = "新潟県", ふりがな = "にいがたけん", 英語名 = "Niigata-ken"},
            new Prefecture {Id = 16, 名称 = "富山県", ふりがな = "とやまけん", 英語名 = "Toyama-ken"},
            new Prefecture {Id = 17, 名称 = "石川県", ふりがな = "いしかわけん", 英語名 = "Ishikawa-ken"},
            new Prefecture {Id = 18, 名称 = "福井県", ふりがな = "ふくいけん", 英語名 = "Fukui-ken"},
            new Prefecture {Id = 19, 名称 = "山梨県", ふりがな = "やまなしけん", 英語名 = "Yamanashi-ken"},
            new Prefecture {Id = 20, 名称 = "長野県", ふりがな = "ながのけん", 英語名 = "Nagano-ken"},
            new Prefecture {Id = 21, 名称 = "岐阜県", ふりがな = "ぎふけん", 英語名 = "Gifu-ken"},
            new Prefecture {Id = 22, 名称 = "静岡県", ふりがな = "しずおかけん", 英語名 = "Shizuoka-ken"},
            new Prefecture {Id = 23, 名称 = "愛知県", ふりがな = "あいちけん", 英語名 = "Aichi-ken"},
            new Prefecture {Id = 24, 名称 = "三重県", ふりがな = "みえけん", 英語名 = "Mie-ken"},
            new Prefecture {Id = 25, 名称 = "滋賀県", ふりがな = "しがけん", 英語名 = "Shiga-ken"},
            new Prefecture {Id = 26, 名称 = "京都府", ふりがな = "きょうとふ", 英語名 = "Kyoto-fu"},
            new Prefecture {Id = 27, 名称 = "大阪府", ふりがな = "おおさかふ", 英語名 = "Osaka-fu"},
            new Prefecture {Id = 28, 名称 = "兵庫県", ふりがな = "ひょうごけん", 英語名 = "Hyogo-ken"},
            new Prefecture {Id = 29, 名称 = "奈良県", ふりがな = "ならけん", 英語名 = "Nara-ken"},
            new Prefecture {Id = 30, 名称 = "和歌山県", ふりがな = "わかやまけん", 英語名 = "Wakayama-ken"},
            new Prefecture {Id = 31, 名称 = "鳥取県", ふりがな = "とっとりけん", 英語名 = "Tottori-ken"},
            new Prefecture {Id = 32, 名称 = "島根県", ふりがな = "しまねけん", 英語名 = "Shimane-ken"},
            new Prefecture {Id = 33, 名称 = "岡山県", ふりがな = "おかやまけん", 英語名 = "Okayama-ken"},
            new Prefecture {Id = 34, 名称 = "広島県", ふりがな = "ひろしまけん", 英語名 = "Hiroshima-ken"},
            new Prefecture {Id = 35, 名称 = "山口県", ふりがな = "やまぐちけん", 英語名 = "Yamaguchi-ken"},
            new Prefecture {Id = 36, 名称 = "徳島県", ふりがな = "とくしまけん", 英語名 = "Tokushima-ken"},
            new Prefecture {Id = 37, 名称 = "香川県", ふりがな = "かがわけん", 英語名 = "Kagawa-ken"},
            new Prefecture {Id = 38, 名称 = "愛媛県", ふりがな = "えひめけん", 英語名 = "Ehime-ken"},
            new Prefecture {Id = 39, 名称 = "高知県", ふりがな = "こうちけん", 英語名 = "Kochi-ken"},
            new Prefecture {Id = 40, 名称 = "福岡県", ふりがな = "ふくおかけん", 英語名 = "Fukuoka-ken"},
            new Prefecture {Id = 41, 名称 = "佐賀県", ふりがな = "さがけん", 英語名 = "Saga-ken"},
            new Prefecture {Id = 42, 名称 = "長崎県", ふりがな = "ながさきけん", 英語名 = "Nagasaki-ken"},
            new Prefecture {Id = 43, 名称 = "熊本県", ふりがな = "くまもとけん", 英語名 = "Kumamoto-ken"},
            new Prefecture {Id = 44, 名称 = "大分県", ふりがな = "おおいたけん", 英語名 = "Oita-ken"},
            new Prefecture {Id = 45, 名称 = "宮崎県", ふりがな = "みやざきけん", 英語名 = "Miyazaki-ken"},
            new Prefecture {Id = 46, 名称 = "鹿児島県", ふりがな = "かごしまけん", 英語名 = "Kagoshima-ken"},
            new Prefecture {Id = 47, 名称 = "沖縄県", ふりがな = "おきなわけん", 英語名 = "Okinawa-ken"},
        };
    }
}
