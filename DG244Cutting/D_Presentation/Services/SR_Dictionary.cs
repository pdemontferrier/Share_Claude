using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Fournit un accès centralisé, traçable et robuste aux textes traduits du dictionnaire
    /// de langue actif.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : service positionné dans B_UseCases/Services/Presentation, implémentant
    /// <see cref="IS_Dictionary"/>. Contrairement à <c>SE_Language</c>
    /// qui manipulent directement les types WPF, ce service ne contient aucune référence à
    /// WPF et peut être injecté dans des UseCases, des services ou des ViewModels sans créer
    /// de dépendance descendante vers D_Presentation.
    /// </para>
    /// <para>
    /// Objectif : encapsuler la résolution d'une clé de traduction avec journalisation
    /// systématique des clés manquantes, sans jamais interrompre ni bloquer le flux
    /// applicatif appelant.
    /// </para>
    /// <para>
    /// Exception architecturale documentée — usage de <c>IS_ErrorLogger</c> : ce service
    /// est l'un des services transverses autorisés à invoquer <c>IS_ErrorLogger</c>
    /// directement, sans passer par un UseCase orchestrateur. Cette exception est délibérée :
    /// les anomalies de dictionnaire (clé absente, dictionnaire non initialisé) constituent
    /// des signaux techniques non bloquants dont la journalisation autonome est nécessaire
    /// pour ne pas interrompre le flux d'affichage et pour permettre leur détection en
    /// exploitation.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Déléguer la résolution du texte à <see cref="ISE_Language.GetEntry"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Retourner une valeur de repli lisible si la clé est absente ou le dictionnaire
    ///     non chargé.
    ///   </description></item>
    ///   <item><description>
    ///     Journaliser toute anomalie (clé absente, erreur inattendue) via
    ///     <see cref="IS_ErrorLogger"/> de manière autonome et non bloquante.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Aucune gestion du dictionnaire actif ni du code culture : déléguées à
    ///     <c>SE_Language</c> et <c>SE_App</c>.
    ///   </description></item>
    ///   <item><description>
    ///     Aucune référence aux types WPF.
    ///   </description></item>
    ///   <item><description>
    ///     Aucune orchestration de flux métier.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public class SR_Dictionary : IS_Dictionary
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        /// <summary>
        /// Message technique générique utilisé comme <c>errorMessage</c> lors des appels à
        /// <see cref="IS_ErrorLogger"/> pour toute anomalie de dictionnaire.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Rédigé en français et en dur dans le code, conformément à la règle 2 de la
        /// mécanique multilingue (section 3.8.2 du référentiel) : les messages techniques
        /// destinés aux fichiers de log ne transitent pas par le dictionnaire de langue.
        /// </para>
        /// </remarks>
        private const string DictionaryErrorMessage = "Erreur de dictionnaire multilingue.";

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Setting de présentation exposant l'accès en lecture au dictionnaire de langue actif,
        /// via la méthode <see cref="ISE_Language.GetEntry"/>.
        /// </summary>
        private readonly ISE_Language _seLanguage;

        /// <summary>
        /// Service transverse de journalisation des erreurs, invoqué de manière autonome et
        /// non bloquante lors des anomalies de dictionnaire (clé absente ou erreur inattendue).
        /// </summary>
        private readonly IS_ErrorLogger _errorLogger;

        /// <summary>
        /// Service de fourniture du contexte applicatif courant (identité utilisateur, poste, adresse IP,
        /// horodatage), utilisé pour enrichir chaque entrée de log.
        /// </summary>
        private readonly IS_AppContext _appContext;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_Dictionary"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instanciée via le conteneur DI dans B_UseCases, avec enregistrement
        /// en tant que singleton pour garantir la cohérence de l'état du dictionnaire actif
        /// tout au long de la durée de vie de l'application.
        /// </para>
        /// <para>
        /// Objectif : préparer le service avec l'accès à l'état linguistique actif et au
        /// logger pour la journalisation autonome des anomalies de résolution.
        /// </para>
        /// </remarks>
        /// <param name="seLanguage">
        /// Setting de présentation exposant l'accès au dictionnaire de langue actif.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="errorLogger">
        /// Service de journalisation des erreurs, utilisé en mode best effort pour les
        /// anomalies de dictionnaire. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="seLanguage"/> ou <paramref name="errorLogger"/> est
        /// <see langword="null"/>.
        /// </exception>
        public SR_Dictionary(
            ISE_Language seLanguage,
            IS_ErrorLogger errorLogger,
            IS_AppContext appContext)
        {
            _callee = GetType().Name;
            _seLanguage = seLanguage ?? throw new ArgumentNullException(nameof(seLanguage));
            _errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne le texte traduit correspondant à la clé fournie dans le dictionnaire de
        /// langue actif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : point d'accès unique aux textes traduits, appelé depuis tout composant
        /// nécessitant un libellé localisé (ViewModels, UseCases, services applicatifs). La
        /// méthode est synchrone afin d'être directement utilisable dans les expressions de
        /// binding WPF sans contrainte d'asynchronisme.
        /// </para>
        /// <para>
        /// Objectif : retourner le texte traduit ou une valeur de repli visible si la clé est
        /// absente, sans jamais interrompre ni bloquer le flux appelant. Toute anomalie est
        /// journalisée de manière autonome via <see cref="IS_ErrorLogger"/> en mode
        /// fire-and-forget (best effort).
        /// </para>
        /// <para>
        /// Comportement en cas de clé absente : retourne <c>[key] not found</c> et déclenche
        /// <see cref="LogMissingKeyAsync"/> avec le code <c>DICT_ER_01</c>. Ce retour visible
        /// est intentionnel : il signale qu'une clé est manquante dans le fichier de
        /// ressources sans interrompre l'affichage.
        /// </para>
        /// <para>
        /// Comportement en cas d'erreur inattendue : retourne la même valeur de repli et
        /// déclenche <see cref="LogUnexpectedErrorAsync"/> avec le code <c>DICT_ER_02</c>.
        /// L'exception n'est jamais propagée vers l'appelant, à l'exception explicite de
        /// <see cref="OperationCanceledException"/>.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour
        /// enrichissement et traçabilité.</param>
        /// <param name="key">Clé du texte à rechercher dans le dictionnaire de langue actif.
        /// Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière
        /// coopérative avant l'accès au dictionnaire.</param>
        /// <returns>
        /// Texte traduit correspondant à la clé si elle est présente dans le dictionnaire
        /// actif ; <c>[key] not found</c> si la clé est absente ou si une erreur inattendue
        /// survient.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant l'accès au
        /// dictionnaire. Aucune autre exception n'est propagée vers l'appelant.
        /// </exception>
        public string GetText(string caller, string key, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetText)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                string? text = _seLanguage.GetEntry(key);

                if (text is null)
                {
                    _ = LogMissingKeyAsync(callChain, key,
                        "Clé absente du dictionnaire ou dictionnaire non initialisé.", ct);
                    return $"[{key}] not found";
                }

                return text;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _ = LogUnexpectedErrorAsync(callChain,
                    $"Erreur inattendue lors de la résolution de la clé '{key}'.", ex, ct);
                return $"[{key}] not found";
            }
        }

        #endregion


        #region === Méthodes privées ===

        /// <summary>
        /// Journalise une anomalie liée à une clé de traduction absente du dictionnaire actif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="GetText"/> lorsque
        /// <see cref="ISE_Language.GetEntry"/> retourne <see langword="null"/>, signalant
        /// que la clé demandée est absente du dictionnaire ou que le dictionnaire n'est pas
        /// encore chargé.
        /// </para>
        /// <para>
        /// Objectif : produire une trace exploitable en log (code <c>DICT_ER_01</c>) sans
        /// interrompre le flux principal. Toute défaillance interne de journalisation est
        /// absorbée silencieusement : ce service ne doit jamais bloquer l'affichage.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par <see cref="GetText"/>.</param>
        /// <param name="key">Clé de traduction concernée par l'anomalie.</param>
        /// <param name="details">Description technique complémentaire de l'anomalie.</param>
        /// <param name="ct">Jeton d'annulation.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de la journalisation.</returns>
        private async Task LogMissingKeyAsync(
            string caller,
            string key,
            string details,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(LogMissingKeyAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                var ex = new Ex_Infrastructure(
                    callChain: callChain,
                    errorId: "DICT_ER_01",
                    errorException: $"Clé de traduction manquante '{key}'. {details}");

                DTO_AppContext appCtx = _appContext.GetAppContext();

                await _errorLogger.ExecuteAsync(callChain, appCtx, DictionaryErrorMessage, ex, ct);
            }
            catch
            {
                // Best effort — le dictionnaire ne doit jamais bloquer le flux principal.
            }
        }

        /// <summary>
        /// Journalise une erreur inattendue survenue lors de la résolution d'une clé de
        /// traduction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="GetText"/> lors d'une exception non prévue
        /// autre qu'<see cref="OperationCanceledException"/>, capturée dans le bloc catch
        /// générique.
        /// </para>
        /// <para>
        /// Objectif : produire une trace technique exploitable en log (code <c>DICT_ER_02</c>)
        /// sans interrompre le flux principal. Toute défaillance interne de journalisation
        /// est absorbée silencieusement.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par <see cref="GetText"/>.</param>
        /// <param name="details">Description contextuelle de l'erreur survenue.</param>
        /// <param name="exception">Exception capturée dans le flux principal.</param>
        /// <param name="ct">Jeton d'annulation.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de la journalisation.</returns>
        private async Task LogUnexpectedErrorAsync(
            string caller,
            string details,
            Exception exception,
            CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(LogUnexpectedErrorAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                var ex = new Ex_Infrastructure(
                    callChain: callChain,
                    errorId: "DICT_ER_02",
                    errorException: $"{details} {exception.Message}");

                DTO_AppContext appCtx = _appContext.GetAppContext();

                await _errorLogger.ExecuteAsync(callChain, appCtx, DictionaryErrorMessage, ex, ct);
            }
            catch
            {
                // Best effort — le dictionnaire ne doit jamais bloquer le flux principal.
            }
        }

        #endregion
    }
}