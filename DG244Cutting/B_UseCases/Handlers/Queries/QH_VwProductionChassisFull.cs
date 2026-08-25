using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.B_UseCases.Handlers.Generic;

namespace DG244Cutting.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// Query Handler spécialisé dédié à la vue de base de données
    /// <see cref="vw_ProductionChassis_Full"/>, dérivant de <see cref="QH_Generic{T}"/> paramétré
    /// pour cette vue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases et honore le contrat
    /// <see cref="IQ_VwProductionChassisFull"/>. Elle applique le patron principal d'extension par
    /// dérivation défini en §4.15.4 du 0230 : elle hérite du socle de lecture sans redéclarer ni
    /// masquer aucune de ses treize lectures, appelle <c>base(repository, classifier)</c> en
    /// première instruction de son constructeur, et ajoute la seule lecture projetée propre au
    /// besoin couvert.
    /// </para>
    /// <para>
    /// Objectif : exposer à la couche des cas d'usage la composition physique d'une série de
    /// production, à raison d'une ligne par châssis, réduite aux seize champs utiles au troisième
    /// onglet de la Page11. Le consommateur prévu est un service métier <c>SR_</c>, qui applique
    /// le tri et la mise en forme et qui atteint la présente classe par son seul contrat.
    /// </para>
    /// <para>
    /// Sous-cas de lecture spécialisée : la lecture relève du second sous-cas du critère de
    /// §4.14.5 du 0230. Elle mobilise la projection SQL traduite côté base de données, API EF Core
    /// absente du contrat <c>IR_Generic&lt;T&gt;</c> ; elle est donc servie par délégation au
    /// repository spécialisé <see cref="IR_VwProductionChassisFull"/> (Patron 2 de §4.15.2),
    /// injecté au constructeur de la présente classe, le repository du socle demeurant privé et
    /// inaccessible au dérivé.
    /// </para>
    /// <para>
    /// Modèle transactionnel : néant. La classe n'ouvre, ne valide ni n'annule aucune transaction,
    /// n'appelle jamais <c>SaveChangesAsync</c> et n'inscrit aucun enregistrement Event Store. La
    /// lecture est neutre vis-à-vis du périmètre transactionnel ; la question est au demeurant
    /// sans objet, la vue étant une source de lecture seule.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Valider la précondition structurelle portant sur l'identifiant de série avant toute
    ///     délégation.
    ///   </description></item>
    ///   <item><description>
    ///     Déléguer la lecture projetée au repository spécialisé consommé via son contrat, et
    ///     rendre son résultat sans transformation aucune.
    ///   </description></item>
    ///   <item><description>
    ///     Enrichir et propager la CallChain reçue au format
    ///     <c>{caller} &gt; {_callee} &gt; {nom de la méthode}</c>, et propager le jeton
    ///     d'annulation au maillon aval.
    ///   </description></item>
    ///   <item><description>
    ///     Requalifier les exceptions non contrôlées via <see cref="IS_ExClassifier"/>, les
    ///     exceptions applicatives typées et l'annulation remontant sans reclassement.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne porte aucun appel EF Core et aucun <c>AsNoTracking()</c> : le suffixe figure au nom
    ///     de la méthode déléguée et l'appel relève du repository. Le filtre <c>IsDeleted</c> est
    ///     sans objet, la vue ne portant pas cette colonne.
    ///   </description></item>
    ///   <item><description>
    ///     Ne redéfinit et ne masque aucune méthode du socle : les treize lectures sont héritées
    ///     telles quelles, la lecture spécialisée est ajoutée à côté du contrat.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune décision métier : le seul contrôle exercé est structurel. Aucun tri,
    ///     aucun filtrage, aucune mise en forme.
    ///   </description></item>
    ///   <item><description>
    ///     Ne journalise pas et ne notifie pas : ces responsabilités appartiennent au UseCase
    ///     orchestrateur.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Surface héritée sur un type sans clé : la vue étant déclarée <c>HasNoKey</c>, trois des
    /// treize lectures héritées sont visibles au consommateur mais échouent à l'exécution sur ce
    /// type. Leur inventaire nominatif et leur cause sont portés par le commentaire de
    /// <see cref="IQ_VwProductionChassisFull"/>, qui constitue l'avertissement opposable au
    /// consommateur.
    /// </para>
    /// </remarks>
    public class QH_VwProductionChassisFull : QH_Generic<vw_ProductionChassis_Full>, IQ_VwProductionChassisFull
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ homonyme de <see cref="QH_Generic{T}"/> :
        /// ce dernier est déclaré <c>private</c> dans le socle et n'est donc pas accessible depuis
        /// une classe dérivée. Le socle n'expose aucune surface <c>protected</c> ; la
        /// re-déclaration est la conséquence normative de cette conception.
        /// </remarks>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>), conservé localement
        /// pour l'usage de la cascade de rattrapage de la lecture spécialisée.
        /// </summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ homonyme de <see cref="QH_Generic{T}"/> :
        /// ce dernier est déclaré <c>private</c> dans le socle et n'est donc pas accessible depuis
        /// une classe dérivée. Le paramètre reçu au constructeur est à la fois transmis à
        /// <c>base</c>, pour l'initialisation du socle, et conservé ici, pour l'usage propre de la
        /// présente classe. Le socle n'est pas modifié.
        /// </remarks>
        private readonly IS_ExClassifier _classifier;

        /// <summary>
        /// Repository spécialisé de la vue <see cref="vw_ProductionChassis_Full"/>, délégué de la
        /// lecture projetée propre à la présente classe.
        /// </summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ <c>_repository</c> de
        /// <see cref="QH_Generic{T}"/> : ce dernier est déclaré <c>private</c> dans le socle, typé
        /// <c>IR_Generic&lt;vw_ProductionChassis_Full&gt;</c>, et n'est donc accessible ni depuis
        /// une classe dérivée ni sous le type spécialisé. La même instance est reçue une seule
        /// fois au constructeur : elle est transmise à <c>base</c> pour l'initialisation du socle,
        /// et conservée ici sous son type spécialisé pour l'appel de la lecture projetée. Aucune
        /// seconde injection du contrat générique n'est introduite - elle produirait deux
        /// résolutions distinctes du conteneur pour un même rôle.
        /// </remarks>
        private readonly IR_VwProductionChassisFull _repository;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="QH_VwProductionChassisFull"/> en propageant le
        /// repository spécialisé et le classificateur d'exceptions au constructeur de la classe de
        /// base, et en conservant localement l'un et l'autre.
        /// </summary>
        /// <remarks>
        /// <para>
        /// L'appel à <c>base(repository, classifier)</c> est obligatoire en première instruction
        /// du constructeur. Il garantit l'initialisation correcte des champs hérités du socle et
        /// constitue le point de contrôle effectif de nullité des deux paramètres : les
        /// affectations locales défensives qui suivent ne sont atteintes que si le socle a déjà
        /// validé les deux références.
        /// </para>
        /// <para>
        /// Le contrat spécialisé <see cref="IR_VwProductionChassisFull"/> étend
        /// <c>IR_Generic&lt;vw_ProductionChassis_Full&gt;</c> ; une injection unique satisfait
        /// donc à la fois le besoin du socle et celui de la lecture spécialisée.
        /// </para>
        /// </remarks>
        /// <param name="repository">
        /// Repository spécialisé de la vue, délégué de la lecture projetée et du socle de lecture.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="repository"/> ou <paramref name="classifier"/> est
        /// <see langword="null"/>. Le contrôle effectif est assuré par le constructeur de la
        /// classe de base, appelé avant le corps du présent constructeur.
        /// </exception>
        public QH_VwProductionChassisFull(
            IR_VwProductionChassisFull repository,
            IS_ExClassifier classifier)
            : base(repository, classifier)
        {
            _callee = GetType().Name;

            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des châssis rattachés à une série de production, réduits aux seize champs
        /// utiles au troisième onglet de la Page11.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// réduction de soixante-seize à seize colonnes est appliquée sur la requête et traduite
        /// en clause <c>SELECT</c> côté serveur de base de données par le repository spécialisé
        /// délégué ; elle n'est jamais réalisée en mémoire.
        /// </para>
        /// <para>
        /// Objectif : offrir au service métier consommateur un lot de lignes brut, qu'il lui
        /// appartient de trier et de mettre en forme. Aucun ordonnancement n'est appliqué : les
        /// trois critères de tri du tableau (<c>COIdOrder</c>, <c>COPartialSeriesIndex</c>,
        /// <c>PCOrderPosition</c>) figurent parmi les champs de service projetés et sont mis à la
        /// disposition de l'appelant.
        /// </para>
        /// <para>
        /// La référence produite par le repository est retournée telle quelle : ni tri, ni
        /// filtrage, ni recopie, ni projection complémentaire ne sont appliqués en sortie.
        /// </para>
        /// <para>Tâches / Actions :</para>
        /// <list type="bullet">
        ///   <item><description>
        ///     Valider la précondition structurelle portant sur <paramref name="productionSeriesId"/>,
        ///     à l'intérieur du bloc de capture.
        ///   </description></item>
        ///   <item><description>
        ///     Contrôler le jeton d'annulation immédiatement après la validation.
        ///   </description></item>
        ///   <item><description>
        ///     Déléguer la lecture projetée au repository spécialisé, en lui transmettant la
        ///     CallChain enrichie et le jeton.
        ///   </description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif. Il s'agit d'un identifiant fonctionnel
        /// étranger, hérité de la table d'origine de la série, et non de la clé de la vue : la vue
        /// n'en a pas.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des châssis projetés de la série demandée, dans l'ordre où la source les rend.
        /// Liste vide si la série ne comporte aucun châssis : ce résultat est nominal et ne
        /// constitue pas une erreur. Ne retourne jamais <see langword="null"/>.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="productionSeriesId"/> est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>), ou si une exception non contrôlée est requalifiée par
        /// le classificateur.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<DTO_VwProductionChassisFull_P11>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByProductionSeriesIdForP11AsNoTrackingAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard §4.7 ;
                // R-4.7.25), puis contrôle du jeton, dans l'ordre validation -> ct. L'Ex_Business
                // typée remonte intacte au composant appelant via catch (Ex_Business) { throw; },
                // sans requalification. Le contrôle duplique délibérément celui que porte le
                // repository délégué : le modèle du projet porte la validation aux deux étages, et
                // l'échec au plus près de l'appelant produit une chaîne d'appel plus courte et plus
                // lisible. Le message est repris à l'identique de celui du repository, de sorte que
                // les deux étages soient indiscernables du point de vue du consommateur.
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des châssis de {typeof(vw_ProductionChassis_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Délégation au repository spécialisé (sous-cas (ii) du critère de lecture
                // spécialisée de §4.14.5) : la projection SQL traduite côté base de données est une
                // API EF Core absente d'IR_Generic<T>, et aucune des treize lectures du socle ne
                // rend un type DTO_. Aucun appel EF Core ni AsNoTracking() n'est porté ici : le
                // suffixe figure au nom de la méthode déléguée et l'appel relève du repository
                // (R-4.14.11, R-4.15.12). La délégation étant INTER-CLASSES, la CallChain propagée
                // ne comporte aucun redoublement du segment de composant, à la différence de ce
                // qu'on observe en délégation intra-classe public -> public.
                //
                // Le résultat est retourné SANS TRANSFORMATION AUCUNE : la référence produite par
                // le repository est rendue telle quelle, liste vide comprise.
                return await _repository.GetByProductionSeriesIdForP11AsNoTrackingAsync(
                    callChain,
                    productionSeriesId,
                    ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}