using System.Windows.Input;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page de sélection de la langue <c>Page02</c> de
    /// l'application DG244Cutting, exposant à la vue les libellés
    /// multilingues des six langues sélectionnables (endonymes), les
    /// six URIs de drapeau correspondantes, les six booléens de
    /// sélection mutuellement exclusive, et la commande asynchrone de
    /// changement de langue invoquant le UseCase <see cref="IU_Language_Apply"/>
    /// via <see cref="IS_UseCaseInvoker"/> au titre d'EA-11.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Composant de la famille VM_Page de la couche
    /// <c>D_Presentation</c>, ViewModel concret de la page
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page02"/>.
    /// La page est accessible à tout utilisateur connecté et présente
    /// six langues au choix sous forme de boutons illustrés (drapeau du
    /// pays + nom endonyme de la langue + RadioButton mutuellement
    /// exclusif via <c>GroupName="LanguageSelection"</c>). Le clic sur
    /// un bouton applique immédiatement le changement de langue à
    /// toute l'application via le UseCase
    /// <see cref="IU_Language_Apply"/> sans navigation forcée — la
    /// page Page02 reste affichée après application, l'utilisateur
    /// sort exclusivement via les boutons transverses du menu
    /// horizontal porté par le couple <c>VM_MH02</c> / <c>MH02</c>.
    /// La transposition fonctionnelle du couple legacy
    /// <c>VM_Page91</c> / <c>Page91</c> du projet BatchStockRelease
    /// substitue les codes langue ISO 639-1 (FR, EN, DE, ...) par des
    /// codes pays ISO 3166-1 alpha-2 (FR, GB, DE, ES, IT, PT) pour
    /// alignement strict sur la convention sémantique structurante de
    /// <see cref="IU_Language_Apply"/> et de la résolution des
    /// drapeaux par <see cref="ISE_Flag.GetFlagUriOrDefault"/>.</para>
    ///
    /// <para>Objectif : Exposer à la vue
    /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page02"/> :</para>
    /// <list type="bullet">
    ///   <item><description>7 propriétés observables
    ///   <c>Label_P02_0n</c> liées aux clés homonymes du dictionnaire
    ///   actif : <see cref="Label_P02_00"/> pour le titre principal de
    ///   la page (« Liste des langues disponibles : »), et
    ///   <see cref="Label_P02_01"/> à <see cref="Label_P02_06"/> pour
    ///   les six noms endonymes de langues (Français, English,
    ///   Deutsch, Español, Italiano, Português dans l'ordre des codes
    ///   pays FR, GB, DE, ES, IT, PT). Toutes ces propriétés sont
    ///   alimentées par la mécanique multilingue factorisée par
    ///   <see cref="VM_Generic"/> : premier chargement au constructeur
    ///   via <see cref="VM_Generic.InitializeLabels"/>, rechargement
    ///   automatique à tout changement de langue dynamique par le
    ///   handler interne d'abonnement INPC à
    ///   <see cref="ISE_App.AppCultureCode"/> de l'ancêtre commun,
    ///   conformément à §4.11.5 du 0230 et à R-4.11.9 du 0231.</description></item>
    ///   <item><description>6 propriétés observables booléennes
    ///   <see cref="IsLanguage1Selected"/> à
    ///   <see cref="IsLanguage6Selected"/>, bindées sur la propriété
    ///   <c>IsChecked</c> des six <c>RadioButton</c> imbriqués dans
    ///   les six boutons de langue, en sélection mutuellement
    ///   exclusive par <c>GroupName="LanguageSelection"</c> côté XAML.
    ///   L'écriture est exposée en accès public pour autoriser le
    ///   helper privé <see cref="ApplyLanguageSelection"/> à
    ///   réinitialiser puis affecter la sélection courante. Les six
    ///   booléens sont alimentés en deux occasions : (1) à l'entrée
    ///   sur la page par <see cref="LoadAsync"/> qui dérive le
    ///   bouton à cocher de la composante pays de
    ///   <see cref="ISE_App.AppCultureCode"/> via les helpers privés
    ///   <see cref="ExtractCountryCodeFromCulture"/> et
    ///   <see cref="ApplyLanguageSelection"/> ; (2) à chaque
    ///   changement de langue déclenché par l'utilisateur par le
    ///   handler privé <see cref="ChangeLanguageAsync"/> qui
    ///   ré-invoque <see cref="ApplyLanguageSelection"/> de manière
    ///   inconditionnelle après invocation du UseCase, par
    ///   transposition littérale du comportement legacy.</description></item>
    ///   <item><description>6 propriétés non-observables
    ///   <c>Uri</c> en auto-property à setter <c>private</c>
    ///   <see cref="Flag1Source"/> à <see cref="Flag6Source"/>,
    ///   alimentées une fois au constructeur par invocation de
    ///   <see cref="ISE_Flag.GetFlagUriOrDefault"/> sur les six codes
    ///   pays « FR », « GB », « DE », « ES », « IT », « PT » dans
    ///   l'ordre des six boutons de langue. Le repli sur
    ///   <see cref="ISE_Flag.DefaultFlagUri"/> en cas de code pays
    ///   absent du référentiel est porté par la sémantique propre de
    ///   <see cref="ISE_Flag.GetFlagUriOrDefault"/> (« or Default »
    ///   dans la signature) : VM_Page02 n'a pas à reproduire
    ///   localement le repli, qui est délégué au Setting de
    ///   présentation. Aucune notification INPC n'est requise sur ces
    ///   propriétés : leur valeur est figée pour toute la durée de
    ///   vie du ViewModel Singleton, par opposition à
    ///   <see cref="ISE_Flag.AppFlagUri"/> qui matérialise quant à
    ///   elle le drapeau de la langue *active* (cf. bandeau de
    ///   l'application) et qui est dynamiquement mise à jour par
    ///   <see cref="IU_Language_Apply"/>.</description></item>
    ///   <item><description>1 commande <see cref="ChangeLanguageCommand"/>
    ///   de type <see cref="ICommand"/>, instanciée au constructeur
    ///   en <see cref="UT_RelayCommandArg1Async{T}"/> de
    ///   <see cref="string"/>, liée au handler privé asynchrone
    ///   <see cref="ChangeLanguageAsync"/>. Le
    ///   <c>CommandParameter</c> transmis par WPF est un code pays
    ///   ISO 3166-1 alpha-2 (« FR », « GB », « DE », « ES », « IT »,
    ///   « PT ») bindé en dur dans le XAML sur chacun des six boutons
    ///   de langue.</description></item>
    /// </list>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exposer les 7 propriétés observables
    ///   <c>Label_P02_0n</c> en accès public en lecture, écriture
    ///   privée via le helper hérité <c>SetProperty&lt;T&gt;</c>.</description></item>
    ///   <item><description>Exposer les 6 propriétés observables
    ///   <c>IsLanguage{n}Selected</c> en accès public en lecture
    ///   et écriture via le helper hérité <c>SetProperty&lt;T&gt;</c>.
    ///   L'écriture publique est nécessaire pour autoriser le helper
    ///   privé <see cref="ApplyLanguageSelection"/> à muter ces
    ///   propriétés depuis l'intérieur du dérivé, et reste sans
    ///   risque opérationnel : le binding WPF côté
    ///   <c>RadioButton.IsChecked</c> est en mode par défaut
    ///   <c>OneWay</c> ne propage pas la sélection visuelle vers le
    ///   ViewModel (la commande
    ///   <see cref="ChangeLanguageCommand"/> est le canal unique de
    ///   sélection effective).</description></item>
    ///   <item><description>Exposer les 6 propriétés
    ///   <c>Flag{n}Source</c> en auto-property
    ///   <c>{ get; private set; }</c>, alimentées une fois au
    ///   constructeur.</description></item>
    ///   <item><description>Exposer la commande asynchrone
    ///   <see cref="ChangeLanguageCommand"/> liée au handler privé
    ///   <see cref="ChangeLanguageAsync"/>.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Generic.LoadLabels"/> pour résoudre les 7 clés
    ///   <c>P02_00</c> à <c>P02_06</c> via
    ///   <see cref="VM_Generic._dictionary"/> hérité et affecter les
    ///   valeurs résolues aux 7 propriétés <c>Label_P02_0n</c>,
    ///   conformément à R-4.11.8 du 0231.</description></item>
    ///   <item><description>Redéfinir
    ///   <see cref="VM_Page_Generic.LoadAsync"/> pour initialiser
    ///   l'état de sélection des 6 booléens
    ///   <c>IsLanguage{n}Selected</c> à partir de la composante pays
    ///   de <see cref="ISE_App.AppCultureCode"/> au montage de la
    ///   page, en encapsulation par le filet hérité
    ///   <see cref="VM_Generic.ExecuteSafeAsync"/> (§4.7.3 du 0230).
    ///   Le hook est invoqué depuis le code-behind de <c>Page02</c>
    ///   au point d'extension <c>OnLoadedAsync</c> exposé par
    ///   <c>Page_Generic</c>.</description></item>
    ///   <item><description>Porter le handler privé asynchrone
    ///   <see cref="ChangeLanguageAsync"/> de la commande
    ///   <see cref="ChangeLanguageCommand"/> : conversion du code
    ///   pays reçu en code culture, invocation de
    ///   <see cref="IU_Language_Apply"/> via
    ///   <see cref="IS_UseCaseInvoker"/> (EA-11), puis mise à jour
    ///   inconditionnelle de l'état de sélection des 6 booléens via
    ///   le helper <see cref="ApplyLanguageSelection"/>, en
    ///   encapsulation par le filet hérité
    ///   <see cref="VM_Generic.ExecuteSafeAsync"/>.</description></item>
    ///   <item><description>Déléguer à <see cref="VM_Generic"/> la
    ///   cérémonie multilingue complète (premier chargement,
    ///   abonnement INPC filtré sur
    ///   <see cref="ISE_App.AppCultureCode"/>, marshalling Dispatcher
    ///   défensif, rechargement) par l'unique appel à
    ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
    ///   instruction du constructeur, conformément à I-4.11.11 et
    ///   R-4.11.8 du 0231.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune logique métier ni règle
    ///   décisionnelle. Le ViewModel orchestre l'invocation d'un
    ///   UseCase dédié pour le changement de langue, sans porter de
    ///   logique propre au-delà de la conversion code pays → code
    ///   culture (helper privé tabulaire) et de la mise à jour de la
    ///   sélection visuelle (helper privé inconditionnel).</description></item>
    ///   <item><description>Aucune décision de navigation : la règle
    ///   R-4.12.2 du 0231 réserve la décision de navigation aux
    ///   UseCases. <see cref="VM_Page02"/> n'injecte ni
    ///   <c>IU_Navigation</c> ni <c>IS_Navigation</c>. Le legacy
    ///   <c>VM_Page91</c> redirigeait vers <c>Page10</c> à la fin du
    ///   handler <c>SetLanguageCode</c> ; cette redirection est
    ///   supprimée nominalement dans la transposition DG244Cutting :
    ///   la page Page02 reste affichée après changement de langue,
    ///   l'utilisateur sort exclusivement via les boutons transverses
    ///   du menu horizontal <c>MH02</c>.</description></item>
    ///   <item><description>Aucun rafraîchissement explicite du titre
    ///   de l'application après changement de langue. Le legacy
    ///   <c>VM_Page91</c> rappelait
    ///   <c>_settingsApp.SetApplicationTitle(...)</c> puis recopiait
    ///   le titre dans <c>MainWindow.CommonBackgroundPad.Title</c>.
    ///   Dans DG244Cutting, ce rafraîchissement est automatique par
    ///   cascade INPC : <see cref="IU_Language_Apply"/> mute
    ///   <see cref="ISE_App.AppCultureCode"/>, ce qui déclenche le
    ///   handler interne d'abonnement INPC de <see cref="VM_Generic"/>
    ///   sur tous les VMs actifs (y compris celui du bandeau qui
    ///   expose <c>Label_App_Ti_00</c>), avec re-invocation
    ///   automatique de <see cref="VM_Generic.LoadLabels"/>. Aucun
    ///   appel explicite à un mécanisme de rafraîchissement n'est
    ///   requis depuis <see cref="VM_Page02"/>.</description></item>
    ///   <item><description>Aucune consommation directe d'un contrat
    ///   <c>IU_</c> ou <c>IQ_</c>, conformément à I-4.10.10 du 0231 :
    ///   le UseCase <see cref="IU_Language_Apply"/> est invoqué
    ///   exclusivement via <see cref="IS_UseCaseInvoker"/> au titre
    ///   d'EA-11.</description></item>
    ///   <item><description>Aucun désabonnement explicite ni aucune
    ///   cérémonie multilingue locale au-delà de la lecture ponctuelle
    ///   et nominale d'<see cref="ISE_App.AppCultureCode"/> dans
    ///   <see cref="LoadAsync"/> au titre d'EA-NN-VMPageLectureCultureCode
    ///   (cf. ci-après). L'abonnement INPC à
    ///   <see cref="ISE_App"/> est branché par
    ///   <see cref="VM_Generic.InitializeLabels"/> et porté par le
    ///   handler interne de l'ancêtre commun, conformément à
    ///   I-4.11.11 du 0231 ; aucun désabonnement n'est requis du
    ///   dérivé. La P4-bis (§4.10.10 du 0230) garantit par ailleurs
    ///   la libération naturelle de l'abonnement à l'arrêt de
    ///   l'application.</description></item>
    /// </list>
    ///
    /// <para>Note sur les exceptions architecturales : Le présent
    /// dérivé porte une exception architecturale propre, nominative
    /// et arbitrée en ouverture du fil
    /// <c>Page-VM_02_Creation</c>.</para>
    ///
    /// <para><b>EA-NN-VMPageLectureCultureCode</b>
    /// (« VM_Page lectrice d'<see cref="ISE_App.AppCultureCode"/> »).
    /// Le présent ViewModel injecte <see cref="ISE_App"/> en
    /// dépendance propre du dérivé (champ
    /// <see cref="_app"/> en <c>private readonly</c>) en sus de la
    /// transmission obligatoire à <see cref="VM_Generic"/> via
    /// <c>base(...)</c>, pour autoriser la lecture ponctuelle de
    /// <see cref="ISE_App.AppCultureCode"/> au sein de
    /// <see cref="LoadAsync"/>. Cette dérogation à la lettre stricte
    /// d'I-4.11.11 du 0231 (« aucun champ propre de stockage
    /// d'ISE_App ») est justifiée par la sémantique unique de la
    /// page de sélection de langue, qui doit lire l'état courant de
    /// la langue active à chaque montage de la page pour initialiser
    /// la sélection visuelle des six RadioButton sur le bouton
    /// correspondant à la langue active. La dérogation est
    /// strictement bornée à cette lecture ponctuelle : VM_Page02 ne
    /// porte aucun handler propre d'abonnement à
    /// <c>PropertyChanged</c> d'<see cref="ISE_App"/>, aucune
    /// propriété observable miroir d'<see cref="ISE_App.AppCultureCode"/>,
    /// aucun marshalling Dispatcher local, et n'accède jamais à
    /// <see cref="_app"/> en dehors de l'invocation de
    /// <see cref="LoadAsync"/>. La conformité aux trois griefs
    /// opérationnels nominaux d'I-4.11.11 (abonnement, propriétés
    /// miroir, marshalling) est préservée : la cérémonie multilingue
    /// reste intégralement portée par <see cref="VM_Generic"/>.
    /// La numérotation formelle <c>EA-NN</c> est à attribuer au
    /// 0231 §17.4 lors d'un fil de maintenance documentaire
    /// ultérieur, signalé en clôture du présent fil sous la rubrique
    /// « Remarques sur l'écosystème documentaire ». Arbitrage
    /// Q-A=[Q-A.1] du fil <c>Page-VM_02_Creation</c>.</para>
    ///
    /// <para>L'injection de <see cref="IU_LogAndNotify"/> par le
    /// ViewModel relève de l'exception architecturale propre du socle
    /// <see cref="VM_Generic"/> (EA-01, §4.15.5 du 0230), héritée et
    /// non re-déclarée à ce niveau ; elle est mobilisée par
    /// <see cref="LoadAsync"/> et par
    /// <see cref="ChangeLanguageAsync"/> qui encapsulent leur
    /// invocation par le filet hérité
    /// <see cref="VM_Generic.ExecuteSafeAsync"/>. L'injection de
    /// <see cref="ISE_Flag"/> au constructeur est nominale au titre
    /// du Setting de présentation portant le référentiel des
    /// drapeaux, sans portée d'EA propre — la consommation se fait
    /// exclusivement au constructeur via
    /// <see cref="ISE_Flag.GetFlagUriOrDefault"/> et ne déclenche
    /// aucune cérémonie INPC. L'injection de
    /// <see cref="IS_UseCaseInvoker"/> est nominale au titre du mode
    /// d'invocation depuis <c>D_Presentation</c> posé en §4.10.10
    /// du 0230 : les ViewModels invoquent les contrats <c>IU_</c>
    /// via <see cref="IS_UseCaseInvoker"/> qui matérialise un
    /// <c>IServiceScope</c> distinct à chaque invocation. EA-11
    /// (« Composant créateur de scope DI par invocation ») est
    /// portée exclusivement par <c>SR_UseCaseInvoker</c> ; le présent
    /// ViewModel en est consommateur et non porteur.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par deux extensions (R-4.4.10 du
    /// 0231) : l'extension <c>=== Propriétés publiques ===</c> au
    /// titre des 14 propriétés observables et des 6 propriétés
    /// auto-property <c>Flag{n}Source</c> et de la commande
    /// <see cref="ChangeLanguageCommand"/> exposées, et l'extension
    /// <c>=== Méthodes protégées ===</c> au titre de l'override
    /// <see cref="LoadLabels"/>. Soit sept régions au total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   13 champs supports des propriétés observables scalaires
    ///   (7 champs supports de libellés <c>_label_p02_0n</c> et
    ///   6 champs supports booléens <c>_isLanguage{n}Selected</c>).
    ///   Les 6 propriétés <c>Flag{n}Source</c> sont déclarées en
    ///   auto-property et n'ont pas de champ support nommé.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   3 champs <c>private readonly</c> stockant les dépendances
    ///   propres au dérivé, affectés au constructeur : <c>_app</c>
    ///   (<see cref="ISE_App"/>, au titre d'EA-NN-VMPageLectureCultureCode),
    ///   <c>_flag</c> (<see cref="ISE_Flag"/>) et
    ///   <c>_useCaseInvoker</c> (<see cref="IS_UseCaseInvoker"/>).</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c>
    ///   (extension §4.4.3) : 7 propriétés observables
    ///   <c>Label_P02_0n</c> en setter privé via
    ///   <c>SetProperty&lt;T&gt;</c>, 6 propriétés observables
    ///   <c>IsLanguage{n}Selected</c> en setter public via
    ///   <c>SetProperty&lt;T&gt;</c>, 6 propriétés
    ///   <c>Flag{n}Source</c> en auto-property à setter privé,
    ///   1 commande <see cref="ChangeLanguageCommand"/> en lecture
    ///   seule avec instanciation en place au constructeur.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : constructeur
    ///   <c>public</c> à cinq paramètres, délégation à
    ///   <see cref="VM_Page_Generic"/> via
    ///   <c>base(dictionary, logAndNotify, app)</c>, affectation du
    ///   champ <see cref="_app"/> au titre d'EA-NN-VMPageLectureCultureCode
    ///   (sans garde locale redondante, la garde sur <c>app</c>
    ///   étant portée par la chaîne <c>base(...)</c> conformément à
    ///   l'item VM-P5 de §4.2 du 0232-Page-VM), gardes
    ///   <see cref="ArgumentNullException"/> locales sur les deux
    ///   dépendances <c>flag</c> et <c>useCaseInvoker</c>,
    ///   alimentation des 6 propriétés <c>Flag{n}Source</c> via
    ///   <see cref="ISE_Flag.GetFlagUriOrDefault"/> sur les 6 codes
    ///   pays ISO 3166-1 alpha-2, instanciation de
    ///   <see cref="ChangeLanguageCommand"/>, invocation finale de
    ///   <see cref="VM_Generic.InitializeLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   override <see cref="LoadAsync"/> selon patron normatif
    ///   §4.15.6 du 0230 à trois constituants
    ///   (<see cref="VM_Generic.BuildFirstCallChain"/> interne,
    ///   <see cref="VM_Generic.ExecuteSafeAsync"/>, propagation du
    ///   <see cref="System.Threading.CancellationToken"/>).</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c>
    ///   (extension §4.4.3) : override <see cref="LoadLabels"/>
    ///   peuplant les 7 propriétés <c>Label_P02_0n</c> via
    ///   <see cref="VM_Generic._dictionary"/>, une affectation par
    ///   ligne dans l'ordre numérique croissant des clés
    ///   (<c>P02_00</c>, <c>P02_01</c>, …, <c>P02_06</c>), sans
    ///   appel à <c>base.LoadLabels(callChain)</c>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   4 helpers privés —
    ///   <see cref="ExtractCountryCodeFromCulture"/> (extraction de
    ///   la composante pays depuis un code culture),
    ///   <see cref="GetCultureCodeFromCountryCode"/> (conversion
    ///   tabulaire code pays → code culture),
    ///   <see cref="ApplyLanguageSelection"/> (mise à jour des
    ///   6 booléens de sélection), et le handler asynchrone de la
    ///   commande <see cref="ChangeLanguageAsync"/>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente : <see cref="VM_Page02"/> n'expose aucun
    /// événement propre, l'événement <c>PropertyChanged</c> étant
    /// porté par <see cref="VM_Generic"/> au titre d'INPC et hérité
    /// par transitivité.</para>
    /// </remarks>
    public class VM_Page02 : VM_Page_Generic
    {
        #region === Propriétés privées ===

        /// <summary>Champ support de <see cref="Label_P02_00"/> (clé <c>P02_00</c>, titre principal de la page).</summary>
        private string _label_p02_00 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P02_01"/> (clé <c>P02_01</c>, endonyme « Français »).</summary>
        private string _label_p02_01 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P02_02"/> (clé <c>P02_02</c>, endonyme « English »).</summary>
        private string _label_p02_02 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P02_03"/> (clé <c>P02_03</c>, endonyme « Deutsch »).</summary>
        private string _label_p02_03 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P02_04"/> (clé <c>P02_04</c>, endonyme « Español »).</summary>
        private string _label_p02_04 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P02_05"/> (clé <c>P02_05</c>, endonyme « Italiano »).</summary>
        private string _label_p02_05 = string.Empty;

        /// <summary>Champ support de <see cref="Label_P02_06"/> (clé <c>P02_06</c>, endonyme « Português »).</summary>
        private string _label_p02_06 = string.Empty;

        /// <summary>Champ support de <see cref="IsLanguage1Selected"/> (langue française, code pays « FR »).</summary>
        private bool _isLanguage1Selected;

        /// <summary>Champ support de <see cref="IsLanguage2Selected"/> (langue anglaise britannique, code pays « GB »).</summary>
        private bool _isLanguage2Selected;

        /// <summary>Champ support de <see cref="IsLanguage3Selected"/> (langue allemande, code pays « DE »).</summary>
        private bool _isLanguage3Selected;

        /// <summary>Champ support de <see cref="IsLanguage4Selected"/> (langue espagnole, code pays « ES »).</summary>
        private bool _isLanguage4Selected;

        /// <summary>Champ support de <see cref="IsLanguage5Selected"/> (langue italienne, code pays « IT »).</summary>
        private bool _isLanguage5Selected;

        /// <summary>Champ support de <see cref="IsLanguage6Selected"/> (langue portugaise, code pays « PT »).</summary>
        private bool _isLanguage6Selected;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Setting Singleton de l'état applicatif global, source des
        /// notifications INPC sur <see cref="ISE_App.AppCultureCode"/>.
        /// Stocké en champ propre du présent dérivé au titre de
        /// l'exception architecturale propre
        /// <c>EA-NN-VMPageLectureCultureCode</c> documentée dans le
        /// <c>&lt;remarks&gt;</c> de classe.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI
        /// au constructeur et transmis à <see cref="VM_Generic"/>
        /// via la chaîne <c>base(...)</c> au titre de la mécanique
        /// multilingue factorisée par l'ancêtre commun (abonnement
        /// INPC interne à <see cref="ISE_App.AppCultureCode"/>),
        /// puis stocké en champ propre du présent dérivé en
        /// dérogation arbitrée à la lettre stricte d'I-4.11.11 du
        /// 0231 (« aucun champ propre de stockage d'ISE_App »).</para>
        ///
        /// <para>Usage strict : La consommation par le présent
        /// ViewModel se fait exclusivement dans
        /// <see cref="LoadAsync"/> par un appel unique à
        /// <see cref="ISE_App.AppCultureCode"/> pour l'extraction de
        /// la composante pays consommée par
        /// <see cref="ApplyLanguageSelection"/>. Aucun autre accès
        /// à <see cref="_app"/> n'est porté par le présent dérivé :
        /// pas de handler propre d'abonnement à
        /// <c>PropertyChanged</c>, pas de propriété observable
        /// miroir, pas de marshalling Dispatcher local.</para>
        /// </remarks>
        private readonly ISE_App _app;

        /// <summary>
        /// Setting Singleton de présentation portant le référentiel
        /// des drapeaux disponibles indexé par code pays
        /// ISO 3166-1 alpha-2 et la résolution centralisée d'un
        /// drapeau à partir d'un code pays avec repli sur le drapeau
        /// par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI
        /// au constructeur. <see cref="ISE_Flag"/> est un Setting de
        /// présentation portant l'état du drapeau de l'application
        /// (<see cref="ISE_Flag.AppFlagUri"/>, dynamiquement mis à
        /// jour par <see cref="IU_Language_Apply"/>) et le
        /// référentiel des drapeaux disponibles
        /// (<see cref="ISE_Flag.ReferenceFlags"/>). La consommation
        /// par le présent ViewModel se fait exclusivement au
        /// constructeur via 6 appels à
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/> sur les
        /// 6 codes pays attendus, pour alimentation des 6 propriétés
        /// auto-property <see cref="Flag1Source"/> à
        /// <see cref="Flag6Source"/>. Aucune propagation INPC n'est
        /// branchée localement : les 6 URIs sont figées pour toute
        /// la durée de vie du ViewModel Singleton et reflètent le
        /// référentiel statique des drapeaux, distinct de la
        /// propriété observable
        /// <see cref="ISE_Flag.AppFlagUri"/> dynamique consommée par
        /// le bandeau de l'application.</para>
        /// </remarks>
        private readonly ISE_Flag _flag;

        /// <summary>
        /// Composant Singleton porteur de l'exception architecturale
        /// EA-11 (« Composant créateur de scope DI par invocation »,
        /// §4.10.10 et §4.15.10 du 0230, §17.4 du 0231), unique voie
        /// d'invocation des UseCases (<c>IU_</c>) depuis un composant
        /// de <c>D_Presentation</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Injecté en Singleton par le conteneur DI
        /// au constructeur, conformément au mode d'invocation depuis
        /// <c>D_Presentation</c> posé en §4.10.10 du 0230. À chaque
        /// invocation, <see cref="IS_UseCaseInvoker"/> matérialise
        /// un <c>IServiceScope</c> distinct, y résout le composant
        /// cible et l'exécute via le délégué fourni, puis dispose le
        /// scope. Le présent ViewModel est consommateur de
        /// <see cref="IS_UseCaseInvoker"/> et non porteur d'EA-11 :
        /// EA-11 est portée exclusivement par
        /// <c>SR_UseCaseInvoker</c>.</para>
        /// <para>Mode d'invocation strict : Le passage par
        /// <see cref="IS_UseCaseInvoker"/> est imposé par la lecture
        /// stricte du §4.10.10 du 0230 qui pose l'interdiction
        /// structurelle de l'injection directe d'un contrat
        /// <c>IU_</c> dans un composant de <c>D_Presentation</c>,
        /// indépendamment de toute question de captive dependency.
        /// Conformité I-4.10.10 du 0231. Le UseCase
        /// <see cref="IU_Language_Apply"/> est invoqué dans
        /// <see cref="ChangeLanguageAsync"/> via cette voie unique,
        /// par la surcharge à retour signalable
        /// <c>InvokeAsync&lt;IU_Language_Apply, bool&gt;</c> exigée
        /// par la signature
        /// <c>Task&lt;bool&gt; ExecuteAsync(...)</c> du contrat
        /// (R-4.14.21).</para>
        /// </remarks>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>Libellé multilingue associé à la clé <c>P02_00</c>, titre principal de la page (« Liste des langues disponibles : »).</summary>
        public string Label_P02_00
        {
            get => _label_p02_00;
            private set => SetProperty(ref _label_p02_00, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P02_01</c>, endonyme de la langue française (« Français »).</summary>
        public string Label_P02_01
        {
            get => _label_p02_01;
            private set => SetProperty(ref _label_p02_01, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P02_02</c>, endonyme de la langue anglaise britannique (« English »).</summary>
        public string Label_P02_02
        {
            get => _label_p02_02;
            private set => SetProperty(ref _label_p02_02, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P02_03</c>, endonyme de la langue allemande (« Deutsch »).</summary>
        public string Label_P02_03
        {
            get => _label_p02_03;
            private set => SetProperty(ref _label_p02_03, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P02_04</c>, endonyme de la langue espagnole (« Español »).</summary>
        public string Label_P02_04
        {
            get => _label_p02_04;
            private set => SetProperty(ref _label_p02_04, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P02_05</c>, endonyme de la langue italienne (« Italiano »).</summary>
        public string Label_P02_05
        {
            get => _label_p02_05;
            private set => SetProperty(ref _label_p02_05, value);
        }

        /// <summary>Libellé multilingue associé à la clé <c>P02_06</c>, endonyme de la langue portugaise (« Português »).</summary>
        public string Label_P02_06
        {
            get => _label_p02_06;
            private set => SetProperty(ref _label_p02_06, value);
        }

        /// <summary>
        /// Indique si la langue française (code pays « FR ») est la
        /// langue actuellement sélectionnée. Propriété bindée sur
        /// <c>RadioButton.IsChecked</c> du premier bouton de langue
        /// côté XAML, en sélection mutuellement exclusive par
        /// <c>GroupName="LanguageSelection"</c>.
        /// </summary>
        public bool IsLanguage1Selected
        {
            get => _isLanguage1Selected;
            set => SetProperty(ref _isLanguage1Selected, value);
        }

        /// <summary>
        /// Indique si la langue anglaise britannique (code pays « GB »)
        /// est la langue actuellement sélectionnée. Propriété bindée
        /// sur <c>RadioButton.IsChecked</c> du deuxième bouton de
        /// langue côté XAML.
        /// </summary>
        public bool IsLanguage2Selected
        {
            get => _isLanguage2Selected;
            set => SetProperty(ref _isLanguage2Selected, value);
        }

        /// <summary>
        /// Indique si la langue allemande (code pays « DE ») est la
        /// langue actuellement sélectionnée. Propriété bindée sur
        /// <c>RadioButton.IsChecked</c> du troisième bouton de langue
        /// côté XAML.
        /// </summary>
        public bool IsLanguage3Selected
        {
            get => _isLanguage3Selected;
            set => SetProperty(ref _isLanguage3Selected, value);
        }

        /// <summary>
        /// Indique si la langue espagnole (code pays « ES ») est la
        /// langue actuellement sélectionnée. Propriété bindée sur
        /// <c>RadioButton.IsChecked</c> du quatrième bouton de langue
        /// côté XAML.
        /// </summary>
        public bool IsLanguage4Selected
        {
            get => _isLanguage4Selected;
            set => SetProperty(ref _isLanguage4Selected, value);
        }

        /// <summary>
        /// Indique si la langue italienne (code pays « IT ») est la
        /// langue actuellement sélectionnée. Propriété bindée sur
        /// <c>RadioButton.IsChecked</c> du cinquième bouton de langue
        /// côté XAML.
        /// </summary>
        public bool IsLanguage5Selected
        {
            get => _isLanguage5Selected;
            set => SetProperty(ref _isLanguage5Selected, value);
        }

        /// <summary>
        /// Indique si la langue portugaise (code pays « PT ») est la
        /// langue actuellement sélectionnée. Propriété bindée sur
        /// <c>RadioButton.IsChecked</c> du sixième bouton de langue
        /// côté XAML.
        /// </summary>
        public bool IsLanguage6Selected
        {
            get => _isLanguage6Selected;
            set => SetProperty(ref _isLanguage6Selected, value);
        }

        /// <summary>
        /// URI du drapeau associé au code pays « FR » (drapeau de la
        /// langue française), résolu une fois au constructeur via
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/>. Propriété bindée
        /// sur <c>Image.Source</c> du premier bouton de langue côté
        /// XAML.
        /// </summary>
        public Uri Flag1Source { get; private set; }

        /// <summary>
        /// URI du drapeau associé au code pays « GB » (drapeau de la
        /// langue anglaise britannique), résolu une fois au
        /// constructeur via <see cref="ISE_Flag.GetFlagUriOrDefault"/>.
        /// Propriété bindée sur <c>Image.Source</c> du deuxième
        /// bouton de langue côté XAML.
        /// </summary>
        public Uri Flag2Source { get; private set; }

        /// <summary>
        /// URI du drapeau associé au code pays « DE » (drapeau de la
        /// langue allemande), résolu une fois au constructeur via
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/>. Propriété
        /// bindée sur <c>Image.Source</c> du troisième bouton de
        /// langue côté XAML.
        /// </summary>
        public Uri Flag3Source { get; private set; }

        /// <summary>
        /// URI du drapeau associé au code pays « ES » (drapeau de la
        /// langue espagnole), résolu une fois au constructeur via
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/>. Propriété
        /// bindée sur <c>Image.Source</c> du quatrième bouton de
        /// langue côté XAML.
        /// </summary>
        public Uri Flag4Source { get; private set; }

        /// <summary>
        /// URI du drapeau associé au code pays « IT » (drapeau de la
        /// langue italienne), résolu une fois au constructeur via
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/>. Propriété
        /// bindée sur <c>Image.Source</c> du cinquième bouton de
        /// langue côté XAML.
        /// </summary>
        public Uri Flag5Source { get; private set; }

        /// <summary>
        /// URI du drapeau associé au code pays « PT » (drapeau de la
        /// langue portugaise), résolu une fois au constructeur via
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/>. Propriété
        /// bindée sur <c>Image.Source</c> du sixième bouton de langue
        /// côté XAML.
        /// </summary>
        public Uri Flag6Source { get; private set; }

        /// <summary>
        /// Commande asynchrone exposée à la vue pour le changement de
        /// langue de l'application, liée au handler privé
        /// <see cref="ChangeLanguageAsync"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Instanciée au constructeur en
        /// <see cref="UT_RelayCommandArg1Async{T}"/> de
        /// <see cref="string"/>, bindée sur la propriété
        /// <c>Command</c> des six boutons de langue côté XAML, avec
        /// un <c>CommandParameter</c> en dur correspondant au code
        /// pays ISO 3166-1 alpha-2 du bouton concerné (« FR », « GB »,
        /// « DE », « ES », « IT », « PT »).</para>
        /// <para>Comportement : Le clic sur un bouton déclenche
        /// l'invocation asynchrone du handler privé
        /// <see cref="ChangeLanguageAsync"/> avec le code pays
        /// transmis en argument, qui orchestre la conversion en code
        /// culture, l'invocation du UseCase
        /// <see cref="IU_Language_Apply"/> via
        /// <see cref="IS_UseCaseInvoker"/>, et la mise à jour
        /// inconditionnelle de l'état de sélection des six booléens
        /// <c>IsLanguage{n}Selected</c>.</para>
        /// </remarks>
        public ICommand ChangeLanguageCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page02"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué par le conteneur DI
        /// lors de la résolution du Singleton <see cref="VM_Page02"/>
        /// par la vue <c>Page02</c> via
        /// <c>App.ServiceProvider.GetRequiredService</c> dans son
        /// propre constructeur (EA-02 Service Locator de
        /// <see cref="DG244Cutting.D_Presentation.Views.Generic.Page_Generic"/>,
        /// étendue aux dérivés directs pour la résolution de leur
        /// ViewModel — cf. §4.15.7 du 0230).</para>
        ///
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Délégation à
        ///   <see cref="VM_Page_Generic"/> via
        ///   <c>base(dictionary, logAndNotify, app)</c> en première
        ///   instruction. La chaîne <c>base(...)</c> remonte à
        ///   <see cref="VM_Generic"/> qui applique les trois gardes
        ///   <see cref="ArgumentNullException"/> sur les trois
        ///   paramètres, stocke <paramref name="dictionary"/> et
        ///   <paramref name="logAndNotify"/> en champs
        ///   <c>protected</c>
        ///   (<see cref="VM_Generic._dictionary"/>,
        ///   <see cref="VM_Generic._logAndNotify"/>) accessibles aux
        ///   dérivés, stocke <paramref name="app"/> en champ
        ///   <c>private</c> non hérité (encapsulation de la mécanique
        ///   multilingue, conformément à I-4.11.11 du 0231), et
        ///   initialise le champ <c>_callee</c> via
        ///   <c>GetType().Name</c>.</description></item>
        ///   <item><description>Affectation du champ propre
        ///   <see cref="_app"/> par référence partagée avec la
        ///   dépendance déjà transmise à <see cref="VM_Generic"/> via
        ///   la chaîne <c>base(...)</c>, au titre de l'exception
        ///   architecturale propre <c>EA-NN-VMPageLectureCultureCode</c>
        ///   documentée dans le <c>&lt;remarks&gt;</c> de classe.
        ///   Aucune garde locale redondante n'est ajoutée sur
        ///   <paramref name="app"/> : la garde est déjà portée par
        ///   <see cref="VM_Generic"/> via la chaîne <c>base(...)</c>,
        ///   et si <paramref name="app"/> était <see langword="null"/>,
        ///   l'exception aurait été levée avant l'entrée dans le corps
        ///   du présent constructeur. Conformité à l'item VM-P5 de
        ///   §4.2 du 0232-Page-VM.</description></item>
        ///   <item><description>Gardes
        ///   <see cref="ArgumentNullException"/> locales sur les deux
        ///   dépendances propres au dérivé non transmises à
        ///   <see cref="VM_Generic"/> (<paramref name="flag"/>,
        ///   <paramref name="useCaseInvoker"/>) et affectation aux
        ///   champs <see cref="_flag"/> et
        ///   <see cref="_useCaseInvoker"/>.</description></item>
        ///   <item><description>Alimentation des 6 propriétés
        ///   auto-property <see cref="Flag1Source"/> à
        ///   <see cref="Flag6Source"/> par 6 appels successifs à
        ///   <see cref="ISE_Flag.GetFlagUriOrDefault"/> sur les
        ///   6 codes pays ISO 3166-1 alpha-2 « FR », « GB », « DE »,
        ///   « ES », « IT », « PT » dans l'ordre des 6 boutons de
        ///   langue. Le repli sur
        ///   <see cref="ISE_Flag.DefaultFlagUri"/> en cas de code pays
        ///   absent du référentiel est porté par la sémantique propre
        ///   de la méthode (« or Default » dans la signature) et
        ///   n'est pas reproduit localement.</description></item>
        ///   <item><description>Instanciation de
        ///   <see cref="ChangeLanguageCommand"/> en
        ///   <see cref="UT_RelayCommandArg1Async{T}"/> de
        ///   <see cref="string"/>, liée au handler privé
        ///   <see cref="ChangeLanguageAsync"/>. L'instanciation
        ///   intervient après l'affectation des dépendances pour que
        ///   le handler puisse y accéder ; elle reste un préalable à
        ///   <see cref="VM_Generic.InitializeLabels"/> qui est
        ///   l'ultime instruction du constructeur.</description></item>
        ///   <item><description>Appel à
        ///   <see cref="VM_Generic.InitializeLabels"/> en dernière
        ///   instruction du corps. Ce hook explicite orchestre la
        ///   séquence normative en trois temps : construction d'une
        ///   CallChain initiale via
        ///   <see cref="VM_Generic.BuildFirstCallChain"/>, premier
        ///   appel synchrone à l'override <see cref="LoadLabels"/>
        ///   peuplant les 7 propriétés <c>Label_P02_0n</c> avant le
        ///   premier binding WPF de la vue, et branchement de
        ///   l'abonnement INPC interne à <see cref="ISE_App"/> pour
        ///   la prise en compte du changement de langue dynamique
        ///   (R-4.11.8 et R-4.11.9 du 0231).</description></item>
        /// </list>
        ///
        /// <para>Règle d'invocation d'<c>InitializeLabels</c>
        /// (R-4.11.8 du 0231) : L'appel à
        /// <see cref="VM_Generic.InitializeLabels"/> est exclusivement
        /// effectué dans le constructeur du ViewModel dérivé concret
        /// final, en dernière instruction, après l'affectation de
        /// toutes les dépendances propres. Cette règle prévient
        /// l'écueil classique de l'invocation virtuelle dans le
        /// constructeur d'une classe de base avec dépendances dérivées
        /// non encore initialisées.</para>
        ///
        /// <para>Filet de sécurité : Aucune invocation susceptible de
        /// lever une exception terminale n'est portée par le
        /// constructeur au-delà des gardes
        /// <see cref="ArgumentNullException"/> et des 6 appels à
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/>. La sémantique
        /// propre de
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/> garantit qu'un
        /// code pays vide, blanc ou non trouvé entraîne le retour de
        /// <see cref="ISE_Flag.DefaultFlagUri"/> sans propagation
        /// d'exception ; les 6 codes pays passés au constructeur sont
        /// statiquement connus et conformes au référentiel. Toute
        /// défaillance de l'invocation virtuelle de
        /// <see cref="LoadLabels"/> par
        /// <see cref="VM_Generic.InitializeLabels"/> est absorbée par
        /// le filet hérité du socle multilingue ; le constructeur est
        /// donc inconditionnellement terminant en l'absence de
        /// paramètres null.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c>. Injecté en Singleton par le conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs, transmis à
        /// <see cref="VM_Page_Generic"/> via <c>base(...)</c>.
        /// Mobilisé par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/> au sein de
        /// <see cref="LoadAsync"/> et de
        /// <see cref="ChangeLanguageAsync"/>. Injecté en Singleton
        /// par le conteneur DI au titre de l'EA-01.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, transmis à <see cref="VM_Page_Generic"/> via
        /// <c>base(...)</c> pour l'alimentation de la mécanique
        /// multilingue factorisée par l'ancêtre commun (abonnement
        /// INPC interne à <see cref="ISE_App.AppCultureCode"/>), ET
        /// stocké en champ propre <see cref="_app"/> du présent
        /// dérivé pour la lecture ponctuelle de
        /// <see cref="ISE_App.AppCultureCode"/> dans
        /// <see cref="LoadAsync"/>, au titre de l'exception
        /// architecturale propre <c>EA-NN-VMPageLectureCultureCode</c>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="flag">Setting Singleton de présentation
        /// portant le référentiel des drapeaux disponibles indexé par
        /// code pays ISO 3166-1 alpha-2. Consommé au constructeur via
        /// 6 appels à
        /// <see cref="ISE_Flag.GetFlagUriOrDefault"/> pour
        /// alimentation des 6 propriétés
        /// <see cref="Flag1Source"/> à <see cref="Flag6Source"/>.
        /// Injecté en Singleton par le conteneur DI.</param>
        /// <param name="useCaseInvoker">Composant Singleton porteur
        /// d'EA-11, unique voie d'invocation du UseCase
        /// <see cref="IU_Language_Apply"/> depuis le présent
        /// ViewModel. Mobilisé par
        /// <see cref="ChangeLanguageAsync"/>. Injecté en Singleton
        /// par le conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="flag"/> ou
        /// <paramref name="useCaseInvoker"/> est
        /// <see langword="null"/>. Les gardes sur
        /// <paramref name="dictionary"/>,
        /// <paramref name="logAndNotify"/> et <paramref name="app"/>
        /// sont portées par <see cref="VM_Generic"/> via la chaîne
        /// <c>base(...)</c>.</exception>
        public VM_Page02(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            ISE_Flag flag,
            IS_UseCaseInvoker useCaseInvoker)
            : base(dictionary, logAndNotify, app)
        {
            _app = app;
            _flag = flag
                ?? throw new ArgumentNullException(nameof(flag));
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));

            Flag1Source = _flag.GetFlagUriOrDefault("FR");
            Flag2Source = _flag.GetFlagUriOrDefault("GB");
            Flag3Source = _flag.GetFlagUriOrDefault("DE");
            Flag4Source = _flag.GetFlagUriOrDefault("ES");
            Flag5Source = _flag.GetFlagUriOrDefault("IT");
            Flag6Source = _flag.GetFlagUriOrDefault("PT");

            ChangeLanguageCommand =
                new UT_RelayCommandArg1Async<string>(ChangeLanguageAsync);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Redéfinit le hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> pour initialiser
        /// l'état de sélection des 6 booléens
        /// <c>IsLanguage{n}Selected</c> à partir de la composante
        /// pays de <see cref="ISE_App.AppCultureCode"/> au montage
        /// de la page.
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// l'orchestrateur appelant côté <c>Page_Generic</c> au
        /// format normatif
        /// <c>{_callee} &gt; OnLoadedHandler &gt; OnLoadedAsync</c>
        /// et propagée tel quel par le code-behind via
        /// <c>_viewModel.LoadAsync(callChain, ct)</c>. Le paramètre
        /// est reçu par contrat du hook au socle
        /// <see cref="VM_Page_Generic"/> mais n'est pas consommé par
        /// le corps du présent override : une CallChain interne
        /// distincte est construite via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> et consommée
        /// par <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément
        /// au patron de surcharge normatif §4.15.6 du 0230.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé
        /// par le code-behind appelant. Propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>. Valeur par
        /// défaut : <see langword="default"/>.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de
        /// l'initialisation de l'état de sélection.</returns>
        /// <remarks>
        /// <para>Contexte : Override du hook canonique
        /// <see cref="VM_Page_Generic.LoadAsync"/> déclaré
        /// <c>public virtual</c> au socle conformément à §4.15.6 du
        /// 0230. Invoquée depuis le code-behind de <c>Page02</c> au
        /// point d'extension <c>OnLoadedAsync</c> exposé par
        /// <c>Page_Generic</c> (§4.15.7 du 0230). Méthode strictement
        /// disjointe de <see cref="LoadLabels"/> : libellés
        /// synchrones au constructeur d'un côté, état de sélection
        /// asynchrone au <c>Loaded</c> de la page de l'autre.</para>
        ///
        /// <para>Objectif : Lire la composante pays de
        /// <see cref="ISE_App.AppCultureCode"/> via le helper privé
        /// <see cref="ExtractCountryCodeFromCulture"/>, puis mettre à
        /// jour l'état de sélection des 6 booléens
        /// <c>IsLanguage{n}Selected</c> via le helper privé
        /// <see cref="ApplyLanguageSelection"/>, qui réinitialise les
        /// 6 booléens à <see langword="false"/> puis affecte
        /// <see langword="true"/> au booléen correspondant au code
        /// pays extrait. Pour un code pays inconnu (cas marginal
        /// structurellement non atteignable par construction de
        /// l'application au démarrage qui force <c>en-GB</c> à
        /// défaut), aucun booléen n'est sélectionné — comportement
        /// défensif minimal.</para>
        ///
        /// <para>Lecture d'<see cref="ISE_App.AppCultureCode"/> :
        /// L'accès se fait via le champ propre <see cref="_app"/>
        /// stocké au constructeur au titre de l'exception
        /// architecturale propre
        /// <c>EA-NN-VMPageLectureCultureCode</c> (cf.
        /// <c>&lt;remarks&gt;</c> de classe). La lecture est
        /// ponctuelle et nominale : un unique accès à la propriété
        /// scalaire <c>AppCultureCode</c> au sein du présent override,
        /// sans abonnement INPC propre, sans propriété observable
        /// miroir, sans marshalling Dispatcher.</para>
        ///
        /// <para>Patron de surcharge normatif (§4.15.6 du 0230) :
        /// L'override construit une CallChain interne
        /// (<c>innerCallChain</c>) via
        /// <see cref="VM_Generic.BuildFirstCallChain"/> hérité, plutôt
        /// que de consommer la CallChain reçue en paramètre. Le
        /// paramètre <paramref name="callChain"/> reçu du hook est
        /// utile à des fins de traçabilité amont mais la CallChain
        /// consommée par le filet est celle reconstruite localement,
        /// garantissant que le format normatif
        /// <c>{_callee} &gt; LoadAsync</c> est appliqué pour
        /// l'opération elle-même.</para>
        ///
        /// <para>Idempotence : La méthode est ré-appelable à chaque
        /// entrée sur la page sans flag de mémoire d'état. Chaque
        /// appel produit une nouvelle lecture de
        /// <see cref="ISE_App.AppCultureCode"/> et une nouvelle
        /// alimentation des 6 booléens via
        /// <see cref="ApplyLanguageSelection"/> — coût négligeable.</para>
        ///
        /// <para>Filet de sécurité : Le corps est encapsulé par le
        /// filet hérité <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// (§4.7.3 du 0230). Aucune exception applicative typée n'est
        /// attendue dans le chemin nominal (lecture d'une propriété
        /// scalaire de Setting et mutations de propriétés observables
        /// du présent ViewModel), mais le filet préserve la robustesse
        /// transverse de la famille selon le pipeline canonique à
        /// quatre captures (No_EC_01 / No_EC_02 / No_EC_03 + propagation
        /// de <see cref="OperationCanceledException"/>).</para>
        ///
        /// <para>Propagation du <see cref="System.Threading.CancellationToken"/> :
        /// Le jeton fourni par l'appelant est propagé à
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément à
        /// §4.6 du 0230, bien que le corps soit purement synchrone et
        /// ne consomme pas effectivement le jeton (encapsulation par
        /// <see cref="Task.CompletedTask"/> en sortie du délégué).</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée silencieusement à l'appelant sur signal
        /// d'annulation coopérative par le filet hérité
        /// <see cref="VM_Generic.ExecuteSafeAsync"/>, conformément à
        /// §4.7.3 du 0230. Aucune journalisation ni notification.
        /// </exception>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, () =>
            {
                string countryCode =
                    ExtractCountryCodeFromCulture(_app.AppCultureCode);
                ApplyLanguageSelection(countryCode);
                return Task.CompletedTask;
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger les
        /// 7 libellés multilingues affichés par la page <c>Page02</c>
        /// à partir des clés <c>P02_00</c> à <c>P02_06</c> du
        /// dictionnaire de langue actif et les affecter aux
        /// 7 propriétés observables <see cref="Label_P02_00"/> à
        /// <see cref="Label_P02_06"/>.
        /// </summary>
        /// <param name="callChain">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> au
        /// changement de langue dynamique (rechargement), et transmise
        /// au service de dictionnaire pour traçabilité.</param>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> conformément à
        /// R-4.11.8 du 0231. Invoquée par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// pour le premier chargement, puis par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> à chaque
        /// changement de langue dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>, avec marshalling
        /// Dispatcher défensif vers le thread UI.</para>
        ///
        /// <para>Objectif : Garantir que les 7 propriétés
        /// <c>Label_P02_0n</c> sont synchronisées avec la langue
        /// active du dictionnaire, tant au moment de l'instanciation
        /// du ViewModel que lors de tout changement ultérieur de
        /// langue dynamique au cours de la session.</para>
        ///
        /// <para>Patron strict : Une affectation par ligne, dans
        /// l'ordre numérique croissant des clés (<c>P02_00</c>,
        /// <c>P02_01</c>, …, <c>P02_06</c>), sans regroupement et
        /// sans condition. Aucun raccourci de type boucle dynamique :
        /// la résolution nominative permet une revue de code aisée et
        /// un repérage statique des clés consommées. Les clés
        /// <c>P02_07</c> à <c>P02_19</c> présentes dans le dictionnaire
        /// fr-FR (alimentées « Non défini » par convention de symétrie
        /// transverse avec les dictionnaires non-FR) ne sont
        /// volontairement pas chargées par le présent override : leur
        /// présence est conservée à titre de matière première pour
        /// d'éventuelles évolutions ultérieures.</para>
        ///
        /// <para>Absence d'appel à <c>base.LoadLabels(callChain)</c> :
        /// L'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun
        /// traitement. L'appel à <c>base.LoadLabels(callChain)</c>
        /// n'apporterait qu'un bruit inutile et est délibérément
        /// omis, conformément à la pratique standard d'override
        /// lorsque la base ne porte aucun traitement, alignée sur le
        /// patron de <c>VM_Page01.LoadLabels</c>.</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local n'est
        /// posé. Le filet est porté exclusivement par
        /// <c>SR_Dictionary</c> conformément à R-4.11.6 et R-4.11.10
        /// du 0231 — toute anomalie (clé absente, erreur inattendue)
        /// est journalisée en interne par <c>SR_Dictionary</c> et
        /// résolue par une valeur de repli <c>[P02_NN] not found</c>,
        /// sans interruption ni propagation d'exception au présent
        /// ViewModel.</para>
        /// </remarks>
        protected override void LoadLabels(string callChain)
        {
            Label_P02_00 = _dictionary.GetText(callChain, "P02_00");
            Label_P02_01 = _dictionary.GetText(callChain, "P02_01");
            Label_P02_02 = _dictionary.GetText(callChain, "P02_02");
            Label_P02_03 = _dictionary.GetText(callChain, "P02_03");
            Label_P02_04 = _dictionary.GetText(callChain, "P02_04");
            Label_P02_05 = _dictionary.GetText(callChain, "P02_05");
            Label_P02_06 = _dictionary.GetText(callChain, "P02_06");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Extrait la composante pays ISO 3166-1 alpha-2 d'un code
        /// culture au format RFC 4646 (par exemple
        /// <c>"fr-FR"</c> → <c>"FR"</c>, <c>"en-GB"</c> → <c>"GB"</c>).
        /// </summary>
        /// <param name="cultureCode">Code culture au format RFC 4646,
        /// typiquement <c>{langue}-{pays}</c>.</param>
        /// <returns>Code pays ISO 3166-1 alpha-2 en majuscules. Si
        /// <paramref name="cultureCode"/> ne comporte pas de séparateur
        /// <c>-</c> ou si la composante après le séparateur est vide,
        /// retourne <paramref name="cultureCode"/> en majuscules tel
        /// quel (cas marginal défensif).</returns>
        /// <remarks>
        /// <para>Contexte : Helper privé consommé exclusivement par
        /// <see cref="LoadAsync"/> pour l'initialisation de l'état
        /// de sélection au montage de la page. Reproduit la sémantique
        /// d'extraction de <c>UC_Language_Apply.ExtractCountryCode</c>
        /// côté <c>B_UseCases</c> (composante après le séparateur
        /// <c>-</c>, mise en majuscules), avec robustesse aux cas
        /// marginaux d'absence de séparateur ou de composante vide.</para>
        ///
        /// <para>Comportement détaillé :</para>
        /// <list type="bullet">
        ///   <item><description><c>"fr-FR"</c> → <c>"FR"</c> :
        ///   séparateur trouvé en index 2, composante après le
        ///   séparateur non vide, retour de
        ///   <c>Substring(3).ToUpperInvariant()</c>.</description></item>
        ///   <item><description><c>"en-GB"</c> → <c>"GB"</c> : idem.</description></item>
        ///   <item><description><c>"FR"</c> → <c>"FR"</c> : séparateur
        ///   absent, retour du paramètre en majuscules tel quel.</description></item>
        ///   <item><description><c>"fr-"</c> → <c>"FR-"</c> :
        ///   séparateur trouvé mais composante vide, retour du
        ///   paramètre en majuscules tel quel (cas marginal défensif).</description></item>
        ///   <item><description><c>""</c> → <c>""</c> : paramètre
        ///   vide, retour du paramètre tel quel.</description></item>
        /// </list>
        /// </remarks>
        private string ExtractCountryCodeFromCulture(string cultureCode)
        {
            int index = cultureCode.LastIndexOf('-');
            return index >= 0 && index < cultureCode.Length - 1
                ? cultureCode.Substring(index + 1).ToUpperInvariant()
                : cultureCode.ToUpperInvariant();
        }

        /// <summary>
        /// Convertit un code pays ISO 3166-1 alpha-2 en code culture
        /// au format RFC 4646 par table de correspondance statique
        /// des 6 langues supportées par l'application.
        /// </summary>
        /// <param name="countryCode">Code pays ISO 3166-1 alpha-2
        /// transmis par WPF en <c>CommandParameter</c> du bouton de
        /// langue cliqué (« FR », « GB », « DE », « ES », « IT »,
        /// « PT »).</param>
        /// <returns>Code culture au format RFC 4646 correspondant au
        /// code pays fourni. Pour un code pays inconnu, retourne le
        /// code culture <c>"en-GB"</c> au titre du repli aligné sur
        /// la politique d'application au démarrage portée par
        /// <c>UC_Application_OnStart</c>.</returns>
        /// <remarks>
        /// <para>Contexte : Helper privé consommé exclusivement par
        /// <see cref="ChangeLanguageAsync"/> pour la conversion du
        /// code pays reçu de la commande en code culture transmis au
        /// UseCase <see cref="IU_Language_Apply"/>. La table de
        /// correspondance est en dur (6 cas + repli), par
        /// transposition littérale du legacy <c>VM_Page91</c> en
        /// substituant les codes langue ISO 639-1 (FR, EN, DE, ES,
        /// IT, PT) par les codes pays ISO 3166-1 alpha-2 (FR, GB,
        /// DE, ES, IT, PT) conformément à la convention sémantique
        /// structurante du fil. La correspondance
        /// <c>"GB" → "en-GB"</c> en particulier remplace la
        /// correspondance legacy <c>"EN" → "en-US"</c>, par
        /// alignement sur le code culture appliqué par défaut au
        /// démarrage de l'application (<c>en-GB</c>).</para>
        ///
        /// <para>Repli défensif : Pour un code pays inconnu, le repli
        /// par défaut est <c>"en-GB"</c> (et non <c>"fr-FR"</c> comme
        /// le legacy), par alignement sur la politique d'application
        /// au démarrage de DG244Cutting qui applique <c>en-GB</c> à
        /// défaut lorsque la langue du poste n'est pas l'une des six
        /// langues supportées. Cas marginal structurellement
        /// non atteignable depuis l'utilisateur (les 6
        /// <c>CommandParameter</c> bindés en dur dans le XAML couvrent
        /// les 6 codes pays attendus), mais le repli est posé en
        /// geste de robustesse.</para>
        /// </remarks>
        private string GetCultureCodeFromCountryCode(string countryCode)
        {
            return countryCode switch
            {
                "FR" => "fr-FR",
                "GB" => "en-GB",
                "DE" => "de-DE",
                "ES" => "es-ES",
                "IT" => "it-IT",
                "PT" => "pt-PT",
                _ => "en-GB"
            };
        }

        /// <summary>
        /// Met à jour l'état de sélection des 6 booléens
        /// <c>IsLanguage{n}Selected</c> en fonction du code pays
        /// fourni, en réinitialisation systématique des 6 booléens
        /// à <see langword="false"/> puis affectation
        /// <see langword="true"/> sur le booléen correspondant au
        /// code pays.
        /// </summary>
        /// <param name="countryCode">Code pays ISO 3166-1 alpha-2
        /// identifiant la langue dont le bouton doit être coché.</param>
        /// <remarks>
        /// <para>Contexte : Helper privé consommé par
        /// <see cref="LoadAsync"/> (au montage de la page pour
        /// initialiser la sélection visuelle sur la langue active) et
        /// par <see cref="ChangeLanguageAsync"/> (après invocation du
        /// UseCase pour rafraîchir la sélection visuelle sur la
        /// nouvelle langue active). Reproduit le comportement du
        /// helper legacy <c>VM_Page91.ApplyLanguageSelection</c> en
        /// substituant les codes langue par les codes pays
        /// ISO 3166-1 alpha-2 conformément à la convention sémantique
        /// structurante du fil.</para>
        ///
        /// <para>Comportement détaillé : Réinitialise inconditionnellement
        /// les 6 booléens à <see langword="false"/>, puis bascule à
        /// <see langword="true"/> le booléen correspondant au code
        /// pays via un <c>switch</c> sur 6 cas. Pour un code pays
        /// inconnu (cas marginal structurellement non atteignable
        /// dans le flux nominal), aucun booléen n'est sélectionné —
        /// comportement défensif sans branche <c>default</c>
        /// basculant arbitrairement sur le français (divergence
        /// nominale avec le legacy
        /// <c>VM_Page91.ApplyLanguageSelection</c> qui basculait sur
        /// <c>IsLanguage1Selected = true</c> dans la branche
        /// <c>default</c>).</para>
        ///
        /// <para>Filet de sécurité : Aucun try/catch local. Les
        /// mutations sont des affectations de booléens via le helper
        /// hérité <c>SetProperty&lt;T&gt;</c>, qui ne lève aucune
        /// exception. L'invocation est implicitement protégée par
        /// l'encapsulation par <see cref="VM_Generic.ExecuteSafeAsync"/>
        /// portée par les deux appelants.</para>
        /// </remarks>
        private void ApplyLanguageSelection(string countryCode)
        {
            IsLanguage1Selected = false;
            IsLanguage2Selected = false;
            IsLanguage3Selected = false;
            IsLanguage4Selected = false;
            IsLanguage5Selected = false;
            IsLanguage6Selected = false;

            switch (countryCode)
            {
                case "FR": IsLanguage1Selected = true; break;
                case "GB": IsLanguage2Selected = true; break;
                case "DE": IsLanguage3Selected = true; break;
                case "ES": IsLanguage4Selected = true; break;
                case "IT": IsLanguage5Selected = true; break;
                case "PT": IsLanguage6Selected = true; break;
            }
        }

        /// <summary>
        /// Handler asynchrone de la commande
        /// <see cref="ChangeLanguageCommand"/>, orchestrant le
        /// changement de langue de l'application via le UseCase
        /// <see cref="IU_Language_Apply"/> invoqué par
        /// <see cref="IS_UseCaseInvoker"/> (EA-11), puis la mise à
        /// jour inconditionnelle de l'état de sélection visuelle.
        /// </summary>
        /// <param name="countryCode">Code pays ISO 3166-1 alpha-2
        /// transmis par WPF en <c>CommandParameter</c> du bouton de
        /// langue cliqué.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone du
        /// changement de langue.</returns>
        /// <remarks>
        /// <para>Contexte : Handler privé asynchrone de signature
        /// <c>private async Task ChangeLanguageAsync(string countryCode)</c>
        /// conforme au délégué <c>Func&lt;string, Task&gt;</c> attendu
        /// par
        /// <see cref="UT_RelayCommandArg1Async{T}"/> avec
        /// <c>T = string</c>. Invoqué exclusivement par WPF au clic
        /// d'un bouton de langue, via la chaîne
        /// <c>Button.Command</c> → <c>UT_RelayCommandArg1Async&lt;string&gt;.Execute</c>
        /// → <c>UT_RelayCommandArg1Async&lt;string&gt;.ExecuteAsync</c>
        /// → présent handler. La commande WPF ne fournit pas de
        /// jeton d'annulation propre ; le présent handler invoque
        /// <see cref="IU_Language_Apply"/> avec
        /// <see cref="CancellationToken.None"/>.</para>
        ///
        /// <para>Séquence d'exécution :</para>
        /// <list type="number">
        ///   <item><description>Construction d'une CallChain interne
        ///   via <see cref="VM_Generic.BuildFirstCallChain"/> au
        ///   format <c>{_callee} &gt; ChangeLanguageAsync</c>.</description></item>
        ///   <item><description>Encapsulation du corps par
        ///   <see cref="VM_Generic.ExecuteSafeAsync"/> selon le
        ///   pipeline canonique à quatre captures (No_EC_01 /
        ///   No_EC_02 / No_EC_03 + propagation de
        ///   <see cref="OperationCanceledException"/>), §4.7.3 du
        ///   0230.</description></item>
        ///   <item><description>Conversion du code pays reçu en code
        ///   culture via le helper privé
        ///   <see cref="GetCultureCodeFromCountryCode"/>.</description></item>
        ///   <item><description>Invocation du UseCase
        ///   <see cref="IU_Language_Apply"/> via
        ///   <see cref="IS_UseCaseInvoker"/>, surcharge à retour
        ///   signalable
        ///   <c>InvokeAsync&lt;IU_Language_Apply, bool&gt;</c>
        ///   exigée par la signature
        ///   <c>Task&lt;bool&gt; ExecuteAsync(...)</c> du contrat
        ///   (R-4.14.21). Le délégué consomme
        ///   <c>innerCallChain</c> et <c>innerCt</c> transmis par
        ///   <see cref="IS_UseCaseInvoker"/>. La valeur de retour
        ///   <c>bool</c> est capturée par la signature mais n'est pas
        ///   exploitée par le handler — la mise à jour de la
        ///   sélection visuelle via
        ///   <see cref="ApplyLanguageSelection"/> est inconditionnelle,
        ///   par transposition littérale du comportement legacy. Le
        ///   traitement terminal des exceptions applicatives typées
        ///   est porté par <c>UC_Language_Apply</c> lui-même via ses
        ///   trois codes <c>La_EC_01</c> / <c>La_EC_02</c> /
        ///   <c>La_EC_03</c> ; le filet <c>ExecuteSafeAsync</c>
        ///   wrappant le présent handler est un filet de sécurité
        ///   ultime additionnel, dont la consommation effective est
        ///   doctrinale (cohérence avec le patron de tous les
        ///   handlers asynchrones de ViewModels).</description></item>
        ///   <item><description>Mise à jour inconditionnelle de
        ///   l'état de sélection des 6 booléens via le helper privé
        ///   <see cref="ApplyLanguageSelection"/> avec le code pays
        ///   reçu — la réussite ou l'échec de l'invocation du UseCase
        ///   ne conditionne pas la mise à jour visuelle (transposition
        ///   littérale du comportement legacy ; en cas d'échec, le
        ///   filet <see cref="VM_Generic.ExecuteSafeAsync"/> capture
        ///   l'exception avant l'atteinte de cette ligne, ce qui
        ///   neutralise la mise à jour dans les faits).</description></item>
        /// </list>
        ///
        /// <para>Annulation coopérative : La commande WPF n'offre pas
        /// de mécanisme natif d'annulation ; le présent handler
        /// invoque <see cref="IS_UseCaseInvoker"/> avec
        /// <see cref="CancellationToken.None"/>. La propagation
        /// d'un jeton d'annulation jusqu'au UseCase est
        /// structurellement non implémentable depuis une commande
        /// WPF déclenchée par un clic utilisateur synchrone.</para>
        ///
        /// <para>Aucun rafraîchissement explicite du titre de
        /// l'application ni navigation : l'effet d'application de la
        /// nouvelle langue se propage automatiquement par cascade
        /// INPC sur <see cref="ISE_App.AppCultureCode"/>, déclenchant
        /// le rechargement des labels sur tous les VMs actifs (y
        /// compris le bandeau de l'application). La page Page02 reste
        /// affichée après application ; l'utilisateur sort
        /// exclusivement via les boutons transverses du menu
        /// horizontal MH02.</para>
        /// </remarks>
        private async Task ChangeLanguageAsync(string countryCode)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                string cultureCode =
                    GetCultureCodeFromCountryCode(countryCode);

                await _useCaseInvoker
                    .InvokeAsync<IU_Language_Apply, bool>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            innerCallChain, cultureCode, innerCt),
                        CancellationToken.None);

                ApplyLanguageSelection(countryCode);
            });
        }

        #endregion
    }
}