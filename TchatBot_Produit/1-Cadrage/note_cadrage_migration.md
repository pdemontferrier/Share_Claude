---
titre: "Migration de la documentation produit vers un format exploitable par le RAG"
sous_titre: "Note de cadrage technique — normes, scripts et enrichissements"
contexte: "Chatbot ADV — moteur RAG Wikit Semantic (GPT-4.1 mini)"
perimetre: "6 documents par gamme — pilote H81, extensible multi-gammes"
version: "2.0"
date: "2026-06"
audience: "Direction du développement / Architecte / IT"
---

# Migration de la documentation produit vers un format exploitable par le RAG

## 1. Le problème central : dimensionner la documentation pour le moteur réel

Le chatbot ADV repose sur un moteur RAG dont le modèle de langage est **GPT-4.1 mini**, un modèle volontairement léger, choisi pour son coût et sa rapidité. Ce choix est raisonnable pour un usage ADV à volume élevé, mais il a une conséquence directe sur la documentation : **le modèle ne compensera pas les défauts de structure du corpus**.

C'est le point que cette note veut établir sans ambiguïté. Un modèle de raisonnement puissant peut ingérer une documentation mal structurée, reconstituer mentalement les relations entre un tableau et sa légende, deviner qu'une caractéristique évoquée dans trois documents datés différemment renvoie à la même réalité, et rattraper une extraction PDF défaillante. Un modèle léger comme GPT-4.1 mini ne le fait pas de façon fiable. Il prend les chunks qu'on lui donne, tels qu'on les lui donne, et répond à partir de leur contenu littéral.

Autrement dit : **tout le travail de raisonnement que le modèle ne fera pas doit être fait en amont, au moment de la préparation documentaire.** La qualité du chatbot ne se joue pas dans le modèle — il est fixé — mais dans la qualité et la granularité du corpus qu'on lui fournit.

C'est pourquoi la migration documentaire n'est pas une commodité mais le facteur déterminant de réussite du projet. Nous transférons, dans la phase de préparation, l'effort d'intelligence que le moteur d'exécution n'a pas les moyens de fournir en temps réel.

## 2. La contrainte structurante : le chunk de 200 mots

Le moteur Wikit Semantic découpe la documentation en **chunks de 200 mots maximum**, et ce découpage s'opère **à chaque titre Markdown, quel que soit son niveau** (`#`, `##` ou `###`). Chaque titre ouvre donc un nouveau chunk, et le chunk s'étend jusqu'au titre suivant.

Trois conséquences en découlent, qui gouvernent toute la norme de migration :

**Chaque section est un chunk autonome.** Il n'existe pas de section « conteneur » qui regrouperait des sous-sections. Un `#` suivi de trois `##` ne produit pas un bloc unique : il produit quatre chunks distincts et indépendants. Le corps d'une section ne doit donc jamais dépendre du contenu d'une autre section pour être compris.

**Le budget réel est d'environ 185 mots de corps.** Le plafond de 200 mots inclut le titre. Un titre auto-porteur consomme 10 à 15 mots. Le corps utile doit donc rester sous 185 mots, avec une marge de sécurité recommandée à 170 mots pour absorber les variations de comptage entre outils.

**Le titre est le premier vecteur de pertinence.** Au moment où l'utilisateur pose une question, le moteur compare la question aux chunks. Le titre voyage avec le corps et pèse fortement dans le score de similarité. Un titre vague (`## Sécurité`) est un titre perdu. Un titre explicite (`## H81 Porte d'entrée PVC — Serrure 6 points : composition et résistance à l'effraction`) capte la requête.

## 3. Norme de titre : gamme + description explicite

Le front matter YAML porte les métadonnées de gouvernance mais **n'est pas indexé dans le corps du chunk**. L'information de gamme ne « ruisselle » donc pas du YAML vers les chunks : elle doit être **répétée explicitement dans chaque titre**.

La règle de titre est la suivante :

> `# [GAMME] [Type produit] — [Sujet précis] : [angle ou contenu]`

Exemples conformes :
- `# H81 Porte PVC — Performances thermiques : coefficients Ud selon remplissage`
- `## H81 Porte d'entrée PVC — Serrure 6 points : composition et résistance à l'effraction`
- `## H81 Porte d'entrée PVC — Vitrage standard : triple vitrage Isol'3 et certification CEKAL`

Chaque titre remplit trois fonctions simultanées : il identifie la gamme (évite la confusion inter-gammes au retrieval), il annonce le sujet (améliore le score de similarité), et il rend le chunk auto-descriptif (le modèle sait de quoi parle le bloc sans dépendre d'un contexte externe).

Un titre bien construit répond seul à la question « ce chunk est-il pertinent pour cette question ? ». C'est l'unité de travail la plus rentable de toute la migration.

## 4. Norme de corps : rédiger pour un modèle qui n'interprète pas

Le principe directeur de la migration doit être posé avant les règles de détail. **Il ne s'agit pas de synthétiser, mais d'organiser, de détailler et de simplifier.** Le travail ne consiste pas à produire une connaissance nouvelle, plus condensée, en croisant les documents. Il consiste à transposer fidèlement le contenu de chaque page dans un format que le moteur lit bien. La valeur ajoutée est dans la forme — découpage par sujet, phrases complètes, vocabulaire normalisé, autonomie sémantique — jamais dans la fusion de contenus issus de sources différentes. Une page peut donner plusieurs chunks ; un chunk ne remonte jamais à plus d'une page.

Le corps de chaque chunk suit six règles, calibrées pour un modèle léger.

**Prose et non listes.** GPT-4.1 mini exploite mieux des phrases complètes que des puces. Une phrase « La serrure est une serrure manuelle à 6 points » est plus robuste qu'une puce « 6 points ». Les listes sont réservées aux énumérations vraies et fermées (liste des coloris, liste des lignes de modèles).

**Une idée par chunk.** Puisque chaque titre crée un chunk, chaque chunk doit traiter un seul sujet atomique. Ne pas mélanger serrurerie et vitrage dans le même bloc, même si le PDF d'origine les présente ensemble. L'atomicité a une borne basse : un sujet trop maigre pour porter un chunk substantiel se rattache à un chunk voisin plutôt que de former un chunk creux. On découpe par idée, on n'émiette pas.

**Autonomie sémantique.** Le corps ne référence jamais « voir ci-dessus », « comme indiqué page 4 », « le tableau précédent ». Ces renvois n'ont aucun sens une fois le chunk isolé. Toute information nécessaire à la compréhension est soit dans le chunk, soit nommée explicitement pour être retrouvée par le moteur.

**Vocabulaire normalisé.** Chaque terme technique a une forme unique et stable, définie dans un glossaire transversal : Ud, Ug, AEV, PMR, P2A, CEKAL, plaxage, laquage. Jamais de synonyme improvisé.

**Données répétées en toutes lettres.** Une valeur importante (Ud = 1,0 W/m².K) est écrite intégralement dans chaque chunk qui en a besoin, plutôt que référencée ailleurs. La redondance maîtrisée est ici une qualité, pas un défaut.

**Traçabilité obligatoire vers une source unique.** La première ligne du corps, immédiatement sous le titre, indique le document PDF d'origine, le numéro de page, la nature du contenu et un numéro chronologique de chunk, au format constant `*Source : nom—du—fichier.pdf, page N — information originale — SC0002*` (ou `— information complémentaire —`). La règle est stricte : **un chunk ne référence qu'un seul document et une seule page.** Un chunk ne consolide jamais deux sources ni deux pages.

Trois conventions de forme s'appliquent à cette ligne. D'abord, la **mention de nature** distingue les chunks issus directement de la page (information originale) des chunks ajoutés pour le moteur à l'étape d'enrichissement (information complémentaire). Ensuite, dans le **nom du fichier tel qu'affiché ici**, les underscores sont remplacés par des tirets longs (`—`) : le moteur RAG occulte les underscores à l'affichage, ce qui rend les références illisibles, tandis que le tiret long reste visible. Ce remplacement ne concerne que le texte affiché de la ligne source ; le nom réel du fichier `.md`, le PDF source et le champ `document_source` du front matter conservent leurs underscores. Enfin, le **numéro chronologique** `SCxxxx` identifie le chunk dans le fichier, sur quatre positions. La numérotation démarre à `SC0002` et non à `SC0001`, car le premier chunk de tout document est un résumé auto-généré par Wikit qui occupe le rang `SC0001`. Elle est continue sur l'ensemble du fichier `.md`, sans jamais repartir à zéro d'une page à l'autre : la reprise du chrono entre pages est décrite en section 7 bis.

Cette ligne préserve le lien avec la documentation originale : elle permet à l'ADV de vérifier une réponse du chatbot en remontant précisément au PDF, à la page et au chunk, et elle rend chaque chunk auditable sans ambiguïté. Elle est indexée dans le corps (elle consomme donc 8 à 10 mots du budget) mais reste sémantiquement neutre : un nom de fichier et un code de chunk ne ressemblent pas à une question ADV et ne perturbent pas le score de pertinence. Elle est mise en italique pour se distinguer du contenu utile.

## 5. Structure cible : un fichier Markdown par document PDF

La règle de structuration est le miroir de la règle de traçabilité : **un document PDF donne un document Markdown.** La migration ne disperse pas un PDF en multiples fichiers et n'agrège pas plusieurs PDF dans un fichier commun. Chaque PDF source a son `.md` miroir, qui le migre et le complète.

Pour une gamme, on obtient donc autant de fichiers Markdown que de documents sources :

- `FIP_H81_08-2025.pdf` → `FIP_H81_08-2025.md`
- `CABP_H81_07-2023.pdf` → `CABP_H81_07-2023.md`
- `Fiche_Excellence_H81_08-2023.pdf` → `Fiche_Excellence_H81_08-2023.md`
- et ainsi de suite pour les autres documents.

Chaque fichier `.md` contient plusieurs chunks (sections `##`), mais **tous les chunks d'un même fichier partagent la même source** — le PDF dont le fichier est issu. Ils se distinguent seulement par le numéro de page. Cette organisation renforce la cohérence : le nom du fichier `.md` est la source, et la ligne `*Source :*` de chaque chunk cite ce même document avec sa page. La traçabilité devient triviale et impossible à casser.

Voici la structure type d'un fichier issu de la Fiche Info Produit.

```markdown
---
document_source: FIP_H81_08-2025.pdf
type_document: fiche_info_produit
gamme_code: H81
gamme_nom: "Porte d'entrée PVC"
collection: "TRYBA PVC"
materiau: PVC
version_doc: "2025.08"
date_validite: 2025-08-01
remplace: null
audiences: [ADV, technique]
glossaire_ref: glossaire_transversal_v1.md
---

## H81 Porte d'entrée PVC — Identité produit et positionnement de gamme
*Source : FIP—H81—08-2025.pdf, page 1 — information originale — SC0002*
[corps ≤ 180 mots]

## H81 Porte d'entrée PVC — Performances thermiques : coefficients Ud selon remplissage
*Source : FIP—H81—08-2025.pdf, page 1 — information originale — SC0003*
[corps ≤ 180 mots]

## H81 Porte d'entrée PVC — Serrure 6 points : composition et résistance à l'effraction
*Source : FIP—H81—08-2025.pdf, page 1 — information originale — SC0004*
[corps ≤ 180 mots]
```

**Schéma canonique du front matter.** Les champs, leur ordre et leur présence
sont fixés. Deux schémas coexistent, l'un pour la documentation produit, l'autre
pour le tarif, qui porte des informations propres à sa nature.

Documentation produit — dix champs, tous obligatoires :

```yaml
document_source: FIP_H81_08-2025.pdf
type_document: fiche_info_produit
gamme_code: H81
gamme_nom: "Porte d'entrée PVC"
collection: "TRYBA PVC"
materiau: PVC
version_doc: "2025.08"
date_validite: 2025-08-01
remplace: null
audiences: [ADV, technique]
```

Tarif — treize champs, dont deux conditionnels :

```yaml
document_source: Tarif_H81_HT_08-04-2026.pdf
document_source_ttc: Tarif_H81_TTC_08-04-2026.pdf   # si une édition TTC existe
type_document: tarif
sous_type: prix
gamme_code: H81
gamme_nom: "Porte d'entrée PVC"
gammes_couvertes: [CA76, CAG76]                      # si plusieurs gammes
collection: "TRYBA PVC"
materiau: PVC
version_doc: "2026.04"
date_validite: 2026-04-08
nb_chunks: 164
audiences: [ADV, commercial]
```

`document_source_ttc` et `gammes_couvertes` sont les seuls champs dont l'absence
est légitime : le premier n'existe que si le tarif dispose d'une édition TTC de
pagination identique, le second que si le fichier couvre plus d'une gamme. Tout
autre champ manquant est une anomalie.

`nb_chunks` mérite une mention particulière : c'est le seul champ qui permette
de détecter la troncature d'un fichier. Il doit être recalculé à chaque
génération et vérifié à chaque contrôle.

Trois champs ont été retirés du schéma et ne doivent pas réapparaître.
`glossaire_ref` référençait un glossaire transversal qui n'a jamais été produit,
ses règles ayant été versées dans les instructions du modèle. `perimetre` était
un texte libre que rien n'exploitait. `remplace` reste dans le schéma de la
documentation produit, où il porte une information réelle, mais sort de celui du
tarif où il valait `null` dans tous les cas sauf un.

**Le générateur fait foi, pas le fichier.** Un front matter corrigé à la main
reste juste jusqu'à la première régénération, après quoi le script réimpose ses
propres valeurs. Toute correction de schéma ou de libellé doit donc être portée
dans le générateur de la gamme avant, ou au plus tard en même temps que, dans
les fichiers. Le script `controle_front_matter_et_libelles.py` confronte les
trois sources — le référentiel de gammes d'`Instructions_V03.md`, les fichiers
du corpus et les générateurs — et doit être passé sans anomalie avant toute mise
en production.

**Conventions de valeur du front matter.** Le front matter est une couche de
normalisation, non une citation : contrairement aux titres et aux corps, qui
suivent la source, ses champs portent des formes fixes et identiques d'un
document à l'autre. Trois règles en découlent. Le nom de collection s'écrit
`"TRYBA ALUMINIUM"` ou `"TRYBA PVC"`, en capitales, et cette forme vaut aussi
partout où le nom de collection apparaît dans le corps des chunks, qui est du
texte indexé. Les valeurs de chaîne sont guillemetées, y compris lorsque YAML
les interpréterait correctement sans guillemets, pour que l'écart soit visible
à la relecture. Le champ `gamme_nom` reprend le libellé de gamme retenu pour les
titres, à l'identique.

Le front matter porte désormais une source unique (`document_source`) et le type de document, au lieu d'une liste. Chaque `##` est un chunk indépendant, dont la première ligne cite le document du fichier, sa page, la nature du contenu (originale ou complémentaire) et son numéro chronologique `SCxxxx`. Ce numéro suit l'ordre des chunks dans le fichier, sur quatre positions, et démarre à `SC0002`, le rang `SC0001` étant réservé au résumé auto-généré par Wikit. Il est continu sur tout le fichier, y compris entre les pages d'un document multi-pages (voir la reprise du chrono en section 7 bis). L'ordre des chunks n'a pas d'importance pour le moteur (il les indexe séparément) mais facilite la maintenance humaine. Le nombre de chunks par fichier dépend du volume du PDF d'origine.

## 6. Exemple complet de chunk calibré

Voici un chunk réel, issu de la Fiche Info Produit H81, calibré sous la contrainte.

```markdown
## H81 Porte d'entrée PVC — Serrure 6 points : composition et résistance à l'effraction
*Source : FIP—H81—08-2025.pdf, page 1 — information originale — SC0002*

La porte d'entrée PVC H81 est équipée en standard d'une serrure manuelle
6 points. Cette serrure comprend deux crochets massifs, un pêne demi-tour
et trois pênes manuels. Elle est complétée par une gâche filante toute
hauteur en acier zingué, qui assure la continuité du verrouillage et une
finition régulière sur toute la hauteur de la porte. Le cylindre fourni
en standard est un cylindre de sécurité débrayable, livré avec une carte
de propriété. L'ensemble de ces éléments assure une haute résistance à
l'effraction. Cette configuration de serrurerie est décrite dans la
section Solidité et sécurité de la fiche produit.
```

Ce chunk compte 13 mots de titre, 9 mots de ligne source (nom de fichier, page, nature et numéro SC) et 92 mots de corps, soit 114 mots au total : sous le plafond de 200. Il ne référence qu'un seul document et une seule page. Son contenu est fidèle à ce que porte la page d'origine, sans emprunt à un autre document. Il est autonome, il nomme sa gamme dans le titre, il porte sa source exacte pour l'audit, et il n'exige aucun contexte externe pour être compris.

Le même sujet, la serrurerie, apparaît aussi dans le CABP et dans la Fiche Excellence. Conformément à la règle d'une source unique par chunk, il donnera lieu à un chunk distinct pour chacun de ces documents, tracé sur sa propre page. Ce point est traité en section 6 bis.

## 6 bis. La redondance inter-documents est assumée

La règle d'une source unique par chunk a une conséquence directe : un sujet présent dans plusieurs documents produit plusieurs chunks, un par document. La serrure 6 points de la H81 est décrite dans la Fiche Info Produit, dans le CABP et dans la Fiche Excellence. La migration produit donc trois chunks distincts sur la serrurerie, chacun tracé sur son document et sa page propres.

Ce choix est assumé et cohérent avec l'objectif de la démarche. Il ne s'agit pas de synthétiser les trois descriptions en une seule, car cela ferait perdre la traçabilité exacte et introduirait un travail d'interprétation. Chaque chunk reste le reflet fidèle de sa page d'origine.

Le moteur pourra donc remonter plusieurs chunks sur un même sujet selon la source. Ce n'est pas un défaut : les trois documents ne disent pas exactement la même chose. La Fiche Info Produit donne la description technique, le CABP donne l'angle commercial et le bénéfice client, la Fiche Excellence inscrit la serrure dans l'engagement qualité. Ces trois angles sont complémentaires et légitimes.

Les métadonnées YAML `audiences` et le nom du document source permettent, si nécessaire, d'orienter le retrieval vers le document le plus adapté à la question posée. La gestion fine de cette priorité relève du paramétrage du moteur et non de la migration documentaire elle-même.

## 7. Les 6 documents par gamme : un fichier md par document

Chaque gamme comporte six documents sources : cinq PDF et un fichier Excel. Le principe est **un fichier Markdown par document source** — relation 1:1. La migration transpose le PDF vers son `.md` miroir en le complétant (structuration, mise en prose, enrichissements), sans jamais fusionner deux documents. La stratégie de traitement diffère selon le type de document, mais elle est constante pour un même type d'un document à l'autre : une Fiche Info Produit se traite de la même manière quelle que soit la gamme. Cette régularité rendra l'outillage possible plus tard, si l'industrialisation le justifie, sans changer la structure des livrables.

Deux documents font exception à cette relation « PDF vers Markdown » : le tarif et l'Excel de compatibilités. Leur nature tabulaire et le risque d'erreur sur des données chiffrées imposent une sortie vers une base structurée interrogée par un outil dédié, et non vers de la prose. Pour ces deux documents, le livrable n'est pas un fichier de chunks mais une base de données. Les quatre documents rédactionnels — CABP, Fiche Info Produit, Fiche Excellence, et le cinquième PDF — suivent en revanche strictement la relation 1:1 vers un fichier Markdown.

Les fiches de stratégie ci-dessous sont elles-mêmes rédigées au format chunk conforme, à titre d'illustration de la norme.

```markdown
## Migration — Rôle et stratégie du document CABP
*Source : CABP—H81—07-2023.pdf — information originale — SC0002*

Le CABP est l'argumentaire commercial en tableau à quatre colonnes
(caractéristiques, avantages, bénéfices, preuves). La colonne « preuves »
contient des instructions de démonstration terrain sans valeur pour l'ADV.
Le traitement retient uniquement les colonnes caractéristiques et bénéfices,
les recompose en prose, et ignore la colonne preuves. Sortie : chunks
techniques rédigés, un par ligne thématique du tableau.
```

```markdown
## Migration — Rôle et stratégie de la Fiche Info Produit
*Source : FIP—H81—08-2025.pdf, page 1 — information originale — SC0002*

La Fiche Info Produit est structurée en trois sections (solidité,
isolation, esthétique) avec listes à puces et un schéma coté numéroté.
Le traitement convertit chaque puce en phrase, éclate les trois sections en
chunks atomiques, et transforme la légende du schéma numéroté en
description textuelle explicite. C'est le document le plus proche du
format cible et le plus simple à migrer.
```

```markdown
## Migration — Rôle et stratégie de la Fiche Excellence
*Source : Fiche—Excellence—H81—08-2023.pdf, page 1 — information originale — SC0002*

La Fiche Excellence contient dix critères qualité et un engagement
contractuel avec ses modalités (rayon 50 km, validité 30 jours). Le traitement
produit des chunks propres à ce document, sans les fusionner avec ceux de
la FIP même lorsque le sujet se recoupe : chaque chunk garde sa source
unique. Les critères techniques donnent des chunks tracés sur cette fiche,
et l'engagement contractuel est isolé dans un chunk dédié tagué engagement
pour éviter toute réponse partielle sur les conditions.
```

```markdown
## Migration — Rôle et stratégie du Tarif
*Source : Tarif—H81—HT—08-04-2026.pdf — information originale — SC0002*

Le tarif compte 133 pages sur gabarit InDesign répétitif, avec grilles de
prix denses. Il ne suit PAS le chemin Markdown : le risque d'hallucination
tarifaire l'interdit. Le traitement extrait les données vers une base
structurée JSON, exposée au chatbot par un outil dédié de calcul de prix.
Le LLM n'interroge jamais le prix en texte : il appelle la fonction.
```

```markdown
## Migration — Rôle et stratégie du 5e PDF
*Source : 5e PDF H81 (à qualifier) — information originale — SC0002*

Le cinquième PDF de la gamme reste à qualifier précisément. Selon son type
(catalogue, notice de pose, avis technique), il suit la stratégie prose
standard : conversion en chunks atomiques, un titre auto-porteur par sujet,
source unique par chunk tracée sur sa page. Sa nature exacte déterminera le
détail du traitement, mais la norme de sortie reste identique à celle des
autres documents rédactionnels.
```

```markdown
## Migration — Rôle et stratégie de l'Excel de compatibilités
*Source : matrice—compatibilites—H81.xlsx — information originale — SC0002*

L'Excel est une matrice de compatibilités croisées (options par modèles).
Une matrice ne se lit pas en prose brute. Le traitement parcourt chaque
croisement et génère une phrase de compatibilité explicite, ou alimente une
base structurée interrogée par un outil dédié, selon le volume de
combinaisons. Chaque chunk généré garde une source unique, la feuille et la
cellule d'origine, pour préserver la traçabilité.
```

## 7 bis. Mode opératoire : six étapes en tours successifs, page par page

La migration d'un document ne s'exécute pas d'un bloc. Elle suit six étapes distinctes, et surtout elle s'applique **page par page** : une page est traitée intégralement, de l'extraction au contrôle, avant de passer à la suivante. Ce séquencement garde le contexte de la page fraîchement en mémoire pendant la rédaction et l'enrichissement, et produit dans le fichier `.md` un enchaînement lisible où chaque page forme un bloc cohérent.

**Chaque étape est un tour de conversation distinct, avec un livrable validé avant de passer au suivant.** C'est une règle d'exécution, pas une simple présentation. L'expérience montre que traiter l'extraction, le découpage, la rédaction et l'enrichissement en une seule passe provoque des omissions : on rédige à partir de l'impression d'ensemble de la page, et une information présente mais discrète — un pied de page, une série de logos de certification — passe sous le radar. Isoler chaque tâche dans son propre tour supprime ce risque. La fidélité à la page n'est pas une qualité de rédacteur, c'est une propriété du processus : un tour dédié à l'extraction garantit la fidélité mieux que n'importe quelle vigilance en passe unique. De même, un tour dédié au découpage, avant toute rédaction, rend visibles les fusions abusives et les oublis de sujet, qu'une passe unique masque.

Le principe directeur reste la séparation entre ce qui est déterministe et ce qui relève du jugement. L'extraction du texte et le contrôle de conformité sont des tâches mécaniques. Le découpage sémantique, la rédaction des titres, la mise en prose et les enrichissements sont des tâches d'intelligence, confiées à un rédacteur ou à un modèle de rédaction sous validation humaine.

**En phase actuelle, le processus est conduit sans script.** Tant que la norme n'est pas éprouvée sur des cas réels, elle continuera d'évoluer au fil des ajustements. Un processus conversationnel se plie instantanément à chaque correction, là où un script devrait être réécrit à chaque changement de règle. On travaille donc entièrement à la main, page par page, ce qui permet d'ajuster la norme facilement. La question de l'outillage automatisé (extraction et contrôle scriptés) est renvoyée à une phase ultérieure d'industrialisation, à décider au vu du volume réel une fois la norme stabilisée. Les six étapes ci-dessous décrivent donc le déroulé sans script.

### Les six étapes

**Étape 1 — Extraction assistée.** Le texte et les tableaux de la page courante sont extraits **au rendu** — `pymupdf`, qui respecte les chemins de rognage — et affichés tels quels, sans transformation, dans un bloc de code Markdown. `pdftotext -layout` peut être appelé en second pour reconstituer la géométrie d'une grille dense, mais son résultat ne fait pas foi : il contient le texte des planches rognées par la maquette, invisible pour tout lecteur (voir section 7 ter). La restitution est exhaustive : titres, corps, listes, légendes de schéma, encadrés, et surtout les éléments discrets à forte valeur — pied de page, mentions légales, logos de certification, labels. Le résultat est une restitution fidèle de la page, isolée avec son numéro. Elle est brute mais vérifiable, et elle fixe le numéro de page qui alimentera la ligne de source. C'est le tour qui garantit qu'aucune information de la page ne sera perdue en aval. En phase industrielle, cette étape pourra être confiée à un script d'extraction.

**Étape 2 — Revue de l'extraction (humain, rapide).** On compare l'extraction affichée au PDF pour détecter ce qui a été mal restitué : tableau mélangé, colonne perdue, encodage défectueux, légende de schéma détachée, pied de page oublié. On annote les zones à problème sans encore corriger. Cette étape prend quelques minutes et évite de propager une erreur dans la suite.

**Étape 3 — Plan de découpage (décision, validée).** Avant toute rédaction, on établit la *liste* des chunks d'information originale à produire pour la page : un titre auto-porteur par sujet atomique, sans encore rédiger les corps. Ce plan matérialise les décisions d'atomicité — un sujet interrogé pour lui-même (la paumelle, le vitrage feuilleté, les certifications, l'exception Crystal) mérite son propre chunk et ne doit pas être fondu dans un chunk voisin. L'atomicité a toutefois une **borne basse** : un sujet trop mince pour porter un chunk substantiel — quelques mots sans réelle matière propre — se rattache à un chunk voisin cohérent plutôt que de former un chunk creux. Découper n'est pas émietter : chaque chunk doit avoir une densité qui justifie son existence. Le livrable est la liste des titres, que l'on valide avant d'écrire quoi que ce soit. C'est le tour qui attrape à la fois les fusions abusives et la sur-atomisation, au moment où les corriger ne coûte rien.

**Étape 4 — Rédaction des chunks originaux (rédaction, validée).** À partir du plan validé, on rédige le corps de chaque chunk : mise en prose, vocabulaire normalisé, ligne de source, calibrage sous 200 mots. Le livrable est l'ensemble des chunks d'information originale de la page, conformes à la norme. Un humain valide la fidélité à la source.

**Étape 5 — Ajout de l'information complémentaire (rédaction, validée).** Sur la même page, on ajoute la couche qui compense le modèle léger : chunks-questions passés au crible des quatre points de vue (ADV, commercial, technique, SAV), synonymes métier, descriptions du visuel. Ces familles et ces points de vue sont détaillés en section 8. Ces chunks sont tracés sur la même page que l'information originale dont ils dérivent. Un humain valide qu'ils n'introduisent aucune information absente de la page.

**Étape 6 — Contrôle de conformité.** On vérifie les règles sur les chunks produits pour la page. Les contrôles de forme portent sur tous les chunks : plafond de 200 mots titre inclus, présence et format complet de la ligne de source (nom de fichier en tirets longs, page, mention de nature, numéro chronologique `SCxxxx` continu à partir de `SC0002`), unicité de la source, format du titre auto-porteur, et — pour les chunks-questions — titre en question pure sans étiquette de profil (aucun tag `(ADV)`, `(commercial)`, `(technique)`, `(SAV)` dans le titre). Pour les chunks d'information complémentaire s'ajoutent les contrôles propres détaillés en fin de section 8 : fidélité à la page, exactitude des synonymes, mention de nature correcte. S'ajoute enfin un **contrôle de couverture SAV, conditionnel** : si la page décrit des composants séparables ou un éclaté numéroté (serrure, paumelles, cylindre, gâche…), au moins une question SAV d'identification ou de remplacement doit avoir été produite. Ce contrôle ne s'applique qu'aux pages qui s'y prêtent : une page de performances thermiques ou de coloris n'a pas de composant à identifier et n'appelle aucune question SAV. En phase actuelle ce contrôle est fait manuellement à la relecture ; c'est la seule étape dont l'automatisation par un petit script de validation reste utile même tôt, car le comptage de mots et la vérification de format sont fastidieux et exacts. Le résultat est une page conforme ou une liste d'anomalies à corriger avant de passer à la suivante.

### Organisation du fichier : deux niveaux de titre, original puis complémentaire

Chaque page produit un ou plusieurs chunks d'information originale, suivis de ses chunks d'information complémentaire. Tous portent le même niveau de titre (`##`), auto-porteur, et se distinguent par la mention de nature en fin de ligne de source. Aucun titre de structure intermédiaire n'est introduit : il n'y a ni titre « document », ni titre « numéro de page », ni titre « information originale », car dans Wikit chaque niveau de titre déclencherait un chunk parasite. Le document est porté par le nom de fichier et le front matter, la page et la nature sont portées par la ligne de source.

```markdown
## H81 Porte d'entrée PVC — Serrure 6 points : composition et résistance à l'effraction
*Source : FIP—H81—08-2025.pdf, page 1 — information originale — SC0002*

La porte d'entrée PVC H81 est équipée en standard d'une serrure manuelle
6 points, composée de deux crochets massifs, d'un pêne demi-tour et de
trois pênes manuels. Elle assure une haute résistance à l'effraction.

## H81 Porte d'entrée PVC — Serrure 6 points : questions fréquentes ADV sur le verrouillage
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0003*

Les collaborateurs ADV demandent souvent combien de points de verrouillage
comporte la porte H81 et si la serrure est manuelle ou automatique. La
serrure standard est manuelle à 6 points. Une version automatique existe
en option. Les termes usuels employés sont serrure multipoints, points de
fermeture, verrouillage renforcé.
```

Les chunks d'une même page se suivent dans le fichier, d'abord les originaux, puis les complémentaires. Cet ordre n'a pas d'effet sur le moteur, qui indexe chaque chunk séparément, mais il rend la lecture humaine du `.md` limpide : on parcourt le fichier page par page, chaque page formant un ensemble cohérent d'information originale et de compléments.

### Reprise du numéro chronologique entre les pages

Un document peut compter plusieurs dizaines de pages, traitées une par une dans des tours successifs. Le numéro chronologique `SCxxxx` doit rester **continu et unique sur tout le fichier**, sans jamais repartir à zéro d'une page à l'autre. La reprise du compteur entre deux pages suit une règle simple, adaptée au travail sans script.

Pour la **première page** d'un document, la numérotation démarre à `SC0002` (le rang `SC0001` étant réservé au résumé Wikit). Pour **chaque page suivante**, juste avant de générer les chunks de la page en cours, l'agent demande à l'humain le dernier numéro de chrono utilisé sur la page précédente, puis reprend la numérotation à ce numéro augmenté de un. Cette demande explicite est préférable à toute autre méthode : l'humain a le fichier `.md` accumulé sous les yeux et connaît donc le dernier `SCxxxx` avec certitude, alors que l'agent peut ne plus l'avoir en mémoire si le fichier est sorti de son contexte. Le compteur n'est donc jamais deviné ni recalculé : il est transmis d'une page à la suivante. Aucun fichier de suivi séparé n'est nécessaire, le `.md` en construction faisant foi.

## 7 ter. Autorité des sources et lecture des PDF

Cette section fixe deux points qui étaient jusqu'ici implicites, et dont le flou a produit des défauts en production : **quelle source fait foi**, et **comment on lit un PDF**. Les deux sont liés, parce qu'un PDF mal lu est un PDF sur lequel on ne peut pas fonder une autorité.

### Le PDF fait foi

**Le PDF est la référence primaire, sans exception.** C'est le document que l'entreprise édite, date, indice et diffuse ; c'est celui que l'ADV a sous les yeux, celui que le commercial ouvre devant le client, celui que le service Produits arbitre. Tout ce que le chatbot restitue doit être vérifiable en ouvrant le PDF à la page citée. Un chunk que l'on ne peut pas confronter au PDF n'est pas traçable, quelle que soit la qualité de la source dont il provient.

**L'Excel n'est pas une référence, c'est une commodité d'extraction.** Sur les tarifs, un fichier Excel accompagne le PDF et facilite la reprise des valeurs chiffrées : il évite de relire des grilles denses cellule par cellule. Cet avantage est réel et on continue de s'en servir. Mais il ne lui confère aucune autorité. L'Excel est un fichier de travail interne, à la nomenclature interne, à la mise à jour désynchronisée, et **il est appelé à disparaître à terme**. Une méthode qui en dépend est une méthode à durée de vie limitée. Toute la chaîne doit donc rester exécutable sans lui.

**Conséquence sur les libellés et les références.** Le libellé d'un poste, la référence d'un produit, sa désignation commerciale, l'unité de facturation et le périmètre de l'offre se relèvent **sur le PDF**. Lorsque l'Excel nomme autrement, c'est le PDF qui est repris et l'écart qui est journalisé. Le cas type relevé sur T81 : l'Excel désigne trois seuils par `AK10123`, `AK10123-RAS1`, `AK10123-RAS2`, quand le tarif publié porte `AS10100`, `AS10100-RA1`, `AS10100-RA2` aux mêmes prix et à la même désignation. Servir la nomenclature Excel rend le chunk inutilisable : l'ADV cherche ce qu'il lit dans le tarif.

**Conséquence sur le périmètre.** Un poste que le PDF ne tarife pas n'existe pas, même s'il figure dans l'Excel. La règle anti-fantôme, jusqu'ici énoncée contre les configurations non tarifées, s'étend aux références : **une référence sans ligne de prix visible dans le PDF ne donne lieu à aucun chunk.** Une référence obsolète que le tarif ne conserve qu'à titre de compatibilité — typiquement dans une colonne « adapté à » — n'est pas une référence à l'offre : elle ne peut pas être servie comme telle. Le cas type : la pièce d'appui `5180`, absente de tout tableau de prix T81, mentionnée seulement comme appui auquel s'adapte l'embout `5186`, et servie par le corpus comme étant à l'offre.

**Conséquence sur les écarts.** Un écart entre l'Excel et le PDF n'est plus un arbitrage à instruire, c'est une correction à appliquer dans le sens du PDF, doublée d'un signalement. La règle de non-arbitrage silencieux continue de s'appliquer aux divergences **internes au PDF** — deux pages qui se contredisent — qui restent exposées avec attribution individuelle et remontées au service Produits.

### Le PDF n'est pas ce que `pdftotext` en dit

Un PDF de tarif est un export InDesign. La maquette y place des planches techniques — posters de profilés, coupes, nomenclatures d'atelier — dans des cadres qui n'en montrent qu'une fraction. **Le cadre rogne à l'affichage, il ne supprime rien du flux de contenu.** Le texte complet de la planche reste dans le fichier, invisible pour tout lecteur humain, et parfaitement extractible.

`pdftotext` ignore les chemins de rognage et restitue ce texte. Mesure faite sur `Tarif_T81_HT_09-07-2026.pdf` : **2 320 jetons sur 23 018 extraits, soit 10,1 %, ne sont visibles nulle part**, concentrés sur 25 des 85 pages, jusqu'à 63 % du texte extrait sur la page 41 et 58 % sur la page 61. On y trouve des références de profilés, des indices de mise à jour, des dates de plan, des cartouches de dessin. Rien ne les distingue d'une donnée de tarif.

Cette couche a déjà produit trois chunks fantômes sur T81 — l'accouplement `NR7` et le seuil `KP484RCY` — et, plus grave, elle a **aveuglé le contrôle** : le script d'audit croisait lui aussi avec `pdftotext`, donc il validait ce qu'il aurait dû rejeter. Générateur et audit partageaient le même angle mort, ce qui est exactement la situation que la règle d'audit autonome vise à interdire.

**Règle.** Trois lectures d'un PDF, trois usages, à ne pas confondre.

| Lecture | Outil | Usage | Fait foi ? |
|---|---|---|---|
| Au rendu | `pymupdf` (`page.get_text()`) | ce qu'un lecteur voit ; existence d'une référence, d'une valeur, d'un libellé | **oui** |
| En géométrie | `pdftotext -layout` | reconstitution de la structure d'une grille dense, alignement colonnes/lignes | non |
| En image | `pdftoppm`, `pdfplumber` | information vectorielle ou colorée : fonds de cellule, pictogrammes, croquis | oui, en appoint du rendu |

`pdftotext` reste utile pour lire la géométrie d'un grand tableau, où il n'a pas d'équivalent. Il n'est **jamais** une preuve d'existence. Avant d'écrire un chunk à partir d'une extraction `-layout`, chaque référence retenue est confirmée au rendu. Le doute se tranche en rastérisant la page et en la regardant.

### Ce que le contrôle croise

Le croisement PDF de l'audit ne porte plus sur le seul montant. Un montant isolé — 33, 39, 17 — se retrouve sur presque n'importe quelle page d'un tarif et ne prouve rien : sur T81, les chunks `SC0222` et `SC0223` citaient la page 60 pour un tableau situé page 61, et le contrôle passait au vert parce que 33 € et 39 € figurent aussi page 60, dans un autre tableau.

Le contrôle porte donc sur le **couple référence + montant, sur la page citée, au rendu**. Quatre issues, et une seule est un succès :

- la référence et le montant sont sur la page citée : conforme ;
- la référence est visible, mais sur une autre page : erreur d'attribution de page, la note de source est fausse ;
- la référence n'est visible nulle part mais existe dans la couche rognée : chunk fantôme, à supprimer ;
- la référence n'existe nulle part dans le PDF : hors tarif, à supprimer ou à corriger vers la nomenclature du PDF.

Un libellé qui énumère plusieurs références — l'Excel en produit, du type `5180-5181-5415-5416` — est décomposé et **chaque référence est contrôlée séparément**. C'est cette décomposition qui fait apparaître qu'un poste agrège des références obsolètes et leurs remplaçantes sous un prix commun, situation qu'un contrôle global masque.

## 8. Enrichissements : compléter la documentation pour le retrieval

La migration seule ne suffit pas. Un modèle léger trouve mieux les réponses quand la documentation anticipe les questions et lui évite d'interpréter. Cette section détaille les enrichissements produits à l'étape 5 du mode opératoire : trois familles de chunks d'information complémentaire, à ajouter **au-delà** du contenu d'origine, sur la même page que celui-ci.

Ces chunks d'enrichissement respectent la même règle que les autres : une source unique et une page unique, avec la mention « information complémentaire » en fin de ligne de source. Un enrichissement dérive toujours du contenu d'une page précise dont il reformule ou explicite l'information ; il porte donc la source de cette page. La règle de fidélité est absolue : **un enrichissement n'introduit jamais une information absente de la page d'origine.** Il reformule, il explicite, il rend accessible ce que la page contient déjà, sous une forme mieux adaptée au moteur. C'est ce point que l'étape 6 contrôle spécifiquement (voir fin de section).

### Famille 1 — Chunks de reformulation par question fréquente

C'est la famille la plus puissante, car elle aligne exactement la formulation de l'utilisateur sur celle du corpus. Le principe : créer des chunks dont **le titre EST la question** telle qu'un utilisateur la pose, et dont le corps donne la réponse directe. Quand la question réelle ressemble mot pour mot au titre du chunk, le score de similarité est maximal et le modèle n'a aucune déduction à faire.

Le travail concret consiste, pour chaque information importante de la page, à se demander « sous quelles formulations cette information sera-t-elle demandée ? », puis à créer un chunk par formulation-type. Une même spécification technique peut ainsi générer plusieurs chunks-questions, selon les angles sous lesquels elle est interrogée.

Ces questions doivent couvrir **quatre points de vue distincts**, car les utilisateurs du chatbot n'abordent pas le produit de la même manière.

Le **point de vue ADV** porte sur la gestion de commande, la faisabilité, les options, les délais, la disponibilité. L'ADV demande « peut-on faire cette porte en telle dimension ? », « telle option est-elle compatible avec tel modèle ? ».

Le **point de vue commercial** porte sur l'argument de vente, le bénéfice client, la différenciation concurrentielle, la garantie. Le commercial demande « qu'est-ce qui rend cette serrure plus sûre que celle d'un concurrent ? », « que répondre à un client qui trouve moins cher ailleurs ? ».

Le **point de vue technique (métreur, poseur)** porte sur la mise en œuvre, les cotes, les tolérances, les contraintes de pose, les compatibilités de chantier. Le métreur ou le poseur demande « quelle est la largeur de passage réelle ? », « quelle réservation prévoir pour ce seuil ? », « ce dormant accepte-t-il une pose en rénovation ? ».

Le **point de vue SAV** porte sur l'identification des composants, les pièces de rechange, le diagnostic d'une panne, le remplacement. L'agent SAV ou le client en après-vente demande « quelle pièce dois-je remplacer sur cette serrure ? », « comment s'appelle ce composant du dormant ? », « cette paumelle est-elle réglable ou à changer ? ». Ce point de vue exploite particulièrement les descriptions de composants et les éclatés numérotés : il transforme une nomenclature en aide au dépannage.

Un même sujet de page donne donc idéalement plusieurs chunks-questions, un par point de vue pertinent, chacun formulé dans le vocabulaire réel du profil concerné. Tous restent tracés sur la même page source et ne contiennent que ce que la page permet de répondre.

**Règle de mesure : la pertinence prime sur la systématique.** Les quatre points de vue sont une grille de lecture, pas un quota. On ne crée un chunk-question pour un point de vue que si ce point de vue interroge réellement le sujet et que la page permet d'y répondre. La serrure intéresse les quatre profils ; une mention de densité du PVC n'intéresse guère que le commercial. Créer mécaniquement quatre questions par sujet produirait des chunks artificiels, sans requête réelle en face, qui diluent l'index. Le bon réflexe est de parcourir les quatre points de vue pour chaque sujet, puis de ne retenir que ceux qui portent une vraie question.

**Règle de formulation : le titre EST la question, sans étiquette de profil.** Le point de vue guide la génération de la question, mais il n'apparaît jamais dans le titre produit. Un titre doit être la question telle que l'utilisateur la tape réellement — « Combien de points de verrouillage et quel type de serrure ? » — et non un intitulé technique suivi d'un tag comme « (ADV) » ou « (commercial) ». Ces étiquettes n'apparaissent dans aucune requête réelle : elles n'aident pas au matching, occupent l'espace du titre et diluent le score de similarité, à l'exact opposé de l'effet recherché. Le profil se lit dans le vocabulaire et l'angle de la question, pas dans une mention explicite.

```markdown
## H81 Porte d'entrée PVC — Combien de points de verrouillage et quel type de serrure ?
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0002*

Les collaborateurs ADV demandent souvent combien de points de verrouillage
comporte la porte H81 et si la serrure est manuelle ou automatique. La
serrure standard est manuelle à 6 points. Une version automatique existe
en option.
```

```markdown
## H81 Porte d'entrée PVC — Qu'est-ce qui rend cette serrure plus sûre que celle d'un concurrent ?
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0002*

Face à un client qui compare, l'argument de sécurité de la serrure H81
repose sur ses 6 points de verrouillage et ses deux crochets massifs qui
remontent dans l'armature du dormant, rendant le décrochement du vantail
impossible depuis l'extérieur. Le cylindre débrayable livré avec carte de
propriété complète cette sécurité. Cette configuration est un point fort
de différenciation à mettre en avant.
```

```markdown
## H81 Porte d'entrée PVC — Quelles implications de pose et de réglage pour la serrure 6 points ?
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0002*

Pour le métreur et le poseur, la serrure 6 points s'accompagne d'une gâche
filante toute hauteur en acier zingué, à intégrer sur toute la hauteur du
dormant. Les crochets se logent dans l'armature métallique du dormant, ce
qui suppose un dormant correctement d'aplomb pour un verrouillage sans
forçage. Le réglage se fait via les paumelles 3D fixées dans les armatures.
```

```markdown
## H81 Porte d'entrée PVC — Quels sont les composants de la serrure et lequel peut-on remplacer ?
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0002*

En après-vente, la serrurerie de la porte H81 se compose de plusieurs
éléments identifiables séparément : le corps de serrure 6 points, les deux
crochets, le pêne demi-tour, les trois pênes manuels, la gâche filante en
acier zingué et le cylindre débrayable. Le cylindre se remplace
indépendamment du corps de serrure. La gâche filante court sur toute la
hauteur du dormant. Ces éléments aident à situer un composant lors d'un
diagnostic ou d'une demande de pièce.
```

### Famille 2 — Injection de synonymes et vocabulaire métier

Les utilisateurs n'emploient pas le vocabulaire du fabricant. Un client ou un commercial dit « poignée » là où la fiche dit « béquille », « double vitrage » là où elle dit « vitrage isolant », « gonds » pour « paumelles ». Si le corpus n'emploie que le terme fabricant, la question formulée en langage courant matche mal le chunk.

Le travail concret consiste à identifier, pour chaque sujet technique de la page, les termes que la page emploie dans leur forme fabricant, puis à lister leurs équivalents courants dans un **chunk de synonymes dédié**. La règle est stricte : les synonymes vivent exclusivement dans ce chunk dédié, et ne sont jamais disséminés en fin de corps des chunks-questions ou des chunks techniques. Ce choix privilégie la lisibilité et l'auditabilité : un seul endroit rassemble le vocabulaire courant d'un sujet, facile à relire et à contrôler à l'étape 6. Cette redondance lexicale contrôlée rapproche les requêtes réelles du contenu indexé, sans dépasser le plafond de mots. Le glossaire transversal centralise ces correspondances pour garantir qu'un même terme reçoit toujours les mêmes synonymes d'un document à l'autre.

Les quatre points de vue enrichissent aussi cette famille : le commercial et le client emploient un vocabulaire grand public, le métreur et le poseur un vocabulaire de chantier (tableau, feuillure, rejingot, réservation), l'ADV un vocabulaire de références et d'options, le SAV un vocabulaire de composants et de pièces détachées. Les registres méritent d'être injectés.

```markdown
## H81 Porte d'entrée PVC — Serrurerie : vocabulaire courant et équivalences métier
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0002*

Les termes employés par les clients et les équipes pour désigner la
serrurerie de la porte H81 varient. La béquille est aussi appelée poignée.
La serrure 6 points est dite serrure multipoints ou verrouillage multipoints.
Le cylindre est appelé barillet. La gâche filante est parfois nommée
gâche continue. Ces équivalences aident à retrouver l'information quelle
que soit la formulation employée.
```

### Famille 3 — Descriptions textuelles du contenu visuel

Les schémas cotés, les éclatés numérotés, les nuanciers, les photos et les bandeaux de labels et de certifications portent une information absente du texte extrait. Le modèle ne voit pas les images ; toute information qui n'existe que dans un visuel lui est inaccessible tant qu'elle n'est pas décrite en toutes lettres.

Le travail concret consiste, pour chaque visuel utile de la page, à créer un chunk qui décrit textuellement ce que montre l'image : la correspondance entre les numéros d'un éclaté et les pièces qu'ils désignent, les noms exacts des teintes d'un nuancier, la configuration présentée sur une photo, les cotes portées sur un schéma. La description reste strictement fidèle au visuel : elle transcrit ce qui est montré, elle n'extrapole pas.

Les quatre points de vue orientent ici le niveau de détail : le métreur et le poseur ont besoin des cotes et des repères de montage lus sur le schéma, le commercial des teintes et des finitions visibles sur le nuancier, l'ADV des configurations et références illustrées, le SAV de la correspondance entre les repères numérotés d'un éclaté et les pièces qu'ils désignent, pour identifier un composant à remplacer.

**Les bandeaux de labels demandent une précaution supplémentaire.** Presque
toutes les fiches du corpus portent en marge une colonne de pictogrammes — CE,
CEKAL, Qualimarine, Qualanod, étiquette d'émissions A+, médaillons de garantie
et de fabrication française, avis technique. Ces labels n'apparaissent souvent
nulle part dans le texte courant de la page : ils n'existent que sous forme de
logo, et sont donc invisibles au modèle tant qu'ils ne sont pas décrits.

Un chunk qui restitue un élément du bandeau doit **dire d'où il vient**, par une
formule du type « le bandeau de labels affiché en marge de la fiche porte… ».
Sans cette mention, l'information paraît sortie de nulle part : un relecteur qui
vérifie le chunk contre le texte du PDF ne la trouvera pas et conclura à une
invention. Le cas s'est produit sur la Fiche Excellence de la TA76 OV, dont un
chunk mentionnait le label Qualanod, absent du texte des dix critères mais bien
présent dans le bandeau ; la mention a d'abord été prise pour une contamination
depuis la gamme jumelle.

Le bandeau ne dit pas la même chose que le texte, et il ne faut pas les
confondre. Sur la TA76 OV, le critère 9 énumère le thermolaquage Qualimarine et
le ton bois, sans l'anodisation, alors que le bandeau porte le logo Qualanod. Un
logo atteste l'existence d'un label, il ne prouve pas que la finition
correspondante est proposée sur la gamme. On décrit donc le bandeau pour ce
qu'il est — une liste de labels affichés — sans en déduire une caractéristique
produit, qui doit être établie sur le texte ou sur une autre source.

```markdown
## H81 Porte d'entrée PVC — Éclaté technique : correspondance des repères numérotés
*Source : FIP—H81—08-2025.pdf, page 1 — information complémentaire — SC0002*

Le schéma en coupe de la fiche numérote les composants de la porte H81. Le
repère 1 désigne les profilés renforcés par armatures, le repère 2 les
inserts thermo-soudés d'angle, le repère 3 les paumelles à réglage 3D, le
repère 4 la serrure 6 points, les repères 5 à 7 les chambres d'isolation et
les joints, le repère 8 le seuil aluminium à rupture de pont thermique. Cette
description rend accessible l'information que seul le schéma portait.
```

```markdown
## TA76 OC Fenêtre Aluminium à ouvrant caché — Bandeau de labels affiché en marge de la fiche
*Source : FE—TA76—OC—08-2023.pdf, page 1 — information complémentaire — SCxxxx*

Le bandeau de labels affiché en marge de la Fiche Excellence porte les
pictogrammes suivants. Un médaillon « Garantie 30 ans » renvoie au Carnet de
garantie. Un médaillon « Fabrication française » signale la production en
France. Le marquage CE cite la norme NF EN 14351-1 +A2:2016. Un logo DTA
signale l'avis technique. Le logo CEKAL atteste les vitrages certifiés.
L'étiquette d'émissions dans l'air intérieur affiche A+. Les logos Qualimarine
et Qualanod désignent les labels du thermolaquage et de l'anodisation. Ces
pictogrammes ne sont pas repris dans le texte des dix critères.
```

### Famille 4 — Chunks de discrimination inter-gammes

Certaines gammes se ressemblent au point que le moteur les confond. Deux gammes de la même famille, aux codes proches (H81 et H81 Access, HA76 et HAM76, TA76 OV et TA76 OC), partagent un vocabulaire et une structure quasi identiques. Lorsqu'une question présuppose, sur une gamme, une caractéristique qu'elle ne possède pas mais que possède sa voisine immédiate, le retrieval remonte le chunk de la voisine et le modèle attribue la caractéristique à la mauvaise gamme. Le corpus ne contient alors aucun chunk qui dise « cette gamme n'a pas cette caractéristique » : l'absence n'est écrite nulle part, donc elle n'est pas récupérable, et le modèle comble le vide en important de la voisine. Un exemple constaté : interrogé sur le matériau des « crochets massifs » de la H81 Access — qui n'en a pas —, le modèle importait la serrure 6 points à crochets massifs de la porte d'entrée H81.

Le travail concret consiste à créer un **chunk qui rend l'absence explicite, donc récupérable** : il énonce positivement la caractéristique réelle de la gamme, nie explicitement la caractéristique importée à tort, et nomme la gamme voisine qui, elle, la possède. Ce renvoi nommé aide le modèle à ne pas confondre et rend service à l'utilisateur ADV, qui apprend où trouver ce qu'il cherchait.

Cette famille se distingue des trois autres par une règle de déclenchement stricte, car l'espace de « ce qu'une gamme n'est pas » est infini et ne doit jamais être documenté systématiquement. Un chunk de discrimination n'est créé que si trois conditions sont réunies : (1) un **contexte documenté** l'a motivé — un cas avéré, typiquement une question posée et la mauvaise réponse obtenue, qui prouve que la confusion se produit réellement ; (2) la caractéristique litigieuse est **absente** de la gamme interrogée dans son corpus déjà corrigé ; (3) elle est **présente** chez une gamme voisine. La troisième condition est ce qui borne la famille : on ne documente le négatif que là où une gamme sœur porte la caractéristique et crée donc une confusion possible. Une caractéristique qui n'existe sur aucune gamme (le modèle la rejette généralement bien) ne justifie pas de chunk. On ne rédige jamais de chunk de discrimination « au cas où » ni en anticipation à la migration : la famille est réactive, jamais préventive.

La création est encadrée par le script `generer_chunk_discrimination.py`, qui exige le contexte documenté, vérifie objectivement l'absence dans la gamme et la présence chez la voisine, impose le gabarit et les contrôles de conformité, refuse de produire si une condition manque, et journalise chaque chunk avec la preuve qui l'a motivé. La condition (2) impose un ordre : si le corpus de la gamme contient encore la caractéristique importée, il faut d'abord la corriger (retrait de la contamination) avant d'ajouter le chunk de discrimination, sans quoi le fichier se contredirait.

**Titre et accentuation.** Le titre d'un chunk de discrimination suit le gabarit
commun à tout le corpus : `## [CODE] [Libellé de gamme] — [sujet]`. Le seul code
de gamme ne suffit pas — un chunk ainsi titré est moins récupérable que ses
voisins et sort du gabarit. Le corps et la ligne de source doivent par ailleurs
être accentués : sur un moteur sémantique, « fermee » et « fermée » ne produisent
pas le même token, et un chunk désaccentué devient partiellement invisible au
retrieval, ce qui prive de son effet le garde-fou qu'il est censé constituer. Le
script `generer_chunk_discrimination.py` impose désormais les deux : le champ
`libelle` est obligatoire dans le contexte, et un texte français d'une longueur
significative totalement dépourvu d'accents déclenche un refus de production.

La ligne de source de ces chunks suit une convention propre, car ils ne proviennent pas d'une page de PDF mais sont déduits d'une absence : `*Source : discrimination inter-gammes [Gamme] — information complémentaire — SCxxxx — motivé par : [référence de la preuve]*`. Cette convention rend visible d'un coup d'œil qu'il s'agit d'un garde-fou et non d'une citation du document, et trace le chunk jusqu'à son cas d'origine.

```markdown
## H81 Access Porte de service PVC — Fermeture 5 points à galets : pas de serrure 6 points à crochets massifs
*Source : discrimination inter-gammes H81—Access — information complémentaire — SC0028 — motivé par : conversations Wikit mal notées 2026-07-21*

La porte de service H81 Access est fermée par une fermeture 5 points à galets,
complétée par une gâche centrale et un cylindre de sécurité débrayable. Elle ne
comporte pas de serrure 6 points ni de crochets massifs. La serrure 6 points à
deux crochets massifs équipe la porte d'entrée H81, qui est une gamme distincte :
cette caractéristique ne doit pas être attribuée à la H81 Access.
```

### Famille 5 — Chunks de précision inter-documents

Les six documents d'une même gamme ne décrivent pas la même caractéristique avec
la même précision. Un document peut qualifier une valeur par la configuration qui
l'atteint là où un autre se contente d'une formule générale. Les deux énoncés ne
se contredisent pas : l'un est plus précis que l'autre. Chaque chunk restant
fidèle à sa source, le corpus se retrouve avec plusieurs chunks portant la même
valeur, dont certains seulement portent la restriction qui en délimite la portée.

Le moteur n'a aucune raison de préférer le chunk précis : tous ont le même poids
au retrieval. Pire, un chunk peut offrir au modèle deux qualificatifs concurrents
pour une même valeur, dont l'un ne restreint rien ; le modèle retient alors le
qualificatif inoffensif et supprime la restriction. Le cas constaté : la valeur
plafond de Ud d'une porte est qualifiée « pour un modèle sans vitrage » par la
Fiche Info Produit et « selon les modèles » par le CABP et la Fiche Excellence.
Interrogé sur la performance de la gamme, le modèle restituait la valeur nue,
sans aucune restriction, en citant pourtant le chunk qui la portait.

Propager la précision dans les chunks des autres documents est exclu : cela leur
ferait dire ce que leur page source ne dit pas, et rendrait infidèles des chunks
étiquetés « information originale ». Le travail concret consiste donc à créer un
**chunk qui énonce la précision et nomme le document qui l'établit**, en laissant
les chunks d'origine intacts. Ce chunk expose le rapport entre les formulations
— laquelle est la plus précise, et que les autres ne la contredisent pas — puis
énonce ce qui ne peut pas être déduit. Cette dernière partie est essentielle :
c'est elle qui empêche le modèle de combler par extrapolation ce que la
documentation ne dit pas.

Un chunk de cette famille peut renvoyer à une autre couche documentaire lorsque
celle-ci permet d'exploiter la précision. Dans le cas cité, le tarif décrit le
remplissage de chaque modèle et permet donc d'identifier les modèles sans
vitrage, alors qu'il ne porte aucun coefficient thermique. Ce renvoi est le seul
cas où un chunk de documentation produit cite le tarif ; il doit énoncer
explicitement ce que la couche citée permet et ce qu'elle ne permet pas.

Trois conditions encadrent la création :

1. Les formulations divergentes doivent avoir été relevées sur les **PDF
   sources**, pas déduites d'une comparaison entre fichiers Markdown. Un écart
   entre deux chunks peut refléter une différence de contenu source légitime.
2. Les énoncés doivent être **compatibles**, l'un précisant l'autre. S'ils se
   contredisent réellement, il ne s'agit pas de cette famille mais d'une
   divergence documentaire, qui relève de l'arbitrage produit et de la règle
   d'exposition des divergences.
3. Les chunks d'origine restent **inchangés**. Cette famille ajoute, elle ne
   corrige jamais.

La ligne de source suit une convention propre, car ces chunks ne proviennent pas
d'une page de PDF mais du rapprochement de plusieurs :
`*Source : précision inter-documents [Gamme] — information complémentaire — SCxxxx — motivé par : [documents et emplacements qui établissent la précision]*`.

```markdown
## HAM76 Porte d'entrée monobloc Aluminium — Portée du coefficient Ud de 1,1 W/m².K : modèles sans vitrage
*Source : précision inter-documents HAM76 — information complémentaire — SC0031 — motivé par : FIP—HAM76—04-2024.pdf, encart PERFORMANCES*

Le coefficient Ud de 1,1 W/m².K est la meilleure valeur de la gamme HAM76. La
Fiche Info Produit précise qu'elle est atteinte pour un modèle sans vitrage.

Les autres documents de la gamme énoncent la même valeur sans cette précision :
le CABP et la Fiche Excellence indiquent un Ud jusqu'à 1,1 W/m².K « selon les
modèles », sans dire quels modèles atteignent ce plafond. Ces formulations ne
contredisent pas la Fiche Info Produit, elles sont moins précises. C'est donc la
précision de la Fiche Info Produit qui s'applique.

Deux conséquences pour la réponse à un client ou à un commercial. La valeur de
1,1 W/m².K ne doit pas être présentée comme atteinte par tous les modèles de la
gamme. Et la documentation ne fournit aucun coefficient Ud propre aux modèles
avec vitrage : cette valeur n'existe dans aucun document de la gamme et ne peut
pas être déduite.
```

Un chunk de renvoi peut compléter le premier en format question, pour rendre la
précision exploitable. Il nomme alors les éléments concernés et referme
explicitement ce que la couche citée ne permet pas :

```markdown
## HAM76 Porte d'entrée monobloc Aluminium — Quels modèles atteignent le Ud de 1,1 W/m².K ?
*Source : précision inter-documents HAM76 — information complémentaire — SC0032 — motivé par : FIP—HAM76—04-2024.pdf, encart PERFORMANCES et Tarif—HAM76—HT—04-05-2026.pdf, caractéristiques des modèles*

[…] Le tarif ne porte aucun coefficient thermique par modèle. Il permet
d'identifier le remplissage de chaque modèle, pas de lui attribuer une valeur de
Ud. Aucun document de la gamme ne donne de Ud propre aux modèles avec vitrage.
```

Un chunk qui nomme des éléments issus du tarif devient **dépendant du tarif en
vigueur** : il doit être revu à chaque révision tarifaire. Cette dépendance est
à consigner au moment de la création.

### Articulation avec l'étape 6 : contrôle spécifique des enrichissements

L'étape 6 vérifie sur les chunks d'enrichissement les mêmes règles de forme que sur les autres (plafond de 200 mots, ligne de source, source unique, titre auto-porteur, et pour les chunks-questions un titre en question pure sans étiquette de profil), plus quatre contrôles qui leur sont propres. Le premier est le **contrôle de fidélité** : chaque affirmation d'un chunk complémentaire doit être justifiable par le contenu de la page source ; toute information ajoutée qui n'y figure pas est une anomalie à corriger. Le deuxième est le **contrôle d'exactitude des synonymes** : un synonyme n'est légitime que s'il désigne réellement la même chose. Un terme voisin mais techniquement distinct est une erreur à retirer, même s'il est plausible. Les faux synonymes récurrents déjà identifiés, à bannir systématiquement, sont : « crémone » pour la serrure multipoints (une crémone est un autre mécanisme), « charnière » pour la paumelle (pièces distinctes), « survitrage » pour le triple vitrage isolant (un survitrage est une seconde vitre rapportée sur un vitrage existant, pas un vitrage isolant intégré). À l'inverse, « barillet » pour le cylindre est correct. Cette liste est évolutive et s'enrichit à mesure que de nouveaux cas apparaissent. Le troisième est le **contrôle de couverture SAV** : si la page décrit des composants séparables ou un éclaté numéroté, on vérifie qu'au moins une question SAV d'identification ou de remplacement a été produite, sans forcer de chunk artificiel sur une page qui ne s'y prête pas. Le quatrième est le **contrôle de mention** : la ligne de source doit porter « information complémentaire », et non « information originale ». Un cinquième contrôle s'applique spécifiquement aux chunks de discrimination inter-gammes (famille 4) : le **contrôle de justification et de non-contradiction**. On vérifie que le chunk est bien rattaché à un contexte documenté (la preuve qui l'a motivé), que la caractéristique niée est effectivement absente du reste du corpus de la gamme — un chunk de discrimination ne doit jamais coexister avec un chunk qui affirmerait la même caractéristique — et que la caractéristique est bien attribuée à la gamme voisine nommée.  Un sixième contrôle s'applique aux chunks de précision inter-documents (famille 5) : le **contrôle de compatibilité et de non-altération**. On vérifie que les formulations rapprochées ont été relevées sur les PDF sources et non déduites d'une comparaison entre fichiers Markdown, qu'elles sont compatibles entre elles — l'une précisant l'autre et non la contredisant, faute de quoi le cas relève de la divergence documentaire —, que les chunks d'origine n'ont pas été modifiés, et que le chunk énonce ce qui ne peut pas être déduit de la documentation. Lorsqu'il renvoie au tarif ou à une autre couche, on vérifie qu'il précise ce que cette couche permet et ne permet pas, et que sa dépendance à la version en vigueur est consignée. Ces vérifications sont assurées par le script dédié, mais restent à recontrôler à l'étape 6 comme les autres. Ces contrôles garantissent que la couche d'enrichissement reste un facilitateur de retrieval fidèle, exact et complet, et jamais une source d'information nouvelle ou erronée non tracée.

## 9. Posture de l'agent rédacteur : consignes d'attitude

Cette section s'adresse directement à l'agent — IA ou humain — qui exécute la migration. Les règles précédentes disent quoi produire ; celle-ci dit dans quel état d'esprit le faire. Les six principes ci-dessous répondent à des erreurs réellement observées et priment sur tout réflexe de rédacteur.

**Fidélité avant tout : n'invente jamais.** Ta matière première est la page, et rien qu'elle. Si une information n'est pas sur la page, elle n'existe pas pour toi : ne la déduis pas, ne la complète pas par ce qui te semble plausible ou vrai « en général ». Une porte peut sembler « destinée à l'habitat » ou « idéale pour les maisons individuelles », mais si la page ne le dit pas, tu ne l'écris pas. Cette discipline vaut pour l'information originale comme pour les enrichissements : un chunk-question ne répond qu'avec ce que la page permet de répondre.

**Avance par petits pas validés.** Ne cherche jamais à produire un document entier d'un seul mouvement. Traite une page à la fois, et à l'intérieur d'une page, une étape à la fois. Livre le résultat de chaque étape et attends la validation avant de passer à la suivante. C'est plus lent en apparence, mais cela évite l'erreur qui se propage : une omission au découpage contamine toute la rédaction en aval. Le séquencement en tours n'est pas une formalité, c'est ta protection contre tes propres angles morts.

**Demande plutôt que de supposer.** Face à une ambiguïté — un terme mal extrait, un tableau dont les colonnes se mélangent, un doute sur le point de vue pertinent, une valeur chiffrée illisible — arrête-toi et pose la question. Ne tranche pas seul en pariant sur l'interprétation la plus probable. Une question coûte quelques secondes à l'humain qui te relit ; une supposition fausse coûte une relecture complète et fragilise la confiance dans tout le corpus.

**Signale tes doutes au moment où tu produis.** Quand un chunk repose sur une interprétation, quand un synonyme te semble incertain, quand tu hésites sur l'atomicité d'un sujet, dis-le explicitement en livrant le chunk. N'attends pas qu'on le découvre. Le rôle de la validation humaine est de se concentrer là où le risque est ; à toi de signaler où il se trouve. Un doute annoncé est une aide, un doute caché est un piège.

**Préfère la sobriété au brillant.** Ton rôle est de transposer, pas de vendre ni d'embellir. N'ajoute pas d'arguments commerciaux qui ne sont pas sur la page, ne renforce pas un bénéfice, ne « améliore » pas le produit par une formulation flatteuse. Une prose neutre, exacte et complète sert mieux le moteur qu'une prose valorisante. Le vocabulaire reste normalisé, les phrases restent simples, l'information reste ce qu'elle est.

**Accepte la redondance, refuse la synthèse.** Ton instinct de rédacteur te poussera à consolider, à dédupliquer, à fusionner ce qui se répète d'un document à l'autre. Désactive ce réflexe. Ici, un même sujet présent dans trois documents donne trois chunks distincts, chacun tracé sur sa source. Tu n'as pas à relier les documents entre eux ni à produire une version unifiée : chaque page est traitée pour elle-même, fidèlement, isolément. La cohérence d'ensemble n'est pas ton affaire, la fidélité de chaque chunk l'est.
