#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Contrôle de conformité AUTONOME des chunks du tarif CA76 / CAG76.

Ne réutilise AUCUNE fonction du générateur : relit les .md produits et les
confronte à la note de cadrage (forme et règles, re-dérivées ici), à l'Excel
(fidélité numérique) et au PDF (pages, pourcentages, marquage graphique).
Toutes les tables ci-dessous sont des RESTATEMENTS indépendants : si le
générateur et l'audit divergent, c'est un écart réel, pas une tautologie.

Contrôles :
   1 décomptes par fichier, recomptés depuis l'Excel
   2 forme : plafond, ligne de source, préfixe de titre, continuité SC, YAML
   3 couverture exhaustive des 4 077 cellules : chacune dans un chunk et un seul
   4 fidélité numérique exhaustive, HT et TTC, contre la cellule
   5 bornes de bandes recalculées indépendamment depuis l'échelle du tableau
   6 anti-fantôme : aucune cellule servie qui n'existe dans l'Excel
   7 frontière de page : une tranche ne déborde jamais la page qu'elle cite
   8 croisée renforcée : marquage re-extrait de la géométrie et reconfronté
   9 bijection des postes forfaitaires, multiensembles (HT, TTC), sans libellés
  10 unité de facturation déclarée sur tout poste chiffré
  11 pourcentages de F4 retrouvés sur leur page PDF (seul témoin : l'Excel n'en
     porte aucun)
  12 unicité des titres dans chaque fichier
  13 absence de tout montant dans les faisabilités et les transverses
  14 vocabulaire : faux synonymes, contamination inter-gammes
  15 croisement PDF page par page, qui valide au passage la table des pages
"""
import re
import subprocess
import sys
import unicodedata
from collections import Counter, OrderedDict, defaultdict

import openpyxl

OUTDIR = "/mnt/user-data/outputs"
XLSX = "/mnt/user-data/uploads/CA76_-infos-tarifs.xlsx"
PDF = "/mnt/user-data/uploads/Tarif_CA76_HT_19-06-2026.pdf"
FEUILLE = "Feuil2"
PLAFOND = 200

FILES = OrderedDict([
    ("methode", "Tarif_CA76_METHODE.md"),
    ("prix", "Tarif_CA76_PRIX_CHASSIS.md"),
    ("options", "Tarif_CA76_OPTIONS.md"),
    ("proportionnelles", "Tarif_CA76_PLUS_VALUES_PROPORTIONNELLES.md"),
    ("faisabilites", "Tarif_CA76_FAISABILITES.md"),
    ("transverses", "Tarif_CA76_TRANSVERSES.md"),
])

RE_SOURCE = re.compile(
    r"^\*Source : Tarif—CA76—HT—19-06-2026\.pdf, page (\d+) — "
    r"information (originale|complémentaire) — (SC\d{4})\*$")

PREFIXES = ("CA76 Coulissant Aluminium — ",
            "CAG76 Coulissant Aluminium à galandage — ")

# ------- restatements indépendants ------------------------------------------
LIBELLE_GRILLE = {
    "coulissant 2 vantaux sur 2 rails": ("2 vantaux 2 rails", "2V 2 rails"),
    "coulissant 3 vantaux sur 2 rails": ("3 vantaux 2 rails", "3V 2 rails"),
    "coulissant 4 vantaux sur 2 rails": ("4 vantaux 2 rails", "4V 2 rails"),
    "coulissant 3 vantaux sur 3 rails": ("3 vantaux 3 rails", "3V 3 rails"),
    "coulissant 6 vantaux sur 3 rails": ("6 vantaux 3 rails", "6V 3 rails"),
    "coulissant à galandage 1 vantail": ("Galandage 1V", "Galandage 1V"),
    "coulissant à galandage 2 vantaux": ("Galandage 2V", "Galandage 2V"),
    "coulissant à galandage 4 vantaux": ("Galandage 4 V", "Galandage 4 V"),
}
PAGES_GRILLE = {
    "2V 2 rails":    [(11, 1000, 2600), (12, 2700, 4200)],
    "3V 2 rails":    [(13, 1600, 3500), (14, 3600, 5400)],
    "4V 2 rails":    [(15, 2100, 3500), (16, 3600, 5000), (17, 5100, 6400)],
    "3V 3 rails":    [(18, 2100, 3800), (19, 3900, 5500)],
    "6V 3 rails":    [(20, 4200, 6300)],
    "Galandage 1V":  [(22, 600, 1800)],
    "Galandage 2V":  [(23, 1000, 3100)],
    "Galandage 4 V": [(24, 2100, 4300)],
}
# chapitres hors périmètre des postes chiffrés, re-dérivés de la note
CHAP_GRILLE = {k[0] for k in LIBELLE_GRILLE.values()}
CHAP_EXCLUS = {"Exemple de calcul", "Vitrage ornementaux"}
# lignes sans montant valant impossibilité produit, et non gratuité
N_IMPOSSIBILITES = 4
# taux attendus en F4, avec la page qui en est le seul témoin
TAUX_F4 = [(7, "3"), (7, "5"), (7, "10"), (26, "15"), (27, "25"),
           (27, "25"), (27, "25"), (40, "160")]

FAUX_SYNONYMES = ["gond", "charnière", "survitrage", "anti-dégondage",
                  "ouverture à soufflet"]
# « crémone » est LÉGITIME sur la gamme CA : le CA76 est fermé par une crémone
# Secure+ à crochets inox. La règle restrictive de H81 et T81 ne s'y transpose pas.
GAMMES_ETRANGERES = ["TA76", "TA80", "CA80", "T81", "H81", "HA76", "HAM76", "FT84"]

BEIGE = (0.997, 0.92, 0.828)

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


def vide(v):
    return v is None or (isinstance(v, str) and not v.strip())


# ------------------------------------------------------------------ Excel
def load_excel():
    wb = openpyxl.load_workbook(XLSX, data_only=True)
    ws = wb[FEUILLE]
    raw = list(ws.iter_rows(values_only=True))
    header = list(raw[0]) + [None] * (131 - len(raw[0]))
    rows = [list(r) + [None] * (131 - len(r)) for r in raw[1:]
            if not all(vide(v) or v in ("HT", "TTC") for v in r)]
    largeurs = {j: int(str(header[j]).split()[2]) for j in range(13, 72)}
    return rows, largeurs


def cellules_grille(rows, largeurs):
    """{(tableau, hauteur, largeur): (ht, ttc)} recompté seul depuis l'Excel."""
    idx = {}
    tabs = {t for _, t in LIBELLE_GRILLE.values()}
    for r in rows:
        tab = " ".join(str(r[5]).split()) if r[5] else ""
        if tab not in tabs:
            continue
        for j in range(13, 72):
            k = j + 59
            if vide(r[j]) and vide(r[k]):
                continue
            idx[(tab, r[12], largeurs[j])] = (
                None if vide(r[j]) else int(round(float(r[j]))),
                None if vide(r[k]) else int(round(float(r[k]))))
    return idx


def echelles(idx):
    """{tableau: (échelle des largeurs, échelle des hauteurs)} recomptées seules."""
    L, H = defaultdict(set), defaultdict(set)
    for (tab, h, l) in idx:
        L[tab].add(l)
        H[tab].add(h)
    return {t: (sorted(L[t]), sorted(H[t])) for t in L}


def postes_excel(rows):
    """Multiensemble des couples (HT, TTC) attendus hors grilles, hors exclusions.
    Le scalaire est HT/TTC, à défaut PV HT/PV TTC."""
    att = set()
    sans = Counter()
    for r in rows:
        chap = " ".join(str(r[2]).split()) if r[2] else ""
        tab = " ".join(str(r[5]).split()) if r[5] else ""
        des = " ".join(str(r[3]).split()) if r[3] else ""
        if not chap or chap in CHAP_GRILLE or chap in CHAP_EXCLUS:
            continue
        if str(r[0]).strip() != "CA76":
            continue
        ht = r[8] if not vide(r[8]) else r[10]
        ttc = r[9] if not vide(r[9]) else r[11]
        if vide(ht):
            # Trois états de vacuité, re-dérivés ici sans le générateur :
            # coquille d'une plus-value proportionnelle, séparateur interne,
            # ou impossibilité produit. Seule la troisième alimente F5.
            if tab == "pv sur grilles prix":
                sans["proportionnelle"] += 1
            elif not tab and not des:
                sans["separateur"] += 1
            else:
                sans["impossibilite"] += 1
            continue
        att.add((chap, tab, des, int(round(float(ht))),
                 None if vide(ttc) else int(round(float(ttc)))))
    return att, sans


# ------------------------------------------------------------------ 2. forme
def check_forme(name, fm, chunks):
    scs = []
    for c in chunks:
        n = len(re.findall(r"\S+", "## " + c["title"] + " " + c["source"]
                           + " " + c["body"]))
        if n > PLAFOND:
            fail(f"[{name}] plafond dépassé ({n} mots) : {c['title'][:60]}")
        m = RE_SOURCE.match(c["source"])
        if not m:
            fail(f"[{name}] ligne de source non conforme : {c['source'][:70]}")
        else:
            scs.append(int(m.group(3)[2:]))
        if not c["title"].startswith(PREFIXES):
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
    for k in ("document_source", "type_document", "gamme_code", "gamme_nom",
              "nb_chunks", "collection"):
        if k not in fm:
            warn(f"[{name}] front matter : clé « {k} » absente")
    if f"nb_chunks: {len(chunks)}" not in fm:
        fail(f"[{name}] nb_chunks du front matter ≠ nombre de chunks ({len(chunks)})")


# ------------------------------------------------- 3/4/5/6/7. grilles de prix
RE_TITRE = re.compile(r"Tarif du (.+?), hauteur (?:jusqu'à \d+|de \d+ à \d+) mm")
RE_HAUTEUR = re.compile(r"en cote tarif de hauteur (?:jusqu'à (\d+)|de (\d+) à (\d+)) mm")
RE_ITEM = re.compile(r"(?:jusqu'à (\d+)|de (\d+) à (\d+)) mm : "
                     r"([\d\u202f\u00a0 ]+) € HT et ([\d\u202f\u00a0 ]+) € TTC")
RE_SEUIL = re.compile(r"À partir de (\d+) mm de largeur, la croisée renforcée "
                      r"est obligatoire et automatique, sans plus-value")


def check_grilles(chunks, idx, ech, croisee):
    couvert, doublons = {}, []
    hors_page, n_seuil = [], 0
    for c in chunks:
        mt = RE_TITRE.search(c["title"])
        corps = c["body"].split("selon la cote tarif en largeur :", 1)
        mh = RE_HAUTEUR.search(c["body"])
        ms = RE_SOURCE.match(c["source"])
        if not (mt and mh and ms and len(corps) == 2):
            fail(f"[prix] chunk non analysable : {c['title'][:60]}")
            continue
        cle = LIBELLE_GRILLE.get(mt.group(1))
        if not cle:
            fail(f"[prix] libellé de grille inconnu : {mt.group(1)[:50]}")
            continue
        tab = cle[1]
        page = int(ms.group(1))
        h_bas = int(mh.group(2)) if mh.group(2) else None
        h_haut = int(mh.group(3) or mh.group(1))
        eL, eH = ech[tab]

        # bande de hauteur recalculée indépendamment
        att_h = None if eH.index(h_haut) == 0 else eH[eH.index(h_haut) - 1] + 1
        if h_bas != att_h:
            fail(f"[bandes] borne basse de hauteur erronée {tab} H{h_haut} : "
                 f"chunk={h_bas} vs attendu={att_h}")

        largeurs_du_chunk = []
        for m in RE_ITEM.finditer(corps[1]):
            l_bas = int(m.group(2)) if m.group(2) else None
            l_haut = int(m.group(3) or m.group(1))
            ht, ttc = montant(m.group(4)), montant(m.group(5))
            largeurs_du_chunk.append(l_haut)
            key = (tab, h_haut, l_haut)
            if key in couvert:
                doublons.append(key)
            couvert[key] = True

            ref = idx.get(key)
            if ref is None:
                fail(f"[anti-fantôme] cellule absente de l'Excel : {tab} "
                     f"H{h_haut} L{l_haut}")
                continue
            if ht != ref[0]:
                fail(f"[fidélité] ÉCART HT {tab} H{h_haut} L{l_haut} : "
                     f"chunk={ht} vs Excel={ref[0]}")
            if ttc != ref[1]:
                fail(f"[fidélité] ÉCART TTC {tab} H{h_haut} L{l_haut} : "
                     f"chunk={ttc} vs Excel={ref[1]}")
            att_l = None if eL.index(l_haut) == 0 else eL[eL.index(l_haut) - 1] + 1
            if l_bas != att_l:
                fail(f"[bandes] borne basse de largeur erronée {tab} L{l_haut} : "
                     f"chunk={l_bas} vs attendu={att_l}")

        # 7. la tranche ne déborde pas la page citée
        plages = [p for p in PAGES_GRILLE[tab] if p[0] == page]
        if not plages:
            fail(f"[pages] {tab} : page {page} citée, hors des pages de cette grille")
        else:
            _, lo, hi = plages[0]
            deborde = [L for L in largeurs_du_chunk if not (lo <= L <= hi)]
            if deborde:
                hors_page.append((tab, page, deborde[:3]))

        # 8. clause de croisée renforcée reconfrontée à la géométrie
        marquees = croisee.get((tab, h_haut), set())
        dans = sorted(L for L in largeurs_du_chunk if L in marquees)
        ms2 = RE_SEUIL.search(c["body"])
        if dans and not ms2:
            fail(f"[croisée] clause absente alors que {len(dans)} largeurs sont "
                 f"marquées : {tab} H{h_haut}")
        elif ms2 and not dans:
            fail(f"[croisée] clause servie sans aucune largeur marquée : "
                 f"{tab} H{h_haut}")
        elif ms2:
            n_seuil += 1
            seuil = min(marquees)
            att_s = None if eL.index(seuil) == 0 else eL[eL.index(seuil) - 1] + 1
            if int(ms2.group(1)) != (att_s if att_s is not None else seuil):
                fail(f"[croisée] seuil erroné {tab} H{h_haut} : "
                     f"chunk={ms2.group(1)} vs attendu={att_s}")

    manquantes = [k for k in idx if k not in couvert]
    if manquantes:
        fail(f"[couverture] {len(manquantes)} cellules de l'Excel sans chunk : "
             f"{manquantes[:4]}")
    else:
        ok(f"[couverture] les {len(idx)} cellules de grille de l'Excel sont "
           f"couvertes, chacune une seule fois")
    if doublons:
        fail(f"[couverture] {len(doublons)} cellules servies plusieurs fois : "
             f"{doublons[:4]}")
    if hors_page:
        fail(f"[pages] {len(hors_page)} tranches débordent la page citée : "
             f"{hors_page[:3]}")
    else:
        ok("[pages] aucune tranche ne cite une page où ses largeurs ne figurent pas")
    ok(f"[fidélité] {len(couvert)} couples HT/TTC de grille confrontés à l'Excel, "
       f"au €")
    ok(f"[bandes] bornes de bandes recalculées indépendamment sur {len(couvert)} "
       f"items et sur {len(set(k[:2] for k in couvert))} bandes de hauteur")
    ok(f"[croisée] {n_seuil} clauses de seuil reconfrontées à la géométrie du PDF")


# ------------------------------------------------------- 8. géométrie du PDF
def carte_croisee():
    """Ré-extraction INDÉPENDANTE du marquage graphique de croisée renforcée."""
    try:
        import pdfplumber
    except ImportError:
        warn("[croisée] pdfplumber indisponible : contrôle non exécuté")
        return {}
    carte = defaultdict(set)
    with pdfplumber.open(PDF) as doc:
        for tab, tranches in PAGES_GRILLE.items():
            for pno, _, _ in tranches:
                p = doc.pages[pno - 1]
                mots = p.extract_words()
                cand = [w for w in mots if re.fullmatch(r"\d{3,4}", w["text"])]
                if not cand:
                    continue
                ytop = min(w["top"] for w in cand)
                cols = sorted({(round((w["x0"] + w["x1"]) / 2, 1), int(w["text"]))
                               for w in cand
                               if w["top"] < ytop + 6 and int(w["text"]) % 100 == 0})
                xleft = min(w["x0"] for w in cand)
                ligs = sorted({(round((w["top"] + w["bottom"]) / 2, 1), int(w["text"]))
                               for w in cand
                               if w["x0"] < xleft + 25 and w["top"] > ytop + 6
                               and 900 <= int(w["text"]) <= 2600
                               and int(w["text"]) % 100 == 0})
                if not (cols and ligs):
                    continue
                for rect in p.rects:
                    if rect.get("non_stroking_color") != BEIGE or rect["height"] >= 20:
                        continue
                    cx = (rect["x0"] + rect["x1"]) / 2
                    cy = (rect["top"] + rect["bottom"]) / 2
                    c = min(cols, key=lambda t: abs(t[0] - cx))
                    l = min(ligs, key=lambda t: abs(t[0] - cy))
                    if abs(c[0] - cx) > 18 or abs(l[0] - cy) > 10:
                        continue
                    carte[(tab, l[1])].add(c[1])
    return carte


# ------------------------------------------------------------ 9/10. postes
RE_POSTE = re.compile(
    r"le poste « (.+?) » (?:est chiffré en plus-value à "
    r"([\d\u202f\u00a0 ]+) € HT(?:, soit ([\d\u202f\u00a0 ]+) € TTC)?"
    r"|ne donne lieu à aucune plus-value)")
RE_UNITE = re.compile(r"Ce montant (?:s'entend|est) [^.]+\.|"
                      r"Le tarif exprime ce montant dans une unité de facturation")


def check_postes(chunks, rows):
    att, n_sans = postes_excel(rows)

    # postes indiscriminables : plusieurs prix sous des colonnes identiques.
    # La note admet deux issues : non générés, ou discriminés depuis le PDF.
    # L'audit n'arbitre pas ; il exige la bijection ET l'unicité des titres.
    par_cle = defaultdict(set)
    for r in rows:
        chap = " ".join(str(r[2]).split()) if r[2] else ""
        tab = " ".join(str(r[5]).split()) if r[5] else ""
        if (not chap or chap in CHAP_GRILLE or chap in CHAP_EXCLUS
                or str(r[0]).strip() != "CA76" or vide(r[8])):
            continue
        par_cle[(chap, tab, " ".join(str(r[3] or "").split()),
                 " ".join(str(r[4] or "").split()))].add(int(round(float(r[8]))))
    n_indiscr = sum(len(v) for v in par_cle.values() if len(v) > 1)

    attendu = Counter((k[3], k[4]) for k in att)
    servi = Counter()
    n_chiffres, sans_unite = 0, []
    for c in chunks:
        m = RE_POSTE.search(c["body"])
        if not m:
            fail(f"[options] chunk sans poste analysable : {c['title'][:55]}")
            continue
        if m.group(2) is None:
            servi[(0, 0)] += 1
            continue
        n_chiffres += 1
        ttc = montant(m.group(3)) if m.group(3) else None
        servi[(montant(m.group(2)), ttc)] += 1
        if not RE_UNITE.search(c["body"]):
            sans_unite.append(c["title"][:55])

    manque, surplus = attendu - servi, servi - attendu
    if manque:
        fail(f"[postes] {sum(manque.values())} couples de l'Excel non servis : "
             f"{list(manque)[:4]}")
    if surplus:
        fail(f"[postes] {sum(surplus.values())} couples servis absents de l'Excel : "
             f"{list(surplus)[:4]}")
    if not manque and not surplus:
        ok(f"[postes] bijection exacte : {sum(attendu.values())} couples (HT, TTC) "
           f"servis, tous présents dans l'Excel, aucun en trop")
    if n_indiscr:
        ok(f"[postes] {n_indiscr} prix se présentent sous des colonnes identiques "
           f"dans l'Excel ; la bijection et l'unicité des titres tranchent")
    if n_sans["impossibilite"] != N_IMPOSSIBILITES:
        warn(f"[postes] {n_sans['impossibilite']} impossibilités produit relevées, "
             f"{N_IMPOSSIBILITES} attendues par la note — écart à consigner")
    else:
        ok(f"[postes] les trois états de cellule vide sont correctement séparés : "
           f"{n_sans['proportionnelle']} coquilles de plus-value proportionnelle, "
           f"{n_sans['separateur']} séparateurs, "
           f"{n_sans['impossibilite']} impossibilités produit reprises en "
           f"faisabilité — aucune servie comme gratuité")
    if sans_unite:
        fail(f"[options] {len(sans_unite)} postes chiffrés sans unité de "
             f"facturation : {sans_unite[:3]}")
    else:
        ok(f"[options] les {n_chiffres} postes chiffrés déclarent tous une unité "
           f"de facturation")


# ------------------------------------------------------- 11. pourcentages F4
def check_proportionnelles(chunks, cache):
    """Les taux ne figurent PAS dans l'Excel : le PDF en est le seul témoin.
    Chaque taux servi doit être retrouvé sur la page qu'il cite."""
    trouves = 0
    for c in chunks:
        ms = RE_SOURCE.match(c["source"])
        mt = re.search(r"plus-value de \+ (\d+) %", c["body"])
        if not (ms and mt):
            fail(f"[proportionnelles] chunk non analysable : {c['title'][:55]}")
            continue
        page = int(ms.group(1))
        txt = re.sub(r"\s+", "", page_text(cache, page))
        if f"{mt.group(1)}%" in txt:
            trouves += 1
        else:
            fail(f"[proportionnelles] taux + {mt.group(1)} % introuvable page "
                 f"{page} : {c['title'][:55]}")
        if "revient à l'ADV" not in c["body"]:
            fail(f"[proportionnelles] le chunk n'énonce pas que l'application du "
                 f"pourcentage revient à l'ADV : {c['title'][:55]}")
    ok(f"[proportionnelles] {trouves}/{len(chunks)} taux retrouvés sur leur page "
       f"PDF, seul témoin de cette nature d'information")


# ------------------------------------------------------- 12/13/14. forme, fond
def check_titres_uniques(chunks_by_file):
    faux = 0
    for nom, chunks in chunks_by_file.items():
        vus = set()
        for c in chunks:
            if c["title"] in vus:
                fail(f"[{nom}] titre non discriminant : {c['title'][:70]}")
                faux += 1
            vus.add(c["title"])
    if not faux:
        ok("[titres] aucun titre dupliqué dans un même fichier")


RE_MONTANT = re.compile(r"\d+\s*(€|%|/ ?ml|/ ?m²)")


def check_sans_montant(name, chunks):
    n = 0
    for c in chunks:
        if RE_MONTANT.search(c["body"]):
            fail(f"[{name}] montant interdit (règles C6/C7) : {c['title'][:55]}")
            n += 1
    if not n:
        ok(f"[{name}] {len(chunks)} chunks vérifiés sans aucun montant")


def check_vocabulaire(all_chunks):
    n_cremone, n_faux, n_contam = 0, 0, 0
    for c in all_chunks:
        txt = c["title"] + " " + c["body"]
        low = unicodedata.normalize("NFC", txt.lower())
        n_cremone += len(re.findall(r"crémones?", low))
        for faux in FAUX_SYNONYMES:
            if re.search(r"\b" + faux + r"\b", low):
                fail(f"[vocabulaire] faux synonyme « {faux} » : {c['title'][:55]}")
                n_faux += 1
        for g in GAMMES_ETRANGERES:
            if re.search(r"\b" + g + r"\b", txt) and "CA80 New" not in txt:
                fail(f"[vocabulaire] gamme étrangère « {g} » : {c['title'][:55]}")
                n_contam += 1
        if "verrouillage" in low and "ventilation" in low:
            fail(f"[vocabulaire] collocation FT84 « verrouillage + ventilation » : "
                 f"{c['title'][:55]}")
    if not n_faux:
        ok("[vocabulaire] aucun faux synonyme, aucune collocation de contamination")
    if not n_contam:
        ok("[vocabulaire] aucune gamme étrangère, hors la mention explicite de "
           "CA80 New destinée à l'écarter")
    ok(f"[vocabulaire] {n_cremone} occurrences de « crémone », terme légitime sur "
       f"la gamme CA et non soumis à la restriction H81/T81")


# ------------------------------------------------------------- 15. pages PDF
def page_text(cache, p):
    if p not in cache:
        cache[p] = subprocess.run(
            ["pdftotext", "-enc", "UTF-8", "-f", str(p), "-l", str(p), PDF, "-"],
            capture_output=True).stdout.decode("utf-8", "replace")
    return cache[p]


def check_pdf(chunks_by_file, cache):
    teste = retrouve = 0
    echecs = defaultdict(int)
    for c in chunks_by_file["options"]:
        m = RE_POSTE.search(c["body"])
        ms = RE_SOURCE.match(c["source"])
        if not (m and ms and m.group(2)):
            continue
        page = int(ms.group(1))
        txt = re.sub(r"\s+", "", page_text(cache, page))
        teste += 1
        if str(montant(m.group(2))) in txt:
            retrouve += 1
        else:
            echecs[page] += 1
    # échantillon de grille : une cellule par chunk de prix, une sur vingt
    for i, c in enumerate(chunks_by_file["prix"]):
        if i % 20:
            continue
        ms = RE_SOURCE.match(c["source"])
        mi = RE_ITEM.search(c["body"].split("en largeur :", 1)[-1])
        if not (ms and mi):
            continue
        page = int(ms.group(1))
        txt = re.sub(r"\s+", "", page_text(cache, page))
        teste += 1
        if str(montant(mi.group(4))) in txt:
            retrouve += 1
        else:
            echecs[page] += 1
    if echecs:
        warn(f"[pdf] {teste - retrouve}/{teste} montants non retrouvés sur la page "
             f"citée — pages à revoir : {dict(sorted(echecs.items()))}")
    else:
        ok(f"[pdf] croisement page par page : {retrouve}/{teste} montants retrouvés "
           f"sur la page citée ; la table des pages est validée d'autant")


# ------------------------------------------------------------------ exécution
def main():
    rows, largeurs = load_excel()
    idx = cellules_grille(rows, largeurs)
    ech = echelles(idx)
    croisee = carte_croisee()
    cache = {}

    chunks_by_file, fms = {}, {}
    print("=== Décomptes ===")
    for name, fname in FILES.items():
        fm, chunks = parse_chunks(f"{OUTDIR}/{fname}")
        chunks_by_file[name], fms[name] = chunks, fm
        print(f"  {name:18s}: {len(chunks):4d} chunks")
        check_forme(name, fm, chunks)

    print(f"\nCellules de grille recomptées depuis l'Excel : {len(idx)}")
    print(f"Cellules marquées croisée renforcée, re-extraites du PDF : "
          f"{sum(len(v) for v in croisee.values())}")

    check_grilles(chunks_by_file["prix"], idx, ech, croisee)
    check_postes(chunks_by_file["options"], rows)
    check_proportionnelles(chunks_by_file["proportionnelles"], cache)
    check_titres_uniques(chunks_by_file)
    check_sans_montant("faisabilites", chunks_by_file["faisabilites"])
    check_sans_montant("transverses", chunks_by_file["transverses"])
    check_sans_montant("methode", chunks_by_file["methode"])
    check_vocabulaire([c for cs in chunks_by_file.values() for c in cs])
    check_pdf(chunks_by_file, cache)

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
