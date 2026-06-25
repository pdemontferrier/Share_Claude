using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Settings.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.App
{
    /// <summary>
    /// Contrat du UseCase orchestrateur du changement de langue de l'application
    /// pour la session courante.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette interface est définie dans <c>A_Domain</c> afin d'être
    /// accessible aux UseCases orchestrateurs amont (typiquement
    /// <c>UC_Application_OnStart</c> au démarrage de l'application) et aux ViewModels
    /// (typiquement un ViewModel de sélection de langue déclenché par l'opérateur)
    /// sans dépendance vers <c>B_UseCases</c>. Son implémentation concrète
    /// <see cref="B_UseCases.UseCases.App.UC_Language_Apply"/> réside dans
    /// <c>B_UseCases/UseCases/App/</c>.</para>
    ///
    /// <para>Objectif : Constituer le point d'entrée unique et robuste pour
    /// l'application d'une langue à l'interface de l'application, en transposant
    /// vers la couche <c>B_UseCases</c> la responsabilité d'orchestration jusqu'alors
    /// portée par le Service de présentation <c>SR_Language</c>. Le UseCase coordonne
    /// dans un ordre déterministe le chargement du dictionnaire XAML correspondant
    /// au code culture demandé, la persistance du code culture dans le contexte
    /// applicatif, la mise à jour de l'URI du drapeau associé à la langue, et la
    /// synchronisation des quatre cibles standard de
    /// <see cref="System.Globalization.CultureInfo"/> .NET afin que les formats
    /// nombre / date / heure rendus par les contrôles WPF et les composants tiers
    /// reflètent la nouvelle langue.</para>
    ///
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Valider que le code culture reçu est non
    ///   <see langword="null"/>, non vide et non composé uniquement d'espaces
    ///   blancs.</description></item>
    ///   <item><description>Court-circuiter immédiatement par idempotence si le
    ///   code culture demandé est déjà le code culture actif.</description></item>
    ///   <item><description>Déclencher le chargement du dictionnaire XAML
    ///   correspondant via <see cref="ISE_Language"/>.</description></item>
    ///   <item><description>Persister le code culture actif dans
    ///   <see cref="ISE_App.AppCultureCode"/>, déclenchant la cascade INPC vers
    ///   le rechargement des labels côté Presentation.</description></item>
    ///   <item><description>Synchroniser l'URI du drapeau associé à la langue via
    ///   <see cref="ISE_Flag"/>.</description></item>
    ///   <item><description>Synchroniser les quatre cibles standard de
    ///   <see cref="System.Globalization.CultureInfo"/> .NET
    ///   (<c>DefaultThreadCurrentCulture</c>, <c>DefaultThreadCurrentUICulture</c>,
    ///   <c>Thread.CurrentThread.CurrentCulture</c>,
    ///   <c>Thread.CurrentThread.CurrentUICulture</c>).</description></item>
    ///   <item><description>Assurer le traitement terminal des erreurs applicatives
    ///   via <c>IU_LogAndNotify</c> selon le patron de trois catch typés
    ///   (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
    ///   <see cref="Ex_Unclassified"/>) avec propagation distincte de
    ///   <see cref="OperationCanceledException"/>.</description></item>
    ///   <item><description>Propager la <c>CallChain</c> à chaque composant
    ///   appelé.</description></item>
    /// </list>
    ///
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Aucune manipulation directe de
    ///   <c>ResourceDictionary</c> WPF : déléguée à <see cref="ISE_Language"/>.</description></item>
    ///   <item><description>Aucune logique métier.</description></item>
    ///   <item><description>Aucun accès aux données persistées en base.</description></item>
    ///   <item><description>Aucune décision sur la langue à appliquer : cette
    ///   responsabilité appartient à l'appelant (UseCase orchestrateur de
    ///   séquence de démarrage ou ViewModel de sélection de langue).</description></item>
    ///   <item><description>Aucune classification d'exception via
    ///   <c>IS_ExClassifier</c> : le patron de trois catch typés couvre nativement
    ///   les trois familles d'exceptions applicatives, et le traitement terminal
    ///   est porté par <c>IU_LogAndNotify</c>.</description></item>
    /// </list>
    ///
    /// <para>Note sur la transactionnalité : Le changement de langue ne constitue
    /// pas un scénario d'écriture en base de données. Ce UseCase n'ouvre donc pas
    /// de transaction SQL. Il s'inscrit dans l'architecture UseCase par son rôle
    /// d'orchestration d'une mécanique transverse de présentation et de runtime,
    /// et par son traitement terminal des erreurs via <c>IU_LogAndNotify</c>,
    /// conformément aux principes définis dans la partie 3 du référentiel.</para>
    ///
    /// <para>Note sur la convention de méthode publique : Ce contrat expose une
    /// méthode publique unique <see cref="ExecuteAsync"/>, conformément à la
    /// configuration nominale du cas Concept au sens de la convention de nommage
    /// UC_ dual-cas Entité / Concept (R-4.14.7 amendée). Le préfixe canonique
    /// <c>ExecuteAsync</c> est conservé sans dérogation : la sémantique du concept
    /// porté (application d'une langue à l'application) ne justifie pas la
    /// substitution par un verbe d'action plus précis admise par la double
    /// dérogation typologiquement bornée de R-4.2.13. Aucune trace de dérogation
    /// n'est par conséquent à porter dans le <c>&lt;remarks&gt;</c> de la méthode
    /// publique.</para>
    ///
    /// <para>Note sur la CallChain : Le paramètre <c>caller</c> reçu en première
    /// position obligatoire conformément à la convention de signature canonique
    /// (R-4.5.7) est enrichi à l'entrée de la méthode publique selon le patron
    /// normatif <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> (R-4.5.1)
    /// et propagé en aval vers <c>IU_LogAndNotify</c> dans chacun des trois catch
    /// typés.</para>
    /// <seealso cref="B_UseCases.UseCases.App.UC_Language_Apply"/>
    /// </remarks>
    public interface IU_Language_Apply
    {
        /// <summary>
        /// Applique la langue correspondant au code culture fourni en orchestrant
        /// l'ensemble des étapes du changement de langue pour la session courante.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Appelée au démarrage de l'application depuis le UseCase
        /// orchestrateur de séquence de démarrage (typiquement
        /// <c>UC_Application_OnStart</c>), ou lors d'un changement de langue
        /// explicite déclenché par l'opérateur depuis une page de sélection de
        /// langue.</para>
        ///
        /// <para>Séquence d'exécution :</para>
        /// <list type="number">
        ///   <item><description>Construction de la <c>CallChain</c> au format
        ///   normatif <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c>
        ///   (R-4.5.1).</description></item>
        ///   <item><description>Validation défensive de
        ///   <paramref name="cultureCode"/> : levée d'<see cref="Ex_Business"/>
        ///   typée si <see langword="null"/>, vide ou composé uniquement
        ///   d'espaces blancs.</description></item>
        ///   <item><description>Vérification de l'annulation coopérative via
        ///   <see cref="CancellationToken.ThrowIfCancellationRequested"/>.</description></item>
        ///   <item><description>Court-circuit d'idempotence : si
        ///   <paramref name="cultureCode"/> est déjà le code culture actif
        ///   (<see cref="ISE_App.AppCultureCode"/>), retour immédiat sans
        ///   effet.</description></item>
        ///   <item><description>Sous-étape d'effet 1/4 - Chargement du dictionnaire
        ///   XAML correspondant via <see cref="ISE_Language"/>.</description></item>
        ///   <item><description>Sous-étape d'effet 2/4 - Persistance du code
        ///   culture dans <see cref="ISE_App.AppCultureCode"/>, déclenchant la
        ///   cascade INPC vers le rechargement des labels côté Presentation.</description></item>
        ///   <item><description>Sous-étape d'effet 3/4 - Synchronisation de l'URI
        ///   du drapeau via <see cref="ISE_Flag"/>.</description></item>
        ///   <item><description>Sous-étape d'effet 4/4 - Synchronisation des
        ///   quatre cibles standard de <see cref="System.Globalization.CultureInfo"/>
        ///   .NET.</description></item>
        /// </list>
        ///
        /// <para>Comportement en cas d'erreur : Les trois familles d'exceptions
        /// applicatives (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) sont captées terminalement par le
        /// UseCase lui-même via le patron de trois catch typés et déléguées au
        /// pipeline terminal <c>IU_LogAndNotify</c> avec une clé dictionnaire
        /// dédiée (<c>No_EC_01</c>, <c>No_EC_02</c>, <c>No_EC_03</c>). Aucune
        /// de ces trois exceptions n'est propagée à l'appelant. Seul
        /// <see cref="OperationCanceledException"/> est propagé conformément à
        /// la doctrine d'annulation coopérative §4.6.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant
        /// (UseCase orchestrateur amont ou ViewModel), enrichie à l'entrée du
        /// UseCase selon le patron normatif
        /// <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c>.</param>
        /// <param name="cultureCode">Code culture à appliquer (par exemple
        /// <c>"fr-FR"</c>, <c>"en-GB"</c>). Doit être non <see langword="null"/>,
        /// non vide et non composé uniquement d'espaces blancs.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut
        /// <see langword="default"/>.</param>
        /// <returns>
        /// <para>Une tâche représentant l'exécution asynchrone du changement de
        /// langue, dont la valeur signalable au sens de R-4.14.21 est :</para>
        /// <list type="bullet">
        ///   <item><description><see langword="true"/> lorsque la langue cible
        ///   est constatée appliquée à l'issue de l'invocation, indistinctement
        ///   au terme du chemin nominal d'exécution des quatre sous-étapes
        ///   d'effet ou au terme du court-circuit d'idempotence (cas où le code
        ///   culture demandé est déjà le code culture actif).</description></item>
        ///   <item><description><see langword="false"/> lorsqu'une des trois
        ///   familles d'exceptions applicatives (<see cref="Ex_Business"/>,
        ///   <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>)
        ///   est captée terminalement par le UseCase ; la valeur permet à
        ///   l'orchestrateur amont (cas de consommation en sous-séquence par
        ///   <c>UC_Application_OnStart</c>) de constater l'échec applicatif
        ///   sans propagation d'exception, conformément à la doctrine de chaîne
        ///   UC → UC normalisée (R-4.14.21).</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque le jeton <paramref name="ct"/> est
        /// déclenché pendant l'opération, conformément à la doctrine d'annulation
        /// coopérative §4.6 du référentiel.
        /// </exception>
        Task<bool> ExecuteAsync(string caller, string cultureCode, CancellationToken ct = default);
    }
}