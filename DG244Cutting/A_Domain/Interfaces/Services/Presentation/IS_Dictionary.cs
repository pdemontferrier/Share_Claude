namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Contrat du service d'accès aux textes traduits du dictionnaire de langue actif.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Interface définie dans A_Domain, implémentée par <c>SR_Dictionary</c>
    /// dans B_UseCases. Consommée par les services, UseCases et ViewModels nécessitant un accès
    /// aux libellés traduits de l'interface utilisateur.</para>
    /// <para>Objectif : Exposer un point d'accès unique, traçable et robuste aux textes
    /// traduits, indépendamment du mécanisme WPF sous-jacent.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Résoudre un texte traduit à partir d'une clé de dictionnaire.</item>
    /// <item>Journaliser toute clé manquante ou anomalie d'accès.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune gestion du dictionnaire actif (déléguée à <c>ISE_Language</c>).</item>
    /// <item>Aucune gestion du code culture (déléguée à <c>ISE_App</c>).</item>
    /// <item>Aucune référence aux types WPF.</item>
    /// </list>
    /// </remarks>
    public interface IS_Dictionary
    {
        /// <summary>
        /// Retourne le texte traduit correspondant à la clé fournie.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée depuis tout composant nécessitant un libellé traduit
        /// (services, UseCases, ViewModels).</para>
        /// <para>Objectif : Retourner le texte traduit ou une valeur de repli lisible si la clé
        /// est absente, sans jamais lever d'exception visible pour le flux appelant.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont transmise par l'appelant.</param>
        /// <param name="key">Clé du texte à rechercher dans le dictionnaire actif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Texte traduit correspondant à la clé, ou <c>[key] not found</c> si la clé est absente.</returns>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est demandée.</exception>
        string GetText(string caller, string key, CancellationToken ct = default);
    }
}