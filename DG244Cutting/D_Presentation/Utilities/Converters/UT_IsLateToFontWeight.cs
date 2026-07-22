using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Convertisseur WPF à sens unique projetant l'indicateur de retard d'une série
    /// de production (<c>IsLate</c>) sur une graisse de police
    /// (<see cref="FontWeight"/>), afin de mettre en gras les lignes des séries en
    /// retard du tableau de bord Page10.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : composant utilitaire de la couche D_Presentation, résidant en
    /// <c>D_Presentation/Utilities/Converters/</c>. Il est instancié directement
    /// en <c>StaticResource</c> côté XAML, sans passer par <c>SR_ConteneurDI</c>,
    /// conformément à la nature de la Famille 6 du référentiel (§2.7.6 du 0230,
    /// R-2.7.10 du 0231).
    /// </para>
    /// <para>
    /// Objectif : projeter <c>IsLate</c> (<see langword="bool"/> porté par
    /// <c>DTO_ProductionSeriesItem</c>) sur un <see cref="FontWeight"/> de ligne du
    /// tableau de bord Page10, afin de mettre en gras les lignes des séries en
    /// retard. Le mapping est symétrique et explicite (Bold / Normal) et
    /// <b>tolérant</b> : toute entrée illisible est repliée sur
    /// <see cref="FontWeights.Normal"/>.
    /// <list type="bullet">
    /// <item><see langword="true"/> → <see cref="FontWeights.Bold"/>.</item>
    /// <item><see langword="false"/> → <see cref="FontWeights.Normal"/>.</item>
    /// <item>défaut (entrée non booléenne, y compris <see langword="null"/>) →
    /// <see cref="FontWeights.Normal"/>.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Responsabilités :
    /// <list type="bullet">
    /// <item>Lire le booléen entrant.</item>
    /// <item>Projeter <see langword="true"/> → <see cref="FontWeights.Bold"/> et
    /// <see langword="false"/> → <see cref="FontWeights.Normal"/>.</item>
    /// <item>Replier toute autre entrée (non booléenne, y compris
    /// <see langword="null"/>) sur <see cref="FontWeights.Normal"/> (mapping
    /// tolérant).</item>
    /// <item>Répondre <see cref="DependencyProperty.UnsetValue"/> sur
    /// <see cref="ConvertBack"/>, le composant étant à sens unique.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item>Aucune logique métier (la projection <c>IsLate</c> → graisse est une
    /// mécanique de présentation pure, non une règle métier).</item>
    /// <item>Aucun stockage d'état entre deux appels.</item>
    /// <item>Aucune dépendance injectée et aucun enregistrement dans
    /// <c>SR_ConteneurDI</c>.</item>
    /// <item>Aucune participation aux chaînes d'appel applicatives de §4.14.9.</item>
    /// <item>Aucune levée d'exception, quelle que soit l'entrée.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Nature « UT_ » : composant utilitaire de la Famille 6 du référentiel
    /// (§2.7.6 du 0230), sans état ni dépendance injectée (R-2.7.10), sans interface
    /// contractuelle en <c>A_Domain</c> (hors parité, R-2.7.6). L'implémentation
    /// directe de <see cref="IValueConverter"/> est une dépendance technique au
    /// framework WPF constitutive du composant, distincte de la règle de parité du
    /// référentiel.
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(FontWeight))]
    public class UT_IsLateToFontWeight : IValueConverter
    {
        #region === Propriétés privées ===

        // A compléter

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Convertit l'indicateur de retard d'une série en graisse de police
        /// (<see cref="FontWeight"/>) de ligne.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// de la source vers la cible (Source → Target).
        /// </para>
        /// <para>
        /// Objectif : projeter <see langword="true"/> → <see cref="FontWeights.Bold"/>
        /// et <see langword="false"/> → <see cref="FontWeights.Normal"/> ; replier
        /// toute autre entrée sur <see cref="FontWeights.Normal"/> (mapping
        /// tolérant). La valeur entrante est transmise boxée par le pipeline WPF.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur source du binding, attendue de type <see langword="bool"/>
        /// (potentiellement boxée). Toute entrée non booléenne (y compris
        /// <see langword="null"/> ou type inattendu) est repliée sur
        /// <see cref="FontWeights.Normal"/>.
        /// </param>
        /// <param name="targetType">
        /// Type cible attendu par la propriété de destination du binding (typiquement
        /// <see cref="FontWeight"/>). Non utilisé par cette implémentation, qui répond
        /// par sa propre projection indépendamment du type cible déclaré.
        /// </param>
        /// <param name="parameter">
        /// Non utilisé par cette implémentation.
        /// </param>
        /// <param name="culture">
        /// Culture courante du binding. Non utilisée par cette implémentation,
        /// la projection retard → graisse étant indépendante de la culture.
        /// </param>
        /// <returns>
        /// <see cref="FontWeights.Bold"/> si et seulement si <paramref name="value"/>
        /// est le booléen <see langword="true"/> ; <see cref="FontWeights.Normal"/>
        /// dans tous les autres cas, toute entrée non booléenne étant repliée sur
        /// <see cref="FontWeights.Normal"/>. Aucune levée d'exception.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Mapping tolérant : gras si et seulement si l'entrée est le booléen
            // true ; graisse normale dans tous les autres cas (false, non booléen,
            // null). IsLate étant un bool non-nullable, le motif value is bool flag
            // suffit. FontWeights.Bold / FontWeights.Normal retournés directement.
            return value is bool flag && flag ? FontWeights.Bold : FontWeights.Normal;
        }

        /// <summary>
        /// Sens inverse non pris en charge : le composant est à sens unique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// inverse (Target → Source). La graisse n'est jamais rééditée vers un
        /// booléen de retard.
        /// </para>
        /// <para>
        /// Objectif : signaler l'absence de conversion inverse par retour de
        /// <see cref="DependencyProperty.UnsetValue"/> (pratique idiomatique des
        /// convertisseurs à sens unique).
        /// </para>
        /// </remarks>
        /// <param name="value">Valeur cible du binding. Non utilisée.</param>
        /// <param name="targetType">Type cible attendu par la propriété source du binding. Non utilisé.</param>
        /// <param name="parameter">Non utilisé.</param>
        /// <param name="culture">Culture courante du binding. Non utilisée.</param>
        /// <returns>
        /// <see cref="DependencyProperty.UnsetValue"/> systématiquement. Aucune
        /// levée d'exception.
        /// </returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Convertisseur à sens unique : aucune réécriture de la graisse vers un booléen.
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}