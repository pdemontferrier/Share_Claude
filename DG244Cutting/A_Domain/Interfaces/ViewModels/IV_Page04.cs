using System.ComponentModel;

namespace DG244Cutting.A_Domain.Interfaces.ViewModels
{
    /// <summary>
    /// Contrat par lequel le menu horizontal dédié d'une page d'édition pilote
    /// les actions de persistance de cette page, sans référence concrète entre
    /// les deux ViewModels.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette interface est définie dans <c>A_Domain</c> afin d'être
    /// accessible à la fois au consommateur — le ViewModel du menu horizontal dédié
    /// <c>VM_MH04</c> — et à l'implémenteur — le ViewModel de page <c>VM_Page04</c> —,
    /// tous deux résidant en <c>D_Presentation</c>, sans qu'aucun des deux ne référence
    /// l'autre concrètement. La raison d'être est celle de <c>IS_Navigation</c> :
    /// l'inversion de dépendance passe par un contrat partagé porté par <c>A_Domain</c>.
    /// Le menu et la page étant hébergés dans deux frames frères aux <c>DataContext</c>
    /// distincts, un binding XAML ne peut franchir leur frontière ; ce contrat injecté
    /// partagé constitue l'unique point de découplage de la jonction menu vers page.
    /// Le contrat est honoré par le ViewModel de page <c>VM_Page04</c> ; le menu le
    /// résout, par injection de <c>IV_Page04</c>, comme la même instance unique qui sert
    /// de <c>DataContext</c> à la page. Cette identité d'instance partagée entre menu et
    /// page est la condition nécessaire du round-trip de notification décrit plus bas.</para>
    ///
    /// <para>Objectif : Standardiser la surface d'actions qu'une page d'édition expose
    /// à son menu dédié : le déclenchement des bascules de mode d'édition — entrée en
    /// création, entrée en modification —, le déclenchement de la persistance —
    /// enregistrement de l'enregistrement en cours d'édition, ajout d'un nouvel
    /// enregistrement — et les gardes d'autorisation observables qui pilotent l'activation
    /// des commandes du menu. Le contrat n'expose que le déclenchement et la garde ; il ne
    /// porte aucun état ni aucune logique de persistance ni de bascule.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Déclencher la bascule de la page vers le mode création
    ///   (chemin d'insertion).</item>
    ///   <item>Déclencher la bascule de la page vers le mode modification
    ///   (chemin de mise à jour).</item>
    ///   <item>Déclencher la persistance de l'enregistrement en cours d'édition
    ///   (chemin de mise à jour).</item>
    ///   <item>Déclencher l'ajout d'un nouvel enregistrement (chemin d'insertion).</item>
    ///   <item>Exposer en lecture seule l'état d'autorisation courant de chacune de ces
    ///   quatre actions.</item>
    ///   <item>Notifier tout changement de ces gardes via
    ///   <see cref="INotifyPropertyChanged"/>, afin que le menu réévalue le
    ///   <c>CanExecute</c> de ses commandes.</item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucun état ni aucune logique de persistance : l'exécution
    ///   effective de l'écriture, son pilotage transactionnel et son issue relèvent de
    ///   l'implémenteur <c>VM_Page04</c> et du UseCase invoqué en aval.</item>
    ///   <item>N'expose aucun cas d'échec métier (données invalides, identifiant déjà
    ///   pris, limite de tentatives, etc.) : ces échecs sont absorbés en aval par le
    ///   pipeline de journalisation et de notification et ne traversent pas ce contrat.</item>
    ///   <item>N'expose pas l'intégralité de la surface publique du ViewModel : seuls les
    ///   membres dont le collaborateur identifié — le menu — a besoin figurent au contrat.</item>
    ///   <item>Ne calcule pas la valeur des gardes (validité de l'édition, droits) : ce
    ///   calcul relève de l'implémenteur.</item>
    /// </list>
    ///
    /// <para>Obligation d'implémentation <see cref="INotifyPropertyChanged"/> : le
    /// contrat hérite de <see cref="INotifyPropertyChanged"/>. L'implémenteur doit émettre
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur <c>nameof(CanSave)</c>,
    /// <c>nameof(CanAdd)</c>, <c>nameof(CanEnterCreate)</c> et <c>nameof(CanEnterEdit)</c>
    /// à chaque transition de la valeur correspondante ; cette
    /// notification est le fondement de la réévaluation du <c>CanExecute</c> des commandes
    /// du menu. Son absence figerait la garde côté menu et constituerait une
    /// non-conformité aval. La notification INPC ne porte que sur les quatre gardes ;
    /// <see cref="SaveAsync"/>, <see cref="AddAsync"/>, <see cref="EnterCreate"/> et
    /// <see cref="EnterEdit"/> ne sont pas notifiants.</para>
    /// </remarks>
    public interface IV_Page04 : INotifyPropertyChanged
    {
        /// <summary>
        /// Déclenche la persistance de l'enregistrement en cours d'édition sur la page
        /// (chemin de mise à jour).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Enregistrer » de ce menu.</para>
        /// <para>Objectif : Déclencher en aval l'écriture de l'enregistrement en édition.
        /// L'exécution effective de l'écriture et le traitement de son issue relèvent de
        /// l'implémenteur et du UseCase invoqué ; le contrat ne promet que le
        /// déclenchement. L'issue succès/échec n'est pas remontée à l'appelant : les
        /// échecs métier sont absorbés en aval et ne traversent pas ce contrat.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone de la persistance ;
        /// aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task SaveAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Déclenche la persistance d'un nouvel enregistrement à partir de l'édition en
        /// cours sur la page (chemin d'insertion).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Ajouter » de ce menu.</para>
        /// <para>Objectif : Déclencher en aval l'insertion d'un nouvel enregistrement.
        /// L'exécution effective de l'écriture et le traitement de son issue relèvent de
        /// l'implémenteur et du UseCase invoqué ; le contrat ne promet que le
        /// déclenchement. L'issue succès/échec n'est pas remontée à l'appelant : les
        /// échecs métier sont absorbés en aval et ne traversent pas ce contrat.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone de l'insertion ;
        /// aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task AddAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Déclenche la bascule de la page vers le mode création (chemin d'insertion).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Nouveau » de ce menu.</para>
        /// <para>Objectif : Déclencher en aval le passage de la page en mode création.
        /// La réalisation effective de la bascule — vidage de la copie de travail,
        /// déverrouillage des champs, sélection de l'onglet d'édition — relève de
        /// l'implémenteur ; le contrat ne promet que le déclenchement. Aucune persistance,
        /// écriture ni navigation n'est portée par cette action : seul l'état d'édition de
        /// la page est modifié. L'issue n'est pas remontée à l'appelant : un abandon
        /// éventuel est absorbé en interne par l'implémenteur et ne traverse pas ce
        /// contrat.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone de la bascule en mode
        /// création ; aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task EnterCreate(string caller, CancellationToken ct = default);

        /// <summary>
        /// Déclenche la bascule de la page vers le mode modification de l'enregistrement
        /// sélectionné (chemin de mise à jour).
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Modifier » de ce menu.</para>
        /// <para>Objectif : Déclencher en aval le passage de la page en mode modification.
        /// La réalisation effective de la bascule — déverrouillage de l'enregistrement
        /// sélectionné — relève de l'implémenteur ; le contrat ne promet que le
        /// déclenchement. Aucune persistance, écriture ni navigation n'est portée par
        /// cette action : seul l'état d'édition de la page est modifié. L'issue n'est pas
        /// remontée à l'appelant : un abandon éventuel est absorbé en interne par
        /// l'implémenteur et ne traverse pas ce contrat.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone de la bascule en mode
        /// modification ; aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task EnterEdit(string caller, CancellationToken ct = default);

        /// <summary>
        /// Obtient une valeur indiquant si l'action « Enregistrer » est actuellement
        /// autorisée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Enregistrer ».</para>
        /// <para>Valeur : <see langword="true"/> si l'action « Enregistrer » est
        /// actuellement autorisée, <see langword="false"/> sinon. Lecture pure, sans effet
        /// de bord ; le calcul de la valeur (validité de l'édition, droits) relève de
        /// l'implémenteur.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur <c>nameof(CanSave)</c>,
        /// faute de quoi la garde resterait figée côté menu.</para>
        /// </remarks>
        bool CanSave { get; }

        /// <summary>
        /// Obtient une valeur indiquant si l'action « Ajouter » est actuellement
        /// autorisée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Ajouter ».</para>
        /// <para>Valeur : <see langword="true"/> si l'action « Ajouter » est actuellement
        /// autorisée, <see langword="false"/> sinon. Lecture pure, sans effet de bord ; le
        /// calcul de la valeur (validité de l'édition, droits) relève de l'implémenteur.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur <c>nameof(CanAdd)</c>,
        /// faute de quoi la garde resterait figée côté menu.</para>
        /// </remarks>
        bool CanAdd { get; }

        /// <summary>
        /// Obtient une valeur indiquant si l'action « Nouveau » est actuellement
        /// autorisée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Nouveau ».</para>
        /// <para>Valeur : <see langword="true"/> si l'action « Nouveau » est actuellement
        /// autorisée, <see langword="false"/> sinon. Lecture pure, sans effet de bord ; le
        /// calcul de la valeur (validité de l'édition, droits) relève de l'implémenteur.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur <c>nameof(CanEnterCreate)</c>,
        /// faute de quoi la garde resterait figée côté menu.</para>
        /// </remarks>
        bool CanEnterCreate { get; }

        /// <summary>
        /// Obtient une valeur indiquant si l'action « Modifier » est actuellement
        /// autorisée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Modifier ».</para>
        /// <para>Valeur : <see langword="true"/> si l'action « Modifier » est actuellement
        /// autorisée, <see langword="false"/> sinon. Lecture pure, sans effet de bord ; le
        /// calcul de la valeur (validité de l'édition, droits) relève de l'implémenteur.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur <c>nameof(CanEnterEdit)</c>,
        /// faute de quoi la garde resterait figée côté menu.</para>
        /// </remarks>
        bool CanEnterEdit { get; }
    }
}