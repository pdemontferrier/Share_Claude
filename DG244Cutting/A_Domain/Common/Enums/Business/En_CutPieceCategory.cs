namespace DG244Cutting.A_Domain.Common.Enums.Business
{
    /// <summary>
    /// Catégorie de pièce du périmètre de découpe piloté, retenue par l’opérateur pour
    /// restreindre la découpe d’une série à une seule catégorie ; le type porte
    /// l’identité de la catégorie, non son libellé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Origine des valeurs : les trois membres correspondent à des valeurs de
    /// <see cref="Entities.DIGIT_TRY.ArticleCategoryMapping.Description"/>, restreintes aux
    /// catégories effectivement découpées sur la machine pilotée. Le référentiel compte dix
    /// catégories distinctes ; les sept autres ne relèvent pas du périmètre piloté et ne
    /// produiraient aucune pièce.
    /// </para>
    /// <para>
    /// Absence de sentinelle : aucun membre n’est déclaré à zéro. La valeur par défaut du type
    /// (<c>default(En_CutPieceCategory)</c>, soit 0) ne correspond à aucun membre et ne désigne
    /// aucune catégorie valide. L’absence de catégorie retenue est portée par la nullité du type
    /// chez les consommateurs, jamais par un membre de l’énumération. L’énumération décrit cet
    /// invariant sans le garantir.
    /// </para>
    /// <para>
    /// Stabilité de la numérotation : la valeur numérique d’un membre est définitive. Renommer un
    /// membre ou modifier sa valeur est proscrit : la sélection est mémorisée dans l’état de
    /// sélection métier, et une renumérotation invaliderait toute valeur déjà mémorisée ou
    /// journalisée.
    /// </para>
    /// <para>
    /// Absence de sémantique d’ordre : la valeur numérique n’encode aucun rang d’affichage ni
    /// aucune priorité de traitement ; elle est une identité, non un classement. Le tri de la
    /// liste proposée à l’opérateur relève du ViewModel qui compose cette liste. L’énumération
    /// voisine <see cref="En_ProductionSeriesStatus"/> documente la convention inverse, sa valeur
    /// entière encodant un rang d’affichage ; cette convention ne se transpose pas ici.
    /// </para>
    /// <para>
    /// Dualité des conversions : deux résolutions distinctes coexistent et ne doivent jamais être
    /// confondues. Le libellé affiché à l’opérateur est issu du dictionnaire de langue et suit la
    /// langue de l’interface ; la valeur de filtrage est la chaîne exacte portée en base par
    /// <see cref="Entities.DIGIT_TRY.vw_ProductionCutPiece_Full.ACMDescription"/>, qui ne suit
    /// jamais la langue. Le type ne porte ni l’une ni l’autre : la conversion du membre en texte
    /// relève du consommateur, et non du type. La confusion entre les deux produit un filtre
    /// silencieusement vide, sans erreur visible.
    /// </para>
    /// <para>
    /// Consommateurs : <c>SE_UseCase</c> porte la sélection en propriété de type nullable.
    /// <c>VM_Page10</c> compose la liste proposée à l’opérateur, en résout les libellés et écrit
    /// la sélection. <c>VM_Page20</c> lit la sélection et la transmet en paramètre.
    /// <c>UC_BarOptimization</c> convertit le membre reçu en valeur de filtrage
    /// <c>ACMDescription</c>.
    /// </para>
    /// <para>
    /// Ajout d’un membre : trois gestes coordonnés sont requis — la déclaration d’un membre à
    /// valeur nouvelle, jamais utilisée auparavant ; l’ajout de sa clé dans les dictionnaires de
    /// langue ; l’ajout de son entrée dans la correspondance membre vers valeur de filtrage portée
    /// par le consommateur. Un quatrième geste, hors périmètre du type, conditionne l’utilité de
    /// l’ajout : l’élargissement correspondant du périmètre de lecture en base, faute de quoi la
    /// nouvelle catégorie ne produirait aucune pièce.
    /// </para>
    /// </remarks>
    public enum En_CutPieceCategory
    {
        /// <summary>
        /// Pièces constituant le cadre fixe du châssis ; désigne la catégorie « Dormant » du
        /// référentiel.
        /// </summary>
        Frame = 1,

        /// <summary>
        /// Pièces constituant la partie ouvrante du châssis ; désigne la catégorie « Ouvrant » du
        /// référentiel.
        /// </summary>
        Sash = 2,

        /// <summary>
        /// Pièces d’accessoire du périmètre effectivement découpé ; désigne la catégorie
        /// « Accessoire » du référentiel.
        /// </summary>
        Accessory = 3
    }
}