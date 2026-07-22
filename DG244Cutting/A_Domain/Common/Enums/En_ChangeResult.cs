namespace DG244Cutting.A_Domain.Common.Enums
{
    /// <summary>
    /// Issue fonctionnelle d’un scénario de mutation, restituée à la couche de
    /// présentation et mappée par le ViewModel vers un comportement d’UI.
    /// </summary>
    /// <remarks>
    /// Type transverse retournable par tout UseCase consommé par un ViewModel,
    /// selon le schéma « issue fonctionnelle restituée à la présentation ». La
    /// valeur par défaut du type est <see cref="Unchanged"/>
    /// (<c>default(En_ChangeResult) == Unchanged</c>), défaut conservateur.
    /// </remarks>
    public enum En_ChangeResult
    {
        /// <summary>
        /// Aucune modification effective de la persistance : le consommateur de
        /// présentation peut conserver son état.
        /// </summary>
        Unchanged = 0,

        /// <summary>
        /// La persistance a été effectivement modifiée : le consommateur de
        /// présentation doit rafraîchir son état.
        /// </summary>
        Changed = 1
    }
}