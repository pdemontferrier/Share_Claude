using System.Threading;
using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Presentation
{
    /// <summary>
    /// Description :
    /// <para>
    /// Contrat du service technique transverse de présentation responsable de la
    /// remontée programmatique de la fenêtre principale de l'application au
    /// premier plan de la session utilisateur Windows.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// L'interface réside en <c>A_Domain/Interfaces/Services/Presentation/</c>
    /// (couche d'appartenance unique pour les contrats de la famille IS_,
    /// invariant 1 de §2.1 du 0230). Son implémentation concrète
    /// <see cref="DG244Cutting.D_Presentation.Services.SR_MainWindowActivator"/>
    /// réside en <c>D_Presentation/Services/</c>. Le service est destiné à être
    /// consommé par injection IS_ depuis des composants — notamment
    /// d'infrastructure — ayant besoin de solliciter un rappel visuel de la
    /// fenêtre principale sans connaître le type technique WPF de celle-ci ni
    /// la propriété statique par laquelle elle est résolue.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Exposer sous forme opaque l'action de remontée programmatique de la
    /// fenêtre principale au premier plan, en propageant explicitement la
    /// CallChain et le <see cref="CancellationToken"/> (R-4.5.5, R-4.6.13 du
    /// 0231), sans exposer aucun type technique WPF en frontière de contrat
    /// (IS5 ; I-2.4.1, I-2.4.2 du 0231).
    /// </para>
    ///
    /// Rôle (cas Concept) :
    /// <para>
    /// Service technique transverse au sens du tableau de §4.7 du 0230, porteur
    /// d'un concept de présentation transverse (activation programmatique de la
    /// fenêtre principale). Le nom d'agent <c>Activator</c> absorbe la
    /// sémantique d'action ; le segment [Action] est facultatif en cas Concept
    /// (R-4.14.8 amendée du 0231). Patron nominatif analogue aux étalons
    /// doctrinaux <c>SR_UseCaseInvoker</c> et <c>SR_ExClassifier</c>.
    /// </para>
    ///
    /// Obligations contractuelles :
    /// <list type="bullet">
    /// <item><description>Résider en <c>A_Domain/Interfaces/Services/Presentation/</c>.</description></item>
    /// <item><description>Implémentation par <see cref="DG244Cutting.D_Presentation.Services.SR_MainWindowActivator"/> active en <c>D_Presentation/Services/</c>.</description></item>
    /// <item><description>Signature de traçabilité sur la méthode publique : <c>string caller</c> en tête et <c>CancellationToken ct = default</c> en queue (R-4.5.5).</description></item>
    /// <item><description>Aucune signature transactionnelle exposée (invariant 9 de §2.1).</description></item>
    /// <item><description>Pureté contractuelle : aucune référence à WPF, EF Core ou tout type technique étranger à A_Domain (I-2.4.1, I-2.4.2, IS5). La fenêtre principale WPF est résolue en interne de l'implémentation par accès à la propriété statique du singleton d'<c>Application</c> ; sa référence n'est ni argument ni retour du contrat.</description></item>
    /// </list>
    ///
    /// Responsabilités :
    /// <list type="bullet">
    /// <item><description>Solliciter, sur le thread UI, la remontée programmatique de la fenêtre principale au premier plan de la session utilisateur Windows.</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune mutation persistante ni participation à la chaîne (1) d'écriture stricte (Service Presentation hors chaîne (1), I-4.14.9, I-4.14.16 du 0231).</description></item>
    /// <item><description>Aucune décision métier ni orchestration de scénario (I-4.14.6 du 0231).</description></item>
    /// <item><description>Aucun appel direct à un Repository (I-4.14.6, I-4.14.9 du 0231).</description></item>
    /// <item><description>Aucun appel direct à un Command Handler ou à un Query Handler (I-4.14.9, I-4.14.16 du 0231).</description></item>
    /// <item><description>Aucune ouverture, validation ou annulation de transaction (I-4.10.1 du 0231).</description></item>
    /// <item><description>Aucune journalisation d'erreur applicative (responsabilité du pipeline terminal du UseCase via <c>UC_LogAndNotify</c>, R-4.7.14 du 0231).</description></item>
    /// <item><description>Aucune notification utilisateur (responsabilité d'<c>IS_Notification</c>).</description></item>
    /// <item><description>Aucune manipulation de l'état d'activation des contrôles enfants de la fenêtre principale (<c>MainWindow.IsEnabled</c>), laquelle relève du pilotage modal porté par d'autres composants (typiquement <c>SR_Notification</c> autour du <c>DialogWindow</c>).</description></item>
    /// </list>
    /// </summary>
    /// <seealso cref="DG244Cutting.D_Presentation.Services.SR_MainWindowActivator"/>
    public interface IS_MainWindowActivator
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>Sollicite la remontée programmatique de la fenêtre principale de
        /// l'application au premier plan de la session utilisateur Windows.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique unique du service technique transverse de
        /// présentation (cas Concept). Nommée <c>Execute</c> conformément au
        /// préfixe par défaut R-4.2.12 du 0231 ; aucune dérogation
        /// typologiquement bornée admise par SR20 du 0232-SR n'est mobilisée.</para>
        /// Objectif :
        /// <para>Exposer sous forme opaque le besoin d'un consommateur amont de
        /// solliciter un rappel visuel de la fenêtre principale, sans exposition
        /// du type technique WPF de celle-ci ni de la propriété statique par
        /// laquelle elle est résolue.</para>
        /// Comportement en cas de fenêtre principale non résolue :
        /// <para>Si la fenêtre principale n'est pas résolue au moment de l'appel
        /// — état transitoire du cycle de vie de l'application (démarrage en
        /// cours, séquence de fermeture engagée) —, la méthode retourne
        /// silencieusement sans lever ni journaliser. Aucun rappel visuel n'a
        /// de valeur d'usage dans cet état transitoire, et l'absence de fenêtre
        /// principale n'est pas une anomalie mais un état de cycle de vie.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant, enrichie localement au format normatif de §4.5 du 0230. Ne doit pas être <see langword="null"/>, vide ou composée uniquement d'espaces.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">Levée en cas de précondition structurelle violée (code <c>BU_ER_01</c> — argument <paramref name="caller"/> null, vide ou composé uniquement d'espaces).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <c>IS_ExClassifier</c>.</exception>
        /// <exception cref="System.OperationCanceledException">Levée lorsque l'annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        void Execute(string caller, CancellationToken ct = default);

        #endregion
    }
}