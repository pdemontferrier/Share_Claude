---
document: note_cadrage_migration_tarif_TA76_OV
version: "1.0"
date_redaction: 2026-09-02
statut: point_de_situation_avant_generation
perimetre: migration du document Tarif TA76 OV vers chunks Markdown indexables par le RAG Wikit
gamme: TA76 OV
gamme_nom: "Fenêtre aluminium à ouvrant visible"
collection: "TRYBA ALUMINIUM"
materiau: aluminium
note_parente: note_cadrage_migration_tarif_H81_v1.md
note_heritee: note_cadrage_migration_tarif_TA76_OC.md
gammes_pilotes_heritees: [H81, T81, TA76 OC]
source_primaire: "TA76_OV-infos-tarifs.xlsx (409 lignes, 137 colonnes, feuille unique Feuil2)"
source_controle: "Tarif_TA76_OV_HT_19-06-2026.pdf et Tarif_TA76_OV_TTC_19-06-2026.pdf (70 pages chacun)"
edition_ttc: presente
livrables_prevus: [generateur_tarif_TA76_OV.py, controle_conformite_TA76_OV.py, Tarif_TA76_OV_METHODE.md, Tarif_TA76_OV_PRIX_CHASSIS.md, Tarif_TA76_OV_OPTIONS.md, Tarif_TA76_OV_CHASSIS_SPECIAUX.md, Tarif_TA76_OV_FAISABILITES.md, Tarif_TA76_OV_TRANSVERSES.md, Message_service_produit_TA76_OV.md]
---

# Note de cadrage — Migration du tarif TA76 OV vers Markdown

## 0. Objet et rapport aux notes antérieures

Ce document est une **déclinaison spécifique à la gamme TA76 OV** de la note de cadrage
tarif. Il hérite de la note H81 les principes généraux — auto-discrimination des chunks,
titre auto-porteur préfixé du code gamme, ligne de source par chunk, plafond de 200 mots
marqueur compris, prose sans puces, numérotation SC continue par fichier depuis SC0002,
fidélité numérique non négociable, anti-fantôme, signalement des arbitrages plutôt que
leur résolution silencieuse.

Il hérite de la note TA76 OC son **architecture en six fichiers**, ses **règles T1 à T7
dans leur forme amendée** et ses **règles OC1 à OC3**. La re-vérification des faits
fondateurs, menée sur l'Excel et croisée aux deux éditions du PDF, a confirmé l'essentiel
de ce socle. Ce n'est donc ni une refondation, comme le passage de H81 à T81, ni une simple
recopie.

Trois choses le distinguent de son parent. La gamme possède **deux grilles de plus** et un
**chapitre de châssis fixes spéciaux natif**, ce qui prive la règle OC1 de son objet. Une
**édition TTC existe**, ce qui lève la principale limite de contrôle de TA76 OC. Et le
risque de collision avec la gamme jumelle, déjà identifié comme le plus élevé du corpus,
devient **bidirectionnel et matérialisé**, puisque le corpus TA76 OC est déjà en place.

## 1. Le produit et la collision de gammes

TA76 OV est la **fenêtre aluminium à ouvrant visible** de la collection TRYBA ALUMINIUM,
conformément au FIP, au CABP et à la Fiche Excellence déjà migrés. Ce n'est ni une
menuiserie PVC ni une porte d'entrée.

La paire **TA76 OC / TA76 OV** est la plus exposée du corpus entier : même code de profilé,
même collection, même matériau, même type de produit, tarifs structurellement jumeaux page
pour page sur soixante-dix pages chacun. Seule l'expression de l'ouvrant les sépare. Quatre
conséquences normatives, opposables à tout chunk du corpus :

1. Le code gamme s'écrit **TA76 OV en toutes lettres dans chaque titre**. Aucun titre ne se
   réduit à « TA76 ».
2. La désignation porte la mention **« à ouvrant visible »**.
3. **Aucun titre ne contient « TA76 OC »**, dans aucun fichier. Le tarif TA76 OV ne renvoie
   nulle part à sa gamme jumelle : rien n'autorise à l'évoquer, sauf dans le chunk de
   vocabulaire de F1, où l'abréviation OV se définit par opposition à OC.
4. La vérification de non-collision est **bidirectionnelle** : aucun chunk TA76 OV ne doit
   pouvoir être servi à la place d'un chunk TA76 OC, **ni l'inverse**. Le corpus TA76 OC
   étant déjà déployé, l'audit relit les douze fichiers ensemble. Voir la règle OV1.

L'ouvrant visible **n'est pas un axe de prix** : la gamme entière est à ouvrant visible.
C'est une désignation, pas une modalité tarifaire.

Un artefact du document source mérite d'être connu : la page 31 porte, dans sa couche de
dessin, un poster de profilés étiqueté « TA76OC ». C'est une erreur du tarif, pas un renvoi
tarifaire. Elle n'est pas transcrite et l'audit ne doit pas la confondre avec une
contamination du corpus.

## 2. Faits vérifiés sur TA76 OV

Vérifications menées sur l'Excel et croisées aux deux éditions du PDF, avant toute
génération. L'Excel compte 409 lignes sur la feuille unique `Feuil2` — et non `Feuil1`,
comme sur TA76 OC — dont 404 non vides et 26 séparatrices, pour 137 colonnes.

### 2.1 Ce qui tient, à l'identique de TA76 OC

**Le prix est dimensionnel par grille.** Aucun prix forfaitaire de châssis. La clé d'un prix
est le triplet type d'ouverture, hauteur, largeur.

**Une cellule est un intervalle, et le tarif l'énonce.** Page 9, en toutes lettres : la
colonne intitulée 1000 couvre « de 901 à 1000 mm de largeur », la ligne intitulée 600 couvre
« de 501 à 600 mm de hauteur ». La règle a été cherchée explicitement dans le tarif, non
transposée. La même page porte les deux mentions de la gamme sœur : les prix valent « pour
châssis sans compléments », et le prix indiqué est celui « du châssis de base (vitrage
standard compris) ».

**L'échelle est le pas de 100 mm**, sur les deux axes, pour les quatorze grilles. Toutes les
plages de largeur sont contiguës, sans exception.

**La conversion des cotes est celle de TA76 OC.** Page 6 : cotes prix `LT × HT`, cotes
fabrication `L × H`, deux régimes, pas de table par dormant. Sans complément,
`LT × HT = L × H` pour les cinq dormants listés — AK10130J (L74) neuf, AK10117J (L83) store,
AK10130J (L74) fixe, AK10120J (LZ109) et AK10121J (LZ139). Avec compléments, `LT × HT`
désigne le châssis nu tandis que `L × H` inclut les compléments ; sur dormant à ailette avec
appui, `HT = H − e`, où e est l'épaisseur de la pièce d'appui donnée page 57 : 20 mm pour
NF-TA84 et NF-TA84-D, 20,5 mm pour AK10118 et AK10119. Les dormants compatibles y sont
également identiques — AK10130 (L74) et AK10117 (L83) pour la pièce neuf, AK10120 (LZ109),
AK10121 (LZ139) et AK10122 (L69) pour la pièce rénovation, AK10102 (L85) pour les pièces
courte et longue. Les profilés d'ouvrant visible ne changent donc rien à ce chapitre,
contrairement à ce que l'on pouvait craindre.

**Valeur d'abaque des grilles**, page 47 : les grilles de prix « définissent les limites de
fabrication des châssis TA76 OV et pourront de ce fait être utilisées comme abaques ».

**Quatre colonnes entièrement vides**, les mêmes que sur T81 et TA76 OC : `Px en T HT`,
`Px fillant HT`, `Px  en T TTC`, `Px  fillant TTC`. Le discriminant qu'elles auraient dû
porter — croisillon en T ou croix contre croisillon filant — vit dans la colonne `Détails`
et n'est donc pas perdu. Le discriminant cintré contre rectangulaire de l'habillage vit dans
la colonne `tableau`. Aucune autre colonne remplie n'est écartée : `Détails montant HT` et
`Détails montant TTC` ne sont renseignées que sur les huit lignes d'exemples de calcul,
elles-mêmes hors périmètre.

**Les finitions aluminium sont tarifées en pourcentage.** Groupe 1 sans plus-value, Groupe 2
à +15 %, Groupe Sublimation tons bois à +25 %, RAL granité autres à +25 %, anodisation
champagne à +25 %. Relèvent de la règle T7 : non transcrites.

**La plus-value en pourcentage adossée aux grilles subsiste.** Les pages 10 à 13 portent
toutes « Plus-value dormants rénovation, AK10120J (LZ109) − AK10121J (LZ139), + 3 % sur les
grilles de prix ». La règle OC2 s'applique sans modification.

**La hiérarchie des sources est énoncée par le tarif**, page 2, dans une formulation
légèrement différente de celle de TA76 OC : « Les logiciels Look et Syscon étant en
constante évolution, cette version PDF reste le seul document de référence. » Le pluriel et
la mention de Look sont à reprendre littéralement dans le chunk de F1.

### 2.2 Ce qui change

**Huit grilles de châssis, pas six.** TA76 OV possède les deux variantes grande hauteur
absentes de la gamme sœur :

| Grille | Bandes de hauteur | Plage de largeur | Cellules |
|---|---|---|---|
| 1 ouvrant à la française | 22, de 500 à 2600 | 500 à 1500 | 204 |
| 1 vantail grande hauteur | 3, de 2700 à 2900 | 500 à 900 | 11 |
| 2 ouvrants égaux à la française | 22, de 500 à 2600 | 900 à 2200 | 280 |
| 2 vantaux grande hauteur | 3, de 2700 à 2900 | 900 à 1400 | 16 |
| Châssis fixes | 22, de 500 à 2600 | 500 à 2500 | 445 |
| Soufflet normal SN | 6, de 500 à 1000 | 500 à 2000 | 96 |
| Soufflet normal à poignée latérale | 5, de 600 à 1000 | 600 à 1200 | 35 |
| Soufflet d'aération SA | 7, de 500 à 1100 | 500 à 2400 | 140 |

S'y ajoutent les deux grilles d'habillage alu sur vitrage, rectangulaire et cintré, de 300 à
1500 mm sur les deux axes, 169 cellules chacune ; et les quatre grilles d'entrée d'air
« Spécificités Belgique », à un seul axe. Ni coulissant, ni forme cintrée d'ouvrant.

**Volumétrie : 1 745 couples HT/TTC**, contre 1 659 sur TA76 OC. Soit 1 227 pour les huit
grilles de châssis, 338 pour les deux habillages, 180 pour les quatre grilles belges. Les
cinq grilles à la française et fixes ne sont pas rectangulaires ; les six autres et les
habillages le sont.

**Les grilles à un seul axe se confirment.** Invisivent EVO sur châssis blanc et sur châssis
d'une autre couleur sont tarifées par la largeur seule, de 100 à 6 000 mm, 60 valeurs
chacune ; THM90 EVO, dans les deux mêmes variantes, de 100 à 3 000 mm, 30 valeurs chacune.
La colonne `hauteur` y est vide et la colonne `HT` porte un zéro de remplissage.

**Le chapitre des châssis fixes spéciaux est natif.** Pages 63 à 66, sous l'en-tête « TA76
OV — CHÂSSIS SPÉCIAUX ». Les soixante-dix pages ont été passées au crible : **il n'existe
aucun renvoi vers TA76 OC**, ni pour ce chapitre, ni pour le SB-CC, ni pour aucune
configuration. La règle OC1 est donc **sans objet dans ce corpus**, et il n'y a pas de règle
symétrique à écrire.

**Les formes n'ouvrent aucune plus-value croisée.** Page 64, le triangle-rectangle, les
trapèzes et les polygones sont réservés au **fixe** ; page 8, l'ouvrant trapèze est marqué
infaisable et les ouvrants à meneaux en croix ou en T sont « en cours d'étude ». La règle T5
reste donc dans sa forme redéfinie sur TA76 OC et ne retrouve pas sa forme T81.

**Le SB-CC est natif.** Page 51, « En SB-CC, utilisation obligatoire de l'ouvrant visible
AM10105 ». Page 45, il est compatible avec le ferrage R20 comme avec le ferrage R20
invisible, le SB-CC sur seuil ne l'étant qu'avec le R20. La page 7 porte deux tables de
cotes utiles, STANDARD et SB/CC.

**Deux mentions propres aux pages de grilles** n'existaient pas sur la gamme sœur : « Non
réalisable avec seuil alu », pages 10 et 11, et le seuil de 550 mm qui sépare le levier en
feuillure du verrou à levier, page 11. Elles relèvent de F5.

**Le reste des natures d'information est stable** : Pack Trybadesign à 91 € le vantail avec
ferrage invisible et poignée Toulon laquée, laquage bloc-baie et son forfait, croisillons à
la grecque au forfait par châssis, croisillons Art Déco au volume, habillage alu sur
vitrage, anodisation et laquages d'imitation, limites de fabrication du battant par
épaisseur de vitrage sous forme d'abaques colorées non transcriptibles en prose.

### 2.3 Les pièges, contrôlés un par un

**L'unité de facturation reste absente de l'Excel** et n'existe que dans le PDF. Relevée
page par page : au champ (28, 29, 30, 31, 66), au volume (30), au châssis (29, 65), au mètre
linéaire (38, 49, 57, 58, 59, 60, 61, 62), forfaitaire (20, 38), à la pièce (46, 49, 50, 51,
62), au sachet (49), à l'ensemble (41, 50), au vantail (52), au mètre carré (24, 25, 26, 27,
57), à la fixation (65, 66), à l'unité (66). Les pages 24 à 27 ajoutent la contrainte
« Surface mini de facturation = 0,5 m² ».

**Cinq blocs chiffrés n'énoncent aucune unité**, exactement les mêmes que sur TA76 OC :
vitrage d'altitude (23), poignées (35), crémones à l'ancienne (36), chatières (43),
plus-value pour rabaisser la poignée (47).

**Un même tableau mêle deux unités sous un montant identique.** Page 38, les meneaux
battants sont à 74 € le mètre linéaire et la plus-value pour deux fixations à angle droit à
74 € forfaitaires. Le rattachement par le montant y est inopérant.

**Le discriminant manquant se reproduit** au chapitre des croisillons en alu laqué : six
lignes portent 18, 31 et 22 € en T ou croix et 13, 28 et 20 € en filant sous des clés
strictement identiques. Le discriminant est la finition, lisible page 28 et rattachable par
le montant.

**La sémantique du zéro compte six sens, contre quatre sur TA76 OC.**

1. *Absence réelle de plus-value*, sept cas, à conserver et à énoncer comme telle : cinq
   vitrages inclus dans la triple offre page 24, deux compositions libres de base page 26.
2. *Faux zéro sur cellule vide au PDF*, trois cas : le panneau phonique en 36, 44 et 48 mm,
   page 27, où seul le 28 mm porte 307 €. TA76 OC n'en avait qu'un.
3. *Cas ambigu*, deux cas, sur le motif Art Déco MG9.
4. *Remplissage scalaire sur ligne de grille*, quatre cas, sur les lignes belges dont le prix
   vit dans les colonnes de largeur.
5. *Ligne séparatrice*, quatre cas, sans chapitre ni désignation.
6. *Bourrage au-delà de la dernière largeur tarifée d'une grille à un seul axe*, **soixante
   cas**, sens nouveau : les deux grilles THM90 EVO portent un zéro de 3 100 à 6 000 mm
   alors que le tarif s'arrête à 3 000 mm. Ce sont d'ailleurs les seuls désalignements
   HT/TTC de tout le fichier, la colonne TTC y étant vide.

**La colonne gamme n'est pas propre.** Sur 387 lignes renseignées, 356 portent TA76 OV, cinq
portent « TA76 OC » sur le chapitre Seuil, et vingt-six portent « TA76OC », coquille
comprise, sur les deux grilles d'habillage alu. Les 31 valeurs ont été retrouvées telles
quelles dans le PDF TA76 OV, pages 62, 32 et 33 : ce sont des données TA76 OV et une
mauvaise étiquette, non une contamination de données. Elles sont générées, l'écart est
consigné.

**Cohérence HT/TTC parfaite** hors les soixante zéros de bourrage : aucun montant scalaire
HT sans TTC ni l'inverse, aucun désalignement sur les 1 745 cellules de grille.

**La pagination est piégée d'une troisième manière.** Les en-têtes de page sont fiables sur
les soixante-dix pages. Le numéro imprimé coïncide avec l'index PDF partout où il existe,
mais **il est absent de la page 24**, en plus des deux couvertures et de la Fiche Info
Produit. Le sommaire général de la page 3 est périmé avec un décalage croissant : juste
jusqu'à COULEURS, puis d'une page à partir des VITRAGES, de deux sur Remplissage, de quatre
d'OPTIONS jusqu'à la fin ; il **omet en outre deux chapitres entiers**, les plus-values
vitrages compositions libres (26) et l'habillage alu sur vitrage (32-33). Les intercalaires
de section sont périmés eux aussi, avec des décalages **différents de ceux du sommaire** :
quatre pages sur OPTIONS, cinq sur FERRAGE et POSE, six sur CHÂSSIS FIXES SPÉCIAUX et
EXEMPLES ; seuls ceux de GÉNÉRALITÉS, GRILLES DE PRIX et COULEURS sont justes.
**Seuls les en-têtes de page font foi, pour la troisième gamme consécutive.**

### 2.4 Écarts Excel ↔ PDF

Neuf écarts relevés, tous consignés au journal, aucun résolu en silence.

**Un décalage d'une ligne sur le tableau des grilles d'entrée d'air, page 41.** L'Excel
attribue à chaque référence le montant de la référence précédente, à partir de FR12 :

| Référence | Excel HT / TTC | PDF HT / TTC |
|---|---|---|
| FR12 | 13 / 19 | 46 / 66 |
| ISOLA2-45 | 46 / 66 | 57 / 82 |
| ISOLA245+RA | 57 / 82 | 85 / 122 |
| ISOLA-HY | 85 / 122 | 98 / 140 |
| ISOLA-HY+RA | 98 / 140 | 105 / 150 |
| HF2245_CE | 105 / 150 | aucune contrepartie |

Le décalage est confirmé indépendamment par les deux éditions, HT et TTC : ce n'est pas un
artefact d'extraction. **Arbitrage rendu : l'Excel fait foi pour le montant**, conformément
à l'amendement OV de la règle OC3, et chaque chunk concerné expose la valeur du PDF avec
attribution.

**Deux valeurs du profil `I45`, 49 € et 38 €**, sans contrepartie page 28, où le tarif ne
porte qu'une seule ligne I45 laquée blanc à 36 € en T ou croix et 24 € en filant. Motif
d'exclusion : la règle T4, deux prix différents ne pouvant porter le même titre en l'absence
de discriminant relevé.

**« Judas optique » à 47 €**, ligne 304, rangée au chapitre Chatière. Le mot n'apparaît dans
aucune des soixante-dix pages et la référence existe sur H81. Motif d'exclusion : le risque
de contamination inter-gammes, non la fidélité. Remontée au service documentation avec les
deux `I45`.

**Le seuil universel.** L'Excel introduit `AK10123` et `AK10255` pour ce que le PDF appelle
`AK10100` et `AK10131`, page 51. Références du PDF retenues, montants de l'Excel.

**Huit références de grilles d'entrée d'air portent le suffixe `_DV`** là où le PDF écrit
`_CE` ou aucun suffixe, l'Excel conservant par ailleurs `_CE` sur deux autres.

**Le profilé `AS20200`**, « Sans PV » page 62, est absent de l'Excel.

**La poignée Toulon a un prix conditionnel** page 35 — sans plus-value dans le pack
Trybadesign, 38 € hors pack — dont l'Excel ne retient que le second terme.

**L'appariement des meneaux** est structurellement faux dans l'Excel, comme sur TA76 OC.

**Coquilles de libellé** : « fillant », « Croisillons fillant », `CAL-NRE` pour `CAL-N RE`,
« SN avec  poignée » à double espace, et le détail « 28, 36, 49 » pour 48 mm.

Hors ces écarts, la fidélité est intégrale : les 120 lignes de grille se retrouvent verbatim
dans le PDF HT et dans le PDF TTC, et tous les montants scalaires se retrouvent sur leur
page attendue.

### 2.5 Postes présents au PDF et absents de l'Excel

Onze blocs sont explicitement tarifés à zéro dans le tarif et n'ont aucune ligne dans le
fichier : les poignées standard Lento, Liège et Toulon (35), la poignée Toulon incluse au
pack Trybadesign (35), les meneaux dormants AK10115J et AK10116J (38), le profil d'ouvrant
AM10100J, les parecloses pour fixe et les parecloses pour ouvrants (39), le levier en
feuillure (46), le ferrage R20 et le ferrage symétrique, tous deux « en standard » (46), les
paumelles (53) et le profilé de finition `AS20200` (62). Ils sont générés à 0 € depuis le
PDF, en information originale.

Trois mentions ne portent aucun montant et relèvent de F5 : « Nous consulter » pour le
flexible de longueur supérieure (49), « non réalisable » pour les gravures Art Déco en
18 mm (30), « non disponible à l'offre » pour les parecloses galbées en 48 mm (39).

### 2.6 Vocabulaire

Le terme **« crémone » n'est pas à restreindre sur cette gamme**. Le tarif l'emploie seul et
librement : page 13, « Ferrures : crémone + compas de sécurité + paumelles » ; page 51,
« Crémone à barillet à béquille double » et « Crémone condamnable (ou crémone serrure) ». La
règle restrictive d'H81 et de T81 ne s'applique pas, et « crémones à l'ancienne » est une
option décorative chiffrée page 36.

**« Anti-décrochement »** est le terme du tarif — page 5, « galets champignons
anti-décrochement » ; pages 46 et 52, « évitant le décrochage de la fenêtre ». Aucune
occurrence d'« anti-dégondage » dans les soixante-dix pages.

Les **paumelles relèvent de la tenue et de la durabilité**, jamais de l'anti-effraction :
page 53, elles sont caractérisées par un poids maximal admissible — 60 kg en P60, 100 kg en
compas OF et en ferrage OB, 130 kg en ferrage invisible. L'anti-effraction est portée par
les gâches de sécurité à galets champignons.

Faux synonymes proscrits, hérités du corpus, et dont aucune occurrence n'a été relevée dans
le tarif : gond ou charnière pour paumelle, survitrage pour triple vitrage isolant,
ouverture à soufflet pour oscillo-battant, anti-dégondage pour anti-décrochement.

## 3. Architecture en six fichiers

Le principe reste celui d'un fichier par nature d'information.

**F1 MÉTHODE ET COTES.** Cotes de tarif contre cotes de fabrication et les deux régimes de
conversion, épaisseurs des pièces d'appui, lecture des grilles par bandes, valeur d'abaque
des grilles, composition d'un prix, cotes utiles et leurs deux tables, vocabulaire et
abréviations de la gamme, statut du PDF comme document de référence. Clé de voûte : sans
lui, tous les prix de F2 sont exploitables de travers.

**F2 PRIX DES CHÂSSIS.** Tout ce qui se lit par bandes : les huit grilles de châssis, les
deux grilles d'habillage alu sur vitrage, les quatre grilles d'entrée d'air belges.

**F3 OPTIONS ET PLUS-VALUES.** Les postes forfaitaires, regroupés iso-prix et iso-unité.

**F4 CHÂSSIS SPÉCIAUX.** Les pages 65 et 66. Contrairement à TA76 OC, ce fichier ne porte
aucune mention de saisie sous une autre gamme : le chapitre est natif.

**F5 FAISABILITÉS ET RESTRICTIONS.** Ce qui se combine avec quoi, sans aucun montant.

**F6 TRANSVERSES.** Offre couleurs, couleurs des accessoires et des joints, laquage
bloc-baie hors son forfait, plus-value dormants rénovation, plus-value vitrage des châssis
spéciaux, majoration de gravure sur vitrage sablé, surfaces minimales de facturation,
historique des évolutions : existence et localisation, aucun montant, aucun pourcentage.

**Le liant inter-fichiers** est le **type d'ouverture**, qui traverse F2, F4 et F5, et doit
y figurer à l'identique, libellé complet et code.

## 4. Les règles normatives

Les règles T1 à T7 de la note T81, amendées par la note TA76 OC, s'appliquent avec les
amendements OV ci-dessous. Les règles OC2 et OC3 s'appliquent, OC3 étant amendée. La règle
OC1 est sans objet. Une règle propre, OV1, s'y ajoute.

### Règle T1 — Découpage des prix de grille (amendée)
Un chunk par grille et par bande de hauteur, énumérant les bandes de largeur. Quand la ligne
ne tient pas sous 200 mots, elle est scindée en tranches contiguës de largeurs, la tranche
figurant au titre. **La coupure est pilotée par le plafond de mots, calculé chunk par chunk,
jamais par une constante.** Anti-fantôme : une cellule n'existant pas au tarif ne donne pas
de chunk.

*Amendement OC — grille à un seul axe.* Lorsque la grille ne comporte pas d'axe de hauteur,
le chunk énumère les seules bandes de largeur, le titre ne porte pas de bande de hauteur, et
le corps énonce explicitement que le tarif ne dépend que de la largeur.

*Amendement OV 1 — variantes grande hauteur.* Deux grilles partagent leur type d'ouverture
avec la grille principale et ne s'en distinguent que par des bandes de hauteur disjointes.
Le titre porte donc **la mention « grande hauteur » en plus de la bande**, faute de quoi deux
chunks du même fichier ne se distingueraient que par un nombre — exactement le défaut que
la maille cherche à éviter.

*Amendement OV 2 — zéro de bourrage.* Un zéro situé au-delà de la dernière largeur tarifée
d'une grille à un seul axe est un remplissage, non un prix. Anti-fantôme : aucune cellule
n'est générée pour les soixante concernées.

### Règle T2 — Rédaction des prix (amendée)
Une phrase d'introduction nommant la grille, le type d'ouverture, son synonyme d'usage et la
bande de hauteur en clair ; puis une énumération en prose où chaque bande de largeur est liée
dans la même proposition à son prix HT et à son prix TTC. **Les bandes sont énoncées comme
intervalles** — « de 901 à 1000 mm » — jamais comme points. La première bande, dont le
plancher n'est pas donné par le tarif, s'écrit « jusqu'à N mm » : aucun plancher n'est
inventé.

*Amendement OC.* Le corps porte les deux mentions de la page 9 : les prix valent pour un
châssis sans complément, vitrage standard compris.

### Règle T3 — Instruction LLM (amendée)
Le modèle réclame trois éléments avant tout prix de châssis : le type d'ouverture, la cote
tarif en largeur, la cote tarif en hauteur. Il vérifie que la cote fournie est bien une cote
de tarif et non une cote de fabrication. Il ne calcule aucun total, n'additionne aucun
composant, n'interpole ni n'extrapole. Si la dimension demandée sort de la grille, il indique
que la configuration n'est pas tarifée plutôt que d'approcher.

*Amendement OC.* La conversion à énoncer en F1 est celle de la gamme — deux régimes et
`HT = H − e` — et non la table T81.

### Règle T4 — Options et plus-values forfaitaires (amendée)
Un chunk par poste, regroupement iso-prix : mêmes chapitre, tableau, désignation, HT et TTC.
Seules les variantes de la colonne Détails fusionnent, et le chunk les nomme toutes. **Tout
poste chiffré déclare son unité de facturation**, relevée page par page dans le PDF. L'unité
n'existe nulle part dans l'Excel. **Le montant servi est unitaire** : le chunk énonce
explicitement que le total s'obtient en multipliant par la quantité concernée, et que ce
calcul revient à l'ADV. Quand l'unité n'a pas pu être établie contre le PDF, le chunk le dit
et renvoie à la page plutôt que de servir un montant nu. Quand plusieurs prix distincts se
présentent sous des colonnes strictement identiques, le discriminant manquant est **repris
du PDF et rattaché par le montant**, non par l'ordre des lignes. À défaut de discriminant
relevé, le poste **n'est pas généré**.

*Amendement OC 1 — le regroupement iso-prix exige l'identité de l'unité.* Deux postes de
même montant mais d'unités différentes ne fusionnent jamais. Ce cas se présente deux fois :
le motif Art Déco MG9, facturé par châssis et quatre angles quand ses homologues de même
montant le sont au volume ; les meneaux battants, à 74 € le mètre linéaire, contre la
plus-value de fixation à 74 € forfaitaires.

*Amendement OC 2 — sémantique du zéro.* Un montant nul est généré avec mention explicite
d'absence de plus-value lorsqu'il correspond à une valeur nulle imprimée au tarif. Il n'est
pas généré lorsqu'il correspond à une cellule vide du PDF, et le chunk voisin énonce alors
l'absence de tarification. Il n'est pas traité comme un poste lorsqu'il n'est qu'un
remplissage sur une ligne de grille ou une ligne séparatrice. Lorsque sa lecture est
ambiguë, il n'est pas généré et l'absence est exposée.

*Amendement OC 3 — maille des croisillons.* Pour les croisillons, la maille est la
**finition**, et le chunk porte dans la même phrase le prix du croisillon en T ou croix et
celui du croisillon filant. Même maille pour les gravures Art Déco au champ et les
croisillons rapportés. Pour les motifs Art Déco au volume, la maille est le **motif**.

*Amendement OV — postes du PDF absents de l'Excel.* Les onze blocs tarifés à zéro par le
tarif et absents du fichier sont générés depuis le PDF, en information originale, avec la
mention explicite d'absence de plus-value. Un poste dont le prix est conditionnel — la
poignée Toulon — donne **un seul chunk portant les deux conditions**, jamais deux chunks qui
se contrediraient à la lecture.

### Règle T5 — Châssis spéciaux (redéfinie sur TA76 OC, maintenue)
Il n'existe aucune plus-value croisée forme × type d'ouvrant : les formes spéciales sont
réservées au châssis fixe. La règle s'applique à la tarification des pages 65 et 66 : un
chunk par poste, anti-fantôme strict, et **lorsqu'une valeur existe sans sa contrepartie, le
chunk transcrit ce qui existe et expose l'absence** plutôt que de la combler ou de supprimer
le prix.

### Règle T6 — Faisabilités et restrictions
Un chunk par restriction, en prose, sans aucun montant, énonçant ce qui est possible et ce
qui ne l'est pas, avec renvoi à la page du tarif. Les abaques de limites de fabrication du
battant par épaisseur de vitrage (pages 54 et 55) ne sont pas transcrites : le chunk énonce
leur existence, leur principe de lecture et renvoie aux pages. Les configurations « en cours
d'étude » de la page 8 sont énoncées comme non faisables et non tarifées.

### Règle T7 — Transverses
Existence, nature et logique tarifaire, renvoi à la page. Aucun prix, aucun pourcentage,
aucune valeur calculable — cohérence avec la règle T3.

### Règle OC1 — sans objet sur cette gamme
Le tarif TA76 OV ne renvoie à aucune autre gamme, ni pour les châssis fixes spéciaux, ni
pour le SB-CC, ni pour aucune configuration. Aucun chunk ne doit énoncer qu'un poste se
saisit ailleurs : ce serait une invention, l'information n'existant que dans le tarif de la
gamme jumelle. La règle est consignée ici pour mémoire, afin qu'une reprise ultérieure ne
la réintroduise pas par symétrie apparente.

### Règle OC2 — Plus-value en pourcentage adossée aux grilles
La plus-value de 3 % sur dormants rénovation figure sur les quatre pages de grilles. C'est
un pourcentage, donc elle relève de T7 et n'est pas transcrite. Elle donne **un chunk
d'orientation unique** en F6, et n'est pas répétée dans les trois cents chunks de prix, où
elle alourdirait chaque corps sans servir la récupération.

### Règle OC3 — Hiérarchie des sources (amendée)
Le tarif énonce sa propre hiérarchie page 2 : « Les logiciels Look et Syscon étant en
constante évolution, cette version PDF reste le seul document de référence. » Il en découle
que **les montants viennent de l'Excel, les libellés et les références viennent du PDF**.
Dès qu'un libellé ou une référence diverge, le PDF fait foi, et la divergence est consignée.
Lorsque deux pages du PDF portent des montants différents pour un même produit, aucune n'est
retenue contre l'autre : les deux chunks sont générés, chacun sourcé à sa page, et chacun
signale l'existence de l'autre montant.

*Amendement OV — divergence de rattachement.* Lorsque le PDF et l'Excel divergent non sur la
valeur mais sur le **rattachement** d'un montant à une référence, **l'Excel fait foi pour le
montant**, le PDF fait foi pour le libellé et la référence, et le chunk expose les deux
valeurs avec attribution par source. Cet amendement résout le décalage de la page 41 et
rend le corpus TA76 OC conforme sur le même tableau. Il n'autorise en revanche aucune
extension : un poste dont le montant est absent de l'Excel n'est pas inventé, et un poste
dont le discriminant est absent des deux sources n'est pas généré.

### Règle OV1 — Discrimination bidirectionnelle avec TA76 OC
Le corpus TA76 OC est déjà déployé et les deux tarifs sont jumeaux page pour page. Douze
postes de châssis spéciaux portent, dans les deux corpus, les mêmes montants, les mêmes
pages et des libellés quasi identiques. Trois obligations en découlent.

D'abord, aucun titre du corpus TA76 OV ne contient « TA76 OC », et tout titre porte le code
gamme complet et la mention « à ouvrant visible ». Ensuite, l'audit relit **les six fichiers
TA76 OC en même temps que les six fichiers TA76 OV** et vérifie qu'aucun titre n'est
identique ni quasi identique d'un corpus à l'autre — la comparaison se faisant sur le titre
privé de son code gamme, seule manière de détecter une collision réelle. Enfin, le contrôle
est **bidirectionnel** : il échoue aussi bien si un chunk OV peut être servi à la place d'un
chunk OC que l'inverse.

### Règle OV2 — Double édition et second témoin TTC
Reprend la règle F8 de FT84, la seule gamme déjà migrée à disposer de deux éditions.

Le tarif existe en deux éditions de même date et de pagination identique, hors taxes et
toutes taxes comprises. La **pagination identique des deux éditions est un fait vérifié**,
non une hypothèse : soixante-dix pages de part et d'autre, en-têtes concordants. La ligne
de source cite l'édition **où l'information figure réellement** — l'édition hors taxes par
défaut, l'édition toutes taxes comprises pour ce qui n'existe que là. Le front matter porte
les deux noms de fichier.

Conséquence de contrôle : la limite assumée sur TA76 OC — aucun second témoin pour les
valeurs TTC — tombe. L'audit croise les 1 745 valeurs TTC de grille et les montants TTC
scalaires contre le PDF TTC, exactement comme les valeurs HT contre le PDF HT.

## 5. Format des chunks

Hérité de la note générale. Spécialisations TA76 OV :

```
## TA76 OV Fenêtre aluminium à ouvrant visible — Tarif du châssis à 1 ouvrant à la française, hauteur de 701 à 800 mm, toutes largeurs tarifées
## TA76 OV Fenêtre aluminium à ouvrant visible — Tarif du châssis à 1 vantail grande hauteur, hauteur de 2701 à 2800 mm, toutes largeurs tarifées
## TA76 OV Fenêtre aluminium à ouvrant visible — Plus-value du vitrage TRY'ver 6-8G-6-10G-6
## TA76 OV Fenêtre aluminium à ouvrant visible — Tarif du croisillon intégré en alu laqué RAL de 18 mm
## TA76 OV Fenêtre aluminium à ouvrant visible — Restrictions de ferrage sur seuil et SB-CC
## TA76 OV Fenêtre aluminium à ouvrant visible — Existence et localisation de l'offre couleurs
```

Le titre de prix porte le libellé du tarif **et** le synonyme d'usage : un ADV demande une
fenêtre à un vantail, pas un ouvrant à la française. Puisque la récupération repose sur la
partie non numérique du chunk, l'enrichissement lexical est le mécanisme de récupération
lui-même.

Ligne de source : `*Source : Tarif—TA76_OV—HT—19-06-2026.pdf, page N — information
originale|complémentaire — SCxxxx*`, em-dashes dans le nom affiché, underscores dans le
champ YAML `document_source`.

Préférer les tournures impersonnelles — *le tarif chiffre X à N €* — qui suppriment les
fautes d'accord à la racine.

## 6. Décomptes prévisionnels

| Fichier | Chunks | Assiette |
|---|---|---|
| Tarif_TA76_OV_METHODE.md | ~9 | PDF p. 2, 6, 7, 9, 47, 57 |
| Tarif_TA76_OV_PRIX_CHASSIS.md | ~330 | 1 745 couples HT/TTC, 14 grilles |
| Tarif_TA76_OV_OPTIONS.md | ~215 | 235 lignes, groupes iso-prix et iso-unité, plus 11 postes à 0 € issus du PDF |
| Tarif_TA76_OV_CHASSIS_SPECIAUX.md | 12 | PDF p. 65, 66 |
| Tarif_TA76_OV_FAISABILITES.md | ~32 | PDF p. 8, 10, 11, 22, 23, 25, 27 à 31, 36, 37, 40, 42, 43, 45 à 55, 64 |
| Tarif_TA76_OV_TRANSVERSES.md | 8 | PDF p. 10-13, 15 à 20, 24 à 27, 30, 65, 69 |
| **Total** | **~606** | |

Détail prévisionnel de F2, issu d'un empaquetage simulé sous plafond et calibré sur les
tailles réelles du corpus TA76 OC : environ 42 chunks pour le 1 ouvrant à la française et 3
pour sa variante grande hauteur, 61 et 3 pour le 2 ouvrants égaux, 81 pour les châssis
fixes, 18 pour le soufflet normal, 10 pour le soufflet normal à poignée latérale, 28 pour le
soufflet d'aération, 26 et 30 pour les deux habillages, 30 pour les quatre grilles belges.
Ce chiffre est une prévision à une dizaine près : l'empaquetage réel est piloté par le
plafond de mots calculé chunk par chunk, et c'est le générateur qui l'arrête.

À comparer aux 564 chunks de TA76 OC. L'écart tient pour l'essentiel aux 86 cellules de
grille supplémentaires et aux deux variantes grande hauteur.

## 7. Industrialisation

**Générateur** (`generateur_tarif_TA76_OV.py`). Lit la feuille unique `Feuil2`, produit les
six fichiers. Fonctions propres à la gamme : calcul des bandes depuis l'échelle du tableau,
empaquetage glouton sous le plafond, variante de grille à un seul axe, variante grande
hauteur, détection du bourrage au-delà de la dernière largeur tarifée, table des unités de
facturation avec clé à trois niveaux, table des discriminants rattachés par le montant,
maille croisillons par finition, table des divergences de rattachement, table des postes du
PDF absents de l'Excel, table des exclusions arbitrées, journal des exclusions, des écarts
et des colonnes non mappées.

**Audit** (`controle_conformite_TA76_OV.py`). Autonome : relit les `.md` sans réutiliser
aucune fonction ni aucune table du générateur et **re-dérive les règles depuis la présente
note**. Contrôles conservés :

- couverture exhaustive des cellules — chaque valeur de l'Excel dans un chunk et un seul ;
- fidélité numérique exhaustive contre la cellule ;
- bornes de bandes recalculées indépendamment ;
- bijection des montants et décompte des postes re-dérivé des mailles de la note ;
- déclaration d'une unité de facturation sur tout poste chiffré ;
- unicité des titres dans chaque fichier ;
- absence de tout montant dans les faisabilités et les transverses ;
- vocabulaire et coquilles de l'Excel non propagées ;
- conformité de la pagination imprimée à l'index PDF, en tolérant l'absence de numéro
  imprimé sur les pages 1, 5, 24 et 70 ;
- croisement PDF page par page, qui valide au passage la table des pages.

Contrôles nouveaux :

- **collision inter-gammes bidirectionnelle** (règle OV1), sur les douze fichiers ;
- **fidélité TTC contre l'édition TTC** (règle OV2) ;
- **exposition des divergences de rattachement** : tout chunk concerné par le décalage de la
  page 41 doit porter la valeur du PDF avec attribution.

## 8. Reste à traiter

- **Asymétrie de périmètre avec TA76 OC sur `HF2245_CE`** : le poste est généré ici, sous
  l'arbitrage désignant l'Excel comme source du montant, alors qu'il avait été écarté du
  corpus TA76 OC faute de source. Les deux gammes jumelles n'ont donc pas le même périmètre
  sur ce point. À trancher dans un sens ou dans l'autre.
- **Message au service produit** (`Message_service_produit_TA76_OV.md`), regroupant les
  points ci-dessous appelant une correction à la source, sur le modèle de HA76, HAM76 et
  FT84.
- **Trois montants exclus** : les deux valeurs `I45` à 49 € et 38 €, et « Judas optique » à
  47 €. Remontée au service documentation, avec le constat que le même défaut existait sur
  TA76 OC, donc qu'il est dans la base et non dans une gamme.
- **Un arbitrage produit en attente** : la lecture du motif Art Déco MG9, dont deux des trois
  types de gravure portent un zéro ambigu.
- **Trente et une lignes mal étiquetées en gamme** dans l'Excel, à faire corriger à la
  source.
- **Décalage de la page 41 dans l'Excel**, à faire corriger à la source : cinq montants mal
  rattachés et une référence surnuméraire.
- **Numéro imprimé absent de la page 24** du PDF, à signaler au service documentation.
- **Poster de profilés étiqueté « TA76OC »** dans la couche de dessin de la page 31 du tarif
  TA76 OV, à signaler également.
- **Instructions.md** : inscrire la paire TA76 OC / TA76 OV parmi les paires à non-import
  explicite, dans les deux sens, et confirmer que la directive sur le terme « crémone » est
  contextuelle et ne s'applique pas aux gammes aluminium.
- **Coquilles de l'Excel** non corrigées automatiquement, à soumettre à relecture :
  « fillant », « TA76OC », `CAL-NRE`, « SN avec  poignée », « 28, 36, 49 ».
