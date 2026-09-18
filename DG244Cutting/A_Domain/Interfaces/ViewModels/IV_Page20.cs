using System.ComponentModel;

namespace DG244Cutting.A_Domain.Interfaces.ViewModels
{
    /// <summary>
    /// Contrat par lequel le menu horizontal dédié de la page de production déclenche
    /// les trois gestes de l'opérateur sur la barre présentée, sans référence concrète
    /// entre les deux ViewModels.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : La page de production présente à l'opérateur la barre retenue par
    /// l'application, approvisionnée juste avant d'être coupée. L'opérateur dispose sur
    /// cette barre de trois gestes — l'accepter, l'écarter, signaler que la matière n'est
    /// pas là —, portés par les boutons du menu horizontal <c>MH20</c> mais dont
    /// l'exécution et les conditions d'activation appartiennent à la page <c>Page20</c>.
    /// Cette interface est définie dans <c>A_Domain</c> afin d'être accessible à la fois
    /// au consommateur — le ViewModel du menu horizontal dédié <c>VM_MH20</c> — et à
    /// l'implémenteur — le ViewModel de page <c>VM_Page20</c> —, tous deux résidant en
    /// <c>D_Presentation</c>, sans qu'aucun des deux ne référence l'autre concrètement.
    /// La raison d'être est celle de <c>IS_Navigation</c> : l'inversion de dépendance
    /// passe par un contrat partagé porté par <c>A_Domain</c>. Le menu et la page étant
    /// hébergés dans deux frames frères aux <c>DataContext</c> distincts, un binding XAML
    /// ne peut franchir leur frontière ; ce contrat injecté partagé constitue l'unique
    /// point de découplage de la jonction menu vers page. Le menu le résout, par injection
    /// de <c>IV_Page20</c>, comme la même instance unique qui sert de <c>DataContext</c> à
    /// la page. Cette identité d'instance partagée entre menu et page est la condition
    /// nécessaire du round-trip de notification décrit plus bas.</para>
    ///
    /// <para>Objectif : Standardiser la surface d'actions que la page de production expose
    /// à son menu dédié : le déclenchement de la validation, du refus et de la mise en
    /// rupture de la barre présentée, et les gardes d'autorisation observables qui
    /// pilotent l'activation des commandes correspondantes du menu. Le contrat n'expose
    /// que le déclenchement et la garde ; il ne porte aucun état, aucune logique de
    /// validation ni aucune règle métier. La pluralité des membres est constitutive du
    /// contrat : trois déclencheurs à nom fonctionnel suffixé <c>Async</c>
    /// (<see cref="ValidateAsync"/>, <see cref="RejectAsync"/>,
    /// <see cref="DeclareOutOfStockAsync"/>), chacun associé à une garde
    /// <c>Can[Action]</c> distincte (<see cref="CanValidate"/>, <see cref="CanReject"/>,
    /// <see cref="CanDeclareOutOfStock"/>) pilotant sa propre commande.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Déclencher l'acceptation de la barre présentée.</item>
    ///   <item>Déclencher la mise à l'écart de la barre présentée.</item>
    ///   <item>Déclencher le signalement de l'absence physique d'une barre neuve.</item>
    ///   <item>Exposer en lecture seule la possibilité structurelle courante de chacun
    ///   de ces trois gestes.</item>
    ///   <item>Notifier tout changement de ces gardes via
    ///   <see cref="INotifyPropertyChanged"/>, afin que le menu réévalue le
    ///   <c>CanExecute</c> de ses commandes.</item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne porte aucun état ni aucune logique : le contrôle de la saisie,
    ///   l'avertissement de l'opérateur, l'invocation des UseCases
    ///   <c>IU_BarValidation</c>, <c>IU_BarRefusal</c> et
    ///   <c>IU_ProductionBar_SetOutOfStock</c>, la désélection de la barre, la relance de
    ///   la séquence d'entrée et la navigation relèvent de l'implémenteur
    ///   <c>VM_Page20</c>.</item>
    ///   <item>N'expose aucun cas d'échec métier ni aucune issue succès/échec : les retours
    ///   signalables des UseCases invoqués s'arrêtent à l'implémenteur, et les échecs sont
    ///   absorbés en aval par le pipeline de journalisation et de notification ; ils ne
    ///   traversent pas ce contrat.</item>
    ///   <item>Ne porte pas la cohérence de la saisie : un motif de refus sélectionné lors
    ///   d'une validation, ou l'absence de motif lors d'un refus, donne lieu à un
    ///   avertissement côté page, jamais à la désactivation d'un bouton.</item>
    ///   <item>N'expose pas l'intégralité de la surface publique du ViewModel : seuls les
    ///   membres dont le collaborateur identifié — le menu — a besoin figurent au
    ///   contrat.</item>
    ///   <item>Ne calcule pas la valeur des gardes : ce calcul relève de
    ///   l'implémenteur.</item>
    ///   <item>N'exige de l'implémenteur aucune défense contre un appel hors garde : le
    ///   respect de la garde est une précondition à la charge du consommateur.</item>
    /// </list>
    ///
    /// <para>Régimes de la page et valeur des gardes :</para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>Régime de la page</term>
    ///     <description><c>CanValidate</c> / <c>CanReject</c> /
    ///     <c>CanDeclareOutOfStock</c></description>
    ///   </listheader>
    ///   <item>
    ///     <term>Barre à valider, neuve</term>
    ///     <description>vrai / vrai / vrai</description>
    ///   </item>
    ///   <item>
    ///     <term>Barre à valider, issue d'une chute</term>
    ///     <description>vrai / vrai / faux</description>
    ///   </item>
    ///   <item>
    ///     <term>Tout en rupture, aucune barre présentée</term>
    ///     <description>faux / faux / faux</description>
    ///   </item>
    /// </list>
    /// <para>Les gardes portent la possibilité structurelle du geste, jamais la cohérence
    /// de la saisie. La rupture est réservée aux barres neuves ; l'indisponibilité d'une
    /// chute relève du refus avec motif.</para>
    ///
    /// <para>Invariants entre gardes :</para>
    /// <list type="bullet">
    ///   <item><see cref="CanDeclareOutOfStock"/> implique <see cref="CanValidate"/> et
    ///   <see cref="CanReject"/>.</item>
    ///   <item><see cref="CanValidate"/> et <see cref="CanReject"/> ont une valeur
    ///   identique dans les trois régimes ci-dessus ; ils restent deux membres distincts,
    ///   chacun pilotant sa propre commande.</item>
    /// </list>
    ///
    /// <para>Obligation d'implémentation <see cref="INotifyPropertyChanged"/> : le
    /// contrat hérite de <see cref="INotifyPropertyChanged"/>. L'implémenteur doit émettre
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur
    /// <c>nameof(CanValidate)</c>, <c>nameof(CanReject)</c> et
    /// <c>nameof(CanDeclareOutOfStock)</c> à chaque transition de la valeur
    /// correspondante ; cette notification est le fondement de la réévaluation du
    /// <c>CanExecute</c> des commandes du menu. Son absence figerait la garde côté menu et
    /// constituerait une non-conformité aval. Transitions minimales à notifier :</para>
    /// <list type="bullet">
    ///   <item>présentation d'une barre ;</item>
    ///   <item>désélection de la barre ;</item>
    ///   <item>passage d'une barre neuve à une barre issue d'une chute, et
    ///   inversement ;</item>
    ///   <item>entrée dans le régime « tout en rupture ».</item>
    /// </list>
    /// <para>La notification INPC ne porte que sur les trois gardes ;
    /// <see cref="ValidateAsync"/>, <see cref="RejectAsync"/> et
    /// <see cref="DeclareOutOfStockAsync"/> ne sont pas notifiants.</para>
    ///
    /// <para>Articulation avec le consommateur <c>VM_MH20</c> : chaque commande du menu a
    /// pour garde <c>!IsProcessing &amp;&amp; CanXxx</c> ; l'appel du déclencheur est
    /// encadré par <c>BeginProcessing</c> / <c>EndProcessing</c> et effectué avec
    /// <c>CancellationToken.None</c> ; la réévaluation du <c>CanExecute</c> s'appuie sur
    /// <c>CommandManager.RequerySuggested</c>.</para>
    /// </remarks>
    public interface IV_Page20 : INotifyPropertyChanged
    {
        /// <summary>
        /// Déclenche l'acceptation de la barre présentée à l'opérateur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Valider » de ce menu.</para>
        /// <para>Objectif : Déclencher en aval l'acceptation de la barre présentée. La
        /// réalisation effective — contrôle de la saisie des défauts et de l'absence de
        /// motif de refus, invocation du UseCase <c>IU_BarValidation</c>, poursuite du
        /// parcours vers le carrefour de découpe ou relance sur barre-déchet — relève de
        /// l'implémenteur ; le contrat ne promet que le déclenchement. Une saisie
        /// incohérente donne lieu à un avertissement côté page, jamais à une désactivation
        /// du bouton. L'issue succès/échec n'est pas remontée à l'appelant : la page se
        /// rafraîchit elle-même ou navigue, et les échecs métier sont absorbés en aval sans
        /// traverser ce contrat.</para>
        /// <para>Précondition : le consommateur n'appelle ce déclencheur que si
        /// <see cref="CanValidate"/> vaut <see langword="true"/>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone de l'acceptation ;
        /// aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task ValidateAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Déclenche la mise à l'écart de la barre présentée à l'opérateur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Refuser » de ce menu.</para>
        /// <para>Objectif : Déclencher en aval le refus de la barre présentée. La
        /// réalisation effective — contrôle de la présence d'un motif, invocation du
        /// UseCase <c>IU_BarRefusal</c>, désélection de la barre, relance de la séquence
        /// d'entrée pour une autre barre — relève de l'implémenteur ; le contrat ne promet
        /// que le déclenchement. L'absence de motif donne lieu à un avertissement côté
        /// page, jamais à une désactivation du bouton. L'issue succès/échec n'est pas
        /// remontée à l'appelant : les échecs métier sont absorbés en aval et ne
        /// traversent pas ce contrat.</para>
        /// <para>Précondition : le consommateur n'appelle ce déclencheur que si
        /// <see cref="CanReject"/> vaut <see langword="true"/>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone du refus ;
        /// aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task RejectAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Déclenche le signalement de l'absence physique de la barre neuve présentée à
        /// l'opérateur.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée par le ViewModel du menu horizontal dédié lorsque
        /// l'opérateur active la commande « Rupture de stock » de ce menu, après
        /// confirmation obtenue par le menu. Un abandon de l'opérateur est traité
        /// localement par le menu et n'atteint jamais ce contrat.</para>
        /// <para>Objectif : Déclencher en aval la mise en rupture de la barre de
        /// production. La réalisation effective — invocation du UseCase
        /// <c>IU_ProductionBar_SetOutOfStock</c>, désélection de la barre, relance de la
        /// séquence d'entrée pour une autre barre — relève de l'implémenteur ; le contrat
        /// ne promet que le déclenchement. La rupture est réservée aux barres neuves.
        /// L'issue succès/échec n'est pas remontée à l'appelant : les échecs métier sont
        /// absorbés en aval et ne traversent pas ce contrat.</para>
        /// <para>Précondition : le consommateur n'appelle ce déclencheur que si
        /// <see cref="CanDeclareOutOfStock"/> vaut <see langword="true"/>.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>Tâche représentant le déclenchement asynchrone de la mise en rupture ;
        /// aucun résultat métier n'est signalé à l'appelant.</returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est déclenché
        /// pendant l'opération, conformément à la doctrine d'annulation coopérative §4.6
        /// du référentiel.
        /// </exception>
        Task DeclareOutOfStockAsync(string caller, CancellationToken ct = default);

        /// <summary>
        /// Indicateur d'autorisation structurelle de l'action « Valider » sur la barre
        /// présentée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Valider ».</para>
        /// <para>Valeur : <see langword="true"/> lorsqu'une barre est présentée à
        /// l'opérateur, <see langword="false"/> sinon. Lecture pure, sans effet de bord ;
        /// la valeur exprime la seule possibilité structurelle du geste, jamais la
        /// cohérence de la saisie, et son calcul relève de l'implémenteur.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur
        /// <c>nameof(CanValidate)</c>, faute de quoi la garde resterait figée côté
        /// menu.</para>
        /// </remarks>
        bool CanValidate { get; }

        /// <summary>
        /// Indicateur d'autorisation structurelle de l'action « Refuser » sur la barre
        /// présentée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Refuser ».</para>
        /// <para>Valeur : <see langword="true"/> lorsqu'une barre est présentée à
        /// l'opérateur, <see langword="false"/> sinon. Lecture pure, sans effet de bord ;
        /// la valeur exprime la seule possibilité structurelle du geste, jamais la
        /// présence d'un motif, et son calcul relève de l'implémenteur.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur
        /// <c>nameof(CanReject)</c>, faute de quoi la garde resterait figée côté
        /// menu.</para>
        /// </remarks>
        bool CanReject { get; }

        /// <summary>
        /// Indicateur d'autorisation structurelle de l'action « Rupture de stock » sur la
        /// barre présentée.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Consommée par le ViewModel du menu horizontal dédié pour
        /// conditionner le <c>CanExecute</c> de sa commande « Rupture de stock ».</para>
        /// <para>Valeur : <see langword="true"/> lorsqu'une barre est présentée à
        /// l'opérateur et qu'il s'agit d'une barre neuve, <see langword="false"/> sinon,
        /// notamment pour une barre issue d'une chute. Lecture pure, sans effet de bord ;
        /// son calcul relève de l'implémenteur. Une valeur <see langword="true"/> implique
        /// que <see cref="CanValidate"/> et <see cref="CanReject"/> valent également
        /// <see langword="true"/>.</para>
        /// <para>Notification INPC : tout changement de valeur doit être signalé par
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> sur
        /// <c>nameof(CanDeclareOutOfStock)</c>, faute de quoi la garde resterait figée
        /// côté menu.</para>
        /// </remarks>
        bool CanDeclareOutOfStock { get; }
    }
}