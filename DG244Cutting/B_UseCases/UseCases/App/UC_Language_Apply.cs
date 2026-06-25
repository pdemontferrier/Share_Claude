using System.Globalization;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase orchestrateur du changement de langue de l'application
    /// DG244Cutting pour la session courante.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette classe est une dépendance injectable enregistrée
    /// en Singleton et consommée via <see cref="IU_Language_Apply"/>. Elle
    /// réside dans <c>B_UseCases/UseCases/App/</c>, couche d'orchestration
    /// applicative. Le composant remplace fonctionnellement le service de
    /// présentation <c>SR_Language</c> de la couche <c>D_Presentation</c>, en
    /// transposant la responsabilité d'orchestration vers la couche
    /// <c>B_UseCases</c> avec adoption du filet terminal
    /// <see cref="IU_LogAndNotify"/> au lieu de la requalification via
    /// <c>IS_ExClassifier</c>.</para>
    ///
    /// <para>Objectif : Constituer le point d'entrée unique et robuste pour
    /// l'application d'une langue à l'interface de l'application. Le UseCase
    /// coordonne dans un ordre déterministe les quatre sous-étapes d'effet
    /// (chargement du dictionnaire XAML, persistance du code culture,
    /// synchronisation du drapeau, synchronisation des cibles
    /// <see cref="System.Globalization.CultureInfo"/> .NET), précédées d'une
    /// validation défensive du code culture et d'un court-circuit
    /// d'idempotence si la langue demandée est déjà active.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Valider que le code culture reçu est non
    ///   <see langword="null"/>, non vide et non composé uniquement d'espaces
    ///   blancs (levée d'<see cref="Ex_Business"/> au code
    ///   <c>BU_ER_01</c>).</item>
    ///   <item>Court-circuiter par idempotence si le code culture demandé est
    ///   déjà le code culture actif (<see cref="ISE_App.AppCultureCode"/>),
    ///   comparaison ordinale insensible à la casse.</item>
    ///   <item>Résoudre et charger le dictionnaire XAML correspondant via
    ///   <see cref="ISE_Language"/>.</item>
    ///   <item>Persister le code culture actif dans
    ///   <see cref="ISE_App.AppCultureCode"/>, déclenchant la cascade INPC
    ///   vers le rechargement des labels côté Presentation.</item>
    ///   <item>Synchroniser l'URI du drapeau associé à la langue via
    ///   <see cref="ISE_Flag"/>.</item>
    ///   <item>Synchroniser les quatre cibles standard de
    ///   <see cref="System.Globalization.CultureInfo"/> .NET
    ///   (<c>DefaultThreadCurrentCulture</c>,
    ///   <c>DefaultThreadCurrentUICulture</c>,
    ///   <c>Thread.CurrentThread.CurrentCulture</c>,
    ///   <c>Thread.CurrentThread.CurrentUICulture</c>).</item>
    ///   <item>Propager la <c>CallChain</c> à chaque composant appelé.</item>
    ///   <item>Assurer le traitement terminal des erreurs via
    ///   <see cref="IU_LogAndNotify"/> selon le patron de trois catch typés
    ///   (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
    ///   <see cref="Ex_Unclassified"/>) avec propagation distincte de
    ///   <see cref="OperationCanceledException"/>.</item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>N'interagit jamais directement avec
    ///   <c>ResourceDictionary</c>, <c>Application.Current.Resources</c> ou
    ///   tout type WPF — ce rôle appartient à <see cref="ISE_Language"/>.</item>
    ///   <item>Ne connaît pas les détails de la couche de persistance — le
    ///   changement de langue n'est pas un scénario d'écriture transactionnel
    ///   en base de données.</item>
    ///   <item>Ne classifie pas les exceptions via <c>IS_ExClassifier</c> :
    ///   le patron de trois catch typés couvre nativement les trois familles
    ///   d'exceptions applicatives, et le traitement terminal est porté par
    ///   <see cref="IU_LogAndNotify"/>.</item>
    ///   <item>Ne décide pas de la langue à appliquer — cette responsabilité
    ///   appartient à l'appelant (UseCase orchestrateur de séquence de
    ///   démarrage ou ViewModel de sélection de langue).</item>
    /// </list>
    ///
    /// <para>Note sur la transactionnalité : Le changement de langue ne
    /// constitue pas un scénario d'écriture en base de données. Ce UseCase
    /// n'ouvre donc pas de transaction SQL (aucune injection
    /// d'<c>IDbContextFactory</c>, aucun appel à <c>BeginTransactionAsync</c>,
    /// <c>SaveChangesAsync</c>, <c>CommitAsync</c> ou <c>RollbackAsync</c>).
    /// Il s'inscrit dans l'architecture UseCase par son rôle d'orchestration
    /// d'une mécanique transverse de présentation et de runtime, et par son
    /// traitement terminal des erreurs via <see cref="IU_LogAndNotify"/>,
    /// conformément aux principes définis dans la partie 3 du référentiel.</para>
    ///
    /// <para>Note sur la convention de méthode publique : Ce UseCase expose
    /// une méthode publique unique <see cref="ExecuteAsync"/>, conformément à
    /// la configuration nominale du cas Concept au sens de la convention de
    /// nommage UC_ dual-cas Entité / Concept (R-4.14.7 amendée). Le préfixe
    /// canonique <c>ExecuteAsync</c> est conservé sans dérogation : la
    /// sémantique du concept porté ne justifie pas la substitution par un
    /// verbe d'action plus précis admise par la double dérogation
    /// typologiquement bornée de R-4.2.13. Aucune trace de dérogation n'est
    /// par conséquent à porter dans le <c>&lt;remarks&gt;</c> de la méthode
    /// publique.</para>
    ///
    /// <para>Note sur la CallChain : La <c>CallChain</c> est construite en
    /// première instruction effective de la méthode publique
    /// <see cref="ExecuteAsync"/> selon le patron normatif
    /// <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> (R-4.5.1) et
    /// propagée en aval vers <see cref="IU_LogAndNotify"/> dans chacun des
    /// trois catch typés.</para>
    ///
    /// <para>Note sur la propagation du <c>CancellationToken</c> : Les sous-
    /// étapes d'effet 1/4 à 4/4 sont synchrones (chargement de dictionnaire
    /// WPF, affectations INPC, lecture statique de
    /// <see cref="System.Globalization.CultureInfo.GetCultureInfo(string)"/>).
    /// Le <paramref name="ct"/> reçu n'est consommé qu'en deux points
    /// coopératifs : l'appel explicite à
    /// <see cref="CancellationToken.ThrowIfCancellationRequested"/> en
    /// sous-étape 2.b, et la propagation à <see cref="IU_LogAndNotify"/> dans
    /// les trois catch typés. Aucune sous-étape d'effet ne propage le <c>ct</c>
    /// puisque les contrats des dépendances injectées
    /// (<see cref="ISE_Language"/>, <see cref="ISE_Flag"/>,
    /// <see cref="ISE_App"/>) sont synchrones.</para>
    /// <seealso cref="IU_Language_Apply"/>
    /// </remarks>
    public class UC_Language_Apply : IU_Language_Apply
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe pour la construction de la <c>CallChain</c>.
        /// Initialisé via <c>GetType().Name</c> dans le constructeur.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Setting applicatif global - source d'autorité du code culture
        /// actif. Mutation déclenchant la cascade INPC vers le rechargement
        /// des labels côté Presentation.
        /// </summary>
        private readonly ISE_App _seApp;

        /// <summary>
        /// Setting de présentation - résolution de l'URI du dictionnaire
        /// XAML et chargement dans les ressources WPF.
        /// </summary>
        private readonly ISE_Language _seLanguage;

        /// <summary>
        /// Setting de présentation - résolution de l'URI du drapeau associé
        /// au code langue et persistance dans l'état runtime.
        /// </summary>
        private readonly ISE_Flag _seFlag;

        /// <summary>
        /// Orchestrateur de traitement terminal des erreurs applicatives -
        /// journalise et notifie les exceptions typées capturées.
        /// </summary>
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le UseCase de changement de langue avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Ce constructeur est appelé par le conteneur
        /// d'injection lors de la création du Singleton.</para>
        /// <para>Objectif : Initialiser <c>_callee</c> dynamiquement et
        /// stocker les dépendances après contrôle de non-nullité. Aucune
        /// logique d'orchestration ni aucun accès aux Settings ne doit figurer
        /// dans ce constructeur.</para>
        /// </remarks>
        /// <param name="seApp">
        /// Setting applicatif global - persistance du code culture actif.
        /// </param>
        /// <param name="seLanguage">
        /// Setting de présentation - résolution et chargement du dictionnaire
        /// XAML.
        /// </param>
        /// <param name="seFlag">
        /// Setting de présentation - résolution et persistance de l'URI du
        /// drapeau.
        /// </param>
        /// <param name="logAndNotify">
        /// Orchestrateur de traitement terminal des erreurs applicatives.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public UC_Language_Apply(
            ISE_App seApp,
            ISE_Language seLanguage,
            ISE_Flag seFlag,
            IU_LogAndNotify logAndNotify)
        {
            _seApp = seApp ?? throw new ArgumentNullException(nameof(seApp));
            _seLanguage = seLanguage ?? throw new ArgumentNullException(nameof(seLanguage));
            _seFlag = seFlag ?? throw new ArgumentNullException(nameof(seFlag));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Séquence interne :</para>
        /// <list type="number">
        ///   <item>Construction de la <c>CallChain</c> au format normatif
        ///   <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c>
        ///   (R-4.5.1).</item>
        ///   <item>Validation défensive de <paramref name="cultureCode"/>
        ///   dans le bloc <c>try</c>, en tête, avant l'appel à
        ///   <see cref="CancellationToken.ThrowIfCancellationRequested"/> :
        ///   levée d'<see cref="Ex_Business"/> au code <c>BU_ER_01</c>
        ///   (paramètre obligatoire manquant - cf. §21.1 du référentiel et
        ///   couverture nominative de la précondition
        ///   <c>string.IsNullOrWhiteSpace</c>) si le code culture est
        ///   <see langword="null"/>, vide ou composé uniquement d'espaces.</item>
        ///   <item>Vérification de l'annulation coopérative via
        ///   <see cref="CancellationToken.ThrowIfCancellationRequested"/>.</item>
        ///   <item>Court-circuit d'idempotence : retour immédiat
        ///   <see langword="true"/> sans effet si
        ///   <paramref name="cultureCode"/> est déjà le code culture actif
        ///   (<see cref="ISE_App.AppCultureCode"/>), comparé en
        ///   <see cref="StringComparison.OrdinalIgnoreCase"/>.</item>
        ///   <item>Sous-étape 1/4 - Résolution de l'URI du dictionnaire XAML
        ///   via <see cref="ISE_Language.GetDictionaryUri(string)"/> et
        ///   chargement via
        ///   <see cref="ISE_Language.LoadDictionary(System.Uri)"/>.</item>
        ///   <item>Sous-étape 2/4 - Persistance du code culture dans
        ///   <see cref="ISE_App.AppCultureCode"/>, déclenchant la cascade
        ///   INPC vers le rechargement des labels côté Presentation.</item>
        ///   <item>Sous-étape 3/4 - Extraction du code pays en majuscules
        ///   via <see cref="ExtractCountryCode(string)"/>, résolution de
        ///   l'URI du drapeau via
        ///   <see cref="ISE_Flag.GetFlagUriOrDefault(string)"/> et affectation
        ///   à <see cref="ISE_Flag.AppFlagUri"/>.</item>
        ///   <item>Sous-étape 4/4 - Synchronisation des quatre cibles standard
        ///   de <see cref="System.Globalization.CultureInfo"/> .NET
        ///   (<c>DefaultThreadCurrentCulture</c>,
        ///   <c>DefaultThreadCurrentUICulture</c>,
        ///   <c>Thread.CurrentThread.CurrentCulture</c>,
        ///   <c>Thread.CurrentThread.CurrentUICulture</c>).</item>
        ///   <item>Retour <see langword="true"/> en clôture du chemin nominal
        ///   après la quatrième sous-étape d'effet.</item>
        /// </list>
        /// <para>Comportement en cas d'erreur : Les trois familles
        /// d'exceptions applicatives (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>)
        /// sont captées terminalement et déléguées au pipeline terminal
        /// <see cref="IU_LogAndNotify"/> avec les clés dictionnaire
        /// <c>La_EC_01</c>, <c>La_EC_02</c> et <c>La_EC_03</c> respectivement
        /// et <c>notify: true</c> (cohérence avec le point d'appel actuel
        /// <c>UC_Application_OnStart</c>). Chacun des trois catch typés
        /// retourne <see langword="false"/> après délégation à
        /// <see cref="IU_LogAndNotify"/>, conformément à la doctrine de
        /// chaîne UC → UC normalisée (R-4.14.21) qui prescrit le signalement
        /// par valeur de retour à l'orchestrateur amont sans propagation
        /// d'exception applicative typée entre UseCases orchestrants.
        /// L'<see cref="OperationCanceledException"/> est positionnée en
        /// dernière position du patron de catch et propagée intacte à
        /// l'appelant conformément à la doctrine d'annulation coopérative
        /// §4.6.</para>
        /// </remarks>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est
        /// déclenché pendant l'opération, conformément à la doctrine
        /// d'annulation coopérative §4.6 du référentiel.
        /// </exception>
        public async Task<bool> ExecuteAsync(string caller, string cultureCode, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 2.a - Validation défensive (BU_ER_01, R-4.7.25)
                if (string.IsNullOrWhiteSpace(cultureCode))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le code culture fourni est null, vide ou composé uniquement d'espaces.");

                // Étape 2.b - Vérification de l'annulation coopérative
                ct.ThrowIfCancellationRequested();

                // Étape 2.c - Court-circuit d'idempotence
                if (string.Equals(cultureCode, _seApp.AppCultureCode, StringComparison.OrdinalIgnoreCase))
                    return true;

                // Étape 2.d - Sous-étape d'effet 1/4 - Chargement du dictionnaire XAML
                Uri dictionaryUri = _seLanguage.GetDictionaryUri(cultureCode);
                _seLanguage.LoadDictionary(dictionaryUri);

                // Étape 2.e - Sous-étape d'effet 2/4 - Persistance du code culture (cascade INPC)
                _seApp.AppCultureCode = cultureCode;

                // Étape 2.f - Sous-étape d'effet 3/4 - Synchronisation de l'URI du drapeau
                string countryCode = ExtractCountryCode(cultureCode);
                _seFlag.AppFlagUri = _seFlag.GetFlagUriOrDefault(countryCode);

                // Étape 2.g - Sous-étape d'effet 4/4 - Synchronisation CultureInfo .NET (4 cibles)
                CultureInfo ci = CultureInfo.GetCultureInfo(cultureCode);
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;

                return true;
            }
            catch (Ex_Business ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct); return false; }
            catch (Ex_Infrastructure ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct); return false; }
            catch (Ex_Unclassified ex) { await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct); return false; }
            catch (OperationCanceledException) { throw; }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Extrait le code pays en majuscules à partir d'un code culture
        /// (par exemple <c>"fr-FR"</c> → <c>"FR"</c>).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode utilitaire consommée par la sous-étape
        /// 2.f de <see cref="ExecuteAsync"/> pour convertir le code culture
        /// en code pays compatible avec le référentiel de drapeaux de
        /// <see cref="ISE_Flag"/>.</para>
        /// <para>Objectif : Isoler la logique d'extraction pour améliorer la
        /// lisibilité de la méthode publique et faciliter les tests unitaires
        /// indépendants.</para>
        /// <para>Comportement : Si le code culture ne contient pas de
        /// séparateur <c>'-'</c>, la chaîne entière est retournée en
        /// majuscules.</para>
        /// </remarks>
        /// <param name="cultureCode">
        /// Code culture source. Doit être non <see langword="null"/> (validé
        /// en amont par <see cref="ExecuteAsync"/>).
        /// </param>
        /// <returns>
        /// Code pays en majuscules, extrait après le premier séparateur
        /// <c>'-'</c>, ou code culture complet en majuscules si aucun
        /// séparateur n'est trouvé.
        /// </returns>
        private static string ExtractCountryCode(string cultureCode)
        {
            int index = cultureCode.IndexOf('-');
            string countryCode = index >= 0 ? cultureCode[(index + 1)..] : cultureCode;
            return countryCode.ToUpperInvariant();
        }

        #endregion
    }
}