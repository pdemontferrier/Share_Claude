using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.User;

namespace DG244Cutting.B_UseCases.Services.User
{
    /// <summary>
    /// Description :
    /// <para>
    /// Service métier spécialisé responsable de la vérification de l’accès d’un utilisateur
    /// à une application donnée.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Ce service est utilisé dans le processus de contrôle d’accès applicatif, après
    /// identification préalable de l’utilisateur et de l’application. Il s’appuie sur
    /// un Query Handler dédié afin d’interroger les autorisations enregistrées en base
    /// de données, sans accéder directement à l’infrastructure technique.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Déterminer si un utilisateur possède un droit d’accès à l’application ciblée,
    /// en respectant les conventions du projet en matière de traçabilité, de découplage
    /// et de classification des exceptions.
    /// </para>
    ///
    /// Utilisateurs cibles :
    /// <para>
    /// UseCases de la couche Application chargés de piloter les scénarios
    /// d’authentification, d’initialisation ou de contrôle d’accès.
    /// </para>
    ///
    /// Tâches / Actions :
    /// <list type="bullet">
    /// <item><description>Construire la CallChain locale du service.</description></item>
    /// <item><description>Déléguer la lecture du droit d’accès au Query Handler <see cref="IQ_UserAppAccess"/>.</description></item>
    /// <item><description>Retourner le résultat booléen du contrôle d’accès applicatif.</description></item>
    /// <item><description>Classifier toute exception via <see cref="SR_ExClassifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class SR_User_CheckAppAccess : IS_User_CheckAppAccess
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserAppAccess _qhUserAppAccess;
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>
        /// Initialise une nouvelle instance du service <see cref="SR_User_CheckAppAccess"/>.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette classe est instanciée via l’injection de dépendances dans la couche
        /// Application. Elle reçoit le Query Handler responsable de la lecture des
        /// autorisations d’accès utilisateur / application.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Préparer le service à exécuter les contrôles d’accès applicatifs dans un cadre
        /// conforme à la Clean Architecture et aux conventions de traçabilité du projet.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Recevoir le Query Handler <see cref="IQ_UserAppAccess"/>.</description></item>
        /// <item><description>Initialiser la variable <c>_callee</c> avec le nom réel de la classe.</description></item>
        /// <item><description>Stocker les dépendances nécessaires au fonctionnement du service.</description></item>
        /// </list>
        /// </summary>
        /// <param name="qhUserAppAccess">Query Handler de lecture des accès utilisateur / application.</param>
        /// <exception cref="ArgumentNullException">Levée si le Query Handler injecté est nul.</exception>
        public SR_User_CheckAppAccess(
            IQ_UserAppAccess qhUserAppAccess,
            IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _qhUserAppAccess = qhUserAppAccess ?? throw new ArgumentNullException(nameof(qhUserAppAccess));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>
        /// Vérifie si un utilisateur possède un droit d’accès sur une application donnée.
        /// </para>
        ///
        /// Contexte :
        /// <para>
        /// Cette méthode est appelée depuis un UseCase d’accès ou d’initialisation.
        /// Elle constitue une opération métier unitaire, limitée au contrôle du lien
        /// d’autorisation entre un utilisateur et une application.
        /// </para>
        ///
        /// Objectif :
        /// <para>
        /// Retourner <c>true</c> si un accès existe, sinon <c>false</c>, tout en
        /// garantissant la traçabilité complète et la reclassification normalisée
        /// des erreurs.
        /// </para>
        ///
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Construire la CallChain du service.</description></item>
        /// <item><description>Propager l’appel au Query Handler <see cref="IQ_UserAppAccess"/>.</description></item>
        /// <item><description>Retourner le résultat du contrôle d’accès applicatif.</description></item>
        /// <item><description>Classifier toute exception via <see cref="SR_ExClassifier"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur à contrôler.</param>
        /// <param name="appId">Identifiant de l’application à contrôler.</param>
        /// <param name="caller">CallChain amont transmise par l’appelant.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// <c>true</c> si l’utilisateur possède un accès à l’application ; sinon <c>false</c>.
        /// </returns>
        /// <exception cref="Ex_Business">Levée si une erreur métier est détectée lors du contrôle d’accès.</exception>
        /// <exception cref="Ex_Infrastructure">Levée si une erreur technique survient lors de l’accès aux données.</exception>
        public async Task<bool> ExecuteAsync(int userId, int appId, string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _qhUserAppAccess.HandleHasUserAccessAppAsync(callChain, appId, userId, ct);
            }
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