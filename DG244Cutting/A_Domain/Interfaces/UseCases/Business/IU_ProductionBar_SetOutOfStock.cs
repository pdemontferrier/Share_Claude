using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// Contrat du UseCase de positionnement de l'état de rupture de stock d'une barre de production
    /// neuve : mise en attente de la barre lorsque sa matière est absente de l'atelier, ou levée de
    /// cette attente lorsque la matière redevient disponible, sans altération du plan de coupe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : interface définie dans <c>A_Domain</c>, domaine <c>Business</c>. Elle est
    /// consommée par le ViewModel <c>VM_Page20</c> et par le menu horizontal de la page de découpe,
    /// via <c>IS_UseCaseInvoker</c> en chaîne (1) directe. L'exécution est déléguée à
    /// l'implémentation concrète
    /// <see cref="DG244Cutting.B_UseCases.UseCases.Business.UC_ProductionBar_SetOutOfStock"/>
    /// résidant en <c>B_UseCases/UseCases/Business</c>.
    /// </para>
    /// <para>
    /// Objectif : l'atelier est approvisionné à la demande, chaque barre étant approvisionnée juste
    /// avant d'être coupée. Il arrive que la matière d'une barre désignée à l'opérateur soit
    /// physiquement absente, typiquement lors d'une rupture chez le fournisseur. La barre est alors
    /// mise en attente : elle n'est ni supprimée ni écartée, conserve son plan de coupe et ses
    /// découpes rattachées, et demeure dans cet état jusqu'à sa libération par l'opérateur. Pendant
    /// l'attente, ses découpes sont exclues du vivier d'optimisation, faute de quoi la même barre
    /// serait indéfiniment proposée, et la série est signalée comme attendant de la matière. À la
    /// libération, les découpes retrouvent immédiatement leur place dans le plan de coupe et la
    /// barre redevient éligible à la préparation de la séquence d'entrée.
    /// </para>
    /// <para>
    /// Portée : seules les barres neuves peuvent être mises en attente. À la différence du refus,
    /// qui écarte une matière défectueuse, la mise en attente ne condamne rien : tout reprend en
    /// l'état lorsque la matière revient.
    /// </para>
    /// <para>
    /// Composant unique pour les deux sens : la mise en attente et la libération écrivent les mêmes
    /// champs avec des valeurs opposées et enchaînent les mêmes Services, dans le même ordre et avec
    /// la même gestion d'erreur. Un composant unique, paramétré par la valeur d'état à positionner,
    /// porte donc l'action dans les deux sens.
    /// </para>
    /// <para>
    /// Frontière transactionnelle : l'état de la barre, le marquage de ses découpes, l'indicateur de
    /// rupture de la série et, pour une mise en attente, l'inscription au journal métier sont validés
    /// ou annulés ensemble. L'indicateur de série est recalculé par le Service qui en est l'écrivain
    /// unique, appelé dans la transaction du UseCase : il partage ainsi le sort des autres écritures
    /// sans ouverture d'une seconde transaction (I-4.10.3). Il n'est jamais imposé : libérer une barre
    /// ne rétablit l'état normal de la série que si aucune autre barre n'y demeure en rupture.
    /// </para>
    /// <para>
    /// Traitement terminal des erreurs : toute défaillance applicative typée est annulée,
    /// journalisée et notifiée par le UseCase lui-même ; le consommateur reçoit un retour
    /// interprétable sans connaissance de la cause.
    /// </para>
    /// <para>
    /// Typologie : UseCase de cas Entité, portant l'action unitaire de positionnement de l'état de
    /// rupture de stock sur l'entité barre de production. La valeur d'état transmise est la valeur
    /// écrite, non un discriminant d'action. La méthode publique unique est nommée
    /// <c>ExecuteAsync</c>, sans dérogation au préfixe ni à la multiplicité. Elle expose un retour
    /// signalable destiné à la présentation (R-4.14.22), dont la sémantique est documentée sur la
    /// méthode.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer le point d'entrée unique de la mise en attente et de la libération d'une barre de production neuve.</description></item>
    /// <item><description>Restituer à la présentation un retour distinguant l'opération validée de l'échec applicatif traité.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>N'altère pas le plan de coupe : ni le rattachement des découpes, ni leur position de coupe, ni les indicateurs d'optimisation ; n'agit sur aucune chute du stock.</description></item>
    /// <item><description>Ne porte aucune règle métier propre : les contrôles d'état de la barre, des découpes et de la série sont portés par les Services métier en aval.</description></item>
    /// <item><description>Ne modifie pas le contexte de sélection de la barre et ne décide pas de la suite du parcours : ces responsabilités incombent au consommateur, à partir du retour.</description></item>
    /// <item><description>N'inscrit aucune action de cycle de vie lors d'une libération, qui constitue un retour à l'état normal.</description></item>
    /// <item><description>N'expose aucun type technique de persistance, conformément à la pureté contractuelle de <c>A_Domain</c>.</description></item>
    /// </list>
    /// </remarks>
    public interface IU_ProductionBar_SetOutOfStock
    {
        // --- Groupe 1 : Mise en attente et libération d'une barre de production ---

        /// <summary>
        /// Positionne l'état de rupture de stock de la barre de production désignée, en répercute
        /// la valeur sur ses découpes rattachées et recalcule l'indicateur de rupture de sa série.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté par la présentation lorsque l'opérateur constate l'absence de la
        /// matière de la barre désignée (mise en attente), ou lorsqu'il libère une barre depuis la
        /// liste des barres en rupture de la série (libération).
        /// </para>
        /// <para>
        /// Effets observables, validés ensemble en cas de succès : l'état de rupture de la barre
        /// prend la valeur demandée ; les découpes rattachées dont le marqueur diffère prennent la
        /// même valeur ; l'indicateur de rupture de la série est recalculé d'après l'état réel de
        /// ses barres et n'est écrit que s'il change ; lors d'une mise en attente uniquement, une
        /// action de cycle de vie de type rupture de stock est inscrite au journal métier pour la
        /// barre, sans commentaire. Les horodatages de mise à jour et les événements techniques
        /// associés accompagnent chaque écriture. En cas d'échec, aucune écriture métier ne
        /// persiste.
        /// </para>
        /// <para>
        /// Échecs métier traités terminalement : l'exécution est annulée, journalisée et notifiée,
        /// et le retour vaut <see langword="false"/>, lorsque l'identifiant de barre n'est pas
        /// strictement positif (<see cref="Ex_Business"/>, code <c>BU_ER_02</c>) ; lorsque la barre
        /// est introuvable (<see cref="Ex_Business"/>, code <c>BU_ER_03</c>) ; lorsque les
        /// Services métier en aval rejettent l'opération - barre validée, épuisée, supprimée ou
        /// refusée, issue d'une chute ou déjà dans l'état demandé ; barre sans découpe rattachée ou
        /// portant une découpe réalisée ou supprimée ; série introuvable ou supprimée ; contexte
        /// applicatif invalide lors d'une mise en attente. Les défaillances techniques et imprévues
        /// typées (<see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) suivent le même
        /// traitement.
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
        /// Identifiant de la barre de production visée. Doit être strictement positif et désigner
        /// une barre neuve existante.
        /// </param>
        /// <param name="isOutOfStock">
        /// Valeur d'état à positionner : <see langword="true"/> met la barre en attente de matière ;
        /// <see langword="false"/> lève cette attente. La barre doit se trouver dans l'état opposé.
        /// </param>
        /// <param name="ct">
        /// Jeton d'annulation coopérative, propagé à tous les appels asynchrones en aval. Par
        /// défaut <see langword="default"/>.
        /// </param>
        /// <returns>
        /// Une tâche dont le résultat, restitué à la présentation, se lit ainsi :
        /// <list type="bullet">
        /// <item><description><see langword="true"/> : opération effectuée, écritures validées en base ; le consommateur poursuit le parcours selon le sens de l'opération ;</description></item>
        /// <item><description><see langword="false"/> : échec applicatif annulé, journalisé et notifié à l'opérateur, aucune écriture métier n'étant persistée ; le consommateur revient au tableau de bord des séries.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Seule exception applicative propagée à l'appelant, lorsque l'annulation coopérative est
        /// signalée via <paramref name="ct"/>, conformément à §4.6. Aucun effet ne persiste.
        /// </exception>
        Task<bool> ExecuteAsync(
            string caller,
            int idProductionBar,
            bool isOutOfStock,
            CancellationToken ct = default);
    }
}