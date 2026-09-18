using CommonResources.Utilities;
using CommonResources.Settings;

namespace BatchStockRelease.D_Presentation.Utilities
{
    /// <summary>
    /// <b>UT_CommonRessources</b>
    /// <para>Classe utilitaire servant de passerelle unique entre le projet <b>BatchStockRelease</b> et le projet <b>CommonRessources</b>.</para>
    /// <para>Elle regroupe toutes les méthodes et propriétés exposées par <b>CommonRessources</b> afin de limiter les dépendances directes depuis les autres couches (notamment UseCases et Services).</para>
    /// <para>Cette classe ne contient aucune logique métier propre : elle délègue simplement l’exécution aux composants du projet <b>CommonRessources</b>.</para>
    /// </summary>
    public static class UT_CommonRessources
    {
        #region === EncryptionHelper ===

        /// <summary>
        /// Crypte une chaîne de caractères en utilisant l’algorithme MD5 défini dans <see cref="EncryptionHelper"/>.
        /// </summary>
        /// <param name="chaine">Chaîne à crypter.</param>
        /// <returns>Résultat du hachage MD5 sous forme de chaîne hexadécimale.</returns>
        public static string Crypte_md5(string chaine)
        {
            return EncryptionHelper.Crypte_md5(chaine);
        }

        #endregion

        #region === CR_VuesSettings ===

        /// <summary>
        /// Retourne une instance de <see cref="Uri"/> correspondant à la référence de vue indiquée.
        /// </summary>
        /// <param name="referenceVue">Nom ou identifiant de la vue (par exemple : "Page10").</param>
        /// <returns>Objet <see cref="Uri"/> construit à partir des paramètres de <see cref="CR_VuesSettings"/>.</returns>
        public static Uri GetVueUri(string? referenceVue)
        {
            string refVue = referenceVue ?? string.Empty;
            return CR_VuesSettings.GetVueUri(refVue);
        }

        #endregion
    }
}