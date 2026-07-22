using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Convertisseur WPF à sens unique projetant le code couleur entier du jour
    /// de fin de production (<c>ProductionEndDay</c>) sur un <see cref="Brush"/>
    /// de fond de ligne du tableau de bord Page10.
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
    /// Objectif : projeter la valeur <c>ProductionEndDay</c> (<see langword="short"/>
    /// porté par <c>DTO_ProductionSeriesItem</c>) sur un <see cref="Brush"/> de
    /// fond, afin de colorer les lignes de série du tableau de bord Page10 et de
    /// permettre à l'opérateur d'identifier d'un coup d'œil la catégorie temporelle
    /// de chaque série. Le mapping est <b>tolérant</b> : seules les sept valeurs 0 à 6
    /// sont mappées explicitement ; toute autre valeur est un cas nominal (jour sans
    /// couleur d'étiquette) rendu par <see cref="Brushes.Transparent"/>.
    /// </para>
    /// <para>
    /// Mapping code → couleur (couleurs figées en dur, sans référence à une palette
    /// de ressources externe) :
    /// <list type="bullet">
    /// <item><c>0</c> → Violet.</item>
    /// <item><c>1</c> → Bleu.</item>
    /// <item><c>2</c> → Orange.</item>
    /// <item><c>3</c> → Jaune.</item>
    /// <item><c>4</c> → Rouge.</item>
    /// <item><c>5</c> → Rose.</item>
    /// <item><c>6</c> → Ocre.</item>
    /// <item>défaut (toute autre valeur : 7, hors plage, ou entrée non
    /// convertible en <see langword="short"/>) → <see cref="Brushes.Transparent"/>.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Responsabilités :
    /// <list type="bullet">
    /// <item>Lire de façon robuste la valeur entrante (potentiellement boxée par le
    /// pipeline WPF) et la ramener à un <see langword="short"/> lorsque cela est
    /// possible.</item>
    /// <item>Projeter les sept codes 0 à 6 sur leur <see cref="Brush"/> respectif.</item>
    /// <item>Replier toute autre valeur, ou toute entrée non convertible en
    /// <see langword="short"/> (y compris <see langword="null"/> ou type inattendu),
    /// sur <see cref="Brushes.Transparent"/>, par cohérence avec le mapping tolérant.</item>
    /// <item>Répondre <see cref="DependencyProperty.UnsetValue"/> sur
    /// <see cref="ConvertBack"/>, le composant étant à sens unique.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item>Aucune logique métier (la projection code → couleur est une mécanique
    /// de présentation pure, non une règle métier).</item>
    /// <item>Aucun stockage d'état entre deux appels.</item>
    /// <item>Aucune dépendance injectée et aucun enregistrement dans
    /// <c>SR_ConteneurDI</c>.</item>
    /// <item>Aucune participation aux chaînes d'appel applicatives de §4.14.9.</item>
    /// <item>Aucune levée d'exception, quelle que soit l'entrée.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Sens unique : le fond coloré n'est jamais réédité vers un code entier ;
    /// <see cref="ConvertBack"/> retourne systématiquement
    /// <see cref="DependencyProperty.UnsetValue"/> (pratique idiomatique des
    /// convertisseurs à sens unique).
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
    [ValueConversion(typeof(short), typeof(Brush))]
    public class UT_ProductionEndDayToBrush : IValueConverter
    {
        #region === Propriétés privées ===

        // Brushes figés (Freeze) et pré-instanciés une fois pour toutes, afin
        // d'éviter toute allocation par appel de Convert et d'autoriser un partage
        // cross-thread par le pipeline WPF. Latitude tranchée au codage : champs
        // static readonly gelés plutôt que construction à chaque appel.

        /// <summary>Fond du code 0 (Violet).</summary>
        private static readonly Brush _brushViolet = CreateFrozen(Color.FromRgb(133, 14, 88));

        /// <summary>Fond du code 1 (Bleu).</summary>
        private static readonly Brush _brushBleu = CreateFrozen(Colors.Blue);

        /// <summary>Fond du code 2 (Orange).</summary>
        private static readonly Brush _brushOrange = CreateFrozen(Color.FromRgb(255, 103, 20));

        /// <summary>Fond du code 3 (Jaune).</summary>
        private static readonly Brush _brushJaune = CreateFrozen(Color.FromRgb(227, 179, 12));

        /// <summary>Fond du code 4 (Rouge).</summary>
        private static readonly Brush _brushRouge = CreateFrozen(Colors.Red);

        /// <summary>Fond du code 5 (Rose).</summary>
        private static readonly Brush _brushRose = CreateFrozen(Color.FromRgb(255, 174, 201));

        /// <summary>Fond du code 6 (Ocre).</summary>
        private static readonly Brush _brushOcre = CreateFrozen(Color.FromRgb(242, 147, 36));

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Convertit le code couleur du jour de fin de production en
        /// <see cref="Brush"/> de fond de ligne.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// de la source vers la cible (Source → Target).
        /// </para>
        /// <para>
        /// Objectif : projeter les sept codes 0 à 6 sur leur <see cref="Brush"/>
        /// respectif ; replier toute autre valeur, ou toute entrée non convertible
        /// en <see langword="short"/>, sur <see cref="Brushes.Transparent"/>
        /// (mapping tolérant). La valeur entrante est lue de façon robuste, le
        /// pipeline WPF pouvant transmettre le <see langword="short"/> boxé.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur source du binding, attendue de type <see langword="short"/>
        /// (potentiellement boxée). Toute valeur non convertible en
        /// <see langword="short"/> (y compris <see langword="null"/>, type inattendu
        /// ou valeur non numérique) est repliée sur <see cref="Brushes.Transparent"/>.
        /// </param>
        /// <param name="targetType">
        /// Type cible attendu par la propriété de destination du binding (typiquement
        /// <see cref="Brush"/>). Non utilisé par cette implémentation, qui répond par
        /// sa propre projection indépendamment du type cible déclaré.
        /// </param>
        /// <param name="parameter">
        /// Non utilisé par cette implémentation.
        /// </param>
        /// <param name="culture">
        /// Culture courante du binding. Non utilisée par cette implémentation,
        /// la projection code → couleur étant indépendante de la culture.
        /// </param>
        /// <returns>
        /// Le <see cref="Brush"/> correspondant au code 0 à 6, ou
        /// <see cref="Brushes.Transparent"/> pour toute autre valeur ou toute entrée
        /// non convertible en <see langword="short"/>. Aucune levée d'exception.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Lecture robuste : le pipeline WPF peut transmettre la valeur boxée.
            // Une entrée non convertible en short (null, type inattendu, valeur non
            // numérique) est traitée comme un cas nominal, replié sur Transparent.
            if (!TryReadShort(value, out short code))
            {
                return Brushes.Transparent;
            }

            // Mapping tolérant : seuls 0 à 6 sont mappés ; tout le reste → Transparent.
            // Latitude tranchée au codage : switch expression plutôt que dictionnaire.
            return code switch
            {
                0 => _brushViolet,
                1 => _brushBleu,
                2 => _brushOrange,
                3 => _brushJaune,
                4 => _brushRouge,
                5 => _brushRose,
                6 => _brushOcre,
                _ => Brushes.Transparent,
            };
        }

        /// <summary>
        /// Sens inverse non pris en charge : le composant est à sens unique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// inverse (Target → Source). Le fond coloré n'est jamais réédité vers un
        /// code entier.
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
            // Convertisseur à sens unique : aucune réécriture du fond vers un code.
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Lit de façon tolérante une valeur boxée et tente de la ramener à un
        /// <see langword="short"/>.
        /// </summary>
        /// <remarks>
        /// Helper de lecture robuste de l'entrée de <see cref="Convert"/> : le
        /// pipeline WPF transmet <c>ProductionEndDay</c> boxé, et la valeur peut
        /// remonter sous une forme entière voisine. La conversion échoue
        /// silencieusement (retour <see langword="false"/>) sans levée d'exception
        /// pour toute entrée non entière ou hors plage <see langword="short"/>.
        /// </remarks>
        /// <param name="value">Valeur reçue par le pipeline WPF, transmise telle quelle par le binding.</param>
        /// <param name="code">Le <see langword="short"/> extrait en cas de succès ; <c>0</c> sinon.</param>
        /// <returns>
        /// <see langword="true"/> si <paramref name="value"/> a pu être ramenée à un
        /// <see langword="short"/> ; <see langword="false"/> pour toute entrée non
        /// convertible (y compris <see langword="null"/>).
        /// </returns>
        private static bool TryReadShort(object? value, out short code)
        {
            switch (value)
            {
                case short s:
                    code = s;
                    return true;

                // Conversion tolérante depuis un entier voisin, sous réserve de
                // tenir dans la plage short ; hors plage → cas nominal Transparent.
                case int i when i >= short.MinValue && i <= short.MaxValue:
                    code = (short)i;
                    return true;

                default:
                    code = 0;
                    return false;
            }
        }

        /// <summary>
        /// Construit un <see cref="SolidColorBrush"/> figé (<see cref="Freezable.Freeze"/>)
        /// pour la couleur fournie.
        /// </summary>
        /// <remarks>
        /// Le gel autorise le partage cross-thread et évite toute réévaluation par
        /// le pipeline WPF. Utilisé à l'initialisation des champs static readonly.
        /// </remarks>
        /// <param name="color">Couleur du pinceau à construire.</param>
        /// <returns>Un <see cref="SolidColorBrush"/> figé.</returns>
        private static Brush CreateFrozen(Color color)
        {
            SolidColorBrush brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        #endregion
    }
}