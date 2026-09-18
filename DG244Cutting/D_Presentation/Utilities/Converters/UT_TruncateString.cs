using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Convertisseur WPF unidirectionnel tronquant une chaîne à l'affichage à une longueur
    /// maximale paramétrable via <c>ConverterParameter</c>, ellipse comprise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte. Composant utilitaire de présentation (famille UT_, Famille 6, §2.7 du 0230)
    /// consommé par le pipeline de binding WPF au travers de l'interface
    /// <see cref="IValueConverter"/>. Il est instancié directement en <c>StaticResource</c>
    /// dans un <c>ResourceDictionary</c> XAML, sans enregistrement dans <c>SR_ConteneurDI</c>,
    /// conformément à la nature de la Famille 6 (R-2.7.10 du 0231).
    /// </para>
    /// <para>
    /// Objectif. Projeter une chaîne source vers une chaîne tronquée à une longueur maximale
    /// fournie par le consommateur, pour affichage de textes potentiellement longs (descriptions,
    /// libellés) dans un espace contraint. La donnée source (DTO_/VM_) n'est jamais altérée : la
    /// troncature est une préoccupation de présentation pure, appliquée à la volée par le binding.
    /// </para>
    /// <para>
    /// Responsabilités.
    /// <list type="bullet">
    /// <item>Tronquer une chaîne dont la longueur excède la longueur maximale, en substituant au
    /// débordement l'ellipse <c>U+2026</c>, de sorte que le résultat ait une longueur strictement
    /// égale à la longueur maximale.</item>
    /// <item>Retourner la chaîne source inchangée lorsque sa longueur est inférieure ou égale à la
    /// longueur maximale.</item>
    /// <item>Lire et résoudre de façon tolérante la longueur maximale depuis
    /// <c>ConverterParameter</c>, avec repli sur la chaîne source inchangée en cas de paramètre
    /// absent, non convertible en entier, ou non positif.</item>
    /// <item>Replier sur <see cref="DependencyProperty.UnsetValue"/> lorsque la valeur source
    /// n'est pas une chaîne, conformément à la règle uniforme adoptée pour l'ensemble des
    /// UT_*Converter du projet.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Non-responsabilités.
    /// <list type="bullet">
    /// <item>Aucune logique métier (la troncature d'affichage est une mécanique de présentation,
    /// non une règle métier).</item>
    /// <item>Aucun stockage d'état entre deux appels.</item>
    /// <item>Aucune dépendance injectée et aucun enregistrement dans <c>SR_ConteneurDI</c>.</item>
    /// <item>Aucune participation aux chaînes d'appel applicatives de §4.14.9 du 0230.</item>
    /// <item>Aucune levée d'exception par <see cref="Convert"/> sur entrées invalides.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Nature « UT_ ». Composant utilitaire de la Famille 6 (§2.7 du 0230), sans état ni
    /// dépendance injectée (R-2.7.10), sans interface contractuelle en <c>A_Domain</c> (R-2.7.6,
    /// la règle de parité de §2.7.4 ne s'appliquant pas à la famille UT_). L'implémentation directe
    /// de <see cref="IValueConverter"/> est une dépendance technique au framework WPF constitutive
    /// du composant, distincte de la règle de parité. Instanciation en <c>StaticResource</c> sans
    /// injection de dépendances.
    /// </para>
    /// <para>
    /// Caractère unidirectionnel. Le convertisseur est unidirectionnel par contrat : seule la
    /// méthode <see cref="Convert"/> est fonctionnelle. La méthode <see cref="ConvertBack"/> lève
    /// systématiquement <see cref="NotSupportedException"/> avec un message explicite, une chaîne
    /// tronquée ne pouvant restituer son originale. Il est attendu des XAML consommateurs qu'ils
    /// déclarent explicitement <c>Mode=OneWay</c> sur les bindings qui consomment ce convertisseur,
    /// par hygiène et pour éviter toute levée inopinée d'exception en cas de configuration ambiguë
    /// du binding.
    /// </para>
    /// <para>
    /// Protocole du paramètre. La longueur maximale est lue de façon tolérante depuis
    /// <c>ConverterParameter</c> et convertie en entier. Si le paramètre est <c>null</c>, non
    /// convertible en entier, ou non positif (inférieur ou égal à zéro), aucune longueur exploitable
    /// n'est résolue : la chaîne source est alors retournée inchangée, sans troncature. La culture
    /// courante du binding n'est pas consultée, la troncature par nombre de caractères étant
    /// indépendante de la culture.
    /// </para>
    /// <para>
    /// Protocole de troncature. L'ellipse est le caractère unique <c>U+2026</c> (« … », longueur 1),
    /// et non trois points ASCII. Elle est comptée dans la longueur maximale : la coupe s'effectue à
    /// <c>(max - 1)</c> caractères, de sorte que le résultat, ellipse comprise, ait une longueur
    /// exactement égale à <c>max</c>. Le cas de bord <c>max == 1</c> produit la seule ellipse, soit
    /// la chaîne « … » de longueur 1, comportement assumé cohérent avec l'invariant « résultat de
    /// longueur inférieure ou égale à max ». Le comptage s'effectue par <see langword="char"/>
    /// simple : les paires de substitution et les clusters de graphèmes ne sont pas traités,
    /// approximation suffisante pour la troncature de descriptions et documentée ici au titre de
    /// l'exhaustivité.
    /// </para>
    /// <para>
    /// Invariants.
    /// <list type="number">
    /// <item>Une chaîne de longueur inférieure ou égale à <c>max</c> est retournée telle quelle.</item>
    /// <item>Une chaîne de longueur supérieure à <c>max</c> est ramenée à une longueur égale à
    /// <c>max</c>, ellipse comprise.</item>
    /// <item>Aucune exception n'est levée par <see cref="Convert"/>, quelles que soient
    /// <c>value</c> et <c>parameter</c>.</item>
    /// <item>La donnée source n'est jamais mutée.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(string), typeof(string))]
    public class UT_TruncateString : IValueConverter
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Ellipse unique <c>U+2026</c> substituée au débordement lors d'une troncature. De
        /// longueur 1, elle est comptée dans la longueur maximale du résultat.
        /// </summary>
        private const string Ellipsis = "\u2026";

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Convertit une chaîne source en chaîne tronquée à la longueur maximale fournie par le
        /// paramètre, ellipse comprise, avec repli sur la chaîne inchangée lorsque le paramètre
        /// est absent, invalide ou non positif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Méthode appelée par le pipeline de binding WPF lors de chaque évaluation de la valeur
        /// source vers la cible. Toute entrée qui n'est pas une chaîne — y compris <c>null</c> —
        /// produit <see cref="DependencyProperty.UnsetValue"/>, comportement idiomatique WPF qui
        /// laisse le binding non résolu et indique au pipeline qu'aucune conversion utile n'a pu
        /// être effectuée ; le paramètre n'est alors pas consulté.
        /// </para>
        /// <para>
        /// Lorsque la valeur est une chaîne, la longueur maximale est résolue de façon tolérante
        /// depuis le paramètre. En l'absence de longueur exploitable (paramètre absent, non
        /// convertible en entier, ou non positif), la chaîne est retournée inchangée. Sinon, si la
        /// longueur de la chaîne excède la longueur maximale, la chaîne est coupée à
        /// <c>(max - 1)</c> caractères et l'ellipse <c>U+2026</c> est concaténée, produisant un
        /// résultat de longueur exactement égale à <c>max</c> ; dans le cas contraire, la chaîne
        /// est retournée inchangée.
        /// </para>
        /// </remarks>
        /// <param name="value">Valeur source du binding. Attendue <see langword="string"/> ; toute
        /// autre valeur, <c>null</c> compris, produit
        /// <see cref="DependencyProperty.UnsetValue"/>.</param>
        /// <param name="targetType">Type cible du binding. Non consulté ; le convertisseur produit
        /// toujours du <see langword="string"/> (ou <see cref="DependencyProperty.UnsetValue"/>).</param>
        /// <param name="parameter">Longueur maximale optionnelle transmise via
        /// <c>ConverterParameter</c>. <c>null</c>, non convertible en entier, ou non positif → repli
        /// sur la chaîne source inchangée.</param>
        /// <param name="culture">Culture du binding. Non consultée, la troncature par nombre de
        /// caractères étant indépendante de la culture.</param>
        /// <returns>La chaîne source inchangée si sa longueur est inférieure ou égale à la longueur
        /// maximale ou si aucune longueur exploitable n'est résolue ; la chaîne tronquée à une
        /// longueur égale à <c>max</c> (ellipse <c>U+2026</c> comprise) sinon ;
        /// <see cref="DependencyProperty.UnsetValue"/> si <paramref name="value"/> n'est pas une
        /// chaîne.</returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Repli idiomatique WPF sur entrée non-string (null compris) ; le paramètre n'est
            // alors pas consulté.
            if (value is not string text)
            {
                return DependencyProperty.UnsetValue;
            }

            // Lecture tolérante de la longueur maximale. Absence de longueur exploitable →
            // chaîne source retournée inchangée, sans troncature.
            if (!TryResolveMaxLength(parameter, out int max))
            {
                return text;
            }

            // Chaîne déjà conforme à la contrainte : aucune troncature, source inchangée.
            if (text.Length <= max)
            {
                return text;
            }

            // Troncature : coupe à (max - 1) caractères puis concaténation de l'ellipse U+2026,
            // pour un résultat de longueur exactement égale à max. Le cas de bord max == 1
            // produit la seule ellipse (coupe à 0 caractère). La source n'est pas mutée :
            // Substring produit une nouvelle chaîne.
            return text.Substring(0, max - 1) + Ellipsis;
        }

        /// <summary>
        /// Opération inverse non supportée par contrat. Lève systématiquement
        /// <see cref="NotSupportedException"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le convertisseur est unidirectionnel par conception : une chaîne tronquée pour affichage
        /// n'admet pas d'opération inverse cohérente, l'ellipse effaçant définitivement le
        /// débordement. L'édition d'un texte par l'utilisateur passe par les composants dédiés,
        /// jamais par le retour inverse d'un convertisseur de troncature. Le contrat de la méthode
        /// est donc une levée systématique d'exception, non une lacune d'implémentation.
        /// </para>
        /// </remarks>
        /// <param name="value">Non consulté.</param>
        /// <param name="targetType">Non consulté.</param>
        /// <param name="parameter">Non consulté.</param>
        /// <param name="culture">Non consulté.</param>
        /// <returns>N'effectue jamais de retour ; lève systématiquement
        /// <see cref="NotSupportedException"/>.</returns>
        /// <exception cref="NotSupportedException">Levée systématiquement avec message explicite.
        /// <see cref="UT_TruncateString"/> est un convertisseur unidirectionnel par contrat.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException(
                "UT_TruncateString is a one-way converter. ConvertBack is not supported.");
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Résout, de façon tolérante, la longueur maximale exploitable à partir du paramètre brut
        /// transmis par le pipeline de binding.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Lecture tolérante du paramètre via <see cref="object.ToString"/> puis conversion en
        /// entier par <see cref="int.TryParse(string, out int)"/>. Le résultat est considéré comme
        /// exploitable si et seulement si la conversion réussit et que l'entier obtenu est
        /// strictement positif. En cas d'absence du paramètre, de non-conversion, ou de valeur
        /// non positive, la méthode signale l'absence de longueur exploitable ; l'appelant
        /// <see cref="Convert"/> replie alors sur la chaîne source inchangée.
        /// </para>
        /// </remarks>
        /// <param name="parameter">Paramètre brut transmis par le pipeline de binding via
        /// <c>ConverterParameter</c>.</param>
        /// <param name="maxLength">Longueur maximale exploitable résolue lorsque la méthode
        /// retourne <see langword="true"/> ; valeur indéterminée sinon.</param>
        /// <returns><see langword="true"/> si une longueur maximale strictement positive a pu être
        /// résolue ; <see langword="false"/> en cas d'absence, de non-conversion ou de valeur non
        /// positive.</returns>
        private static bool TryResolveMaxLength(object? parameter, out int maxLength)
        {
            maxLength = 0;

            string? requested = parameter?.ToString();
            if (!int.TryParse(requested, out int parsed) || parsed <= 0)
            {
                return false;
            }

            maxLength = parsed;
            return true;
        }

        #endregion
    }
}