#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Générateur de chunks Markdown pour le tarif TA76 OC
(fenêtre aluminium à ouvrant caché, collection TRYBA ALUMINIUM).

Conforme à note_cadrage_migration_tarif_TA76_OC.md : règles T1 à T7 héritées de
T81 et amendées, plus les règles propres OC1 (chapitre à saisie sous la gamme
jumelle), OC2 (pourcentage adossé aux grilles) et OC3 (hiérarchie des sources).

Six fichiers :
  F1 METHODE          cotes, lecture par bandes, vocabulaire        (règle T3)
  F2 PRIX_CHASSIS     toutes les grilles, y compris à un seul axe   (T1, T2)
  F3 OPTIONS          plus-values forfaitaires                      (règle T4)
  F4 CHASSIS_SPECIAUX pages 65-66, à saisir en TA76 OV              (T5, OC1)
  F5 FAISABILITES     restrictions, sans aucun montant              (règle T6)
  F6 TRANSVERSES      orientation sans montant ni pourcentage       (T7, OC2)

Principes : fidélité numérique (toute valeur recopiée de la cellule, jamais
calculée), anti-fantôme, SC continue par fichier depuis SC0002, ligne de source
normée, plafond de 200 mots marqueur compris, journal exhaustif.
"""
import os
import re
import sys
from collections import OrderedDict, defaultdict

import openpyxl

XLSX = "/mnt/user-data/uploads/TA76_OC-infos-tarifs.xlsx"
FEUILLE = "Feuil1"
PDF_SOURCE = "Tarif—TA76_OC—HT—19-06-2026.pdf"   # nom affiché dans la ligne de source
PDF_YAML = "Tarif_TA76_OC_HT_19-06-2026.pdf"     # nom dans le front matter
OUTDIR = "/mnt/user-data/outputs"
PLAFOND = 200
GAMME = "TA76 OC"
DESIGNATION = "Fenêtre Aluminium à ouvrant caché"
PREFIXE = f"{GAMME} {DESIGNATION} — "
PRODUIT = "fenêtre aluminium à ouvrant caché TA76 OC"

# ============================================================ index de colonnes
C_CHAP, C_TAB, C_GAMME, C_CLE, C_DES, C_DET = 0, 1, 2, 3, 4, 5
C_HT, C_TTC = 8, 9
C_HAUTEUR = 16
COLS_HT_L = list(range(17, 77))     # Px L 100..6000 HT
COLS_TTC_L = list(range(77, 137))   # Px L 100..6000 TTC
NCOLS = 137

ALERTS, JOURNAL = [], []

# ============================================================ table des pages
# Établie contre les EN-TÊTES DE PAGE du PDF. Le sommaire général de la page 3
# porte une pagination périmée (décalage de 4 pages à partir du chapitre
# OPTIONS) et n'est PAS utilisé ; les intercalaires de section non plus.
# L'audit revérifie chaque attribution en cherchant le montant sur la page citée.
PAGES = {
    ("1 OF", None): 10,
    ("2 OF", None): 11,
    ("Châssis fixes", None): 12,
    ("Châssis à soufflet", "SN"): 13,
    ("Châssis à soufflet", "SN avec poignée"): 13,
    ("Châssis à soufflet", "SA"): 13,
    ("Laquage bloc-baie", "Forfait"): 20,
    ("Vitrage-généralités", "Vitrage d'altitude"): 23,
    ("PV vitrages", None): 24,
    ("PV vitrages ornementaux", None): 25,
    ("PV composition libre", None): 26,
    ("Remplissage", "PV soubassement"): 27,
    ("Remplissage", "panneaux moulurés"): 27,
    ("Calcul croisillons intégrés", "Croisillons en alu laqué"): 28,
    ("Croisillons intégrés+grecque", "Croisillons en alu 10 mm"): 29,
    ("Croisillons intégrés+grecque", 'Forfait "grecque"'): 29,
    ("Croisillons Art Déco", "/ champ"): 30,
    ("Croisillons Art Déco", "/ volume"): 30,
    ("Croisillons rapportés", "2F en alu"): 31,
    ("Habillage Alu sur vitrage", "habillage rectangulaire"): 32,
    ("Habillage Alu sur vitrage", "habillage cintrés"): 33,
    ("Poignées", "En option"): 35,
    ("Crémones à l'ancienne", "prix et finitions"): 36,
    ("Meneaux complémentaires", "Meneaux battants"): 38,
    ("Grilles d'entrée d'air", None): 41,
    ("Grilles d'air spé Belgique", None): 42,
    ("Chatière", None): 43,
    ("Ferrage R20", "PV options"): 46,
    ("Ferrage SA R20", "PV tringle"): 49,
    ("Ferrage SA R20", "PV pour guide supp"): 49,
    ("Ferrage SA R20", "PV selon long flexible"): 49,
    ("Ferrage SA R20", "PV pour compas de sécu"): 50,
    ("Ferrage SA R20", "Cmd spéciales pour SA"): 50,
    ("Ferrage porte-fenêtre R20", "Ensemble serrure SB"): 51,
    ("Ferrage porte-fenêtre R20", "Ensemble serrure CC"): 51,
    ("Ferrage porte-fenêtre R20", "Seuil universel"): 51,
    ("Ferrage porte-fenêtre R20", "Poignée de tirage"): 51,
    ("Pack Trybadesign", "Ferrage invisible"): 52,
    ("Pièce d'appui et élargisseurs", "Pièces d'appui"): 57,
    ("Pièce d'appui et élargisseurs", "Elargisseurs de seuil"): 57,
    ("Pièce d'appui et élargisseurs", "Profilés complémentaires"): 57,
    ("Elargisseurs, tapées", "U d'assemblage"): 58,
    ("Elargisseurs, tapées", "Elargisseurs"): 58,
    ("Elargisseurs, tapées", "Tapées de doublage"): 59,
    ("Bavettes ext", None): 60,
    ("Couvre_joints", "couvre-joints"): 61,
    ("Couvre_joints", "couvre-joints spécial réno"): 61,
    ("Seuil", "Seuil"): 62,
    ("Châssis spé-Tarification", None): 65,
    ("Croisillons", None): 66,
}
# surcharge par désignation, quand un chapitre s'étale sur deux pages
PAGES_DES = {
    ("Ferrage R20", "PV options", "PV rabaisser poignée"): 47,
}

# ============================================================ unités (règle T4)
# L'unité de facturation n'existe QUE dans le PDF. Table relevée page par page.
# Clé à trois niveaux : (chap, tab, des), puis (chap, tab, None), puis
# (chap, None, None). Une entrée absente => unité non établie : le chunk le dit
# et renvoie à la page plutôt que de servir un montant nu.
UNITES = {
    ("Laquage bloc-baie", "Forfait", None): ("forfaitaire", "par volet roulant"),
    ("PV vitrages", None, None): ("m2", "par mètre carré de surface vitrée du châssis"),
    ("PV vitrages ornementaux", None, None): ("m2", "par mètre carré de surface vitrée du châssis"),
    ("PV composition libre", None, None): ("m2", "par mètre carré de surface vitrée du châssis"),
    ("Remplissage", "PV soubassement", None): ("m2", "par mètre carré de panneau"),
    ("Remplissage", "panneaux moulurés", None): ("m2", "par mètre carré de panneau"),
    ("Calcul croisillons intégrés", None, None): ("champ", "par champ"),
    ("Croisillons intégrés+grecque", "Croisillons en alu 10 mm", None): ("champ", "par champ"),
    ("Croisillons intégrés+grecque", 'Forfait "grecque"', None):
        ("forfaitaire", "par châssis, les quatre angles compris"),
    ("Croisillons Art Déco", "/ champ", None): ("champ", "par champ"),
    ("Croisillons Art Déco", "/ volume", None): ("volume", "par volume"),
    ("Croisillons rapportés", None, None): ("champ", "par champ"),
    ("Meneaux complémentaires", "Meneaux battants", "Prix HT"):
        ("ml", "par mètre linéaire de longueur réelle, mesurée en fond de feuillure"),
    ("Meneaux complémentaires", "Meneaux battants",
     "Plus-value pour 2 fixations à angle droit"):
        ("forfaitaire", "par meneau, pour la fixation de ses deux extrémités"),
    ("Grilles d'entrée d'air", None, None): ("ensemble", "pour l'ensemble"),
    ("Ferrage R20", "PV options", None): ("piece", "par pièce"),
    ("Ferrage SA R20", "PV tringle", None): ("ml", "par mètre linéaire"),
    ("Ferrage SA R20", "PV pour guide supp", None): ("sachet", "par sachet de dix pièces"),
    ("Ferrage SA R20", "PV selon long flexible", None): ("piece", "par pièce"),
    ("Ferrage SA R20", "PV pour compas de sécu", None): ("piece", "par pièce"),
    ("Ferrage SA R20", "Cmd spéciales pour SA", "F25"): ("piece", "par pièce"),
    ("Ferrage SA R20", "Cmd spéciales pour SA", "CEFI"): ("ensemble", "pour l'ensemble"),
    ("Ferrage porte-fenêtre R20", "Ensemble serrure SB", None): ("ensemble", "pour l'ensemble"),
    ("Ferrage porte-fenêtre R20", "Ensemble serrure CC", None): ("ensemble", "pour l'ensemble"),
    ("Ferrage porte-fenêtre R20", "Seuil universel", None): ("piece", "par pièce"),
    ("Ferrage porte-fenêtre R20", "Poignée de tirage", None):
        ("forfaitaire", "posé sur châssis"),
    ("Pack Trybadesign", "Ferrage invisible", None): ("vantail", "par vantail"),
    ("Pièce d'appui et élargisseurs", "Pièces d'appui", None):
        ("ml", "par mètre linéaire, posé sur châssis"),
    ("Pièce d'appui et élargisseurs", "Elargisseurs de seuil", None):
        ("ml", "par mètre linéaire, posé sur châssis"),
    ("Pièce d'appui et élargisseurs", "Profilés complémentaires", None):
        ("m2", "par mètre carré, posé sur châssis"),
    ("Elargisseurs, tapées", None, None): ("ml", "par mètre linéaire, coupé sur mesure"),
    ("Bavettes ext", None, None): ("ml", "par mètre linéaire, coupé sur mesure"),
    ("Couvre_joints", None, None): ("ml", "par mètre linéaire, coupé sur mesure"),
    ("Seuil", "Seuil", "5263"): ("ml", "par mètre linéaire"),
    ("Seuil", "Seuil", None): ("piece", "par pièce"),
    ("Châssis spé-Tarification", None, "Triangle et trapèze"):
        ("chassis", "par châssis, vitrage non compris"),
    ("Châssis spé-Tarification", None, "Meneau/traverse"): ("fixation", "par fixation"),
    ("Croisillons", None, "Champs"): ("champ", "par champ"),
    ("Croisillons", None, "Croisillons sur partie cintrée"): ("fixation", "par fixation"),
    ("Croisillons", None, None): ("unite", "par unité"),
    # unités NON établies : le tarif ne les énonce pas sur la page
    ("Poignées", None, None): (None, None),
    ("Crémones à l'ancienne", None, None): (None, None),
    ("Chatière", None, None): (None, None),
    ("Vitrage-généralités", None, None): (None, None),
    ("Ferrage R20", "PV options", "PV rabaisser poignée"): (None, None),
}

# surface minimale de facturation, énoncée pages 24 à 27
SURFACE_MINI = {"PV vitrages", "PV vitrages ornementaux", "PV composition libre",
                "Remplissage"}

# ============================================================ arbitrages rendus
# Lignes Excel exclues de la génération, avec leur motif. Aucune n'est écartée
# en silence : chacune ressort au journal.
EXCLUSIONS = {
    283: "HF2245_CE (105 € HT) : référence absente du PDF, aucune occurrence dans "
         "les 70 pages — montant sans source, non généré (non-invention)",
    298: "Judas optique (47 € HT) : le mot « judas » n'apparaît nulle part dans le "
         "tarif TA76 OC ; la référence existe sur H81 — contamination inter-gammes "
         "probable, non généré",
    189: "Croisillon I45 en T ou croix à 49 € HT : le PDF page 28 ne porte qu'un "
         "seul prix pour I45 (36 €) — montant sans source ni discriminant, non généré",
    191: "Croisillon I45 filant à 38 € HT : le PDF page 28 ne porte qu'un seul prix "
         "pour I45 (24 €) — montant sans source ni discriminant, non généré",
    170: "Panneau phonique en 36 mm : 0 € dans l'Excel, cellule vide au PDF page 27 "
         "— configuration non tarifée, non générée (anti-fantôme). L'absence est "
         "exposée dans le chunk du panneau phonique en 28 mm",
    210: "Motif Art Déco MG9, gravure transparente sur vitrage transparent : 0 € "
         "dans l'Excel contre une cellule fusionnée au PDF page 30 — lecture "
         "ambiguë, non généré, absence exposée dans le chunk MG9",
    212: "Motif Art Déco MG9, gravure transparente sur vitrage sablé : 0 € dans "
         "l'Excel contre une cellule fusionnée au PDF page 30 — lecture ambiguë, "
         "non généré, absence exposée dans le chunk MG9",
}
CHAP_EXCLUS = {"Exemple de calcul"}   # totaux additionnés, hors corpus

# ============================================================ règle OC3
# Références divergentes entre l'Excel et le PDF. Le PDF fait foi sur le libellé
# et la référence ; le montant reste celui de la cellule.
REF_PDF = {
    "FR15_DV": "FR15", "ME30_DV": "ME30_CE", "ME30+RA_DV": "ME30+RA_CE",
    "FR12_DV": "FR12", "ISOLA2-45_DV": "ISOLA2-45_CE",
    "ISOLA245+RA_DV": "ISOLA245+RA_CE", "ISOLA-HY_DV": "ISOLA-HY_CE",
    "ISOLA-HY+RA_DV": "ISOLA-HY+RA_CE",
    "AS10101-RA1": "AS10100-RA1", "AK10100-RA2": "AS10100-RA2",
    "AK10123": "AS10100", "AK10255": "AK10131",
}
# Divergences de montant entre deux pages du PDF pour un même produit.
# Aucune n'est retenue contre l'autre : chaque chunk expose l'existence de l'autre.
DIVERGENCES = {
    317: "Le tarif porte un second montant pour ce même seuil AS10100, de 188 € HT "
         "et 269 € TTC la pièce, page 62. Les deux valeurs figurent au tarif et "
         "aucune ne prévaut sur l'autre ; la divergence doit être arbitrée par le "
         "service produits.",
    350: "Le tarif porte un second montant pour ce même seuil AS10100, de 196 € HT "
         "et 280 € TTC la pièce, page 51. Les deux valeurs figurent au tarif et "
         "aucune ne prévaut sur l'autre ; la divergence doit être arbitrée par le "
         "service produits.",
    318: "Le tarif porte un second profil plinthe sur seuil pour fixe latéral, "
         "référencé 5120SN, à 96 € HT et 138 € TTC la pièce, page 62. Les deux "
         "postes figurent au tarif et aucune ne prévaut sur l'autre ; la divergence "
         "doit être arbitrée par le service produits.",
    349: "Le tarif porte un second profil plinthe sur seuil pour fixe latéral, "
         "référencé AK10131, à 99 € HT et 142 € TTC la pièce, page 51. Les deux "
         "postes figurent au tarif et aucune ne prévaut sur l'autre ; la divergence "
         "doit être arbitrée par le service produits.",
}

# ============================================================ grilles (règle T1)
# (chapitre, tableau) -> (libellé long, synonyme d'usage)
GRILLES_2D = OrderedDict([
    (("1 OF", None),
     ("châssis à 1 ouvrant à la française",
      "fenêtre à un vantail ouvrant à la française")),
    (("2 OF", None),
     ("châssis à 2 ouvrants égaux à la française",
      "fenêtre à deux vantaux ouvrants à la française")),
    (("Châssis fixes", None), ("châssis fixe", "fenêtre fixe sans ouvrant")),
    (("Châssis à soufflet", "SN"),
     ("châssis à soufflet normal (SN)", "fenêtre à soufflet à poignée standard")),
    (("Châssis à soufflet", "SN avec poignée"),
     ("châssis à soufflet normal à poignée latérale (SN)",
      "fenêtre à soufflet à commande latérale")),
    (("Châssis à soufflet", "SA"),
     ("châssis à soufflet d'aération avec ferme-imposte (SA)",
      "fenêtre à soufflet d'aération à ferme-imposte")),
    (("Habillage Alu sur vitrage", "habillage rectangulaire"),
     ("habillage alu rectangulaire sur vitrage",
      "plaque d'habillage rectangulaire posée sur le double vitrage")),
    (("Habillage Alu sur vitrage", "habillage cintrés"),
     ("habillage alu cintré sur vitrage",
      "plaque d'habillage cintrée posée sur le double vitrage")),
])
# grilles à un seul axe (amendement OC de la règle T1)
GRILLES_1D = OrderedDict([
    (("Grilles d'air spé Belgique", "Invisivent EVO sur châssis blanc"),
     ("grille d'entrée d'air Invisivent EVO sur châssis blanc",
      "entrée d'air Invisivent des spécificités Belgique")),
    (("Grilles d'air spé Belgique", "Invisivent EVO sur châssis autre couleur"),
     ("grille d'entrée d'air Invisivent EVO sur châssis d'une autre couleur",
      "entrée d'air Invisivent des spécificités Belgique")),
    (("Grilles d'air spé Belgique", "THM90 EVO sur châssis blanc"),
     ("grille d'entrée d'air THM90 EVO sur châssis blanc",
      "entrée d'air THM90 des spécificités Belgique")),
    (("Grilles d'air spé Belgique", "THM90 EVO sur châssis autre couleur"),
     ("grille d'entrée d'air THM90 EVO sur châssis d'une autre couleur",
      "entrée d'air THM90 des spécificités Belgique")),
])
CHAP_GRILLE = {"1 OF", "2 OF", "Châssis fixes", "Châssis à soufflet",
               "Habillage Alu sur vitrage", "Grilles d'air spé Belgique"}
CHAP_SPECIAUX = {"Châssis spé-Tarification", "Croisillons"}

# ============================================================ croisillons (OC3)
# Maille par finition : un chunk porte le prix du croisillon en T ou croix ET
# celui du croisillon filant. Le discriminant de finition est absent de l'Excel
# et repris du PDF page 28, rattaché PAR LE MONTANT et non par l'ordre des lignes.
CROISILLONS = [
    # (chap, tab, des_excel, déterminant, finition, ht_T, ht_filant)
    ("Calcul croisillons intégrés", "Croisillons en alu laqué", "I18+26",
     "le", "croisillon intégré en alu laqué blanc RAL 9016, en 18 mm (profil I18) "
     "comme en 26 mm (profil I26)", 18, 13),
    ("Calcul croisillons intégrés", "Croisillons en alu laqué", "I18+26",
     "le", "croisillon intégré en alu laqué RAL, en 18 mm (profil I18) comme en "
     "26 mm (profil I26)", 31, 28),
    ("Calcul croisillons intégrés", "Croisillons en alu laqué", "I18+26",
     "le", "croisillon intégré en alu laqué, finition Chêne d'Or en 18 mm (profil I18) "
     "ou tons bois en 26 mm (profil I26)", 22, 20),
    ("Calcul croisillons intégrés", "Croisillons en alu laqué", "I45",
     "le", "croisillon intégré en alu laqué blanc RAL 9016, en 45 mm (profil I45)",
     36, 24),
    ("Croisillons intégrés+grecque", "Croisillons en alu 10 mm",
     "I10LF\nI10WF\nI10PFF",
     "le", "croisillon intégré en aluminium de 10 mm, teinte laiton (I10LF), blanche "
     "(I10WF) ou plomb foncé (I10PFF)", 22, 19),
    ("Croisillons Art Déco", "/ champ", "Gravure 10 ",
     "la", "gravure Art Déco de 10 mm de largeur sur le verre extérieur", 18, 13),
    ("Croisillons Art Déco", "/ champ", "Gravure 18",
     "la", "gravure Art Déco de 18 mm de largeur sur le verre extérieur", 18, 13),
    ("Croisillons rapportés", "2F en alu", "Grp de couleurs 1 sans PV",
     "le", "croisillon rapporté deux faces en aluminium (référence AK10208), groupe "
     "de couleurs 1 sans plus-value", 28, 21),
    ("Croisillons rapportés", "2F en alu", "Grp de couleurs 2",
     "le", "croisillon rapporté deux faces en aluminium (référence AK10208), groupe "
     "pour un châssis du groupe de couleurs 2", 32, 24),
    ("Croisillons rapportés", "2F en alu", "Autres grp de couleurs",
     "le", "croisillon rapporté deux faces en aluminium (référence AK10208), autres "
     "pour un châssis d'un autre groupe de couleurs", 35, 26),
]
# tableaux traités par la maille « finition » ; le reste du chapitre suit la
# voie générique (le forfait du motif à la grecque, notamment).
TAB_CROISILLONS = {
    ("Calcul croisillons intégrés", "Croisillons en alu laqué"),
    ("Croisillons intégrés+grecque", "Croisillons en alu 10 mm"),
    ("Croisillons Art Déco", "/ champ"),
    ("Croisillons rapportés", "2F en alu"),
}
TAB_ARTDECO_VOLUME = ("Croisillons Art Déco", "/ volume")

# motifs Art Déco au volume : la maille est le MOTIF, le montant étant identique
# pour les trois types de gravure (cellule fusionnée au PDF page 30).
MOTIFS_ARTDECO = [
    ("MG1 à 7 sauf 2", "les",
     "motifs Art Déco MG1 à MG7 hors MG2, gravés en 8 ou 10 mm de largeur", 173, None),
    ("MG2", "le", "motif Art Déco MG2, gravé en 10 mm de largeur", 310, None),
    ("MG8", "le", "motif Art Déco MG8 losanges, gravé en 10 mm de largeur", 243, None),
    ("MGE2B et MGE3B", "les",
     "motifs Art Déco en étoile sans cintre MGE2B et MGE3B, gravés en 10 mm de largeur",
     310, None),
    ("MG9", "le", "motif Art Déco MG9 à la grecque, gravé en 10 mm de largeur", 310,
     "Le tarif ne porte de valeur, pour ce motif, que sur la gravure sablée sur "
     "vitrage transparent : les deux gravures transparentes ne sont pas chiffrées "
     "page 30 et leur lecture doit être arbitrée par le service produits. Ce "
     "montant s'entend par châssis, les quatre angles compris, et non au volume "
     "comme les autres motifs."),
]

# ============================================================ libellés
# Modèle de libellé par (chapitre, tableau). {d} = désignation, {t} = détails.
LIBELLES = {
    ("Laquage bloc-baie", "Forfait"): "Forfait de laquage du volet roulant {desc}",
    ("Vitrage-généralités", "Vitrage d'altitude"): "Plus-value pour vitrage d'altitude",
    ("PV vitrages", None): "Plus-value du vitrage {d}",
    ("PV vitrages ornementaux", None): "Plus-value du vitrage ornemental {d}",
    ("PV composition libre", None): "Plus-value du vitrage en composition libre {d}",
    ("Remplissage", "PV soubassement",
     "Panneau phonique (Rw = 38 dB) Groupe 1 sans PV"):
        "Plus-value du panneau de soubassement phonique en teinte du groupe 1, en {t} mm",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe 1 sans PV"):
        "Plus-value du panneau de soubassement standard en teinte du groupe 1, en {t} mm",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe 2 PV 15 %"):
        "Plus-value du panneau de soubassement standard en teinte du groupe 2, en {t} mm",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe Sublimation"):
        "Plus-value du panneau de soubassement standard en teinte du groupe sublimation, "
        "en {t} mm",
    ("Remplissage", "panneaux moulurés"): "Plus-value du {d} en {t} mm",
    ("Croisillons intégrés+grecque", 'Forfait "grecque"'):
        "Forfait du motif de croisillons à la grecque",
        ("Poignées", "En option", "Poignées à clé"):
        "Tarif de la poignée à clé {t} en option|Tarif des poignées à clé {t} en option",
    ("Poignées", "En option", "Autres poignées"):
        "Tarif de la poignée {t} en option|Tarif des poignées {t} en option",
    ("Crémones à l'ancienne", "prix et finitions"):
        "Tarif de la crémone à l'ancienne, référence {d}",
    ("Meneaux complémentaires", "Meneaux battants", "Prix HT"):
        "Tarif du meneau battant complémentaire",
    ("Meneaux complémentaires", "Meneaux battants",
     "Plus-value pour 2 fixations à angle droit"):
        "Plus-value pour deux fixations à angle droit du meneau battant",
    ("Grilles d'entrée d'air", None): "Tarif de la grille d'entrée d'air {d}",
    ("Chatière", None): "Tarif de la chatière {d}",
    ("Ferrage R20", "PV options", "Entrebaîlleur OF"):
        "Plus-value de l'entrebâilleur en ouverture à la française",
    ("Ferrage R20", "PV options", "OB"): "Plus-value de l'ouverture oscillo-battante",
    ("Ferrage R20", "PV options", "OB inversé"):
        "Plus-value de l'ouverture oscillo-battante inversée",
    ("Ferrage R20", "PV options", "PV rabaisser poignée"):
        "Plus-value pour rabaisser la poignée",
    ("Ferrage SA R20", "PV tringle"):
        "Plus-value de la tringle du soufflet d'aération, référence {d}",
    ("Ferrage SA R20", "PV pour guide supp"):
        "Plus-value du guide supplémentaire du soufflet d'aération ({d})",
    ("Ferrage SA R20", "PV selon long flexible"):
        "Plus-value du renvoi d'angle flexible {d} du soufflet d'aération",
    ("Ferrage SA R20", "PV pour compas de sécu"):
        "Plus-value du compas de sécurité pour soufflet d'aération ({d})",
    ("Ferrage SA R20", "Cmd spéciales pour SA"):
        "Tarif de la commande spéciale {d} pour soufflet d'aération",
    ("Ferrage porte-fenêtre R20", "Ensemble serrure SB"):
        "Tarif de l'ensemble de serrure SB pour porte-fenêtre",
    ("Ferrage porte-fenêtre R20", "Ensemble serrure CC"):
        "Tarif de l'ensemble de serrure CC pour porte-fenêtre",
    ("Ferrage porte-fenêtre R20", "Seuil universel"):
        "Tarif du seuil universel, référence {d}",
    ("Ferrage porte-fenêtre R20", "Poignée de tirage"):
        "Tarif de la poignée de tirage, référence {d}",
    ("Pack Trybadesign", "Ferrage invisible"): "Plus-value du pack Trybadesign",
    ("Pièce d'appui et élargisseurs", "Pièces d'appui"): "Tarif de la pièce d'appui",
    ("Pièce d'appui et élargisseurs", "Elargisseurs de seuil"):
        "Tarif de l'élargisseur de seuil",
    ("Pièce d'appui et élargisseurs", "Profilés complémentaires"):
        "Tarif du panneau élargisseur RVT70",
    ("Seuil", "Seuil"): "Tarif du seuil, référence {d}",
    ("Châssis spé-Tarification", None, "Triangle et trapèze"):
        "Plus-value du châssis fixe spécial de forme triangle ou trapèze",
    ("Châssis spé-Tarification", None, "Meneau/traverse"):
        "Tarif de la fixation spéciale de meneau ou de traverse sur châssis fixe spécial",
    ("Croisillons", None, "Champs"):
        "Tarif du champ de croisillons de châssis fixe spécial",
    ("Croisillons", None, "Etoiles 2 branches"):
        "Tarif de l'étoile de croisillons à deux branches de châssis fixe spécial",
    ("Croisillons", None, "Etoiles 3 branches"):
        "Tarif de l'étoile de croisillons à trois branches de châssis fixe spécial",
    ("Croisillons", None, "Etoile 4 branches"):
        "Tarif de l'étoile de croisillons à quatre branches de châssis fixe spécial",
    ("Croisillons", None, "Toutes connexions en + de 4"):
        "Tarif de chaque connexion de croisillons au-delà de la quatrième",
    ("Croisillons", None, "cintres"):
        "Tarif du cintre de croisillons de châssis fixe spécial",
    ("Croisillons", None, "Demi-lune • de 100 x 50 x 8 mm"):
        "Tarif de la demi-lune de croisillons de 100 x 50 x 8 mm",
    ("Croisillons", None, "Demi-lune • de 130 x 65 x 8 mm"):
        "Tarif de la demi-lune de croisillons de 130 x 65 x 8 mm",
    ("Croisillons", None, "Demi-lune • de 250 x 125 x 8 mm"):
        "Tarif de la demi-lune de croisillons de 250 x 125 x 8 mm",
    ("Croisillons", None, "Croisillons sur partie cintrée"):
        "Tarif du croisillon aboutissant sur une partie cintrée, biaise ou sur angle",
}
# produits dont la désignation Excel n'est qu'un palier de teinte
PRODUITS_TEINTE = {
    ("Elargisseurs, tapées", "U d'assemblage"):
        "U d'assemblage et cache-rainure (références 30806 et 30813)",
    ("Elargisseurs, tapées", "Elargisseurs"):
        "élargisseur et complément d'habillage (références AK10129 et AK10128)",
    ("Elargisseurs, tapées", "Tapées de doublage"):
        "tapée de doublage (références A-T35, A-T46, A-T55, A-T66, A-T86 et A-TCV40) "
        "ou coulisse inversée pour TRYBA VS et VI Evolution (A/CT35, A/CT46, A/CT55 et A/CT66)",
    ("Bavettes ext", None):
        "bavette extérieure (références 30604 B1, 30606 B2, 30608 B3, 30610 B4 et 30612 B5)",
    ("Couvre_joints", "couvre-joints"):
        "couvre-joint intérieur ou extérieur (références 30801 C1, 30812 C3, 30802 C2, "
        "30814 C4, CJ02 et CJ70)",
    ("Couvre_joints", "couvre-joints spécial réno"):
        "couvre-joint spécial rénovation (références 12117 C5, CJ32/42, 30818 C6 et CJ22)",
}
TEINTES = {
    "Prix  Blanc et RAL 7016 Gr": "en blanc ou en RAL 7016 granité",
    "Prix Blanc et RAL 7016 Gr": "en blanc ou en RAL 7016 granité",
    "Prix  Teinte Std Grp1": "en teinte standard du groupe 1",
    "Prix  autre couleur": "dans une autre couleur",
}
# descriptions reprises du PDF pour les postes dont l'Excel ne porte qu'un code
DESC_PDF = {
    ("Grilles d'entrée d'air", "FR15"): "mortaise de 250 x 12 mm pour grille Mini Eséa 30",
    ("Grilles d'entrée d'air", "FR22"): "mortaise de 172 x 12 mm pour grille Mini Eséa 22",
    ("Grilles d'entrée d'air", "FR12"): "mortaise de 354 x 12 mm pour grilles ISOLA2 et ISOLA HY",
    ("Grilles d'entrée d'air", "ME30_CE"):
        "grille Mini Eséa 30 m³/h avec capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ME22_CE"):
        "grille Mini Eséa 22 m³/h avec capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ME30+RA_CE"):
        "grille Mini Eséa 30 m³/h avec rallonge et déflecteur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ME22+RA_CE"):
        "grille Mini Eséa 22 m³/h avec rallonge et capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ISOLA2-45_CE"):
        "grille ISOLA2 de 45 m³/h intérieure avec capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ISOLA245+RA_CE"):
        "grille ISOLA2 de 45 m³/h avec rallonge et capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ISOLA-HY_CE"):
        "grille hygroréglable de 8 à 40 m³/h avec capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ISOLA-HY+RA_CE"):
        "grille hygroréglable de 8 à 40 m³/h avec rallonge et capot extérieur, "
        "mortaise et vis comprises",
    ("Grilles d'entrée d'air", "GrilleGazPan"):
        "fourniture et pose de deux grilles gaz sur panneau, moustiquaire intégrée",
    ("Grilles d'entrée d'air", "GrilleGazVit"):
        "fourniture et pose de deux grilles gaz sur vitrage, moustiquaire intégrée",
    ("Grilles d'entrée d'air", "TrouPanPerso"):
        "trou personnalisé sur panneau, diamètre minimal de 120 mm",
    ("Grilles d'entrée d'air", "TrouVitPerso"):
        "trou personnalisé sur vitrage, diamètre minimal de 120 mm",
    ("Chatière", "CHAT"): "chatière pour panneau",
    ("Chatière", "CHAT_VIT"): "chatière pour vitrage",
    ("Chatière", "CHAT_PUCE"): "chatière à puce électronique pour panneau",
    ("Chatière", "CHAT_PUCE_VIT"): "chatière à puce électronique pour vitrage",
    ("Seuil", "5120SN"): "profil plinthe sur seuil dans le cas d'un fixe latéral",
    ("Seuil", "AS10100"):
        "seuil en aluminium anodisé nature clippé à un profilé PVC pour rupture de "
        "pont thermique",
    ("Seuil", "AS10100-RA1"): "seuil avec rallonge de 81 mm, anodisé nature ou anodisé noir",
    ("Seuil", "AS10100-RA2"): "seuil avec rallonge de 110 mm, anodisé nature ou anodisé noir",
    ("Seuil", "5263"): "bouclier de protection pour porte-fenêtre",
    ("Ferrage porte-fenêtre R20", "AS10100"):
        "seuil universel en aluminium anodisé nature clippé à un profilé PVC pour "
        "rupture de pont thermique, incompatible avec le ferrage invisible",
    ("Ferrage porte-fenêtre R20", "AK10131"):
        "profil plinthe sur seuil dans le cadre du fixe latéral",
    ("Ferrage porte-fenêtre R20", "AK40112"): "poignée de tirage, teinte noire",
    ("Ferrage SA R20", "FL 70"):
        "flexible de 700 mm de longueur, pour un ébrasement inférieur à 380 mm",
    ("Ferrage SA R20", "FL 100"):
        "flexible de 1000 mm de longueur, pour un ébrasement inférieur à 680 mm",
    ("Ferrage SA R20", "T8PR-01"):
        "tringle en alu nature de 8 mm de diamètre avec profil de recouvrement, "
        "pour poignée blanche",
    ("Ferrage SA R20", "T8PR-EV1"):
        "tringle en alu nature de 8 mm de diamètre avec profil de recouvrement, "
        "pour poignée EV1 nature",
    ("Ferrage SA R20", "Coupe sur mesure"):
        "tringle et profil de recouvrement coupés sur mesure",
    ("Ferrage SA R20", "F25"):
        "commande centrale pour deux vantaux, comprenant un T en alu nature, un "
        "recouvrement blanc et un accouplement en alu nature",
    ("Ferrage SA R20", "CEFI"):
        "commande électrique en alu nature, comprenant le moteur et le compas, "
        "nécessitant l'élargisseur AK10129",
    ("Laquage bloc-baie", "Face ext"): "en face extérieure",
    ("Laquage bloc-baie", "2 faces"): "sur les deux faces",
    ("Meneaux complémentaires", "Prix HT"): "meneau battant complémentaire",
    ("Crémones à l'ancienne", "CAL-01"): "finition blanche",
    ("Crémones à l'ancienne", "CAL-NRE"): "finition noir mat",
    ("Crémones à l'ancienne", "CAL-LA"): "finition laiton poli",
    ("Crémones à l'ancienne", "CAL-LAV"): "finition laiton vieilli",
    ("Crémones à l'ancienne", "CAL-FP"): "finition fer patiné",
    ("Crémones à l'ancienne", "CAL-TRI-TBRL"): "finition mixte TBRL",
    ("Crémones à l'ancienne", "CAL-TRI-TLRB"): "finition mixte TLRB",
}
# notes complémentaires accrochées à un poste précis
NOTES = {
    (170, None): None,
    ("Remplissage", "PV soubassement", "Panneau phonique (Rw = 38 dB) Groupe 1 sans PV"):
        "Le tarif ne porte aucune valeur pour ce panneau phonique en 36 mm : cette "
        "épaisseur n'est pas tarifée page 27.",
    ("Meneaux complémentaires", "Meneaux battants", "Prix HT"):
        "Le tarif précise que les meneaux battants ne peuvent être disposés qu'en "
        "filant, et qu'une plus-value de couleur s'ajoute sur les châssis de couleur.",
    ("Poignées", "En option", "Autres poignées"):
        "La poignée Toulon est sans plus-value lorsqu'elle est incluse au pack "
        "Trybadesign ; le montant indiqué vaut hors pack Trybadesign.",
}

# sujet employé dans le corps du chunk : groupe nominal complet, déterminant
# compris. {d} = référence du PDF, {desc} = description reprise du PDF,
# {t} = variantes de la colonne Détails.
SUJETS = {
    ("Laquage bloc-baie", "Forfait"): "le forfait de laquage du volet roulant {desc}",
    ("Vitrage-généralités", "Vitrage d'altitude"): "la plus-value pour vitrage d'altitude",
    ("PV vitrages", None): "le vitrage {d}",
    ("PV vitrages ornementaux", None): "le vitrage ornemental {d}",
    ("PV composition libre", None): "le vitrage en composition libre {d}",
    ("Remplissage", "PV soubassement",
     "Panneau phonique (Rw = 38 dB) Groupe 1 sans PV"):
        "le panneau de soubassement phonique d'indice Rw = 38 dB, en teinte du groupe 1, "
        "de {t} mm d'épaisseur",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe 1 sans PV"):
        "le panneau de soubassement standard en teinte du groupe 1, de {t} mm d'épaisseur",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe 2 PV 15 %"):
        "le panneau de soubassement standard en teinte du groupe 2, de {t} mm d'épaisseur",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe Sublimation"):
        "le panneau de soubassement standard en teinte du groupe sublimation, de {t} mm "
        "d'épaisseur",
    ("Remplissage", "panneaux moulurés"): "le {d} de {t} mm d'épaisseur",
    ("Croisillons intégrés+grecque", 'Forfait "grecque"'):
        "le motif de croisillons à la grecque, ses quatre angles compris",
    ("Poignées", "En option", "Poignées à clé"):
        "la poignée à clé {t}|les poignées à clé {t}",
    ("Poignées", "En option", "Autres poignées"): "la poignée {t}|les poignées {t}",
    ("Crémones à l'ancienne", "prix et finitions"):
        "la crémone à l'ancienne {d}, {desc}",
    ("Grilles d'entrée d'air", None): "la {desc}, référence {d}",
    ("Chatière", None): "la {desc}, référence {d}",
    ("Ferrage R20", "PV options", "Entrebaîlleur OF"):
        "l'entrebâilleur en ouverture à la française, en ferrage R20",
    ("Ferrage R20", "PV options", "OB"):
        "l'ouverture oscillo-battante (OB), en ferrage R20",
    ("Ferrage R20", "PV options", "OB inversé"):
        "l'ouverture oscillo-battante inversée, en ferrage R20",
    ("Ferrage R20", "PV options", "PV rabaisser poignée"):
        "la plus-value permettant de rabaisser la poignée sous sa hauteur standard",
    ("Ferrage SA R20", "PV tringle"): "la {desc}, référence {d}",
    ("Ferrage SA R20", "PV pour guide supp"):
        "le guide supplémentaire {d} du soufflet d'aération, à prévoir tous les mètres",
    ("Ferrage SA R20", "PV selon long flexible"): "le {desc}, référence {d}",
    ("Ferrage SA R20", "PV pour compas de sécu"):
        "le compas de sécurité pour soufflet d'aération, référence {d}",
    ("Ferrage SA R20", "Cmd spéciales pour SA"): "la {desc}, référence {d}",
    ("Ferrage porte-fenêtre R20", "Ensemble serrure SB"):
        "l'ensemble de serrure SB pour porte-fenêtre, comprenant une crémone à "
        "barillet à béquille double et sa garniture en fourniture seule",
    ("Ferrage porte-fenêtre R20", "Ensemble serrure CC"):
        "l'ensemble de serrure CC pour porte-fenêtre, comprenant une crémone "
        "condamnable et sa garniture avec poignée standard et rosette séparée",
    ("Ferrage porte-fenêtre R20", "Seuil universel"): "le {desc}, référence {d}",
    ("Ferrage porte-fenêtre R20", "Poignée de tirage"): "la {desc}, référence {d}",
    ("Pack Trybadesign", "Ferrage invisible"):
        "le pack Trybadesign, qui comprend le ferrage invisible et la poignée Toulon "
        "laquée à la teinte du châssis",
    ("Pièce d'appui et élargisseurs", "Pièces d'appui"):
        "la pièce d'appui, quelle que soit sa référence parmi NF-TA84, NF-TA84-D, "
        "AK10118 et AK10119",
    ("Pièce d'appui et élargisseurs", "Elargisseurs de seuil"):
        "l'élargisseur de seuil, quelle que soit sa référence parmi les profils "
        "12157, 5300, 5301 et 5307",
    ("Pièce d'appui et élargisseurs", "Profilés complémentaires"):
        "le panneau élargisseur RVT70 de 70 mm, affaiblissement de 30 dB",
    ("Meneaux complémentaires", "Meneaux battants", "Prix HT"):
        "les meneaux et traverses supplémentaires placés dans un ouvrant, sous les "
        "références AK10113 (MB114) et AK10114 (MB60) en vitrage de 28 mm, AK10111 "
        "(MB114) et AK10112 (MB60) en vitrage de 36 mm",
    ("Meneaux complémentaires", "Meneaux battants",
     "Plus-value pour 2 fixations à angle droit"):
        "la fixation à angle droit d'un meneau battant",
    ("Seuil", "Seuil"): "le {desc}, référence {d}",
    ("Châssis spé-Tarification", None, "Triangle et trapèze"):
        "la plus-value applicable à un châssis fixe spécial de forme triangle ou "
        "trapèze, à ajouter au prix du châssis rectangulaire qui l'englobe",
    ("Châssis spé-Tarification", None, "Meneau/traverse"):
        "la fixation spéciale d'un meneau ou d'une traverse, qu'elle soit biaise, sur "
        "angle ou sur partie biaise",
    ("Croisillons", None, "Champs"):
        "le champ délimité par les croisillons d'un châssis fixe spécial",
    ("Croisillons", None, "Etoiles 2 branches"):
        "l'étoile de croisillons à deux branches d'un châssis fixe spécial",
    ("Croisillons", None, "Etoiles 3 branches"):
        "l'étoile de croisillons à trois branches d'un châssis fixe spécial",
    ("Croisillons", None, "Etoile 4 branches"):
        "l'étoile de croisillons à quatre branches d'un châssis fixe spécial",
    ("Croisillons", None, "Toutes connexions en + de 4"):
        "chaque connexion de croisillons au-delà de la quatrième",
    ("Croisillons", None, "cintres"):
        "le cintre de croisillons d'un châssis fixe spécial, de rayon minimal 125 mm",
    ("Croisillons", None, "Demi-lune • de 100 x 50 x 8 mm"):
        "la demi-lune de croisillons de 100 par 50 par 8 mm",
    ("Croisillons", None, "Demi-lune • de 130 x 65 x 8 mm"):
        "la demi-lune de croisillons de 130 par 65 par 8 mm",
    ("Croisillons", None, "Demi-lune • de 250 x 125 x 8 mm"):
        "la demi-lune de croisillons de 250 par 125 par 8 mm",
    ("Croisillons", None, "Croisillons sur partie cintrée"):
        "le croisillon aboutissant sur une partie cintrée, biaise ou sur l'angle d'un "
        "châssis, qui nécessite un fraisage d'adaptation",
}


# ============================================================ blocs statiques
# F1, F5 et F6 sont rédigés à partir du PDF, page par page, en information
# originale. Aucun montant ni pourcentage en F5 et F6 (règles T6 et T7).
F1_BLOCS = [
    ("Distinction entre cote de tarif et cote de fabrication", 6,
     "Sur la {p}, toutes les cotes sont exprimées en millimètres et le tarif "
     "distingue deux jeux de cotes. Les cotes de référence tarif, notées L exposant T "
     "par H exposant T, servent au chiffrage des devis. Les cotes L par H servent de "
     "référence pour la commande et la fabrication des châssis. Le tarif l'écrit ainsi : "
     "les cotes prix sont L exposant T par H exposant T, les cotes fabrication sont "
     "L par H. Un prix lu sur la mauvaise cote est un prix faux : avant tout chiffrage, "
     "il faut établir si la dimension fournie est une cote de tarif ou une cote de "
     "fabrication."),
    ("Passage de la cote de fabrication à la cote de tarif selon le dormant", 6,
     "Sur la {p}, le passage d'un jeu de cotes à l'autre suit deux régimes. Pour un "
     "châssis sans complément, les cotes de tarif sont égales aux cotes de fabrication, "
     "et cela vaut pour le dormant neuf sans ailette AK10130 (L74), pour le dormant "
     "AK10117 (L83), pour le dormant AK10130 (L74) en version fixe, ainsi que pour les "
     "dormants à ailette AK10120 (LZ109) et AK10121 (LZ139). Pour un châssis avec "
     "compléments, toutes teintes, les cotes de tarif désignent le châssis nu tandis que "
     "les cotes de fabrication incluent les compléments. Sur un dormant à ailette avec "
     "appui, la hauteur de tarif s'obtient en retranchant de la hauteur de fabrication "
     "l'épaisseur de la pièce d'appui."),
    ("Épaisseur des pièces d'appui à déduire pour obtenir la hauteur de tarif", 57,
     "Sur la {p}, le tarif indique que les pièces d'appui se commandent sous forme de "
     "compléments, que leur plus-value s'ajoute à la valeur du châssis nu, et que "
     "l'épaisseur de la pièce d'appui employée doit être déduite de la hauteur de "
     "fabrication pour obtenir la hauteur de tarif du châssis. Les pièces d'appui neuf "
     "NF-TA84 et rénovation NF-TA84-D ont une hauteur de 20 mm ; les pièces d'appui "
     "courte AK10118 et longue AK10119 ont une hauteur de 20,5 mm. Le dormant compatible "
     "diffère selon la pièce : la pièce d'appui neuf convient aux dormants AK10130 (L74) "
     "et AK10117 (L83), la pièce rénovation aux dormants AK10120 (LZ109), AK10121 "
     "(LZ139) et AK10122 (L69), les pièces courte et longue au dormant AK10102 (L85)."),
    ("Lecture des grilles de prix par bandes de dimensions", 9,
     "Sur la {p}, le tarif énonce sa règle de lecture des grilles de prix. Une valeur de "
     "grille ne correspond pas à une dimension ponctuelle mais à une bande de "
     "dimensions : la colonne intitulée 1000 couvre les largeurs de 901 à 1000 mm, et la "
     "ligne intitulée 600 couvre les hauteurs de 501 à 600 mm. Une demande dont les cotes "
     "tombent à l'intérieur d'une bande se lit donc directement sur la valeur de cette "
     "bande, sans arrondi ni interpolation. La même page précise que les prix indiqués "
     "sont des valeurs pour châssis sans compléments, et que le prix lu est celui du "
     "châssis de base, vitrage standard compris."),
    ("Valeur d'abaque des grilles de prix et limites de fabrication", 47,
     "Sur la {p}, le tarif indique que les grilles de prix des différents types de "
     "châssis définissent les limites de fabrication des châssis TA76 OC et peuvent de "
     "ce fait être utilisées comme abaques. Le contour d'une grille est donc une limite "
     "de fabrication : une dimension qui sort de la grille n'est pas une dimension dont "
     "le prix serait à chercher ailleurs, c'est une configuration qui n'est pas réalisable "
     "au tarif. Les grilles précisent par ailleurs que leurs limites dimensionnelles "
     "valent jusqu'à un vitrage de 50 kg par mètre carré."),
    ("Composition d'un prix et conduite à tenir devant une demande de chiffrage", 9,
     "Le prix d'un ensemble se compose du prix de grille du châssis, lu sur le type "
     "d'ouverture et les deux cotes de tarif, auquel s'ajoutent les plus-values des "
     "options retenues, chacune servie avec son unité de facturation. Trois éléments "
     "sont nécessaires avant de restituer un prix de châssis : le type d'ouverture, la "
     "cote de tarif en largeur et la cote de tarif en hauteur. Aucun total ne doit être "
     "calculé ni aucune valeur interpolée : les montants sont restitués tels qu'ils "
     "figurent au tarif, et l'addition des postes revient à l'ADV."),
    ("Vocabulaire et abréviations du tarif", 8,
     "Le tarif emploie des abréviations constantes qu'il faut savoir lire. OC désigne "
     "l'ouvrant caché, qui caractérise la gamme entière, et OV l'ouvrant visible de la "
     "gamme jumelle TA76 OV. OF désigne l'ouverture à la française et OB "
     "l'oscillo-battant, dont il existe une variante inversée. SN désigne le soufflet "
     "normal et SA le soufflet d'aération avec ferme-imposte. SB et CC désignent deux "
     "ensembles de serrures de porte-fenêtre. LF désigne le levier en feuillure du "
     "battant semi-fixe. Les notations 1V et 2VTX désignent respectivement un vantail et "
     "deux vantaux."),
    ("Statut du PDF comme document de référence du tarif", 2,
     "Sur la {p}, le tarif indique que les spécifications techniques et les informations "
     "tarifaires qu'il contient ont été validées par le Service Produits, et que toute "
     "construction qui ne peut être traitée à l'aide du tarif n'est pas réalisable. Il "
     "précise également que le logiciel Syscon étant en constante évolution, cette "
     "version PDF reste le seul document de référence, et invite à s'y référer en cas de "
     "doute. Les prix sont exprimés en euros, hors éco-participation."),
]

F5_BLOCS = [
    ("Faisabilité des châssis et renvois vers la gamme jumelle à ouvrant visible", 8,
     "Sur la {p}, le tarif énonce la faisabilité des châssis en TA76 OC. Parmi les "
     "châssis fixes, plusieurs configurations ne sont pas réalisables en TA76 OC et sont "
     "renvoyées à la gamme TA76 OV ; d'autres configurations de fixes, notamment celles "
     "à traverses et à recoupes, sont réalisables en TA76 OC. Parmi les ouvrants, la "
     "fenêtre à un et à deux vantaux est réalisable, tandis que plusieurs configurations "
     "à recoupes et la configuration en angle biais ne le sont pas. Les portes-fenêtres "
     "SB et CC sont réalisables, avec un renvoi vers TA76 OV. La page distingue chaque "
     "cas par un schéma : elle doit être consultée pour toute configuration inhabituelle."),
    ("Restrictions du ferrage R20 et du ferrage invisible", 45,
     "Sur la {p}, le tarif énonce les restrictions de ferrage. Le seuil est compatible "
     "avec le ferrage R20 mais pas avec le ferrage R20 invisible. L'oscillo-battant "
     "inversé et l'entrebâilleur en ouverture à la française sont compatibles avec les "
     "deux ferrages. L'entrebâilleur en ouverture à la française n'est en revanche pas "
     "compatible avec le seuil ni avec un châssis cintré. Le SB-CC et le SB-CC sur seuil "
     "ne sont réalisables ni en ferrage R20 ni en ferrage R20 invisible : le SB-CC n'est "
     "faisable qu'en gamme TA76 OV, et le SB-CC sur seuil qu'en gamme TA76 OV hors "
     "ferrage invisible. L'ouverture maximale du vantail en ferrage R20 invisible est de "
     "110 degrés."),
    ("Incompatibilité du pack Trybadesign avec le seuil", 52,
     "Sur la {p}, le tarif indique que le pack Trybadesign est incompatible avec le "
     "seuil. Le pack comprend en standard des ferrures de sécurité à trois galets "
     "champignons au minimum, un ferrage invisible dont les organes de rotation sont "
     "clamés dans les parois aluminium des dormants et des battants, et une poignée "
     "Toulon laquée à la teinte du châssis. L'ouverture maximale du vantail en ferrage "
     "invisible est de 110 degrés. La page porte également les limites dimensionnelles "
     "propres au ferrage invisible, exprimées en dimensions de battant, qui diffèrent de "
     "celles du ferrage R20."),
    ("Règles de ferrage et limites dimensionnelles des grilles de prix", 10,
     "Les pages de grilles de prix énoncent les règles de ferrage communes aux châssis "
     "à ouvrant. La largeur maximale du battant est de 1,5 fois sa hauteur, la surface "
     "maximale du battant est de 2,4 mètres carrés, et le poids maximal par vantail est "
     "de 80 kg. Les limites dimensionnelles des grilles valent jusqu'à un vitrage de "
     "50 kg par mètre carré ; au-delà, il faut se reporter aux pages de limites de "
     "fabrication du battant par épaisseur de vitrage. Sur le châssis à deux ouvrants "
     "égaux, le levier en feuillure équipe le battant semi-fixe à partir d'une hauteur de "
     "550 mm, et un verrou à levier est employé en dessous."),
    ("Limites de fabrication du battant selon l'épaisseur du vitrage", 54,
     "Les pages 54 et 55 du tarif portent quatre abaques donnant, pour un poids de "
     "vitrage de 30, 35, 40 et 55 kg par mètre carré, les couples de dimensions de "
     "battant réalisables et le type de paumelles admissible. Ces abaques distinguent "
     "deux domaines : celui où la paumelle P60, le compas OF et le ferrage OB sont tous "
     "possibles, et celui où seuls le compas OF et le ferrage OB le sont. Le tarif "
     "explique la méthode de calcul du poids : l'épaisseur totale de verre en millimètres "
     "multipliée par 2,5 donne le poids en kilogrammes par mètre carré. Ces abaques ne "
     "sont pas transcrites ici et doivent être lues directement sur les pages 54 et 55."),
    ("Poids maximal admissible par type de paumelles", 53,
     "Sur la {p}, le tarif indique le poids maximal admissible de chaque type de "
     "paumelles. La paumelle P60 admet un poids maximal de 60 kg. Le compas OF, le "
     "ferrage OB et le ferrage invisible admettent chacun un poids maximal de 80 kg, "
     "cette limite étant restreinte par les profilés à ouvrant caché. Les paumelles P60, "
     "le compas OF et le ferrage OB sont disponibles en blanc, en titane et en noir. Les "
     "paumelles relèvent ici de la tenue et de la durabilité du châssis, non de la "
     "résistance à l'effraction, qui est assurée par les gâches de sécurité à galets "
     "champignons anti-décrochement."),
    ("Faisabilités des poignées selon la configuration du châssis", 37,
     "Sur la {p}, le tarif croise chaque poignée avec ses teintes disponibles et six "
     "configurations : un vantail, deux vantaux, SB, CC, poignée à clé 100N et "
     "oscillo-battant inversé. Les poignées Lento, Liège et Toulon couvrent le plus large "
     "domaine, avec des teintes admises qui varient d'une poignée à l'autre : la Lento "
     "n'existe pas en titane anodisé mais en titane laqué brillant, la Liège en titane "
     "anodisé, la Toulon en RAL granité sur un vantail, deux vantaux et SB uniquement. "
     "Les poignées Lilly, Hélène et Camille ont un domaine restreint. Cette page doit "
     "être consultée avant toute proposition de poignée."),
    ("Limites d'utilisation des crémones à l'ancienne", 36,
     "Sur la {p}, le tarif énonce les limites d'utilisation des crémones à l'ancienne "
     "selon la hauteur extérieure du battant. À 2000 mm, la crémone est utilisable sur "
     "fenêtre en ouverture à la française avec béquille en option, ainsi que sur fenêtre "
     "oscillo-battante et sur porte-fenêtre dans les deux modes d'ouverture. À 2400 mm, "
     "elle reste utilisable en ouverture à la française sur fenêtre et sur porte-fenêtre, "
     "mais pas en oscillo-battant. À 2500 mm, elle n'est utilisable dans aucune "
     "configuration. Le tarif précise que les logiciels Look et Syscon ne sont pas "
     "bloqués à ces limites préconisées."),
    ("Faisabilités de composition des triples vitrages", 22,
     "Sur la {p}, le tarif énonce les règles de composition d'un triple vitrage, par "
     "ordre de priorité. Un vitrage solaire TRYBASUN ou STOPSOL se situe côté extérieur. "
     "Un vitrage ornemental ou porteur de croisillons Art Déco se situe au milieu. Un "
     "vitrage à couche Isol'3 se situe côté intérieur et côté extérieur. Un vitrage "
     "feuilleté se situe côté intérieur, et également à l'extérieur en cas d'allège. Le "
     "vitrage phonique se place sans importance de position. Il est impératif d'avoir un "
     "vitrage à couche thermique côté intérieur et côté extérieur."),
    ("Faisabilités de composition des doubles vitrages", 22,
     "Sur la {p}, le tarif énonce les règles de composition d'un double vitrage, par "
     "ordre de priorité. Un vitrage solaire TRYBASUN ou STOPSOL se situe côté extérieur. "
     "Un vitrage ornemental ou porteur de croisillons Art Déco se situe côté extérieur. "
     "Un vitrage à couche Isol'3 se situe côté intérieur. Un vitrage feuilleté se situe "
     "côté intérieur, et également à l'extérieur en cas d'allège. Il est impératif "
     "d'avoir au moins un vitrage à couche thermique. Le rapport maximal entre la largeur "
     "et la hauteur, pour un verre de 4 mm, est de 6. Les vitrages de dimensions "
     "inférieures à 190 par 350 mm ne peuvent pas être fabriqués en TPS."),
    ("Vitrages exclus de la certification Cekal", 22,
     "Sur la {p}, le tarif indique que tous les vitrages sont titulaires de la "
     "certification Cekal, à quatre exceptions près : les vitrages Art Déco, les vitrages "
     "avec croisillons laiton ou couleur, le vitrage Cathédrale, et les vitrages de "
     "petites dimensions, c'est-à-dire inférieurs à 350 par 350 mm avec un écarteur de "
     "16 mm ou inférieurs à 410 par 410 mm avec un écarteur de 20 mm. Cette exclusion "
     "porte sur la certification et non sur la faisabilité."),
    ("Conditions de mise en œuvre du vitrage d'altitude", 23,
     "Sur la {p}, le tarif indique qu'à partir d'une altitude de 600 mètres en triple "
     "vitrage et de 1000 mètres en double vitrage, la fabrication doit faire l'objet "
     "d'une saisie spéciale « vitrage d'altitude ». Pour pouvoir proposer le vitrage "
     "d'altitude, il est impératif de disposer d'un écarteur de 12 mm sans croisillons, "
     "ou de 14 mm avec croisillons. Le tarif précise que des déformations optiques "
     "légères, convexes ou concaves, peuvent se manifester sous l'effet des différences "
     "de pression ou de température, et ne peuvent motiver un remplacement du vitrage."),
    ("Contraintes de transport et d'accouplement des châssis", 23,
     "Sur la {p}, le tarif énonce les contraintes liées au moyen de transport. Les "
     "châssis sont livrés en cassette, sanglés et protégés par cales en polystyrène, sauf "
     "pour un ensemble dont une dimension dépasse 2,20 m, cas dans lequel il faut "
     "contacter le service expédition. À partir d'une dimension de vitrage supérieure à "
     "2000 par 2000 mm ou d'un poids supérieur à 80 kg, le vitrage est livré non posé. "
     "Tout châssis dont la largeur ou la hauteur dépasse 2500 mm doit être réalisé en "
     "plusieurs parties avec accouplement."),
    ("Vitrages ornementaux indisponibles en triple vitrage", 25,
     "Sur la {p}, le tarif indique que pour des raisons techniques les verres Opale, "
     "Delta, Gothique et Mastercarré ne sont pas disponibles en triple vitrage. Les "
     "verres Chinchilla, Cathédrale, Granité, Dépoli, Sablé et Stopsol sont disponibles "
     "aussi bien en double qu'en triple vitrage. La page présente un visuel de chaque "
     "aspect, qui permet de montrer le rendu au client avant commande."),
    ("Dimensions limites des panneaux de remplissage", 27,
     "Sur la {p}, le tarif énonce les dimensions limites des panneaux. Un panneau "
     "standard mesure au minimum 195 par 195 mm et au maximum 3000 par 1500 mm, la "
     "hauteur minimale de soubassement étant de 250 mm. Un panneau mouluré mesure au "
     "minimum 270 par 270 mm et au maximum 2000 par 1000 mm ou 1000 par 2000 mm. Les "
     "tons bois et l'anodisation sont impossibles sur panneau mouluré, les autres RAL "
     "étant soumis à demande de faisabilité. Le soubassement standard a une hauteur de "
     "350 mm et sa valeur est précalculée au bas des grilles de prix."),
    ("Méthode de calcul du prix d'un soubassement", 27,
     "Sur la {p}, le tarif décrit la méthode de calcul du prix d'un soubassement. Le prix "
     "du soubassement est une plus-value à appliquer sur sa valeur vitrage. Il faut "
     "partir du prix du châssis complet, vitrage compris, chercher dans la grille de prix "
     "des châssis fixes la valeur vitrage correspondant à la taille du remplissage, "
     "calculer la plus-value du panneau, l'ajouter au châssis, puis ajouter le prix de la "
     "traverse et des fixations pour traverse. Les dimensions du panneau se calculent à "
     "partir de la hauteur de soubassement mesurée de l'extérieur du dormant à l'axe de "
     "la traverse, et d'une largeur égale à la largeur du châssis divisée par le nombre "
     "de vantaux. Ce calcul revient à l'ADV."),
    ("Méthode de calcul du prix des croisillons au nombre de champs", 28,
     "Sur la {p}, le tarif précise que le calcul du prix des croisillons, incorporés "
     "comme rapportés, se fait au nombre de champs et non au mètre linéaire. Un champ est "
     "une surface de vitrage délimitée par les croisillons, par le dormant ou par "
     "l'ouvrant. Le tarif distingue deux cas : les champs avec croisillons à jonctions en "
     "T ou en croix, et les champs avec croisillons sans jonction, dits filants. Le "
     "comptage des champs relève de l'ADV : le tarif donne un prix par champ, jamais un "
     "prix total."),
    ("Conditions de pose et de garantie des croisillons intégrés", 28,
     "Sur la {p}, le tarif énonce les conditions de garantie des croisillons intégrés. "
     "Pour bénéficier de la garantie TRYBA, les croisillons doivent toujours être "
     "incorporés en laissant au moins 2 mm entre le croisillon et le vitrage. Le "
     "certificat CEKAL est applicable dès 1 mm, mais cette exécution entraîne des bruits "
     "de contact et des taches de contact, et n'est pas couverte par la garantie. Pour "
     "les croisillons en finition laquée, seule la teinte RAL est reprise, et non la "
     "finition granitée. Sur châssis anodisé nature les croisillons sont laqués en RAL "
     "9006, sur châssis rouille en RAL 8016, et sur châssis RAL 9010 brillant en blanc "
     "RAL 9016."),
    ("Position des croisillons et des écarteurs selon le type de vitrage", 29,
     "Sur la {p}, le tarif indique que dans le cas d'un châssis avec croisillons "
     "intégrés, les écarteurs de vitrage seront traditionnels, en inox noir, ou en TPS "
     "selon les impératifs techniques. Dans le cas d'un triple vitrage, les croisillons "
     "se situent entre le verre intermédiaire et le verre extérieur. La page précise "
     "également que les croisillons teintés blanc et plomb foncé sont laqués, tandis que "
     "la teinte laiton est obtenue par anodisation."),
    ("Restrictions de gravure des motifs Art Déco", 30,
     "Sur la {p}, le tarif énonce trois restrictions sur les gravures Art Déco. La "
     "gravure ne se fait que sur vitrage de 6 mm. Aucune gravure n'est possible sur "
     "vitrage ornemental ni sur vitrage d'altitude. Il est impossible de combiner un "
     "vitrage Isol'3 associé à un vitrage TRYBASUN avec un motif Art Déco. La page "
     "indique en outre que la gravure en 18 mm de largeur n'est réalisable qu'en gravure "
     "sablée sur vitrage transparent, les deux gravures transparentes étant non "
     "réalisables dans cette largeur. La taille maximale d'un vitrage gravé est de 1600 "
     "par 2500 mm."),
    ("Incompatibilités des croisillons rapportés en double vitrage", 31,
     "Sur la {p}, le tarif indique qu'en double vitrage, les croisillons rapportés sont "
     "incompatibles avec deux vitrages décoratifs : le Cathédrale et le Delta blanc. Les "
     "croisillons rapportés deux faces en aluminium portent la référence de profil "
     "AK10208 et mesurent 26 mm de largeur."),
    ("Dimensions minimales des motifs de croisillons à la grecque", 29,
     "Sur la {p}, le tarif énonce les dimensions minimales de vitrage permettant un "
     "croisillon à la grecque : 400 par 400 mm sur un châssis à un vantail, et 250 par "
     "400 mm sur un châssis à deux vantaux. La page donne également les cotes X et Y du "
     "motif selon la taille du vitrage et le nombre de vantaux, ainsi que la condition "
     "d'apparition d'un croisillon intermédiaire, à partir d'un Y supérieur à 1700 mm."),
    ("Règles d'implantation des grilles d'entrée d'air", 40,
     "Sur la {p}, le tarif énonce trois règles d'implantation des grilles d'entrée d'air. "
     "En présence d'un volet roulant, si la menuiserie est équipée d'un coffre de volet "
     "roulant, la grille est proposée posée sur la trappe de visite du caisson. Sur "
     "l'ouvrant, l'entrée d'air est placée sur la traverse haute d'un battant, "
     "prioritairement le battant semi-fixe. Sur le dormant, dans le cas d'un châssis neuf "
     "AK10130, il faut prévoir un élargisseur AK10129 en partie haute. Lors de la pose, "
     "l'entrée d'air doit être parfaitement centrée sur la mortaise."),
    ("Limites d'utilisation des grilles d'entrée d'air selon le châssis", 40,
     "Sur la {p}, le tarif donne les largeurs minimales permettant de poser une grille "
     "centrée sur le clair de jour du vitrage en conservant des gardes latérales de "
     "20 mm, pour le dormant AK10130 (L74). Les grilles Mini ESEA 30 et Mini ESEA 22 "
     "requièrent 387 mm de largeur hors tout battant sur un et deux vantaux, 774 mm sur "
     "soufflet normal et 820 mm de largeur hors tout dormant sur soufflet d'aération. Les "
     "grilles ISOLA 45 et hygroréglable requièrent respectivement 522 mm, 1044 mm et "
     "1044 mm. Sur soufflet normal et soufflet d'aération, la grille est positionnée au "
     "quart de la largeur hors tout, côté gauche."),
    ("Restrictions de pose des chatières", 43,
     "Sur la {p}, le tarif indique que les chatières ne sont disponibles que pour des "
     "panneaux ou des vitrages de 28 et 36 mm. Le panneau ou le vitrage doit mesurer au "
     "minimum 340 par 340 mm, l'évidement mesure 168 par 175 mm et le passage de l'animal "
     "146 par 135 mm pour une largeur d'épaule maximale de 150 mm. Sur des panneaux "
     "aluminium, il faut employer la référence de chatière à puce électronique afin de "
     "limiter l'effet de cage de Faraday, et cette référence n'est pas disponible sur "
     "panneaux renforcés. Les chatières sont disponibles en blanc ou en brun."),
    ("Mise en œuvre du soufflet d'aération sur ébrasement", 49,
     "Sur la {p}, le tarif distingue deux mises en œuvre du soufflet d'aération selon "
     "l'ébrasement. Pour un ébrasement inférieur à 100 mm, la tringle de descente peut "
     "être déviée au montage, avec un angle maximal de 30 degrés. Pour un ébrasement "
     "supérieur à 100 mm et inférieur à 680 mm, un renvoi d'angle flexible est "
     "nécessaire, à choisir en 700 ou en 1000 mm selon l'ébrasement à contourner. "
     "Au-delà, le tarif renvoie à une consultation. Un guide supplémentaire est à prévoir "
     "tous les mètres."),
    ("Implantation du levier de commande du soufflet d'aération", 48,
     "Sur la {p}, le tarif énonce l'implantation du levier de commande du soufflet "
     "d'aération. Sur une fenêtre à un battant, avec ou sans volet roulant, le levier se "
     "place toujours du côté de la poignée de fenêtre. Sur une fenêtre à deux vantaux "
     "sans volet roulant, il se place côté vantail secondaire ; avec volet roulant, il se "
     "place du côté opposé à la commande, et il est recommandé de placer la commande côté "
     "paumelles. La grille de prix des châssis à ferme-imposte tient compte d'une "
     "longueur de tringle et de profil de recouvrement de 1,60 mètre linéaire."),
    ("Restrictions des ensembles de serrures SB et CC de porte-fenêtre", 51,
     "Sur la {p}, le tarif énonce les restrictions des deux ensembles de serrures de "
     "porte-fenêtre. L'ensemble SB, dont le verrouillage des galets s'obtient par "
     "relevage de la béquille en position 2, n'admet ni oscillo-battant ni poignée de "
     "tirage, et requiert une hauteur minimale de battant de 1641 mm. L'ensemble CC, dont "
     "le cylindre se place au-dessus de la poignée, admet l'oscillo-battant mais pas la "
     "poignée de tirage, et requiert une hauteur minimale de battant de 1741 mm. Dans les "
     "deux cas la hauteur de poignée est de 1070 mm. Le tarif avertit qu'une solution "
     "SB-CC n'apporte pas les garanties nécessaires pour une porte d'entrée ou une porte "
     "secondaire, et que l'option SB-CC n'est compatible qu'avec l'ouvrant droit."),
    ("Hauteur de poignée en fonction des dimensions du vantail", 47,
     "Sur la {p}, le tarif donne la hauteur de poignée standard en fonction de la hauteur "
     "de vantail, séparément pour l'exécution en ouverture à la française et pour "
     "l'exécution oscillo-battante, et distingue les battants standards des battants hors "
     "standard. En fenêtre, la poignée est au milieu jusqu'à 359 mm de hauteur de "
     "vantail, puis à 175, 220, 270, 420, 520, 620 et 720 mm par paliers de hauteur "
     "croissante. En porte-fenêtre, elle est à 820 mm ou à 1000 mm selon la hauteur du "
     "vantail. Une plus-value existe pour rabaisser la poignée."),
    ("Conditions de réalisation des châssis fixes spéciaux", 64,
     "Sur la {p}, le tarif énonce les conditions de réalisation des châssis fixes "
     "spéciaux, qui sont à saisir en gamme TA76 OV. Les limites dimensionnelles dépendent "
     "de la largeur du sertissage du plus petit angle, plafonnée à 350 mm. Pour un châssis "
     "fixe triangle-rectangle sur profilé dormant AK10130, l'angle minimal est de 30 "
     "degrés, le coefficient multiplicateur de 0,58 et la hauteur minimale de 140 mm, ce "
     "qui permet de déduire une hauteur minimale en fonction d'une largeur. Pour les "
     "trapèzes et les polygones, l'angle minimal et les angles supérieurs sont fonction "
     "de la longueur maximale de soudure. Le tarif avertit qu'il faut vérifier ces "
     "conditions avant tout chiffrage, et qu'aucune anodisation n'est possible sur châssis "
     "fixe."),
    ("Impossibilités de laquage du volet roulant selon la teinte", 19,
     "Sur la {p}, le tarif indique qu'il est impossible de proposer un volet roulant "
     "laqué pour deux teintes : le laquage champagne et le laquage bronze. La page 20 "
     "ajoute que le laquage deux faces n'est réalisable qu'avec des manœuvres par moteur "
     "filaire ou radio sans manœuvre de secours. Les mêmes pages recensent, teinte par "
     "teinte et élément par élément, les combinaisons impossibles entre la teinte du "
     "châssis aluminium et les composants du coffre de volet roulant, ainsi que les "
     "teintes pour lesquelles aucune saisie de coffre n'est possible."),
    ("Absence de bicoloration possible dans les coffres de volet roulant", 19,
     "Sur la {p}, le tarif indique qu'aucune bicoloration n'est possible dans les coffres "
     "de volet roulant : les combinaisons bicolores et les combinaisons RAL granité "
     "intérieur avec RAL granité extérieur ne permettent aucune saisie de coffre. Le "
     "laquage champagne et le laquage bronze ne permettent pas non plus de saisie de "
     "coffre. Pour l'anodisation nature, une cornière de 40 par 20 mm est à prévoir à la "
     "saisie."),
]

F6_BLOCS = [
    ("Existence et localisation de l'offre couleurs", 15,
     "Les pages 15 et 16 du tarif portent l'offre couleurs de la gamme. Les teintes sont "
     "réparties en groupes dont la logique tarifaire est un pourcentage appliqué au prix "
     "du châssis : un groupe est sans plus-value, les autres portent chacun un "
     "pourcentage propre. L'offre comprend des teintes monocolores en finition granitée "
     "mate, des laquages lisses, l'anodisation nature, des laquages d'imitation "
     "anodisation, une gamme Futura sablée, une teinte Rouille, ainsi que des "
     "combinaisons bicolores dont chaque association d'une teinte intérieure et d'une "
     "teinte extérieure est explicitement autorisée par un tableau. Les tons bois sublimés "
     "exclusifs sont des teintes approchantes des décors PVC. Les pourcentages doivent "
     "être lus directement sur les pages 15 et 16."),
    ("Existence et localisation des teintes des accessoires et des joints", 17,
     "Les pages 17 et 18 du tarif donnent, pour chaque groupe de couleur et chaque "
     "combinaison de teintes intérieure et extérieure du dormant et de l'ouvrant, la "
     "teinte de la poignée et des paumelles ainsi que celle de la grille de ventilation. "
     "La teinte des accessoires n'est donc pas choisie librement : elle est déterminée "
     "par la teinte du châssis. Le tarif précise que les joints sont noirs quelles que "
     "soient les teintes intérieure et extérieure du châssis. Les correspondances doivent "
     "être lues directement sur les pages 17 et 18."),
    ("Existence et localisation du laquage bloc-baie", 19,
     "Les pages 19 et 20 du tarif portent le laquage bloc-baie, c'est-à-dire "
     "l'harmonisation de teinte entre le châssis aluminium et les composants du coffre de "
     "volet roulant Chrono One 200 et 230 : référence de coffre, cornière, lame finale et "
     "coulisses en face extérieure, coffre PVC en face intérieure. Le tarif y indique, "
     "teinte par teinte, la finition à retenir sur chaque composant ou l'impossibilité de "
     "la réaliser. Cette table de correspondance ne porte pas de prix, à l'exception d'un "
     "forfait de laquage du volet roulant, qui est tarifé et se lit page 20."),
    ("Existence et localisation de la plus-value pour dormants de rénovation", 10,
     "Les quatre pages de grilles de prix, de la page 10 à la page 13, portent chacune "
     "une plus-value pour dormants de rénovation, applicable aux dormants AK10120 "
     "(LZ109) et AK10121 (LZ139). Cette plus-value est exprimée en pourcentage à "
     "appliquer sur les grilles de prix, et non en montant. Elle concerne donc tout "
     "chiffrage de châssis en rénovation sur ces deux dormants. Le pourcentage doit être "
     "lu directement sur la page de la grille concernée, et son application au prix de "
     "grille revient à l'ADV."),
    ("Existence et localisation de la plus-value de vitrage des châssis spéciaux", 65,
     "Sur la {p}, la tarification des châssis fixes spéciaux, à saisir en gamme TA76 OV, "
     "comporte deux composantes : une plus-value chiffrée à ajouter au prix du châssis "
     "rectangulaire englobant la forme à calculer, et une plus-value de vitrage exprimée "
     "en pourcentage. La seconde n'est pas transcrite ici et doit être lue directement "
     "sur la page 65. Le tarif précise que le prix de départ est celui du châssis "
     "rectangulaire lu dans la grille des châssis fixes, et que les prix des compléments "
     "et accessoires s'ajoutent au prix du châssis nu."),
    ("Existence et localisation de la majoration de gravure sur vitrage sablé", 30,
     "Sur la {p}, le tarif prévoit une variante de gravure Art Déco consistant en une "
     "gravure transparente sur vitrage sablé. Cette variante porte une majoration "
     "exprimée en pourcentage, appliquée à la plus-value du vitrage pour verre sablé, et "
     "non un montant. Le pourcentage et la plus-value de vitrage à laquelle il s'applique "
     "doivent être lus directement sur les pages 30 et 25, et leur combinaison revient à "
     "l'ADV."),
    ("Existence et localisation des surfaces minimales de facturation", 24,
     "Les pages 24 à 27 du tarif portent une surface minimale de facturation applicable "
     "aux plus-values de vitrage et de remplissage, qui sont exprimées au mètre carré. "
     "Cette surface plancher signifie qu'une plus-value calculée sur une surface "
     "inférieure est facturée sur la surface minimale. Elle doit être lue directement sur "
     "la page de la plus-value concernée, et son application revient à l'ADV."),
    ("Existence et localisation de l'historique des évolutions du tarif", 69,
     "Sur la {p}, le tarif porte un tableau récapitulant ses évolutions successives, avec "
     "pour chacune la modification apportée, la ou les pages concernées et la date "
     "d'application. Ce tableau recense aussi bien les corrections de contenu — évolution "
     "de la carte couleurs, mise à jour des schémas de ferrage, ajout de références de "
     "poignées, ouverture des châssis fixes spéciaux à la gamme TA76 OV — que les hausses "
     "successives appliquées à l'ensemble des prix. Pour savoir si une information a "
     "changé et à quelle date, cette page est la source à consulter."),
]

# postes à zéro présents au PDF mais absents de l'Excel (amendement OC 2 de T4)
SANS_PV = [
    ("Tarif des poignées Lento, Liège et Toulon en standard", 35,
     "Sur la {p}, les poignées Lento (référence PLENTO), Liège (référence PLIEGE) et "
     "Toulon (référence PTOULON) sont proposées en standard sans plus-value sur la "
     "fenêtre aluminium à ouvrant caché TA76 OC : leur prix est nul et elles n'entraînent "
     "aucun supplément par rapport au prix de grille du châssis. Elles sont disponibles "
     "en blanc, en titane ou en noir. Les poignées à clé et les autres modèles de "
     "poignées relèvent en revanche de l'option et portent chacun une plus-value."),
    ("Tarif des meneaux dormants complémentaires", 38,
     "Sur la {p}, les meneaux dormants complémentaires, références AK10115 (MD114) et "
     "AK10116 (MD141), sont proposés sans plus-value sur la fenêtre aluminium à ouvrant "
     "caché TA76 OC : leur prix est nul. Ils peuvent être disposés en T, en croix ou en "
     "filant. Les meneaux battants, situés dans les ouvrants, portent en revanche une "
     "plus-value et ne peuvent être disposés qu'en filant."),
    ("Tarif des profils d'ouvrant en design droit et galbé", 39,
     "Sur la {p}, les profils d'ouvrant sont proposés sans plus-value sur la fenêtre "
     "aluminium à ouvrant caché TA76 OC : leur prix est nul, quel que soit le design "
     "retenu. Pour un vitrage de 28 mm, le design droit porte la référence AK10105J (Z51) "
     "et le design galbé la référence AK10109J (Z51). Pour un vitrage de 36 mm, le design "
     "droit porte la référence AK10101J (Z51) et le design galbé la référence AK10107J "
     "(Z51)."),
    ("Tarif des parecloses pour châssis fixe", 39,
     "Sur la {p}, les parecloses pour châssis fixe sont proposées sans plus-value sur la "
     "fenêtre aluminium à ouvrant caché TA76 OC : leur prix est nul, quel que soit le "
     "design retenu. Pour un vitrage de 28 mm, le design droit porte la référence AK10203 "
     "et le design galbé la référence AK10206. Pour un vitrage de 36 mm, le design droit "
     "porte la référence AK10202 et le design galbé la référence AK10205. En cas de "
     "parecloses galbées, les parecloses hautes et basses restent droites."),
    ("Tarif du ferrage R20 et du ferrage symétrique", 46,
     "Sur la {p}, le ferrage TRYBASAFE R20 est fourni en standard sur les fenêtres et "
     "portes-fenêtres de la gamme aluminium à ouvrant caché TA76 OC, sans plus-value : il "
     "comprend des ferrures de sécurité à trois galets champignons au minimum, "
     "l'assemblage des cadres coupés d'onglet par sertissage et collage, des paumelles "
     "vissées dans les parois aluminium des dormants et des battants, et des gâches de "
     "sécurité à galet champignon évitant le décrochement de la fenêtre. Le ferrage "
     "symétrique est également fourni en standard, sans plus-value."),
    ("Tarif du levier en feuillure du battant semi-fixe", 46,
     "Sur la {p}, le levier en feuillure est proposé sans plus-value sur la fenêtre "
     "aluminium à ouvrant caché TA76 OC : son prix est nul. C'est un système de "
     "verrouillage qui équipe systématiquement le battant du semi-fixe à partir d'une "
     "hauteur de battant supérieure à 591 mm ; en dessous de 590 mm, c'est un verrou à "
     "levier qui équipe le battant, également sans plus-value."),
    ("Tarif des paumelles et des organes de rotation", 53,
     "Sur la fenêtre aluminium à ouvrant caché TA76 OC, le tarif porte les paumelles et "
     "les organes de rotation à 0 €, quel que soit le type retenu ({p}). La paumelle P60, le compas OF, le ferrage OB et le ferrage "
     "invisible sont tous fournis sans supplément ; ils se distinguent par le poids "
     "maximal de vantail qu'ils admettent, non par leur prix. Les paumelles P60, le "
     "compas OF et le ferrage OB sont disponibles en blanc, en titane et en noir."),
    ("Tarif du profilé de finition pour seuil AS20200", 62,
     "Sur la {p}, le profilé de finition pour seuil AS10100, référencé AS20200 et "
     "disponible en anodisé nature ou en anodisé noir, est proposé sans plus-value sur la "
     "fenêtre aluminium à ouvrant caché TA76 OC : son prix est nul. Les autres postes de "
     "la page — profil plinthe, seuil et ses deux versions à rallonge, bouclier de "
     "protection — portent en revanche chacun un montant."),
]


# ============================================================ utilitaires
def clean(v):
    return "" if v is None else str(v).replace("\xa0", " ").strip()


def norm(v):
    return re.sub(r"\s+", " ", clean(v))


def fmt_euro(v):
    """Recopie la valeur de la cellule, mise en forme. Jamais de calcul."""
    if v in (None, ""):
        return None
    try:
        n = int(round(float(v)))
    except (ValueError, TypeError):
        return None
    return f"{n:,}".replace(",", "\u202f")


def count_words(*parts):
    return len(re.findall(r"\S+", " ".join(parts)))


def sc_id(n):
    return f"SC{n:04d}"


def source_line(page, sc, nature="originale"):
    return f"*Source : {PDF_SOURCE}, page {page} — information {nature} — {sc}*"


def emit(title, source, body):
    n = count_words("##", title, source, body)   # le marqueur compte aussi
    if n > PLAFOND:
        ALERTS.append(f"PLAFOND DÉPASSÉ ({n} mots) : {title[:80]}")
    return f"## {title}\n{source}\n\n{body}\n"


def page_of(chap, tab, des=None):
    if (chap, tab, des) in PAGES_DES:
        return PAGES_DES[(chap, tab, des)]
    for k in ((chap, tab), (chap, None)):
        if k in PAGES:
            return PAGES[k]
    JOURNAL.append(f"PAGE NON ÉTABLIE : {chap} / {tab}")
    return "?"


def unite_of(chap, tab, des=None):
    for k in ((chap, tab, des), (chap, tab, None), (chap, None, None)):
        if k in UNITES:
            return UNITES[k]
    return ("__absente__", None)


def phrase_unite(chap, tab, des=None, pluriel=False):
    """Règle T4 : le montant servi est unitaire, le total revient à l'ADV."""
    code, libelle = unite_of(chap, tab, des)
    page = page_of(chap, tab, des)
    if code == "__absente__":
        JOURNAL.append(f"UNITÉ NON RENSEIGNÉE dans la table : {chap} / {tab} / {des}")
        code, libelle = None, None
    if libelle is None:
        JOURNAL.append(f"UNITÉ NON ÉTABLIE (absente du PDF) : {chap} / {tab} / {des} "
                       f"— le chunk renvoie à la page {page}")
        s = ("Le tarif n'énonce pas d'unité de facturation pour ce montant : elle doit "
             f"être lue page {page} du tarif.")
        return " " + s
    if code == "forfaitaire":
        d = "Ces montants sont" if pluriel else "Ce montant est"
        return f" {d} forfaitaire{'s' if pluriel else ''}, {libelle}."
    d = "Ces montants s'entendent" if pluriel else "Ce montant s'entend"
    m = "en les multipliant" if pluriel else "en le multipliant"
    return (f" {d} {libelle} : le total s'obtient {m} par la quantité concernée, "
            f"calcul qui revient à l'ADV.")


def phrase_surface_mini(chap):
    if chap in SURFACE_MINI:
        return " Le tarif fixe une surface minimale de facturation de 0,5 m²."
    return ""


def enumere(items):
    items = [i for i in items if i]
    if not items:
        return ""
    if len(items) == 1:
        return items[0]
    return ", ".join(items[:-1]) + " et " + items[-1]


def yaml_front(sous_type, nb):
    return (
        "---\n"
        f"document_source: {PDF_YAML}\n"
        "type_document: tarif\n"
        f"sous_type: {sous_type}\n"
        "gamme_code: TA76_OC\n"
        f'gamme_nom: "{DESIGNATION}"\n'
        'collection: "TRYBA ALUMINIUM"\n'
        "materiau: aluminium\n"
        'version_doc: "2026.06"\n'
        "date_validite: 2026-06-19\n"
        f"nb_chunks: {nb}\n"
        "audiences: [ADV, commercial]\n"
        "---\n\n"
    )


# ============================================================ chargement
def load_rows():
    wb = openpyxl.load_workbook(XLSX, data_only=True)
    ws = wb[FEUILLE]
    raw = list(ws.iter_rows(values_only=True))
    header = list(raw[0]) + [None] * (NCOLS - len(raw[0]))
    rows = []
    for i, r in enumerate(raw[1:], start=2):
        r = list(r) + [None] * (NCOLS - len(r))
        if all(v is None for v in r):
            continue
        rows.append({"xl": i, "v": r})
    largeurs = {j: int(str(header[j]).split()[2]) for j in COLS_HT_L}
    return header, rows, largeurs


def controle_gamme(rows):
    variantes = defaultdict(list)
    for r in rows:
        g = norm(r["v"][C_GAMME])
        if not g:
            continue
        if g.replace(" ", "") != GAMME.replace(" ", ""):
            JOURNAL.append(f"LIGNE HORS GAMME : Excel {r['xl']} — colonne gamme = "
                           f"« {g} » — exclue")
        elif g != GAMME:
            variantes[g].append(r["xl"])
    for g, xls in variantes.items():
        JOURNAL.append(f"GRAPHIE DE GAMME NORMALISÉE : « {g} » lu comme « {GAMME} » "
                       f"sur {len(xls)} lignes (Excel {xls[0]} à {xls[-1]})")


# ============================================================ bandes (page 9)
def bande(valeurs, val):
    """Bande couverte par une cote : (précédente + 1) .. val. None si première."""
    i = valeurs.index(val)
    return None if i == 0 else valeurs[i - 1] + 1


def dire_bande(bas, haut, unite="mm"):
    return f"jusqu'à {haut} {unite}" if bas is None else f"de {bas} à {haut} {unite}"


def lignes_grille(rows, chap, tab, largeurs):
    out = []
    for r in rows:
        v = r["v"]
        if norm(v[C_CHAP]) != chap or (norm(v[C_TAB]) or None) != tab:
            continue
        cells = []
        for j, k in zip(COLS_HT_L, COLS_TTC_L):
            if v[j] is None and v[k] is None:
                continue
            if v[j] is None or v[k] is None:
                JOURNAL.append(f"HT/TTC DÉSALIGNÉ : Excel {r['xl']} ({chap}/{tab}), "
                               f"largeur {largeurs[j]}")
                continue
            cells.append((largeurs[j], v[j], v[k]))
        if cells:
            out.append((v[C_HAUTEUR], cells, r["xl"]))
    return out


def echelle_largeurs(lignes):
    ech = set()
    for _, cells, _ in lignes:
        ech |= {c[0] for c in cells}
    return sorted(ech)


# ============================================================ F2 : prix de grille
def gen_grille_2d(chap, tab, libelle, synonyme, rows, largeurs, sc):
    """Règles T1 et T2. Découpage piloté par le plafond, jamais par une constante."""
    lignes = lignes_grille(rows, chap, tab, largeurs)
    ech_L = echelle_largeurs(lignes)
    ech_H = sorted({h for h, _, _ in lignes})
    page = page_of(chap, tab)
    chunks = []
    for h, cells, xl in lignes:
        idx = [ech_L.index(L) for L, _, _ in cells]
        if idx != list(range(idx[0], idx[0] + len(idx))):
            JOURNAL.append(f"LARGEURS NON CONTIGUËS : Excel {xl} ({chap}/{tab})")
        h_bas = bande(ech_H, h)
        items = [f"en largeur {dire_bande(bande(ech_L, L), L)}, "
                 f"{fmt_euro(ht)} € HT et {fmt_euro(ttc)} € TTC"
                 for L, ht, ttc in cells]
        i = 0
        while i < len(items):
            j = len(items)
            while j > i:
                lot = items[i:j]
                titre_l = (f"largeurs {dire_bande(bande(ech_L, cells[i][0]), cells[j - 1][0])}"
                           if (i or j < len(items)) else "toutes largeurs tarifées")
                title = (f"{PREFIXE}Tarif du {libelle}, hauteur "
                         f"{dire_bande(h_bas, h)}, {titre_l}")
                src = source_line(page, sc_id(sc))
                body = (f"Sur la grille de prix du {libelle} de la {PRODUIT}, "
                        f"ou {synonyme}, pour une cote tarif en hauteur "
                        f"{dire_bande(h_bas, h)}, le tarif est le suivant : "
                        + " ; ".join(lot) +
                        ". Ces prix s'entendent hors éco-participation et valent pour "
                        "un châssis sans complément, vitrage standard compris.")
                if count_words("##", title, src, body) <= PLAFOND or j == i + 1:
                    chunks.append(emit(title, src, body))
                    sc += 1
                    i = j
                    break
                j -= 1
    return chunks, sc


def gen_grille_1d(chap, tab, libelle, synonyme, rows, largeurs, sc):
    """Amendement OC de la règle T1 : grille à un seul axe, la largeur."""
    lignes = lignes_grille(rows, chap, tab, largeurs)
    page = page_of(chap, tab)
    chunks = []
    for h, cells, xl in lignes:
        if h not in (None, ""):
            JOURNAL.append(f"HAUTEUR INATTENDUE sur grille à un seul axe : Excel {xl}")
        ech_L = [c[0] for c in cells]
        items = [f"en largeur {dire_bande(bande(ech_L, L), L)}, "
                 f"{fmt_euro(ht)} € HT et {fmt_euro(ttc)} € TTC"
                 for L, ht, ttc in cells]
        i = 0
        while i < len(items):
            j = len(items)
            while j > i:
                lot = items[i:j]
                title = (f"{PREFIXE}Tarif de la {libelle}, largeurs "
                         f"{dire_bande(bande(ech_L, cells[i][0]), cells[j - 1][0])}")
                src = source_line(page, sc_id(sc))
                body = (f"Sur la grille de prix de la {libelle} de la {PRODUIT}, "
                        f"ou {synonyme}, le tarif ne dépend que de la largeur et non "
                        f"de la hauteur : "
                        + " ; ".join(lot) +
                        ". Ces prix s'entendent hors éco-participation.")
                if count_words("##", title, src, body) <= PLAFOND or j == i + 1:
                    chunks.append(emit(title, src, body))
                    sc += 1
                    i = j
                    break
                j -= 1
    return chunks, sc


def gen_f2(rows, largeurs):
    chunks, sc = [], 2
    for (chap, tab), (lib, syn) in GRILLES_2D.items():
        c, sc = gen_grille_2d(chap, tab, lib, syn, rows, largeurs, sc)
        chunks += c
    for (chap, tab), (lib, syn) in GRILLES_1D.items():
        c, sc = gen_grille_1d(chap, tab, lib, syn, rows, largeurs, sc)
        chunks += c
    return chunks


# ============================================================ F3 : options
def ref_pdf(des):
    """Règle OC3 : la référence du PDF fait foi."""
    return REF_PDF.get(des, des)


def libelle_poste(chap, tab, des, details):
    """Titre du poste. Le PDF fait foi sur le libellé (règle OC3)."""
    d = ref_pdf(des)
    t = enumere(sorted(set(details))) if details else ""
    if (chap, tab) in PRODUITS_TEINTE or (chap, None) in PRODUITS_TEINTE:
        prod = PRODUITS_TEINTE.get((chap, tab), PRODUITS_TEINTE.get((chap, None)))
        teinte = TEINTES.get(des, des)
        return f"Tarif du {prod.split(' (')[0]} {teinte}", prod, teinte
    modele = LIBELLES.get((chap, tab, des), LIBELLES.get((chap, tab),
                          LIBELLES.get((chap, None))))
    if modele and "|" in modele:
        sing, plur = modele.split("|", 1)
        modele = plur if len(set(details)) > 1 else sing
    if modele is None:
        JOURNAL.append(f"LIBELLÉ NON ÉTABLI : {chap} / {tab} / {des}")
        modele = "Tarif {d}"
    desc = DESC_PDF.get((chap, d), "")
    return modele.format(d=d, t=t, desc=desc), None, None


def sujet_poste(chap, tab, des, details):
    """Groupe nominal employé dans le corps. Le PDF fait foi (règle OC3)."""
    d = ref_pdf(des)
    modele = SUJETS.get((chap, tab, des), SUJETS.get((chap, tab),
                        SUJETS.get((chap, None))))
    desc = DESC_PDF.get((chap, d), "")
    t = enumere(sorted(set(details))) if details else ""
    if modele is None:
        JOURNAL.append(f"SUJET NON ÉTABLI : {chap} / {tab} / {des}")
        return f"le poste {d}" if d else "ce poste"
    if "|" in modele:
        sing, plur = modele.split("|", 1)
        modele = plur if len(set(details)) > 1 else sing
    if "{desc}" in modele and not desc:
        JOURNAL.append(f"DESCRIPTION PDF MANQUANTE : {chap} / {d}")
        modele = modele.replace(", {desc}", "").replace("{desc}, ", "")
        modele = modele.replace("la {desc}", "le poste").replace("le {desc}", "le poste")
    return re.sub(r"\s+", " ", modele.format(d=d, t=t, desc=desc)).strip()


def gen_poste(chap, tab, des, details, ht, ttc, sc, xl_list, nature="originale"):
    d = ref_pdf(des)
    titre, prod, teinte = libelle_poste(chap, tab, des, details)
    page = page_of(chap, tab, des)
    if prod:                                   # produit décliné par palier de teinte
        body = (f"Sur la {PRODUIT}, le tarif chiffre le {prod} {teinte} à "
                f"{fmt_euro(ht)} € HT, soit {fmt_euro(ttc)} € TTC.")
    else:
        sujet = sujet_poste(chap, tab, des, details)
        virgule = "," if "," in sujet else ""
        if ht == 0:
            body = (f"Sur la {PRODUIT}, le tarif porte {sujet}{virgule} à 0 € HT et "
                    f"0 € TTC : ce poste n'entraîne aucune plus-value.")
        else:
            body = (f"Sur la {PRODUIT}, le tarif chiffre {sujet}{virgule} à "
                    f"{fmt_euro(ht)} € HT, soit {fmt_euro(ttc)} € TTC.")
    if ht != 0:
        body += phrase_unite(chap, tab, des, pluriel=False)
        body += phrase_surface_mini(chap)
    if des in REF_PDF:
        body += (f" Le fichier de tarification désigne ce poste par le code "
                 f"{norm(des)} ; c'est la référence {d} du tarif qui fait foi.")
        JOURNAL.append(f"RÉFÉRENCE DIVERGENTE : Excel « {norm(des)} » / PDF « {d} » "
                       f"({chap} / {tab})")
    if chap == "Meneaux complémentaires" and details:
        JOURNAL.append(f"APPARIEMENT RECONSTRUIT DEPUIS LE PDF : {chap} / {des} — "
                       f"la colonne Détails de l'Excel porte « "
                       f"{enumere(sorted(set(details)))} », appariement démenti par la "
                       f"page 38 ; le montant est inchangé, le rattachement est celui "
                       f"du PDF")
    if "Groupe 2" in des:
        body += (" La teinte du groupe 2 porte par ailleurs une plus-value exprimée en "
                 "pourcentage, à lire page 15 du tarif.")
    if "Sublimation" in des:
        body += (" La teinte du groupe sublimation porte par ailleurs une plus-value "
                 "exprimée en pourcentage, à lire page 16 du tarif.")
    if ht == 0 and chap.startswith("PV vitrages"):
        body += (" Le tarif le range parmi les vitrages inclus dans la triple offre, "
                 "disponibles sans supplément.")
    note = NOTES.get((chap, tab, des))
    if note:
        body += " " + note
    for xl in xl_list:
        if xl in DIVERGENCES:
            body += " " + DIVERGENCES[xl]
            JOURNAL.append(f"DIVERGENCE DE MONTANT EXPOSÉE : Excel {xl} — {chap} / {d}")
    body += " Les prix s'entendent hors éco-participation."
    return emit(PREFIXE + titre, source_line(page, sc_id(sc), nature), body)


def gen_croisillons(rows, sc):
    """Amendement OC 3 de la règle T4 : maille par finition, deux prix par chunk."""
    index = defaultdict(list)
    for r in rows:
        v = r["v"]
        chap, tab, des = norm(v[C_CHAP]), norm(v[C_TAB]), norm(v[C_DES])
        if (chap, tab) not in TAB_CROISILLONS:
            continue
        if r["xl"] in EXCLUSIONS or v[C_HT] is None:
            continue
        index[(chap, tab, des, int(v[C_HT]))].append((r["xl"], v[C_TTC], norm(v[C_DET])))
    chunks, vus = [], set()
    for chap, tab, des, det, finition, ht_t, ht_f in CROISILLONS:
        des_n = norm(des)
        kt, kf = (chap, tab, des_n, ht_t), (chap, tab, des_n, ht_f)
        if kt not in index or kf not in index:
            JOURNAL.append(f"CROISILLON NON APPARIÉ, non généré : {chap} / {tab} / "
                           f"{des_n} / {ht_t} € et {ht_f} € introuvables dans l'Excel")
            continue
        vus |= {kt, kf}
        ttc_t, ttc_f = index[kt][0][1], index[kf][0][1]
        article = {"le": "du", "la": "de la", "les": "des"}[det]
        title = f"{PREFIXE}Tarif {article} {finition}"
        body = (f"Sur la {PRODUIT}, {det} {finition} est chiffré en plus-value à "
                f"{fmt_euro(ht_t)} € HT et {fmt_euro(ttc_t)} € TTC lorsqu'il comporte "
                f"une jonction en T ou en croix, et à {fmt_euro(ht_f)} € HT et "
                f"{fmt_euro(ttc_f)} € TTC lorsqu'il est filant, c'est-à-dire sans "
                f"jonction.")
        if det == "la":
            body = body.replace("lorsqu'il comporte", "lorsqu'elle comporte") \
                       .replace("lorsqu'il est filant", "lorsqu'elle est filante") \
                       .replace("est chiffré en", "est chiffrée en")
        body += phrase_unite(chap, tab, des_n, pluriel=True)
        if chap == "Calcul croisillons intégrés":
            body += (" Le discriminant de finition ne figure pas dans le fichier de "
                     "tarification : il est repris du tableau du tarif, page 28, et "
                     "rattaché par le montant.")
        body += " Les prix s'entendent hors éco-participation."
        chunks.append(emit(title, source_line(page_of(chap, tab), sc_id(sc)), body))
        sc += 1
    for k in index:
        if k not in vus:
            JOURNAL.append(f"CROISILLON NON COUVERT par la table de finitions : {k}")
    return chunks, sc


def gen_artdeco_volume(rows, sc):
    """Maille par motif : le montant est identique pour les trois types de gravure."""
    chap, tab = TAB_ARTDECO_VOLUME
    index = defaultdict(list)
    for r in rows:
        v = r["v"]
        if (norm(v[C_CHAP]), norm(v[C_TAB])) != (chap, tab):
            continue
        if r["xl"] in EXCLUSIONS or v[C_HT] is None:
            continue
        index[norm(v[C_DET])].append((r["xl"], v[C_HT], v[C_TTC]))
    chunks = []
    for det_excel, article, libelle, ht, note in MOTIFS_ARTDECO:
        lignes = index.get(det_excel, [])
        if not lignes:
            JOURNAL.append(f"MOTIF ART DÉCO sans ligne exploitable : {det_excel}")
            continue
        montants = {int(x[1]) for x in lignes}
        if montants != {ht}:
            JOURNAL.append(f"MOTIF ART DÉCO {det_excel} : montants Excel "
                           f"{sorted(montants)} contre {ht} attendu — non généré")
            continue
        ttc = lignes[0][2]
        art = {"le": "du", "la": "de la", "les": "des"}[article]
        pl = article == "les"
        title = f"{PREFIXE}Tarif {art} {libelle}"
        v = "sont chiffrés" if pl else "est chiffré"
        body = (f"Sur la {PRODUIT}, {article} {libelle} {v} en plus-value à "
                f"{fmt_euro(ht)} € HT, soit {fmt_euro(ttc)} € TTC.")
        if note:
            body += " " + note
        else:
            body += (" Ce montant est le même pour la gravure transparente sur vitrage "
                     "transparent et pour la gravure sablée sur vitrage transparent.")
            body += phrase_unite(chap, tab, None, pluriel=False)
        body += " Les prix s'entendent hors éco-participation."
        chunks.append(emit(title, source_line(page_of(chap, tab), sc_id(sc)), body))
        sc += 1
    for k in index:
        if k not in {m[0] for m in MOTIFS_ARTDECO}:
            JOURNAL.append(f"MOTIF ART DÉCO non couvert par la table : {k}")
    return chunks, sc


def gen_f3(rows):
    chunks, sc = [], 2
    # 1. croisillons et motifs, mailles propres
    c, sc = gen_croisillons(rows, sc)
    chunks += c
    c, sc = gen_artdeco_volume(rows, sc)
    chunks += c
    # 2. postes forfaitaires génériques, regroupement iso-prix ET iso-unité
    groupes = OrderedDict()
    for r in rows:
        v = r["v"]
        chap, tab, des = norm(v[C_CHAP]), norm(v[C_TAB]), norm(v[C_DES])
        if not chap or chap in CHAP_GRILLE or chap in CHAP_SPECIAUX \
                or chap in CHAP_EXCLUS or (chap, tab) in TAB_CROISILLONS \
                or (chap, tab) == TAB_ARTDECO_VOLUME:
            continue
        if r["xl"] in EXCLUSIONS:
            JOURNAL.append(f"EXCLUSION ARBITRÉE : Excel {r['xl']} — {EXCLUSIONS[r['xl']]}")
            continue
        if v[C_HT] is None:
            JOURNAL.append(f"LIGNE SANS MONTANT, non générée (anti-fantôme) : "
                           f"Excel {r['xl']} — {chap} / {tab} / {des}")
            continue
        unite = unite_of(chap, tab, des)[0]
        key = (chap, tab or None, des, int(v[C_HT]), int(v[C_TTC]), unite)
        groupes.setdefault(key, {"det": [], "xl": []})
        if norm(v[C_DET]):
            groupes[key]["det"].append(norm(v[C_DET]))
        groupes[key]["xl"].append(r["xl"])
    for (chap, tab, des, ht, ttc, _u), g in groupes.items():
        chunks.append(gen_poste(chap, tab, des, g["det"], ht, ttc, sc, g["xl"]))
        sc += 1
    # 3. postes à zéro relevés au PDF et absents de l'Excel
    for titre, page, corps in SANS_PV:
        body = corps.format(p=f"page {page}") + " Les prix s'entendent hors éco-participation."
        chunks.append(emit(PREFIXE + titre, source_line(page, sc_id(sc)), body))
        sc += 1
    return chunks


# ============================================================ F4 : châssis spéciaux
MENTION_OV = (" Ce poste appartient au chapitre des châssis fixes spéciaux du tarif "
              "TA76 OC, dont le tarif précise qu'il est à saisir en gamme TA76 OV. Le "
              "prix se lit donc bien au tarif de la fenêtre aluminium à ouvrant caché "
              "TA76 OC, mais la saisie de la commande s'effectue en TA76 OV.")


def gen_f4(rows):
    """Règle T5 redéfinie et règle OC1."""
    chunks, sc = [], 2
    groupes = OrderedDict()
    for r in rows:
        v = r["v"]
        chap, tab, des = norm(v[C_CHAP]), norm(v[C_TAB]), norm(v[C_DES])
        if chap not in CHAP_SPECIAUX:
            continue
        if v[C_HT] is None:
            JOURNAL.append(f"LIGNE SANS MONTANT, non générée : Excel {r['xl']} — {chap}")
            continue
        key = (chap, tab or None, des, int(v[C_HT]), int(v[C_TTC]))
        groupes.setdefault(key, []).append(r["xl"])
    for (chap, tab, des, ht, ttc), xls in groupes.items():
        titre, _, _ = libelle_poste(chap, tab, des, [])
        page = page_of(chap, tab, des)
        sujet = sujet_poste(chap, tab, des, [])
        virgule = "," if "," in sujet else ""
        body = (f"Sur la {PRODUIT}, le tarif chiffre {sujet}{virgule} à "
                f"{fmt_euro(ht)} € HT, soit {fmt_euro(ttc)} € TTC.")
        body += phrase_unite(chap, tab, des)
        body += MENTION_OV
        body += " Les prix s'entendent hors éco-participation."
        chunks.append(emit(PREFIXE + titre, source_line(page, sc_id(sc)), body))
        sc += 1
    return chunks


# ============================================================ F5 / F6 : statiques
def gen_statique(blocs, nature="originale"):
    chunks, sc = [], 2
    for titre, page, corps in blocs:
        body = corps.format(p=f"page {page}")
        chunks.append(emit(PREFIXE + titre, source_line(page, sc_id(sc), nature), body))
        sc += 1
    return chunks


# ============================================================ journal colonnes
def journal_colonnes(header, rows):
    mappees = set([C_CHAP, C_TAB, C_GAMME, C_CLE, C_DES, C_DET, 6, 7, C_HT, C_TTC,
                   C_HAUTEUR] + COLS_HT_L + COLS_TTC_L)
    for j in range(NCOLS):
        rempli = sum(1 for r in rows if r["v"][j] not in (None, ""))
        if rempli == 0:
            JOURNAL.append(f"COLONNE ENTIÈREMENT VIDE : « {header[j]} » (index {j})")
        elif j not in mappees:
            JOURNAL.append(f"COLONNE REMPLIE NON MAPPÉE : « {header[j]} » "
                           f"(index {j}, {rempli} valeurs)")
    for j in (10, 11):
        rempli = sum(1 for r in rows if r["v"][j] not in (None, ""))
        if rempli:
            chaps = {norm(r["v"][C_CHAP]) for r in rows if r["v"][j] not in (None, "")}
            JOURNAL.append(f"COLONNE « {norm(header[j])} » : {rempli} valeurs, non "
                           f"transcrites — elles ne concernent que le ou les chapitres "
                           f"{', '.join(sorted(chaps))}, hors périmètre")


# ============================================================ écriture
def write_file(fname, sous_type, chunks):
    os.makedirs(OUTDIR, exist_ok=True)
    path = os.path.join(OUTDIR, fname)
    with open(path, "w", encoding="utf-8") as f:
        f.write(yaml_front(sous_type, len(chunks)))
        f.write("\n".join(chunks))
    return path, len(chunks)


def main():
    header, rows, largeurs = load_rows()
    controle_gamme(rows)
    journal_colonnes(header, rows)

    sorties = [
        ("Tarif_TA76_OC_METHODE.md", "methode", gen_statique(F1_BLOCS)),
        ("Tarif_TA76_OC_PRIX_CHASSIS.md", "prix_chassis", gen_f2(rows, largeurs)),
        ("Tarif_TA76_OC_OPTIONS.md", "options", gen_f3(rows)),
        ("Tarif_TA76_OC_CHASSIS_SPECIAUX.md", "chassis_speciaux", gen_f4(rows)),
        ("Tarif_TA76_OC_FAISABILITES.md", "faisabilites", gen_statique(F5_BLOCS)),
        ("Tarif_TA76_OC_TRANSVERSES.md", "transverses", gen_statique(F6_BLOCS)),
    ]
    total = 0
    print("=" * 74)
    print("GÉNÉRATION DES CHUNKS — TARIF TA76 OC")
    print("=" * 74)
    for fname, st, chunks in sorties:
        path, n = write_file(fname, st, chunks)
        total += n
        print(f"  {fname:42} {n:4} chunks")
    print(f"  {'TOTAL':42} {total:4} chunks")

    if ALERTS:
        print("\n--- ALERTES (%d) ---" % len(ALERTS))
        for a in ALERTS:
            print("  !", a)
    print(f"\n--- JOURNAL ({len(JOURNAL)} entrées) ---")
    for j in JOURNAL:
        print("  -", j)
    return 1 if ALERTS else 0


if __name__ == "__main__":
    sys.exit(main())
