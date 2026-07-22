using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Service métier responsable du marquage comme lu d'un message applicatif existant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/Services/User</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par le UseCase orchestrateur
    /// de marquage de message via son interface <see cref="IS_UserAppMessage_MarkAsRead"/>.
    /// </para>
    /// <para>
    /// Il constitue le maillon 3 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR
    /// (R-4.14.19) et est, pour l'entité <see cref="UserAppMessage"/>, un consommateur direct
    /// admissible d'<see cref="IC_Generic{T}"/> au titre de l'énoncé-parapluie de la dixième
    /// obligation contractuelle de §4.14.4 amendée. Il consomme également
    /// <see cref="IQ_Generic{T}"/> en amont de la mutation pour la lecture préalable de
    /// l'entité ciblée, opérée dans le même Service au titre de l'action métier unitaire
    /// « MarkAsRead », sans intercalation d'un DataProvider ni d'orchestration multi-étapes
    /// par appel d'un autre Service.
    /// </para>
    /// <para>
    /// Objectif : porter l'action métier unitaire de marquage comme lu, en chargeant l'entité
    /// par son identifiant, en positionnant <c>IsRead</c> à <see langword="true"/> sur
    /// l'instance chargée, puis en déléguant la mise à jour au Command Handler générique,
    /// sans exposer la logique de persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Valider la précondition structurelle <c>messageId &gt; 0</c>.</description></item>
    /// <item><description>Charger l'entité <see cref="UserAppMessage"/> via <see cref="IQ_Generic{T}.HandleGetByIdAsync"/>.</description></item>
    /// <item><description>Vérifier l'existence effective de l'entité chargée.</description></item>
    /// <item><description>Positionner <c>IsRead</c> à <see langword="true"/> sur l'entité chargée.</description></item>
    /// <item><description>Déléguer la mise à jour au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à chaque appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne positionne pas les champs d'audit : <c>UpdatedAt</c> est centralisé dans <c>CH_Generic&lt;T&gt;</c> par convention de réflexion.</description></item>
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction : ce rôle appartient au UseCase orchestrateur.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Query Handler et le Command Handler.</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserAppMessage_MarkAsRead"/>
    /// <seealso cref="IQ_Generic{T}"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_UserAppMessage_MarkAsRead : IS_UserAppMessage_MarkAsRead
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        private readonly IQ_Generic<UserAppMessage> _queryHandler;
        private readonly IC_Generic<UserAppMessage> _commandHandler;
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_UserAppMessage_MarkAsRead"/> avec ses dépendances.
        /// </summary>
        /// <param name="queryHandler">Query Handler générique consommé pour la lecture préalable du message ciblé.</param>
        /// <param name="commandHandler">Command Handler générique consommé pour la mise à jour du message.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserAppMessage_MarkAsRead(
            IQ_Generic<UserAppMessage> queryHandler,
            IC_Generic<UserAppMessage> commandHandler,
            IS_ExClassifier classifier)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _commandHandler = commandHandler ?? throw new ArgumentNullException(nameof(commandHandler));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Marque comme lu par son destinataire le message applicatif identifié par
        /// <paramref name="messageId"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de marquage de message, à
        /// l'intérieur de la transaction qu'il a ouverte. La lecture préalable est déléguée
        /// à <see cref="IQ_Generic{T}.HandleGetByIdAsync"/> ; la mise à jour est déléguée à
        /// <see cref="IC_Generic{T}.HandleUpdateAsync"/>. Le champ d'audit <c>UpdatedAt</c>
        /// est positionné par <c>CH_Generic&lt;T&gt;</c> par réflexion ; le Service n'y touche pas.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Vérifier la précondition structurelle <c>messageId &gt; 0</c>.</description></item>
        /// <item><description>Charger l'entité par son identifiant via le Query Handler générique.</description></item>
        /// <item><description>Vérifier l'existence effective de l'entité chargée.</description></item>
        /// <item><description>Positionner <c>IsRead = true</c> et déléguer la mise à jour au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne positionne pas les champs d'audit techniques (responsabilité de <c>CH_Generic&lt;T&gt;</c>).</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> ni ne pilote la transaction.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="messageId">Identifiant du message à marquer comme lu. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="messageId"/> est inférieur ou égal à zéro (code <c>BU_ER_02</c>)
        /// ou si aucun message n'existe pour cet identifiant (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture préalable ou de la mise à jour.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task ExecuteAsync(string caller, int messageId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (messageId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {nameof(UserAppMessage)} est invalide : {messageId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                UserAppMessage? entity = await _queryHandler.HandleGetByIdAsync(callChain, messageId, ct);

                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Aucun {nameof(UserAppMessage)} trouvé pour l'identifiant : {messageId}.");

                entity.IsRead = true;

                await _commandHandler.HandleUpdateAsync(callChain, entity, ct);
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

        // A compléter

        #endregion
    }
}