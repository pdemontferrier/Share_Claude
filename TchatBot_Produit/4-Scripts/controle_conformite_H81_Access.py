#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Contrôle de conformité des chunks Markdown du tarif H81 Access.

Ce script ne réutilise aucune fonction de `generateur_tarif_H81_Access.py` : il
relit les Markdown produits, redéclare ses propres tables — table des pages,
enveloppes dimensionnelles relevées sur les pages-modèles, tables de prix saisies
littéralement depuis le PDF — de sorte qu'une divergence entre les deux soit un
écart réel et non une tautologie.

Quinze familles de contrôle : front matter, décomptes, forme et plafond, ligne de
source, continuité SC, unicité des titres, table des pages, fidélité numérique
des prix de portes, fidélité et couverture exhaustive de la grille des fixes et
du meneau battant, bijection des postes forfaitaires par multiensembles,
déclaration d'une unité de facturation, absence de montant en méthode,
faisabilités et transverses, taux proportionnels, caractéristiques et enveloppes,
anti-fantôme, discrimination H81 / H81 Access et vocabulaire, liant
inter-fichiers.

Sortie : code 0 si aucune anomalie, 1 sinon.
"""
import collections
import os
import re
import sys

# --------------------------------------------------------------------------
# Tables de prix saisies littéralement depuis le PDF, page par page.
# Elles proviennent de `verif_A2_excel_vs_pdf_H81_Access.py`, dont la saisie a
PDF_MODELES = [
    ("Porte vitrée",        21, 2273, 4113, 2039, 3689),
    ("Porte panneau plein", 22, 2273, 4113, 2039, 3689),
    ("Cypris",              23, 2814, 4762, 2524, 4272),
    ("Dahpnis",             24, 2814, 4762, 2524, 4272),
    ("Madrane",             25, 2814, 4762, 2524, 4272),
    ("Persane",             26, 2814, 4762, 2524, 4272),
    ("Santéria",            27, 2489, 4437, 2233, 3981),
    ("T1L",                 28, 2814, 4762, 2524, 4272),
    ("T2L",                 29, 2489, 4437, 2233, 3981),
    ("Melbourne",           30, 3149, 5866, 2825, 5261),
    ("Vienne",              31, 3149, 5866, 2825, 5261),
]

HAUTEURS = list(range(300, 2700, 100))
GRILLE_HT = [
[149,162,186,196,215,243,262,273,284,294,305,315,326,336,347,358,368,391,402,413,426,442,457,473],
[162,187,200,219,248,269,281,293,305,316,328,340,352,364,382,402,420,454,473,493,512,532,551,571],
[186,200,220,250,273,286,299,312,325,338,358,379,402,425,447,469,492,531,553,576,600,623,645,669],
[196,219,250,274,288,303,318,332,355,381,407,433,459,485,511,537,563,606,634,660,687,714,740,767],
[215,248,273,288,305,320,340,370,399,428,457,486,517,546,575,604,634,683,714,744,774,803,834,864],
[243,269,286,303,320,344,376,410,442,474,508,540,573,606,639,671,705,759,794,827,861,894,928,961],
[262,281,299,318,340,376,413,448,485,521,558,595,630,667,703,740,775,836,873,911,947,985,1022,1060],
[273,293,312,332,370,410,448,489,529,569,608,648,688,727,767,807,847,913,953,994,1035,1075,1116,1157],
[284,305,325,355,399,442,485,529,572,615,658,702,744,787,830,874,917,988,1033,1077,1122,1166,1210,1255],
[294,316,338,381,428,474,521,569,615,662,708,755,801,848,894,942,988,1065,1113,1161,1209,1257,1304,1352],
[305,328,358,407,457,508,558,608,658,708,758,809,859,908,959,1009,1059,1141,1193,1244,1296,1348,1399,1451],
[315,340,379,433,486,540,595,648,702,755,809,862,916,969,1023,1076,1130,1218,1273,1327,1382,1438,1493,1548],
[326,352,402,459,517,573,630,688,744,801,859,916,972,1030,1087,1144,1201,1294,1352,1412,1470,1528,1587,1794],
[336,364,425,485,546,606,667,727,787,848,908,969,1030,1090,1151,1211,1272,1370,1432,1495,1557,1619,1833,1901],
[347,382,447,511,575,639,703,767,830,894,959,1023,1087,1151,1215,1278,1342,1446,1512,1578,1792,1865,1936,2009],
[358,402,469,537,604,671,740,807,874,942,1009,1076,1144,1211,1278,1347,1414,1523,1592,1812,1888,1965,2041,2117],
[368,420,492,563,634,705,775,847,917,988,1059,1130,1201,1272,1342,1414,1484,1600,1824,1904,1984,2064,2145,2225],
[391,454,531,606,683,759,836,913,988,1065,1141,1218,1294,1370,1446,1523,1600,1828,1912,1996,2080,2165,2249,2334],
[402,473,553,634,714,794,873,953,1033,1113,1193,1273,1352,1432,1512,1592,1824,1912,2000,2088,2177,2264,2353,2441],
[413,493,576,660,744,827,911,994,1077,1161,1244,1327,1412,1495,1578,1812,1904,1996,2088,2181,2273,2365,2457,2549],
[426,512,600,687,774,861,947,1035,1122,1209,1296,1382,1470,1557,1792,1888,1984,2080,2177,2273,2369,2464,2561,2657],
[442,532,623,714,803,894,985,1075,1166,1257,1348,1438,1528,1619,1865,1965,2064,2165,2264,2365,2464,2565,2665,2765],
[457,551,645,740,834,928,1022,1116,1210,1304,1399,1493,1587,1833,1936,2041,2145,2249,2353,2457,2561,2665,2770,2874],
[473,571,669,767,864,961,1060,1157,1255,1352,1451,1548,1794,1901,2009,2117,2225,2334,2441,2549,2657,2765,2874,2982],
]
GRILLE_TTC = [
[128,139,160,169,184,209,226,235,244,253,262,271,281,288,298,308,316,336,346,354,366,380,393,406],
[139,161,171,189,214,231,242,252,262,272,282,293,302,313,328,346,361,390,406,424,440,457,473,491],
[160,171,190,216,235,246,257,269,280,291,308,326,346,365,385,403,423,456,476,495,516,535,555,575],
[169,189,216,236,248,261,273,285,306,327,350,373,394,417,439,461,484,521,544,566,589,613,635,658],
[184,214,235,248,262,275,293,318,342,367,393,418,444,469,494,519,544,587,613,639,665,690,716,742],
[209,231,246,261,275,295,323,352,380,407,437,465,492,521,549,576,605,652,681,710,740,768,797,825],
[226,242,257,273,293,323,354,386,417,447,479,510,542,573,603,635,666,718,749,782,813,846,877,909],
[235,252,269,285,318,352,386,419,454,489,522,557,590,624,658,693,727,784,817,853,889,922,958,994],
[244,262,280,306,342,380,417,454,491,529,565,602,639,676,714,750,787,849,887,925,963,1000,1039,1077],
[253,272,291,327,367,407,447,489,529,569,609,648,688,728,768,809,849,915,955,996,1038,1079,1119,1161],
[262,282,308,350,393,437,479,522,565,609,651,694,737,780,824,866,909,980,1024,1067,1112,1157,1201,1245],
[271,293,326,373,418,465,510,557,602,648,694,740,786,832,878,924,970,1046,1092,1139,1187,1234,1282,1328],
[281,302,346,394,444,492,542,590,639,688,737,786,835,884,933,982,1031,1111,1161,1211,1262,1312,1362,1539],
[288,313,365,417,469,521,573,624,676,728,780,832,884,935,987,1040,1092,1177,1230,1283,1336,1390,1573,1631],
[298,328,385,439,494,549,603,658,714,768,824,878,933,987,1043,1098,1152,1242,1298,1354,1538,1601,1662,1724],
[308,346,403,461,519,576,635,693,750,809,866,924,982,1040,1098,1156,1214,1308,1367,1556,1620,1686,1752,1817],
[316,361,423,484,544,605,666,727,787,849,909,970,1031,1092,1152,1214,1274,1374,1565,1634,1703,1772,1841,1909],
[336,390,456,521,587,652,718,784,849,915,980,1046,1111,1177,1242,1308,1374,1570,1641,1713,1785,1857,1930,2002],
[346,406,476,544,613,681,749,817,887,955,1024,1092,1161,1230,1298,1367,1565,1641,1717,1791,1868,1943,2020,2094],
[354,424,495,566,639,710,782,853,925,996,1067,1139,1211,1283,1354,1556,1634,1713,1791,1871,1950,2029,2108,2187],
[366,440,516,589,665,740,813,889,963,1038,1112,1187,1262,1336,1538,1620,1703,1785,1868,1950,2033,2115,2197,2281],
[380,457,535,613,690,768,846,922,1000,1079,1157,1234,1312,1390,1601,1686,1772,1857,1943,2029,2115,2202,2287,2373],
[393,473,555,635,716,797,877,958,1039,1119,1201,1282,1362,1573,1662,1752,1841,1930,2020,2108,2197,2287,2377,2466],
[406,491,575,658,742,825,909,994,1077,1161,1245,1328,1539,1631,1724,1817,1909,2002,2094,2187,2281,2373,2466,2559],
]

MEN_LONG = [320,350,400,500,600,700,800,900,1000,1100,1200,1300,1400,1500,1600,1700,
            1800,1900,2000,2100,2200,2300,2400,2500,2600]
MEN_HT  = [19,20,23,29,34,39,45,50,57,62,68,73,78,84,89,95,100,105,111,116,122,127,133,138,143]
MEN_TTC = [17,18,21,25,30,34,39,44,49,54,59,63,68,73,77,82,86,91,96,100,105,110,114,118,124]

BLOCS = [
 ("Pack Evo", 47, 50, 11,
  [531,163,120,102], [460,141,104,89]),
 ("PV vitrages portes", 52, 79, 16,
  [0,12,48,36,113,22,103,65,0,125,24,48,54,54,60,60,191,191,191,24,48,54,54,60,60,191,191,191],
  [0,11,42,32,98,19,89,57,0,109,21,42,46,46,52,52,165,165,165,21,42,46,46,52,52,165,165,165]),
 ("Vitrages pour fixes", 131, 158, 18,
  [0,11,22,55,22,103,22,103,65,125,24,48,54,54,60,60,191,191,191,24,48,54,54,60,60,191,191,191],
  [0,10,19,47,19,89,19,89,57,109,21,42,46,46,52,52,165,165,165,21,42,46,46,52,52,165,165,165]),
 ("Croisillons", 160, 179, 19,
  [18,13,31,28,22,20,18,13,31,28,22,20,36,24,22,19,22,19,22,19],
  [16,11,26,24,19,18,16,11,26,24,19,18,36,24,19,17,19,17,19,17]),
 ("Remplissage", 181, 190, 20,
  [39,180,205,436,167,391,224,52,63,63],
  [34,156,178,377,144,142,194,46,55,55]),
 ("PV vitrages panneaux", 192, 199, 32,
  [0,0,0,0,0,88,88,88], [0,0,0,0,0,77,77,77]),
 ("Garnitures", 201, 204, 33,
  [0,0,102,102], [0,0,89,89]),
 ("Options et accessoires", 206, 208, 34,
  [307,558,122], [266,483,105]),
 ("Élargisseurs", 210, 215, 35,
  [17,21,26,17,21,26], [15,19,23,15,19,23]),
 ("Profilés complémentaires", 217, 222, 36,
  [17,21,26,22,28,12], [15,19,23,20,24,11]),
 ("Tapées de doublage", 224, 227, 37,
  [19,24,19,24], [17,22,17,22]),
 ("Accouplements statiques", 229, 233, 38,
  [17,21,72,72,86], [15,19,63,63,75]),
 ("Seuils", 235, 239, 39,
  [38,49,11,17,11], [34,43,10,15,10]),
 ("Fabrications spéciales : cintres", 241, 246, 41,
  [742,742,1092,1092,1092,111], [642,642,946,946,946,97]),
 ("Exemple de calculs", 248, 257, 42,
  [2273,2273,2814,423,3236,3237,531,489,39,4294],
  [2039,2039,2524,379,1821,2902,460,521,34,3916]),
]

# --------------------------------------------------------------------------
# Tables redéclarées par l'audit
# --------------------------------------------------------------------------

PREFIXE = "H81 Access Porte de service PVC"
DOC_AFFICHE = "Tarif—H81—Access—HT—08-04-2026.pdf"
DOC_YAML = "Tarif_H81_Access_HT_08-04-2026.pdf"
PLAFOND = 200

FICHIERS = {
    "Tarif_H81_Access_METHODE.md": "methode",
    "Tarif_H81_Access_PRIX_PORTES.md": "prix_portes",
    "Tarif_H81_Access_PRIX_FIXES.md": "prix_fixes",
    "Tarif_H81_Access_OPTIONS.md": "options",
    "Tarif_H81_Access_PLUS_VALUES_PROPORTIONNELLES.md": "plus_values_proportionnelles",
    "Tarif_H81_Access_CARACTERISTIQUES.md": "caracteristiques",
    "Tarif_H81_Access_FAISABILITES.md": "faisabilites",
    "Tarif_H81_Access_TRANSVERSES.md": "transverses",
}

SANS_MONTANT = {"Tarif_H81_Access_METHODE.md",
                "Tarif_H81_Access_FAISABILITES.md",
                "Tarif_H81_Access_TRANSVERSES.md"}

# Pages du PDF admissibles par fichier, relevées indépendamment sur le document.
PAGES_ADMISES = {
    "Tarif_H81_Access_METHODE.md": {2, 12, 13, 17, 19, 37, 40, 41},
    "Tarif_H81_Access_PRIX_PORTES.md": set(range(21, 32)),
    "Tarif_H81_Access_PRIX_FIXES.md": {17},
    "Tarif_H81_Access_OPTIONS.md": set(range(21, 32)) | {11, 12, 13, 16, 18, 19,
                                                         20, 32, 33, 34, 35, 36,
                                                         37, 38, 39, 41},
    "Tarif_H81_Access_PLUS_VALUES_PROPORTIONNELLES.md": {14, 41},
    "Tarif_H81_Access_CARACTERISTIQUES.md": set(range(21, 32)),
    "Tarif_H81_Access_FAISABILITES.md": {7, 9, 12, 14, 21, 40},
    "Tarif_H81_Access_TRANSVERSES.md": {8, 13, 15, 33, 43, 45},
}

# Enveloppe dimensionnelle par modèle, relevée sur les pages-modèles du PDF
# (Lmin, Lmax, Hmin, Hmax sur les quatre profils de dormant).
ENVELOPPES_PDF = {
    "Porte vitrée": (656, 1405, 1483, 2793),
    "Porte panneau plein": (656, 1405, 1483, 2793),
    "Cypris": (656, 1405, 1483, 2793),
    "Dahpnis": (656, 1405, 1483, 2793),
    "Madrane": (656, 1405, 1483, 2793),
    "Persane": (656, 1405, 1483, 2793),
    "Santéria": (656, 1405, 1483, 2793),
    "T1L": (656, 1405, 1483, 2793),
    "T2L": (656, 1405, 1483, 2793),
    "Melbourne": (935, 1266, 1891, 2399),
    "Vienne": (928, 1264, 1859, 2397),
}

MODELES = [m for m, *_ in PDF_MODELES]

# Chapitres du PDF hors périmètre des chunks d'options.
BLOCS_HORS_OPTIONS = {"Exemple de calculs"}

# Vocabulaire proscrit (faux synonymes). Le chunk de gouvernance du vocabulaire
# les cite pour les proscrire : il est le seul admis à les porter.
BANNIS = [r"\bgonds?\b", r"\bcharni[èe]res?\b", r"\bsurvitrages?\b",
          r"\bcr[ée]mones?\b", r"\banti-d[ée]gondage\b",
          r"ouverture\s+[àa]\s+soufflet"]
PREFIXE_VOCABULAIRE = PREFIXE + " — Vocabulaire"

# Gammes voisines : aucune ne doit apparaître dans le corpus.
GAMMES_ETRANGERES = [r"\bT81\b", r"\bHA76\b", r"\bHAM76\b", r"\bCA76\b",
                     r"\bCA80\b", r"\bTA76\b", r"\bFT84\b", r"\bCAG76\b"]

# Formules d'unité admises : tout poste chiffré d'OPTIONS doit en porter une.
UNITES_ADMISES = ["forfaitaire", "ni forfaitaire ni proportionnelle", "au mètre carré", "par champ", "par face",
                  "au mètre linéaire", "par châssis", "à la pièce",
                  "par fixation", "par garniture", "s'ajoute au prix du châssis"]

RE_SOURCE = re.compile(
    r"^\*Source : " + re.escape(DOC_AFFICHE) +
    r", page (\d+) — information (originale|complémentaire) — SC(\d{4})\*$")
RE_MONTANT = re.compile(r"(\d+)\s*€\s*(HT|TTC)")
# Formulation A2 : « … est de X € HT ; le tarif TTC correspondant est de Y €. »
RE_TTC_A2 = re.compile(r"tarif TTC correspondant est de (\d+)\s*€")
RE_TRIPLET_FIXE = re.compile(r"(\d+) mm, (\d+) € HT, (\d+) € TTC")

ANOMALIES = []
CONTROLES = {"total": 0}


def controle(famille, ok, message):
    CONTROLES["total"] += 1
    CONTROLES[famille] = CONTROLES.get(famille, 0) + 1
    if not ok:
        ANOMALIES.append((famille, message))


# --------------------------------------------------------------------------
# Lecture des Markdown produits
# --------------------------------------------------------------------------

def lit_fichier(chemin):
    texte = open(chemin, encoding="utf-8").read()
    m = re.match(r"^---\n(.*?)\n---\n", texte, re.S)
    if not m:
        return None, []
    front = {}
    for ligne in m.group(1).splitlines():
        if ":" in ligne:
            cle, val = ligne.split(":", 1)
            front[cle.strip()] = val.strip()
    corps = texte[m.end():]
    chunks = []
    for bloc in re.split(r"\n(?=## )", corps):
        bloc = bloc.strip()
        if not bloc.startswith("## "):
            continue
        lignes = bloc.split("\n")
        titre = lignes[0][3:].strip()
        source = lignes[1].strip() if len(lignes) > 1 else ""
        texte_corps = "\n".join(lignes[2:]).strip()
        chunks.append({"titre": titre, "source": source, "corps": texte_corps,
                       "bloc": bloc,
                       "mots": len(re.findall(r"\S+", bloc))})
    return front, chunks


# --------------------------------------------------------------------------
# Familles de contrôle
# --------------------------------------------------------------------------

def f1_front_matter_et_decomptes(nom, front, chunks):
    obligatoires = ["document_source", "type_document", "sous_type", "gamme_code",
                    "gamme_nom", "collection", "materiau", "version_doc",
                    "date_validite", "nb_chunks", "audiences"]
    for cle in obligatoires:
        controle("front matter", cle in front, "%s : champ %s absent" % (nom, cle))
    controle("front matter", front.get("document_source") == DOC_YAML,
             "%s : document_source inattendu (%s)" % (nom, front.get("document_source")))
    controle("front matter", front.get("gamme_code") == "H81 Access",
             "%s : gamme_code inattendu" % nom)
    controle("front matter", front.get("sous_type") == FICHIERS[nom],
             "%s : sous_type inattendu (%s)" % (nom, front.get("sous_type")))
    controle("décomptes", front.get("nb_chunks") == str(len(chunks)),
             "%s : nb_chunks annoncé %s pour %d chunks réels" % (
                 nom, front.get("nb_chunks"), len(chunks)))


def f2_forme(nom, chunks):
    for c in chunks:
        controle("plafond", c["mots"] <= PLAFOND,
                 "%s : %d mots — %s" % (nom, c["mots"], c["titre"][:70]))
        controle("préfixe", c["titre"].startswith(PREFIXE + " — "),
                 "%s : titre sans préfixe auto-discriminant — %s" % (nom, c["titre"][:70]))
        controle("ligne de source", RE_SOURCE.match(c["source"]) is not None,
                 "%s : ligne de source non conforme — %s" % (nom, c["titre"][:60]))
        controle("prose", not re.search(r"^\s*[-•*]\s", c["corps"], re.M),
                 "%s : puce détectée — %s" % (nom, c["titre"][:60]))


def f3_continuite_sc(nom, chunks):
    scs = []
    for c in chunks:
        m = RE_SOURCE.match(c["source"])
        if m:
            scs.append(int(m.group(3)))
    attendu = list(range(2, 2 + len(chunks)))
    controle("continuité SC", scs == attendu,
             "%s : numérotation SC discontinue ou ne démarrant pas à SC0002" % nom)


def f4_unicite_titres(tous):
    vus = {}
    for nom, c in tous:
        if c["titre"] in vus:
            controle("unicité des titres", False,
                     "titre dupliqué (%s et %s) : %s" % (vus[c["titre"]], nom,
                                                         c["titre"][:80]))
        else:
            vus[c["titre"]] = nom
    controle("unicité des titres", True, "")


def f5_pages(nom, chunks):
    admises = PAGES_ADMISES[nom]
    for c in chunks:
        m = RE_SOURCE.match(c["source"])
        if not m:
            continue
        page = int(m.group(1))
        controle("table des pages", page in admises,
                 "%s : page %d hors des pages admises — %s" % (nom, page,
                                                               c["titre"][:60]))


def f6_fidelite_prix_portes(chunks):
    attendu = {}
    for nom, page, ht1, ht2, ttc1, ttc2 in PDF_MODELES:
        attendu[(nom, "1 vantail")] = (ht1, ttc1, page)
        attendu[(nom, "2 vantaux")] = (ht2, ttc2, page)
    vus = set()
    for c in chunks:
        m = re.match(re.escape(PREFIXE) + r" — Tarif (.+?) (1 vantail|2 vantaux) \(",
                     c["titre"])
        if not m:
            controle("fidélité portes", False,
                     "titre de prix non analysable : %s" % c["titre"][:70])
            continue
        cle = (m.group(1), m.group(2))
        if cle not in attendu:
            controle("fidélité portes", False, "couple inconnu du PDF : %s" % (cle,))
            continue
        ht, ttc, page = attendu[cle]
        montants = dict((r, int(v)) for v, r in RE_MONTANT.findall(c["corps"]))
        m_ttc = RE_TTC_A2.search(c["corps"])
        if m_ttc:
            montants["TTC"] = int(m_ttc.group(1))
        controle("fidélité portes", montants.get("HT") == ht,
                 "%s %s : HT %s au lieu de %d" % (cle[0], cle[1],
                                                  montants.get("HT"), ht))
        controle("fidélité portes", montants.get("TTC") == ttc,
                 "%s %s : TTC %s au lieu de %d" % (cle[0], cle[1],
                                                   montants.get("TTC"), ttc))
        controle("fidélité portes",
                 RE_SOURCE.match(c["source"]) and
                 int(RE_SOURCE.match(c["source"]).group(1)) == page,
                 "%s %s : page citée incorrecte" % cle)
        vus.add(cle)
    controle("couverture portes", vus == set(attendu),
             "portes non couvertes : %s" % sorted(set(attendu) - vus))


def f7_fidelite_fixes(chunks):
    grille_vue, meneau_vu = {}, {}
    for c in chunks:
        m_h = re.search(r"hauteur (\d+) mm", c["titre"])
        for larg, ht, ttc in RE_TRIPLET_FIXE.findall(c["corps"]):
            if m_h:
                cle = (int(m_h.group(1)), int(larg))
                controle("unicité des cotes", cle not in grille_vue,
                         "couple de fixe servi deux fois : %s" % (cle,))
                grille_vue[cle] = (int(ht), int(ttc))
            else:
                cle = int(larg)
                controle("unicité des cotes", cle not in meneau_vu,
                         "palier de meneau servi deux fois : %d" % cle)
                meneau_vu[cle] = (int(ht), int(ttc))

    for i, h in enumerate(HAUTEURS):
        for j, l in enumerate(range(300, 2700, 100)):
            attendu = (GRILLE_HT[i][j], GRILLE_TTC[i][j])
            controle("fidélité fixes", grille_vue.get((h, l)) == attendu,
                     "fixe H%d x L%d : %s au lieu de %s" % (
                         h, l, grille_vue.get((h, l)), attendu))
    controle("couverture fixes", len(grille_vue) == 576,
             "grille des fixes : %d couples servis au lieu de 576" % len(grille_vue))

    for i, lg in enumerate(MEN_LONG):
        attendu = (MEN_HT[i], MEN_TTC[i])
        controle("fidélité meneau", meneau_vu.get(lg) == attendu,
                 "meneau %d mm : %s au lieu de %s" % (lg, meneau_vu.get(lg), attendu))
    controle("couverture meneau", len(meneau_vu) == 25,
             "meneau battant : %d paliers servis au lieu de 25" % len(meneau_vu))


def f8_fidelite_options(chunks):
    """Bijection par multiensembles, insensible aux libellés : tout couple
    (HT, TTC) servi doit provenir du PDF, et tout couple du PDF hors périmètre
    déclaré doit être servi."""
    attendu = collections.Counter()
    for lib, r0, r1, page, ht, ttc in BLOCS:
        if lib in BLOCS_HORS_OPTIONS:
            continue
        if lib == "Pack Evo":
            # le forfait de 531 € est servi à la maille option × modèle
            for h, t in zip(ht, ttc):
                attendu[(h, t)] += 11 if h == 531 else 1
            continue
        for h, t in zip(ht, ttc):
            attendu[(h, t)] += 1
    attendu[(63, 55)] += 1          # panneau phonique, porté par le modèle
    attendu[(0, 0)] += 2            # deux postes à zéro nu repris du PDF

    vu = collections.Counter()
    for c in chunks:
        montants = RE_MONTANT.findall(c["corps"])
        ht = [int(v) for v, r in montants if r == "HT"]
        ttc = [int(v) for v, r in montants if r == "TTC"]
        m_ttc = RE_TTC_A2.search(c["corps"])
        if m_ttc:
            ttc = [int(m_ttc.group(1))]
        controle("forme des options", len(ht) == 1,
                 "montant HT absent ou multiple — %s" % c["titre"][:70])
        if not ht:
            continue
        vu[(ht[0], ttc[0] if ttc else 0)] += 1

    for cle in sorted(set(attendu) | set(vu)):
        controle("fidélité options", attendu[cle] == vu[cle],
                 "couple %s : %d chunk(s) servi(s) pour %d attendu(s) au PDF" % (
                     cle, vu[cle], attendu[cle]))


def f9_unites(chunks):
    for c in chunks:
        controle("déclaration d'unité",
                 any(u in c["corps"] for u in UNITES_ADMISES),
                 "poste chiffré sans unité de facturation — %s" % c["titre"][:70])


def f10_absence_de_montant(nom, chunks):
    for c in chunks:
        controle("absence de montant", "€" not in c["bloc"],
                 "%s : montant en euros dans un fichier qui n'en porte pas — %s" % (
                     nom, c["titre"][:60]))
        controle("absence de montant", "%" not in c["bloc"],
                 "%s : pourcentage hors du fichier des plus-values "
                 "proportionnelles — %s" % (nom, c["titre"][:60]))


def f11_taux_proportionnels(chunks):
    corpus = " ".join(c["corps"] for c in chunks)
    controle("taux proportionnels", "15 %" in corpus,
             "le taux de plaxage de 15 % est absent")
    controle("taux proportionnels", "100 %" in corpus,
             "le taux de vitrage sur cintre de 100 % est absent")
    controle("taux proportionnels", len(chunks) == 2,
             "%d chunks proportionnels au lieu de 2" % len(chunks))


def f12_caracteristiques(chunks):
    vus = set()
    for c in chunks:
        m = re.match(re.escape(PREFIXE) + r" — Caractéristiques (.+?) \(ligne ",
                     c["titre"])
        if not m:
            controle("caractéristiques", False,
                     "titre non analysable : %s" % c["titre"][:70])
            continue
        modele = m.group(1)
        vus.add(modele)
        env = ENVELOPPES_PDF.get(modele)
        controle("caractéristiques", env is not None,
                 "modèle inconnu du PDF : %s" % modele)
        if env:
            Lmin, Lmax, Hmin, Hmax = env
            attendu = ("largeur comprise entre %d et %d mm et de hauteur comprise "
                       "entre %d et %d mm" % (Lmin, Lmax, Hmin, Hmax))
            controle("caractéristiques", attendu in c["corps"],
                     "%s : enveloppe dimensionnelle non conforme au PDF" % modele)
        controle("caractéristiques", "1,3 W/m².K" in c["corps"],
                 "%s : coefficient Ud absent ou altéré" % modele)
    controle("caractéristiques", vus == set(ENVELOPPES_PDF),
             "modèles sans chunk de caractéristiques : %s" % sorted(
                 set(ENVELOPPES_PDF) - vus))


def f13_anti_fantome(par_fichier):
    """Les 24 lignes de grille du chapitre Fixes portent un 0 structurel en HT et
    TTC. Aucun chunk d'options ne doit en être issu."""
    options = par_fichier["Tarif_H81_Access_OPTIONS.md"]
    for c in options:
        controle("anti-fantôme",
                 "fixe latéral" not in c["titre"].lower(),
                 "chunk d'option issu de la grille des fixes — %s" % c["titre"][:70])
    # Les deux montants de structure sans libellé (T2L, Melbourne) ne produisent rien.
    for modele in ("T2L", "Melbourne"):
        fautifs = [c for c in options
                   if re.search(r"Option .* sur %s \(" % modele, c["titre"])
                   and "Pack Evo" not in c["titre"]]
        controle("anti-fantôme", not fautifs,
                 "%s : option de modèle générée alors que la page-modèle n'en "
                 "porte pas" % modele)
    # Aucune option de modèle assortie d'un renvoi transverse.
    for c in options:
        controle("anti-fantôme", "vitrage ornemental" not in c["titre"].lower(),
                 "option à renvoi transverse générée — %s" % c["titre"][:70])


def f14_discrimination_et_vocabulaire(tous):
    for nom, c in tous:
        controle("discrimination", "H81 Access" in c["titre"],
                 "%s : titre sans marqueur de gamme — %s" % (nom, c["titre"][:60]))
        controle("discrimination",
                 "porte de service" in c["corps"].lower(),
                 "%s : corps sans mention « porte de service » — %s" % (
                     nom, c["titre"][:60]))
        nu = re.findall(r"H81(?! Access)(?!_)(?!—)", c["bloc"].replace(
            "Tarif—H81—Access", "").replace("Tarif_H81_Access", ""))
        controle("discrimination", not nu,
                 "%s : occurrence de « H81 » non suivie de « Access » — %s" % (
                     nom, c["titre"][:60]))
        for motif in GAMMES_ETRANGERES:
            controle("contamination inter-gammes",
                     not re.search(motif, c["bloc"]),
                     "%s : gamme étrangère (%s) — %s" % (nom, motif, c["titre"][:60]))
        if c["titre"].startswith(PREFIXE_VOCABULAIRE):
            continue
        for motif in BANNIS:
            controle("vocabulaire", not re.search(motif, c["bloc"], re.I),
                     "%s : terme proscrit (%s) — %s" % (nom, motif, c["titre"][:60]))


def f15_liant_inter_fichiers(par_fichier):
    prix = " ".join(c["titre"] for c in par_fichier["Tarif_H81_Access_PRIX_PORTES.md"])
    carac = " ".join(c["titre"] for c in
                     par_fichier["Tarif_H81_Access_CARACTERISTIQUES.md"])
    opts = " ".join(c["titre"] for c in par_fichier["Tarif_H81_Access_OPTIONS.md"])
    for modele in MODELES:
        controle("liant inter-fichiers", modele in prix,
                 "%s absent des titres de prix" % modele)
        controle("liant inter-fichiers", modele in carac,
                 "%s absent des titres de caractéristiques" % modele)
        controle("liant inter-fichiers", modele in opts,
                 "%s absent des titres d'options" % modele)


# --------------------------------------------------------------------------
# Exécution
# --------------------------------------------------------------------------

def main():
    racine = os.environ.get("H81A_OUT", "/mnt/user-data/outputs")
    par_fichier, tous = {}, []
    for nom in FICHIERS:
        chemin = os.path.join(racine, nom)
        if not os.path.exists(chemin):
            ANOMALIES.append(("présence", "fichier absent : %s" % nom))
            par_fichier[nom] = []
            continue
        front, chunks = lit_fichier(chemin)
        par_fichier[nom] = chunks
        tous += [(nom, c) for c in chunks]
        f1_front_matter_et_decomptes(nom, front, chunks)
        f2_forme(nom, chunks)
        f3_continuite_sc(nom, chunks)
        f5_pages(nom, chunks)
        if nom in SANS_MONTANT:
            f10_absence_de_montant(nom, chunks)

    f4_unicite_titres(tous)
    f6_fidelite_prix_portes(par_fichier["Tarif_H81_Access_PRIX_PORTES.md"])
    f7_fidelite_fixes(par_fichier["Tarif_H81_Access_PRIX_FIXES.md"])
    f8_fidelite_options(par_fichier["Tarif_H81_Access_OPTIONS.md"])
    f9_unites(par_fichier["Tarif_H81_Access_OPTIONS.md"])
    f11_taux_proportionnels(
        par_fichier["Tarif_H81_Access_PLUS_VALUES_PROPORTIONNELLES.md"])
    f12_caracteristiques(par_fichier["Tarif_H81_Access_CARACTERISTIQUES.md"])
    f13_anti_fantome(par_fichier)
    f14_discrimination_et_vocabulaire(tous)
    f15_liant_inter_fichiers(par_fichier)

    print("=== Contrôle de conformité — Tarif H81 Access ===\n")
    print("Chunks relus : %d" % len(tous))
    for nom in FICHIERS:
        print("  %-50s %4d" % (nom, len(par_fichier[nom])))
    print("\nContrôles exécutés : %d" % CONTROLES["total"])
    for famille in sorted(k for k in CONTROLES if k != "total"):
        print("  %-30s %6d" % (famille, CONTROLES[famille]))

    reels = [a for a in ANOMALIES if a[1]]
    print("\nAnomalies détectées : %d" % len(reels))
    for famille, message in reels:
        print("  ✗ [%s] %s" % (famille, message))
    if not reels:
        print("  ✓ Aucune anomalie.")

    print("\nLimite déclarée du contrôle : le PDF TTC n'ayant pu être rejoint, la "
          "fidélité TTC\nest contrôlée contre la table de saisie littérale de "
          "verif_A2_excel_vs_pdf_H81_Access.py,\net non contre une ré-extraction "
          "indépendante du document TTC.")
    return 1 if reels else 0


if __name__ == "__main__":
    sys.exit(main())
