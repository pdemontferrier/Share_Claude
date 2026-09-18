namespace DG244Cutting.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// Définit le contrat du service chargé d'alimenter le contexte poste de l'utilisateur courant.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat défini en <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.3 amendée, 1ère obligation). Il est consommé par injection
    /// de dépendances depuis le UseCase orchestrateur d'initialisation du contexte utilisateur,
    /// qui en délègue l'exécution au service concret
    /// <see cref="DG244Cutting.B_UseCases.Services.User.SR_UserDeviceContext"/>
    /// résidant en <c>B_UseCases/Services/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (contexte poste de l'utilisateur courant).</para>
    /// <para>Objectif : Centraliser la résolution et l'injection du nom du poste, de l'adresse
    /// IPv4 et de l'utilisateur Windows dans le paramètre partagé <c>ISE_User</c>.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Résoudre les informations techniques du poste courant.</description></item>
    /// <item><description>Mettre à jour l'état porté par <c>ISE_User</c> via l'opération atomique <c>SetDeviceContext</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne charge pas de données depuis une base de données.</description></item>
    /// <item><description>Ne pilote pas la séquence d'authentification ni le cycle de session.</description></item>
    /// <item><description>Ne calcule pas les droits d'accès utilisateur.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.B_UseCases.Services.User.SR_UserDeviceContext"/>
    public interface IS_UserDeviceContext
    {
        // --- Groupe 1 : Alimentation du contexte poste ---

        /// <summary>
        /// Alimente le contexte poste de l'utilisateur courant.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode appelée au démarrage de l'application ou lors de la
        /// reconstruction du contexte utilisateur local par le UseCase orchestrateur.</para>
        /// <para>Objectif : Garantir que <c>ISE_User</c> contient les informations techniques
        /// du poste courant (identifiant machine, adresse IPv4, compte Windows).</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant ; enrichie en interne au format normatif <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c> conformément à §4.5.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Une annulation est propagée sans requalification.</param>
        /// <returns>Une tâche représentant l'opération asynchrone.</returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée en cas de défaillance technique imprévue de la résolution du contexte poste
        /// (résolution réseau, accès à l'identité Windows, mise à jour atomique du Setting) ;
        /// produite par requalification via <c>IS_ExClassifier</c> dans le quatrième catch du
        /// patron canonique.
        /// </exception>
        /// <exception cref="System.OperationCanceledException">
        /// Levée si l'annulation coopérative est demandée via <paramref name="ct"/> ; propagée
        /// sans requalification (priorité du troisième catch sur le catch terminal, R-4.6.13).
        /// </exception>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}