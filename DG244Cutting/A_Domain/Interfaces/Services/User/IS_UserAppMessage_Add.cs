using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier d'ajout d'un message applicatif à destination
    /// d'une application identifiée.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le UseCase orchestrateur d'ajout de message applicatif, qui en délègue
    /// l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserAppMessage_Add"/>
    /// résidant en <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de messages applicatifs inter-applications).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin d'ajout d'un
    /// enregistrement <c>UserAppMessage</c> à partir du contexte applicatif transporté par
    /// <see cref="DTO_AppContext"/> et des trois arguments métier portant le contenu propre
    /// du message (application destinataire, sujet, contenu textuel), sans exposer aux
    /// couches supérieures la délégation au Command Handler générique ni la convention
    /// de propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire d'ajout d'un message applicatif.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne lit pas le contexte applicatif : celui-ci est résolu par le UseCase et fourni via <see cref="DTO_AppContext"/>.</description></item>
    /// <item><description>Ne décrit pas la mise à jour (marquage lu) ni la suppression de message : ces actions relèvent de contrats distincts.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserAppMessage_Add
    {
        // --- Groupe 1 : Ajout d'un message applicatif ---

        /// <summary>
        /// Ajoute un nouvel enregistrement <c>UserAppMessage</c> à destination de
        /// l'application désignée par <paramref name="idApplicationRecipient"/>,
        /// à partir du contexte applicatif fourni et du contenu fourni.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase d'ajout de message applicatif, à l'intérieur
        /// de la transaction qu'il a ouverte. Le service applique sa logique
        /// fonctionnelle propre (construction de l'entité à partir du contexte applicatif
        /// et des arguments métier, positionnement de l'état non lu) puis délègue la
        /// mutation au Command Handler générique <c>IC_Generic&lt;UserAppMessage&gt;</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Construire l'entité <c>UserAppMessage</c> en état non lu, à partir du contexte applicatif et des arguments métier reçus.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format
        /// normatif de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="appContext">
        /// Contexte applicatif courant (identité application/utilisateur, horodatage
        /// applicatif), résolu et transmis par le UseCase orchestrateur. Ne doit pas
        /// être <see langword="null"/>.
        /// </param>
        /// <param name="idApplicationRecipient">
        /// Identifiant de l'application destinataire du message (référence
        /// <c>AppList(Id)</c>). Doit être strictement positif.
        /// </param>
        /// <param name="subject">
        /// Sujet du message. Ne doit être ni <see langword="null"/>, ni vide, ni
        /// constitué exclusivement d'espaces.
        /// </param>
        /// <param name="content">
        /// Contenu textuel du message. Peut être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> lorsque <paramref name="appContext"/> est
        /// <see langword="null"/> ou lorsque <paramref name="subject"/> est
        /// <see langword="null"/>, vide ou ne contient que des espaces ; avec le code
        /// <c>BU_ER_02</c> lorsque <paramref name="idApplicationRecipient"/> n'est pas
        /// strictement positif.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée lorsqu'une défaillance technique survient lors de la délégation au
        /// Command Handler.
        /// </exception>
        /// <exception cref="Ex_Unclassified">
        /// Levée par classification terminale lorsqu'une exception non typée du projet
        /// est interceptée et requalifiée par <c>IS_ExClassifier</c>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation coopérative est demandée via <paramref name="ct"/>.
        /// </exception>
        Task ExecuteAsync(
            string caller,
            DTO_AppContext appContext,
            int idApplicationRecipient,
            string subject,
            string? content,
            CancellationToken ct = default);
    }
}