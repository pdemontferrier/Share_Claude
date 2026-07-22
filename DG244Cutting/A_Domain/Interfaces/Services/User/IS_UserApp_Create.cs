using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;

namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Contrat du service métier d'ajout en base d'un nouvel enregistrement
    /// <c>UserApp</c> à partir d'une entité de travail déjà préparée en amont.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le UseCase orchestrateur de création d'utilisateur, qui en délègue l'exécution
    /// au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserApp_Create"/> résidant en
    /// <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine <c>User</c>
    /// (cycle de vie des comptes utilisateurs).
    /// </para>
    /// <para>
    /// Objectif : exprimer, sous une forme abstraite et stable, le besoin d'ajout d'un
    /// enregistrement <see cref="UserApp"/> déjà préparé en amont (entité neuve dont le
    /// mot de passe est déjà haché, les initiales déjà calculées et les valeurs métier
    /// par défaut déjà positionnées), sans exposer aux couches supérieures la délégation
    /// au Command Handler générique ni la convention de propagation de la CallChain.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire d'ajout d'un compte utilisateur.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne calcule ni le hachage du mot de passe, ni les initiales, ni aucune valeur métier par défaut : l'entité est fournie déjà préparée par l'amont.</description></item>
    /// <item><description>Ne porte pas le contrôle d'unicité du Login : celui-ci est assuré par le UseCase orchestrateur.</description></item>
    /// <item><description>Ne décrit pas la mise à jour ni la suppression d'un compte utilisateur : ces actions relèvent de contrats distincts.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.Handlers.Generic.IC_Generic{T}"/>
    public interface IS_UserApp_Create
    {
        // --- Groupe 1 : Ajout d'un compte utilisateur ---

        /// <summary>
        /// Ajoute un nouvel enregistrement <see cref="UserApp"/> en base à partir de
        /// l'entité neuve déjà préparée fournie par l'amont.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase de création d'utilisateur, à l'intérieur de
        /// la transaction qu'il a ouverte et après qu'il a assuré le contrôle d'unicité du
        /// Login. Le service valide les invariants d'entrée structurels puis délègue la
        /// mutation d'insertion au Command Handler générique
        /// <c>IC_Generic&lt;UserApp&gt;</c>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur l'entité et ses champs obligatoires.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format
        /// normatif de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="entity">
        /// Entité <see cref="UserApp"/> neuve et déjà préparée en amont (mot de passe haché,
        /// initiales calculées, valeurs métier par défaut positionnées). Ne doit pas être
        /// <see langword="null"/> et ses champs obligatoires <c>LastName</c>, <c>FirstName</c>,
        /// <c>Login</c> et <c>PasswordHash</c> ne doivent être ni vides ni constitués
        /// exclusivement d'espaces.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> lorsque <paramref name="entity"/> est
        /// <see langword="null"/>, ou lorsque l'un de ses champs obligatoires
        /// <c>LastName</c>, <c>FirstName</c>, <c>Login</c> ou <c>PasswordHash</c> est
        /// <see langword="null"/>, vide ou ne contient que des espaces.
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
            UserApp entity,
            CancellationToken ct = default);
    }
}