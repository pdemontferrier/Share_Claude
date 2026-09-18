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
    /// <see cref="vw_ProductionBar_Full"/>, dérivant de <see cref="QH_Generic{T}"/> paramétré pour
    /// cette vue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases et honore le contrat
    /// <see cref="IQ_VwProductionBarFull"/>. Elle applique le patron principal d'extension par
    /// dérivation défini en §4.15.4 du 0230 : elle hérite du socle de lecture sans redéclarer ni
    /// masquer aucune de ses treize lectures, appelle <c>base(repository, classifier)</c> en
    /// première instruction de son constructeur, et ajoute les trois lectures projetées propres
    /// aux besoins couverts.
    /// </para>
    /// <para>
    /// Objectif : exposer aux couches supérieures les barres retenues par l'optimisation pour les
    /// séries de production, à raison d'une ligne par barre, selon trois lectures qui partagent le
    /// même type de projection et les mêmes dix-huit champs. La première rend la composition en
    /// barres d'une série, destinée au quatrième onglet de la Page11. Les deux autres servent la
    /// validation des barres par l'opérateur en Page20 : la lecture de la barre présentée, par
    /// laquelle l'opérateur vérifie qu'il a pris la bonne matière avant de l'accepter, et la
    /// lecture des barres de la série en rupture de stock, par laquelle il voit ce qui empêche la
    /// progression et peut libérer une barre dont la matière est revenue. Ces barres proviennent
    /// soit du stock de chutes issues de séries antérieures, soit du stock de barres neuves ;
    /// chacune porte cinq indicateurs d'état qui jalonnent son parcours, le refus s'accompagnant
    /// d'un motif conservé sur l'enregistrement. Les consommateurs prévus sont au nombre de deux
    /// et atteignent la présente classe par son seul contrat : un viewModel <c>VM_Page11</c>, qui
    /// applique le tri et la mise en forme de la lecture de consultation, et un viewModel
    /// <c>VM_Page20</c>, qui exploite les lectures de validation, la seconde dans l'ordre où elle
    /// est rendue.
    /// </para>
    /// <para>
    /// Portée du résultat - lecture Page11 : aucun enregistrement de la série n'est écarté ; les
    /// barres refusées, marquées comme logiquement supprimées, sont rendues avec leur motif, et le
    /// lot est livré sans ordonnancement, le tri relevant de l'appelant.
    /// </para>
    /// <para>
    /// Portée du résultat - lectures Page20 : la barre présentée est rendue quel que soit son
    /// état ; son absence n'est pas une erreur à ce niveau et son traitement appartient à
    /// l'appelant. Les barres en rupture rendues sont les barres de la série marquées en rupture
    /// de stock et non refusées, selon un critère strictement aligné sur celui du service
    /// <c>SR_ProductionSeries_SetBarOutOfStockFlag</c> ; elles sont ordonnées par ordre
    /// d'affichage de l'article puis par identifiant de barre, ordre constitutif du contrat, et
    /// une liste vide y est un résultat nominal.
    /// </para>
    /// <para>
    /// Sous-cas de lecture spécialisée : les trois lectures relèvent du second sous-cas du
    /// critère de §4.14.5 du 0230. Elles mobilisent la projection SQL traduite côté base de
    /// données, API EF Core absente du contrat <c>IR_Generic&lt;T&gt;</c> ; elles sont donc
    /// servies par délégation au repository spécialisé <see cref="IR_VwProductionBarFull"/>
    /// (Patron 2 de §4.15.2), injecté au constructeur de la présente classe, le repository du
    /// socle demeurant privé et inaccessible au dérivé. La lecture des barres en rupture requiert
    /// en outre un tri sur deux colonnes, que le socle n'expose pas ; la lecture de la barre
    /// présentée ne peut davantage être servie par <c>HandleGetByIdAsNoTrackingAsync</c>,
    /// inopérante sur ce type sans clé.
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
    ///     Valider les préconditions structurelles portant sur les identifiants reçus -
    ///     identifiant de série pour la lecture de consultation et pour la lecture des barres en
    ///     rupture, identifiant de barre pour la lecture de la barre présentée - avant toute
    ///     délégation.
    ///   </description></item>
    ///   <item><description>
    ///     Déléguer chaque lecture projetée au repository spécialisé consommé via son contrat, et
    ///     rendre son résultat sans transformation aucune, ordre compris.
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
    ///     de la méthode déléguée et l'appel relève du repository.
    ///   </description></item>
    ///   <item><description>
    ///     N'applique aucun filtrage sur l'indicateur de suppression logique. Pour la lecture
    ///     Page11, les barres refusées, marquées comme logiquement supprimées, font partie
    ///     intégrante du résultat attendu et sont rendues avec leur motif de refus. Pour la
    ///     lecture de la barre présentée en Page20, la barre désignée est rendue quel que soit son
    ///     état, refus compris. Pour la lecture des barres en rupture en Page20, l'exclusion des
    ///     barres refusées fait partie du critère de sélection porté par le repository délégué ;
    ///     la présente classe n'y ajoute rien.
    ///   </description></item>
    ///   <item><description>
    ///     Ne redéfinit et ne masque aucune méthode du socle : les treize lectures sont héritées
    ///     telles quelles, les lectures spécialisées sont ajoutées à côté du contrat.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune décision métier : le seul contrôle exercé est structurel. Aucun tri,
    ///     aucun filtrage, aucune mise en forme n'est appliqué à ce niveau ; l'ordre établi en aval
    ///     pour la lecture des barres en rupture est conservé intact, et la valeur absente de la
    ///     lecture de la barre présentée est relayée sans être qualifiée.
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
    /// <see cref="IQ_VwProductionBarFull"/>, qui constitue l'avertissement opposable au
    /// consommateur.
    /// </para>
    /// </remarks>
    public class QH_VwProductionBarFull : QH_Generic<vw_ProductionBar_Full>, IQ_VwProductionBarFull
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
        /// Repository spécialisé de la vue <see cref="vw_ProductionBar_Full"/>, délégué de la
        /// lecture projetée propre à la présente classe.
        /// </summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ <c>_repository</c> de
        /// <see cref="QH_Generic{T}"/> : ce dernier est déclaré <c>private</c> dans le socle, typé
        /// <c>IR_Generic&lt;vw_ProductionBar_Full&gt;</c>, et n'est donc accessible ni depuis une
        /// classe dérivée ni sous le type spécialisé. La même instance est reçue une seule fois au
        /// constructeur : elle est transmise à <c>base</c> pour l'initialisation du socle, et
        /// conservée ici sous son type spécialisé pour l'appel de la lecture projetée. Aucune
        /// seconde injection du contrat générique n'est introduite - elle produirait deux
        /// résolutions distinctes du conteneur pour un même rôle.
        /// </remarks>
        private readonly IR_VwProductionBarFull _repository;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="QH_VwProductionBarFull"/> en propageant le
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
        /// Le contrat spécialisé <see cref="IR_VwProductionBarFull"/> étend
        /// <c>IR_Generic&lt;vw_ProductionBar_Full&gt;</c> ; une injection unique satisfait donc à
        /// la fois le besoin du socle et celui de la lecture spécialisée.
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
        public QH_VwProductionBarFull(
            IR_VwProductionBarFull repository,
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
        /// Rend la liste des barres retenues par l'optimisation pour une série de production,
        /// réduites aux dix-huit champs utiles au quatrième onglet de la Page11.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// réduction de quatre-vingt-deux à dix-huit colonnes est appliquée sur la requête et
        /// traduite en clause <c>SELECT</c> côté serveur de base de données par le repository
        /// spécialisé délégué ; elle n'est jamais réalisée en mémoire.
        /// </para>
        /// <para>
        /// Objectif : offrir au consommateur un lot de lignes brut, qu'il lui appartient de trier
        /// et de mettre en forme. Aucun ordonnancement n'est appliqué : les trois critères de tri
        /// du tableau (<c>ARSortOrder</c>, <c>PBIsNewBar</c>, <c>PBId</c>) figurent parmi les
        /// champs projetés et sont mis à la disposition de l'appelant.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique : les barres
        /// refusées font partie intégrante du résultat attendu et sont rendues avec leur motif de
        /// refus.
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
        /// Liste des barres projetées de la série demandée, dans l'ordre où la source les rend,
        /// soit un ordre indéterminé. Liste vide si la série ne comporte aucune barre : ce
        /// résultat est nominal et ne constitue pas une erreur. Ne retourne jamais
        /// <see langword="null"/>.
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
        public async Task<List<DTO_VwProductionBarFull>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
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
                // les deux étages soient indiscernables du point de vue du consommateur, seule la
                // CallChain les différenciant dans un journal.
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des barres de {typeof(vw_ProductionBar_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

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
                // le repository est rendue telle quelle, liste vide comprise. Aucun filtrage n'est
                // appliqué sur l'indicateur de suppression logique : les barres refusées font
                // partie intégrante du résultat attendu.
                return await _repository.GetByProductionSeriesIdAsNoTrackingAsync(
                    callChain,
                    productionSeriesId,
                    ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend la barre de production désignée par son identifiant, telle que l'écran de
        /// validation des barres de la Page20 la présente à l'opérateur, réduite aux dix-huit
        /// champs du type de projection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection et la réduction de quatre-vingt-deux à dix-huit colonnes sont appliquées sur
        /// la requête et traduites en SQL côté serveur de base de données par le repository
        /// spécialisé délégué ; elles ne sont jamais réalisées en mémoire. La présente classe n'y
        /// prend aucune part et n'ajoute aucun coût à cette lecture.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « quelle est la barre qui m'est présentée ». La
        /// lecture sert l'onglet de détail de la barre présentée : l'opérateur y vérifie la
        /// matière - référence, désignation, couleur, longueur, origine neuve ou chute, nombre de
        /// découpes placées - avant d'accepter la barre.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur les indicateurs d'état : la barre désignée est rendue
        /// quel que soit son état, qu'elle soit validée, utilisée, en rupture de stock ou refusée.
        /// La lecture rend au plus une barre, l'identifiant demandé étant la clé primaire de la
        /// table des barres de production qui pilote la vue ; aucun ordonnancement n'est appliqué.
        /// </para>
        /// <para>
        /// La référence produite par le repository est retournée telle quelle, valeur absente
        /// comprise : ni filtrage, ni recopie, ni projection complémentaire ne sont appliqués en
        /// sortie. La valeur absente n'est ni qualifiée ni contrôlée à ce niveau : son traitement
        /// appartient à l'appelant.
        /// </para>
        /// <para>Tâches / Actions :</para>
        /// <list type="bullet">
        ///   <item><description>
        ///     Valider la précondition structurelle portant sur <paramref name="idProductionBar"/>,
        ///     à l'intérieur du bloc de capture.
        ///   </description></item>
        ///   <item><description>
        ///     Contrôler le jeton d'annulation immédiatement après la validation.
        ///   </description></item>
        ///   <item><description>
        ///     Déléguer la lecture de la barre au repository spécialisé, en lui transmettant la
        ///     CallChain enrichie et le jeton.
        ///   </description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre, correspondant à la colonne <c>PBId</c> de la vue, clé primaire
        /// de la table des barres de production. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// La barre désignée, projetée, ou <see langword="null"/> si aucune ligne de la vue ne
        /// porte cet identifiant. Ce retour absent n'est pas une erreur au niveau du Query Handler
        /// et ne lève aucune exception : la barre ayant été désignée en amont, il traduit une
        /// incohérence dont le traitement appartient à l'appelant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="idProductionBar"/> est inférieur ou égal à zéro
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
        public async Task<DTO_VwProductionBarFull?> HandleGetByProductionBarIdForP20AsNoTrackingAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByProductionBarIdForP20AsNoTrackingAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard §4.7 ;
                // R-4.7.25), puis contrôle du jeton, dans l'ordre validation -> ct. L'Ex_Business
                // typée remonte intacte au composant appelant via catch (Ex_Business) { throw; },
                // sans requalification. Le contrôle duplique délibérément celui que porte le
                // repository délégué : le modèle du projet porte la validation aux deux étages, et
                // l'échec au plus près de l'appelant produit une chaîne d'appel plus courte et plus
                // lisible. Le message est repris à l'identique de celui du repository, de sorte que
                // les deux étages soient indiscernables du point de vue du consommateur, seule la
                // CallChain les différenciant dans un journal.
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre fourni pour la projection de la barre présentée de {typeof(vw_ProductionBar_Full).Name} est invalide : '{idProductionBar}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Délégation au repository spécialisé (sous-cas (ii) du critère de lecture
                // spécialisée de §4.14.5) : la projection SQL traduite côté base de données est une
                // API EF Core absente d'IR_Generic<T>, et aucune des treize lectures du socle ne
                // rend un type DTO_ ; HandleGetByIdAsNoTrackingAsync est en outre inopérante sur ce
                // type sans clé. Aucun appel EF Core ni AsNoTracking() n'est porté ici : le suffixe
                // figure au nom de la méthode déléguée et l'appel relève du repository (R-4.14.11,
                // R-4.15.12). La délégation étant INTER-CLASSES, la CallChain propagée ne comporte
                // aucun redoublement du segment de composant.
                //
                // Le résultat est retourné SANS TRANSFORMATION AUCUNE : la référence produite par
                // le repository est rendue telle quelle, null compris. Le null n'est pas une erreur
                // à ce niveau : il est relayé sans être qualifié ni contrôlé, son traitement
                // appartenant à l'appelant. Aucun filtrage d'état n'est ajouté : la barre est
                // rendue quel que soit son état, refus compris.
                return await _repository.GetByProductionBarIdAsNoTrackingAsync(
                    callChain,
                    idProductionBar,
                    ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend les barres d'une série de production mises en attente pour rupture de stock et non
        /// refusées, réduites aux dix-huit champs du type de projection, à destination de l'écran
        /// de validation des barres de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement et la réduction de quatre-vingt-deux à dix-huit colonnes
        /// sont appliqués sur la requête et traduits en SQL côté serveur de base de données par le
        /// repository spécialisé délégué ; ils ne sont jamais réalisés en mémoire. La présente
        /// classe n'y prend aucune part et n'ajoute aucun coût à cette lecture.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « quelles barres bloquent la série pour rupture de
        /// stock ». La lecture sert l'onglet des barres en rupture de la série : l'opérateur y voit
        /// ce qui empêche la progression et peut libérer une barre dont la matière est revenue.
        /// </para>
        /// <para>
        /// Une barre est retenue lorsqu'elle appartient à la série demandée, qu'elle est marquée
        /// en rupture de stock (<c>PBIsOutOfStock</c>) et qu'elle n'est pas refusée
        /// (<c>PBIsDeleted</c>) ; la validation n'est pas un critère d'exclusion. Ce critère,
        /// strictement aligné sur celui du service <c>SR_ProductionSeries_SetBarOutOfStockFlag</c>,
        /// est intégralement porté par le repository délégué ; la présente classe n'y ajoute
        /// aucun filtrage complémentaire.
        /// </para>
        /// <para>
        /// L'ordre <c>ARSortOrder</c> croissant puis <c>PBId</c> croissant est porté par le
        /// repository délégué et constitue le contrat de la lecture : le départage par
        /// <c>PBId</c> garantit qu'une même situation d'atelier présente toujours les barres dans
        /// le même ordre. La présente classe le conserve intact et ne retrie pas.
        /// </para>
        /// <para>
        /// La référence produite par le repository est retournée telle quelle, liste vide
        /// comprise : ni tri, ni filtrage, ni recopie, ni projection complémentaire ne sont
        /// appliqués en sortie.
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
        ///     Déléguer la lecture des barres en rupture au repository spécialisé, en lui
        ///     transmettant la CallChain enrichie et le jeton.
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
        /// Liste des barres en rupture et non refusées de la série, projetées et ordonnées par
        /// <c>ARSortOrder</c> puis par <c>PBId</c> croissants. Ne retourne jamais
        /// <see langword="null"/>. Une liste vide est un résultat nominal et ne constitue pas une
        /// erreur.
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
        public async Task<List<DTO_VwProductionBarFull>> HandleGetOutOfStockByProductionSeriesIdForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetOutOfStockByProductionSeriesIdForP20AsNoTrackingAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard §4.7 ;
                // R-4.7.25), puis contrôle du jeton, dans l'ordre validation -> ct. L'Ex_Business
                // typée remonte intacte au composant appelant via catch (Ex_Business) { throw; },
                // sans requalification. Le contrôle duplique délibérément celui que porte le
                // repository délégué : le modèle du projet porte la validation aux deux étages, et
                // l'échec au plus près de l'appelant produit une chaîne d'appel plus courte et plus
                // lisible. Le message est repris à l'identique de celui du repository, de sorte que
                // les deux étages soient indiscernables du point de vue du consommateur, seule la
                // CallChain les différenciant dans un journal.
                if (productionSeriesId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de série de production fourni pour la projection des barres en rupture de stock de {typeof(vw_ProductionBar_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Délégation au repository spécialisé (sous-cas (ii) du critère de lecture
                // spécialisée de §4.14.5) : la projection SQL traduite côté base de données et le
                // tri sur deux colonnes sont des API EF Core absentes d'IR_Generic<T>, et aucune des
                // treize lectures du socle ne rend un type DTO_. Aucun appel EF Core ni
                // AsNoTracking() n'est porté ici : le suffixe figure au nom de la méthode déléguée
                // et l'appel relève du repository (R-4.14.11, R-4.15.12). La délégation étant
                // INTER-CLASSES, la CallChain propagée ne comporte aucun redoublement du segment
                // de composant.
                //
                // Le résultat est retourné SANS TRANSFORMATION AUCUNE : la référence produite par
                // le repository est rendue telle quelle, liste vide comprise. L'ordre ARSortOrder
                // puis PBId, porté par le repository, est constitutif du contrat : il n'est ni
                // repris ni altéré ici. Aucun filtrage complémentaire sur PBIsDeleted n'est ajouté :
                // l'exclusion des barres refusées est intégralement portée par le repository.
                return await _repository.GetOutOfStockByProductionSeriesIdAsNoTrackingAsync(
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