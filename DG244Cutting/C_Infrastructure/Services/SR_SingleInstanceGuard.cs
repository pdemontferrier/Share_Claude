using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;

namespace DG244Cutting.C_Infrastructure.Services
{
    /// <summary>
    /// Description :
    /// <para>
    /// Service d’infrastructure de garde d’unicité de l’instance de
    /// l’application DG244Cutting sur la session Windows courante. Implémente
    /// <see cref="IS_SingleInstanceGuard"/>. À l’acquisition, sérialise
    /// l’instance courante via un Mutex nommé Win32 scopé <c>Local\</c>. Sur
    /// tentative de lancement d’une seconde instance, la seconde signale
    /// l’instance primaire préexistante via un EventWaitHandle nommé Win32
    /// (<c>EventResetMode.AutoReset</c>) et retourne <see langword="false"/>
    /// au consommateur amont pour auto-terminaison propre. Côté instance
    /// primaire, un thread d’écoute long-running attend le signal et sollicite
    /// la remontée programmatique de la fenêtre principale via
    /// <see cref="IS_MainWindowActivator"/>.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Réside en <c>C_Infrastructure/Services/</c> conformément à la deuxième
    /// obligation contractuelle de §4.14.3 amendée du 0230 (interface en
    /// <c>Services/Infrastructure/</c> → implémentation en
    /// <c>C_Infrastructure/Services/</c>, sous-cas (b) Infrastructure).
    /// Enregistré en Singleton dans <c>SR_ConteneurDI</c>, portée admise au
    /// titre de P4-bis (§4.10.10 du 0230, R-4.10.14 du 0231) : les deux
    /// dépendances injectées (<see cref="IS_ExClassifier"/>,
    /// <see cref="IS_MainWindowActivator"/>) sont Singleton, aucune dépendance
    /// scoped n’est consommée. Le cycle de vie Singleton est cohérent avec
    /// l’état persistant tenu par le service entre l’opération Acquire
    /// (démarrage applicatif) et l’opération Release (clôture applicative) :
    /// handle du Mutex nommé, EventWaitHandle nommé, thread d’écoute et
    /// CancellationTokenSource interne.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Exposer une méthode publique unique <c>Execute</c> multiplexée par
    /// <see cref="En_SingleInstanceOperation"/>, encapsulant intégralement les
    /// primitives noyau Win32 mobilisées (Mutex nommé, EventWaitHandle nommé)
    /// et la gestion du thread d’écoute côté instance primaire. Frontière de
    /// retour <see langword="bool"/> polymorphe : Acquire true/false selon
    /// primaire/seconde, Release true nominal. Application du patron à quatre
    /// catch canonique sur la méthode publique (§4.7 ; R-4.7.1, R-4.7.6,
    /// R-4.7.25, R-4.6.13).
    /// </para>
    ///
    /// Rôle (cas Concept) :
    /// <para>
    /// Service porteur d’un concept d’infrastructure transverse (garde
    /// d’unicité d’instance). Le nom d’agent <c>Guard</c> absorbe la
    /// sémantique d’action ; segment [Action] facultatif absent (SR20 du
    /// 0232-SR, patron nominatif analogue à <see cref="IS_ExClassifier"/>).
    /// Méthode publique unique nommée <c>Execute</c> conformément au préfixe
    /// par défaut R-4.2.12 ; aucune dérogation SR20 mobilisée.
    /// </para>
    ///
    /// Obligations contractuelles :
    /// <list type="bullet">
    /// <item><description>Construire la CallChain en première instruction effective de la méthode publique au format <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c> (R-4.5.5).</description></item>
    /// <item><description>Appliquer le patron à quatre catch dans l’ordre canonique sur la méthode publique (R-4.6.13, R-4.7.25).</description></item>
    /// <item><description>Encapsuler intégralement les primitives noyau Win32 : aucun type <c>Mutex</c>, <c>EventWaitHandle</c>, <c>Task</c>, <c>Thread</c> ou <c>CancellationTokenSource</c> exposé en frontière publique du contrat (IS5).</description></item>
    /// <item><description>Nommer les primitives noyau via constantes privées scopées <c>Local\</c> pour restreindre l’objet noyau à la session Windows courante (deux comptes Windows distincts sur la même machine disposent chacun de leur unicité).</description></item>
    /// </list>
    ///
    /// Comportements spécifiques :
    /// <list type="bullet">
    /// <item><description>À l’acquisition, capture ciblée de <see cref="AbandonedMutexException"/> à la construction du Mutex traitée comme succès d’acquisition (l’instance courante devient primaire ; le processus antérieur détenteur du Mutex sans libération propre est considéré terminé).</description></item>
    /// <item><description>Idempotence Acquire : double Acquire sur instance déjà primaire retourne <see langword="true"/> sans effet de bord (flag interne <c>_acquired</c>).</description></item>
    /// <item><description>Idempotence Release : Release orphelin (sans Acquire préalable réussi, cas seconde instance ayant appelé Release avant auto-terminaison) retourne <see langword="true"/> sans effet de bord (flag interne <c>_acquired</c>).</description></item>
    /// <item><description>Séparation stricte du <c>ct</c> public (courte durée, gouverne l’exécution de <c>Execute</c>) et du <c>_listenerCts</c> interne (longue durée, gouverne le cycle de vie du thread d’écoute) ; aucune propagation entre les deux.</description></item>
    /// <item><description>Thread d’écoute : boucle <c>WaitHandle.WaitAny</c> avec catch large silencieux et sortie de boucle sur exception non prévue (posture silencieuse pure ; aucune journalisation directe possible depuis un SR Infrastructure hors EA-09, I-4.7.6).</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune décision d’auto-terminaison du processus courant sur retour <see langword="false"/> de Acquire (relève du consommateur amont).</description></item>
    /// <item><description>Aucune mutation persistante ni participation à la chaîne (1) d’écriture stricte (SR24, SR25 ➖).</description></item>
    /// <item><description>Aucun appel direct à un Repository (I-4.14.6).</description></item>
    /// <item><description>Aucun appel à un Command Handler ou à un Query Handler (I-4.14.6, I-4.14.9).</description></item>
    /// <item><description>Aucune journalisation directe via <c>IS_ErrorLogger</c> ni notification directe via <c>IS_Notification</c> (I-4.7.6, hors EA-09).</description></item>
    /// <item><description>Aucune ouverture, validation ou annulation de transaction (I-4.10.1).</description></item>
    /// </list>
    /// </summary>
    /// <seealso cref="IS_SingleInstanceGuard"/>
    /// <seealso cref="IS_MainWindowActivator"/>
    /// <seealso cref="IS_ExClassifier"/>
    public class SR_SingleInstanceGuard : IS_SingleInstanceGuard
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement par <c>GetType().Name</c>
        /// pour la construction du segment local de la CallChain (§4.5 ; R-4.5.5).
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Nom Win32 du Mutex nommé scopé <c>Local\</c> à la session Windows
        /// courante, versionné <c>v1</c> pour permettre une évolution ultérieure
        /// sans collision avec des instances antérieures encore en mémoire.
        /// </summary>
        private const string MutexName = @"Local\DG244Cutting.SingleInstance.v1";

        /// <summary>
        /// Nom Win32 de l’EventWaitHandle nommé scopé <c>Local\</c> à la session
        /// Windows courante, versionné <c>v1</c>. Support du canal de signalement
        /// out-of-band de la seconde instance vers l’instance primaire.
        /// </summary>
        private const string EventName = @"Local\DG244Cutting.Activation.v1";

        /// <summary>
        /// Délai de garde d’attente de la terminaison propre du thread d’écoute
        /// à l’opération Release. En cas de dépassement, la clôture applicative
        /// n’est pas bloquée : le thread termine ultérieurement sur détection
        /// de l’annulation.
        /// </summary>
        private static readonly TimeSpan ListenerJoinTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Flag interne discriminant les quatre cas d’ordonnancement du
        /// consommateur amont : Acquire normal (<see langword="false"/> → <see langword="true"/>),
        /// double Acquire (<see langword="true"/> → <see langword="true"/>, no-op),
        /// Release normal (<see langword="true"/> → <see langword="false"/>),
        /// Release orphelin (<see langword="false"/> → <see langword="false"/>, no-op).
        /// </summary>
        private bool _acquired;

        /// <summary>
        /// Mutex nommé Win32 détenu par l’instance primaire pour la durée de
        /// vie de l’application. <see langword="null"/> tant que l’acquisition
        /// n’a pas eu lieu ou après libération.
        /// </summary>
        private Mutex? _mutex;

        /// <summary>
        /// EventWaitHandle nommé Win32 créé côté instance primaire comme canal
        /// de signalement out-of-band de la seconde instance vers la primaire.
        /// <see langword="null"/> tant que l’acquisition n’a pas eu lieu ou
        /// après libération.
        /// </summary>
        private EventWaitHandle? _activationEvent;

        /// <summary>
        /// Source d’annulation coopérative interne, indépendante du <c>ct</c>
        /// public, gouvernant le cycle de vie du thread d’écoute côté instance
        /// primaire. Annulée à l’opération Release pour arrêter proprement le
        /// thread bloqué en <c>WaitHandle.WaitAny</c>.
        /// </summary>
        private CancellationTokenSource? _listenerCts;

        /// <summary>
        /// Référence sur le thread d’écoute long-running démarré à
        /// l’acquisition côté instance primaire. Utilisée à la libération pour
        /// attendre la terminaison propre du thread avec délai de garde
        /// <see cref="ListenerJoinTimeout"/>.
        /// </summary>
        private Task? _listenerTask;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types
        /// applicatifs normalisés (<see cref="Ex_Infrastructure"/> ou
        /// <see cref="Ex_Unclassified"/>), consommé dans le catch
        /// <c>Exception</c> terminal du patron à quatre catch de la méthode
        /// publique <c>Execute</c> (§4.7 ; R-4.7.25).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        /// <summary>
        /// Service transversal de présentation sollicité par le thread
        /// d’écoute interne au réveil du signal envoyé par une seconde
        /// instance, pour la remontée programmatique de la fenêtre principale
        /// au premier plan de la session utilisateur Windows.
        /// </summary>
        private readonly IS_MainWindowActivator _activator;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>Initialise une nouvelle instance du service
        /// <see cref="SR_SingleInstanceGuard"/>.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Instancié par le conteneur d’injection de dépendances dans la
        /// couche Infrastructure (Singleton, cf. enregistrement dans
        /// <c>SR_ConteneurDI</c>, section <c>// Infrastructure</c>). Les deux
        /// dépendances injectées sont de portée Singleton, admissibles au
        /// titre de la portée Singleton du service au regard de P4-bis
        /// (§4.10.10 du 0230).</para>
        /// Objectif :
        /// <para>Initialiser le champ <c>_callee</c> par réflexion sur le type
        /// courant en première instruction (R-4.5.5, patron
        /// <see cref="IS_MainWindowActivator"/>) et valider les dépendances
        /// obligatoires par garde <see cref="ArgumentNullException"/>.</para>
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee = GetType().Name</c> en première instruction (R-4.5.5).</description></item>
        /// <item><description>Valider et stocker <see cref="IS_ExClassifier"/>.</description></item>
        /// <item><description>Valider et stocker <see cref="IS_MainWindowActivator"/>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="classifier">Service transversal d’utilité de requalification des exceptions brutes en exceptions typées (R-4.7.25).</param>
        /// <param name="activator">Service transversal de présentation sollicité par le thread d’écoute au réveil du signal envoyé par une seconde instance.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="classifier"/> ou <paramref name="activator"/> est <see langword="null"/>.</exception>
        public SR_SingleInstanceGuard(
            IS_ExClassifier classifier,
            IS_MainWindowActivator activator)
        {
            _callee = GetType().Name;
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _activator = activator ?? throw new ArgumentNullException(nameof(activator));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <inheritdoc/>
        public bool Execute(string caller, En_SingleInstanceOperation operation, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Execute)}";

            try
            {
                // Aucune précondition structurelle à valider sur caller (patron
                // SR_DigitTryDb_TestConnection, arbitrage Q-8 = B du fil
                // SR_SingleInstanceGuard_Creation). Position validation -> ct
                // dégénérée à ct seul, conforme à §4.7 (R-4.7.25).
                ct.ThrowIfCancellationRequested();

                // Multiplexage par l'opération demandée. La forme switch expression
                // avec discard force implicitement la couverture : toute valeur
                // d'énumération hors définition est requalifiée par le catch
                // terminal via _classifier (patron à quatre catch canonique,
                // Q-4 = A du fil SR_SingleInstanceGuard_Creation).
                return operation switch
                {
                    En_SingleInstanceOperation.Acquire => AcquireInternal(callChain, ct),
                    En_SingleInstanceOperation.Release => ReleaseInternal(callChain, ct),
                    _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, "Valeur d’opération non prise en charge.")
                };
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
        /// Branche Acquire : tente d’acquérir l’unicité d’instance via
        /// construction du Mutex nommé Win32. Sur succès (instance primaire),
        /// crée l’EventWaitHandle nommé et démarre le thread d’écoute. Sur
        /// échec (seconde instance), signale l’instance primaire préexistante
        /// via l’EventWaitHandle et retourne <see langword="false"/>.
        /// </summary>
        /// <param name="callChain">CallChain enrichie de la méthode publique appelante.</param>
        /// <param name="ct">Jeton d’annulation coopérative de la méthode publique appelante.</param>
        /// <returns><see langword="true"/> si l’instance courante est primaire, <see langword="false"/> si une instance primaire préexistait.</returns>
        private bool AcquireInternal(string callChain, CancellationToken ct)
        {
            // Idempotence Acquire : double Acquire sur instance déjà primaire
            // retourne true sans effet de bord (arbitrage N-3 du fil).
            if (_acquired)
                return true;

            ct.ThrowIfCancellationRequested();

            bool createdNew;
            try
            {
                // Tentative d'acquisition initiale du Mutex nommé Win32.
                // L'argument initiallyOwned = true réserve la propriété du Mutex
                // à l'instance courante en cas de création (createdNew = true).
                _mutex = new Mutex(true, MutexName, out createdNew);
            }
            catch (AbandonedMutexException)
            {
                // Q-4 = A : capture ciblée à la construction. Un processus
                // antérieur détenait le Mutex sans le libérer (crash, terminaison
                // brutale). L'instance courante a néanmoins acquis le Mutex :
                // elle devient la nouvelle instance primaire. Poursuite du flux
                // d'acquisition primaire.
                createdNew = true;
            }

            if (!createdNew)
            {
                // Seconde instance : une instance primaire détient déjà le Mutex.
                // Signalement out-of-band via l'EventWaitHandle nommé existant
                // (posture silencieuse pure, arbitrage Q-3 = A et Q-5 du fil).
                try
                {
                    using EventWaitHandle existingEvent = EventWaitHandle.OpenExisting(EventName);
                    existingEvent.Set();
                }
                finally
                {
                    // Libération de la référence locale au Mutex : l'instance
                    // secondaire n'en détient pas la propriété. Dispose systématique.
                    _mutex?.Dispose();
                    _mutex = null;
                }

                // Retour au consommateur amont pour auto-terminaison propre du
                // processus courant (relève du consommateur, non du service).
                return false;
            }

            // Instance primaire : création de l'EventWaitHandle nommé (canal de
            // signalement) et démarrage du thread d'écoute long-running.
            _activationEvent = new EventWaitHandle(false, EventResetMode.AutoReset, EventName);
            _listenerCts = new CancellationTokenSource();

            CancellationToken listenerCt = _listenerCts.Token;
            _listenerTask = Task.Factory.StartNew(
                () => ListenerLoop(listenerCt),
                listenerCt,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            _acquired = true;
            return true;
        }

        /// <summary>
        /// Branche Release : libère les ressources techniques d’unicité
        /// d’instance. Arrête proprement le thread d’écoute via annulation
        /// coopérative interne, dispose les primitives noyau Win32, libère le
        /// Mutex. Idempotente en cas de Release orphelin (seconde instance ou
        /// Release avant Acquire).
        /// </summary>
        /// <param name="callChain">CallChain enrichie de la méthode publique appelante.</param>
        /// <param name="ct">Jeton d’annulation coopérative de la méthode publique appelante.</param>
        /// <returns><see langword="true"/> systématiquement (retour nominal ; posture idempotente arbitrages N-3 et Q-10 = B du fil).</returns>
        private bool ReleaseInternal(string callChain, CancellationToken ct)
        {
            // Idempotence Release : Release orphelin (sans Acquire préalable
            // réussi) retourne true sans effet de bord (arbitrages N-3 et
            // Q-10 = B du fil). Cas typique : seconde instance ayant appelé
            // Release par prudence avant auto-terminaison ; ou Release avant
            // Acquire côté consommateur amont.
            if (!_acquired)
                return true;

            ct.ThrowIfCancellationRequested();

            // Signal d'arrêt au thread d'écoute (indépendant du ct public,
            // arbitrage N-2 du fil).
            _listenerCts?.Cancel();

            // Attente de la terminaison propre du thread avec délai de garde.
            // En cas de dépassement, la clôture applicative n'est pas bloquée :
            // le thread termine ultérieurement sur détection de l'annulation.
            try
            {
                _listenerTask?.Wait(ListenerJoinTimeout);
            }
            catch (AggregateException)
            {
                // Toute exception remontée par le thread d'écoute est absorbée
                // à la jointure (posture silencieuse pure, arbitrage Q-11 = A) ;
                // la libération des ressources doit se poursuivre sans être
                // interrompue par l'état résiduel du thread.
            }

            // Libération des ressources noyau dans l'ordre inverse d'acquisition.
            _activationEvent?.Dispose();
            _activationEvent = null;

            _listenerCts?.Dispose();
            _listenerCts = null;

            _listenerTask = null;

            if (_mutex is not null)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch (ApplicationException)
                {
                    // Le Mutex n'était pas détenu par le thread courant : cas
                    // dégradé sans conséquence sur la libération (le Dispose
                    // ci-après ferme le handle Win32 sous-jacent quoi qu'il
                    // arrive).
                }

                _mutex.Dispose();
                _mutex = null;
            }

            _acquired = false;
            return true;
        }

        /// <summary>
        /// Boucle du thread d’écoute côté instance primaire. Attend
        /// bloquantement un signal sur l’EventWaitHandle nommé ou une
        /// annulation coopérative interne. Au réveil par signal, sollicite la
        /// remontée programmatique de la fenêtre principale via
        /// <see cref="IS_MainWindowActivator"/>. Catch large silencieux avec
        /// sortie de boucle sur exception non prévue (arbitrage Q-11 = A du
        /// fil).
        /// </summary>
        /// <param name="listenerCt">Jeton d’annulation coopérative interne, indépendant du <c>ct</c> public de la méthode <c>Execute</c>.</param>
        private void ListenerLoop(CancellationToken listenerCt)
        {
            // Snapshot local de l'event à l'entrée de boucle : le champ
            // _activationEvent est susceptible d'être disposé par ReleaseInternal
            // pendant l'exécution du thread, la référence locale reste stable
            // pour la durée de l'attente.
            EventWaitHandle? snapshot = _activationEvent;
            if (snapshot is null)
                return;

            WaitHandle[] handles = new WaitHandle[] { snapshot, listenerCt.WaitHandle };

            while (!listenerCt.IsCancellationRequested)
            {
                int signaled;
                try
                {
                    // Attente bloquante multi-handles : réveil sur signal event
                    // (index 0, seconde instance détectée) ou sur annulation
                    // coopérative interne (index 1, Release en cours).
                    signaled = WaitHandle.WaitAny(handles);
                }
                catch
                {
                    // Q-11 = A : catch large silencieux avec sortie de boucle
                    // sur exception non prévue (Mutex/Event disposé sous nos
                    // pieds, ObjectDisposedException, ...). Aucune journalisation
                    // directe possible depuis un SR Infrastructure hors EA-09
                    // (I-4.7.6). Le service reste à un état stable ; l'anomalie
                    // est visible au comportement (plus de rappel programmatique
                    // de la MainWindow).
                    break;
                }

                // Index 1 = annulation coopérative interne (Release engagée) :
                // sortie propre de la boucle.
                if (signaled == 1)
                    break;

                // Index 0 = signal reçu d'une seconde instance : sollicitation
                // de la remontée programmatique de la fenêtre principale.
                // CallChain d'origine thread interne au format
                // "{_callee}.ListenerLoop" (arbitrage N-1 du fil).
                try
                {
                    _activator.Execute($"{_callee}.ListenerLoop", listenerCt);
                }
                catch
                {
                    // Q-11 = A : catch large silencieux avec sortie de boucle.
                    // Toute exception typée remontée par l'activator (Ex_Business,
                    // Ex_Infrastructure, OperationCanceledException) ou brute
                    // est absorbée ; sortie propre.
                    break;
                }
            }
        }

        #endregion
    }
}