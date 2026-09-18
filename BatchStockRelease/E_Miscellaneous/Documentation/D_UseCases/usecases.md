# UseCases

Les **UseCases** orchestrent les scénarios métiers complets.

## Exemple : UC_BarNewStockAllocation
Responsable de l’affectation des barres neuves à un lot.

### Étapes du processus :
1. Réinitialisation des ruptures (`IS_DecoupeBarre_ClearOutOfStockStatus`)
2. Identification des articles internes à traiter
3. Traitement des allocations (`IS_DecoupeBarre_ProcessAllocation`)
4. Mise à jour du statut du lot (`IS_DecoupeBarre_UpdateOutOfStockStatus`)

### Convention de nommage :
- Tous les UseCases définissent `UseCaseName = GetType().Name`
- Appels de services :  
  `await _service.ExecuteAsync(param1, param2, UseCaseName);`