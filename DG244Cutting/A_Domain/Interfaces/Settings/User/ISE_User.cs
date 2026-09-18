using System.ComponentModel;
using DG244Cutting.A_Domain.Entities.App;

namespace DG244Cutting.A_Domain.Interfaces.Settings.User
{
    /// <summary>
    /// Définit le contrat du contexte utilisateur courant partagé par l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat Singleton défini dans <c>A_Domain</c>, consommé par
    /// injection de dépendances dans les composants autorisés (UseCases, ViewModels).</para>
    /// <para>Objectif : Exposer l'état utilisateur partagé via une abstraction découplée
    /// de l'implémentation <see cref="DG244Cutting.B_UseCases.Settings.User.SE_User"/>. Les
    /// propriétés dont l'évolution doit rester cohérente (contexte poste, droits d'accès) sont
    /// exposées en lecture seule par le contrat ; leur écriture est canalisée par les méthodes
    /// publiques dédiées.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Exposer l'identité de l'utilisateur courant.</description></item>
    /// <item><description>Exposer le contexte poste courant.</description></item>
    /// <item><description>Exposer l'état de session et de fermeture.</description></item>
    /// <item><description>Exposer les droits d'accès aux pages déjà calculés.</description></item>
    /// <item><description>Permettre la notification de changements d'état via <see cref="INotifyPropertyChanged"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne pas charger de données depuis une base ou un service externe.</description></item>
    /// <item><description>Ne pas contenir de logique métier complexe.</description></item>
    /// <item><description>Ne pas orchestrer le calcul ou le chargement des droits d'accès.</description></item>
    /// </list>
    /// </remarks>
    public interface ISE_User : INotifyPropertyChanged
    {
        // --- Groupe 1 : Identité utilisateur (mutable) ---

        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur applicatif courant.
        /// </summary>
        int AppUserId { get; set; }

        /// <summary>
        /// Obtient ou définit le nom complet de l'utilisateur courant.
        /// </summary>
        string AppUserFullName { get; set; }

        // --- Groupe 2 : Contexte poste (lecture seule via le contrat) ---

        /// <summary>
        /// Obtient l'identifiant du poste courant.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetDeviceContext"/>.
        /// </remarks>
        string AppDeviceId { get; }

        /// <summary>
        /// Obtient l'adresse IP du poste courant.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetDeviceContext"/>.
        /// </remarks>
        string AppDeviceIP { get; }

        /// <summary>
        /// Obtient le nom du compte système du poste courant.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetDeviceContext"/>.
        /// </remarks>
        string AppDeviceUser { get; }

        // --- Groupe 3 : État session et accès (mutable) ---

        /// <summary>
        /// Obtient ou définit l'identifiant de la session applicative courante.
        /// </summary>
        int AppSessionId { get; set; }

        /// <summary>
        /// Obtient ou définit le nombre de tentatives de connexion.
        /// </summary>
        int UserAttempt { get; set; }

        /// <summary>
        /// Obtient ou définit le type de commande de fermeture demandé.
        /// </summary>
        string CloseCommandType { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si une fermeture forcée est demandée.
        /// </summary>
        bool ForceClose { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant de la session sélectionnée.
        /// </summary>
        int SelectedSessionId { get; set; }

        /// <summary>
        /// Obtient ou définit le nom complet associé à la session sélectionnée.
        /// </summary>
        string SelectedSessionFullName { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'utilisateur peut accéder à l'application.
        /// </summary>
        bool CanUserAccessApp { get; set; }

        // --- Groupe 4 : Droits d'accès aux pages (lecture seule via le contrat) ---

        /// <summary>
        /// Obtient les droits d'accès aux pages actuellement stockés.
        /// </summary>
        /// <remarks>
        /// Écriture exclusive via <see cref="SetPageAccessRight"/> et <see cref="ClearPageAccessRights"/>.
        /// </remarks>
        IReadOnlyDictionary<string, PageRights> PagesUserRights { get; }

        // --- Groupe 5 : Opérations atomiques ---

        /// <summary>
        /// Définit de manière atomique le contexte du poste courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par un composant dédié qui résout les informations techniques du poste.</para>
        /// <para>Objectif : Mettre à jour de manière atomique les trois composantes du contexte poste
        /// (identifiant, adresse IP, compte système), sans introduire de logique technique dans la classe de paramètres.</para>
        /// </remarks>
        /// <param name="deviceId">Identifiant du poste.</param>
        /// <param name="deviceIP">Adresse IP du poste.</param>
        /// <param name="deviceUser">Compte système du poste.</param>
        void SetDeviceContext(string deviceId, string deviceIP, string deviceUser);

        /// <summary>
        /// Incrémente le compteur de tentatives de connexion.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée après un échec d'authentification.</para>
        /// <para>Objectif : Centraliser l'évolution du compteur et notifier les observateurs de l'état concret.</para>
        /// </remarks>
        void IncrementUserAttempt();

        /// <summary>
        /// Définit les droits d'accès d'une page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par un composant externe ayant déjà calculé ou chargé les droits applicables.</para>
        /// <para>Objectif : Mettre à jour de manière atomique les droits associés à une page donnée.</para>
        /// </remarks>
        /// <param name="pageCode">Code de la page concernée.</param>
        /// <param name="pageRights">Droits à appliquer à la page.</param>
        /// <exception cref="ArgumentException">Levée lorsque <paramref name="pageCode"/> est vide ou blanc.</exception>
        /// <exception cref="ArgumentNullException">Levée lorsque <paramref name="pageRights"/> est <see langword="null"/>.</exception>
        void SetPageAccessRight(string pageCode, PageRights pageRights);

        /// <summary>
        /// Vide les droits d'accès aux pages.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée avant l'application d'un nouveau jeu de droits calculé par un composant externe.</para>
        /// <para>Objectif : Garantir que l'état interne ne conserve aucun droit obsolète.</para>
        /// </remarks>
        void ClearPageAccessRights();

        /// <summary>
        /// Obtient les droits d'accès d'une page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lorsqu'un composant doit consulter les droits actuellement stockés pour une page.</para>
        /// <para>Objectif : Fournir une vue sûre des droits d'une page sans exposer l'état interne mutable.</para>
        /// </remarks>
        /// <param name="pageCode">Code de la page recherchée.</param>
        /// <returns>Une copie des droits de la page, ou <see langword="null"/> si la page n'est pas connue.</returns>
        PageRights? GetPageRights(string pageCode);

        /// <summary>
        /// Réinitialise l'état utilisateur partagé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée lors de l'initialisation ou lorsqu'un redémarrage logique du contexte utilisateur est nécessaire.</para>
        /// <para>Objectif : Garantir un état vide, cohérent et reproductible.</para>
        /// </remarks>
        void Reset();
    }
}