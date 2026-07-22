using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Convertisseur de valeur WPF qui retourne la négation logique d'un booléen.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce convertisseur appartient à la couche D_Presentation et réside en
    /// <c>D_Presentation/Utilities/Converters/</c>. Il est instancié directement par WPF
    /// en <c>StaticResource</c> au sein d'un <c>ResourceDictionary</c> XAML, sans
    /// passage par le conteneur de dépendances applicatif (R-2.7.10).
    /// </para>
    /// <para>
    /// Objectif : permettre aux liaisons WPF d'exposer la négation d'une valeur
    /// booléenne source, typiquement pour inverser un état d'activation, de visibilité
    /// ou de validation sans devoir introduire une propriété miroir côté ViewModel.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item>Inverser la valeur d'un booléen reçu dans le sens source vers cible.</item>
    /// <item>Inverser symétriquement la valeur d'un booléen reçu dans le sens cible vers source pour les liaisons en mode <c>TwoWay</c> ou <c>OneWayToSource</c>.</item>
    /// <item>Retourner <see cref="DependencyProperty.UnsetValue"/> sur toute entrée non booléenne, conformément à l'idiome de repli WPF documenté.</item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item>Aucune logique métier ; aucune règle décisionnelle applicative propre au domaine de la découpe.</item>
    /// <item>Aucune dépendance injectée par le conteneur DI ; aucun état d'instance (R-2.7.10).</item>
    /// <item>Aucune participation aux chaînes d'appel applicatives de §4.14.9 ; aucune propagation de CallChain ni de <see cref="System.Threading.CancellationToken"/>.</item>
    /// <item>Aucune journalisation directe ; aucun traitement d'exception applicatif ; aucune levée d'exception sur entrée invalide.</item>
    /// </list>
    /// <para>
    /// Nature « UT_ » : composant de la Famille 6 (Utilities) du référentiel, sans état
    /// et sans dépendance injectée. La règle de parité interface/implémentation ne lui
    /// est pas applicable (R-2.7.6) ; le contrat externe <see cref="IValueConverter"/>
    /// du pipeline de binding WPF tient lieu de contrat structurel.
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class UT_InverseBooleanConverter : IValueConverter
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
        /// Convertit la valeur source du binding en sa négation booléenne.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : méthode invoquée par le pipeline de binding WPF lors de la
        /// propagation d'une valeur depuis la source du binding (typiquement une
        /// propriété de ViewModel) vers la cible (typiquement une propriété de
        /// dépendance d'un contrôle).
        /// </para>
        /// <para>
        /// Objectif : produire la négation logique de <paramref name="value"/>
        /// lorsque celle-ci est un booléen ; signaler une entrée invalide par la
        /// sentinelle <see cref="DependencyProperty.UnsetValue"/> sans lever
        /// d'exception, conformément à l'idiome WPF documenté.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur source à convertir. Lorsqu'il s'agit d'un <see cref="bool"/>, la
        /// méthode retourne sa négation. Toute autre valeur — y compris
        /// <see langword="null"/> ou une valeur de type non booléen — déclenche le
        /// comportement de repli.
        /// </param>
        /// <param name="targetType">
        /// Type de la propriété cible du binding. Paramètre fourni par le pipeline
        /// WPF et non exploité par la présente implémentation : la cohérence
        /// typologique est déclarée statiquement par
        /// <see cref="ValueConversionAttribute"/> sur la classe.
        /// </param>
        /// <param name="parameter">
        /// Paramètre optionnel transmissible depuis le XAML via <c>ConverterParameter</c>.
        /// Non exploité par la présente implémentation : le convertisseur ne porte
        /// aucune paramétrabilité d'exécution.
        /// </param>
        /// <param name="culture">
        /// Culture associée à la conversion. Non exploitée par la présente
        /// implémentation : l'inversion booléenne n'est porteuse d'aucune
        /// sensibilité culturelle.
        /// </param>
        /// <returns>
        /// La négation booléenne de <paramref name="value"/> si celle-ci est un
        /// <see cref="bool"/> ; <see cref="DependencyProperty.UnsetValue"/> dans
        /// tous les autres cas, indiquant au pipeline de binding qu'aucune valeur
        /// valide n'a pu être produite.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Inversion logique si la valeur reçue est un booléen ; repli WPF idiomatique sinon.
            if (value is bool flag)
            {
                return !flag;
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convertit la valeur cible du binding en sa négation booléenne, dans le sens cible vers source.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : méthode invoquée par le pipeline de binding WPF lors de la
        /// propagation d'une valeur depuis la cible du binding vers la source,
        /// uniquement applicable aux bindings en mode <c>TwoWay</c> ou
        /// <c>OneWayToSource</c>.
        /// </para>
        /// <para>
        /// Objectif : produire la négation logique de <paramref name="value"/> de
        /// manière symétrique à <see cref="Convert(object?, Type, object?, CultureInfo)"/>,
        /// préservant la propriété d'involution de l'opération de négation et
        /// garantissant la cohérence du round-trip dans les liaisons bidirectionnelles.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur cible à reconvertir vers la source. Mêmes règles de validation et
        /// même comportement de repli que pour
        /// <see cref="Convert(object?, Type, object?, CultureInfo)"/>.
        /// </param>
        /// <param name="targetType">
        /// Type de la propriété source du binding. Paramètre fourni par le pipeline
        /// WPF et non exploité par la présente implémentation.
        /// </param>
        /// <param name="parameter">
        /// Paramètre optionnel transmissible depuis le XAML via <c>ConverterParameter</c>.
        /// Non exploité par la présente implémentation.
        /// </param>
        /// <param name="culture">
        /// Culture associée à la conversion. Non exploitée par la présente implémentation.
        /// </param>
        /// <returns>
        /// La négation booléenne de <paramref name="value"/> si celle-ci est un
        /// <see cref="bool"/> ; <see cref="DependencyProperty.UnsetValue"/> dans
        /// tous les autres cas.
        /// </returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Symétrie stricte avec Convert : même règle, même repli.
            if (value is bool flag)
            {
                return !flag;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}