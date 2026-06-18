
# 🧾 CHANGELOG — BatchStockRelease

Ce fichier décrit l’historique des modifications majeures, mineures et correctives apportées à l’application.  
Le format suit les recommandations de [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/) et les principes de [Semantic Versioning](https://semver.org/lang/fr/).

---

# [1.4.0.0] – 2025-10-30  
### Refactoring complet du monitoring de connexion base de données

#### 🔁 Changements majeurs
- **Refonte du système de monitoring** :  
  Remplacement de l’ancien service `SR_ConnectionMonitor` par une nouvelle architecture modulaire et découplée basée sur le pattern Clean Architecture.

#### 🧱 Détails des évolutions
- **Renommage et refactorisation**
  - `SR_ConnectionMonitor` renommé en `SR_DB_Monitoring`, désormais responsable uniquement de la **surveillance technique** de la connexion à la base de données.
  - `UC_StartMonitoringDB` renomé en `UC_DB_Monitoring` est enrichi pour gérer les **cycles de reconnexion** (rapide et lente), la **fermeture contrôlée de l’application**, et la **gestion visuelle des modales d’attente** via `IS_Notification`.

- **Suppression**
  - Suppression du UseCase `UC_StopMonitoringDB` devenu obsolète.  
    → Les opérations d’arrêt sont désormais intégrées au service `SR_DB_Monitoring` via la méthode `StopMonitoringAsync()`.

- **Nouvelles interfaces**
  - Création de l’interface **`I_DB_MonitoringController`** permettant un contrôle unifié du cycle de vie du monitoring (start / stop / état).  
    → Facilite la testabilité et la réutilisation du service dans d’autres contextes applicatifs.

- **Évolution des Settings**
  - Ajout dans `SE_App` :
    - Propriétés :  
      `IsConnected`, `IsDialogOpen`, `DatabaseCheckInterval`, `DatabaseCheckFirstLoop`, `DatabaseCheckSecondLoop`
    - Événements :  
      `ConnectionLost`, `ConnectionRestored`
    - Méthodes :  
      `NotifyConnectionLost()`, `NotifyConnectionRestored()`
  - Ces éléments sont désormais exposés via `IS_Settings_App`, assurant une **abstraction propre** et un **respect total du DIP** (Dependency Inversion Principle).

- **MainWindow**
  - Intégration de l’injection directe des dépendances via le conteneur DI.  
  - Abonnements aux événements de connexion effectués via `IS_Settings_App`.
  - Lancement du monitoring orchestré par `UC_StartMonitoringDB`.

- **Conteneur DI**
  - Ajout des enregistrements :
    ```csharp
    services.AddSingleton<IS_DB_Monitoring, SR_DB_Monitoring>();
    services.AddSingleton<I_DB_MonitoringController, SR_DB_Monitoring>();
    services.AddTransient<IU_DB_Monitoring, UC_DB_Monitoring>();
    ```

#### 🧩 Résultat
- Architecture **plus claire**, **plus testable** et **entièrement découplée** entre Infrastructure, UseCases et UI.  
- Gestion des connexions désormais **événementielle et réactive**, conforme aux principes SOLID et DDD.  
- Amélioration de la **stabilité**, de la **traçabilité** et de la **maintenabilité** du module de monitoring.

---

## [1.4.0.0] - 2025-10-29
### Added
- **Service `SR_ConnectionMonitor`**
  - Nouveau service de surveillance asynchrone de la connexion base de données (`geststock`).
  - Implémente les événements `ConnectionLost` et `ConnectionRestored` permettant de suspendre et relancer les listeners temps réel.
  - Gestion centralisée du `CancellationTokenSource` pour éviter les fuites mémoire et les exceptions à la fermeture.
  - Surveillance continue via `MonitorLoopAsync()` avec délai configurable depuis `IS_Settings_App.GetDatabaseCheckInterval()`.
  - Ajout de `BeginInvoke` pour éviter tout blocage de thread lors de l’ouverture/fermeture de la boîte de dialogue de déconnexion.
  - Journalisation simplifiée : interception explicite de `TaskCanceledException` lors du shutdown pour éviter les faux positifs dans les logs.

- **UseCases `UC_StartMonitoringDB` & `UC_StopMonitoringDB`**
  - Introduction de deux orchestrateurs dédiés à la gestion du cycle de vie du monitoring base de données.
  - Centralisation du démarrage et de l’arrêt du service `SR_ConnectionMonitor` dans la couche UseCase.
  - Gestion standardisée des exceptions (`Ex_Business`, `Ex_Infrastructure`, `Ex_General`) et traçabilité via `useCaseName` et `callChain`.
  - Simplification du code dans `MainWindow` et amélioration de la maintenabilité.

### Changed
- **`MainWindow.xaml.cs`**
  - Refactorisation complète du cycle de vie de la fenêtre :
    - `OnLoaded` : démarrage orchestré des listeners et du monitoring via `UC_StartMonitoringDB`.
    - `OnClosed` : arrêt propre via `UC_StopMonitoringDB`, libération du CTS et désabonnement des événements.
  - Gestion explicite des événements `ConnectionLost` et `ConnectionRestored` pour relancer automatiquement les tâches asynchrones.
  - Application des conventions d’appel (`caller`, `callChain`, `UseCaseName`) à toutes les méthodes.
  - Nettoyage des `Dispatcher.Invoke()` bloquants remplacés par des `BeginInvoke()` non bloquants.
  - Fermeture contrôlée dans `OnClosing()` avec vérification de la connexion DB via `_settingsUser.GetForceClose()`.

- **`SR_Notification`**
  - Transformation complète de la logique modale :
    - Remplacement de `Dispatcher.Invoke()` par `Dispatcher.BeginInvoke()` pour éviter tout blocage du thread UI.
    - Remplacement de `ShowDialog()` par `Show()` pour une fenêtre non bloquante.
    - Maintien d’un comportement visuel modale via la désactivation temporaire de `MainWindow`.
  - Ajout de la suppression du bouton “Fermer (X)” par code natif Win32 (`DeleteMenu` / `GetSystemMenu`).

- **`SR_Shutdown`**
  - Refactorisation complète pour un comportement non bloquant :
    - Remplacement de `Dispatcher.Invoke()` par `BeginInvoke()` dans toutes les méthodes (`ForceShutdown`, `Shutdown`, `ShutdownWithDelayAsync`).
    - Suppression des blocages de thread lors des fermetures différées.
    - Intégration du flag `_settingsUser.SetForceClose(true)` pour indiquer une fermeture forcée en cas de coupure DB prolongée.
  - Fermeture différée (`ShutdownWithDelayAsync`) désormais asynchrone, affichant un message via `IS_Notification` avant extinction de l’application.

- **`UC_User_CloseApplication`**
  - Ajout de la gestion du cas “fermeture forcée” :
    - Si la base de données est inaccessible, le UseCase ne tente plus de fermer la session utilisateur.
    - Ajout du code erreur `UCA_01` : *Fermeture forcée de l’application sans accès à la base*.
  - Enregistrement du contexte dans les logs via `_logAndNotify.ExecuteAsync("No_EC_19", new Ex_Infrastructure(...))`.

- **`DialogWindow`**
  - Suppression du bouton de fermeture “X” par API Win32 (`GetSystemMenu` / `DeleteMenu`).
  - Empêchement du redimensionnement (`ResizeMode="NoResize"`).
  - Centrage automatique corrigé via `CenterWindowOverOwnerWithoutCoveringTaskbar()` pour éviter tout recouvrement de la barre des tâches.
  - Configuration finale :
    - `WindowStyle="ToolWindow"`
    - `Topmost="True"`
    - `ShowInTaskbar="False"`

### Fixed
- Correction du gel du thread UI lors de l’ouverture d’une fenêtre de notification en cas de perte de connexion.
- Suppression des logs erronés `UnknownCallChain` et `Task was canceled` lors de la fermeture du monitoring.
- Correction du centrage de la boîte de dialogue lorsque la `MainWindow` est maximisée.
- Stabilisation du cycle de fermeture de l’application en cas de coupure prolongée du réseau.
- Résolution de l’erreur `Missing translation key: UnknownErrorId` dans `SR_Dictionary > GetText` suite aux exceptions de tâches annulées.

### Architecture
- Consolidation de la gestion du `CancellationTokenSource` dans tous les services asynchrones.
- Respect complet des conventions d’appel (`_callee`, `caller`, `callChain`) et des commentaires XML standardisés.
- Adoption de la séparation stricte :  
  - **Service (SR_)** = logique d’exécution unitaire.  
  - **UseCase (UC_)** = orchestration des processus.  
  - **MainWindow / UI** = interaction utilisateur et présentation.

---

## [1.4.0.0] - 2025-10-26
### Changed
- **Page00 — Authentification utilisateur :**
  - Refactorisation complète du ViewModel `VM_Page00` pour implémenter une logique MVVM pure.
  - Remplacement de la gestion d’événement `On_Valider_Click` par une commande asynchrone `LoginCommand`.
  - Ajout des propriétés `Login` et `Password` liées directement à la vue (`Page00.xaml`).
  - Mise à jour du fichier XAML avec binding bidirectionnel et intégration du helper `UT_PasswordBoxHelper`.
  - Initialisation par défaut des propriétés non-nullables pour conformité au compilateur C# 8+ (CS8618).
  - Intégration des nouveaux UseCases d’authentification (`IU_User_Authentification` et `IS_User_IsLoginPasswordValid`).

### Added
- **Nouveau service :** `SR_User_IsLoginPasswordValid` — Validation des identifiants utilisateur via `IQ_User`.
- **Nouvel utilitaire :** `UT_PasswordBoxHelper` — Permet le binding sécurisé du mot de passe (`PasswordBox`) dans WPF.
- **Documentation XML complète :** ajoutée à tous les nouveaux fichiers et méthodes publiques concernées.

### Removed
- **Services obsolètes :**
  - `SR_UserAccess` (remplacé par le UseCase `IU_User_Authentification`).
  - `SR_UserAuthentification` (fusionné dans la nouvelle logique d’authentification centralisée).

### Impact
- La page de connexion est désormais totalement découplée de la couche présentation.
- L’architecture MVVM est respectée de bout en bout, facilitant la maintenabilité et les tests unitaires.
- Le processus d’authentification est uniformisé sur la base des nouveaux UseCases et conventions DDD.

---

## [1.4.0.0] - 2025-10-24
### Added
- **BA_MainWindow** : création d’un nouveau composant `UserControl` dédié à la bannière principale de l’application.
  - Déplacement du bloc de boutons supérieur (langue, utilisateur, messages, fermeture, etc.) hors de `MainWindow.xaml`.
  - Ajout d’un ViewModel dédié `VM_BA_MainWindow` pour gérer la logique d’interaction (navigation, icônes dynamiques, visibilité, état des messages non lus).
  - Application des styles via le service `IS_ControlStyler` pour harmoniser l’apparence du bandeau.
  - Gestion complète des événements et initialisation asynchrone du ViewModel.
  - Intégration du composant dans `MainWindow` avec instanciation différée après initialisation du conteneur DI, évitant l’erreur `XDG0003`.

### Changed
- **MainWindow.xaml / MainWindow.xaml.cs**
  - Refactorisation complète pour externaliser la bannière dans un composant réutilisable.
  - Ajout d’un `ContentControl` (`BannerHost`) servant de point d’ancrage pour le chargement différé de `BA_MainWindow`.

### Technical
- Enregistrement de `VM_BA_MainWindow` dans `SR_ConteneurDI.cs` pour injection via `App._serviceProvider`.
- Ajout de la documentation XML complète pour `BA_MainWindow` (classe et XAML), conforme aux conventions du projet BatchStockRelease.

---

## [1.4.0.0] - 2025-10-24
### Changed
- **Refactorisation complète de la fenêtre principale (`MainWindow`)** :
  - Introduction du ViewModel `VM_MainWindow` pour centraliser la logique de présentation et appliquer le modèle MVVM complet.
  - Suppression des manipulations directes d’UI dans `MainWindow.xaml.cs` au profit de propriétés bindables.
  - Intégration des commandes `UT_RelayCommandArg0` et `UT_RelayCommandArg0Async` pour la navigation et les actions principales.
  - Mise en place d’un mécanisme d’abonnement unifié aux événements (`PropertyChanged`) via les services `SR_Settings_User`, `SR_Settings_App` et `SR_Flags`.

- **Gestion dynamique des messages** :
  - Création d’un mécanisme global d’état applicatif via `SE_App.HasUnreadMessages`.
  - Refactorisation du service `SR_Messages` pour propager l’état des messages non lus à la couche UI via `SE_App` et non plus par événement direct.
  - Intégration complète dans le `VM_MainWindow` : mise à jour automatique de la propriété `HasUnreadMessages` et liaison au XAML via converters (`BoolToVisibilityConverter`, `InverseBoolToVisibilityConverter`).

- **Amélioration de la traçabilité des exceptions** :
  - Vérification et mise à jour de la classe `Ex_Classifier` pour préserver les exceptions d’origine (`InnerException`), éviter les reclassements inutiles et garantir la propagation cohérente des `Ex_Business` et `Ex_Infrastructure`.
  - Ajout de la gestion du `callChain` complet sur tous les niveaux (UseCase → Service → Sous-service).

### Added
- **Classe `SE_App`** : centralise l’état global de l’application (messages non lus, autres états futurs).
- **Service `SR_Settings_App`** : relai de `SE_App.PropertyChanged` vers les ViewModels.
- **Converters XAML** : ajout de `InverseBoolToVisibilityConverter` pour gérer la visibilité conditionnelle des icônes de message.

### Removed
- Événement `UnreadMessagesStatusChanged` dans `SR_Messages` (remplacé par la mise à jour automatique via `SE_App`).
- Méthodes redondantes `DisplayButtonBanner()` et `HideButtonBanner()` dans `MainWindow`.

### Notes
Cette version finalise la migration du projet vers une architecture **MVVM complète et réactive**, garantissant un découplage propre entre la logique métier (UseCases, Services) et la couche de présentation (ViewModels, XAML).

---

## [1.4.0.0] - 2025-10-21

### 🔧 Refactorisation de `MainWindow.xaml.cs` et surtout de la méthode `CheckUserAuthentificationAsync`
#### 🧩 Objectif
Clarifier le flux d’authentification et de séparer la logique métier de la logique d’interface.

**Orchestration séquentielle introduite :**
1. Identification automatique via le compte Windows.  
2. Redirection vers `Page00` si aucun utilisateur n’est reconnu.  
3. Exécution du use case `UC_User_AccessApp` pour vérifier les droits et initialiser le contexte utilisateur.  
4. Navigation vers `Page10` en cas d’accès autorisé, ou retour vers `Page00` avec notification d’avertissement.

**Nouvelles méthodes privées extraites pour une meilleure lisibilité et maintenabilité :**
- `EnsureUserFromDeviceAsync()` — Identification de l’utilisateur par login Windows.  
- `NavigateToPage()`, `NavigateToLogin()`, `NavigateToHome()` — Centralisation de la logique de navigation.  
- `InitializeUiOnce()` — Initialisation visuelle de la fenêtre et du contexte UI.  
- `HandleExceptionAsync()` — Gestion unifiée des erreurs et notifications côté interface.

**Documentation XML ajoutée :**  
Chaque méthode documentée avec les sections *Description*, *Contexte*, *Objectif*, *Tâches / Actions* et *Exceptions*.

**Réorganisation du fichier avec des régions structurées :**
    - `=== Propriétés privées ===`
    - `=== Constructeur ===`
    - `=== Méthodes publiques ===`
    - `=== Méthodes privées ===`

### ➕ Added
#### Use Case
- **`UC_User_AccessApp`**  
  Orchestration complète de la vérification d’accès utilisateur : mise à jour du contexte, initialisation des droits et ouverture de session.

#### Services
- **`SR_User_CheckAppDeviceUser`** : Identification automatique de l’utilisateur à partir de son login Windows et retour de son identifiant (`userId`).  
- **`SR_User_UpdateContext`** : Mise à jour des informations utilisateur dans le contexte applicatif.  
- **`SR_User_InitializePageAccessRights`** : Initialisation des droits d’accès aux pages à partir de la base de données.

### 🚀 Improved
- Uniformisation du **traçage applicatif** via les variables `caller`, `_callee` et `callChain` dans toutes les méthodes.  
- Clarification de la **gestion des erreurs** avec classification homogène :  
  `Ex_Business`, `Ex_Infrastructure`, `Exception`.  
- Séparation claire des responsabilités entre :
  - **Couche UI** : navigation et affichage  
  - **Couche métier** : use cases  
  - **Services spécialisés** : tâches unitaires  
- Amélioration de la **réactivité UI** et suppression de la dépendance directe à `SE_User` dans `OnSettingsPropertyChanged`.

### 🛠️ Fixed
- Suppression du bloc redondant lié à `AppUserID` dans `OnSettingsPropertyChanged`, désormais géré via le processus d’authentification.  
- Correction de la mise à jour de l’affichage du nom utilisateur après identification.  
- Correction d’une potentielle fuite de logique de navigation entre `MainWindow` et `_navigation`, évitant toute dépendance circulaire.

---

## [1.4.0.0] – 2025-10-18
### 🔧 Refactorisation du démarrage applicatif et mise en place du UseCase `UC_Application_OnStart`

#### 🧩 Objectif
Améliorer la maintenabilité, la traçabilité et la cohérence architecturale du processus de démarrage de l’application.

#### 🧠 Détails des modifications
- **Création du UseCase `UC_Application_OnStart`**
  - Orchestration centralisée du processus de démarrage de l’application.
  - Gestion structurée des exceptions `Ex_Business` et `Ex_Infrastructure`.
  - Traçabilité complète via `UseCaseName`, `caller`, et `callChain`.

- **Refactorisation du fichier `App.xaml.cs`**
  - Division de la méthode `Application_Startup` en trois méthodes :
    - `_ProcessStartupArguments()` : traitement des arguments d’exécution (lecture de `iduser`).
    - `_LaunchApplicationAsync()` : exécution du UseCase de démarrage et lancement conditionnel de la fenêtre principale.
    - `OnResolveAssembly()` : maintien de la résolution dynamique des bibliothèques externes.
  - Adoption d’une structure claire avec régions normalisées :
    - `=== Propriétés privées ===`
    - `=== Constructeur ===`
    - `=== Méthodes publiques ===`
    - `=== Méthodes privées ===`
  - Documentation XML complète conforme au standard interne (Description / Contexte / Objectif / Tâches).

- **Ajout et intégration des services de démarrage**
  - `SR_StartupDatabaseConnectivity` : vérifie la connectivité à la base de données.
  - `SR_UserSessionIntegrity` : contrôle l’unicité de la session utilisateur.
  - `SR_ApplicationAvailability` : valide l’accessibilité applicative.
  - `SR_StartupContextUpdater` : met à jour le titre et le contexte utilisateur.
  - Chacun des services dispose de son interface dédiée (`IS_...`) et de commentaires XML standardisés.

#### 🧾 Résultats
- Séparation stricte entre **orchestration (UseCase)** et **tâches unitaires (Services)**.
- Traçabilité complète du flux de démarrage via `callChain`.
- Démarrage plus robuste, testable et maintenable.
- Code plus cohérent avec les principes **DDD**, **CQRS**, et **Clean Architecture**.

---

### ✨ Added
- Mise en place du **UseCase `UC_Application_OnStart`** pour orchestrer le processus complet de démarrage de l’application.  
- Création des **services associés** :
  - `SR_StartupDatabaseConnectivity` : vérification de la connectivité à la base de données.  
  - `SR_UserSessionIntegrity` : contrôle d’intégrité des sessions utilisateur.  
  - `SR_ApplicationAvailability` : validation de la disponibilité applicative.  
  - `SR_StartupContextUpdater` : mise à jour du titre et du contexte utilisateur au démarrage.  
- Ajout des **interfaces dédiées** (`IS_...`) à chaque service, documentées selon le format XML interne.  
- Intégration complète du `callChain` dans les flux d’exécution pour une traçabilité uniforme.  

### 🧩 Changed
- Refactorisation complète du fichier **`App.xaml.cs`** :
  - Découpage de la méthode `Application_Startup` en trois méthodes :
    - `_ProcessStartupArguments()` pour la gestion des paramètres d’exécution.  
    - `_LaunchApplicationAsync()` pour l’exécution du UseCase et le lancement conditionnel de la fenêtre principale.  
    - `OnResolveAssembly()` pour la résolution dynamique des ressources partagées.  
  - Application stricte des conventions internes de structuration des fichiers (`=== Propriétés privées ===`, `=== Dépendances privées ===`, etc.).  
  - Documentation XML normalisée ajoutée sur toutes les méthodes publiques et privées.  
- Uniformisation de la gestion des exceptions :
  - Les services lèvent désormais les exceptions classifiées (`Ex_Business`, `Ex_Infrastructure`) sans notifier directement.  
  - Les UseCases deviennent les orchestrateurs responsables du logging et de la notification.  

### 🧠 Fixed
- Correction de la logique de détection des sessions utilisateur actives (`SR_UserSessionIntegrity`).  
- Correction des erreurs de conversion liées à l’appel de `_settingsUser.GetAppUserId()` dans le UseCase.  
- Amélioration de la stabilité du démarrage : les exceptions d’infrastructure sont désormais gérées proprement via le `UC_Application_OnStart`.  

---

## [1.4.0.0] - 2025-10-16
### Refactoring des services d’initialisation, de navigation et de session

#### 🧭 Navigation
- **Refactorisation complète de `SE_Navigation`** :
  - Génération automatique du dictionnaire `PageMappings` à partir de conventions (`PageNN` / `MH_PageNN`).
  - Suppression des définitions manuelles `PageXX_Source` et `MH_PageXX_Source`.
  - Clarification de la responsabilité : configuration stateless (aucun état runtime).
  - Documentation XML complète pour IntelliSense et génération automatique de documentation.
- **Refactorisation de `SR_Navigation`** :
  - Introduction de l’interface `INavigationState` et de son implémentation `NavigationState` pour encapsuler l’état (page actuelle, menu actif, historique).
  - Injection de dépendance via `AddSingleton<INavigationState, NavigationState>()` dans le conteneur DI.
  - Remplacement des accesseurs redondants `GetPageXX_Source()` par les méthodes génériques `GetPageSource(pageName)` et `GetMenuSource(pageName)`.
  - Réorganisation des responsabilités :
    - `ExecuteAsync()` → orchestration de la navigation.
    - `HasRight()` → méthode générique pour la vérification des droits d’accès.
  - Documentation XML détaillée et gestion centralisée des exceptions.

#### 🪟 Gestion de la fenêtre principale
- **Refactorisation de `SR_Window`** :
  - Extraction des logiques en méthodes privées (`UpdateFromMaximized`, `UpdateFromNormal`, `UpdateMarginAdjusted`).
  - Simplification de la gestion des dimensions et position de la fenêtre principale.
  - Remplacement des calculs empiriques (`+500`) par des corrections contrôlées.
  - Ajout d’une documentation XML claire décrivant le rôle et le comportement.
- **Refactorisation de `SE_Window`** :
  - Documentation XML complète pour toutes les propriétés (largeur, hauteur, marge ajustée, position).
  - Clarification du rôle de cache d’état partagé.
  - Définition explicite des valeurs minimales par défaut (1020×680 px).

#### ⚙️ Initialisation de l’application
- **Refactorisation complète de `SR_OnStart`** :
  - Découpage de `ExecuteAsync()` en trois méthodes :
    - `ExecuteAsync()` → orchestrateur global.
    - `VerifyStartupConditionsAsync()` → vérifications (base de données, session, accessibilité).
    - `UpdateStartupContextAsync()` → mise à jour du contexte utilisateur et du titre applicatif.
  - Ajout de `ServiceName` et `caller` pour la traçabilité.
  - Bloc `try/catch` global avec notifications structurées.
  - Documentation XML complète et cohérente avec les conventions du projet.

#### 👤 Gestion des sessions utilisateur
- **Refactorisation complète de `SR_UserSession`** :
  - Découpage de `OpenUserSessionAsync()` en trois méthodes :
    - `OpenUserSessionAsync()` → orchestrateur.
    - `EnsureUserSessionStateAsync()` → vérification et préparation de session.
    - `ApplyUserSessionUpdatesAsync()` → mise à jour du contexte utilisateur.
  - Uniformisation des messages d’erreur et de la traçabilité (`ServiceName > caller`).
  - Documentation XML complète pour toutes les méthodes publiques et privées.
  - Structure prête à être étendue avec un audit ou un event sourcing.

#### 💡 Divers
- Mise à jour du conteneur DI (`SR_ConteneurDI.cs`) :
  - Enregistrement explicite du service de navigation et de son état :
    ```csharp
    services.AddSingleton<INavigationState, NavigationState>();
    services.AddSingleton<IS_Navigation, SR_Navigation>();
    ```
- Nettoyage des dépendances inutiles dans les services.
- Uniformisation du style de documentation XML sur tous les fichiers refactorés.

---

### ✅ Résumé des bénéfices

| Catégorie | Amélioration |
|------------|--------------|
| **Architecture** | Meilleure séparation des responsabilités (Orchestration / Vérification / Mise à jour). |
| **Testabilité** | Services plus faciles à tester individuellement (grâce à l’injection d’état et au découpage). |
| **Lisibilité** | Code clarifié, homogène et documenté pour IntelliSense. |
| **Maintenance** | Réduction drastique des redondances (`GetPageXX_Source`, calculs en dur, duplication logique). |
| **Stabilité** | Gestion des erreurs et notifications uniformisée dans tout le code applicatif. |

---

## [1.4.0.0] — 2025-10-16
### 🔧 Refactorisation complète des ViewModels

Cette version marque une étape importante dans l’évolution technique de **BatchStockRelease**, avec une refonte complète des ViewModels afin de renforcer la cohérence architecturale et la maintenabilité du projet.

#### 🚀 Améliorations principales
- Intégration du **fichier générique `VM_Page_Generic`** sur l’ensemble des pages de l’application.
- Uniformisation des méthodes de traçabilité via **`BuildFirstCallChain()`**.
- Centralisation de la gestion d’exceptions dans **`ExecuteSafeAsync()`**.
- Refactorisation complète des pages **VM_Page00 → VM_Page99**.
- Mise à jour des conventions de **documentation XML** :
  - Ajout de résumés détaillés avec `<summary>`, `<para>` et `<list>`.
  - Harmonisation des balises IntelliSense pour tous les ViewModels.
- Renforcement de la cohérence **MVVM + Clean Architecture + DDD** :
  - Application stricte du principe **Single Responsibility**.
  - Séparation claire des méthodes métier, UI et exceptionnelles.
- Amélioration de la **testabilité et de la maintenabilité** :
  - Découpage systématique des méthodes complexes.
  - Réduction des duplications et clarification des dépendances injectées.
- Mise à jour de la nomenclature :
  - `BuildCallChain` renommée en **`BuildFirstCallChain`**.
  - `OnPropertyChanged` remplacée par **`SetProperty`**.

#### 🪵 Gestion d’erreurs et logs
- Adoption d’un modèle unique de gestion des exceptions :
  - `Ex_Business`, `Ex_Infrastructure`, `Ex_Classifier` appliqués à toutes les pages.
- Intégration systématique du service **`IS_LogAndNotify`** pour les événements critiques.

#### 🧩 Documentation technique
- Chaque ViewModel intègre désormais une documentation XML complète :
  - Contexte fonctionnel
  - Objectif utilisateur
  - Vue associée
  - Liste des sections fonctionnelles
  - Spécificités techniques

#### 🧠 Architecture
- Standardisation de la structure des fichiers et namespaces :
  - `D_Presentation.ViewModels.Pages`
  - `D_Presentation.ViewModels.Generic`
- Conformité totale avec les principes de la plateforme **ERP_LeVerandier** servant de base.

#### 🏷 Version
> Numéro de build : **1.4.0.0**  
> Date de publication : 16 October 2025  
> Auteurs : **Département Informatique — Le Vérandier by Tryba**

---

## [1.4.0.0] – 2025-10-09
### 🔧 Refactorisation complète des ViewModels des menu horizontaux
- Intégration de nouvelles **méthodes génériques de gestion de l’état de traitement** dans `VM_MH_Page_Generic` :
  - `IsProcessing` : propriété centralisée indiquant les opérations en cours.
  - `BeginProcessing()` et `EndProcessing()` : méthodes protégées pour gérer uniformément le curseur d’attente et la synchronisation des traitements asynchrones dans toutes les pages.
  - Uniformisation des méthodes de traçabilité via **`BuildFirstCallChain()`**.
- Normalisation complète de la **documentation XML** des fichiers `VM_MH_PageXX`, selon la structure standardisée :
  - **Description générale**
  - **Contexte**
  - **Objectif**
  - **Héritage**
- Uniformisation du nommage des méthodes et commandes (`ExecuteXxx`, `XxxCommand`, `_callee`).
- Harmonisation du style de code et des chaînes d’appel (`callChain = $"{_callee} > {nameof(Méthode)}";`).

#### 🔧 Modifié
- Refactorisation complète des fichiers du module *MenuHorizontal* pour alignement avec les principes **MVVM**, **DDD** et **Clean Architecture** :
  - `VM_MH_Page10` à `VM_MH_Page99`
- Déplacement de la gestion de l’état de traitement (`IsProcessing`, `Mouse.OverrideCursor`) depuis les ViewModels dérivés vers `VM_MH_Page_Generic`.

#### 🧹 Nettoyé
- Suppression du code redondant dans les ViewModels (`IsProcessing`, `try/finally` répétitifs).
- Amélioration de la lisibilité des commentaires XML.

#### 📘 Documentation
- Création d’un **modèle standard de commentaire XML** applicable à toutes les classes `VM_MH_PageXX`, garantissant la cohérence documentaire du module *MenuHorizontal* :
  - `<para><b>Description :</b></para>`
  - `<para><b>Contexte :</b></para>`
  - `<para><b>Objectif :</b></para>`
  - `<para><b>Héritage :</b></para>`

---

## [1.3.1.3] — 2025-07-15  
### ⚙️ Version précédente (stabilisation)
- Stabilisation des fonctionnalités de stock et d’approvisionnement.  
- Améliorations mineures sur les vues `Page30`, `Page31` et `Page32`.  
- Correction des anomalies de navigation entre les pages.  
- Optimisation du temps de chargement des listes d’articles et de chariots.  

---

## 📘 Historique des versions
| Version | Date | Description |
|----------|------|--------------|
| **1.4.0.0** | 2025-10-16 | Refactorisation complète des ViewModels et intégration générique |
| **1.3.1.3** | 2025-07-15 | Version stable post-intégration |
| **1.3.0.0** | 2025-06-10 | Introduction de la base multi-langue |
| **1.2.0.0** | 2025-05-01 | Ajout du module de gestion des barres neuves |
| **1.0.0.0** | 2025-02-01 | Première version publique de BatchStockRelease |

---

## 🧩 Structure des versions
| Type de version | Description |
|-----------------|--------------|
| **MAJEURE (1)** | Changements d’architecture ou de fonctionnalités principales. |
| **MINEURE (4)** | Améliorations techniques, refactorings, ou ajouts non disruptifs. |
| **CORRECTIVE (0)** | Corrections de bugs et stabilisations mineures. |
| **BUILD (0)** | Numéro de compilation interne. |

---
