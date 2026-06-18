# Codes d'erreur spécifiques

Inventaire centralisé des codes d'erreur spécifiques de la solution, au format `xxxx_ER_NN`.

La convention de format, les règles de référencement et la typologie des origines sont définies dans la section **4.6.3** du référentiel **023 — Spécifications techniques de développement logiciel**. Le présent fichier ne reproduit pas ces règles : il en est l'application opératoire et l'inventaire transverse.

Toute introduction d'un nouveau code spécifique dans le code applicatif doit s'accompagner de l'ajout de la ligne correspondante dans la table ci-dessous.

| Code | Origine | Composant générateur | Description |
| --- | --- | --- | --- |
| DICT_ER_01 | Service technique | SR_Dictionary | Clé de traduction absente du dictionnaire actif ou dictionnaire non initialisé. |
| DICT_ER_02 | Service technique | SR_Dictionary | Erreur inattendue survenue lors de la résolution d'une clé de traduction. |
