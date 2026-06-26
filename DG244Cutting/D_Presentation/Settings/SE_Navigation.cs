using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Référentiel central de navigation WPF de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette classe est une dépendance injectable enregistrée en Singleton
    /// et consommée via <see cref="ISE_Navigation"/>. Elle réside dans <c>D_Presentation/Settings/</c>
    /// conformément au modèle de Settings distribués défini dans le référentiel normatif, section 2.5.</para>
    /// <para>Objectif : Centraliser la définition des pages connues de l'application —
    /// noms logiques, URIs de vues, URIs de menus horizontaux, région de menu vertical — de manière
    /// stateless, immuable et reproductible, indépendamment de tout état d'exécution.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Générer automatiquement le dictionnaire <see cref="PageMappings"/> à partir de
    ///   conventions de nommage (<c>PageNN.xaml</c>, <c>MH_PageNN.xaml</c>).</item>
    ///   <item>Exposer les accès par nom logique via <see cref="TryGetPageMapping"/>.</item>
    ///   <item>Exposer la page par défaut et l'URI du menu horizontal réduit commun.</item>
    ///   <item>Exposer les noms logiques de la page de connexion et de la page de repli.</item>
    ///   <item>Servir de source d'autorité pour l'existence d'une page dans le système.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucun état d'exécution (page courante, historique de navigation) —
    ///   ce rôle appartient à <c>UC_Navigation</c>.</item>
    ///   <item>Ne vérifie pas les droits d'accès — ce rôle appartient à <c>UC_Navigation</c>
    ///   en coordination avec <c>ISE_User</c>.</item>
    ///   <item>Ne prend aucune décision de navigation — ce rôle appartient à <c>UC_Navigation</c>.</item>
    ///   <item>Ne connaît pas <c>MainWindow</c> ni les frames WPF — ce rôle appartient à
    ///   <c>SR_Navigation</c>.</item>
    /// </list>
    /// </remarks>
    public class SE_Navigation : ISE_Navigation
    {
        #region === Propriétés privées ===

        /// <summary>Pattern relatif des URIs de pages XAML.</summary>
        private const string PagePathPattern = "D_Presentation/Views/Pages/Page{0}.xaml";

        /// <summary>Pattern relatif des URIs de menus horizontaux XAML.</summary>
        private const string MenuPathPattern = "D_Presentation/Views/Components/HorizontalMenus/MH{0}.xaml";

        /// <summary>Nom logique de la page de connexion, partagé entre la propriété
        /// publique <see cref="LoginPageName"/> et l'ajustement déclaratif du constructeur.</summary>
        private const string InitialLoginPageName = "Page00";

        /// <summary>Dictionnaire interne des mappings de pages.</summary>
        private readonly Dictionary<string, PageMapping> _pageMappings;

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient le dictionnaire complet des pages connues de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Ce dictionnaire est consulté par <c>UC_Navigation</c> pour
        /// vérifier l'existence d'une page avant toute tentative de navigation.</para>
        /// <para>Objectif : Retourner une vue en lecture seule du référentiel interne
        /// afin d'interdire toute modification externe.</para>
        /// </remarks>
        public IReadOnlyDictionary<string, PageMapping> PageMappings => _pageMappings;

        /// <summary>
        /// Obtient le nom logique de la page par défaut de l'application.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <c>UC_Navigation</c> lors de l'initialisation
        /// de l'application ou lors d'un repli vers l'état initial.</para>
        /// </remarks>
        public string DefaultPageName => "Page10";

        /// <summary>
        /// Obtient le nom logique de la page de connexion, exemptée du contrôle
        /// des droits d'accès.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <c>UC_Navigation</c> pour identifier
        /// la page sur laquelle aucune vérification de droits n'est appliquée, et
        /// éviter ainsi un blocage de la navigation initiale avant authentification.</para>
        /// </remarks>
        public string LoginPageName => InitialLoginPageName;

        /// <summary>
        /// Obtient le nom logique de la page de repli, vers laquelle toute navigation
        /// non autorisée est silencieusement redirigée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisé par <c>UC_Navigation</c> lorsqu'une tentative
        /// de navigation vers une page pour laquelle l'utilisateur ne dispose pas des
        /// droits requis est interceptée, pour rediriger silencieusement vers la page
        /// de repli sans propager d'erreur à l'interface.</para>
        /// </remarks>
        public string FallbackPageName => "Page99";

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Cette propriété est calculée à la demande à partir du référentiel.
        /// Si <see cref="DefaultPageName"/> n'est pas trouvé dans <see cref="PageMappings"/>, une
        /// <see cref="InvalidOperationException"/> est levée car cela révèle une incohérence de
        /// configuration interne.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Levée si <see cref="DefaultPageName"/> ne figure pas dans <see cref="PageMappings"/>,
        /// ce qui révèle une incohérence de configuration interne.
        /// </exception>
        public PageMapping DefaultPageMapping =>
            _pageMappings.TryGetValue(DefaultPageName, out var mapping)
                ? mapping
                : throw new InvalidOperationException(
                    $"La page par défaut '{DefaultPageName}' est absente du référentiel de navigation. " +
                    $"Vérifier la liste des pages déclarées dans le constructeur de SE_Navigation.");

        /// <summary>
        /// Obtient l'URI du composant de menu horizontal réduit, commun à toutes les pages
        /// qui n'ont pas de menu horizontal propre.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Utilisée par <c>SR_Navigation</c> lors de la transition
        /// vers une nouvelle page (réduction temporaire du menu) et par <c>UC_Navigation</c>
        /// lors de la navigation vers <c>Page00</c>.</para>
        /// </remarks>
        public Uri MH_Reduce_Source { get; } = new Uri(
            "D_Presentation/Views/Components/HorizontalMenus/MH_Reduce.xaml",
            UriKind.Relative);

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        // A compléter

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le référentiel de navigation en générant le dictionnaire des pages
        /// à partir des conventions de nommage de l'application DG244Cutting.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Ce constructeur est appelé par le conteneur d'injection lors de
        /// la création du Singleton. Il ne reçoit aucun paramètre : le référentiel est entièrement
        /// déterminé par les conventions internes du projet.</para>
        /// <para>Objectif : Produire un état initial complet, immuable et cohérent,
        /// sans dépendance vers aucun service externe.</para>
        /// </remarks>
        public SE_Navigation()
        {
            // Déclaration normative des pages connues de l'application DG244Cutting
            // Chaque entrée correspond à un suffixe numérique utilisé dans les conventions
            // de nommage des fichiers : PageNN.xaml et MH_PageNN.xaml
            var pageSuffixes = new[]
            {
                "00", "01", "02", "03",
                "10",
                "20", "21", "22", "23",
                "30", "31", "32", "33",
                "40", "41",
                "50",
                "60",
                "70",
                "90", "97", "98", "99"
            };

            // Génération automatique du dictionnaire par convention
            // La clé est le nom logique "PageNN", les valeurs sont calculées par pattern
            _pageMappings = pageSuffixes.ToDictionary(
                suffix => $"Page{suffix}",
                suffix =>
                {
                    var pageName = $"Page{suffix}";
                    var pageUri = new Uri(string.Format(PagePathPattern, suffix), UriKind.Relative);
                    var mhUri = new Uri(string.Format(MenuPathPattern, suffix), UriKind.Relative);
                    // La région de menu vertical est déterminée par le premier chiffre du suffixe
                    var mvName = $"MV{(suffix.Length > 0 ? suffix[0] : '0')}";

                    return new PageMapping(pageName, pageUri, mhUri, mvName);
                });

            // Ajustement déclaratif : Page00 (connexion) utilise le menu réduit par design,
            // car aucun menu horizontal métier n'est affiché sur la page de connexion.
            if (_pageMappings.TryGetValue(InitialLoginPageName, out var page00Mapping))
            {
                _pageMappings[InitialLoginPageName] = page00Mapping with { MHUri = MH_Reduce_Source };
            }
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Tente d'obtenir le <see cref="PageMapping"/> associé à un nom logique de page.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par <c>UC_Navigation</c> avant toute décision de navigation,
        /// pour valider l'existence de la page cible dans le référentiel.</para>
        /// <para>Objectif : Éviter de propager un nom de page inconnu vers les couches
        /// de présentation ou de service.</para>
        /// </remarks>
        /// <param name="pageName">Nom logique de la page (ex. <c>"Page10"</c>).</param>
        /// <param name="mapping">
        /// Lorsque la méthode retourne <see langword="true"/>, contient le <see cref="PageMapping"/>
        /// correspondant ; sinon, contient la valeur par défaut du type.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la page est connue du référentiel ;
        /// <see langword="false"/> sinon.
        /// </returns>
        public bool TryGetPageMapping(string pageName, out PageMapping mapping)
        {
            if (string.IsNullOrWhiteSpace(pageName))
            {
                mapping = default;
                return false;
            }

            return _pageMappings.TryGetValue(pageName.Trim(), out mapping);
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}