using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.ViewModels.Components.DialogWindow;

namespace DG244Cutting.D_Presentation.Views.Components.DialogWindow
{
    /// <summary>
    /// Composant visuel Window singulier de fenêtre de dialogue applicatif
    /// modal non bloquant de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant hors familles canoniques (UC, SR, CR, CH, QH,
    /// SE, RS, DTO, VM-Page, MH, Page), instancié en portée Transient par le
    /// conteneur d'injection de dépendances à chaque résolution. L'asymétrie
    /// Transient (présente View) / Singleton (<see cref="VM_DialogWindow"/>)
    /// est volontaire vis-à-vis du couple étalon <c>Banner</c> / <c>VM_Banner</c>
    /// (tous deux Singleton) — elle est justifiée par la nature
    /// <see cref="Window"/> WPF éphémère, instanciée à chaque appel à
    /// <c>OpenDialog</c> par <c>SR_Notification</c> (au Fil 2 ultérieur). Son
    /// <see cref="FrameworkElement.DataContext"/> est
    /// <see cref="VM_DialogWindow"/>, ViewModel Singleton injecté au
    /// constructeur du présent code-behind. Étalon documentaire désigné par le
    /// présent fil : <c>Banner</c>, lui-même calqué sur <c>MainWindow</c>.</para>
    /// <para>Objectif : Constituer la fenêtre WPF de dialogue applicatif modal
    /// non bloquant, recevoir ses trois dépendances par injection, affecter
    /// <see cref="VM_DialogWindow"/> à son
    /// <see cref="FrameworkElement.DataContext"/> pour activer les bindings WPF
    /// de <c>DialogWindow.xaml</c>, appliquer au
    /// <see cref="FrameworkElement.Loaded"/> la maximisation et la stylisation
    /// via <see cref="IS_ControlStyler.StyleWindow"/>, et supprimer le bouton
    /// Fermer (X) du menu système au
    /// <see cref="Window.OnSourceInitialized(EventArgs)"/> via P/Invoke
    /// <c>user32.dll</c> (alignement fidèle legacy).</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Servir de composant visuel — la composition XAML est portée par
    ///   <c>DialogWindow.xaml</c> et se limite à un <see cref="System.Windows.Controls.Grid"/>
    ///   racine porteur d'un <see cref="System.Windows.Controls.Border"/> de
    ///   fond, d'un <see cref="System.Windows.Controls.Border"/> de titre
    ///   englobant un <see cref="System.Windows.Controls.Image"/> (logo) et un
    ///   <see cref="System.Windows.Controls.TextBlock"/> (titre), et d'un
    ///   <see cref="System.Windows.Controls.TextBlock"/> de contenu central.</item>
    ///   <item>Recevoir ses trois dépendances par injection au constructeur et
    ///   les stocker en champs <c>private readonly</c>, avec gardes
    ///   <see cref="ArgumentNullException"/>.</item>
    ///   <item>Affecter <see cref="FrameworkElement.DataContext"/> à
    ///   <see cref="VM_DialogWindow"/> pour alimenter les bindings WPF des
    ///   deux propriétés observables (<see cref="VM_DialogWindow.DW_Title"/>,
    ///   <see cref="VM_DialogWindow.DW_Content"/>) et de la propriété immuable
    ///   d'URI d'icône (<see cref="VM_DialogWindow.LogoIconUri"/>).</item>
    ///   <item>S'abonner à l'unique événement WPF du composant
    ///   (<see cref="FrameworkElement.Loaded"/>) et garantir qu'aucune
    ///   exception ne remonte au framework WPF par un filet de sécurité ultime
    ///   au bord du handler.</item>
    ///   <item>Appliquer au <see cref="FrameworkElement.Loaded"/> la
    ///   maximisation de la fenêtre (<c>WindowState = WindowState.Maximized</c>,
    ///   alignement fidèle legacy) et la stylisation des six contrôles XAML via
    ///   la signature unique <see cref="IS_ControlStyler.StyleWindow"/>.</item>
    ///   <item>Supprimer le bouton Fermer (X) du menu système au
    ///   <see cref="Window.OnSourceInitialized(EventArgs)"/> via P/Invoke
    ///   <c>user32.dll</c> (<c>GetSystemMenu</c> et <c>DeleteMenu</c> avec la
    ///   commande <c>SC_CLOSE</c>) — alignement fidèle legacy.</item>
    ///   <item>Porter le <see cref="CancellationTokenSource"/> local au
    ///   composant, conformément à la doctrine de l'annulation coopérative
    ///   §4.6.5 du 0230 (chaque point d'entrée porte son propre CTS).
    ///   Posture parallèle à <c>Banner</c>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Porter de logique applicative propre — le composant est un
    ///   relais visuel pur entre <see cref="VM_DialogWindow"/> et le rendu
    ///   WPF.</item>
    ///   <item>Consommer directement un Setting (sauf observation indirecte
    ///   via <see cref="VM_DialogWindow"/>), un Repository ou un UseCase
    ///   métier — toute consommation transverse passe par
    ///   <see cref="VM_DialogWindow"/>.</item>
    ///   <item>Piloter l'ouverture / fermeture de la fenêtre — la séquence
    ///   <c>OpenDialog</c> sur <see cref="DG244Cutting.A_Domain.Interfaces.Settings.Presentation.ISE_Window"/>
    ///   puis <see cref="Window.Show"/> est portée par <c>SR_Notification</c>
    ///   (au Fil 2).</item>
    ///   <item>Désabonner explicitement le handler
    ///   <see cref="FrameworkElement.Loaded"/> — le cycle de vie Transient est
    ///   borné par <see cref="Window.Show"/> et <see cref="Window.Close"/> ;
    ///   la libération est portée par le GC à la collecte de l'instance après
    ///   fermeture.</item>
    ///   <item>Libérer explicitement le <see cref="CancellationTokenSource"/>
    ///   — voir la note dédiée ci-dessous.</item>
    ///   <item>Classifier ou traiter les exceptions applicatives typées
    ///   (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>).
    ///   Le traitement terminal est délégué à <see cref="IU_LogAndNotify"/>.</item>
    /// </list>
    /// <para>Note sur la libération du <see cref="CancellationTokenSource"/>
    /// sous statut Transient : Posture distincte de <c>Banner</c> (Singleton,
    /// CTS non libéré, vie alignée sur le processus). La présente
    /// <see cref="Window"/> étant Transient, son cycle de vie est aligné sur la
    /// durée d'une ouverture de dialog (de <c>SR_Notification.OpenDialog</c> à
    /// <see cref="Window.Close"/>). Aucun consommateur de <c>_cts.Token</c>
    /// n'est porté par le présent code-behind (le filet de sécurité ultime
    /// utilise <see cref="CancellationToken.None"/>) ; la non-libération
    /// explicite est sans conséquence fonctionnelle. Le champ
    /// <see cref="CancellationTokenSource"/> est maintenu par parallélisme
    /// strict avec <c>Banner._cts</c> et reste disponible pour toute extension
    /// ultérieure portée par un fil dédié.</para>
    /// <para>Note sur le statut hors familles canoniques :
    /// <see cref="DialogWindow"/> n'appartient à aucune des familles canoniques
    /// de l'écosystème et ne dispose donc d'aucun 0232 d'autorité documentaire.
    /// Les conventions documentaires et structurelles appliquées sont calquées
    /// nominativement sur celles de <c>Banner</c> (étalon canonique désigné par
    /// le fil, lui-même calqué sur <c>MainWindow</c>), modulo l'asymétrie
    /// <see cref="Window"/> / <see cref="System.Windows.Controls.UserControl"/>,
    /// l'ajout du bloc P/Invoke de suppression du bouton Fermer (X) au
    /// <see cref="Window.OnSourceInitialized(EventArgs)"/>, l'ajout de la
    /// maximisation au <see cref="FrameworkElement.Loaded"/>, et la suppression
    /// des éléments propres à <c>Banner</c> non applicables (commandes,
    /// polling, abonnement à un événement applicatif système). Le présent
    /// composant servira ultérieurement, conjointement avec
    /// <see cref="VM_DialogWindow"/>, d'exemple canonique pour la rédaction
    /// de la section 5.11 du 0230 et du 0232-VI-VM à venir.</para>
    /// <para>Exception architecturale propre — Filet de sécurité ultime hors
    /// UseCase (EA-NN-FilSecuriteDialogWindow). <see cref="DialogWindow"/>
    /// consomme directement <see cref="IU_LogAndNotify"/> au titre d'un filet
    /// de sécurité de dernier recours en frontière de WPF, au bord de l'unique
    /// handler <see cref="OnLoadedHandler"/>. Cette consommation est
    /// habituellement réservée aux UseCases (R-4.7.14) ; elle est admise
    /// nominativement pour <see cref="DialogWindow"/> par parallèle exact avec
    /// <c>EA-NN-FilSecuriteBanniere</c> déjà documentée pour <c>Banner</c>,
    /// elle-même calquée sur <c>EA-NN-FilSecuriteShell</c> de <c>MainWindow</c>
    /// au titre de §4.7.4 amendée. Périmètre strict : la présente exception se
    /// matérialise à un seul site au sein du présent composant —
    /// <see cref="OnLoadedHandler"/>. La clé dictionnaire utilisée est
    /// <c>"No_EC_03"</c> (exception non classifiée localement) ; le paramètre
    /// <c>ct</c> passé à <see cref="IU_LogAndNotify.ExecuteAsync"/> est
    /// systématiquement <see cref="CancellationToken.None"/>, posture
    /// parallèle à <c>Banner.OnLoadedHandler</c> ; <c>notify: false</c> — la
    /// défaillance de stylisation à l'amorçage ne justifie pas une
    /// notification opérateur supplémentaire (alignement strict
    /// <c>Banner.OnLoadedHandler</c>). La numérotation <c>NN</c> sera
    /// attribuée au moment de l'intégration de la présente exception dans le
    /// 0230 / 0231.</para>
    /// <para>Aucune Exception architecturale propre <c>async void</c> n'est
    /// portée par le présent composant : <see cref="OnLoadedHandler"/> est
    /// strictement synchrone, n'invoquant que des opérations synchrones
    /// (<see cref="WindowState"/> et <see cref="IS_ControlStyler.StyleWindow"/>).
    /// La justification <c>EA-NN-AsyncVoidOnLoadedHandler</c> documentée pour
    /// <c>MainWindow</c> n'est pas applicable ici — alignement strict
    /// <c>Banner</c>.</para>
    /// </remarks>
    public partial class DialogWindow : Window
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom dynamique de la classe courante, utilisé comme premier maillon
        /// de la <c>callChain</c> construite par le handler du composant
        /// (§4.5 du 0230).
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Source de jeton d'annulation coopérative locale au composant,
        /// conformément à la doctrine §4.6.5 du 0230 (chaque point d'entrée
        /// porte son propre CTS).
        /// </summary>
        /// <remarks>
        /// <para>Non libéré explicitement — voir la note dédiée dans la
        /// documentation de classe : posture distincte de <c>Banner</c>
        /// (Singleton, CTS non libéré, vie alignée sur le processus) ; le
        /// présent composant Transient voit son CTS implicitement libéré par
        /// le GC à la collecte de l'instance après
        /// <see cref="Window.Close"/>. Aucun consommateur de <c>_cts.Token</c>
        /// n'est porté par le présent code-behind (le filet de sécurité ultime
        /// utilise <see cref="CancellationToken.None"/>) ; le champ est
        /// maintenu par parallélisme strict avec <c>Banner._cts</c> et reste
        /// disponible pour toute extension ultérieure.</para>
        /// </remarks>
        private readonly CancellationTokenSource _cts;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// ViewModel singulier de la fenêtre de dialogue, affecté à
        /// <see cref="FrameworkElement.DataContext"/> pour alimenter
        /// l'ensemble des bindings WPF de <c>DialogWindow.xaml</c>
        /// (<see cref="VM_DialogWindow.DW_Title"/>,
        /// <see cref="VM_DialogWindow.DW_Content"/>,
        /// <see cref="VM_DialogWindow.LogoIconUri"/>).
        /// </summary>
        private readonly VM_DialogWindow _vmDialogWindow;

        /// <summary>
        /// Service technique de présentation appliquant la stylisation des
        /// contrôles WPF. Consommé au <see cref="FrameworkElement.Loaded"/>
        /// du composant pour styliser les six contrôles de la fenêtre de
        /// dialogue via la signature unique
        /// <see cref="IS_ControlStyler.StyleWindow"/>.
        /// </summary>
        private readonly IS_ControlStyler _controlStyler;

        /// <summary>
        /// Pipeline terminal de gestion d'erreurs applicatives. Consommé au
        /// bord du handler <see cref="OnLoadedHandler"/> au titre du filet de
        /// sécurité ultime — voir « Exception architecturale propre —
        /// EA-NN-FilSecuriteDialogWindow » dans la documentation de classe.
        /// </summary>
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Suppression du bouton Fermer (X) ===

        /// <summary>
        /// P/Invoke <c>user32.dll</c> — récupère un handle vers le menu système
        /// de la fenêtre identifiée par <paramref name="hWnd"/>.
        /// </summary>
        /// <remarks>
        /// <para>Reproduction stricte du bloc P/Invoke de la
        /// <c>Shared.Views.Components.DialogWindow</c> legacy. Utilisée au
        /// <see cref="OnSourceInitialized(EventArgs)"/> pour obtenir le menu
        /// système préalablement à la suppression de la commande
        /// <c>SC_CLOSE</c>.</para>
        /// </remarks>
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// P/Invoke <c>user32.dll</c> — supprime une entrée du menu système
        /// identifiée par <paramref name="uPosition"/> et
        /// <paramref name="uFlags"/>.
        /// </summary>
        /// <remarks>
        /// <para>Reproduction stricte du bloc P/Invoke de la
        /// <c>Shared.Views.Components.DialogWindow</c> legacy. Utilisée au
        /// <see cref="OnSourceInitialized(EventArgs)"/> pour supprimer la
        /// commande <c>SC_CLOSE</c> (bouton Fermer (X)) du menu système.</para>
        /// </remarks>
        [DllImport("user32.dll")]
        private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        /// <summary>
        /// Flag <c>MF_BYCOMMAND</c> — indique à <see cref="DeleteMenu"/> que la
        /// position passée est un identifiant de commande et non un index de
        /// position.
        /// </summary>
        private const uint MF_BYCOMMAND = 0x00000000;

        /// <summary>
        /// Identifiant de la commande système <c>SC_CLOSE</c> du menu système
        /// — correspond au bouton Fermer (X).
        /// </summary>
        private const uint SC_CLOSE = 0xF060;

        /// <summary>
        /// Surcharge de <see cref="Window.OnSourceInitialized(EventArgs)"/> —
        /// supprime le bouton Fermer (X) du menu système de la fenêtre via
        /// P/Invoke <c>user32.dll</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF au moment où
        /// le handle natif de la fenêtre est créé, préalablement au premier
        /// rendu visuel. Reproduction stricte du
        /// <c>OnSourceInitialized</c> de la
        /// <c>Shared.Views.Components.DialogWindow</c> legacy.</para>
        /// <para>Objectif : Récupérer le handle natif de la fenêtre via
        /// <see cref="WindowInteropHelper"/>, obtenir le menu système via
        /// <see cref="GetSystemMenu"/>, puis supprimer la commande
        /// <c>SC_CLOSE</c> via <see cref="DeleteMenu"/> — empêchant
        /// l'utilisateur de fermer la fenêtre via le bouton Fermer (X), la
        /// fermeture étant exclusivement contrôlée par <c>SR_Notification</c>
        /// (au Fil 2 ultérieur).</para>
        /// </remarks>
        /// <param name="e">Arguments de l'événement
        /// <see cref="Window.OnSourceInitialized(EventArgs)"/>.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Récupère le handle natif de la fenêtre.
            IntPtr handle = new WindowInteropHelper(this).Handle;

            // Supprime le bouton Fermer (X) du menu système.
            IntPtr hMenu = GetSystemMenu(handle, false);
            DeleteMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du composant visuel de fenêtre
        /// de dialogue <see cref="DialogWindow"/> avec ses trois dépendances
        /// injectées par le conteneur DI.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur d'injection
        /// de dépendances lors de la résolution Transient. L'instance est
        /// ensuite consommée par <c>SR_Notification</c> (au Fil 2 ultérieur)
        /// qui invoque <see cref="Window.Show"/> postérieurement à
        /// l'invocation de <c>OpenDialog</c> sur
        /// <see cref="DG244Cutting.A_Domain.Interfaces.Settings.Presentation.ISE_Window"/>.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item>Résolution et stockage des trois dépendances avec gardes
        ///   <see cref="ArgumentNullException"/>, dans l'ordre fonctionnel
        ///   ViewModel → Service de stylisation → UseCase pipeline terminal
        ///   d'erreurs.</item>
        ///   <item>Initialisation des champs locaux : <c>_callee</c> à partir
        ///   du nom de type (cohérence avec la doctrine CallChain du projet,
        ///   §4.5), <c>_cts</c> par instanciation d'un nouveau
        ///   <see cref="CancellationTokenSource"/> local au composant
        ///   (§4.6.5).</item>
        ///   <item>Invocation de <see cref="System.Windows.Markup.IComponentConnector.InitializeComponent"/>
        ///   pour la composition XAML — étape impérativement préalable à toute
        ///   affectation de <see cref="FrameworkElement.DataContext"/> ou
        ///   abonnement à un événement WPF du composant.</item>
        ///   <item>Affectation de <see cref="FrameworkElement.DataContext"/> à
        ///   <c>_vmDialogWindow</c> pour activer l'ensemble des bindings WPF
        ///   déclarés par <c>DialogWindow.xaml</c>.</item>
        ///   <item>Abonnement à l'unique événement WPF du composant
        ///   (<see cref="FrameworkElement.Loaded"/>).</item>
        /// </list>
        /// <para>Filet de sécurité : Aucune invocation susceptible de lever
        /// une exception terminale n'est portée par le constructeur ; les
        /// gardes <see cref="ArgumentNullException"/> agissent en amont de
        /// toute initialisation. Aucune intervention de
        /// <see cref="IU_LogAndNotify"/> n'est donc requise dans le périmètre
        /// du constructeur — le filet de sécurité ultime
        /// <c>EA-NN-FilSecuriteDialogWindow</c> se matérialise exclusivement
        /// au bord du handler <see cref="OnLoadedHandler"/>.</para>
        /// </remarks>
        /// <param name="vmDialogWindow">ViewModel singulier de la fenêtre de
        /// dialogue, affecté à <see cref="FrameworkElement.DataContext"/>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="controlStyler">Service technique de stylisation des
        /// contrôles WPF — consommé au <see cref="FrameworkElement.Loaded"/>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="logAndNotify">Pipeline terminal de gestion d'erreurs —
        /// filet de sécurité ultime du handler <see cref="OnLoadedHandler"/>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si l'une des
        /// dépendances injectées est <see langword="null"/>.</exception>
        public DialogWindow(
            VM_DialogWindow vmDialogWindow,
            IS_ControlStyler controlStyler,
            IU_LogAndNotify logAndNotify)
        {
            _vmDialogWindow = vmDialogWindow ?? throw new ArgumentNullException(nameof(vmDialogWindow));
            _controlStyler = controlStyler ?? throw new ArgumentNullException(nameof(controlStyler));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));

            _callee = GetType().Name;
            _cts = new CancellationTokenSource();

            InitializeComponent();

            DataContext = _vmDialogWindow;

            Loaded += OnLoadedHandler;
        }

        #endregion

        #region === Méthodes publiques ===

        // Aucune méthode publique exposée par DialogWindow hors handlers framework.

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler synchrone invoqué par le runtime WPF en réponse à
        /// l'événement <see cref="FrameworkElement.Loaded"/> du composant —
        /// maximise la fenêtre et applique la stylisation des six contrôles
        /// XAML via <see cref="IS_ControlStyler.StyleWindow"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode invoquée par le runtime WPF lors du
        /// premier rendu visuel du composant, postérieurement à
        /// <see cref="Window.Show"/> par <c>SR_Notification</c>. Unique
        /// handler de cycle de vie WPF abonné par le présent composant
        /// (l'<see cref="Window.OnSourceInitialized(EventArgs)"/> est une
        /// surcharge, pas un abonnement).</para>
        /// <para>Objectif : Construire la <c>callChain</c> locale, puis
        /// maximiser la fenêtre (<c>WindowState = WindowState.Maximized</c>,
        /// alignement fidèle legacy) et invoquer
        /// <see cref="IS_ControlStyler.StyleWindow"/> avec les six contrôles
        /// XAML correspondants. Les <c>x:Name</c> sont strictement alignés sur
        /// les paramètres formels du service ; le second paramètre
        /// <c>Border? background2</c> est passé à <see langword="null"/>
        /// (alignement strict signature legacy : un seul Border de fond,
        /// <c>Background_1</c>).</para>
        /// <para>Filet de sécurité ultime : Le bloc <c>try/catch</c>
        /// englobant garantit qu'aucune exception ne remonte au framework
        /// WPF — voir « Exception architecturale propre —
        /// EA-NN-FilSecuriteDialogWindow » dans la documentation de classe.
        /// La délégation à <see cref="IU_LogAndNotify"/> est faite en
        /// fire-and-forget (handler synchrone, le retour de
        /// <see cref="IU_LogAndNotify.ExecuteAsync"/> est volontairement
        /// ignoré via la décharge <c>_</c>). <c>notify: false</c> — la
        /// défaillance de stylisation ou de maximisation à l'amorçage ne
        /// justifie pas une notification opérateur : l'utilisateur reste
        /// capable de lire le contenu de la fenêtre non stylisée. Aucun bloc
        /// <c>catch (OperationCanceledException)</c> distinct : les opérations
        /// invoquées sont synchrones et ne supportent pas l'annulation
        /// coopérative ; une éventuelle
        /// <see cref="OperationCanceledException"/> sera donc journalisée
        /// comme une défaillance non classifiée. Posture canonique parallèle
        /// à <c>Banner.OnLoadedHandler</c>.</para>
        /// <para>CancellationToken du filet de sécurité : Le paramètre
        /// <c>ct</c> passé à <see cref="IU_LogAndNotify.ExecuteAsync"/> est
        /// <see cref="CancellationToken.None"/> et non <c>_cts.Token</c> — le
        /// <c>_cts.Token</c> peut être en état imprévisible vis-à-vis du
        /// moment de défaillance, et <see cref="CancellationToken.None"/>
        /// garantit que la journalisation ne sera pas interrompue par un
        /// signal d'annulation concomitant. Posture parallèle à
        /// <c>Banner.OnLoadedHandler</c>.</para>
        /// </remarks>
        /// <param name="sender">Source de l'événement (l'instance courante de
        /// <see cref="DialogWindow"/>).</param>
        /// <param name="e">Arguments de l'événement
        /// <see cref="FrameworkElement.Loaded"/> — non consommés par le
        /// handler.</param>
        private void OnLoadedHandler(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLoadedHandler)}";

            try
            {
                WindowState = WindowState.Maximized;
                _controlStyler.StyleWindow(Background_1, null, TitleBorder, LogoImage, TitleContent, MainContent);
            }
            catch (Exception ex)
            {
                _ = _logAndNotify.ExecuteAsync(
                    callChain,
                    "No_EC_03",
                    ex,
                    notify: false,
                    ct: CancellationToken.None);
            }
        }

        #endregion
    }
}