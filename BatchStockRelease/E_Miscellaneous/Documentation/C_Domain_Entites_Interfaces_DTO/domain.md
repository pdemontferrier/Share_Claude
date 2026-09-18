# Domaine Applicatif

## Entités principales
- `DecoupeLot` : Définit un lot de découpe et son état d’approvisionnement.
- `DecoupeBarre` : Contient les barres associées à un lot.
- `ArticleInterne` : Représente une référence matière première.
- `User`, `UserSession`, `UserAppPageDroit` : Gestion des utilisateurs et de leurs droits.

## Interfaces et Repositories
- `IQ_DecoupeBarre`, `IR_DecoupeBarre`
- `IQ_DecoupeLot`, `IR_DecoupeLot`
- `IC_CommandeClientAction`, `IR_CommandeClientAction`