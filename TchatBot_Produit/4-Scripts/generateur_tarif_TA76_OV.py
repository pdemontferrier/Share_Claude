#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Générateur de chunks Markdown pour le tarif TA76 OV
(fenêtre aluminium à ouvrant visible, collection TRYBA ALUMINIUM).

Conforme à note_cadrage_migration_tarif_TA76_OV.md : règles T1 à T7 héritées de
T81 et amendées par TA76 OC, règles OC2 et OC3 (OC3 amendée), règle OC1 déclarée
sans objet, et règles propres OV1 (discrimination bidirectionnelle) et OV2
(double édition, second témoin TTC).

Six fichiers :
  F1 METHODE          cotes, lecture par bandes, vocabulaire        (règle T3)
  F2 PRIX_CHASSIS     les 14 grilles, dont 2 à un seul axe          (T1, T2)
  F3 OPTIONS          plus-values forfaitaires                      (règle T4)
  F4 CHASSIS_SPECIAUX pages 65-66, chapitre natif                   (règle T5)
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

XLSX = "/mnt/user-data/uploads/TA76_OV-infos-tarifs.xlsx"
FEUILLE = "Feuil2"                                # et non Feuil1 comme sur TA76 OC
PDF_SOURCE = "Tarif—TA76_OV—HT—19-06-2026.pdf"    # nom affiché dans la ligne de source
PDF_YAML = "Tarif_TA76_OV_HT_19-06-2026.pdf"      # nom dans le front matter
PDF_TTC_YAML = "Tarif_TA76_OV_TTC_19-06-2026.pdf"
OUTDIR = "/mnt/user-data/outputs"
PLAFOND = 200
GAMME = "TA76 OV"
DESIGNATION = "Fenêtre Aluminium à ouvrant visible"
PREFIXE = f"{GAMME} {DESIGNATION} — "
PRODUIT = "fenêtre aluminium à ouvrant visible TA76 OV"

# ============================================================ index de colonnes
# ATTENTION : l'ordre des colonnes diffère de celui de TA76 OC. La hauteur est
# ici en colonne 8 et non 16, les montants scalaires en 9 et 10 et non 8 et 9.
C_CHAP, C_TAB, C_GAMME, C_CLE, C_DES, C_DET = 0, 1, 2, 3, 4, 5
C_MENTION_HT, C_MENTION_TTC = 6, 7
C_HAUTEUR = 8
C_HT, C_TTC = 9, 10
C_DET_MT_HT, C_DET_MT_TTC = 11, 12
COLS_T_FILANT = [13, 14, 15, 16]     # entièrement vides, journalisées
COLS_HT_L = list(range(17, 77))      # Px L 100..6000 HT
COLS_TTC_L = list(range(77, 137))    # Px L 100..6000 TTC
NCOLS = 137

ALERTS, JOURNAL = [], []

# ============================================================ table des pages
# Établie contre les EN-TÊTES DE PAGE du PDF. Le sommaire général de la page 3
# est périmé (décalage croissant, de une à quatre pages) et omet deux chapitres ;
# les intercalaires de section le sont aussi, avec des décalages différents de
# quatre à six pages. Ni l'un ni les autres ne sont utilisés.
# L'audit revérifie chaque attribution en cherchant le montant sur la page citée.
PAGES = {
    ("1 OF", None): 10,
    ("1 OF", "1 V grande hauteur"): 10,
    ("2 OF", None): 11,
    ("2 OF", "2V grande hauteur"): 11,
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
    ("PV vitrages ornementaux", None, None):
        ("m2", "par mètre carré de surface vitrée du châssis"),
    ("PV composition libre", None, None):
        ("m2", "par mètre carré de surface vitrée du châssis"),
    ("Remplissage", "PV soubassement", None): ("m2", "par mètre carré de panneau"),
    ("Remplissage", "panneaux moulurés", None): ("m2", "par mètre carré de panneau"),
    ("Calcul croisillons intégrés", None, None): ("champ", "par champ"),
    ("Croisillons intégrés+grecque", "Croisillons en alu 10 mm", None):
        ("champ", "par champ"),
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
    ("Ferrage SA R20", "PV pour guide supp", None):
        ("sachet", "par sachet de dix pièces"),
    ("Ferrage SA R20", "PV selon long flexible", None): ("piece", "par pièce"),
    ("Ferrage SA R20", "PV pour compas de sécu", None): ("piece", "par pièce"),
    ("Ferrage SA R20", "Cmd spéciales pour SA", "F25"): ("piece", "par pièce"),
    ("Ferrage SA R20", "Cmd spéciales pour SA", "CEFI"): ("ensemble", "pour l'ensemble"),
    ("Ferrage porte-fenêtre R20", "Ensemble serrure SB", None):
        ("ensemble", "pour l'ensemble"),
    ("Ferrage porte-fenêtre R20", "Ensemble serrure CC", None):
        ("ensemble", "pour l'ensemble"),
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
    225: "Croisillon I45 en T ou croix à 49 € HT : le PDF page 28 ne porte qu'un "
         "seul prix pour I45 (36 €) — montant sans source ni discriminant, non "
         "généré (règle T4)",
    227: "Croisillon I45 filant à 38 € HT : le PDF page 28 ne porte qu'un seul prix "
         "pour I45 (24 €) — montant sans source ni discriminant, non généré (règle T4)",
    304: "Judas optique (47 € HT) : le mot « judas » n'apparaît nulle part dans les "
         "70 pages du tarif TA76 OV ; la référence existe sur H81 — contamination "
         "inter-gammes probable, non généré",
    195: "Panneau phonique en 36 mm : 0 € dans l'Excel, cellule vide au PDF page 27 "
         "— configuration non tarifée, non générée (anti-fantôme). L'absence est "
         "exposée dans le chunk du panneau phonique en 28 mm",
    196: "Panneau phonique en 44 mm : 0 € dans l'Excel, cellule vide au PDF page 27 "
         "— configuration non tarifée, non générée (anti-fantôme)",
    197: "Panneau phonique en 48 mm : 0 € dans l'Excel, cellule vide au PDF page 27 "
         "— configuration non tarifée, non générée (anti-fantôme)",
    246: "Motif Art Déco MG9, gravure transparente sur vitrage transparent : 0 € "
         "dans l'Excel contre une cellule fusionnée au PDF page 30 — lecture "
         "ambiguë, non généré, absence exposée dans le chunk MG9",
    248: "Motif Art Déco MG9, gravure transparente sur vitrage sablé : 0 € dans "
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
    "AK10123": "AK10100", "AK10255": "AK10131",
    "CAL-NRE": "CAL-N RE",
}

# Divergences exposées, jamais arbitrées en silence.
# 1. Décalage de rattachement de la page 41 : l'Excel attribue à chaque référence
#    le montant de la précédente. Arbitrage du 02/09/2026 : l'Excel fait foi pour
#    le montant (amendement OV de la règle OC3), la valeur du PDF est exposée.
# 2. Seuils portés à deux montants sur deux pages : aucune ne prévaut sur l'autre.
DIVERGENCES = {
    284: "Le tarif imprime pour cette mortaise, page 41, un montant de 46 € HT et "
         "66 € TTC. Le fichier de tarification porte la valeur servie ici : les deux "
         "valeurs sont exposées et la divergence doit être arbitrée par le service "
         "produits.",
    285: "Le tarif imprime pour cette grille, page 41, un montant de 57 € HT et "
         "82 € TTC. Le fichier de tarification porte la valeur servie ici : les deux "
         "valeurs sont exposées et la divergence doit être arbitrée par le service "
         "produits.",
    286: "Le tarif imprime pour cette grille, page 41, un montant de 85 € HT et "
         "122 € TTC. Le fichier de tarification porte la valeur servie ici : les deux "
         "valeurs sont exposées et la divergence doit être arbitrée par le service "
         "produits.",
    287: "Le tarif imprime pour cette grille, page 41, un montant de 98 € HT et "
         "140 € TTC. Le fichier de tarification porte la valeur servie ici : les deux "
         "valeurs sont exposées et la divergence doit être arbitrée par le service "
         "produits.",
    288: "Le tarif imprime pour cette grille, page 41, un montant de 105 € HT et "
         "150 € TTC. Le fichier de tarification porte la valeur servie ici : les deux "
         "valeurs sont exposées et la divergence doit être arbitrée par le service "
         "produits.",
    289: "Le tarif ne porte aucun prix pour cette référence : la grille NICOLL "
         "HF2245 n'apparaît que page 40, dans la table des limites d'implantation. "
         "Le montant servi provient du fichier de tarification et n'est pas imprimé "
         "au tarif ; il doit être confirmé par le service produits.",
    323: "Le tarif porte un second montant pour ce même seuil, de 188 € HT et "
         "269 € TTC la pièce, page 62, sous la référence AS10100. Les deux valeurs "
         "figurent au tarif et aucune ne prévaut sur l'autre ; la divergence doit "
         "être arbitrée par le service produits.",
    379: "Le tarif porte un second montant pour ce même seuil, de 196 € HT et "
         "280 € TTC la pièce, page 51. Les deux valeurs figurent au tarif et aucune "
         "ne prévaut sur l'autre ; la divergence doit être arbitrée par le service "
         "produits.",
    324: "Le tarif porte un second profil plinthe sur seuil pour fixe latéral, "
         "référencé 5120SN, à 96 € HT et 138 € TTC la pièce, page 62. Les deux "
         "postes figurent au tarif et aucun ne prévaut sur l'autre ; la divergence "
         "doit être arbitrée par le service produits.",
    378: "Le tarif porte un second profil plinthe sur seuil pour fixe latéral, "
         "référencé AK10131, à 99 € HT et 142 € TTC la pièce, page 51. Les deux "
         "postes figurent au tarif et aucun ne prévaut sur l'autre ; la divergence "
         "doit être arbitrée par le service produits.",
}

# ============================================================ grilles (règle T1)
# (chapitre, tableau) -> (libellé long, synonyme d'usage)
GRILLES_2D = OrderedDict([
    (("1 OF", None),
     ("châssis à 1 ouvrant à la française",
      "fenêtre à un vantail ouvrant à la française")),
    (("1 OF", "1 V grande hauteur"),
     ("châssis à 1 ouvrant à la française en grande hauteur",
      "fenêtre à un vantail ouvrant à la française de grande hauteur")),
    (("2 OF", None),
     ("châssis à 2 ouvrants égaux à la française",
      "fenêtre à deux vantaux ouvrants à la française")),
    (("2 OF", "2V grande hauteur"),
     ("châssis à 2 ouvrants égaux à la française en grande hauteur",
      "fenêtre à deux vantaux ouvrants à la française de grande hauteur")),
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
      "plaque d'habillage rectangulaire posée sur le double vitrage", "de l'")),
    (("Habillage Alu sur vitrage", "habillage cintrés"),
     ("habillage alu cintré sur vitrage",
      "plaque d'habillage cintrée posée sur le double vitrage", "de l'")),
])
# Amendement OV 1 de la règle T1 : les grilles grande hauteur prolongent l'échelle
# de hauteur de leur grille principale. Le pas de 100 mm et la règle de lecture de
# la page 9 suffisent à en déduire les bandes ; rien n'est inventé.
ECHELLE_H_HERITEE = {
    ("1 OF", "1 V grande hauteur"): ("1 OF", None),
    ("2 OF", "2V grande hauteur"): ("2 OF", None),
}
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
     "I10LF I10WF I10PFF",
     "le", "croisillon intégré en aluminium de 10 mm, teinte laiton (I10LF), blanche "
     "(I10WF) ou plomb foncé (I10PFF)", 22, 19),
    ("Croisillons Art Déco", "/ champ", "Gravure 10",
     "la", "gravure Art Déco de 10 mm de largeur sur le verre extérieur", 18, 13),
    ("Croisillons Art Déco", "/ champ", "Gravure 18",
     "la", "gravure Art Déco de 18 mm de largeur sur le verre extérieur", 18, 13),
    ("Croisillons rapportés", "2F en alu", "Grp de couleurs 1 sans PV",
     "le", "croisillon rapporté deux faces en aluminium (référence AK10208), pour un "
     "châssis du groupe de couleurs 1 sans plus-value", 28, 21),
    ("Croisillons rapportés", "2F en alu", "Grp de couleurs 2",
     "le", "croisillon rapporté deux faces en aluminium (référence AK10208), pour un "
     "châssis du groupe de couleurs 2", 32, 24),
    ("Croisillons rapportés", "2F en alu", "Autres grp de couleurs",
     "le", "croisillon rapporté deux faces en aluminium (référence AK10208), pour un "
     "châssis d'un autre groupe de couleurs", 35, 26),
]
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
    ("Seuil", "Seuil"): "Tarif du seuil de châssis spécial, référence {d}",
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
        "le|du|U d'assemblage et cache-rainure (références 30806 et 30813)",
    ("Elargisseurs, tapées", "Elargisseurs"):
        "l'|de l'|élargisseur et complément d'habillage (références AK10129 et AK10128)",
    ("Elargisseurs, tapées", "Tapées de doublage"):
        "la|de la|tapée de doublage (références A-T35, A-T46, A-T55, A-T66, A-T86 et A-TCV40) "
        "ou coulisse inversée pour TRYBA VS et VI Evolution (A/CT35, A/CT46, A/CT55 "
        "et A/CT66)",
    ("Bavettes ext", None):
        "la|de la|bavette extérieure (références 30604 B1, 30606 B2, 30608 B3, 30610 B4 et "
        "30612 B5)",
    ("Couvre_joints", "couvre-joints"):
        "le|du|couvre-joint intérieur ou extérieur (références 30801 C1, 30812 C3, 30802 C2, "
        "30814 C4, CJ02 et CJ70)",
    ("Couvre_joints", "couvre-joints spécial réno"):
        "le|du|couvre-joint spécial rénovation (références 12117 C5, CJ32/42, 30818 C6 "
        "et CJ22)",
}
TEINTES = {
    "Prix Blanc et RAL 7016 Gr": "en blanc ou en RAL 7016 granité",
    "Prix Teinte Std Grp1": "en teinte standard du groupe 1",
    "Prix autre couleur": "dans une autre couleur",
}
# descriptions reprises du PDF pour les postes dont l'Excel ne porte qu'un code
DESC_PDF = {
    ("Grilles d'entrée d'air", "FR15"):
        "mortaise de 250 x 12 mm pour grille Mini Eséa 30",
    ("Grilles d'entrée d'air", "FR22"):
        "mortaise de 172 x 12 mm pour grille Mini Eséa 22",
    ("Grilles d'entrée d'air", "FR12"):
        "mortaise de 354 x 12 mm pour grilles ISOLA2 et ISOLA HY",
    ("Grilles d'entrée d'air", "ME30_CE"):
        "grille Mini Eséa 30 m³/h avec capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ME22_CE"):
        "grille Mini Eséa 22 m³/h avec capot extérieur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ME30+RA_CE"):
        "grille Mini Eséa 30 m³/h avec rallonge et déflecteur, mortaise et vis comprises",
    ("Grilles d'entrée d'air", "ME22+RA_CE"):
        "grille Mini Eséa 22 m³/h avec rallonge et capot extérieur, mortaise et vis "
        "comprises",
    ("Grilles d'entrée d'air", "ISOLA2-45_CE"):
        "grille ISOLA2 de 45 m³/h intérieure avec capot extérieur, mortaise et vis "
        "comprises",
    ("Grilles d'entrée d'air", "ISOLA245+RA_CE"):
        "grille ISOLA2 de 45 m³/h avec rallonge et capot extérieur, mortaise et vis "
        "comprises",
    ("Grilles d'entrée d'air", "ISOLA-HY_CE"):
        "grille hygroréglable de 8 à 40 m³/h avec capot extérieur, mortaise et vis "
        "comprises",
    ("Grilles d'entrée d'air", "ISOLA-HY+RA_CE"):
        "grille hygroréglable de 8 à 40 m³/h avec rallonge et capot extérieur, "
        "mortaise et vis comprises",
    ("Grilles d'entrée d'air", "HF2245_CE"):
        "grille NICOLL HF2245, dont la largeur hors tout battant est de 539 mm",
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
        "pont thermique, incompatible avec l'oscillo-battant inversé",
    ("Seuil", "AS10100-RA1"):
        "seuil avec rallonge de 81 mm, anodisé nature ou anodisé noir",
    ("Seuil", "AS10100-RA2"):
        "seuil avec rallonge de 110 mm, anodisé nature ou anodisé noir",
    ("Seuil", "5263"): "bouclier de protection pour porte-fenêtre",
    ("Ferrage porte-fenêtre R20", "AK10100"):
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
    ("Crémones à l'ancienne", "CAL-N RE"): "finition noir mat",
    ("Crémones à l'ancienne", "CAL-LA"): "finition laiton poli",
    ("Crémones à l'ancienne", "CAL-LAV"): "finition laiton vieilli",
    ("Crémones à l'ancienne", "CAL-FP"): "finition fer patiné",
    ("Crémones à l'ancienne", "CAL-TRI-TBRL"): "finition mixte TBRL",
    ("Crémones à l'ancienne", "CAL-TRI-TLRB"): "finition mixte TLRB",
}
# notes complémentaires accrochées à un poste précis
NOTES = {
    ("Remplissage", "PV soubassement", "Panneau phonique (Rw = 38 dB) Groupe 1 sans PV"):
        "Le tarif ne porte aucune valeur pour ce panneau phonique en 36, 44 et 48 mm : "
        "ces trois épaisseurs ne sont pas tarifées page 27.",
    ("Meneaux complémentaires", "Meneaux battants", "Prix HT"):
        "Le tarif précise que les meneaux battants sont réalisables en T, en croix ou "
        "en filant, et qu'une plus-value de couleur s'ajoute sur les châssis de couleur.",
    ("Poignées", "En option", "Autres poignées"):
        "La poignée Toulon est sans plus-value lorsqu'elle est incluse au pack "
        "Trybadesign ; le montant indiqué vaut hors pack Trybadesign.",
    ("Ferrage porte-fenêtre R20", "Seuil universel", "AK10123"):
        "Le tarif précise que ce seuil est incompatible avec le ferrage invisible.",
}

# sujet employé dans le corps du chunk : groupe nominal complet, déterminant
# compris. {d} = référence du PDF, {desc} = description reprise du PDF,
# {t} = variantes de la colonne Détails.
SUJETS = {
    ("Laquage bloc-baie", "Forfait"): "le forfait de laquage du volet roulant {desc}",
    ("Vitrage-généralités", "Vitrage d'altitude"):
        "la plus-value pour vitrage d'altitude",
    ("PV vitrages", None): "le vitrage {d}",
    ("PV vitrages ornementaux", None): "le vitrage ornemental {d}",
    ("PV composition libre", None): "le vitrage en composition libre {d}",
    ("Remplissage", "PV soubassement",
     "Panneau phonique (Rw = 38 dB) Groupe 1 sans PV"):
        "le panneau de soubassement phonique d'indice Rw = 38 dB, en teinte du "
        "groupe 1, de {t} mm d'épaisseur",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe 1 sans PV"):
        "le panneau de soubassement standard en teinte du groupe 1, de {t} mm "
        "d'épaisseur",
    ("Remplissage", "PV soubassement", "Panneau standard Groupe 2 PV 15 %"):
        "le panneau de soubassement standard en teinte du groupe 2, de {t} mm "
        "d'épaisseur",
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
        "références AM10103 (MB56) et AM10104 (MB77), pour un vitrage de 28, 36 ou "
        "48 mm",
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
     "référence pour la commande et la fabrication des châssis. Le tarif l'écrit "
     "ainsi : les cotes prix sont L exposant T par H exposant T, les cotes fabrication "
     "sont L par H. Un prix lu sur la mauvaise cote est un prix faux : avant tout "
     "chiffrage, il faut établir si la dimension fournie est une cote de tarif ou une "
     "cote de fabrication."),
    ("Passage de la cote de fabrication à la cote de tarif selon le dormant", 6,
     "Sur la {p}, le passage d'un jeu de cotes à l'autre suit deux régimes sur la "
     "fenêtre aluminium à ouvrant visible TA76 OV. Pour un "
     "châssis sans complément, les cotes de tarif sont égales aux cotes de "
     "fabrication, et cela vaut pour le dormant neuf sans ailette AK10130J (L74), pour "
     "le dormant AK10117J (L83), pour le dormant AK10130J (L74) en version fixe, ainsi "
     "que pour les dormants à ailette AK10120J (LZ109) et AK10121J (LZ139). Pour un "
     "châssis avec compléments, toutes teintes, les cotes de tarif désignent le châssis "
     "nu tandis que les cotes de fabrication incluent les compléments. Sur un dormant à "
     "ailette avec appui, la hauteur de tarif s'obtient en retranchant de la hauteur de "
     "fabrication l'épaisseur de la pièce d'appui."),
    ("Épaisseur des pièces d'appui à déduire pour obtenir la hauteur de tarif", 57,
     "Sur la {p}, le tarif indique que les pièces d'appui se commandent sous forme de "
     "compléments, que leur plus-value s'ajoute à la valeur du châssis nu, et que "
     "l'épaisseur de la pièce d'appui employée doit être déduite de la hauteur de "
     "fabrication pour obtenir la hauteur de tarif du châssis. Les pièces d'appui neuf "
     "NF-TA84 et rénovation NF-TA84-D ont une hauteur de 20 mm ; les pièces d'appui "
     "courte AK10118 et longue AK10119 ont une hauteur de 20,5 mm. Le dormant "
     "compatible diffère selon la pièce : la pièce d'appui neuf convient aux dormants "
     "AK10130 (L74) et AK10117 (L83), la pièce rénovation aux dormants AK10120 (LZ109), "
     "AK10121 (LZ139) et AK10122 (L69), les pièces courte et longue au dormant "
     "AK10102 (L85)."),
    ("Lecture des grilles de prix par bandes de dimensions", 9,
     "Sur la {p}, le tarif énonce sa règle de lecture des grilles de prix. Une valeur "
     "de grille ne correspond pas à une dimension ponctuelle mais à une bande de "
     "dimensions : la colonne intitulée 1000 couvre les largeurs de 901 à 1000 mm, et "
     "la ligne intitulée 600 couvre les hauteurs de 501 à 600 mm. Une demande dont les "
     "cotes tombent à l'intérieur d'une bande se lit donc directement sur la valeur de "
     "cette bande, sans arrondi ni interpolation. La même page précise que les prix "
     "indiqués sont des valeurs pour châssis sans compléments, et que le prix lu est "
     "celui du châssis de base, vitrage standard compris."),
    ("Valeur d'abaque des grilles de prix et limites de fabrication", 47,
     "Sur la {p}, le tarif indique que les grilles de prix des différents types de "
     "châssis définissent les limites de fabrication des châssis TA76 OV et peuvent de "
     "ce fait être utilisées comme abaques. Le contour d'une grille est donc une limite "
     "de fabrication : une dimension qui sort de la grille n'est pas une dimension dont "
     "le prix serait à chercher ailleurs, c'est une configuration qui n'est pas "
     "réalisable au tarif. Les grilles précisent par ailleurs que leurs limites "
     "dimensionnelles valent jusqu'à un vitrage de 50 kg par mètre carré."),
    ("Composition d'un prix et conduite à tenir devant une demande de chiffrage", 9,
     "Sur la fenêtre aluminium à ouvrant visible TA76 OV, le prix d'un ensemble se "
     "compose du prix de grille du châssis, lu sur le type "
     "d'ouverture et les deux cotes de tarif, auquel s'ajoutent les plus-values des "
     "options retenues, chacune servie avec son unité de facturation. Trois éléments "
     "sont nécessaires avant de restituer un prix de châssis : le type d'ouverture, la "
     "cote de tarif en largeur et la cote de tarif en hauteur. Aucun total ne doit être "
     "calculé ni aucune valeur interpolée : les montants sont restitués tels qu'ils "
     "figurent au tarif, et l'addition des postes revient à l'ADV."),
    ("Cotes utiles et dimensions du clair de vitre intérieur", 7,
     "Sur la {p}, le tarif donne les cotes utiles, c'est-à-dire les dimensions du clair "
     "de vitre intérieur hors joint, pour chaque combinaison de dormant et d'ouvrant. "
     "Deux tables coexistent : l'une pour l'ouvrant standard AK10100J (Z64), l'autre "
     "pour l'ouvrant SB/CC AM10105J (Z109). Les valeurs y sont données position par "
     "position, du dormant seul au meneau fixe accompagné d'un ouvrant, pour les "
     "dormants AK10130J (L74) neuf, AK10117J (L83) store et AK10120J (LZ109) "
     "rénovation, ainsi que pour les meneaux dormants AK10115J (MD114) et AK10116J "
     "(MD141). Ces cotes ne servent pas au chiffrage mais au contrôle du clair de jour."),
    ("Vocabulaire et abréviations du tarif", 8,
     "Le tarif emploie des abréviations constantes qu'il faut savoir lire. OV désigne "
     "l'ouvrant visible, qui caractérise la gamme entière et dont le profilé porte la "
     "référence AM10100J. OF désigne l'ouverture à la française et OB "
     "l'oscillo-battant, dont il existe une variante inversée. SN désigne le soufflet "
     "normal et SA le soufflet d'aération avec ferme-imposte. SB et CC désignent deux "
     "ensembles de serrures de porte-fenêtre. LF désigne le levier en feuillure du "
     "battant semi-fixe. Les notations 1V et 2VTX désignent respectivement un vantail "
     "et deux vantaux."),
    ("Statut du PDF comme document de référence du tarif", 2,
     "Sur la {p}, le tarif indique que les spécifications techniques et les "
     "informations tarifaires qu'il contient ont été validées par le Service Produits, "
     "et que toute construction qui ne peut être traitée à l'aide du tarif n'est pas "
     "réalisable. Il précise également que les logiciels Look et Syscon étant en "
     "constante évolution, cette version PDF reste le seul document de référence, et "
     "invite à s'y référer en cas de doute. Les prix sont exprimés en euros, hors "
     "éco-participation."),
]

# postes tarifés à zéro par le PDF et absents de l'Excel (amendement OV de T4)
SANS_PV = [
    ("Tarif des poignées standard Lento, Liège et Toulon", 35,
     "Sur la {p}, le tarif porte les poignées standard sans plus-value : la poignée "
     "Lento, référence PLENTO, la poignée Liège, référence PLIEGE, et la poignée "
     "Toulon, référence PTOULON, sont proposées à 0 € HT et 0 € TTC. Ces trois "
     "poignées sont disponibles en blanc, en titane ou en noir. Leur choix n'entraîne "
     "donc aucune plus-value sur le prix du châssis."),
    ("Absence de plus-value de la poignée Toulon incluse au pack Trybadesign", 35,
     "Sur la {p}, le tarif indique que la poignée Toulon, référence PTOULON, laquée à "
     "la teinte du châssis, est incluse au pack Trybadesign et n'entraîne alors aucune "
     "plus-value, soit 0 € HT et 0 € TTC. Hors pack Trybadesign, la même poignée est "
     "chiffrée : son montant se lit sur la même page."),
    ("Tarif des meneaux dormants complémentaires", 38,
     "Sur la {p}, le tarif porte les meneaux dormants complémentaires sans plus-value : "
     "les références AK10115J (MD114) et AK10116J (ZM89) sont proposées à 0 € HT et "
     "0 € TTC. L'une comme l'autre peut être disposée en T, en croix ou en filant. "
     "Contrairement aux meneaux battants, placés dans les ouvrants, les meneaux "
     "dormants n'entraînent donc aucune plus-value."),
    ("Tarif du profil d'ouvrant des parecloses", 39,
     "Sur la {p}, le tarif porte le profil d'ouvrant AM10100J (Z64) sans plus-value, "
     "soit 0 € HT et 0 € TTC, pour un vitrage de 28, 36 ou 48 mm. Ce profil est celui "
     "de l'ouvrant visible de la gamme et son emploi n'entraîne aucun supplément."),
    ("Tarif des parecloses pour châssis fixe", 39,
     "Sur la {p}, le tarif porte les parecloses pour châssis fixe sans plus-value, soit "
     "0 € HT et 0 € TTC, quel que soit le design retenu. En vitrage de 28 mm, les "
     "références sont AK10203 en design droit et AK10206 en design galbé ; en 36 mm, "
     "AK10202 en design droit et AK10205 en design galbé ; en 48 mm, AM10204 en design "
     "droit, le design galbé n'étant pas disponible à l'offre."),
    ("Tarif des parecloses pour ouvrants", 39,
     "Sur la {p}, le tarif porte les parecloses pour ouvrants sans plus-value, soit "
     "0 € HT et 0 € TTC, quel que soit le design retenu. En vitrage de 28 mm, les "
     "références sont AM10201 en design droit et AM10205 en design galbé ; en 36 mm, "
     "AM10202 en design droit et AK10206 en design galbé ; en 48 mm, AM10203 en design "
     "droit, le design galbé n'étant pas disponible à l'offre."),
    ("Tarif du levier en feuillure du battant semi-fixe", 46,
     "Sur la {p}, le tarif porte le levier en feuillure sans plus-value, soit 0 € HT et "
     "0 € TTC. Ce système de verrouillage équipe systématiquement le battant du "
     "semi-fixe à partir d'une hauteur de battant supérieure à 591 mm ; en dessous de "
     "590 mm, c'est un verrou à levier qui est monté. L'un comme l'autre est compris "
     "dans le prix du châssis."),
    ("Tarif du ferrage de sécurité TRYBASAFE R20", 46,
     "Sur la {p}, le tarif porte le ferrage TRYBASAFE R20 en standard sur les fenêtres "
     "et les portes-fenêtres, soit 0 € HT et 0 € TTC. Il comprend des ferrures de "
     "sécurité dotées de trois galets champignons au minimum, l'assemblage des cadres "
     "coupés d'onglet par sertissage et collage, des paumelles vissées dans les parois "
     "aluminium des dormants et des battants, et des gâches de sécurité à galet "
     "champignon évitant le décrochage de la fenêtre."),
    ("Tarif du ferrage symétrique", 46,
     "Sur la {p}, le tarif porte le ferrage symétrique en standard, soit 0 € HT et "
     "0 € TTC. Il est donc compris dans le prix du châssis et ne donne lieu à aucune "
     "plus-value, contrairement à l'entrebâilleur en ouverture à la française, à "
     "l'oscillo-battant et à l'oscillo-battant inversé, qui sont chiffrés sur la même "
     "page."),
    ("Tarif des paumelles selon le type de ferrage", 53,
     "Sur la {p}, le tarif porte les paumelles sans plus-value, soit 0 € HT et 0 € TTC, "
     "quel que soit le type retenu. La paumelle P60 admet un poids maximal de 60 kg, le "
     "compas OF et le ferrage OB un poids maximal de 100 kg, le ferrage invisible un "
     "poids maximal de 130 kg. Les paumelles P60, compas OF et ferrage OB sont "
     "disponibles en blanc, en titane et en noir."),
    ("Tarif du profilé de finition pour seuil AS20200", 62,
     "Sur la {p}, le tarif porte le profilé de finition AS20200, destiné au seuil "
     "AS10100 et disponible en anodisé nature ou en anodisé noir, sans plus-value, soit "
     "0 € HT et 0 € TTC. Ce profilé ne figure pas dans le fichier de tarification : sa "
     "gratuité est établie contre le tarif."),
]

F5_BLOCS = [
    ("Faisabilité des châssis fixes, des ouvrants et des portes-fenêtres", 8,
     "Sur la {p}, le tarif donne la faisabilité des châssis en TA76 OV. Les châssis "
     "fixes sont réalisables dans toutes les compositions présentées, y compris les "
     "formes à angle. Les ouvrants simples et à traverse sont réalisables. Les ouvrants "
     "comportant des meneaux en croix ou en T sont marqués en cours d'étude et ne sont "
     "donc ni réalisables ni tarifés à ce jour. L'ouvrant de forme trapèze n'est pas "
     "réalisable. Les portes-fenêtres SB/CC sont réalisables."),
    ("Restrictions du ferrage R20 et du ferrage R20 invisible", 45,
     "Sur la {p}, le tarif oppose le ferrage R20 au ferrage R20 invisible, dont "
     "l'ouverture est limitée à 110 degrés. Le seuil et le SB-CC sur seuil sont "
     "compatibles avec le ferrage R20 mais pas avec le ferrage R20 invisible. "
     "L'oscillo-battant inversé, l'entrebâilleur en ouverture à la française et le "
     "SB-CC sont compatibles avec les deux ferrages. Le tarif ajoute que "
     "l'entrebâilleur en ouverture à la française n'est compatible ni avec le seuil ni "
     "avec un châssis cintré."),
    ("Incompatibilités du pack Trybadesign", 52,
     "Sur la {p}, le tarif indique que le pack Trybadesign, qui apporte le ferrage "
     "invisible et la poignée Toulon laquée à la teinte du châssis, est incompatible "
     "avec le seuil et avec le SB/CC sur seuil. Il précise également que l'ouverture "
     "maximale du vantail en ferrage invisible est de 110 degrés, et que les organes de "
     "rotation invisibles sont clamés dans les parois aluminium des dormants et des "
     "battants."),
    ("Règles de ferrage et limites dimensionnelles des grilles de prix", 10,
     "Sur la {p}, le tarif énonce les règles de ferrage communes aux grilles de prix "
     "des châssis à ouvrant. La largeur du battant ne peut excéder une fois et demie sa "
     "hauteur, sa surface ne peut excéder 2,4 mètres carrés, et le poids maximal par "
     "vantail est de 100 kg. Le ferrage oscillo-battant est réalisable jusqu'à une "
     "hauteur de 2600 mm au maximum. Les limites dimensionnelles des grilles valent "
     "jusqu'à un vitrage de 50 kg par mètre carré ; au-delà, il faut se reporter aux "
     "pages des limites de fabrication du battant."),
    ("Restriction du seuil aluminium et verrouillage du battant semi-fixe", 11,
     "Sur la {p}, le tarif indique que les châssis à un ouvrant comme à deux ouvrants "
     "égaux à la française ne sont pas réalisables avec un seuil aluminium. La même "
     "page précise le verrouillage du battant semi-fixe : le levier en feuillure "
     "équipe le battant à partir d'une hauteur de 550 mm, le verrou à levier étant "
     "monté en dessous de cette hauteur."),
    ("Limites de fabrication du battant selon l'épaisseur du vitrage", 54,
     "Les pages 54 et 55 du tarif portent quatre abaques donnant, pour un vitrage de "
     "25, 30, 35 puis 40 kg par mètre carré, les couples de dimensions de battant "
     "réalisables et le type de paumelles admissible. Chaque case y est colorée selon "
     "que la configuration accepte la paumelle P60, le compas OF et le ferrage OB, ou "
     "seulement le compas OF et le ferrage OB. Ces abaques ne sont pas transcrites ici : "
     "elles se lisent directement sur les pages 54 et 55, en croisant la largeur et la "
     "hauteur du battant."),
    ("Poids maximal admissible par type de paumelles", 53,
     "Sur la {p}, le tarif donne le poids maximal admissible de chaque type de "
     "paumelles. La paumelle P60 admet 60 kg au maximum, le compas OF 100 kg, le "
     "ferrage OB 100 kg et le ferrage invisible 130 kg. Ce poids est celui du battant "
     "complet, vitrage compris, et conditionne le choix du ferrage. Les paumelles "
     "relèvent donc de la tenue et de la durabilité du châssis."),
    ("Faisabilités des poignées selon la configuration du châssis", 37,
     "Sur la {p}, le tarif croise chaque poignée et chacune de ses teintes avec les "
     "configurations un vantail, deux vantaux, soufflet normal, SB-CC, poignée à clé "
     "100N et oscillo-battant inversé. La poignée Lento existe en blanc, noir et titane "
     "laqué brillant mais n'est jamais compatible avec le SB-CC. La poignée Liège est "
     "la seule compatible avec le SB-CC. La poignée Toulon accepte en outre le RAL "
     "granité sur les trois premières configurations. Les poignées Lilly, Hélène et "
     "Camille ont des faisabilités plus restreintes, lisibles sur la page."),
    ("Limites d'utilisation des crémones à l'ancienne", 36,
     "Sur la {p}, le tarif donne les limites d'utilisation de la crémone à l'ancienne "
     "selon la hauteur extérieure du battant. À 2000 mm, elle convient à la fenêtre en "
     "ouverture à la française avec béquille en option, à la fenêtre oscillo-battante "
     "et aux deux configurations de porte-fenêtre. À 2400 mm, elle ne convient qu'aux "
     "configurations en ouverture à la française. À 2500 mm, aucune configuration n'est "
     "admise. Le tarif précise que les logiciels Look et Syscon ne sont pas bloqués sur "
     "ces limites préconisées."),
    ("Faisabilités de composition des triples vitrages", 22,
     "Sur la {p}, le tarif fixe l'ordre de priorité à respecter pour composer un triple "
     "vitrage. Le vitrage solaire TRYBASUN ou STOPSOL se place côté extérieur ; le "
     "vitrage ornemental ou à croisillons Art Déco se place au milieu ; le vitrage à "
     "couche Isol'3 se place côté intérieur et extérieur ; le vitrage feuilleté se place "
     "côté intérieur, et également à l'extérieur en cas d'allège ; le vitrage phonique "
     "se place sans contrainte de position. Le tarif ajoute qu'il est impératif d'avoir "
     "un vitrage à couche thermique côté intérieur et côté extérieur."),
    ("Faisabilités de composition des doubles vitrages", 22,
     "Sur la {p}, le tarif fixe l'ordre de priorité à respecter pour composer un double "
     "vitrage. Le vitrage solaire TRYBASUN ou STOPSOL se place côté extérieur ; le "
     "vitrage ornemental ou à croisillons Art Déco se place côté extérieur ; le vitrage "
     "à couche Isol'3 se place côté intérieur ; le vitrage feuilleté se place côté "
     "intérieur, et également à l'extérieur en cas d'allège. Le tarif ajoute qu'il est "
     "impératif d'avoir au moins un vitrage à couche thermique, et que le rapport "
     "maximal entre la largeur et la hauteur est de six pour les verres de 4 mm."),
    ("Vitrages exclus de la certification Cekal", 22,
     "Sur la {p}, le tarif indique que tous les vitrages sont titulaires de la "
     "certification Cekal, à quatre exceptions près : les vitrages Art Déco, les "
     "vitrages à croisillons laiton ou couleur, le vitrage Cathédrale, et les vitrages "
     "de petites dimensions, c'est-à-dire inférieurs à 350 par 350 mm avec un écarteur "
     "de 16 mm, ou inférieurs à 410 par 410 mm avec un écarteur de 20 mm. La même page "
     "précise que les vitrages de dimension inférieure à 190 par 350 mm ne peuvent pas "
     "être fabriqués en TPS et reçoivent des écarteurs traditionnels en inox noir."),
    ("Conditions de mise en œuvre du vitrage d'altitude", 23,
     "Sur la {p}, le tarif impose une saisie spéciale « vitrage d'altitude » pour toute "
     "fabrication destinée à un site situé à plus de 600 mètres d'altitude en triple "
     "vitrage, ou à plus de 1000 mètres en double vitrage. Cette saisie est "
     "conditionnée à un écarteur de 12 mm sans croisillons, ou de 14 mm avec "
     "croisillons. Le tarif précise que des déformations optiques convexes ou concaves "
     "peuvent apparaître sous l'effet de la pression ou de la température et ne peuvent "
     "motiver un remplacement du vitrage."),
    ("Contraintes de transport et d'accouplement des châssis", 23,
     "Sur la {p}, le tarif indique que les châssis sont livrés en cassette, sanglés et "
     "protégés par des cales en polystyrène, sauf pour un ensemble dont une dimension "
     "dépasse 2,20 mètres, cas qui impose de contacter le service expédition. À partir "
     "d'une dimension de vitrage supérieure à 2000 par 2000 mm ou d'un poids supérieur "
     "à 80 kg, le vitrage est livré non posé. Tout châssis dont la largeur ou la "
     "hauteur dépasse 2500 mm doit être réalisé en plusieurs parties avec accouplement."),
    ("Vitrages ornementaux indisponibles en triple vitrage", 25,
     "Sur la {p}, le tarif indique que les verres Opale, Delta, Gothique et Mastercarré "
     "ne sont pas disponibles en triple vitrage pour des raisons techniques. Les verres "
     "Chinchilla, Cathédrale, Granité, Dépoli, Sablé et Stopsol sont disponibles aussi "
     "bien en double qu'en triple vitrage. La même page rappelle qu'une surface "
     "minimale de facturation s'applique aux plus-values de vitrage."),
    ("Dimensions limites des panneaux de remplissage", 27,
     "Sur la {p}, le tarif fixe les dimensions limites des panneaux. Un panneau "
     "standard mesure au minimum 195 par 195 mm et au maximum 3000 par 1500 mm, la "
     "hauteur minimale d'un soubassement étant de 250 mm. Un panneau mouluré mesure au "
     "minimum 270 par 270 mm et au maximum 2000 par 1000 mm ou 1000 par 2000 mm. Le "
     "tarif ajoute que les tons bois et l'anodisation sont impossibles sur un panneau "
     "mouluré, les autres teintes RAL étant soumises à une demande de faisabilité."),
    ("Méthode de calcul du prix d'un soubassement", 27,
     "Sur la {p}, le tarif décrit la méthode de calcul du prix d'un soubassement. Il "
     "faut partir du prix du châssis complet, vitrage compris, chercher dans la grille "
     "de prix des châssis fixes la valeur vitrage correspondant à la taille du "
     "remplissage, calculer la plus-value du panneau, l'ajouter au châssis, puis "
     "ajouter le prix de la traverse et de ses fixations. Les soubassements standards "
     "de 350 mm de hauteur sont précalculés au bas des grilles de prix par groupe de "
     "couleurs. Ce calcul revient à l'ADV."),
    ("Méthode de calcul du prix des croisillons au nombre de champs", 28,
     "Sur la {p}, le tarif indique que le prix des croisillons, incorporés comme "
     "rapportés, se calcule au nombre de champs et non au mètre linéaire. Un champ est "
     "une surface de vitrage délimitée par les croisillons, le dormant ou l'ouvrant. "
     "Deux types de champs sont distingués : le cas A, où les croisillons comportent "
     "une jonction en T ou en croix, et le cas B, où ils sont filants, c'est-à-dire "
     "sans jonction. Le décompte des champs revient à l'ADV."),
    ("Conditions de pose et de garantie des croisillons intégrés", 28,
     "Sur la {p}, le tarif indique que quatre types de croisillons incorporés peuvent "
     "équiper les fenêtres : en alu laqué de 18 mm en blanc, laqué ou Chêne d'Or, en "
     "alu laqué de 26 mm en blanc, laqué ou tons bois, et en alu de 10 mm en teinte "
     "laiton, blanc ou plomb foncé. Pour bénéficier de la garantie TRYBA, les "
     "croisillons doivent être incorporés en laissant au moins 2 mm entre le croisillon "
     "et le vitrage. Un espace de 1 mm reste conforme au certificat Cekal mais n'est "
     "pas couvert par la garantie."),
    ("Position des croisillons et des écarteurs selon le type de vitrage", 29,
     "Sur la {p}, le tarif indique que les écarteurs de vitrage des châssis à "
     "croisillons intégrés sont traditionnels, en inox noir, ou en TPS selon les "
     "impératifs techniques. Dans le cas d'un triple vitrage, les croisillons se "
     "situent entre le verre intermédiaire et le verre extérieur. La même page précise "
     "que les croisillons de 10 mm en teinte blanche et plomb foncé sont laqués, tandis "
     "que la teinte laiton est obtenue par anodisation."),
    ("Dimensions minimales des motifs de croisillons à la grecque", 29,
     "Sur la {p}, le tarif fixe les dimensions minimales de vitrage permettant un "
     "croisillon à la grecque : 400 par 400 mm sur un châssis à un vantail, 250 par "
     "400 mm sur un châssis à deux vantaux. La cote X sur Y du motif dépend de la "
     "taille du vitrage et se lit dans le tableau de la même page. Un croisillon "
     "intermédiaire est ajouté à partir d'une cote Y supérieure à 1700 mm."),
    ("Restrictions de gravure des motifs Art Déco", 30,
     "Sur la {p}, le tarif restreint la gravure Art Déco au vitrage de 6 mm uniquement. "
     "Aucune gravure n'est possible sur un vitrage ornemental ni sur un vitrage "
     "d'altitude, et il est impossible de combiner un Isol'3 associé à un vitrage "
     "TRYBASUN avec un motif Art Déco. La gravure de 18 mm de largeur n'est réalisable "
     "que sablée sur vitrage transparent : les deux gravures transparentes y sont "
     "marquées non réalisables. La taille maximale d'un vitrage gravé est de 1600 par "
     "2500 mm."),
    ("Incompatibilités des croisillons rapportés en double vitrage", 31,
     "Sur la {p}, le tarif indique qu'en double vitrage les croisillons rapportés sont "
     "incompatibles avec les vitrages décoratifs Cathédrale et Delta blanc. Les "
     "croisillons rapportés deux faces en aluminium, référence AK10208, sont par "
     "ailleurs disponibles en laquage simple comme en laquage double face."),
    ("Disponibilité des parecloses selon l'épaisseur du vitrage", 39,
     "Sur la {p}, le tarif indique que les parecloses, pour châssis fixe comme pour "
     "ouvrants, existent en design droit et en design galbé pour les vitrages de 28 et "
     "36 mm, mais que le design galbé n'est pas disponible à l'offre pour le vitrage de "
     "48 mm. Il précise également qu'en cas de parecloses galbées, les parecloses "
     "hautes et basses restent droites."),
    ("Règles d'implantation des grilles d'entrée d'air", 40,
     "Sur la {p}, le tarif fixe trois règles d'implantation des grilles d'entrée d'air. "
     "En présence d'un volet roulant, la grille est posée sur la trappe de visite du "
     "caisson. Sur l'ouvrant, elle est placée sur la traverse haute d'un battant, "
     "prioritairement le battant semi-fixe. Sur le dormant, dans le cas d'un châssis "
     "neuf AK10130, il faut prévoir un élargisseur AK10129 en partie haute et y insérer "
     "une grille GV_E. Le tarif insiste sur le centrage de l'entrée d'air sur la "
     "mortaise."),
    ("Limites d'utilisation des grilles d'entrée d'air selon le châssis", 40,
     "Sur la {p}, le tarif donne les largeurs minimales permettant de poser une grille "
     "centrée sur le clair de jour du vitrage en conservant des gardes latérales de "
     "20 mm, sur le dormant AK10130 (L74). La Mini ESEA 30 comme la Mini ESEA 22 "
     "demandent 387 mm de largeur hors tout battant en un ou deux vantaux, 774 mm en "
     "soufflet normal et 820 mm de largeur hors tout dormant en soufflet d'aération. "
     "L'ISOLA 45 et l'ISOLA HY demandent 522 mm, 1044 mm et 1044 mm. La NICOLL HF2245 "
     "demande 539 mm et n'est admise qu'en un ou deux vantaux."),
    ("Restriction de teinte des grilles Invisivent", 42,
     "Sur la {p}, le tarif indique que pour un châssis laqué nature, en tons bois ou en "
     "teinte Rouille, le passage est forcé sur l'Invisivent noire. La même page précise "
     "que la grille THM90 EVO n'est disponible qu'en vitrage de 28 mm, que la largeur "
     "totale de l'Invisivent EVO est égale à la largeur de fabrication, et que celle du "
     "THM90 EVO est égale à la largeur du vitrage diminuée de 6 mm."),
    ("Restrictions de pose des chatières", 43,
     "Sur la {p}, le tarif indique que les chatières ne sont disponibles que pour des "
     "panneaux ou des vitrages de 28 et 36 mm. Le panneau ou le vitrage doit mesurer au "
     "minimum 340 par 340 mm, le passage de l'animal étant de 146 par 135 mm et la "
     "largeur d'épaule maximale de l'animal de 150 mm. Sur un panneau aluminium, le "
     "tarif recommande la chatière à puce pour limiter l'effet de cage de Faraday ; "
     "elle n'est pas disponible sur panneau renforcé."),
    ("Implantation du levier de commande du soufflet d'aération", 48,
     "Sur la {p}, le tarif fixe l'implantation du levier de commande du soufflet "
     "d'aération, posé au même niveau que la poignée du châssis à un ou deux vantaux. "
     "Sur une fenêtre à un battant, le levier est toujours du côté de la poignée, avec "
     "ou sans volet roulant. Sur une fenêtre à deux vantaux, il est du côté du vantail "
     "secondaire sans volet roulant, et du côté opposé à la commande avec volet "
     "roulant. En présence d'un volet roulant, la commande est placée côté paumelles. "
     "Le tarif conseille l'élargisseur AK10128 sur la traverse haute du dormant."),
    ("Mise en œuvre du soufflet d'aération sur ébrasement", 49,
     "Sur la {p}, le tarif distingue deux mises en œuvre selon l'ébrasement. En dessous "
     "de 100 mm, la tringle de descente peut être déviée au montage par un renvoi "
     "coudé. Au-delà de 100 mm et jusqu'à 680 mm, un renvoi d'angle flexible est "
     "nécessaire, de 700 mm ou de 1000 mm selon l'ébrasement à contourner. Pour un "
     "flexible de longueur supérieure, le tarif renvoie à une consultation. Un guide "
     "supplémentaire est à prévoir tous les mètres."),
    ("Restrictions des ensembles de serrures SB et CC de porte-fenêtre", 51,
     "Sur la {p}, le tarif indique qu'en SB-CC l'ouvrant visible AM10105 est "
     "obligatoire. L'ensemble SB interdit l'oscillo-battant et la poignée de tirage, sa "
     "hauteur de poignée est de 1070 mm et la hauteur minimale de battant de 1641 mm ; "
     "le verrouillage des galets se fait par relevage de la béquille en position 2. "
     "L'ensemble CC autorise l'oscillo-battant mais interdit la poignée de tirage, avec "
     "la même hauteur de poignée et une hauteur minimale de battant de 1741 mm. Le "
     "tarif avertit qu'une solution SB-CC n'apporte pas les garanties nécessaires aux "
     "fermetures de portes d'entrée ou de portes secondaires."),
    ("Hauteur de poignée en fonction des dimensions du vantail", 47,
     "Sur la {p}, le tarif donne la hauteur de poignée standard en fonction de la "
     "hauteur du vantail, séparément pour l'exécution en ouverture à la française et "
     "pour l'exécution oscillo-battante. La poignée est au milieu jusqu'à 359 mm, puis "
     "à 175, 220, 270, 420, 520, 620 et 720 mm selon les tranches de hauteur de "
     "vantail. Sur une porte-fenêtre, elle est à 820 ou 1000 mm. La même page donne les "
     "hauteurs de battant maximales admissibles hors standard, ainsi que les dimensions "
     "minimales de fabrication de chaque configuration de ferrage."),
    ("Conditions de réalisation des châssis fixes spéciaux", 64,
     "Sur la {p}, le tarif conditionne la réalisation des châssis fixes spéciaux à la "
     "largeur de sertissage du plus petit angle, limitée à 350 mm au maximum. Sur le "
     "profilé dormant AK10130, l'angle minimal d'un châssis fixe triangle-rectangle est "
     "de 30 degrés, le cœfficient multiplicateur de 0,58 et la hauteur minimale de "
     "140 mm. Les trapèzes sont soumis à un angle minimal fonction de la longueur, les "
     "polygones à une longueur maximale de soudure sur leurs trois angles supérieurs. "
     "Ces conditions doivent être vérifiées avant tout chiffrage."),
    ("Impossibilités de laquage du volet roulant selon la teinte", 19,
     "Sur la {p}, le tarif indique qu'il est impossible de proposer un volet roulant "
     "laqué pour un châssis en laquage champagne ou en laquage bronze. La teinte "
     "Rouille, le laquage nature et plusieurs bicolorations sont également marqués "
     "impossibles sur certains composants du coffre. Le tarif précise en outre que le "
     "laquage deux faces n'est réalisable qu'avec des manœuvres par moteur filaire ou "
     "radio, sans manœuvre de secours."),
    ("Absence de bicoloration possible dans les coffres de volet roulant", 19,
     "Sur la {p}, le tarif indique qu'aucune bicoloration n'est possible dans les "
     "coffres de volet roulant : ni pour un châssis bicolore associant le RAL 9016 "
     "granité intérieur à une autre teinte extérieure lorsqu'une nuance est nécessaire "
     "pour le volet, ni pour un châssis dont les deux faces sont en RAL granité. La "
     "saisie du coffre est alors impossible et doit être traitée hors bicoloration."),
]

F6_BLOCS = [
    ("Existence et localisation de l'offre couleurs", 15,
     "Les pages 15 et 16 du tarif portent l'offre couleurs de la gamme. Les teintes "
     "sont réparties en groupes dont la logique tarifaire est un pourcentage appliqué "
     "au prix du châssis : un groupe est sans plus-value, les autres portent chacun un "
     "pourcentage propre. L'offre comprend des teintes monocolores en finition granitée "
     "mate, des laquages lisses, l'anodisation nature, des laquages d'imitation "
     "anodisation, une gamme Futura sablée, une teinte Rouille, ainsi que des "
     "combinaisons bicolores dont chaque association d'une teinte intérieure et d'une "
     "teinte extérieure est explicitement autorisée par un tableau. Les tons bois "
     "sublimés exclusifs sont des teintes approchantes des décors PVC. Les pourcentages "
     "doivent être lus directement sur les pages 15 et 16."),
    ("Existence et localisation des teintes des accessoires et des joints", 17,
     "Les pages 17 et 18 du tarif donnent, pour chaque groupe de couleur et chaque "
     "combinaison de teintes intérieure et extérieure du dormant et de l'ouvrant, la "
     "teinte de la poignée et des paumelles ainsi que celle de la grille de "
     "ventilation. La teinte des accessoires n'est donc pas choisie librement : elle "
     "est déterminée par la teinte du châssis. Le tarif précise que les joints sont "
     "noirs quelles que soient les teintes intérieure et extérieure du châssis. Les "
     "correspondances doivent être lues directement sur les pages 17 et 18."),
    ("Existence et localisation du laquage bloc-baie", 19,
     "Les pages 19 et 20 du tarif portent le laquage bloc-baie, c'est-à-dire "
     "l'harmonisation de teinte entre le châssis aluminium et les composants du coffre "
     "de volet roulant Chrono One 200 et 230 : référence de coffre, cornière, lame "
     "finale et coulisses en face extérieure, coffre PVC en face intérieure. Le tarif y "
     "indique, teinte par teinte, la finition à retenir sur chaque composant ou "
     "l'impossibilité de la réaliser. Cette table de correspondance ne porte pas de "
     "prix, à l'exception d'un forfait de laquage du volet roulant, qui est tarifé et "
     "se lit page 20."),
    ("Existence et localisation de la plus-value pour dormants de rénovation", 10,
     "Les quatre pages de grilles de prix du tarif TA76 OV, de la page 10 à la page 13, "
     "portent chacune "
     "une plus-value pour dormants de rénovation, applicable aux dormants AK10120J "
     "(LZ109) et AK10121J (LZ139). Cette plus-value est exprimée en pourcentage à "
     "appliquer sur les grilles de prix, et non en montant. Elle concerne donc tout "
     "chiffrage de châssis en rénovation sur ces deux dormants. Le pourcentage doit "
     "être lu directement sur la page de la grille concernée, et son application au "
     "prix de grille revient à l'ADV."),
    ("Existence et localisation de la plus-value de vitrage des châssis spéciaux", 65,
     "Sur la page 65, la tarification des châssis fixes spéciaux comporte deux "
     "composantes : une plus-value chiffrée à ajouter au prix du châssis rectangulaire "
     "englobant la forme à calculer, et une plus-value de vitrage exprimée en "
     "pourcentage. La seconde n'est pas transcrite ici et doit être lue directement sur "
     "la page 65. Le tarif précise que le prix de départ est celui du châssis "
     "rectangulaire lu dans la grille des châssis fixes, et que les prix des "
     "compléments et accessoires s'ajoutent au prix du châssis nu."),
    ("Existence et localisation de la majoration de gravure sur vitrage sablé", 30,
     "Sur la page 30, le tarif prévoit une variante de gravure Art Déco consistant en "
     "une gravure transparente sur vitrage sablé. Cette variante porte une majoration "
     "exprimée en pourcentage, appliquée à la plus-value du vitrage pour verre sablé, "
     "et non un montant. Le pourcentage et la plus-value de vitrage à laquelle il "
     "s'applique doivent être lus directement sur les pages 30 et 25, et leur "
     "combinaison revient à l'ADV."),
    ("Existence et localisation des surfaces minimales de facturation", 24,
     "Les pages 24 à 27 du tarif portent une surface minimale de facturation applicable "
     "aux plus-values de vitrage et de remplissage, qui sont exprimées au mètre carré. "
     "Cette surface plancher signifie qu'une plus-value calculée sur une surface "
     "inférieure est facturée sur la surface minimale. Elle doit être lue directement "
     "sur la page de la plus-value concernée, et son application revient à l'ADV."),
    ("Existence et localisation de l'historique des évolutions du tarif", 69,
     "Sur la page 69, le tarif porte un tableau récapitulant ses évolutions "
     "successives, avec pour chacune la modification apportée, la ou les pages "
     "concernées et la date d'application. Ce tableau recense aussi bien les "
     "corrections de contenu — évolution de la carte couleurs, mise à jour des schémas "
     "de ferrage, ajout de références de poignées, ouverture des châssis fixes spéciaux "
     "— que les hausses successives appliquées à l'ensemble des prix. Pour savoir si "
     "une information a changé et à quelle date, cette page est la source à consulter."),
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
        return (" Le tarif n'énonce pas d'unité de facturation pour ce montant : elle "
                f"doit être lue page {page} du tarif.")
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
        f"document_source_ttc: {PDF_TTC_YAML}\n"
        "type_document: tarif\n"
        f"sous_type: {sous_type}\n"
        "gamme_code: TA76_OV\n"
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
    """31 lignes portent l'étiquette de la gamme jumelle. Leurs valeurs ont été
    retrouvées telles quelles dans le PDF TA76 OV : ce sont des données TA76 OV
    et une étiquette fautive. Elles sont CONSERVÉES et l'écart est consigné."""
    fautives = defaultdict(list)
    for r in rows:
        g = norm(r["v"][C_GAMME])
        if not g:
            continue
        if g.replace(" ", "") != GAMME.replace(" ", ""):
            fautives[g].append(r["xl"])
    for g, xls in fautives.items():
        JOURNAL.append(f"ÉTIQUETTE DE GAMME FAUTIVE : « {g} » sur {len(xls)} lignes "
                       f"(Excel {xls[0]} à {xls[-1]}) — valeurs retrouvées au PDF "
                       f"TA76 OV, lignes conservées, écart à corriger à la source")


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
            if v[j] is not None and v[k] is None and float(v[j]) == 0:
                continue          # bourrage, journalisé en bloc plus bas
            if v[j] is None or v[k] is None:
                JOURNAL.append(f"HT/TTC DÉSALIGNÉ : Excel {r['xl']} ({chap}/{tab}), "
                               f"largeur {largeurs[j]}")
                continue
            cells.append((largeurs[j], v[j], v[k]))
        if cells:
            out.append((v[C_HAUTEUR], cells, r["xl"]))
    return out


def journal_bourrage(rows, largeurs):
    """Amendement OV 2 de la règle T1 : zéro au-delà de la dernière largeur tarifée."""
    for r in rows:
        v = r["v"]
        chap = norm(v[C_CHAP])
        if chap not in CHAP_GRILLE:
            continue
        larg = [largeurs[j] for j, k in zip(COLS_HT_L, COLS_TTC_L)
                if v[j] is not None and v[k] is None and float(v[j]) == 0]
        if larg:
            JOURNAL.append(f"BOURRAGE NON GÉNÉRÉ (anti-fantôme) : Excel {r['xl']} "
                           f"({chap} / {norm(v[C_TAB])}) — {len(larg)} zéros de "
                           f"{larg[0]} à {larg[-1]} mm, au-delà de la dernière largeur "
                           f"tarifée, colonne TTC vide")


def echelle_largeurs(lignes):
    ech = set()
    for _, cells, _ in lignes:
        ech |= {c[0] for c in cells}
    return sorted(ech)


# ============================================================ F2 : prix de grille
def gen_grille_2d(chap, tab, libelle, synonyme, rows, largeurs, sc, art="du "):
    """Règles T1 et T2. Découpage piloté par le plafond, jamais par une constante."""
    lignes = lignes_grille(rows, chap, tab, largeurs)
    ech_L = echelle_largeurs(lignes)
    ech_H = sorted({h for h, _, _ in lignes})
    # amendement OV 1 : la grille grande hauteur prolonge l'échelle de sa grille mère
    parent = ECHELLE_H_HERITEE.get((chap, tab))
    if parent:
        mere = lignes_grille(rows, parent[0], parent[1], largeurs)
        ech_H = sorted(set(ech_H) | {h for h, _, _ in mere})
        JOURNAL.append(f"ÉCHELLE DE HAUTEUR PROLONGÉE : {chap} / {tab} hérite de "
                       f"l'échelle de {parent[0]} / {parent[1] or '—'}, la bande de la "
                       f"première hauteur étant ainsi bornée par le tarif et non inventée")
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
                title = (f"{PREFIXE}Tarif {art}{libelle}, hauteur "
                         f"{dire_bande(h_bas, h)}, {titre_l}")
                src = source_line(page, sc_id(sc))
                body = (f"Sur la grille de prix {art}{libelle} de la {PRODUIT}, "
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
    for (chap, tab), spec in GRILLES_2D.items():
        lib, syn = spec[0], spec[1]
        art = spec[2] if len(spec) > 2 else "du "
        c, sc = gen_grille_2d(chap, tab, lib, syn, rows, largeurs, sc, art)
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
        det, contracte, prod = prod.split("|", 2)   # article défini, article contracté
        teinte = TEINTES.get(norm(des), norm(des))
        return (f"Tarif {contracte}{prod.split(' (')[0]} {teinte}",
                (det, prod), teinte)
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
        det, nom = prod
        espace = "" if det.endswith("'") else " "
        body = (f"Sur la {PRODUIT}, le tarif chiffre {det}{espace}{nom} {teinte} à "
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
    if "Groupe 2" in des:
        body += (" La teinte du groupe 2 porte par ailleurs une plus-value exprimée en "
                 "pourcentage, à lire page 15 du tarif.")
    if "Sublimation" in des:
        body += (" La teinte du groupe sublimation porte par ailleurs une plus-value "
                 "exprimée en pourcentage, à lire page 16 du tarif.")
    if ht == 0 and chap.startswith("PV vitrages"):
        body += (" Le tarif le range parmi les vitrages inclus dans la triple offre, "
                 "disponibles sans supplément.")
    note = NOTES.get((chap, tab, des), NOTES.get((chap, tab)))
    if note:
        body += " " + note
    for xl in xl_list:
        if xl in DIVERGENCES:
            body += " " + DIVERGENCES[xl]
            JOURNAL.append(f"DIVERGENCE EXPOSÉE : Excel {xl} — {chap} / {d}")
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
        if r["xl"] in EXCLUSIONS:
            JOURNAL.append(f"EXCLUSION ARBITRÉE : Excel {r['xl']} — {EXCLUSIONS[r['xl']]}")
            continue
        if v[C_HT] is None:
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
        # une apposition ouverte par une virgule doit être refermée avant le verbe
        fin_v = finition + ("," if "," in finition else "")
        body = (f"Sur la {PRODUIT}, {det} {fin_v} est chiffré en plus-value à "
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
        if r["xl"] in EXCLUSIONS:
            JOURNAL.append(f"EXCLUSION ARBITRÉE : Excel {r['xl']} — {EXCLUSIONS[r['xl']]}")
            continue
        if v[C_HT] is None:
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
        verbe = "sont chiffrés" if pl else "est chiffré"
        lib_v = libelle + ("," if "," in libelle else "")
        body = (f"Sur la {PRODUIT}, {article} {lib_v} {verbe} en plus-value à "
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
    # 3. postes à zéro relevés au PDF et absents de l'Excel (amendement OV de T4)
    for titre, page, corps in SANS_PV:
        body = (marqueur_gamme(corps.format(p=f"page {page}"))
                + " Les prix s'entendent hors éco-participation.")
        chunks.append(emit(PREFIXE + titre, source_line(page, sc_id(sc)), body))
        sc += 1
    return chunks


# ============================================================ F4 : châssis spéciaux
# Règle OC1 SANS OBJET : le tarif TA76 OV ne renvoie à aucune autre gamme.
# Aucune mention de saisie sous une gamme tierce ne doit figurer ici.
def gen_f4(rows):
    """Règle T5 dans sa forme redéfinie sur TA76 OC."""
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
        body += (" Ce poste appartient au chapitre des châssis fixes spéciaux du tarif "
                 "TA76 OV, dont la tarification se lit pages 65 et 66 et s'ajoute au "
                 "prix du châssis rectangulaire englobant la forme à calculer.")
        body += " Les prix s'entendent hors éco-participation."
        chunks.append(emit(PREFIXE + titre, source_line(page, sc_id(sc)), body))
        sc += 1
    return chunks


# ============================================================ F5 / F6 : statiques
def marqueur_gamme(body):
    """Auto-discrimination : le code gamme doit figurer dans le CORPS et pas
    seulement dans le titre, faute de quoi un chunk servi hors contexte serait
    indiscernable de son homologue de la gamme jumelle (règle OV1)."""
    if GAMME in body:
        return body
    for avant, apres in (("du tarif", f"du tarif {GAMME}"),
                         ("Le tarif", f"Le tarif {GAMME}"),
                         ("le tarif", f"le tarif {GAMME}")):
        if avant in body:
            return body.replace(avant, apres, 1)
    JOURNAL.append("MARQUEUR DE GAMME AJOUTÉ EN CLÔTURE : aucun point d'insertion "
                   "naturel dans le corps")
    return body + f" Cette règle est celle de la {PRODUIT}."


def gen_statique(blocs, nature="originale"):
    chunks, sc = [], 2
    for titre, page, corps in blocs:
        body = marqueur_gamme(corps.format(p=f"page {page}"))
        chunks.append(emit(PREFIXE + titre, source_line(page, sc_id(sc), nature), body))
        sc += 1
    return chunks


# ============================================================ journal colonnes
def journal_colonnes(header, rows):
    mappees = set([C_CHAP, C_TAB, C_GAMME, C_CLE, C_DES, C_DET, C_MENTION_HT,
                   C_MENTION_TTC, C_HAUTEUR, C_HT, C_TTC] + COLS_HT_L + COLS_TTC_L)
    for j in range(NCOLS):
        rempli = sum(1 for r in rows if r["v"][j] not in (None, ""))
        if rempli == 0:
            JOURNAL.append(f"COLONNE ENTIÈREMENT VIDE : « {norm(header[j])} » (index {j})")
        elif j not in mappees and j not in (C_DET_MT_HT, C_DET_MT_TTC):
            JOURNAL.append(f"COLONNE REMPLIE NON MAPPÉE : « {norm(header[j])} » "
                           f"(index {j}, {rempli} valeurs)")
    for j in (C_DET_MT_HT, C_DET_MT_TTC):
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
    journal_bourrage(rows, largeurs)

    sorties = [
        ("Tarif_TA76_OV_METHODE.md", "methode", gen_statique(F1_BLOCS)),
        ("Tarif_TA76_OV_PRIX_CHASSIS.md", "prix_chassis", gen_f2(rows, largeurs)),
        ("Tarif_TA76_OV_OPTIONS.md", "options", gen_f3(rows)),
        ("Tarif_TA76_OV_CHASSIS_SPECIAUX.md", "chassis_speciaux", gen_f4(rows)),
        ("Tarif_TA76_OV_FAISABILITES.md", "faisabilites", gen_statique(F5_BLOCS)),
        ("Tarif_TA76_OV_TRANSVERSES.md", "transverses", gen_statique(F6_BLOCS)),
    ]
    total = 0
    print("=" * 74)
    print("GÉNÉRATION DES CHUNKS — TARIF TA76 OV")
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
