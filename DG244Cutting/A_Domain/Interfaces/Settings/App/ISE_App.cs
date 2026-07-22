using System.ComponentModel;

namespace DG244Cutting.A_Domain.Interfaces.Settings.App
{
    /// <summary>
    /// Contrat d'accès aux paramètres globaux de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat Singleton défini dans <c>A_Domain</c>, consommé par
    /// injection de dépendances dans les composants autorisés (UseCases, ViewModels).</para>
    /// <para>Objectif : Exposer l'état applicatif partagé via une abstraction découplée
    /// de l'implémentation <see cref="DG244Cutting.B_UseCases.Settings.App.SE_App"/>. Les
    /// propriétés mutables dont l'évolution est associée à un effet de bord (émission
    /// d'événements) sont exposées en lecture seule par le contrat ; leur écriture est
    /// canalisée par les méthodes publiques dédiées.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Exposer les constantes d'identification et de configuration applicative</item>
    /// <item>Exposer l'état de connexion à la base de données et les délais de surveillance</item>
    /// <item>Exposer les chemins de répertoires partagés</item>
    /// <item>Permettre la notification de changements d'état via <see cref="INotifyPropertyChanged"/></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier ni règle décisionnelle</item>
    /// <item>Aucun accès aux données (repository, base, service externe)</item>
    /// <item>Aucune orchestration de flux applicatif</item>
    /// <item>Aucune configuration visuelle (couleurs, polices, marges)</item>
    /// </list>
    /// </remarks>
    public interface ISE_App : INotifyPropertyChanged
    {
        // --- Groupe 1 : Événements ---

        /// <summary>
        /// Événement levé lorsque l'application perd la connexion à la base de données.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Déclenché par <see cref="NotifyConnectionLost"/>.</para>
        /// <para>Objectif : Permettre à la couche présentation de suspendre les processus
        /// temps réel et d'afficher la modale de déconnexion.</para>
        /// </remarks>
        event EventHandler? ConnectionLost;

        /// <summary>
        /// Événement levé lorsque l'application retrouve la connexion à la base de données.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Déclenché par <see cref="NotifyConnectionRestored"/>.</para>
        /// <para>Objectif : Permettre à la couche présentation de relancer les tâches
        /// suspendues et de fermer la modale d'attente.</para>
        /// </remarks>
        event EventHandler? ConnectionRestored;

        // --- Groupe 2 : Identité applicative ---

        /// <summary>
        /// Obtient ou définit le titre principal de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Titre principal de l'application, affiché par la fenêtre
        /// principale via binding sur la propriété <c>Title</c> du composant WPF.</para>
        /// <para>Objectif : Centraliser le titre applicatif dans un état partagé injectable,
        /// consommé par la couche de présentation et susceptible d'évoluer en cours d'exécution
        /// (par exemple à des fins de signalement d'environnement ou de mode actif).</para>
        /// </remarks>
        string ApplicationTitle { get; set; }

        /// <summary>Obtient l'identifiant unique de l'application dans l'écosystème ERP.</summary>
        int AppId { get; }

        /// <summary>Obtient l'identifiant d'action associé à l'accès utilisateur dans l'ERP.</summary>
        int AppAction { get; }

        /// <summary>Obtient l'environnement d'exécution courant (<c>"Dev"</c> ou <c>"Prod"</c>).</summary>
        string Environment { get; }

        // --- Groupe 3 : Dates (calculées à chaque accès) ---

        /// <summary>Obtient la date du jour, sans composante horaire.</summary>
        DateTime AppDate { get; }

        /// <summary>Obtient la date et l'heure système actuelles.</summary>
        DateTime AppDateTime { get; }

        // --- Groupe 4 : Chemin ressources communes (calculé) ---

        /// <summary>Obtient le chemin absolu du répertoire des ressources communes (<c>SharedProjectPath</c>).</summary>
        Uri SharedProjectPath { get; }

        // --- Groupe 5 : Délais applicatifs (immuables) ---

        /// <summary>Obtient le délai (en secondes) avant la fermeture automatique d'une commande utilisateur.</summary>
        int CloseCommandDelay { get; }

        /// <summary>Obtient le délai (en secondes) entre deux vérifications de nouveaux messages.</summary>
        int MessageCheckDelay { get; }

        /// <summary>Obtient le délai (en secondes) de la durrée d'affichage d'une notification.</summary>
        int NotificationDelay { get; }

        // --- Groupe 6 : Code culture applicatif (mutable) ---

        /// <summary>
        /// Obtient ou définit le code culture de la langue applicative courante (ex. : <c>"fr-FR"</c>, <c>"en-GB"</c>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Culture appliquée à l'ensemble de l'application, au format BCP 47.
        /// Le cycle de changement de langue qui pilote l'écriture de cette propriété est décrit en
        /// partie 3 (3.9.5).</para>
        /// <para>Objectif : Centraliser le code culture actif dans un état partagé injectable,
        /// consommé pour la résolution des libellés multilingues.</para>
        /// </remarks>
        string AppCultureCode { get; set; }

        // --- Groupe 7 : État messages utilisateur (mutable) ---

        /// <summary>Obtient ou définit un indicateur signalant la présence de messages non lus.</summary>
        bool HasUnreadMessages { get; set; }

        // --- Groupe 8 : Monitoring connexion base de données (lecture seule via le contrat) ---

        /// <summary>
        /// Obtient l'état de connexion courant de l'application.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="NotifyConnectionLost"/> et
        /// <see cref="NotifyConnectionRestored"/>. Cette restriction garantit que toute mutation
        /// est accompagnée de l'émission de l'événement correspondant.
        /// </remarks>
        bool IsConnected { get; }

        /// <summary>Obtient l'intervalle (en secondes) entre deux vérifications de la connexion.</summary>
        int DatabaseCheckInterval { get; }

        /// <summary>Obtient le délai du premier cycle de reconnexion rapide (en secondes).</summary>
        int DatabaseCheckFirstLoop { get; }

        /// <summary>Obtient le délai du second cycle de reconnexion lente (en secondes).</summary>
        int DatabaseCheckSecondLoop { get; }

        // --- Groupe 9 : Répertoires réseau partagés (immuables) ---

        /// <summary>Obtient le chemin UNC du premier serveur réseau partagé.</summary>
        string DirectoryPathServer1 { get; }

        /// <summary>Obtient le chemin UNC du second serveur réseau partagé.</summary>
        string DirectoryPathServer2 { get; }

        /// <summary>Obtient le chemin relatif du premier répertoire GED.</summary>
        string DirectoryPathGed1 { get; }

        /// <summary>Obtient le chemin relatif du deuxième répertoire GED.</summary>
        string DirectoryPathGed2 { get; }

        /// <summary>Obtient le chemin relatif du troisième répertoire GED (liste de débit).</summary>
        string DirectoryPathGed3 { get; }

        /// <summary>Obtient le chemin relatif du modèle de projet XLSM.</summary>
        string DirectoryPathProjectModel { get; }

        /// <summary>Obtient le chemin absolu du modèle de lot de fabrication XLSM.</summary>
        string DirectoryPathBatchModel { get; }

        /// <summary>Obtient le chemin relatif du dossier des lots de fabrication.</summary>
        string DirectoryPathLotsDeFabrication { get; }

        /// <summary>Obtient le chemin relatif du dossier machine Elumatec.</summary>
        string DirectoryPathElumatec { get; }

        /// <summary>Obtient le chemin relatif du sous-dossier Elumatec 1.</summary>
        string DirectoryPathElumatec1 { get; }

        /// <summary>Obtient le chemin relatif du sous-dossier Elumatec 2.</summary>
        string DirectoryPathElumatec2 { get; }

        /// <summary>Obtient le chemin relatif du sous-dossier Elumatec 3.</summary>
        string DirectoryPathElumatec3 { get; }

        // --- Groupe 10 : Opérations atomiques ---

        /// <summary>
        /// Notifie la perte de connexion à la base de données.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le mécanisme de surveillance lorsque la base devient inaccessible.</para>
        /// <para>Objectif : Mettre à jour <see cref="IsConnected"/> à <see langword="false"/>
        /// et déclencher l'événement <see cref="ConnectionLost"/>.</para>
        /// </remarks>
        void NotifyConnectionLost();

        /// <summary>
        /// Notifie la restauration de la connexion à la base de données.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le mécanisme de surveillance lorsque la base redevient accessible.</para>
        /// <para>Objectif : Mettre à jour <see cref="IsConnected"/> à <see langword="true"/>
        /// et déclencher l'événement <see cref="ConnectionRestored"/>.</para>
        /// </remarks>
        void NotifyConnectionRestored();

        /// <summary>
        /// Réinitialise l'état mutable de l'application aux valeurs par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de l'initialisation du Singleton ou d'un redémarrage applicatif.</para>
        /// <para>Objectif : Garantir un état cohérent et reproductible pour toutes les propriétés modifiables.</para>
        /// </remarks>
        void Reset();
    }
}