using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Contrat du service d'orchestration du changement de langue de l'application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans A_Domain, implémentée par <c>SR_Language</c>
    /// positionné dans D_Presentation. Appelée au démarrage de l'application et lors de tout
    /// changement de langue explicite déclenché par l'opérateur depuis la page de sélection
    /// de langue.
    /// </para>
    /// <para>
    /// Objectif : découpler l'orchestration du changement de langue de ses consommateurs
    /// (séquence de démarrage via le UseCase orchestrateur, ViewModel de sélection de langue),
    /// en exposant un point d'entrée unique, traçable et robuste.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Persister le code culture actif dans <see cref="ISE_App.AppCultureCode"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Déclencher le chargement du dictionnaire de langue correspondant via
    ///     <see cref="ISE_Language"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Synchroniser l'icône de drapeau affichée dans l'interface via <see cref="ISE_Flag"/>.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Aucune manipulation directe de <c>ResourceDictionary</c> WPF : déléguée à
    ///     <c>SE_Language</c>.
    ///   </description></item>
    ///   <item><description>Aucune logique métier.</description></item>
    ///   <item><description>Aucun accès aux données persistées.</description></item>
    ///   <item><description>
    ///     Aucune prise de décision sur la langue à appliquer : cette responsabilité appartient
    ///     à l'appelant (UseCase de démarrage ou ViewModel de sélection).
    ///   </description></item>
    /// </list>
    /// </remarks>
    public interface IS_Language
    {
        /// <summary>
        /// Applique la langue correspondant au code culture fourni en orchestrant les trois
        /// étapes du changement de langue.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée au démarrage de l'application (depuis le UseCase orchestrateur
        /// de démarrage) ou lors d'un changement de langue explicite déclenché par l'opérateur.
        /// </para>
        /// <para>
        /// Objectif : exécuter de manière ordonnée et atomique les trois étapes du changement
        /// de langue : (1) persistance du code culture dans <c>ISE_App.AppCultureCode</c>,
        /// (2) chargement du dictionnaire XAML via <c>ISE_Language</c>, (3) mise à jour du
        /// drapeau via <c>ISE_Flag</c>.
        /// </para>
        /// <para>
        /// Comportement en cas d'erreur : toute exception technique inattendue est requalifiée
        /// en <see cref="Ex_Infrastructure"/> avant
        /// propagation. L'appelant (UseCase) prend en charge le traitement terminal.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour
        /// enrichissement et traçabilité.</param>
        /// <param name="cultureCode">
        /// Code culture à appliquer (ex. : <c>"fr-FR"</c>, <c>"en-GB"</c>). Ne doit pas être
        /// <see langword="null"/>, vide ou composé uniquement d'espaces blancs.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière
        /// coopérative.</param>
        /// <returns>Tâche représentant l'exécution asynchrone du changement de langue.</returns>
        /// <exception cref="ArgumentException">
        /// Levée si <paramref name="cultureCode"/> est <see langword="null"/>, vide ou composé
        /// uniquement d'espaces blancs.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors du chargement du dictionnaire ou de
        /// la résolution de l'URI de drapeau (fichier XAML introuvable, ressource WPF invalide,
        /// etc.).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant
        /// l'exécution.
        /// </exception>
        Task ExecuteAsync(string caller, string cultureCode, CancellationToken ct = default);
    }
}