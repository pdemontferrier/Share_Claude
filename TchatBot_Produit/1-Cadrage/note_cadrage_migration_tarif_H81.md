---
document: note_cadrage_migration_tarif
version: "1.1"
date_redaction: 2026-07-29
date_maj: 2026-09-01
statut: point_de_situation_fige
perimetre: migration du document Tarif vers chunks Markdown indexables par le RAG Wikit
gamme_pilote: H81
gammes_cibles: [HA76, HAM76, H81, H81_Access, T81, TA76_OV, TA76_OC, CA76, CA80_New, FT84]
source_primaire: "H81-modèles_porte.xlsx (base structurée déjà constituée)"
source_controle: "Tarif_H81_HT_08-04-2026.pdf (133 pages, référence de fidélité)"
note_parente: note_cadrage_migration.md
livrables_h81: [generateur_tarif.py, controle_conformite.py, Tarif_H81_PRIX.md, Tarif_H81_OPTIONS.md, Tarif_H81_CARACTERISTIQUES.md, Tarif_H81_COMPAT_EQUIPEMENTS.md, Tarif_H81_PAGES_TRANSVERSES.md]
---

# Note de cadrage — Migration du Tarif vers Markdown

## 0. Objet et statut de ce document

Ce document fige un point de situation sur la stratégie de migration du **document Tarif**
vers des chunks Markdown indexables par le RAG Wikit. Il a été établi à partir de l'analyse
du tarif H81 (PDF du 08-04-2026 et fichier Excel associé), pris comme gamme pilote. Il est
destiné à survivre à l'évolution du contexte : il consigne non seulement les règles retenues,
mais le raisonnement et les faits vérifiés qui les fondent, afin qu'une reprise ultérieure
n'ait pas à refaire le chemin.

Ce cadrage est une **déclinaison spécifique au tarif** de la note de cadrage générale de
migration. Il ne s'y substitue pas : il en hérite les principes (auto-discrimination des chunks,
titres auto-porteurs préfixés du code gamme, ligne de source par chunk, plafond de 200 mots,
prose sans puces, numérotation SC continue depuis SC0002) et en spécialise l'application au cas
particulier du tarif.

## 1. Pourquoi le tarif est un cas spécifique

Le tarif se distingue des documents rédactionnels (FIP, CABP, FE) sur deux plans.

D'abord, sa matière est **numérique et critique** : un prix erroné n'est pas une imprécision
tolérable mais une faute directement visible par l'ADV et le client. Le risque d'hallucination
tarifaire — un modèle léger qui recopie le mauvais nombre, confond une colonne avec sa voisine
ou mélange HT et TTC — est le risque directeur qui commande toute l'architecture.

Ensuite, sa source n'est pas un texte à extraire mais une **base déjà structurée** : le fichier
Excel associé au tarif contient, modèle par modèle, l'essentiel des données (prix, dimensions,
options, teintes, performances). Cet Excel est donc pris comme **source primaire** de la
migration, le PDF servant de **référence de contrôle** pour la fidélité numérique et pour capter
ce que l'Excel ne couvre pas encore.

## 2. Voies écartées et voie retenue

Trois voies ont été examinées pour restituer les prix au chatbot.

La première, un **outil de calcul de prix appelé via function calling / MCP**, est la plus sûre
sur le papier : le prix est lu de façon déterministe par une fonction, jamais rédigé par le
modèle. Elle a été **écartée** pour la mise en œuvre à court terme. Elle suppose trois conditions
indépendantes (que Wikit sache appeler un outil MCP dans l'offre souscrite ; que le serveur
d'outil soit développé et hébergé ; que la base qu'il interroge existe), dont la deuxième et la
troisième supposent un développement logiciel et une maintenance récurrente sans appui DSI. Dans
le délai contraint (mise en ligne des dix gammes à brève échéance) et sans relais informatique
interne, cette voie n'est pas réaliste.

La deuxième, un **JSON déposé et indexé** dans la bibliothèque SharePoint, est faisable
immédiatement mais réintroduit le risque directeur : un objet de données brut (accolades,
clés, valeurs) ne ressemble pas à une question d'ADV, se récupère mal, et se lit mal par un
modèle léger qui peut attraper la valeur d'une clé voisine.

La voie **retenue** est le **chunk de prix en prose**, généré automatiquement depuis l'Excel.
Chaque chunk énonce un prix en toutes lettres dans une phrase qui épouse la formulation d'une
question réelle. Le RAG le récupère bien parce qu'il ressemble à la réponse attendue ; le modèle
ne « navigue » pas dans une grille, il restitue une phrase où le prix est déjà lié sans
ambiguïté à sa configuration. La fidélité est garantie par construction : un script recopie la
valeur de la cellule, il n'invente rien. Cette voie ne demande aucun serveur, aucun hébergement,
aucune maintenance logicielle ; à chaque révision tarifaire, on régénère les chunks et on les
redépose, exactement comme pour les fichiers rédactionnels.

Le risque résiduel — le prix reste lu par le modèle, non calculé par une fonction — est
fortement réduit par la mise en prose et par des instructions strictes (voir §6), mais n'est
pas nul. Son acceptation relève des sponsors du projet, pas d'une décision technique.

## 3. Architecture en cinq fichiers par nature d'information

Le principe directeur du découpage n'est pas « un fichier par document » ni « un fichier par
référence », mais **un fichier par nature d'information**. Chaque nature a sa propre logique de
variation, donc sa propre maille de factorisation. Cet éclatement évite de recopier dans chaque
référence des informations partagées, et maintient chaque chunk sous le plafond de 200 mots.

Les cinq fichiers sont les suivants.

**Fichier PRIX.** Il porte les tarifs. Le prix est une propriété du **modèle individuel** (voir
faits vérifiés au §4) : il ne varie ni avec la dimension, ni de façon lisible avec la collection
ou la ligne. Ce fichier ne se factorise donc pas — il contient un chunk par référence et par
configuration de vantaux.

**Fichier OPTIONS ET PLUS-VALUES.** Il porte les options et leurs plus-values chiffrées. La
plus-value se factorise au niveau de la donnée (une même option, par exemple le vitrage sécurité
44/6 à +196 € HT, revient sur des dizaines de modèles avec la même valeur), mais la maille de
chunk retenue est le **couple option × modèle**, par symétrie avec le fichier prix et pour la
même raison : l'ADV interroge une option pour un modèle précis (« combien coûte le vitrage
sécurité sur Azurite »). Un chunk par couple option × modèle épouse donc la requête réelle sans
ambiguïté. Seuls les couples dont la **plus-value est réellement chiffrée** donnent un chunk (voir
§4) ; les options renvoyant à une page transverse (Offre couleurs, Plus-value vitrages) sans
montant sur la ligne modèle sont exclues de ce fichier et relèvent du traitement des pages
transverses.

**Fichier CARACTÉRISTIQUES COMMUNES.** Il porte les caractéristiques techniques indépendantes du
prix : vitrage de base, performance thermique (Ud) et plage dimensionnelle. La maille retenue est
**un chunk par modèle** réunissant ces trois éléments. Ce choix privilégie l'autonomie de la
référence sur la factorisation : les trois caractéristiques n'ont pas le même axe de variation
(l'Ud se factorise par valeur, le vitrage est quasi individuel, les plages sont inégalement
partagées), si bien qu'aucune maille factorisée unique ne les couvre ; regrouper par modèle rend
chaque référence complète et récupérable d'un seul chunk, au prix d'une redondance dimensionnelle
assumée (une plage partagée est réécrite dans chaque chunk concerné). La plage est restituée sous
forme d'**enveloppe** (largeur et hauteur min/max tous profils confondus), pas profil par profil,
pour rester lisible et sous le plafond ; le tarif n'est pas le lieu du détail dimensionnel fin.

**Fichier COMPATIBILITÉ ÉQUIPEMENTS.** Il porte la compatibilité de montage des équipements
(judas optique, heurtoir, passe-lettres, chatière) — information distincte des trois autres :
les équipements **ne portent aucun prix** dans le tarif (donc hors fichier options), et leur
compatibilité par modèle est **absente du corpus technique** (le FIP ne cite que quelques
accessoires au niveau de la gamme, sans compatibilité par modèle), donc sans duplication. La
maille retenue est **un chunk par équipement**, listant les modèles compatibles — l'information
est nativement « par équipement » (quels modèles acceptent la chatière) et se factorise donc à
ce niveau. Ce fichier est distinct des caractéristiques communes car sa maille (par équipement)
diffère de la leur (par modèle). Cas particulier : le poussoir incurvé, propre à un seul modèle
et **chiffré**, n'est pas un équipement de compatibilité mais une option — il relève du fichier
options (règle 4), pas de ce fichier.

**Fichier PAGES TRANSVERSES (orientation).** Il porte l'existence et la localisation des tarifs
transverses — teintes (plaxage, offre couleurs, laquage RAL) et plus-values vitrages — qui sont
définis dans des pages dédiées du tarif, indépendantes des pages-modèles. Décision de périmètre :
ces prix **ne sont pas migrés** ; seule leur existence l'est. Motif : ces grilles expriment des
plus-values en **pourcentage** (plaxage +15 %, +25 %), **au mètre linéaire** (laquage accessoires
€/ml) ou **au mètre carré** (vitrages €/m²), qui supposent un calcul — précisément ce que la règle
de sécurité interdit au LLM (règle 3). Y placer ces montants reviendrait à mettre dans le corpus
des nombres inutilisables sans enfreindre cette règle, sur des grilles longues et à extraction PDF
fragile (risque directeur maximal). La maille est **un chunk par bloc transverse** (teintes,
laquage, vitrages) : chaque chunk nomme l'existence et la logique tarifaire, renvoie à la page du
tarif, et ne reproduit aucun montant. L'ADV est orienté vers la source pour le calcul.

### Raisonnement derrière l'éclatement

L'éclatement répond directement au problème central : **une référence porte une grande quantité
d'informations hétérogènes**, et les mélanger dans un chunk unique le rend illisible pour le
modèle et noie le prix, tandis que tout éclater en chunks minuscules sature l'index. Le bon grain
s'obtient en regroupant par **intention de question** (un chunk = ce qu'un ADV vient chercher
d'un seul tenant) et en factorisant par **nature de variation** (chaque information vit là où sa
logique de variation la place).

Ce découpage a une **contrepartie assumée** : il découple le prix des caractéristiques. Le prix
d'un modèle est dans le fichier prix, ses dimensions dans le fichier caractéristiques, ses
options dans le fichier options — trois chunks dans trois fichiers pour une même référence. Le
**liant** est le préfixe de titre commun (code gamme + modèle), répété à l'identique dans les
trois fichiers, qui permet au RAG de rassembler les morceaux d'une même référence quand une
question porte sur le produit entier. Ce liant doit être encodé dès le gabarit.

Un point de vigilance subsiste sur l'efficacité de l'index : le découplage impose que chaque
chunk soit auto-porteur (le chunk prix doit nommer son modèle, sa ligne et sa collection ;
le chunk options doit nommer les modèles qu'il couvre), faute de quoi la factorisation gagnée
se paierait en perte de récupérabilité.

## 4. Faits vérifiés sur H81 (fondements de l'architecture)

Les décisions ci-dessus reposent sur des vérifications menées sur les 84 références de H81.
Elles sont consignées ici parce qu'elles fondent les règles et devront être re-vérifiées sur
chaque gamme avant génération.

Le **prix ne varie pas avec la dimension**. Aucune des 84 références n'a plusieurs prix selon le
profil : le tarif est forfaitaire par référence, la plage dimensionnelle étant une limite de
fabrication et non un palier de prix. Conséquence : il n'existe pas de « fichier prix par
dimension » ; la dimension est une caractéristique, rangée dans le fichier caractéristiques.

Le **prix ne se factorise pas proprement par collection**. Un même prix couvre plusieurs
collections et une même collection porte plusieurs prix (par exemple la collection Crystal
existe à trois prix distincts). La collection n'est donc pas la clé du prix : elle est une
mention descriptive, pas un axe de découpage.

Le **prix est attaché au modèle individuel**. Il n'y a que 6 prix HT distincts pour 84
références, mais leur regroupement ne suit aucun axe simple ; c'est le modèle qui détermine son
prix. L'unité tarifaire est donc le modèle.

**Aucun modèle n'existe sur plusieurs lignes** : chaque modèle appartient à une seule ligne
(Vitrée, Contemporaine, Traditionnelle, Accord). La ligne est donc, comme la collection, une
propriété du modèle et non un second axe tarifaire. En pratique, le modèle seul identifie la
référence — 84 modèles = 84 références.

Répartition des 84 modèles : ligne Vitrée 25, Contemporaine 21, Traditionnelle 23, Accord 15.

**Complétude des configurations de vantaux — vérifié.** Sur les 84 références H81 : aucun trou en
1 vantail (les 84 ont un prix HT/TTC 1V) ; 4 modèles sans prix 2 vantaux (Annecy et Avignon en
ligne Contemporaine, Angers et Dijon en ligne Traditionnelle) ; aucune incohérence HT/TTC (tout
prix HT présent a son TTC). Décompte prix H81 : 84 chunks 1V + 80 chunks 2V = **164 chunks de
prix**. Les 4 configurations 2V absentes ne doivent pas être générées (chunks fantômes).

**Options et plus-values — vérifié.** H81 compte **85 couples option × modèle avec plus-value
chiffrée**, répartis en cinq familles : vitrage sécurité 44/6 (53 couples, +196 € HT), impression
approchant RAL 7016 (15 couples, +758 € HT), impression 2 tons dépolis (15 couples, +325 € HT),
et deux options propres à un seul modèle (panneau phonique, panneau renforcé rainuré). S'y
ajoutent **89 couples sans plus-value chiffrée**, qui renvoient à une page transverse (Offre
couleurs, Plus-value vitrages) : ceux-là sont **exclus** du fichier options et relèvent du
chantier des pages transverses. Décompte options H81 : **85 chunks**.

**Caractéristiques — vérifié.** Sur les 84 modèles H81 : l'Ud se factorise (6 valeurs de 1,0 à
1,3 W/m².K, la plus fréquente couvrant 46 modèles ; à noter une anomalie de saisie « 1,2 W/m2.K »
vs « 1,2W/m2.K » à normaliser au générateur) ; le vitrage de base est quasi individuel (nombreuses
descriptions distinctes, la plus partagée ne couvrant que 15 modèles) ; les plages dimensionnelles
sont inégalement partagées (46 plages distinctes ; deux plages « standard » couvrent 20 et 13
modèles, le reste en longue traîne de 1-2 modèles). Cette hétérogénéité d'axes justifie la maille
« un chunk par modèle » plutôt qu'une factorisation. Décompte caractéristiques H81 : **84 chunks**.

**Compatibilité équipements — vérifié.** Quatre équipements récurrents, présents pour les 84
modèles avec une faisabilité oui/non propre à chaque modèle : judas optique (18 modèles
compatibles), heurtoir (6), passe-lettres (32), chatière (12). La colonne « faisabilité » signifie
donc **compatibilité de montage équipement × modèle** (ni « inclus de série », ni constante).
Aucun équipement n'est chiffré. Le poussoir incurvé est un cas distinct (un seul modèle, Garissa,
et chiffré à 163 € HT) : il relève des options. Décompte compatibilité H81 : **4 chunks**.

**Pages transverses — vérifié.** Les tarifs transverses figurent dans le PDF aux pages 20
(plaxage / offre couleurs), 21 (laquage RAL, plus-value au ml) et 23 (plus-values vitrages, grille
à colonnes performances + €/m²). L'Excel ne porte pas ces prix (colonnes teinte renvoyant à « voir
PV page Offre couleurs », colonnes de prix teinte vides) : ils n'existent que dans le PDF. Ces
grilles mêlent pourcentages, €/ml et €/m² — non transcriptibles sans calcul. Migration en
**3 chunks d'orientation** (un par bloc : teintes, laquage, vitrages), sans montant.

Décompte total H81 des cinq fichiers : 164 (prix) + 85 (options) + 84 (caractéristiques) +
4 (compatibilité équipements) + 3 (pages transverses) = **340 chunks**.

## 5. Les règles normatives

### Règle 1 — Règle de découpage

**Un chunk de prix par couple référence × configuration de vantaux.**

La référence est le modèle (qui implique sa ligne et sa collection). La configuration est le
nombre de vantaux. Cette règle vaut pour toutes les gammes, y compris celles à 3 et 4 vantaux
(gammes coulissantes notamment) : chaque configuration réellement tarifée donne son chunk.

Justification : la question de prix arrive **toujours avec le nombre de vantaux** comme prérequis
de contexte (l'ADV et le commercial n'interrogent jamais un prix sans connaître la configuration).
La configuration est donc un critère de recherche, pas seulement un contenu. Séparer par
configuration produit des chunks qui épousent chacun une requête réelle et distincte, sans
collision entre eux (le vantail figure au titre).

Cette règle résout aussi la contrainte des 200 mots dans les gammes à 3-4 vantaux : puisque
chaque chunk ne porte qu'une configuration, il reste court quel que soit le nombre de
configurations possibles, au lieu d'empiler toutes les configurations dans un chunk surchargé.

On ne génère un chunk que pour une **configuration réellement tarifée** : pas de chunk fantôme
pour une configuration absente du tarif. Cela impose, **avant toute génération et pour chaque
gamme**, une vérification systématique des « trous » : le générateur teste la présence effective
du prix (HT et TTC) pour chaque configuration de chaque référence, et n'émet un chunk que si le
prix existe réellement. Certaines références n'existent qu'en une seule configuration au tarif ;
générer mécaniquement toutes les configurations produirait des chunks fantômes pointant vers des
prix inexistants. Cette vérification conditionne aussi le décompte réel des chunks à produire.

### Règle 2 — Règle de rédaction

**Une phrase par configuration, indiquant dans la même phrase le prix HT et le prix TTC.**

Le corps du chunk énonce la configuration et lie, dans une seule phrase, son prix HT et son prix
TTC, sans grille et sans autre configuration. Cette liaison syntaxique porte la désambiguïsation
que la structure ne garantit pas : le modèle n'a pas à choisir un prix parmi plusieurs, chaque
prix est déjà collé à sa configuration par la phrase.

Forme de référence du corps :
« En [N] vantail(aux) [égaux], le modèle [Modèle] de la [gamme, désignation produit],
ligne [Ligne], collection [Collection], est proposé au tarif de [montant] € HT, soit
[montant] € TTC. »

Une seconde phrase peut porter les mentions de cadre (éco-participation exclue, renvoi à une page
transverse le cas échéant). Le plafond de 200 mots (titre + source + corps) reste impératif.

### Règle 3 — Règle d'instruction LLM

**À défaut de nombre de vantaux précisé, le LLM demande la clarification ; il n'explore pas les
configurations.**

Pour toute demande de prix dont la configuration de vantaux n'est pas spécifiée, le modèle doit
réclamer cette précision avant de répondre, et ne jamais restituer plusieurs configurations par
défaut ni tenter d'énumérer les solutions possibles. Cette instruction sert l'usage (la question
réelle cible toujours une configuration) et la maîtrise du risque (moins le modèle explore, moins
il dérape). Elle est à encoder dans Instructions.md.

Instruction complémentaire de sécurité tarifaire : le modèle ne restitue qu'un prix présent en
toutes lettres dans un chunk ; il ne calcule jamais un prix, ne l'interpole pas entre deux
valeurs, et n'en déduit aucun. Si la référence ou la configuration demandée n'est pas trouvée,
il l'indique plutôt que d'estimer.

### Règle 4 — Fichier options et plus-values

**Un chunk par couple option × modèle, uniquement pour les plus-values réellement chiffrées.**

Découpage : un chunk pour chaque couple (option, modèle) dont la plus-value porte un montant sur
la ligne modèle. Les couples renvoyant à une page transverse sans montant sont exclus (traités
au chantier des pages transverses). La détection des couples chiffrés se fait, comme pour les
trous de vantaux, avant génération : ne pas produire de chunk pour une plus-value non chiffrée.

Rédaction : le corps énonce, en une phrase, l'option pour le modèle avec sa plus-value HT et TTC,
sur le modèle de la règle 2. Une courte description de l'option (issue littéralement de la source)
peut suivre. Le préfixe de titre reprend à l'identique celui du chunk prix du même modèle (liant
inter-fichiers).

### Règle 5 — Fichier caractéristiques communes

**Un chunk par modèle, réunissant vitrage de base, Ud et plage dimensionnelle.**

Découpage : un chunk par modèle (pas de factorisation ; autonomie de la référence privilégiée).
Rédaction en prose : vitrage de base et dimensions de vitrage, puis Ud, puis plage dimensionnelle
donnée sous forme d'**enveloppe** (largeur min–max et hauteur min–max tous profils confondus), en
précisant que les limites exactes dépendent du profil de dormant. Le vitrage, l'Ud et les
dimensions de vitrage sont transcrits littéralement ; l'enveloppe dimensionnelle est une synthèse
(min des minis, max des maxis sur les profils), seule dérogation admise à la transcription brute,
justifiée par la lisibilité et le plafond. Le préfixe de titre reprend à l'identique celui du
chunk prix du même modèle.

### Règle 6 — Fichier compatibilité équipements

**Un chunk par équipement, listant les modèles compatibles.**

Découpage : un chunk par équipement récurrent (judas optique, heurtoir, passe-lettres, chatière).
Rédaction en prose : nommer l'équipement, puis énumérer les modèles où il est montable (colonne
faisabilité = « oui »), puis indiquer que sur les autres modèles il n'est pas réalisable. Préciser
que l'équipement n'est pas chiffré dans le tarif, pour éviter toute confusion avec une plus-value.
La liste des modèles est extraite littéralement. Un équipement propre à un seul modèle et chiffré
(poussoir incurvé) ne relève pas de ce fichier mais des options (règle 4).

### Règle 7 — Fichier pages transverses (orientation)

**Un chunk par bloc transverse, orientant sans reproduire aucun montant.**

Découpage : un chunk par bloc transverse (teintes/plaxage/offre couleurs ; laquage RAL ;
plus-values vitrages). Rédaction en prose : nommer ce qui existe (nature de l'option transverse),
décrire sa **logique tarifaire** (groupes, pourcentage, €/ml, €/m²) sans citer de montant, renvoyer
explicitement à la page du tarif, et indiquer que le montant se lit directement sur cette page.
Aucun prix, aucun pourcentage chiffré, aucune valeur calculable ne figure dans ces chunks —
cohérence avec la règle 3 (le LLM ne calcule jamais). Ce fichier est le complément non chiffré du
fichier options.

## 6. Format des chunks (rappel hérité et spécialisations)

Hérité de la note de cadrage générale : titre auto-porteur préfixé du code gamme ; ligne de
source par chunk au format `*Source : DOCUMENT—GAMME—DATE.pdf, page N — information
(originale|complémentaire) — SC[nnnn]*` (em-dashes dans le nom affiché, underscores dans le
champ YAML `document_source`) ; plafond de 200 mots ; prose sans puces ; SC continue depuis
SC0002 ; pas de profil d'audience dans les titres.

Spécialisation tarif — forme du titre du chunk prix :
`## [GAMME] [Désignation produit] — Tarif [Modèle] [N vantaux] (ligne [Ligne], collection [Collection])`

Exemple : `## H81 Porte PVC — Tarif Azurite 2 vantaux (ligne Vitrée, collection Crystal)`

Spécialisation tarif — forme du titre du chunk option :
`## [GAMME] [Désignation produit] — Option [libellé option] sur [Modèle] (ligne [Ligne], collection [Collection])`

Exemple : `## H81 Porte PVC — Option impression approchant RAL 7016 sur Azurite (ligne Vitrée, collection Crystal)`

Spécialisation tarif — forme du titre du chunk caractéristiques :
`## [GAMME] [Désignation produit] — Caractéristiques [Modèle] (ligne [Ligne], collection [Collection])`

Exemple : `## H81 Porte PVC — Caractéristiques Azurite (ligne Vitrée, collection Crystal)`

Spécialisation tarif — forme du titre du chunk compatibilité équipement :
`## [GAMME] [Désignation produit] — Compatibilité de l'équipement [équipement] par modèle`

Exemple : `## H81 Porte PVC — Compatibilité de l'équipement chatière par modèle`

Spécialisation tarif — forme du titre du chunk page transverse :
`## [GAMME] [Désignation produit] — Existence et localisation des tarifs de [bloc transverse]`

Exemple : `## H81 Porte PVC — Existence et localisation des tarifs de plus-values vitrages`

Le préfixe commun `[GAMME] [Désignation] — ... [Modèle]` est le liant inter-fichiers : il doit
être identique dans les chunks prix, options et caractéristiques d'une même référence. Le chunk
compatibilité équipement fait exception (maille par équipement, non par modèle) : il n'a pas de
modèle unique au titre et ne participe donc pas au liant par référence.

## 7. Reste à faire

Fait : les **cinq gabarits sont figés et éprouvés** sur H81 (prix 164, options 85,
caractéristiques 84, compatibilité équipements 4, pages transverses 3 ; total 340). **Tous les
arbitrages de périmètre et de maille sont clos** : trous de vantaux, périmètre options, maille
caractéristiques (enveloppe dimensionnelle), sens de la « faisabilité », rattachement des
équipements (fichier dédié par équipement), traitement des pages transverses (orientation sans
prix, fichier dédié). Le **générateur** et le **contrôle de conformité** sont écrits, exécutés et
validés sur H81 (voir §8).

Restent à traiter :
- **extension aux dix gammes** : le générateur est aujourd'hui spécifique à H81 (nom de gamme,
  désignation, pages transverses et liste des configurations de vantaux en dur). L'étendre suppose
  de le paramétrer par gamme et, surtout, de **re-vérifier gamme par gamme les faits qui fondent
  les règles** — les gammes coulissantes à 3-4 vantaux changeront la structure des configurations,
  les décomptes, et peuvent faire apparaître des natures d'information absentes de H81 ;
- **colonnes chiffrées non mappées** signalées par le journal du générateur (vitrage analogue
  BQ-BU, teinte de grille BH-BI) : plus-values réelles écartées par le choix du périmètre restreint
  (option 1), à réintégrer au fichier options si décidé ;
- **page source des équipements** : fixée à 14 en dur dans le générateur, à confirmer contre le PDF ;
- **colonnes non tarifaires non mappées** (sens A/B, motifs, schémas, « Belgique » au sens non
  élucidé) : décision de traitement séparée (chunks visuels ? hors périmètre tarif ?) ;
- extensions possibles du contrôle : fidélité du vitrage et des dimensions contre l'Excel (au-delà
  des montants et de l'Ud déjà couverts), croisement PDF exhaustif (aujourd'hui par échantillon) ;
- confirmer, si souhaité, la lecture de la « faisabilité » auprès d'Aurélien (arbitrage clos par la
  donnée, confirmation de prudence).

## 8. Industrialisation : générateur et contrôle (bilan H81)

**Générateur** (`generateur_tarif.py`). Lit la feuille « modèles portes », groupe les lignes par
modèle (une référence = un modèle, qui implique sa ligne et sa collection) et produit les cinq
fichiers. Fonctions clés : détection anti-fantôme (une configuration de vantaux n'est générée que
si le prix existe ; une option n'est générée que si sa plus-value est **strictement positive** —
les plus-values nulles, qui sont des renvois transverses, sont exclues), calcul de l'enveloppe
dimensionnelle (min des minis, max des maxis sur les 4 profils), normalisation de l'Ud (extraction
de la valeur seule, unification des graphies « W/m2.K » → « W/m².K »), préservation des acronymes
et codes dans les libellés d'options (RAL, 44/6), numérotation SC continue par fichier depuis
SC0002, ligne de source normée, contrôle du plafond de 200 mots, et **journal des colonnes
remplies non mappées** (aucune information n'est écartée en silence).

Résultat sur H81 : **340 chunks** exactement conformes aux décomptes attendus (164 + 85 + 84 + 4
+ 3), aucun dépassement de plafond. Six défauts ont été détectés et corrigés lors du test sur la
gamme pilote : un bug de logique (plus-values nulles doublant les options, révélé par le décompte
de contrôle) et cinq scories de rédaction (casse d'acronyme, grammaire de la compatibilité, Ud
redondant, reconstruction du vitrage, accord de participe — résolu par une tournure neutre
« peut équiper »).

**Contrôle de conformité** (`controle_conformite.py`). Audit **autonome** : relit les .md produits
sans réutiliser aucune fonction du générateur, et les confronte à la note de cadrage (forme) et à
l'Excel/PDF (fidélité). Contrôles : décomptes, plafond, ligne de source (regex), continuité SC,
préfixe de titre, liant inter-fichiers, front matter ; **fidélité numérique exhaustive** des 164
prix, des 85 plus-values d'options et des 84 valeurs d'Ud (comparaison au € / à la décimale contre
l'Excel) ; anti-fantôme ; absence de tout montant dans les transverses (règle 7) ; croisement PDF
par échantillon.

Résultat sur H81 : **12 contrôles réussis, 0 échec, 0 avertissement**. Chaque nombre servi pour
H81 est traçable à une cellule Excel, et la vérification est rejouable à chaque régénération.

Limites connues du contrôle : la fidélité exhaustive porte sur les montants et l'Ud, non sur le
vitrage ni les dimensions ; le croisement PDF est par échantillon (choix assumé vu le bruit
d'extraction) ; l'audit vérifie la conformité, non la qualité rédactionnelle (celle-ci relève de
la relecture humaine).
