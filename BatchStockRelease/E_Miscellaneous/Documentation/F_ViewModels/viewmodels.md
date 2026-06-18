# ViewModels

Chaque page WPF possède un ViewModel dédié, appliquant le pattern MVVM.

## Exemple :
`VM_Page10` : gestion de la page d’accueil de l’opérateur magasinier.

### Rôle :
- Charger la liste des lots à approvisionner.
- Gérer la sélection du lot.
- Interagir avec les UseCases d’approvisionnement (`UC_BarDropStockRelease`, `UC_BarNewStockRelease`).

### Convention :
- Les ViewModels héritent de `VM_Page_Generic`.
- Les commandes sont implémentées via `UT_RelayCommandArg1`.
