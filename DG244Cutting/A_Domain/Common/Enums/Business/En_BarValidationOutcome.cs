namespace DG244Cutting.A_Domain.Common.Enums.Business
{
    /// <summary>
    /// Issue de la validation d’une barre de production, exprimée par un
    /// discriminant à valeurs mutuellement exclusives et non persisté.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Undetermined"/> est la valeur par défaut du type
    /// (<c>default(En_BarValidationOutcome) == Undetermined</c>) : sentinelle et
    /// garde défensive, elle signale une issue non valorisée. Elle n’est produite
    /// ni par une validation aboutie ni par un échec traité. Une issue non
    /// valorisée ne doit jamais conduire à la découpe.
    /// </para>
    /// <para>
    /// Le type est retourné par la méthode publique du contrat
    /// <c>IU_BarValidation</c>, dont l’implémentation est portée par
    /// <c>UC_BarValidation</c>, au titre du retour signalable à la présentation.
    /// Il est lu par <c>VM_Page20</c>, via <c>IS_UseCaseInvoker</c>, pour router
    /// le parcours opérateur : <see cref="BarSealed"/> conduit au carrefour entre
    /// Page21 et Page22 ; <see cref="BarUnusable"/> conduit au rafraîchissement de
    /// Page20 pour la barre suivante ; <see cref="Failed"/> conduit au retour à
    /// Page10.
    /// </para>
    /// <para>
    /// Invariants : les trois issues réelles <see cref="BarSealed"/>,
    /// <see cref="BarUnusable"/> et <see cref="Failed"/> couvrent l’ensemble des
    /// aboutissements de la validation d’une barre et sont mutuellement
    /// exclusives. L’énumération décrit ces invariants sans les garantir.
    /// </para>
    /// </remarks>
    public enum En_BarValidationOutcome
{
    /// <summary>
    /// Sentinelle — issue non valorisée ; défaut du type et garde défensive,
    /// jamais retournée par la validation d’une barre et jamais interprétable
    /// comme une validation.
    /// </summary>
    Undetermined = 0,

    /// <summary>
    /// La barre a été validée et son plan de coupe scellé, que des zones
    /// défectueuses aient été signalées ou non.
    /// </summary>
    BarSealed = 1,

    /// <summary>
    /// Les défauts déclarés rendent la barre improductive : aucune découpe n’y
    /// est plaçable et la barre est écartée ; issue régulière du traitement, et
    /// non un échec.
    /// </summary>
    BarUnusable = 2,

    /// <summary>
    /// Le traitement a échoué et l’échec a été pris en charge : transaction
    /// annulée, exception journalisée, utilisateur notifié ; le consommateur n’a
    /// aucune notification à émettre.
    /// </summary>
    Failed = 3
}
}