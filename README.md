# NAreaCode - 履歴付市区町村コードデータベース
国勢調査等の市区町村別の統計データを過去から集計して比較しようとすると、平成の大合併により過去のデータは合併前の市町村のデータを集計する必要があって普通に手作業ですると非常に手間がかかる作業になります。

そこで、[統計LOD](https://data.e-stat.go.jp/lodw/)で、標標準地域コードのデータが公開されているので、それを利用して、1970年以降の任意の時点での市区町村コード表の作成や対応表が作成できることを目標にしてこのデータベースを作成しました。
- 動作環境 Visual Studio 2017 RC3 又は .NET Core SDK 1.0 RC3

### コマンド
- new 履歴付市区町村コードデータベースを新規作成します。処理に1時間ぐらいかかるし、統計LOD のサイトに負荷をかけるのでできるだけ使わない方がいいです。
- update 統計LODのデータが更新されたときに履歴付市区町村コードデータベースを更新します。
- test Municipality Map Maker ウェブ版からダウンロードしたテストデータとのチェックをしています。

### データ
[dataフォルダー](https://github.com/timej/NAreaCode/tree/master/NAreaCode/data) に作成したデータを入れてあります。

- StandardAreaCodeList.json 期間付き市区町村コード
- ChangeEventList0.json 統計LODの変更事由データ
- ChangeEventList.json 市町村の変更事由データで必要な修正をしている
- WardChangeEventList.json 政令指定都市の区の変更事由データ
-	District0.json 現在ある郡のデータ
-	District.json 郡のデータ
- codelist_19701001and20151001.tsv Municipality Map Maker ウェブ版からダウンロードしたテストデータ

※市区町村コードの対応表が必要な場合は、[Municipality Map Maker ウェブ版](http://www.tkirimura.com/mmm/)で、対応表がダウンロードできるようになってているものが、そちらを利用してください。

## 統計LODの標準地域コードの使い方

- SPARQLエンドポイント: https://data.e-stat.go.jp/lod/sparql/query
- 検索用画面: https://data.e-stat.go.jp/lod/sparql/

以下のPREFIXが必要です。

```
PREFIX rdf:<http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs:<http://www.w3.org/2000/01/rdf-schema#>
PREFIX org:<http://www.w3.org/ns/org#>
PREFIX dcterms:<http://purl.org/dc/terms/>
PREFIX sacs:<http://data.e-stat.go.jp/lod/terms/sacs#>
PREFIX sac:<http://data.e-stat.go.jp/lod/sac/>
PREFIX sace:<http://data.e-stat.go.jp/lod/sace/>
PREFIX sacr:<http://data.e-stat.go.jp/lod/sacr/>
PREFIX owl:<http://www.w3.org/2002/07/owl#>
```

前橋市の現行標準地域コードを使って現在有効な期間つき標準地域コードを照会
```
SELECT * WHERE {sac:C10201 ?p ?o.}
```
期限付きコードが C10201-20090505であることがわかるので、以下でその期間つき標準地域コードを照会
```
#PREFIXは以下省略。
SELECT * WHERE {sac:C10201-20090505 ?p ?o.}
```
以下の結果がえられる
```
---------------------------------------------------------------------------------------------------
| p                                       | o                                                     |
===================================================================================================
| sacs:previousCode                       | sac:C10201-20090401                                   |
| rdf:type                                | sacs:StandardAreaCode                                 |
| rdfs:label                              | "前橋市"@ja                                              |
| rdfs:label                              | "Maebashi-shi"@en                                     |
| rdfs:label                              | "まえばしし"@ja-hrkt                                       |
| sacs:previousMunicipality               | sac:C10303-20090505                                   |
| dcterms:issued                          | "2009-05-05"^^<http://www.w3.org/2001/XMLSchema#date> |
| dcterms:isPartOf                        | sac:C10000-19700401                                   |
| sacs:administrativeClass                | sacs:CoreCity                                         |
| org:resultedFrom                        | sace:C5100                                            |
| dcterms:identifier                      | "10201"                                               |
| <http://imi.ipa.go.jp/ns/core/rdf#市区町村> | "前橋市"@ja                                              |
| <http://imi.ipa.go.jp/ns/core/rdf#都道府県> | "群馬県"@ja                                              |
| sacs:checkDigit                         | "6"                                                   |
---------------------------------------------------------------------------------------------------
```
2009-05-05に何かがあったことが分かるので、それを調べるために変更事由を照会
```
SELECT * WHERE {sace:C5100 ?p ?o.}
```
以下の結果になり、富士見村が前橋市に編入合併したことがわかる。
```
---------------------------------------------------------------------------------------------------------
| p                         | o                                                                         |
=========================================================================================================
| rdf:type                  | sacs:CodeChangeEvent                                                      |
| org:resultingOrganization | sac:C10201-20090505                                                       |
| org:resultingOrganization | sac:C10303-20090505                                                       |
| dcterms:description       | "富士見村(10303)が前橋市(10201)に編入"@ja                                            |
| dcterms:description       | "Fujimi-mura(10303) in Seta-gun is absorbed into Maebashi-shi(10201)."@en |
| org:originalOrganization  | sac:C10201-20090401                                                       |
| org:originalOrganization  | sac:C10303-19981101                                                       |
| dcterms:identifier        | "5100"                                                                    |
| sacs:reasonForChange      | sacr:absorption                                                           |
| dcterms:date              | "2009-05-05"^^<http://www.w3.org/2001/XMLSchema#date>                     |
---------------------------------------------------------------------------------------------------------
