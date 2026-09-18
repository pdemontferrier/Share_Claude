using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier de création d'une session utilisateur en état connecté.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le UseCase orchestrateur d'ouverture de programme, qui en délègue l'exécution
    /// au service concret <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserAppSession_Create"/>
    /// résidant en <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin de création
    /// d'un enregistrement de session connectée à partir du contexte applicatif transporté
    /// par <see cref="DTO_AppContext"/>, sans exposer aux couches supérieures la délégation
    /// au Command Handler générique ni la convention de propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de création d'une session utilisateur connectée.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne lit pas le contexte applicatif : celui-ci est résolu par le UseCase et fourni via <see cref="DTO_AppContext"/>.</description></item>
    /// <item><description>Ne décrit pas la mise à jour ni la suppression de session : ces actions relèvent de contrats distincts.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserAppSession_Create
    {
        // --- Groupe 1 : Création de session ---

        /// <summary>
        /// Crée un nouvel enregistrement de session utilisateur en état connecté à partir
        /// du contexte applicatif fourni.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase d'ouverture de programme, à l'intérieur de la
        /// transaction qu'il a ouverte. Le service applique sa logique fonctionnelle propre
        /// puis délègue la mutation au Command Handler générique <c>IC_Generic&lt;UserAppSession&gt;</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire l'entité de session connectée sans date de déconnexion.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="appContext">
        /// Contexte applicatif courant (identité application/utilisateur, device, horodatage
        /// applicatif), résolu et transmis par le UseCase orchestrateur. Ne doit pas être
        /// <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée lorsque <paramref name="appContext"/> est <see langword="null"/> ou qu'une précondition structurelle n'est pas satisfaite.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors de l'écriture.</exception>
        Task ExecuteAsync(string caller, DTO_AppContext appContext, CancellationToken ct = default);
    }
}