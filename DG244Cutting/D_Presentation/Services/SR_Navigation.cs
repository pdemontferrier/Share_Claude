using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;
using DG244Cutting.D_Presentation.Views.Shell;
using System.Windows;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Service technique de navigation WPF de l'application DG244Cutting.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : Cette classe est une dépendance injectable enregistrée selon
    /// le cycle de vie approprié (Singleton recommandé) et consommée via <see cref="IS_Navigation"/>.
    /// Elle constitue le seul composant de la solution autorisé à interagir directement avec
    /// <c>MainWindow</c>, les frames WPF et le <c>Dispatcher</c>.</para>
    /// <para>Objectif : Exécuter fidèlement et de manière techniquement robuste les
    /// opérations de navigation WPF qui lui sont déléguées par <c>UC_Navigation</c>, sans jamais
    /// prendre de décision relative à la logique de navigation.</para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Naviguer la frame <c>ActivePage</c> vers une URI de page XAML.</item>
    ///   <item>Naviguer la frame <c>ActiveHorizontalMenu</c> vers une URI de menu XAML.</item>
    ///   <item>Réduire le menu horizontal via une URI de menu réduit fournie par l'appelant
    ///   en paramètre de méthode.</item>
    ///   <item>Rafraîchir la page courante par réinstanciation de son type.</item>
    ///   <item>Garantir la synchronisation avec le thread UI via le <c>Dispatcher</c>.</item>
    ///   <item>Classifier et relancer toute exception non prévue via <see cref="IS_ExClassifier"/>.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item>Ne prend aucune décision de navigation — ce rôle appartient à <c>UC_Navigation</c>.</item>
    ///   <item>Ne vérifie aucun droit d'accès — ce rôle appartient à <c>UC_Navigation</c>.</item>
    ///   <item>Ne maintient aucun état de navigation (page courante, historique) — ce rôle
    ///   appartient à <c>UC_Navigation</c>.</item>
    ///   <item>Ne journalise pas et ne notifie pas — la gestion terminale des erreurs
    ///   appartient à <c>UC_Navigation</c> via <c>IU_LogAndNotify</c>.</item>
    /// </list>
    /// </remarks>
    public class SR_Navigation : IS_Navigation
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe pour la construction de la <c>CallChain</c>.
        /// Initialisé via <c>GetType().Name</c> dans le constructeur.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Classificateur d'exceptions — requalifie toute exception brute .NET non prévue
        /// en exception applicative typée avant de la relancer.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le service de navigation WPF avec sa dépendance.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Ce constructeur est appelé par le conteneur d'injection
        /// lors de la résolution de <see cref="IS_Navigation"/>.</para>
        /// <para>Objectif : Initialiser <c>_callee</c> dynamiquement et stocker la
        /// dépendance injectée. Aucune logique métier ni aucun accès à l'état runtime
        /// ne doit figurer dans ce constructeur.</para>
        /// <para>Note d'architecture : Ce service n'injecte aucune interface
        /// <c>ISE_</c>. Toute valeur dont il a besoin pour son exécution lui est transmise
        /// par paramètre depuis <see cref="UC_Navigation"/>, qui assure seul la lecture
        /// des Settings de navigation.</para>
        /// </remarks>
        /// <param name="classifier">
        /// Classificateur d'exceptions applicatives.
        /// </param>
        public SR_Navigation(IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _classifier = classifier;
        }

        #endregion

        #region === Implémentation de IS_Navigation : méthodes publiques ===

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Contexte : Délégation reçue de <c>UC_Navigation</c> après validation
        /// complète de la décision de navigation.</para>
        /// <para>Comportement : Navigue <c>ActivePage</c> vers <c>mapping.PageUri</c>.
        /// L'appel est exécuté sur le thread UI via le <c>Dispatcher</c>. Si aucune fenêtre
        /// principale de type <see cref="MainWindow"/> n'est disponible, la méthode lève une
        /// <see cref="Ex_Infrastructure"/> — le système ne peut pas naviguer sans shell.</para>
        /// </remarks>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si <c>Application.Current</c>, son <c>Dispatcher</c> ou la <c>MainWindow</c>
        /// sont introuvables, ou si toute autre défaillance technique survient lors de la
        /// navigation WPF.
        /// </exception>
        public void NavigateToPage(string caller, PageMapping mapping)
        {
            string callChain = $"{caller} > {_callee} > {nameof(NavigateToPage)}";

            try
            {
                ExecuteOnMainWindow(callChain, mainWindow =>
                {
                    mainWindow.ActivePage.Navigate(mapping.PageUri);
                });
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Comportement : Navigue <c>ActiveHorizontalMenu</c> vers
        /// <paramref name="menuUri"/>. L'appel est exécuté sur le thread UI via le
        /// <c>Dispatcher</c>.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="menuUri"/> est <see langword="null"/>.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si la <c>MainWindow</c> est introuvable ou si toute autre défaillance
        /// technique survient.
        /// </exception>
        public void NavigateHorizontalMenu(string caller, Uri menuUri)
        {
            string callChain = $"{caller} > {_callee} > {nameof(NavigateHorizontalMenu)}";

            if (menuUri is null)
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_01,
                    "L'URI de menu horizontal fournie est nulle.");

            try
            {
                ExecuteOnMainWindow(callChain, mainWindow =>
                {
                    mainWindow.ActiveHorizontalMenu.Navigate(menuUri);
                });
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Comportement : Navigue <c>ActiveHorizontalMenu</c> vers
        /// <paramref name="reduceSource"/>, l'URI du menu horizontal réduit fournie par
        /// l'appelant. L'appel est exécuté sur le thread UI via le <c>Dispatcher</c>.
        /// Aucune logique additionnelle n'est exécutée.</para>
        /// <para>Note d'architecture : L'URI cible est lue par
        /// <see cref="UC_Navigation"/> dans <c>ISE_Navigation.MH_ReduceSource</c> puis transmise
        /// à ce service en paramètre, conformément à la séparation des rôles entre orchestrateur
        /// (qui consulte les Settings) et exécuteur technique (qui agit sur les frames WPF).</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis l'appelant.</param>
        /// <param name="reduceSource">URI du menu horizontal réduit à charger dans la frame.</param>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="reduceSource"/> est <see langword="null"/>.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si la <c>MainWindow</c> est introuvable ou si toute autre défaillance
        /// technique survient.
        /// </exception>
        public void ReduceHorizontalMenu(string caller, Uri reduceSource)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ReduceHorizontalMenu)}";

            if (reduceSource is null)
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_01,
                    "L'URI du menu horizontal réduit fournie est nulle.");

            try
            {
                ExecuteOnMainWindow(callChain, mainWindow =>
                {
                    mainWindow.ActiveHorizontalMenu.Navigate(reduceSource);
                });
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw _classifier.Execute(callChain, ex);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <para>Comportement : Réinstancie le type de la page actuellement contenue
        /// dans <c>ActivePage</c> via <see cref="Activator.CreateInstance(Type)"/>, puis navigue
        /// vers cette nouvelle instance. Si la frame ne contient aucun contenu
        /// (<c>ActivePage.Content is null</c>), la méthode retourne sans action — le rafraîchissement
        /// d'une frame vide n'est pas une erreur.</para>
        /// </remarks>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si la <c>MainWindow</c> est introuvable, si la réinstanciation de la page
        /// échoue, ou si toute autre défaillance technique survient.
        /// </exception>
        public void RefreshCurrentPage(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(RefreshCurrentPage)}";

            try
            {
                ExecuteOnMainWindow(callChain, mainWindow =>
                {
                    var currentContent = mainWindow.ActivePage.Content;
                    if (currentContent is null)
                        return;

                    var pageType = currentContent.GetType();
                    var freshInstance = Activator.CreateInstance(pageType)
                        ?? throw new Ex_Infrastructure(
                            callChain,
                            Ex_Infrastructure.ErrorCodes.IN_ER_04,
                            $"Activator.CreateInstance a retourné null pour le type '{pageType.FullName}'.",
                            null);

                    mainWindow.ActivePage.Navigate(freshInstance);
                });
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

        #region === Méthodes privées : infrastructure WPF ===

        /// <summary>
        /// Exécute une action sur la <see cref="MainWindow"/> en garantissant la synchronisation
        /// avec le thread UI via le <c>Dispatcher</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Cette méthode est le point d'accès unique à <c>MainWindow</c>
        /// dans ce service. Elle centralise la mécanique de Dispatcher afin d'éviter toute
        /// duplication dans les méthodes publiques.</para>
        /// <para>Comportement selon le contexte d'appel :</para>
        /// <list type="bullet">
        ///   <item>Si l'appelant est déjà sur le thread UI (<c>Dispatcher.CheckAccess()</c>),
        ///   l'action est exécutée immédiatement de manière synchrone.</item>
        ///   <item>Si l'appelant est sur un thread de fond, l'action est postée via
        ///   <c>Dispatcher.BeginInvoke</c> de façon asynchrone, sans bloquer le thread appelant.</item>
        /// </list>
        /// <para>Gestion de l'absence de MainWindow : Si <c>Application.Current.MainWindow</c>
        /// n'est pas de type <see cref="MainWindow"/>, une <see cref="Ex_Infrastructure"/> est levée.
        /// Cette situation révèle une incohérence de composition dans le shell WPF.</para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne de traçabilité courante, utilisée pour la construction de la <c>CallChain</c>
        /// de cette méthode privée et pour la contextualisation des exceptions levées.
        /// </param>
        /// <param name="action">Action WPF à exécuter sur la fenêtre principale.</param>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si <c>Application.Current</c> est null, si le <c>Dispatcher</c> est
        /// indisponible, ou si <c>MainWindow</c> n'est pas de type <see cref="MainWindow"/>.
        /// </exception>
        private void ExecuteOnMainWindow(string caller, Action<MainWindow> action)
        {
            string callChain = $"{caller} > {nameof(ExecuteOnMainWindow)}";

            var dispatcher = Application.Current?.Dispatcher;

            if (dispatcher is null)
                throw new Ex_Infrastructure(
                    callChain,
                    Ex_Infrastructure.ErrorCodes.IN_ER_04,
                    "Application.Current ou son Dispatcher est indisponible. " +
                    "La navigation WPF ne peut pas être exécutée hors d'un contexte applicatif actif.",
                    null);

            if (dispatcher.CheckAccess())
            {
                // Déjà sur le thread UI : exécution synchrone immédiate
                ExecuteActionOnMainWindow(callChain, action);
            }
            else
            {
                // Thread de fond : délégation asynchrone non bloquante
                dispatcher.BeginInvoke(new Action(() =>
                    ExecuteActionOnMainWindow(callChain, action)));
            }
        }

        /// <summary>
        /// Résout la <see cref="MainWindow"/> depuis <c>Application.Current.MainWindow</c>
        /// et exécute l'action fournie sur celle-ci.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : Méthode auxiliaire appelée depuis
        /// <see cref="ExecuteOnMainWindow"/> une fois le thread UI garanti. Elle centralise
        /// la résolution et la vérification de la fenêtre principale.</para>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité propagée depuis <see cref="ExecuteOnMainWindow"/>.</param>
        /// <param name="action">Action WPF à exécuter sur la fenêtre principale.</param>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si <c>Application.Current.MainWindow</c> n'est pas de type <see cref="MainWindow"/>.
        /// </exception>
        private void ExecuteActionOnMainWindow(string caller, Action<MainWindow> action)
        {
            string callChain = $"{caller} > {nameof(ExecuteActionOnMainWindow)}";

            if (Application.Current.MainWindow is not MainWindow mainWindow)
                throw new Ex_Infrastructure(
                    callChain,
                    Ex_Infrastructure.ErrorCodes.IN_ER_04,
                    "Application.Current.MainWindow n'est pas de type MainWindow. " +
                    "Vérifier la composition du shell WPF dans le CompositionRoot.",
                    null);

            action(mainWindow);
        }

        #endregion
    }
}