namespace DG244Cutting.A_Domain.Common.Enums
{
    /// <summary>
    /// Résultat de la procédure de fermeture d’application.
    /// </summary>
    public enum En_CloseResult
    {
        /// <summary>
        /// La fermeture a été annulée par l’utilisateur.
        /// </summary>
        Cancelled = 0,

        /// <summary>
        /// La fermeture a été validée après déconnexion de la session.
        /// </summary>
        Closed = 1,

        /// <summary>
        /// La fermeture est forcée, sans confirmation.
        /// </summary>
        ForceClosed = 2
    }
}