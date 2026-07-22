using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Converter WPF unidirectionnel transformant un <see cref="DateTime"/> en chaîne formatée
    /// pour affichage, avec format paramétrable via <c>ConverterParameter</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte. Composant utilitaire de présentation (famille UT_) consommé par le pipeline
    /// de binding WPF au travers de l'interface <see cref="IValueConverter"/>. Instancié en
    /// <c>StaticResource</c> dans un <c>ResourceDictionary</c> XAML, sans enregistrement au
    /// conteneur d'injection de dépendances (R-2.7.10).
    /// </para>
    /// <para>
    /// Objectif. Convertir une valeur <see cref="DateTime"/> en représentation textuelle
    /// formatée selon un format texte fourni par le consommateur, avec repli sur un format par
    /// défaut historiquement hérité du legacy (<c>yyyy/MM/dd HH:mm</c>) lorsque le paramètre
    /// est absent, vide ou invalide.
    /// </para>
    /// <para>
    /// Responsabilités. Formatage d'une date en chaîne d'affichage. Lecture et résolution
    /// tolérante du paramètre de format. Transmission de la culture courante du binding au
    /// formatage pour respect de la localisation des noms de mois, jours et séparateurs.
    /// </para>
    /// <para>
    /// Non-responsabilités. Le composant ne porte aucune logique métier, ne consulte aucun
    /// Setting, n'accède à aucune source de données, et ne participe à aucune chaîne d'appel
    /// applicative au sens de §4.14.9 du référentiel 0230. Il ne convertit pas de chaîne vers
    /// <see cref="DateTime"/> : voir caractère unidirectionnel ci-dessous.
    /// </para>
    /// <para>
    /// Nature « UT_ ». Classe utilitaire sans état et sans dépendance injectée conformément à
    /// R-2.7.10. Aucune interface contractuelle dans A_Domain n'est requise (R-2.7.6). La seule
    /// donnée nommée est la constante de compilation <c>DefaultFormat</c> qui ne porte pas
    /// d'état d'instance.
    /// </para>
    /// <para>
    /// Caractère unidirectionnel. Le converter est unidirectionnel par contrat : seule la
    /// méthode <see cref="Convert"/> est fonctionnelle. La méthode <see cref="ConvertBack"/>
    /// lève systématiquement <see cref="NotSupportedException"/> avec un message explicite. Il
    /// est attendu des XAML consommateurs qu'ils déclarent explicitement <c>Mode=OneWay</c> sur
    /// les bindings qui consomment ce converter, par hygiène et pour éviter toute levée
    /// inopinée d'exception en cas de configuration ambiguë du binding.
    /// </para>
    /// <para>
    /// Protocole du paramètre de format. Le format effectif est résolu selon le protocole
    /// suivant. Si <c>ConverterParameter</c> est <c>null</c>, vide, ou ne contient que des
    /// caractères blancs, le format par défaut <c>yyyy/MM/dd HH:mm</c> est appliqué — valeur
    /// strictement identique à celle du legacy pour compatibilité ascendante. Si une chaîne
    /// non blanche est fournie, elle est utilisée comme chaîne de format passée à
    /// <see cref="DateTime.ToString(string, IFormatProvider)"/>. Si cette chaîne s'avère
    /// invalide en tant que format de date (levée de <see cref="FormatException"/>), le
    /// converter replie silencieusement sur le format par défaut sans propager l'exception au
    /// pipeline de binding. La culture transmise par le pipeline de binding au paramètre
    /// <c>culture</c> de l'interface est intégralement répercutée au formatage, garantissant
    /// la sensibilité culturelle de la sortie.
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class UT_DateFormatConverter : IValueConverter
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Format de date par défaut appliqué lorsque <c>ConverterParameter</c> est absent,
        /// vide, blanc, ou invalide. Valeur strictement identique au format en dur du legacy
        /// <c>DateFormatConverter</c> pour compatibilité ascendante.
        /// </summary>
        private const string DefaultFormat = "yyyy/MM/dd HH:mm";

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Convertit une valeur <see cref="DateTime"/> en chaîne formatée selon le paramètre
        /// de format fourni, avec repli sur le format par défaut lorsque le paramètre est
        /// absent, vide, blanc ou invalide.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Méthode appelée par le pipeline de binding WPF lors de chaque évaluation de la
        /// valeur source vers la cible. La culture transmise par le pipeline est intégralement
        /// répercutée au formatage. Toute entrée qui n'est pas un <see cref="DateTime"/> — y
        /// compris <c>null</c> — produit <see cref="DependencyProperty.UnsetValue"/>,
        /// comportement idiomatique WPF qui laisse le binding non résolu et indique au pipeline
        /// qu'aucune conversion utile n'a pu être effectuée.
        /// </para>
        /// </remarks>
        /// <param name="value">Valeur source du binding. Attendue <see cref="DateTime"/>.</param>
        /// <param name="targetType">Type cible du binding. Non consulté ; le converter produit
        /// toujours du <see cref="string"/>.</param>
        /// <param name="parameter">Chaîne de format optionnelle transmise via
        /// <c>ConverterParameter</c>. <c>null</c>, vide, blanc ou invalide → repli sur
        /// <c>yyyy/MM/dd HH:mm</c>.</param>
        /// <param name="culture">Culture du binding, intégralement répercutée à
        /// <see cref="DateTime.ToString(string, IFormatProvider)"/>.</param>
        /// <returns>Chaîne formatée si <paramref name="value"/> est un <see cref="DateTime"/> ;
        /// <see cref="DependencyProperty.UnsetValue"/> sinon.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Pattern matching unique sur DateTime ; couvre nativement le cas DateTime? boxé
            // avec valeur. Toute autre entrée (null compris) bascule vers UnsetValue.
            if (value is DateTime date)
            {
                string format = ResolveFormat(parameter);

                try
                {
                    return date.ToString(format, culture);
                }
                catch (FormatException)
                {
                    // Repli silencieux sur le format par défaut en cas de chaîne de format
                    // invalide. DefaultFormat est par construction toujours valide, aucune
                    // levée récursive n'est possible.
                    return date.ToString(DefaultFormat, culture);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Opération inverse non supportée par contrat. Lève systématiquement
        /// <see cref="NotSupportedException"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le converter est unidirectionnel par conception : le formatage d'une date à des fins
        /// d'affichage n'admet pas d'opération inverse cohérente. L'édition d'une date par
        /// l'utilisateur passe par les composants dédiés (<c>DatePicker</c>, <c>TextBox</c> avec
        /// parseur explicite, <c>MaskedTextBox</c>), jamais par le retour inverse d'un converter
        /// de format. Le contrat de la méthode est donc une levée systématique d'exception, non
        /// une lacune d'implémentation.
        /// </para>
        /// </remarks>
        /// <param name="value">Non consulté.</param>
        /// <param name="targetType">Non consulté.</param>
        /// <param name="parameter">Non consulté.</param>
        /// <param name="culture">Non consulté.</param>
        /// <returns>N'effectue jamais de retour ; lève systématiquement
        /// <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">Levée systématiquement avec message
        /// explicite. <see cref="UT_DateFormatConverter"/> est un converter unidirectionnel
        /// par contrat.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException(
                "UT_DateFormatConverter is a one-way converter. ConvertBack is not supported.");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Résout la chaîne de format effective à appliquer, à partir du paramètre brut transmis
        /// par le pipeline de binding.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Lecture tolérante du paramètre via <see cref="object.ToString"/>. Si la chaîne est
        /// <c>null</c>, vide ou ne contient que des caractères blancs, retourne
        /// <see cref="DefaultFormat"/>. Sinon, retourne la chaîne fournie telle quelle. La
        /// validation de la chaîne en tant que format de date n'est pas effectuée ici ; elle
        /// est portée par le <c>try / catch (FormatException)</c> de <see cref="Convert"/> qui
        /// replie sur <see cref="DefaultFormat"/> en cas d'invalidité.
        /// </para>
        /// </remarks>
        /// <param name="parameter">Paramètre brut transmis par le pipeline de binding via
        /// <c>ConverterParameter</c>.</param>
        /// <returns>Chaîne de format effective à appliquer.</returns>
        private static string ResolveFormat(object? parameter)
        {
            string? requested = parameter?.ToString();
            return string.IsNullOrWhiteSpace(requested) ? DefaultFormat : requested;
        }

        #endregion
    }
}