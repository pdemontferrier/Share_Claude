using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier de marquage comme lu d'un message applicatif existant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.3 amendée, 1ère obligation). Elle est consommée par
    /// injection de dépendances par le UseCase orchestrateur de marquage de message
    /// (UC_UserAppMessage_MarkAsRead, à produire dans un fil distinct), qui en délègue
    /// l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserAppMessage_MarkAsRead"/>
    /// résidant en <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (messagerie applicative).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin de marquage
    /// comme lu d'un message applicatif identifié par son entier, sans exposer aux couches
    /// supérieures la délégation au Query Handler générique (lecture préalable), la
    /// délégation au Command Handler générique (mise à jour) ni la convention de
    /// propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de marquage comme lu d'un message identifié.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne décrit ni la création, ni l'envoi, ni la suppression du message : ces actions relèvent de contrats distincts.</description></item>
    /// <item><description>N'expose pas les modalités d'accès aux données : la lecture et l'écriture sont confiées aux Handlers en aval.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IQ_Generic{T}"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserAppMessage_MarkAsRead
    {
        // --- Groupe 1 : Marquage de lecture ---

        /// <summary>
        /// Marque comme lu par son destinataire le message applicatif identifié par son entier.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de marquage, à l'intérieur de la
        /// transaction qu'il a ouverte. Le service charge l'entité ciblée via
        /// <c>IQ_Generic&lt;UserAppMessage&gt;</c>, positionne la propriété <c>IsRead</c> à
        /// <see langword="true"/> sur l'instance chargée, puis délègue la mise à jour au
        /// Command Handler générique <c>IC_Generic&lt;UserAppMessage&gt;</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="messageId"/>.</description></item>
        /// <item><description>Charger le message visé via le Query Handler générique.</description></item>
        /// <item><description>Positionner <c>IsRead</c> à <see langword="true"/> sur l'entité chargée.</description></item>
        /// <item><description>Déléguer la mise à jour au Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="messageId">
        /// Identifiant du message applicatif à marquer comme lu. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée lorsque <paramref name="messageId"/> est inférieur ou égal à zéro (code <c>BU_ER_02</c>),
        /// ou lorsqu'aucun message n'existe pour cet identifiant (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée lorsqu'une défaillance technique survient lors de la lecture préalable ou de la mise à jour.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task ExecuteAsync(string caller, int messageId, CancellationToken ct = default);
    }
}