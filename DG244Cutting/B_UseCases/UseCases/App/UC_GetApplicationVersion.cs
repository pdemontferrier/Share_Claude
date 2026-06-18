using System.Reflection;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;

namespace DG244Cutting.B_UseCases.UseCases.App
{
    /// <summary>
    /// Implémentation du UseCase chargé de produire le numéro de version courant
    /// de l'application DG244Cutting par lecture de l'attribut Version de
    /// l'assembly d'exécution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : Ce UseCase implémente le contrat <see cref="IU_GetApplicationVersion"/>.
    /// Il est consommé en chaîne (1) directe par <c>VM_Page98.LoadDataAsync</c>
    /// pour alimentation d'une propriété observable <c>VersionNumber</c> bindée à
    /// la vue <c>Page98</c>. Il ne participe à aucune chaîne UC → UC normalisée
    /// au sens de R-4.14.21 (ni consommé en sous-séquence par un orchestrateur
    /// amont, ni lui-même orchestrateur amont d'un autre UseCase). Sa portée DI
    /// est Singleton (P4-bis, §4.10.10 du 0230), aucune dépendance Scoped n'étant
    /// injectée et aucun accès au DbContext partagé n'étant effectué.
    /// </para>
    /// <para>
    /// Objectif : Encapsuler l'accès direct à
    /// <see cref="Assembly.GetExecutingAssembly"/> dans un UseCase doté du
    /// pipeline d'erreur standard, de manière à isoler du reste du code la
    /// connaissance de la mécanique réflexive et à offrir un contrat IU_ stable
    /// au ViewModel consommateur.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Construire la CallChain locale au patron point intermédiaire (§4.5 du 0230).</description></item>
    ///   <item><description>Lire la version portée par l'assembly d'exécution courant.</description></item>
    ///   <item><description>Retourner la version formatée par <see cref="Version.ToString()"/> ou <see cref="string.Empty"/> en l'absence de donnée.</description></item>
    ///   <item><description>Déléguer toute exception applicative typée au pipeline de traitement terminal via <see cref="IU_LogAndNotify"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne formate pas le numéro de version au-delà du <see cref="Version.ToString()"/> natif (aucune troncature, aucune mise en forme localisée).</description></item>
    ///   <item><description>Ne porte aucune logique métier, aucune persistance, aucune mutation d'état applicatif.</description></item>
    ///   <item><description>N'ouvre aucune transaction et n'accède pas au DbContext partagé (item UC14 sans objet).</description></item>
    ///   <item><description>Ne consomme aucun Command Handler, Repository, Query Handler, Service métier, Setting ou autre UseCase.</description></item>
    /// </list>
    /// <para>Exceptions architecturales propres :</para>
    /// <para>
    /// Accès direct à <see cref="Assembly.GetExecutingAssembly"/> justifié par la
    /// nature transverse de l'information portée - identité d'application
    /// non métier, sans entité persistée ni Service d'infrastructure dédié. Le
    /// passage par un Service technique IS_/SR_ intermédiaire a été examiné et
    /// écarté à l'atelier en amont : l'opération est trop élémentaire pour
    /// justifier la création d'un contrat Service et de son implémentation.
    /// L'accès reste confiné à la méthode publique <see cref="ExecuteAsync"/> du
    /// présent UseCase et ne se propage à aucune autre famille.
    /// </para>
    /// <para>
    /// Statut d'inscription : La présente exception architecturale est documentée
    /// dans le présent <c>remarks</c> au titre de la traçabilité dans le code
    /// (§4.3 du 0230). Son inscription formelle à l'inventaire de §4.15.9 du
    /// 0230 sous un numéro <c>EA-NN</c> dédié relève d'un fil de maintenance
    /// documentaire ultérieur et n'a pas été produite au présent fil de
    /// production. La famille UseCases ne porte par ailleurs aucune EA propre à
    /// la connaissance du présent fil (invariant 15 de §2.1 du 0232-UC).
    /// </para>
    /// </remarks>
    /// <seealso cref="IU_GetApplicationVersion"/>
    public class UC_GetApplicationVersion : IU_GetApplicationVersion
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe courante, capturé à la construction et utilisé pour la
        /// construction de la CallChain locale (patron point intermédiaire, §4.5
        /// du 0230). Initialisé dans le constructeur par <c>GetType().Name</c>
        /// conformément à R-4.4.5 et R-4.4.7.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Contrat du UseCase orchestrant le traitement terminal des erreurs
        /// (journalisation et notification). Injecté par le conteneur DI et
        /// invoqué dans chacun des trois blocs <c>catch</c> typés du pipeline de
        /// capture standard (§4.7.3 du 0230) avec la clé dictionnaire
        /// <c>No_EC_NN</c> appropriée.
        /// </summary>
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_GetApplicationVersion"/>.
        /// </summary>
        /// <param name="logAndNotify">
        /// Contrat du UseCase de traitement terminal des erreurs. Ne doit pas
        /// être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="logAndNotify"/> est <see langword="null"/>.
        /// </exception>
        public UC_GetApplicationVersion(IU_LogAndNotify logAndNotify)
        {
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Produit le numéro de version courant de l'application DG244Cutting sous
        /// forme d'une chaîne de caractères.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : Cette méthode est appelée par un ViewModel consommateur
        /// (<c>VM_Page98.LoadDataAsync</c> dans la chaîne d'appel actuellement
        /// référencée) après construction de la CallChain racine via
        /// <c>BuildFirstCallChain</c>.
        /// </para>
        /// <para>
        /// Objectif : Lire le numéro de version porté par
        /// <see cref="Assembly.GetExecutingAssembly"/> et le restituer dans un
        /// format directement consommable par la couche de présentation.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        ///   <item><description>Construire la CallChain locale au patron point intermédiaire.</description></item>
        ///   <item><description>Vérifier le signal d'annulation coopérative à l'entrée du bloc <c>try</c>.</description></item>
        ///   <item><description>Lire la propriété <see cref="AssemblyName.Version"/> de l'assembly d'exécution courant.</description></item>
        ///   <item><description>Retourner <see cref="Version.ToString()"/> lorsque la version est présente, ou <see cref="string.Empty"/> en cas d'absence légitime ou d'exception applicative typée capturée.</description></item>
        /// </list>
        /// <para>
        /// Particularité doctrinale : La signature <see cref="Task{TResult}"/>
        /// avec <c>TResult = string</c> est retenue non au titre du retour
        /// signalable de R-4.14.21, puisque ce UseCase n'est pas consommé en
        /// sous-séquence par un orchestrateur amont, mais parce que la valeur
        /// retournée est la donnée fonctionnelle elle-même - le numéro de
        /// version - et non un signal d'issue de scénario. Cette particularité
        /// est tracée nominativement conformément à R-4.2.13. Le préfixe
        /// normatif <c>ExecuteAsync</c> est préservé, aucune dérogation au
        /// préfixe n'étant portée par cette méthode.
        /// </para>
        /// <para>Pipeline de capture (§4.7.3 du 0230, item UC10) :</para>
        /// <list type="bullet">
        ///   <item><description><see cref="Ex_Business"/> : délégation à <see cref="IU_LogAndNotify"/> avec la clé <c>"No_EC_01"</c>, retour <see cref="string.Empty"/>.</description></item>
        ///   <item><description><see cref="Ex_Infrastructure"/> : délégation à <see cref="IU_LogAndNotify"/> avec la clé <c>"No_EC_02"</c>, retour <see cref="string.Empty"/>.</description></item>
        ///   <item><description><see cref="Ex_Unclassified"/> : délégation à <see cref="IU_LogAndNotify"/> avec la clé <c>"No_EC_03"</c>, retour <see cref="string.Empty"/>.</description></item>
        ///   <item><description><see cref="OperationCanceledException"/> : propagation silencieuse par <c>throw;</c> sans journalisation ni notification, conformément à §4.6.3 du 0230.</description></item>
        /// </list>
        /// <para>
        /// Aucun <c>catch (Exception ex)</c> terminal côté UseCase, conformément
        /// à l'item UC12 et à §4.7.3 du 0230. Le filet ultime applicatif est
        /// porté en aval par <c>VM_Page_Generic.ExecuteSafeAsync</c> (§4.15.5
        /// du 0230).
        /// </para>
        /// <para>
        /// CallChain : Construite localement au format
        /// <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> selon le
        /// patron point intermédiaire de §4.5 du 0230 ; propagée à
        /// <see cref="IU_LogAndNotify"/> dans chaque bloc <c>catch</c> typé.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// CallChain amont construite par le ViewModel consommateur via
        /// <c>BuildFirstCallChain</c>. Obligatoire ; transmise telle quelle au
        /// pipeline d'erreur le cas échéant.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative. Vérifié à l'entrée du bloc <c>try</c>
        /// et propagé à <see cref="IU_LogAndNotify"/> en cas de capture
        /// d'exception applicative typée. Valeur par défaut :
        /// <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Le numéro de version courant de l'application, tel que retourné par
        /// <see cref="Version.ToString()"/>, ou <see cref="string.Empty"/> si
        /// l'assembly d'exécution n'expose pas de propriété <c>Version</c>, ou
        /// si une exception applicative typée est capturée par le pipeline
        /// d'erreur standard.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée silencieusement à l'appelant sur signal d'annulation
        /// coopérative, conformément à §4.6.3 du 0230.
        /// </exception>
        public async Task<string> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                var version = Assembly.GetExecutingAssembly().GetName().Version;

                if (version is null)
                    return string.Empty;

                return version.ToString();
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                return string.Empty;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                return string.Empty;
            }
            catch (Ex_Unclassified ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                return string.Empty;
            }
            catch (OperationCanceledException)
            {
                // Signal d'annulation coopérative : propagation silencieuse à
                // l'appelant. Aucune journalisation, aucune notification
                // (§4.6.3 du 0230, item UC11).
                throw;
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}