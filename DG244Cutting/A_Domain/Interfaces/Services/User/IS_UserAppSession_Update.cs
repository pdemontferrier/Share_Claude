using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier de mise à jour de l'état connecté/déconnecté d'une session utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le UseCase orchestrateur d'ouverture ou de fermeture de programme, qui en délègue
    /// l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserAppSession_Update"/>
    /// résidant en <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin de mise à jour
    /// d'une session existante (informations device, statut connecté/déconnecté, date de
    /// connexion ou de déconnexion) à partir du contexte applicatif transporté par
    /// <see cref="DTO_AppContext"/>, sans exposer aux couches supérieures la délégation
    /// au Command Handler générique ni la convention de propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de mise à jour d'une session utilisateur.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne lit pas le contexte applicatif : celui-ci est résolu par le UseCase et fourni via <see cref="DTO_AppContext"/>.</description></item>
    /// <item><description>Ne décrit pas la création ni la suppression de session : ces actions relèvent de contrats distincts.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserAppSession_Update
    {
        // --- Groupe 1 : Mise à jour de session ---

        /// <summary>
        /// Met à jour une session existante avec l'état connecté/déconnecté, les informations
        /// device et la date correspondante issus du contexte applicatif fourni.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée à l'ouverture (état connecté) ou à la fermeture (état déconnecté)
        /// du programme, à l'intérieur de la transaction ouverte par le UseCase orchestrateur.
        /// Le service applique sa logique fonctionnelle propre (positionnement conditionnel des
        /// dates selon l'état) puis délègue la mutation au Command Handler générique
        /// <c>IC_Generic&lt;UserAppSession&gt;</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Mettre à jour les informations device et le statut de connexion de la session.</description></item>
        /// <item><description>Positionner la date de connexion ou de déconnexion selon l'état demandé.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="session">
        /// Session existante à mettre à jour. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="isConnected">
        /// Nouvel état de la session : <see langword="true"/> pour une session connectée
        /// (positionne la date de connexion et annule la date de déconnexion) ;
        /// <see langword="false"/> pour une session déconnectée (positionne la date de déconnexion).
        /// </param>
        /// <param name="appContext">
        /// Contexte applicatif courant (device, horodatage applicatif), résolu et transmis
        /// par le UseCase orchestrateur. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée lorsque <paramref name="session"/> ou <paramref name="appContext"/> est <see langword="null"/>.</exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors de l'écriture.</exception>
        Task ExecuteAsync(
            string caller,
            UserAppSession session,
            bool isConnected,
            DTO_AppContext appContext,
            CancellationToken ct = default);
    }
}