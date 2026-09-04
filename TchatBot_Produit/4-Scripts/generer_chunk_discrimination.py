#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
generer_chunk_discrimination.py

Outil ASSISTÉ de rédaction d'un « chunk de discrimination inter-gammes ».

Rôle et limites (à lire avant usage)
------------------------------------
Ce script N'INVENTE PAS et NE DÉCIDE PAS seul. Il encadre et sécurise l'ajout
d'un chunk de discrimination qui fait suite à un CONTEXTE DOCUMENTÉ (une
question posée + la mauvaise réponse obtenue). Le déclenchement (constater
qu'un cas résiste) et le contenu rédactionnel restent du ressort humain, après
diagnostic. Le script :

  1. exige un contexte documenté (question + réponse fautive + preuve d'origine) ;
  2. vérifie objectivement, dans les fichiers .md du corpus, que la
     caractéristique litigieuse est ABSENTE de la gamme interrogée et PRÉSENTE
     chez la gamme voisine désignée ;
  3. impose le gabarit du chunk (titre préfixé du code gamme, ligne source,
     identifiant SC, plafond de mots) ;
  4. effectue les contrôles de conformité (doublon, contradiction, format) ;
  5. génère le chunk + une entrée de traçabilité reliant le chunk à sa preuve.

Si une condition de sécurité n'est pas remplie, le script REFUSE de produire et
explique pourquoi. C'est volontaire : il vaut mieux un refus qu'un chunk négatif
hasardeux.

Usage
-----
    python generer_chunk_discrimination.py --contexte contexte.json --corpus /chemin/vers/corpus

Le fichier de contexte (JSON) est rempli par la personne qui a diagnostiqué le
cas. Voir le gabarit imprimé par :

    python generer_chunk_discrimination.py --gabarit
"""

import argparse
import json
import os
import re
import sys
from datetime import date


PLAFOND_MOTS = 200          # plafond de mots par chunk (titre + source + corps)
SC_RESERVE = "SC0001"       # réservé au résumé Wikit ; les chunks commencent à SC0002


# --------------------------------------------------------------------------- #
# Gabarit du fichier de contexte documenté
# --------------------------------------------------------------------------- #

GABARIT_CONTEXTE = {
    "preuve_origine": {
        "question_posee": "Sur la H81 Access, les crochets massifs sont-ils en acier ou en inox ?",
        "reponse_fautive": "... la serrure 6 points (deux crochets massifs, ...) ...",
        "reference_preuve": "capture conversation Wikit 2026-07-21 #A / signalement Aurélien",
        "date_constat": "2026-07-21"
    },
    "gamme_interrogee": {
        "code": "H81 Access",
        # Libellé de gamme, identique à celui des autres chunks du fichier.
        "libelle": "Porte de service PVC",
        "fichier_md": "FIP_H81_Access_06-2024.md"
    },
    "caracteristique_litigieuse": {
        # terme(s) qui NE DOIT PAS apparaître dans la gamme interrogée
        "termes_absents_attendus": ["crochets massifs", "serrure 6 points"],
        # la gamme voisine qui, elle, possède réellement la caractéristique
        "gamme_voisine_code": "H81",
        "fichier_md_voisine": "FIP_H81_08-2025.md",
        # terme(s) prouvant la présence chez la voisine
        "termes_presents_voisine": ["crochets massifs", "serrure 6 points"]
    },
    "contenu_valide": {
        # Rédigé et validé par l'humain APRÈS diagnostic. Formulation positive
        # privilégiée + renvoi nommé vers la gamme qui possède la caractéristique.
        "titre_court": "Fermeture : 5 points à galets, sans serrure 6 points à crochets",
        "corps": (
            "La porte de service H81 Access est équipée d'une fermeture 5 points "
            "à galets, complétée par une gâche centrale et un cylindre de sécurité "
            "débrayable. Elle ne comporte pas de serrure 6 points à crochets "
            "massifs : ce type de serrure équipe la porte d'entrée H81, gamme "
            "distincte. Pour la H81 Access, aucun crochet massif n'est documenté."
        )
    }
}


# --------------------------------------------------------------------------- #
# Utilitaires
# --------------------------------------------------------------------------- #

def compter_mots(texte: str) -> int:
    """Compte les mots comme les scripts de conformité du projet (\\S+)."""
    return len(re.findall(r"\S+", texte))


def lire_fichier(chemin: str) -> str:
    with open(chemin, "r", encoding="utf-8") as f:
        return f.read()


def extraire_gamme_code_du_front_matter(contenu: str) -> str | None:
    m = re.search(r"^gamme_code:\s*(.+)$", contenu, flags=re.MULTILINE)
    return m.group(1).strip() if m else None


def extraire_document_source(contenu: str) -> str | None:
    m = re.search(r"^document_source:\s*(.+)$", contenu, flags=re.MULTILINE)
    return m.group(1).strip() if m else None


def dernier_identifiant_sc(contenu: str) -> int:
    """Renvoie le plus grand numéro SCxxxx présent, 1 si aucun (SC0001 réservé)."""
    nums = [int(x) for x in re.findall(r"SC(\d{4})", contenu)]
    return max(nums) if nums else 1


def terme_present(contenu: str, terme: str) -> bool:
    """Recherche insensible à la casse, sur le corps (hors lignes de titre/source
    qui énoncent parfois un bannissement). Ici on cherche la présence réelle."""
    return re.search(re.escape(terme), contenu, flags=re.IGNORECASE) is not None


# --------------------------------------------------------------------------- #
# Contrôles de sécurité — le cœur du dispositif
# --------------------------------------------------------------------------- #

class RefusProduction(Exception):
    """Levée dès qu'une condition de sécurité n'est pas remplie."""


def valider_contexte(ctx: dict) -> None:
    """Vérifie que le contexte documenté est complet. Sans preuve, pas de chunk."""
    manquants = []

    preuve = ctx.get("preuve_origine", {})
    for champ in ("question_posee", "reponse_fautive", "reference_preuve", "date_constat"):
        if not preuve.get(champ):
            manquants.append(f"preuve_origine.{champ}")

    gi = ctx.get("gamme_interrogee", {})
    for champ in ("code", "libelle", "fichier_md"):
        if not gi.get(champ):
            manquants.append(f"gamme_interrogee.{champ}")

    cl = ctx.get("caracteristique_litigieuse", {})
    for champ in ("termes_absents_attendus", "gamme_voisine_code",
                  "fichier_md_voisine", "termes_presents_voisine"):
        if not cl.get(champ):
            manquants.append(f"caracteristique_litigieuse.{champ}")

    cv = ctx.get("contenu_valide", {})
    for champ in ("titre_court", "corps"):
        if not cv.get(champ):
            manquants.append(f"contenu_valide.{champ}")

    if manquants:
        raise RefusProduction(
            "Contexte documenté incomplet. Champs manquants :\n  - "
            + "\n  - ".join(manquants)
            + "\n\nUn chunk de discrimination ne peut être créé sans preuve "
              "d'origine (question + réponse fautive) et sans diagnostic validé."
        )


def verifier_accentuation(ctx: dict) -> None:
    """Refuse un contenu saisi sans accents.

    Un chunk désaccentué est partiellement invisible au retrieval : sur un moteur
    sémantique, « fermee » et « fermée » ne produisent pas le même token. Le cas
    s'est produit sur le chunk SC0028 de la H81 Access, resté sans accent alors
    que tout le reste du fichier en portait. Le contrôle est heuristique : il
    signale un texte français d'une longueur significative totalement dépourvu de
    caractères accentués, ce qui n'arrive pas dans une rédaction normale.
    """
    cv = ctx["contenu_valide"]
    for champ in ("titre_court", "corps"):
        texte = cv[champ]
        if len(texte) < 60:
            continue
        if not re.search(r"[àâäçéèêëîïôöùûüÀÂÄÇÉÈÊËÎÏÔÖÙÛÜ]", texte):
            raise RefusProduction(
                f"ACCENTUATION MANQUANTE dans contenu_valide.{champ} : "
                f"{len(texte)} caractères sans aucun accent. Un texte français "
                "de cette longueur en comporte nécessairement. Un chunk "
                "désaccentué est mal récupéré par le moteur. Ressaisir le texte "
                "avec ses accents."
            )


def verifier_absence_dans_gamme(corpus: str, ctx: dict) -> None:
    """La caractéristique DOIT être absente de la gamme interrogée."""
    cl = ctx["caracteristique_litigieuse"]
    chemin = os.path.join(corpus, ctx["gamme_interrogee"]["fichier_md"])
    if not os.path.isfile(chemin):
        raise RefusProduction(f"Fichier gamme interrogée introuvable : {chemin}")

    contenu = lire_fichier(chemin)
    presents_a_tort = [t for t in cl["termes_absents_attendus"] if terme_present(contenu, t)]
    if presents_a_tort:
        raise RefusProduction(
            "ABSENCE NON VÉRIFIÉE. Les termes suivants, censés être absents de "
            f"la gamme {ctx['gamme_interrogee']['code']}, sont présents dans "
            f"{ctx['gamme_interrogee']['fichier_md']} :\n  - "
            + "\n  - ".join(presents_a_tort)
            + "\n\nCela signifie soit que le corpus contient encore la "
              "contamination (à corriger d'abord), soit que la caractéristique "
              "existe réellement sur cette gamme (auquel cas un chunk de "
              "discrimination NIANT sa présence serait FAUX). Diagnostic à revoir."
        )


def verifier_presence_chez_voisine(corpus: str, ctx: dict) -> None:
    """La caractéristique DOIT être présente chez la gamme voisine désignée.
    C'est ce qui justifie la discrimination : sans voisine porteuse, l'absence
    n'est pas source de confusion inter-gammes et le chunk n'est pas justifié."""
    cl = ctx["caracteristique_litigieuse"]
    chemin = os.path.join(corpus, cl["fichier_md_voisine"])
    if not os.path.isfile(chemin):
        raise RefusProduction(f"Fichier gamme voisine introuvable : {chemin}")

    contenu = lire_fichier(chemin)
    absents = [t for t in cl["termes_presents_voisine"] if not terme_present(contenu, t)]
    if absents:
        raise RefusProduction(
            "PRÉSENCE CHEZ LA VOISINE NON VÉRIFIÉE. Les termes suivants, censés "
            f"prouver la présence chez {cl['gamme_voisine_code']}, sont absents "
            f"de {cl['fichier_md_voisine']} :\n  - " + "\n  - ".join(absents)
            + "\n\nSi la caractéristique n'existe pas non plus chez une voisine, "
              "l'absence n'est pas génératrice de confusion inter-gammes : un "
              "chunk de discrimination n'est pas le bon remède (le modèle rejette "
              "généralement bien ces cas). Diagnostic à revoir."
        )


def verifier_pas_de_doublon(corpus: str, ctx: dict) -> None:
    """Évite d'ajouter un chunk de discrimination déjà présent."""
    chemin = os.path.join(corpus, ctx["gamme_interrogee"]["fichier_md"])
    contenu = lire_fichier(chemin)
    titre = ctx["contenu_valide"]["titre_court"].lower()
    for ligne in contenu.splitlines():
        if ligne.startswith("## ") and titre in ligne.lower():
            raise RefusProduction(
                f"DOUBLON PROBABLE. Un chunk au titre proche existe déjà :\n  {ligne}"
            )


def verifier_plafond_mots(chunk: str) -> None:
    n = compter_mots(chunk)
    if n > PLAFOND_MOTS:
        raise RefusProduction(
            f"PLAFOND DÉPASSÉ : {n} mots (max {PLAFOND_MOTS}). Raccourcir le corps."
        )


# --------------------------------------------------------------------------- #
# Génération du chunk et de la traçabilité
# --------------------------------------------------------------------------- #

def construire_ligne_source(ctx: dict, sc_id: str) -> str:
    """Ligne source adaptée au chunk de discrimination.

    Un chunk de discrimination ne provient pas d'une page de PDF : il est déduit
    d'une ABSENCE, motivée par une preuve documentée. On le marque explicitement
    comme tel — « chunk de discrimination » — plutôt que de forger un faux numéro
    de page. Le nom de gamme reste affiché en em-dashes, conformément à la
    convention des lignes source du projet.
    """
    code_affiche = ctx["gamme_interrogee"]["code"].replace(" ", "—")
    ref = ctx["preuve_origine"]["reference_preuve"]
    return (f"*Source : discrimination inter-gammes {code_affiche} — "
            f"information complémentaire — {sc_id} — motivé par : {ref}*")


def construire_chunk(ctx: dict, sc_id: str) -> str:
    gi = ctx["gamme_interrogee"]
    cv = ctx["contenu_valide"]
    # Titre préfixé du LIBELLÉ COMPLET de gamme (auto-discrimination), comme tout
    # chunk du corpus : « CODE Libellé — sujet ». Le seul code ne suffit pas, il
    # rend le chunk moins récupérable que ses voisins et le sort du gabarit.
    titre = f"## {gi['code']} {gi['libelle']} — {cv['titre_court']}"
    source = construire_ligne_source(ctx, sc_id)
    corps = cv["corps"].strip()
    return f"{titre}\n{source}\n\n{corps}\n"


def construire_entree_tracabilite(ctx: dict, sc_id: str) -> dict:
    return {
        "sc_id": sc_id,
        "gamme": ctx["gamme_interrogee"]["code"],
        "fichier_md": ctx["gamme_interrogee"]["fichier_md"],
        "type": "chunk_discrimination_inter_gammes",
        "caracteristique_niee": ctx["caracteristique_litigieuse"]["termes_absents_attendus"],
        "gamme_voisine_porteuse": ctx["caracteristique_litigieuse"]["gamme_voisine_code"],
        "preuve_origine": ctx["preuve_origine"],
        "date_generation": date.today().isoformat(),
    }


# --------------------------------------------------------------------------- #
# Orchestration
# --------------------------------------------------------------------------- #

def generer(contexte_path: str, corpus: str, dry_run: bool = True) -> None:
    ctx = json.loads(lire_fichier(contexte_path))

    # 1. Le contexte documenté est-il complet ? (preuve d'origine obligatoire)
    valider_contexte(ctx)

    verifier_accentuation(ctx)
    # 2. Vérifications objectives dans le corpus
    verifier_absence_dans_gamme(corpus, ctx)
    verifier_presence_chez_voisine(corpus, ctx)
    verifier_pas_de_doublon(corpus, ctx)

    # 3. Identifiant SC suivant, à partir du fichier cible
    chemin_cible = os.path.join(corpus, ctx["gamme_interrogee"]["fichier_md"])
    prochain = dernier_identifiant_sc(lire_fichier(chemin_cible)) + 1
    sc_id = f"SC{prochain:04d}"

    # 4. Construction et contrôle de conformité (plafond)
    chunk = construire_chunk(ctx, sc_id)
    verifier_plafond_mots(chunk)

    tracabilite = construire_entree_tracabilite(ctx, sc_id)

    # 5. Sortie
    print("=" * 70)
    print("CHUNK DE DISCRIMINATION — PROPOSITION (à valider avant intégration)")
    print("=" * 70)
    print(chunk)
    print("-" * 70)
    print(f"Mots : {compter_mots(chunk)} / {PLAFOND_MOTS}")
    print(f"Identifiant attribué : {sc_id}")
    print("-" * 70)
    print("ENTRÉE DE TRAÇABILITÉ :")
    print(json.dumps(tracabilite, ensure_ascii=False, indent=2))
    print("=" * 70)

    if dry_run:
        print("\n[dry-run] Aucun fichier modifié. Relancez avec --appliquer pour "
              "ajouter le chunk en fin de fichier et journaliser la traçabilité.")
        return

    # Ajout en fin de fichier (append), conforme au workflow d'assemblage du projet
    with open(chemin_cible, "a", encoding="utf-8") as f:
        f.write("\n" + chunk)
    journal = os.path.join(corpus, "journal_discrimination.jsonl")
    with open(journal, "a", encoding="utf-8") as f:
        f.write(json.dumps(tracabilite, ensure_ascii=False) + "\n")
    print(f"\n[appliqué] Chunk ajouté à {chemin_cible}")
    print(f"[appliqué] Traçabilité journalisée dans {journal}")


def imprimer_gabarit() -> None:
    print("Gabarit du fichier de contexte documenté (JSON) :\n")
    print(json.dumps(GABARIT_CONTEXTE, ensure_ascii=False, indent=2))
    print("\nRemplir APRÈS diagnostic d'un cas avéré. Les champs 'contenu_valide' "
          "sont rédigés et validés par l'humain ; le script vérifie le reste.")


def main() -> int:
    p = argparse.ArgumentParser(description=__doc__,
                                formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("--contexte", help="Chemin du fichier de contexte documenté (JSON)")
    p.add_argument("--corpus", help="Répertoire du corpus (.md)")
    p.add_argument("--appliquer", action="store_true",
                   help="Applique réellement (par défaut : dry-run)")
    p.add_argument("--gabarit", action="store_true",
                   help="Imprime le gabarit du fichier de contexte et quitte")
    args = p.parse_args()

    if args.gabarit:
        imprimer_gabarit()
        return 0

    if not args.contexte or not args.corpus:
        p.error("--contexte et --corpus sont requis (ou utilisez --gabarit)")

    try:
        generer(args.contexte, args.corpus, dry_run=not args.appliquer)
    except RefusProduction as e:
        print("REFUS DE PRODUCTION\n-------------------", file=sys.stderr)
        print(str(e), file=sys.stderr)
        return 2
    except (FileNotFoundError, json.JSONDecodeError) as e:
        print(f"Erreur d'entrée : {e}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
