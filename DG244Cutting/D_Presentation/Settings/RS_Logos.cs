namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Référentiel statique des URI d'accès aux ressources logos de l'application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Composant RS_ (Référentiel Statique) défini dans
    /// <c>D_Presentation/Settings</c>. Il agrège les URI <c>pack://</c> pointant
    /// vers les fichiers de logo stockés sous <c>D_Presentation/Resources/Logos/</c>
    /// de la présente assembly.
    /// </para>
    /// <para>
    /// Objectif : Centraliser la résolution des chemins de ressources logos
    /// afin que tout composant consommateur référence un logo via un identifiant
    /// typé stable, sans coder en dur de chemin <c>pack://</c>. Cette centralisation
    /// garantit que tout déplacement, ajout ou suppression d'une ressource est
    /// répercuté en un seul point.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer un ensemble immuable d'URI absolues vers les logos de l'application.</description></item>
    ///   <item><description>Encapsuler la construction des URI <c>pack://</c> derrière une base nommée (<see cref="LogosBase"/>) afin d'éviter toute duplication de chaîne dans les déclarations publiques.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune logique de stylisation : la mise en forme des contrôles consommateurs relève de <c>IS_ControlStyler</c>.</description></item>
    ///   <item><description>Ne porte aucun état mutable ni aucune préférence utilisateur : tout choix d'affichage dynamique relève d'un Setting (<c>SE_*</c>).</description></item>
    ///   <item><description>Ne participe pas à la CallChain : un référentiel statique n'orchestre aucun flux et ne propage aucune trace.</description></item>
    /// </list>
    /// <para>
    /// Nature « Référentiel Statique » : conformément à la section 2.7.5 du
    /// référentiel, un RS_ contient des données stables au runtime, non persistées
    /// en base, et résolues en compilation lorsque c'est techniquement possible.
    /// Les URI exposés ici sont construits par concaténation à partir d'une
    /// constante <c>const string</c>, ce qui supprime tout problème d'ordre
    /// d'initialisation statique.
    /// </para>
    /// </remarks>
    internal static class RS_Logos
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Préfixe <c>pack://</c> pointant vers le dossier <c>Resources/Logos</c>
        /// de l'assembly courante <c>DG244Cutting</c>. Constante compilée afin de
        /// supprimer toute dépendance à l'ordre d'initialisation des champs
        /// statiques.
        /// </summary>
        private const string LogosBase = "pack://application:,,,/DG244Cutting;component/D_Presentation/Resources/Logos/";

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // --------- Logos ---------

        /// <summary>URI du logo principal en couleurs.</summary>
        public static readonly Uri Logo_Source = new(LogosBase + "RE_logo.png", UriKind.Absolute);

        /// <summary>URI du logo en variante noir sur blanc.</summary>
        public static readonly Uri LogoBW_Source = new(LogosBase + "RE_logo_BW.png", UriKind.Absolute);

        /// <summary>URI du logo en variante blanc sur noir.</summary>
        public static readonly Uri LogoWB_Source = new(LogosBase + "RE_logo_WB.png", UriKind.Absolute);

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}