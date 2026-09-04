---
document: note_cadrage_migration_tarif_T81
version: "1.1"
date_redaction: 2026-09-01
statut: point_de_situation
perimetre: migration du document Tarif T81 vers chunks Markdown indexables par le RAG Wikit
gamme: T81
gamme_nom: "Fenêtre PVC"
note_parente: note_cadrage_migration_tarif_v1.md
gamme_pilote_heritee: H81
source_primaire: "T81_-infos-tarifs.xlsx (536 lignes, 95 colonnes, feuille unique)"
source_controle: "Tarif_T81_HT_09-07-2026.pdf (85 pages) et Tarif_T81_TTC_09-07-2026.pdf"
livrables: [generateur_tarif_T81.py, controle_conformite_T81.py, Tarif_T81_METHODE.md, Tarif_T81_PRIX_CHASSIS.md, Tarif_T81_OPTIONS.md, Tarif_T81_CHASSIS_SPECIAUX.md, Tarif_T81_FAISABILITES.md, Tarif_T81_TRANSVERSES.md]
---

# Note de cadrage — Migration du tarif T81 vers Markdown

## 0. Objet et rapport à la note H81

Ce document est une **déclinaison spécifique à la gamme T81** de la note de cadrage
tarif établie sur H81. Il en hérite les principes généraux — auto-discrimination des
chunks, titre auto-porteur préfixé du code gamme, ligne de source par chunk, plafond de
200 mots, prose sans puces, numérotation SC continue depuis SC0002, fidélité numérique
non négociable, anti-fantôme, primauté du PDF sur l'Excel telle qu'établie en section 7 ter
de la note maître, signalement des arbitrages plutôt que leur résolution
silencieuse — mais il **remplace son architecture et ses sept règles**.

Motif : la re-vérification des faits fondateurs, prescrite par la note H81 avant toute
génération, a montré qu'aucun d'entre eux ne tient sur T81. Ce n'est pas un ajustement
de paramètres, c'est un autre objet tarifaire.

## 1. Ce que T81 change par rapport à H81

**Le produit.** T81 est la fenêtre et porte-fenêtre PVC de la collection TRYBA PVC,
conformément au FIP, au CABP et à la Fiche Excellence déjà migrés. Ce n'est ni une porte
d'entrée ni une menuiserie aluminium.

**Le prix varie avec la dimension.** Là où les 84 références H81 avaient un tarif
forfaitaire, T81 tarife sur des grilles hauteur × largeur : **1 825 cellules de prix HT**,
autant de TTC, réparties en neuf grilles plus celle de l'habillage. La dimension n'est
plus une caractéristique, elle est la clé du prix.

**Il n'y a pas de modèle.** Aucune colonne modèle, ligne ou collection. L'identification
d'un prix est le quadruplet chapitre, tableau, hauteur, largeur. L'unité tarifaire de H81
disparaît, et avec elle le liant inter-fichiers fondé sur le nom du modèle.

**Le fait qui refonde tout : une cellule est un intervalle.** La page 10 du tarif énonce
la règle de lecture : la colonne intitulée 1000 couvre les largeurs de 901 à 1000 mm, la
ligne intitulée 600 couvre les hauteurs de 501 à 600 mm. Un prix de grille n'appartient
pas à un point dimensionnel mais à une bande. C'est ce fait qui rend l'architecture
tenable : le chunk énonce la bande en toutes lettres, la demande de l'ADV tombe dedans, et
**le modèle lit au lieu de calculer**. Sans lui, toute demande hors cote ronde aurait exigé
un arrondi, c'est-à-dire un raisonnement numérique proscrit.

**Deux cotes coexistent.** Le tarif distingue les cotes de tarif LT × HT, qui servent au
chiffrage, des cotes de fabrication L × H, qui servent à la commande. Un prix lu sur la
mauvaise cote est un prix faux. Cette distinction n'avait aucun équivalent H81 et devient
un chunk de premier rang.

**Le problème de récupération est numérique, pas volumétrique.** Sur H81, le prix était
accroché à un nom — Azurite, Garissa — et les noms s'encodent bien. Sur T81, la coordonnée
d'un prix est un couple de nombres, et deux chunks qui ne diffèrent que par « 1400 » contre
« 1500 » sont quasi identiques pour un moteur sémantique. Deux principes en découlent, qui
commandent la maille : la discrimination doit reposer sur la **partie non numérique** du
chunk — type d'ouverture, synonymes, vocabulaire métier — et la maille doit être la **plus
grosse unité tenant sous 200 mots**, chaque subdivision créant un frère de plus à départager.

## 2. Faits vérifiés sur T81

Vérifications menées sur l'Excel et croisées au PDF, avant toute génération.

**Configurations.** Il n'y a pas un axe « nombre de vantaux » à deux modalités mais un axe
type d'ouverture à neuf grilles : 1 ouvrant à la française et sa variante grande hauteur,
2 ouvrants égaux et sa variante grande hauteur, châssis fixe, soufflet normal, soufflet
normal à poignée latérale, soufflet d'aération, coulissant à translation. Le vocabulaire
« 1 vantail / 2 vantaux » ne subsiste que comme modalité de plus-value, sur les chapitres
Angles et Cintres.

**Grilles.** Elles ne sont pas rectangulaires : chaque hauteur a sa propre plage de
largeurs tarifées. Le contour de la grille est la limite de fabrication — le tarif l'écrit
en toutes lettres page 48, les grilles de prix « pourront de ce fait être utilisées comme
abaques ». Aucun désalignement HT/TTC n'a été relevé sur les 1 825 cellules.

**Structure des colonnes.** 95 colonnes : identification, HT et TTC scalaires, plus-values
de cintre par type d'ouvrant, plus-values par forme, hauteur, puis 33 + 33 colonnes de prix
par largeur. Quatre colonnes — Px en T et Px filant, HT et TTC — sont **entièrement vides**.
Aucune colonne Ud, vitrage, plage dimensionnelle ni faisabilité.

**Deux fichiers H81 perdent leur source.** Les caractéristiques communes (Ud, vitrage de
base, plage) n'existent pas dans l'Excel T81. La compatibilité équipements non plus : il n'y
a pas de colonne faisabilité, et la chatière comme le judas sont ici **chiffrés**, donc
options. Leur fonction ne disparaît pas pour autant : elle est reprise par le fichier
faisabilités, alimenté par le PDF.

**Trois fois, l'Excel n'est pas auto-suffisant.** C'est le constat structurant de cette
gamme, et il appelle une vigilance permanente.
1. *L'habillage PVC sur vitrage* porte deux lignes par hauteur, sans colonne pour les
   distinguer. La légende de la page 78 les identifie : habillage cintré et habillage
   rectangulaire. Le discriminant n'existe que dans le PDF et n'est récupérable que par
   l'ordre des lignes.
2. *L'unité de facturation* — au mètre linéaire, au mètre carré, à la pièce, au champ, à
   l'ensemble, au battant, à l'angle — figure dans le PDF et **jamais** dans l'Excel. Or
   17 € au mètre linéaire n'est pas 17 €.
3. *Les croisillons en alu laqué* portent jusqu'à trois prix différents sous des colonnes
   strictement identiques : la finition qui les distingue (laqué blanc, laqué RAL, ton bois)
   n'existe que dans la colonne Désignation du tableau du PDF, page 31.

**Trous et exclusions.** Cinq lignes vides en fin de tableau des vitrages ; la pièce d'appui
5304 en plaxage sans prix ; le cintre oblong sans valeur TTC, seule incohérence HT/TTC du
fichier ; l'œil-de-bœuf et l'oblong sans plus-value 2 vantaux ; deux lignes attribuées aux
gammes T82 et T83 ; onze lignes d'exemples de calcul.

## 3. Architecture en six fichiers

Le principe H81 est conservé — un fichier par nature d'information, chaque nature ayant sa
propre logique de variation. Ce sont les natures qui changent.

**F1 MÉTHODE ET COTES.** Cotes de tarif contre cotes de fabrication, passage de l'une à
l'autre selon le dormant, lecture des grilles par bandes, valeur d'abaque des grilles,
composition d'un prix, vocabulaire et abréviations. Nouveau par rapport à H81, et clé de
voûte : sans lui, tous les prix de F2 sont exploitables de travers.

**F2 PRIX DES CHÂSSIS.** Les neuf grilles dimensionnelles. Maille : une bande de hauteur,
découpée en tranches de bandes de largeur dictées par le plafond.

**F3 OPTIONS ET PLUS-VALUES.** Les postes forfaitaires, regroupés strictement iso-prix.

**F4 CHÂSSIS SPÉCIAUX.** Cintres, formes sur accessoires, angles, CVR, croisillons de
châssis spéciaux, habillage PVC. Maille distincte de F3 : plus-value croisée forme × type
d'ouvrant, ou grille dimensionnelle pour l'habillage.

**F5 FAISABILITÉS ET RESTRICTIONS.** Ce qui se combine avec quoi, sans aucun montant.
Successeur fonctionnel du fichier compatibilité équipements de H81.

**F6 TRANSVERSES.** Offre couleurs, teintes des accessoires, joints, laquage bloc-baie :
existence et localisation, aucun montant, aucun pourcentage.

**Le liant inter-fichiers change de nature.** Il n'est plus le nom du modèle mais le **type
d'ouverture**, qui traverse F2, F4 et F5, et doit y figurer à l'identique, libellé complet
et code.

## 4. Les règles normatives T1 à T7

### Règle T1 — Découpage des prix de grille
Un chunk par grille et par bande de hauteur, énumérant les bandes de largeur. Quand la
ligne ne tient pas sous 200 mots, elle est scindée en tranches contiguës de largeurs, la
tranche figurant au titre. **La coupure est pilotée par le plafond de mots, calculé chunk
par chunk, jamais par une constante.** Anti-fantôme : une cellule n'existant pas au tarif ne
donne pas de chunk.

### Règle T2 — Rédaction des prix
Une phrase d'introduction nommant la grille, le type d'ouverture, son synonyme d'usage et la
bande de hauteur en clair ; puis une énumération en prose où chaque bande de largeur est liée
dans la même proposition à son prix HT et à son prix TTC. **Les bandes sont énoncées comme
intervalles** — « de 901 à 1000 mm » — jamais comme points. La première bande, dont le
plancher n'est pas donné par le tarif, s'écrit « jusqu'à N mm » : aucun plancher n'est inventé.

### Règle T3 — Instruction LLM
Le modèle réclame trois éléments avant tout prix de châssis : le type d'ouverture, la cote
tarif en largeur, la cote tarif en hauteur. Il vérifie que la cote fournie est bien une cote
de tarif et non une cote de fabrication. Il ne calcule aucun total, n'additionne aucun
composant, n'interpole ni n'extrapole. Si la dimension demandée sort de la grille, il indique
que la configuration n'est pas tarifée plutôt que d'approcher.

### Règle T4 — Options et plus-values forfaitaires
Un chunk par poste, regroupement strictement iso-prix : mêmes chapitre, tableau, désignation,
HT et TTC. Seules les variantes de la colonne Détails fusionnent, et le chunk les nomme
toutes. **Tout poste chiffré déclare son unité de facturation**, relevée page par page dans le PDF —
au mètre linéaire, au mètre carré, à la pièce, au champ, à l'ensemble, au battant, au montant,
à l'angle, à la paumelle, au volet, au châssis, au sachet, ou forfaitaire. L'unité n'existe
nulle part dans l'Excel. **Le montant servi est unitaire** : le chunk énonce explicitement que
le total s'obtient en multipliant par la quantité concernée, et que ce calcul revient à l'ADV.
Cette formulation est la contrepartie de la règle T3 — le modèle restitue un prix unitaire
lisible tel quel, il ne multiplie pas. Quand l'unité n'a pas pu être établie contre le PDF, le
chunk le dit et renvoie à la page plutôt que de servir un montant nu. Les montants à 0 € sont générés
avec la mention explicite d'absence de plus-value — inversion assumée par rapport à H81, où le
zéro signalait un renvoi transverse à exclure. Quand plusieurs prix distincts se présentent sous des colonnes strictement identiques, le
discriminant manquant est **repris de la colonne Désignation du tableau du PDF et rattaché par
le montant**, non par l'ordre des lignes : un rattachement par le montant se vérifie, un
rattachement par l'ordre ne se vérifie pas. À défaut de discriminant relevé, le poste **n'est
pas généré** : deux prix différents ne peuvent pas porter le même titre, et aucun discriminant
n'est inventé.

### Règle T5 — Plus-values croisées et châssis spéciaux
Un chunk par forme, énonçant ses plus-values pour chaque type d'ouvrant tarifé. Anti-fantôme
strict sur les combinaisons absentes. Lorsqu'une valeur HT existe sans TTC, le chunk transcrit
le HT et **expose l'absence** plutôt que de la combler ou de supprimer le prix.

### Règle T6 — Faisabilités et restrictions
Un chunk par restriction, en prose, sans aucun montant, énonçant ce qui est possible et ce qui
ne l'est pas, avec renvoi à la page du tarif.

### Règle T7 — Transverses
Existence, nature et logique tarifaire, renvoi à la page. Aucun prix, aucun pourcentage,
aucune valeur calculable — cohérence avec la règle T3.

## 5. Format des chunks

Hérité de la note générale. Spécialisations T81 :

```
## T81 Fenêtre PVC — Tarif châssis à 1 ouvrant à la française, hauteur de 701 à 800 mm, largeurs jusqu'à 1000 mm
## T81 Fenêtre PVC — Vitrage TRY'ver 6-10G-4-10G-6, plus-value tarif
## T81 Fenêtre PVC — Plus-value de la forme plein cintre selon le type d'ouvrant
## T81 Fenêtre PVC — Compatibilité des options avec les ferrages R20 et RC2
## T81 Fenêtre PVC — Existence et localisation des tarifs de l'offre couleurs
```

Le titre de prix porte le libellé du tarif **et** le synonyme d'usage : un ADV demande une
fenêtre à un vantail, pas un ouvrant à la française. Puisque la récupération repose sur la
partie non numérique du chunk, l'enrichissement lexical n'est pas un confort mais le
mécanisme de récupération lui-même.

**Vocabulaire.** Le terme « crémone » n'est jamais employé seul : seule la locution complète
« crémone à l'ancienne », nom d'une option décorative de la gamme, est admise — règle déjà
inscrite au corpus technique T81. Le libellé tronqué « Crémones » de la colonne chapitre de
l'Excel n'est pas transcrit tel quel ; c'est le titre du tarif, page 38, qui fait foi. La
dérogation porte sur un libellé de structure, jamais sur une valeur.

## 6. Décomptes

| Fichier | Chunks | Assiette |
|---|---|---|
| Tarif_T81_METHODE.md | 6 | PDF p. 3, 6, 8, 10, 48 |
| Tarif_T81_PRIX_CHASSIS.md | 276 | 1 583 cellules de grille |
| Tarif_T81_OPTIONS.md | 271 | groupes iso-prix forfaitaires |
| Tarif_T81_CHASSIS_SPECIAUX.md | 86 | 12 plus-values croisées, 30 postes, 44 chunks d'habillage (242 cellules) |
| Tarif_T81_FAISABILITES.md | 3 | PDF p. 38, 46 |
| Tarif_T81_TRANSVERSES.md | 4 | PDF p. 17, 19, 20, 21 |
| **Total** | **646** | |

Les 1 825 cellules de prix se ramènent à 320 chunks sans qu'aucune grille n'entre dans un
chunk. À comparer aux 340 chunks de H81 pour la gamme entière.

## 7. Industrialisation

**Générateur** (`generateur_tarif_T81.py`). Lit la feuille unique, produit les six fichiers.
Fonctions propres à T81 : calcul des bandes depuis l'échelle du tableau, empaquetage glouton
sous le plafond, détection de non-contiguïté des largeurs, table des unités de facturation,
libellés auto-discriminants avec suffixe de variante en cas de collision, exclusion des postes
indiscriminables, journal des exclusions et des colonnes non mappées.

**Audit** (`controle_conformite_T81.py`). Autonome : relit les .md sans réutiliser aucune
fonction du générateur et **re-dérive les règles depuis la présente note**. Dix contrôles :
décomptes, forme, couverture exhaustive des cellules, fidélité numérique, bijection des postes,
bandes recalculées, unités déclarées, unicité des titres, absence de montant en F5 et F6,
vocabulaire, croisement PDF page par page. Le contrôle 9 a été refondu en septembre 2026
(voir section 9) : lecture au rendu et croisement du couple référence + montant. Le script
dépend désormais de `pymupdf` en plus d'`openpyxl`.

Contrôle nouveau par rapport à H81 : la **couverture exhaustive des cellules**, qui vérifie que
chacune des 1 825 valeurs se retrouve dans un chunk et un seul. Le découpage par tranches de
largeurs crée en effet un risque de perte comme de doublon que H81 n'avait pas.

**Résultat** : 19 contrôles réussis, 0 échec, 2 avertissements. Cinq défauts ont été détectés
et corrigés au cours des passes successives : dépassement du plafond d'un mot
(le marqueur de titre n'était pas compté dans l'empaquetage), quarante-quatre titres non
discriminants, deux attributions de page erronées, l'absence d'unité de facturation sur les
plus-values croisées, et une unité erronée sur la tarification des angles — relevée « par
angle » alors que le tarif la donne par châssis, vitrage non compris.

## 8. Reste à traiter

- **Trois unités de facturation non établies** : les seuils APE-70, PE725 et KP484RCY, dont les
  références sont absentes de la couche texte de la page 66. Ces trois chunks renvoient à la page.
- **Instructions.md** : encoder que le modèle restitue un prix unitaire avec son unité et
  n'effectue jamais la multiplication, l'addition des postes revenant à l'ADV.
- **Lignes T82 et T83** : exclues et consignées, arbitrage en attente auprès du service
  documentation.
- **Deux montants non retrouvés sur la page citée** (pages 47 et 75), dont le forfait de coupe
  CVR, absent de la couche texte de sa page.
- **Coquilles de libellé de l'Excel** (« Laiton veilli », « menau battant ») : non corrigées
  automatiquement, à soumettre à relecture.
- **Instructions.md** : encoder la règle T3 dans sa forme T81 (type d'ouverture, cote tarif en
  largeur et en hauteur, interdiction d'additionner) et vérifier que la directive sur le terme
  « crémone » est bien contextuelle et non une interdiction sèche.
- **Enrichissement de F1, F5 et F6**, aujourd'hui volontairement limités aux blocs vérifiés
  page à page.

## 9. Rejeu du contrôle 9 sur extraction au rendu — septembre 2026

**Motif.** Une remontée terrain sur la pièce d'appui `5180`, servie comme étant à l'offre
alors que le tarif ne la porte plus qu'en compatibilité d'embout, a conduit à réexaminer la
lecture du PDF. Le tarif T81 contient une couche non rendue — planches techniques placées
dans des cadres qui les rognent — représentant **2 320 jetons sur 23 018**, soit 10,1 % du
texte extrait par `pdftotext`, répartis sur 25 des 85 pages. Générateur et audit lisaient
tous deux le PDF avec `pdftotext` : ils partageaient donc le même angle mort, ce qui vidait
de sa substance la règle d'audit autonome.

**Ce qui a changé dans le contrôle.** L'extraction passe au rendu (`pymupdf`). Le croisement
ne porte plus sur le seul montant mais sur le **couple référence + montant, sur la page
citée**. Les libellés énumérant plusieurs références sont décomposés et chaque référence est
contrôlée séparément. Script : `controle9_pdf_visible_T81.py`.

**Résultat du rejeu** — 291 postes testés, 198 porteurs d'une référence, 302 références
confirmées sur leur page.

| Constat | Nombre | Statut |
|---|---|---|
| Montant absent de la page citée | 5 | à instruire |
| Référence visible, mais sur une autre page | 13 | note de source à corriger |
| Référence présente uniquement dans la couche rognée | 3 | chunk fantôme, suppression |
| Référence absente du PDF | 10 | hors tarif ou nomenclature Excel |

**Chunks fantômes.** `SC0262` et `SC0263` (accouplement statique `NR7`) et `SC0272`
(seuil `KP484RCY`). Ces trois références n'existent que sur les planches rognées des pages 61
et 63. Elles ne sont à l'offre nulle part dans le tarif publié.

**Nomenclature Excel servie à la place de la nomenclature tarif.** `SC0266`, `SC0267`,
`SC0268` servent `AK10123`, `AK10123-RAS1`, `AK10123-RAS2` là où la page 66 porte
`AS10100`, `AS10100-RA1` et `AS10100-RA2`, aux mêmes prix — 188, 207 et 226 € — et à la même
désignation. À noter en outre que `SC0267` rattache la désignation « bouclier de protection
pour PF » au seuil à 207 €, alors que le bouclier est la référence `5263` à 12 €/ml : la
colonne désignation de l'Excel est décalée d'une ligne sur ce bloc.

**Postes de pièces d'appui.** `SC0222` et `SC0223` agrègent sous un prix commun des
références obsolètes et leurs remplaçantes — `5180`, `5181`, `5182` d'une part, `5415` et
`5416` d'autre part. Seules `5415` et `5416` sont tarifées, page 61 et non page 60. La
référence `5182` n'apparaît nulle part dans le PDF, pas même dans la couche rognée. Les
chunks `SC0216` à `SC0219` et `SC0243` portent des erreurs d'attribution de page de même
nature.

**Reclassement de trois points de la section 8.** Les seuils `APE-70`, `PE725` et
`KP484RCY` n'étaient pas des « unités de facturation non établies » : ce sont des postes sans
existence dans le tarif publié. Ils relèvent de la règle anti-fantôme, non du renvoi à la
page.

**Intégration.** Le contrôle couple référence + montant remplace le contrôle 9 de
`controle_conformite_T81.py` depuis septembre 2026. Le script lit désormais le PDF au rendu
(`pymupdf`) ; `pdftotext` n'y subsiste que pour qualifier une référence absente du rendu —
planche rognée ou absence pure — et pour mesurer l'ampleur de la couche non rendue, reportée
en avertissement dans le rapport d'audit. Le script autonome `controle9_pdf_visible_T81.py`
qui a servi au rejeu n'a plus lieu d'être maintenu.

**Reste à conduire.** Basculer `generateur_tarif_T81.py` sur l'extraction au rendu, traiter
les 31 anomalies remontées, puis rejouer le contrôle sur les neuf autres gammes dont les
tarifs ont été migrés sous la même méthode.
