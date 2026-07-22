using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Service métier responsable de l'inscription en mise à jour d'un utilisateur applicatif
    /// existant, à partir de sa page d'administration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/Services/User</c>. Il est résolu par injection de dépendances et ne doit
    /// jamais être instancié directement. Il est consommé par le UseCase de mise à jour
    /// d'utilisateur via son interface <see cref="IS_UserApp_Update"/> ; il est le consommateur
    /// direct de <see cref="IC_Generic{T}"/> pour l'entité <see cref="UserApp"/>, selon le patron
    /// normatif de consommation par injection (§4.15.3) et l'énoncé-parapluie de la dixième
    /// obligation contractuelle amendée de §4.14.4. Maillon 3 de la chaîne (1) d'écriture stricte
    /// VM → UC → SR → CH → CR (R-4.14.19).
    /// </para>
    /// <para>
    /// Objectif : porter l'action métier unitaire de mise à jour d'un utilisateur applicatif, en
    /// validant les invariants structurels de la copie de travail reçue puis en déléguant la
    /// mutation au Command Handler générique, sans exposer la logique de persistance ni assumer
    /// de responsabilité transactionnelle. La copie de travail est reçue détachée et complète :
    /// le service ne charge pas l'existant et ne mappe pas sélectivement les champs.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Valider les préconditions structurelles sur l'entité reçue (non-nullité, identifiant cible, champs requis).</description></item>
    /// <item><description>Déléguer l'écriture au Command Handler générique via <see cref="IC_Generic{T}.HandleUpdateAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à l'appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction : ce rôle appartient au UseCase orchestrateur.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> ni <c>IS_AppContext</c> : la copie de travail lui est fournie complète par argument depuis le UseCase.</description></item>
    /// <item><description>Ne charge pas l'existant ni ne mappe sélectivement : la fidélité des champs persistés, y compris ceux non exposés par la page, est garantie en amont.</description></item>
    /// <item><description>Ne contrôle pas l'unicité du <c>Login</c> : la clé métier est verrouillée en modification.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : cette responsabilité est centralisée dans <see cref="IC_Generic{T}"/> par réflexion (R-4.15.7).</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserApp_Update"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_UserApp_Update : IS_UserApp_Update
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IC_Generic<UserApp> _chGeneric;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_UserApp_Update"/> avec ses dépendances.
        /// </summary>
        /// <param name="chGeneric">Command Handler générique consommé pour l'écriture de l'utilisateur applicatif.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserApp_Update(
            IC_Generic<UserApp> chGeneric,
            IS_ExClassifier classifier)
        {
            _chGeneric = chGeneric ?? throw new ArgumentNullException(nameof(chGeneric));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Inscrit en mise à jour l'utilisateur applicatif fourni comme copie de travail
        /// détachée et complète, après validation des invariants structurels d'entrée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de mise à jour d'utilisateur, à
        /// l'intérieur de la transaction qu'il a ouverte. La mutation effective est déléguée au
        /// Command Handler générique, qui positionne le champ <c>UpdatedAt</c> à l'UTC courant et
        /// inscrit l'enregistrement Event Store associé.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="entity"/>.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne charge pas l'existant ni ne mappe sélectivement : la copie de travail est reçue complète.</description></item>
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique (R-4.15.7).</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> ni ne pilote la transaction.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="entity">
        /// Copie de travail détachée et complète de l'utilisateur à mettre à jour : tous les
        /// champs persistés reflètent l'état voulu, les <c>Initials</c> sont déjà calculées, le
        /// <c>PasswordHash</c> est déjà haché, et l'<c>Id</c> de l'enregistrement cible est
        /// renseigné. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="entity"/> est
        /// <see langword="null"/> ou si l'un des champs requis (<c>LastName</c>, <c>FirstName</c>,
        /// <c>Login</c>, <c>PasswordHash</c>, <c>Initials</c>) est <see langword="null"/>, vide ou
        /// ne contient que des espaces ; avec le code <c>BU_ER_02</c> si <c>entity.Id</c> n'est
        /// pas strictement positif.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si l'écriture en base échoue lors de la délégation au Command Handler.
        /// </exception>
        /// <exception cref="Ex_Unclassified">
        /// Levée par classification terminale lorsqu'une exception non typée du projet est
        /// interceptée et requalifiée par <see cref="IS_ExClassifier"/>.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation coopérative est demandée via <paramref name="ct"/>.
        /// </exception>
        public async Task ExecuteAsync(
            string caller,
            UserApp entity,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"L'utilisateur fourni pour la mise à jour d'un {nameof(UserApp)} est nul.");

                if (entity.Id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de l'{nameof(UserApp)} à mettre à jour est invalide : {entity.Id}. Doit être strictement positif.");

                if (string.IsNullOrWhiteSpace(entity.LastName))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le nom de famille fourni pour la mise à jour d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.FirstName))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prénom fourni pour la mise à jour d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.Login))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le login fourni pour la mise à jour d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.PasswordHash))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le mot de passe haché fourni pour la mise à jour d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.Initials))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Les initiales fournies pour la mise à jour d'un {nameof(UserApp)} sont nulles, vides ou ne contiennent que des espaces.");

                ct.ThrowIfCancellationRequested();

                await _chGeneric.HandleUpdateAsync(callChain, entity, ct);
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