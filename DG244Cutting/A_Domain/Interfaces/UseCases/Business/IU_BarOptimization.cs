using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// Contrat du UseCase de préparation de la prochaine barre de production d'une série :
    /// détermination de la matière à mobiliser et des découpes à y réaliser, puis
    /// matérialisation de la barre provisoire correspondante avec son plan de coupe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par le ViewModel <c>VM_Page20</c>, via <c>IS_UseCaseInvoker</c> en chaîne (1)
    /// directe, lors de la séquence d'entrée sur la page de découpe, lorsqu'aucune barre n'est en
    /// cours de traitement pour la série sélectionnée. L'exécution est déléguée à
    /// l'implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.Business.UC_BarOptimization"/> résidant en
    /// <c>B_UseCases/UseCases/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande, barre par barre ; aucune barre n'est
    /// préparée à l'avance. Avant chaque barre, l'opérateur doit savoir quelle matière prendre et
    /// quelles pièces y couper. Le UseCase répond à cette question en quatre temps : il résout la
    /// prochaine combinaison référence / couleur à traiter dans la série et en extrait l'article
    /// interne, clé de compatibilité avec le stock de chutes ; il lit le vivier des découpes de
    /// cet article et les chutes disponibles ; il délègue au moteur d'optimisation le choix du
    /// contenant (une chute réutilisable en priorité, à défaut une barre neuve) et de son
    /// contenu ; il orchestre enfin la matérialisation du résultat en confiant chaque écriture au
    /// Service métier qui en porte la responsabilité. Une seule barre est préparée par
    /// invocation.
    /// </para>
    /// <para>
    /// Le consommateur ne transmet que l'identifiant de la série : la résolution de l'article
    /// interne est portée par le UseCase, qui la tire de la prochaine découpe à réaliser. La
    /// séquence d'entrée de la page se réduit ainsi, pour ce qui concerne la préparation d'une
    /// barre, à une invocation unique dont le retour pilote la suite du parcours.
    /// </para>
    /// <para>
    /// Frontière transactionnelle : la création de la barre, le rattachement des découpes et la
    /// réservation de la chute source sont indissociables et sont validés ou annulés ensemble.
    /// Une barre dont les découpes ne seraient pas rattachées laisserait la série incohérente ;
    /// une chute réservée sans barre correspondante serait retirée du stock sans contrepartie.
    /// </para>
    /// <para>
    /// Traitement terminal des erreurs : toute défaillance applicative typée est annulée,
    /// journalisée et notifiée par le UseCase lui-même ; le consommateur reçoit un retour
    /// interprétable sans connaissance de la cause.
    /// </para>
    /// <para>
    /// Typologie : UseCase de cas Concept à méthode publique unique. Le concept porté, la
    /// préparation d'une barre de production, orchestre des écritures sur trois entités
    /// distinctes (barre de production, découpe, chute du stock), dont aucune n'est seule cible.
    /// La méthode publique conserve le nom par défaut <c>ExecuteAsync</c>, sans dérogation au
    /// préfixe ni à la multiplicité. Elle expose un retour signalable destiné à la présentation
    /// (R-4.14.22), dont la sémantique est documentée sur la méthode.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée unique de la préparation de la prochaine barre de production d'une série.</description></item>
    /// <item><description>Restituer à la présentation un retour distinguant la barre préparée, l'absence de découpe à optimiser et l'échec applicatif traité.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'implémente aucune règle de calcul : ni placement des découpes, ni choix de la chute, ni qualification du résidu ; le calcul est entièrement délégué au moteur d'optimisation.</description></item>
    /// <item><description>N'écrit pas directement en base : chaque écriture est déléguée à son Service métier.</description></item>
    /// <item><description>Ne positionne pas le contexte de sélection de la barre : cette responsabilité incombe au ViewModel consommateur, à partir du retour.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie de la barre : la traçabilité métier commence à sa validation.</description></item>
    /// <item><description>Ne pose aucun indicateur d'approvisionnement de la série (barre de chute fournie, barre neuve fournie).</description></item>
    /// <item><description>Ne réserve aucun stock pour une barre neuve, qui ne consomme aucune matière gérée par l'application.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_BarOptimization
    {
        // --- Groupe 1 : Préparation de la prochaine barre de production ---

        /// <summary>
        /// Détermine la prochaine matière à couper pour la série de production désignée et
        /// matérialise en base la barre provisoire retenue, avec ses découpes positionnées et,
        /// pour une barre de chute, la réservation de la chute source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté par <c>VM_Page20</c> à l'entrée sur la page de découpe, lorsqu'aucune
        /// barre n'est en cours de traitement pour la série sélectionnée.
        /// </para>
        /// <para>
        /// Effets observables, validés ensemble sur une optimisation aboutie : création d'une
        /// barre de production provisoire ; rattachement à cette barre des découpes retenues,
        /// chacune positionnée selon son rang dans le plan de coupe calculé ; réservation de la
        /// chute source au profit de la série, sur une barre de chute uniquement ; inscription des
        /// événements techniques associés. Sur toute autre issue, aucun effet ne persiste.
        /// </para>
        /// <para>
        /// Échecs métier traités terminalement : l'exécution est annulée, journalisée et notifiée,
        /// et le retour vaut <see langword="null"/>, lorsque l'identifiant de série n'est pas
        /// strictement positif (<see cref="Ex_Business"/>, code <c>BU_ER_02</c>) ; lorsque la
        /// prochaine découpe ne porte pas d'article interne strictement positif, ou lorsque le
        /// moteur d'optimisation rend une issue inexploitable - anomalie de données, issue
        /// étrangère à l'optimisation d'une barre, issue non valorisée ou inconnue
        /// (<see cref="Ex_Business"/>, code <c>BU_ER_04</c>). Les rejets émis en aval par les
        /// lectures et par les Services métier, ainsi que les défaillances techniques et
        /// imprévues typées (<see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>),
        /// suivent le même traitement.
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
        /// Identifiant de la série de production sélectionnée, pour laquelle la prochaine barre est
        /// préparée. Doit être strictement positif.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par
        /// défaut <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, se lit ainsi :
        /// <list type="bullet">
        /// <item><description>valeur strictement positive : identifiant de la barre provisoire créée, après validation de la transaction ; le consommateur affiche la barre à préparer ;</description></item>
        /// <item><description>zéro : aucune découpe ne reste à optimiser dans la série, aucune écriture n'ayant été réalisée ; le consommateur engage le contrôle de clôture de la série ;</description></item>
        /// <item><description><see langword="null"/> : échec applicatif annulé, journalisé et notifié ; le consommateur revient au tableau de bord des séries.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/>, conformément à §4.6. Aucun effet ne persiste.
        /// </exception>
        Task<int?> ExecuteAsync(
            string caller,
            int idProductionSeries,
            CancellationToken ct = default);
    }
}