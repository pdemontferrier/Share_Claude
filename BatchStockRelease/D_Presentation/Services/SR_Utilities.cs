using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Utilities;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// <b>SR_Utilities</b>
    /// <para>Service applicatif fournissant des méthodes utilitaires génériques, sans dépendance directe au projet <b>CommonRessources</b>.</para>
    /// <para>Les appels sont redirigés via <see cref="UT_CommonRessources"/> afin d’assurer un découplage complet entre la couche métier et les ressources communes.</para>
    /// </summary>
    public class SR_Utilities : IS_Utilities
    {
        private readonly string ServiceName;

        /// <summary>
        /// Initialise une nouvelle instance du service <see cref="SR_Utilities"/>.
        /// </summary>
        public SR_Utilities()
        {
            ServiceName = GetType().Name;
        }

        /// <summary>
        /// Crypte une chaîne de caractères en utilisant la méthode <see cref="UT_CommonRessources.Crypte_md5(string)"/>.
        /// </summary>
        /// <param name="chaine">Chaîne à crypter.</param>
        /// <returns>Chaîne hachée en MD5.</returns>
        public string GetCrypte_md5(string chaine)
        {
            return UT_CommonRessources.Crypte_md5(chaine);
        }

        /// <summary>
        /// Retourne une URI correspondant à la référence de vue passée en paramètre.
        /// </summary>
        /// <param name="referenceVue">Nom ou identifiant de la vue (ex : "Page10").</param>
        /// <returns><see cref="Uri"/> représentant le chemin de la vue correspondante.</returns>
        public Uri GetVueUri(string? referenceVue)
        {
            return UT_CommonRessources.GetVueUri(referenceVue);
        }
    }
}