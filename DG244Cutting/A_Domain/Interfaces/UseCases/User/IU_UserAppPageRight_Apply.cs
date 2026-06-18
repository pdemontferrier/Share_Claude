using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// Contrat du UseCase d'application des droits de pages de l'utilisateur courant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c> conformément à l'obligation de
    /// placement des contrats (§4.14.1). Elle est consommée par injection de dépendances
    /// par le pipeline d'authentification ou de réinitialisation du contexte utilisateur,
    /// qui en délègue l'exécution au UseCase concret
    /// <see cref="DG244Cutting.B_UseCases.UseCases.User.UC_UserAppPageRight_Apply"/>
    /// résidant en <c>B_UseCases/UseCases/User</c>. Le contrat appartient au domaine
    /// <c>User</c> (cycle de session / authentification).
    /// </para>
    /// <para>
    /// Objectif : orchestrer la constitution de l'état des droits de pages dans le contexte
    /// utilisateur partagé — initialisation par défaut au moindre privilège, chargement des
    /// droits spécifiques via le Query Handler, application dans le contexte — avec un
    /// traitement terminal des erreurs conforme à la section 4.7. Le scénario est en
    /// lecture seule côté base de données ; son seul effet est l'écriture du contexte
    /// utilisateur partagé, qui ne constitue pas une mutation persistée et n'ouvre donc pas
    /// de transaction (§4.10).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée du scénario d'application des droits de pages.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne décide pas du moment d'appel ni de l'identité de l'utilisateur : ceux-ci sont fournis par le contexte applicatif.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de A_Domain.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_UserAppPageRight_Apply
    {
        // --- Groupe 1 : Application des droits de pages ---

        /// <summary>
        /// Constitue l'état des droits de pages de l'utilisateur courant dans le contexte
        /// utilisateur partagé, en initialisant les droits par défaut puis en appliquant
        /// les droits spécifiques chargés.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté après authentification ou lors d'une réinitialisation du
        /// contexte utilisateur. L'identité de l'utilisateur et de l'application est lue
        /// depuis le contexte applicatif courant. Le UseCase initialise les droits par
        /// défaut (moindre privilège), charge les droits spécifiques via le Query Handler,
        /// puis applique ces droits dans le contexte utilisateur partagé.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Initialiser les droits de pages par défaut au moindre privilège.</description></item>
        /// <item><description>Charger les droits spécifiques de l'utilisateur via le Query Handler.</description></item>
        /// <item><description>Appliquer les droits chargés dans le contexte utilisateur partagé.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif
        /// de la section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant utilisateur ou l'identifiant application lus du contexte
        /// applicatif courant ne sont pas strictement positifs.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée lorsqu'une défaillance technique survient lors du chargement des droits.</exception>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}