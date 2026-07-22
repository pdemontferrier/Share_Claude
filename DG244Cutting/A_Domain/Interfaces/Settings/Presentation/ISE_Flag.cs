using System.ComponentModel;

namespace DG244Cutting.A_Domain.Interfaces.Settings.Presentation
{
    /// <summary>
    /// Contrat du composant Singleton de présentation gérant le drapeau de langue affiché.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat défini dans <c>A_Domain</c>, consommé par injection
    /// dans les ViewModels et les services de langue autorisés. Constitue l'unique point
    /// d'entrée vers <see cref="DG244Cutting.D_Presentation.Settings.SE_Flag"/>.</para>
    /// <para>Objectif : Exposer l'état du drapeau courant via une abstraction découplée
    /// de l'implémentation, en garantissant la notificabilité des changements.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Exposer l'URI du drapeau actuellement affiché (lecture/écriture)</item>
    /// <item>Exposer l'URI de repli par défaut (lecture seule)</item>
    /// <item>Exposer le référentiel des drapeaux disponibles</item>
    /// <item>Résoudre un drapeau à partir d'un code pays</item>
    /// <item>Réinitialiser le drapeau courant à la valeur par défaut</item>
    /// <item>Permettre la notification de changements d'état via <see cref="INotifyPropertyChanged"/></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier ni orchestration applicative</item>
    /// <item>Aucun accès aux données ou services externes</item>
    /// <item>Aucune propagation de CallChain</item>
    /// </list>
    /// </remarks>
    public interface ISE_Flag : INotifyPropertyChanged
    {
        // --- Groupe 1 : Drapeau par défaut (immuable) ---

        /// <summary>
        /// Obtient l'URI du drapeau par défaut utilisé en cas de repli.
        /// </summary>
        Uri DefaultFlagUri { get; }

        // --- Groupe 2 : Drapeau courant (mutable) ---

        /// <summary>
        /// Obtient ou définit l'URI du drapeau actuellement affiché dans l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Propriété centrale consommée par les ViewModels et composants
        /// visuels liés à la langue active.</para>
        /// <para>Objectif : Permettre la lecture et la mise à jour centralisée du drapeau
        /// courant. Toute modification déclenche une notification <see cref="INotifyPropertyChanged"/>.</para>
        /// </remarks>
        Uri AppFlagUri { get; set; }

        // --- Groupe 3 : Référentiel des drapeaux (lecture seule) ---

        /// <summary>
        /// Obtient le référentiel des drapeaux disponibles, indexé par code pays ISO 3166-1 alpha-2.
        /// </summary>
        IReadOnlyDictionary<string, Uri> ReferenceFlags { get; }

        // --- Groupe 4 : Opérations ---

        /// <summary>
        /// Retourne l'URI du drapeau correspondant au code pays donné, ou le drapeau par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par les services de langue lors du changement de langue
        /// ou de l'initialisation de l'interface.</para>
        /// <para>Objectif : Fournir une résolution centralisée avec repli explicite.</para>
        /// </remarks>
        /// <param name="countryCode">Code pays ISO 3166-1 alpha-2 à résoudre (ex : "FR", "DE").
        /// Une valeur vide, blanche ou non trouvée entraîne le retour du drapeau par défaut.</param>
        /// <returns>URI du drapeau correspondant, ou <see cref="DefaultFlagUri"/> si non trouvé.</returns>
        Uri GetFlagUriOrDefault(string countryCode);

        /// <summary>
        /// Réinitialise le drapeau courant à la valeur par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée à l'initialisation de l'application ou en cas de repli.</para>
        /// <para>Objectif : Restaurer un état d'affichage stable et cohérent.</para>
        /// </remarks>
        void ResetToDefault();
    }
}