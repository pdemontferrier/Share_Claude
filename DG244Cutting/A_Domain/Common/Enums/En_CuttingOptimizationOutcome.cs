namespace DG244Cutting.A_Domain.Common.Enums
{
    /// <summary>
    /// Issue d’une invocation du moteur d’optimisation de découpe, exprimée par un
    /// discriminant à valeurs mutuellement exclusives.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Undetermined"/> est la valeur par défaut du type
    /// (<c>default(En_CuttingOptimizationOutcome) == Undetermined</c>) : sentinelle
    /// et garde défensive, elle signale une issue non valorisée par le moteur et
    /// n’est jamais produite par un calcul abouti. Un résultat dont l’issue n’a pas
    /// été valorisée ne doit jamais être interprété comme un succès.
    /// </para>
    /// <para>
    /// Le type est transporté par la propriété <c>Outcome</c> de
    /// <see cref="DG244Cutting.A_Domain.DTOs.Business.DTO_CuttingOptimizationResult"/>.
    /// Il est valorisé par les deux méthodes du moteur d’optimisation de découpe,
    /// dont le contrat est exposé par
    /// <see cref="DG244Cutting.A_Domain.Interfaces.Services.Business.IS_CuttingOptimizer"/>
    /// et l’implémentation portée par
    /// <see cref="DG244Cutting.B_UseCases.Services.Business.SR_CuttingOptimizer"/>.
    /// </para>
    /// <para>
    /// Invariant de cohérence : sur toute issue autre que <see cref="Success"/>, le
    /// contenant et le contenu du résultat porteur sont sans objet. L’énumération
    /// décrit cet invariant sans le garantir.
    /// </para>
    /// </remarks>
    public enum En_CuttingOptimizationOutcome
    {
        /// <summary>
        /// Sentinelle — issue non valorisée par le moteur ; défaut du type et garde
        /// défensive, jamais produite par un calcul abouti.
        /// </summary>
        Undetermined = 0,

        /// <summary>
        /// Un contenant a été retenu et garni d’au moins une découpe.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Aucune découpe ne reste à réaliser pour l’article ; situation nominale,
        /// et non un échec.
        /// </summary>
        EmptyPool = 2,

        /// <summary>
        /// Aucune découpe ne peut être placée, même sur une barre neuve de pleine
        /// longueur ; traduit une incohérence des données d’entrée.
        /// </summary>
        DataAnomaly = 3,

        /// <summary>
        /// Les défauts déclarés rendent la barre fournie improductive : aucun segment
        /// exploitable ne reçoit de découpe.
        /// </summary>
        NoUsableSegment = 4
    }
}