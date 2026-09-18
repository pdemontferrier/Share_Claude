using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Service métier responsable de l'ajout en base d'un nouvel enregistrement
    /// <see cref="UserApp"/> à partir d'une entité de travail déjà préparée en amont.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce service appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/Services/User</c>. Il est résolu par injection de dépendances et ne
    /// doit jamais être instancié directement. Il est consommé par le UseCase de création
    /// d'utilisateur via son interface <see cref="IS_UserApp_Create"/> ; il est le
    /// consommateur direct unique de <see cref="IC_Generic{T}"/> pour l'entité
    /// <see cref="UserApp"/>, selon le patron normatif de consommation par injection
    /// (§4.15.3) et l'énoncé-parapluie de la dixième obligation contractuelle amendée de
    /// §4.14.4. Maillon 3 de la chaîne (1) d'écriture stricte VM → UC → SR → CH → CR
    /// (R-4.14.19).
    /// </para>
    /// <para>
    /// Objectif : porter l'action métier unitaire d'ajout d'un compte utilisateur, en
    /// validant les invariants d'entrée structurels de l'entité reçue puis en déléguant
    /// la mutation d'insertion au Command Handler générique, sans exposer la logique de
    /// persistance ni assumer de responsabilité transactionnelle.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Valider les préconditions structurelles sur l'entité et ses champs obligatoires <c>LastName</c>, <c>FirstName</c>, <c>Login</c>, <c>PasswordHash</c>.</description></item>
    /// <item><description>Déléguer l'écriture au Command Handler générique via <see cref="IC_Generic{T}.HandleAddAsync"/>.</description></item>
    /// <item><description>Propager la CallChain et le jeton d'annulation à l'appel aval.</description></item>
    /// <item><description>Requalifier les exceptions non prévues via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'ouvre, ne valide ni n'annule aucune transaction : ce rôle appartient au UseCase orchestrateur.</description></item>
    /// <item><description>Ne calcule ni le hachage du mot de passe, ni les initiales, ni aucune valeur métier par défaut : l'entité est fournie déjà préparée par l'amont.</description></item>
    /// <item><description>Ne porte pas le contrôle d'unicité du Login : celui-ci est assuré par le UseCase orchestrateur ; aucune injection d'<c>IQ_</c>.</description></item>
    /// <item><description>N'injecte aucune interface <c>ISE_</c> : aucune valeur de configuration n'est nécessaire à l'action, l'entité lui étant fournie par argument depuis le UseCase.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : l'accès aux données passe par le Command Handler.</description></item>
    /// <item><description>Ne positionne pas les champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) : cette responsabilité est centralisée dans <see cref="IC_Generic{T}"/> par réflexion (R-4.15.7).</description></item>
    /// <item><description>Ne journalise ni ne notifie : ces rôles relèvent du pipeline terminal du UseCase.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IS_UserApp_Create"/>
    /// <seealso cref="IC_Generic{T}"/>
    public class SR_UserApp_Create : IS_UserApp_Create
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
        /// Initialise une nouvelle instance de <see cref="SR_UserApp_Create"/> avec ses dépendances.
        /// </summary>
        /// <param name="chGeneric">Command Handler générique consommé pour l'écriture du compte utilisateur.</param>
        /// <param name="classifier">Service de classification des exceptions non contrôlées.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public SR_UserApp_Create(
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
        /// Ajoute un nouvel enregistrement <see cref="UserApp"/> en base à partir de
        /// l'entité neuve déjà préparée fournie par l'amont.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le UseCase orchestrateur de création d'utilisateur, à
        /// l'intérieur de la transaction qu'il a ouverte et après qu'il a assuré le
        /// contrôle d'unicité du Login. La mutation effective est déléguée au Command
        /// Handler générique, qui positionne les champs d'audit et inscrit l'enregistrement
        /// Event Store associé.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="entity"/> et ses champs obligatoires.</description></item>
        /// <item><description>Déléguer l'écriture au Command Handler générique, l'entité étant transmise telle quelle.</description></item>
        /// </list>
        /// <para>Non-responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Ne positionne pas les champs d'audit : cette responsabilité est centralisée dans le Command Handler générique (R-4.15.7).</description></item>
        /// <item><description>Ne reconstruit ni ne remappe l'entité : elle est déléguée telle quelle (insertion d'une entité neuve).</description></item>
        /// <item><description>N'appelle pas <c>SaveChangesAsync</c> ni ne pilote la transaction.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="entity">
        /// Entité <see cref="UserApp"/> neuve et déjà préparée en amont (mot de passe haché,
        /// initiales calculées, valeurs métier par défaut positionnées). Ne doit pas être
        /// <see langword="null"/> et ses champs obligatoires <c>LastName</c>, <c>FirstName</c>,
        /// <c>Login</c> et <c>PasswordHash</c> ne doivent être ni vides ni constitués
        /// exclusivement d'espaces.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée avec le code <c>BU_ER_01</c> si <paramref name="entity"/> est
        /// <see langword="null"/>, ou si l'un de ses champs obligatoires <c>LastName</c>,
        /// <c>FirstName</c>, <c>Login</c> ou <c>PasswordHash</c> est <see langword="null"/>,
        /// vide ou ne contient que des espaces.
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
                        $"L'entité fournie pour la création d'un {nameof(UserApp)} est nulle.");

                if (string.IsNullOrWhiteSpace(entity.LastName))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le nom de famille fourni pour la création d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.FirstName))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prénom fourni pour la création d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.Login))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le login fourni pour la création d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                if (string.IsNullOrWhiteSpace(entity.PasswordHash))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le mot de passe haché fourni pour la création d'un {nameof(UserApp)} est nul, vide ou ne contient que des espaces.");

                ct.ThrowIfCancellationRequested();

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