namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase orchestrateur de la séquence de démarrage applicatif de DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Contrat défini en <c>A_Domain</c> conformément à la 1ère obligation
    /// contractuelle de §4.14.2 amendée. Il est consommé par injection de dépendances depuis le
    /// point d'amorçage WPF <c>App.xaml.cs</c> au Jalon 3 de la séquence de démarrage applicatif
    /// (§3.10 du 0230), après Jalon 1 (construction du conteneur DI via
    /// <c>SR_ConteneurDI.ConfigureServices</c>) et Jalon 2 (traitement des arguments de ligne de
    /// commande par <c>App.ProcessStartupArguments</c>).</para>
    /// <para>Objectif : Orchestrer de manière ordonnée et atomique la séquence de
    /// démarrage applicatif, en validant la disponibilité des prérequis techniques et
    /// contextuels, et en signalant à <c>App.xaml.cs</c> par valeur booléenne le verdict de
    /// démarrage (ouverture de <c>MainWindow</c> ou <c>Current.Shutdown()</c>) conformément à la
    /// sémantique de Jalon 4 décrite en §3.10.6 du 0230.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Initialiser le dictionnaire de langue et le drapeau associé à partir du code culture reçu (R-3.10.5).</description></item>
    ///   <item><description>Alimenter le contexte poste de l'utilisateur courant dans <c>ISE_User</c> (R-3.10.8, §3.10.6).</description></item>
    ///   <item><description>Mettre à jour le titre applicatif dans <c>ISE_App.ApplicationTitle</c>.</description></item>
    ///   <item><description>Identifier l'utilisateur applicatif associé au login Windows du poste courant et orchestrer l'ouverture de la session applicative associée par consommation de <c>IU_UserAppSession_Open</c> en sous-séquence (R-4.14.21), avec conversion du retour <see langword="false"/> en <see cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business"/> code <c>BU_ER_04</c> captée terminalement.</description></item>
    ///   <item><description>Charger le nom complet de l'utilisateur courant si <c>ISE_User.AppUserId &gt; 0</c>.</description></item>
    ///   <item><description>Appliquer les droits de pages de l'utilisateur identifié dans le contexte utilisateur partagé par consommation de <c>IU_UserAppPageRight_Apply</c> en sous-séquence (R-4.14.21), avec exploitation directe du retour <see langword="false"/> propagé à <c>App.xaml.cs</c> sans conversion en exception applicative typée (la notification utilisateur en cas d'échec est portée par le pipeline interne du UseCase consommé via son propre <c>IU_LogAndNotify</c>).</description></item>
    ///   <item><description>Vérifier la connectivité à la base de données et notifier l'état de connexion sur <c>ISE_App</c>.</description></item>
    ///   <item><description>Vérifier la disponibilité applicative (verrou administrateur).</description></item>
    ///   <item><description>Vérifier l'absence de session active sur un autre poste pour le couple (utilisateur, application).</description></item>
    ///   <item><description>Signaler à <c>App.xaml.cs</c> le verdict de démarrage par retour booléen.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>N'ouvre, ne valide ni n'annule aucune transaction EF Core : le UseCase est non transactionnel par construction (concept transverse, §3.10 du 0230 ; I-4.10.3 vérifiée par construction).</description></item>
    ///   <item><description>Ne décide ni d'ouvrir <c>MainWindow</c> ni de fermer l'application : il signale ; la décision relève de <c>App.xaml.cs</c>.</description></item>
    ///   <item><description>Ne propage aucune exception applicative typée (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>) ; chaque catch typé délègue à <c>IU_LogAndNotify</c> et retourne <see langword="false"/> (R-3.10.3). Seule <see cref="System.OperationCanceledException"/> est propagée sans requalification (R-4.6.13).</description></item>
    /// </list>
    /// <para>Particularité du retour <see cref="System.Threading.Tasks.Task{TResult}"/> de type <see langword="bool"/> au regard de R-4.14.21 :
    /// Le retour signalable <c>Task&lt;bool&gt;</c> exposé par la présente interface n'est pas
    /// la marque d'une consommation en sous-séquence par un orchestrateur UseCase amont au sens
    /// de R-4.14.21 - le consommateur amont effectif est <c>App.xaml.cs</c>, point d'amorçage WPF
    /// situé hors de la famille UseCases. Le retour booléen porte une signalisation succès/échec
    /// applicatif captée terminalement par <c>App.xaml.cs</c> à l'issue du Jalon 3 pour arbitrer
    /// entre Jalon 4a (ouverture de <c>MainWindow</c> si <see langword="true"/>) et Jalon 4b
    /// (<c>Current.Shutdown()</c> si <see langword="false"/>), conformément à §3.10.6 du 0230.
    /// En conséquence, l'item UC21 de la checklist §4.3.3 du 0232-UC est marqué ➖ pour
    /// l'implémentation correspondante. L'item UC22 est marqué ✅ : le UseCase consomme
    /// désormais trois UseCases en sous-séquence conformément à R-4.14.21 :</para>
    /// <list type="bullet">
    ///   <item><description><see cref="DG244Cutting.A_Domain.Interfaces.UseCases.User.IU_UserAppSession_Open"/>
    ///   à l'étape 6 d'<c>ExecuteAsync</c> (méthode privée
    ///   <c>IdentifyDeviceUserAndOpenSessionAsync</c>), introduit par le fil d'Extension
    ///   <c>UC_Application_OnStart_Extension</c> (1ère Extension du couple).</description></item>
    ///   <item><description><see cref="DG244Cutting.A_Domain.Interfaces.UseCases.User.IU_UserAppPageRight_Apply"/>
    ///   à l'étape 8 d'<c>ExecuteAsync</c> (méthode privée
    ///   <c>ApplyUserPageRightAsync</c>), introduit par le fil d'Extension
    ///   <c>UC_Application_OnStart_Extension</c> (2ème Extension successive du couple).</description></item>
    ///   <item><description><see cref="DG244Cutting.A_Domain.Interfaces.UseCases.App.IU_Language_Apply"/>
    ///   à l'étape 1 d'<c>ExecuteAsync</c> (méthode privée <c>ApplyCultureAsync</c>),
    ///   introduit par le présent fil de Refactoring
    ///   <c>UC_ApplicationOnStart_Refactoring</c> en substitution du Service
    ///   <c>IS_Language</c>.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.B_UseCases.UseCases.App.UC_Application_OnStart"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.UseCases.User.IU_UserAppSession_Open"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.UseCases.User.IU_UserAppPageRight_Apply"/>
    /// <seealso cref="DG244Cutting.A_Domain.Interfaces.UseCases.App.IU_Language_Apply"/>
    public interface IU_Application_OnStart
    {
        /// <summary>
        /// Exécute la séquence de démarrage applicatif et signale par valeur booléenne le verdict
        /// au point d'amorçage <c>App.xaml.cs</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode appelée par <c>App.xaml.cs</c> au Jalon 3 de la séquence
        /// de démarrage applicatif (§3.10 du 0230), après construction du conteneur DI et
        /// traitement des arguments de ligne de commande. Le <paramref name="cultureCode"/> est
        /// transmis depuis <c>App.xaml.cs</c> sur la base de <c>CultureInfo.CurrentCulture.Name</c>
        /// (dérogation assumée à la logique de priorité à 3 niveaux décrite en §3.10.8 du 0230 ;
        /// cf. <c>&lt;remarks&gt;</c> de l'implémentation <c>UC_Application_OnStart</c>).</para>
        /// <para>Objectif : Enchaîner les dix étapes du Jalon 3 dans l'ordre normatif posé
        /// en §3.10 du 0230 : 1) acquisition de l'unicité d'instance ; 2) initialisation
        /// langue ; 3) titre applicatif ; 4) connectivité base ; 5) disponibilité applicative ;
        /// 6) contexte poste ; 7) identification DeviceUser et ouverture de session ; 8) nom
        /// complet utilisateur ; 9) application des droits de pages ; 10) intégrité de session.
        /// Retourner ensuite le verdict applicatif à <c>App.xaml.cs</c>.</para>
        /// <para>Comportement vis-à-vis des erreurs (R-3.10.3) : Aucune exception applicative
        /// typée (<c>Ex_Business</c>, <c>Ex_Infrastructure</c>, <c>Ex_Unclassified</c>) n'est
        /// propagée par cette méthode. Chaque catch typé applicatif délègue à <c>IU_LogAndNotify</c>
        /// puis retourne <see langword="false"/>. Seule <see cref="System.OperationCanceledException"/>
        /// est propagée sans requalification, conformément à R-4.6.13.</para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant <c>App.xaml.cs</c>, propagée en interne selon le
        /// format normatif <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> (§4.5 du
        /// 0230). Ne doit pas être <see langword="null"/>, vide ou composé uniquement d'espaces
        /// blancs.
        /// </param>
        /// <param name="cultureCode">
        /// Code culture à appliquer au démarrage (ex. : <c>"fr-FR"</c>, <c>"en-GB"</c>), au format
        /// BCP 47. Transmis par <c>App.xaml.cs</c> sur la base de
        /// <c>CultureInfo.CurrentCulture.Name</c>. Ne doit pas être <see langword="null"/>, vide
        /// ou composé uniquement d'espaces blancs.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Une annulation est propagée sans requalification
        /// (R-4.6.13).
        /// </param>
        /// <returns>
        /// <see langword="true"/> si la séquence de démarrage applicatif est validée dans son
        /// intégralité : <c>App.xaml.cs</c> procède alors à l'ouverture de <c>MainWindow</c>
        /// (Jalon 4a, §3.10.6 du 0230). <see langword="false"/> si l'une des dix étapes de la
        /// séquence aboutit à un refus de démarrage applicatif ou à un signal de terminaison
        /// silencieuse (seconde instance détectée avec signalement à l'instance primaire
        /// préexistante, échec de connectivité base, indisponibilité applicative, échec
        /// d'ouverture de session applicative, échec d'application des droits de pages, conflit
        /// de session, exception applicative typée captée terminalement) : <c>App.xaml.cs</c>
        /// procède alors à <c>Current.Shutdown()</c> (Jalon 4b, §3.10.6 du 0230).
        /// </returns>
        /// <exception cref="System.OperationCanceledException">
        /// Levée si <paramref name="ct"/> est annulé avant ou pendant l'exécution. Aucune
        /// requalification ni capture par catch terminal applicatif : remontée stricte à
        /// l'appelant <c>App.xaml.cs</c> conformément à la priorité d'annulation coopérative
        /// (R-4.6.13).
        /// </exception>
        Task<bool> ExecuteAsync(string caller, string cultureCode, CancellationToken ct = default);
    }
}