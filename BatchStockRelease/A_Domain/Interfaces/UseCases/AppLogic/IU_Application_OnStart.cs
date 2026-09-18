using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// UseCase orchestrateur responsable du processus complet de démarrage de l’application.
    /// Il regroupe et pilote les vérifications techniques et contextuelles nécessaires avant
    /// le chargement de la fenêtre principale.
    /// </para>
    /// 
    /// <b>Contexte :</b>
    /// <para>
    /// Ce UseCase est appelé lors du lancement initial de l’application WPF, depuis la méthode
    /// <see cref="BatchStockRelease.App.Application_Startup"/> de la classe <see cref="BatchStockRelease.App"/>.
    /// Il fait partie du cycle d’amorçage global, qui vise à garantir la stabilité, la connectivité
    /// et la validité de la session utilisateur avant d’afficher l’interface principale.
    /// </para>
    /// 
    /// <b>Objectif :</b>
    /// <para>
    /// S’assurer que toutes les conditions nécessaires à l’exécution sont réunies :
    /// connexion base de données, disponibilité applicative, cohérence de session, et paramétrage
    /// linguistique.
    /// Si toutes les conditions sont réunies, il met à jour le contexte global (titre, utilisateur)
    /// et autorise le lancement du <see cref="MainWindow"/>.
    /// </para>
    /// 
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Configurer la langue courante à partir du code culture système.</description></item>
    /// <item><description>Mettre à jour le titre et les informations de contexte utilisateur.</description></item>
    /// <item><description>Tester la connexion à la base de données.</description></item>
    /// <item><description>Vérifier l’absence de sessions actives sur d’autres postes.</description></item>
    /// <item><description>Contrôler l’accessibilité de l’application (verrou administrateur).</description></item>
    /// <item><description>Notifier et journaliser toute erreur rencontrée via <see cref="BatchStockRelease.A_Domain.Interfaces.Services.AppLogic.IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IU_Application_OnStart
    {
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Exécute le scénario complet d’initialisation de l’application.
        /// </para>
        /// 
        /// <b>Contexte :</b>
        /// <para>
        /// Méthode appelée par la procédure de démarrage (<see cref="BatchStockRelease.App.Application_Startup"/>)
        /// afin de vérifier la disponibilité de l’environnement et de préparer le contexte utilisateur.
        /// </para>
        /// 
        /// <b>Objectif :</b>
        /// <para>
        /// Garantir que l’application peut être ouverte en toute sécurité et dans un état stable.
        /// </para>
        /// 
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Configurer la langue courante à partir du code culture système.</description></item>
        /// <item><description>Tester la connexion à la base de données.</description></item>
        /// <item><description>Vérifier l’absence de sessions actives sur d’autres postes.</description></item>
        /// <item><description>Contrôler l’accessibilité de l’application (verrou administrateur).</description></item>
        /// <item><description>Mettre à jour le titre et les informations de contexte utilisateur.</description></item>
        /// <item><description>Notifier et journaliser toute erreur rencontrée via <see cref="BatchStockRelease.A_Domain.Interfaces.Services.AppLogic.IS_LogAndNotify"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="cultureCode">Code de la culture courante (ex. "fr-FR").</param>
        /// <param name="caller">Chaîne d’appelante utilisée pour la traçabilité (CallChain).</param>
        /// <returns><c>true</c> si l’initialisation réussit, sinon <c>false</c>.</returns>
        /// <exception cref="Ex_Business">Si une règle métier est violée.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur d’accès aux ressources est détectée.</exception>
        Task<bool> ExecuteAsync(string cultureCode, string caller);
    }
}
