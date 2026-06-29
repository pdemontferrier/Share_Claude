using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.Infrastructure;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// UseCase orchestrateur du test unitaire de connectivité à la base de données
    /// partagée et de la propagation de l'état observé sur l'indicateur applicatif
    /// <see cref="ISE_App.IsConnected"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et
    /// réside en <c>B_UseCases/UseCases/App/</c>. Il est résolu par injection de
    /// dépendances et constitue le maillon orchestrant le diagnostic de
    /// connectivité à la base SQL partagée, consommé en chaîne (1) directe par un
    /// ViewModel de bandeau opérant en boucle de surveillance (typiquement
    /// <c>VM_BA_MainWindow</c> dans un fil ultérieur), sans sous-séquence amont
    /// prévue (signature <see cref="Task"/> simple, sans retour signalable —
    /// dimension iv-1 et iv-2 de la pré-qualification = NON, invariant 13 de §2.1
    /// et R-4.14.21 non applicables).
    /// </para>
    /// <para>
    /// Configuration typologique : cas Concept à méthode publique unique (segment
    /// <c>DigitTryDb</c> nom propre de base de données non rattaché à une entité
    /// du modèle de domaine, segment <c>TestConnection</c> action facultative
    /// qualifiant le concept porté en miroir nominal du Service Infrastructure
    /// consommé en aval ; R-4.14.7 amendée), domaine <c>App</c> (mécanique
    /// transverse de surveillance applicative), non transactionnel par
    /// construction (le scénario se limite à une délégation au Service
    /// Infrastructure suivie d'une propagation sur Singleton applicatif sans
    /// aucune mutation EF Core ; R-4.10.1 ; item UC14 de la checklist §4.3 sans
    /// objet). Combinaison structurelle alignée sur les étalons
    /// <c>UC_LogAndNotify</c> et <c>UC_Navigation</c> du domaine App ; aucune
    /// atypicité à signaler sur les quatre dimensions de pré-qualification.
    /// </para>
    /// <para>
    /// Objectif : garantir qu'à chaque invocation, l'état de connectivité à la
    /// base partagée soit déterminé par une délégation au Service Infrastructure
    /// <see cref="IS_DigitTryDb_TestConnection"/> et propagé sur l'indicateur
    /// applicatif <see cref="ISE_App.IsConnected"/> via les opérations atomiques
    /// <see cref="ISE_App.NotifyConnectionLost"/> ou
    /// <see cref="ISE_App.NotifyConnectionRestored"/>, et que toute défaillance
    /// applicative soit traitée terminalement sans propagation à l'appelant. La
    /// cadence d'invocation est hors périmètre : elle relève du consommateur
    /// amont (typiquement la boucle de surveillance de
    /// <c>VM_BA_MainWindow</c>).
    /// </para>
    /// <para>
    /// Écart doctrinal 1 - Asymétrie de mutation de
    /// <see cref="ISE_App.IsConnected"/> entre les trois catch typés (arbitrage
    /// Q-Asym-1 = Option B du fil de création). Le patron uniforme de trois
    /// catch typés (R-4.7.14, I-4.7.6) est respecté en structure, mais l'effet
    /// secondaire de mutation de l'indicateur applicatif est différencié selon
    /// la sémantique de chaque famille d'exception : (a) Ex_Business — aucune
    /// mutation, branche défensive injoignable en pratique (aucune précondition
    /// structurelle interne au présent UseCase ; le contrat
    /// <see cref="IS_DigitTryDb_TestConnection"/> ne documente pas Ex_Business
    /// comme exception levée ; catch défensif au sens de R-4.7.6 conservé par
    /// uniformité doctrinale) ; (b) Ex_Infrastructure —
    /// <see cref="ISE_App.NotifyConnectionLost"/>, perte de connexion
    /// techniquement identifiée par requalification du classifier en aval ;
    /// (c) Ex_Unclassified — <see cref="ISE_App.NotifyConnectionLost"/> par
    /// prudence, l'exception non classifiable ne permet pas de conclure à la
    /// disponibilité de la base ; laisser <see cref="ISE_App.IsConnected"/>
    /// dans son état antérieur entraînerait un risque d'état observable
    /// contradictoire côté bandeau de Présentation. Cet écart est sans
    /// incidence sur la conformité au patron canonique : R-4.7.14 prescrit
    /// l'existence des trois catch typés et leur ordre, sans prescrire de
    /// symétrie d'effet secondaire métier.
    /// </para>
    /// <para>
    /// Écart doctrinal 2 - Asymétrie du paramètre <c>notify</c> de
    /// <see cref="IU_LogAndNotify.ExecuteAsync"/> entre les trois catch typés
    /// (arbitrage Q-Asym-2 = Option B du fil de création). La journalisation
    /// reste exhaustive dans les trois catch (best-effort R-4.7.14) ; seule la
    /// notification opérateur est différenciée : (a) Ex_Business — défaut
    /// <c>notify = true</c>, signalement explicite d'une anomalie applicative
    /// inattendue au titre de la défense en profondeur ; (b) Ex_Infrastructure
    /// — <c>notify = false</c>, suppression de la notification IU_LogAndNotify
    /// car la modale de déconnexion opérateur est portée nativement par
    /// l'événement <see cref="ISE_App.ConnectionLost"/> déclenché par
    /// <see cref="ISE_App.NotifyConnectionLost"/> (cf. arbitrage Q-Asym-1) ;
    /// doubler la notification à chaque tick de la boucle de surveillance
    /// saturerait l'opérateur sans valeur ajoutée fonctionnelle ; (c)
    /// Ex_Unclassified — défaut <c>notify = true</c>, signalement explicite
    /// d'un cas inattendu non classifiable. Cet écart est sans incidence sur
    /// la conformité au patron canonique : le contrat
    /// <see cref="IU_LogAndNotify"/> expose <c>notify</c> en paramètre de
    /// modulation explicitement documenté avec valeur par défaut <c>true</c>.
    /// </para>
    /// <para>
    /// Périmètre comportemental — branche <see langword="else"/> du <c>try</c>.
    /// La branche <see langword="else"/> du retour binaire du Service
    /// Infrastructure (retour <see langword="false"/> de <c>Database.CanConnectAsync</c>
    /// sans exception sous-jacente) est instrumentée par levée volontaire d'une
    /// <see cref="Ex_Infrastructure"/> portant le code spécifique
    /// <see cref="DBCN_ER_01"/> conforme R-4.7.9 et R-4.7.10 (constante privée
    /// hébergée en région <c>=== Propriétés privées ===</c>, inscription
    /// concomitante au fichier d'inventaire
    /// <c>E_Miscellaneous/Documentation/Exceptions/SpecificErrorCodes.md</c>).
    /// La levée converge naturellement dans le <c>catch</c>
    /// (<see cref="Ex_Infrastructure"/>) existant, qui en absorbe terminalement
    /// la source au même titre que les requalifications du classifier amont
    /// (R-4.7.14 : appel à <see cref="IU_LogAndNotify"/> strictement dans un
    /// <c>catch</c>). Deux sources d'<see cref="Ex_Infrastructure"/> convergent
    /// ainsi dans ce catch, discriminables en log par
    /// <see cref="Ex_Infrastructure.ErrorId"/> :
    /// <list type="bullet">
    /// <item><description>Codes génériques <c>IN_ER_*</c> de
    /// <see cref="Ex_Infrastructure.ErrorCodes"/> — requalification du classifier
    /// amont via <c>IS_ExClassifier</c> côté Service Infrastructure, à partir
    /// d'une exception .NET ou tierce sous-jacente préservée en
    /// <see cref="Exception.InnerException"/>.</description></item>
    /// <item><description>Code spécifique <c>DBCN_ER_01</c> — levée intentionnelle
    /// de la branche <see langword="else"/>, sans exception sous-jacente
    /// (<see cref="Exception.InnerException"/> = <see langword="null"/>),
    /// conforme à R-4.7.11 par interprétation littérale (la règle prescrit
    /// l'<c>innerException</c> pour les <see cref="Ex_Infrastructure"/> d'origine
    /// tierce, ce qui n'est pas le cas d'une levée volontaire à motif
    /// intentionnel).</description></item>
    /// </list>
    /// Le message technique transporté respecte R-4.7.13 (français, texte libre,
    /// destiné aux logs et au support). Le pattern de levée volontaire est
    /// transposé de <c>UC_Application_OnStart.IdentifyDeviceUserAndOpenSessionAsync</c>
    /// (branche <c>if (!sessionOpened)</c>, levée d'<see cref="Ex_Business"/>
    /// avec code générique <see cref="Ex_Business.ErrorCodes.BU_ER_04"/>) à
    /// <see cref="Ex_Infrastructure"/> avec code spécifique conforme R-4.7.9.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Construire la CallChain au format normatif et la propager au Service Infrastructure consommé en aval (R-4.5.5, R-4.5.7).</description></item>
    /// <item><description>Déléguer le test de connectivité à <see cref="IS_DigitTryDb_TestConnection.ExecuteAsync"/>.</description></item>
    /// <item><description>Propager l'état observé sur le Singleton applicatif via <see cref="ISE_App.NotifyConnectionLost"/> ou <see cref="ISE_App.NotifyConnectionRestored"/>, de manière inconditionnelle côté UseCase (le setter de <see cref="ISE_App.IsConnected"/> protège l'idempotence INPC et événementielle).</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/> dans les trois <c>catch</c> typés selon la politique d'asymétrie arbitrée (R-4.7.14, I-4.7.6).</description></item>
    /// <item><description>Propager <see cref="OperationCanceledException"/> à l'appelant sans journalisation ni notification (§4.6, R-4.6.13).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune précondition structurelle validée par levée d'<see cref="Ex_Business"/> interne : la méthode publique ne reçoit que la CallChain amont et le jeton d'annulation ; l'item UC9 de la checklist §4.3 est sans objet sur le présent composant.</description></item>
    /// <item><description>N'ouvre aucune transaction et ne réalise aucune mutation EF Core : scénario non transactionnel par construction (I-4.10.1 trivialement respectée) ; l'item UC14 de la checklist §4.3 est sans objet sur le présent composant.</description></item>
    /// <item><description>Ne porte aucune primitive de test de connectivité ni aucune requalification d'exception : ces rôles relèvent du Service Infrastructure <see cref="IS_DigitTryDb_TestConnection"/> et de <c>IS_ExClassifier</c> consommé en aval (I-4.14.4 amendée).</description></item>
    /// <item><description>N'appelle jamais directement un Command Handler (I-4.14.4 amendée), un Query Handler ni un Repository.</description></item>
    /// <item><description>Ne pilote pas la cadence d'invocation : celle-ci relève du consommateur amont (typiquement la boucle de surveillance de <c>VM_BA_MainWindow</c>).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_DigitTryDb_TestConnection"/>
    public class UC_DigitTryDb_TestConnection : IU_DigitTryDb_TestConnection
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        /// <summary>
        /// Code spécifique d'erreur infrastructure au format <c>xxxx_ER_NN</c>
        /// (R-4.7.9) porté par la levée volontaire d'<see cref="Ex_Infrastructure"/>
        /// instrumentant la branche <see langword="else"/> du <c>try</c> de
        /// <see cref="ExecuteAsync"/> (retour binaire négatif du Service
        /// Infrastructure <see cref="IS_DigitTryDb_TestConnection"/> sans
        /// exception sous-jacente requalifiée).
        /// </summary>
        /// <remarks>
        /// <para>Convergence dans le <c>catch</c> (<see cref="Ex_Infrastructure"/>)
        /// existant au même titre que les requalifications du classifier amont
        /// portées par les codes génériques <see cref="Ex_Infrastructure.ErrorCodes"/>
        /// (préfixe <c>IN_ER_</c>). Les deux sources sont discriminables en log
        /// par la valeur de <see cref="Ex_Infrastructure.ErrorId"/> : <c>IN_ER_*</c>
        /// pour la requalification du classifier amont via <c>IS_ExClassifier</c>
        /// côté Service Infrastructure ; <c>DBCN_ER_01</c> pour la levée
        /// intentionnelle de la branche <see langword="else"/>.</para>
        /// <para>Inventaire centralisé en
        /// <c>E_Miscellaneous/Documentation/Exceptions/SpecificErrorCodes.md</c>
        /// conformément à R-4.7.10 (origine : Service technique ; composant
        /// générateur : <c>UC_DigitTryDb_TestConnection</c>).</para>
        /// </remarks>
        private const string DBCN_ER_01 = "DBCN_ER_01";

        #endregion

        #region === Dépendances privées ===

        private readonly IS_DigitTryDb_TestConnection _databaseConnectivity;
        private readonly ISE_App _settingsApp;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_DigitTryDb_TestConnection"/>
        /// avec ses dépendances.
        /// </summary>
        /// <param name="databaseConnectivity">
        /// Service Infrastructure de diagnostic binaire de connectivité à la base
        /// de données partagée, support du test unitaire délégué via
        /// <see cref="IS_DigitTryDb_TestConnection.ExecuteAsync"/>.
        /// </param>
        /// <param name="settingsApp">
        /// Singleton applicatif cible de la propagation de l'état observé via les
        /// opérations atomiques <see cref="ISE_App.NotifyConnectionLost"/> et
        /// <see cref="ISE_App.NotifyConnectionRestored"/>.
        /// </param>
        /// <param name="logAndNotify">
        /// Pipeline terminal de journalisation et de notification des erreurs.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si l'un des paramètres est <see langword="null"/>.
        /// </exception>
        public UC_DigitTryDb_TestConnection(
            IS_DigitTryDb_TestConnection databaseConnectivity,
            ISE_App settingsApp,
            IU_LogAndNotify logAndNotify)
        {
            _databaseConnectivity = databaseConnectivity ?? throw new ArgumentNullException(nameof(databaseConnectivity));
            _settingsApp = settingsApp ?? throw new ArgumentNullException(nameof(settingsApp));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Orchestre le test unitaire de connectivité à la base de données
        /// partagée puis propage l'état observé sur l'indicateur applicatif
        /// <see cref="ISE_App.IsConnected"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée depuis un ViewModel de bandeau en chaîne (1)
        /// directe, typiquement à chaque tick d'une boucle de surveillance
        /// orchestrée par <c>VM_BA_MainWindow</c>. La cadence d'invocation est
        /// hors périmètre du présent UseCase : elle relève strictement du
        /// consommateur amont. À chaque invocation, le UseCase délègue le test
        /// à <see cref="IS_DigitTryDb_TestConnection.ExecuteAsync"/>, puis
        /// propage l'état observé sur le Singleton applicatif via les
        /// opérations atomiques <see cref="ISE_App.NotifyConnectionLost"/> ou
        /// <see cref="ISE_App.NotifyConnectionRestored"/>. La propagation est
        /// inconditionnelle côté UseCase (sans test d'égalité préalable) : le
        /// setter de <see cref="ISE_App.IsConnected"/> protège l'émission INPC
        /// et celle des événements <see cref="ISE_App.ConnectionLost"/> et
        /// <see cref="ISE_App.ConnectionRestored"/> par un test d'égalité
        /// interne, garantissant l'idempotence observable côté consommateur
        /// (les appels successifs avec le même état n'émettent ni cascade INPC
        /// ni événement supplémentaire).
        /// </para>
        /// <para>
        /// Aucune précondition structurelle n'est validée à l'intérieur du
        /// <c>try</c> : la méthode publique ne reçoit que la CallChain amont
        /// (enrichie en première instruction effective) et le jeton
        /// d'annulation coopérative. Aucun input métier susceptible de violer
        /// un invariant structurel n'est consommé. En conséquence, le présent
        /// <c>ExecuteAsync</c> ne lève aucune <see cref="Ex_Business"/>
        /// interne ; le bloc <c>catch (Ex_Business)</c> est conservé par
        /// uniformité doctrinale (R-4.7.14) au titre de la défense en
        /// profondeur sans source identifiable en amont.
        /// </para>
        /// <para>
        /// Chemin nominal du <c>try</c> : (i) délégation du test de
        /// connectivité au Service Infrastructure, retour booléen ; (ii) si
        /// <see langword="true"/>, <see cref="ISE_App.NotifyConnectionRestored"/> ;
        /// si <see langword="false"/>, levée volontaire d'une
        /// <see cref="Ex_Infrastructure"/> portant le code spécifique
        /// <see cref="DBCN_ER_01"/> conforme R-4.7.9. La branche
        /// <see langword="false"/> du retour binaire du Service (retour négatif
        /// de la primitive EF Core <c>Database.CanConnectAsync</c> sans
        /// exception) correspond à une perte de connexion observée sans
        /// défaillance technique requalifiée ; elle est instrumentée par
        /// l'<see cref="Ex_Infrastructure"/> intentionnelle pour converger
        /// dans le <c>catch</c> (<see cref="Ex_Infrastructure"/>) existant
        /// (mutation <see cref="ISE_App.NotifyConnectionLost"/> et délégation
        /// à <see cref="IU_LogAndNotify"/> avec clé <c>No_EC_02</c> et
        /// <c>notify = false</c>). Elle ne lève pas <see cref="Ex_Business"/>
        /// (cas distinct de la branche analogue du UseCase
        /// <c>UC_Application_OnStart</c> qui refuse le démarrage applicatif
        /// par levée de BU_ER_04 — strictement hors périmètre du présent
        /// UseCase qui n'a pas vocation à refuser une exécution mais à
        /// observer en boucle). Le pattern de levée volontaire dans le chemin
        /// nominal d'un UseCase est lui en revanche transposé de
        /// <c>UC_Application_OnStart.IdentifyDeviceUserAndOpenSessionAsync</c>
        /// (étalon doctrinal du fil amont), avec substitution de la famille
        /// d'exception (<see cref="Ex_Infrastructure"/> au lieu d'
        /// <see cref="Ex_Business"/>) et du registre de code (spécifique
        /// <c>DBCN_ER_01</c> conforme R-4.7.9 au lieu de générique
        /// <c>BU_ER_04</c>).
        /// </para>
        /// <para>
        /// Traitement terminal des erreurs (politique d'asymétrie arbitrée,
        /// cf. <c>&lt;remarks&gt;</c> de classe pour la justification
        /// doctrinale) :
        /// </para>
        /// <list type="bullet">
        /// <item><description><c>catch (Ex_Business ex)</c> — Branche défensive injoignable en pratique ; aucune mutation de <see cref="ISE_App.IsConnected"/> ; délégation à <see cref="IU_LogAndNotify"/> avec clé <c>No_EC_01</c> et <c>notify = true</c> (défaut).</description></item>
        /// <item><description><c>catch (Ex_Infrastructure ex)</c> — Convergence terminale de deux sources discriminables par <see cref="Ex_Infrastructure.ErrorId"/> : (a) requalification du classifier amont via <c>IS_ExClassifier</c> côté Service Infrastructure (codes génériques <c>IN_ER_*</c>, avec <see cref="Exception.InnerException"/> préservée) ; (b) levée volontaire de la branche <see langword="else"/> du <c>try</c> portant le code spécifique <see cref="DBCN_ER_01"/> (sans <see cref="Exception.InnerException"/>, conforme R-4.7.11 par interprétation littérale). Dans les deux cas : <see cref="ISE_App.NotifyConnectionLost"/> ; délégation à <see cref="IU_LogAndNotify"/> avec clé <c>No_EC_02</c> et <c>notify = false</c> (journalisation seule, la modale opérateur étant portée par l'événement <see cref="ISE_App.ConnectionLost"/>).</description></item>
        /// <item><description><c>catch (Ex_Unclassified ex)</c> — Exception non classifiable ; <see cref="ISE_App.NotifyConnectionLost"/> par prudence ; délégation à <see cref="IU_LogAndNotify"/> avec clé <c>No_EC_03</c> et <c>notify = true</c> (défaut).</description></item>
        /// <item><description><c>catch (OperationCanceledException)</c> — Propagation directe à l'appelant (§4.6, R-4.6.13) ; aucune mutation de <see cref="ISE_App.IsConnected"/> ; aucun appel à <see cref="IU_LogAndNotify"/>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Par défaut <see langword="default"/>.
        /// </param>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée,
        /// conformément à §4.6. Les exceptions applicatives typées
        /// (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles
        /// sont captées terminalement par les trois <c>catch</c> typés et
        /// traitées par <see cref="IU_LogAndNotify"/> selon la politique
        /// d'asymétrie arbitrée.
        /// </exception>
        public async Task ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Délégation du test de connectivité au Service Infrastructure.
                // Aucune ouverture de transaction côté UseCase (non transactionnel
                // par construction) ; le DbContext de courte durée est produit et
                // disposé par le Service lui-même au titre du Pattern 3 du câblage
                // triple (§4.8.5 du 0230).
                bool isConnected = await _databaseConnectivity.ExecuteAsync(callChain, ct);

                // Propagation inconditionnelle de l'état observé sur le Singleton
                // applicatif. Le setter de ISE_App.IsConnected protège l'émission
                // INPC et celle des événements ConnectionLost / ConnectionRestored
                // par un test d'égalité interne (SetField) ; aucune comparaison
                // préalable n'est requise ici.
                if (isConnected)
                {
                    _settingsApp.NotifyConnectionRestored();
                }
                else
                {
                    // Levée volontaire d'Ex_Infrastructure portant le code spécifique
                    // DBCN_ER_01 : le retour binaire négatif du Service Infrastructure,
                    // sans exception sous-jacente requalifiée, est instrumenté en
                    // défaillance infrastructure intentionnelle pour converger dans le
                    // catch (Ex_Infrastructure) existant (mutation NotifyConnectionLost
                    // et journalisation par IU_LogAndNotify avec No_EC_02, notify: false).
                    // Pattern transposé de UC_Application_OnStart.IdentifyDeviceUserAndOpenSessionAsync
                    // (branche if (!sessionOpened), levée Ex_Business BU_ER_04) à Ex_Infrastructure
                    // avec code spécifique conforme R-4.7.9 et R-4.7.10. Absence d'innerException
                    // conforme à R-4.7.11 par interprétation littérale (levée volontaire sans
                    // exception sous-jacente, distincte des requalifications du classifier amont).
                    throw new Ex_Infrastructure(
                        callChain,
                        DBCN_ER_01,
                        "Échec du diagnostic de connectivité signalé par IS_DigitTryDb_TestConnection " +
                        "(retour false sans exception sous-jacente) ; perte de connexion observée.");
                }
            }
            catch (Ex_Business ex)
            {
                // Branche défensive injoignable en pratique (aucune précondition
                // structurelle interne ; le contrat IS_DigitTryDb_TestConnection
                // ne documente pas Ex_Business comme exception levée). Conservée
                // par uniformité doctrinale (R-4.7.14, I-4.7.6).
                // Q-Asym-1 Option B : aucune mutation de IsConnected.
                // Q-Asym-2 Option B : notify = true (défaut), signalement
                // explicite d'une anomalie applicative inattendue.
                // Pas de RollbackAsync : aucune transaction n'a été ouverte
                // (non transactionnel par construction).
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
            }
            catch (Ex_Infrastructure ex)
            {
                // Perte de connexion techniquement identifiée par requalification
                // du classifier côté Service Infrastructure.
                // Q-Asym-1 Option B : NotifyConnectionLost pour mise à jour de
                // l'état observable côté bandeau.
                _settingsApp.NotifyConnectionLost();
                // Q-Asym-2 Option B : notify = false, suppression de la
                // notification IU_LogAndNotify ; la modale opérateur est portée
                // nativement par l'événement ISE_App.ConnectionLost déclenché
                // ci-dessus. Évite la saturation à chaque tick de boucle.
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, notify: false, ct: ct);
            }
            catch (Ex_Unclassified ex)
            {
                // Exception non classifiable. La cause étant indéterminée quant à
                // la connectivité, on signale par prudence la perte de connexion
                // pour ne pas laisser le bandeau dans un état contradictoire.
                // Q-Asym-1 Option B : NotifyConnectionLost par prudence.
                _settingsApp.NotifyConnectionLost();
                // Q-Asym-2 Option B : notify = true (défaut), signalement
                // explicite d'un cas inattendu non couvert par la classification
                // standard.
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative : propagation directe à l'appelant
                // conformément à §4.6 et R-4.6.13.
                // Pas de RollbackAsync ici - aucune transaction n'a été ouverte.
                // Pas d'appel à IU_LogAndNotify - l'annulation n'est pas une erreur.
                // Pas de mutation de IsConnected - l'annulation n'est pas une
                // information sur la connectivité.
                throw;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}