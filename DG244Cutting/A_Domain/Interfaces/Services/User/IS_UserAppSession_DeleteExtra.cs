using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier de suppression des sessions utilisateur supplémentaires.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le UseCase orchestrateur du cycle de session, qui en délègue l'exécution au
    /// service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserAppSession_DeleteExtra"/>
    /// résidant en <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin de suppression
    /// physique des sessions supplémentaires afin d'éviter les doublons, sans exposer aux
    /// couches supérieures la délégation au Command Handler générique ni la convention de
    /// propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de suppression des sessions supplémentaires.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne détermine pas quelles sessions sont supplémentaires : la sélection relève du UseCase orchestrateur, qui fournit la collection.</description></item>
    /// <item><description>Ne décrit pas la création ni la mise à jour de session : ces actions relèvent de contrats distincts.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserAppSession_DeleteExtra
    {
        // --- Groupe 1 : Suppression des sessions supplémentaires ---

        /// <summary>
        /// Supprime physiquement les sessions supplémentaires fournies afin d'éviter
        /// la persistance de doublons.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur du cycle de session, à
        /// l'intérieur de la transaction qu'il a ouverte, lorsqu'une règle impose de ne
        /// conserver qu'une session pertinente. Le service ignore silencieusement les
        /// éléments null ou d'identifiant non strictement positif, puis délègue chaque
        /// suppression au Command Handler générique <c>IC_Generic&lt;UserAppSession&gt;</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Parcourir la collection fournie en ignorant les éléments non éligibles.</description></item>
        /// <item><description>Déléguer chaque suppression physique au Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="additionalSessions">
        /// Sessions supplémentaires à supprimer. Ne doit pas être <see langword="null"/> ;
        /// les éléments null ou d'identifiant non strictement positif sont ignorés.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée lorsque <paramref name="additionalSessions"/> est <see langword="null"/>.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors d'une suppression.</exception>
        Task ExecuteAsync(
            string caller,
            IEnumerable<UserAppSession> additionalSessions,
            CancellationToken ct = default);
    }
}