using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// Contrat du UseCase de refus, par l'opérateur, de la barre de production qui lui a été
    /// désignée : annulation intégrale du placement provisoire posé par l'optimisation, mise à
    /// l'écart motivée de la barre et, pour une barre de chute, retrait définitif de la chute
    /// source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par le ViewModel <c>VM_Page20</c>, via <c>IS_UseCaseInvoker</c> en chaîne (1)
    /// directe, lorsque l'opérateur refuse la barre présentée sur la page de découpe. L'exécution
    /// est déléguée à l'implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.Business.UC_BarRefusal"/> résidant en
    /// <c>B_UseCases/UseCases/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande et chaque barre présentée à
    /// l'opérateur est une barre provisoire, dont les découpes sont placées provisoirement et
    /// dont la chute source, pour une barre de chute, est réservée au profit de la série.
    /// L'opérateur peut refuser cette barre lorsque la matière est inutilisable, endommagée
    /// au-delà des deux zones de défaut admissibles, ou lorsque la chute est introuvable à son
    /// emplacement. Rien de ce que l'optimisation avait posé ne doit alors subsister : la barre
    /// est écartée du circuit avec son motif, les découpes qu'elle portait retournent au vivier
    /// des pièces à optimiser en conservant la trace du refus, la chute source d'une barre de
    /// chute est retirée du stock, et le refus est inscrit au journal métier.
    /// </para>
    /// <para>
    /// Le motif du refus est obligatoire et est sélectionné par l'opérateur dans une liste
    /// proposée par la présentation. Il est opaque pour le UseCase : la pertinence des motifs
    /// proposés - motifs de qualité pour toute barre, motifs de disponibilité pour les seules
    /// barres de chute - est garantie par la liste présentée, et le UseCase n'en contrôle pas la
    /// famille. Le motif est conservé sans altération sur la barre et dans le journal métier.
    /// </para>
    /// <para>
    /// Le refus d'une barre issue d'une chute signale que la chute elle-même est en cause :
    /// défectueuse, mal mesurée ou introuvable. La chute est donc retirée définitivement du
    /// stock et non libérée ; si elle est retrouvée ultérieurement, sa réintroduction relève de
    /// la saisie manuelle du stock de chutes. Une barre neuve refusée ne touche aucun stock géré
    /// par l'application.
    /// </para>
    /// <para>
    /// Journal métier : la nature d'action de refus de barre couvre deux situations que la base
    /// ne distingue pas - le refus d'une barre par l'opérateur, porté par le présent contrat, et
    /// la barre constatée inexploitable lors d'une validation avec défauts. Le commentaire de
    /// l'entrée de journal, qui porte ici le motif saisi par l'opérateur, est le seul
    /// discriminant entre ces deux situations.
    /// </para>
    /// <para>
    /// Frontière transactionnelle : la mise à l'écart de la barre, la libération de ses
    /// découpes, le retrait de la chute source et l'inscription au journal métier sont
    /// indissociables et sont validés ou annulés ensemble. Une barre écartée dont les découpes
    /// resteraient rattachées priverait la série de pièces à réaliser ; des découpes libérées
    /// sur une barre maintenue permettraient de les placer deux fois.
    /// </para>
    /// <para>
    /// Traitement terminal des erreurs : toute défaillance applicative typée est annulée,
    /// journalisée et notifiée par le UseCase lui-même ; le consommateur reçoit un retour
    /// interprétable sans connaissance de la cause.
    /// </para>
    /// <para>
    /// Typologie : UseCase de cas Concept à méthode publique unique. Le concept porté, le refus
    /// d'une barre de production, orchestre des écritures sur quatre entités distinctes (barre
    /// de production, découpe, chute du stock, action de cycle de vie), dont aucune n'est seule
    /// cible. La méthode publique conserve le nom par défaut <c>ExecuteAsync</c>, sans
    /// dérogation au préfixe ni à la multiplicité. Elle expose un retour signalable destiné à la
    /// présentation (R-4.14.22), dont la sémantique est documentée sur la méthode.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée unique du refus d'une barre de production par l'opérateur.</description></item>
    /// <item><description>Restituer à la présentation un retour distinguant le refus effectué de l'échec applicatif traité.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'implémente aucune règle métier propre : les contrôles d'état et les écritures sont portés par les Services métier.</description></item>
    /// <item><description>N'écrit pas directement en base : chaque écriture est déléguée à son Service métier.</description></item>
    /// <item><description>Ne duplique aucun contrôle d'état porté par les Services métier ; seule la présence du motif est contrôlée avant toute écriture, parce qu'elle conditionne le geste même du refus.</description></item>
    /// <item><description>Ne contrôle pas la famille du motif, dont la pertinence relève de la liste présentée.</description></item>
    /// <item><description>N'agit pas sur le contexte de sélection : la désélection de la barre et la relance de la séquence d'entrée incombent au ViewModel consommateur, à partir du retour.</description></item>
    /// <item><description>Ne pose aucun indicateur d'approvisionnement de la série.</description></item>
    /// <item><description>Ne libère pas la chute source d'une barre de chute : elle est retirée définitivement du stock.</description></item>
    /// <item><description>Ne modifie pas le nombre de découpes réalisées sur la barre, aucune coupe n'ayant eu lieu.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_BarRefusal
    {
        // --- Groupe 1 : Refus d'une barre de production ---

        /// <summary>
        /// Annule intégralement, en une transaction unique, le placement provisoire de la barre de
        /// production refusée par l'opérateur, et inscrit le refus au journal métier avec son
        /// motif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté par <c>VM_Page20</c> lorsque l'opérateur refuse la barre présentée,
        /// après sélection d'un motif dans la liste proposée.
        /// </para>
        /// <para>
        /// Effets observables, validés ensemble sur un refus abouti : la barre reçoit le motif de
        /// refus et est supprimée logiquement ; les découpes qui lui étaient rattachées sont
        /// marquées refusées, ne sont plus placées provisoirement, sont détachées de la barre et
        /// perdent leur position de coupe ; pour une barre de chute uniquement, la chute source
        /// est supprimée logiquement, sa réservation restant inchangée ; une entrée de journal
        /// métier de refus de barre est inscrite pour la barre, avec le motif pour commentaire ;
        /// les événements techniques associés sont inscrits. Sur toute autre issue, aucun effet ne
        /// persiste.
        /// </para>
        /// <para>
        /// Échecs métier traités terminalement : l'exécution est annulée, journalisée et notifiée,
        /// et le retour vaut <see langword="false"/>, lorsque l'identifiant de barre n'est pas
        /// strictement positif (<see cref="Ex_Business"/>, code <c>BU_ER_02</c>) ; lorsque le
        /// motif est nul, vide ou composé uniquement d'espaces (<see cref="Ex_Business"/>, code
        /// <c>BU_ER_01</c>) ; lorsque la barre est introuvable (<see cref="Ex_Business"/>, code
        /// <c>BU_ER_03</c>) ; lorsqu'une barre de chute ne référence aucune chute source
        /// strictement positive, incohérence des données persistées (<see cref="Ex_Business"/>,
        /// code <c>BU_ER_04</c>). Les rejets émis en aval par les Services métier - motif trop
        /// long, barre déjà refusée, épuisée, en rupture ou scellée, barre sans découpe
        /// rattachée, découpe dans un état incompatible, notamment sur une barre déjà validée,
        /// chute introuvable ou déjà retirée, contexte applicatif invalide - ainsi que les
        /// défaillances techniques et imprévues typées (<see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>), suivent le même traitement.
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
        /// <param name="idProductionBar">
        /// Identifiant de la barre de production refusée par l'opérateur. Doit être strictement
        /// positif.
        /// </param>
        /// <param name="rejectionReason">
        /// Motif du refus sélectionné par l'opérateur. Ne doit être ni nul, ni vide, ni composé
        /// uniquement d'espaces ; sa longueur est bornée par la colonne de destination. Transmis
        /// sans aucune altération.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par
        /// défaut <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, se lit ainsi :
        /// <list type="bullet">
        /// <item><description><see langword="true"/> : refus effectué, transaction validée ; le consommateur désélectionne la barre et relance la séquence d'entrée, qui propose une autre barre pour la même référence ;</description></item>
        /// <item><description><see langword="false"/> : échec applicatif annulé, journalisé et notifié ; le consommateur revient au tableau de bord des séries.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/>, conformément à §4.6. Aucun effet ne persiste.
        /// </exception>
        Task<bool> ExecuteAsync(
            string caller,
            int idProductionBar,
            string rejectionReason,
            CancellationToken ct = default);
    }
}