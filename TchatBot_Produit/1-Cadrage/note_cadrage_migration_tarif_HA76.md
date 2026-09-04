---
document: note_cadrage_migration_tarif_HA76
version: "1.0"
date_redaction: 2026-09-01
statut: point_de_situation
perimetre: migration du document Tarif HA76 vers chunks Markdown indexables par le RAG Wikit
gamme: HA76
gamme_nom: "Porte d'entrée aluminium"
collection: "TRYBA ALUMINIUM"
note_parente: note_cadrage_migration_tarif_H81_v1.md
gammes_pilotes_heritees: [H81, T81, CA76, FT84]
source_primaire: "HA76_-infos-tarifs.xlsx (596 lignes de données, 139 colonnes, feuille unique Feuil1)"
source_controle: "Tarif_HA76_HT_23-06-2026.pdf (137 pages, édition HT seule)"
livrables: [generateur_tarif_HA76.py, controle_conformite_HA76.py, Tarif_HA76_METHODE.md, Tarif_HA76_PRIX_MODELES.md, Tarif_HA76_OPTIONS_MODELES.md, Tarif_HA76_CARACTERISTIQUES.md, Tarif_HA76_COMPAT_EQUIPEMENTS.md, Tarif_HA76_CATALOGUE_OPTIONS.md, Tarif_HA76_FAISABILITES.md, Tarif_HA76_TRANSVERSES.md, Message_service_produit_HA76.md]
---

# Note de cadrage — Migration du tarif HA76 vers Markdown

## 0. Objet et rapport aux notes précédentes

Ce document est une déclinaison spécifique à la gamme HA76 de la note de cadrage tarif
établie sur H81. Il en hérite les principes généraux — auto-discrimination des chunks,
titre auto-porteur préfixé du code gamme, ligne de source par chunk, plafond de 200 mots,
prose sans puces, numérotation SC continue depuis SC0002, fidélité numérique non
négociable, anti-fantôme, signalement des arbitrages plutôt que leur résolution
silencieuse — et **conserve son coeur de règles**, contrairement à T81 qui avait dû les
remplacer.

Il y ajoute trois acquis des gammes postérieures : le fichier METHODE de T81, la règle
d'arbitrage des divergences de CA76 (une divergence sur une valeur est exposée et remontée,
une divergence sur un libellé manifestement fautif est tranchée en faveur du PDF et
consignée), et le message au service Produits de FT84.

## 1. Ce que HA76 change par rapport à H81

**Le produit.** HA76 est la porte d'entrée aluminium de la collection TRYBA ALUMINIUM,
conformément au FIP, au CABP et à la Fiche Excellence déjà migrés. Ce n'est pas une porte
PVC, et ce n'est pas la porte monobloc HAM76.

**L'Excel est hybride.** Les colonnes A à CE reproduisent la structure de modèles de H81 ;
les colonnes CF à EI reproduisent la structure de chapitres et de grilles de T81 et CA76.
Un seul fichier porte donc deux régimes tarifaires. Le mapping de colonnes du générateur
H81 est intégralement caduc.

**Une ligne de l'Excel n'est pas un modèle.** C'est le fait structurant. 389 lignes
décrivent 92 modèles, à raison de une à sept lignes par modèle. La ligne supplémentaire
n'est pas un axe de prix : c'est un **emplacement d'affichage** de la page PDF du modèle.
Sur Arosa, les sept lignes répètent à l'identique le prix, l'Ud et les limites
dimensionnelles, et ne diffèrent que par l'emplacement rempli — un équipement, une teinte,
une option de vitrage par ligne. Une génération ligne à ligne produirait 389 chunks de prix
au lieu de 92. Le groupement par modèle est donc une opération préalable obligatoire, et
non un détail d'implémentation.

**Les prix unitaires portent leur unité dans le PDF, pas dans l'Excel.** Les chapitres
Vitrages, Vitrages fixes et les habillages portent des valeurs dans des colonnes intitulées
simplement HT et TTC. Les pages 27, 29, 129, 130 et 131 du PDF révèlent que ce sont des
montants au mètre carré ou au mètre linéaire. Transcrire ces valeurs comme des forfaits
serait une faute de fidélité invisible à tout contrôle purement numérique.

**Une grille dimensionnelle apparaît.** Le chapitre Fixes tarife les fixes latéraux vitrés
et les impostes vitrées sur une table de 24 hauteurs par 24 largeurs, soit 553 prix HT et
553 prix TTC. H81 n'avait aucun équivalent.

**Il n'existe pas de page transverse des équipements.** Chez H81, la compatibilité des
équipements était rassemblée sur une page. Ici elle est portée modèle par modèle, sur la
page du modèle, par un symbole — check vert ou croix rouge — et non par un texte.

## 2. Faits vérifiés sur HA76

**Le prix d'un modèle ne varie pas avec la dimension.** Le fait H81 tient. Les colonnes de
limites portent des limites de fabrication, non des paliers de prix. Deux profils de
dormant au lieu de quatre chez H81 : AL10101 avec AL10108, et AL10100.

**Le prix se factorise strictement par modèle.** Le HT en un vantail, le HT en deux vantaux
et le TTC en deux vantaux sont rigoureusement constants sur toutes les lignes d'un même
modèle. Zéro divergence sur 92 modèles. Le TTC en un vantail fait exception sur quinze
modèles (voir §3).

**Aucun modèle n'appartient à deux lignes ni à deux références.** La ligne — Vitrée 35
modèles, Contemporaine 25, Accord 18, Traditionnelle 14 — et la référence — Crystal,
Accord, Excellence, Porte du mois permanente, Passion, Artiste, Mesure, Vitrée, Panneau
lisse — sont des propriétés du modèle. La référence est l'équivalent HA76 de la collection
H81. Le liant inter-fichiers par nom de modèle reste valide.

**Configurations de vantaux : deux, comme H81.** 92 modèles ont un prix en un vantail,
77 en deux vantaux. Les quinze modèles sans prix en deux vantaux sont exactement les quinze
modèles de la référence Porte du mois permanente. Décompte prix : 92 + 77 = 169 chunks.

**Options rattachées au modèle : cinq familles.** Options de vitrage, vitrage analogue,
options de vitrage de la ligne Artiste, teintes de grille, option spécifique, auxquelles
s'ajoute le panneau analogue pour fixe latéral. 220 couples option × modèle sont servis,
dont 133 avec une plus-value strictement positive.

**Compatibilité des équipements.** Judas optique compatible sur 22 modèles, heurtoir sur
12, passe-lettres sur 27, chatière sur 13. Aucun n'est chiffré sur les pages modèles. Le
poussoir central est un cas distinct : propre à deux modèles et chiffré 287 €, il relève
des options — c'est l'exception que la règle 6 de H81 avait déjà prévue pour le poussoir
incurvé. Un cas nouveau apparaît : le passe-lettres marqué d'un double renvoi de note sur
Paris et Paris plein, qui signale une incompatibilité avec le poussoir central.

**Catalogue d'options hors modèle.** Quinze chapitres portent 157 lignes tarifées sans
rattachement à un modèle. Huit sont retenus comme forfaitaires : ferrage, plus-values de
vitrages pour panneaux, garnitures standards, garnitures design et rosettes, poussoirs et
heurtoirs, options de vantail et de sécurité, meneaux complémentaires, biométrie, plus les
meneaux battants tarifés par longueur au chapitre Fixes. Six sont écartés : Vitrages et
Vitrages fixes au mètre carré, renforts et bavettes et couvre-joints au mètre linéaire,
exemples de calcul.

**Correspondance des pages.** La page PDF vaut la page imprimée, vérifiée sur les pieds de
page. Les pages modèles vont de 30 à 121. Les transverses sont aux pages 21 à 29 en amont
et 122 à 135 en aval.

## 3. Divergences exposées et non arbitrées

Sept écarts ont été relevés et remontés au service Produits par un message dédié. Aucun
n'est tranché par la migration, à l'exception d'un libellé manifestement fautif.

**Prix TTC incohérents sur les quinze modèles Porte du mois — bloquant.** Le prix HT est
constant, le prix TTC varie entre les lignes d'un même modèle et se trouve inférieur au
prix HT. Soixante occurrences. Le PDF étant une édition HT seule, la source de contrôle ne
permet pas de trancher. Le corpus **gèle le TTC** de ces quinze modèles : il sert le prix HT
et renvoie l'ADV à l'édition TTC du tarif.

**Longueur de meneau battant.** L'Excel porte 250 mm là où la page 28 du PDF porte 350 mm,
en rompant la progression des longueurs. S'agissant d'un libellé et non d'un montant, la
valeur est rectifiée à 350 mm d'après le PDF, et la correction est consignée au journal du
générateur.

**Sommaire décalé de quatre pages** sur toute la section postérieure aux pages modèles. Les
pieds de page font foi.

**Page 137 étrangère** : un tarif de portes de garage de juillet 2018, exclu du périmètre.

**Page 20** : la Fiche Info Produit HA76 d'avril 2024, déjà migrée pour elle-même, exclue du
périmètre tarif pour éviter la double source.

**Exemples de calcul internement incohérents** : la somme des lignes ne correspond pas au
total annoncé, en HT comme en TTC. Chapitre exclu.

**Libellé du panneau analogue absent de l'Excel** alors que le montant existe sur cinq
modèles. La page 95, vérifiée visuellement, intitule ce bloc « Panneau analogue pour fixe
latéral ». Le libellé est restitué depuis le PDF et consigné.

## 4. Chapitre gelé — la grille des fixes

La page 28 indique que la table des fixes vitrés est en lecture directe en fonction de la
dimension du fixe. Elle **n'énonce aucune règle d'intervalle**, contrairement à la page 10
du tarif T81 qui précisait qu'une colonne couvre une bande de largeurs. Or c'est exactement
ce fait qui rendait l'architecture T81 tenable : sans lui, toute demande hors cote ronde
exige un arrondi, c'est-à-dire un raisonnement numérique proscrit par la règle 3.

Les 553 prix de cette table ne sont donc pas intégrés. Un chunk d'orientation renvoie à la
page 28. L'intégration est conditionnée à la précision de la règle de lecture par le service
Produits. Cette décision est reportée telle quelle dans le message qui leur est adressé.

## 5. Architecture en huit fichiers

Le principe reste celui de H81 : un fichier par nature d'information, chacune avec sa maille
propre.

**METHODE**, cadre de lecture : portée du tarif et distinction avec HAM76, unité tarifaire,
indépendance du prix et de la dimension, exigence de la configuration de vantaux, interdit
de calcul entre HT et TTC, unités non forfaitaires, périmètre absent, portée des limites de
fabrication.

**PRIX_MODELES**, un chunk par modèle et par configuration de vantaux réellement tarifée.

**OPTIONS_MODELES**, un chunk par couple option × modèle, plus-values chiffrées et
plus-values explicitement nulles, à l'exclusion des renvois transverses.

**CARACTERISTIQUES**, un chunk par modèle réunissant vitrage de base, dimensions de
vitrage, Ud et enveloppe dimensionnelle sur les deux profils.

**COMPAT_EQUIPEMENTS**, un chunk par équipement listant les modèles compatibles.

**CATALOGUE_OPTIONS**, un chunk par référence d'option tarifée indépendamment du modèle.

**FAISABILITES**, faisabilités non tarifaires : teintes par référence, limites
dimensionnelles générales, réalisation des fixes et des meneaux, modèles tarifés en un seul
vantail.

**TRANSVERSES**, un chunk par bloc, nommant l'existence et la logique tarifaire sans
reproduire aucun montant.

## 6. Règles normatives HA76 — H1 à H10

**H1 — Découpage des prix.** Un chunk par couple modèle × configuration de vantaux
réellement tarifée. Anti-fantôme préalable : aucune configuration absente du tarif ne donne
un chunk. Reprend la règle 1 de H81.

**H2 — Normalisation préalable des emplacements d'affichage.** Le modèle, et non la ligne
de l'Excel, est l'unité de génération. Le prix, l'Ud, le vitrage et les limites se lisent au
niveau du groupe de lignes. Règle nouvelle, imposée par le fait structurant du §1.

**H3 — Rédaction des prix.** Une phrase par configuration, liant dans la même phrase le prix
HT et le prix TTC, sans grille et sans autre configuration. Reprend la règle 2 de H81.

**H4 — Instruction LLM.** À défaut de nombre de vantaux précisé, le modèle demande la
clarification et n'explore pas les configurations. Il ne restitue qu'un prix présent en
toutes lettres, ne calcule jamais, n'interpole pas, ne déduit pas le TTC du HT. Reprend et
durcit la règle 3 de H81 : sur HA76, le rapport entre HT et TTC varie d'une ligne de tarif à
l'autre, ce qui interdit formellement toute conversion.

**H5 — Options rattachées au modèle.** Un chunk par couple option × modèle. Sont servies les
plus-values chiffrées et les plus-values explicitement nulles, énoncées « sans plus-value ».
Sont écartés les couples sans montant et ceux dont la remarque renvoie à une page
transverse. Extension de la règle 4 de H81 : chez H81 une plus-value nulle était toujours un
renvoi, ce n'est plus le cas ici.

**H6 — Caractéristiques.** Un chunk par modèle, avec l'enveloppe dimensionnelle calculée sur
les deux profils, seule dérogation admise à la transcription littérale. Le chunk rappelle que
ces limites valent pour une menuiserie blanche à serrure six points. Reprend la règle 5 de
H81, adaptée au nombre de profils.

**H7 — Compatibilité des équipements.** Un chunk par équipement. La faisabilité se lit sur un
symbole et non sur un texte. La page source est la plage des pages modèles, non une page
transverse. Un équipement chiffré relève des options. Reprend la règle 6 de H81, adaptée à la
source.

**H8 — Catalogue d'options hors modèle.** Un chunk par référence forfaitaire, titre portant le
chapitre d'origine, corps précisant que le montant est indépendant du modèle. Règle nouvelle.

**H9 — Unités non forfaitaires.** Tout montant exprimé au mètre carré, au mètre linéaire ou
en pourcentage est exclu du corpus chiffré et traité en orientation seule. Extension de la
règle 7 de H81 à un cas que H81 n'avait pas rencontré, parce que ces valeurs n'étaient pas
dans son Excel.

**H10 — Transverses.** Un chunk par bloc transverse, nommant la nature et la logique
tarifaire, renvoyant à la page, sans aucun montant ni pourcentage. Reprend la règle 7 de H81.

## 7. Format des chunks et discrimination HA76 / HAM76

Le préfixe retenu est `HA76 Porte d'entrée aluminium — `, aligné sur la désignation dominante
du corpus technique et sur l'intitulé imprimé en tête de chaque page du tarif. Aucune
désignation nouvelle n'a été fabriquée.

Le risque de collision avec HAM76 est réel : 67 des 91 chunks HAM76 déjà déposés portent une
désignation strictement identique, le mot *monobloc* — qui est la différence produit réelle,
panneau monobloc de 86 mm contre panneau de remplissage de 40 mm ou vitrage — n'apparaissant
que dans 24 titres. La discrimination repose donc sur trois leviers cumulés : le code de
gamme en tête de titre, le nom du modèle dans le titre, et la mention explicite « de la gamme
HA76 » dans le corps de chaque chunk de prix, d'option et de caractéristiques. Un chunk de
tête du fichier METHODE énonce en outre que le tarif ne s'applique pas à HAM76. L'audit
vérifie ces trois points.

Formes de titre :

## HA76 Porte d'entrée aluminium — Tarif Azurite 1 vantail (ligne vitrée, référence Crystal)
## HA76 Porte d'entrée aluminium — Option Vitrage sécurité 44/6 sur Azurite (ligne vitrée, référence Crystal)
## HA76 Porte d'entrée aluminium — Caractéristiques Azurite (ligne vitrée, référence Crystal)
## HA76 Porte d'entrée aluminium — Compatibilité de l'équipement chatière par modèle
## HA76 Porte d'entrée aluminium — Tarif catalogue biométrie : fourniture
## HA76 Porte d'entrée aluminium — Existence et localisation des tarifs de l'offre couleurs

Ligne de source : `*Source : Tarif—HA76—HT—23-06-2026.pdf, page N — information originale —
SCnnnn*`, la forme au pluriel `pages 30 à 121` étant admise pour les chunks de compatibilité
d'équipement, dont la maille couvre l'ensemble des pages modèles.

## 8. Décomptes

| Fichier | Chunks |
|---|---|
| METHODE | 8 |
| PRIX_MODELES | 169 |
| OPTIONS_MODELES | 220 |
| CARACTERISTIQUES | 92 |
| COMPAT_EQUIPEMENTS | 5 |
| CATALOGUE_OPTIONS | 75 |
| FAISABILITES | 10 |
| TRANSVERSES | 10 |
| **Total** | **589** |

## 9. Industrialisation

**Générateur** (`generateur_tarif_HA76.py`). Lit la feuille unique, groupe les lignes par
modèle, produit les huit fichiers. Fonctions clés : groupement des emplacements d'affichage,
détection anti-fantôme, gel du TTC sur divergence intra-modèle, enveloppe dimensionnelle sur
deux profils, normalisation de l'Ud, restitution du libellé du panneau analogue depuis le
PDF, correction consignée de la longueur de meneau, numérotation SC continue par fichier
depuis SC0002, contrôle du plafond de 200 mots, journal des colonnes remplies non mappées.

Un défaut a été détecté et corrigé lors de la mise au point : la normalisation des espaces
par `\s+` détruisait l'espace fine insécable des montants, transformant `3 247 €` en une
graphie non conforme. La substitution exclut désormais ce caractère.

**Contrôle de conformité** (`controle_conformite_HA76.py`). Audit autonome, sans réutilisation
d'aucune fonction du générateur. Quatorze familles de contrôles : décomptes, plafond, ligne de
source, continuité SC, préfixe de titre, unicité des titres, prose sans puces, front matter,
anti-fantôme et dédoublonnage, fidélité exhaustive des prix modèles, des plus-values d'options,
des Ud et des enveloppes dimensionnelles, des montants de catalogue, absence de tout montant
dans les fichiers d'orientation, non-fuite des prix de la grille gelée, discrimination
HA76 / HAM76, liant inter-fichiers, croisement PDF par échantillon.

Résultat : **31 contrôles réussis, 0 échec, 0 avertissement**. 323 prix modèles, 340
plus-values d'options, 184 valeurs d'Ud et de dimensions et 68 montants de catalogue sont
traçables à une cellule de l'Excel, et la vérification est rejouable à chaque régénération.

Limites connues : la fidélité exhaustive porte sur les montants, l'Ud et les enveloppes
dimensionnelles, non sur les libellés de vitrage ; le croisement PDF est par échantillon ;
l'audit vérifie la conformité, non la qualité rédactionnelle.

## 10. Reste à traiter

- **Grille des fixes** : 553 prix HT et 553 TTC en attente de la règle de lecture (§4).
- **Prix TTC des quinze modèles Porte du mois** : gelés en attente d'arbitrage.
- **Colonnes remplies non mappées** signalées au journal : vues extérieure et intérieure,
  orientation sens A et sens B, images, descriptions de teintes, motifs et accessoires,
  compatibilité PG, limites dimensionnelles du panneau analogue. Décision de traitement
  séparée : chunks visuels, ou hors périmètre tarif.
- **Alignement rétroactif des titres HAM76** : requalifier les 67 chunks HAM76 dont la
  désignation ne porte pas le mot *monobloc*, pour réduire la surface de collision. Relève du
  corpus technique déjà déposé, non de ce chantier.
- **Relecture humaine d'un échantillon** : l'audit ne voit pas la qualité rédactionnelle.
