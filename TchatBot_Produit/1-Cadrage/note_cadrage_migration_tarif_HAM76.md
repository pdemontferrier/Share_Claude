---
document: note_cadrage_migration_tarif_HAM76
version: "1.1"
date_redaction: 2026-09-01
statut: point_de_situation
perimetre: migration du document Tarif HAM76 vers chunks Markdown indexables par le RAG Wikit
gamme: HAM76
gamme_nom: "Porte d'entrée monobloc Aluminium"
collection: "TRYBA ALUMINIUM"
note_parente: note_cadrage_migration_tarif_HA76.md
note_souche: note_cadrage_migration_tarif_H81_v1.md
gammes_pilotes_heritees: [H81, T81, CA76, FT84, HA76]
source_primaire: "HAM76_-infos-tarifs.xlsx (285 lignes, 103 colonnes, feuille unique Feuil1)"
source_controle: "Tarif_HAM76_HT_04-05-2026.pdf (76 pages, édition HT seule)"
livrables: [echantillon_relecture_HAM76.md, Addendum_service_produit_HAM76.md, generateur_tarif_HAM76.py, controle_conformite_HAM76.py, Tarif_HAM76_METHODE.md, Tarif_HAM76_PRIX_MODELES.md, Tarif_HAM76_OPTIONS_MODELES.md, Tarif_HAM76_CARACTERISTIQUES.md, Tarif_HAM76_COMPAT_EQUIPEMENTS.md, Tarif_HAM76_CATALOGUE_OPTIONS.md, Tarif_HAM76_FAISABILITES.md, Tarif_HAM76_TRANSVERSES.md, Message_service_produit_HAM76.md, journal_generation_HAM76.txt]
---

# Note de cadrage — Migration du tarif HAM76 vers Markdown

## 0. Objet et rapport aux notes précédentes

Ce document est une déclinaison spécifique à la gamme HAM76 de la note de cadrage tarif
établie sur HA76, elle-même dérivée de la note souche H81. Il en hérite les principes
généraux — auto-discrimination des chunks, titre auto-porteur préfixé du code gamme,
ligne de source par chunk, plafond de 200 mots, prose sans puces, numérotation SC continue
depuis SC0002, fidélité numérique non négociable, anti-fantôme, exposition des divergences
plutôt que leur résolution silencieuse, arbitrage des libellés manifestement fautifs en
faveur du PDF avec consignation — et conserve l'architecture en huit fichiers de HA76.

Il porte en revanche un **corps de règles renuméroté**, HAM1 à HAM11, et non un réemploi
des identifiants H1 à H10. Ce choix n'est pas cosmétique : cinq règles sur dix changent de
substance sur HAM76, dont une s'inverse. Conserver les mêmes identifiants pour des règles
différentes exposerait toute lecture croisée des deux notes à une confusion.

HA76 avait été choisie comme note parente parce que HA76 et HAM76 sont deux portes
d'entrée aluminium et que leurs tarifs avaient toutes les chances d'avoir la même forme.
Ce pari était partiellement fondé : la structure hybride de l'Excel et le piège des unités
se retrouvent à l'identique, mais le régime tarifaire de HAM76 est nettement plus simple,
et la re-vérification des faits fondateurs a invalidé cinq acquis de HA76.

## 1. Ce que HAM76 change par rapport à HA76

**Le produit.** HAM76 est la porte d'entrée **monobloc** aluminium de la collection TRYBA
ALUMINIUM, conformément au FIP, au CABP et à la Fiche Excellence déjà migrés, et à
l'intitulé imprimé en tête de chacune des 76 pages du tarif. Le discriminant produit
factuel est le panneau monobloc de 86 mm, constitué de deux faces aluminium de 2 mm sur
une âme isolante haute densité de 82 mm collée sur les ouvrants, là où HA76 porte un
panneau de remplissage de 40 mm ou un vitrage.

**Une seule configuration de vantaux.** C'est le fait le plus structurant. La page 10 du
tarif déclare la porte deux vantaux irréalisable ; l'Excel n'a que deux colonnes de prix,
`Tarif HT 1V` et `Tarif TTC 1V` ; les 53 pages modèles ne portent que la mention
« 1 vantail modèle de base ». Là où H81 et HA76 avaient deux configurations, HAM76 n'en a
qu'une. La conséquence dépasse le décompte : l'exigence de clarification du nombre de
vantaux, qui était le cœur de la règle 3 de H81 et de la règle H4 de HA76, devient
nuisible et doit être remplacée par une règle d'énoncé d'indisponibilité.

**Aucun coefficient Ud.** L'Excel ne porte aucune colonne de performance thermique, et le
mot « Ud » n'apparaît nulle part dans le texte exploitable du tarif — la seule valeur
figure sur la fiche info produit insérée en page 11, exclue du périmètre. Le chunk de
caractéristiques perd donc l'un de ses trois éléments constitutifs chez H81 et HA76.

**Un seul couple de dormants dans le bloc modèles.** HA76 en avait deux, H81 quatre. Ici
les 53 modèles sont tous décrits sur le seul couple AL10101 / AL10108, le AL10100 en L70
n'apparaissant qu'au chapitre du ferrage, page 12. L'enveloppe dimensionnelle cesse d'être
une synthèse : elle devient une transcription littérale, et la seule dérogation à la
fidélité brute qu'admettait la règle H6 devient sans objet.

**Aucun second axe descriptif.** HA76 rattachait chaque modèle à une ligne *et* à une
référence commerciale. HAM76 n'a que la ligne de design — Création, Evasion, Intemporelle,
Epurée, Actuelle, Evolution, Nature, Tradition, Harmonie. Les titres en sont d'autant plus
courts.

**La compatibilité des équipements est uniformément négative.** Comme sur HA76, elle est
portée modèle par modèle par un symbole et non par un texte. Mais les 212 cellules portent
une croix rouge : aucun modèle HAM76 n'accepte le judas optique, le heurtoir, le
passe-lettres ni la chatière. L'information reste utile, et devient une information
d'absence.

**Une nature d'information nouvelle.** Le tarif porte une faisabilité des poignées de
tirage extérieures par modèle, absente de HA76 : deux types, poussoir inox et poignée
encastrée, avec une dimension d'ouvrant minimale et deux restrictions particulières.

**Un piège technique nouveau.** Le PDF HAM76 porte des couches de texte dupliquées
caractère à caractère : l'en-tête ressort en `PPoorrttee dd''eennttrrééee`. Toute
extraction doit dédoubler avant usage, faute de quoi la table modèle vers page est vide et
le croisement PDF échoue en silence. C'est l'équivalent HAM76 du piège `\s+` de HA76.

**Le piège de HA76, lui, ne se présente pas.** Le séparateur de milliers du PDF HAM76 est
l'espace ordinaire U+0020, non l'espace fine insécable. La précaution est néanmoins
maintenue à l'écriture, les chunks produits portant bien un U+202F, par cohérence
inter-gammes.

## 2. Faits vérifiés sur HAM76

**Une ligne de l'Excel n'est pas un modèle.** Le fait structurant de HA76 tient, sous une
forme plus régulière : **212 lignes décrivent 53 modèles, à raison de exactement quatre
lignes par modèle, sans exception**. L'emplacement d'affichage est ici l'équipement, chaque
quadruplet portant dans l'ordre judas optique, heurtoir, passe-lettres et chatière. Une
génération ligne à ligne produirait 212 chunks de prix au lieu de 53. Le groupement par
modèle reste une opération préalable obligatoire, et le générateur contrôle que tous les
groupes font quatre lignes.

**Le prix ne varie pas avec la dimension.** Le fait H81 tient. Les colonnes de limites
portent des limites de fabrication, non des paliers de prix.

**Le prix se factorise strictement par modèle.** Zéro divergence intra-modèle sur les 53
modèles, en HT comme en TTC, et de même sur la ligne, les quatre bornes dimensionnelles et
le code gamme. Cinq niveaux de prix seulement : 4 069 € pour un modèle, 4 687 € pour neuf,
4 893 € pour vingt, 5 099 € pour vingt, 5 305 € pour trois.

**Aucun trou de configuration.** Les 53 modèles ont un prix hors taxes et un prix toutes
taxes comprises. L'anti-fantôme sur les vantaux reste codé mais ne trouve rien : c'est
devenu un contrôle de non-régression.

**Répartition par ligne de design.** Création 8, Nature 8, Evolution 7, Evasion 6,
Tradition 6, Intemporelle 5, Epurée 5, Actuelle 5, Harmonie 3. Aucun modèle n'appartient à
deux lignes.

**Le TTC ne se déduit pas du HT.** Le rapport varie de 1,200943 à 1,219710 selon le niveau
de prix des modèles, et bien davantage au catalogue — 1,138 sur ZAE751/400, 1,424 sur la
poignée encastrée, 1,127 sur la rosette. Toute conversion est formellement impossible, et
la règle est ici mieux fondée encore que sur HA76.

**Options rattachées au modèle : deux familles.** Vitrage ornemental et insert de panneau,
pour **128 couples option × modèle porteurs d'un libellé** : 114 vitrages ornementaux sur
38 modèles à 176 €, 73 € et 248 € hors taxes, trois vitrages avec print grille à 0 € sur
Alessia, Eliane et Leslie, et onze inserts à 0 € — Insert Corten sur Ode, Aria et Cadence,
Insert Chêne Blanchi sur huit modèles de la ligne Nature. Onze modèles n'ont aucune option,
quatre en ont une, vingt-huit en ont trois, dix en ont quatre.

**Les plus-values nulles sont des options incluses, pas des renvois.** Vérifié sur la
donnée : chacune des quatorze porte sur sa page modèle un visuel dédié, un cartouche
« Option de vitrage panneau » ou « Option d'insert panneau », et la mention « PV sur le
prix du modèle de base ». Aucune ne renvoie à une page transverse. C'est le cas de figure
HA76, et non celui de H81 où une plus-value nulle était toujours un renvoi.

**L'anti-fantôme se déplace du montant vers le libellé.** 264 cellules de plus-value
portent la valeur zéro sans aucun libellé : 70 sur le vitrage, 194 sur l'insert. Générer
sur la présence d'un montant produirait 264 chunks d'options inexistantes. Le critère est
donc la présence du libellé, jamais celle du montant.

**Compatibilité des équipements.** Quatre équipements, 212 cellules, 212 croix rouges.
Aucun équipement n'est chiffré, donc aucune bascule vers les options.

**Faisabilité des poignées de tirage.** 106 couples : poussoir inox réalisable sur 46
modèles, poignée encastrée sur 39, dimension d'ouvrant minimale de 800 mm sur les 85
couples réalisables. Cohérence parfaite entre le symbole, la mention Oui ou Non et la
présence de la dimension. Deux restrictions particulières : Fuji en PR500 uniquement,
Bérénice hors PR 500.

**Catalogue d'options hors modèle.** Six tableaux portent 24 lignes tarifées sans
rattachement à un modèle. Vingt sont retenues, quatre écartées faute d'attestation au PDF.
S'y ajoutent trois options chiffrées présentes au seul PDF.

**Correspondance des pages.** La page PDF vaut la page imprimée, vérifiée sur les 73 pieds
de page numérotés de 2 à 75, et non sur le sommaire. **Aucun décalage**, contrairement à
HA76 dont le sommaire décalait de quatre pages toute sa section arrière. Les pages modèles
vont de 19 à 71, un modèle par page. Les transverses sont aux pages 4 à 18 en amont et 72 à
75 en aval.

**Pages étrangères.** La page 11 est une fiche info produit HAM76 datée 10-2025, insérée
sans numéro de page, exclue du périmètre tarif pour éviter la double source — mais voir la
divergence D14. La page 76 est une quatrième de couverture portant un résidu textuel de
tarif de portes de garage de juillet 2018, hors périmètre. C'est le pendant exact de la
page 137 de HA76.

## 3. Divergences exposées et non arbitrées

Quatorze écarts ont été relevés et remontés au service Produits par un message dédié.
Aucun n'est tranché par la migration, à l'exception de trois libellés manifestement fautifs
ou absents.

**D1 à D2 — références tarifées dans l'Excel et absentes du PDF publié — bloquant.** Trois
poussoirs, ZAE35/400 à 364 € HT, ZAE35/800 à 494 € HT et ZAE351200 à 598 € HT, ainsi qu'une
béquille inox BDE-DG/O à 0 €, n'apparaissent nulle part dans les 76 pages du document
publié. Divergence de périmètre non arbitrable : les quatre références sont écartées du
corpus en attendant réponse, et chacune est nominativement journalisée.

**D3 — limites dimensionnelles contradictoires — bloquant.** La page 9 annonce, comme
valables pour tous les modèles de la gamme, des limites de 810 à 1160 mm en largeur et de
2075 à 2280 mm en hauteur, quand les 53 pages modèles portent 798 à 1248 mm et 1892 ou
2065 à 2445 mm, pour les mêmes dormants. Écart de 88 mm en largeur maximale et de 165 mm en
hauteur maximale. Les deux jeux sont exposés dans le corpus avec attribution de page,
aucun n'est arbitré.

**D4 — porte deux vantaux — bloquant.** La page 10 la déclare irréalisable, la page 12
donne des limites de réalisation en hauteur pour deux vantaux. Le corpus suit la page 10 et
les 53 pages modèles, et sert une réponse d'indisponibilité.

**D5 — codes gamme aberrants.** La colonne « Gamme description » de l'Excel porte HAM77 à
HAM88 sur les douze lignes des modèles Aria, Cadence et Cantate, par recopie incrémentée
involontaire. La colonne voisine « Gamme » porte bien HAM76 sur les 212 lignes. **Cette
colonne est interdite de lecture** dans la chaîne de génération, et un contrôle d'audit
dédié vérifie qu'aucune de ces désignations n'a fui dans le corpus.

**D6 — libellé de béquilles inversé — tranché en faveur du PDF.** L'Excel porte
« BDEL int - BPEL int » à 102 € alors que la page 72 tarife la combinaison BDEL/BPEL à 0 €
et BDSL/BPSL à 102 €. S'agissant d'un libellé et non d'un montant, la valeur du PDF est
retenue et la correction consignée au journal.

**D7 — libellés absents alors que la donnée existe — restitués depuis le PDF.** La rosette
encastrée à 55 € n'a pas de désignation dans l'Excel ; la page 73 l'identifie « rosette
encastrée inox, niveau de sécurité R20, réf. ROC ». La famille « Vitrage ornemental » est
vide sur les trois lignes du modèle Indigo, seul cas sur 38 modèles ; la page 27 porte le
cartouche. Les deux libellés sont restitués et consignés. Cas identique au « Panneau
analogue » de HA76.

**D8 — modèle Titane absent du classement alphabétique de la page 9**, qui liste 52 modèles
pour 53. Sans effet sur la génération.

**D9 — compatibilité porte de garage.** L'Excel dit « Applique pour PGS et PGSL » sur Gong
et Opéra là où le PDF imprime « Hublot », et les visuels rattachés sont faux sur trois
modèles sur cinq : Gong pointe des visuels Salsa, Opéra des visuels Aria, Brume des visuels
d'un modèle Madine étranger à la gamme. Chapitre écarté du corpus.

**D10 — coquilles présentes dans les deux sources.** « Vitragre avec print grille. » sur
trois modèles et « appliqus inox » sur Résine figurent à l'identique dans l'Excel et dans
le PDF. Il n'y a donc pas de divergence à arbitrer : les libellés sont **transcrits
littéralement**, et la correction est demandée aux deux sources.

**D11 — options chiffrées absentes de l'Excel.** Ferrage six points automatique et
changement de teinte des paumelles en page 12, plinthe automatique en page 13, toutes à
0 €. Elles sont **captées depuis le PDF** et intégrées : ne pas les servir laisserait le
chatbot muet sur trois options réellement offertes.

**D12 — colonne de faisabilité mêlant deux natures.** La colonne attendue en Oui ou Non
porte une remarque sur Fuji et Bérénice, en doublon de la colonne prévue. Le symbole fait
foi.

**D13 — 264 cellules de plus-value à zéro sans option correspondante.** Traitées par
l'anti-fantôme sur le libellé.

**D14 — fiche info produit insérée en 10-2025** alors que le corpus technique HAM76 porte
la version 04-2024. Hors périmètre tarif, mais remontée prioritaire : le corpus technique
sert peut-être une version périmée de dix-huit mois.

**D15 — « anti-dégondage » attribué à la paumelle.** Découverte tardivement, lors de la
préparation de l'échantillon de relecture. La page 13 énonce que la configuration à trois
lames de la paumelle « apporte donc un système anti-dégondage directement dans la
paumelle ». Deux règles du projet s'y opposent : la gouvernance du vocabulaire retient
anti-décrochement et bannit anti-dégondage, et la discipline des catégories veut que la
paumelle relève de la tenue et non de l'anti-effraction — défaut récurrent principal de la
campagne de test de phase 2.

Le défaut est de **niveau source** : il n'est donc pas corrigé. La transcription reste
littérale, et un chunk de vocabulaire porté en information complémentaire dans METHODE
pose le terme retenu au niveau du corpus, sur le modèle de celui de la Fiche Excellence
T81. Ce chunk énonce en outre que la classification du dispositif au regard de la
résistance à l'effraction n'est pas donnée par le tarif et ne peut donc pas en être
déduite — ce qui borne le risque sans arbitrer la source. Le point fait l'objet d'un
addendum au message adressé au service Produits.

## 4. Chapitre gelé — la grille des fixes

La page 17 indique que la table de tarif des fixes vitrés est « en lecture directe, en
fonction de la dimension L x H du fixe », et **n'énonce aucune règle d'intervalle** —
exactement comme la page 28 de HA76, et contrairement à la page 10 du tarif T81 qui
précisait qu'une colonne couvre une bande de largeurs. Or c'est ce fait qui rendait
l'architecture T81 tenable : sans lui, toute demande hors cote ronde exige un arrondi,
c'est-à-dire un raisonnement numérique proscrit par la règle HAM4.

La table compte 24 hauteurs par 24 largeurs, de 300 à 2600 mm par pas de 100 mm, soit **553
prix hors taxes et 553 prix toutes taxes comprises** et 23 cases vides rigoureusement
identiques en HT et en TTC. Ces trous sont cohérents avec le PDF : la case 300 × 300 y est
explicitement signalée techniquement irréalisable, les 22 autres sont blanches dans l'angle
des grandes dimensions.

Ces 1106 prix ne sont pas intégrés. Un chunk d'orientation renvoie à la page 17.
L'intégration est conditionnée à la précision de la règle de lecture par le service
Produits, ainsi qu'à la confirmation que les 22 cases vides correspondent bien à des
combinaisons irréalisables et non simplement non tarifées. Ces deux demandes sont reportées
telles quelles dans le message qui leur est adressé.

## 5. Architecture en huit fichiers

Le principe reste celui de HA76 : un fichier par nature d'information, chacune avec sa
maille propre.

**METHODE**, cadre de lecture : portée du tarif et distinction avec HA76 et H81, unité
tarifaire, indépendance du prix et de la dimension, configuration unique de vantaux,
interdit de calcul entre HT et TTC, unités non forfaitaires, périmètre absent, portée des
limites de fabrication, et vocabulaire retenu. Ce dernier chunk est le seul du corpus tarif
porté en information complémentaire.

**PRIX_MODELES**, un chunk par modèle, la configuration étant unique.

**OPTIONS_MODELES**, un chunk par couple option × modèle porteur d'un libellé, plus-values
chiffrées et plus-values explicitement nulles.

**CARACTERISTIQUES**, un chunk par modèle réunissant la description du modèle de base et
l'enveloppe dimensionnelle, avec l'énoncé explicite de l'absence de coefficient Ud au tarif.

**COMPAT_EQUIPEMENTS**, un chunk par équipement et un chunk par type de poignée de tirage
extérieure.

**CATALOGUE_OPTIONS**, un chunk par référence d'option tarifée indépendamment du modèle.

**FAISABILITES**, faisabilités non tarifaires : typologie des châssis, sens d'ouverture et
sens du panneau, limites dimensionnelles générales avec exposition de D3, limites de
réalisation en hauteur et gâches ponctuelles, paumelles et réglages, seuil et plinthe
automatique, couleur des accessoires, réalisation des fixes et des meneaux, largeur de
passage, configuration unique de la gamme.

**TRANSVERSES**, un chunk par bloc, nommant l'existence et la logique tarifaire sans
reproduire aucun montant ni aucun pourcentage.

## 6. Règles normatives HAM76 — HAM1 à HAM12

**HAM1 — Découpage des prix.** Un chunk par modèle réellement tarifé. Anti-fantôme
préalable maintenu bien qu'il ne trouve rien : il vaut contrôle de non-régression. Reprend
H1 de HA76.

**HAM2 — Normalisation préalable des emplacements d'affichage.** Le modèle, et non la ligne
de l'Excel, est l'unité de génération. Le générateur contrôle que chaque groupe fait
exactement quatre lignes. Reprend H2.

**HAM3 — Rédaction des prix.** Une phrase liant le prix HT et le prix TTC, sans grille. La
configuration cesse d'être un discriminant entre chunks et devient une constante de gamme,
énoncée « en un vantail, seule configuration réalisable de la gamme ». Reprend H3, allégée.

**HAM4 — Instruction LLM.** **Règle inversée.** L'exigence de clarification du nombre de
vantaux, cœur de H4, est supprimée : elle ferait poser une question dont la réponse est
unique. Elle est remplacée par une règle d'énoncé : une demande de prix en deux vantaux
reçoit une réponse d'indisponibilité et un renvoi à la page 10, jamais un montant.
L'interdiction de calcul, d'interpolation et de conversion entre HT et TTC est maintenue et
renforcée.

**HAM5 — Options rattachées au modèle.** Un chunk par couple option × modèle. Sont servies
les plus-values chiffrées et les plus-values explicitement nulles, énoncées « sans
plus-value ». **L'anti-fantôme porte sur la présence du libellé, jamais sur celle du
montant.** Extension de H5, dont la sémantique du zéro est confirmée sur la donnée et non
par analogie.

**HAM6 — Caractéristiques.** Un chunk par modèle, réunissant la description du modèle de
base et l'enveloppe dimensionnelle sur l'unique couple de dormants. La transcription est
intégralement littérale : la dérogation admise par H6 pour l'enveloppe calculée devient
sans objet. Le chunk rappelle que ces limites valent pour une menuiserie blanche à serrure
six points, et énonce que le tarif ne porte aucun coefficient Ud.

**HAM7 — Compatibilité des équipements.** Un chunk par équipement. La faisabilité se lit
sur un symbole. **La règle admet et sert l'absence totale** : les quatre chunks énoncent
qu'aucun modèle n'accepte l'équipement et qu'une demande doit recevoir une réponse
d'indisponibilité et non un prix. Cas de gestion bidirectionnelle de l'absence déjà prévu
dans Instructions.md.

**HAM8 — Catalogue d'options hors modèle.** Un chunk par référence forfaitaire, titre
portant le chapitre d'origine, corps précisant que le montant est indépendant du modèle.
Le critère d'inclusion est **l'attestation de la référence dans le PDF publié** : il écarte
les quatre références de D1 et D2, et fait entrer les trois options de D11. Reprend H8,
avec un critère explicite là où HA76 se contentait de l'Excel.

**HAM9 — Unités non forfaitaires.** Tout montant exprimé au mètre carré ou en pourcentage
est exclu du corpus chiffré et traité en orientation seule. S'applique aux 20 lignes de
plus-values de vitrages pour fixes, dont l'unité n'existe que dans le PDF, et aux
plus-values de l'offre couleurs. Reprend H9.

**HAM10 — Transverses.** Un chunk par bloc transverse, nommant la nature et la logique
tarifaire, renvoyant à la page, sans aucun montant ni pourcentage. Le taux de la hausse
générale du 1er mai 2026 n'est pas servi, son usage supposerait de l'appliquer à un prix.
Reprend H10.

**HAM11 — Faisabilité des poignées de tirage extérieures.** *Règle nouvelle.* Un chunk par
type de poignée, listant les modèles réalisables, la dimension d'ouvrant minimale et les
restrictions particulières. Même maille que HAM7, dont elle partage la forme de question.

**HAM12 — Coquille orthographique manifeste dans un libellé porté en titre.** *Règle
nouvelle.* Lorsque l'Excel et le PDF portent la **même** coquille manifeste sur un libellé
d'option, il n'y a pas de divergence à arbitrer et la règle héritée de CA76 ne s'applique
pas. Le corpus rétablit alors l'orthographe **dans le titre seulement**, et consigne la
graphie du tarif verbatim dans le corps du chunk. Justification : le titre est une clé de
récupération, non une transcription — une coquille y dégrade l'indexation sans rien
apporter, tandis que le corps conserve la trace exacte de la source. La règle est bornée :
elle ne s'applique ni aux montants, ni aux codes de référence, ni aux descriptions
transcrites dans le corps d'un chunk. Elle a servi une fois, sur « Vitragre avec print
grille », rétabli au titre de trois chunks. La coquille « appliqus inox » du modèle Résine
reste, elle, transcrite littéralement : elle figure dans une description de corps et non
dans un titre.

## 7. Format des chunks et discrimination HAM76 / HA76

Le préfixe retenu est **`HAM76 Porte d'entrée monobloc Aluminium — `**.

Ce choix rompt délibérément avec le critère qui avait fixé le préfixe HA76, à savoir la
désignation *dominante* du corpus technique. Sur les 91 chunks HAM76 déjà déposés, 38
portent « HAM76 Porte d'entrée aluminium », 29 « HAM76 Porte Aluminium » et seulement 24
« HAM76 Porte d'entrée monobloc Aluminium » — soit **67 chunks, 73,6 %, dont la désignation
est strictement identique, au code de gamme près, à une désignation en usage sur HA76**. La
désignation dominante est donc précisément celle qui produit la collision. Est retenue à sa
place la désignation *attestée et discriminante* : elle figure dans les trois front matters
techniques, dans 24 chunks déposés, et surtout en tête de chacune des 76 pages du tarif.
Aucune désignation nouvelle n'a été fabriquée. Sur HA76 les deux critères coïncidaient ; ici
ils divergent, et la discrimination l'emporte.

La discrimination repose sur quatre leviers cumulés, tous vérifiés par l'audit : le code de
gamme en tête de titre, le mot *monobloc* présent dans les 287 chunks, le nom du modèle au
titre, et la mention explicite « de la gamme HAM76 » dans le corps des 234 chunks de prix,
d'options et de caractéristiques. Un chunk de tête du fichier METHODE énonce en outre que
le tarif ne s'applique ni à HA76 ni à H81, en s'appuyant sur la phrase du tarif lui-même en
page 10 — « contrairement aux portes H81 et HA76, le sens du panneau est déterminé par le
sens d'ouverture de la porte ». La distinction est donc sourcée dans le document et non
asserée par la migration.

Deux contrôles négatifs complètent le dispositif : HA76 et H81 ne peuvent apparaître que
dans un énoncé de distinction explicite, la recherche se faisant sur mot entier puisque
HA76 n'est pas un sous-mot de HAM76 ; et aucune désignation HAM77 à HAM88 ne peut avoir fui
depuis la colonne interdite.

**Risques résiduels.** Le premier est l'asymétrie interne au corpus HAM76 : les chunks
tarif portent « monobloc », les 67 chunks techniques non. Une question sans mot discriminant
peut donc encore ramener un chunk technique HAM76 ou HA76. L'alignement rétroactif de ces
67 titres reste ouvert au §10 de la note HA76 ; ce chantier ne le clôt pas. Le second est
la collision intra-gamme entre noms de modèles et vocabulaire couleur : les modèles
*Titane*, *Agate*, *Graphite*, *Emeraude* et *Grès* coexistent avec les teintes de paumelles
« titane » et les RAL « Gris agate 7038 » et « Gris graphite 7024 », et le modèle *Marine*
avec le champ lexical de l'environnement marin, déjà source d'un défaut remonté. Le
troisième, l'homonymie de modèles entre HAM76 et HA76, n'a pas pu être levé : la table
modèle vers page de HA76 était externalisée et son fichier n'est pas au projet.

Formes de titre :

## HAM76 Porte d'entrée monobloc Aluminium — Tarif Opus 1 vantail (ligne Création)
## HAM76 Porte d'entrée monobloc Aluminium — Option Mastercarré Isol'3 44/2-16G-4 sur Indigo (ligne Evasion)
## HAM76 Porte d'entrée monobloc Aluminium — Caractéristiques Opus (ligne Création)
## HAM76 Porte d'entrée monobloc Aluminium — Compatibilité de l'équipement chatière par modèle
## HAM76 Porte d'entrée monobloc Aluminium — Faisabilité de la poignée de tirage extérieure poussoir inox par modèle
## HAM76 Porte d'entrée monobloc Aluminium — Tarif catalogue poussoirs inox : ZAE751/400
## HAM76 Porte d'entrée monobloc Aluminium — Existence et localisation des tarifs de l'offre couleurs, groupes 1 et 2

Ligne de source : `*Source : Tarif—HAM76—HT—04-05-2026.pdf, page N — information originale —
SCnnnn*`, les formes `pages 19 à 71` et `pages 9 et 19 à 71` étant admises pour les chunks
dont la maille couvre l'ensemble des pages modèles.

## 8. Décomptes

| Fichier | Chunks |
|---|---|
| METHODE | 9 |
| PRIX_MODELES | 53 |
| OPTIONS_MODELES | 128 |
| CARACTERISTIQUES | 53 |
| COMPAT_EQUIPEMENTS | 6 |
| CATALOGUE_OPTIONS | 23 |
| FAISABILITES | 10 |
| TRANSVERSES | 6 |
| **Total** | **288** |

288 chunks pour 76 pages, contre 589 pour les 137 pages de HA76. L'écart tient à trois
faits : une seule configuration de vantaux au lieu de deux, 53 modèles au lieu de 92, et
l'absence de Ud qui allège les caractéristiques sans en réduire le nombre.

## 9. Industrialisation

**Générateur** (`generateur_tarif_HAM76.py`). Lit la feuille unique, groupe les lignes par
modèle en contrôlant la taille des groupes, construit la table modèle vers page et produit
les huit fichiers. Fonctions clés : dédoublement du texte du PDF, construction de la table
modèle vers page par lecture des en-têtes avec **arrêt si la couverture n'est pas de
100 %** — le gabarit HA76 repliait sur une page par défaut, ce qui masquait le défaut —,
neutralisation de la colonne de désignation aberrante, anti-fantôme sur le libellé,
restitution consignée de deux libellés et correction consignée d'un troisième, captation
de trois options depuis le PDF, écartement journalisé de quatre références non attestées,
numérotation SC continue par fichier depuis SC0002, contrôle du plafond de 200 mots,
journal des colonnes remplies non mappées.

Sur 103 colonnes : 24 alimentent le corpus, 49 portent la grille gelée, 8 sont vides, 19
sont remplies et non mappées et sont journalisées. Trois appellent une décision séparée :
la colonne de désignation aberrante, les motifs et accessoires avec leurs 34 schémas — qui
seraient la matière de chunks visuels —, et la compatibilité porte de garage, écartée pour
cause de libellés et de visuels erronés.

**Contrôle de conformité** (`controle_conformite_HAM76.py`). Audit autonome, sans
réutilisation d'aucune fonction du générateur : il relit les .md, l'Excel et le PDF, et
recalcule toute constante utile depuis les sources. Dix-huit familles de contrôles :
décomptes, plafond, ligne de source avec vérification qu'aucune page exclue n'est citée,
continuité SC, préfixe de titre, unicité des titres, prose sans puces, front matter,
anti-fantôme sur les prix et sur les options, fidélité exhaustive des prix modèles, des
plus-values d'options, des enveloppes dimensionnelles et des descriptions de modèle de base,
des montants de catalogue, attestation dans le PDF des libellés restitués, absence de tout
montant dans les fichiers d'orientation, non-fuite de la grille gelée et des pages à unité,
discrimination inter-gammes, liant inter-fichiers, croisement PDF, et gouvernance du
vocabulaire — tout faux synonyme présent dans le corpus doit être couvert par un chunk
nommant le terme retenu, et toute coquille rétablie au titre doit voir sa graphie d'origine
consignée dans le corps.

Résultat : **26 contrôles réussis, 0 échec, 0 avertissement**. 106 prix modèles, 242 valeurs
de plus-value, 212 bornes dimensionnelles, 53 descriptions de modèle de base et 36 montants
de catalogue sont traçables à une cellule de l'Excel, et la vérification est rejouable à
chaque régénération.

Le croisement PDF est ici **exhaustif** et non par échantillon comme sur HA76 : les 53
modèles, leur nom, leur prix hors taxes et l'absence de mention de deux vantaux sont
vérifiés page à page.

Deux contrôles ont dû être repris lors de la mise au point, et l'erreur venait dans les deux
cas de l'audit et non du corpus : un rapprochement du catalogue par sous-chaîne qui
confondait ZAE751 et ZAE751/400, et un test de non-fuite des montants au mètre carré qui
déclenchait sur une simple collision d'entiers — 73 € est à la fois une plus-value de
vitrage pour fixe et le prix d'un Mastercarré. Le second a été remplacé par l'invariant
réel : aucun chunk sourcé sur les pages 17 ou 18 ne porte de montant.

Limites connues : la fidélité exhaustive porte sur les montants et les dimensions, non sur
les libellés de vitrage ; l'audit vérifie la conformité, non la qualité rédactionnelle.

## 10. Reste à traiter

- **Grille des fixes** : 553 prix HT et 553 TTC en attente de la règle de lecture, et
  confirmation du statut des 22 cases vides de l'angle des grandes dimensions (§4).
- **Quatre divergences bloquantes** D1 à D4, en attente d'arbitrage du service Produits.
- **Version de la fiche info produit** (D14) : si la fiche 10-2025 fait foi, le corpus
  technique HAM76 est à rafraîchir. Relève du corpus technique, non de ce chantier.
- **Colonnes remplies non mappées** signalées au journal : vues extérieure et intérieure,
  images d'équipements et d'options, motifs et accessoires avec leurs 34 schémas,
  compatibilité porte de garage. Décision de traitement séparée : chunks visuels, ou hors
  périmètre tarif.
- **Alignement rétroactif des titres HAM76** : requalifier les 67 chunks techniques dont la
  désignation ne porte pas le mot *monobloc*, pour réduire la surface de collision avec
  HA76. Relève du corpus technique déjà déposé.
- **Homonymie de modèles HAM76 / HA76** : à lever dès que la liste des 92 modèles HA76 sera
  disponible.
- **D15 — « anti-dégondage »** : en attente d'arbitrage produit sur le libellé de la page 13.
- **Consignes de comportement dans les chunks de contenu** : plusieurs chunks de METHODE et
  de COMPAT_EQUIPEMENTS prescrivent une conduite au modèle, ce que faisait déjà HA76. La
  place naturelle d'une telle consigne est `Instructions.md`, et la dupliquer crée deux
  sources pour une même règle. **Décision de niveau projet, non tranchée ici** : la
  convention de HA76 est conservée pour ne pas désaligner un corpus déjà déposé à quelques
  jours du déploiement. À trancher une fois pour les cinq gammes restantes.
- **Relecture humaine d'un échantillon** : 41 chunks sur 288 signalés dans
  `echantillon_relecture_HAM76.md`. L'audit ne voit pas la qualité rédactionnelle — la
  préparation de cet échantillon a d'ailleurs révélé seize accords fautifs invisibles à
  tout contrôle numérique.
