namespace DG244Cutting.A_Domain.Common.Enums.Business
{
    /// <summary>
    /// Entité métier sur laquelle porte une action du cycle de vie ; la valeur sert
    /// de clé de lecture de l’identifiant <c>IdSource</c> d’une entrée de
    /// traçabilité métier en désignant la table dans laquelle rechercher la ligne
    /// qu’il identifie.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Le type est un repère de code : il rend lisible, au point d’appel, la source
    /// d’une action que le seul littéral numérique rendrait opaque. Il n’est pas une
    /// donnée lue à l’exécution.
    /// </para>
    /// <para>
    /// Invariant de fidélité : chaque membre reproduit une ligne non supprimée du
    /// référentiel <see cref="Entities.DIGIT_TRY.LifecycleActionSource"/>. Sa valeur
    /// est strictement identique à l’identifiant <c>Id</c> de la ligne et son nom au
    /// <c>SourceName</c> de celle-ci, y compris lorsque ce dernier diffère du nom de
    /// la classe d’entité générée (<c>ProductionChassis</c> et non
    /// <see cref="Entities.DIGIT_TRY.ProductionChassi"/>). Toute divergence de
    /// valeur rattacherait les entrées de traçabilité à une autre table et les
    /// rendrait inexploitables.
    /// </para>
    /// <para>
    /// Duplication assumée : le type reproduit intégralement un référentiel tenu en
    /// base de données et susceptible d’évoluer. Tout ajout de ligne, ou toute
    /// modification d’identifiant, dans ce référentiel appelle une mise à jour
    /// corrélative du type.
    /// </para>
    /// <para>
    /// Conversion au point d’écriture : la propriété
    /// <c>IdLifecycleActionSource</c> de
    /// <see cref="Entities.DIGIT_TRY.LifecycleAction"/> est de type
    /// <see langword="short"/>, type sous-jacent de la présente énumération. La
    /// conversion explicite d’une valeur du type vers cette propriété est opérée par
    /// le service de journalisation du cycle de vie, et non par le type.
    /// </para>
    /// <para>
    /// Valeur par défaut non déclarée : aucun membre n’est déclaré à zéro, les
    /// identifiants du référentiel commençant à 1 et une source étant toujours
    /// choisie explicitement par son appelant. La valeur par défaut du type
    /// (<c>default(En_LifecycleActionSource)</c>, soit 0) ne correspond donc à
    /// aucun membre et ne désigne aucune source valide ; son rejet relève du
    /// service de journalisation dont le contrat est exposé par
    /// <c>IS_LifecycleAction_Add</c>. L’énumération décrit cet invariant sans le
    /// garantir.
    /// </para>
    /// </remarks>
    public enum En_LifecycleActionSource : short
    {
        /// <summary>
        /// Série de production — ligne <c>Id</c> 1 du référentiel, code
        /// <c>PSE</c>, <c>SourceName</c> <c>ProductionSeries</c>.
        /// </summary>
        ProductionSeries = 1,

        /// <summary>
        /// Commande client — ligne <c>Id</c> 2 du référentiel, code <c>PCO</c>,
        /// <c>SourceName</c> <c>CustomerOrder</c>.
        /// </summary>
        CustomerOrder = 2,

        /// <summary>
        /// Châssis — ligne <c>Id</c> 3 du référentiel, code <c>PCH</c>,
        /// <c>SourceName</c> <c>ProductionChassis</c>.
        /// </summary>
        ProductionChassis = 3,

        /// <summary>
        /// Pièce à découper — ligne <c>Id</c> 4 du référentiel, code <c>PCP</c>,
        /// <c>SourceName</c> <c>ProductionCutPiece</c>.
        /// </summary>
        ProductionCutPiece = 4,

        /// <summary>
        /// Barre à découper — ligne <c>Id</c> 5 du référentiel, code <c>PBA</c>,
        /// <c>SourceName</c> <c>ProductionBar</c>.
        /// </summary>
        ProductionBar = 5
    }
}