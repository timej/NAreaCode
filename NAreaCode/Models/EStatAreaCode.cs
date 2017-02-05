using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using VDS.RDF;
using VDS.RDF.Query;

namespace NAreaCode.Models
{
    //統計LOD（https://data.e-stat.go.jp/lodw/）の「統計に用いる標準地域コード」からデータを取得
    //統計LOD 標準地域コードの解説
    //http://data.e-stat.go.jp/lodw/rdfschema/sacs/

    class EStatAreaCode
    {
        private List<StandardAreaCode> StandardAreaCodeList;
        private List<StandardAreaCode> StandardAreaLodList0;
        private List<ChangeEvent> ChangeEventList;
        private List<District> DistrictList;

        private Dictionary<int, string> AreaCodeDic;
        private List<ChangeEvent0> ChangeEventList0;
        private List<CurrentArea> CurrentAreaList;

        static readonly SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://data.e-stat.go.jp/lod/sparql/query?"));

        const string Prefix = @"
            PREFIX rdf:<http://www.w3.org/1999/02/22-rdf-syntax-ns#>
            PREFIX rdfs:<http://www.w3.org/2000/01/rdf-schema#>
            PREFIX org:<http://www.w3.org/ns/org#>
            PREFIX dcterms:<http://purl.org/dc/terms/>
            PREFIX sacs:<http://data.e-stat.go.jp/lod/terms/sacs#>
            PREFIX sac:<http://data.e-stat.go.jp/lod/sac/>
            PREFIX sace:<http://data.e-stat.go.jp/lod/sace/>
            PREFIX sacr:<http://data.e-stat.go.jp/lod/sacr/>
            PREFIX owl:<http://www.w3.org/2002/07/owl#>
        ";

        const string dcterms = "http://purl.org/dc/terms/";
        const string org = "http://www.w3.org/ns/org#";
        const string sac = "http://data.e-stat.go.jp/lod/sac/C";
        const string sacs = "http://data.e-stat.go.jp/lod/terms/sacs#";
        const string rdfs = "http://www.w3.org/2000/01/rdf-schema#";

        private readonly DateTime _startDate = new DateTime(1970, 4, 1);
        private readonly string _path;
        private readonly bool _isUpdate;


        public EStatAreaCode(bool isUpdate, string path)
        {
            _path = path;
            _isUpdate = isUpdate;

            GetChangeEventList();
            GetCurrentAreaCode();
            GetDistrict();
            GetStandardAreaCode();
        }


        #region StandardAreaCode
        //標準地域コードの取得

        private void GetStandardAreaCode()
        {
            string path = Path.Combine(_path, "StandardAreaCodeList.json");

            if (_isUpdate && File.Exists(path))
                StandardAreaLodList0 = JsonUtils.LoadFromJson<List<StandardAreaCode>>(path);

            StandardAreaCodeList = new List<StandardAreaCode>();
            foreach (var area in CurrentAreaList)
            {
                if (area.AdministrativeClass == "DesignatedCity" || area.AdministrativeClass == "CoreCity" ||
                    area.AdministrativeClass == "SpecialCity" || area.AdministrativeClass == "City" ||
                    area.AdministrativeClass == "SpecialWard" || area.AdministrativeClass == "Town" ||
                    area.AdministrativeClass == "Village")
                {
                    StandardAreaCode areaCode = new StandardAreaCode
                    {
                        Id = area.Id,
                        廃止年月日 = DateTime.MaxValue,
                        施行データ = new List<int>(),
                        廃止データ = new List<int>()
                    };
                    ExecuteLod(area.Id, areaCode, true);
                }
            }
            JsonUtils.SaveToJson(path, StandardAreaCodeList);
                string districtPath = Path.Combine(_path, "District.json");
                JsonUtils.SaveToJson(districtPath, DistrictList);
        }

        private void ExecuteLod(int code, StandardAreaCode areaCode, bool isExistData)
        {
            var privAreas = new List<int>();
            var areaLodList = MakeAreaLod(code, areaCode, isExistData, false, privAreas);

            StandardAreaCodeList.AddRange(areaLodList);

            foreach (var privArea in privAreas)
            {
                if (privArea == code)
                    continue;
                //47361と47362のコードは再利用している
                if ((code == 47371 && privArea == 47361) || (code == 47372 && privArea == 47362))
                    continue;
                var lod = StandardAreaCodeList.FirstOrDefault(x => x.Id == privArea);
                if (lod == null)
                    ExecuteLod(privArea, null, false);
            }
        }

        private List<StandardAreaCode> MakeAreaLod(int code, StandardAreaCode areaCode, bool isExistData, bool isCheck, List<int> privAreas)
        {
            var areaLodList = new List<StandardAreaCode>();
            var events = ChangeEventList.Where(x => x.変更前地域.Contains(code) || x.変更後地域.Contains(code)).ToList();
            events.Sort((x, y) =>
            {
                int c = DateTime.Compare(y.施行年月日, x.施行年月日);
                if (c == 0)
                    return y.Id - x.Id;
                return c;
            });

            DateTime date = DateTime.MaxValue;

            foreach (var v in events)
            {
                if(v.変更事由 == 変更事由.編入合併)
                {
                    if(code == v.変更前地域[0])
                        privAreas.AddRange(v.変更前地域);
                }
                else
                    privAreas.AddRange(v.変更前地域);
                if (v.変更後地域.Contains(code))
                {
                    if (isExistData)
                    {
                        if (date == v.施行年月日)
                        {
                            var lod = areaLodList.FirstOrDefault(x => x.施行年月日 == date);
                            if (lod == null)
                            {

                                //同日で以下のような変更がある場合の対応として
                                //"大野村(08404)が鹿島町(08405)に編入"
                                //"鹿島町(08405)が鹿嶋町に名称変更し、鹿嶋市(08222)に市制施行"
                                areaCode.施行年月日 = v.施行年月日;
                                areaCode.施行データ.Add(v.Id);
                                if (!isCheck) GetAreaData(areaCode, v.施行年月日);
                                areaLodList.Add(areaCode);

                                areaCode = new StandardAreaCode
                                {
                                    Id = code,
                                    施行年月日 = _startDate,
                                    廃止年月日 = v.施行年月日,
                                    施行データ = new List<int>(),
                                    廃止データ = new List<int>()
                                };
                            }
                            else
                            {
                                lod.施行データ.Add(v.Id);
                                lod.施行データ.Sort();
                            }
                        }
                        else
                        {
                            areaCode.施行年月日 = v.施行年月日;
                            areaCode.施行データ.Add(v.Id);
                            if (!isCheck) GetAreaData(areaCode, v.施行年月日);
                            areaLodList.Add(areaCode);
                            isExistData = false;
                        }
                    }
                    else
                    {
                        if (date == v.施行年月日)
                        {
                            var lod = areaLodList.FirstOrDefault(x => x.施行年月日 == date);
                            if (lod != null)
                            {
                                lod.施行データ.Add(v.Id);
                                lod.施行データ.Sort();
                            }
                            else
                                Console.WriteLine("引継データがない");
                        }
                        else
                            Console.WriteLine("引継データがない");
                    }
                }
                else
                {
                    if (isExistData)
                    {
                        if (date != v.施行年月日)
                            Console.WriteLine("元データがない");
                    }
                }
                if (v.変更前地域.Contains(code))
                {
                    if (date == v.施行年月日)
                    {
                        if (isExistData)
                        {
                            areaCode.廃止データ.Add(v.Id);
                            areaCode.廃止データ.Sort();
                        }
                        else
                        {
                            areaCode = new StandardAreaCode
                            {
                                Id = code,
                                施行年月日 = _startDate,
                                廃止年月日 = v.施行年月日,
                                施行データ = new List<int>(),
                                廃止データ = new List<int>()
                            };
                            areaCode.廃止データ.Add(v.Id);
                            isExistData = true;
                        }
                    }
                    else
                    {
                        areaCode = new StandardAreaCode
                        {
                            Id = code,
                            施行年月日 = _startDate,
                            廃止年月日 = v.施行年月日,
                            施行データ = new List<int>(),
                            廃止データ = new List<int>()
                        };
                        areaCode.廃止データ.Add(v.Id);
                        isExistData = true;
                    }
                }
                else
                {
                    isExistData = false;
                }
                date = v.施行年月日;
            }
            if (isExistData)
            {
                areaCode.施行年月日 = _startDate;
                if (!isCheck) GetAreaData(areaCode, _startDate);
                areaLodList.Add(areaCode);
            }
            return areaLodList;
        }

        private void GetAreaData(StandardAreaCode areaCode, DateTime issued)
        {
            string durationAreaCode;
            if (areaCode.Id / 1000 == 47 && issued < new DateTime(1972, 5, 15))
            {
                if (areaCode.Id == 47307)
                {
                    areaCode.名称 = "上本部村";
                    areaCode.ふりがな = "かみもとぶそん";
                    areaCode.英語名 = "Kamimotobu-son";
                    areaCode.郡支庁 = 47300;
                    return;
                }
                else if (areaCode.Id == 47342)
                {
                    areaCode.名称 = "糸満町";
                    areaCode.ふりがな = "いとまんちょう";
                    areaCode.英語名 = "Itoman-cho";
                    areaCode.郡支庁 = 47340;
                    return;
                }
                else
                {
                    durationAreaCode = areaCode.Id + "-19720515";
                }
            }
            else
                durationAreaCode = areaCode.Id + "-" + issued.ToString("yyyyMMdd");
            //政令市の区に編入しているケース
            if (durationAreaCode == "26100-20050401") //771京北町
                durationAreaCode = "26100-19700401";
            else if (durationAreaCode == "34100-20050425") //801湯来町
                durationAreaCode = "34100-19850320";
            else if (durationAreaCode == "22100-20060331") //1141蒲原町
                durationAreaCode = "22100-20050401";
            else if (durationAreaCode == "22100-20081101") //5093由比町
                durationAreaCode = "22100-20050401";
            else if (durationAreaCode == "40130-19750301") //218早良町
                durationAreaCode = "40130-19720401";

            var areaCode0 = StandardAreaLodList0?.FirstOrDefault(x => x.Id == areaCode.Id && x.施行年月日 == issued);
            if (areaCode0 != null)
            {
                areaCode.名称 = areaCode0.名称;
                areaCode.ふりがな = areaCode0.ふりがな;
                areaCode.英語名 = areaCode0.英語名;
                areaCode.郡支庁 = areaCode0.郡支庁;
                areaCode.郡名称 = areaCode0.郡名称;
                areaCode.郡ふりがな = areaCode0.郡ふりがな;
                return;
            }

            string query = Prefix + $"SELECT * WHERE {{sac:C{durationAreaCode} ?p ?o. FILTER(?p = rdfs:label || ?p = dcterms:isPartOf || ?p = sacs:districtOfSubPrefecture)}}";
            var results = endpoint.QueryWithResultSet(query);
            foreach (var result in results)
            {
                string p = result["p"].ToString();
                if (p == rdfs + "label")
                {
                    string o = result["o"].ToString();
                    if (o.EndsWith("@ja"))
                        areaCode.名称 = o.Substring(0, o.Length - 3);
                    if (o.EndsWith("@ja-hrkt"))
                        areaCode.ふりがな = o.Substring(0, o.Length - 8);
                    if (o.EndsWith("@en"))
                        areaCode.英語名 = o.Substring(0, o.Length - 3);
                }
                else if (p == dcterms + "isPartOf")
                {
                    string part = result["o"].ToString().Substring(sac.Length);
                    //都道府県はパス
                    if (part[2] == '0')
                        continue;
                    int shortpart = int.Parse(part.Substring(0, 5));
                    //北海道
                    if (areaCode.Id / 1000 == 1)
                    {
                        //北方領土を他と区分するため、支庁コードを99にした
                        areaCode.郡支庁 = areaCode.Id > 1694 ? 99 : SubPrefecture.Hokkaido.First(x => x.地域コード == shortpart).Id;
                    }
                    else
                    {
                        var district = DistrictList.FirstOrDefault(x => x.Id == shortpart);
                        if(district == null)
                        {
                            DistrictList.Add(GetDistrictFromLod(part));
                        }
                        areaCode.郡支庁 = shortpart;
                    }
                }
                //北海道・対馬のみ
                else if (p == sacs + "districtOfSubPrefecture")
                {
                    string o = result["o"].ToString();
                    if (o.EndsWith("@ja"))
                        areaCode.郡名称 = o.Substring(0, o.Length - 3);
                    else if (o.EndsWith("@ja-hrkt"))
                        areaCode.郡ふりがな = o.Substring(0, o.Length - 8);
                }
            }
        }
        #endregion

        #region District

        //郡データの作成
        private void GetDistrict()
        {

            string path = Path.Combine(_path, "District.json");
            if (_isUpdate && File.Exists(path))
            {
                DistrictList = JsonUtils.LoadFromJson<List<District>>(path);
                return;
            }

            path = Path.Combine(_path, "District0.json");
            if (_isUpdate && File.Exists(path))
            {
                DistrictList = JsonUtils.LoadFromJson<List<District>>(path);
                return;
            }

            //郡コード表の初期作成（現時点で存在している郡）
            DistrictList = new List<District>();
            foreach (var area in CurrentAreaList)
            {
                if (area.AdministrativeClass == "District" || area.AdministrativeClass == "SubPrefecture")
                {
                    var district = GetDistrictFromLod(area.CurrentAreaCode);
                    //北海道
                    if (area.Id / 1000 == 1)
                    {
                        var subPrefecture = SubPrefecture.Hokkaido.FirstOrDefault(x => x.地域コード == area.Id);
                        if (subPrefecture == null)
                            Console.WriteLine("支庁コードが間違っている");
                        else
                        {
                            if (subPrefecture.振興局名 != district.名称)
                                Console.WriteLine("支庁名が間違っている");
                        }
                    }
                    else
                    {
                        DistrictList.Add(district);
                    }
                }
            }
            JsonUtils.SaveToJson(path, DistrictList);           
        }

        //統計LODから郡名を取得
        private District GetDistrictFromLod(string areaCode)
        {
            var district = new District
            {
                Id = int.Parse(areaCode.Substring(0,5))
            };

            string query = Prefix + $"SELECT ?o WHERE {{ sac:C{areaCode} rdfs:label ?o . }}";
            var results = endpoint.QueryWithResultSet(query);
            foreach(var result in results)
            {
                string s = result["o"].ToString();
                if(s.EndsWith("@ja"))
                    district.名称 = s.Substring(0, s.Length - 3);
                else if(s.EndsWith("@ja-hrkt"))
                    district.ふりがな = s.Substring(0, s.Length - 8);
            }

            return district;
        }
        #endregion

        #region CurrentAreaCode

        //現行地域コードの取得
        // SELECT ?s ?o WHERE
        // {?s owl:sameAs ?o.?s rdf:type sacs:CurrentStandardAreaCode.}
        private void GetCurrentAreaCode()
        {
            //string path = Path.Combine(_path, "CurrentArea.json");

            CurrentAreaList = new List<CurrentArea>();
            string query = Prefix + "SELECT ?s ?o ?ad WHERE {?s owl:sameAs ?o.?s rdf:type sacs:CurrentStandardAreaCode.{?o sacs:administrativeClass ?ad.}}";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            foreach (var area in results)
            {
                CurrentAreaList.Add(new CurrentArea
                {
                    Id = int.Parse(area["s"].ToString().Substring(sac.Length)),
                    CurrentAreaCode = area["o"].ToString().Substring(sac.Length),
                    AdministrativeClass = area["ad"].ToString().Substring(sacs.Length)
                });
            }
            CurrentAreaList.Sort((x, y) => x.Id - y.Id);
            //JsonUtils.SaveToJson(path, CurrentAreaList);
            
        }
        #endregion

        #region ChangeEvent
        //変更事由データの取得


        private void GetChangeEventList()
        {
            GetAllChangeEvent();
            GetChangeEventList0();
            ModifyChangeEventList();
        }

        //すべての変更データのIDと変更事由を取得
        //SELECT ?s ?o WHERE {?s sacs:reasonForChange ?o . }

        private void GetAllChangeEvent()
        {
            string path = Path.Combine(_path, "AllChangeEvent.json");

            AreaCodeDic = new Dictionary<int, string>();
            string query = Prefix + "SELECT ?s ?o WHERE {?s sacs:reasonForChange ?o . }";

            SparqlResultSet results = endpoint.QueryWithResultSet(query);

            foreach (var result in results)
            {
                AreaCodeDic.Add(int.Parse(result["s"].ToString().Substring("http://data.e-stat.go.jp/lod/sacs/C".Length)), result["o"].ToString().Substring("http://data.e-stat.go.jp/lod/sacr/".Length));
            }
            JsonUtils.SaveToJson(path, AreaCodeDic);
        }

        // SELECT ?p ?o WHERE { sace:C1154 ?p ?o .}
        //|rdf:type|sacs:CodeChangeEvent
        //|org:resultingOrganization | sac:C27140-20060401
        //|org:resultingOrganization | sac:C27201-20060401
        //dcterms:description | "堺市(27201)の堺市(27140)への政令指定都市施行"@ja
        //dcterms:description| "Sakai-shi(27201) shifts to a city designated by the Cabinet Order, and it becomes Sakai-shi(27140)."@en |
        //org:originalOrganization  | sac:C27201-20050201
        //dcterms:identifier        | "1154"
        //sacs:reasonForChange      | sacr:shiftToDesignatedCity
        //dcterms:date              | "2006-04-01"^^<http://www.w3.org/2001/XMLSchema#date>

        private void GetChangeEventList0()
        {
            string path = Path.Combine(_path, "ChangeEventList0.json");

            if (_isUpdate && File.Exists(path))
            {
                ChangeEventList0 = JsonUtils.LoadFromJson<List<ChangeEvent0>>(path);
                foreach (var change in AreaCodeDic)
                {
                    if (change.Value != "changesOfBoundaries")
                    {
                        if(ChangeEventList0.All(x => x.Id != change.Key))
                        {
                            ChangeEvent0 ev = new ChangeEvent0
                            {
                                Id = change.Key,
                                ReasonForChange = change.Value,
                                Original = new List<string>(),
                                Resulting = new List<string>()
                            };
                            GetChangeEvent0(ev);
                            ChangeEventList0.Add(ev);
                        }
                    }
                }
            }
            else
            {
                ChangeEventList0 = new List<ChangeEvent0>();
                foreach (var change in AreaCodeDic)
                {
                    if (change.Value != "changesOfBoundaries")
                    {
                        ChangeEvent0 ev = new ChangeEvent0
                        {
                            Id = change.Key,
                            ReasonForChange = change.Value,
                            Original = new List<string>(),
                            Resulting = new List<string>()
                        };
                        GetChangeEvent0(ev);
                        ChangeEventList0.Add(ev);
                    }
                }
            }
            ChangeEventList0.Sort((x, y) => x.Id - y.Id);
            JsonUtils.SaveToJson(path, ChangeEventList0);
        }

        //変更事由データの取得
        private void GetChangeEvent0(ChangeEvent0 ev)
        {
            string query = Prefix + "SELECT ?p ?o WHERE { sace:C" + ev.Id + " ?p ?o.}";
            SparqlResultSet results = endpoint.QueryWithResultSet(query);
            foreach(var result in results)
            {
                var p = result["p"].ToString();
                var o = result["o"].ToString();
                if (p.StartsWith(dcterms + "date"))
                    ev.Date = DateTime.Parse(o.Substring(0, o.IndexOf('^')));
                else if (p.StartsWith(org + "resultingOrganization"))
                    ev.Resulting.Add(o.Substring(sac.Length));
                else if (p.StartsWith(org + "originalOrganization"))
                    ev.Original.Add(o.Substring(sac.Length));
                else if (p.StartsWith(dcterms + "description"))
                {
                    if (o.EndsWith("@ja"))
                        ev.Description = o.Substring(0, o.Length - 3);
                }
            }
        }

        private void ModifyChangeEventList()
        {
            ChangeEventList = new List<ChangeEvent>();

            //データのバグ修正
            var event1 = ChangeEventList0.First(x => x.Id == 394);
            event1.Original = new List<string> {"47352-20001222", "47351-20001222" };

            var event2 = ChangeEventList0.First(x => x.Id == 992);
            event2.Original = new List<string> {"47344-19900301", "47343-19970401" };


            foreach (var ev0 in ChangeEventList0)
            {
                var ev = new ChangeEvent
                {
                    Id = ev0.Id,
                    施行年月日 = ev0.Date,
                    変更事由の詳細 = ev0.Description
                };

                switch (ev0.ReasonForChange)
                {
                    case "establishmentOfNewMunicipalityByMmerging":
                        ev.変更事由 = 変更事由.新設合併;
                        ModifyOfEstablishment(ev, ev0);
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ChangeEventList.Add(ev);
                        break;
                    case "absorption":
                        ev.変更事由 = 変更事由.編入合併;
                        ModifyOfAbsorption(ev, ev0);
                        ChangeEventList.Add(ev);
                        break;
                    case "shiftToDesignatedCity":
                        ev.変更事由 = 変更事由.政令指定都市施行;
                        GetResultingCode(ev, ev0, new string[] { ")への政令指定都市施行", ")への政令指定都市移行" });
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ChangeEventList.Add(ev);
                        break;
                    case "changesToCity":
                        ev.変更事由 = 変更事由.市制施行;
                        GetResultingCode(ev, ev0, new string[] { ")に市制施行" });
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ChangeEventList.Add(ev);
                        break;
                    case "changesOfBoundariesOfDistrict":
                        //次のレコードの削除して、2つのレコードにする
                        //鳳至郡穴水町(17421)、門前町(17422)が鳳珠郡穴水町(17461)、門前町(17462)に郡の区域変更
                        if (ev0.Id == 634)
                        {
                            ChangeEventList.Add(new ChangeEvent
                            {
                                Id = 99999991,
                                施行年月日 = new DateTime(2005, 3, 1),
                                変更事由 = 変更事由.郡の区域変更,
                                変更前地域 = new List<int> { 17421 },
                                変更後地域 = new List<int> { 17461 },
                                変更事由の詳細 = "鳳至郡穴水町(17421)が鳳珠郡穴水町(17461)に郡の区域変更"
                            });
                            ChangeEventList.Add(new ChangeEvent
                            {
                                Id = 99999992,
                                施行年月日 = new DateTime(2005, 3, 1),
                                変更事由 = 変更事由.郡の区域変更,
                                変更前地域 = new List<int> { 17422 },
                                変更後地域 = new List<int> { 17462 },
                                変更事由の詳細 = "鳳至郡門前町(17422)が鳳珠郡門前町(17462)に郡の区域変更"
                            });
                        }
                        else
                        {
                            ev.変更事由 = 変更事由.郡の区域変更;
                            GetResultingCode(ev, ev0, new string[] { ")に郡の区域変更", ")に区域変更" });
                            ev.変更前地域 = ToStandardCode(ev0.Original);
                            ChangeEventList.Add(ev);
                        }
                        break;
                    case "changesOfNames":
                        //変更理由が「名称変更」になっているが、「市制施行」して「名称変更」しているものがある
                        if (ev0.Resulting.Count > 1)
                        {
                            if (ev0.ReasonForChange == "changesOfNames" && ev0.Description.Contains("市制施行"))
                            {
                                ev.変更事由 = 変更事由.市制施行名称変更;
                                GetResultingCode(ev, ev0, new string[] { ")に名称変更" });
                                ev.変更前地域 = ToStandardCode(ev0.Original);
                            }
                            else
                                Console.WriteLine(ev0.Description);
                        }
                        else
                        {
                            ev.変更事由 = 変更事由.名称変更;
                            ev.変更前地域 = ToStandardCode(ev0.Original);
                            ev.変更後地域 = ToStandardCode(ev0.Resulting);
                        }
                        ChangeEventList.Add(ev);
                        break;
                    case "changesToTown":
                        ev.変更事由 = 変更事由.町制施行;
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ev.変更後地域 = ToStandardCode(ev0.Resulting);
                        ChangeEventList.Add(ev);
                        break;

                    case "establishmentOfWards":
                        ev.変更事由 = 変更事由.区の新設;
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ev.変更後地域 = ToStandardCode(ev0.Resulting);
                        ChangeEventList.Add(ev);
                        break;
                    case "separation":
                        ev.変更事由 = 変更事由.分離;
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ev.変更後地域 = ToStandardCode(ev0.Resulting);
                        ChangeEventList.Add(ev);
                        break;
                    case "divisionIntoSeveralMunicipalities":
                        ev.変更事由 = 変更事由.分割;
                        ev.変更前地域 = ToStandardCode(ev0.Original);
                        ev.変更後地域 = ToStandardCode(ev0.Resulting);
                        ChangeEventList.Add(ev);
                        break;
                    case "others":
                        ev.変更事由 = 変更事由.その他;
                        if(ev.Id >= 2 && ev.Id <= 9)
                        {
                            ev.変更前地域 = new List<int> {
                                int.Parse(ev.変更事由の詳細.Substring(ev.変更事由の詳細.IndexOf('(')+1,5))};
                        }
                        else
                            ev.変更前地域 = ToStandardCode(ev0.Original);
                        ev.変更後地域 = ToStandardCode(ev0.Resulting);
                        //沖縄県を1970年4月1日からにするため復帰のデータを削除
                        if (ev.Id != 1)
                            ChangeEventList.Add(ev);
                        break;

                    case "shiftToAnotherKindOfCity":
                    case "establishmentOfNewDistrict":
                    case "abolishmentOfDistrict":
                        break;
                    default:
                        Console.WriteLine("登録されていない変更事由。詳細: " + ev0.Description);
                        break;
                }
            }

            ChangeEventList.Add(new ChangeEvent
            {
                Id = 99999993,
                施行年月日 = new DateTime(1971, 11, 1),
                変更事由 = 変更事由.編入合併,
                変更前地域 = new List<int> { 47308, 47307 },
                変更後地域 = new List<int> { 47308 },
                変更事由の詳細 = "上本部村(47307)が本部町(47308)に編入"
            });
            ChangeEventList.Add(new ChangeEvent
            {
                Id = 99999994,
                施行年月日 = new DateTime(1971, 12, 1),
                変更事由 = 変更事由.市制施行,
                変更前地域 = new List<int> { 47342 },
                変更後地域 = new List<int> { 47210 },
                変更事由の詳細 = "糸満町(47342)が糸満市(47210)に市制施行"
            });

            string path = Path.Combine(_path, "ChangeEventList.json");
            JsonUtils.SaveToJson(path, ChangeEventList);
         
        }

        // 編入合併
        private void ModifyOfAbsorption(ChangeEvent ev, ChangeEvent0 ev0)
        {
            //政令指定市の区への編入が4件あるので例外処理
            if(ev0.Id == 771) //京北町
            {
                ev.変更前地域 = new List<int> { 26100, 26381};
                ev.変更後地域 = new List<int> { 26100 };
            }
            else if (ev0.Id == 801) //湯来町
            {
                ev.変更前地域 = new List<int> { 34100, 34324 };
                ev.変更後地域 = new List<int> { 34100 };
            }
            else if (ev0.Id == 1141) //蒲原町
            {
                ev.変更前地域 = new List<int> { 22100, 22382 };
                ev.変更後地域 = new List<int> { 22100 };
            }
            else if (ev0.Id == 5093) //由比町
            {
                ev.変更前地域 = new List<int> { 22100, 22383 };
                ev.変更後地域 = new List<int> { 22100 };
            }
            else if (ev0.Id == 218) //早良町→福岡市
            {
                ev.変更前地域 = new List<int> { 40100, 40321 };
                ev.変更後地域 = new List<int> { 40100 };
            }
            else if (ev0.Description.EndsWith(")に編入"))
            {
                int code = int.Parse(ev0.Description.Substring(ev0.Description.Length - 9, 5));
                ev.変更後地域 = new List<int> { code };

                //変更前の地域コードリストで、最初に編入先の市町村になるようにする
                int originArea = 0;
                ev.変更前地域 = new List<int>();
                foreach (var e in ev0.Original)
                {
                    int area = int.Parse(e.Substring(0, 5));
                    if (area == code)
                        originArea = area;
                    else
                        ev.変更前地域.Add(area);
                }
                ev.変更前地域.Sort();
                if (originArea != 0)
                    ev.変更前地域.Insert(0, originArea);
                else
                    Console.WriteLine("編入の変更前の期間つき標準地域コードの処理に問題がある。");
            }
            else
                Console.WriteLine("編入の詳細が異なる。");
        }

        //新設合併
        private void ModifyOfEstablishment(ChangeEvent ev, ChangeEvent0 ev0)
        {
            int pos = ev0.Description.IndexOf("が合併し、");
            if(pos == -1)
                pos = ev0.Description.IndexOf("が統合し、");
            if (pos >-1)
            {
                string v = ev0.Description.Substring(pos + 5);
                int l;
                if ((l = v.IndexOf('(')) > -1)
                {
                    int code = int.Parse(v.Substring(l + 1, 5));
                    ev.変更後地域 = new List<int> { code };
                }
                else
                {
                    var name = v.Substring(0, v.Length - 3);
                    string u = ev0.Description.Substring(0, pos - 1);
                    char[] separator = { '(', ')' };
                    string[] ss = u.Split(separator);
                    for(int m = 0; m < ss.Length -1; m++)
                    {
                        if(ss[m].TrimStart('、') == name)
                            ev.変更後地域 = new List<int> { int.Parse(ss[m+1]) };
                    }
                    if(ev.変更後地域 == null)
                        Console.WriteLine("同じ名前の市町村がない。");
                }
            }
            else
                Console.WriteLine("合併の詳細が異なる");
        }


        private void GetResultingCode(ChangeEvent ev, ChangeEvent0 ev0, string[] endWiths)
        {
            foreach(var endWith in endWiths)
            {
                if(ev0.Description.EndsWith(endWith))
                {
                    int code = int.Parse(ev0.Description.Substring(ev0.Description.Length - endWith.Length - 5, 5));
                    ev.変更後地域 = new List<int> { code };
                    return;
                }
            }
            Console.WriteLine($"変更事由の詳細が処理できない。ID:{ev0.Id} 変更事由の詳細: {ev0.Description}");
        }

        private List<int> ToStandardCode(List<string> list0)
        {
            return list0.Select(x => int.Parse(x.Substring(0, 5))).OrderBy(x => x).ToList();
        }

        #endregion

        public static void GetPrefectureData(Prefecture pref)
        {
            string query = Prefix +
                           $"SELECT * WHERE {{sac:C{pref.Id.ToString("00")}000-19700401 ?p ?o. FILTER(?p = rdfs:label)}}";
            var results = endpoint.QueryWithResultSet(query);
            foreach (var result in results)
            {
                string p = result["p"].ToString();
                if (p == rdfs + "label")
                {
                    string o = result["o"].ToString();
                    if (o.EndsWith("@ja"))
                        pref.名称 = o.Substring(0, o.Length - 3);
                    if (o.EndsWith("@ja-hrkt"))
                        pref.ふりがな = o.Substring(0, o.Length - 8);
                    if (o.EndsWith("@en"))
                        pref.英語名 = o.Substring(0, o.Length - 3);
                }
            }
        }
    }
}
