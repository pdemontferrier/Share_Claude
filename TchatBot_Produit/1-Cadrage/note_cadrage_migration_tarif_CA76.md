---
document: note_cadrage_migration_tarif_CA76
version: "1.0"
date_redaction: 2026-09-01
statut: point_de_situation
perimetre: migration du document Tarif CA76 / CAG76 vers chunks Markdown indexables par le RAG Wikit
gamme: CA76
gamme_nom: "Coulissant Aluminium"
gammes_couvertes: [CA76, CAG76]
collection: "TRYBA ALUMINIUM"
note_parente: note_cadrage_migration_tarif_H81_v1.md
gamme_pilote_heritee: T81
source_primaire: "CA76_-infos-tarifs.xlsx (392 lignes de données, 131 colonnes, feuille unique Feuil2)"
source_controle: "Tarif_CA76_HT_19-06-2026.pdf (60 pages)"
livrables: [generateur_tarif_CA76.py, controle_conformite_CA76.py, Tarif_CA76_METHODE.md, Tarif_CA76_PRIX_CHASSIS.md, Tarif_CA76_OPTIONS.md, Tarif_CA76_PLUS_VALUES_PROPORTIONNELLES.md, Tarif_CA76_FAISABILITES.md, Tarif_CA76_TRANSVERSES.md]
---

# Note de cadrage — Migration du tarif CA76 vers Markdown

## 0. Objet et rapport aux notes H81 et T81

Ce document est la déclinaison propre à la gamme CA76 de la note de cadrage
générale produite sur H81. Il hérite de T81 et non de H81 : CA76 est un tarif
**dimensionnel**, comme la fenêtre PVC T81 et contrairement à la porte d'entrée
H81, dont l'unité tarifaire était le modèle. Les règles T1 à T7 sont donc le
point de départ ; les règles C1 à C8 énoncées ici les remplacent partout où
CA76 s'en écarte.

Les principes généraux restent intacts et ne sont pas redémontrés : fidélité
numérique non négociable, anti-fantôme, non-invention, auto-discrimination par
le titre, exposition des divergences avec attribution par source, un fichier par
nature d'information, plafond de 200 mots marqueur compris, prose sans puces,
numérotation SC continue par fichier depuis SC0002, ligne de source normée avec
em-dashes dans le nom affiché et underscores dans le champ YAML.

## 1. Ce que CA76 change par rapport à T81

Sept natures d'information n'existaient dans aucune des deux gammes précédentes.
Trois d'entre elles commandent l'architecture.

**La plus-value proportionnelle appliquée au prix de grille.** H81 et T81
renvoyaient tout pourcentage aux transverses, sans montant, parce qu'un
pourcentage suppose un calcul. Sur CA76 ce renvoi vide le tarif de sa substance :
sans les taux du dormant et de la teinte, l'ADV ne peut chiffrer qu'un dormant
neuf en teinte de groupe 1. Ces taux sont donc transcrits littéralement, leur
application restant explicitement à la charge de l'ADV. Nature nouvelle, donc
fichier nouveau : F4.

**L'attribut de cellule porté par un remplissage graphique.** Le marquage
« croisée renforcée obligatoire automatique sans plus-value » est un attribut par
cellule qui n'existe ni dans l'Excel ni dans la couche texte du PDF : seul le
remplissage beige des cellules le porte. T81 avait des discriminants absents de
l'Excel mais au moins textuels ; ici il faut descendre à la géométrie.

**Deux produits sous une même couverture.** CA76, coulissant, et CAG76, sa
déclinaison à galandage, partagent un tarif mais ont des grilles, des
faisabilités et des règles de croisée distinctes. Le préfixe de titre est donc
double, et la paire CA76 / CA80 New, identifiée comme à haut risque de collision
sémantique, impose que les libellés soient distinctifs sans ambiguïté.

S'y ajoutent quatre différences moins structurantes : des **formules de calcul
inscrites au tarif** (prix des tapées égal à la largeur plus deux fois la
hauteur, méthode des soubassements, comptage des croisillons au champ) ; une
**table de faisabilité à trois entrées** croisant poignée, teinte et
configuration de cylindre ; un **renvoi tarifé vers une autre gamme**, le forfait
de laquage portant sur le volet roulant Chrono One ; et une **troisième
sémantique de cellule vide**, l'impossibilité produit.

Enfin, **le vocabulaire diverge** : sur la gamme CA, « crémone » est le terme
exact, le CA76 étant fermé par une crémone Secure+ à crochets inox. La règle
restrictive appliquée à H81 et T81 ne s'y transpose pas.

## 2. Faits vérifiés sur CA76

Tous les faits ci-dessous ont été établis par la donnée avant que l'architecture
ne soit figée, et non déduits de la ressemblance avec T81.

**Structure tarifaire.** Prix dimensionnel sur grilles hauteur par largeur. Pas
de modèle, pas de ligne, pas de collection tarifaire. **Huit grilles**, dont
l'axe n'est ni le type d'ouverture de T81 ni le modèle d'H81 mais le triplet
**produit × nombre de vantaux × nombre de rails** : CA76 en 2 vantaux 2 rails,
3 vantaux 2 rails, 4 vantaux 2 rails, 3 vantaux 3 rails et 6 vantaux 3 rails ;
CAG76 en galandage 1 vantail, 2 vantaux et 4 vantaux. **4 077 cellules HT et
4 077 cellules TTC**, pas de désalignement. Pas de rectangle : 81 cellules
manquent, toutes en troncature de fin de ligne, aucun trou intérieur, aucune
ligne discontinue.

**Une cellule est un intervalle, et le tarif l'écrit.** Page 6, bloc « Lecture du
tarif » : la colonne 1000 couvre les largeurs de 901 à 1000 mm, la ligne 600
couvre les hauteurs de 501 à 600 mm. Le pas est de 100 mm sur les deux axes,
partout. C'est ce fait, cherché explicitement et non supposé, qui rend
l'architecture T81 transposable.

**Cote tarif et cote de fabrication sont distinguées** (page 6), mais la
conversion diverge : sur T81 le dormant agissait sur la cote, ici il agit sur le
prix, en pourcentage.

**Les deux sources concordent parfaitement sur les grilles.** Les 4 077 cellules
du PDF et celles de l'Excel ont été confrontées une à une : aucune cellule
présente d'un seul côté, aucun écart de valeur. Le TTC vaut environ 1,43 fois le
HT, arrondi à l'euro : ce n'est pas la TVA mais un coefficient de passage, jamais
recalculable, ce qui impose de transcrire les deux valeurs.

**Colonne gamme propre.** 392 lignes sur 392 en CA76, aucune ligne étrangère.
Aucune colonne de largeur vide. `Mention HT` et `Mention TTC` sont constantes et
sans information ; `champs clé` est un identifiant unique servant la traçabilité.

**Les unités de facturation n'existent que dans le PDF**, et elles sont plus
riches que sur T81 : pourcentage, mètre carré, champ, volume, mètre linéaire,
pièce, châssis, forfait, prix de l'ensemble. Deux seuils sont à préserver, la
surface minimale de facturation de 0,5 m² et la règle explicite du comptage des
croisillons au champ et non au mètre linéaire.

**Trois états de cellule vide.** Un zéro vaut absence de plus-value, lecture T81
et non H81. Une case blanche vaut soit une impossibilité produit — la Halo en
cylindre traversant, le panneau phonique en 36 mm, la gravure ADP sur le motif
MG9 — soit un non-renseignement. La vacuité elle-même a deux encodages, `None`
et chaîne vide, à traiter identiquement.

**Le sommaire est faux à partir de la page 36**, faute d'y avoir indexé la page
« Plus-values vitrages compositions libres ». Le sommaire général et les
sommaires de section portent le même décalage de +1 jusqu'à la fin — l'inverse
de T81, où seuls les sommaires de section étaient périmés. Seuls les en-têtes et
les pieds de page font foi ; ils ont été relevés page à page et le numéro
imprimé coïncide avec l'index PDF sur les 60 pages.

**La couche texte porte « TA76 OC » sur vingt-quatre pages**, résidu du gabarit
de la gamme voisine, jamais dans le corps. À filtrer. À l'inverse `NF-TA84` et
`NF-TA84-D`, page 52, sont des références légitimes de pièce d'appui.

## 3. Divergences exposées et non arbitrées

Quatre écarts internes au tarif ou entre ses deux sources ont été relevés. Aucun
n'est tranché par la migration ; tous sont remontés au service Produits.

**Vitrages ornementaux, page 35 — bloquant.** Les dix-neuf plus-values divergent
entre l'Excel et le PDF, par un décalage systématique d'une ligne sur les deux
blocs. Vérifié sur la page rastérisée : ce n'est pas un artefact d'extraction.
Le chapitre est **gelé**, aucun chunk n'est produit.

**Poignée Delta Flap.** L'Excel la tarife 21 € HT en option ; la page 47 la range
parmi les poignées standard sans plus-value et le montant n'y figure nulle part.

**Croisillons, page 38.** Le texte annonce quatre types en 18, 26 et 10 mm ; le
tableau tarife du 18, du 26 et du 45 mm.

**Galandage 4 vantaux.** La grille page 24 se dit valable sur 1 ou 2 rails ; la
page 9 ne montre cette composition qu'en 2 rails. Les deux énoncés sont rapportés
avec attribution.

Sont par ailleurs **exclus du périmètre chiffré** : les exemples de calcul de la
page 58, périmés dans les deux sources — 2 930 € et 8 890 € annoncés là où les
grilles donnent 2 873 € et 8 459 € —, et la page 5, Fiche Info Produit d'édition
10-2025 insérée sans pagination, déjà migrée en édition 01-2026 avec des valeurs
concordantes.

## 4. Règle d'arbitrage des divergences

Une divergence portant sur une **valeur** est exposée avec attribution par
source, jamais arbitrée en silence, et remontée au service Produits.

Une divergence portant sur un **libellé ou un code de référence**, dont l'une des
deux sources est manifestement fautive, est tranchée en faveur du PDF, qui se
déclare document de référence à sa page 2 et que l'ADV a sous les yeux. Chaque
correction est consignée au journal et ne touche aucun montant. Appliqué à onze
libellés : la mention « 28 et 32 mm » rendue à 28 et 36 mm, « ral 7017 » à
RAL 7016, « Teinte Std grp 2 » à groupe 1, huit suffixes `_DV` rendus à `_CE`, et
deux lignes de tapées réattribuées à leur chapitre.

## 5. Architecture en six fichiers

| Fichier | Nature | Règle |
|---|---|---|
| `Tarif_CA76_METHODE.md` | cotes, lecture par bandes, méthodes de calcul, vocabulaire | C3 |
| `Tarif_CA76_PRIX_CHASSIS.md` | les huit grilles dimensionnelles | C1, C2 |
| `Tarif_CA76_OPTIONS.md` | plus-values forfaitaires chiffrées en euros | C4 |
| `Tarif_CA76_PLUS_VALUES_PROPORTIONNELLES.md` | plus-values en pourcentage | C5 |
| `Tarif_CA76_FAISABILITES.md` | restrictions et impossibilités, sans montant | C6 |
| `Tarif_CA76_TRANSVERSES.md` | existence et localisation, sans montant | C7 |

Le liant inter-fichiers change une troisième fois : ni le modèle d'H81, ni le
type d'ouverture de T81, mais le **triplet produit × vantaux × rails**, écrit à
l'identique partout où il apparaît.

## 6. Les règles normatives C1 à C8

### Règle C1 — Découpage des prix de grille

La maille est **une grille, une bande de hauteur, une tranche contiguë de bandes
de largeur**. La tranche est la plus grosse qui tienne sous le plafond : la
coupure est pilotée par le comptage des mots, jamais par une constante. Une
tranche ne franchit jamais la frontière d'une page du PDF, une grille s'étalant
sur deux ou trois pages : un chunk ne peut pas citer une page où son contenu ne
figure pas.

### Règle C2 — Rédaction des prix

Chaque bande est écrite en toutes lettres, « de 1001 à 1100 mm », pour qu'une
cote non ronde tombe dans une bande énoncée sans que le modèle ait à calculer.
La première bande de chaque échelle n'a pas de plancher dans la source : elle
s'écrit « jusqu'à N mm », aucun plancher n'étant inventé. HT et TTC figurent dans
la même proposition. Aucun prix n'est calculé ni interpolé.

### Règle C3 — Instruction LLM

Le modèle lit, il ne calcule pas. Cela vaut pour les prix de grille, pour les
montants unitaires dont le total revient à l'ADV, pour les pourcentages dont
l'application revient à l'ADV, et pour les formules inscrites au tarif, dont le
chunk énonce la méthode sans jamais l'exécuter.

### Règle C4 — Options et plus-values forfaitaires

Regroupement strictement iso-prix : mêmes chapitre, tableau, désignation, HT et
TTC. Seules les variantes de la colonne Détails fusionnent. Le scalaire retenu
est HT/TTC, à défaut PV HT/PV TTC — les deux couples coexistent sur une même
ligne, pour les meneaux, et ne sont pas interchangeables. **Tout poste chiffré
déclare son unité de facturation** ; lorsque le tarif ne l'indique pas, le chunk
le dit et renvoie à la page plutôt que de servir un montant nu. Un discriminant
absent de l'Excel est repris de la colonne Désignation du PDF et **rattaché par
le montant, jamais par l'ordre des lignes** ; à défaut de rattachement, le poste
n'est pas généré et il est consigné.

### Règle C5 — Plus-values proportionnelles

Les taux ne figurent dans aucune cellule de l'Excel, qui n'en garde que la
coquille : deux lignes de doublage sans valeur, la ligne « Dormant à ailettes »
étant même absente. Ils sont relevés page à page dans le PDF, transcrits
littéralement, et chaque chunk énonce que l'application du pourcentage revient à
l'ADV. Le chunk porte aussi le périmètre du taux : les plus-values de dormant ne
valent que sur les grilles à 2 rails, le tarif déclarant les dormants à ailette
et le doublage impossibles en 3 rails et sur le galandage.

### Règle C6 — Faisabilités et restrictions

Aucun montant. Les impossibilités produit relevées comme cellules vides y sont
reprises, de sorte qu'une case blanche ne puisse jamais être servie comme une
gratuité. Les divergences internes au tarif y sont exposées avec attribution par
page.

### Règle C7 — Transverses

Aucun montant. Le chunk énonce l'existence de l'information et sa localisation,
et renvoie à la page pour la valeur.

### Règle C8 — Croisée renforcée

L'attribut est récupéré des coordonnées vectorielles du PDF et rattaché aux
couples hauteur–largeur par ancrage sur les en-têtes de largeur et la colonne des
hauteurs. Le marquage se vérifie être un **effet de seuil** : pour une bande de
hauteur donnée, il couvre toutes les largeurs tarifées au-delà d'un seuil, ce qui
permet de l'énoncer en une proposition au lieu d'énumérer les cellules. La forme
est contrôlée contre les largeurs réellement tarifées de la ligne, les dernières
lignes étant tronquées. Si elle n'est pas vérifiée, les largeurs sont énumérées ;
si l'ancrage échoue, la clause n'est pas produite et le cas est consigné.

## 7. Format des chunks

Titre auto-porteur préfixé de `CA76 Coulissant Aluminium — ` ou de
`CAG76 Coulissant Aluminium à galandage — `. Ligne de source normée
`*Source : Tarif—CA76—HT—19-06-2026.pdf, page N — information originale|complémentaire — SCxxxx*`.
Plafond de 200 mots, marqueur `##` compris. Prose, sans puces. Front matter YAML
portant `document_source`, `type_document`, `sous_type`, `gamme_code`,
`gammes_couvertes`, `collection`, `version_doc`, `date_validite` et `nb_chunks`.

## 8. Décomptes

| Fichier | Chunks |
|---|---|
| METHODE | 12 |
| PRIX_CHASSIS | 794 |
| OPTIONS | 162 |
| PLUS_VALUES_PROPORTIONNELLES | 8 |
| FAISABILITES | 33 |
| TRANSVERSES | 10 |
| **Total** | **1 019** |

Hors 19 postes de vitrages ornementaux gelés. Décompte arrêté après la relecture d'échantillon de l'étape 6, qui a fait passer les chunks de prix à une seule bande d'un accord pluriel à un accord singulier. À comparer aux 340 chunks d'H81 et
aux 646 de T81, pour deux fois et demie plus de cellules de prix que T81.

## 9. Industrialisation

`generateur_tarif_CA76.py` produit les six fichiers depuis l'Excel et le PDF, et
tient un journal exhaustif : colonnes non mappées, lignes exclues et leur motif,
corrections de libellé, discriminants repris du PDF, unités non établies,
cellules de croisée récupérées.

`controle_conformite_CA76.py` **ne réutilise aucune fonction du générateur**. Il
relit les Markdown, redéclare ses propres tables et re-extrait lui-même le
marquage graphique, de sorte qu'une divergence entre les deux soit un écart réel
et non une tautologie. Il exécute quinze familles de contrôles : couverture
exhaustive des cellules, fidélité numérique exhaustive HT et TTC, bornes de
bandes recalculées indépendamment, anti-fantôme, frontière de page, seuils de
croisée reconfrontés à la géométrie, bijection des postes forfaitaires par
multiensembles insensibles aux libellés, déclaration d'une unité sur tout poste
chiffré, taux de F4 retrouvés sur leur page, unicité des titres, absence de tout
montant en méthode, faisabilités et transverses, vocabulaire et contamination
inter-gammes, forme, décomptes, et croisement PDF page par page qui valide au
passage la table des pages.

## 10. Scories relevées à la relecture d'échantillon

Six défauts de rédaction, invisibles pour l'audit parce qu'ils portent sur la
langue et non sur les nombres, ont été relevés sur un échantillon de vingt et un
chunks couvrant tous les cas de figure, puis corrigés à la source dans le
générateur : un accord au pluriel fautif et une apposition non fermée dans le
patron des plus-values proportionnelles ; une redondance entre le taux et sa
glose ; une contradiction entre une unité forfaitaire et la phrase de
multiplication qui la suivait ; un accord pluriel sur les chunks de prix ne
portant qu'une seule bande ; l'annonce systématique de la colonne Détails comme
« la référence », catégorie fausse dans la majorité des cas ; des abréviations
non développées dans les titres ; et un en-tête de colonne recopié dans six
désignations de couvre-joints.

## 11. Reste à traiter

Arbitrage produit sur la page 35, qui débloquera les 19 chunks gelés. Arbitrages
secondaires sur Delta Flap, les croisillons 45 mm et le galandage 4 vantaux, dont
l'exposition actuelle tient sans eux. Signalement au service documentation du
décalage du sommaire à partir de la page 36, du résidu « TA76 OC » dans la couche
texte et du dos de couverture emprunté à un tarif Portes de garage de 2018.
Relecture d'échantillon et tests de non-régression avant indexation.
