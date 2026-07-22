using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier d'inscription en mise à jour d'un utilisateur
    /// applicatif existant, à partir de sa page d'administration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le UseCase orchestrateur de mise à jour d'utilisateur, qui en délègue l'exécution
    /// au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserApp_Update"/> résidant en
    /// <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine <c>User</c>.
    /// Maillon SR de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR (R-4.14.19).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin d'inscription en
    /// mise à jour d'un enregistrement <see cref="UserApp"/> existant, fourni sous la forme
    /// d'une copie de travail détachée et complète, sans exposer aux couches supérieures la
    /// délégation au Command Handler générique ni la convention de propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de mise à jour d'un utilisateur applicatif.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne charge pas l'enregistrement existant ni ne mappe sélectivement : la copie de travail lui est fournie complète et détachée en amont.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans <see cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/> par réflexion (R-4.15.7).</description></item>
    /// <item><description>Ne décrit ni la création ni la suppression d'un utilisateur : ces actions relèvent de contrats distincts.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserApp_Update
    {
        // --- Groupe 1 : Mise à jour d'un utilisateur applicatif ---

        /// <summary>
        /// Inscrit en mise à jour l'utilisateur applicatif fourni comme copie de travail
        /// détachée et complète, après validation des invariants structurels d'entrée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de mise à jour d'utilisateur, à
        /// l'intérieur de la transaction qu'il a ouverte. Le service valide les préconditions
        /// structurelles sur la copie de travail reçue, puis délègue la mutation au Command
        /// Handler générique <c>IC_Generic&lt;UserApp&gt;</c>. La copie de travail est reçue
        /// telle quelle : les <c>Initials</c> sont déjà calculées en amont, le
        /// <c>PasswordHash</c> est déjà haché en amont, et l'<c>Id</c> de l'enregistrement
        /// cible est renseigné.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="entity"/> (non-nullité, identifiant cible, champs requis).</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne charge pas l'existant ni ne mappe sélectivement : la fidélité des champs persistés, y compris ceux non exposés par la page, est garantie en amont.</description></item>
        /// <item><description>Ne contrôle pas l'unicité du <c>Login</c> : la clé métier est verrouillée en modification.</description></item>
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique (R-4.15.7).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="entity">
        /// Copie de travail détachée et complète de l'utilisateur à mettre à jour : tous les
        /// champs persistés reflètent l'état voulu, les <c>Initials</c> sont déjà calculées,
        /// le <c>PasswordHash</c> est déjà haché, et l'<c>Id</c> de l'enregistrement cible est
        /// renseigné. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> lorsque <paramref name="entity"/> est
        /// <see langword="null"/> ou lorsque l'un des champs requis (<c>LastName</c>,
        /// <c>FirstName</c>, <c>Login</c>, <c>PasswordHash</c>, <c>Initials</c>) est
        /// <see langword="null"/>, vide ou ne contient que des espaces ; avec le code
        /// <c>BU_ER_02</c> lorsque <c>entity.Id</c> n'est pas strictement positif.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée lorsqu'une défaillance technique survient lors de la délégation au Command Handler.
        /// </exception>
        /// <exception cref="Ex_Unclassified">
        /// Levée par classification terminale lorsqu'une exception non typée du projet est
        /// interceptée et requalifiée par <c>IS_ExClassifier</c>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation coopérative est demandée via <paramref name="ct"/>.
        /// </exception>
        Task ExecuteAsync(
            string caller,
            UserApp entity,
            CancellationToken ct = default);
    }
}