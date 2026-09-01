using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DG244Cutting.A_Domain.DTOs.Business;

namespace DG244Cutting.D_Presentation.Utilities.Converters
{
    /// <summary>
    /// Convertisseur WPF à sens unique projetant l'état d'une découpe de production
    /// (<c>DTO_VwProductionCutPieceFull_P11</c>) sur un <see cref="Brush"/> de couleur
    /// de police, afin de marquer visuellement les découpes réalisées et les découpes
    /// refusées du tableau des découpes de la Page11.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : composant utilitaire de la couche D_Presentation, résidant en
    /// <c>D_Presentation/Utilities/Converters/</c>. Il est instancié directement en
    /// <c>StaticResource</c> côté XAML, sans passer par <c>SR_ConteneurDI</c>,
    /// conformément à la nature de la Famille 6 du référentiel (§2.7.2 du 0230,
    /// R-2.7.10 du 0231). Le cinquième onglet de la Page11 présente les découpes
    /// d'une série de production. La découpe est l'unité élémentaire du travail
    /// d'atelier et l'objet même de l'application : chaque châssis d'une commande se
    /// décompose en ouvrants, eux-mêmes en pièces de profilé à découper. Une étape
    /// d'optimisation affecte chaque pièce à une barre et lui attribue une position
    /// de coupe ; l'opérateur exécute ensuite la coupe sur la machine. Une découpe
    /// peut être refusée à l'exécution - défaut constaté sur la matière, erreur de
    /// positionnement. Elle retourne alors au vivier des pièces à optimiser : son
    /// affectation à une barre est défaite, sa position effacée, et l'indicateur de
    /// refus est positionné. Cet indicateur est remis à zéro dès qu'une nouvelle
    /// affectation lui est donnée par une optimisation ultérieure : il traduit l'état
    /// courant de la pièce et non son historique.
    /// </para>
    /// <para>
    /// Objectif : rendre le pinceau de couleur de police traduisant l'état d'une
    /// découpe, par lecture et départage des deux indicateurs
    /// <c>PCPIsCut</c> (découpe réalisée) et <c>PCPIsCutRefused</c> (découpe refusée)
    /// en un point unique. La ligne entière est transmise au convertisseur par liaison
    /// sans chemin, les deux indicateurs devant être lus conjointement pour
    /// déterminer une couleur unique. Le marquage répond à un besoin d'atelier : le
    /// cinquième onglet est le plus fin de la page et celui qui compte le plus de
    /// lignes, chaque châssis produisant plusieurs découpes. Sur un tel volume, le
    /// repérage immédiat des pièces réalisées et des pièces en attente de reprise
    /// vaut mieux qu'une lecture case par case. Les états correspondants étant par
    /// ailleurs affichés en clair dans des colonnes dédiées, la couleur ne porte
    /// aucune information exclusive : elle accélère la lecture.
    /// </para>
    /// <para>
    /// Mapping état vers couleur, dans l'ordre d'évaluation strict :
    /// <list type="number">
    /// <item><c>PCPIsCut</c> à <see langword="true"/> : vert (code d'origine
    /// #59C64A), <c>PCPIsCutRefused</c> n'étant pas examiné.</item>
    /// <item><c>PCPIsCut</c> à <see langword="false"/> et <c>PCPIsCutRefused</c> à
    /// <see langword="true"/> : rouge (code d'origine #FF3E3E).</item>
    /// <item>les deux indicateurs à <see langword="false"/> :
    /// <see cref="DependencyProperty.UnsetValue"/>, l'élément conservant la couleur
    /// appliquée au chargement par le service de stylisation.</item>
    /// <item>entrée <see langword="null"/> ou d'un type inattendu :
    /// <see cref="DependencyProperty.UnsetValue"/>, strictement identique au cas
    /// précédent.</item>
    /// </list>
    /// </para>
    /// <para>
    /// La priorité de la réalisation sur le refus est absolue et évaluée en premier.
    /// <c>PCPIsCutRefused</c> traduisant un état courant réversible, remis à zéro à
    /// chaque nouvelle affectation d'optimisation, la cooccurrence des deux
    /// indicateurs est possible dans le modèle : cette priorité constitue donc un
    /// départage fonctionnel réel, et non une garde défensive. Une découpe refusée
    /// puis recoupée avec succès porte la couleur de la réalisation. Ce point marque
    /// une divergence délibérée avec le composant homologue de l'onglet des barres,
    /// <c>UT_BarStateToBrush_P11</c>, qui évalue le refus en premier : le refus d'une
    /// barre est terminal - il marque l'enregistrement comme logiquement supprimé -,
    /// celui d'une découpe est réversible. La lecture conjointe des deux composants
    /// ne doit donc pas conclure à une incohérence.
    /// </para>
    /// <para>
    /// Responsabilités :
    /// <list type="bullet">
    /// <item>Filtrer l'entrée sur le type <c>DTO_VwProductionCutPieceFull_P11</c>.</item>
    /// <item>Départager <c>PCPIsCut</c> et <c>PCPIsCutRefused</c> selon la priorité
    /// absolue de la réalisation, et rendre le pinceau correspondant.</item>
    /// <item>Replier le cas neutre, l'entrée <see langword="null"/> et l'entrée
    /// d'un type inattendu sur <see cref="DependencyProperty.UnsetValue"/>.</item>
    /// <item>Répondre <see cref="DependencyProperty.UnsetValue"/> sur
    /// <see cref="ConvertBack"/>, le composant étant à sens unique.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Non-responsabilités :
    /// <list type="bullet">
    /// <item>Aucune logique métier (la projection état vers couleur est une
    /// mécanique de présentation pure, non une règle métier).</item>
    /// <item>Aucun stockage d'état entre deux appels.</item>
    /// <item>Aucune dépendance injectée et aucun enregistrement dans
    /// <c>SR_ConteneurDI</c>.</item>
    /// <item>Aucune participation aux chaînes d'appel applicatives de §4.14.9.</item>
    /// <item>Aucune levée d'exception, quelle que soit l'entrée.</item>
    /// <item>Aucun marquage des colonnes de cases à cocher du gabarit : la couleur
    /// de police n'a pas d'effet visible sur une case à cocher, dont la coche est
    /// dessinée par le gabarit du contrôle. Le marquage d'une ligne est donc partiel
    /// par construction : douze colonnes de texte portent la liaison, les quatre
    /// colonnes de cases à cocher n'en portent pas.</item>
    /// <item>Aucun marquage par l'italique. L'onglet des barres porte un troisième
    /// marquage, l'italique sur les barres en rupture de stock, assuré par
    /// <c>UT_IsBarOutOfStockToFontStyle</c>. Le document fonctionnel n'en prévoit pas
    /// d'équivalent pour les découpes, bien que le DTO porte l'indicateur
    /// <c>PCPIsBarOutOfStock</c>.</item>
    /// </list>
    /// </para>
    /// <para>Usage type :</para>
    /// <code>
    /// &lt;generic:Page_Generic.Resources&gt;
    ///     &lt;converters:UT_CutPieceStateToBrush_P11 x:Key="CutPieceStateToBrush_P11" /&gt;
    /// &lt;/generic:Page_Generic.Resources&gt;
    ///
    /// &lt;TextBlock Text="{Binding PCPProfileName}"
    ///            Foreground="{Binding Converter={StaticResource CutPieceStateToBrush_P11}}" /&gt;
    /// </code>
    /// <para>
    /// La liaison de la couleur de police est une liaison sans chemin : l'absence de
    /// <c>Path</c> substitue le contexte de données de la ligne à l'une de ses
    /// propriétés dans l'argument de conversion, de sorte que les deux indicateurs
    /// soient lus conjointement en un point unique. Cette forme a pour conséquence
    /// assumée que le composant est couplé au type de DTO des découpes de la Page11,
    /// et porte à ce titre le suffixe de destination <c>_P11</c>.
    /// </para>
    /// <para>
    /// Sens unique : la couleur de police n'est jamais rééditée vers un état de
    /// découpe ; <see cref="ConvertBack"/> retourne systématiquement
    /// <see cref="DependencyProperty.UnsetValue"/>, suivant la convention des
    /// convertisseurs de mise en forme conditionnelle du projet.
    /// </para>
    /// <para>
    /// Nature « UT_ » : composant utilitaire de la Famille 6 du référentiel
    /// (§2.7.2 du 0230), sans état ni dépendance injectée (R-2.7.10), sans interface
    /// contractuelle en <c>A_Domain</c> (hors parité, R-2.7.6, R-4.14.5), ne
    /// participant à aucune des chaînes d'appel applicatives de §4.14.9. Les deux
    /// champs <c>static readonly</c> portant les pinceaux figés sont des constantes
    /// immuables et ne constituent pas un état au sens de R-2.7.10. L'implémentation
    /// directe de <see cref="IValueConverter"/> est une dépendance technique au
    /// framework WPF constitutive du composant, distincte de la règle de parité du
    /// référentiel.
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(DTO_VwProductionCutPieceFull_P11), typeof(Brush))]
    public class UT_CutPieceStateToBrush_P11 : IValueConverter
    {
        #region === Propriétés privées ===

        // Brushes figés (Freeze) et pré-instanciés une fois pour toutes, afin
        // d'éviter toute allocation par appel de Convert et d'autoriser un partage
        // cross-thread par le pipeline WPF, sur le tableau qui compte le plus de
        // lignes de la page. Constantes immuables, sans caractère d'état au sens de
        // R-2.7.10. Les teintes sont exprimées en hexadécimal pour conserver la trace
        // directe des codes couleur arbitrés en amont. Elles sont identiques à celles
        // de UT_BarStateToBrush_P11, par cohérence visuelle entre les deux onglets.
        // Les champs sont déclarés dans l'ordre d'évaluation de Convert.

        /// <summary>Couleur de police d'une découpe réalisée (vert, code d'origine #59C64A).</summary>
        private static readonly Brush _brushGreen = CreateFrozen(Color.FromRgb(0x59, 0xC6, 0x4A));

        /// <summary>Couleur de police d'une découpe refusée (rouge, code d'origine #FF3E3E).</summary>
        private static readonly Brush _brushRed = CreateFrozen(Color.FromRgb(0xFF, 0x3E, 0x3E));

        #endregion

        #region === Dépendances privées ===

        // A compléter

        #endregion

        #region === Constructeur ===

        // A compléter

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Convertit l'état d'une découpe de production en <see cref="Brush"/> de
        /// couleur de police de ligne.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// de la source vers la cible (Source vers Target). La ligne entière est
        /// transmise par liaison sans chemin, les deux indicateurs devant être lus
        /// conjointement.
        /// </para>
        /// <para>
        /// Objectif : départager <c>PCPIsCut</c> et <c>PCPIsCutRefused</c> selon la
        /// priorité absolue de la réalisation et rendre le pinceau figé correspondant ;
        /// replier le cas neutre, l'entrée <see langword="null"/> et l'entrée d'un type
        /// inattendu sur <see cref="DependencyProperty.UnsetValue"/>, de sorte que
        /// l'élément conserve la couleur appliquée au chargement par le service de
        /// stylisation. Un repli sur une couleur explicite l'écraserait ; un repli sur
        /// un pinceau transparent rendrait le texte invisible en cas de liaison mal
        /// formée, transformant une erreur de câblage silencieuse en perte de données
        /// à l'écran.
        /// </para>
        /// <para>
        /// La priorité de la réalisation est un départage fonctionnel réel : les deux
        /// indicateurs peuvent coexister dans le modèle, <c>PCPIsCutRefused</c>
        /// traduisant un état courant réversible et non un historique.
        /// </para>
        /// <para>
        /// L'entrée est filtrée par motif direct, sans helper de lecture robuste.
        /// Le helper <c>TryReadShort</c> porté par
        /// <c>UT_ProductionEndDayToBrush</c> n'est pas transposé ici : il y est
        /// motivé par le fait que le pipeline WPF transmet un <see langword="short"/>
        /// boxé susceptible de remonter sous une forme entière voisine, alors que le
        /// présent composant lit <c>PCPIsCut</c> et <c>PCPIsCutRefused</c>, deux
        /// <see langword="bool"/> non-nullables, par déréférencement d'une référence
        /// d'objet. Aucune lecture tolérante intermédiaire n'est requise.
        /// </para>
        /// <para>
        /// Fonction pure : aucune mutation, aucun état conservé entre deux appels,
        /// aucune allocation par appel. Deux appels sur la même entrée rendent la
        /// même référence de pinceau.
        /// </para>
        /// </remarks>
        /// <param name="value">
        /// Valeur source du binding, attendue de type
        /// <c>DTO_VwProductionCutPieceFull_P11</c> et transmise par liaison sans
        /// chemin. Toute autre valeur, y compris <see langword="null"/>, est admise
        /// sans erreur et repliée sur <see cref="DependencyProperty.UnsetValue"/>.
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
        /// Culture courante du binding. Non utilisée par cette implémentation, la
        /// projection état vers couleur étant indépendante de la culture.
        /// </param>
        /// <returns>
        /// Le pinceau vert figé si la découpe est réalisée, indépendamment de son
        /// indicateur de refus ; le pinceau rouge figé si elle est refusée sans être
        /// réalisée ; <see cref="DependencyProperty.UnsetValue"/> si aucun des deux
        /// indicateurs n'est positionné, ainsi que pour toute entrée
        /// <see langword="null"/> ou d'un type inattendu. Aucun autre retour n'est
        /// possible et aucune exception n'est levée.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Filtrage de motif direct : PCPIsCut et PCPIsCutRefused étant des bool
            // non-nullables du DTO, aucun helper de lecture robuste n'est requis.
            // Entrée null ou d'un type inattendu : cas nominal replié sur UnsetValue,
            // au même titre que le cas neutre ci-dessous.
            if (value is not DTO_VwProductionCutPieceFull_P11 cutPiece)
            {
                return DependencyProperty.UnsetValue;
            }

            // Priorité absolue de la réalisation, évaluée en premier : départage
            // fonctionnel réel, l'indicateur de refus traduisant un état courant
            // réversible et non un historique. Une découpe refusée puis recoupée avec
            // succès relève de cette branche.
            if (cutPiece.PCPIsCut)
            {
                return _brushGreen;
            }

            if (cutPiece.PCPIsCutRefused)
            {
                return _brushRed;
            }

            // Cas neutre : découpe encore au vivier, ou affectée à une barre mais pas
            // encore coupée. L'élément conserve la couleur appliquée au chargement
            // par le service de stylisation.
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Sens inverse non pris en charge : le composant est à sens unique.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée par le pipeline WPF de binding lors de la propagation
        /// inverse (Target vers Source). La couleur de police n'est jamais rééditée
        /// vers un état de découpe.
        /// </para>
        /// <para>
        /// Objectif : signaler l'absence de conversion inverse par retour de
        /// <see cref="DependencyProperty.UnsetValue"/>, suivant la convention des
        /// convertisseurs de mise en forme conditionnelle du projet, à laquelle le
        /// présent composant se rattache.
        /// </para>
        /// </remarks>
        /// <param name="value">Valeur cible du binding. Non utilisée.</param>
        /// <param name="targetType">Type cible attendu par la propriété source du binding. Non utilisé.</param>
        /// <param name="parameter">Non utilisé.</param>
        /// <param name="culture">Culture courante du binding. Non utilisée.</param>
        /// <returns>
        /// <see cref="DependencyProperty.UnsetValue"/> systématiquement. Aucune levée
        /// d'exception, en particulier aucune <c>NotSupportedException</c>.
        /// </returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Convertisseur à sens unique : aucune réécriture de la couleur de police
            // vers un état de découpe.
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Construit un <see cref="SolidColorBrush"/> figé (<see cref="Freezable.Freeze"/>)
        /// pour la couleur fournie.
        /// </summary>
        /// <remarks>
        /// Le gel autorise le partage cross-thread entre toutes les lignes du tableau
        /// et évite toute réévaluation par le pipeline WPF. Utilisé à l'initialisation
        /// des champs <c>static readonly</c>. La méthode est dupliquée depuis
        /// <c>UT_ProductionEndDayToBrush</c> et <c>UT_BarStateToBrush_P11</c>, dont
        /// elle constitue la troisième occurrence au projet, et n'est pas factorisée :
        /// une factorisation supposerait un composant partagé qui n'existe pas au
        /// projet, dont la création sortirait du périmètre du fil au sens de §1.4.4
        /// du 0230.
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