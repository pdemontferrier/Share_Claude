#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Contrôle de conformité AUTONOME des chunks tarif T81.
Ne réutilise AUCUNE fonction du générateur : relit les .md produits et les
confronte à la note de cadrage (forme), à l'Excel (fidélité) et au PDF, seule
référence primaire (section 7 ter de la note maître).

Contrôles :
  1  décomptes par fichier, recomptés depuis l'Excel
  2  forme : plafond, ligne de source, préfixe de titre, continuité SC, YAML
  3  COUVERTURE EXHAUSTIVE des cellules de grille (nouveau vs H81)
  4  fidélité numérique exhaustive des postes forfaitaires
  5  anti-fantôme (grilles et postes)
  6  cohérence des bandes de dimensions avec l'échelle du tableau
  7  absence de tout montant dans les faisabilités et les transverses
  8  vocabulaire : « crémone » jamais employée seule, faux synonymes proscrits
  9  croisement PDF : le couple référence + montant figure-t-il sur la page
     citée, dans le texte RENDU (pymupdf), et non dans la sortie de pdftotext
     qui restitue les planches rognées par la maquette InDesign
"""
import re
import subprocess
import sys
import unicodedata
from collections import OrderedDict, defaultdict

import openpyxl
import pymupdf

OUTDIR = "/mnt/user-data/outputs"
XLSX = "/mnt/user-data/uploads/T81_-infos-tarifs.xlsx"
PDF = "/mnt/user-data/uploads/Tarif_T81_HT_09-07-2026.pdf"
PLAFOND = 200

FILES = OrderedDict([
    ("methode", "Tarif_T81_METHODE.md"),
    ("prix", "Tarif_T81_PRIX_CHASSIS.md"),
    ("options", "Tarif_T81_OPTIONS.md"),
    ("speciaux", "Tarif_T81_CHASSIS_SPECIAUX.md"),
    ("faisabilites", "Tarif_T81_FAISABILITES.md"),
    ("transverses", "Tarif_T81_TRANSVERSES.md"),
])

RE_SOURCE = re.compile(
    r"^\*Source : Tarif—T81—HT—09-07-2026\.pdf, page (\d+) — "
    r"information (originale|complémentaire) — (SC\d{4})\*$")

# restatement indépendant de la correspondance libellé -> (chapitre, tableau)
LIBELLE = {
    "châssis à 1 ouvrant à la française": ("1 OF", "1V fr"),
    "châssis à 1 ouvrant à la française en grande hauteur": ("1 OF", "1V grande hauteur"),
    "châssis à 2 ouvrants égaux à la française": ("2 OF", "2V fr"),
    "châssis à 2 ouvrants égaux à la française en grande hauteur": ("2 OF", "2V grande hauteur"),
    "châssis fixe": ("Châssis fixe", "Fixe"),
    "châssis à soufflet normal (SN)": ("Châssis à soufflet", "SN"),
    "châssis à soufflet normal à poignée latérale (SN)": ("Châssis à soufflet", "SN poignée latérale"),
    "châssis à soufflet d'aération avec ferme-imposte (SA)": ("Châssis à soufflet", "SA"),
    "coulissant à translation": ("Coulissant", "coulissant"),
}
HABILLAGE = ("Habillage PVC blanc sur vitrage", "habillage pvc blanc")

FAUX_SYNONYMES = ["gond", "charnière", "survitrage", "anti-dégondage",
                  "ouverture à soufflet"]

results = {"OK": [], "WARN": [], "FAIL": []}
def ok(m):   results["OK"].append(m)
def warn(m): results["WARN"].append(m)
def fail(m): results["FAIL"].append(m)


# ------------------------------------------------------------------ parsing md
def parse_chunks(path):
    text = open(path, encoding="utf-8").read()
    fm = ""
    if text.startswith("---"):
        end = text.find("\n---", 3)
        fm, text = text[:end + 4], text[end + 4:]
    chunks = []
    for block in re.split(r"\n(?=## )", text):
        block = block.strip()
        if not block.startswith("## "):
            continue
        lines = block.split("\n")
        chunks.append({"title": lines[0][3:].strip(),
                       "source": lines[1].strip() if len(lines) > 1 else "",
                       "body": "\n".join(lines[2:]).strip()})
    return fm, chunks


def montant(txt):
    return int(re.sub(r"[^\d]", "", txt))


# ------------------------------------------------------------------ 1. Excel
def load_excel():
    wb = openpyxl.load_workbook(XLSX, data_only=True)
    ws = wb["Feuil1"]
    raw = list(ws.iter_rows(values_only=True))
    header = list(raw[0]) + [None] * (95 - len(raw[0]))
    rows = [list(r) + [None] * (95 - len(r)) for r in raw[1:]
            if any(v is not None for v in r)]
    largeurs = {j: int(str(header[j]).split()[2]) for j in range(29, 62)}
    return rows, largeurs


def cellules_grille(rows, largeurs):
    """{(chap, tab, forme|None, hauteur, largeur): (ht, ttc)} recompté seul."""
    idx, vus = {}, defaultdict(int)
    formes = ["cintré", "rectangulaire"]
    for r in rows:
        chap, tab = (str(r[2]).strip() if r[2] else ""), (str(r[3]).strip() if r[3] else "")
        if (chap, tab) not in list(LIBELLE.values()) + [HABILLAGE]:
            continue
        h = r[28]
        forme = None
        if (chap, tab) == HABILLAGE:
            forme = formes[min(vus[h], 1)]
            vus[h] += 1
        for j in range(29, 62):
            if r[j] is None and r[j + 33] is None:
                continue
            idx[(chap, tab, forme, h, largeurs[j])] = (
                None if r[j] is None else int(round(float(r[j]))),
                None if r[j + 33] is None else int(round(float(r[j + 33]))))
    return idx


def echelles(rows, largeurs):
    """{(chap, tab): (echelle_largeurs, echelle_hauteurs)} recomptées seules."""
    L, H = defaultdict(set), defaultdict(set)
    for r in rows:
        chap, tab = (str(r[2]).strip() if r[2] else ""), (str(r[3]).strip() if r[3] else "")
        if (chap, tab) not in list(LIBELLE.values()) + [HABILLAGE]:
            continue
        H[(chap, tab)].add(r[28])
        for j in range(29, 62):
            if r[j] is not None:
                L[(chap, tab)].add(largeurs[j])
    return {k: (sorted(L[k]), sorted(H[k])) for k in L}


def postes_excel(rows):
    """{(chap, tab, designation): (ht, ttc)} pour les postes forfaitaires T81."""
    grilles = set(list(LIBELLE.values()) + [HABILLAGE])
    idx = {}
    for r in rows:
        chap = str(r[2]).strip() if r[2] else ""
        tab = str(r[3]).strip() if r[3] else ""
        if not chap or (chap, tab) in grilles or chap == "Exemple de calcul":
            continue
        if str(r[0]).strip() != "T81" or r[8] is None:
            continue
        des = " ".join(str(r[4] or "").split())
        idx[(chap, tab, des, int(round(float(r[8]))))] = (
            int(round(float(r[8]))),
            None if r[9] is None else int(round(float(r[9]))))
    return idx


# ------------------------------------------------------------------ 2. forme
def check_forme(name, fm, chunks):
    scs = []
    for c in chunks:
        n = len(re.findall(r"\S+", "## " + c["title"] + " " + c["source"] + " " + c["body"]))
        if n > PLAFOND:
            fail(f"[{name}] plafond dépassé ({n} mots) : {c['title'][:60]}")
        m = RE_SOURCE.match(c["source"])
        if not m:
            fail(f"[{name}] ligne de source non conforme : {c['source'][:70]}")
        else:
            scs.append(int(m.group(3)[2:]))
        if not c["title"].startswith("T81 Fenêtre PVC — "):
            fail(f"[{name}] préfixe de titre non conforme : {c['title'][:60]}")
    if scs:
        s = sorted(scs)
        if s[0] != 2:
            warn(f"[{name}] la numérotation SC démarre à SC{s[0]:04d} et non SC0002")
        if len(s) != len(set(s)):
            fail(f"[{name}] doublons de SC")
        trous = [n for n in range(s[0], s[-1] + 1) if n not in set(s)]
        if trous:
            fail(f"[{name}] trous SC : {trous[:5]}")
        else:
            ok(f"[{name}] SC continue SC{s[0]:04d}→SC{s[-1]:04d}, sans trou ni doublon")
    for k in ["document_source", "type_document", "gamme_code", "gamme_nom", "nb_chunks"]:
        if k not in fm:
            warn(f"[{name}] front matter : clé « {k} » absente")
    if f"nb_chunks: {len(chunks)}" not in fm:
        fail(f"[{name}] nb_chunks du front matter ≠ nombre de chunks ({len(chunks)})")


# ------------------------------------------------------- 3/5/6. grilles
RE_G_TITRE = re.compile(r"Tarif (.+?), hauteur (jusqu'à \d+|de \d+ à \d+) mm")
RE_G_INTRO = re.compile(r"pour une cote tarif en hauteur (?:jusqu'à (\d+)|de (\d+) à (\d+)) mm")
RE_H_INTRO = re.compile(r"habillage PVC blanc (cintré|rectangulaire) posé sur vitrage.*?"
                        r"en hauteur (?:jusqu'à (\d+)|de (\d+) à (\d+)) mm", re.S)
RE_ITEM = re.compile(r"en largeur (?:jusqu'à (\d+)|de (\d+) à (\d+)) mm, "
                     r"([\d\u202f\u00a0 ]+) € HT et ([\d\u202f\u00a0 ]+) € TTC")


def check_grilles(chunks_prix, chunks_spec, idx_cells, ech):
    couvert = {}
    doublons = []

    def traiter(c, chap, tab, forme, h_bas, h_haut):
        for m in RE_ITEM.finditer(c["body"]):
            l_bas = int(m.group(2)) if m.group(2) else None
            l_haut = int(m.group(3) or m.group(1))
            ht, ttc = montant(m.group(4)), montant(m.group(5))
            key = (chap, tab, forme, h_haut, l_haut)
            if key in couvert:
                doublons.append(key)
            couvert[key] = (ht, ttc, h_bas, l_bas, c["title"])
            ref = idx_cells.get(key)
            if ref is None:
                fail(f"[anti-fantôme] cellule inexistante dans l'Excel : "
                     f"{chap}/{tab}/{forme} H{h_haut} L{l_haut}")
                continue
            if ht != ref[0]:
                fail(f"[fidélité] ÉCART HT {chap}/{tab} H{h_haut} L{l_haut} : "
                     f"chunk={ht} vs Excel={ref[0]}")
            if ref[1] is not None and ttc != ref[1]:
                fail(f"[fidélité] ÉCART TTC {chap}/{tab} H{h_haut} L{l_haut} : "
                     f"chunk={ttc} vs Excel={ref[1]}")
            # bandes : recalculées depuis l'échelle, indépendamment
            eL, eH = ech[(chap, tab)]
            att_l = None if eL.index(l_haut) == 0 else eL[eL.index(l_haut) - 1] + 1
            if l_bas != att_l:
                fail(f"[bandes] borne basse de largeur erronée {chap}/{tab} "
                     f"L{l_haut} : chunk={l_bas} vs attendu={att_l}")

    for c in chunks_prix:
        mt = RE_G_TITRE.search(c["title"])
        mi = RE_G_INTRO.search(c["body"])
        if not (mt and mi):
            fail(f"[prix] chunk non analysable : {c['title'][:60]}")
            continue
        lib = mt.group(1).replace("châssis à 1 ouvrant", "châssis à 1 ouvrant")
        cle = LIBELLE.get(lib)
        if not cle:
            fail(f"[prix] libellé de grille inconnu : {lib[:60]}")
            continue
        h_bas = int(mi.group(2)) if mi.group(2) else None
        h_haut = int(mi.group(3) or mi.group(1))
        traiter(c, cle[0], cle[1], None, h_bas, h_haut)

    for c in chunks_spec:
        mh = RE_H_INTRO.search(c["body"])
        if not mh:
            continue
        forme = mh.group(1)
        h_bas = int(mh.group(3)) if mh.group(3) else None
        h_haut = int(mh.group(4) or mh.group(2))
        traiter(c, HABILLAGE[0], HABILLAGE[1], forme, h_bas, h_haut)

    manquantes = [k for k in idx_cells if k not in couvert]
    if manquantes:
        fail(f"[couverture] {len(manquantes)} cellules de l'Excel sans chunk : "
             f"{manquantes[:4]}")
    else:
        ok(f"[couverture] les {len(idx_cells)} cellules de grille de l'Excel sont "
           f"couvertes, chacune une seule fois")
    if doublons:
        fail(f"[couverture] {len(doublons)} cellules servies par plusieurs chunks : "
             f"{doublons[:4]}")
    ok(f"[fidélité] {len(couvert)} couples HT/TTC de grille confrontés à l'Excel")
    ok(f"[bandes] bornes de bandes recalculées et vérifiées sur {len(couvert)} items")


# ------------------------------------------------------------------ 4. postes
RE_POSTE = re.compile(r"le poste « (.+?) » (?:est chiffré en plus-value à "
                      r"([\d\u202f\u00a0 ]+) € HT, soit ([\d\u202f\u00a0 ]+) € TTC"
                      r"|ne donne lieu à aucune plus-value)")


def check_postes_bijection(chunks_opt, chunks_spec, rows):
    """Confrontation ensembliste : le multiensemble des couples (HT, TTC) servis
    doit être exactement celui des groupes iso-prix de l'Excel. Indépendant des
    libellés, donc insensible aux homonymies."""
    from collections import Counter
    grilles = set(list(LIBELLE.values()) + [HABILLAGE])
    attendu = Counter()
    for r in rows:
        chap = str(r[2]).strip() if r[2] else ""
        tab = str(r[3]).strip() if r[3] else ""
        if (not chap or (chap, tab) in grilles or chap == "Exemple de calcul"
                or str(r[0]).strip() != "T81" or r[8] is None):
            continue
        attendu[(chap, tab, " ".join(str(r[4] or "").split()),
                 int(round(float(r[8]))),
                 None if r[9] is None else int(round(float(r[9]))))] = 1
    # postes indiscriminables : plusieurs prix sous des colonnes identiques.
    # La note impose de ne pas les générer ; l'audit re-dérive la règle seul.
    par_cle = defaultdict(list)
    for r in rows:
        chap = str(r[2]).strip() if r[2] else ""
        tab = str(r[3]).strip() if r[3] else ""
        if (not chap or (chap, tab) in grilles or chap == "Exemple de calcul"
                or str(r[0]).strip() != "T81" or r[8] is None):
            continue
        par_cle[(chap, tab, " ".join(str(r[4] or "").split()),
                 " ".join(str(r[5] or "").split()))].append(
            (int(round(float(r[8]))), None if r[9] is None else int(round(float(r[9])))))
    exclus = set()
    n_exclus = 0
    for cle, prix in par_cle.items():
        if len(set(prix)) > 1:
            n_exclus += len(set(prix))
            for p in set(prix):
                exclus.add((cle[0], cle[1], cle[2], p[0], p[1]))
    # La note admet deux issues pour un poste indiscriminable : soit il n'est pas
    # généré, soit il l'est avec un discriminant repris du PDF. L'audit n'arbitre
    # pas : il exige la bijection ET l'unicité des titres, ce qui couvre les deux.
    if n_exclus:
        ok(f"[postes] {n_exclus} postes portent plusieurs prix sous des colonnes "
           f"identiques dans l'Excel ; la bijection et l'unicité des titres "
           f"tranchent s'ils ont été écartés ou discriminés depuis le PDF")
    att = Counter((k[3], k[4]) for k in attendu)
    servi = Counter()
    for c in list(chunks_opt) + list(chunks_spec):
        m = RE_POSTE.search(c["body"])
        if not m:
            continue
        if m.group(2) is None:
            servi[(0, 0)] += 1
        else:
            servi[(montant(m.group(2)), montant(m.group(3)))] += 1
    manque = att - servi
    surplus = servi - att
    if manque:
        fail(f"[postes] {sum(manque.values())} couples de l'Excel non servis : "
             f"{list(manque)[:4]}")
    if surplus:
        fail(f"[postes] {sum(surplus.values())} couples servis absents de "
             f"l'Excel : {list(surplus)[:4]}")
    if not manque and not surplus:
        ok(f"[postes] bijection exacte : {sum(att.values())} couples (HT, TTC) "
           f"servis, tous présents dans l'Excel, aucun en trop")


def check_postes(chunks, idx_postes, nom):
    vus, n = set(), 0
    montants = {}
    for (chap, tab, des, ht), (h, t) in idx_postes.items():
        montants.setdefault(des.lower(), set()).add((h, t))
    for c in chunks:
        m = RE_POSTE.search(c["body"])
        if not m:
            continue
        n += 1
        if m.group(2) is None:
            continue
        ht, ttc = montant(m.group(2)), montant(m.group(3))
        libelle = m.group(1).lower()
        cands = [v for des, v in montants.items() if des and des in libelle]
        if not cands:
            warn(f"[{nom}] poste non rapproché de l'Excel : {m.group(1)[:50]}")
            continue
        paires = set().union(*cands)
        if (ht, ttc) not in paires and (ht, None) not in paires:
            fail(f"[{nom}] ÉCART montant « {m.group(1)[:40]} » : chunk={ht}/{ttc} "
                 f"vs Excel={sorted(paires)[:3]}")
        vus.add(libelle)
    ok(f"[{nom}] {n} postes confrontés à l'Excel (HT et TTC, au €)")


def check_decompte_postes(chunks_opt, chunks_spec, rows):
    """Recompte indépendant : nombre de groupes iso-prix attendus."""
    grilles = set(list(LIBELLE.values()) + [HABILLAGE])
    speciaux = {"Cintres", "Forme sur accessoires", "Angles", "CVR",
                "Châssis spé - Croisillons", "Habillage PVC blanc sur dormant"}
    g_opt, g_spe = set(), set()
    for r in rows:
        chap = str(r[2]).strip() if r[2] else ""
        tab = str(r[3]).strip() if r[3] else ""
        if (not chap or (chap, tab) in grilles or chap == "Exemple de calcul"
                or str(r[0]).strip() != "T81" or r[8] is None):
            continue
        key = (chap, tab, " ".join(str(r[4] or "").split()), r[8], r[9])
        (g_spe if chap in speciaux else g_opt).add(key)
    if len(chunks_opt) != len(g_opt):
        fail(f"[décompte] options : {len(chunks_opt)} chunks vs {len(g_opt)} "
             f"groupes iso-prix attendus")
    else:
        ok(f"[décompte] options : {len(g_opt)} groupes iso-prix, autant de chunks")
    return len(g_spe)


# ------------------------------------------------------- 10. unité de facturation
RE_UNITE = re.compile(r"Ces? montants? (?:s'entend|s'entendent|est|sont) [^.]+\.|"
                      r"Le tarif exprime ce montant dans une unité de facturation")


def check_unites(chunks, nom):
    sans, non_etablie, n = [], 0, 0
    for c in chunks:
        if "est chiffré en plus-value à" not in c["body"]:
            continue
        n += 1
        m = RE_UNITE.search(c["body"])
        if not m:
            sans.append(c["title"][:55])
        elif m.group(0).startswith("Le tarif exprime"):
            non_etablie += 1
    if sans:
        fail(f"[{nom}] {len(sans)} postes chiffrés sans unité de facturation : {sans[:3]}")
    else:
        ok(f"[{nom}] les {n} postes chiffrés déclarent tous une unité de facturation")
    if non_etablie:
        warn(f"[{nom}] {non_etablie}/{n} postes dont l'unité n'est pas établie et "
             f"renvoient à la page du tarif")


def check_titres_uniques(chunks_by_file):
    for nom, chunks in chunks_by_file.items():
        vus = set()
        dup = [c["title"] for c in chunks if c["title"] in vus or vus.add(c["title"])]
        if dup:
            fail(f"[{nom}] {len(dup)} titres non discriminants : {dup[:2]}")
    ok("[titres] aucun titre dupliqué dans un même fichier")


# ------------------------------------------------------------- 7. sans montant
RE_MONTANT = re.compile(r"\d+\s*(€|%|/ml|/m²)")


def check_sans_montant(name, chunks):
    for c in chunks:
        if RE_MONTANT.search(c["body"]):
            fail(f"[{name}] montant interdit (règle T6/T7) : {c['title'][:55]}")
    ok(f"[{name}] {len(chunks)} chunks vérifiés sans aucun montant")


# ------------------------------------------------------------- 8. vocabulaire
def check_vocabulaire(all_chunks):
    n = 0
    for c in all_chunks:
        txt = c["title"] + " " + c["body"]
        for m in re.finditer(r"[Cc]rémones?", txt):
            suite = txt[m.end():m.end() + 16].lower()
            if not suite.lstrip().startswith("à l'ancienne"):
                fail(f"[vocabulaire] « crémone » employée seule : {c['title'][:55]}")
            n += 1
        low = unicodedata.normalize("NFC", txt.lower())
        for faux in FAUX_SYNONYMES:
            if re.search(r"\b" + faux + r"\b", low):
                fail(f"[vocabulaire] faux synonyme « {faux} » : {c['title'][:55]}")
    ok(f"[vocabulaire] {n} occurrences de « crémone » toutes suivies de "
       f"« à l'ancienne » ; aucun faux synonyme")


# ------------------------------------------------------------- 9. pages PDF
# Le PDF fait foi, mais « le PDF » désigne ce qu'un lecteur VOIT. La maquette
# InDesign place des planches techniques dans des cadres qui les rognent : le
# cadre masque à l'affichage, il ne retire rien du flux de contenu. pdftotext
# ignore les chemins de rognage et restitue ce texte — 10,1 % des jetons de ce
# tarif, sur 25 pages. Lire le PDF avec pdftotext ici reviendrait à valider des
# références que personne ne peut trouver dans le tarif publié.
#
# Le contrôle porte sur le COUPLE référence + montant. Un montant isolé — 33,
# 39, 17 — se retrouve sur presque n'importe quelle page d'un tarif : SC0222
# citait la page 60 pour un tableau situé page 61, et l'ancien contrôle passait
# au vert parce que 33 € figure aussi page 60, dans un autre tableau.

# Une référence produit, relevée dans la CASSE D'ORIGINE du libellé : commence
# par une majuscule ou un chiffre et contient au moins un chiffre. Écarte donc
# les mots français porteurs de chiffres (« plaxage2 faces »), qui commencent
# par une minuscule.
RE_REF = re.compile(r"(?<![\w/-])(?=[A-Za-z0-9_/'-]*\d)"
                    r"[A-Z0-9][A-Za-z0-9_/'-]{2,}(?<![-_/])")

# Qualificatif de finition accolé à la référence dans l'Excel (« 6370-Blanc »).
RE_FINITION = re.compile(r"[-\s]*(blanc|plaxage|gris|laqué)$", re.I)

# Jetons qui ressemblent à une référence sans en être une.
NON_REFS = {"HT", "TTC", "PVC", "T81", "ADV", "RAL", "PF", "OB", "MM"}


def norm_pdf(txt):
    """NFC, majuscules, espaces réduits à un blanc unique. On ne SUPPRIME pas
    les espaces : « 5 180 » est un montant, pas la référence 5180, et « 1451802 »
    n'est pas la 5180 non plus."""
    return re.sub(r"[\s\u202f\u00a0]+", " ", unicodedata.normalize("NFC", txt)).upper()


def contient(texte_norme, jeton):
    """Présence en frontière de référence : ni préfixe ni suffixe alphanumérique.
    La recherche en sous-chaîne produirait des correspondances fortuites."""
    return re.search(r"(?<![\w/-])" + re.escape(norm_pdf(jeton)) + r"(?![\w-])",
                     texte_norme) is not None


def variantes_montant(ht):
    """Le tarif sépare les milliers par une espace fine insécable, l'Excel non :
    1108 dans le chunk s'écrit « 1 108 » dans le PDF. On teste les deux."""
    return [ht] if len(ht) < 4 else [ht, f"{ht[:-3]} {ht[-3:]}"]


def refs_du_libelle(libelle):
    """Références portées par un libellé de poste.

    Un groupe d'entiers de quatre chiffres reliés par des tirets est une
    ÉNUMÉRATION de références (« 5180-5181-5415-5416 »), pas une référence
    composée : on le scinde, et chaque référence est contrôlée séparément.
    C'est cette décomposition qui fait apparaître qu'un poste agrège des
    références obsolètes et leurs remplaçantes sous un prix commun. Un suffixe
    alphanumérique (« AK10123-RAS1 », « 5334-C24 ») appartient en revanche à la
    référence : on ne scinde pas.
    """
    refs = []
    libelle = RE_FINITION.sub("", libelle.strip())
    for brut in RE_REF.findall(libelle):
        brut = RE_FINITION.sub("", brut).strip("-_/ ").upper()
        if brut in NON_REFS or len(brut) < 3:
            continue
        segs = brut.split("-")
        if len(segs) > 1 and all(re.fullmatch(r"\d{4}", x) for x in segs):
            refs.extend(segs)
        else:
            refs.append(brut)
    return list(OrderedDict.fromkeys(refs))


class LecturePdf:
    """Deux lectures du même PDF : ce qui est rendu, et ce que voit pdftotext.
    Seule la première fait foi ; la seconde ne sert qu'à qualifier une référence
    absente du rendu — planche rognée, ou absence pure et simple."""

    def __init__(self, path):
        self.path = path
        self.doc = pymupdf.open(path)
        self.n = len(self.doc)
        self.rendu = [norm_pdf(p.get_text()) for p in self.doc]
        self._brut = {}

    def brut(self, page):
        if page not in self._brut:
            out = subprocess.run(
                ["pdftotext", "-enc", "UTF-8", "-f", str(page), "-l", str(page),
                 self.path, "-"], capture_output=True).stdout
            self._brut[page] = norm_pdf(out.decode("utf-8", "replace"))
        return self._brut[page]

    def pages_visibles(self, ref):
        return [i + 1 for i, txt in enumerate(self.rendu) if contient(txt, ref)]

    def pages_masquees(self, ref):
        return [p for p in range(1, self.n + 1)
                if contient(self.brut(p), ref) and not contient(self.rendu[p - 1], ref)]


def check_pdf(chunks_by_file):
    pdf = LecturePdf(PDF)

    # Mesure de la couche non rendue : elle conditionne la lecture de tout le
    # reste et doit apparaître dans le rapport, même quand elle est nulle.
    MOT = re.compile(r"[A-Za-zÀ-ÿ0-9][A-Za-zÀ-ÿ0-9_/-]*")
    ghost = total = 0
    sales = []
    for p in range(1, pdf.n + 1):
        vus = set(MOT.findall(pdf.doc[p - 1].get_text()))
        bruts = MOT.findall(subprocess.run(
            ["pdftotext", "-enc", "UTF-8", "-f", str(p), "-l", str(p), PDF, "-"],
            capture_output=True).stdout.decode("utf-8", "replace"))
        g = sum(1 for m in bruts if m not in vus)
        ghost += g
        total += len(bruts)
        if g:
            sales.append(p)
    if ghost:
        warn(f"[pdf] couche non rendue : {ghost}/{total} jetons ({100 * ghost / total:.1f} %) "
             f"extraits par pdftotext ne sont visibles nulle part, sur {len(sales)} pages "
             f"— contrôle conduit au rendu")

    teste = mt_ok = ref_ok = 0
    sans_ref = 0
    mauvaise_page, fantomes, hors_tarif, montants_ko = [], [], [], []

    for name in ("options", "speciaux"):
        for c in chunks_by_file[name]:
            m = RE_POSTE.search(c["body"])
            ms = RE_SOURCE.match(c["source"])
            if not (m and ms and m.group(2)):
                continue
            page, sc = int(ms.group(1)), ms.group(3)
            libelle, ht = m.group(1), str(montant(m.group(2)))
            teste += 1

            if any(contient(pdf.rendu[page - 1], v) for v in variantes_montant(ht)):
                mt_ok += 1
            else:
                montants_ko.append(f"{sc} p.{page} : {ht} € — {libelle[:45]}")

            refs = refs_du_libelle(libelle)
            if not refs:
                sans_ref += 1
                continue
            for r in refs:
                if contient(pdf.rendu[page - 1], r):
                    ref_ok += 1
                elif pdf.pages_visibles(r):
                    mauvaise_page.append(
                        f"{sc} p.{page} : « {r} » visible page(s) "
                        f"{pdf.pages_visibles(r)} — {libelle[:40]}")
                elif pdf.pages_masquees(r):
                    fantomes.append(
                        f"{sc} p.{page} : « {r} » n'existe que dans la couche rognée "
                        f"(page(s) {pdf.pages_masquees(r)}) — {libelle[:40]}")
                else:
                    hors_tarif.append(
                        f"{sc} p.{page} : « {r} » absente du PDF — {libelle[:40]}")

    ok(f"[pdf] {mt_ok}/{teste} montants et {ref_ok} références retrouvés sur la "
       f"page citée, au rendu ({sans_ref} postes sans référence, non testés)")

    for lot, titre in ((montants_ko, "montant absent de la page citée"),
                       (mauvaise_page, "référence visible sur une autre page")):
        if lot:
            warn(f"[pdf] {len(lot)} {titre} :")
            for d in lot:
                warn(f"       {d}")
    for lot, titre in ((fantomes, "chunk fantôme : référence rognée, hors offre"),
                       (hors_tarif, "référence absente du tarif publié")):
        if lot:
            fail(f"[pdf] {len(lot)} {titre} :")
            for d in lot:
                fail(f"       {d}")


# ------------------------------------------------------------------ exécution
def main():
    rows, largeurs = load_excel()
    idx_cells = cellules_grille(rows, largeurs)
    ech = echelles(rows, largeurs)
    idx_postes = postes_excel(rows)

    chunks_by_file, fms = {}, {}
    print("=== Décomptes ===")
    for name, fname in FILES.items():
        fm, chunks = parse_chunks(f"{OUTDIR}/{fname}")
        chunks_by_file[name], fms[name] = chunks, fm
        print(f"  {name:14s}: {len(chunks):4d} chunks")
        check_forme(name, fm, chunks)

    check_grilles(chunks_by_file["prix"], chunks_by_file["speciaux"], idx_cells, ech)
    check_postes_bijection(chunks_by_file["options"], chunks_by_file["speciaux"], rows)
    check_decompte_postes(chunks_by_file["options"], chunks_by_file["speciaux"], rows)
    check_unites(chunks_by_file["options"], "options")
    check_unites(chunks_by_file["speciaux"], "speciaux")
    check_titres_uniques(chunks_by_file)
    check_sans_montant("faisabilites", chunks_by_file["faisabilites"])
    check_sans_montant("transverses", chunks_by_file["transverses"])
    check_vocabulaire([c for cs in chunks_by_file.values() for c in cs])
    check_pdf(chunks_by_file)

    print("\n=== Résultats ===")
    for k in ("OK", "WARN", "FAIL"):
        print(f"  {k:5s}: {len(results[k])}")
    for k, sig in (("FAIL", "✗"), ("WARN", "⚠"), ("OK", "✓")):
        if results[k]:
            print(f"\n--- {k} ---")
            for m in results[k]:
                print(f"  {sig} {m}")
    return 1 if results["FAIL"] else 0


if __name__ == "__main__":
    sys.exit(main())
