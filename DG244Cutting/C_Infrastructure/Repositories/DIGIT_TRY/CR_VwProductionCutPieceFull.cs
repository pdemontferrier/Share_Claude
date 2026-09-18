using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DG244Cutting.C_Infrastructure.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Repository concret spécialisé dédié à la vue de base de données
    /// <see cref="vw_ProductionCutPiece_Full"/>, dérivant de <see cref="CR_Generic{T}"/> paramétré
    /// pour cette vue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans C_Infrastructure et honore le contrat
    /// <see cref="IR_VwProductionCutPieceFull"/>. Elle est adossée à une vue de base de données
    /// plutôt qu'à une table, à l'instar de <c>CR_VwProductionChassisFull</c> et de
    /// <c>CR_VwProductionBarFull</c>. Elle applique le
    /// Patron 2 « Extension par dérivation » défini en §4.15.2 du 0230 : elle hérite de
    /// <see cref="CR_Generic{T}"/> sans redéfinir aucune des dix-huit méthodes du socle (R-4.15.3
    /// et I-4.15.1 du 0231), et ajoute les quatre lectures projetées propres aux besoins couverts.
    /// </para>
    /// <para>
    /// Objectif : servir, à raison d'une ligne par découpe, les découpes de production à trois
    /// destinations : le cinquième onglet de la Page11, qui présente le détail des découpes d'une
    /// série ; le moteur d'optimisation de la Page20, qui recherche dans une série la prochaine
    /// découpe à traiter puis le vivier des découpes à placer pour un article interne ; l'onglet
    /// de contrôle du plan de coupe de la Page20, qui présente à l'opérateur, avant l'acceptation
    /// d'une barre, les découpes posées sur cette barre dans leur ordre de réalisation. La
    /// découpe est l'unité élémentaire du travail d'atelier et l'objet même de l'application :
    /// chaque châssis d'une commande se décompose en ouvrants, eux-mêmes en pièces de profilé à
    /// découper. Chaque
    /// découpe porte l'identification du profilé dont elle est issue, sa géométrie de coupe, les
    /// dimensions du profilé, sa position sur la barre affectée et quatre indicateurs d'état
    /// jalonnant son parcours. La vue expose deux cent trente-trois colonnes.
    /// </para>
    /// <para>
    /// La lecture Page11 rapatrie vingt champs - les seize champs d'affichage de l'onglet, plus
    /// quatre champs de service non affichés dédiés à l'identification des lignes et à la
    /// vérification de cohérence du lot, et concourant aux critères d'ordonnancement mis à la
    /// disposition de l'appelant. Les deux lectures Page20 rapatrient treize champs - données de
    /// découpe, paramètres de coupe, caractéristiques d'article, identification et affichage. La
    /// lecture du plan de coupe rapatrie les mêmes vingt champs que la lecture Page11.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture Page11 : la lecture n'applique aucun filtrage au-delà de la
    /// série. Chez les découpes, à la différence des barres, le refus n'est pas porté par
    /// l'indicateur de suppression logique mais par la colonne distincte et projetée
    /// <c>PCPIsCutRefused</c> : le refus relève de l'affichage et non de l'exclusion. Une découpe
    /// non encore affectée à une barre - dite au vivier - porte une position et un identifiant de
    /// barre absents ; cet état est nominal. Une série dont l'optimisation n'a pas encore été
    /// lancée, ou qui ne comporte aucune découpe, produit une liste vide, résultat nominal.
    /// </para>
    /// <para>
    /// Portée du résultat - lectures d'optimisation de la Page20 : les deux lectures filtrent sur
    /// le prédicat commun de disponibilité pour l'optimisation, déclaré une seule fois par la
    /// classe, et ordonnent leur résultat côté SQL, le tri faisant partie de leur contrat. Le
    /// refus n'est pas non plus, pour ces lectures, un critère d'exclusion. L'absence de découpe
    /// disponible y est nominale. Ce prédicat reste propre à ces deux lectures : il écarte par
    /// construction les découpes engagées dans une optimisation, que la lecture du plan de coupe
    /// restitue précisément.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture du plan de coupe : la lecture retient les découpes rattachées
    /// à la barre demandée et non supprimées logiquement, sans référence à la série. Les découpes
    /// en rupture de stock et les découpes refusées sont restituées. Le résultat est ordonné côté
    /// SQL dans l'ordre de réalisation - positions renseignées d'abord, par position puis par clé
    /// technique croissantes, découpes sans position en fin de liste. Une barre sans découpe
    /// rattachée produit une liste vide, résultat nominal.
    /// </para>
    /// <para>
    /// Justification du Patron 2 (Cas 3 du critère taxonomique de §4.14.6 du 0230) : les méthodes
    /// <see cref="GetByProductionSeriesIdForP11AsNoTrackingAsync"/>,
    /// <see cref="GetNextReferenceToCutForP20AsNoTrackingAsync"/>,
    /// <see cref="GetOptimizationPoolForP20AsNoTrackingAsync"/> et
    /// <see cref="GetByProductionBarIdForP20AsNoTrackingAsync"/> mobilisent toutes quatre la
    /// projection SQL traduite côté base de données, soit un <c>Select</c> retournant un type
    /// <c>DTO_</c> par expression LINQ-to-Entities. Cette API ne figure pas au contrat
    /// <c>IR_Generic&lt;T&gt;</c> et ne peut pas y figurer : le contrat exposerait alors une
    /// dépendance à EF Core, incompatible avec sa résidence en A_Domain. Servir ces besoins par
    /// les dix-huit méthodes du socle imposerait de matérialiser les deux cent trente-trois
    /// colonnes puis d'en écarter en mémoire deux cent treize pour la lecture Page11 et pour la
    /// lecture du plan de coupe, et deux cent vingt pour les lectures d'optimisation de la Page20,
    /// ce qui ferait perdre la réduction côté base - laquelle est la finalité même de la classe.
    /// La recherche de la prochaine découpe requiert en outre un tri sur deux colonnes, et la
    /// lecture du plan de coupe un tri sur trois clés, ce que le socle n'expose pas.
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
    ///     Implémenter les quatre lectures projetées déclarées par
    ///     <see cref="IR_VwProductionCutPieceFull"/> - lecture de consultation de la Page11,
    ///     recherche de la prochaine découpe, lecture du vivier et lecture du plan de coupe d'une
    ///     barre pour la Page20 -, en appliquant
    ///     la sélection, le tri et la réduction de colonnes sur la requête et non après
    ///     matérialisation.
    ///   </description></item>
    ///   <item><description>
    ///     Porter, en un point unique, le prédicat de disponibilité pour l'optimisation commun aux
    ///     deux lectures d'optimisation de la Page20, afin que celles-ci appliquent strictement le
    ///     même critère.
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
    ///     Page11 n'applique aucun ordonnancement ; les lectures Page20 ordonnent côté SQL, l'ordre
    ///     faisant partie de leur contrat.
    ///   </description></item>
    ///   <item><description>
    ///     Pour la lecture Page11, n'écarte aucun enregistrement de la série et n'applique aucun
    ///     filtrage sur l'indicateur de suppression logique <c>PCPIsDeleted</c>. Aucune lecture ne
    ///     recourt à <c>IgnoreQueryFilters</c> : aucun filtre global n'est configuré sur le
    ///     contexte de données. Le refus d'une découpe est porté par la colonne distincte
    ///     <c>PCPIsCutRefused</c>, projetée au même titre que les autres champs d'affichage de la
    ///     Page11 ; il n'est pas non plus un critère d'exclusion pour les lectures Page20.
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
    /// <see cref="IR_VwProductionCutPieceFull"/>, qui constitue l'avertissement opposable au
    /// consommateur.
    /// </para>
    /// </remarks>
    public class CR_VwProductionCutPieceFull : CR_Generic<vw_ProductionCutPiece_Full>, IR_VwProductionCutPieceFull
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

        /// <summary>
        /// Prédicat de disponibilité pour l'optimisation, commun aux deux lectures destinées au
        /// moteur d'optimisation de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée (<c>PCPIsCut</c>), n'est
        /// engagée dans aucune optimisation, ni définitive (<c>PCPIsOptimized</c>) ni provisoire
        /// (<c>PCPIsOptimizedTemp</c>), n'est pas supprimée (<c>PCPIsDeleted</c>) et n'est pas
        /// bloquée par une rupture de stock de sa barre (<c>PCPIsBarOutOfStock</c>). Une découpe en
        /// rupture reste à réaliser, mais demeure hors du vivier tant que la matière n'est pas
        /// libérée.
        /// </para>
        /// <para>
        /// Le prédicat ne porte ni la restriction à la série ni la restriction à l'article
        /// interne, appliquées par chaque méthode consommatrice. Il n'inclut pas davantage
        /// <c>PCPIsCutRefused</c> : le refus d'une découpe n'est pas un critère d'exclusion, une
        /// découpe refusée et non coupée restant à réaliser.
        /// </para>
        /// <para>
        /// Sa déclaration sous forme d'arbre d'expression, et non de délégué, est requise pour
        /// qu'EF Core le traduise en clause <c>WHERE</c> côté serveur de base de données. Sa
        /// déclaration unique garantit que les deux lectures appliquent strictement le même
        /// critère. Le champ est statique : le prédicat ne dépend d'aucun état d'instance.
        /// </para>
        /// </remarks>
        private static readonly Expression<Func<vw_ProductionCutPiece_Full, bool>> _isAvailableForOptimization =
            v => !v.PCPIsCut
                && !v.PCPIsOptimized
                && !v.PCPIsOptimizedTemp
                && !v.PCPIsDeleted
                && !v.PCPIsBarOutOfStock;

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
        /// Initialise une instance de <see cref="CR_VwProductionCutPieceFull"/> en propageant le
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
        public CR_VwProductionCutPieceFull(DbContext context, IS_ExClassifier classifier)
            : base(context, classifier)
        {
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des découpes rattachées à une série de production, réduites aux vingt
        /// champs utiles au cinquième onglet de la Page11, la réduction étant appliquée sur la
        /// requête et traduite en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La projection est appliquée sur la requête et non après matérialisation : seules vingt
        /// des deux cent trente-trois colonnes de la vue transitent depuis le serveur de base de
        /// données. C'est la raison d'être de la méthode et la justification du Patron 2. Le
        /// rapport de réduction y est le plus marqué du périmètre de la Page11, et le nombre de
        /// lignes le plus élevé, chaque châssis d'une série produisant plusieurs découpes.
        /// </para>
        /// <para>
        /// Aucun ordonnancement n'est appliqué. Les quatre critères de tri du tableau
        /// (<c>ACMDescription</c>, <c>ARSortOrder</c>, <c>PCPIdProductionBar</c>,
        /// <c>PCPCutPositionInBar</c>) figurent tous parmi les champs projetés - les premier et
        /// quatrième au titre des champs d'affichage, les deuxième et troisième au titre des
        /// champs de service - et sont mis à la disposition de l'appelant, qui trie en mémoire
        /// après extraction.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique
        /// <c>PCPIsDeleted</c>, et aucun recours n'est fait à <c>IgnoreQueryFilters</c> : aucun
        /// filtre global n'est configuré sur le contexte de données. Chez les découpes, à la
        /// différence du repository des barres, le refus n'est pas porté par l'indicateur de
        /// suppression logique mais par la colonne distincte <c>PCPIsCutRefused</c>, qui figure
        /// parmi les champs projetés.
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
        /// Liste des découpes projetées de la série demandée, jamais <see langword="null"/>, dans
        /// un ordre indéterminé. Liste vide si la série ne comporte aucune découpe - cas d'une
        /// série dont l'optimisation n'a pas encore été lancée, ou d'une série sans découpe : ce
        /// résultat est nominal et ne constitue pas une erreur.
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
        public async Task<List<DTO_VwProductionCutPieceFull_P11>> GetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByProductionSeriesIdForP11AsNoTrackingAsync)}";

            try
            {
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des découpes de {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à vingt colonnes sur deux cent
                // trente-trois. Cette API n'est pas exposée par IR_Generic<T> et ne peut pas
                // l'être (dépendance EF Core interdite en A_Domain). C'est la justification
                // doctrinale du Patron 2 selon §4.14.6 du 0230 (Cas 3).
                // Aucun filtrage sur PCPIsDeleted et aucun IgnoreQueryFilters : chez les découpes,
                // le refus est porté par la colonne distincte PCPIsCutRefused, qui est projetée.
                return await _context.Set<vw_ProductionCutPiece_Full>()
                    .AsNoTracking()
                    .Where(v => v.PSId == productionSeriesId)
                    .Select(v => new DTO_VwProductionCutPieceFull_P11
                    {
                        // Seize champs d'affichage, dans l'ordre des colonnes du tableau.
                        PCPBarReference = v.PCPBarReference,
                        PCPProfileName = v.PCPProfileName,
                        PCPBarColorCodeInOut = v.PCPBarColorCodeInOut,
                        ACMDescription = v.ACMDescription,
                        PCPCutInclinationLeft = v.PCPCutInclinationLeft,
                        PCPCutPivotLeft = v.PCPCutPivotLeft,
                        PCPCutDimension = v.PCPCutDimension,
                        PCPCutPivotRight = v.PCPCutPivotRight,
                        PCPCutInclinationRight = v.PCPCutInclinationRight,
                        PCPProfileHeight = v.PCPProfileHeight,
                        PCPProfileWidth = v.PCPProfileWidth,
                        PCPCutPositionInBar = v.PCPCutPositionInBar,
                        PCPIsBarSupplied = v.PCPIsBarSupplied,
                        PCPIsBarOutOfStock = v.PCPIsBarOutOfStock,
                        PCPIsCut = v.PCPIsCut,
                        PCPIsCutRefused = v.PCPIsCutRefused,

                        // Quatre champs de service non affichés : cohérence du lot reçu,
                        // identification des lignes, et deux des quatre critères d'ordonnancement
                        // laissé à la charge de l'appelant.
                        PSId = v.PSId,
                        PCPId = v.PCPId,
                        ARSortOrder = v.ARSortOrder,
                        PCPIdProductionBar = v.PCPIdProductionBar
                    })
                    .ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Désigne la prochaine découpe à réaliser dans une série de production, tous articles
        /// confondus, réduite aux treize champs utiles au moteur d'optimisation de la Page20, la
        /// sélection, le tri et la réduction étant traduits en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le filtrage applique la restriction à la série puis le prédicat commun
        /// <see cref="_isAvailableForOptimization"/> : sont écartées les découpes coupées,
        /// engagées dans une optimisation provisoire ou définitive, supprimées ou bloquées par une
        /// rupture de stock. Le refus d'une découpe n'est pas un critère d'exclusion. Aucun recours
        /// n'est fait à <c>IgnoreQueryFilters</c> : aucun filtre global n'est configuré sur le
        /// contexte de données.
        /// </para>
        /// <para>
        /// Le tri par <c>PCPReferenceColor</c> croissant, puis par <c>PCPId</c> croissant, est
        /// appliqué sur les colonnes de la vue avant la projection, et la matérialisation se
        /// limite au premier enregistrement. Le tri est constitutif du contrat et non décoratif :
        /// il fixe l'ordre dans lequel l'atelier traite les références. Le départage par
        /// <c>PCPId</c> garantit qu'une même série présente toujours la même référence en premier.
        /// </para>
        /// <para>
        /// L'article interne de la découpe retournée est renseigné, en vertu d'un invariant de
        /// données garanti par l'import des séries ; la requête ne le contrôle pas.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à treize colonnes sur deux cent
        /// trente-trois ; le tri porte en outre sur deux colonnes, ce que le socle n'expose pas.
        /// <c>GetFirstOrDefaultAsNoTrackingAsync</c> avec prédicat ne trie pas et matérialise
        /// l'entité complète ; <c>GetPagedAsNoTrackingAsync</c>, appelée pour une fenêtre d'un
        /// seul enregistrement, ne trie que sur une colonne et matérialise elle aussi l'entité
        /// complète.
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
        /// La prochaine découpe disponible de la série, projetée, ou <see langword="null"/> si la
        /// série ne compte plus aucune découpe optimisable. Ce retour absent est nominal et ne
        /// constitue pas une erreur.
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
        public async Task<DTO_VwProductionCutPieceFull_P20?> GetNextReferenceToCutForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetNextReferenceToCutForP20AsNoTrackingAsync)}";

            try
            {
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la recherche de la prochaine découpe à réaliser dans {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à treize colonnes sur deux cent
                // trente-trois. Le tri sur deux colonnes, appliqué avant la projection, et la
                // réduction au premier enregistrement sont traduits en ORDER BY et TOP(1). Ces
                // API ne sont pas exposées par IR_Generic<T> (Cas 3 de §4.14.6 du 0230).
                return await _context.Set<vw_ProductionCutPiece_Full>()
                    .AsNoTracking()
                    .Where(v => v.PSId == productionSeriesId)
                    .Where(_isAvailableForOptimization)
                    .OrderBy(v => v.PCPReferenceColor)
                    .ThenBy(v => v.PCPId)
                    .Select(v => new DTO_VwProductionCutPieceFull_P20
                    {
                        // Données de découpe.
                        PCPId = v.PCPId,
                        PCPCutDimension = v.PCPCutDimension,

                        // Paramètres de coupe.
                        PCPSawCutLength = v.PCPSawCutLength,
                        PCPFinishingCutLength = v.PCPFinishingCutLength,

                        // Caractéristiques d'article.
                        PCPIdArticleInternal = v.PCPIdArticleInternal,
                        AIStandardBarLengthMm = v.AIStandardBarLengthMm,
                        ARMinScrapLength = v.ARMinScrapLength,
                        AIManageScraps = v.AIManageScraps,
                        ARSortOrder = v.ARSortOrder,

                        // Identification et affichage.
                        PCPReferenceColor = v.PCPReferenceColor,
                        PCPBarReference = v.PCPBarReference,
                        PCPBarColorCodeInOut = v.PCPBarColorCodeInOut,
                        PCPProfileName = v.PCPProfileName
                    })
                    .FirstOrDefaultAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend l'ensemble des découpes restant à réaliser pour un article interne d'une série de
        /// production, réduites aux treize champs utiles au moteur d'optimisation de la Page20, la
        /// sélection, le tri et la réduction étant traduits en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le résultat constitue le vivier d'optimisation : la matière sur laquelle le moteur
        /// calcule le remplissage d'une barre de l'article interne demandé. Toutes les découpes
        /// disponibles de la série pour cet article sont rendues, sans limite de nombre.
        /// </para>
        /// <para>
        /// Le filtrage applique la restriction à la série puis le prédicat commun
        /// <see cref="_isAvailableForOptimization"/> : sont écartées les découpes coupées,
        /// engagées dans une optimisation provisoire ou définitive, supprimées ou bloquées par une
        /// rupture de stock. Le refus d'une découpe n'est pas un critère d'exclusion. Aucun recours
        /// n'est fait à <c>IgnoreQueryFilters</c> : aucun filtre global n'est configuré sur le
        /// contexte de données.
        /// La restriction à l'article interne est appliquée en dernier.
        /// </para>
        /// <para>
        /// Le résultat est ordonné par <c>PCPId</c> croissant, tri appliqué sur la colonne de la
        /// vue avant la projection. Cet ordre porte le déterminisme du moteur d'optimisation : il
        /// départage les combinaisons de remplissage équivalentes, de sorte qu'une même situation
        /// d'atelier conduit toujours à la même barre proposée.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à treize colonnes sur deux cent
        /// trente-trois. <c>GetFilteredAsNoTrackingAsync</c> servirait la sélection, mais ne trie
        /// pas et matérialise l'entité complète.
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
        /// <param name="idArticleInternal">
        /// Identifiant de l'article interne dont le vivier est demandé, correspondant à la
        /// colonne <c>PCPIdArticleInternal</c> de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes disponibles de l'article interne dans la série, projetées et
        /// ordonnées, jamais <see langword="null"/>. Une liste vide est un résultat nominal et ne
        /// constitue pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée, dans cet ordre de contrôle, si l'identifiant de série fourni est inférieur ou
        /// égal à zéro, puis si l'identifiant d'article interne fourni est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c> dans les deux cas).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<DTO_VwProductionCutPieceFull_P20>> GetOptimizationPoolForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            int idArticleInternal,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetOptimizationPoolForP20AsNoTrackingAsync)}";

            try
            {
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la lecture du vivier d'optimisation de {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                if (idArticleInternal <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'article interne fourni pour la lecture du vivier d'optimisation de {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{idArticleInternal}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à treize colonnes sur deux cent
                // trente-trois. Cette API n'est pas exposée par IR_Generic<T> (Cas 3 de §4.14.6
                // du 0230). Le tri par PCPId, appliqué avant la projection, est traduit en
                // ORDER BY et porte le déterminisme du moteur d'optimisation.
                return await _context.Set<vw_ProductionCutPiece_Full>()
                    .AsNoTracking()
                    .Where(v => v.PSId == productionSeriesId)
                    .Where(_isAvailableForOptimization)
                    .Where(v => v.PCPIdArticleInternal == idArticleInternal)
                    .OrderBy(v => v.PCPId)
                    .Select(v => new DTO_VwProductionCutPieceFull_P20
                    {
                        // Données de découpe.
                        PCPId = v.PCPId,
                        PCPCutDimension = v.PCPCutDimension,

                        // Paramètres de coupe.
                        PCPSawCutLength = v.PCPSawCutLength,
                        PCPFinishingCutLength = v.PCPFinishingCutLength,

                        // Caractéristiques d'article.
                        PCPIdArticleInternal = v.PCPIdArticleInternal,
                        AIStandardBarLengthMm = v.AIStandardBarLengthMm,
                        ARMinScrapLength = v.ARMinScrapLength,
                        AIManageScraps = v.AIManageScraps,
                        ARSortOrder = v.ARSortOrder,

                        // Identification et affichage.
                        PCPReferenceColor = v.PCPReferenceColor,
                        PCPBarReference = v.PCPBarReference,
                        PCPBarColorCodeInOut = v.PCPBarColorCodeInOut,
                        PCPProfileName = v.PCPProfileName
                    })
                    .ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend le plan de coupe posé sur une barre de production, c'est-à-dire les découpes qui
        /// lui sont rattachées et non supprimées, dans leur ordre de réalisation, réduites aux
        /// vingt champs utiles à l'onglet de contrôle du plan de coupe de la Page20, la sélection,
        /// le tri et la réduction étant traduits en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le filtrage restreint la vue aux découpes dont <c>PCPIdProductionBar</c> est égal à
        /// l'identifiant demandé et dont <c>PCPIsDeleted</c> est faux. Le prédicat est écrit en
        /// ligne : le prédicat commun <see cref="_isAvailableForOptimization"/> ne convient pas,
        /// car il écarte les découpes engagées dans une optimisation provisoire ou définitive,
        /// c'est-à-dire précisément les découpes rattachées à une barre. Les découpes en rupture
        /// de stock (<c>PCPIsBarOutOfStock</c>) et les découpes refusées (<c>PCPIsCutRefused</c>)
        /// ne sont pas écartées. Aucun recours n'est fait à <c>IgnoreQueryFilters</c> : aucun
        /// filtre global n'est configuré sur le contexte de données.
        /// </para>
        /// <para>
        /// Le tri est appliqué sur les colonnes de la vue avant la projection, selon trois clés :
        /// l'absence de position de coupe, puis <c>PCPCutPositionInBar</c> croissant, puis
        /// <c>PCPId</c> croissant. La première clé est écrite sous la forme d'une comparaison
        /// d'égalité à <see langword="null"/>, traduite par EF Core en expression conditionnelle
        /// SQL ; le tri croissant sur cette valeur booléenne place les positions renseignées avant
        /// les positions absentes. Le filtrage par motif (<c>is null</c>) n'est pas admis dans un
        /// arbre d'expression. Une découpe rattachée sans position est ainsi rendue en fin de
        /// liste, où l'anomalie reste visible sans perturber l'ordre nominal.
        /// </para>
        /// <para>
        /// La projection recopie terme à terme, dans le même ordre et selon les mêmes deux groupes,
        /// les vingt champs de <see cref="GetByProductionSeriesIdForP11AsNoTrackingAsync"/>. Elle
        /// emploie <see cref="DTO_VwProductionCutPieceFull_P11"/>, type conçu pour la consultation
        /// de la Page11 : cet emprunt est un écart assumé et provisoire, un type d'affichage
        /// propre à l'onglet de contrôle étant appelé à le remplacer, la substitution ne touchant
        /// que la clause de projection et la signature. La projection n'est pas mutualisée avec
        /// celle de la lecture Page11.
        /// </para>
        /// <para>
        /// Justification du Cas 3 (§4.14.6 du 0230) : la projection <c>Select</c> vers un type
        /// <c>DTO_</c> est traduite côté serveur et réduit le flux à vingt colonnes sur deux cent
        /// trente-trois ; le tri porte en outre sur trois clés, ce que le socle n'expose pas.
        /// <c>GetFilteredAsNoTrackingAsync</c> servirait la sélection, mais ne trie pas et
        /// matérialise l'entité complète.
        /// </para>
        /// <para>
        /// L'appel à <c>AsNoTracking</c> est sans effet sur un type déclaré sans clé et sur une
        /// requête projetée vers un type non entité. Il est néanmoins conservé : il documente
        /// l'intention de lecture pure et aligne le corps sur les autres lectures de la classe.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production dont le plan de coupe est demandé, rapproché de
        /// la colonne <c>PCPIdProductionBar</c> de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes projetées du plan de coupe de la barre, ordonnées dans l'ordre de
        /// réalisation, jamais <see langword="null"/>. Une liste vide est un résultat nominal et
        /// ne constitue pas une erreur ; son interprétation revient à l'appelant.
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
        public async Task<List<DTO_VwProductionCutPieceFull_P11>> GetByProductionBarIdForP20AsNoTrackingAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByProductionBarIdForP20AsNoTrackingAsync)}";

            try
            {
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre de production fourni pour la lecture du plan de coupe de {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{idProductionBar}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à vingt colonnes sur deux cent
                // trente-trois. Le tri sur trois clés, appliqué avant la projection, est traduit
                // en ORDER BY. Ces API ne sont pas exposées par IR_Generic<T> (Cas 3 de §4.14.6
                // du 0230).
                // Prédicat écrit en ligne : _isAvailableForOptimization écarte les découpes
                // rattachées à une barre et ne convient pas. Les découpes en rupture de stock et
                // les découpes refusées sont conservées.
                // Première clé de tri écrite « == null » : le motif « is » est interdit dans un
                // arbre d'expression (CS8122) ; la comparaison est traduite en CASE WHEN ... IS
                // NULL et place les positions renseignées en premier.
                return await _context.Set<vw_ProductionCutPiece_Full>()
                    .AsNoTracking()
                    .Where(v => v.PCPIdProductionBar == idProductionBar && !v.PCPIsDeleted)
                    .OrderBy(v => v.PCPCutPositionInBar == null)
                    .ThenBy(v => v.PCPCutPositionInBar)
                    .ThenBy(v => v.PCPId)
                    .Select(v => new DTO_VwProductionCutPieceFull_P11
                    {
                        // Seize champs d'affichage, dans l'ordre de la projection de la lecture
                        // Page11.
                        PCPBarReference = v.PCPBarReference,
                        PCPProfileName = v.PCPProfileName,
                        PCPBarColorCodeInOut = v.PCPBarColorCodeInOut,
                        ACMDescription = v.ACMDescription,
                        PCPCutInclinationLeft = v.PCPCutInclinationLeft,
                        PCPCutPivotLeft = v.PCPCutPivotLeft,
                        PCPCutDimension = v.PCPCutDimension,
                        PCPCutPivotRight = v.PCPCutPivotRight,
                        PCPCutInclinationRight = v.PCPCutInclinationRight,
                        PCPProfileHeight = v.PCPProfileHeight,
                        PCPProfileWidth = v.PCPProfileWidth,
                        PCPCutPositionInBar = v.PCPCutPositionInBar,
                        PCPIsBarSupplied = v.PCPIsBarSupplied,
                        PCPIsBarOutOfStock = v.PCPIsBarOutOfStock,
                        PCPIsCut = v.PCPIsCut,
                        PCPIsCutRefused = v.PCPIsCutRefused,

                        // Quatre champs de service non affichés : série d'appartenance,
                        // identification des lignes, ordre d'affichage de la référence article
                        // et rattachement à la barre, ce dernier support de la vérification de
                        // cohérence du lot reçu.
                        PSId = v.PSId,
                        PCPId = v.PCPId,
                        ARSortOrder = v.ARSortOrder,
                        PCPIdProductionBar = v.PCPIdProductionBar
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