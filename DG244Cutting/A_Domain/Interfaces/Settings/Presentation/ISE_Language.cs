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
    /// <item>Convertir un code pays (ISO 3166-1 alpha-2) en code culture .NET BCL.</item>
    /// <item>Extraire un code pays (ISO 3166-1 alpha-2) à partir d'un code culture .NET BCL.</item>
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
        /// <para>Contexte : Appelée par <c>UC_Language_Apply</c> avant le chargement du dictionnaire.</para>
        /// <para>Objectif : Centraliser la correspondance culture → URI dans un état partagé injectable.</para>
        /// </remarks>
        /// <param name="cultureCode">Code culture à résoudre (ex. : <c>"fr-FR"</c>, <c>"en-GB"</c>).</param>
        /// <returns>URI du fichier XAML correspondant, ou l'URI anglaise par défaut si la culture est inconnue.</returns>
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

        // --- Groupe 4 : Conversion entre code pays et code culture ---

        /// <summary>
        /// Retourne le code culture .NET BCL correspondant au code pays ISO 3166-1 alpha-2 fourni.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le composant en charge de la détermination de la langue
        /// d'affichage à partir d'un identifiant pays issu d'un profil utilisateur ou d'une
        /// configuration régionale.</para>
        /// <para>Objectif : Centraliser la résolution codePays → codeCulture sur un référentiel
        /// CLDR de l'Unicode Consortium (élément <c>territoryInfo</c>), aligné sur la convention
        /// .NET BCL des cultureCodes spécifiques. La table de correspondance couvre les 27 pays
        /// de l'Union européenne ainsi qu'une sélection des principaux pays hors UE dont la
        /// langue officielle principale ne pose pas d'ambiguïté. Si le code pays fourni n'est pas
        /// reconnu, est <see langword="null"/>, vide ou composé uniquement de caractères blancs,
        /// la méthode retourne silencieusement <c>"en-GB"</c> par défaut.</para>
        /// </remarks>
        /// <param name="countryCode">Code pays ISO 3166-1 alpha-2 à résoudre (ex. : <c>"FR"</c>, <c>"DE"</c>).</param>
        /// <returns>Code culture .NET BCL correspondant (ex. : <c>"fr-FR"</c>, <c>"de-DE"</c>), ou <c>"en-GB"</c> par défaut.</returns>
        string GetCultureCodeFromCountryCode(string countryCode);

        /// <summary>
        /// Extrait le code pays (ISO 3166-1 alpha-2) à partir d'un code culture .NET BCL.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le composant en charge de l'extraction d'un identifiant
        /// pays normalisé à partir d'un code culture, pour interrogation de référentiels indexés
        /// par pays.</para>
        /// <para>Objectif : Isoler la logique d'extraction et garantir un retour stable et
        /// normalisé en majuscules. L'extraction est robuste aux codes culture BCP-47 à trois
        /// segments présents dans la BCL .NET (typiquement <c>"zh-Hans-CN"</c>, <c>"sr-Latn-RS"</c>,
        /// <c>"uz-Latn-UZ"</c>) grâce au recours à <see cref="string.LastIndexOf(char)"/> sur le
        /// séparateur <c>'-'</c> ; le segment situé après le dernier séparateur est retourné
        /// en majuscules. Si le code culture ne contient aucun séparateur ou se termine par un
        /// séparateur sans segment suivant, le code culture complet est retourné en majuscules.
        /// Si l'entrée est <see langword="null"/>, vide ou composée uniquement de caractères
        /// blancs, la méthode retourne silencieusement <c>"GB"</c> par défaut.</para>
        /// </remarks>
        /// <param name="cultureCode">Code culture .NET BCL à analyser (ex. : <c>"fr-FR"</c>, <c>"zh-Hans-CN"</c>).</param>
        /// <returns>Code pays ISO 3166-1 alpha-2 en majuscules, ou <c>"GB"</c> par défaut si l'entrée est non exploitable.</returns>
        string ExtractCountryCodeFromCulture(string cultureCode);
    }
}