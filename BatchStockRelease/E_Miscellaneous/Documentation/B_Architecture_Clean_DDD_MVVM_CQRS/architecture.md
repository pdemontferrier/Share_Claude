# Architecture

## Clean Architecture
La solution est structurée en quatre couches :

- **A_Domain** : Entités métier, Interfaces et DTO.
- **B_UseCases** : Logique applicative, Services et orchestrations métier.
- **C_Infrastructure** : Accès aux données (EF Core, Repositories, Providers).
- **D_Presentation** : Interface WPF (Vues, ViewModels, Services d’UI).

## Patterns clés
- **MVVM** pour la présentation.
- **DDD** pour le modèle métier.
- **CQRS** pour la séparation Command/Query.
- **SOLID** et **DI** pour garantir le découplage et la testabilité.