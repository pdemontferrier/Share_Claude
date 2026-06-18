using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace DG244Cutting.D_Presentation.ViewModels.Generic
{
    /// <summary>
    /// Classe ancêtre commune à tous les ViewModels de la couche
    /// Présentation de l'application DG244Cutting, socle technique
    /// transverse de la famille VM.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Socle technique transverse abstrait, ancêtre
    /// commun à parité des deux familles dérivées
    /// <c>VM_Page_Generic</c> et <c>VM_MH_Generic</c>. Réside dans
    /// <c>D_Presentation/ViewModels/Generic</c>. La classe est
    /// déclarée <c>abstract</c>, ce qui interdit son instanciation
    /// directe et impose la dérivation systématique pour tout
    /// ViewModel de l'application. Conformité R-4.15.1 règle 7.</para>
    ///
    /// <para>Objectif : Factoriser au plus haut niveau de la
    /// hiérarchie des ViewModels les mécaniques transverses
    /// communes aux deux familles dérivées : implémentation
    /// d'<see cref="INotifyPropertyChanged"/>, construction et
    /// propagation de la CallChain initiale via
    /// <see cref="BuildFirstCallChain"/>, filet de sécurité
    /// applicatif <see cref="ExecuteSafeAsync"/>, et mécanique
    /// multilingue complète (premier chargement des libellés via
    /// <see cref="InitializeLabels"/> et abonnement INPC à
    /// <see cref="ISE_App.AppCultureCode"/> pour la réactivité au
    /// changement de langue dynamique). Aucun hook propre à une
    /// famille particulière (par exemple <c>LoadAsync</c>) n'est
    /// porté à ce niveau ; ces hooks relèvent des classes filles
    /// <c>VM_Page_Generic</c> et <c>VM_MH_Generic</c>.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Implémenter
    ///   <see cref="INotifyPropertyChanged"/> et exposer aux
    ///   dérivés les helpers <see cref="OnPropertyChanged"/> et
    ///   <see cref="SetProperty{T}"/> pour la notification INPC
    ///   standard, conformément à §4.14.7.</description></item>
    ///   <item><description>Initialiser le champ
    ///   <see cref="_callee"/> via <c>GetType().Name</c> et
    ///   exposer <see cref="BuildFirstCallChain"/> aux dérivés
    ///   pour la construction canonique de la CallChain initiale,
    ///   conformément à §4.5.1 et à R-4.5.5.</description></item>
    ///   <item><description>Exposer <see cref="ExecuteSafeAsync"/>,
    ///   filet de sécurité applicatif à cinq captures
    ///   conformément à §4.7.3, qui délègue le traitement
    ///   terminal des erreurs à
    ///   <see cref="IU_LogAndNotify.ExecuteAsync"/>.</description></item>
    ///   <item><description>Factoriser la mécanique multilingue
    ///   complète : déclarer <see cref="LoadLabels"/> en
    ///   <c>protected virtual</c> pour override par les dérivés,
    ///   exposer <see cref="InitializeLabels"/> qui orchestre le
    ///   premier appel à <see cref="LoadLabels"/> et le
    ///   branchement de l'abonnement INPC à
    ///   <see cref="ISE_App.AppCultureCode"/>, et porter en
    ///   interne <see cref="OnAppPropertyChangedHandler"/> avec
    ///   marshalling Dispatcher défensif. Conformité R-4.11.8,
    ///   R-4.11.9 et I-4.11.11.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>N'expose aucune propriété observable
    ///   propre : les propriétés sont déclarées par les
    ///   ViewModels dérivés concrets et alimentées par leur
    ///   override de <see cref="LoadLabels"/> ou par leurs
    ///   méthodes de chargement métier propres.</description></item>
    ///   <item><description>Ne déclare aucune commande WPF propre
    ///   ni aucun hook spécifique à une famille particulière de
    ///   ViewModels. Les hooks de chargement métier
    ///   (<c>LoadAsync</c>) sont introduits à parité par les
    ///   classes filles <c>VM_Page_Generic</c> et
    ///   <c>VM_MH_Generic</c>.</description></item>
    ///   <item><description>Ne décide ni ne déclenche aucune
    ///   navigation : la décision de navigation appartient aux
    ///   UseCases, conformément à R-4.12.2.
    ///   <see cref="VM_Generic"/> n'injecte ni
    ///   <c>IU_Navigation</c> ni <c>IS_Navigation</c>.</description></item>
    ///   <item><description>Ne désabonne pas l'abonnement INPC à
    ///   <see cref="ISE_App.PropertyChanged"/> : tous les
    ///   ViewModels dérivés de <see cref="VM_Generic"/> sont
    ///   Singleton (P4-bis, §4.10.10) et leur durée de vie est
    ///   alignée sur celle du processus, qui assure naturellement
    ///   la libération de l'abonnement à l'arrêt de
    ///   l'application. Conformité INV-VM-20.</description></item>
    /// </list>
    ///
    /// <para>Exception architecturale documentée — EA-1 (injection
    /// directe d'<c>IU_LogAndNotify</c> en couche Présentation) :</para>
    ///
    /// <para>Cette classe injecte directement
    /// <see cref="IU_LogAndNotify"/>, dépendance de couche
    /// <c>B_UseCases</c>, dans un composant de la couche
    /// <c>D_Presentation</c>. Cette injection contourne le principe
    /// général d'injection des UseCases métier exclusivement par
    /// les ViewModels dérivés au cas par cas, qui transitent
    /// normalement par <c>IS_UseCaseInvoker</c> au titre d'EA-11.
    /// Elle est délibérée et justifiée par le rôle transverse du
    /// filet de sécurité <see cref="ExecuteSafeAsync"/> : la
    /// capture des exceptions applicatives doit être disponible à
    /// toute la hiérarchie des VM sans cérémonie répétée par
    /// chaque dérivé. Cf. §4.15.5 du 0230 pour la formalisation
    /// complète de cette exception architecturale.</para>
    ///
    /// <para>Structure des régions :</para>
    ///
    /// <para>La classe applique la structure normative à sept
    /// régions, conformément à §4.4.2 (cinq régions standard) et
    /// à §4.4.3 (deux extensions présentes) :</para>
    ///
    /// <list type="number">
    ///   <item><description><c>=== Propriétés privées ===</c> :
    ///   champ <see cref="_callee"/>.</description></item>
    ///   <item><description><c>=== Dépendances privées ===</c> :
    ///   champs <see cref="_dictionary"/>,
    ///   <see cref="_logAndNotify"/> et <c>_app</c>.</description></item>
    ///   <item><description><c>=== Événements / Délégués /
    ///   Indexeurs ===</c> : extension présente au titre de
    ///   l'événement <see cref="PropertyChanged"/> requis par
    ///   l'implémentation d'<see cref="INotifyPropertyChanged"/>.
    ///   Conformité §4.4.3.</description></item>
    ///   <item><description><c>=== Constructeur ===</c> :
    ///   constructeur <c>protected</c> à trois paramètres.</description></item>
    ///   <item><description><c>=== Méthodes publiques ===</c> :
    ///   région obligatoirement présente mais vide avec le
    ///   marqueur <c>// A compléter</c>. La classe n'expose aucune
    ///   méthode publique propre.</description></item>
    ///   <item><description><c>=== Méthodes protégées ===</c> :
    ///   extension présente au titre de R-4.4.10 car la classe
    ///   expose six méthodes <c>protected</c>
    ///   (<see cref="BuildFirstCallChain"/>,
    ///   <see cref="ExecuteSafeAsync"/>,
    ///   <see cref="OnPropertyChanged"/>,
    ///   <see cref="SetProperty{T}"/>, <see cref="LoadLabels"/>,
    ///   <see cref="InitializeLabels"/>).</description></item>
    ///   <item><description><c>=== Méthodes privées ===</c> :
    ///   handler <see cref="OnAppPropertyChangedHandler"/>.</description></item>
    /// </list>
    ///
    /// <para>L'extension <c>=== Propriétés publiques ===</c> n'est
    /// pas présente : <see cref="VM_Generic"/> n'expose aucune
    /// propriété observable propre.</para>
    /// </remarks>
    public abstract class VM_Generic : INotifyPropertyChanged
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom dynamique du type concret de la classe courante,
        /// utilisé par <see cref="BuildFirstCallChain"/> pour la
        /// construction de la CallChain initiale.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Initialisé via <c>GetType().Name</c>
        /// dans le constructeur, après l'affectation des trois
        /// dépendances injectées, conformément à l'ordre canonique
        /// d'initialisation prescrit par R-4.4.7.</para>
        /// <para>Objectif : Garantir que la CallChain produite par
        /// les dérivés porte le nom du type effectivement instancié
        /// (par exemple <c>VM_Page10</c>, <c>VM_MH_Reduce</c>) et
        /// non celui de la classe de base. Conformité §4.5.1 et
        /// R-4.5.5.</para>
        /// </remarks>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service d'accès au dictionnaire multilingue, consommé
        /// par les ViewModels dérivés pour le chargement de leurs
        /// libellés via leur override de
        /// <see cref="LoadLabels"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton injecté par le conteneur DI
        /// au constructeur. Conformité R-4.11.7.</para>
        /// <para>Visibilité : <c>protected</c> pour exposition aux
        /// dérivés qui en font un usage légitime documenté dans
        /// leurs overrides de <see cref="LoadLabels"/>. Conformité
        /// INV-VM-14.</para>
        /// </remarks>
        protected readonly IS_Dictionary _dictionary;

        /// <summary>
        /// Orchestrateur du traitement terminal des erreurs
        /// applicatives, consommé par
        /// <see cref="ExecuteSafeAsync"/> et accessible aux dérivés
        /// pour les cas exceptionnels où le filet ne s'applique pas
        /// (par exemple, journalisation d'un événement applicatif
        /// non issu d'une exception).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton injecté par le conteneur DI
        /// au constructeur au titre de l'EA-1 (injection directe
        /// d'un UseCase en couche Présentation), documentée dans
        /// le bloc <c>remarks</c> de la classe.</para>
        /// <para>Visibilité : <c>protected</c> pour usage légitime
        /// documenté par les dérivés. Conformité INV-VM-13.</para>
        /// </remarks>
        protected readonly IU_LogAndNotify _logAndNotify;

        /// <summary>
        /// Setting Singleton de l'état applicatif global, source
        /// des notifications INPC sur <c>AppCultureCode</c>
        /// consommées par
        /// <see cref="OnAppPropertyChangedHandler"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Singleton injecté par le conteneur DI
        /// au constructeur.</para>
        /// <para>Visibilité : <c>private</c> au titre de I-4.11.11
        /// du 0231 : aucun dérivé n'a d'usage légitime documenté
        /// pour accéder directement à <see cref="ISE_App"/> au
        /// titre du multilingue. L'accès à <see cref="ISE_App"/>
        /// pour d'autres usages reste possible par injection
        /// paramétrée propre du dérivé concerné, sans passer par ce
        /// champ. Conformité INV-VM-15.</para>
        /// </remarks>
        private readonly ISE_App _app;

        #endregion

        #region === Événements / Délégués / Indexeurs ===

        /// <summary>
        /// Événement requis par l'implémentation
        /// d'<see cref="INotifyPropertyChanged"/>, levé à chaque
        /// notification de changement d'une propriété observable
        /// d'un ViewModel dérivé.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Levé par les helpers
        /// <see cref="OnPropertyChanged"/> et
        /// <see cref="SetProperty{T}"/>. Conformité §4.14.7 et
        /// INV-VM-09.</para>
        /// </remarks>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de
        /// <see cref="VM_Generic"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Constructeur <c>protected</c> conforme
        /// à R-4.15.1 règle 7 (instanciation directe interdite,
        /// instanciation par dérivation uniquement). La classe
        /// étant <c>abstract</c>, le constructeur n'est invoqué
        /// que par les constructeurs des classes filles via
        /// <c>base(...)</c>. Conformité INV-VM-02.</para>
        ///
        /// <para>Séquence d'initialisation (R-4.4.7, INV-VM-06) :</para>
        /// <list type="number">
        ///   <item><description>Garde nullité et affectation des
        ///   trois dépendances <see cref="_dictionary"/>,
        ///   <see cref="_logAndNotify"/> et <c>_app</c>, dans cet
        ///   ordre.</description></item>
        ///   <item><description>Initialisation du champ
        ///   <see cref="_callee"/> via
        ///   <c>GetType().Name</c>.</description></item>
        /// </list>
        ///
        /// <para>Ce que le constructeur ne fait pas :
        /// l'abonnement INPC à
        /// <see cref="ISE_App.PropertyChanged"/> n'est pas branché
        /// ici, et le premier appel à <see cref="LoadLabels"/>
        /// n'est pas déclenché. Ces deux opérations sont confiées
        /// à <see cref="InitializeLabels"/>, que le ViewModel
        /// dérivé concret final invoque en dernière instruction de
        /// son propre constructeur, après l'affectation de ses
        /// dépendances propres. Ce différé est délibéré et
        /// structurant : il évite l'écueil classique de
        /// l'invocation virtuelle dans le constructeur d'une
        /// classe de base (un override de
        /// <see cref="LoadLabels"/> qui consommerait une
        /// dépendance propre au dérivé verrait cette dépendance
        /// non encore initialisée). Conformité R-4.11.8 et
        /// INV-VM-18.</para>
        /// </remarks>
        /// <param name="dictionary">Service d'accès au dictionnaire
        /// multilingue. Injecté en Singleton par le conteneur DI.</param>
        /// <param name="logAndNotify">Orchestrateur du traitement
        /// terminal des erreurs. Injecté en Singleton par le
        /// conteneur DI au titre de l'EA-1.</param>
        /// <param name="app">Setting Singleton de l'état applicatif
        /// global, source des notifications INPC sur
        /// <c>AppCultureCode</c>. Injecté en Singleton par le
        /// conteneur DI.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un
        /// des trois paramètres est <see langword="null"/>.</exception>
        protected VM_Generic(
            IS_Dictionary dictionary,
            IU_LogAndNotify logAndNotify,
            ISE_App app)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _app = app ?? throw new ArgumentNullException(nameof(app));

            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Méthodes protégées ===

        /// <summary>
        /// Construit la CallChain initiale d'une opération
        /// déclenchée depuis le ViewModel courant, au format
        /// normatif <c>{_callee} &gt; {callerMemberName}</c>.
        /// </summary>
        /// <param name="callerMemberName">Nom de la méthode
        /// appelante, renseigné automatiquement par le compilateur
        /// via <see cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>Chaîne représentant la CallChain initiale au
        /// format normatif.</returns>
        /// <remarks>
        /// <para>Contexte : Méthode helper exposée
        /// <c>protected</c> à tous les dérivés. Conformité §4.5.1
        /// et INV-VM-11.</para>
        /// <para>Objectif : Centraliser la construction canonique
        /// de la CallChain initiale et garantir l'uniformité du
        /// format à travers toute la hiérarchie des ViewModels.
        /// L'attribut <see cref="CallerMemberNameAttribute"/>
        /// résout automatiquement le nom de la méthode appelante,
        /// évitant ainsi toute saisie en dur sujette à
        /// désynchronisation après renommage.</para>
        /// </remarks>
        protected string BuildFirstCallChain([CallerMemberName] string callerMemberName = "")
        {
            return $"{_callee} > {callerMemberName}";
        }

        /// <summary>
        /// Filet de sécurité applicatif qui exécute une action
        /// asynchrone et capture les exceptions selon le pipeline
        /// normatif à cinq captures.
        /// </summary>
        /// <param name="callChain">CallChain de l'opération
        /// englobante, transmise au pipeline de journalisation et
        /// de notification.</param>
        /// <param name="action">Action asynchrone à exécuter sous
        /// la protection du filet.</param>
        /// <param name="ct">Token d'annulation coopérative propagé
        /// au pipeline de journalisation.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de
        /// l'action sous filet.</returns>
        /// <remarks>
        /// <para>Contexte : Méthode helper exposée
        /// <c>protected</c> à tous les dérivés. Filet de dernier
        /// recours au niveau ViewModel, conformément à §4.7.3 du
        /// 0230 et à INV-VM-12.</para>
        ///
        /// <para>Pipeline de capture (cinq cas, ordre normatif) :</para>
        /// <list type="number">
        ///   <item><description><c>catch (Ex_Business ex)</c> →
        ///   délégation à
        ///   <see cref="IU_LogAndNotify.ExecuteAsync"/> avec la
        ///   clé dictionnaire <c>No_EC_01</c>.</description></item>
        ///   <item><description><c>catch (Ex_Infrastructure ex)</c>
        ///   → délégation à
        ///   <see cref="IU_LogAndNotify.ExecuteAsync"/> avec la
        ///   clé dictionnaire <c>No_EC_02</c>.</description></item>
        ///   <item><description><c>catch (Ex_Unclassified ex)</c>
        ///   → délégation à
        ///   <see cref="IU_LogAndNotify.ExecuteAsync"/> avec la
        ///   clé dictionnaire <c>No_EC_03</c>.</description></item>
        ///   <item><description><c>catch (OperationCanceledException)</c>
        ///   → remontée silencieuse par <c>throw;</c> ; aucune
        ///   journalisation ni notification. Règle absolue §4.7.3
        ///   du 0230 : l'annulation coopérative n'est pas une
        ///   erreur, ne produit pas d'identifiant normalisé, ne
        ///   déclenche pas de log d'erreur ni de notification
        ///   opérateur.</description></item>
        ///   <item><description><c>catch (Exception ex)</c> →
        ///   filet ultime, délégation à
        ///   <see cref="IU_LogAndNotify.ExecuteAsync"/> avec la
        ///   clé dictionnaire <c>No_EC_03</c>. Capture toute
        ///   exception brute .NET ayant échappé aux quatre
        ///   captures précédentes.</description></item>
        /// </list>
        ///
        /// <para>Statut de filet de dernier recours :
        /// <see cref="ExecuteSafeAsync"/> ne se substitue pas au
        /// pipeline standard de traitement des erreurs (capture
        /// par les blocs <c>catch</c> des UseCases et délégation
        /// à <see cref="IU_LogAndNotify"/>). Il intervient
        /// uniquement pour les exceptions qui auraient échappé à
        /// ce pipeline standard, ou pour celles levées au sein
        /// même du ViewModel avant d'atteindre un UseCase.</para>
        /// </remarks>
        protected async Task ExecuteSafeAsync(
            string callChain,
            Func<Task> action,
            CancellationToken ct = default)
        {
            try
            {
                await action();
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
            }
            catch (Ex_Unclassified ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
            }
            catch (OperationCanceledException)
            {
                // Remontée silencieuse, règle absolue §4.7.3 du 0230.
                // Aucune journalisation ni notification.
                throw;
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
            }
        }

        /// <summary>
        /// Déclenche la notification <see cref="PropertyChanged"/>
        /// pour la propriété spécifiée.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée,
        /// renseigné automatiquement par le compilateur via
        /// <see cref="CallerMemberNameAttribute"/> lorsque la
        /// méthode est invoquée depuis le setter de la propriété
        /// concernée.</param>
        /// <remarks>
        /// <para>Helper standard du pattern
        /// <see cref="INotifyPropertyChanged"/>. Conformité §4.14.7
        /// et INV-VM-08. Les ViewModels dérivés l'invoquent
        /// typiquement après une mutation d'état qui ne passe pas
        /// par un champ support unique (par exemple, mise à jour
        /// d'une collection observable ou d'une propriété
        /// calculée).</para>
        /// </remarks>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Met à jour un champ support et déclenche
        /// <see cref="PropertyChanged"/> uniquement si la nouvelle
        /// valeur diffère de l'ancienne selon
        /// <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <typeparam name="T">Type du champ support.</typeparam>
        /// <param name="field">Référence au champ support, modifié
        /// par effet de bord.</param>
        /// <param name="value">Nouvelle valeur à affecter.</param>
        /// <param name="propertyName">Nom de la propriété
        /// concernée, renseigné automatiquement.</param>
        /// <returns><see langword="true"/> si la valeur a été
        /// modifiée et la notification émise,
        /// <see langword="false"/> si la nouvelle valeur est égale
        /// à l'ancienne.</returns>
        /// <remarks>
        /// <para>Helper standard du pattern
        /// <see cref="INotifyPropertyChanged"/>, à privilégier sur
        /// <see cref="OnPropertyChanged"/> direct pour les
        /// propriétés observables classiques avec champ support
        /// dédié. Conformité §4.14.7 et INV-VM-08.</para>
        /// </remarks>
        protected bool SetProperty<T>(
            ref T field,
            T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Charge les libellés multilingues du ViewModel dérivé.
        /// Implémentation par défaut vide : les ViewModels dérivés
        /// override cette méthode pour alimenter leurs propriétés
        /// observables de libellés à partir de
        /// <see cref="_dictionary"/>.
        /// </summary>
        /// <param name="callChain">CallChain transmise par
        /// l'orchestrateur (premier appel via
        /// <see cref="InitializeLabels"/>, appels ultérieurs via
        /// <see cref="OnAppPropertyChangedHandler"/>).</param>
        /// <remarks>
        /// <para>Contexte : Méthode <c>protected virtual</c>
        /// déclarée au niveau de <see cref="VM_Generic"/> pour
        /// override par les ViewModels dérivés concrets.
        /// Conformité R-4.11.8 et INV-VM-16. Caractère synchrone,
        /// conformément à §4.11.5.</para>
        ///
        /// <para>L'implémentation par défaut est vide : seul un
        /// dérivé qui affiche des libellés multilingues a vocation
        /// à l'override.</para>
        ///
        /// <para>Patron de surcharge normatif :</para>
        /// <example>
        /// <code>
        /// protected override void LoadLabels(string callChain)
        /// {
        ///     Title = _dictionary.GetText(callChain, "Pxx_Ti_01");
        ///     Description = _dictionary.GetText(callChain, "Pxx_Ti_02");
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        protected virtual void LoadLabels(string callChain)
        {
            // Implémentation par défaut vide. Les ViewModels dérivés
            // qui affichent des libellés multilingues override cette
            // méthode pour alimenter leurs propriétés observables via
            // _dictionary.GetText(callChain, key).
        }

        /// <summary>
        /// Orchestre l'initialisation complète de la mécanique
        /// multilingue du ViewModel : premier appel à
        /// <see cref="LoadLabels"/> et branchement de l'abonnement
        /// INPC à <see cref="ISE_App.PropertyChanged"/>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode <c>protected</c> non virtuelle,
        /// exposée aux ViewModels dérivés. Conformité R-4.11.8 et
        /// INV-VM-17.</para>
        ///
        /// <para>Séquence d'orchestration (trois temps) :</para>
        /// <list type="number">
        ///   <item><description>Construction d'une CallChain
        ///   initiale via
        ///   <see cref="BuildFirstCallChain"/>.</description></item>
        ///   <item><description>Premier appel à
        ///   <see cref="LoadLabels"/> avec cette
        ///   CallChain.</description></item>
        ///   <item><description>Branchement de l'abonnement INPC
        ///   à <see cref="ISE_App.PropertyChanged"/> sur le
        ///   handler interne
        ///   <see cref="OnAppPropertyChangedHandler"/>.</description></item>
        /// </list>
        ///
        /// <para>Règle générale d'invocation (INV-VM-18) :
        /// <see cref="InitializeLabels"/> est appelée
        /// exclusivement dans le constructeur du ViewModel dérivé
        /// concret final, en dernière instruction, après
        /// l'affectation de toutes ses dépendances propres. AUCUNE
        /// classe intermédiaire de la chaîne
        /// <see cref="VM_Generic"/> → <c>VM_X_Generic</c> →
        /// <c>VM_X_Y</c> ne l'invoque. Cette règle prévient
        /// l'écueil de l'invocation virtuelle dans le constructeur
        /// d'une classe de base avec dépendances dérivées non
        /// encore initialisées.</para>
        ///
        /// <para>Patron d'invocation par le dérivé concret
        /// final :</para>
        /// <example>
        /// <code>
        /// public VM_PageXX(
        ///     IS_Dictionary dictionary,
        ///     IU_LogAndNotify logAndNotify,
        ///     ISE_App app,
        ///     IS_UseCaseInvoker invoker)
        ///     : base(dictionary, logAndNotify, app)
        /// {
        ///     _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
        ///
        ///     // Dernière instruction du constructeur, après
        ///     // l'affectation de toutes les dépendances propres.
        ///     InitializeLabels();
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        protected void InitializeLabels()
        {
            string callChain = BuildFirstCallChain();
            LoadLabels(callChain);
            _app.PropertyChanged += OnAppPropertyChangedHandler;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Handler de l'abonnement INPC à
        /// <see cref="ISE_App.PropertyChanged"/> : filtre les
        /// notifications sur <see cref="ISE_App.AppCultureCode"/>
        /// et marshalle le rechargement des libellés sur le thread
        /// UI.
        /// </summary>
        /// <param name="sender">Source de la notification
        /// (typiquement l'instance <see cref="ISE_App"/> injectée).</param>
        /// <param name="e">Argument décrivant la propriété modifiée.</param>
        /// <remarks>
        /// <para>Contexte : Handler <c>private</c> branché par
        /// <see cref="InitializeLabels"/>. Conformité R-4.11.9 et
        /// INV-VM-19.</para>
        ///
        /// <para>Comportement (trois temps) :</para>
        /// <list type="number">
        ///   <item><description>Filtrage : seule la propriété
        ///   <c>AppCultureCode</c> déclenche le rechargement.
        ///   Toute autre notification est silencieusement
        ///   ignorée.</description></item>
        ///   <item><description>Reconstruction de la CallChain via
        ///   <see cref="BuildFirstCallChain"/>, distincte de celle
        ///   du chargement initial.</description></item>
        ///   <item><description>Marshalling Dispatcher défensif :
        ///   le rechargement est marshalé sur le thread UI via
        ///   <c>Dispatcher.BeginInvoke</c> lorsque l'application
        ///   WPF est disponible
        ///   (<see cref="Application.Current"/> non
        ///   <see langword="null"/>) ; en l'absence
        ///   d'<see cref="Application"/> (contexte de tests
        ///   unitaires, démarrage tardif), l'appel à
        ///   <see cref="LoadLabels"/> est effectué
        ///   directement.</description></item>
        /// </list>
        /// </remarks>
        private void OnAppPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ISE_App.AppCultureCode))
            {
                return;
            }

            string callChain = BuildFirstCallChain();

            Dispatcher? dispatcher = Application.Current?.Dispatcher;
            if (dispatcher is null)
            {
                // Contexte non-WPF (tests unitaires, démarrage tardif).
                // Marshalling défensif : appel direct.
                LoadLabels(callChain);
                return;
            }

            dispatcher.BeginInvoke(new Action(() => LoadLabels(callChain)));
        }

        #endregion
    }
}