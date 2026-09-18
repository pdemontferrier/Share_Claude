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
    /// <see cref="vw_ProductionCutPiece_Full"/>, dérivant de <see cref="QH_Generic{T}"/> paramétré
    /// pour cette vue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases et honore le contrat
    /// <see cref="IQ_VwProductionCutPieceFull"/>. Elle applique le patron principal d'extension
    /// par dérivation défini en §4.15.4 du 0230 : elle hérite du socle de lecture sans redéclarer
    /// ni masquer aucune de ses treize lectures, appelle <c>base(repository, classifier)</c> en
    /// première instruction de son constructeur, et ajoute les quatre lectures projetées propres
    /// aux besoins couverts.
    /// </para>
    /// <para>
    /// Objectif : exposer aux couches supérieures le détail des découpes de production, à raison
    /// d'une ligne par découpe, selon quatre lectures. La première rend l'ensemble des découpes
    /// d'une série, réduit aux vingt champs utiles au cinquième onglet de la Page11. Les deux
    /// suivantes servent le moteur d'optimisation des barres de la Page20 et sont réduites à
    /// treize champs : la recherche de la prochaine découpe à réaliser dans la série, tous
    /// articles confondus, et la lecture du vivier des découpes disponibles d'un article interne
    /// de la série. La quatrième rend le plan de coupe posé sur une barre de production - les
    /// découpes rattachées à la barre et non supprimées, dans leur ordre de réalisation -, réduit
    /// aux mêmes vingt champs que la première, pour l'onglet de contrôle du plan de coupe de la
    /// Page20. La découpe est l'unité élémentaire du travail d'atelier : chaque
    /// châssis d'une commande se décompose en ouvrants, eux-mêmes en pièces de profilé à découper.
    /// Chaque découpe porte l'identification du profilé dont elle est issue, sa géométrie de coupe
    /// - une longueur encadrée à gauche et à droite d'une inclinaison et d'un pivot -, les
    /// dimensions du profilé, sa position sur la barre affectée, et quatre indicateurs d'état qui
    /// jalonnent son parcours : barre approvisionnée, barre en rupture de stock, découpe réalisée,
    /// découpe refusée. Les consommateurs prévus sont au nombre de trois et atteignent la présente
    /// classe par son seul contrat : un viewModel <c>VM_Page11</c>, qui applique le tri et la
    /// mise en forme de la lecture de consultation, le service de lecture du vivier
    /// <c>IS_ProductionCutPiece_GetPool</c>, qui exploite les lectures d'optimisation dans
    /// l'ordre où elles sont rendues, et un viewModel <c>VM_Page20</c>, qui présente la lecture
    /// du plan de coupe dans l'ordre où elle est rendue.
    /// </para>
    /// <para>
    /// Sous-cas de lecture spécialisée : les quatre lectures relèvent du second sous-cas du critère
    /// de §4.14.5 du 0230. Elles mobilisent la projection SQL traduite côté base de données, API
    /// EF Core absente du contrat <c>IR_Generic&lt;T&gt;</c> ; elles sont donc servies par
    /// délégation au repository spécialisé <see cref="IR_VwProductionCutPieceFull"/> (Patron 2
    /// de §4.15.2), injecté au constructeur de la présente classe, le repository du socle
    /// demeurant privé et inaccessible au dérivé. Pour la lecture Page11, le rapport de réduction
    /// - vingt colonnes sur deux cent trente-trois - est le plus marqué du périmètre de la
    /// Page11, et le nombre de lignes attendu le plus élevé, chaque châssis d'une série produisant
    /// plusieurs découpes. Les lectures Page20 réduisent le flux à treize colonnes sur deux cent
    /// trente-trois ; la recherche de la prochaine découpe requiert en outre un tri sur deux
    /// colonnes, que le socle n'expose pas. La lecture du plan de coupe réduit le flux à vingt
    /// colonnes sur deux cent trente-trois et requiert en outre un tri sur trois clés, que le
    /// socle n'expose pas davantage.
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
    ///     identifiant de série pour les lectures par série, identifiant de barre pour la lecture
    ///     du plan de coupe, identifiant d'article interne pour la lecture du vivier - avant toute
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
    ///     N'applique aucun filtrage sur l'indicateur de suppression logique <c>PCPIsDeleted</c>.
    ///     Pour la lecture Page11, à la différence du Query Handler des barres, où le refus est
    ///     précisément porté par cet indicateur, le refus d'une découpe est porté par la colonne
    ///     distincte <c>PCPIsCutRefused</c>, projetée au titre des champs d'affichage : la
    ///     formulation retenue pour les barres n'est pas transposable ici. Pour les lectures
    ///     d'optimisation Page20, l'exclusion des découpes supprimées fait partie du critère de
    ///     disponibilité porté par le repository délégué ; la présente classe n'y ajoute rien.
    ///     Pour la lecture du plan de coupe, l'exclusion des découpes supprimées est portée par le
    ///     repository délégué ; la présente classe n'y ajoute rien.
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
    /// <see cref="IQ_VwProductionCutPieceFull"/>, qui constitue l'avertissement opposable au
    /// consommateur.
    /// </para>
    /// </remarks>
    public class QH_VwProductionCutPieceFull : QH_Generic<vw_ProductionCutPiece_Full>, IQ_VwProductionCutPieceFull
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
        /// pour l'usage des cascades de rattrapage des lectures spécialisées.
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
        /// Repository spécialisé de la vue <see cref="vw_ProductionCutPiece_Full"/>, délégué des
        /// lectures projetées propres à la présente classe.
        /// </summary>
        /// <remarks>
        /// Ce champ double, sans le remplacer, le champ <c>_repository</c> de
        /// <see cref="QH_Generic{T}"/> : ce dernier est déclaré <c>private</c> dans le socle, typé
        /// <c>IR_Generic&lt;vw_ProductionCutPiece_Full&gt;</c>, et n'est donc accessible ni depuis
        /// une classe dérivée ni sous le type spécialisé. La même instance est reçue une seule
        /// fois au constructeur : elle est transmise à <c>base</c> pour l'initialisation du socle,
        /// et conservée ici sous son type spécialisé pour l'appel des lectures projetées. Aucune
        /// seconde injection du contrat générique n'est introduite - elle produirait deux
        /// résolutions distinctes du conteneur pour un même rôle.
        /// </remarks>
        private readonly IR_VwProductionCutPieceFull _repository;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="QH_VwProductionCutPieceFull"/> en propageant le
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
        /// Le contrat spécialisé <see cref="IR_VwProductionCutPieceFull"/> étend
        /// <c>IR_Generic&lt;vw_ProductionCutPiece_Full&gt;</c> ; une injection unique satisfait
        /// donc à la fois le besoin du socle et celui de la lecture spécialisée.
        /// </para>
        /// </remarks>
        /// <param name="repository">
        /// Repository spécialisé de la vue, délégué des lectures projetées et du socle de lecture.
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
        public QH_VwProductionCutPieceFull(
            IR_VwProductionCutPieceFull repository,
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
        /// Rend la liste des découpes rattachées à une série de production, réduites aux vingt
        /// champs utiles au cinquième onglet de la Page11.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// réduction de deux cent trente-trois à vingt colonnes est appliquée sur la requête et
        /// traduite en clause <c>SELECT</c> côté serveur de base de données par le repository
        /// spécialisé délégué ; elle n'est jamais réalisée en mémoire. La présente classe n'y
        /// prend aucune part et n'ajoute aucun coût à cette lecture.
        /// </para>
        /// <para>
        /// Objectif : offrir au consommateur un lot de lignes brut, qu'il lui appartient de trier
        /// et de mettre en forme. Aucun ordonnancement n'est appliqué : les quatre critères de tri
        /// du tableau (<c>ACMDescription</c>, <c>ARSortOrder</c>, <c>PCPIdProductionBar</c>,
        /// <c>PCPCutPositionInBar</c>) figurent tous parmi les champs projetés et sont mis à la
        /// disposition de l'appelant.
        /// </para>
        /// <para>
        /// Aucun filtrage n'est appliqué sur l'indicateur de suppression logique
        /// <c>PCPIsDeleted</c>, et aucun recours n'est fait à <c>IgnoreQueryFilters</c>. Chez les
        /// découpes, à la différence des barres, le refus n'est pas porté par cet indicateur mais
        /// par la colonne distincte <c>PCPIsCutRefused</c>, projetée au titre des champs
        /// d'affichage : le refus relève de l'affichage et non de l'exclusion.
        /// </para>
        /// <para>
        /// Une découpe non encore affectée à une barre - dite au vivier - porte
        /// <c>PCPCutPositionInBar</c> et <c>PCPIdProductionBar</c> à l'état absent : état nominal,
        /// sans incidence sur la lecture.
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
        /// Liste des découpes projetées de la série demandée, dans l'ordre où la source les rend,
        /// soit un ordre indéterminé. Liste vide si la série ne comporte aucune découpe : ce
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
        public async Task<List<DTO_VwProductionCutPieceFull_P11>> HandleGetByProductionSeriesIdForP11AsNoTrackingAsync(
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
                        $"L'identifiant de série de production fourni pour la projection des découpes de {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

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
                // appliqué sur l'indicateur de suppression logique PCPIsDeleted : chez les
                // découpes, le refus est porté par la colonne distincte et projetée
                // PCPIsCutRefused, et relève de l'affichage et non de l'exclusion.
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

        /// <summary>
        /// Désigne la prochaine découpe à réaliser dans une série de production, tous articles
        /// confondus, réduite aux treize champs utiles au moteur d'optimisation de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement, la réduction au premier enregistrement et la réduction de
        /// deux cent trente-trois à treize colonnes sont appliqués sur la requête et traduits en
        /// SQL côté serveur de base de données par le repository spécialisé délégué ; ils ne sont
        /// jamais réalisés en mémoire. La présente classe n'y prend aucune part et n'ajoute aucun
        /// coût à cette lecture.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « que reste-t-il à faire dans la série ». La découpe
        /// retournée est la première des découpes disponibles de la série, dans l'ordre du couple
        /// référence et couleur <c>PCPReferenceColor</c> croissant, puis de la clé technique
        /// <c>PCPId</c> croissante. Son article interne détermine la matière sur laquelle porte
        /// l'optimisation et constitue la clé de compatibilité avec le stock de chutes.
        /// </para>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée, n'est engagée dans aucune
        /// optimisation, ni provisoire ni définitive, n'est pas supprimée et n'est pas bloquée par
        /// une rupture de stock de sa barre. Le refus d'une découpe n'est pas un critère
        /// d'exclusion : une découpe refusée et non coupée reste à réaliser.
        /// </para>
        /// <para>
        /// La sélection et l'ordonnancement - départage par <c>PCPId</c> compris, qui garantit
        /// qu'une même série présente toujours la même référence en premier - sont portés par le
        /// repository délégué ; la présente classe ne retrie pas. L'article interne de la découpe
        /// retournée est renseigné en vertu d'un invariant de données garanti par l'import des
        /// séries ; la présente classe ne le contrôle pas.
        /// </para>
        /// <para>
        /// La référence produite par le repository est retournée telle quelle, valeur absente
        /// comprise : ni filtrage, ni recopie, ni projection complémentaire ne sont appliqués en
        /// sortie.
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
        ///     Déléguer la recherche au repository spécialisé, en lui transmettant la CallChain
        ///     enrichie et le jeton.
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
        /// La prochaine découpe disponible de la série, projetée, ou <see langword="null"/> si la
        /// série ne compte plus aucune découpe optimisable. Ce retour absent est nominal et ne
        /// constitue pas une erreur.
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
        public async Task<DTO_VwProductionCutPieceFull_P20?> HandleGetNextReferenceToCutForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetNextReferenceToCutForP20AsNoTrackingAsync)}";

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
                        $"L'identifiant de série de production fourni pour la recherche de la prochaine découpe à réaliser dans {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{productionSeriesId}'. Doit être strictement positif.");

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
                // le repository est rendue telle quelle, null compris. Le null est nominal (série
                // sans plus aucune découpe optimisable). L'ordre de sélection est porté par le
                // repository et n'est pas repris ici.
                return await _repository.GetNextReferenceToCutForP20AsNoTrackingAsync(
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
        /// Rend l'ensemble des découpes disponibles pour un article interne d'une série de
        /// production, réduites aux treize champs utiles au moteur d'optimisation de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement et la réduction de deux cent trente-trois à treize colonnes
        /// sont appliqués sur la requête et traduits en SQL côté serveur de base de données par le
        /// repository spécialisé délégué ; ils ne sont jamais réalisés en mémoire. La présente
        /// classe n'y prend aucune part et n'ajoute aucun coût à cette lecture.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « que peut-on placer sur une barre de cet article ».
        /// Le résultat constitue le vivier d'optimisation : la matière sur laquelle le moteur
        /// calcule le remplissage d'une barre - chute du stock ou barre neuve - de l'article
        /// interne demandé. Toutes les découpes disponibles de la série pour cet article sont
        /// rendues, sans limite de nombre.
        /// </para>
        /// <para>
        /// Une découpe est disponible lorsqu'elle n'est pas coupée, n'est engagée dans aucune
        /// optimisation, ni provisoire ni définitive, n'est pas supprimée et n'est pas bloquée par
        /// une rupture de stock de sa barre. Le refus d'une découpe n'est pas un critère
        /// d'exclusion : une découpe refusée et non coupée reste à réaliser.
        /// </para>
        /// <para>
        /// L'ordre <c>PCPId</c> croissant est porté par le repository délégué et fonde le
        /// déterminisme du moteur : une même situation d'atelier conduit toujours à la même barre
        /// proposée. La présente classe le conserve intact et ne retrie pas.
        /// </para>
        /// <para>
        /// La référence produite par le repository est retournée telle quelle, liste vide
        /// comprise : ni tri, ni filtrage, ni recopie, ni projection complémentaire ne sont
        /// appliqués en sortie.
        /// </para>
        /// <para>Tâches / Actions :</para>
        /// <list type="bullet">
        ///   <item><description>
        ///     Valider, à l'intérieur du bloc de capture et dans cet ordre, les préconditions
        ///     structurelles portant sur <paramref name="productionSeriesId"/> puis sur
        ///     <paramref name="idArticleInternal"/>, ordre aligné sur celui du repository.
        ///   </description></item>
        ///   <item><description>
        ///     Contrôler le jeton d'annulation immédiatement après la validation.
        ///   </description></item>
        ///   <item><description>
        ///     Déléguer la lecture du vivier au repository spécialisé, en lui transmettant la
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
        /// <param name="idArticleInternal">
        /// Identifiant de l'article interne dont le vivier est demandé, correspondant à la colonne
        /// <c>PCPIdArticleInternal</c> de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes disponibles de l'article interne dans la série, projetées et
        /// ordonnées par <c>PCPId</c> croissant. Ne retourne jamais <see langword="null"/>. Une
        /// liste vide est un résultat nominal et ne constitue pas une erreur.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée, dans cet ordre de contrôle, si <paramref name="productionSeriesId"/> est
        /// inférieur ou égal à zéro, puis si <paramref name="idArticleInternal"/> est inférieur ou
        /// égal à zéro (code <c>BU_ER_02</c> dans les deux cas).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>), ou si une exception non contrôlée est requalifiée par
        /// le classificateur.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<DTO_VwProductionCutPieceFull_P20>> HandleGetOptimizationPoolForP20AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            int idArticleInternal,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetOptimizationPoolForP20AsNoTrackingAsync)}";

            try
            {
                // Préconditions structurelles validées DANS le bloc try (patron standard §4.7 ;
                // R-4.7.25), dans l'ordre série -> article interne aligné sur le repository, puis
                // contrôle du jeton, dans l'ordre validation -> ct. L'Ex_Business typée remonte
                // intacte au composant appelant via catch (Ex_Business) { throw; }, sans
                // requalification. Les contrôles dupliquent délibérément ceux que porte le
                // repository délégué : le modèle du projet porte la validation aux deux étages, et
                // l'échec au plus près de l'appelant produit une chaîne d'appel plus courte et plus
                // lisible. Les messages sont repris à l'identique de ceux du repository, de sorte
                // que les deux étages soient indiscernables du point de vue du consommateur, seule
                // la CallChain les différenciant dans un journal.
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

                // Délégation au repository spécialisé (sous-cas (ii) du critère de lecture
                // spécialisée de §4.14.5) : la projection SQL traduite côté base de données est une
                // API EF Core absente d'IR_Generic<T>, et aucune des treize lectures du socle ne
                // rend un type DTO_. Aucun appel EF Core ni AsNoTracking() n'est porté ici : le
                // suffixe figure au nom de la méthode déléguée et l'appel relève du repository
                // (R-4.14.11, R-4.15.12). La délégation étant INTER-CLASSES, la CallChain propagée
                // ne comporte aucun redoublement du segment de composant.
                //
                // Le résultat est retourné SANS TRANSFORMATION AUCUNE : la référence produite par
                // le repository est rendue telle quelle, liste vide comprise. L'ordre PCPId
                // croissant, porté par le repository, fonde le déterminisme du moteur
                // d'optimisation : il n'est ni repris ni altéré ici.
                return await _repository.GetOptimizationPoolForP20AsNoTrackingAsync(
                    callChain,
                    productionSeriesId,
                    idArticleInternal,
                    ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Rend le plan de coupe posé sur une barre de production, soit les découpes qui lui sont
        /// rattachées et non supprimées, dans leur ordre de réalisation, réduites aux vingt champs
        /// utiles à l'onglet de contrôle du plan de coupe de la Page20.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : lecture stricte, sans écriture, sans transformation et sans règle métier. La
        /// sélection, l'ordonnancement et la réduction de deux cent trente-trois à vingt colonnes
        /// sont appliqués sur la requête et traduits en SQL côté serveur de base de données par le
        /// repository spécialisé délégué ; ils ne sont jamais réalisés en mémoire. La présente
        /// classe n'y prend aucune part et n'ajoute aucun coût à cette lecture.
        /// </para>
        /// <para>
        /// Objectif : répondre à la question « quelles découpes l'optimisation a-t-elle posées sur
        /// cette barre, et dans quel ordre ». Le plan de coupe est présenté à l'opérateur avant
        /// l'acceptation de la barre, afin qu'il en vérifie les références, les désignations, les
        /// couleurs, la géométrie de coupe de chaque pièce et l'ordre de réalisation. Le
        /// rattachement à la barre suffit à désigner le plan : la série n'est pas demandée.
        /// </para>
        /// <para>
        /// Sont restituées les découpes rattachées à la barre demandée et dont l'indicateur de
        /// suppression logique <c>PCPIsDeleted</c> est faux ; cette exclusion est portée par le
        /// repository délégué. Les découpes marquées en rupture de stock
        /// (<c>PCPIsBarOutOfStock</c>), état nominal et réversible sous une barre présentée, et
        /// les découpes refusées (<c>PCPIsCutRefused</c>) sont restituées. La lecture ne vérifie
        /// pas que la barre porte effectivement un plan de coupe : cette condition relève de
        /// l'appelant.
        /// </para>
        /// <para>
        /// L'ordre de réalisation - découpes à position de coupe <c>PCPCutPositionInBar</c>
        /// renseignée d'abord, par position croissante puis par <c>PCPId</c> croissant, découpes
        /// rattachées sans position ensuite, selon le même départage - est porté par le repository
        /// délégué et constitue le contrat de la lecture. La présente classe le conserve intact et
        /// ne retrie pas : réordonner la liste fausserait le plan de coupe sans erreur visible.
        /// </para>
        /// <para>
        /// Le type de retour <see cref="DTO_VwProductionCutPieceFull_P11"/>, conçu pour la
        /// consultation de la Page11, est un emprunt assumé et provisoire : un type d'affichage
        /// propre à l'onglet de contrôle est appelé à le remplacer, la substitution ne touchant que
        /// la signature de la présente méthode et la projection du repository délégué.
        /// </para>
        /// <para>
        /// La référence produite par le repository est retournée telle quelle, liste vide
        /// comprise : ni tri, ni filtrage, ni recopie, ni projection complémentaire ne sont
        /// appliqués en sortie.
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
        ///     Déléguer la lecture du plan de coupe au repository spécialisé, en lui transmettant la
        ///     CallChain enrichie et le jeton.
        ///   </description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production dont le plan de coupe est demandé, rapproché de
        /// la colonne <c>PCPIdProductionBar</c> de la vue. Doit être strictement positif.
        /// </param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>
        /// Liste des découpes projetées du plan de coupe de la barre, dans l'ordre de réalisation.
        /// Liste vide si aucune découpe non supprimée n'est rattachée à la barre : ce résultat est
        /// nominal et ne constitue pas une erreur, son interprétation revenant à l'appelant. Ne
        /// retourne jamais <see langword="null"/>.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="idProductionBar"/> est inférieur ou égal à zéro
        /// (code <c>BU_ER_02</c>). Un identifiant nul ou négatif traduit un contexte de barre non
        /// renseigné, anomalie qui doit remonter plutôt que produire une liste vide
        /// indiscernable d'une barre sans découpe.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>), ou si une exception non contrôlée est requalifiée par
        /// le classificateur.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<DTO_VwProductionCutPieceFull_P11>> HandleGetByProductionBarIdForP20AsNoTrackingAsync(
            string caller,
            int idProductionBar,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByProductionBarIdForP20AsNoTrackingAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard §4.7 ;
                // R-4.7.25), puis contrôle du jeton, dans l'ordre validation -> ct : si
                // l'identifiant est invalide et le jeton annulé, l'Ex_Business l'emporte.
                // L'Ex_Business typée remonte intacte au composant appelant via
                // catch (Ex_Business) { throw; }, sans requalification. Le contrôle duplique délibérément celui que
                // porte le repository délégué : le modèle du projet porte la validation aux deux
                // étages, et l'échec au plus près de l'appelant produit une chaîne d'appel plus
                // courte et plus lisible. Le message est repris à l'identique de celui du
                // repository, de sorte que les deux étages soient indiscernables du point de vue du
                // consommateur, seule la CallChain les différenciant dans un journal.
                if (idProductionBar <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant de barre de production fourni pour la lecture du plan de coupe de {typeof(vw_ProductionCutPiece_Full).Name} est invalide : '{idProductionBar}'. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Délégation au repository spécialisé (sous-cas (ii) du critère de lecture
                // spécialisée de §4.14.5) : la projection SQL traduite côté base de données
                // (Select vers un type DTO_) et le tri sur trois clés sont des API EF Core absentes
                // d'IR_Generic<T>, et aucune des treize lectures du socle ne rend un type DTO_.
                // Aucun appel EF Core ni AsNoTracking() n'est porté ici : le suffixe figure au nom
                // de la méthode déléguée et l'appel relève du repository (R-4.14.11, R-4.15.12).
                // La délégation étant INTER-CLASSES, la CallChain propagée ne comporte aucun
                // redoublement du segment de composant.
                //
                // Le résultat est retourné SANS TRANSFORMATION AUCUNE : la référence produite par
                // le repository est rendue telle quelle, liste vide comprise. L'ordre de
                // réalisation, porté par le repository, est constitutif du plan de coupe : il
                // n'est ni repris ni altéré ici.
                return await _repository.GetByProductionBarIdForP20AsNoTrackingAsync(
                    callChain,
                    idProductionBar,
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