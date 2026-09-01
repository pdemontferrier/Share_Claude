using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.Business;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// Contrat spécialisé du repository dédié à la vue de base de données
    /// <see cref="vw_ProductionCutPiece_Full"/>, qui expose le détail des découpes composant
    /// les séries de production.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain et étend le contrat générique
    /// <see cref="IR_Generic{T}"/> paramétré pour <see cref="vw_ProductionCutPiece_Full"/>. Son
    /// implémentation concrète <c>CR_VwProductionCutPieceFull</c> réside dans
    /// C_Infrastructure/Repositories/DIGIT_TRY/ et dérive de
    /// <c>CR_Generic&lt;vw_ProductionCutPiece_Full&gt;</c>.
    /// </para>
    /// <para>
    /// Objectif : servir la question fermée « quelles découpes composent telle série de
    /// production, avec les seules caractéristiques que l'écran de consultation affiche ». La
    /// découpe est l'unité élémentaire du travail d'atelier et l'objet même de l'application :
    /// chaque châssis d'une commande se décompose en ouvrants, eux-mêmes en pièces de profilé à
    /// découper. Chaque découpe porte l'identification du profilé dont elle est issue, sa
    /// géométrie de coupe - une longueur encadrée à gauche et à droite d'une inclinaison et d'un
    /// pivot -, les dimensions du profilé, et sa position sur la barre qui lui a été affectée.
    /// Quatre indicateurs d'état jalonnent son parcours : la barre nécessaire est-elle
    /// approvisionnée, est-elle en rupture de stock, la découpe a-t-elle été réalisée, a-t-elle
    /// été refusée. La vue source expose deux cent trente-trois colonnes ; le cinquième onglet de
    /// la Page11 en affiche seize. La lecture exposée par le présent contrat rapatrie vingt champs
    /// - les seize champs d'affichage, plus quatre champs de service non affichés servant à
    /// l'identification des lignes, à la vérification de cohérence du lot reçu et à
    /// l'ordonnancement laissé à la charge de l'appelant.
    /// </para>
    /// <para>
    /// Portée du résultat : la lecture exposée n'applique aucun filtrage. Chez les découpes, à la
    /// différence des barres, le refus n'est pas porté par l'indicateur de suppression logique
    /// mais par une colonne distincte et projetée, <c>PCPIsCutRefused</c> : le refus relève de
    /// l'affichage et non de l'exclusion. Une découpe non encore affectée à une barre - dite au
    /// vivier - porte une position et un identifiant de barre absents ; cet état est nominal et ne
    /// traduit aucune anomalie. Une série dont l'optimisation n'a pas encore été lancée, ou qui ne
    /// comporte aucune découpe, produit une liste vide, résultat nominal et non erreur.
    /// </para>
    /// <para>
    /// Justification du Patron 2 : cette interface n'existe que parce que l'opération de son
    /// périmètre nécessite une API EF Core non disponible dans <see cref="IR_Generic{T}"/> - en
    /// l'occurrence la projection SQL traduite côté base de données, soit un <c>Select</c>
    /// retournant un type <c>DTO_</c> par expression LINQ-to-Entities. Aucune des dix-huit
    /// méthodes du contrat générique ne rend un type <c>DTO_</c> : toutes rendent l'entité
    /// paramétrée, une liste de cette entité, un booléen ou un entier. Servir le besoin par
    /// consommation directe du contrat générique imposerait de matérialiser les deux cent
    /// trente-trois colonnes puis d'en écarter deux cent treize en mémoire, ce qui relèverait du
    /// Cas 2
    /// (transformation LINQ-to-Objects portée par un Query Handler) et ferait perdre la réduction
    /// côté base qui est la finalité même du composant. Conformément au sous-bloc « Critère de
    /// création d'un Repository spécialisé CR_[Entité] » de §4.14.6 du 0230, la projection SQL
    /// traduite côté base est nominativement rangée au Cas 3, seul cas justifiant la création d'un
    /// repository spécialisé.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Hériter du contrat <see cref="IR_Generic{T}"/> paramétré pour
    ///     <see cref="vw_ProductionCutPiece_Full"/> afin d'exposer aux consommateurs les dix-huit
    ///     méthodes du socle générique en même temps que la lecture projetée propre à la vue.
    ///   </description></item>
    ///   <item><description>
    ///     Déclarer la lecture projetée qui mobilise la projection SQL absente du contrat
    ///     générique, et dont la réduction de deux cent trente-trois à vingt colonnes est traduite
    ///     en clause <c>SELECT</c> côté serveur de base de données.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne redéclare aucune des dix-huit méthodes du contrat <see cref="IR_Generic{T}"/> :
    ///     leur implémentation par défaut portée par
    ///     <c>CR_Generic&lt;vw_ProductionCutPiece_Full&gt;</c> est finale, conformément à R-4.15.3
    ///     et I-4.15.1 du 0231.
    ///   </description></item>
    ///   <item><description>
    ///     Ne porte aucune règle métier, aucun calcul et aucun ordonnancement : le tri du tableau
    ///     relève de l'appelant, qui l'applique en mémoire après extraction.
    ///   </description></item>
    ///   <item><description>
    ///     N'expose aucune opération d'écriture. La vue étant une source de lecture seule, toute
    ///     signature de persistance serait sans objet ; le contrat n'en déclare aucune, et le
    ///     socle n'en expose aucune qui soit opérante sur ce type (cf. avertissement ci-dessous).
    ///   </description></item>
    ///   <item><description>
    ///     N'écarte aucun enregistrement, et n'applique en particulier aucun filtrage sur
    ///     l'indicateur de suppression logique <c>PCPIsDeleted</c>. À la différence du repository
    ///     des barres, où le refus est précisément porté par cet indicateur, le refus d'une
    ///     découpe est porté par la colonne distincte <c>PCPIsCutRefused</c>, projetée au même
    ///     titre que les autres champs d'affichage.
    ///   </description></item>
    /// </list>
    /// <para>
    /// AVERTISSEMENT DE SURFACE HÉRITÉE. La vue <see cref="vw_ProductionCutPiece_Full"/> est
    /// déclarée sans clé dans le contexte EF Core (<c>HasNoKey</c>, <c>ToView</c>). L'héritage du
    /// contrat générique expose donc au consommateur dix-huit méthodes dont huit sont visibles
    /// mais échouent à l'exécution sur ce type. Elles ne doivent jamais être appelées :
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///     <c>AddAsync</c> - suivi impossible sur un type sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>GetByIdAsync</c> - <c>FindAsync</c> n'est pas supporté sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>GetByIdAsNoTrackingAsync</c> - résolution par <c>EF.Property&lt;int&gt;(e, "Id")</c>,
    ///     propriété inexistante sur la vue.
    ///   </description></item>
    ///   <item><description>
    ///     <c>GetAnyAsync</c> - <c>FindAsync</c> n'est pas supporté sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>UpdateAsync</c> - suivi impossible sur un type sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>UpdateRangeAsync</c> - suivi impossible sur un type sans clé.
    ///   </description></item>
    ///   <item><description>
    ///     <c>DeleteAsync</c> - <c>FindAsync</c>, puis suppression trackée.
    ///   </description></item>
    ///   <item><description>
    ///     <c>SoftDeleteAsync</c> - <c>FindAsync</c>, puis mise à jour trackée.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Les dix méthodes restantes sont opérantes sur ce type. Parmi elles, les trois lectures
    /// nominalement trackées - <c>GetFirstOrDefaultAsync</c>, <c>GetAllAsync</c> et
    /// <c>GetFilteredAsync</c> - fonctionnent, mais rendent des instances non suivies : EF Core
    /// ne trace jamais un type sans clé.
    /// </para>
    /// <para>
    /// Cette conséquence est assumée. L'extension du contrat générique est la forme canonique de
    /// la famille (Patron 2 « Extension par dérivation », §4.15.2 du 0230) ; la contrainte du
    /// socle n'exige qu'un type référence, ce que la vue satisfait ; l'héritage est valide à la
    /// compilation. La restriction porte sur l'usage et non sur la structure, et elle est portée
    /// par le présent avertissement.
    /// </para>
    /// </remarks>
    public interface IR_VwProductionCutPieceFull : IR_Generic<vw_ProductionCutPiece_Full>
    {
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
        /// (code <c>BU_ER_02</c>). Un identifiant nul ou négatif traduit un contexte de sélection
        /// non renseigné, anomalie fonctionnelle qui doit remonter plutôt que produire une lecture
        /// vide.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête
        /// projetée (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<DTO_VwProductionCutPieceFull_P11>> GetByProductionSeriesIdForP11AsNoTrackingAsync(
            string caller,
            int productionSeriesId,
            CancellationToken ct = default);
    }
}