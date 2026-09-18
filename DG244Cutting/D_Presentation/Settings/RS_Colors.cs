using System.Windows.Media;

namespace DG244Cutting.D_Presentation.Settings
{
    /// <summary>
    /// Référentiel statique des pinceaux de couleur de l'application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Composant RS_ (Référentiel Statique) défini dans
    /// <c>D_Presentation/Settings</c>. Il agrège sous forme de
    /// <see cref="Brush"/> figés l'ensemble des teintes employées par la couche
    /// de présentation. Sa consommation s'effectue par référence directe à la
    /// classe statique depuis les composants de la même couche - convertisseurs
    /// de mise en forme conditionnelle, ViewModels, vues - sans injection et
    /// sans médiation contractuelle.
    /// </para>
    /// <para>
    /// Objectif : Constituer le point unique de résolution des teintes de
    /// l'application. Plusieurs pages de l'atelier aluminium font porter à la
    /// couleur une information opérationnelle directe : le code de délai de
    /// production de la Page10 est rendu par un dégradé à sept pas, et les états
    /// d'une barre comme d'une découpe sont signalés sur la Page11 par un vert et
    /// un rouge que l'opérateur lit d'un coup d'oeil avant même de lire la ligne.
    /// La cohérence de ces teintes entre les écrans conditionne la fiabilité de
    /// cette lecture rapide ; un référentiel unique garantit qu'un ajustement de
    /// teinte est répercuté partout et qu'aucune dérive silencieuse ne s'installe
    /// entre deux pages.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer un ensemble immuable de pinceaux figés, à raison d'une entrée et d'une seule par teinte physique, sans alias ni doublon de valeur.</description></item>
    ///   <item><description>Encapsuler la construction et le gel des pinceaux derrière une fabrique unique (<see cref="CreateFrozen"/>), afin qu'une seule forme de déclaration coexiste dans le bloc des membres exposés.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne déclare aucun alias par rôle : la classe porte des teintes, non des rôles applicatifs. Le rattachement d'une teinte à un état métier relève du composant qui la consomme.</description></item>
    ///   <item><description>Ne porte aucune logique de stylisation : l'application d'un pinceau à un contrôle relève de <c>IS_ControlStyler</c>.</description></item>
    ///   <item><description>Ne porte aucun état mutable, aucune préférence utilisateur et aucune donnée dépendant du contexte d'exécution : de telles valeurs relèvent d'un Setting (<c>SE_*</c>).</description></item>
    ///   <item><description>Ne participe pas à la CallChain : un référentiel statique n'orchestre aucun flux et ne propage aucune trace.</description></item>
    /// </list>
    /// <para>
    /// Nature « Référentiel Statique » : conformément à la section 2.7.5 du
    /// référentiel, un RS_ contient des données stables au runtime, non persistées
    /// en base, et résolues en compilation lorsque c'est techniquement possible.
    /// Les pinceaux exposés ici ne sont pas résolubles en compilation, un
    /// <see cref="SolidColorBrush"/> n'étant pas une constante de compilation :
    /// ils sont préalloués par appel à une méthode privée statique. Ce patron
    /// n'introduit aucune dépendance à l'ordre d'initialisation des champs
    /// statiques, un appel de méthode n'étant pas une référence de champ.
    /// </para>
    /// <para>
    /// Double source de vérité assumée avec le projet Shared : la matière
    /// chromatique de l'application est aujourd'hui hébergée par le projet Shared,
    /// qui n'est pas développé selon le modèle de développement en vigueur mais a
    /// vocation à l'être. Le présent référentiel applique dès maintenant, sur le
    /// périmètre DG244Cutting, la forme que le référentiel de couleurs partagé
    /// devra prendre, et sert de base à cette migration. Tant qu'elle n'est pas
    /// engagée, Shared demeure inchangé et les teintes communes aux deux
    /// périmètres sont déclarées de part et d'autre.
    /// </para>
    /// <para>
    /// Certaines entrées n'ont aucun site d'appel dans DG244Cutting : elles
    /// préparent la migration de Shared. L'absence de consommateur immédiat ne
    /// constitue pas une non-conformité.
    /// </para>
    /// </remarks>
    internal static class RS_Colors
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Propriétés publiques ===

        // --------- Neutres ---------

        /// <summary>Blanc pur #FFFFFF.</summary>
        public static readonly Brush White_Brush = CreateFrozen(Color.FromRgb(0xFF, 0xFF, 0xFF));

        /// <summary>Gris clair #BFBFBF.</summary>
        public static readonly Brush Grey_Brush = CreateFrozen(Color.FromRgb(0xBF, 0xBF, 0xBF));

        /// <summary>Anthracite bleuté #29293D.</summary>
        public static readonly Brush Anthracite_Brush = CreateFrozen(Color.FromRgb(0x29, 0x29, 0x3D));

        // --------- Bleus ---------

        /// <summary>Bleu clair #0C9BEA.</summary>
        public static readonly Brush BlueLight_Brush = CreateFrozen(Color.FromRgb(0x0C, 0x9B, 0xEA));

        /// <summary>Bleu moyen #0472B9.</summary>
        public static readonly Brush BlueMedium_Brush = CreateFrozen(Color.FromRgb(0x04, 0x72, 0xB9));

        /// <summary>Bleu pur #0000FF. Code de délai de production 1 de la Page10.</summary>
        public static readonly Brush Blue_Brush = CreateFrozen(Color.FromRgb(0x00, 0x00, 0xFF));

        // --------- Verts ---------

        /// <summary>Vert #59C64A. Barre utilisée et découpe coupée, onglets 4 et 5 de la Page11.</summary>
        public static readonly Brush Green_Brush = CreateFrozen(Color.FromRgb(0x59, 0xC6, 0x4A));

        // --------- Rouges ---------

        /// <summary>
        /// Rouge #FF3E3E. Barre refusée et découpe refusée, onglets 4 et 5 de la
        /// Page11 ; code de délai de production 4 de la Page10.
        /// </summary>
        public static readonly Brush Red_Brush = CreateFrozen(Color.FromRgb(0xFF, 0x3E, 0x3E));

        // --------- Oranges et jaunes ---------

        /// <summary>Orange clair #FFB245.</summary>
        public static readonly Brush OrangeLight_Brush = CreateFrozen(Color.FromRgb(0xFF, 0xB2, 0x45));

        /// <summary>Orange #FF6714. Code de délai de production 2 de la Page10.</summary>
        public static readonly Brush Orange_Brush = CreateFrozen(Color.FromRgb(0xFF, 0x67, 0x14));

        /// <summary>Ocre #F29324. Code de délai de production 6 de la Page10.</summary>
        public static readonly Brush Ocher_Brush = CreateFrozen(Color.FromRgb(0xF2, 0x93, 0x24));

        /// <summary>Jaune #E3B30C. Code de délai de production 3 de la Page10.</summary>
        public static readonly Brush Yellow_Brush = CreateFrozen(Color.FromRgb(0xE3, 0xB3, 0x0C));

        // --------- Violets et roses ---------

        /// <summary>Violet #850E58. Code de délai de production 0 de la Page10.</summary>
        public static readonly Brush Violet_Brush = CreateFrozen(Color.FromRgb(0x85, 0x0E, 0x58));

        /// <summary>Rose #FFAEC9. Code de délai de production 5 de la Page10.</summary>
        public static readonly Brush Pink_Brush = CreateFrozen(Color.FromRgb(0xFF, 0xAE, 0xC9));

        // --------- Transparence ---------

        /// <summary>
        /// Transparent. Repli des convertisseurs de mise en forme conditionnelle.
        /// </summary>
        /// <remarks>
        /// Construit à partir de <see cref="Colors.Transparent"/> et non de
        /// <see cref="Color.FromRgb"/>, cette signature forçant le canal alpha à
        /// 255 et ne pouvant donc pas exprimer la transparence.
        /// </remarks>
        public static readonly Brush Transparent_Brush = CreateFrozen(Colors.Transparent);

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Construit un <see cref="SolidColorBrush"/> figé
        /// (<see cref="Freezable.Freeze"/>) pour la couleur fournie.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : fabrique d'initialisation des champs <c>static readonly</c>
        /// du bloc des membres exposés. Elle est le seul point de construction de
        /// la classe, de sorte qu'une unique forme de déclaration coexiste dans ce
        /// bloc.
        /// </para>
        /// <para>
        /// Objectif : le gel supprime les allocations répétées au point d'appel,
        /// supprime la mécanique de notification propre aux
        /// <see cref="Freezable"/>, évite le clonage du pinceau lorsqu'il est
        /// référencé depuis un style ou un modèle, et autorise son partage entre
        /// threads.
        /// </para>
        /// <para>
        /// Aucun contrôle de gelabilité n'est effectué : un
        /// <see cref="SolidColorBrush"/> construit sur une couleur littérale, sans
        /// animation ni sous-objet, est toujours gelable.
        /// </para>
        /// </remarks>
        /// <param name="color">Couleur du pinceau à construire.</param>
        /// <returns>
        /// Un <see cref="SolidColorBrush"/> figé, rendu sous le type de base
        /// <see cref="Brush"/>. Ne retourne jamais <see langword="null"/> et ne
        /// lève aucune exception.
        /// </returns>
        private static Brush CreateFrozen(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        #endregion
    }
}