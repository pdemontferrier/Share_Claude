---
document: note_cadrage_migration_tarif_H81_Access
version: "1.0"
date_redaction: 2026-09-01
statut: point_de_situation_avant_generation
perimetre: migration du document Tarif H81 Access vers chunks Markdown indexables par le RAG Wikit
gamme: H81 Access
gamme_nom: "Porte de service PVC"
collection: "TRYBA PVC"
materiau: PVC
note_parente: note_cadrage_migration_tarif_H81_v1.md
gamme_pilote_heritee: H81
regles_importees_de: CA76
source_primaire: "H81_Access-modèles_porte.xlsx (257 lignes, 105 colonnes, feuille unique Feuil1)"
source_controle: "Tarif_H81_Access_HT_08-04-2026.pdf et Tarif_H81_Access_TTC_08-04-2026.pdf (45 pages)"
livrables_prevus: [generateur_tarif_H81_Access.py, controle_conformite_H81_Access.py, Tarif_H81_Access_METHODE.md, Tarif_H81_Access_PRIX_PORTES.md, Tarif_H81_Access_PRIX_FIXES.md, Tarif_H81_Access_OPTIONS.md, Tarif_H81_Access_PLUS_VALUES_PROPORTIONNELLES.md, Tarif_H81_Access_CARACTERISTIQUES.md, Tarif_H81_Access_FAISABILITES.md, Tarif_H81_Access_TRANSVERSES.md, message_service_produits_H81_Access.md]
---

# Note de cadrage — Migration du tarif H81 Access vers Markdown

## 0. Objet et statut

Ce document fige les étapes 1 à 3 de la migration du tarif H81 Access : amorçage,
re-vérification des faits fondateurs, détection du périmètre et décompte
prévisionnel. Il est rédigé pour permettre une reprise directe à l'**étape 4**
(adaptation du générateur et de l'audit) dans un nouveau contexte, sans refaire
le chemin.

Il est une déclinaison propre à H81 Access de la note de cadrage tarif établie
sur H81. Il en hérite les principes généraux, qui ne sont pas redémontrés :
fidélité numérique non négociable, anti-fantôme, non-invention, signalement des
arbitrages plutôt que leur résolution silencieuse, auto-discrimination par le
titre, un fichier par nature d'information, plafond de 200 mots marqueur compris,
prose sans puces, numérotation SC continue par fichier depuis SC0002, ligne de
source normée avec em-dashes dans le nom affiché et underscores dans le champ
YAML.

H81 Access est un **tarif hybride** : forfaitaire par modèle pour la porte, comme
H81 ; dimensionnel pour ses fixes, comme T81 et CA76. Les règles H81 sont donc le
socle, et les règles C1 à C8 de CA76 sont importées là où le forfait ne suffit
plus.

## 1. Identité de la gamme et discrimination

Établie depuis `FIP_H81_Access_06-2024.md`, non supposée : matériau **PVC**,
désignation produit **Porte de service PVC**, collection **TRYBA PVC**. Confirmé
par la couverture du tarif.

La paire H81 / H81 Access est identifiée comme à haut risque de collision dans la
note de cadrage générale. Le préfixe de titre retenu est
`H81 Access Porte de service PVC — `, identique à celui du FIP déjà migré, ce qui
assure le liant inter-documents et oppose deux marqueurs au préfixe
`H81 Porte PVC — ` de la gamme voisine : le token `Access` et le syntagme
`de service`. `H81` restant un préfixe strict de `H81 Access`, le sens de
contamination probable est H81 vers Access ; la mention « porte de service »
figure donc aussi dans la première phrase du corps de chaque chunk, et non au
seul titre.

Le mot **collection** est réservé à `TRYBA PVC`. Le second niveau de classement
du tarif — `Vitrage TRYBA`, `Panneau lisse`, `Création`, `Élégance`, porté par la
colonne E nommée `Référence` — est désigné par **famille**, pour ne pas créer de
faux synonyme : `(ligne Traditionnelle, famille Création)`.

## 2. État des sources

**Source primaire** : `H81_Access-modèles_porte.xlsx`, feuille unique `Feuil1`,
257 lignes, 105 colonnes. Sa structure de colonnes **n'est pas celle de H81** :
le générateur H81 lu tel quel produirait des chunks silencieusement faux. Le
classeur juxtapose deux blocs dans la même feuille.

*Bloc « fiche porte »*, colonnes A à AX, lignes 2 à 45 : les 11 modèles, à raison
de **4 lignes par modèle** (une par équipement), les données de modèle n'étant
portées que par la première ligne du groupe.

| Contenu | H81 | H81 Access |
|---|---|---|
| Modèle | A | **F** |
| Ligne / second niveau | D / E | D (`Ligne`) / E (`Référence`) |
| Prix HT 1V / 2V | G / H | **I / J** |
| Prix TTC 1V / 2V | I / J | **K / L** |
| Ud | O | O |
| Modèle de base (vitrage/panneau) | R | R |
| Dimensions mini panneau | P | P |
| Plages dimensionnelles | AF–AU | **AA–AP** |
| Faisabilité équipement / équipement | — | **S / T** |
| Pack Evo HT / TTC | — | **V / W** |
| Teintes / mention teinte | — | **X / Z** |
| Option vitrage-panneau et sa PV | — | **AQ–AX** |
| **Page PDF** | **F** | **absente** |

Les plages dimensionnelles se lisent en AA–AP par blocs de quatre colonnes et par
profil de dormant, dans l'ordre Mini L, Mini H, Maxi L, Maxi H, pour 5103, 5107,
5114 puis 5120.

*Bloc « chapitres tarifaires »*, colonnes AY à DA, lignes 47 à 257 :
`Chapitre` (AY), `Tableau` (AZ), `Désignation` (BA), `Détails` (BB), `HT` (BC),
`TTC` (BD), `Hauteur` (BE), puis la grille `Px L 300` à `Px L 2600` en HT
(BF–CC) et en TTC (CD–DA). **196 lignes portent un HT et un TTC**, réparties en
17 chapitres.

**Sources de contrôle** : les deux PDF, HT et TTC, 45 pages, pagination
identique. Le millésime de couverture est 05-2026, la version de pied de page
V.09/04/2026, le nom de fichier porte 08-04-2026 ; la date retenue pour la ligne
de source et le front matter est **08-04-2026**, par cohérence avec H81.

## 3. Faits vérifiés

Tous les faits ci-dessous ont été établis dans les cellules avant que
l'architecture ne soit figée, et non déduits de la ressemblance avec H81.

**Onze modèles, onze références.** Porte vitrée et Porte panneau plein (lignes
Vitrée et Contemporaine), Cypris, Dahpnis, Madrane, Persane, Santéria, T1L, T2L
(ligne Traditionnelle, famille Création), Melbourne et Vienne (ligne
Traditionnelle, famille Élégance). Aucun modèle n'existe sur plusieurs lignes.

**Deux configurations de vantaux** : un vantail, deux vantaux égaux. **Aucun
trou** : les onze modèles portent les quatre montants. L'anti-fantôme ne retire
rien, mais le test reste exécuté avant génération.

**Le prix des portes ne varie pas avec la dimension** et **ne se factorise pas
par la ligne ni par la famille** : quatre prix HT distincts en un vantail pour
onze modèles — 2 273 € (Porte vitrée, Porte panneau plein), 2 489 € (Santéria,
T2L), 2 814 € (Cypris, Dahpnis, Madrane, Persane, T1L), 3 149 € (Melbourne,
Vienne). La ligne Traditionnelle porte trois prix ; la famille Création en porte
deux. L'unité tarifaire reste le **modèle**.

**Le prix des fixes est dimensionnel.** Lignes 81 à 104 : une grille de
24 hauteurs par 24 largeurs, de 300 à 2 600 mm au pas de 100, soit **576 couples
(H, L) entièrement renseignés en HT et en TTC**, sans trou. S'y ajoute le meneau
battant, tarifé par longueur sur **25 paliers** (lignes 105 à 129).

**La sémantique d'intervalle n'est pas déclarée.** À la différence de CA76, dont
la page 6 énonce qu'une colonne couvre une bande de 100 mm, le tarif H81 Access
dit seulement, page 17, que la table se lit « en lecture directe, en fonction de
la dimension L x H ». Aucune règle de lecture pour une cote intermédiaire.
Conséquence normative : les chunks énoncent les **cotes exactes**, jamais des
bandes, et le trou documentaire est remonté au service Produits.

**Les tarifs transverses sont dans l'Excel.** Le fait qui fondait la règle 7 de
H81 — « l'Excel ne porte pas ces prix » — **ne tient pas**. Les quinze chapitres
tarifaires y figurent en HT et en TTC : Pack Evo, PV vitrages portes, Fixes,
Vitrages pour fixes, Croisillons, Remplissage, PV vitrages panneaux, Garnitures,
Options et accessoires, Élargisseurs, Profilés complémentaires, Tapées de
doublage, Accouplements statiques, Seuils, Fabrications spéciales cintres,
Exemple de calculs. L'exclusion ne peut donc plus être motivée par l'absence de
donnée : elle devient un choix de périmètre assumé.

**L'Excel perd l'unité de facturation.** Les colonnes s'appellent `HT` et `TTC`,
sans qualifieur. Qu'un montant soit un forfait, un €/m², un €/ml, un €/champ, un
€/pièce, un €/châssis ou un €/face n'est lisible que dans le PDF. Migrer depuis
l'Excel seul produirait des chunks numériquement fidèles et **sémantiquement
faux** — défaut qu'un audit de fidélité numérique ne détecte pas.

**La faisabilité des équipements est une constante négative.** La colonne S vaut
`croix rouge` sur les **44 lignes** : les quatre équipements (passe-lettres,
chatière, judas optique, heurtoir) ne sont montables sur aucun des onze modèles.
Confirmé sur les onze pages-modèles du PDF. La règle 6 de H81, formulée pour
lister les modèles compatibles, produirait quatre chunks à liste vide.

**Le plaxage n'est faisable que sur quatre modèles** : Porte vitrée, Porte
panneau plein, Cypris, Dahpnis. Les sept autres sont blanc teinté masse
uniquement. La plus-value correspondante, +15 %, **n'est pas dans l'Excel** : la
colonne Z ne porte qu'un renvoi, « voir PV page Offre couleurs ».

**Deux options seulement sont portées au niveau du modèle** : le Pack Evo, à
531 € HT et 460 € TTC, identique sur les onze modèles, et le panneau phonique, à
63 € HT et 55 € TTC, sur la seule Porte panneau plein. Les options de vitrage
ornemental sont à 0 € avec renvoi transverse. Tout le reste des options est
tarifé au niveau de la gamme.

**L'Ud est constant** : 1,3 W/m².K sur les onze modèles, sous deux libellés,
« Ud porte vitrée » et « Ud porte pleine », avec une anomalie de saisie sur T2L
(double espace) à normaliser au générateur.

**La colonne page est absente**, contrairement à H81. La table des pages est
donc établie en dur contre le PDF et vérifiée par l'audit.

**Le sommaire du PDF est faux** à partir de la page 11 : il annonce Ferrage 11,
Paumelles 12, Offre couleurs 13, là où la pagination réelle est Pack Evo 11,
Ferrage 12, Paumelles 13, Offre couleurs 14 — décalage d'un rang, la page Pack
Evo n'étant pas indexée. Le tableau « ordre alphabétique » de la page 6 est
fiable **sauf pour Persane**, donné en 27 alors que sa page est 26. Seules les
pages elles-mêmes font foi.

Table des pages retenue : pages-modèles 21 à 31 dans l'ordre Porte vitrée 21,
Porte panneau plein 22, Cypris 23, Dahpnis 24, Madrane 25, Persane 26,
Santéria 27, T1L 28, T2L 29, Melbourne 30, Vienne 31. Transverses et chapitres :
Limites dimensionnelles 7, Descriptif des lignes 8, Typologie 9, Pack Evo 11,
Ferrage standard 12, Paumelles 13, Offre couleurs 14, Couleur des accessoires 15,
PV vitrages portes 16, Fixes et meneau battant 17, Vitrages pour fixes 18,
Croisillons 19, Panneaux de soubassement 20, PV vitrages panneaux 32,
Garnitures 33, Options et accessoires 34, Élargisseurs 35, Profilés 36,
Tapées 37, Accouplements 38, Seuils 39, Cintres 40 et 41, Exemple de calculs 42,
Cotes de fabrication 43, Largeur de passage 44, Évolutions du tarif 45.

### Synthèse

| Fait fondateur H81 | Sur H81 Access |
|---|---|
| Prix indépendant de la dimension | tient pour les portes, tombe pour les fixes |
| Prix non factorisable par ligne ou collection | tient |
| Prix attaché au modèle individuel | tient |
| Un modèle égale une ligne | tient |
| Trous de vantaux à détecter | tient en principe, aucun trou en fait |
| Options chiffrées au couple option × modèle | tombe partiellement |
| Faisabilité équipement variable par modèle | tombe, constante négative |
| Transverses absents de l'Excel | tombe |
| Colonne page dans l'Excel | tombe |

## 4. Fidélité des sources : acquise

L'Excel a été confronté aux deux PDF cellule à cellule : **1 540 contrôles,
0 écart**. Périmètre couvert : les 44 prix de modèles, les 576 couples de la
grille des fixes en HT et en TTC, les 25 paliers de meneau battant, et les
196 lignes des quinze chapitres tarifaires. Script conservé et réemployable :
`verif_A2_excel_vs_pdf_H81_Access.py`, dont la table de contrôle PDF est saisie
littéralement page par page.

Conséquence : **il n'y a pas de défaut de saisie dans l'Excel**. Les anomalies
relevées au paragraphe 5 sont présentes à l'identique dans le tarif publié ; ce
sont des défauts de la documentation source, non de la base.

**Le TTC n'est jamais recalculable depuis le HT.** Il lui est inférieur sur la
quasi-totalité du tarif, avec **39 ratios distincts** compris entre 0,85 et 0,92
— 0,897 sur les portes, 0,864 sur un quart des postes. Ce n'est ni une TVA ni un
coefficient de passage unique. Les deux valeurs sont donc transcrites, jamais
dérivées, et la formulation ne doit pas suggérer de dérivation (voir règle A2).

## 5. Divergences exposées, non arbitrées

Aucune n'est tranchée par la migration ; toutes sont remontées au service
Produits par le message joint aux livrables.

**Écarts de libellé entre l'Excel et le PDF**, tranchés en faveur du PDF qui se
déclare document de référence à sa page 2, sans qu'aucun montant soit touché, et
consignés au journal : `GMCA` rendu à `GMECA` ; `44/2-20G-Isol'3 5` rendu à
`44/2-20G-Isol'3 4` ; les codes `PR28-PR32` et `PR28-PR33` rendus à `PP28/PP32`
sur les panneaux standards, `PR33` n'existant dans aucune source ; la mention
« et Blanc veiné » retirée des moulures Chêne d'Or, absente du PDF.

**Incohérences internes au tarif**, présentes dans les deux sources et relevant
d'un arbitrage produit : le panneau lisse renforcé décor 1 face, à 391 € HT pour
142 € TTC, là où le rapport attendu donnerait environ 338 € ; les croisillons
45 mm, seuls postes où HT et TTC sont strictement égaux (36 € et 24 €) ; la
structure fixe de l'exemple de calculs, seul poste où le TTC dépasse le HT
(489 € contre 521 €) ; le total TTC de l'exemple Cypris plaxé, annoncé à 1 821 €
là où la somme des lignes donne 2 903 €.

**Contradiction produit sur les onze pages-modèles** : la note de bas de page
indique que les limites de fabrication valent « pour une menuiserie en aluminium
blanc et une serrure 6 points », alors que la gamme est en PVC et que le ferrage
standard est à 5 points, le 6 points relevant du Pack Evo. Report probable d'un
tarif aluminium.

**Trou documentaire** : la table des fixes de la page 17 ne dit pas quel prix
s'applique à une cote intermédiaire, faute d'énoncer ce que couvre une colonne.

**Défauts de composition** : sommaire décalé d'un rang à partir de la page 11 ;
page de Persane erronée au tableau alphabétique de la page 6.

## 6. Règles retenues

Les sept règles de H81 sont conservées comme socle. Les adaptations suivantes,
notées A1 à A7, ont été arbitrées et validées.

**A1 — Prix et caractéristiques des portes : règles 1, 2 et 5 inchangées**, au
remapping des colonnes près. Un chunk de prix par couple modèle × configuration
de vantaux ; un chunk de caractéristiques par modèle, réunissant le modèle de
base, l'Ud et l'enveloppe dimensionnelle (minimum des minima, maximum des maxima
sur les quatre profils de dormant), seule synthèse admise à la transcription
brute.

**A2 — Formulation du couple HT/TTC.** La forme H81, « X € HT, **soit** Y € TTC »,
affirme une dérivation qui est ici arithmétiquement fausse et invite le modèle à
corriger ou à inverser. Forme retenue : « … est proposé au tarif de X € HT ; le
tarif TTC correspondant est de Y €. » Le point vaut sans doute aussi pour H81,
dont les chunks sont déjà produits ; sa réouverture est une décision distincte.

**A3 — Options : maille dédoublée, unité obligatoire.** Le couple option × modèle
est conservé pour les deux options portées par le modèle. Une maille **option de
gamme** couvre les forfaits indépendants du modèle. Importée de la règle C4 de
CA76 : **tout poste chiffré déclare son unité de facturation**, reprise du PDF ;
un poste dont l'unité ne peut être établie n'est pas généré et part au journal.
Le total, lorsqu'il suppose une multiplication, reste explicitement à la charge
de l'ADV.

Sur le zéro, lecture CA76 avec discriminant explicite : un 0 accompagné d'une
mention de renvoi (« Voir page Plus-value panneau », « voir PV page Offre
couleurs ») est un renvoi transverse et reste exclu ; un 0 nu — béquilles BDEL et
BPEL, ferrage 5 points, changement de teinte de paumelles — est une **absence de
plus-value**, information utile, et donne un chunk.

**A4 — Équipements : formulation en négatif.** Quatre chunks, un par équipement,
énonçant que l'équipement n'est montable sur aucun des onze modèles, lesquels
sont nommés pour l'auto-portance, avec mention qu'il n'est pas chiffré au tarif.
Source : pages-modèles 21 à 31.

**A5 — Faisabilités regroupées.** Un fichier `FAISABILITES`, dénomination CA76,
absorbe les équipements, la faisabilité des teintes par modèle, la typologie de
châssis de la page 9, l'indisponibilité du ferrage automatique et les
faisabilités de teintes en cintre. Aucun montant n'y figure.

**A6 — Approche CA76 importée, avec une adaptation.** Les postes au mètre carré,
au mètre linéaire, au champ, à la pièce, au châssis et à la face restent dans
`OPTIONS` avec leur unité déclarée ; seul le **pourcentage** relève de
`PLUS_VALUES_PROPORTIONNELLES` — deux postes ici, le plaxage à +15 % (page 14) et
la plus-value vitrage de +100 % sur les cintres (page 41). L'adaptation porte sur
la grille des fixes : la règle C2 de CA76 écrit les prix par bandes parce que le
tarif déclare cette sémantique ; **H81 Access ne la déclare pas**, les chunks
énoncent donc les cotes exactes, et un chunk de méthode rappelle que la table se
lit directement, sans interpolation.

**A7 — Table des pages en dur**, établie au paragraphe 3 et vérifiée
indépendamment par l'audit contre le PDF.

## 7. Architecture et décompte prévisionnel

| Fichier | Nature | Chunks (prév.) |
|---|---|---|
| `Tarif_H81_Access_METHODE.md` | lecture de la table des fixes, comptage des croisillons au champ, calcul des tapées (L + 2H), longueur du meneau selon dormant, formules de cintrage, interdiction de calculer, vocabulaire | ~8 |
| `Tarif_H81_Access_PRIX_PORTES.md` | 11 modèles × 2 configurations | 22 |
| `Tarif_H81_Access_PRIX_FIXES.md` | grille 24 × 24 et 25 paliers de meneau | ~51 |
| `Tarif_H81_Access_OPTIONS.md` | forfaits et postes à unité | ~105 |
| `Tarif_H81_Access_PLUS_VALUES_PROPORTIONNELLES.md` | plaxage +15 %, vitrage cintre +100 % | 2 |
| `Tarif_H81_Access_CARACTERISTIQUES.md` | 11 modèles | 11 |
| `Tarif_H81_Access_FAISABILITES.md` | équipements, teintes, typologie, ferrage, cintres | ~8 |
| `Tarif_H81_Access_TRANSVERSES.md` | existence et localisation, sans montant | ~5 |
| **Total** | | **~212** |

Les décomptes des fixes et des options sont des ordres de grandeur : conformément
à la règle C1, la coupure des chunks de grille est pilotée par le comptage des
mots, jamais par une constante. Le décompte définitif sort du générateur.

**Exclusion de périmètre** : le chapitre « Exemple de calculs » (10 lignes,
page 42). Aucun prix qui ne figure ailleurs, deux erreurs arithmétiques, et une
forme — une addition — que la règle 3 interdit au modèle de reproduire. Précédent
identique sur CA76, dont les exemples de la page 58 ont été exclus au même motif.

**Piège d'anti-fantôme à traiter explicitement** : les 24 lignes de la grille des
fixes portent un 0 dans les colonnes `HT` et `TTC` du bloc chapitres. Ce sont des
remplissages de structure, pas des prix. Un test naïf « HT renseigné »
produirait 24 chunks fantômes à 0 €. Le générateur exclut le chapitre `Fixes` du
traitement forfaitaire.

## 8. Format des chunks

Titre auto-porteur préfixé de `H81 Access Porte de service PVC — `. Ligne de
source `*Source : Tarif—H81—Access—HT—08-04-2026.pdf, page N — information
originale|complémentaire — SCxxxx*`. Front matter portant `document_source:
Tarif_H81_Access_HT_08-04-2026.pdf`, `type_document: tarif`, `sous_type`,
`gamme_code: H81 Access`, `gamme_nom`, `collection`, `materiau`, `version_doc`,
`date_validite`, `nb_chunks` et `audiences`. Plafond de 200 mots marqueur
compris, prose sans puces, SC continue par fichier depuis SC0002.

Formes de titre :
`… — Tarif [Modèle] [N vantaux] (ligne [Ligne], famille [Famille])` ;
`… — Tarif fixe latéral vitré et imposte vitrée, hauteur [H] mm` ;
`… — Option [libellé] sur [Modèle] (ligne …, famille …)` ;
`… — Option [libellé] (tarif de gamme)` ;
`… — Caractéristiques [Modèle] (ligne …, famille …)` ;
`… — Faisabilité de [équipement ou attribut] par modèle` ;
`… — Existence et localisation des tarifs de [bloc transverse]`.

Le liant inter-fichiers est le préfixe `H81 Access Porte de service PVC — … 
[Modèle]`, écrit à l'identique dans les fichiers prix, options et
caractéristiques d'une même référence.

## 9. Point de reprise

**Fait** : étapes 1 à 3. Amorçage, identité produit, re-vérification complète des
faits fondateurs, croisement de fidélité (1 540 contrôles, 0 écart), relevé des
divergences, arbitrages A1 à A7, architecture et décompte prévisionnel.

**À faire, étape 4** : adapter `generateur_tarif_H81.py` et
`controle_conformite_H81.py` en versions H81 Access — remapping intégral des
colonnes, table des pages en dur, gestion des deux blocs du classeur, exclusion
du chapitre Fixes du traitement forfaitaire, mapping des unités de facturation
depuis le PDF, journal des colonnes remplies non mappées.

**Étape 5** : générer les huit fichiers, exécuter l'audit (décomptes, plafond,
ligne de source, continuité SC, préfixe et liant, front matter, fidélité
numérique exhaustive des prix, des plus-values et de l'Ud, anti-fantôme,
déclaration d'une unité sur tout poste chiffré, absence de montant en
faisabilités et transverses, discrimination H81 / H81 Access, croisement PDF), et
rédiger le message au service Produits reprenant le paragraphe 5.

**Étape 6** : soumettre un échantillon de relecture couvrant tous les cas de
figure — l'audit vérifie la conformité, non la qualité rédactionnelle.

**Restent ouverts** : deux confirmations non encore données, l'architecture à
huit fichiers avec la séparation `PRIX_PORTES` / `PRIX_FIXES`, et l'exclusion du
chapitre « Exemple de calculs ». Ainsi que la question, distincte, de la
réouverture de la formulation A2 sur les chunks H81 déjà produits.

**Limite déclarée du contrôle** : le PDF TTC n'ayant pu être rejoint, le contrôle
de fidélité TTC restera fondé sur la table de saisie du script A2, et non sur une
ré-extraction indépendante du document. À consigner dans le rapport d'audit.

## 10. À joindre au fil de reprise

1. `H81_Access-modèles_porte.xlsx` — source primaire.
2. `Tarif_H81_Access_HT_08-04-2026.pdf` — source de contrôle (14,4 Mo).
3. `verif_A2_excel_vs_pdf_H81_Access.py` — table de contrôle PDF déjà saisie,
   dont la reconstitution serait coûteuse.
4. Cette note.

Sont déjà dans le projet et n'ont pas à être rejoints :
`note_cadrage_migration.md`, `note_cadrage_migration_tarif_H81_v1.md`,
`note_cadrage_migration_tarif_CA76.md`, `generateur_tarif_H81.py`,
`controle_conformite_H81.py`, `FIP_H81_Access_06-2024.md`.
