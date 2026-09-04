---
document: note_cadrage_migration_tarif_TA76_OC
version: "1.0"
date_redaction: 2026-09-01
statut: point_de_situation
perimetre: migration du document Tarif TA76 OC vers chunks Markdown indexables par le RAG Wikit
gamme: TA76 OC
gamme_nom: "Fenêtre aluminium à ouvrant caché"
collection: TRYBA ALUMINIUM
note_parente: note_cadrage_migration_tarif_H81_v1.md
note_heritee: note_cadrage_migration_tarif_T81_v1.md
source_primaire: "TA76_OC-infos-tarifs.xlsx (375 lignes, 137 colonnes, feuille unique Feuil1)"
source_controle: "Tarif_TA76_OC_HT_19-06-2026.pdf (70 pages)"
edition_ttc: absente
livrables: [generateur_tarif_TA76_OC.py, controle_conformite_TA76_OC.py, Tarif_TA76_OC_METHODE.md, Tarif_TA76_OC_PRIX_CHASSIS.md, Tarif_TA76_OC_OPTIONS.md, Tarif_TA76_OC_CHASSIS_SPECIAUX.md, Tarif_TA76_OC_FAISABILITES.md, Tarif_TA76_OC_TRANSVERSES.md]
---

# Note de cadrage — Migration du tarif TA76 OC vers Markdown

## 0. Objet et rapport aux notes antérieures

Ce document est une **déclinaison spécifique à la gamme TA76 OC** de la note de cadrage
tarif. Il hérite de la note H81 les principes généraux — auto-discrimination des chunks,
titre auto-porteur préfixé du code gamme, ligne de source par chunk, plafond de 200 mots
marqueur compris, prose sans puces, numérotation SC continue par fichier depuis SC0002,
fidélité numérique non négociable, anti-fantôme, signalement des arbitrages plutôt que
leur résolution silencieuse.

Il hérite de la note T81 son **architecture en six fichiers** et ses **règles T1 à T7**,
que la re-vérification des faits fondateurs a confirmées dans leur principe. Il les
**amende sur cinq points** et y **ajoute trois règles propres**, numérotées OC1 à OC3.

Contrairement au passage de H81 à T81, il ne s'agit donc pas d'une refondation. TA76 OC
est, comme T81, une fenêtre à frappe tarifée sur grilles dimensionnelles. Mais la
ressemblance s'arrête à la structure : le contenu diverge sur quatre faits fondateurs, et
l'Excel présente dix écarts avec le PDF, dont trois montants sans source.

## 1. Le produit et la collision de gammes

TA76 OC est la **fenêtre aluminium à ouvrant caché** de la collection TRYBA ALUMINIUM,
conformément au FIP, au CABP et à la Fiche Excellence déjà migrés. Ce n'est ni une
menuiserie PVC ni une porte d'entrée.

La paire **TA76 OC / TA76 OV** est identifiée comme à haut risque de collision sémantique :
même code de profilé, même collection, vocabulaire quasi identique, seule l'expression de
l'ouvrant les sépare. Trois conséquences normatives, opposables à tout chunk du corpus :

1. Le code gamme s'écrit **TA76 OC en toutes lettres dans chaque titre**. Aucun titre ne
   se réduit à « TA76 ».
2. La désignation porte la mention **« à ouvrant caché »**.
3. Aucun chunk ne doit pouvoir être servi à la place d'un chunk TA76 OV. L'audit vérifie
   en fin de parcours qu'aucun titre ni aucun corps ne laisse la gamme indéterminée.

L'ouvrant caché **n'est pas un axe de prix** : la gamme entière est à ouvrant caché. C'est
une désignation, pas une modalité tarifaire.

## 2. Faits vérifiés sur TA76 OC

Vérifications menées sur l'Excel et croisées au PDF, avant toute génération.

### 2.1 Ce qui tient, à l'identique de T81

**Le prix est dimensionnel par grille.** Aucun prix forfaitaire de châssis. La clé d'un
prix est le triplet type d'ouverture, hauteur, largeur.

**Une cellule est un intervalle, et le tarif l'énonce.** Page 9, en toutes lettres : la
colonne intitulée 1000 couvre « de 901 à 1000 mm de largeur », la ligne intitulée 600
couvre « de 501 à 600 mm de hauteur ». La règle a été cherchée explicitement dans le
tarif, non transposée depuis T81. La même page ajoute deux mentions propres à cette
gamme : les prix valent « pour châssis sans compléments », et le prix indiqué est celui
« du châssis de base (vitrage standard compris) ».

**L'échelle est le pas de 100 mm**, sur les deux axes, pour toutes les grilles.

**Cohérence HT/TTC parfaite.** Sur les 1 659 cellules de grille HT, l'ensemble des
largeurs renseignées en HT l'est exactement en TTC. Sur les 237 montants scalaires, aucun
HT sans TTC ni l'inverse.

**Quatre colonnes entièrement vides**, les mêmes que sur T81 : `Px en T HT`,
`Px fillant HT`, `Px en T TTC`, `Px fillant TTC`. Aucune autre colonne remplie n'est
écartée.

**Colonne gamme propre.** 353 lignes renseignées, toutes TA76 OC. Aucune ligne attribuée à
une autre gamme, contrairement aux lignes T82 et T83 de T81.

### 2.2 Ce qui change

**Six grilles de châssis, pas neuf.** 1 ouvrant à la française (20 bandes de hauteur, 500
à 2400), 2 ouvrants égaux à la française (20, 500 à 2400), châssis fixes (22, 500 à 2600),
soufflet normal SN (6, 500 à 1000), soufflet normal à poignée latérale (5, 600 à 1000),
soufflet d'aération avec ferme-imposte SA (7, 500 à 1100). Ni variante grande hauteur, ni
coulissant.

**Une grille à un seul axe apparaît.** Les quatre grilles d'entrée d'air « Spécificités
Belgique » — Invisivent EVO et THM90 EVO, chacune sur châssis blanc et sur châssis d'une
autre couleur — sont tarifées par la **largeur seule**, sans hauteur, de 100 à 6000 mm
pour l'Invisivent et de 100 à 3000 mm pour le THM90. La colonne `hauteur` y est vide et la
colonne `HT` porte un zéro de remplissage. La règle T1 ne prévoyait pas ce cas.

**Volumétrie.** 1 659 cellules HT et autant en TTC : 1 141 pour les six grilles de
châssis, 338 pour les deux grilles d'habillage alu sur vitrage, 180 pour les quatre
grilles belges. Toutes les plages de largeur sont contiguës. Les grilles 1 OF, 2 OF et
châssis fixes ne sont pas rectangulaires ; SN, SN à poignée latérale, SA et les deux
habillages le sont.

**La conversion des cotes n'est pas celle de T81.** La distinction existe (page 6 : cotes
prix `LT × HT`, cotes fabrication `L × H`), mais il n'y a pas de table par dormant. Il y a
deux régimes. Sans complément, `LT × HT = L × H` pour les cinq dormants listés page 6 —
AK10130 (L74) neuf, AK10117 (L83) store, AK10130 (L74) fixe, AK10120 (LZ109), AK10121
(LZ139). Avec compléments, `LT × HT` désigne le châssis nu tandis que `L × H` inclut les
compléments ; sur dormant à ailette avec appui, `HT = H − e`, où **e est l'épaisseur de la
pièce d'appui, donnée page 57** : 20 mm pour NF-TA84 et NF-TA84-D, 20,5 mm pour AK10118 et
AK10119. La table de conversion de T81 ne vaut pas ici.

**Les finitions aluminium sont tarifées en pourcentage.** Groupe 1 sans plus-value, Groupe
2 à +15 %, Groupe Sublimation tons bois à +25 %, RAL granité autres à +25 %, anodisation
champagne à +25 %. L'anodisation nature relève du Groupe 2. Relèvent de la règle T7 : non
transcrits.

**Une plus-value en pourcentage est adossée à chaque grille de prix.** Les pages 10 à 13
portent toutes « Plus-value dormants rénovation, AK10120 (LZ109) − AK10121 (LZ139),
+ 3 % sur les grilles de prix ». Pourcentage, donc T7, mais vivant à l'intérieur du
périmètre de F2 : traitée par la règle OC2.

**Natures d'information nouvelles.** Pack Trybadesign, ferrage invisible et poignée Toulon
laquée, 91 € par vantail. Laquage bloc-baie, avec un forfait réellement chiffré. Croisillons
à la grecque, forfait par châssis. Croisillons Art Déco tarifés au volume. Habillage alu sur
vitrage. Anodisation et laquages d'imitation. Limites de fabrication du battant par
épaisseur de vitrage, sous forme d'abaques colorées non transcriptibles en prose.

**Un chapitre entier se saisit sous la gamme jumelle.** Les pages 63 à 66, « Châssis fixes
spéciaux — à saisir en TA76 OV », portent des prix figurant au tarif TA76 OC mais dont la
saisie s'effectue en TA76 OV. Traité par la règle OC1.

### 2.3 Les pièges T81 sur cette gamme

**L'unité de facturation reste absente de l'Excel** et n'existe que dans le PDF. Relevée
page par page : au champ (28, 29, 30, 31), au volume (30), au châssis (29, 65), au mètre
linéaire (38, 49, 57, 58, 59, 60, 61, 62), forfaitaire (20, 38), à la pièce (46, 49, 50,
51, 62), au sachet (49), à l'ensemble (41, 50), au vantail (52), au mètre carré (24, 25,
26, 27, 57), à la fixation (65, 66), à l'unité (66). Les pages 24 à 27 ajoutent une
contrainte propre : « Surface mini de facturation = 0,5 m² ».

**Cinq blocs chiffrés n'énoncent aucune unité** : poignées (35), crémones à l'ancienne
(36), chatières (43), vitrage d'altitude (23), plus-value pour rabaisser la poignée (47).

**Un même tableau mêle deux unités sous un montant identique.** Page 38, les meneaux
battants sont à 74 € le mètre linéaire et la plus-value pour deux fixations à angle droit
à 74 € forfaitaires. Le rattachement par le montant est ici inopérant.

**Le discriminant manquant se reproduit**, au chapitre des croisillons en alu laqué : quatre
groupes portent des prix différents sous des clés strictement identiques. Le discriminant
est la finition, présent page 28 et rattachable par le montant.

**Deux pièges T81 ne se reproduisent pas.** Le discriminant cintré / rectangulaire de
l'habillage, qui n'existait que dans le PDF sur T81, figure ici dans la colonne `tableau`.
Le discriminant croisillon en T ou croix / croisillon filant, que les quatre colonnes vides
ne portent pas, figure dans la colonne `Détails`.

**La sémantique du zéro n'est pas uniforme.** Quatorze zéros, quatre sens. Sept sont une
absence réelle de plus-value, à conserver et à énoncer comme telle : cinq vitrages inclus
dans la triple offre (page 24) et deux compositions libres de base (page 26). Quatre sont
un remplissage structurel sur les lignes de grilles belges, dont le prix vit dans les
colonnes de largeur. Un est un faux zéro sur une case non tarifée : le panneau phonique en
36 mm, dont la cellule est vide au PDF page 27. Les deux derniers, sur le motif Art Déco
MG9, sont ambigus.

**La pagination est piégée, à l'inverse de T81.** Le numéro imprimé en pied de page
coïncide exactement avec l'index PDF sur les 70 pages, et les en-têtes de page sont
fiables. C'est le **sommaire général de la page 3 qui est périmé**, avec un décalage de
quatre pages à partir du chapitre OPTIONS et jusqu'à la fin. Les intercalaires de section
sont inégaux : ceux de GÉNÉRALITÉS et de GRILLES DE PRIX sont justes, ceux de VITRAGES et
de CHÂSSIS FIXES SPÉCIAUX ne le sont pas. **Seuls les en-têtes de page font foi.**

### 2.4 Écarts Excel ↔ PDF

Dix écarts relevés, tous consignés au journal, aucun résolu en silence.

Trois montants n'ont aucune contrepartie dans les 70 pages : `HF2245_CE` à 105 €, « Judas
optique » à 47 €, et deux valeurs du profil `I45` à 49 € et 38 €. Le mot « judas »
n'apparaît nulle part dans le tarif et la référence existe sur H81 : contamination
inter-gammes probable.

Le seuil `AS10100` vaut 196 € la pièce page 51 et 188 € la pièce page 62 ; le profil
plinthe vaut 99 € (`AK10131`, page 51) et 96 € (`5120SN`, page 62). L'Excel résout en
introduisant `AK10123` et `AK10255`, références absentes du PDF.

Huit références de grilles d'entrée d'air portent le suffixe `_DV` dans l'Excel là où le
PDF écrit `_CE` ou aucun suffixe, l'Excel conservant par ailleurs `_CE` sur deux autres.

Deux coquilles de référence : `AS10101-RA1` et `AK10100-RA2` pour `AS10100-RA1` et
`AS10100-RA2`. Coquilles de libellé : « fillant », « TA76OC ».

Le profilé `AS20200`, « Sans PV » page 62, est absent de l'Excel. La poignée Toulon a un
prix conditionnel page 35 — sans plus-value dans le pack Trybadesign, 38 € hors pack —
dont l'Excel ne retient que le second terme. L'appariement des meneaux est
structurellement faux dans l'Excel.

### 2.5 Vocabulaire

Le terme **« crémone » n'est pas à restreindre sur cette gamme**. Le tarif l'emploie seul
et librement : page 13, « Ferrures : crémone + compas de sécurité + paumelles » ; page 51,
« Crémone à barillet à béquille double » et « Crémone condamnable (ou crémone serrure) ».
La règle restrictive d'H81 et de T81 ne s'applique pas ici, et « crémones à l'ancienne »
est une option décorative chiffrée page 36.

**« Anti-décrochement »** est le terme du tarif — page 5, « galets champignons
anti-décrochement » ; pages 46 et 52, « évitant le décrochage de la fenêtre ». Aucune
occurrence d'« anti-dégondage ».

Les **paumelles relèvent de la tenue et de la durabilité**, jamais de l'anti-effraction :
page 53, elles sont caractérisées par un poids maximal admissible, la limite étant
« restreinte par les profilés à ouvrant caché ». L'anti-effraction est portée par les
gâches de sécurité à galets champignons.

Faux synonymes proscrits, hérités du corpus : gond ou charnière pour paumelle, survitrage
pour triple vitrage isolant, ouverture à soufflet pour oscillo-battant, anti-dégondage pour
anti-décrochement.

## 3. Architecture en six fichiers

Le principe reste celui d'un fichier par nature d'information.

**F1 MÉTHODE ET COTES.** Cotes de tarif contre cotes de fabrication et les deux régimes de
conversion, lecture des grilles par bandes, valeur d'abaque des grilles, composition d'un
prix, vocabulaire et abréviations de la gamme, statut du PDF comme document de référence.
Clé de voûte : sans lui, tous les prix de F2 sont exploitables de travers.

**F2 PRIX DES CHÂSSIS.** **Tout ce qui se lit par bandes** : les six grilles de châssis,
les deux grilles d'habillage alu sur vitrage, les quatre grilles d'entrée d'air belges.
Écart assumé avec T81, qui rangeait l'habillage en F4 : ici F2 coïncide avec la règle T1,
et F4 est réservé au seul chapitre à saisie sous gamme jumelle, dont le contexte ne doit
pas être dilué.

**F3 OPTIONS ET PLUS-VALUES.** Les postes forfaitaires, regroupés iso-prix et iso-unité.

**F4 CHÂSSIS SPÉCIAUX.** Les pages 64 à 66, chapitre « à saisir en TA76 OV ».

**F5 FAISABILITÉS ET RESTRICTIONS.** Ce qui se combine avec quoi, sans aucun montant.

**F6 TRANSVERSES.** Offre couleurs, couleurs des accessoires et des joints, laquage
bloc-baie hors son forfait, plus-value dormants rénovation, plus-value vitrage des châssis
spéciaux, majoration de gravure sur vitrage sablé : existence et localisation, aucun
montant, aucun pourcentage.

**Le liant inter-fichiers** est le **type d'ouverture**, qui traverse F2, F4 et F5, et doit
y figurer à l'identique, libellé complet et code.

## 4. Les règles normatives

Les règles T1 à T7 de la note T81 s'appliquent, avec les amendements ci-dessous, et sont
complétées par OC1 à OC3.

### Règle T1 — Découpage des prix de grille (amendée)
Un chunk par grille et par bande de hauteur, énumérant les bandes de largeur. Quand la
ligne ne tient pas sous 200 mots, elle est scindée en tranches contiguës de largeurs, la
tranche figurant au titre. **La coupure est pilotée par le plafond de mots, calculé chunk
par chunk, jamais par une constante.** Anti-fantôme : une cellule n'existant pas au tarif
ne donne pas de chunk.

*Amendement OC — grille à un seul axe.* Lorsque la grille ne comporte pas d'axe de
hauteur, le chunk énumère les seules bandes de largeur, le titre ne porte pas de bande de
hauteur, et le corps énonce explicitement que le tarif ne dépend que de la largeur.

### Règle T2 — Rédaction des prix (amendée)
Une phrase d'introduction nommant la grille, le type d'ouverture, son synonyme d'usage et
la bande de hauteur en clair ; puis une énumération en prose où chaque bande de largeur est
liée dans la même proposition à son prix HT et à son prix TTC. **Les bandes sont énoncées
comme intervalles** — « de 901 à 1000 mm » — jamais comme points. La première bande, dont
le plancher n'est pas donné par le tarif, s'écrit « jusqu'à N mm » : aucun plancher n'est
inventé.

*Amendement OC.* Le corps porte les deux mentions de la page 9 : les prix valent pour un
châssis sans complément, vitrage standard compris.

### Règle T3 — Instruction LLM (amendée)
Le modèle réclame trois éléments avant tout prix de châssis : le type d'ouverture, la cote
tarif en largeur, la cote tarif en hauteur. Il vérifie que la cote fournie est bien une cote
de tarif et non une cote de fabrication. Il ne calcule aucun total, n'additionne aucun
composant, n'interpole ni n'extrapole. Si la dimension demandée sort de la grille, il
indique que la configuration n'est pas tarifée plutôt que d'approcher.

*Amendement OC.* La conversion à énoncer en F1 est celle de la gamme — deux régimes et
`HT = H − e` — et non la table T81.

### Règle T4 — Options et plus-values forfaitaires (amendée)
Un chunk par poste, regroupement iso-prix : mêmes chapitre, tableau, désignation, HT et
TTC. Seules les variantes de la colonne Détails fusionnent, et le chunk les nomme toutes.
**Tout poste chiffré déclare son unité de facturation**, relevée page par page dans le PDF.
L'unité n'existe nulle part dans l'Excel. **Le montant servi est unitaire** : le chunk
énonce explicitement que le total s'obtient en multipliant par la quantité concernée, et
que ce calcul revient à l'ADV. Quand l'unité n'a pas pu être établie contre le PDF, le
chunk le dit et renvoie à la page plutôt que de servir un montant nu. Quand plusieurs prix
distincts se présentent sous des colonnes strictement identiques, le discriminant manquant
est **repris du PDF et rattaché par le montant**, non par l'ordre des lignes. À défaut de
discriminant relevé, le poste **n'est pas généré**.

*Amendement OC 1 — le regroupement iso-prix exige l'identité de l'unité.* Deux postes de
même montant mais d'unités différentes ne fusionnent jamais. Ce cas se présente deux fois :
le motif Art Déco MG9, facturé par châssis et quatre angles quand ses homologues de même
montant le sont au volume ; les meneaux battants, à 74 € le mètre linéaire, contre la
plus-value de fixation à 74 € forfaitaires.

*Amendement OC 2 — sémantique du zéro à quatre cas.* Un montant nul est généré avec mention
explicite d'absence de plus-value lorsqu'il correspond à une valeur nulle imprimée au tarif.
Il n'est pas généré lorsqu'il correspond à une cellule vide du PDF, et le chunk voisin
énonce alors l'absence de tarification. Il n'est pas traité comme un poste lorsqu'il n'est
qu'un remplissage sur une ligne de grille. Lorsque sa lecture est ambiguë, il n'est pas
généré et l'absence est exposée.

*Amendement OC 3 — maille des croisillons.* Pour les croisillons, la maille est la
**finition**, et le chunk porte dans la même phrase le prix du croisillon en T ou croix et
celui du croisillon filant. Un ADV demande le prix d'un croisillon laqué RAL, non le prix
d'une jonction. Même maille pour les gravures Art Déco au champ et les croisillons
rapportés. Pour les motifs Art Déco au volume, la maille est le **motif**, le montant étant
identique pour les trois types de gravure.

### Règle T5 — Châssis spéciaux (redéfinie)
Il n'existe aucune plus-value croisée forme × type d'ouvrant sur TA76 OC. La règle
s'applique désormais à la tarification des châssis spéciaux des pages 64 à 66 : un chunk
par poste, anti-fantôme strict, et **lorsqu'une valeur existe sans sa contrepartie, le chunk
transcrit ce qui existe et expose l'absence** plutôt que de la combler ou de supprimer le
prix.

### Règle T6 — Faisabilités et restrictions
Un chunk par restriction, en prose, sans aucun montant, énonçant ce qui est possible et ce
qui ne l'est pas, avec renvoi à la page du tarif. Les abaques de limites de fabrication du
battant par épaisseur de vitrage (pages 54 et 55) ne sont pas transcrites : le chunk énonce
leur existence, leur principe de lecture et renvoie aux pages.

### Règle T7 — Transverses
Existence, nature et logique tarifaire, renvoi à la page. Aucun prix, aucun pourcentage,
aucune valeur calculable — cohérence avec la règle T3.

### Règle OC1 — Chapitre à saisie sous la gamme jumelle
Les chunks des pages 64 à 66 conservent le préfixe de titre `TA76 OC` et la désignation
« à ouvrant caché », parce que les prix figurent au tarif TA76 OC. Leur corps énonce
**explicitement** que la saisie s'effectue en TA76 OV. Aucun chunk ne doit permettre de
conclure que le produit change de gamme, ni l'inverse. Les restrictions symétriques des
pages 8 et 45 — le SB-CC n'est faisable qu'en TA76 OV, certains châssis fixes ne sont
faisables qu'en TA76 OV — sont portées par F5.

### Règle OC2 — Plus-value en pourcentage adossée aux grilles
La plus-value de 3 % sur dormants rénovation figure sur les quatre pages de grilles. C'est
un pourcentage, donc elle relève de T7 et n'est pas transcrite. Elle donne **un chunk
d'orientation unique** en F6, et n'est pas répétée dans les trois cents chunks de prix, où
elle alourdirait chaque corps sans servir la récupération.

### Règle OC3 — Hiérarchie des sources
Le tarif énonce sa propre hiérarchie page 2 : « Le logiciel Syscon étant en constante
évolution, cette version PDF reste le seul document de référence. » Il en découle que
**les montants viennent de l'Excel, les libellés et les références viennent du PDF**.
L'Excel reste la source primaire de fidélité numérique ; dès qu'un libellé, une référence
ou un rattachement diverge, le PDF fait foi, et la divergence est consignée au journal.
Lorsque deux pages du PDF portent des montants différents pour un même produit, **aucune
n'est retenue contre l'autre** : les deux chunks sont générés, chacun sourcé à sa page, et
chacun signale l'existence de l'autre montant.

## 5. Format des chunks

Hérité de la note générale. Spécialisations TA76 OC :

```
## TA76 OC Fenêtre aluminium à ouvrant caché — Tarif du châssis à 1 ouvrant à la française, hauteur de 701 à 800 mm, toutes largeurs tarifées
## TA76 OC Fenêtre aluminium à ouvrant caché — Plus-value du vitrage TRY'ver 6-8G-6-10G-6
## TA76 OC Fenêtre aluminium à ouvrant caché — Tarif du croisillon intégré en alu laqué RAL
## TA76 OC Fenêtre aluminium à ouvrant caché — Restrictions de ferrage sur seuil et SB-CC
## TA76 OC Fenêtre aluminium à ouvrant caché — Existence et localisation de l'offre couleurs
```

Le titre de prix porte le libellé du tarif **et** le synonyme d'usage : un ADV demande une
fenêtre à un vantail, pas un ouvrant à la française. Puisque la récupération repose sur la
partie non numérique du chunk, l'enrichissement lexical est le mécanisme de récupération
lui-même.

Ligne de source : `*Source : Tarif—TA76_OC—HT—19-06-2026.pdf, page N — information
originale|complémentaire — SCxxxx*`, em-dashes dans le nom affiché, underscores dans le
champ YAML `document_source`.

## 6. Décomptes prévisionnels

| Fichier | Chunks | Assiette |
|---|---|---|
| Tarif_TA76_OC_METHODE.md | ~8 | PDF p. 2, 6, 7, 9, 47, 57 |
| Tarif_TA76_OC_PRIX_CHASSIS.md | 305 | 1 659 cellules HT et autant en TTC |
| Tarif_TA76_OC_OPTIONS.md | ~190 | groupes iso-prix et iso-unité |
| Tarif_TA76_OC_CHASSIS_SPECIAUX.md | 12 | PDF p. 65, 66 |
| Tarif_TA76_OC_FAISABILITES.md | ~26 | PDF p. 8, 22, 23, 25, 27 à 31, 36, 37, 41, 43, 45, 47, 51 à 55, 64 |
| Tarif_TA76_OC_TRANSVERSES.md | ~8 | PDF p. 10-13, 15 à 20, 30, 65, 69 |
| **Total** | **~549** | |

Détail de F2, issu de l'empaquetage réel sous plafond : 36 chunks pour le 1 ouvrant à la
française, 54 pour le 2 ouvrants égaux, 80 pour les châssis fixes, 18 pour le soufflet
normal, 9 pour le soufflet normal à poignée latérale, 28 pour le soufflet d'aération, 26 et
26 pour les deux habillages, 28 pour les quatre grilles belges.

## 7. Industrialisation

**Générateur** (`generateur_tarif_TA76_OC.py`). Lit la feuille unique, produit les six
fichiers. Fonctions propres à la gamme : calcul des bandes depuis l'échelle du tableau,
empaquetage glouton sous le plafond, variante de grille à un seul axe, table des unités de
facturation avec clé à trois niveaux, table des discriminants rattachés par le montant,
maille croisillons par finition, table des exclusions arbitrées, journal des exclusions et
des colonnes non mappées.

**Audit** (`controle_conformite_TA76_OC.py`). Autonome : relit les `.md` sans réutiliser
aucune fonction du générateur et **re-dérive les règles depuis la présente note**.
Contrôles conservés de T81 : couverture exhaustive des cellules — chaque valeur de l'Excel
dans un chunk et un seul ; fidélité numérique exhaustive contre la cellule ; bornes de
bandes recalculées indépendamment ; bijection des postes forfaitaires par multiensembles
HT/TTC, insensible aux libellés ; déclaration d'une unité de facturation sur tout poste
chiffré ; unicité des titres dans chaque fichier ; absence de tout montant dans les
faisabilités et les transverses ; vocabulaire ; croisement PDF page par page, qui valide au
passage la table des pages.

Contrôles nouveaux : **auto-discrimination OC/OV**, qui vérifie qu'aucun titre ne réduit la
gamme à « TA76 » et qu'aucun chunk n'omet la mention « à ouvrant caché » ; **conformité de
la table des pages aux en-têtes**, et non au sommaire ; **cohérence unité–montant** dans le
regroupement iso-prix.

## 8. Reste à traiter

- **Aucune édition TTC du tarif n'existe** : les 1 659 valeurs TTC de grille et les montants
  TTC scalaires ne sont contrôlables que contre l'Excel, sans second témoin. Limite assumée,
  signalée par l'audit en avertissement.
- **Trois montants exclus faute de source** : `HF2245_CE`, « Judas optique », et les deux
  valeurs `I45` à 49 € et 38 €. Remontée au service documentation.
- **Deux arbitrages produit en attente** : la lecture du motif Art Déco MG9, et la
  divergence des seuils entre les pages 51 et 62.
- **Suspicion de contamination inter-gammes** sur le judas optique, à confirmer auprès du
  service produits.
- **Instructions.md** : encoder la règle T3 dans sa forme TA76 OC, confirmer que la
  directive sur le terme « crémone » est contextuelle et ne s'applique pas aux gammes
  aluminium, et inscrire la paire TA76 OC / TA76 OV parmi les paires à non-import explicite.
- **Coquilles de l'Excel** non corrigées automatiquement, à soumettre à relecture :
  « fillant », « TA76OC », `AS10101-RA1`, `AK10100-RA2`.
