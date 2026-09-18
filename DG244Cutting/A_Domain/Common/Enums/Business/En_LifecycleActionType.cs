namespace DG244Cutting.A_Domain.Common.Enums.Business
{
    /// <summary>
    /// Nature d’une action de cycle de vie inscrite au journal métier
    /// LifecycleAction, reflet dans le code de la table de référence
    /// LifecycleActionType.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Correspondance avec la base : chaque membre porte l’identifiant et reprend
    /// le code de la ligne correspondante de
    /// <see cref="Entities.DIGIT_TRY.LifecycleActionType"/>, dont l’énumération est
    /// le miroir complet. Les membres <c>BarValidated</c> à
    /// <c>CuttingCompleted</c> (valeurs 3 à 8) sont ceux du parcours de
    /// découpe. Les autres membres sont déclarés par fidélité à la table, sans
    /// consommateur prévu dans l’application à ce jour.
    /// </para>
    /// <para>
    /// Type sous-jacent : <c>short</c>, aligné sur
    /// <see cref="Entities.DIGIT_TRY.LifecycleAction.IdLifecycleActionType"/>. Ce
    /// choix s’écarte des autres énumérations du domaine, qui emploient le type
    /// sous-jacent par défaut. La conversion explicite vers la valeur de l’entité,
    /// au point d’écriture du journal, relève du Service de journalisation
    /// <c>IS_LifecycleAction_Add</c>.
    /// </para>
    /// <para>
    /// Absence de sentinelle : <c>default(En_LifecycleActionType)</c> vaut 0 et ne
    /// correspond à aucun membre déclaré. Une valeur non déclarée n’est pas une
    /// nature d’action valide. Ce choix s’écarte délibérément du patron à
    /// sentinelle des autres énumérations métier.
    /// </para>
    /// <para>
    /// Indépendance vis-à-vis de la source : l’énumération qualifie la nature de
    /// l’action, indépendamment de l’entité sur laquelle elle porte. Elle ne
    /// garantit pas la cohérence du couple source / type, et ne décrit aucune
    /// correspondance entre ses membres et les sources.
    /// </para>
    /// <para>
    /// Risque de duplication : une ligne ajoutée ou un identifiant modifié dans la
    /// table n’est pas répercuté automatiquement dans l’énumération. Leur
    /// synchronisation est manuelle ; toute divergence conduirait à qualifier à
    /// tort les entrées du journal.
    /// </para>
    /// </remarks>
    public enum En_LifecycleActionType : short
    {
        /// <summary>
        /// Série importée depuis Axapta / Look 3E.
        /// </summary>
        Imported = 1,

        /// <summary>
        /// Série validée en interne avant son lancement.
        /// </summary>
        Validated = 2,

        /// <summary>
        /// Barre optimisée acceptée telle quelle pour la série.
        /// </summary>
        BarValidated = 3,

        /// <summary>
        /// Barre optimisée écartée pour la série. Cette valeur couvre deux
        /// situations que la base ne distingue pas : le refus de la barre par
        /// l’opérateur, et la barre acceptée avec défauts puis constatée
        /// inexploitable ; le commentaire de l’entrée de journal porte la
        /// distinction.
        /// </summary>
        BarRefused = 4,

        /// <summary>
        /// Barre optimisée acceptée malgré des défauts déclarés, ce qui entraîne la
        /// recomposition de son plan de coupe.
        /// </summary>
        BarWithDefectsValidated = 5,

        /// <summary>
        /// Découpe réalisée et validée.
        /// </summary>
        CuttingValidated = 6,

        /// <summary>
        /// Découpe refusée, faute de pouvoir l’exécuter.
        /// </summary>
        CuttingRefused = 7,

        /// <summary>
        /// Toutes les découpes de la série sont réalisées ; fin du parcours de
        /// découpe.
        /// </summary>
        CuttingCompleted = 8,

        /// <summary>
        /// Démarrage officiel de la production de la série.
        /// </summary>
        ProductionStarted = 9,

        /// <summary>
        /// Toutes les opérations de production de la série sont terminées.
        /// </summary>
        ProductionCompleted = 10,

        /// <summary>
        /// Date d’expédition de la série planifiée et enregistrée.
        /// </summary>
        ShippingPlanned = 11,

        /// <summary>
        /// Série expédiée vers son point de livraison.
        /// </summary>
        Shipped = 12,

        /// <summary>
        /// Anomalie ou incident détecté dans le cycle de vie de la série.
        /// </summary>
        Error = 13,

        /// <summary>
        /// Série réactivée après correction ou modification.
        /// </summary>
        Reopened = 14,

        /// <summary>
        /// Rupture de stock de la barre optimisée pour la série.
        /// </summary>
        BarOutOfStock = 15
    }
}