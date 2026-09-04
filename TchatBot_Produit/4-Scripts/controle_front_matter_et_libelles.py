#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Contrôle croisé des libellés de gamme et du front matter.

Trois sources sont confrontées :

  1. le RÉFÉRENTIEL   — la liste des gammes déclarée dans Instructions_V03.md ;
  2. les FICHIERS     — titres de chunks et front matter des .md du corpus ;
  3. les GÉNÉRATEURS  — libellé et schéma de front matter qu'ils produiraient.

Le point 3 est celui qui manquait : un corpus corrigé à la main reste juste
jusqu'à la première régénération, après quoi le générateur réimpose ses propres
valeurs. Deux incidents l'ont montré — le front matter incomplet de la FT84, et
trois générateurs restés sur l'ancien libellé de gamme, qui auraient réécrit
1 674 titres.

Usage :
    python3 controle_front_matter_et_libelles.py [repertoire]

Sortie : un rapport par gamme, puis un bilan. Code de retour 1 si anomalie.
"""

import os
import re
import sys
import glob
import collections

# --------------------------------------------------------------------------- #
# Schéma canonique
# --------------------------------------------------------------------------- #

CANON_TARIF = [
    "document_source", "document_source_ttc", "type_document", "sous_type",
    "gamme_code", "gamme_nom", "gammes_couvertes", "collection", "materiau",
    "version_doc", "date_validite", "nb_chunks", "audiences",
]
# Champs dont l'absence est légitime : ils dépendent du document ou du périmètre.
CONDITIONNELS = {"document_source_ttc", "gammes_couvertes"}

CANON_DOC = [
    "document_source", "type_document", "gamme_code", "gamme_nom",
    "collection", "materiau", "version_doc", "date_validite", "remplace",
    "audiences",
]

# Champs retirés du schéma : leur présence est une anomalie.
BANNIS = {"glossaire_ref", "perimetre", "remplace"}   # remplace : tarif uniquement

COLLECTIONS_VALIDES = {'"TRYBA ALUMINIUM"', '"TRYBA PVC"'}

SUFFIXES_TARIF = [
    "_PLUS_VALUES_PROPORTIONNELLES", "_CATALOGUE_OPTIONS", "_COMPAT_EQUIPEMENTS",
    "_PAGES_TRANSVERSES", "_CHASSIS_SPECIAUX", "_CARACTERISTIQUES",
    "_OPTIONS_MODELES", "_PRIX_SUR_MESURE", "_PRIX_CHASSIS", "_FAISABILITES",
    "_PRIX_MODELES", "_PRIX_PORTES", "_TRANSVERSES", "_PRIX_FIXES",
    "_PRIX_STOCK", "_METHODE", "_OPTIONS", "_PRIX",
]

# Sous-gammes tolérées dans les titres d'une gamme donnée.
SOUS_GAMMES = {"CA76": ["CAG76"]}


# --------------------------------------------------------------------------- #
# Lecture
# --------------------------------------------------------------------------- #

def lire(chemin):
    with open(chemin, "r", encoding="utf-8") as f:
        return f.read()


def gamme_de_fichier_tarif(nom):
    base = nom[len("Tarif_"):-len(".md")]
    for suf in sorted(SUFFIXES_TARIF, key=len, reverse=True):
        if base.endswith(suf):
            return base[: -len(suf)]
    return base


def referentiel(repertoire):
    """Libellés de gamme déclarés dans Instructions_V03.md.

    C'est la source d'autorité : les fichiers comme les générateurs sont
    comparés à elle, jamais l'un à l'autre. Une divergence entre deux copies
    ne dit pas laquelle est juste.
    """
    chemin = os.path.join(repertoire, "Instructions_V03.md")
    if not os.path.isfile(chemin):
        raise SystemExit("Instructions_V03.md introuvable : pas de référentiel.")
    ref = {}
    for code, libelle in re.findall(r"^-\s+\*\*(.+?)\*\*\s+—\s+(.+?)\s*$",
                                    lire(chemin), flags=re.M):
        ref[code.strip().replace(" ", "_")] = (code.strip(), libelle.strip())
    if not ref:
        raise SystemExit("Aucune gamme lue dans Instructions_V03.md.")
    return ref


def champs_front_matter(contenu):
    """Noms des champs de premier niveau, dans l'ordre du fichier."""
    if not contenu.startswith("---"):
        return None, {}
    bloc = contenu.split("---")[1]
    champs, valeurs = [], {}
    for ligne in bloc.strip().split("\n"):
        if ":" in ligne and not ligne.startswith((" ", "\t", "#")):
            cle, _, val = ligne.partition(":")
            champs.append(cle.strip())
            valeurs[cle.strip()] = val.strip()
    return champs, valeurs


# --------------------------------------------------------------------------- #
# Contrôles sur les fichiers
# --------------------------------------------------------------------------- #

def controler_fichier(chemin, code, libelle, canon, anomalies):
    nom = os.path.basename(chemin)
    t = lire(chemin)
    attendu = f"{code} {libelle}"
    tolerés = {attendu} | {f"{sg} " for sg in SOUS_GAMMES.get(code.replace(" ", "_"), [])}

    titres = re.findall(r"^## (.+)$", t, flags=re.M)

    # 1. Préfixe de titre
    for titre in titres:
        prefixe = titre.split("—")[0].strip()
        if prefixe == attendu:
            continue
        if any(prefixe.startswith(sg) for sg in SOUS_GAMMES.get(code.replace(" ", "_"), [])):
            continue
        anomalies.append((nom, "titre", f"« {prefixe} » au lieu de « {attendu} »"))
        break  # un signalement par fichier suffit

    # 2. Ligne de source unique par chunk
    for bloc in re.split(r"\n(?=## )", t)[1:]:
        lignes = bloc.split("\n")
        if len(lignes) < 2 or not lignes[1].startswith("*Source :"):
            anomalies.append((nom, "source", f"chunk sans ligne de source : {lignes[0][:60]}"))
            break

    # 3. Numérotation SC continue et unique
    sc = re.findall(r"—\s*(SC\d{4})\b", t)
    if len(sc) != len(titres):
        anomalies.append((nom, "SC", f"{len(sc)} identifiants pour {len(titres)} chunks"))
    elif sc != sorted(sc) or len(sc) != len(set(sc)):
        anomalies.append((nom, "SC", "numérotation non continue ou doublon"))

    # 4. Front matter
    champs, valeurs = champs_front_matter(t)
    if champs is None:
        anomalies.append((nom, "front matter", "absent"))
        return len(titres)

    bannis = BANNIS if canon is CANON_TARIF else {"glossaire_ref", "perimetre"}
    presents_bannis = [c for c in champs if c in bannis]
    if presents_bannis:
        anomalies.append((nom, "front matter", "champ retiré du schéma : " + ", ".join(presents_bannis)))

    manquants = [c for c in canon if c not in champs and c not in CONDITIONNELS]
    if manquants:
        anomalies.append((nom, "front matter", "champ manquant : " + ", ".join(manquants)))

    inconnus = [c for c in champs if c not in canon and c not in bannis]
    if inconnus:
        anomalies.append((nom, "front matter", "champ hors schéma : " + ", ".join(inconnus)))

    ordre_attendu = [c for c in canon if c in champs]
    if [c for c in champs if c in canon] != ordre_attendu:
        anomalies.append((nom, "front matter", "ordre des champs non conforme"))

    # 5. Cohérence des valeurs
    if valeurs.get("gamme_nom", "").strip('"') != libelle:
        anomalies.append((nom, "gamme_nom", f"« {valeurs.get('gamme_nom')} » au lieu de « {libelle} »"))
    if "collection" in valeurs and valeurs["collection"] not in COLLECTIONS_VALIDES:
        anomalies.append((nom, "collection", f"valeur ou guillemetage : {valeurs['collection']}"))
    if "nb_chunks" in valeurs and valeurs["nb_chunks"].isdigit():
        if int(valeurs["nb_chunks"]) != len(titres):
            anomalies.append((nom, "nb_chunks", f"déclaré {valeurs['nb_chunks']}, réel {len(titres)}"))

    return len(titres)


# --------------------------------------------------------------------------- #
# Contrôles sur les générateurs
# --------------------------------------------------------------------------- #

def libelle_du_generateur(source):
    """Reconstitue le préfixe de titre que le générateur produirait.

    Trois écritures coexistent dans le projet : une constante PREFIXE littérale,
    une f-string GAMME + DESIGNATION, ou un libellé écrit en dur dans les titres.
    """
    m = re.search(r'^PREFIXE(?:_CA)?\s*=\s*"([^"{]+)"', source, flags=re.M)
    if m:
        return m.group(1).replace("—", "").strip()

    gm = re.search(r'^GAMME\s*=\s*"([^"]+)"', source, flags=re.M)
    de = re.search(r'^(?:DESIGNATION|GAMME_NOM)\s*=\s*"([^"]+)"', source, flags=re.M)
    if gm and de:
        return f"{gm.group(1)} {de.group(1)}"

    m = re.search(r'"##?\s*([A-Z][A-Za-z0-9 ]+ [^—"{]+?)\s*—', source)
    return m.group(1).strip() if m else None


def bloc_front_matter_generateur(source):
    i = source.find("fm = [")
    if i >= 0:
        j = source.find('"---",', source.find('"---",', i) + 5)
        return source[i:j] if j > 0 else source[i:i + 1500]
    m = re.search(r'"---\\n"', source)
    if m:
        j = source.find('"---\\n\\n"', m.end())
        return source[m.start(): j if j > 0 else m.end() + 1500]
    return ""


def controler_generateur(chemin, code, libelle, anomalies):
    nom = os.path.basename(chemin)
    src = lire(chemin)

    emis = libelle_du_generateur(src)
    attendu = f"{code} {libelle}"
    if emis is None:
        anomalies.append((nom, "générateur", "libellé de gamme illisible dans le script"))
    elif emis != attendu:
        anomalies.append((nom, "générateur", f"produirait « {emis} » au lieu de « {attendu} »"))

    bloc = bloc_front_matter_generateur(src)
    if not bloc:
        anomalies.append((nom, "générateur", "bloc de front matter introuvable"))
        return

    champs = [c for c in re.findall(r"(\w+):", re.sub(r"\{[^}]*\}", "", bloc))
              if c in CANON_TARIF or c in BANNIS]
    presents_bannis = [c for c in champs if c in BANNIS]
    if presents_bannis:
        anomalies.append((nom, "générateur", "émet un champ retiré : " + ", ".join(presents_bannis)))

    manquants = [c for c in CANON_TARIF if c not in champs and c not in CONDITIONNELS]
    if manquants:
        anomalies.append((nom, "générateur", "n'émet pas : " + ", ".join(manquants)))

    utiles = [c for c in champs if c in CANON_TARIF]
    if utiles != [c for c in CANON_TARIF if c in utiles]:
        anomalies.append((nom, "générateur", "ordre des champs non conforme"))

    if "glossaire_ref" in src:
        anomalies.append((nom, "générateur", "référence encore glossaire_ref"))


# --------------------------------------------------------------------------- #
# Programme principal
# --------------------------------------------------------------------------- #

def main():
    rep = sys.argv[1] if len(sys.argv) > 1 else "/mnt/project"
    ref = referentiel(rep)

    par_gamme = collections.defaultdict(lambda: {"fichiers": 0, "chunks": 0, "anomalies": []})
    orphelins = []

    # Documentation produit
    for chemin in sorted(glob.glob(os.path.join(rep, "FIP_*.md"))
                         + glob.glob(os.path.join(rep, "CABP_*.md"))
                         + glob.glob(os.path.join(rep, "FE_*.md"))):
        nom = os.path.basename(chemin)
        cle = nom.split("_", 1)[1].rsplit("_", 1)[0]
        if cle not in ref:
            orphelins.append(nom)
            continue
        code, libelle = ref[cle]
        e = par_gamme[cle]
        e["chunks"] += controler_fichier(chemin, code, libelle, CANON_DOC, e["anomalies"])
        e["fichiers"] += 1

    # Tarifs
    for chemin in sorted(glob.glob(os.path.join(rep, "Tarif_*.md"))):
        nom = os.path.basename(chemin)
        cle = gamme_de_fichier_tarif(nom)
        if cle not in ref:
            orphelins.append(nom)
            continue
        code, libelle = ref[cle]
        e = par_gamme[cle]
        e["chunks"] += controler_fichier(chemin, code, libelle, CANON_TARIF, e["anomalies"])
        e["fichiers"] += 1

    # Générateurs
    for chemin in sorted(glob.glob(os.path.join(rep, "generateur_tarif_*.py"))):
        cle = os.path.basename(chemin)[len("generateur_tarif_"):-len(".py")]
        if cle not in ref:
            orphelins.append(os.path.basename(chemin))
            continue
        code, libelle = ref[cle]
        controler_generateur(chemin, code, libelle, par_gamme[cle]["anomalies"])

    # Rapport
    print("=" * 78)
    print("CONTRÔLE CROISÉ — libellés de gamme et front matter")
    print(f"Répertoire : {rep}")
    print(f"Référentiel : Instructions_V03.md ({len(ref)} gammes déclarées)")
    print("=" * 78)
    print(f"\n{'GAMME':<13}{'FICHIERS':>9}{'CHUNKS':>8}{'ANOMALIES':>11}")

    total_anomalies = 0
    for cle in sorted(par_gamme):
        e = par_gamme[cle]
        total_anomalies += len(e["anomalies"])
        etat = "—" if not e["anomalies"] else str(len(e["anomalies"]))
        print(f"{cle:<13}{e['fichiers']:>9}{e['chunks']:>8}{etat:>11}")

    for cle in sorted(par_gamme):
        if not par_gamme[cle]["anomalies"]:
            continue
        print(f"\n--- {cle} " + "-" * (72 - len(cle)))
        for nom, categorie, detail in par_gamme[cle]["anomalies"]:
            print(f"   [{categorie}] {nom}")
            print(f"       {detail}")

    if orphelins:
        print("\n--- fichiers non rattachés à une gamme du référentiel " + "-" * 22)
        for nom in orphelins:
            print(f"   {nom}")

    print("\n" + "=" * 78)
    if total_anomalies or orphelins:
        print(f"ANOMALIES : {total_anomalies}"
              + (f" | fichiers orphelins : {len(orphelins)}" if orphelins else ""))
        return 1
    print("CONFORME — aucune anomalie.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
