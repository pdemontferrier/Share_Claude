# Codes d'erreur spécifiques : SpecificErrorCodes.md

Inventaire centralisé des codes d'erreur spécifiques de la solution, au format `xxxx_ER_NN`.

La convention de format, les règles de référencement et la typologie des origines sont définies dans la section **4.6.3** du référentiel **023 — Spécifications techniques de développement logiciel**. Le présent fichier ne reproduit pas ces règles : il en est l'application opératoire et l'inventaire transverse.

Toute introduction d'un nouveau code spécifique dans le code applicatif doit s'accompagner de l'ajout de la ligne correspondante dans la table ci-dessous.

| Code | Origine | Composant générateur | Description |
| --- | --- | --- | --- |
| DICT_ER_01 | Service technique | SR_Dictionary | Clé de traduction absente du dictionnaire actif ou dictionnaire non initialisé. |
| DICT_ER_02 | Service technique | SR_Dictionary | Erreur inattendue survenue lors de la résolution d'une clé de traduction. |
| DBCN_ER_01 | Service technique | UC_DigitTryDb_TestConnection | Perte de connexion à la base de données partagée observée par retour binaire négatif du Service Infrastructure IS_DigitTryDb_TestConnection sans exception sous-jacente. Levée intentionnellement dans la branche else du try de ExecuteAsync pour converger dans le catch (Ex_Infrastructure) existant. |
| CLOS_ER_01 | Module métier | UC_CloseCommand_Check | Trace informationnelle de fermeture forcée de l'application : déclenchement d'une fermeture temporisée sur détection d'une commande d'accessibilité applicative ou de session. Émise en fire-and-forget via IU_LogAndNotify (clé No_EC_01, notify false), portée par l'errorId d'une Ex_Business conteneur jamais levée. |
