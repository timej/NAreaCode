using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAreaCode.Models
{
    class SubPrefecture
    {
        public int Id { get; set; }
        public int 地域コード { get; set; }
        public string 振興局名 { get; set; }
        public string 支庁名 { get; set; }
        public string 地方名 { get; set; }
        public int 四区分コード { get; set; }
        public string 四区分名 { get; set; }

        public static readonly List<SubPrefecture> Hokkaido = new List<SubPrefecture>
        {
            new SubPrefecture {
                Id = 1,
                地域コード = 1300,
                振興局名 = "石狩振興局",
                支庁名 = "石狩支庁",
                地方名 = "石狩",
                四区分コード = 1,
                四区分名 = "道央"
            },
            new SubPrefecture {
                Id = 2,
                地域コード = 1330,
                振興局名 = "渡島総合振興局",
                支庁名 = "渡島支庁",
                地方名 = "渡島",
                四区分コード = 2,
                四区分名 = "道南"
            },
            new SubPrefecture{
                Id = 3,
                地域コード = 1360,
                振興局名 = "檜山振興局",
                支庁名 = "檜山支庁",
                地方名 = "檜山",
                四区分コード = 2,
                四区分名 = "道南"
            },
            new SubPrefecture{
                Id = 4,
                地域コード = 1390,
                振興局名 = "後志総合振興局",
                支庁名 = "後志支庁",
                地方名 = "後志",
                四区分コード = 1,
                四区分名 = "道央"
            },
            new SubPrefecture{
                Id = 5,
                地域コード = 1420,
                振興局名 = "空知総合振興局",
                支庁名 = "空知支庁",
                地方名 = "空知",
                四区分コード = 1,
                四区分名 = "道央"
            },
            new SubPrefecture{
                Id = 6,
                地域コード = 1450,
                振興局名 = "上川総合振興局",
                支庁名 = "上川支庁",
                地方名 = "上川",
                四区分コード = 3,
                四区分名 = "道北"
            },
            new SubPrefecture{
                Id = 7,
                地域コード = 1480,
                振興局名 = "留萌振興局",
                支庁名 = "留萌支庁",
                地方名 = "留萌",
                四区分コード = 3,
                四区分名 = "道北"
            },
            new SubPrefecture{
                Id = 8,
                地域コード = 1510,
                振興局名 = "宗谷総合振興局",
                支庁名 = "宗谷支庁",
                地方名 = "宗谷",
                四区分コード = 3,
                四区分名 = "道北"
            },
            new SubPrefecture{
                Id = 9,
                地域コード = 1540,
                振興局名 = "オホーツク総合振興局",
                支庁名 = "網走支庁",
                地方名 = "オホーツク",
                四区分コード = 4,
                四区分名 = "道東"
            },
            new SubPrefecture{
                Id = 10,
                地域コード = 1570,
                振興局名 = "胆振総合振興局",
                支庁名 = "胆振支庁",
                地方名 = "胆振",
                四区分コード = 1,
                四区分名 = "道央"
            },
            new SubPrefecture{
                Id = 11,
                地域コード = 1600,
                振興局名 = "日高振興局",
                支庁名 = "日高支庁",
                地方名 = "日高",
                四区分コード = 1,
                四区分名 = "道央"
            },
            new SubPrefecture{
                Id = 12,
                地域コード = 1630,
                振興局名 = "十勝総合振興局",
                支庁名 = "十勝支庁",
                地方名 = "十勝",
                四区分コード = 4,
                四区分名 = "道東"
            },
            new SubPrefecture{
                Id = 13,
                地域コード = 1660,
                振興局名 = "釧路総合振興局",
                支庁名 = "釧路支庁",
                地方名 = "釧路",
                四区分コード = 4,
                四区分名 = "道東"
            },
            new SubPrefecture{
                Id = 14,
                地域コード = 1690,
                振興局名 = "根室振興局",
                支庁名 = "根室支庁",
                地方名 = "根室",
                四区分コード = 4,
                四区分名 = "道東"
            },
            new SubPrefecture {
                Id = 99,
                地域コード = 1999,
                振興局名 = "北方領土",
                支庁名 = "北方領土",
                地方名 = "北方領土",
                四区分コード = 5,
                四区分名 = "北方領土"
            },
        };
    }
}
