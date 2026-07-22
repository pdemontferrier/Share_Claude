using System.Windows;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.D_Presentation.Views.Components.DialogWindow;
using Microsoft.Extensions.DependencyInjection;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Description :
    /// <para>
    /// Service technique transverse de présentation responsable de l'affichage
    /// des notifications utilisateur et du pilotage de la fenêtre de dialogue
    /// non bloquante (<see cref="DialogWindow"/>) dans l'application WPF.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette classe réside en <c>D_Presentation/Services/</c> et implémente
    /// <see cref="IS_Notification"/> conformément à la cinquième cellule de la
    /// table d'arborescence canonique de §4.14.3 amendée du 0230 (sous-cas (c)
    /// Presentation, R-4.14.8 amendée du 0231). Elle est consommée par injection
    /// IS_ depuis les ViewModels, les Menu Handlers et le UseCase terminal
    /// <c>UC_LogAndNotify</c> au titre du pipeline standard de gestion d'erreurs
    /// (§4.7.5 du 0230). Le service est enregistré en singleton dans
    /// <c>SR_ConteneurDI</c>, portée admise au titre de P4-bis (§4.10.10) :
    /// les dépendances injectées (<see cref="IS_Dictionary"/>, <see cref="ISE_Window"/>,
    /// <see cref="IS_ExClassifier"/> et <see cref="IServiceProvider"/>) sont toutes
    /// de portée Singleton dans le pipeline DI .NET standard, aucune dépendance
    /// scoped n'étant consommée. La <see cref="DialogWindow"/> mobilisée est la
    /// View locale du projet (<c>DG244Cutting.D_Presentation.Views.Components.DialogWindow</c>)
    /// livrée au fil <c>DialogWindow_Creation</c>, résolue en Transient via
    /// <see cref="IServiceProvider.GetRequiredService"/> en remplacement de
    /// l'instanciation par <c>new</c> qui contournait le conteneur DI et empêchait
    /// le wiring du DataContext par le constructeur paramétré de la View.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Afficher des notifications cohérentes, robustes et réutilisables à partir
    /// de clés dictionnaire, en propageant systématiquement la CallChain et le
    /// <see cref="CancellationToken"/>, et en appliquant le patron à quatre catch
    /// dans l'ordre canonique (R-4.7.25, R-4.6.13 du 0231).
    /// </para>
    ///
    /// Rôle (cas Concept) :
    /// <para>
    /// Service technique transverse au sens du tableau de §4.7 du 0230. Le
    /// service ne porte aucune action métier unitaire et n'est rattaché à aucune
    /// entité unique ; il porte un concept de présentation transverse (notification
    /// utilisateur). Dérogation typologiquement bornée au préfixe Execute admise
    /// au titre de SR20 du 0232-SR (multiplicité des opérations propres du concept
    /// porté) ; la trace nominative de la dérogation est portée par le
    /// <c>&lt;remarks&gt;</c> de chaque méthode publique.
    /// </para>
    ///
    /// Obligations contractuelles :
    /// <list type="bullet">
    /// <item><description>Construire la CallChain en première instruction effective de chaque méthode au format <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c> (R-4.5.5).</description></item>
    /// <item><description>Valider les préconditions structurelles à l'intérieur du bloc try, avant <c>ct.ThrowIfCancellationRequested()</c>, et lever <see cref="Ex_Business"/> avec code <c>BU_ER_01</c> en cas de violation (R-4.7.25).</description></item>
    /// <item><description>Appliquer le patron à quatre catch dans l'ordre canonique (R-4.6.13, R-4.7.25).</description></item>
    /// <item><description>Mobiliser exclusivement les opérations atomiques d'<see cref="ISE_Window"/> pour l'écriture du titre, du contenu et de l'état d'ouverture de la fenêtre dialogue.</description></item>
    /// <item><description>Ne jamais exposer de type technique WPF en frontière de retour publique : le mapping <see cref="MessageBoxResult"/> → <see cref="bool"/> de <see cref="ConfirmationReturn"/> est strictement interne à la couche D_Presentation et le contrat <see cref="IS_Notification"/> n'expose aucun type WPF (IS5).</description></item>
    /// </list>
    ///
    /// Responsabilités :
    /// <list type="bullet">
    /// <item><description>Résoudre les messages et les titres standards via <see cref="IS_Dictionary"/>.</description></item>
    /// <item><description>Afficher les notifications via <see cref="MessageBox"/> sur le dispatcher WPF.</description></item>
    /// <item><description>Résoudre la <see cref="DialogWindow"/> locale en Transient via <see cref="IServiceProvider"/> à la première ouverture (le DataContext étant wiré par le constructeur paramétré de la View).</description></item>
    /// <item><description>Piloter l'ouverture et la fermeture d'une <see cref="DialogWindow"/> non bloquante via <see cref="ISE_Window.OpenDialog(string, string)"/> et <see cref="ISE_Window.CloseDialog"/>, l'idempotence de l'écriture des libellés étant portée par le helper canonique <c>SetField</c> du <see cref="ISE_Window"/> consommé.</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune mutation persistante ni participation à la chaîne (1) d'écriture stricte (SR24, SR25 ➖).</description></item>
    /// <item><description>Aucun appel direct à un Repository (SR14, I-4.14.6).</description></item>
    /// <item><description>Aucun appel d'un autre Service applicatif au sens de l'orchestration de scénario (SR15, I-4.14.6).</description></item>
    /// <item><description>Aucune journalisation d'erreur via <c>IS_ErrorLogger</c> ni notification interne récursive (SR16, I-4.7.6).</description></item>
    /// <item><description>Aucune ouverture, validation ou annulation de transaction (SR13, I-4.10.1).</description></item>
    /// </list>
    /// </summary>
    /// <seealso cref="IS_Notification"/>
    /// <seealso cref="ISE_Window"/>
    /// <seealso cref="IS_Dictionary"/>
    /// <seealso cref="IS_ExClassifier"/>
    /// <seealso cref="DialogWindow"/>
    public class SR_Notification : IS_Notification
    {
        #region === Propriétés privées ===

        private readonly string _callee;
        private DialogWindow? _dialogWindow;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Dictionary _dictionary;
        private readonly ISE_Window _seWindow;
        private readonly IS_ExClassifier _classifier;
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>Initialise une nouvelle instance du service <see cref="SR_Notification"/>.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Cette classe est instanciée par le conteneur d'injection de dépendances dans
        /// la couche Presentation. Les quatre dépendances injectées sont des contrats stables
        /// (deux services transversaux d'utilité, un Setting de présentation et le
        /// <see cref="IServiceProvider"/> du conteneur DI), conformes aux dépendances admises
        /// pour un Service Presentation (SR24, R-2.5.6). Toutes les dépendances sont de portée
        /// Singleton dans le pipeline DI .NET standard ; aucune dépendance scoped n'est consommée,
        /// la portée Singleton du Service reste admise au titre de P4-bis (§4.10.10).</para>
        /// Objectif :
        /// <para>Initialiser le champ <c>_callee</c> par réflexion sur le type courant et
        /// valider les quatre dépendances obligatoires par garde <see cref="ArgumentNullException"/>.</para>
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee = GetType().Name</c> en première instruction (R-4.5.5).</description></item>
        /// <item><description>Valider et stocker <see cref="IS_Dictionary"/>.</description></item>
        /// <item><description>Valider et stocker <see cref="ISE_Window"/>.</description></item>
        /// <item><description>Valider et stocker <see cref="IS_ExClassifier"/>.</description></item>
        /// <item><description>Valider et stocker <see cref="IServiceProvider"/> (utilisé pour la résolution Transient de la <see cref="DialogWindow"/> locale dans <see cref="OpenDialogWindow"/>).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="dictionary">Service de dictionnaire multilingue (résolution clés → textes).</param>
        /// <param name="seWindow">Setting transverse de présentation centralisant l'état partagé des fenêtres principale et dialogue.</param>
        /// <param name="classifier">Service transversal d'utilité de requalification des exceptions brutes en exceptions typées (R-4.7.25).</param>
        /// <param name="serviceProvider">Conteneur d'injection de dépendances racine, utilisé pour résoudre en Transient la <see cref="DialogWindow"/> locale à la première ouverture du dialogue.</param>
        /// <exception cref="ArgumentNullException">Levée si une dépendance obligatoire est <see langword="null"/>.</exception>
        public SR_Notification(
            IS_Dictionary dictionary,
            ISE_Window seWindow,
            IS_ExClassifier classifier,
            IServiceProvider serviceProvider)
        {
            _callee = GetType().Name;
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            _seWindow = seWindow ?? throw new ArgumentNullException(nameof(seWindow));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'information standard.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_01</c> et l'icône Information.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Information(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Information)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_01", MessageBoxButton.OK, MessageBoxImage.Information, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message de type Stop signalant une situation bloquante.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_02</c> et l'icône Stop.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Stop(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Stop)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_02", MessageBoxButton.OK, MessageBoxImage.Stop, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'erreur critique.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_03</c> et l'icône Error.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Error(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Error)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_03", MessageBoxButton.OK, MessageBoxImage.Error, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche une question à l'utilisateur (boîte Yes/No sans retour).</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_04</c>, les boutons Yes/No et l'icône Question.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Question(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Question)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_04", MessageBoxButton.YesNo, MessageBoxImage.Question, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'avertissement non bloquant.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_05</c> et l'icône Warning.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Warning(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Warning)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_05", MessageBoxButton.OK, MessageBoxImage.Warning, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message de type valeur non valide.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_06</c> et l'icône Warning.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void NotValid(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(NotValid)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_06", MessageBoxButton.OK, MessageBoxImage.Warning, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message de confirmation (Yes/No sans retour).</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_07</c>, les boutons Yes/No et l'icône Question.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Confirmation(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Confirmation)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_07", MessageBoxButton.YesNo, MessageBoxImage.Question, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message de succès.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_08</c> et l'icône Exclamation.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void Success(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Success)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_08", MessageBoxButton.OK, MessageBoxImage.Exclamation, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche un message d'information importante.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// puis déléguer à la méthode privée <see cref="ShowMessage"/> avec le titre standard
        /// <c>No_Ti_09</c> et l'icône Warning.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void ImportantInformation(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ImportantInformation)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                ShowMessage(callChain, messageKey, additionalInfo, "No_Ti_09", MessageBoxButton.OK, MessageBoxImage.Warning, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Affiche une boîte de confirmation et retourne la réponse utilisateur sous forme de <see cref="bool"/>.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).
        /// Retour purifié vers <see cref="bool"/> au titre du refactoring de pureté contractuelle
        /// d'A_Domain (IS5 / I-2.4.1, I-2.4.2). Le mapping <see cref="MessageBoxResult"/> →
        /// <see cref="bool"/> est strictement interne à la couche D_Presentation et ne fuit
        /// jamais au contrat <see cref="IS_Notification"/>.</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// résoudre le message et le titre standard <c>No_Ti_07</c> via dictionnaire,
        /// présenter la boîte Yes/No sur le dispatcher WPF, et restituer le choix utilisateur.
        /// Mapping strict : <see langword="true"/> si et seulement si <see cref="MessageBoxResult.Yes"/> ;
        /// <see langword="false"/> dans tous les autres cas (No, fermeture forcée par
        /// l'utilisateur, tout autre résultat retourné par la boîte). Toute défaillance
        /// technique remonte requalifiée au consommateur amont — aucune valeur de repli
        /// silencieuse (SR9).</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal à afficher.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns><see langword="true"/> si l'utilisateur a répondu Yes ; <see langword="false"/> dans tous les autres cas (No, fermeture forcée).</returns>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> ou <paramref name="messageKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public bool ConfirmationReturn(string caller, string messageKey, string? additionalInfo = null, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ConfirmationReturn)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(messageKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'messageKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                string message = SafeGetDictionaryText(callChain, messageKey, ct);
                string title = SafeGetDictionaryText(callChain, "No_Ti_07", ct);
                string fullMessage = additionalInfo == null ? message : $"{message} {additionalInfo}";

                return Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show(fullMessage, title, MessageBoxButton.YesNo, MessageBoxImage.Question)) == MessageBoxResult.Yes;
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Ouvre une fenêtre de dialogue non bloquante centrée sur la fenêtre principale.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).
        /// La méthode mobilise l'opération atomique <see cref="ISE_Window.OpenDialog(string, string)"/>
        /// — seule voie d'écriture admise sur les propriétés <c>DW_Title</c>, <c>DW_Content</c>
        /// et <c>DW_IsOpen</c> exposées en lecture seule par le contrat (§2.5.4 du 0230).</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider les préconditions structurelles,
        /// résoudre <paramref name="titleKey"/> et <paramref name="contentKey"/> en interne via
        /// <see cref="IS_Dictionary"/> (en cohérence avec la résolution interne pratiquée par les
        /// autres méthodes publiques de notification du service), puis sur le dispatcher WPF :
        /// pivoter l'instanciation par <see cref="ISE_Window.DW_IsOpen"/> (source de vérité) ;
        /// à la première ouverture, résoudre la <see cref="DialogWindow"/> locale en Transient
        /// via <see cref="IServiceProvider.GetRequiredService"/> en lieu et place de
        /// <c>new DialogWindow()</c> (le DataContext étant wiré par le constructeur paramétré
        /// de la View), affecter <c>Owner = MainWindow</c> et
        /// <see cref="WindowStartupLocation.CenterOwner"/>, appeler
        /// <see cref="ISE_Window.OpenDialog(string, string)"/> AVANT le <c>Show()</c>, désactiver
        /// la fenêtre principale, puis afficher la fenêtre dialogue ; à la réouverture sur dialog
        /// déjà ouverte, déléguer à <see cref="ISE_Window.OpenDialog(string, string)"/> seule, dont
        /// l'idempotence est portée par <c>SetField</c> dans <c>SE_Window</c> (mise à jour des
        /// libellés à la volée via INPC sans réouverture WPF). Une double garde défensive
        /// <c>_dialogWindow != null &amp;&amp; _dialogWindow.IsVisible</c> est conservée à titre
        /// de filet secondaire couvrant une éventuelle désynchronisation transitoire entre la
        /// fenêtre WPF effective et l'état partagé.</para>
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Construire la callChain au format normatif.</description></item>
        /// <item><description>Valider <paramref name="caller"/>, <paramref name="titleKey"/>, <paramref name="contentKey"/> (A3).</description></item>
        /// <item><description>Vérifier l'annulation coopérative.</description></item>
        /// <item><description>Résoudre <paramref name="titleKey"/> et <paramref name="contentKey"/> via <see cref="SafeGetDictionaryText"/>.</description></item>
        /// <item><description>Sur le dispatcher WPF : pivot sur <see cref="ISE_Window.DW_IsOpen"/> avec double garde défensive ; branche d'instanciation (résolution Transient via <see cref="IServiceProvider"/>, affectation Owner / WindowStartupLocation, opération atomique <c>OpenDialog</c>, désactivation <c>MainWindow</c>, <c>Show()</c>) ; branche idempotente (<c>OpenDialog</c> seule).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="titleKey">Clé dictionnaire du titre à afficher.</param>
        /// <param name="contentKey">Clé dictionnaire du contenu à afficher.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/>, <paramref name="titleKey"/> ou <paramref name="contentKey"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void OpenDialogWindow(string caller, string titleKey, string contentKey, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(OpenDialogWindow)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(titleKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'titleKey' ne peut pas être null, vide ou composé uniquement d'espaces.");
                if (string.IsNullOrWhiteSpace(contentKey))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'contentKey' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                string title = SafeGetDictionaryText(callChain, titleKey, ct);
                string content = SafeGetDictionaryText(callChain, contentKey, ct);

                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Pivot d'instanciation : DW_IsOpen est la source de vérité (arbitrage Q2 du
                    // fil SR_Notification_Refactoring). La double garde
                    // _dialogWindow != null && _dialogWindow.IsVisible bloque toute double
                    // instanciation en cas de désync transitoire entre View WPF effective et état
                    // partagé.
                    bool shouldInstantiate = !_seWindow.DW_IsOpen
                                             && (_dialogWindow == null || !_dialogWindow.IsVisible);

                    if (shouldInstantiate)
                    {
                        // Première ouverture : résolution Transient de la View locale via le
                        // conteneur DI (le DataContext est wiré par le constructeur paramétré de
                        // la View — arbitrage α du fil SR_Notification_Refactoring).
                        _dialogWindow = _serviceProvider.GetRequiredService<DialogWindow>();
                        _dialogWindow.Owner = Application.Current.MainWindow;
                        _dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                        _seWindow.OpenDialog(title, content);

                        if (Application.Current.MainWindow != null)
                            Application.Current.MainWindow.IsEnabled = false;

                        _dialogWindow.Show();
                    }
                    else
                    {
                        // Dialog déjà ouverte : idempotence portée par SetField dans
                        // SE_Window.OpenDialog (arbitrage Q1 du fil SR_Notification_Refactoring).
                        // Les libellés sont publiés à la volée aux bindings via INPC, sans
                        // réouverture WPF.
                        _seWindow.OpenDialog(title, content);
                    }
                }));
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Ferme la fenêtre de dialogue si elle est ouverte et réactive la fenêtre principale.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique du service technique transverse de notification (cas Concept,
        /// dérogation typologiquement bornée au préfixe Execute admise au titre de SR20).
        /// La méthode mobilise l'opération atomique <see cref="ISE_Window.CloseDialog"/>
        /// qui repositionne <c>DW_Title</c>/<c>DW_Content</c> à chaîne vide et <c>DW_IsOpen</c>
        /// à <see langword="false"/>.</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider la précondition structurelle,
        /// puis sur le dispatcher WPF : fermer la fenêtre WPF si visible, réactiver
        /// inconditionnellement la fenêtre principale (dans la branche nominale, pas de
        /// <c>finally</c>), puis appeler <see cref="ISE_Window.CloseDialog"/>.</para>
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Construire la callChain au format normatif.</description></item>
        /// <item><description>Valider <paramref name="caller"/> (A3).</description></item>
        /// <item><description>Vérifier l'annulation coopérative.</description></item>
        /// <item><description>Sur le dispatcher WPF : fermeture conditionnelle, réactivation inconditionnelle de <c>MainWindow</c>, opération atomique <c>CloseDialog</c>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> est null, vide ou whitespace (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        public void CloseDialogWindow(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(CloseDialogWindow)}";

            try
            {
                if (string.IsNullOrWhiteSpace(caller))
                    throw new Ex_Business(callChain, Ex_Business.ErrorCodes.BU_ER_01, "Le paramètre 'caller' ne peut pas être null, vide ou composé uniquement d'espaces.");

                ct.ThrowIfCancellationRequested();

                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_dialogWindow != null && _dialogWindow.IsVisible)
                    {
                        _dialogWindow.Close();
                        _dialogWindow = null;
                    }

                    if (Application.Current.MainWindow != null)
                        Application.Current.MainWindow.IsEnabled = true;

                    _seWindow.CloseDialog();
                }));
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Description :
        /// <para>Affiche un message générique avec le titre, le contenu et l'icône spécifiés.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode privée d'articulation factorisant l'affichage des messages standards
        /// pour les neuf méthodes publiques de notification déléguant à <see cref="MessageBox"/>.
        /// La callChain reçue en entrée a déjà été enrichie par la méthode publique amont au
        /// format <c>{caller} &gt; {_callee} &gt; {nameof(méthode publique)}</c>.</para>
        /// Objectif :
        /// <para>Construire la callChain privée au format <c>{caller} &gt; {nameof(ShowMessage)}</c>,
        /// résoudre le message principal et le titre via dictionnaire (ordre paramètre correct
        /// caller-puis-key), assembler le message complet, puis afficher la boîte sur le
        /// dispatcher WPF. Aucune précondition structurelle ici : les arguments sont propagés
        /// par les méthodes publiques amont qui les ont déjà validés.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont enrichie par la méthode publique appelante.</param>
        /// <param name="messageKey">Clé dictionnaire du message principal.</param>
        /// <param name="additionalInfo">Texte complémentaire optionnel déjà résolu.</param>
        /// <param name="titleKey">Clé dictionnaire du titre standard.</param>
        /// <param name="button">Type de boutons affichés.</param>
        /// <param name="icon">Icône du message.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        private void ShowMessage(string caller, string messageKey, string? additionalInfo, string titleKey, MessageBoxButton button, MessageBoxImage icon, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(ShowMessage)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                string message = SafeGetDictionaryText(callChain, messageKey, ct);
                string title = SafeGetDictionaryText(callChain, titleKey, ct);
                string fullMessage = additionalInfo == null ? message : $"{message} {additionalInfo}";

                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show(fullMessage, title, button, icon));
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Description :
        /// <para>Récupère un texte du dictionnaire avec propagation explicite de la CallChain.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode privée d'articulation factorisant la résolution des clés de message
        /// et de titre standard de notification. La callChain reçue en entrée a déjà été
        /// enrichie par la méthode publique ou privée amont.</para>
        /// Objectif :
        /// <para>Construire la callChain privée au format <c>{caller} &gt; {nameof(SafeGetDictionaryText)}</c>,
        /// puis déléguer à <see cref="IS_Dictionary.GetText"/> avec ordre paramètre correct
        /// (caller, key, ct). Conformément au comportement contractuel d'<c>IS_Dictionary</c>
        /// (§4.11.4 du 0230 — repli interne sur <c>[key] not found</c> et journalisation
        /// autonome en mode fire-and-forget en cas d'anomalie), aucune absorption d'exception
        /// au-delà du patron à quatre catch n'est requise ici : le repli sémantique est porté
        /// par le service de dictionnaire lui-même.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont enrichie par la méthode appelante.</param>
        /// <param name="key">Clé du dictionnaire à rechercher.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns>Texte traduit retourné par <see cref="IS_Dictionary.GetText"/>, ou repli interne <c>[key] not found</c> géré par le service de dictionnaire.</returns>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        private string SafeGetDictionaryText(string caller, string key, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(SafeGetDictionaryText)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                return _dictionary.GetText(callChain, key, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}