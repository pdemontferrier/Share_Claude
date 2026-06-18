using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Orchestre l'application d'une langue à l'interface de l'application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : service de présentation positionné dans D_Presentation, car son
    /// orchestration dépend de composants propres à la couche de présentation
    /// (<c>ISE_Language</c>, <c>ISE_Flag</c>) et il n'est appelé que depuis cette même couche
    /// (séquence de démarrage, page de sélection de langue). Il implémente
    /// <see cref="IS_Language"/>, défini dans A_Domain, garantissant ainsi que les appelants
    /// (UseCases, ViewModels) n'ont aucune dépendance directe vers D_Presentation.
    /// </para>
    /// <para>
    /// Objectif : coordonner les trois étapes du changement de langue dans un ordre déterministe
    /// et robuste : (1) persistance du code culture dans <see cref="ISE_App.AppCultureCode"/>,
    /// (2) chargement du dictionnaire XAML via <see cref="ISE_Language"/>,
    /// (3) synchronisation du drapeau via <see cref="ISE_Flag"/>.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Valider le code culture reçu avant toute opération.
    ///   </description></item>
    ///   <item><description>
    ///     Persister le code culture actif dans <see cref="ISE_App.AppCultureCode"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Déléguer le chargement du dictionnaire à <see cref="ISE_Language"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Déléguer la mise à jour de l'icône de drapeau à <see cref="ISE_Flag"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Requalifier toute exception technique imprévue via <see cref="IS_ExClassifier"/>
    ///     avant propagation.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Aucune manipulation directe de <c>ResourceDictionary</c> WPF : déléguée à
    ///     <c>SE_Language</c>.
    ///   </description></item>
    ///   <item><description>Aucune logique métier.</description></item>
    ///   <item><description>Aucun accès aux données persistées.</description></item>
    ///   <item><description>
    ///     Aucune décision sur la langue à appliquer : cette responsabilité appartient à
    ///     l'appelant (UseCase de démarrage ou ViewModel de sélection de langue).
    ///   </description></item>
    /// </list>
    /// </remarks>
    public class SR_Language : IS_Language
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Setting applicatif global permettant de persister le code culture actif.
        /// </summary>
        private readonly ISE_App _seApp;

        /// <summary>
        /// Setting de présentation gérant l'état du dictionnaire de langue actif, la résolution
        /// de l'URI XAML et le chargement dans les ressources WPF.
        /// </summary>
        private readonly ISE_Language _seLanguage;

        /// <summary>
        /// Setting de présentation gérant la résolution et la persistance de l'URI du drapeau
        /// correspondant à la langue active.
        /// </summary>
        private readonly ISE_Flag _seFlag;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_Language"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : instanciée via le conteneur DI dans D_Presentation, avec enregistrement
        /// en tant que service transitoire ou singleton selon la politique de l'application.
        /// </para>
        /// <para>
        /// Objectif : préparer le service avec l'ensemble des dépendances nécessaires à
        /// l'orchestration complète et robuste d'un changement de langue.
        /// </para>
        /// </remarks>
        /// <param name="seApp">
        /// Setting applicatif global permettant de persister le code culture actif.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="seLanguage">
        /// Setting de présentation gérant le dictionnaire de langue actif.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="seFlag">
        /// Setting de présentation gérant la résolution de l'URI du drapeau.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'une des dépendances obligatoires est <see langword="null"/>.
        /// </exception>
        public SR_Language(
            ISE_App seApp,
            ISE_Language seLanguage,
            ISE_Flag seFlag,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _seApp = seApp ?? throw new ArgumentNullException(nameof(seApp));
            _seLanguage = seLanguage ?? throw new ArgumentNullException(nameof(seLanguage));
            _seFlag = seFlag ?? throw new ArgumentNullException(nameof(seFlag));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Applique la langue correspondant au code culture fourni en orchestrant les trois
        /// étapes du changement de langue.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée au démarrage de l'application (depuis le UseCase orchestrateur
        /// de démarrage) ou lors d'un changement de langue explicite déclenché par l'opérateur.
        /// </para>
        /// <para>
        /// Objectif : exécuter de manière ordonnée et atomique les trois étapes du changement
        /// de langue : (1) persistance du code culture dans <c>ISE_App.AppCultureCode</c>,
        /// (2) chargement du dictionnaire XAML via <c>ISE_Language</c>, (3) mise à jour du
        /// drapeau via <c>ISE_Flag</c>.
        /// </para>
        /// <para>
        /// Comportement en cas d'erreur : toute exception technique inattendue est requalifiée
        /// en <see cref="Ex_Infrastructure"/> avant
        /// propagation. L'appelant (UseCase) prend en charge le traitement terminal.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour
        /// enrichissement et traçabilité.</param>
        /// <param name="cultureCode">
        /// Code culture à appliquer (ex. : <c>"fr-FR"</c>, <c>"en-GB"</c>). Ne doit pas être
        /// <see langword="null"/>, vide ou composé uniquement d'espaces blancs.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière
        /// coopérative.</param>
        /// <returns>Tâche représentant l'exécution asynchrone du changement de langue.</returns>
        /// <exception cref="ArgumentException">
        /// Levée si <paramref name="cultureCode"/> est <see langword="null"/>, vide ou composé
        /// uniquement d'espaces blancs.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors du chargement du dictionnaire ou de
        /// la résolution de l'URI de drapeau (fichier XAML introuvable, ressource WPF invalide,
        /// etc.).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant
        /// l'exécution.
        /// </exception>
        public Task ExecuteAsync(string caller, string cultureCode, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(cultureCode))
                throw new ArgumentException(
                    "Le code culture ne peut pas être nul, vide ou composé uniquement d'espaces.",
                    nameof(cultureCode));

            try
            {
                _seApp.AppCultureCode = cultureCode;

                ApplyDictionary(callChain, cultureCode, ct);
                ApplyFlag(callChain, cultureCode, ct);

                return Task.CompletedTask;
            }
            catch (ArgumentException) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Résout l'URI du dictionnaire XAML et délègue son chargement à
        /// <see cref="ISE_Language"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> lors de chaque changement de
        /// langue, après la persistance du code culture.
        /// </para>
        /// <para>
        /// Objectif : déléguer entièrement la gestion WPF à <c>SE_Language</c>, ce service
        /// restant agnostique de <c>ResourceDictionary</c>.
        /// </para>
        /// <para>
        /// Note CallChain : la chaîne est enrichie du nom de cette méthode privée afin de
        /// garantir la traçabilité complète en cas d'exception produite par <c>SE_Language</c>.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par la méthode publique appelante.</param>
        /// <param name="cultureCode">Code culture à appliquer (ex. : <c>"fr-FR"</c>).</param>
        /// <param name="ct">Jeton d'annulation.</param>
        private void ApplyDictionary(string caller, string cultureCode, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ApplyDictionary)}";

            ct.ThrowIfCancellationRequested();

            Uri dictionaryUri = _seLanguage.GetDictionaryUri(cultureCode);
            _seLanguage.LoadDictionary(dictionaryUri);
        }

        /// <summary>
        /// Résout l'URI du drapeau correspondant au code culture et l'applique via
        /// <see cref="ISE_Flag"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis <see cref="ExecuteAsync"/> après le chargement du
        /// dictionnaire, afin de synchroniser l'icône de drapeau affichée dans l'interface.
        /// </para>
        /// <para>
        /// Objectif : mettre à jour <see cref="ISE_Flag.AppFlagUri"/> avec l'URI du drapeau
        /// correspondant au code langue extrait du code culture fourni.
        /// </para>
        /// <para>
        /// Note CallChain : la chaîne est enrichie du nom de cette méthode privée afin de
        /// garantir la traçabilité complète en cas d'exception produite par <c>SE_Flag</c>.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par la méthode publique appelante.</param>
        /// <param name="cultureCode">Code culture source (ex. : <c>"fr-FR"</c>).</param>
        /// <param name="ct">Jeton d'annulation.</param>
        private void ApplyFlag(string caller, string cultureCode, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ApplyFlag)}";

            ct.ThrowIfCancellationRequested();

            string languageCode = ExtractLanguageCode(cultureCode);
            _seFlag.AppFlagUri = _seFlag.GetFlagUriOrDefault(languageCode);
        }

        /// <summary>
        /// Extrait le code langue en majuscules à partir d'un code culture
        /// (ex. : <c>"fr-FR"</c> → <c>"FR"</c>).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : méthode utilitaire appelée par <see cref="ApplyFlag"/> pour convertir
        /// le code culture en code langue compatible avec le référentiel de drapeaux de
        /// <see cref="ISE_Flag"/>.
        /// </para>
        /// <para>
        /// Objectif : isoler la logique d'extraction pour améliorer la lisibilité et faciliter
        /// les tests unitaires indépendants.
        /// </para>
        /// <para>
        /// Comportement : si le code culture ne contient pas de séparateur <c>'-'</c>, la
        /// chaîne entière est retournée en majuscules.
        /// </para>
        /// </remarks>
        /// <param name="cultureCode">
        /// Code culture source. Doit être non nul (validé en amont par
        /// <see cref="ExecuteAsync"/>).
        /// </param>
        /// <returns>
        /// Code langue en majuscules, extrait avant le premier séparateur <c>'-'</c>, ou code
        /// culture complet en majuscules si aucun séparateur n'est trouvé.
        /// </returns>
        private static string ExtractLanguageCode(string cultureCode)
        {
            int index = cultureCode.IndexOf('-');
            string languageCode = index >= 0 ? cultureCode[..index] : cultureCode;
            return languageCode.ToUpperInvariant();
        }

        #endregion
    }
}