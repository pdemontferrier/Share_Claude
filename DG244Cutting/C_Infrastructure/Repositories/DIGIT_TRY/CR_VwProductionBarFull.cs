using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace DG244Cutting.C_Infrastructure.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Repository concret spécialisé dédié à la vue de base de données
    /// <see cref="vw_ProductionBar_Full"/>, dérivant de <see cref="CR_Generic{T}"/> paramétré
    /// pour cette vue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans C_Infrastructure et honore le contrat
    /// <see cref="IR_VwProductionBarFull"/>. Elle est adossée à une vue de base de données plutôt
    /// qu'à une table, à l'instar de <c>CR_VwProductionChassisFull</c> et de
    /// <c>CR_VwProductionCutPieceFull</c>. Elle applique le Patron 2 « Extension par dérivation »
    /// défini en §4.15.2 du 0230 : elle hérite de <see cref="CR_Generic{T}"/> sans redéfinir
    /// aucune des dix-huit méthodes du socle (R-4.15.3 et I-4.15.1 du 0231), et ajoute les trois
    /// lectures projetées propres aux besoins couverts.
    /// </para>
    /// <para>
    /// Objectif : servir, à raison d'une ligne par barre, les barres retenues par l'optimisation à
    /// deux destinations : l'onglet de consultation des barres de la Page11, qui présente la
    /// composition en barres d'une série de production, et l'écran de validation des barres de la
    /// Page20, qui présente à l'opérateur la barre désignée puis les barres de la série mises en
    /// attente pour rupture de stock. Ces barres proviennent soit du stock de chutes issues de
    /// séries antérieures, soit du stock de barres neuves, et portent cinq indicateurs d'état
    /// jalonnant leur parcours. La vue expose quatre-vingt-deux colonnes. Les trois lectures
    /// rapatrient les mêmes dix-huit champs - les seize champs d'affichage du tableau de
    /// consultation, plus deux champs de service non affichés dédiés à l'identification des lignes
    /// et à la vérification de cohérence du lot, et concourant aux critères d'ordonnancement.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture Page11 : la lecture ne filtre pas les enregistrements marqués
    /// comme logiquement supprimés, à la différence des autres lectures du projet. Le refus d'une
    /// barre par l'opérateur marque l'enregistrement de cette façon, et l'écran doit afficher ces
    /// barres refusées avec leur motif : elles font partie intégrante du résultat attendu. Une
    /// série dont l'optimisation n'a pas encore été lancée ne porte aucune barre ; une liste vide
    /// est alors un résultat nominal.
    /// </para>
    /// <para>
    /// Portée du résultat - lectures Page20 : la lecture de la barre présentée ne filtre aucun
    /// indicateur d'état et rend au plus une barre ; l'absence de ligne produit une valeur absente,
    /// sans exception, dont le traitement appartient à l'appelant. La lecture des barres en rupture
    /// retient les barres de la série marquées en rupture de stock et exclut les barres refusées,
    /// par alignement strict sur le critère du service qui calcule l'indicateur de rupture de la
    /// série ; elle ordonne son résultat côté SQL, le tri faisant partie de son contrat, et une
    /// liste vide y est un résultat nominal.
    /// </para>
    /// <para>
    /// Justification du Patron 2 (Cas 3 du critère taxonomique de §4.14.6 du 0230) : les méthodes
    /// <see cref="GetByProductionSeriesIdAsNoTrackingAsync"/>,
    /// <see cref="GetByProductionBarIdAsNoTrackingAsync"/> et
    /// <see cref="GetOutOfStockByProductionSeriesIdAsNoTrackingAsync"/> mobilisent toutes
    /// trois la projection SQL traduite côté base de données, soit un <c>Select</c> retournant un
    /// type <c>DTO_</c> par expression LINQ-to-Entities. Cette API ne figure pas au contrat
    /// <c>IR_Generic&lt;T&gt;</c> et ne peut pas y figurer : le contrat exposerait alors une
    /// dépendance à EF Core, incompatible avec sa résidence en A_Domain. Servir ces besoins par
    /// les dix-huit méthodes du socle imposerait de matérialiser les quatre-vingt-deux colonnes
    /// puis d'en écarter soixante-quatre en mémoire, ce qui ferait perdre la réduction côté base -
    /// laquelle est la finalité même de la classe. La lecture des barres en rupture requiert en
    /// outre un tri sur deux colonnes, que le socle n'expose pas.
    /// </para>
    /// <para>
    /// Modèle transactionnel : la classe reçoit le <see cref="DbContext"/> partagé sous son type
    /// abstrait, en portée Scoped, et le transmet au socle par appel à <c>base</c>. Elle n'ouvre,
    /// ne valide ni n'annule aucune transaction, et n'appelle jamais <c>SaveChangesAsync</c> : la
    /// persistance est portée exclusivement par le UseCase orchestrateur (R-4.14.11, §4.10.4 du
    /// 0230). La question est au demeurant sans objet ici, la vue étant une source de lecture
    /// seule.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Implémenter les trois lectures projetées déclarées par
    ///     <see cref="IR_VwProductionBarFull"/> - lecture de consultation de la Page11, lecture de
    ///     la barre présentée et lecture des barres en rupture de la série pour la Page20 -, en
    ///     appliquant la sélection, le tri et la réduction de colonnes sur la requête et non après
    ///     matérialisation.
    ///   </description></item>
    ///   <item><description>
    ///     Respecter le pattern d'enrichissement de CallChain (§4.5 du 0230) et le pattern de
    ///     classification d'exceptions (§4.7.3 du 0230) identiques à ceux des méthodes héritées de
    ///     <see cref="CR_Generic{T}"/>.
    ///   </description></item>
    ///   <item><description>
    ///     Propager le <see cref="CancellationToken"/> à tous les points de coopération EF Core
    ///     (§4.6 du 0230).
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne redéfinit aucune méthode publique héritée de <see cref="CR_Generic{T}"/> : leurs
    ///     implémentations sont finales, aucune n'étant déclarée <c>virtual</c> dans le socle.
    ///   </description></item>
    ///   <item><description>
    ///     Ne crée pas son propre DbContext : elle opère sur celui injecté au constructeur et
    ///     propagé via le champ <c>_context</c> hérité, déclaré <c>protected</c> dans le socle,
    ///     conformément à R-4.15.4 du 0231.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune règle métier, aucun calcul et aucun renommage de champ : chaque
    ///     projection est une recopie terme à terme, types et nullabilité inclus. La lecture
    ///     Page11 n'applique aucun ordonnancement ; la lecture des barres en rupture ordonne côté
    ///     SQL, l'ordre faisant partie de son contrat ; la question est sans objet pour la lecture
    ///     de la barre présentée, qui rend au plus une barre.
    ///   </description></item>
    ///   <item><description>
    ///     Pour la lecture Page11 et pour la lecture de la barre présentée, n'écarte aucun
    ///     enregistrement au motif qu'il serait marqué comme logiquement supprimé. Aucune lecture
    ///     ne recourt à <c>IgnoreQueryFilters</c> : aucun filtre global n'est configuré sur le
    ///     contexte de données, et la vue ne filtre pas l'indicateur de suppression de sa table
    ///     pilote.
    ///   </description></item>
    ///   <item><description>
    ///     Ne journalise pas et ne notifie pas : ces responsabilités appartiennent aux couches
    ///     amont (§3.6, §4.8 du 0230).
    ///   </description></item>
    /// </list>
    /// <para>
    /// Surface héritée sur un type sans clé : la vue étant déclarée <c>HasNoKey</c>, huit des
    /// dix-huit méthodes héritées sont visibles au consommateur mais échouent à l'exécution sur ce
    /// type. Leur inventaire nominatif et leur cause sont portés par le commentaire de
    /// <see cref="IR_VwProductionBarFull"/>, qui constitue l'avertissement opposable au
    /// consommateur.
    /// </para>
    /// </remarks>
    public class CR_VwProductionBarFull : CR_Generic<vw_ProductionBar_Full>, IR_VwProductionBarFull
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.
        /// </summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ homonyme de <see cref="CR_Generic{T}"/> :
        /// ce dernier est déclaré <c>private</c> dans le socle et n'est donc pas accessible depuis
        /// une classe dérivée, <c>CR_Generic&lt;T&gt;</c> n'exposant que <c>_context</c> à sa
        /// surface protégée (R-4.15.4 du 0231, §4.15.2 du 0230). Sa déclaration en région
        /// <c>=== Propriétés privées ===</c> et son initialisation en constructeur sont
        /// obligatoires pour toute classe participant à la CallChain (R-4.4.4, R-4.4.5, R-4.5.5 et
        /// R-4.5.6 du 0231). §4.15.4 du 0230 tire explicitement la même conséquence pour la
        /// famille sœur des Query Handlers : le dérivé re-déclare <c>_callee</c> et ré-injecte
        /// <c>_classifier</c> pour son propre bloc de capture.
        /// </remarks>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>), conservé localement
        /// pour l'usage de la cascade de rattrapage des méthodes spécialisées.
        /// </summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ homonyme de <see cref="CR_Generic{T}"/> :
        /// ce dernier est déclaré <c>private</c> dans le socle et n'est donc pas accessible depuis
        /// une classe dérivée. <c>CR_Generic&lt;T&gt;</c> n'expose qu'un seul membre
        /// <c>protected</c> à ses dérivés, le champ <c>_context</c> ; un Repository spécialisé qui
        /// ajoute ses propres méthodes consomme <see cref="IS_ExClassifier"/> par injection dans
        /// son propre constructeur (R-4.15.4 du 0231, §4.15.2 du 0230, sous-bloc « Surface
        /// protégée pour la dérivation »). Le paramètre reçu au constructeur est à la fois
        /// transmis à <c>base</c>, pour l'initialisation du socle, et conservé ici, pour l'usage
        /// propre de la présente classe. Le socle n'est pas modifié : il relève du régime de
        /// stabilité de §3.14.3 du 0230 et de la doctrine du patrimoine fermé de §4.15.1.
        /// </remarks>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="CR_VwProductionBarFull"/> en propageant le
        /// DbContext partagé et le classificateur d'exceptions au constructeur de la classe de
        /// base, en conservant localement le classificateur, puis en résolvant le nom du composant
        /// utilisé dans la CallChain.
        /// </summary>
        /// <remarks>
        /// <para>
        /// L'appel à <c>base(context, classifier)</c> est obligatoire en première instruction du
        /// constructeur, conformément à §3.14.4 du 0230 et à R-3.14.7 du 0231. Il garantit
        /// l'initialisation correcte des champs hérités du socle.
        /// </para>
        /// <para>
        /// Le contexte est reçu sous son type abstrait <see cref="DbContext"/> et jamais sous son
        /// type concret. La racine de composition résout déjà ce type abstrait vers
        /// <c>DigitTryDbContext</c> en portée Scoped ; aucun enregistrement supplémentaire n'est
        /// requis. Le contexte n'est pas stocké localement : le champ <c>_context</c> du socle est
        /// <c>protected</c> et directement utilisable.
        /// </para>
        /// <para>
        /// L'ordre des opérations du corps suit R-4.4.7 du 0231 : affectation des dépendances
        /// injectées, puis initialisation du champ <c>_callee</c> par <c>GetType().Name</c>
        /// (R-4.5.6 du 0231). La résolution au type runtime rend le segment de CallChain exact en
        /// cas de renommage de la classe comme en cas de dérivation ultérieure, et exclut tout
        /// codage en dur du nom de classe (§4.5.2 du 0230).
        /// </para>
        /// </remarks>
        /// <param name="context">
        /// Instance du DbContext EF Core partagé pour la durée du UseCase en cours d'exécution.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est <see langword="null"/>. Le contrôle de
        /// <paramref name="context"/> est assuré par le constructeur de la classe de base.
        /// </exception>
        public CR_VwProductionBarFull(DbContext context, IS_ExClassifier classifier)
            : base(context, classifier)
        {
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des barres retenues par l'optimisation pour une série de production,
        /// réduites aux dix-huit champs utiles au quatrième onglet de la Page11, la réduction
        /// étant appliquée sur la requête et traduite en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La projection est appliquée sur la requête et non après matérialisation : seules
        /// dix-huit des quatre-vingt-deux colonnes de la vue transitent depuis le serveur de base
        /// de données. C'est la raison d'être de la méthode et la justification du Patron 2.
        /// </para>
        /// <para>
        /// Aucun ordonnancement n'est appliqué. Les trois critères de tri du tableau
        /// (<c>ARSortOrder</c>, <c>PBIsNewBar</c>, <c>PBId</c>) figurent parmi les champs projetés
        /// - les deux premiers au titre des champs d'affichage, le troisième au titre des champs
        /// de service - et sont mis à la disposition de l'appelant, qui trie en mémoire après
        /// extraction.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique, et aucun recours
        /// n'est fait à <c>IgnoreQueryFilters</c> : aucun filtre global n'est configuré sur le
        /// contexte de données, et la vue ne filtre pas l'indicateur de suppression de sa table
        /// pilote. Les barres refusées, marquées comme logiquement supprimées, font partie du
        /// résultat attendu et sont rendues avec leur motif de refus.
        /// </para>
        /// <para>
        /// L'appel à <c>AsNoTracking</c> est sans effet sur un type déclaré sans clé, qu'EF Core
        /// ne trace jamais, et sur une requête projetée vers un type non entité. Il est néanmoins
        /// conservé : il documente l'intention de lecture pure et aligne le corps sur l'étalon
        /// <c>GetFilteredAsNoTrackingAsync</c> du socle.
        /// </para>
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
        /// Liste des barres projetées de la série demandée, jamais <see langword="null"/>, dans un
        /// ordre indéterminé. Liste vide si la série ne comporte aucune barre - cas d'une série
        /// dont l'optimisation n'a pas encore été lancée : ce résultat est nominal et ne constitue
        /// pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de série fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<DTO_VwProductionBarFull>> GetByProductionSeriesIdAsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByProductionSeriesIdAsNoTrackingAsync)}";

            try
            {
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des barres de {typeof(vw_ProductionBar_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à dix-huit colonnes sur
                // quatre-vingt-deux. Cette API n'est pas exposée par IR_Generic<T> et ne peut pas
                // l'être (dépendance EF Core interdite en A_Domain). C'est la justification
                // doctrinale du Patron 2 selon §4.14.6 du 0230 (Cas 3).
                // Aucun filtrage sur PBIsDeleted et aucun IgnoreQueryFilters : les barres refusées
                // font partie du résultat attendu.
                return await _context.Set<vw_ProductionBar_Full>()
                    .AsNoTracking()
                    .Where(v => v.PSId == productionSeriesId)
                    .Select(v => new DTO_VwProductionBarFull
                    {
                        // Seize champs d'affichage, dans l'ordre des colonnes du tableau.
                        ARReference = v.ARReference,
                        ARDesignation = v.ARDesignation,
                        AIIdColorRalFinish = v.AIIdColorRalFinish,
                        ARFamilyCategoryPrincipal = v.ARFamilyCategoryPrincipal,
                        PBBarLength = v.PBBarLength,
                        ARBarHeightMm = v.ARBarHeightMm,
                        ARBarWidthMm = v.ARBarWidthMm,
                        ARSortOrder = v.ARSortOrder,
                        PBCutPieceCount = v.PBCutPieceCount,
                        PBResidueLength = v.PBResidueLength,
                        PBIsNewBar = v.PBIsNewBar,
                        PBIsValidated = v.PBIsValidated,
                        PBIsUsed = v.PBIsUsed,
                        PBIsOutOfStock = v.PBIsOutOfStock,
                        PBIsDeleted = v.PBIsDeleted,
                        PBRejectionReason = v.PBRejectionReason,

                        // Deux champs de service non affichés : cohérence du lot reçu et
                        // identification des lignes, ce dernier concourant par ailleurs à
                        // l'ordonnancement laissé à la charge de l'appelant.
                        PSId = v.PSId,
                        PBId = v.PBId
                    })
                    .ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend la barre désignée par son identifiant, telle que l'écran de validation des barres
        /// de la Page20 la présente à l'opérateur, réduite aux dix-huit champs du type de
        /// projection, la sélection et la réduction étant traduites en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La lecture sert l'onglet de détail de la barre présentée, où l'opérateur vérifie la
        /// matière avant d'accepter la barre. Le type rendu est
        /// <see cref="DTO_VwProductionBarFull"/>.
        /// </para>
        /// <para>
        /// Le filtrage se limite à l'identifiant de barre : aucun filtrage n'est appliqué sur les
        /// indicateurs d'état, et la barre désignée est rendue qu'elle soit validée, utilisée, en
        /// rupture de stock ou refusée. Aucun recours n'est fait à <c>IgnoreQueryFilters</c> :
        /// aucun filtre global n'est configuré sur le contexte de données.
        /// </para>
        /// <para>
        /// <c>PBId</c> étant la clé primaire de la table des barres qui pilote la vue, laquelle
        /// porte une ligne par barre, au plus un enregistrement satisfait le filtre. La
        /// matérialisation se limite au premier enregistrement, sans tri.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à dix-huit colonnes sur
        /// quatre-vingt-deux. <c>GetFirstOrDefaultAsNoTrackingAsync</c> avec prédicat servirait la
        /// sélection, mais matérialise l'entité complète ; <c>GetByIdAsNoTrackingAsync</c> est
        /// inopérante sur ce type sans clé.
        /// </para>
        /// <para>
        /// L'appel à <c>AsNoTracking</c> est sans effet sur un type déclaré sans clé, qu'EF Core
        /// ne trace jamais, et sur une requête projetée vers un type non entité. Il est néanmoins
        /// conservé : il documente l'intention de lecture pure et aligne le corps sur les autres
        /// lectures de la classe.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre, correspondant à la colonne <c>PBId</c> de la vue, clé primaire
        /// de la table des barres de production. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// La barre désignée, projetée, ou <see langword="null"/> si aucune ligne de la vue ne
        /// porte cet identifiant. Ce retour absent n'est pas une erreur au niveau du repository et
        /// ne lève aucune exception : la barre ayant été désignée en amont, il traduit une
        /// incohérence dont le traitement appartient à l'appelant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de barre fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<DTO_VwProductionBarFull?> GetByProductionBarIdAsNoTrackingAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByProductionBarIdAsNoTrackingAsync)}";

            try
            {
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre fourni pour la projection de la barre présentée de {typeof(vw_ProductionBar_Full).Name} est invalide : '{idProductionBar}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à dix-huit colonnes sur
                // quatre-vingt-deux ; la réduction au premier enregistrement est traduite en TOP(1).
                // Cette API n'est pas exposée par IR_Generic<T> et ne peut pas l'être (dépendance
                // EF Core interdite en A_Domain). C'est la justification doctrinale du Patron 2
                // selon §4.14.6 du 0230 (Cas 3).
                // Aucun filtrage sur les indicateurs d'état et aucun IgnoreQueryFilters : la barre
                // désignée est rendue quel que soit son état.
                return await _context.Set<vw_ProductionBar_Full>()
                    .AsNoTracking()
                    .Where(v => v.PBId == idProductionBar)
                    .Select(v => new DTO_VwProductionBarFull
                    {
                        // Seize champs d'affichage, dans l'ordre des colonnes du tableau.
                        ARReference = v.ARReference,
                        ARDesignation = v.ARDesignation,
                        AIIdColorRalFinish = v.AIIdColorRalFinish,
                        ARFamilyCategoryPrincipal = v.ARFamilyCategoryPrincipal,
                        PBBarLength = v.PBBarLength,
                        ARBarHeightMm = v.ARBarHeightMm,
                        ARBarWidthMm = v.ARBarWidthMm,
                        ARSortOrder = v.ARSortOrder,
                        PBCutPieceCount = v.PBCutPieceCount,
                        PBResidueLength = v.PBResidueLength,
                        PBIsNewBar = v.PBIsNewBar,
                        PBIsValidated = v.PBIsValidated,
                        PBIsUsed = v.PBIsUsed,
                        PBIsOutOfStock = v.PBIsOutOfStock,
                        PBIsDeleted = v.PBIsDeleted,
                        PBRejectionReason = v.PBRejectionReason,

                        // Deux champs de service non affichés : cohérence de l'enregistrement reçu
                        // et identification de la barre.
                        PSId = v.PSId,
                        PBId = v.PBId
                    })
                    .FirstOrDefaultAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend les barres d'une série de production mises en attente pour rupture de stock et non
        /// refusées, réduites aux dix-huit champs du type de projection, à destination de l'écran
        /// de validation des barres de la Page20, la sélection, le tri et la réduction étant
        /// traduits en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La lecture sert l'onglet des barres en rupture de la série, où l'opérateur voit ce qui
        /// bloque la série et peut libérer une barre dont la matière est revenue. Le type rendu est
        /// <see cref="DTO_VwProductionBarFull"/>.
        /// </para>
        /// <para>
        /// Le filtrage retient les barres de la série marquées en rupture de stock
        /// (<c>PBIsOutOfStock</c>) et non refusées (<c>PBIsDeleted</c>). Ce prédicat est
        /// strictement aligné sur celui du service <c>SR_ProductionSeries_SetBarOutOfStockFlag</c>,
        /// qui calcule l'indicateur de rupture de la série : la liste rendue coïncide exactement
        /// avec les barres qui maintiennent la série en attente. La validation n'est pas un critère
        /// d'exclusion. Aucun recours n'est fait à <c>IgnoreQueryFilters</c> : aucun filtre global
        /// n'est configuré sur le contexte de données.
        /// </para>
        /// <para>
        /// Le tri par <c>ARSortOrder</c> croissant, puis par <c>PBId</c> croissant, est appliqué
        /// sur les colonnes de la vue avant la projection. Il est constitutif du contrat ; le
        /// départage par <c>PBId</c> garantit un ordre stable pour une même situation d'atelier.
        /// Toutes les barres retenues sont rendues, sans limite de nombre.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à dix-huit colonnes sur
        /// quatre-vingt-deux ; le tri porte en outre sur deux colonnes, ce que le socle n'expose
        /// pas. <c>GetFilteredAsNoTrackingAsync</c> servirait la sélection, mais ne trie pas et
        /// matérialise l'entité complète ; <c>GetPagedAsNoTrackingAsync</c> ne trie que sur une
        /// colonne, impose une fenêtre bornée et matérialise elle aussi l'entité complète.
        /// </para>
        /// <para>
        /// L'appel à <c>AsNoTracking</c> est sans effet sur un type déclaré sans clé et sur une
        /// requête projetée vers un type non entité. Il est néanmoins conservé : il documente
        /// l'intention de lecture pure et aligne le corps sur les autres lectures de la classe.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="productionSeriesId">
        /// Identifiant technique de la série de production, correspondant à la colonne <c>PSId</c>
        /// de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des barres en rupture et non refusées de la série, projetées et ordonnées, jamais
        /// <see langword="null"/>. Une liste vide est un résultat nominal et ne constitue pas une
        /// erreur : la série ne compte alors aucune barre en rupture, par exemple après la
        /// libération concurrente de la dernière d'entre elles.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant de série fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<DTO_VwProductionBarFull>> GetOutOfStockByProductionSeriesIdAsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetOutOfStockByProductionSeriesIdAsNoTrackingAsync)}";

            try
            {
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des barres en rupture de stock de {typeof(vw_ProductionBar_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à dix-huit colonnes sur
                // quatre-vingt-deux. Le tri sur deux colonnes, appliqué avant la projection, est
                // traduit en ORDER BY. Ces API ne sont pas exposées par IR_Generic<T> (Cas 3 de
                // §4.14.6 du 0230).
                // Prédicat strictement aligné sur celui de SR_ProductionSeries_SetBarOutOfStockFlag
                // (IsOutOfStock && !IsDeleted) : une barre refusée est exclue, la validation n'est
                // pas un critère d'exclusion.
                return await _context.Set<vw_ProductionBar_Full>()
                    .AsNoTracking()
                    .Where(v => v.PSId == productionSeriesId
                        && v.PBIsOutOfStock
                        && !v.PBIsDeleted)
                    .OrderBy(v => v.ARSortOrder)
                    .ThenBy(v => v.PBId)
                    .Select(v => new DTO_VwProductionBarFull
                    {
                        // Seize champs d'affichage, dans l'ordre des colonnes du tableau.
                        ARReference = v.ARReference,
                        ARDesignation = v.ARDesignation,
                        AIIdColorRalFinish = v.AIIdColorRalFinish,
                        ARFamilyCategoryPrincipal = v.ARFamilyCategoryPrincipal,
                        PBBarLength = v.PBBarLength,
                        ARBarHeightMm = v.ARBarHeightMm,
                        ARBarWidthMm = v.ARBarWidthMm,
                        ARSortOrder = v.ARSortOrder,
                        PBCutPieceCount = v.PBCutPieceCount,
                        PBResidueLength = v.PBResidueLength,
                        PBIsNewBar = v.PBIsNewBar,
                        PBIsValidated = v.PBIsValidated,
                        PBIsUsed = v.PBIsUsed,
                        PBIsOutOfStock = v.PBIsOutOfStock,
                        PBIsDeleted = v.PBIsDeleted,
                        PBRejectionReason = v.PBRejectionReason,

                        // Deux champs de service non affichés : cohérence du lot reçu et
                        // identification des lignes, ce dernier départageant par ailleurs le tri
                        // appliqué côté serveur.
                        PSId = v.PSId,
                        PBId = v.PBId
                    })
                    .ToListAsync(ct);
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