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
    /// <see cref="vw_ProductionChassis_Full"/>, dérivant de <see cref="CR_Generic{T}"/> paramétré
    /// pour cette vue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans C_Infrastructure et honore le contrat
    /// <see cref="IR_VwProductionChassisFull"/>. Elle est le premier repository spécialisé de la
    /// solution adossé à une vue de base de données plutôt qu'à une table. Elle applique le
    /// Patron 2 « Extension par dérivation » défini en §4.15.2 du 0230 : elle hérite de
    /// <see cref="CR_Generic{T}"/> sans redéfinir aucune des dix-huit méthodes du socle (R-4.15.3
    /// et I-4.15.1 du 0231), et ajoute la seule lecture projetée propre au besoin couvert.
    /// </para>
    /// <para>
    /// Objectif : servir au troisième onglet de la Page11 la composition physique d'une série de
    /// production, à raison d'une ligne par châssis. La vue expose soixante-seize colonnes ;
    /// l'écran en affiche onze. La lecture rapatrie seize champs - les onze champs d'affichage,
    /// plus cinq champs de service non affichés dédiés à l'identification des lignes, à la
    /// vérification de cohérence du lot et aux trois critères d'ordonnancement mis à la
    /// disposition de l'appelant.
    /// </para>
    /// <para>
    /// Justification du Patron 2 (Cas 3 du critère taxonomique de §4.14.6 du 0230) : la méthode
    /// <see cref="GetByProductionSeriesIdForP11AsNoTrackingAsync"/> mobilise la projection SQL
    /// traduite côté base de données, soit un <c>Select</c> retournant un type <c>DTO_</c> par
    /// expression LINQ-to-Entities. Cette API ne figure pas au contrat
    /// <c>IR_Generic&lt;T&gt;</c> et ne peut pas y figurer : le contrat exposerait alors une
    /// dépendance à EF Core, incompatible avec sa résidence en A_Domain. Servir le besoin par les
    /// dix-huit méthodes du socle imposerait de matérialiser les soixante-seize colonnes puis
    /// d'en écarter soixante en mémoire, ce qui ferait perdre la réduction côté base - laquelle
    /// est la finalité même de la classe.
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
    ///     Implémenter la lecture projetée déclarée par <see cref="IR_VwProductionChassisFull"/>,
    ///     en appliquant la réduction de colonnes sur la requête et non après matérialisation.
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
    ///     Ne porte aucune règle métier, aucun calcul, aucun ordonnancement et aucun renommage de
    ///     champ : la projection est une recopie terme à terme, types et nullabilité inclus.
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
    /// <see cref="IR_VwProductionChassisFull"/>, qui constitue l'avertissement opposable au
    /// consommateur.
    /// </para>
    /// </remarks>
    public class CR_VwProductionChassisFull : CR_Generic<vw_ProductionChassis_Full>, IR_VwProductionChassisFull
    {
        #region === Propriétés privées ===

        // Aucune propriété privée spécifique. Le nom du composant utilisé dans la CallChain est
        // résolu localement par GetType().Name, conformément à l'exemple canonique §3.2 du
        // 0232-CR : le champ _callee de CR_Generic<T> est déclaré private et n'est donc pas
        // accessible depuis la présente classe dérivée.

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
        /// une classe dérivée. Le paramètre reçu au constructeur est à la fois transmis à
        /// <c>base</c>, pour l'initialisation du socle, et conservé ici, pour l'usage propre de la
        /// présente classe. Le socle n'est pas modifié : il relève du régime de stabilité de
        /// §3.14.3 du 0230 et de la doctrine du patrimoine fermé de §4.15.1.
        /// </remarks>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="CR_VwProductionChassisFull"/> en propageant le
        /// DbContext partagé et le classificateur d'exceptions au constructeur de la classe de
        /// base, et en conservant localement le classificateur.
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
        public CR_VwProductionChassisFull(DbContext context, IS_ExClassifier classifier)
            : base(context, classifier)
        {
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Rend la liste des châssis rattachés à une série de production, réduits aux seize champs
        /// utiles au troisième onglet de la Page11, la réduction étant appliquée sur la requête et
        /// traduite en SQL.
        /// </summary>
        /// <remarks>
        /// <para>
        /// La projection est appliquée sur la requête et non après matérialisation : seules seize
        /// des soixante-seize colonnes de la vue transitent depuis le serveur de base de données.
        /// C'est la raison d'être de la méthode et la justification du Patron 2.
        /// </para>
        /// <para>
        /// Aucun ordonnancement n'est appliqué. Les trois critères de tri du tableau
        /// (<c>COIdOrder</c>, <c>COPartialSeriesIndex</c>, <c>PCOrderPosition</c>) figurent parmi
        /// les champs de service projetés et sont mis à la disposition de l'appelant, qui trie en
        /// mémoire après extraction.
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
        /// Liste des châssis projetés de la série demandée. Liste vide si la série ne comporte
        /// aucun châssis : ce résultat est nominal et ne constitue pas une erreur.
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
        public async Task<List<DTO_VwProductionChassisFull_P11>> GetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {GetType().Name} > {nameof(GetByProductionSeriesIdForP11AsNoTrackingAsync)}";

            try
            {
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des châssis de {typeof(vw_ProductionChassis_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Mobilise la projection SQL d'EF Core : le Select est traduit en clause SELECT
                // côté base de données et restreint le flux à seize colonnes sur soixante-seize.
                // Cette API n'est pas exposée par IR_Generic<T> et ne peut pas l'être (dépendance
                // EF Core interdite en A_Domain). C'est la justification doctrinale du Patron 2
                // selon §4.14.6 du 0230 (Cas 3).
                return await _context.Set<vw_ProductionChassis_Full>()
                    .AsNoTracking()
                    .Where(v => v.PSId == productionSeriesId)
                    .Select(v => new DTO_VwProductionChassisFull_P11
                    {
                        // Onze champs d'affichage, dans l'ordre des colonnes du tableau.
                        PCSeriesPosition = v.PCSeriesPosition,
                        PCCustomerPosition = v.PCCustomerPosition,
                        PCBarcodeId = v.PCBarcodeId,
                        PCQuantity = v.PCQuantity,
                        PCWindowSystemCode = v.PCWindowSystemCode,
                        PCElementHeight = v.PCElementHeight,
                        PCElementWidth = v.PCElementWidth,
                        PCColorNameIntExt = v.PCColorNameIntExt,
                        PCWindowText = v.PCWindowText,
                        PCSlidingTypeDetailed = v.PCSlidingTypeDetailed,
                        PCOpeningTypeText = v.PCOpeningTypeText,

                        // Cinq champs de service non affichés : identification, cohérence du lot,
                        // et trois critères d'ordonnancement laissés à la charge de l'appelant.
                        PSId = v.PSId,
                        PCId = v.PCId,
                        COIdOrder = v.COIdOrder,
                        COPartialSeriesIndex = v.COPartialSeriesIndex,
                        PCOrderPosition = v.PCOrderPosition
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