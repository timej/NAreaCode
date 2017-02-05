# NAreaCode

## 統計LODの標準地域コードの使い方
前橋市の現行標準地域コードを使って現在有効な期間つき標準地域コードを照会
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
