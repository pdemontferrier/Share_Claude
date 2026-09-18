using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// Contrat du UseCase de clôture des découpes d'une série de production : il statue sur
    /// l'achèvement de la série et, lorsqu'aucune découpe ne reste à réaliser, la clôture en une
    /// seule transaction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par le ViewModel <c>VM_Page20</c>, via <c>IS_UseCaseInvoker</c> en chaîne (1)
    /// directe, lorsque la séquence d'entrée sur la page de découpe ne trouve plus aucune découpe
    /// optimisable pour la série sélectionnée. L'exécution est déléguée à l'implémentation
    /// concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.Business.UC_ProductionSeries_CompleteCutting"/>
    /// résidant en <c>B_UseCases/UseCases/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : une série de production regroupe des découpes issues des commandes clients, que
    /// le parcours de découpe traite barre après barre, chaque barre étant approvisionnée juste
    /// avant d'être coupée. Lorsque la recherche de matière ne retourne plus rien, cet épuisement
    /// admet deux lectures, que le UseCase départage. Si aucune découpe ne subsiste, la série est
    /// achevée : elle est marquée comme telle, ce qui la classe parmi les séries terminées du
    /// tableau de bord et oriente désormais son ouverture vers la consultation ; les chutes
    /// qu'elle avait réservées sans les consommer sont rendues au stock ; l'événement est inscrit
    /// au journal métier. Si des découpes subsistent, elles sont nécessairement bloquées par une
    /// rupture de stock, puisque la recherche de matière exclut les découpes bloquées : la série
    /// n'est pas achevée, rien n'est écrit, et l'opérateur est maintenu sur la page de découpe,
    /// où il visualise ce qui bloque la progression.
    /// </para>
    /// <para>
    /// Test de clôture insensible au blocage : une découpe en rupture de stock reste une découpe à
    /// faire. Ce parti interdit de clôturer une série dont la matière manque encore.
    /// </para>
    /// <para>
    /// Libération des chutes réservées : elle intervient à la clôture parce que celle-ci est le
    /// seul moment où l'on sait avec certitude que la série ne consommera plus rien. Sans elle,
    /// une chute retenue par une barre mise en attente puis jamais libérée resterait invisible au
    /// stock, alors qu'elle est physiquement présente en atelier.
    /// </para>
    /// <para>
    /// Frontière transactionnelle : la pose de l'indicateur d'achèvement, la levée des
    /// réservations de chutes et l'inscription au journal métier sont validées ou annulées
    /// ensemble. Le test de clôture s'exécute dans la même transaction que les écritures qu'il
    /// conditionne.
    /// </para>
    /// <para>
    /// Traitement terminal des erreurs : toute défaillance applicative typée est annulée,
    /// journalisée et notifiée par le UseCase lui-même ; le consommateur reçoit un retour
    /// interprétable sans connaissance de la cause. Seule l'annulation coopérative est propagée.
    /// </para>
    /// <para>
    /// Typologie : UseCase de cas Entité, portant l'action unitaire de clôture des découpes sur
    /// l'entité série de production ; la levée des réservations et l'inscription au journal en
    /// sont les conséquences obligées. La méthode publique unique est nommée
    /// <c>ExecuteAsync</c>, sans dérogation au préfixe ni à la multiplicité. Elle expose un retour
    /// signalable destiné à la présentation (R-4.14.22), dont la sémantique est documentée sur la
    /// méthode.
    /// </para>
    /// <para>
    /// Écarts au document fonctionnel 0221, §4.2.3 : le test de clôture, que ce document place
    /// dans le ViewModel, est porté par le UseCase, car il constitue une règle métier et doit
    /// s'exécuter dans la transaction des écritures qu'il conditionne. La levée des réservations
    /// de chutes à la clôture, que ce document ne décrit pas alors qu'il en décrit la pose, est
    /// ajoutée aux effets de la clôture.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée unique du constat d'achèvement et de la clôture des découpes d'une série de production.</description></item>
    /// <item><description>Garantir l'absence de toute écriture lorsque des découpes restent à réaliser, y compris lorsqu'elles sont bloquées par une rupture de stock.</description></item>
    /// <item><description>Restituer à la présentation un retour distinguant la série clôturée, la série non achevée et l'échec applicatif traité.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'écrit pas directement en base : chaque écriture est déléguée à son Service métier.</description></item>
    /// <item><description>Ne duplique pas les contrôles d'état de la série (existence, suppression logique, clôture antérieure), portés par le Service de clôture.</description></item>
    /// <item><description>Ne décide d'aucune navigation ni d'aucun état d'interface : l'orientation de l'opérateur relève du consommateur, à partir du retour.</description></item>
    /// <item><description>Ne renseigne pas la date de fin de production de la série.</description></item>
    /// <item><description>Ne libère aucune réservation de chute d'une série non clôturée.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_ProductionSeries_CompleteCutting
    {
        // --- Groupe 1 : Clôture des découpes d'une série ---

        /// <summary>
        /// Statue sur l'achèvement des découpes de la série de production désignée et, si aucune
        /// découpe ne reste à réaliser, la clôture en une seule transaction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté par <c>VM_Page20</c> lorsque la séquence d'entrée sur la page de
        /// découpe ne trouve plus aucune découpe optimisable pour la série sélectionnée.
        /// </para>
        /// <para>
        /// Test de clôture : la série reste inachevée tant qu'elle compte au moins une découpe non
        /// coupée et non supprimée, qu'elle soit ou non bloquée par une rupture de stock.
        /// </para>
        /// <para>
        /// Effets observables, présents uniquement sur une clôture et validés ensemble : pose de
        /// l'indicateur d'achèvement des découpes sur la série ; levée des réservations de chutes
        /// détenues par la série ; inscription au journal métier d'une entrée de source série de
        /// production et de type découpes achevées, rattachée à l'identifiant de la série, sans
        /// commentaire ; inscription des événements techniques associés. Sur toute autre issue,
        /// aucun effet ne persiste. L'absence de chute réservée n'est pas une erreur.
        /// </para>
        /// <para>
        /// Échecs métier traités terminalement : l'exécution est annulée, journalisée et notifiée,
        /// et le retour vaut <see langword="null"/>, lorsque l'identifiant de série n'est pas
        /// strictement positif (<see cref="Ex_Business"/>, code <c>BU_ER_02</c>) ; lorsque la série
        /// est introuvable (<see cref="Ex_Business"/>, code <c>BU_ER_03</c>) ; lorsque la série est
        /// supprimée logiquement ou déjà clôturée (<see cref="Ex_Business"/>, code
        /// <c>BU_ER_04</c>) ; lorsque le contexte applicatif est absent ou invalide
        /// (<see cref="Ex_Business"/>, code <c>BU_ER_01</c> ou <c>BU_ER_02</c>). Les défaillances
        /// techniques et imprévues typées (<see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) suivent le même traitement.
        /// </para>
        /// <para>
        /// Défaillances non typées : une défaillance de persistance transitoire est rejouée par la
        /// stratégie d'exécution ; une défaillance de persistance non transitoire, ou l'épuisement
        /// des réexécutions, n'est pas typée par le UseCase et remonte au consommateur, où elle est
        /// captée par le filet de sécurité <c>VM_Generic.ExecuteSafeAsync</c>. Aucun effet ne
        /// persiste dans ce cas.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, enrichie localement selon le format normatif de la
        /// section 4.5. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="idProductionSeries">
        /// Identifiant de la série de production sélectionnée, dont l'achèvement est examiné. Doit
        /// être strictement positif.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par
        /// défaut <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, se lit ainsi :
        /// <list type="bullet">
        /// <item><description><see langword="true"/> : série clôturée, transaction validée ; le consommateur revient au tableau de bord des séries ;</description></item>
        /// <item><description><see langword="false"/> : des découpes non coupées subsistent, toutes bloquées par une rupture de stock, et aucune écriture n'a été réalisée ; le consommateur maintient l'opérateur sur la page de découpe, onglet des barres en rupture ;</description></item>
        /// <item><description><see langword="null"/> : échec applicatif annulé, journalisé et notifié ; le consommateur revient au tableau de bord des séries.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/>, conformément à §4.6. Aucun effet ne persiste.
        /// </exception>
        Task<bool?> ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default);
    }
}