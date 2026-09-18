---
document: note_cadrage_migration_tarif_FT84
version: "1.0"
date_redaction: 2026-09-01
statut: point_de_situation
perimetre: migration du document Tarif FT84 vers chunks Markdown indexables par le RAG Wikit
gamme: FT84
gamme_nom: "Fenêtre de toit PVC"
gammes_couvertes: [FT84]
collection: "TRYBA PVC"
note_parente: note_cadrage_migration_tarif_H81_v1.md
gammes_pilotes_heritees: [T81, CA76]
source_primaire: "FT84_-infos-tarifs.xlsx (34 lignes de données, 37 colonnes, feuille unique Feuil1)"
source_controle: "Tarif_FT84_HT_28-05-2026.pdf et Tarif_FT84_TTC_28-05-2026.pdf (24 pages chacun)"
livrables: [generateur_tarif_FT84.py, controle_conformite_FT84.py, Tarif_FT84_METHODE.md, Tarif_FT84_PRIX_SUR_MESURE.md, Tarif_FT84_PRIX_STOCK.md, Tarif_FT84_OPTIONS.md, Tarif_FT84_FAISABILITES.md, Tarif_FT84_TRANSVERSES.md, Message_service_produit_FT84.md]
---

# Note de cadrage — Migration du tarif FT84 vers Markdown

## 0. Objet et rapport aux notes H81, T81 et CA76

Cette note est la déclinaison propre à la gamme FT84 de la note de cadrage générale
produite sur H81. Elle hérite des principes communs sans les redémontrer : fidélité
numérique non négociable, anti-fantôme, non-invention, auto-discrimination par le
titre, exposition des divergences avec attribution par source, un fichier par nature
d'information, plafond de 200 mots marqueur compris, prose sans puces, numérotation
SC continue par fichier depuis SC0002, ligne de source normée avec em-dashes dans le
nom affiché et underscores dans le champ YAML.

Les règles F1 à F9 énoncées ici remplacent les règles 1 à 7 d'H81, T1 à T7 de T81 et
C1 à C8 de CA76 partout où FT84 s'en écarte.

## 1. Ce que FT84 change par rapport aux trois gammes précédentes

**Deux formes tarifaires coexistent dans un même document.** C'est le fait qui
commande toute l'architecture, et aucune des trois gammes précédentes ne le présentait.
Le régime **dimensions sur mesure** tarife par grille dimensionnelle à bandes, comme
T81 et CA76. Le régime **dimensions stock** tarife au forfait par **code de dimension
normalisée**, forme absente d'H81 comme de T81 et de CA76. Les deux régimes ne
partagent ni le niveau de prix — prix bruts d'un côté, prix nets sans remise de
l'autre —, ni la sémantique de la dimension — bande contre point —, ni les coloris
admis, ni le montant d'une même option. Ils ne partagent donc aucun chunk et le
régime figure dans tous les titres.

**Les bandes sont écrites dans les en-têtes.** Sur T81 et CA76, la règle de lecture
d'une cellule comme intervalle devait être cherchée dans les généralités. Ici
l'en-tête porte directement « De 460 à 524 », intervalle et bornes comprises. La règle
n'a pas à être supposée, elle est lisible sur la grille elle-même. Le pas n'est pas
constant : 64, 66, 200, 200, 90 et 110 mm en largeur ; 150, 200, 200, 130, 90 et 50 mm
en hauteur. Rien du pas régulier de 100 mm de CA76.

**La première bande porte un plancher.** Les règles T2 et C2 imposaient « jusqu'à N
mm » parce que la source ne donnait pas de plancher à la première bande. Ici les
planchers sont donnés, 460 mm en largeur et 740 mm en hauteur. Ils sont donc
transcrits, ni omis ni inventés.

**Le rattachement d'un discriminant repris du PDF change de mécanisme.** Les règles T4
et C4 imposaient le rattachement par le montant, jamais par l'ordre des lignes. Ce
mécanisme est ici **impossible** : les numéros de modèle stock, absents de l'Excel, ne
peuvent pas être rattachés par leur montant, plusieurs modèles portant le même — 760 €
désigne les modèles 12 et 13, 868 € les modèles 32 et 33. L'ancrage retenu est la
**coordonnée hauteur par largeur**, le code figurant dans la même colonne que le prix,
dans le même tableau. Il est plus fort que l'ordre des lignes et se revérifie par
recalcul, le code se décodant.

**La distinction fondatrice de T81 s'effondre.** Le tarif distingue par la notation les
cotes de tarif des cotes de fabrication, puis énonce page 9 leur égalité. Une seule
dimension est à relever. Le piège de premier rang est ailleurs, écrit en rouge sur la
même page : prendre systématiquement les dimensions des tableaux intérieurs et non
celles du cadre dormant existant. C'est à cet avertissement qu'est donné le rang que
T81 donnait à la distinction des cotes.

**Deux produits partagent le tarif**, comme CA76 et CAG76 : la fenêtre de toit FT84 et
le volet de toit solaire TRYBA VTS, tarifé au niveau du châssis avec sa propre grille
et vendu non monté. Le volet n'est pas une plus-value. Le préfixe de titre reste
cependant simple, le volet n'étant pas une gamme mais un accessoire dédié qui ne se
monte que sur la FT84.

**Le liant inter-fichiers change une quatrième fois** : ni le modèle d'H81, ni le type
d'ouverture de T81, ni le triplet produit-vantaux-rails de CA76, mais le couple
**régime × produit**, augmenté du code modèle dans le régime stock.

## 2. Faits vérifiés sur FT84

Tous les faits ci-dessous ont été établis par la donnée avant que l'architecture ne
soit figée, et non déduits de la ressemblance avec T81 ou CA76.

**Volumétrie.** 140 cellules HT et 140 cellules TTC dans l'Excel, réparties en trois
séries par bande de hauteur : fenêtre FT84, volet TRYBA VTS et valeur vitrage. **Aucun
désalignement HT/TTC**, vérifié cellule par cellule. Après gel de la série valeur
vitrage, le périmètre chiffré compte 102 cellules et 14 postes forfaitaires.

**Régime sur mesure.** Six bandes de hauteur de 740 à 1560 mm, sept bandes de largeur
de 460 à 1310 mm. La grille n'est pas rectangulaire : les deux dernières bandes de
hauteur s'arrêtent à 1080 mm de largeur, en troncature de fin de ligne, sans aucun
trou intérieur. Les bandes 1291-1420 et 1421-1510 portent des prix de fenêtre et de
volet **strictement identiques** mais des largeurs tarifées différentes ; elles ne sont
donc pas fusionnables.

**Régime stock.** Quatre hauteurs et six largeurs ponctuelles, **dix-sept modèles**
seulement. Le code se décode : premier chiffre pour le rang de hauteur, second pour le
rang de largeur. La règle a été vérifiée sur les dix-sept modèles, sans exception, et
elle est recalculée indépendamment par l'audit. Contrairement au régime sur mesure, le
stock présente des **trous intérieurs** : 919 × 1085, 1119 × 1285 et 1339 × 495 ne
portent aucun modèle.

**La colonne scalaire HT/TTC de l'Excel n'est pas le prix du châssis.** C'est le piège
majeur de cette gamme. Les colonnes nommées sobrement `HT` et `TTC` portent les prix de
l'**abergement ardoises**, rattachés à la bande de hauteur. Le libellé, le nombre de
tôles et la distinction brut/net n'existent que dans le PDF, pages 10 et 11, et sont
rattachés par le montant, les deux jeux de valeurs étant disjoints. Un générateur qui
aurait mappé ces colonnes comme prix principal se serait trompé de bout en bout.

**Trois états de cellule, dont aucun zéro.** Il n'y a pas un seul zéro dans ce tarif :
la question de la sémantique du zéro, ouverte sur H81, tranchée en sens inverse sur
T81 et CA76, est ici sans objet. Le vide a deux valeurs. La première est **écrite** :
la mention « Impossible » figure en clair sur la ligne du volet, le tarif donnant son
motif — volet impossible en deçà de 525 mm de largeur. La seconde est le blanc muet :
troncature de fin de ligne en sur mesure, absence de dimension offerte en stock. Ces
deux valeurs ne se confondent pas et ne se servent pas de la même façon.

**Le coefficient de passage HT vers TTC n'est ni la TVA ni une constante.** Il vaut
1,265 à 1,315 sur le châssis sur mesure, 1,289 à 1,323 sur le volet, 1,124 à 1,147 sur
la valeur vitrage, et **2,24 à 2,35 sur le régime stock**, l'écart s'expliquant par le
prix net sans remise. Aucun montant ne se déduit d'un autre. Plus tranché encore que le
coefficient d'environ 1,43 de CA76.

**Pagination saine**, contrairement à T81 et CA76. Le numéro imprimé au pied de page
coïncide avec l'index PDF sur les vingt-quatre pages, le sommaire général est exact, et
les deux éditions hors taxes et toutes taxes comprises ont une pagination identique.
Deux pages ne sont pas indexées au sommaire, la page 17 des plus-values vitrages et la
page 23 des évolutions. Deux pages sont indexées sans porter de pied de page, la
page 5 et la page 22.

**Couche texte propre.** Aucune contamination inter-gammes, contrairement au résidu
« TA76 OC » relevé sur vingt-quatre pages de CA76. Aucun faux synonyme : gond,
charnière, paumelle, survitrage, anti-dégondage et anti-décrochement sont à zéro
occurrence. Les termes « crochets massifs », « gâches » et « ventilation », marqueurs
de contamination FT84 dans les autres gammes, sont ici légitimes et conservés.

**Colonnes non mappées.** La colonne `Détails` est entièrement vide. Les colonnes
`Gamme`, `Mention HT` et `Mention TTC` sont constantes et sans information. Aucune
colonne de largeur n'est vide. Les 34 lignes sont toutes en FT84, sans ligne étrangère.

## 3. Axes anticipés qui ne sont pas des axes de prix

Le **système d'ouverture** n'est ni un axe, ni une option, ni une gamme distincte :
projection et rotation coexistent sur la même fenêtre, positions 1 et 2 de la poignée.
La **motorisation** est un forfait d'option, en projection seule, non installable sur
une fenêtre déjà posée. Le **type de couverture** n'est pas un axe de prix direct, mais
il en touche un par une seule voie : l'ardoise impose l'abergement ardoises, tarifé et
variable avec la bande de hauteur. La **pente de toit** est une pure faisabilité. Le
**dormant de rénovation**, décrit dans la fiche info produit, ne porte aucune ligne au
tarif : cette absence est signalée telle quelle. Le **jumelage** est asymétrique, le
montage horizontal étant tarifé et le montage vertical décrit sans montant.

## 4. Périmètre et arbitrages

**Chapitre gelé.** La série valeur vitrage du régime sur mesure, 38 montants HT et 38
TTC, n'est pas migrée. Motif triple : ces montants ne figurent sur aucune grille du PDF
de référence, vérification faite sur la page rastérisée pour écarter un défaut
d'extraction ; vingt-quatre de leurs vingt-huit valeurs distinctes sont introuvables
dans le PDF entier ; et la gamme ne porte aucune plus-value de vitrage à laquelle cette
base s'appliquerait, les deux compositions des pages 16 et 17 étant marquées standard
et les vitrages décoratifs déclarés indisponibles. L'existence de la valeur vitrage est
signalée sans montant, avec renvoi au service Produits. Précédent : les vitrages
ornementaux de la page 35 sur CA76.

**Pages exclues du périmètre chiffré.** La page 8 porte une grille illustrative dont la
structure de bandes — trois largeurs au lieu de sept — et les valeurs sont périmées et
sans rapport avec la grille réelle de la page 10 ; elle cite en outre une base Isol'3
quand le vitrage de la gamme est un Isol'4. Sa règle de lecture et sa légende du
châssis nu sont conservées, aucun de ses nombres ne l'est. La page 5 est la fiche info
produit d'édition 05-2026, déjà migrée à l'identique. La page 24 est le dos de
couverture.

**Divergences exposées et non arbitrées**, toutes remontées au service Produits par le
message joint : la tuile canal romane déclarée non réalisable page 6 tout en recevant
une pente minimale dans le bloc voisin de la même page ; l'abergement rangé page 8
parmi ce que comprend le prix du châssis nu tout en étant tarifé séparément pages 10 et
11 ; le renvoi de page erroné de la page 8 ; la divergence de date entre les deux
éditions sur la même ligne d'évolution ; l'écart de millésimes entre la fiche info
produit et la fiche excellence sur l'Uw et le Sw.

**Unité de facturation non établie** sur les neuf postes d'abergement ardoises : le
tarif juxtapose un nombre de tôles droite et gauche et un montant, sans dire si le
montant couvre l'ensemble ou la tôle. Les chunks le déclarent et renvoient à la page.

## 5. Architecture en six fichiers

| Fichier | Nature | Règle |
|---|---|---|
| `Tarif_FT84_METHODE.md` | cotes, régimes, lecture par bandes, correspondance des codes modèle, vocabulaire | F3 |
| `Tarif_FT84_PRIX_SUR_MESURE.md` | grilles dimensionnelles à bandes, fenêtre et volet | F1, F2 |
| `Tarif_FT84_PRIX_STOCK.md` | forfaits par code de dimension normalisée | F4 |
| `Tarif_FT84_OPTIONS.md` | plus-values et postes forfaitaires chiffrés en euros | F5 |
| `Tarif_FT84_FAISABILITES.md` | restrictions, impossibilités et divergences, sans montant | F6 |
| `Tarif_FT84_TRANSVERSES.md` | existence et localisation, sans montant | F7 |

## 6. Les règles normatives F1 à F9

### Règle F1 — Découpage des prix de grille, régime sur mesure

La maille est **une bande de hauteur, un produit, une tranche contiguë de bandes de
largeur**. La tranche est la plus grosse qui tienne sous le plafond, la coupure étant
pilotée par le comptage des mots, séparateurs compris, jamais par une constante. Sur
FT84 la maille maximale est atteinte : chaque couple bande de hauteur et produit tient
en un seul chunk. Anti-fantôme : une cellule absente du tarif ne donne pas de chunk.

### Règle F2 — Rédaction des prix de grille

Chaque bande est écrite en toutes lettres, « de 991 à 1080 mm », plancher compris,
pour qu'une cote non ronde tombe dans une bande énoncée sans que le modèle ait à
calculer. Les prix hors taxes et toutes taxes comprises figurent dans la même
proposition. Les montants sont écrits sans séparateur de milliers, comme les grilles du
tarif elles-mêmes. Toute largeur non tarifée est **exposée** en fin de chunk, avec son
motif quand le tarif le donne : mention « Impossible » d'un côté, absence de
tarification de l'autre. Une case vide n'est jamais servie comme une gratuité.

### Règle F3 — Instruction LLM

Le modèle lit, il ne calcule pas. Il réclame trois éléments avant tout prix de châssis :
le régime, sur mesure ou stock ; le produit, fenêtre FT84 ou volet TRYBA VTS ; et les
deux dimensions de fabrication. Il ne convertit aucune cote, n'interpole pas entre deux
bandes, n'additionne aucun poste et ne déduit jamais un montant toutes taxes comprises
d'un montant hors taxes. Si la dimension demandée sort de la grille ou n'est pas offerte
en stock, il l'indique plutôt que d'approcher. Un montant du régime sur mesure ne se
substitue jamais à un montant du régime stock.

### Règle F4 — Prix du régime stock

La maille est **une hauteur et un produit**, énumérant les modèles offerts avec leur
code, leur largeur et leurs deux prix. Le code de modèle est **repris du PDF** et
rattaché par la coordonnée hauteur par largeur, le rattachement par le montant étant
impossible sur cette gamme. Les largeurs non offertes sont énoncées comme telles. La
mention « Impossible » n'est reprise que là où le tarif la porte effectivement,
c'est-à-dire là où une dimension stock existe à cette largeur : ailleurs la case est
vide sans mention, et la reprendre serait une invention.

### Règle F5 — Options et plus-values forfaitaires

Un chunk par poste chiffré. **Tout poste chiffré déclare son unité de facturation** ;
lorsque le tarif ne l'indique pas, le chunk le dit et renvoie à la page plutôt que de
servir un montant nu. Le montant servi est unitaire et le chunk énonce que le total
revient à l'ADV. Un poste dont le montant diffère entre les deux régimes donne deux
chunks distincts, et chacun énonce que sa valeur ne se substitue pas à celle de l'autre
régime.

### Règle F6 — Faisabilités et restrictions

Aucun montant. Les impossibilités produit et les dimensions non offertes y sont
reprises, de sorte qu'une case vide ne puisse jamais être servie comme une gratuité.
Les divergences internes au tarif y sont exposées avec attribution par page, sans
arbitrage. Les absences de tarification, comme le dormant de rénovation ou le jumelage
vertical, y sont signalées telles quelles : ni inclusion, ni impossibilité, ni report
d'un montant voisin.

### Règle F7 — Transverses

Existence, nature et localisation de l'information, renvoi à la page. Aucun montant.
Les taux de TVA et les mentions d'éligibilité y figurent à titre d'information, assortis
du rappel que les montants toutes taxes comprises sont lus au tarif et jamais
recalculés.

### Règle F8 — Double édition

Le tarif existe en deux éditions de même date et de pagination identique, hors taxes et
toutes taxes comprises. La ligne de source cite l'édition **où l'information figure
réellement** : l'édition hors taxes par défaut, l'édition toutes taxes comprises pour
les informations qui n'existent que là, comme les taux de TVA. Le front matter porte les
deux noms de fichier. La pagination identique des deux éditions est un fait vérifié,
non une hypothèse.

### Règle F9 — Vocabulaire

La collocation « verrouillage et ventilation », les « crochets massifs » et les
« gâches fixées dans les armatures » appartiennent au vocabulaire propre de cette
fenêtre et ne sont pas traités comme des marqueurs de contamination. Les faux synonymes
habituels restent proscrits. Les coquilles de libellé de l'Excel — « FR84 » pour FT84,
« Ouverture motoriséé » — sont corrigées en faveur du PDF, qui se déclare document de
référence à sa page 2, et chaque correction est consignée au journal. Aucune correction
ne touche un montant.

## 7. Format des chunks

Titre auto-porteur préfixé de `FT84 Fenêtre de toit PVC — `. Ligne de source normée
`*Source : Tarif—FT84—HT—28-05-2026.pdf, page N — information originale — SCxxxx*`,
avec la variante TTC prévue par la règle F8. Plafond de 200 mots, marqueur `##`
compris. Prose, sans puces. Front matter YAML portant `document_source`,
`document_source_ttc`, `type_document`, `sous_type`, `gamme_code`, `gammes_couvertes`,
`collection`, `version_doc`, `date_validite` et `nb_chunks`.

## 8. Décomptes

| Fichier | Chunks | Assiette |
|---|---|---|
| `Tarif_FT84_METHODE.md` | 10 | PDF p. 6, 8, 9, 10, 11 |
| `Tarif_FT84_PRIX_SUR_MESURE.md` | 12 | 70 cellules de grille |
| `Tarif_FT84_PRIX_STOCK.md` | 8 | 32 cellules, 17 modèles |
| `Tarif_FT84_OPTIONS.md` | 14 | 9 postes d'abergement, 5 forfaits |
| `Tarif_FT84_FAISABILITES.md` | 16 | PDF p. 6, 10, 11, 14, 16, 17, 19, 20, 21 |
| `Tarif_FT84_TRANSVERSES.md` | 11 | PDF p. 8, 16, 17, 22, 23 |
| **Total** | **71** | |

Hors 76 montants de valeur vitrage gelés. À comparer aux 340 chunks d'H81, aux 646 de
T81 et aux 1 019 de CA76.

## 9. Industrialisation

`generateur_tarif_FT84.py` lit l'Excel et produit les six fichiers, en tenant un journal
exhaustif : colonnes non mappées, lignes exclues et leur motif, postes gelés,
discriminants repris du PDF avec leur mécanisme de rattachement, unités de facturation
non établies, divergences exposées et pages exclues.

`controle_conformite_FT84.py` **ne réutilise aucune fonction du générateur**. Il relit
les Markdown, redéclare ses propres tables, re-dérive les bandes depuis les en-têtes de
l'Excel, redécode lui-même les numéros de modèle et re-extrait la couche texte du PDF,
de sorte qu'une divergence entre les deux soit un écart réel et non une tautologie. Il
exécute quinze contrôles : forme et front matter, ligne de source, continuité SC,
plafond, absence de puces, unicité des titres, couverture exhaustive des cellules,
anti-fantôme, fidélité numérique exhaustive hors taxes et toutes taxes comprises, bornes
de bandes recalculées indépendamment, décodage indépendant des codes modèle, bijection
des postes forfaitaires par multiensembles insensibles aux libellés, déclaration d'une
unité sur tout poste chiffré, absence de tout montant en méthode, faisabilités et
transverses, absence des montants gelés, vocabulaire et contamination inter-gammes,
table des pages validée au pied de page, et croisement PDF page par page.

**Résultat** : 15 contrôles réussis, 0 échec, 0 avertissement, sur 71 chunks. Chaque
nombre servi est traçable à une cellule de l'Excel et retrouvé sur la page du PDF qu'il
cite.

Contrôle nouveau par rapport aux gammes précédentes : le **redécodage indépendant des
numéros de modèle**, qui vérifie que chaque code servi correspond bien à la coordonnée
hauteur par largeur de son prix. Il ferme le risque propre au mécanisme de rattachement
retenu par la règle F4.

## 10. Scories relevées à la relecture d'échantillon

Six défauts, invisibles pour l'audit parce qu'ils portent sur la langue ou sur le sens
et non sur les nombres, ont été relevés sur un échantillon de vingt et un chunks
couvrant tous les cas de figure, puis corrigés à la source dans le générateur.

Deux sont des **défauts de fond**. La mention « Impossible » était servie sur la largeur
de 495 mm aux hauteurs de 1339 et 1541 mm, où le tarif ne la porte pas, la case y étant
vide faute de modèle offert : la clause est désormais conditionnée à l'existence d'une
dimension stock à cette largeur. Et les cotes du croquis de la tôle d'abergement,
figurant page 11, étaient citées dans des chunks attribués à la page 10.

Quatre sont des scories de rédaction : une énumération construite avec des « et »
répétés au lieu de virgules ; deux codes contigus énoncés en intervalle, « modèles 43 à
44 » au lieu de « modèles 43 et 44 » ; une préposition flottante entre « la largeur de
1085 mm » et « la largeur 495 mm » ; et une phrase confuse donnant trois cotes de tôle
comme un produit de dimensions.

## 11. Reste à traiter

- **Arbitrage produit sur la valeur vitrage**, qui débloquerait ou clôturerait
  définitivement les 76 montants gelés.
- **Arbitrages secondaires** sur la tuile romane, sur l'abergement inclus et facturé, et
  sur l'unité de facturation de l'abergement ardoises, dont l'exposition actuelle tient
  sans eux.
- **Signalement au service documentation** du renvoi de page erroné de la page 8, de la
  grille illustrative périmée de la même page, de la divergence de date entre éditions
  page 23, et du dos de couverture emprunté à un tarif Portes de garage de 2018. Ces
  points font l'objet du message joint.
- **Instructions.md** : encoder la règle F3 dans sa forme FT84, c'est-à-dire l'exigence
  du régime avant toute réponse de prix, l'interdiction de substituer un régime à
  l'autre, et l'interdiction de dériver un montant toutes taxes comprises d'un montant
  hors taxes, le coefficient variant de 1,12 à 2,35 selon la série.
- **Écart de millésimes du corpus FT84** : la fiche info produit d'édition 05-2026 et la
  fiche excellence d'édition 08-2022 divergent sur l'Uw et le Sw. Le tarif prend le parti
  de la fiche info produit. Divergence préexistante, à exposer avec attribution.
- Relecture métier et tests de non-régression avant indexation.
