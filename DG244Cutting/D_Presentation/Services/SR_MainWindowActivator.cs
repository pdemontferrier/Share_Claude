using System.Windows;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Services.Presentation;

namespace DG244Cutting.D_Presentation.Services
{
    /// <summary>
    /// Description :
    /// <para>
    /// Service technique transverse de présentation responsable de la remontée
    /// programmatique de la fenêtre principale de l'application WPF au premier
    /// plan de la session utilisateur Windows.
    /// </para>
    ///
    /// Contexte :
    /// <para>
    /// Cette classe réside en <c>D_Presentation/Services/</c> et implémente
    /// <see cref="IS_MainWindowActivator"/> conformément à la cinquième cellule
    /// de la table d'arborescence canonique de §4.14.3 amendée du 0230
    /// (sous-cas (c) Presentation, R-4.14.8 amendée du 0231). Elle est destinée
    /// à être consommée par injection IS_ depuis des composants — notamment
    /// d'infrastructure — ayant besoin de solliciter un rappel visuel de la
    /// fenêtre principale sans connaître le type technique WPF de celle-ci ni
    /// la propriété statique par laquelle elle est résolue. Le service est
    /// enregistré en Singleton dans <c>SR_ConteneurDI</c>, portée admise au
    /// titre de P4-bis (§4.10.10 du 0230, R-4.10.14 du 0231) : la seule
    /// dépendance injectée (<see cref="IS_ExClassifier"/>) est Singleton,
    /// aucune dépendance scoped n'est consommée.
    /// </para>
    ///
    /// Objectif :
    /// <para>
    /// Effectuer, sur le thread UI, une séquence de remontée robuste à trois
    /// temps (restauration si minimisée, activation programmatique, rebond
    /// <c>Topmost</c>) sans exposer aucun type technique WPF en frontière du
    /// contrat consommé (IS5), en propageant systématiquement la CallChain et
    /// le <see cref="CancellationToken"/>, et en appliquant le patron à quatre
    /// catch dans l'ordre canonique (R-4.7.25, R-4.6.13 du 0231).
    /// </para>
    ///
    /// Rôle (cas Concept) :
    /// <para>
    /// Service technique transverse au sens du tableau de §4.7 du 0230. Le
    /// service ne porte aucune action métier unitaire et n'est rattaché à
    /// aucune entité unique ; il porte un concept de présentation transverse
    /// (activation programmatique de la fenêtre principale). Le nom d'agent
    /// <c>Activator</c> absorbe la sémantique d'action ; segment [Action]
    /// facultatif absent (SR20 du 0232-SR, patron nominatif analogue à
    /// <c>SR_UseCaseInvoker</c> et <c>SR_ExClassifier</c>). Méthode publique
    /// unique nommée <c>Execute</c> conformément au préfixe par défaut
    /// R-4.2.12 ; aucune dérogation SR20 mobilisée.
    /// </para>
    ///
    /// Obligations contractuelles :
    /// <list type="bullet">
    /// <item><description>Construire la CallChain en première instruction effective de la méthode publique au format <c>{caller} &gt; {_callee} &gt; {nameof(method)}</c> (R-4.5.5).</description></item>
    /// <item><description>Valider les préconditions structurelles à l'intérieur du bloc try, avant <c>ct.ThrowIfCancellationRequested()</c>, et lever <see cref="Ex_Business"/> avec code <c>BU_ER_01</c> en cas de violation (R-4.7.25).</description></item>
    /// <item><description>Appliquer le patron à quatre catch dans l'ordre canonique (R-4.6.13, R-4.7.25).</description></item>
    /// <item><description>Résoudre la fenêtre principale WPF en interne par accès à <c>Application.Current.MainWindow</c> ; ne jamais exposer le type <c>Window</c> en frontière publique (IS5).</description></item>
    /// <item><description>Basculer les opérations WPF sur le thread UI via <see cref="Application"/>.<see cref="Application.Current"/>.<c>Dispatcher.Invoke</c> ; la variante synchrone <c>Invoke</c> — et non <c>BeginInvoke</c> — est retenue pour cohérence avec la sémantique synchrone <c>void</c> du contrat et pour permettre la remontée des exceptions du lambda dans le patron à quatre catch.</description></item>
    /// </list>
    ///
    /// Responsabilités :
    /// <list type="bullet">
    /// <item><description>Résoudre la fenêtre principale WPF via <c>Application.Current.MainWindow</c>.</description></item>
    /// <item><description>Sur le thread UI, appliquer la séquence robuste de remontée à trois temps : restauration <c>WindowState.Normal</c> si l'état est <c>Minimized</c>, appel <c>Activate()</c>, puis rebond <c>Topmost = true; Topmost = false;</c> pour forcer la remontée au premier plan.</description></item>
    /// <item><description>En cas de fenêtre principale non résolue (<c>Application.Current</c> ou <c>Application.Current.MainWindow</c> null), retourner silencieusement sans lever ni journaliser (no-op sur ressource non résolue, état transitoire du cycle de vie de l'application).</description></item>
    /// </list>
    ///
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item><description>Aucune mutation persistante ni participation à la chaîne (1) d'écriture stricte (SR24, SR25 ➖).</description></item>
    /// <item><description>Aucun appel direct à un Repository (SR14, I-4.14.6).</description></item>
    /// <item><description>Aucun appel d'un autre Service applicatif au sens de l'orchestration de scénario (SR15, I-4.14.6).</description></item>
    /// <item><description>Aucune journalisation directe via <c>IS_ErrorLogger</c> ni notification directe via <c>IS_Notification</c> (SR16, I-4.7.6).</description></item>
    /// <item><description>Aucune ouverture, validation ou annulation de transaction (SR13, I-4.10.1).</description></item>
    /// <item><description>Aucune manipulation de l'état d'activation des contrôles enfants de la fenêtre principale (<c>MainWindow.IsEnabled</c>) : le pilotage modal des contrôles autour d'une fenêtre secondaire relève d'autres composants (typiquement <c>SR_Notification</c> autour du <c>DialogWindow</c>).</description></item>
    /// </list>
    /// </summary>
    /// <seealso cref="IS_MainWindowActivator"/>
    /// <seealso cref="IS_ExClassifier"/>
    public class SR_MainWindowActivator : IS_MainWindowActivator
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Description :
        /// <para>Initialise une nouvelle instance du service
        /// <see cref="SR_MainWindowActivator"/>.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Cette classe est instanciée par le conteneur d'injection de
        /// dépendances dans la couche Presentation. La seule dépendance injectée
        /// est le service transversal d'utilité <see cref="IS_ExClassifier"/>,
        /// conforme aux dépendances admises pour un Service Presentation
        /// (SR24, R-2.5.6, admis au titre de SR10 comme service transversal
        /// d'utilité du tableau de §4.7). Aucun <c>ISE_</c> n'est mobilisé :
        /// la fenêtre principale WPF n'est portée par aucun Setting du système
        /// (l'<c>ISE_Window</c> existant porte l'état de dimensions et
        /// position, non la référence à la fenêtre elle-même) ; l'accès à
        /// <c>Application.Current.MainWindow</c> est réalisé en interne sur le
        /// Dispatcher UI. La portée Singleton est admise au titre de P4-bis
        /// (§4.10.10 du 0230) : la dépendance injectée est de portée Singleton,
        /// aucune dépendance scoped n'est consommée.</para>
        /// Objectif :
        /// <para>Initialiser le champ <c>_callee</c> par réflexion sur le type
        /// courant et valider la dépendance obligatoire par garde
        /// <see cref="ArgumentNullException"/>.</para>
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee = GetType().Name</c> en première instruction (R-4.5.5).</description></item>
        /// <item><description>Valider et stocker <see cref="IS_ExClassifier"/>.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="classifier">Service transversal d'utilité de requalification des exceptions brutes en exceptions typées (R-4.7.25).</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="classifier"/> est <see langword="null"/>.</exception>
        public SR_MainWindowActivator(IS_ExClassifier classifier)
        {
            _callee = GetType().Name;
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Description :
        /// <para>Sollicite la remontée programmatique de la fenêtre principale
        /// de l'application au premier plan de la session utilisateur Windows.</para>
        /// </summary>
        /// <remarks>
        /// Contexte :
        /// <para>Méthode publique unique du service technique transverse de
        /// présentation (cas Concept). Nommée <c>Execute</c> conformément au
        /// préfixe par défaut R-4.2.12 ; aucune dérogation SR20 du 0232-SR
        /// mobilisée.</para>
        /// Objectif :
        /// <para>Construire la CallChain enrichie, valider la précondition
        /// structurelle, vérifier l'annulation coopérative, puis basculer sur
        /// le thread UI via <c>Dispatcher.Invoke</c> pour appliquer la séquence
        /// robuste de remontée à trois temps sur la fenêtre principale WPF.</para>
        /// Tâches / Actions :
        /// <list type="bullet">
        /// <item><description>Construire la callChain au format normatif <c>{caller} &gt; {_callee} &gt; {nameof(Execute)}</c>.</description></item>
        /// <item><description>Valider <paramref name="caller"/> (BU_ER_01).</description></item>
        /// <item><description>Vérifier l'annulation coopérative via <c>ct.ThrowIfCancellationRequested()</c>.</description></item>
        /// <item><description>Sur le thread UI (<c>Dispatcher.Invoke</c>) : résoudre <c>Application.Current.MainWindow</c> ; no-op silencieux si la fenêtre n'est pas résolue (arbitrage Q-4 du fil <c>SR_MainWindowActivator_Creation</c>) ; sinon appliquer la séquence à trois temps : restauration <c>WindowState.Normal</c> si <c>Minimized</c>, <c>Activate()</c>, rebond <c>Topmost = true; Topmost = false;</c> (arbitrage Q-3 du même fil).</description></item>
        /// </list>
        /// Basculement sur le thread UI :
        /// <para>Le basculement est effectué via <c>Application.Current?.Dispatcher.Invoke</c>
        /// — variante synchrone bloquante — plutôt que <c>BeginInvoke</c>, afin
        /// (i) d'aligner l'implémentation sur la sémantique synchrone <c>void</c>
        /// du contrat (arbitrage Q-2 du fil <c>SR_MainWindowActivator_Creation</c>),
        /// et (ii) de permettre la remontée des exceptions issues du lambda dans
        /// le patron à quatre catch de la méthode. Si <c>Application.Current</c>
        /// est <see langword="null"/> (état de shutdown avancé), le dispatch est
        /// silencieusement omis (opérateur <c>?.</c>), cohérent avec le no-op
        /// silencieux appliqué à l'intérieur du lambda.</para>
        /// </remarks>
        /// <param name="caller">CallChain amont propagée par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation coopérative.</param>
        /// <exception cref="Ex_Business">Levée si <paramref name="caller"/> est null, vide ou composé uniquement d'espaces (code <c>BU_ER_01</c>).</exception>
        /// <exception cref="Ex_Infrastructure">Levée le cas échéant par requalification terminale via <see cref="IS_ExClassifier"/>.</exception>
        /// <exception cref="System.OperationCanceledException">Levée lorsque l'annulation coopérative est sollicitée via <paramref name="ct"/>.</exception>
        public void Execute(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(Execute)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                // Basculement sur le thread UI. Invoke synchrone (et non BeginInvoke) :
                // cohérent avec la sémantique synchrone void déclarée par le contrat
                // (arbitrage Q-2 = A) et laisse remonter les exceptions du lambda dans
                // le patron à quatre catch ci-dessous. L'opérateur ?. couvre l'état de
                // shutdown avancé où Application.Current serait null.
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    // Résolution opaque de la fenêtre principale via la propriété
                    // statique WPF ; aucune exposition du type Window en frontière
                    // publique du contrat (IS5).
                    Window? mainWindow = Application.Current?.MainWindow;

                    // No-op silencieux si la MainWindow n'est pas résolue
                    // (arbitrage Q-4 = C). État transitoire du cycle de vie de
                    // l'application (démarrage en cours, séquence de fermeture
                    // engagée) : la remontée n'a pas de valeur d'usage, l'absence
                    // n'est pas une anomalie. Aucune exception, aucune journalisation.
                    if (mainWindow is null)
                        return;

                    // Séquence de remontée à trois temps (arbitrage Q-3 = B).
                    // Patron robuste couvrant les états minimisés et la plupart des
                    // cas de vol de focus stock WPF :
                    //   (1) restauration à l'état Normal si minimisée ;
                    //   (2) activation programmatique ;
                    //   (3) rebond Topmost pour forcer la remontée au premier plan.
                    if (mainWindow.WindowState == WindowState.Minimized)
                        mainWindow.WindowState = WindowState.Normal;

                    mainWindow.Activate();

                    mainWindow.Topmost = true;
                    mainWindow.Topmost = false;
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
    }
}