using System.Threading;
using DG244Cutting.A_Domain.Common.Enums;
using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// Description :
    /// <para>
    /// Contrat du service d’infrastructure responsable de la garantie d’unicité
    /// de l’instance de l’application DG244Cutting sur la session Windows
    /// courante, et du signalement à l’instance primaire préexistante de toute
    /// tentative de lancement d’une seconde instance afin de permettre à
    /// celle-ci de ramener sa fenêtre principale au premier plan.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// L’interface réside en <c>A_Domain/Interfaces/Services/Infrastructure/</c>
    /// (couche d’appartenance unique pour les contrats de la famille IS_,
    /// invariant 1 de §2.1 du 0232-SR). Son implémentation concrète
    /// <see cref="DG244Cutting.C_Infrastructure.Services.SR_SingleInstanceGuard"/>
    /// réside en <c>C_Infrastructure/Services/</c> conformément à la deuxième
    /// obligation contractuelle de §4.14.3 amendée du 0230 (sous-cas (b)
    /// Infrastructure). Le service est destiné à être consommé par injection
    /// IS_ depuis le UseCase orchestrateur de démarrage applicatif
    /// <c>UC_Application_OnStart</c> pour l’opération d’acquisition, et depuis
    /// le service de clôture applicative <c>SR_Shutdown</c> pour l’opération
    /// de libération.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Exposer, sous une forme abstraite et stable, une méthode publique unique
    /// multiplexée par un paramètre d’opération
    /// <see cref="En_SingleInstanceOperation"/>, permettant au démarrage de
    /// tenter l’acquisition d’unicité (avec signalement out-of-band à
    /// l’instance primaire préexistante en cas d’échec) et à la clôture de
    /// libérer proprement les ressources techniques d’unicité. Le résultat est
    /// un binaire polymorphe selon l’opération : pour Acquire,
    /// <see langword="true"/> si l’instance courante est primaire,
    /// <see langword="false"/> si une instance primaire préexistait et a été
    /// signalée ; pour Release, <see langword="true"/> systématiquement
    /// (retour nominal).
    /// </para>
    ///
    /// Rôle (cas Concept) :
    /// <para>
    /// Service porteur d’un concept d’infrastructure transverse (garde
    /// d’unicité d’instance applicative sur la session Windows). Le nom d’agent
    /// <c>Guard</c> absorbe la sémantique d’action ; le segment [Action] est
    /// facultatif en cas Concept (R-4.14.8 amendée du 0231). Le service ne
    /// porte aucune action métier ni applicative unitaire sur une entité
    /// identifiée : il n’est pas producteur de mutation, pas consommateur de
    /// <c>IC_</c>, pas consommateur de <c>IQ_</c>, pas producteur de DTO_ par
    /// agrégation. Patron nominatif analogue aux étalons doctrinaux
    /// <c>SR_ExClassifier</c>, <c>SR_UseCaseInvoker</c> et
    /// <c>SR_MainWindowActivator</c>.
    /// </para>
    ///
    /// Obligations contractuelles :
    /// <list type="bullet">
    /// <item><description>Résider en <c>A_Domain/Interfaces/Services/Infrastructure/</c>.</description></item>
    /// <item><description>Implémentation par <see cref="DG244Cutting.C_Infrastructure.Services.SR_SingleInstanceGuard"/> active en <c>C_Infrastructure/Services/</c> (invariant 1 de §2.1 du 0232-SR).</description></item>
    /// <item><description>Signature de traçabilité sur la méthode publique : <c>string caller</c> et <c>CancellationToken ct = default</c> aux positions canoniques (R-4.5.5, R-4.6.13 du 0231).</description></item>
    /// <item><description>Multiplexage par le paramètre discriminant <see cref="En_SingleInstanceOperation"/> désigné en tête de signature.</description></item>
    /// <item><description>Pureté contractuelle : aucune référence à une primitive noyau Windows, à un mécanisme WPF, ou à tout type technique étranger à <c>A_Domain</c> (I-2.4.1, I-2.4.2, IS5). Les primitives Win32 mobilisées sont intégralement encapsulées dans l’implémentation.</description></item>
    /// </list>
    ///
    /// Responsabilités :
    /// <list type="bullet">
    /// <item><description>Déclarer l’opération unitaire multiplexée par le paramètre <see cref="En_SingleInstanceOperation"/>.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l’annulation coopérative via un <see cref="CancellationToken"/>.</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune décision d’auto-terminaison du processus courant : sur un retour <see langword="false"/> de l’opération Acquire, la responsabilité de la sortie propre du processus relève du consommateur amont (typiquement <c>UC_Application_OnStart</c>).</description></item>
    /// <item><description>Aucune notification directe à l’utilisateur : la posture « silencieuse pure » côté seconde instance est portée en frontière de contrat, aucun message n’est présenté à l’utilisateur au titre du signalement d’unicité (I-4.7.6).</description></item>
    /// <item><description>Aucune journalisation directe via <c>IS_ErrorLogger</c> (I-4.7.6, hors-portée EA-09).</description></item>
    /// <item><description>Aucune mutation persistante ni participation à la chaîne (1) d’écriture stricte (I-4.14.9, I-4.14.16 du 0231).</description></item>
    /// <item><description>Aucun appel direct à un Repository, à un Command Handler ou à un Query Handler (I-4.14.6, I-4.14.9 du 0231).</description></item>
    /// <item><description>Aucune ouverture, validation ou annulation de transaction (I-4.10.1 du 0231).</description></item>
    /// <item><description>Aucune orchestration de scénario par appel d’un autre Service applicatif (I-4.14.6). La consommation d’<c>IS_MainWindowActivator</c> par le thread d’écoute interne, au réveil du signal de seconde instance, est admise au titre du service transversal d’utilité (§4.7 du 0230, patron analogue à la consommation d’<c>IS_ExClassifier</c>).</description></item>
    /// </list>
    /// </summary>
    /// <seealso cref="En_SingleInstanceOperation"/>
    /// <seealso cref="DG244Cutting.C_Infrastructure.Services.SR_SingleInstanceGuard"/>
    public interface IS_SingleInstanceGuard
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>Sollicite l’opération d’unicité d’instance désignée par
        /// <paramref name="operation"/> : acquisition au démarrage applicatif,
        /// libération à la clôture applicative.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique unique du service d’infrastructure de garde
        /// d’unicité (cas Concept). Nommée <c>Execute</c> conformément au
        /// préfixe par défaut R-4.2.12 du 0231. Multiplexée par
        /// <paramref name="operation"/> discriminant les deux branches Acquire
        /// et Release.</para>
        /// Objectif :
        /// <para>Exposer sous forme opaque une frontière de retour
        /// <see langword="bool"/> polymorphe selon l’opération :
        /// (i) pour Acquire, discrimination entre instance primaire acquise
        /// (<see langword="true"/>) et seconde instance détectée avec signal
        /// envoyé à l’instance primaire préexistante (<see langword="false"/>) ;
        /// sur <see langword="false"/>, la responsabilité d’auto-terminaison
        /// propre du processus courant relève du consommateur amont ;
        /// (ii) pour Release, retour nominal <see langword="true"/>, la
        /// libération étant idempotente y compris en cas d’appel orphelin
        /// (Release sans Acquire préalable réussi, cas dégradé sans effet
        /// de bord).</para>
        /// Comportement du thread d’écoute côté instance primaire :
        /// <para>Sur retour <see langword="true"/> de Acquire, l’implémentation
        /// démarre en interne un thread d’écoute en attente bloquante sur la
        /// primitive noyau de signalement. Au réveil (signal envoyé par une
        /// seconde instance ayant échoué à acquérir l’unicité), le thread
        /// sollicite la remontée programmatique de la fenêtre principale via
        /// <c>IS_MainWindowActivator</c>. Le thread est arrêté proprement lors
        /// de l’opération Release par un <c>CancellationTokenSource</c>
        /// interne, indépendant du <paramref name="ct"/> public (le
        /// <paramref name="ct"/> public gouverne la seule exécution de la
        /// méthode <c>Execute</c>, non le cycle de vie du thread d’écoute).</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant, enrichie localement au format normatif de §4.5 du 0230.</param>
        /// <param name="operation">Opération multiplexée à exécuter : <see cref="En_SingleInstanceOperation.Acquire"/> ou <see cref="En_SingleInstanceOperation.Release"/>.</param>
        /// <param name="ct">Jeton d’annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// Pour <see cref="En_SingleInstanceOperation.Acquire"/> :
        /// <see langword="true"/> si l’instance courante est l’instance
        /// primaire de la session Windows, <see langword="false"/> si une
        /// instance primaire préexistait et a été signalée avec succès.
        /// Pour <see cref="En_SingleInstanceOperation.Release"/> :
        /// <see langword="true"/> systématiquement (retour nominal).
        /// </returns>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c> en cas de défaillance technique identifiable sur les primitives noyau mobilisées par l’implémentation.</exception>
        /// <exception cref="System.OperationCanceledException">Levée lorsque l’annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        bool Execute(string caller, En_SingleInstanceOperation operation, CancellationToken ct = default);

        #endregion
    }
}