using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using DG244Cutting.A_Domain.Common.Enums.Business;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Business;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.Business;
using DG244Cutting.A_Domain.Interfaces.ViewModels;
using DG244Cutting.D_Presentation.Utilities.RelayCommands;
using DG244Cutting.D_Presentation.ViewModels.Generic;

namespace DG244Cutting.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la page de validation de barre <c>Page20</c> : présente à
    /// l'opérateur la barre que l'application lui désigne pour la série
    /// sélectionnée, recueille sa décision sur cette barre et enchaîne les
    /// traitements qui en découlent.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : La Page20 est le cœur du parcours de production.
    /// L'approvisionnement y est conduit à la demande : chaque barre est
    /// désignée juste avant d'être coupée. L'opérateur prend la barre
    /// présentée, en constate l'état, puis la valide — avec ou sans zones
    /// défectueuses —, la refuse en indiquant un motif, ou la déclare en
    /// rupture de stock lorsqu'il s'agit d'une barre neuve introuvable. Le
    /// ViewModel est enregistré en Singleton ; seule la vue est reconstruite
    /// à chaque navigation ou rafraîchissement, et l'état présenté est donc
    /// intégralement réinitialisé à chaque chargement.</para>
    /// <para>Objectif : Déterminer, avant tout affichage, la barre à
    /// présenter ou l'issue qui en tient lieu, exposer l'état de la page en
    /// deux régimes exclusifs, et traduire chaque geste de l'opérateur en
    /// invocation de UseCase suivie de la suite de parcours appropriée.</para>
    /// <para>Séquence d'entrée, rejouée à chaque affichage de la page :</para>
    /// <list type="number">
    ///   <item><description>Recherche d'une barre en cours pour la série
    ///   sélectionnée, c'est-à-dire issue d'une optimisation, ni épuisée, ni
    ///   refusée, ni en rupture. Une barre en cours non validée est
    ///   présentée ; une barre en cours déjà validée n'est pas présentée et
    ///   l'opérateur est conduit directement à la page de découpe
    ///   <c>Page21</c> pour reprendre le travail engagé.</description></item>
    ///   <item><description>À défaut, optimisation d'une nouvelle barre pour
    ///   la série. Une barre optimisée est présentée ; l'absence de toute
    ///   découpe optimisable conduit à l'étape 3 ; un échec, déjà notifié par
    ///   le UseCase, ramène à la page de sélection des séries
    ///   <c>Page10</c>.</description></item>
    ///   <item><description>À défaut, statut d'achèvement de la série. Une
    ///   série achevée, ou un échec déjà notifié, ramène à la
    ///   <c>Page10</c>. Une série inachevée dont toutes les découpes
    ///   restantes sont bloquées par des ruptures de stock place la page en
    ///   régime bloqué.</description></item>
    /// </list>
    /// <para>Régimes d'affichage, jamais actifs simultanément :</para>
    /// <list type="bullet">
    ///   <item><description>Barre présentée : les onglets « Barre »,
    ///   « Défauts » et « Découpes » sont actifs, l'onglet « Ruptures » est
    ///   inactif, et les commandes du menu horizontal suivent leurs gardes
    ///   <see cref="CanValidate"/>, <see cref="CanReject"/> et
    ///   <see cref="CanDeclareOutOfStock"/>.</description></item>
    ///   <item><description>Régime bloqué : seul l'onglet « Ruptures » est
    ///   actif ; il liste les barres en rupture de la série, dont chacune
    ///   peut être libérée par sa case à cocher. Les trois gardes du menu
    ///   sont fausses.</description></item>
    /// </list>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Exécuter la séquence d'entrée au chargement de
    ///   la page et écrire le contexte de sélection de barre
    ///   (<see cref="ISE_UseCase.SelectBar"/> après reprise ou optimisation,
    ///   <see cref="ISE_UseCase.ClearBar"/> après refus, barre inutilisable
    ///   ou rupture).</description></item>
    ///   <item><description>Exposer la fiche de la barre — l'identification
    ///   de la série de production à laquelle elle appartient, l'emplacement
    ///   où en prendre la matière et les caractéristiques du profilé —, son
    ///   image de section, la liste des motifs de refus applicables à son
    ///   origine, le plan de coupe et la liste des barres en
    ///   rupture.</description></item>
    ///   <item><description>Porter la saisie transitoire du motif de refus
    ///   et des deux zones défectueuses, sans aucune persistance avant
    ///   validation.</description></item>
    ///   <item><description>Honorer le contrat <see cref="IV_Page20"/> par
    ///   lequel le menu horizontal de la page déclenche la validation, le
    ///   refus et la déclaration de rupture, et notifier tout changement des
    ///   gardes associées.</description></item>
    ///   <item><description>Contrôler la cohérence de la saisie avant
    ///   validation ou refus, et avertir l'opérateur d'une saisie
    ///   incomplète ou incohérente sans quitter la page.</description></item>
    ///   <item><description>Libérer une barre en rupture depuis l'onglet
    ///   « Ruptures » par la commande
    ///   <see cref="ReleaseBarCommand"/>.</description></item>
    ///   <item><description>Traduire l'issue de chaque UseCase en
    ///   navigation ou en rafraîchissement de la page.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne porte aucune règle de gestion : l'optimisation,
    ///   la validation, le refus, la mise en rupture, la libération et
    ///   l'achèvement de série sont intégralement portés par les UseCases
    ///   invoqués, qui en assurent la transaction.</description></item>
    ///   <item><description>Ne notifie aucun échec de traitement : les
    ///   UseCases notifient eux-mêmes leurs échecs et les signalent par leur
    ///   valeur de retour, que le ViewModel se borne à traduire en
    ///   navigation.</description></item>
    ///   <item><description>Ne demande pas la confirmation de la déclaration
    ///   de rupture, portée par le menu horizontal.</description></item>
    ///   <item><description>N'accède à aucune donnée ni ne navigue
    ///   directement : lectures, écritures et navigations transitent par
    ///   <see cref="IS_UseCaseInvoker"/>.</description></item>
    /// </list>
    /// <para>Note sur les exceptions architecturales : Le ViewModel hérite
    /// d'EA-01 par <see cref="VM_Generic"/>, dont le filet
    /// <see cref="VM_Generic.ExecuteSafeAsync"/> enveloppe chacune de ses
    /// méthodes publiques et de sa commande. Il consomme EA-11 : UseCases,
    /// Query Handlers et navigation sont résolus à l'invocation par
    /// <see cref="IS_UseCaseInvoker"/>, sans injection directe d'un contrat
    /// <c>IU_</c> ou <c>IQ_</c>. Cette médiation matérialise un scope
    /// distinct par invocation et maintient la portée Singleton du
    /// ViewModel.</para>
    ///
    /// <para>Convention <c>PXX_00 = nom de la page</c> :</para>
    ///
    /// <para>La clé <c>P20_00</c> exposée par le présent ViewModel
    /// matérialise la convention selon laquelle, pour toute page de la
    /// série 10-80 de l'application DG244Cutting, la clé <c>_00</c> du
    /// dictionnaire multilingue désigne le nom de la page. L'application
    /// est conçue selon une métaphore de livre : les parties 1 à 8
    /// portent les fonctionnalités métier, la partie 9 porte
    /// l'administration, chaque partie étant subdivisée en sections 0 à
    /// 9. La clé <c>_00</c> isole donc, dans cette nomenclature,
    /// l'identifiant nominal de la page elle-même, distinct des libellés
    /// fonctionnels portés par les clés <c>_01</c> à <c>_09</c> de la
    /// même section. Cette convention est introduite par le présent fil
    /// et conditionne la cohérence des futures Page20 → Page80 ; son
    /// inscription formelle dans le 0230 relèvera d'un fil de
    /// maintenance documentaire ultérieur.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à cinq régions
    /// standard (§4.4.2) complétée par deux extensions (§4.4.3) :
    /// Propriétés publiques et Méthodes protégées. Soit sept régions au
    /// total :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> : constantes
    ///   de navigation, d'onglets et de filtrage des motifs, table des clés
    ///   de motifs de refus, cache des libellés de motifs, champs support des
    ///   propriétés observables.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> : médiateur
    ///   des UseCases, contexte de sélection, service de notification,
    ///   référentiel des sections de profil.</description></item>
    ///   <item><description><c>=== Propriétés publiques ===</c> : nom de
    ///   page, état de la barre présentée, saisie, collections, régimes,
    ///   gardes du contrat, commande de libération, libellés
    ///   multilingues.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> : délégation à
    ///   <see cref="VM_Page_Generic"/>, gardes locales, instanciation de la
    ///   commande et invocation finale de
    ///   <see cref="VM_Generic.InitializeLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> : override de
    ///   <see cref="LoadAsync"/> et implémentation de
    ///   <see cref="IV_Page20"/>.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> : override de
    ///   <see cref="LoadLabels"/>.</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> : réinitialisation
    ///   de l'état, chargements, construction des motifs, libération de
    ///   barre, avertissement de saisie, notification des gardes, enveloppes
    ///   de navigation et de rafraîchissement.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Événements / Délégués / Indexeurs ===</c>
    /// n'est pas présente : <see cref="VM_Page20"/> n'expose aucun
    /// événement propre, l'événement <c>PropertyChanged</c> étant porté
    /// par <see cref="VM_Generic"/> au titre d'INPC et hérité par
    /// transitivité.</para>
    /// </remarks>
    public class VM_Page20 : VM_Page_Generic, IV_Page20
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom logique de la page de sélection des séries, destination des
        /// issues d'échec et d'achèvement.
        /// </summary>
        private const string SeriesSelectionPageName = "Page10";

        /// <summary>
        /// Nom logique de la page de découpe, destination d'une barre validée.
        /// </summary>
        private const string CuttingPageName = "Page21";

        /// <summary>
        /// Index de l'onglet « Barre », sélectionné à la présentation d'une barre.
        /// </summary>
        private const int BarTabIndex = 0;

        /// <summary>
        /// Index de l'onglet « Défauts », sélectionné lorsque la saisie des
        /// zones défectueuses doit être corrigée.
        /// </summary>
        private const int DefectsTabIndex = 1;

        /// <summary>
        /// Index de l'onglet « Ruptures », sélectionné en régime bloqué.
        /// </summary>
        private const int OutOfStockTabIndex = 3;

        /// <summary>
        /// Borne exclusive des valeurs de motif proposées pour une barre
        /// neuve : seule la famille Qualité (valeurs 1 à 9) s'applique.
        /// </summary>
        private const int NewBarReasonUpperBound = 10;

        /// <summary>
        /// Borne exclusive des valeurs de motif proposées pour une chute :
        /// familles Qualité et Écart de stock (valeurs 1 à 19). Le motif
        /// système, de valeur 90, n'est jamais proposé.
        /// </summary>
        private const int ScrapReasonUpperBound = 90;

        /// <summary>
        /// Table de correspondance, dans l'ordre de présentation, entre chaque
        /// motif de refus sélectionnable et la clé multilingue de son libellé.
        /// </summary>
        /// <remarks>
        /// <para>Le motif système <see cref="En_BarRejectionReason.BarUnproductiveAfterDefects"/>
        /// n'y figure pas : il est posé par l'application et n'est jamais
        /// proposé à l'opérateur.</para>
        /// </remarks>
        private static readonly IReadOnlyList<KeyValuePair<En_BarRejectionReason, string>> RejectionReasonKeys =
            new List<KeyValuePair<En_BarRejectionReason, string>>
            {
                new(En_BarRejectionReason.BarDeformed, "P20_30"),
                new(En_BarRejectionReason.BarDamaged, "P20_31"),
                new(En_BarRejectionReason.TooManyDefects, "P20_32"),
                new(En_BarRejectionReason.ColorNotCompliant, "P20_33"),
                new(En_BarRejectionReason.ReferenceNotCompliant, "P20_34"),
                new(En_BarRejectionReason.ScrapNotFound, "P20_35"),
                new(En_BarRejectionReason.ScrapAlreadyConsumed, "P20_36"),
                new(En_BarRejectionReason.ScrapLengthMismatch, "P20_37")
            };

        /// <summary>
        /// Cache des libellés localisés des motifs de refus, indexé par clé
        /// multilingue, alimenté par <see cref="LoadLabels"/> et consommé par
        /// <see cref="BuildRejectionReasons"/>.
        /// </summary>
        /// <remarks>
        /// <para>Le cache confine la résolution des libellés à
        /// <see cref="LoadLabels"/> : la construction de la liste des motifs,
        /// déclenchée aussi au chargement d'une barre, ne consulte jamais le
        /// dictionnaire.</para>
        /// </remarks>
        private readonly Dictionary<string, string> _rejectionReasonLabels = new();

        /// <summary>
        /// Champ support de la propriété observable <see cref="PageName"/>,
        /// initialisé à <see cref="string.Empty"/> et écrasé au
        /// constructeur par le premier appel à <see cref="LoadLabels"/>
        /// orchestré par <see cref="VM_Generic.InitializeLabels"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : L'initialisation à <see cref="string.Empty"/>
        /// garantit que la propriété est dans un état défini avant le
        /// premier binding WPF, même dans l'hypothèse théorique où la
        /// résolution de la clé <c>P20_00</c> échouerait silencieusement
        /// — auquel cas <c>SR_Dictionary</c> retourne la valeur de repli
        /// <c>[P20_00] not found</c> qui sera affectée par
        /// <see cref="LoadLabels"/>. La valeur <see cref="string.Empty"/>
        /// n'est observable que pendant la fenêtre nanoseconde entre la
        /// construction du champ et le premier appel à
        /// <see cref="LoadLabels"/> au constructeur ; elle est ensuite
        /// écrasée avant le premier binding.</para>
        /// </remarks>
        private string _pageName = string.Empty;

        /// <summary>
        /// Champ support de <see cref="CurrentBar"/>.
        /// </summary>
        private DTO_VwProductionBarFull? _currentBar;

        /// <summary>
        /// Champ support de <see cref="OriginLabel"/>.
        /// </summary>
        private string _originLabel = string.Empty;

        /// <summary>
        /// Champ support de <see cref="SourceLocation"/>.
        /// </summary>
        private string _sourceLocation = string.Empty;

        /// <summary>
        /// Champ support de <see cref="ProfilSectionUri"/>, initialisé au
        /// constructeur sur l'image de section par défaut.
        /// </summary>
        private Uri _profilSectionUri;

        /// <summary>
        /// Champ support de <see cref="RejectionReasons"/>.
        /// </summary>
        private IReadOnlyList<KeyValuePair<En_BarRejectionReason, string>> _rejectionReasons =
            Array.Empty<KeyValuePair<En_BarRejectionReason, string>>();

        /// <summary>
        /// Champ support de <see cref="SelectedRejectionReason"/>.
        /// </summary>
        private En_BarRejectionReason? _selectedRejectionReason;

        /// <summary>
        /// Champ support de <see cref="DefectStart1"/>.
        /// </summary>
        private int? _defectStart1;

        /// <summary>
        /// Champ support de <see cref="DefectEnd1"/>.
        /// </summary>
        private int? _defectEnd1;

        /// <summary>
        /// Champ support de <see cref="DefectStart2"/>.
        /// </summary>
        private int? _defectStart2;

        /// <summary>
        /// Champ support de <see cref="DefectEnd2"/>.
        /// </summary>
        private int? _defectEnd2;

        /// <summary>
        /// Champ support de <see cref="IsBarPresented"/>.
        /// </summary>
        private bool _isBarPresented;

        /// <summary>
        /// Champ support de <see cref="IsBlockedRegime"/>.
        /// </summary>
        private bool _isBlockedRegime;

        /// <summary>
        /// Champ support de <see cref="SelectedTabIndex"/>.
        /// </summary>
        private int _selectedTabIndex;

        /// <summary>
        /// Champ support de <see cref="Label_P20_01"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_01 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_02"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_02 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_03"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_03 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_04"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_04 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_05"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_05 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_06"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_06 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_07"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_07 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_08"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_08 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_09"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_09 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_10"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_10 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_11"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_11 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_12"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_12 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_13"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_13 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_14"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_14 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_15"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_15 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_16"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_16 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_17"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_17 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_18"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_18 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_19"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_19 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_20"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_20 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_21"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_21 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_22"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_22 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_23"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_23 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_39"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_39 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_40"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_40 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_41"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_41 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_42"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_42 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_43"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_43 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_44"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_44 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_45"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_45 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_46"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_46 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_47"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_47 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_48"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_48 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_49"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_49 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_50"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_50 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_51"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_51 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_52"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_52 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_53"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_53 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_54"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_54 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_55"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_55 = string.Empty;

        /// <summary>
        /// Champ support de <see cref="Label_P20_56"/>, initialisé à
        /// <see cref="string.Empty"/> et alimenté par <see cref="LoadLabels"/>.
        /// </summary>
        private string _label_p20_56 = string.Empty;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Médiateur par lequel sont invoqués les UseCases, les Query Handlers
        /// et la navigation (EA-11).
        /// </summary>
        private readonly IS_UseCaseInvoker _useCaseInvoker;

        /// <summary>
        /// Contexte de sélection métier : fournit la série sélectionnée et
        /// reçoit la sélection ou la désélection de la barre traitée.
        /// </summary>
        private readonly ISE_UseCase _seUseCase;

        /// <summary>
        /// Service de notification utilisé pour les avertissements de saisie.
        /// </summary>
        private readonly IS_Notification _notification;

        /// <summary>
        /// Référentiel des images de section de profil, qui résout l'image
        /// associée à la référence de la barre présentée.
        /// </summary>
        private readonly ISE_BarProfilSection _barProfilSection;

        #endregion

        #region === Propriétés publiques ===

        /// <summary>
        /// Obtient le nom multilingue de la page <c>Page20</c>, miroir du
        /// libellé associé à la clé <c>P20_00</c> dans le dictionnaire de
        /// langue actif.
        /// </summary>
        /// <value>
        /// Chaîne localisée résolue à partir du dictionnaire de langue
        /// actif. En cas de clé absente, <c>SR_Dictionary</c> retourne la
        /// valeur de repli <c>[P20_00] not found</c> conformément à
        /// R-4.11.6 du 0231.
        /// </value>
        /// <remarks>
        /// <para>Contexte : Propriété bindable consommée par la vue
        /// <see cref="DG244Cutting.D_Presentation.Views.Pages.Page20"/>
        /// via le binding standard <c>Text="{Binding PageName}"</c> sur
        /// le <c>TextBlock</c> de titre de la page. L'accesseur en
        /// écriture est privé : la valeur ne peut être modifiée qu'à
        /// travers l'override de <see cref="LoadLabels"/>, appelé
        /// initialement par <see cref="VM_Generic.InitializeLabels"/> au
        /// constructeur puis par le handler interne d'abonnement à
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> de
        /// <see cref="ISE_App"/> porté par <see cref="VM_Generic"/> à
        /// chaque changement de langue dynamique, avec marshalling
        /// Dispatcher défensif vers le thread UI. Conformément à la
        /// convention <c>PXX_00 = nom de la page</c> introduite par le
        /// présent ViewModel, la clé <c>P20_00</c> désigne nominalement
        /// le nom de la page dans la nomenclature multilingue de la
        /// série 10-80.</para>
        /// </remarks>
        public string PageName
        {
            get => _pageName;
            private set => SetProperty(ref _pageName, value);
        }

        /// <summary>
        /// Obtient la barre présentée à l'opérateur, ou <see langword="null"/>
        /// lorsqu'aucune barre n'est présentée.
        /// </summary>
        /// <remarks>
        /// <para>Alimente la fiche de l'onglet « Barre ». Tout changement
        /// notifie les trois gardes du contrat <see cref="IV_Page20"/>.</para>
        /// </remarks>
        public DTO_VwProductionBarFull? CurrentBar
        {
            get => _currentBar;
            private set
            {
                if (SetProperty(ref _currentBar, value))
                {
                    RaiseGuardsChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le libellé localisé de l'origine de la barre présentée :
        /// barre neuve ou chute.
        /// </summary>
        /// <value>Chaîne vide lorsqu'aucune barre n'est présentée.</value>
        public string OriginLabel
        {
            get => _originLabel;
            private set => SetProperty(ref _originLabel, value);
        }

        /// <summary>
        /// Obtient l'emplacement où l'opérateur doit aller prendre la matière
        /// de la barre présentée.
        /// </summary>
        /// <value>Pour une barre de chute, l'emplacement réel où cette chute a
        /// été rangée. Pour une barre neuve, le nom du magasin de profilés
        /// neufs, unique et localisé. Chaîne vide, jamais
        /// <see langword="null"/>, lorsque aucune barre n'est présentée ou que
        /// la chute présentée ne porte aucun emplacement enregistré.</value>
        /// <remarks>
        /// <para>Contexte : L'approvisionnement est conduit à la demande,
        /// chaque barre étant prise juste avant d'être coupée. La fiche de
        /// l'onglet « Barre » décrit la barre désignée ; elle doit aussi dire
        /// où la trouver, faute de quoi l'opérateur ne dispose d'aucun moyen
        /// de localiser la matière. La nature de l'emplacement diffère selon
        /// l'origine de la barre : une chute a été rangée à un emplacement
        /// précis lors de sa qualification ou de sa saisie manuelle, tandis
        /// qu'une barre neuve provient toujours du même magasin de
        /// profilés.</para>
        /// <para>Objectif : Exposer une valeur unique, directement lisible,
        /// couvrant les deux origines sans que la vue ait à les
        /// distinguer.</para>
        /// <para>Règle de résolution, appliquée à l'identique aux deux points
        /// d'affectation — la présentation d'une barre et le rechargement des
        /// libellés au changement de langue :</para>
        /// <list type="bullet">
        ///   <item><description>Barre neuve
        ///   (<see cref="DTO_VwProductionBarFull.PBIsNewBar"/> vrai) : valeur
        ///   de <see cref="Label_P20_22"/>, nom localisé du magasin de
        ///   profilés neufs.</description></item>
        ///   <item><description>Barre de chute : valeur de
        ///   <see cref="DTO_VwProductionBarFull.CSLScrapLocationSource"/>,
        ///   emplacement réel de rangement de la chute
        ///   d'origine.</description></item>
        ///   <item><description>Chute sans emplacement enregistré, la colonne
        ///   étant nullable : chaîne vide. Aucun repli textuel n'est
        ///   substitué — un libellé d'absence occuperait la place sans rien
        ///   apprendre à l'opérateur.</description></item>
        ///   <item><description>Aucune barre présentée : chaîne vide, posée
        ///   par la réinitialisation de l'état en tête de séquence
        ///   d'entrée.</description></item>
        /// </list>
        /// <para>Invariant — emplacement réel, jamais déduit : la valeur n'est
        /// en aucun cas dérivée des colonnes d'emplacement candidat du futur
        /// résidu portées par la même vue. Celles-ci désignent où ranger le
        /// résidu à venir, non où prendre la barre ; les afficher ici
        /// enverrait l'opérateur au mauvais endroit dès lors que la chute a
        /// été rangée ailleurs.</para>
        /// <para>Comportement au changement de langue : la valeur suit la
        /// langue active pour une barre neuve, le nom du magasin étant un
        /// libellé du dictionnaire, et en est indépendante pour une chute,
        /// l'emplacement étant une donnée de stock. La recomposition est
        /// portée par <see cref="LoadLabels"/> et intervient après
        /// l'affectation de <see cref="Label_P20_22"/>.</para>
        /// <para>L'accesseur en écriture est privé : la valeur est
        /// exclusivement dérivée de la barre courante et des libellés, et
        /// n'est jamais saisie — l'onglet est en lecture seule hors le
        /// sélecteur de motif de refus.</para>
        /// </remarks>
        public string SourceLocation
        {
            get => _sourceLocation;
            private set => SetProperty(ref _sourceLocation, value);
        }

        /// <summary>
        /// Obtient l'adresse de l'image de section de profil de la barre
        /// présentée.
        /// </summary>
        /// <value>Image associée à la référence de la barre, ou image par
        /// défaut lorsqu'aucune barre n'est présentée ou que la référence
        /// n'est pas répertoriée.</value>
        public Uri ProfilSectionUri
        {
            get => _profilSectionUri;
            private set => SetProperty(ref _profilSectionUri, value);
        }

        /// <summary>
        /// Obtient la liste des motifs de refus proposés pour la barre
        /// présentée, chacun associé à son libellé localisé.
        /// </summary>
        /// <value>Pour une barre neuve, les seuls motifs de la famille
        /// Qualité ; pour une chute, les motifs des familles Qualité et Écart
        /// de stock. Liste vide lorsqu'aucune barre n'est présentée. Le motif
        /// système n'y figure jamais.</value>
        public IReadOnlyList<KeyValuePair<En_BarRejectionReason, string>> RejectionReasons
        {
            get => _rejectionReasons;
            private set => SetProperty(ref _rejectionReasons, value);
        }

        /// <summary>
        /// Obtient ou définit le motif de refus choisi par l'opérateur.
        /// </summary>
        /// <value><see langword="null"/> tant qu'aucun motif n'est choisi.</value>
        /// <remarks>
        /// <para>Un motif est obligatoire pour refuser la barre et doit être
        /// absent pour la valider. L'accesseur en écriture est public pour la
        /// liaison bidirectionnelle du sélecteur de l'onglet « Barre ».</para>
        /// </remarks>
        public En_BarRejectionReason? SelectedRejectionReason
        {
            get => _selectedRejectionReason;
            set => SetProperty(ref _selectedRejectionReason, value);
        }

        /// <summary>
        /// Obtient ou définit la position de début de la première zone
        /// défectueuse, en millimètres depuis le début de la barre.
        /// </summary>
        /// <value><see langword="null"/> en l'absence de saisie.</value>
        /// <remarks>
        /// <para>Saisie transitoire de l'onglet « Défauts », transmise à la
        /// validation et jamais persistée auparavant. L'accesseur en écriture
        /// est public pour la liaison bidirectionnelle.</para>
        /// </remarks>
        public int? DefectStart1
        {
            get => _defectStart1;
            set => SetProperty(ref _defectStart1, value);
        }

        /// <summary>
        /// Obtient ou définit la position de fin de la première zone
        /// défectueuse, en millimètres depuis le début de la barre.
        /// </summary>
        /// <value><see langword="null"/> en l'absence de saisie.</value>
        /// <remarks>
        /// <para>Saisie transitoire de l'onglet « Défauts ». L'accesseur en
        /// écriture est public pour la liaison bidirectionnelle.</para>
        /// </remarks>
        public int? DefectEnd1
        {
            get => _defectEnd1;
            set => SetProperty(ref _defectEnd1, value);
        }

        /// <summary>
        /// Obtient ou définit la position de début de la seconde zone
        /// défectueuse, en millimètres depuis le début de la barre.
        /// </summary>
        /// <value><see langword="null"/> en l'absence de saisie.</value>
        /// <remarks>
        /// <para>Saisie transitoire de l'onglet « Défauts », admise
        /// seulement en présence d'une première zone. L'accesseur en écriture
        /// est public pour la liaison bidirectionnelle.</para>
        /// </remarks>
        public int? DefectStart2
        {
            get => _defectStart2;
            set => SetProperty(ref _defectStart2, value);
        }

        /// <summary>
        /// Obtient ou définit la position de fin de la seconde zone
        /// défectueuse, en millimètres depuis le début de la barre.
        /// </summary>
        /// <value><see langword="null"/> en l'absence de saisie.</value>
        /// <remarks>
        /// <para>Saisie transitoire de l'onglet « Défauts ». L'accesseur en
        /// écriture est public pour la liaison bidirectionnelle.</para>
        /// </remarks>
        public int? DefectEnd2
        {
            get => _defectEnd2;
            set => SetProperty(ref _defectEnd2, value);
        }

        /// <summary>
        /// Obtient le plan de coupe de la barre présentée, trié par position
        /// de découpe croissante, les découpes sans position figurant en fin
        /// de liste.
        /// </summary>
        /// <remarks>
        /// <para>Instance unique pour toute la durée de vie du ViewModel,
        /// vidée puis remplie à chaque chargement.</para>
        /// </remarks>
        public ObservableCollection<DTO_VwProductionCutPieceFull_P11> CutPlan { get; } = new();

        /// <summary>
        /// Obtient la liste des barres en rupture de stock de la série
        /// sélectionnée, alimentée en régime bloqué.
        /// </summary>
        /// <remarks>
        /// <para>Instance unique pour toute la durée de vie du ViewModel,
        /// vidée puis remplie à chaque chargement.</para>
        /// </remarks>
        public ObservableCollection<DTO_VwProductionBarFull> OutOfStockBars { get; } = new();

        /// <summary>
        /// Obtient une valeur indiquant qu'une barre est présentée à
        /// l'opérateur.
        /// </summary>
        /// <remarks>
        /// <para>Active les onglets « Barre », « Défauts » et « Découpes ».
        /// Jamais vraie en même temps que <see cref="IsBlockedRegime"/>. Tout
        /// changement notifie les trois gardes du contrat
        /// <see cref="IV_Page20"/>.</para>
        /// </remarks>
        public bool IsBarPresented
        {
            get => _isBarPresented;
            private set
            {
                if (SetProperty(ref _isBarPresented, value))
                {
                    RaiseGuardsChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant que la page est en régime bloqué :
        /// aucune découpe n'est optimisable et les découpes restantes sont
        /// bloquées par des ruptures de stock.
        /// </summary>
        /// <remarks>
        /// <para>Active le seul onglet « Ruptures » et conditionne
        /// l'exécution de <see cref="ReleaseBarCommand"/>. Jamais vraie en
        /// même temps que <see cref="IsBarPresented"/>. Tout changement
        /// notifie les trois gardes du contrat <see cref="IV_Page20"/>.</para>
        /// </remarks>
        public bool IsBlockedRegime
        {
            get => _isBlockedRegime;
            private set
            {
                if (SetProperty(ref _isBlockedRegime, value))
                {
                    RaiseGuardsChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'index de l'onglet sélectionné.
        /// </summary>
        /// <remarks>
        /// <para>Positionné par le ViewModel sur l'onglet « Barre » à la
        /// présentation d'une barre, sur l'onglet « Défauts » lorsque la
        /// saisie doit être corrigée, et sur l'onglet « Ruptures » en régime
        /// bloqué. L'accesseur en écriture est public pour la liaison
        /// bidirectionnelle du contrôle d'onglets.</para>
        /// </remarks>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        /// <summary>
        /// Obtient une valeur indiquant que la validation de la barre peut
        /// être demandée.
        /// </summary>
        /// <value><see langword="true"/> si et seulement si une barre est
        /// présentée.</value>
        public bool CanValidate => IsBarPresented;

        /// <summary>
        /// Obtient une valeur indiquant que le refus de la barre peut être
        /// demandé.
        /// </summary>
        /// <value><see langword="true"/> si et seulement si une barre est
        /// présentée.</value>
        public bool CanReject => IsBarPresented;

        /// <summary>
        /// Obtient une valeur indiquant que la barre peut être déclarée en
        /// rupture de stock.
        /// </summary>
        /// <value><see langword="true"/> si et seulement si une barre neuve
        /// est présentée : une chute absente relève du refus pour écart de
        /// stock, non de la rupture.</value>
        public bool CanDeclareOutOfStock => IsBarPresented && CurrentBar is { PBIsNewBar: true };

        /// <summary>
        /// Obtient la commande qui libère une barre en rupture de stock depuis
        /// l'onglet « Ruptures ».
        /// </summary>
        /// <remarks>
        /// <para>Paramètre : identifiant de la barre à libérer. Exécutable
        /// uniquement en régime bloqué. Une libération réussie rafraîchit la
        /// page, qui rejoue sa séquence d'entrée ; un échec, déjà notifié,
        /// ramène à la page de sélection des séries.</para>
        /// </remarks>
        public ICommand ReleaseBarCommand { get; }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_01</c> : l'en-tête de l'onglet 1, qui présente la fiche de la barre.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_01] not found</c> en cas de clé absente.</value>
        public string Label_P20_01
        {
            get => _label_p20_01;
            private set => SetProperty(ref _label_p20_01, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_02</c> : l'en-tête de l'onglet 2, consacré à la saisie des zones défectueuses.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_02] not found</c> en cas de clé absente.</value>
        public string Label_P20_02
        {
            get => _label_p20_02;
            private set => SetProperty(ref _label_p20_02, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_03</c> : l'en-tête de l'onglet 3, qui liste les découpes prévues sur la barre.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_03] not found</c> en cas de clé absente.</value>
        public string Label_P20_03
        {
            get => _label_p20_03;
            private set => SetProperty(ref _label_p20_03, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_04</c> : l'en-tête de l'onglet 4, qui liste les barres en rupture de stock de la série.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_04] not found</c> en cas de clé absente.</value>
        public string Label_P20_04
        {
            get => _label_p20_04;
            private set => SetProperty(ref _label_p20_04, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_05</c> : l'intitulé de l'origine de la barre dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_05] not found</c> en cas de clé absente.</value>
        public string Label_P20_05
        {
            get => _label_p20_05;
            private set => SetProperty(ref _label_p20_05, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_06</c> : l'intitulé de la référence dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_06] not found</c> en cas de clé absente.</value>
        public string Label_P20_06
        {
            get => _label_p20_06;
            private set => SetProperty(ref _label_p20_06, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_07</c> : l'intitulé de la désignation dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_07] not found</c> en cas de clé absente.</value>
        public string Label_P20_07
        {
            get => _label_p20_07;
            private set => SetProperty(ref _label_p20_07, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_08</c> : l'intitulé de la couleur dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_08] not found</c> en cas de clé absente.</value>
        public string Label_P20_08
        {
            get => _label_p20_08;
            private set => SetProperty(ref _label_p20_08, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_09</c> : l'intitulé de la catégorie dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_09] not found</c> en cas de clé absente.</value>
        public string Label_P20_09
        {
            get => _label_p20_09;
            private set => SetProperty(ref _label_p20_09, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_10</c> : l'intitulé de la longueur de barre dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_10] not found</c> en cas de clé absente.</value>
        public string Label_P20_10
        {
            get => _label_p20_10;
            private set => SetProperty(ref _label_p20_10, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_11</c> : l'intitulé du nombre de découpes dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_11] not found</c> en cas de clé absente.</value>
        public string Label_P20_11
        {
            get => _label_p20_11;
            private set => SetProperty(ref _label_p20_11, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_12</c> : l'intitulé du reste prévisionnel dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_12] not found</c> en cas de clé absente.</value>
        public string Label_P20_12
        {
            get => _label_p20_12;
            private set => SetProperty(ref _label_p20_12, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_13</c> : l'intitulé du sélecteur de motif de refus de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_13] not found</c> en cas de clé absente.</value>
        public string Label_P20_13
        {
            get => _label_p20_13;
            private set => SetProperty(ref _label_p20_13, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_14</c> : la valeur d'origine « barre neuve », reprise par <see cref="OriginLabel"/>.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_14] not found</c> en cas de clé absente.</value>
        public string Label_P20_14
        {
            get => _label_p20_14;
            private set => SetProperty(ref _label_p20_14, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_15</c> : la valeur d'origine « chute », reprise par <see cref="OriginLabel"/>.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_15] not found</c> en cas de clé absente.</value>
        public string Label_P20_15
        {
            get => _label_p20_15;
            private set => SetProperty(ref _label_p20_15, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_16</c> : l'intitulé de la borne de début de la première zone défectueuse.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_16] not found</c> en cas de clé absente.</value>
        public string Label_P20_16
        {
            get => _label_p20_16;
            private set => SetProperty(ref _label_p20_16, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_17</c> : l'intitulé de la borne de fin de la première zone défectueuse.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_17] not found</c> en cas de clé absente.</value>
        public string Label_P20_17
        {
            get => _label_p20_17;
            private set => SetProperty(ref _label_p20_17, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_18</c> : l'intitulé de la borne de début de la seconde zone défectueuse.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_18] not found</c> en cas de clé absente.</value>
        public string Label_P20_18
        {
            get => _label_p20_18;
            private set => SetProperty(ref _label_p20_18, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_19</c> : l'intitulé de la borne de fin de la seconde zone défectueuse.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_19] not found</c> en cas de clé absente.</value>
        public string Label_P20_19
        {
            get => _label_p20_19;
            private set => SetProperty(ref _label_p20_19, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_20</c> : l'intitulé du numéro de série de production dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_20] not found</c> en cas de clé absente.</value>
        public string Label_P20_20
        {
            get => _label_p20_20;
            private set => SetProperty(ref _label_p20_20, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_21</c> : l'intitulé de l'emplacement d'origine de la barre dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_21] not found</c> en cas de clé absente.</value>
        public string Label_P20_21
        {
            get => _label_p20_21;
            private set => SetProperty(ref _label_p20_21, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_22</c> : le nom du magasin de profilés neufs, repris par <see cref="SourceLocation"/>.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_22] not found</c> en cas de clé absente.</value>
        public string Label_P20_22
        {
            get => _label_p20_22;
            private set => SetProperty(ref _label_p20_22, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_23</c> : l'intitulé de la désignation de la série de production dans la fiche de l'onglet 1.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_23] not found</c> en cas de clé absente.</value>
        public string Label_P20_23
        {
            get => _label_p20_23;
            private set => SetProperty(ref _label_p20_23, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_39</c> : l'en-tête de colonne « référence », commun aux onglets 3 et 4.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_39] not found</c> en cas de clé absente.</value>
        public string Label_P20_39
        {
            get => _label_p20_39;
            private set => SetProperty(ref _label_p20_39, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_40</c> : l'en-tête de colonne « désignation », commun aux onglets 3 et 4.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_40] not found</c> en cas de clé absente.</value>
        public string Label_P20_40
        {
            get => _label_p20_40;
            private set => SetProperty(ref _label_p20_40, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_41</c> : l'en-tête de colonne « couleur », commun aux onglets 3 et 4.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_41] not found</c> en cas de clé absente.</value>
        public string Label_P20_41
        {
            get => _label_p20_41;
            private set => SetProperty(ref _label_p20_41, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_42</c> : l'en-tête de colonne « catégorie », commun aux onglets 3 et 4.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_42] not found</c> en cas de clé absente.</value>
        public string Label_P20_42
        {
            get => _label_p20_42;
            private set => SetProperty(ref _label_p20_42, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_43</c> : l'en-tête de colonne « hauteur », commun aux onglets 3 et 4.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_43] not found</c> en cas de clé absente.</value>
        public string Label_P20_43
        {
            get => _label_p20_43;
            private set => SetProperty(ref _label_p20_43, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_44</c> : l'en-tête de colonne « largeur », commun aux onglets 3 et 4.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_44] not found</c> en cas de clé absente.</value>
        public string Label_P20_44
        {
            get => _label_p20_44;
            private set => SetProperty(ref _label_p20_44, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_45</c> : l'en-tête de colonne de l'inclinaison gauche de coupe (onglet 3).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_45] not found</c> en cas de clé absente.</value>
        public string Label_P20_45
        {
            get => _label_p20_45;
            private set => SetProperty(ref _label_p20_45, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_46</c> : l'en-tête de colonne du pivot gauche de coupe (onglet 3).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_46] not found</c> en cas de clé absente.</value>
        public string Label_P20_46
        {
            get => _label_p20_46;
            private set => SetProperty(ref _label_p20_46, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_47</c> : l'en-tête de colonne de la longueur de découpe (onglet 3).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_47] not found</c> en cas de clé absente.</value>
        public string Label_P20_47
        {
            get => _label_p20_47;
            private set => SetProperty(ref _label_p20_47, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_48</c> : l'en-tête de colonne du pivot droit de coupe (onglet 3).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_48] not found</c> en cas de clé absente.</value>
        public string Label_P20_48
        {
            get => _label_p20_48;
            private set => SetProperty(ref _label_p20_48, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_49</c> : l'en-tête de colonne de l'inclinaison droite de coupe (onglet 3).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_49] not found</c> en cas de clé absente.</value>
        public string Label_P20_49
        {
            get => _label_p20_49;
            private set => SetProperty(ref _label_p20_49, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_50</c> : l'en-tête de colonne de la position de la découpe dans la barre (onglet 3).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_50] not found</c> en cas de clé absente.</value>
        public string Label_P20_50
        {
            get => _label_p20_50;
            private set => SetProperty(ref _label_p20_50, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_51</c> : l'en-tête de colonne de la longueur de barre (onglet 4).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_51] not found</c> en cas de clé absente.</value>
        public string Label_P20_51
        {
            get => _label_p20_51;
            private set => SetProperty(ref _label_p20_51, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_52</c> : l'en-tête de colonne de l'ordre de tri de l'article (onglet 4).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_52] not found</c> en cas de clé absente.</value>
        public string Label_P20_52
        {
            get => _label_p20_52;
            private set => SetProperty(ref _label_p20_52, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_53</c> : l'en-tête de colonne du nombre de découpes (onglet 4).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_53] not found</c> en cas de clé absente.</value>
        public string Label_P20_53
        {
            get => _label_p20_53;
            private set => SetProperty(ref _label_p20_53, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_54</c> : l'en-tête de colonne de la longueur du reste (onglet 4).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_54] not found</c> en cas de clé absente.</value>
        public string Label_P20_54
        {
            get => _label_p20_54;
            private set => SetProperty(ref _label_p20_54, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_55</c> : l'en-tête de colonne de l'indicateur de barre neuve (onglet 4).
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_55] not found</c> en cas de clé absente.</value>
        public string Label_P20_55
        {
            get => _label_p20_55;
            private set => SetProperty(ref _label_p20_55, value);
        }

        /// <summary>
        /// Obtient le libellé multilingue de la clé <c>P20_56</c> : l'en-tête de la colonne « rupture », seule colonne éditable de l'onglet 4, par laquelle une barre est libérée.
        /// </summary>
        /// <value>Chaîne localisée résolue par <see cref="LoadLabels"/> ; valeur de
        /// repli <c>[P20_56] not found</c> en cas de clé absente.</value>
        public string Label_P20_56
        {
            get => _label_p20_56;
            private set => SetProperty(ref _label_p20_56, value);
        }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="VM_Page20"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur invoqué une seule fois par le
        /// conteneur d'injection, le ViewModel étant enregistré en Singleton
        /// et relayé sous le contrat <see cref="IV_Page20"/>. Il n'effectue
        /// aucun accès aux données : le chargement est porté par
        /// <see cref="LoadAsync"/>.</para>
        /// <para>Séquence d'initialisation :</para>
        /// <list type="number">
        ///   <item><description>Délégation à <see cref="VM_Page_Generic"/>
        ///   via <c>base(dictionary, logAndNotify, app)</c>, qui applique les
        ///   gardes des trois premiers paramètres.</description></item>
        ///   <item><description>Affectation des quatre dépendances propres,
        ///   chacune protégée par une garde
        ///   <see cref="ArgumentNullException"/>.</description></item>
        ///   <item><description>Initialisation de l'image de section sur
        ///   l'image par défaut et instanciation de
        ///   <see cref="ReleaseBarCommand"/>.</description></item>
        ///   <item><description>Appel à
        ///   <see cref="VM_Generic.InitializeLabels"/> en dernière instruction,
        ///   qui charge les libellés et branche le rechargement au changement
        ///   de langue.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue, transmis à <see cref="VM_Page_Generic"/>.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement terminal des
        /// erreurs, transmis à <see cref="VM_Page_Generic"/> et mobilisé par
        /// le filet <see cref="VM_Generic.ExecuteSafeAsync"/> (EA-01).</param>
        /// <param name="app">État applicatif global, transmis à
        /// <see cref="VM_Page_Generic"/> pour la mécanique multilingue ; le
        /// présent ViewModel ne le stocke pas.</param>
        /// <param name="useCaseInvoker">Médiateur des UseCases, des Query
        /// Handlers et de la navigation (EA-11).</param>
        /// <param name="seUseCase">Contexte de sélection métier.</param>
        /// <param name="notification">Service de notification des
        /// avertissements de saisie.</param>
        /// <param name="barProfilSection">Référentiel des images de section de
        /// profil.</param>
        /// <exception cref="ArgumentNullException">Levée si
        /// <paramref name="useCaseInvoker"/>, <paramref name="seUseCase"/>,
        /// <paramref name="notification"/> ou
        /// <paramref name="barProfilSection"/> est <see langword="null"/>. Les
        /// gardes des trois premiers paramètres sont portées par
        /// <see cref="VM_Generic"/> via la chaîne <c>base(...)</c>.</exception>
        public VM_Page20(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app,
            IS_UseCaseInvoker useCaseInvoker,
            ISE_UseCase seUseCase,
            IS_Notification notification,
            ISE_BarProfilSection barProfilSection)
            : base(dictionary, logAndNotify, app)
        {
            _useCaseInvoker = useCaseInvoker
                ?? throw new ArgumentNullException(nameof(useCaseInvoker));
            _seUseCase = seUseCase
                ?? throw new ArgumentNullException(nameof(seUseCase));
            _notification = notification
                ?? throw new ArgumentNullException(nameof(notification));
            _barProfilSection = barProfilSection
                ?? throw new ArgumentNullException(nameof(barProfilSection));

            _profilSectionUri = _barProfilSection.DefaultBarProfilSectionUri;

            ReleaseBarCommand = new UT_RelayCommandArg1Async<int>(
                ReleaseBarAsync,
                _ => IsBlockedRegime);

            InitializeLabels();
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Exécute la séquence d'entrée de la page : réinitialise l'état,
        /// puis détermine la barre à présenter ou l'issue qui en tient lieu.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Invoquée par la vue à chaque montage, y compris
        /// après un rafraîchissement. Le ViewModel étant Singleton, l'état
        /// hérité d'un affichage antérieur est d'abord effacé.</para>
        /// <para>Issues :</para>
        /// <list type="bullet">
        ///   <item><description>Barre en cours non validée : la barre est
        ///   sélectionnée puis présentée.</description></item>
        ///   <item><description>Barre en cours validée : la barre est
        ///   sélectionnée et l'opérateur est conduit à la page de
        ///   découpe.</description></item>
        ///   <item><description>Barre nouvellement optimisée : la barre est
        ///   sélectionnée puis présentée.</description></item>
        ///   <item><description>Échec d'optimisation : retour à la page de
        ///   sélection des séries.</description></item>
        ///   <item><description>Rien d'optimisable et série inachevée : régime
        ///   bloqué, onglet « Ruptures » sélectionné, barres en rupture
        ///   chargées.</description></item>
        ///   <item><description>Rien d'optimisable et série achevée, ou échec
        ///   du statut d'achèvement : retour à la page de sélection des
        ///   séries.</description></item>
        ///   <item><description>Barre à présenter introuvable : avertissement
        ///   puis retour à la page de sélection des séries.</description></item>
        /// </list>
        /// <para>Les échecs des UseCases sont notifiés par ceux-ci ; le
        /// ViewModel n'ajoute aucune notification. Toute exception applicative
        /// est absorbée par <see cref="VM_Generic.ExecuteSafeAsync"/>.</para>
        /// </remarks>
        /// <param name="callChain">Chaîne d'appel transmise par la vue ; la
        /// méthode construit sa propre chaîne interne.</param>
        /// <param name="ct">Jeton d'annulation coopérative, propagé à toutes
        /// les invocations.</param>
        /// <returns>Tâche représentant l'exécution de la séquence
        /// d'entrée.</returns>
        public override async Task LoadAsync(
            string callChain,
            CancellationToken ct = default)
        {
            string innerCallChain = BuildFirstCallChain();

            await ExecuteSafeAsync(innerCallChain, async () =>
            {
                ResetState();

                int idSeries = _seUseCase.IdSeriesSelected;

                // Étape 1 - Barre en cours pour la série sélectionnée.
                ProductionBar? inProgressBar = await _useCaseInvoker
                    .InvokeAsync<IQ_Generic<ProductionBar>, ProductionBar?>(
                        (handler, innerCt) => handler.HandleGetFirstOrDefaultAsNoTrackingAsync(
                            innerCallChain,
                            b => b.IdProductionSeries == idSeries
                                && b.IsOptimizedTemp
                                && !b.IsUsed
                                && !b.IsDeleted
                                && !b.IsOutOfStock,
                            innerCt),
                        ct);

                if (inProgressBar is not null)
                {
                    _seUseCase.SelectBar(inProgressBar.Id);

                    if (inProgressBar.IsValidated)
                    {
                        await NavigateToAsync(innerCallChain, CuttingPageName, ct);
                        return;
                    }

                    await LoadPresentedBarAsync(innerCallChain, inProgressBar.Id, ct);
                    return;
                }

                // Étape 2 - Optimisation d'une nouvelle barre.
                int? optimizedBarId = await _useCaseInvoker
                    .InvokeAsync<IU_BarOptimization, int?>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            innerCallChain,
                            idSeries,
                            innerCt),
                        ct);

                if (optimizedBarId is > 0)
                {
                    _seUseCase.SelectBar(optimizedBarId.Value);
                    await LoadPresentedBarAsync(innerCallChain, optimizedBarId.Value, ct);
                    return;
                }

                if (optimizedBarId != 0)
                {
                    await NavigateToAsync(innerCallChain, SeriesSelectionPageName, ct);
                    return;
                }

                // Étape 3 - Statut d'achèvement de la série.
                bool? isCompleted = await _useCaseInvoker
                    .InvokeAsync<IU_ProductionSeries_CompleteCutting, bool?>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            innerCallChain,
                            idSeries,
                            innerCt),
                        ct);

                if (isCompleted == false)
                {
                    IsBlockedRegime = true;
                    SelectedTabIndex = OutOfStockTabIndex;
                    await LoadOutOfStockBarsAsync(innerCallChain, idSeries, ct);
                    return;
                }

                await NavigateToAsync(innerCallChain, SeriesSelectionPageName, ct);
            }, ct);
        }

        /// <summary>
        /// Valide la barre présentée, avec ou sans zones défectueuses, après
        /// contrôle de la cohérence de la saisie.
        /// </summary>
        /// <remarks>
        /// <para>Contrôles préalables, dans l'ordre ; le premier qui échoue
        /// interrompt la méthode sans invoquer le UseCase :</para>
        /// <list type="number">
        ///   <item><description>Aucune barre présentée : sortie
        ///   silencieuse.</description></item>
        ///   <item><description>Motif de refus sélectionné : avertissement
        ///   <c>No_Wa_16</c>.</description></item>
        ///   <item><description>Zone renseignée par une seule de ses deux
        ///   bornes, ou seconde zone sans première zone : avertissement
        ///   <c>No_Wa_17</c> et sélection de l'onglet
        ///   « Défauts ».</description></item>
        ///   <item><description>Borne négative, zone dont la fin ne suit pas
        ///   le début, ou seconde zone commençant avant la fin de la
        ///   première : avertissement <c>No_Wa_19</c> et sélection de
        ///   l'onglet « Défauts ». Deux zones contiguës sont
        ///   admises.</description></item>
        ///   <item><description>Borne supérieure à la longueur de la barre :
        ///   avertissement <c>No_Wa_20</c> et sélection de l'onglet
        ///   « Défauts ».</description></item>
        /// </list>
        /// <para>Ces contrôles reproduisent ceux du traitement de validation :
        /// une saisie incohérente est ainsi signalée sur la page au lieu
        /// d'être convertie en échec de traitement.</para>
        /// <para>Suites selon l'issue de la validation :</para>
        /// <list type="bullet">
        ///   <item><description>Barre scellée : navigation vers la page de
        ///   découpe.</description></item>
        ///   <item><description>Barre improductive après défauts : la barre
        ///   est désélectionnée et la page est rafraîchie pour présenter une
        ///   autre barre.</description></item>
        ///   <item><description>Échec, issue indéterminée ou toute autre
        ///   valeur : retour à la page de sélection des séries, l'échec étant
        ///   déjà notifié.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel de l'appelant, typiquement le
        /// menu horizontal de la page.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de validation.</returns>
        public Task ValidateAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(ValidateAsync)}";

            return ExecuteSafeAsync(callChain, async () =>
            {
                if (CurrentBar is not DTO_VwProductionBarFull bar)
                    return;

                if (SelectedRejectionReason is not null)
                {
                    _notification.Warning(callChain, "No_Wa_16", null, ct);
                    return;
                }

                int? start1 = DefectStart1;
                int? end1 = DefectEnd1;
                int? start2 = DefectStart2;
                int? end2 = DefectEnd2;

                if (start1.HasValue != end1.HasValue || start2.HasValue != end2.HasValue)
                {
                    WarnDefectInput(callChain, "No_Wa_17", ct);
                    return;
                }

                if (start2.HasValue && !start1.HasValue)
                {
                    WarnDefectInput(callChain, "No_Wa_17", ct);
                    return;
                }

                if (start1 < 0 || end1 < 0 || start2 < 0 || end2 < 0)
                {
                    WarnDefectInput(callChain, "No_Wa_19", ct);
                    return;
                }

                if (start1 >= end1)
                {
                    WarnDefectInput(callChain, "No_Wa_19", ct);
                    return;
                }

                if (start2.HasValue && (start2 >= end2 || start2 < end1))
                {
                    WarnDefectInput(callChain, "No_Wa_19", ct);
                    return;
                }

                int barLength = bar.PBBarLength;
                if (start1 > barLength || end1 > barLength || start2 > barLength || end2 > barLength)
                {
                    WarnDefectInput(callChain, "No_Wa_20", ct);
                    return;
                }

                En_BarValidationOutcome outcome = await _useCaseInvoker
                    .InvokeAsync<IU_BarValidation, En_BarValidationOutcome>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            callChain,
                            bar.PBId,
                            start1,
                            end1,
                            start2,
                            end2,
                            innerCt),
                        ct);

                switch (outcome)
                {
                    case En_BarValidationOutcome.BarSealed:
                        await NavigateToAsync(callChain, CuttingPageName, ct);
                        break;

                    case En_BarValidationOutcome.BarUnusable:
                        _seUseCase.ClearBar();
                        await RefreshPageAsync(callChain, ct);
                        break;

                    default:
                        await NavigateToAsync(callChain, SeriesSelectionPageName, ct);
                        break;
                }
            }, ct);
        }

        /// <summary>
        /// Refuse la barre présentée avec le motif choisi par l'opérateur.
        /// </summary>
        /// <remarks>
        /// <para>Sans barre présentée, la méthode sort silencieusement. Sans
        /// motif choisi, elle avertit l'opérateur (<c>No_Wa_18</c>) sans
        /// invoquer le traitement. Le motif est transmis sous la forme du nom
        /// du membre de <see cref="En_BarRejectionReason"/>, par exemple
        /// <c>BarDeformed</c>.</para>
        /// <para>Un refus réussi désélectionne la barre et rafraîchit la page
        /// pour présenter une autre barre ; un échec, déjà notifié, ramène à
        /// la page de sélection des séries.</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel de l'appelant, typiquement le
        /// menu horizontal de la page.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de refus.</returns>
        public Task RejectAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(RejectAsync)}";

            return ExecuteSafeAsync(callChain, async () =>
            {
                if (CurrentBar is not DTO_VwProductionBarFull bar)
                    return;

                if (SelectedRejectionReason is not En_BarRejectionReason reason)
                {
                    _notification.Warning(callChain, "No_Wa_18", null, ct);
                    return;
                }

                bool isRefused = await _useCaseInvoker
                    .InvokeAsync<IU_BarRefusal, bool>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            callChain,
                            bar.PBId,
                            reason.ToString(),
                            innerCt),
                        ct);

                if (isRefused)
                {
                    _seUseCase.ClearBar();
                    await RefreshPageAsync(callChain, ct);
                    return;
                }

                await NavigateToAsync(callChain, SeriesSelectionPageName, ct);
            }, ct);
        }

        /// <summary>
        /// Déclare en rupture de stock la barre neuve présentée.
        /// </summary>
        /// <remarks>
        /// <para>Sans barre présentée, ou lorsque la barre présentée est une
        /// chute, la méthode sort silencieusement. La confirmation de
        /// l'opérateur est recueillie en amont par le menu horizontal.</para>
        /// <para>Une déclaration réussie désélectionne la barre et rafraîchit
        /// la page, qui présente une autre barre ou bascule en régime
        /// bloqué ; un échec, déjà notifié, ramène à la page de sélection des
        /// séries.</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel de l'appelant, typiquement le
        /// menu horizontal de la page.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant l'opération de déclaration.</returns>
        public Task DeclareOutOfStockAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {nameof(DeclareOutOfStockAsync)}";

            return ExecuteSafeAsync(callChain, async () =>
            {
                if (CurrentBar is not { PBIsNewBar: true } bar)
                    return;

                bool isDeclared = await _useCaseInvoker
                    .InvokeAsync<IU_ProductionBar_SetOutOfStock, bool>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            callChain,
                            bar.PBId,
                            true,
                            innerCt),
                        ct);

                if (isDeclared)
                {
                    _seUseCase.ClearBar();
                    await RefreshPageAsync(callChain, ct);
                    return;
                }

                await NavigateToAsync(callChain, SeriesSelectionPageName, ct);
            }, ct);
        }

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Redéfinit le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> pour charger l'ensemble des
        /// libellés multilingues de la page <c>Page20</c> depuis le
        /// dictionnaire de langue actif.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode redéfinissant le point d'extension
        /// <see cref="VM_Generic.LoadLabels"/> conformément à R-4.11.8
        /// du 0231. Invoquée par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// pour le premier chargement, puis par le handler interne
        /// d'abonnement INPC de <see cref="VM_Generic"/> à chaque
        /// changement de langue dynamique notifié par
        /// <see cref="ISE_App.AppCultureCode"/>, avec marshalling
        /// Dispatcher défensif vers le thread UI.</para>
        /// <para>Objectif : Maintenir tous les libellés de la page
        /// synchronisés avec la langue active :</para>
        /// <list type="bullet">
        ///   <item><description><see cref="PageName"/> depuis la clé
        ///   <c>P20_00</c> ;</description></item>
        ///   <item><description>les libellés des onglets, de la fiche et de
        ///   la saisie des défauts, clés <c>P20_01</c> à
        ///   <c>P20_23</c>, dont la clé <c>P20_22</c> qui porte non un
        ///   intitulé mais le nom du magasin de profilés
        ///   neufs ;</description></item>
        ///   <item><description>le cache privé des libellés des huit motifs
        ///   de refus sélectionnables, clés <c>P20_30</c> à
        ///   <c>P20_37</c> ;</description></item>
        ///   <item><description>les en-têtes de colonnes des onglets
        ///   « Découpes » et « Ruptures », clés <c>P20_39</c> à
        ///   <c>P20_56</c>.</description></item>
        /// </list>
        /// <para>Lorsqu'une barre est présentée, le libellé d'origine,
        /// l'emplacement d'origine et la liste des motifs sont recomposés dans
        /// la nouvelle langue ; le motif déjà choisi est conservé. La
        /// recomposition de <see cref="SourceLocation"/> suit l'affectation de
        /// <see cref="Label_P20_22"/>, dont elle dépend pour une barre
        /// neuve ; sur une barre de chute, la valeur recomposée est
        /// l'emplacement de stock, indépendant de la langue.</para>
        /// <para>Absence d'appel à <c>base.LoadLabels(callChain)</c> :
        /// L'implémentation par défaut de
        /// <see cref="VM_Generic.LoadLabels"/> ne porte aucun
        /// traitement. L'appel à <c>base.LoadLabels(callChain)</c>
        /// n'apporterait qu'un bruit inutile et est délibérément omis,
        /// conformément à la pratique standard d'override lorsque la
        /// base ne porte aucun traitement.</para>
        /// <para>Filet de sécurité : Aucun try/catch local n'est posé.
        /// Le filet est porté exclusivement par <c>SR_Dictionary</c>
        /// conformément à R-4.11.6 et R-4.11.10 du 0231 — toute
        /// anomalie (clé absente, erreur inattendue) est journalisée en
        /// interne par <c>SR_Dictionary</c> et résolue par la valeur de
        /// repli <c>[clé] not found</c>, sans interruption ni
        /// propagation d'exception au présent ViewModel. L'unique
        /// exception susceptible d'être propagée serait
        /// <see cref="OperationCanceledException"/>, structurellement
        /// impossible ici puisque <c>IS_Dictionary.GetText</c> est
        /// invoquée sans
        /// <see cref="System.Threading.CancellationToken"/> explicite
        /// (paramètre optionnel par défaut <c>default</c>, équivalent
        /// à <see cref="System.Threading.CancellationToken.None"/>).</para>
        /// </remarks>
        /// <param name="caller">CallChain construite par
        /// <see cref="VM_Generic.InitializeLabels"/> au constructeur
        /// (premier chargement) ou par le handler interne d'abonnement
        /// INPC de <see cref="VM_Generic"/> au changement de langue
        /// dynamique (rechargement), et transmise au service de
        /// dictionnaire pour traçabilité.</param>
        protected override void LoadLabels(string caller)
        {
            string callChain = $"{caller} > {nameof(LoadLabels)}";

            PageName = _dictionary.GetText(callChain, "P20_00");
            Label_P20_01 = _dictionary.GetText(callChain, "P20_01");
            Label_P20_02 = _dictionary.GetText(callChain, "P20_02");
            Label_P20_03 = _dictionary.GetText(callChain, "P20_03");
            Label_P20_04 = _dictionary.GetText(callChain, "P20_04");
            Label_P20_05 = _dictionary.GetText(callChain, "P20_05");
            Label_P20_06 = _dictionary.GetText(callChain, "P20_06");
            Label_P20_07 = _dictionary.GetText(callChain, "P20_07");
            Label_P20_08 = _dictionary.GetText(callChain, "P20_08");
            Label_P20_09 = _dictionary.GetText(callChain, "P20_09");
            Label_P20_10 = _dictionary.GetText(callChain, "P20_10");
            Label_P20_11 = _dictionary.GetText(callChain, "P20_11");
            Label_P20_12 = _dictionary.GetText(callChain, "P20_12");
            Label_P20_13 = _dictionary.GetText(callChain, "P20_13");
            Label_P20_14 = _dictionary.GetText(callChain, "P20_14");
            Label_P20_15 = _dictionary.GetText(callChain, "P20_15");
            Label_P20_16 = _dictionary.GetText(callChain, "P20_16");
            Label_P20_17 = _dictionary.GetText(callChain, "P20_17");
            Label_P20_18 = _dictionary.GetText(callChain, "P20_18");
            Label_P20_19 = _dictionary.GetText(callChain, "P20_19");
            Label_P20_20 = _dictionary.GetText(callChain, "P20_20");
            Label_P20_21 = _dictionary.GetText(callChain, "P20_21");
            Label_P20_22 = _dictionary.GetText(callChain, "P20_22");
            Label_P20_23 = _dictionary.GetText(callChain, "P20_23");

            _rejectionReasonLabels["P20_30"] = _dictionary.GetText(callChain, "P20_30");
            _rejectionReasonLabels["P20_31"] = _dictionary.GetText(callChain, "P20_31");
            _rejectionReasonLabels["P20_32"] = _dictionary.GetText(callChain, "P20_32");
            _rejectionReasonLabels["P20_33"] = _dictionary.GetText(callChain, "P20_33");
            _rejectionReasonLabels["P20_34"] = _dictionary.GetText(callChain, "P20_34");
            _rejectionReasonLabels["P20_35"] = _dictionary.GetText(callChain, "P20_35");
            _rejectionReasonLabels["P20_36"] = _dictionary.GetText(callChain, "P20_36");
            _rejectionReasonLabels["P20_37"] = _dictionary.GetText(callChain, "P20_37");

            Label_P20_39 = _dictionary.GetText(callChain, "P20_39");
            Label_P20_40 = _dictionary.GetText(callChain, "P20_40");
            Label_P20_41 = _dictionary.GetText(callChain, "P20_41");
            Label_P20_42 = _dictionary.GetText(callChain, "P20_42");
            Label_P20_43 = _dictionary.GetText(callChain, "P20_43");
            Label_P20_44 = _dictionary.GetText(callChain, "P20_44");
            Label_P20_45 = _dictionary.GetText(callChain, "P20_45");
            Label_P20_46 = _dictionary.GetText(callChain, "P20_46");
            Label_P20_47 = _dictionary.GetText(callChain, "P20_47");
            Label_P20_48 = _dictionary.GetText(callChain, "P20_48");
            Label_P20_49 = _dictionary.GetText(callChain, "P20_49");
            Label_P20_50 = _dictionary.GetText(callChain, "P20_50");
            Label_P20_51 = _dictionary.GetText(callChain, "P20_51");
            Label_P20_52 = _dictionary.GetText(callChain, "P20_52");
            Label_P20_53 = _dictionary.GetText(callChain, "P20_53");
            Label_P20_54 = _dictionary.GetText(callChain, "P20_54");
            Label_P20_55 = _dictionary.GetText(callChain, "P20_55");
            Label_P20_56 = _dictionary.GetText(callChain, "P20_56");

            if (CurrentBar is DTO_VwProductionBarFull bar)
            {
                OriginLabel = bar.PBIsNewBar ? Label_P20_14 : Label_P20_15;
                SourceLocation = bar.PBIsNewBar
                    ? Label_P20_22
                    : (bar.CSLScrapLocationSource ?? string.Empty);

                En_BarRejectionReason? selectedReason = SelectedRejectionReason;
                RejectionReasons = BuildRejectionReasons(bar.PBIsNewBar);
                _selectedRejectionReason = selectedReason;
                OnPropertyChanged(nameof(SelectedRejectionReason));
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Ramène l'état présenté par la page à ses valeurs par défaut.
        /// </summary>
        /// <remarks>
        /// <para>Première instruction de la séquence d'entrée : aucun état
        /// ne survit d'un affichage à l'autre. Les deux régimes sont remis à
        /// faux, les gardes du contrat étant notifiées en
        /// conséquence.</para>
        /// </remarks>
        private void ResetState()
        {
            CurrentBar = null;
            OriginLabel = string.Empty;
            SourceLocation = string.Empty;
            ProfilSectionUri = _barProfilSection.DefaultBarProfilSectionUri;
            RejectionReasons = Array.Empty<KeyValuePair<En_BarRejectionReason, string>>();
            SelectedRejectionReason = null;
            DefectStart1 = null;
            DefectEnd1 = null;
            DefectStart2 = null;
            DefectEnd2 = null;
            CutPlan.Clear();
            OutOfStockBars.Clear();
            IsBarPresented = false;
            IsBlockedRegime = false;
            SelectedTabIndex = BarTabIndex;
        }

        /// <summary>
        /// Charge et présente la barre désignée, avec sa fiche, son image de
        /// section, ses motifs de refus et son plan de coupe.
        /// </summary>
        /// <remarks>
        /// <para>Si la barre est introuvable, elle est désélectionnée,
        /// l'opérateur est averti (<c>No_Wa_21</c>) et reconduit à la page de
        /// sélection des séries. Sinon, le plan de coupe est trié par position
        /// croissante, les découpes sans position en fin de liste, puis la
        /// page passe en régime « barre présentée » sur l'onglet
        /// « Barre ».</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel de l'appelant.</param>
        /// <param name="idBar">Identifiant de la barre à présenter.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant le chargement.</returns>
        private async Task LoadPresentedBarAsync(string caller, int idBar, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(LoadPresentedBarAsync)}";

            DTO_VwProductionBarFull? presentedBar = await _useCaseInvoker
                .InvokeAsync<IQ_VwProductionBarFull, DTO_VwProductionBarFull?>(
                    (handler, innerCt) => handler.HandleGetByProductionBarIdAsNoTrackingAsync(
                        callChain,
                        idBar,
                        innerCt),
                    ct);

            if (presentedBar is null)
            {
                _seUseCase.ClearBar();
                _notification.Warning(callChain, "No_Wa_21", null, ct);
                await NavigateToAsync(callChain, SeriesSelectionPageName, ct);
                return;
            }

            CurrentBar = presentedBar;
            ProfilSectionUri = _barProfilSection.GetBarProfilSectionUriOrDefault(presentedBar.ARReference);
            OriginLabel = presentedBar.PBIsNewBar ? Label_P20_14 : Label_P20_15;
            SourceLocation = presentedBar.PBIsNewBar
                ? Label_P20_22
                : (presentedBar.CSLScrapLocationSource ?? string.Empty);
            RejectionReasons = BuildRejectionReasons(presentedBar.PBIsNewBar);

            List<DTO_VwProductionCutPieceFull_P11> cutPieces = await _useCaseInvoker
                .InvokeAsync<IQ_VwProductionCutPieceFull, List<DTO_VwProductionCutPieceFull_P11>>(
                    (handler, innerCt) => handler.HandleGetByProductionBarIdForP20AsNoTrackingAsync(
                        callChain,
                        idBar,
                        innerCt),
                    ct);

            List<DTO_VwProductionCutPieceFull_P11> sortedCutPieces = cutPieces
                .OrderBy(c => c.PCPCutPositionInBar is null)
                .ThenBy(c => c.PCPCutPositionInBar)
                .ToList();

            CutPlan.Clear();
            foreach (var cutPiece in sortedCutPieces) CutPlan.Add(cutPiece);

            IsBarPresented = true;
            SelectedTabIndex = BarTabIndex;
        }

        /// <summary>
        /// Charge la liste des barres en rupture de stock de la série, dans
        /// l'ordre fourni par la lecture.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant.</param>
        /// <param name="idSeries">Identifiant de la série sélectionnée.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant le chargement.</returns>
        private async Task LoadOutOfStockBarsAsync(string caller, int idSeries, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(LoadOutOfStockBarsAsync)}";

            List<DTO_VwProductionBarFull> bars = await _useCaseInvoker
                .InvokeAsync<IQ_VwProductionBarFull, List<DTO_VwProductionBarFull>>(
                    (handler, innerCt) => handler.HandleGetOutOfStockByProductionSeriesIdAsNoTrackingAsync(
                        callChain,
                        idSeries,
                        innerCt),
                    ct);

            OutOfStockBars.Clear();
            foreach (var bar in bars) OutOfStockBars.Add(bar);
        }

        /// <summary>
        /// Compose la liste des motifs de refus applicables à l'origine de la
        /// barre, à partir des libellés mis en cache par
        /// <see cref="LoadLabels"/>.
        /// </summary>
        /// <param name="isNewBar"><see langword="true"/> pour une barre
        /// neuve, qui n'admet que les motifs de la famille Qualité ;
        /// <see langword="false"/> pour une chute, qui admet aussi les motifs
        /// de la famille Écart de stock.</param>
        /// <returns>Liste ordonnée des motifs applicables, chacun associé à
        /// son libellé localisé.</returns>
        private IReadOnlyList<KeyValuePair<En_BarRejectionReason, string>> BuildRejectionReasons(bool isNewBar)
        {
            int upperBound = isNewBar ? NewBarReasonUpperBound : ScrapReasonUpperBound;

            return RejectionReasonKeys
                .Where(entry => (int)entry.Key < upperBound)
                .Select(entry => new KeyValuePair<En_BarRejectionReason, string>(
                    entry.Key,
                    _rejectionReasonLabels.TryGetValue(entry.Value, out string? label) ? label : string.Empty))
                .ToList();
        }

        /// <summary>
        /// Libère une barre en rupture de stock ; exécutée par
        /// <see cref="ReleaseBarCommand"/>.
        /// </summary>
        /// <remarks>
        /// <para>Une libération réussie rafraîchit la page, qui rejoue sa
        /// séquence d'entrée ; aucune barre n'étant sélectionnée en régime
        /// bloqué, aucune désélection n'est nécessaire. Un échec, déjà
        /// notifié, ramène à la page de sélection des séries.</para>
        /// </remarks>
        /// <param name="idBar">Identifiant de la barre à libérer.</param>
        /// <returns>Tâche représentant l'opération de libération.</returns>
        private Task ReleaseBarAsync(int idBar)
        {
            string callChain = BuildFirstCallChain();

            return ExecuteSafeAsync(callChain, async () =>
            {
                bool isReleased = await _useCaseInvoker
                    .InvokeAsync<IU_ProductionBar_SetOutOfStock, bool>(
                        (useCase, innerCt) => useCase.ExecuteAsync(
                            callChain,
                            idBar,
                            false,
                            innerCt),
                        CancellationToken.None);

                if (isReleased)
                {
                    await RefreshPageAsync(callChain, CancellationToken.None);
                    return;
                }

                await NavigateToAsync(callChain, SeriesSelectionPageName, CancellationToken.None);
            }, CancellationToken.None);
        }

        /// <summary>
        /// Avertit l'opérateur d'une saisie de défauts à corriger et
        /// sélectionne l'onglet « Défauts ».
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant.</param>
        /// <param name="messageKey">Clé du message d'avertissement.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        private void WarnDefectInput(string caller, string messageKey, CancellationToken ct)
        {
            _notification.Warning(caller, messageKey, null, ct);
            SelectedTabIndex = DefectsTabIndex;
        }

        /// <summary>
        /// Notifie le changement des trois gardes du contrat
        /// <see cref="IV_Page20"/>, afin que le menu horizontal réévalue ses
        /// commandes.
        /// </summary>
        private void RaiseGuardsChanged()
        {
            OnPropertyChanged(nameof(CanValidate));
            OnPropertyChanged(nameof(CanReject));
            OnPropertyChanged(nameof(CanDeclareOutOfStock));
        }

        /// <summary>
        /// Demande la navigation vers une page par l'intermédiaire du
        /// UseCase de navigation.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant.</param>
        /// <param name="pageName">Nom logique de la page cible.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant la demande de navigation.</returns>
        private Task NavigateToAsync(string caller, string pageName, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(NavigateToAsync)}";

            return _useCaseInvoker.InvokeAsync<IU_Navigation>(
                (navigation, innerCt) => navigation.NavigateToPageAsync(
                    callChain,
                    pageName,
                    innerCt),
                ct);
        }

        /// <summary>
        /// Demande le rafraîchissement de la page courante par
        /// l'intermédiaire du UseCase de navigation ; la page reconstruite
        /// rejoue sa séquence d'entrée.
        /// </summary>
        /// <param name="caller">Chaîne d'appel de l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <returns>Tâche représentant la demande de rafraîchissement.</returns>
        private Task RefreshPageAsync(string caller, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(RefreshPageAsync)}";

            return _useCaseInvoker.InvokeAsync<IU_Navigation>(
                (navigation, innerCt) => navigation.RefreshCurrentPageAsync(
                    callChain,
                    innerCt),
                ct);
        }

        #endregion
    }
}