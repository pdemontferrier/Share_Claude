using System.Diagnostics;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase responsable de la gestion centralisée de la journalisation et de la notification
    /// des erreurs applicatives, conformément à la section 4.6.4 du référentiel normatif.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Ce UseCase est appelé exclusivement depuis les UseCases (<c>UC_*</c>) dans
    /// leurs blocs <c>catch</c>, après réception d'une exception typée du projet
    /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
    /// Il peut également être appelé depuis <c>VM_Page_Generic.ExecuteSafeAsync</c> qui agit comme
    /// filet de sécurité de dernier recours au niveau des ViewModels.
    /// </para>
    /// <para>
    /// Objectif : Fournir un point d'entrée unique et robuste pour le traitement terminal
    /// des erreurs, garantissant que journal et notification reçoivent des données cohérentes
    /// construites dans une séquence déterministe et résistante aux défaillances internes.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Construire le détail technique à partir des propriétés de l'exception typée.</description></item>
    ///   <item><description>Traduire la clé dictionnaire en message destiné à l'opérateur.</description></item>
    ///   <item><description>Déléguer la journalisation technique à <see cref="IS_ErrorLogger"/>.</description></item>
    ///   <item><description>Déléguer la notification opérateur à <see cref="IS_Notification"/>.</description></item>
    ///   <item><description>Garantir le principe best-effort : ne jamais interrompre le flux appelant.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne classifie pas les exceptions : ce rôle appartient à <see cref="IS_ExClassifier"/>.</description></item>
    ///   <item><description>Ne construit pas la CallChain : cette responsabilité appartient à chaque composant.</description></item>
    ///   <item><description>Ne persiste pas directement les logs : cette responsabilité est déléguée à <see cref="IS_ErrorLogger"/>.</description></item>
    /// </list>
    /// <para>Comportement vis-à-vis du CancellationToken :</para>
    /// <para>
    /// Le <see cref="CancellationToken"/> reçu par <see cref="ExecuteAsync"/> est ignoré
    /// activement dans toutes les opérations internes. Une erreur qui atteint ce UseCase
    /// mérite d'être journalisée et signalée, quel que soit l'état d'annulation en cours.
    /// Tous les appels internes utilisent <see cref="CancellationToken.None"/> pour garantir
    /// que la journalisation et la notification ne soient jamais interrompues par un signal
    /// d'annulation concomitant à la défaillance.
    /// </para>
    /// </remarks>
    public class UC_LogAndNotify : IU_LogAndNotify
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom dynamique de la classe courante, utilisé dans la construction de la CallChain
        /// conformément aux règles de la section 4.5.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de fourniture du contexte applicatif courant (identité utilisateur, poste, adresse IP,
        /// horodatage), utilisé pour enrichir chaque entrée de log.
        /// </summary>
        private readonly IS_AppContext _appContext;

        /// <summary>
        /// Service transverse de journalisation des erreurs, invoqué de manière autonome et
        /// non bloquante lors des anomalies de dictionnaire (clé absente ou erreur inattendue).
        /// </summary>
        private readonly IS_ErrorLogger _errorLogger;

        private readonly IS_Notification _notification;
        private readonly IS_Dictionary _dictionary;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_LogAndNotify"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Instanciation via le conteneur d'injection de dépendances en Singleton.
        /// </para>
        /// </remarks>
        /// <param name="errorLogger">Service de journalisation technique des erreurs.</param>
        /// <param name="notification">Service de notification opérateur.</param>
        /// <param name="dictionary">Service de traduction multilingue.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'une des dépendances fournies est <see langword="null"/>.
        /// </exception>
        public UC_LogAndNotify(
            IS_AppContext appContext,
            IS_ErrorLogger errorLogger,
            IS_Notification notification,
            IS_Dictionary dictionary)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre le traitement terminal d'une erreur applicative : construction du détail
        /// technique, traduction du message opérateur, journalisation et notification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Cette méthode est appelée depuis les blocs <c>catch</c> des UseCases,
        /// après réception d'une exception typée du projet, ou depuis le filet de sécurité
        /// <c>VM_Page_Generic.ExecuteSafeAsync</c>.
        /// </para>
        /// <para>
        /// Séquence d'exécution (4 étapes isolées, principe best-effort) :
        /// </para>
        /// <list type="number">
        ///   <item><description>Construction de <c>errorDetails</c> au format <c>ErrorId — message</c>.</description></item>
        ///   <item><description>Traduction de <paramref name="dictionaryKey"/> via <see cref="IS_Dictionary"/>.</description></item>
        ///   <item><description>Journalisation via <see cref="IS_ErrorLogger.ExecuteAsync"/>.</description></item>
        ///   <item><description>Notification opérateur via <see cref="IS_Notification.Error"/> si <paramref name="notify"/> vaut <see langword="true"/>.</description></item>
        /// </list>
        /// <para>
        /// Robustesse : Chaque étape est isolée dans son propre bloc <c>try/catch</c>.
        /// La défaillance d'une étape n'empêche pas l'exécution des suivantes. Les erreurs
        /// internes sont tracées via <see cref="Debug.WriteLine(string)"/> en environnement
        /// de développement, sans impact en production.
        /// </para>
        /// <para>
        /// CancellationToken : Le <paramref name="ct"/> reçu est conservé dans la signature
        /// pour conformité avec la convention normative (section 4.6.5) mais il est
        /// ignoré activement dans toutes les opérations internes. Les appels internes
        /// utilisent <see cref="CancellationToken.None"/> afin de garantir que la journalisation
        /// et la notification d'une erreur ne soient jamais interrompues par un signal
        /// d'annulation concomitant.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain amont transmise par le composant appelant.</param>
        /// <param name="dictionaryKey">
        /// Clé dictionnaire du message opérateur. Valeurs standards : <c>"No_EC_01"</c> (Ex_Business),
        /// <c>"No_EC_02"</c> (Ex_Infrastructure), <c>"No_EC_03"</c> (Ex_Unclassified).
        /// </param>
        /// <param name="ex">
        /// Exception typée à traiter (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>
        /// ou <see cref="Ex_Unclassified"/>). Si <see langword="null"/>, la méthode retourne
        /// immédiatement sans traitement.
        /// </param>
        /// <param name="notify">
        /// Indique si une notification opérateur doit être produite. Défaut : <see langword="true"/>.
        /// </param>
        /// <param name="ct">
        /// Token d'annulation. Conservé par convention mais volontairement ignoré dans
        /// les opérations internes (voir remarques).
        /// </param>
        /// <returns>Une tâche représentant l'exécution asynchrone du traitement terminal.</returns>
        public async Task ExecuteAsync(
            string caller,
            string dictionaryKey,
            Exception ex,
            bool notify = true,
            CancellationToken ct = default)
        {
            // Le paramètre ct est intentionnellement ignoré à partir d'ici.
            // Voir la documentation XML de cette méthode pour la justification.

            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            if (ex is null)
                return;

            // --- Étape 1 : construction du détail technique ---
            string errorDetails = TryBuildErrorDetails(ex);

            // --- Étape 2 : traduction de la clé dictionnaire ---
            string errorMessage = TryTranslateDictionaryKey(callChain, dictionaryKey);

            // --- Étape 3 : journalisation (indépendante de l'étape 4) ---
            await TryLogErrorAsync(callChain, errorMessage, ex);

            // --- Étape 4 : notification opérateur (exécutée même si l'étape 3 a échoué) ---
            if (notify)
            {
                TryNotifyOperator(callChain, dictionaryKey, errorDetails);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Construit le détail technique à partir des propriétés de l'exception typée,
        /// en isolant toute défaillance éventuelle.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Première étape du pipeline de traitement terminal. Le détail
        /// produit est destiné à la journalisation et potentiellement à la notification.
        /// </para>
        /// <para>
        /// Robustesse : En cas d'échec interne (exception inattendue lors de l'extraction
        /// des propriétés), retourne une valeur de secours normalisée et trace l'incident
        /// via <see cref="Debug.WriteLine(string)"/>.
        /// </para>
        /// </remarks>
        /// <param name="ex">Exception à analyser.</param>
        /// <returns>Chaîne au format <c>ErrorId — message</c>, ou <c>"? — UnknownException"</c> en cas d'échec.</returns>
        private static string TryBuildErrorDetails(Exception ex)
        {
            try
            {
                return ex switch
                {
                    Ex_Business bex => BuildTypedDetail(bex.ErrorId, bex.InnerException, bex.ErrorException),
                    Ex_Infrastructure iex => BuildTypedDetail(iex.ErrorId, iex.InnerException, iex.ErrorException),
                    Ex_Unclassified uex => BuildTypedDetail(uex.ErrorId, uex.InnerException, uex.ErrorException),
                    _ => $"? — {ex.Message}"
                };
            }
            catch (Exception internalEx)
            {
                Debug.WriteLine(
                    $"[UC_LogAndNotify.TryBuildErrorDetails] Échec interne lors de la " +
                    $"construction du détail technique : {internalEx.Message}");
                return "? — UnknownException";
            }
        }

        /// <summary>
        /// Assemble le détail technique normalisé au format <c>ErrorId — message</c>
        /// à partir des propriétés d'une exception typée du projet.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Objectif : Centraliser la logique de construction du détail pour éviter
        /// la duplication entre les trois types d'exceptions. Privilégie <c>InnerException.Message</c>
        /// comme source la plus précise, avec repli sur <c>ErrorException</c> puis sur une
        /// valeur par défaut.
        /// </para>
        /// </remarks>
        /// <param name="errorId">Identifiant d'erreur porté par l'exception typée.</param>
        /// <param name="innerException">Exception d'origine enchaînée (peut être <see langword="null"/>).</param>
        /// <param name="errorException">Message technique porté par l'exception typée (repli).</param>
        /// <returns>Chaîne normalisée au format <c>ErrorId — message</c>.</returns>
        private static string BuildTypedDetail(
            string? errorId,
            Exception? innerException,
            string? errorException)
        {
            string id = errorId ?? "?";
            string detail = innerException?.Message
                         ?? errorException
                         ?? "UnknownException";

            return $"{id} — {detail}";
        }

        /// <summary>
        /// Récupère le texte traduit associé à une clé dictionnaire, en isolant toute
        /// défaillance éventuelle du service de traduction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Deuxième étape du pipeline. Le texte produit est destiné
        /// à la journalisation en tant que message principal.
        /// </para>
        /// <para>
        /// Robustesse : En cas d'échec de la traduction, retourne la clé brute comme
        /// valeur de repli et trace l'incident via <see cref="Debug.WriteLine(string)"/>.
        /// Cette stratégie préserve la traçabilité de l'erreur dans les logs même si le
        /// dictionnaire est indisponible.
        /// </para>
        /// <para>
        /// CancellationToken : Appel au dictionnaire avec <see cref="CancellationToken.None"/>
        /// pour empêcher toute interruption de la traduction par un signal d'annulation amont.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="key">Clé dictionnaire à résoudre.</param>
        /// <returns>Texte traduit si la résolution réussit, sinon la clé brute.</returns>
        private string TryTranslateDictionaryKey(string caller, string key)
        {
            string callChain = $"{caller} > {nameof(TryTranslateDictionaryKey)}";

            try
            {
                return _dictionary.GetText(callChain, key, CancellationToken.None);
            }
            catch (Exception internalEx)
            {
                Debug.WriteLine(
                    $"[UC_LogAndNotify.TryTranslateDictionaryKey] Échec de la traduction " +
                    $"de la clé '{key}' : {internalEx.Message}");
                return key;
            }
        }

        /// <summary>
        /// Délègue la journalisation technique à <see cref="IS_ErrorLogger"/> en isolant
        /// toute défaillance éventuelle du service de log.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Troisième étape du pipeline. Indépendante de la notification
        /// opérateur : l'échec de la journalisation n'empêche pas la notification d'être
        /// exécutée à l'étape suivante.
        /// </para>
        /// <para>
        /// Robustesse : En cas d'échec du logger, l'incident est tracé via
        /// <see cref="Debug.WriteLine(string)"/> mais n'est pas propagé à l'appelant.
        /// </para>
        /// <para>
        /// CancellationToken : Appel au logger avec <see cref="CancellationToken.None"/>
        /// pour garantir que la journalisation soit tentée intégralement, indépendamment
        /// de tout signal d'annulation amont.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="errorMessage">Message principal déjà traduit (ou clé brute en repli).</param>
        /// <param name="ex">Exception typée d'origine, transmise intégralement au logger.</param>
        /// <returns>Une tâche représentant l'exécution asynchrone de la journalisation.</returns>
        private async Task TryLogErrorAsync(string caller, string errorMessage, Exception ex)
        {
            string callChain = $"{caller} > {nameof(TryLogErrorAsync)}";

            try
            {
                DTO_AppContext appCtx = _appContext.GetAppContext();

                await _errorLogger.ExecuteAsync(callChain, appCtx, errorMessage, ex, CancellationToken.None);
            }
            catch (Exception internalEx)
            {
                Debug.WriteLine(
                    $"[UC_LogAndNotify.TryLogErrorAsync] Échec de la journalisation : " +
                    $"{internalEx.Message}");
            }
        }

        /// <summary>
        /// Délègue la notification opérateur à <see cref="IS_Notification"/> en isolant
        /// toute défaillance éventuelle du service de notification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Quatrième étape du pipeline. Exécutée si et seulement si le
        /// paramètre <c>notify</c> de <see cref="ExecuteAsync"/> vaut <see langword="true"/>.
        /// Indépendante de l'étape de journalisation : l'échec du logger n'empêche pas
        /// la notification d'être tentée.
        /// </para>
        /// <para>
        /// Robustesse : En cas d'échec du service de notification, l'incident est
        /// tracé via <see cref="Debug.WriteLine(string)"/> mais n'est pas propagé à l'appelant.
        /// </para>
        /// <para>
        /// CancellationToken : Appel à la notification avec <see cref="CancellationToken.None"/>
        /// pour garantir que l'opérateur soit informé de l'erreur quelle que soit la situation
        /// d'annulation en cours.
        /// </para>
        /// <para>
        /// Contrat avec IS_Notification : Le service de notification reçoit la
        /// <paramref name="dictionaryKey"/> (et non le texte traduit) car il effectue lui-même
        /// la traduction. Ce comportement est partagé avec d'autres composants qui appellent
        /// <see cref="IS_Notification"/>.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="dictionaryKey">Clé dictionnaire du message opérateur, non traduite.</param>
        /// <param name="errorDetails">Détail technique au format <c>ErrorId — message</c>.</param>
        private void TryNotifyOperator(string caller, string dictionaryKey, string errorDetails)
        {
            string callChain = $"{caller} > {nameof(TryNotifyOperator)}";

            try
            {
                _notification.Error(callChain, dictionaryKey, errorDetails, CancellationToken.None);
            }
            catch (Exception internalEx)
            {
                Debug.WriteLine(
                    $"[UC_LogAndNotify.TryNotifyOperator] Échec de la notification " +
                    $"opérateur pour la clé '{dictionaryKey}' : {internalEx.Message}");
            }
        }

        #endregion
    }
}