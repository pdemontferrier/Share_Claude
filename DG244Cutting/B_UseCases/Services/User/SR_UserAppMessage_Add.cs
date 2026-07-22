using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Service métier responsable de l'ajout d'un message applicatif à destination
    /// d'une application identifiée, en état non lu.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/Services/User</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par le UseCase d'ajout de
    /// message applicatif via son interface <see cref="IS_UserAppMessage_Add"/> ; il est le
    /// consommateur direct unique de <see cref="IC_Generic{T}"/> pour l'entité
    /// <see cref="UserAppMessage"/>, selon le patron normatif de consommation par injection
    /// (§4.15.3) et l'énoncé-parapluie de la dixième obligation contractuelle amendée de
    /// §4.14.4. Maillon 3 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR
    /// (R-4.14.19).
    /// </para>
    /// <para>
    /// Objectif : porter l'action métier unitaire d'ajout d'un message applicatif, en
    /// construisant l'entité à partir du <see cref="DTO_AppContext"/> reçu et des trois
    /// arguments métier (application destinataire, sujet, contenu) puis en déléguant la
    /// mutation au Command Handler générique, sans exposer la logique de persistance ni
    /// assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Valider les préconditions structurelles sur le contexte applicatif, l'identifiant destinataire et le sujet.</description></item>
    /// <item><description>Construire l'entité <see cref="UserAppMessage"/> en état non lu, à partir du contexte applicatif et des arguments métier.</description></item>
    /// <item><description>Déléguer l'écriture au Command Handler générique via <see cref="IC_Generic{T}.HandleAddAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à l'appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction : ce rôle appartient au UseCase orchestrateur.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> ni <c>IS_AppContext</c> : le contexte applicatif lui est fourni par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : cette responsabilité est centralisée dans <see cref="IC_Generic{T}"/> par réflexion (R-4.15.7).</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserAppMessage_Add"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_UserAppMessage_Add : IS_UserAppMessage_Add
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IC_Generic<UserAppMessage> _chGeneric;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_UserAppMessage_Add"/> avec ses dépendances.
        /// </summary>
        /// <param name="chGeneric">Command Handler générique consommé pour l'écriture du message applicatif.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserAppMessage_Add(
            IC_Generic<UserAppMessage> chGeneric,
            IS_ExClassifier classifier)
        {
            _chGeneric = chGeneric ?? throw new ArgumentNullException(nameof(chGeneric));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Ajoute un nouvel enregistrement <see cref="UserAppMessage"/> à destination de
        /// l'application désignée par <paramref name="idApplicationRecipient"/>, à partir
        /// du contexte applicatif fourni et du contenu fourni.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur d'ajout de message applicatif,
        /// à l'intérieur de la transaction qu'il a ouverte. La mutation effective est
        /// déléguée au Command Handler générique, qui positionne les champs d'audit et
        /// inscrit l'enregistrement Event Store associé.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="appContext"/>, <paramref name="idApplicationRecipient"/> et <paramref name="subject"/>.</description></item>
        /// <item><description>Construire l'entité <see cref="UserAppMessage"/> en état non lu.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique (R-4.15.7).</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> ni ne pilote la transaction.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="appContext">
        /// Contexte applicatif courant fournissant l'identité application/utilisateur et
        /// l'horodatage applicatif. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idApplicationRecipient">
        /// Identifiant de l'application destinataire du message. Doit être strictement positif.
        /// </param>
        /// <param name="subject">
        /// Sujet du message. Ne doit être ni <see langword="null"/>, ni vide, ni constitué
        /// exclusivement d'espaces.
        /// </param>
        /// <param name="content">Contenu textuel du message. Peut être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="appContext"/> est
        /// <see langword="null"/> ou si <paramref name="subject"/> est <see langword="null"/>,
        /// vide ou ne contient que des espaces ; avec le code <c>BU_ER_02</c> si
        /// <paramref name="idApplicationRecipient"/> n'est pas strictement positif.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si l'écriture en base échoue lors de la délégation au Command Handler.
        /// </exception>
        /// <exception cref="Ex_Unclassified">
        /// Levée par classification terminale lorsqu'une exception non typée du projet
        /// est interceptée et requalifiée par <see cref="IS_ExClassifier"/>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation coopérative est demandée via <paramref name="ct"/>.
        /// </exception>
        public async Task ExecuteAsync(
            string caller,
            DTO_AppContext appContext,
            int idApplicationRecipient,
            string subject,
            string? content,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (appContext is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le contexte applicatif fourni pour la création d'un {nameof(UserAppMessage)} est nul.");

                if (idApplicationRecipient <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de l'application destinataire fourni pour {nameof(UserAppMessage)} est invalide : {idApplicationRecipient}. Doit être strictement positif.");

                if (string.IsNullOrWhiteSpace(subject))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le sujet fourni pour la création d'un {nameof(UserAppMessage)} est nul, vide ou ne contient que des espaces.");

                ct.ThrowIfCancellationRequested();

                var entity = new UserAppMessage
                {
                    IdApplicationSender = appContext.AppId,
                    IdUserSender = appContext.AppUserId,
                    IdApplicationRecipient = idApplicationRecipient,
                    SentAt = appContext.AppDateTime,
                    Subject = subject,
                    Content = content,
                    IsRead = false
                };

                await _chGeneric.HandleAddAsync(callChain, entity, ct);
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