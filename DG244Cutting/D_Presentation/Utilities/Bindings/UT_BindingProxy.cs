using System.Windows;

namespace DG244Cutting.D_Presentation.Utilities.Bindings
{
    /// <summary>
    /// Proxy de propagation du DataContext WPF pour les branches du LogicalTree
    /// ne participant pas à l'héritage standard du DataContext.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Certaines branches XAML de WPF — en particulier les descendants d'un
    /// FlowDocument (Paragraph, Run, ListItem...), les ContextMenu détachés du
    /// LogicalTree de la page, les DataGridColumn et les ToolTip non rattachés à un
    /// élément hôte — ne participent pas à la propagation standard du DataContext.
    /// Les éléments XAML résidant dans ces branches ne peuvent pas binder directement
    /// sur les propriétés du ViewModel hôte de la page, ce qui interdit l'expression
    /// déclarative de libellés multilingues ou de données dynamiques dans ces zones.
    /// </para>
    /// <para>
    /// Objectif : Fournir un objet Freezable, déclarable en ressource de page, porteur
    /// d'une unique DependencyProperty publique nommée Data, servant de canal indirect
    /// de propagation du DataContext. La ressource capte le DataContext de la page via
    /// Data="{Binding}" ; les éléments XAML non-héritiers indirectent ensuite leur
    /// binding via Source={StaticResource Proxy}, Path=Data.X.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer une DependencyProperty publique Data de type object, captable depuis une ressource XAML.</description></item>
    ///   <item><description>Honorer le contrat Freezable de WPF par la surcharge de CreateInstanceCore.</description></item>
    ///   <item><description>Servir indistinctement n'importe quelle page exposant un FlowDocument ou tout autre conteneur non-héritier du DataContext.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune logique métier ni aucun traitement applicatif.</description></item>
    ///   <item><description>Ne connaît ni les ViewModels, ni les Services, ni les UseCases : la liaison s'opère exclusivement par binding WPF.</description></item>
    ///   <item><description>Ne participe à aucune mécanique transverse du référentiel (CallChain, annulation coopérative, gestion d'erreurs, Event Store, transactionnalité, multilingue, navigation).</description></item>
    ///   <item><description>Ne dépend d'aucun service injecté et n'est pas enregistrée dans le conteneur DI (SR_ConteneurDI).</description></item>
    /// </list>
    /// <para>Usage type :</para>
    /// <code>
    /// &lt;UserControl.Resources&gt;
    ///     &lt;bindings:UT_BindingProxy x:Key="Proxy" Data="{Binding}" /&gt;
    /// &lt;/UserControl.Resources&gt;
    ///
    /// &lt;FlowDocumentScrollViewer&gt;
    ///     &lt;FlowDocument&gt;
    ///         &lt;Paragraph&gt;
    ///             &lt;Run Text="{Binding Source={StaticResource Proxy}, Path=Data.Titre}" /&gt;
    ///         &lt;/Paragraph&gt;
    ///     &lt;/FlowDocument&gt;
    /// &lt;/FlowDocumentScrollViewer&gt;
    /// </code>
    /// <para>
    /// Statut hors familles canoniques : La classe UT_BindingProxy relève de la famille
    /// UT_ (Utility WPF) au sens de 2.7 du 0230 (Famille 6 - Utilitaires). Cette famille
    /// n'est pas couverte par la règle de parité interface/implémentation posée en
    /// 2.7.4 du 0230 et indexée par R-2.7.6 du 0231 ; aucun contrat IS_ ou autre n'est
    /// attendu.
    /// </para>
    /// <para>
    /// Exceptions architecturales : La classe ne porte aucune exception architecturale
    /// propre. Aucune ligne EA-NN ne lui est associée à l'inventaire de 4.15.9 du 0230.
    /// </para>
    /// <para>
    /// Note d'attribution historique : Le pattern Freezable porteur d'une
    /// DependencyProperty Data servant de proxy de propagation du DataContext est
    /// historiquement attribué à Josh Smith. Son emploi dans le présent projet
    /// constitue une application standard et largement documentée du pattern WPF
    /// idiomatique.
    /// </para>
    /// </remarks>
    public class UT_BindingProxy : Freezable
    {
        #region === Propriétés privées ===

        // Aucun champ d'état interne : la classe est sans état au sens de la famille UT_ (2.7 du 0230, Famille 6 - Utilitaires).

        #endregion

        #region === Dépendances privées ===

        // Aucune dépendance injectée : la famille UT_ ne consomme aucun service du conteneur DI (2.7 du 0230, Famille 6 - Utilitaires).

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// DependencyProperty publique enregistrée sous le nom logique "Data", support du
        /// canal de propagation indirect du DataContext pour les branches du LogicalTree
        /// WPF non-héritières de l'héritage standard.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : DependencyProperty obligatoire pour qu'un Freezable serve de proxy
        /// de binding en XAML. La déclaration sous forme de champ statique public en
        /// lecture seule est la forme canonique imposée par WPF pour toute propriété
        /// de dépendance ; elle est enregistrée auprès du système de propriétés WPF par
        /// DependencyProperty.Register, avec pour nom logique "Data", pour type de valeur
        /// object, pour type propriétaire UT_BindingProxy, et pour métadonnée par défaut
        /// UIPropertyMetadata(null).
        /// </para>
        /// <para>
        /// Objectif : Servir de support de stockage et de notification pour la propriété
        /// d'instance Data. Toute écriture ou lecture sur Data passe par GetValue et
        /// SetValue sur cette DependencyProperty, ce qui permet à la propriété d'être
        /// la cible et la source de bindings WPF dans le LogicalTree d'une page
        /// consommatrice de UT_BindingProxy.
        /// </para>
        /// </remarks>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data",
                typeof(object),
                typeof(UT_BindingProxy),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Propriété d'instance publique exposée au binding XAML, façade typée de
        /// DataProperty et canal effectif de propagation du DataContext capté.
        /// </summary>
        /// <value>
        /// Valeur courante du DataContext capté par la ressource UT_BindingProxy
        /// déclarée en Page.Resources, ou null si aucun DataContext n'a encore été
        /// propagé sur la ressource.
        /// </value>
        /// <remarks>
        /// <para>
        /// Contexte : Façade typée d'accès à DataProperty pour les consommateurs WPF.
        /// L'implémentation repose sur la paire GetValue / SetValue indirectant la
        /// lecture et l'écriture vers le système de propriétés de dépendance,
        /// conformément au patron canonique des DependencyProperty.
        /// </para>
        /// <para>
        /// Objectif : Exposer un point de binding unique nommé Data sur la ressource
        /// UT_BindingProxy déclarée en Page.Resources. La page capte son DataContext
        /// par Data="{Binding}" sur la ressource ; les éléments XAML résidant dans
        /// une branche non-héritière du DataContext (descendants d'un FlowDocument,
        /// ContextMenu détaché, DataGridColumn, ToolTip détaché) accèdent ensuite au
        /// DataContext par Source={StaticResource Proxy}, Path=Data.NomDeLaProprieteCible.
        /// </para>
        /// </remarks>
        public object? Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        #endregion

        #region === Constructeur ===

        // Aucun constructeur explicite : le constructeur par défaut implicite suffit, conformément à la nature sans état et sans dépendance injectée de la classe (famille UT_, 2.7 du 0230).

        #endregion

        #region === Méthodes publiques ===

        // Aucune méthode publique : la surface comportementale est entièrement portée par la propriété publique Data et la DependencyProperty associée déclarées en région "Propriétés publiques".

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Crée et retourne une nouvelle instance de UT_BindingProxy, conformément au
        /// contrat abstrait de la classe de base Freezable.
        /// </summary>
        /// <returns>
        /// Une nouvelle instance de UT_BindingProxy, retournée typée Freezable au sens
        /// du contrat de la classe de base.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Contexte : Méthode abstraite de System.Windows.Freezable dont l'implémentation
        /// est obligatoire dans toute classe concrète dérivant directement ou
        /// indirectement de Freezable. Elle constitue le point d'extension par lequel
        /// le mécanisme de clonage interne de WPF construit une nouvelle instance du
        /// type concret lorsque l'objet Freezable est dupliqué - par exemple par CloneCore
        /// ou par les opérations internes du système de ressources WPF.
        /// </para>
        /// <para>
        /// Objectif : Retourner une instance fraîchement construite de UT_BindingProxy,
        /// servant de base à toute opération de duplication requise par WPF.
        /// L'invocation reste interne au framework et n'est jamais déclenchée
        /// explicitement par le code applicatif consommateur.
        /// </para>
        /// <para>
        /// Remarques sur le contrat Freezable : L'absence de cette surcharge interdit
        /// la compilation de toute classe concrète dérivant de Freezable, la méthode
        /// étant déclarée abstract sur la classe de base. Sa présence est par conséquent
        /// une condition structurelle de l'existence même de UT_BindingProxy en tant
        /// que Freezable consommable par WPF. Le retour direct d'une nouvelle instance,
        /// sans aucune logique additionnelle, est l'implémentation idiomatique et
        /// suffisante pour un Freezable sans état mutable propre : aucune copie de
        /// champ n'est nécessaire, la DependencyProperty Data étant prise en charge
        /// par le mécanisme générique de duplication des DependencyObject.
        /// </para>
        /// </remarks>
        protected override Freezable CreateInstanceCore()
        {
            return new UT_BindingProxy();
        }

        #endregion

        #region === Méthodes privées ===

        // Aucune méthode privée : la classe ne porte aucune logique interne à factoriser, l'implémentation se limitant aux accesseurs canoniques de la DependencyProperty et à la surcharge du contrat Freezable.

        #endregion
    }
}