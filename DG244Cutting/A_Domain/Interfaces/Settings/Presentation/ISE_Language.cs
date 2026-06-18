namespace DG244Cutting.A_Domain.Interfaces.Settings.Presentation
{
    /// <summary>
    /// Contrat d'accès à l'état du dictionnaire de langue actif de l'application.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat Singleton défini dans <c>A_Domain</c>, consommé principalement
    /// par <c>SR_Language</c> et <c>SR_Dictionary</c> via injection de dépendances.</para>
    /// <para>Objectif : Exposer l'état linguistique partagé (dictionnaire actif, résolution
    /// d'URI) sans aucune dépendance vers WPF, afin de préserver l'indépendance de <c>A_Domain</c>.
    /// La classe d'implémentation est seule à connaître <c>ResourceDictionary</c>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Résoudre l'URI du fichier de ressources correspondant à un code culture.</item>
    /// <item>Charger et appliquer le dictionnaire de langue à partir d'une URI.</item>
    /// <item>Exposer l'accès aux textes traduits du dictionnaire actif.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier.</item>
    /// <item>Aucun accès aux données.</item>
    /// <item>Aucune orchestration de flux applicatif.</item>
    /// <item>Aucune propagation de CallChain.</item>
    /// <item>Aucune référence directe aux types WPF (<c>ResourceDictionary</c> encapsulé dans l'implémentation).</item>
    /// </list>
    /// </remarks>
    public interface ISE_Language
    {
        // --- Groupe 1 : Résolution de l'URI ---

        /// <summary>
        /// Retourne l'URI du fichier de ressources XAML correspondant au code culture fourni.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>SR_Language</c> avant le chargement du dictionnaire.</para>
        /// <para>Objectif : Centraliser la correspondance culture → URI dans un état partagé injectable.</para>
        /// </remarks>
        /// <param name="cultureCode">Code culture à résoudre (ex. : <c>"fr-FR"</c>, <c>"en-GB"</c>).</param>
        /// <returns>URI du fichier XAML correspondant, ou l'URI française par défaut si la culture est inconnue.</returns>
        Uri GetDictionaryUri(string cultureCode);

        // --- Groupe 2 : Chargement du dictionnaire ---

        /// <summary>
        /// Charge et applique le dictionnaire de langue correspondant à l'URI fournie.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>SR_Language</c> lors du démarrage ou d'un changement de langue.</para>
        /// <para>Objectif : Injecter le nouveau dictionnaire dans les ressources WPF fusionnées
        /// et le persister comme dictionnaire actif, sans exposer <c>ResourceDictionary</c> à l'extérieur.</para>
        /// </remarks>
        /// <param name="dictionaryUri">URI du fichier XAML à charger.</param>
        void LoadDictionary(Uri dictionaryUri);

        // --- Groupe 3 : Accès aux textes traduits ---

        /// <summary>
        /// Retourne le texte traduit correspondant à la clé fournie, ou <see langword="null"/> si indisponible.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>SR_Dictionary</c> pour résoudre chaque clé de traduction.</para>
        /// <para>Objectif : Fournir un accès simple et découplé aux textes du dictionnaire actif,
        /// sans exposer <c>ResourceDictionary</c> aux consommateurs.</para>
        /// </remarks>
        /// <param name="key">Clé de traduction à rechercher.</param>
        /// <returns>Texte traduit si la clé existe et le dictionnaire est chargé ; <see langword="null"/> sinon.</returns>
        string? GetEntry(string key);
    }
}