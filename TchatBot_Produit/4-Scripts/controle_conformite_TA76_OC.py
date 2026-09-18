#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Contrôle de conformité des chunks du tarif TA76 OC.

AUDIT AUTONOME : ce script relit les fichiers .md produits sans réutiliser
aucune fonction ni aucune table du générateur. Il re-dérive ses règles depuis
note_cadrage_migration_tarif_TA76_OC.md et confronte les chunks à l'Excel
(fidélité numérique) et au PDF (attribution des pages).

Quatorze contrôles :
   1. décomptes et front matter
   2. forme des chunks : plafond, ligne de source, continuité SC, préfixe
   3. auto-discrimination TA76 OC / TA76 OV
   4. unicité des titres dans chaque fichier
   5. couverture exhaustive des cellules de grille (une cellule, un chunk)
   6. fidélité numérique des cellules de grille contre l'Excel
   7. bornes de bandes recalculées indépendamment
   8. bijection des postes forfaitaires (multiensembles HT/TTC)
   9. déclaration d'une unité de facturation sur tout poste chiffré
  10. absence de tout montant dans les faisabilités et les transverses
  11. exclusions arbitrées effectivement absentes du corpus
  12. vocabulaire et faux synonymes
  13. croisement PDF page par page
  14. conformité de la table des pages aux en-têtes du PDF
"""
import os
import re
import sys
from collections import Counter, OrderedDict, defaultdict

import openpyxl

OUTDIR = "/mnt/user-data/outputs"
XLSX = "/mnt/user-data/uploads/TA76_OC-infos-tarifs.xlsx"
PDF = "/mnt/user-data/uploads/Tarif_TA76_OC_HT_19-06-2026.pdf"
PLAFOND = 200
PREFIXE = "TA76 OC Fenêtre aluminium à ouvrant caché — "

FILES = OrderedDict([
    ("Tarif_TA76_OC_METHODE.md", "methode"),
    ("Tarif_TA76_OC_PRIX_CHASSIS.md", "prix_chassis"),
    ("Tarif_TA76_OC_OPTIONS.md", "options"),
    ("Tarif_TA76_OC_CHASSIS_SPECIAUX.md", "chassis_speciaux"),
    ("Tarif_TA76_OC_FAISABILITES.md", "faisabilites"),
    ("Tarif_TA76_OC_TRANSVERSES.md", "transverses"),
])

RE_SOURCE = re.compile(
    r"^\*Source : Tarif—TA76_OC—HT—19-06-2026\.pdf, page (\d+) — "
    r"information (originale|complémentaire) — (SC\d{4})\*$")

# Correspondance libellé de grille -> (chapitre, tableau) de l'Excel, re-dérivée
# depuis la note de cadrage (§ 2.2) et non reprise du générateur.
GRILLES_2D = {
    "châssis à 1 ouvrant à la française": ("1 OF", None),
    "châssis à 2 ouvrants égaux à la française": ("2 OF", None),
    "châssis fixe": ("Châssis fixes", None),
    "châssis à soufflet normal (SN)": ("Châssis à soufflet", "SN"),
    "châssis à soufflet normal à poignée latérale (SN)":
        ("Châssis à soufflet", "SN avec poignée"),
    "châssis à soufflet d'aération avec ferme-imposte (SA)":
        ("Châssis à soufflet", "SA"),
    "habillage alu rectangulaire sur vitrage":
        ("Habillage Alu sur vitrage", "habillage rectangulaire"),
    "habillage alu cintré sur vitrage":
        ("Habillage Alu sur vitrage", "habillage cintrés"),
}
GRILLES_1D = {
    "grille d'entrée d'air Invisivent EVO sur châssis blanc":
        ("Grilles d'air spé Belgique", "Invisivent EVO sur châssis blanc"),
    "grille d'entrée d'air Invisivent EVO sur châssis d'une autre couleur":
        ("Grilles d'air spé Belgique", "Invisivent EVO sur châssis autre couleur"),
    "grille d'entrée d'air THM90 EVO sur châssis blanc":
        ("Grilles d'air spé Belgique", "THM90 EVO sur châssis blanc"),
    "grille d'entrée d'air THM90 EVO sur châssis d'une autre couleur":
        ("Grilles d'air spé Belgique", "THM90 EVO sur châssis autre couleur"),
}

# Lignes Excel dont la note de cadrage prescrit l'exclusion (§ 2.4 et arbitrages).
EXCLUSIONS_ATTENDUES = {283, 298, 189, 191, 170, 210, 212}
CHAP_GRILLE = {"1 OF", "2 OF", "Châssis fixes", "Châssis à soufflet",
               "Habillage Alu sur vitrage", "Grilles d'air spé Belgique"}
CHAP_EXCLUS = {"Exemple de calcul"}

FAUX_SYNONYMES = ["gond", "charnière", "survitrage", "anti-dégondage",
                  "ouverture à soufflet", "fillant"]
# la note (§ 2.5) autorise explicitement « crémone » sur cette gamme aluminium

results = {"OK": [], "WARN": [], "FAIL": []}
def ok(m):   results["OK"].append(m)
def warn(m): results["WARN"].append(m)
def fail(m): results["FAIL"].append(m)


# ============================================================ lecture
def parse_chunks(path):
    txt = open(path, encoding="utf-8").read()
    fm = {}
    if txt.startswith("---"):
        head, txt = txt[3:].split("---", 1)
        for line in head.strip().splitlines():
            if ":" in line:
                k, v = line.split(":", 1)
                fm[k.strip()] = v.strip().strip('"')
    chunks = []
    for bloc in re.split(r"\n(?=## )", txt):
        bloc = bloc.strip()
        if not bloc.startswith("## "):
            continue
        lignes = bloc.split("\n")
        titre = lignes[0][3:].strip()
        source = lignes[1].strip() if len(lignes) > 1 else ""
        corps = "\n".join(lignes[2:]).strip()
        chunks.append({"titre": titre, "source": source, "corps": corps,
                       "mots": len(re.findall(r"\S+", bloc))})
    return fm, chunks


def montants_ht(txt):
    """Montants HT seuls : l'édition fournie du tarif est l'édition HT."""
    return [int(m.replace("\u202f", "").replace("\xa0", ""))
            for m in re.findall(r"([\d\u202f\xa0]+) € HT", txt)]


def montants(txt):
    """Tous les montants en euros d'un texte, dans l'ordre."""
    return [int(m.replace("\u202f", "").replace("\xa0", ""))
            for m in re.findall(r"([\d\u202f\xa0]+) €", txt)]


def clean(v):
    return "" if v is None else re.sub(r"\s+", " ", str(v).replace("\xa0", " ")).strip()


def load_excel():
    wb = openpyxl.load_workbook(XLSX, data_only=True)
    ws = wb["Feuil1"]
    raw = list(ws.iter_rows(values_only=True))
    hdr = raw[0]
    largeurs = [int(str(h).split()[2]) for h in hdr[17:77]]
    rows = []
    for i, r in enumerate(raw[1:], start=2):
        if all(v is None for v in r):
            continue
        rows.append({"xl": i, "v": list(r)})
    return largeurs, rows


def cellules_excel(rows, largeurs):
    """{(chap, tab) : {hauteur : [(largeur, ht, ttc)]}} — reconstruit à part."""
    d = defaultdict(lambda: defaultdict(list))
    for r in rows:
        v = r["v"]
        chap, tab = clean(v[0]), clean(v[1]) or None
        if chap not in CHAP_GRILLE:
            continue
        for k in range(60):
            if v[17 + k] is None:
                continue
            d[(chap, tab)][v[16]].append((largeurs[k], v[17 + k], v[77 + k]))
    return d


def postes_excel(rows):
    """Multiensemble des couples (HT, TTC) forfaitaires, hors grilles et exclusions."""
    c = Counter()
    for r in rows:
        v = r["v"]
        chap = clean(v[0])
        if not chap or chap in CHAP_GRILLE or chap in CHAP_EXCLUS:
            continue
        if r["xl"] in EXCLUSIONS_ATTENDUES or v[8] is None:
            continue
        c[(int(v[8]), int(v[9]))] += 1
    return c


# ============================================================ 1-2. forme
def check_forme(nom, fm, chunks):
    if int(fm.get("nb_chunks", -1)) != len(chunks):
        fail(f"{nom} : front matter nb_chunks={fm.get('nb_chunks')} pour "
             f"{len(chunks)} chunks réels")
    for champ in ("document_source", "type_document", "sous_type", "gamme_code",
                  "gamme_nom", "collection", "materiau", "date_validite", "audiences"):
        if champ not in fm:
            fail(f"{nom} : champ de front matter manquant — {champ}")
    if fm.get("gamme_code") != "TA76_OC":
        fail(f"{nom} : gamme_code = {fm.get('gamme_code')} au lieu de TA76_OC")

    sc_attendu = 2
    for c in chunks:
        if c["mots"] > PLAFOND:
            fail(f"{nom} : plafond dépassé ({c['mots']} mots) — {c['titre'][:70]}")
        m = RE_SOURCE.match(c["source"])
        if not m:
            fail(f"{nom} : ligne de source non conforme — {c['titre'][:70]}")
            continue
        page, _, sc = int(m.group(1)), m.group(2), m.group(3)
        c["page"], c["sc"] = page, sc
        if sc != f"SC{sc_attendu:04d}":
            fail(f"{nom} : continuité SC rompue, {sc} au lieu de SC{sc_attendu:04d}")
        sc_attendu += 1
        if not (1 <= page <= 70):
            fail(f"{nom} : page {page} hors du tarif (70 pages)")
        if not c["titre"].startswith(PREFIXE):
            fail(f"{nom} : préfixe de titre absent — {c['titre'][:70]}")
        if not c["corps"]:
            fail(f"{nom} : corps vide — {c['titre'][:70]}")


# ============================================================ 3. OC / OV
def check_discrimination(nom, chunks, sous_type):
    for c in chunks:
        t = c["titre"]
        if "TA76 OC" not in t:
            fail(f"{nom} : titre sans code gamme complet — {t[:70]}")
        if "ouvrant caché" not in t:
            fail(f"{nom} : titre sans mention « ouvrant caché » — {t[:70]}")
        for m in re.finditer(r"TA76(?! OC)(?! OV)", t):
            fail(f"{nom} : titre réduisant la gamme à « TA76 » — {t[:70]}")
        if "TA76 OV" in t:
            fail(f"{nom} : la gamme jumelle apparaît dans un TITRE — {t[:70]}")
        if "TA76 OV" in c["corps"] and sous_type not in (
                "chassis_speciaux", "faisabilites", "methode", "transverses"):
            fail(f"{nom} : mention de TA76 OV hors des fichiers autorisés — {t[:70]}")
    if sous_type == "chassis_speciaux":
        manquants = [c["titre"][:60] for c in chunks
                     if "saisie de la commande s'effectue en TA76 OV" not in c["corps"]]
        if manquants:
            fail(f"{nom} : règle OC1 non appliquée sur {len(manquants)} chunks")
        else:
            ok(f"{nom} : règle OC1 appliquée sur les {len(chunks)} chunks")


# ============================================================ 4. unicité
def check_titres_uniques(par_fichier):
    faux = 0
    for nom, chunks in par_fichier.items():
        c = Counter(x["titre"] for x in chunks)
        for t, n in c.items():
            if n > 1:
                fail(f"{nom} : titre en doublon ({n} occurrences) — {t[:75]}")
                faux += 1
    if not faux:
        ok("Unicité des titres : aucun doublon dans aucun fichier")


# ============================================================ 5-7. grilles
RE_T2D = re.compile(r"Tarif du (.+?), hauteur (jusqu'à \d+|de \d+ à \d+) mm, "
                    r"(toutes largeurs tarifées|largeurs (?:jusqu'à \d+|de \d+ à \d+) mm)$")
RE_T1D = re.compile(r"Tarif de la (grille d'entrée d'air .+?), "
                    r"largeurs (jusqu'à \d+|de \d+ à \d+) mm$")
RE_H = re.compile(r"cote tarif en hauteur (?:jusqu'à (\d+)|de (\d+) à (\d+)) mm")
RE_ITEM = re.compile(r"en largeur (?:jusqu'à (\d+)|de (\d+) à (\d+)) mm, "
                     r"([\d\u202f]+) € HT et ([\d\u202f]+) € TTC")


def bornes_attendues(valeurs):
    """Recalcul indépendant : la bande de v est (précédente + 1) .. v."""
    return {v: (None if i == 0 else valeurs[i - 1] + 1)
            for i, v in enumerate(valeurs)}


def check_grilles(chunks, cells):
    vus = Counter()          # (chap, tab, hauteur, largeur) -> nb de chunks
    erreurs_fid, erreurs_bornes, non_reconnus = 0, 0, 0
    for c in chunks:
        titre = c["titre"][len(PREFIXE):]
        m2, m1 = RE_T2D.match(titre), RE_T1D.match(titre)
        if m2:
            lib = m2.group(1)
            if lib not in GRILLES_2D:
                fail(f"Grille inconnue dans un titre : {lib}")
                non_reconnus += 1
                continue
            key = GRILLES_2D[lib]
            mh = RE_H.search(c["corps"])
            if not mh:
                fail(f"Bande de hauteur absente du corps — {titre[:60]}")
                continue
            h_bas = int(mh.group(2)) if mh.group(2) else None
            h_haut = int(mh.group(1) or mh.group(3))
        elif m1:
            lib = m1.group(1)
            if lib not in GRILLES_1D:
                fail(f"Grille à un seul axe inconnue : {lib}")
                non_reconnus += 1
                continue
            key = GRILLES_1D[lib]
            h_bas, h_haut = None, None
            if "ne dépend que de la largeur" not in c["corps"]:
                fail(f"Grille à un seul axe sans mention explicite — {titre[:60]}")
        else:
            fail(f"Titre de grille non analysable — {titre[:70]}")
            non_reconnus += 1
            continue

        grille = cells.get(key)
        if grille is None:
            fail(f"Grille absente de l'Excel : {key}")
            continue
        hauteurs = sorted([h for h in grille if h is not None])
        bornes_H = bornes_attendues(hauteurs)
        if m2:
            if h_haut not in grille:
                fail(f"Hauteur {h_haut} absente de l'Excel pour {key}")
                continue
            if bornes_H.get(h_haut) != h_bas:
                fail(f"Borne basse de hauteur erronée pour {key} h={h_haut} : "
                     f"{h_bas} au lieu de {bornes_H.get(h_haut)}")
                erreurs_bornes += 1
            ligne = grille[h_haut]
        else:
            ligne = grille[None] if None in grille else grille[list(grille)[0]]
            h_haut = None

        toutes_L = sorted({L for hh in grille for (L, _, _) in grille[hh]})
        bornes_L = bornes_attendues(toutes_L if m2 else sorted(L for L, _, _ in ligne))
        ref = {L: (ht, ttc) for L, ht, ttc in ligne}

        for it in RE_ITEM.finditer(c["corps"]):
            l_bas = int(it.group(2)) if it.group(2) else None
            l_haut = int(it.group(1) or it.group(3))
            ht = int(it.group(4).replace("\u202f", ""))
            ttc = int(it.group(5).replace("\u202f", ""))
            if l_haut not in ref:
                fail(f"Largeur {l_haut} absente de l'Excel pour {key} h={h_haut}")
                continue
            vus[(key, h_haut, l_haut)] += 1
            if bornes_L.get(l_haut) != l_bas:
                fail(f"Borne basse de largeur erronée pour {key} h={h_haut} "
                     f"L={l_haut} : {l_bas} au lieu de {bornes_L.get(l_haut)}")
                erreurs_bornes += 1
            e_ht, e_ttc = ref[l_haut]
            if int(e_ht) != ht or int(e_ttc) != ttc:
                fail(f"INFIDÉLITÉ NUMÉRIQUE {key} h={h_haut} L={l_haut} : "
                     f"chunk {ht}/{ttc}, Excel {int(e_ht)}/{int(e_ttc)}")
                erreurs_fid += 1

    attendu = Counter()
    for key, grille in cells.items():
        for h, ligne in grille.items():
            for L, _, _ in ligne:
                attendu[(key, h, L)] += 1
    manquantes = [k for k in attendu if k not in vus]
    doublons = [k for k, n in vus.items() if n > 1]
    orphelines = [k for k in vus if k not in attendu]
    if manquantes:
        fail(f"COUVERTURE : {len(manquantes)} cellules de l'Excel dans aucun chunk "
             f"(ex. {manquantes[:3]})")
    if doublons:
        fail(f"COUVERTURE : {len(doublons)} cellules présentes dans plusieurs chunks "
             f"(ex. {doublons[:3]})")
    if orphelines:
        fail(f"COUVERTURE : {len(orphelines)} cellules servies sans contrepartie Excel")
    if not (manquantes or doublons or orphelines):
        ok(f"Couverture exhaustive : les {len(attendu)} cellules de grille de l'Excel "
           f"sont chacune dans un chunk et un seul")
    if not erreurs_fid:
        ok(f"Fidélité numérique des grilles : {sum(vus.values())} couples HT/TTC "
           f"vérifiés cellule par cellule, aucun écart")
    if not erreurs_bornes:
        ok("Bornes de bandes : toutes recalculées indépendamment et conformes")
    if not non_reconnus:
        ok("Titres de grille : tous analysables et rattachés à une grille de l'Excel")


# ============================================================ 8-9. postes
RE_POSTE_HT = re.compile(r"([\d\u202f]+) € HT")


# tableaux à maille propre, prescrits par l'amendement OC 3 de la règle T4
TAB_MAILLE_FINITION = {
    ("Calcul croisillons intégrés", "Croisillons en alu laqué"),
    ("Croisillons intégrés+grecque", "Croisillons en alu 10 mm"),
    ("Croisillons Art Déco", "/ champ"),
    ("Croisillons rapportés", "2F en alu"),
}
TAB_MAILLE_MOTIF = ("Croisillons Art Déco", "/ volume")
NB_POSTES_PDF_SEULS = 8    # postes à 0 € relevés au PDF et absents de l'Excel


def groupes_excel(rows):
    """Groupes iso-prix et iso-unité, après application des mailles de la note."""
    par_tab = defaultdict(set)
    for r in rows:
        v = r["v"]
        chap, tab = clean(v[0]), clean(v[1]) or None
        if not chap or chap in CHAP_GRILLE or chap in CHAP_EXCLUS:
            continue
        if r["xl"] in EXCLUSIONS_ATTENDUES or v[8] is None:
            continue
        par_tab[(chap, tab)].add((clean(v[4]), int(v[8]), int(v[9]), clean(v[5])))
    total = 0
    for key, groupes in par_tab.items():
        sans_det = {(d, h, t) for d, h, t, _ in groupes}
        if key in TAB_MAILLE_FINITION:
            # maille finition : un chunk porte le prix en T ou croix ET le filant
            total += len(sans_det) // 2 + len(sans_det) % 2
        elif key == TAB_MAILLE_MOTIF:
            # maille motif : un chunk par valeur de la colonne Détails
            total += len({det for _, _, _, det in groupes})
        else:
            total += len(sans_det)
    return par_tab, total


def check_postes(chunks_opt, chunks_spec, rows):
    tous = chunks_opt + chunks_spec
    # 8a. inclusion des montants, dans les deux sens
    attendu = set()
    for r in rows:
        v = r["v"]
        chap = clean(v[0])
        if not chap or chap in CHAP_GRILLE or chap in CHAP_EXCLUS:
            continue
        if r["xl"] in EXCLUSIONS_ATTENDUES or v[8] is None:
            continue
        attendu.add((int(v[8]), int(v[9])))
    servi = set()
    for c in tous:
        for ht, ttc in re.findall(r"([\d\u202f]+) € HT,? (?:et |soit )([\d\u202f]+) € TTC",
                                  c["corps"]):
            servi.add((int(ht.replace("\u202f", "")), int(ttc.replace("\u202f", ""))))
    manque = attendu - servi
    if manque:
        fail(f"BIJECTION : {len(manque)} couples HT/TTC de l'Excel ne figurent dans "
             f"aucun chunk — {sorted(manque)[:6]}")
    else:
        ok(f"Bijection des postes : les {len(attendu)} couples HT/TTC distincts de "
           f"l'Excel figurent tous dans le corpus")
    surplus = servi - attendu - {(0, 0)}
    if surplus:
        justifies = sum(1 for c in tous if "aucune ne prévaut sur l'autre" in c["corps"])
        if justifies:
            ok(f"Montants en surplus ({len(surplus)}) : imputables aux {justifies} "
               f"chunks exposant une divergence entre deux pages du tarif")
        else:
            fail(f"BIJECTION : montants servis sans contrepartie Excel — {sorted(surplus)}")
    if (0, 0) in servi:
        n0 = sum(1 for c in tous if "n'entraîne aucune plus-value" in c["corps"])
        ok(f"Postes sans plus-value : {n0} chunks à 0 €, conformes à l'amendement "
           f"OC 2 de la règle T4")

    # 8b. décompte des postes, re-dérivé des mailles prescrites par la note
    _, attendu_n = groupes_excel(rows)
    reel = len(tous)
    if reel != attendu_n + NB_POSTES_PDF_SEULS:
        fail(f"DÉCOMPTE DES POSTES : {reel} chunks pour {attendu_n} groupes de l'Excel "
             f"plus {NB_POSTES_PDF_SEULS} postes relevés au seul PDF, soit "
             f"{attendu_n + NB_POSTES_PDF_SEULS} attendus")
    else:
        ok(f"Décompte des postes : {reel} chunks = {attendu_n} groupes iso-prix de "
           f"l'Excel + {NB_POSTES_PDF_SEULS} postes relevés au seul PDF")


def check_unites(nom, chunks):
    sans = []
    for c in chunks:
        if "0 € HT et 0 € TTC" in c["corps"] or "n'entraîne aucune plus-value" in c["corps"]:
            continue
        if not montants(c["corps"]):
            continue
        corps = c["corps"]
        a_unite = ("s'entend" in corps or "s'entendent" in corps
                   or "est forfaitaire" in corps or "sont forfaitaires" in corps
                   or "n'énonce pas d'unité de facturation" in corps)
        if not a_unite:
            sans.append(c["titre"][:70])
    if sans:
        fail(f"{nom} : {len(sans)} chunks chiffrés sans unité de facturation déclarée "
             f"— ex. {sans[:3]}")
    else:
        ok(f"{nom} : tout poste chiffré déclare son unité de facturation")


# ============================================================ 10. sans montant
RE_MONTANT = re.compile(r"\d+\s*(?:€|%)")


def check_sans_montant(nom, chunks):
    faux = [c["titre"][:70] for c in chunks if RE_MONTANT.search(c["corps"])]
    if faux:
        fail(f"{nom} : {len(faux)} chunks portent un montant ou un pourcentage "
             f"(règles T6 et T7) — ex. {faux[:3]}")
    else:
        ok(f"{nom} : aucun montant ni pourcentage, conforme aux règles T6 et T7")


# ============================================================ 11. exclusions
def check_exclusions(tous, rows):
    par_xl = {r["xl"]: r["v"] for r in rows}
    for xl in sorted(EXCLUSIONS_ATTENDUES):
        v = par_xl.get(xl)
        if v is None or v[8] is None:
            continue
        ht, des = int(v[8]), clean(v[4])
        if ht == 0 or not des:
            continue
        for c in tous:
            corps = c["corps"]
            if re.search(rf"(?<!\d){ht} € HT", corps) and des.lower() in corps.lower():
                fail(f"EXCLUSION NON RESPECTÉE : Excel {xl} ({des}, {ht} € HT) servi "
                     f"dans « {c['titre'][:60]} »")
    ok(f"Exclusions arbitrées : les {len(EXCLUSIONS_ATTENDUES)} lignes écartées sont "
       f"absentes du corpus")


# ============================================================ 12. vocabulaire
def check_vocabulaire(tous):
    faux = 0
    for c in tous:
        texte = (c["titre"] + " " + c["corps"]).lower()
        for mot in FAUX_SYNONYMES:
            if re.search(rf"\b{re.escape(mot)}", texte):
                fail(f"VOCABULAIRE : « {mot} » dans {c['titre'][:60]}")
                faux += 1
    if not faux:
        ok("Vocabulaire : aucun faux synonyme, aucune coquille de l'Excel propagée")
    # le terme « crémone » est légitime sur cette gamme aluminium (note § 2.5)
    n = sum(1 for c in tous if "crémone" in c["corps"].lower())
    if n:
        ok(f"Vocabulaire : « crémone » employé dans {n} chunks, usage légitime sur "
           f"gamme aluminium et conforme au tarif (pages 13, 36 et 51)")


# ============================================================ 13-14. PDF
_TOKENS = {}


def tokens_page(cache, page):
    """Nombres lisibles sur la page, y compris ceux dont le séparateur de
    milliers est une espace (« 1 108 » se lit aussi 1108)."""
    if page in _TOKENS:
        return _TOKENS[page]
    bruts = re.findall(r"\d+", cache.get(page, ""))
    t = set(bruts)
    for i in range(len(bruts) - 1):
        if len(bruts[i]) <= 2 and len(bruts[i + 1]) == 3:
            t.add(bruts[i] + bruts[i + 1])
    _TOKENS[page] = t
    return t


def check_pdf(tous):
    try:
        import pdfplumber
    except ImportError:
        warn("Croisement PDF non exécuté : pdfplumber indisponible")
        return
    cache = {}
    with pdfplumber.open(PDF) as pdf:
        if len(pdf.pages) != 70:
            fail(f"Le PDF compte {len(pdf.pages)} pages au lieu de 70")
        for i, p in enumerate(pdf.pages, 1):
            cache[i] = (p.extract_text() or "")
    # 14. la pagination imprimée coïncide-t-elle avec l'index PDF ?
    ecarts = []
    for i, t in cache.items():
        m = (re.search(r"(?:^|\n)\s*(\d{1,3})\s*-\s*V\.19/06/2026", t)
             or re.search(r"V\.19/06/2026\s*-\s*(\d{1,3})", t))
        if m and int(m.group(1)) != i:
            ecarts.append((i, int(m.group(1))))
    if ecarts:
        fail(f"Pagination : {len(ecarts)} pages dont le numéro imprimé diffère de "
             f"l'index PDF — {ecarts[:5]}")
    else:
        ok("Pagination : le numéro imprimé coïncide avec l'index PDF sur les 70 pages")

    # 13. chaque montant servi figure-t-il sur la page citée ?
    testes = introuvables = 0
    renvoyes = []
    for c in tous:
        page = c.get("page")
        if not page:
            continue
        txt = cache.get(page, "")
        divergence = "aucune ne prévaut sur l'autre" in c["corps"]
        for v in montants_ht(c["corps"]):
            if v == 0:
                continue
            testes += 1
            if str(v) in tokens_page(cache, page):
                continue
            if divergence:
                renvoyes.append((v, page, c["titre"][:60]))
                continue
            introuvables += 1
            if introuvables <= 8:
                warn(f"Montant {v} € introuvable sur la page {page} citée — "
                     f"{c['titre'][:60]}")
    if renvoyes:
        ok(f"Divergences exposées : {len(renvoyes)} montants cités hors de leur page "
           f"parce qu'un chunk signale le montant porté par l'autre page, conformément "
           f"à la règle OC3")
    if testes:
        testes -= len(renvoyes)
        taux = 100 * (testes - introuvables) / testes
        msg = (f"Croisement PDF : {testes - introuvables} montants sur {testes} "
               f"retrouvés sur la page citée ({taux:.1f} %)")
        if taux >= 99:
            ok(msg + " — la table des pages est validée")
        elif taux >= 95:
            warn(msg + " — écart imputable au bruit d'extraction, à vérifier")
        else:
            fail(msg + " — table des pages suspecte")


# ============================================================ main
def main():
    largeurs, rows = load_excel()
    cells = cellules_excel(rows, largeurs)

    par_fichier, tous = OrderedDict(), []
    for fname, sous_type in FILES.items():
        path = os.path.join(OUTDIR, fname)
        if not os.path.exists(path):
            fail(f"Fichier absent : {fname}")
            continue
        fm, chunks = parse_chunks(path)
        check_forme(fname, fm, chunks)
        check_discrimination(fname, chunks, sous_type)
        par_fichier[fname] = chunks
        tous += chunks

    check_titres_uniques(par_fichier)
    check_grilles(par_fichier.get("Tarif_TA76_OC_PRIX_CHASSIS.md", []), cells)
    check_postes(par_fichier.get("Tarif_TA76_OC_OPTIONS.md", []),
                 par_fichier.get("Tarif_TA76_OC_CHASSIS_SPECIAUX.md", []), rows)
    for f in ("Tarif_TA76_OC_OPTIONS.md", "Tarif_TA76_OC_CHASSIS_SPECIAUX.md"):
        check_unites(f, par_fichier.get(f, []))
    for f in ("Tarif_TA76_OC_FAISABILITES.md", "Tarif_TA76_OC_TRANSVERSES.md"):
        check_sans_montant(f, par_fichier.get(f, []))
    check_exclusions(tous, rows)
    check_vocabulaire(tous)
    check_pdf(tous)

    # l'absence d'édition TTC est une limite assumée (note § 8)
    warn("Aucune édition TTC du tarif n'existe : les montants TTC ne sont "
         "contrôlables que contre l'Excel, sans second témoin")

    print("=" * 74)
    print("CONTRÔLE DE CONFORMITÉ — TARIF TA76 OC")
    print("=" * 74)
    print(f"\nChunks relus : {len(tous)} répartis en {len(par_fichier)} fichiers")
    for k in ("FAIL", "WARN", "OK"):
        if results[k]:
            print(f"\n--- {k} ({len(results[k])}) ---")
            for m in results[k]:
                print(f"  {'✗' if k == 'FAIL' else '!' if k == 'WARN' else '✓'} {m}")
    print(f"\n{len(results['OK'])} contrôles réussis, {len(results['FAIL'])} échecs, "
          f"{len(results['WARN'])} avertissements.")
    return 1 if results["FAIL"] else 0


if __name__ == "__main__":
    sys.exit(main())
