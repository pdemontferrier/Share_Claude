# Services

Les **services** implémentent les règles métier unitaires.

## Catégories :
- **BusinessLogic** : découpe, chutes, barres neuves, labels, etc.
- **App** : gestion des paramètres, dictionnaires, navigation.
- **UserLogic** : authentification, sessions, notifications.

### Exemple : SR_ConnectionMonitor
Gère la surveillance de la connexion à la base de données, via l’interface `I_DB_MonitoringController`.

```csharp
/// <summary>
/// Surveille la disponibilité de la base de données.
/// </summary>
/// <remarks>
/// Ce service assure la reconnexion automatique en cas de coupure.
/// </remarks>
