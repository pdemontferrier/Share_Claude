using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Convertisseur WPF d'un booléen en <see cref="Visibility"/>, avec paramétrabilité
    /// du sens de conversion via <c>ConverterParameter</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : composant utilitaire de la couche D_Presentation, résidant en
    /// <c>D_Presentation/Utilities/Converters/</c>. Il est instancié directement
    /// en <c>StaticResource</c> côté XAML, sans passer par <c>SR_ConteneurDI</c>,
    /// conformément à la nature de la Famille 6 du référentiel (§2.7 du 0230,
    /// R-2.7.10 du 0231).
    /// </para>
    /// <para>
    /// Objectif : projeter une valeur booléenne issue d'un binding sur une valeur
    /// de <see cref="Visibility"/>, avec bascule optionnelle du sens de conversion
    /// pilotée par le <c>ConverterParameter</c>. Permet à un seul composant de
    /// couvrir les usages anciennement portés par deux converters distincts
    /// (<c>BoolToVisibilityConverter</c> et <c>InverseBoolToVisibilityConverter</c>
    /// du projet legacy).
    /// </para>
    /// <para>
    /// Responsabilités :
    /// <list type="bullet">
    /// <item>Projeter <see langword="true"/> / <see langword="false"/> vers
    /// <see cref="Visibility.Visible"/> / <see cref="Visibility.Collapsed"/>
    /// selon le mode déterminé par le paramètre (direct ou inversé).</item>
    /// <item>Mapper inversement <see cref="Visibility.Visible"/> et toute autre
    /// valeur de <see cref="Visibility"/> vers le booléen correspondant au mode,
    /// pour les bindings <c>TwoWay</c>.</item>
    /// <item>Replier sur <see cref="DependencyProperty.UnsetValue"/> en cas
    /// d'entrée invalide, dans les deux sens, conformément à la règle uniforme
    /// adoptée pour l'ensemble des UT_*Converter du projet.</item>
    /// <item>Tolérer toute valeur de paramètre inattendue par retour silencieux
    /// au mode direct, sans levée d'exception.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item>Aucune logique métier (la projection bool ↔ Visibility est une
    /// mécanique de présentation, non une règle métier).</item>
    /// <item>Aucun stockage d'état entre deux appels.</item>
    /// <item>Aucune dépendance injectée et aucun enregistrement dans
    /// <c>SR_ConteneurDI</c>.</item>
    /// <item>Aucune participation aux chaînes d'appel applicatives de §4.14.9.</item>
    /// <item>Aucune levée d'exception sur entrées invalides.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Nature « UT_ » : composant utilitaire de la Famille 6 du référentiel
    /// (§2.7 du 0230), sans état ni dépendance injectée (R-2.7.10), sans interface
    /// contractuelle en <c>A_Domain</c> (R-2.7.6). L'implémentation directe de
    /// <see cref="IValueConverter"/> est une dépendance technique au framework WPF
    /// constitutive du composant, distincte de la règle de parité du référentiel.
    /// </para>
    /// <para>
    /// Protocole du paramètre :
    /// <list type="bullet">
    /// <item>Valeur <c>"Inverse"</c> (comparaison
    /// <see cref="StringComparison.OrdinalIgnoreCase"/>) → mode inversé.</item>
    /// <item>Toute autre valeur, y compris <see langword="null"/>, chaîne vide
    /// ou jeton inattendu → mode direct.</item>
    /// <item>Aucune levée d'exception en cas de paramètre invalide ; le repli
    /// silencieux sur le mode direct est constitutif du contrat et n'est pas
    /// paramétrable.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class UT_BoolToVisibilityConverter : IValueConverter
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
        /// Convertit une valeur booléenne en valeur de <see cref="Visibility"/>
        /// selon le mode déterminé par le paramètre.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// de la source vers la cible (Source → Target).
        /// </para>
        /// <para>
        /// Objectif : projeter <see langword="true"/> / <see langword="false"/>
        /// sur <see cref="Visibility.Visible"/> / <see cref="Visibility.Collapsed"/>
        /// en mode direct, et appliquer le mapping inverse en mode inversé.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur source du binding, attendue de type <see langword="bool"/>.
        /// Si la valeur n'est pas un <see langword="bool"/> (y compris
        /// <see langword="null"/>), la méthode retourne
        /// <see cref="DependencyProperty.UnsetValue"/>.
        /// </param>
        /// <param name="targetType">
        /// Type cible attendu par la propriété de destination du binding (typiquement
        /// <see cref="Visibility"/>). Non utilisé par cette implémentation, qui
        /// répond par sa propre projection indépendamment du type cible déclaré.
        /// </param>
        /// <param name="parameter">
        /// Pilote le sens de conversion. La valeur <c>"Inverse"</c> (comparaison
        /// <see cref="StringComparison.OrdinalIgnoreCase"/>) sélectionne le mode
        /// inversé ; toute autre valeur, y compris <see langword="null"/>,
        /// sélectionne le mode direct. Aucune levée d'exception.
        /// </param>
        /// <param name="culture">
        /// Culture courante du binding. Non utilisée par cette implémentation,
        /// la conversion booléen ↔ <see cref="Visibility"/> étant indépendante
        /// de la culture.
        /// </param>
        /// <returns>
        /// En mode direct : <see cref="Visibility.Visible"/> si <paramref name="value"/>
        /// vaut <see langword="true"/>, <see cref="Visibility.Collapsed"/> sinon.
        /// En mode inversé : <see cref="Visibility.Collapsed"/> si <paramref name="value"/>
        /// vaut <see langword="true"/>, <see cref="Visibility.Visible"/> sinon.
        /// <see cref="DependencyProperty.UnsetValue"/> si <paramref name="value"/>
        /// n'est pas un <see langword="bool"/>.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Repli idiomatique WPF sur entrée invalide.
            if (value is not bool flag)
            {
                return DependencyProperty.UnsetValue;
            }

            // Lecture centralisée du protocole de ConverterParameter.
            bool isInverse = IsInverseMode(parameter);

            // Projection bool → Visibility selon le mode.
            // Mode direct  : true → Visible,   false → Collapsed.
            // Mode inversé : true → Collapsed, false → Visible.
            return (flag ^ isInverse) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convertit une valeur de <see cref="Visibility"/> en valeur booléenne
        /// selon le mode déterminé par le paramètre.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// inverse (Target → Source), typiquement pour les bindings en
        /// <c>TwoWay</c>.
        /// </para>
        /// <para>
        /// Objectif : projeter <see cref="Visibility.Visible"/> et les autres
        /// valeurs de <see cref="Visibility"/> sur le booléen correspondant au mode.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur cible du binding, attendue de type <see cref="Visibility"/>.
        /// Si la valeur n'est pas une <see cref="Visibility"/> (y compris
        /// <see langword="null"/>), la méthode retourne
        /// <see cref="DependencyProperty.UnsetValue"/>.
        /// </param>
        /// <param name="targetType">
        /// Type cible attendu par la propriété source du binding (typiquement
        /// <see langword="bool"/>). Non utilisé par cette implémentation.
        /// </param>
        /// <param name="parameter">
        /// Pilote le sens de conversion, identique au mode utilisé par
        /// <see cref="Convert"/>. La valeur <c>"Inverse"</c> (comparaison
        /// <see cref="StringComparison.OrdinalIgnoreCase"/>) sélectionne le mode
        /// inversé ; toute autre valeur sélectionne le mode direct. Aucune levée
        /// d'exception.
        /// </param>
        /// <param name="culture">
        /// Culture courante du binding. Non utilisée par cette implémentation.
        /// </param>
        /// <returns>
        /// En mode direct : <see langword="true"/> si <paramref name="value"/>
        /// vaut <see cref="Visibility.Visible"/>, <see langword="false"/> sinon.
        /// En mode inversé : <see langword="false"/> si <paramref name="value"/>
        /// vaut <see cref="Visibility.Visible"/>, <see langword="true"/> sinon.
        /// <see cref="DependencyProperty.UnsetValue"/> si <paramref name="value"/>
        /// n'est pas une <see cref="Visibility"/>.
        /// </returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Repli idiomatique WPF sur entrée invalide.
            if (value is not Visibility visibility)
            {
                return DependencyProperty.UnsetValue;
            }

            // Lecture centralisée du protocole de ConverterParameter.
            bool isInverse = IsInverseMode(parameter);

            // Projection Visibility → bool selon le mode.
            // Mode direct  : Visible → true,  autre → false.
            // Mode inversé : Visible → false, autre → true.
            bool isVisible = visibility == Visibility.Visible;
            return isVisible ^ isInverse;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Détermine si le paramètre demande le mode inversé de conversion.
        /// </summary>
        /// <remarks>
        /// Helper centralisateur de la lecture du protocole de
        /// <c>ConverterParameter</c>, partagé par <see cref="Convert"/> et
        /// <see cref="ConvertBack"/>. Voir le protocole du paramètre documenté
        /// sur la classe.
        /// </remarks>
        /// <param name="parameter">
        /// Valeur reçue par le pipeline WPF, transmise telle quelle par le binding.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si <paramref name="parameter"/> vaut
        /// <c>"Inverse"</c> (comparaison
        /// <see cref="StringComparison.OrdinalIgnoreCase"/>) ;
        /// <see langword="false"/> dans tous les autres cas, y compris
        /// <see langword="null"/>, chaîne vide ou valeur inattendue.
        /// </returns>
        private static bool IsInverseMode(object? parameter)
        {
            return parameter?.ToString()?.Equals("Inverse", StringComparison.OrdinalIgnoreCase) == true;
        }

        #endregion
    }
}