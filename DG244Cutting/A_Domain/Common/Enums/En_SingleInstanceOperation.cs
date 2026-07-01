namespace DG244Cutting.A_Domain.Common.Enums
{
    /// <summary>
    /// Opération multiplexée exposée par le service de garde d’unicité d’instance
    /// applicative sur la session Windows courante.
    /// </summary>
    public enum En_SingleInstanceOperation
    {
        /// <summary>
        /// Tentative d’acquisition de l’unicité d’instance au démarrage applicatif.
        /// </summary>
        Acquire = 0,

        /// <summary>
        /// Libération de l’unicité d’instance à la clôture applicative.
        /// </summary>
        Release = 1
    }
}