using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using System.Windows;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Service technique transverse de présentation pilotant la fermeture effective de la
    /// fenêtre principale WPF (<c>Application.Current.MainWindow</c>).
    /// </summary>
    /// <remarks>
    /// <para>Contexte : implémentation résidant en <c>D_Presentation/Services</c>
    /// conformément à la deuxième obligation contractuelle de §4.14.3 amendée (couche
    /// cohérente avec le sous-dossier <c>Presentation</c> de l'interface <c>IS_Shutdown</c>
    /// — table de R-4.14.8 amendée). Sous-cas (c) cas Concept ordinaire de la famille
    /// Services.</para>
    /// <para>Objectif : isoler en D_Presentation le geste WPF terminal de fermeture
    /// de la fenêtre principale, déclenché par le UseCase orchestrateur de fermeture
    /// (<c>UC_CloseApplication</c>, fil d'Extension à venir) en délégation terminale.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Construire la <c>callChain</c> au format normatif
    ///   <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> en première instruction
    ///   effective de la méthode publique (§4.5, R-4.5.5).</description></item>
    ///   <item><description>Vérifier l'annulation coopérative via
    ///   <c>ct.ThrowIfCancellationRequested()</c> à l'intérieur du bloc <c>try</c>, dans
    ///   l'ordre validation → ct → opérations (R-4.7.25).</description></item>
    ///   <item><description>Planifier la fermeture de <c>Application.Current.MainWindow</c>
    ///   via <c>Application.Current?.Dispatcher.BeginInvoke</c>, en pilotage non bloquant
    ///   symétrique à celui pratiqué par <c>SR_Notification</c>.</description></item>
    ///   <item><description>Appliquer le patron à quatre catch canonique
    ///   <c>Ex_Business</c> → <c>Ex_Infrastructure</c> → <c>OperationCanceledException</c>
    ///   → <c>Exception</c> avec requalification terminale via <c>IS_ExClassifier</c>
    ///   (R-4.7.1, R-4.7.6, R-4.7.25, R-4.6.13).</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne consomme aucun <c>IC_</c> (Service Presentation hors
    ///   chaîne (1) d'écriture stricte ; I-4.14.9, I-4.14.16).</description></item>
    ///   <item><description>Ne consomme aucun <c>ISE_</c> : aucune valeur de configuration
    ///   n'est nécessaire au geste de fermeture.</description></item>
    ///   <item><description>Ne met à jour aucun Setting utilisateur (en particulier le
    ///   flag <c>ForceClose</c>) — responsabilité des consommateurs amont.</description></item>
    ///   <item><description>N'émet aucune notification utilisateur — responsabilité des
    ///   consommateurs amont via <c>IS_Notification</c> (I-4.7.6).</description></item>
    ///   <item><description>N'introduit aucun <c>Task.Delay</c> et n'orchestre aucune
    ///   séquence multi-étapes (I-4.14.6).</description></item>
    ///   <item><description>N'ouvre, ne valide ni n'annule aucune transaction ; n'appelle
    ///   ni <c>SaveChangesAsync</c> ni <c>BeginTransactionAsync</c>
    ///   (I-4.10.1).</description></item>
    /// </list>
    /// <para><seealso cref="IS_Shutdown"/></para>
    /// <para><seealso cref="IS_ExClassifier"/></para>
    /// </remarks>
    public sealed class SR_Shutdown : IS_Shutdown
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe courante, utilisé pour la propagation de la CallChain au
        /// format normatif <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c> (R-4.5.5).
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de classification des exceptions non contrôlées, consommé dans le
        /// quatrième catch terminal pour la requalification (R-4.7.25). Service transversal
        /// d'utilité du tableau de §4.7 ; son injection ne relève pas de l'orchestration
        /// de scénario interdite par I-4.14.6.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SR_Shutdown"/>.
        /// </summary>
        /// <param name="classifier">Service de classification des exceptions non
        /// contrôlées.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est <see langword="null"/>.
        /// </exception>
        public SR_Shutdown(IS_ExClassifier classifier)
        {
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Déclenche la fermeture effective de la fenêtre principale WPF par pilotage non
        /// bloquant du <c>Dispatcher</c>.
        /// </summary>
        /// <remarks>
        /// <para>Pilotage : la fermeture est planifiée via
        /// <c>Application.Current?.Dispatcher.BeginInvoke</c> et ferme
        /// <c>Application.Current.MainWindow</c> en appel non bloquant — symétrique au
        /// pilotage Dispatcher pratiqué par <c>SR_Notification</c>. La méthode retourne
        /// immédiatement après planification.</para>
        /// <para>Robustesse : les références <c>Application.Current</c> et
        /// <c>Application.Current.MainWindow</c> sont consultées avec opérateurs de
        /// propagation du <see langword="null"/> ; si l'instance applicative ou sa fenêtre
        /// principale est absente au moment de l'appel, le geste est silencieux. Ce cas
        /// non nominal relève de l'orchestrateur amont.</para>
        /// <para>Ordre canonique <c>try</c> : annulation coopérative (préconditions
        /// structurelles non applicables en l'absence d'argument métier) puis opération
        /// principale, conformément à R-4.7.25.</para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être
        /// <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative.
        /// Par défaut <see langword="default"/>.</param>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est demandée avant la planification de la fermeture.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée par requalification terminale via <see cref="IS_ExClassifier"/> si une
        /// exception non prévue est issue du sous-système WPF.
        /// </exception>
        public async Task ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Aucune précondition structurelle métier à valider : la méthode n'expose
                // aucun argument métier. La vérification d'annulation coopérative est donc
                // la première instruction effective du try.
                ct.ThrowIfCancellationRequested();

                // Pilotage WPF non bloquant : planification de la fermeture sur le
                // Dispatcher de l'application courante. Le geste est terminal.
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Application.Current?.MainWindow?.Close();
                }));

                // Maintien de la signature asynchrone du contrat IS_Shutdown ; le geste
                // de fermeture est planifié sur le Dispatcher, sans attente.
                await Task.CompletedTask;
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
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