namespace DG244Cutting.A_Domain.Common.Enums.Business
{
    /// <summary>
    /// Motif de mise à l’écart d’une barre de production hors du circuit de production,
    /// par refus de l’opérateur ou par constat d’improductivité après prise en compte de
    /// ses défauts ; le type porte l’identité du motif, non son libellé.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Identité, non libellé : le type ne porte aucun texte. Le libellé présenté à
    /// l’opérateur provient du dictionnaire de langue, et la correspondance entre un membre
    /// et sa clé de dictionnaire relève du ViewModel consommateur.
    /// </para>
    /// <para>
    /// Convention des plages : la famille d’un motif se déduit de sa valeur. Les valeurs 1
    /// à 9 désignent un motif de qualité, applicable à toute barre. Les valeurs 10 à 19
    /// désignent un écart de stock, applicable aux seules barres issues d’une chute. La
    /// valeur 90 désigne le motif système, jamais sélectionnable. La liste proposée à
    /// l’opérateur est filtrée selon l’origine de la barre : valeurs strictement
    /// inférieures à 10 pour une barre neuve, strictement inférieures à 90 pour une barre
    /// de chute. Un membre ajouté hors de la plage de sa famille fausserait ce filtrage
    /// sans erreur visible.
    /// </para>
    /// <para>
    /// Stabilité et persistance : le motif est inscrit, sous la forme du nom du membre,
    /// dans <see cref="Entities.DIGIT_TRY.ProductionBar.RejectionReason"/> et dans le
    /// commentaire de l’entrée du journal métier. Renommer un membre ou modifier sa valeur
    /// est proscrit, sous peine d’invalider l’historique. La conversion du membre en texte
    /// relève du consommateur, et non du type.
    /// </para>
    /// <para>
    /// Absence de sentinelle : aucun membre n’est déclaré à zéro, un motif étant toujours
    /// choisi explicitement, par l’opérateur dans la liste proposée ou par l’application
    /// pour la barre-déchet. La valeur par défaut du type
    /// (<c>default(En_BarRejectionReason)</c>, soit 0) ne correspond donc à aucun membre
    /// et ne désigne aucun motif valide ; son rejet relève du point de conversion du
    /// membre en motif textuel. L’énumération décrit cet invariant sans le garantir.
    /// </para>
    /// <para>
    /// Consommateurs : le ViewModel de Page20 (<c>VM_Page20</c>) compose la liste
    /// proposée à l’opérateur, la filtre selon l’origine de la barre, résout les libellés
    /// et convertit le membre sélectionné. Le UseCase de validation de barre
    /// (<c>UC_BarValidation</c>) pose le motif système. Le motif textuel est ensuite reçu
    /// par <c>IU_BarRefusal</c> et par
    /// <see cref="Interfaces.Services.Business.IS_ProductionBar_Reject"/>, qui le
    /// transmettent sans l’interpréter.
    /// </para>
    /// <para>
    /// Ajout d’un motif : trois gestes coordonnés sont requis — la déclaration d’un membre
    /// numéroté dans la plage de sa famille, l’ajout de sa clé dans les dictionnaires de
    /// langue et l’ajout de son entrée dans la correspondance du ViewModel consommateur.
    /// </para>
    /// </remarks>
    public enum En_BarRejectionReason
    {
        // --- Famille Qualité (1 à 9) : matière présente mais inutilisable ; toute barre ---

        /// <summary>
        /// Barre voilée, tordue ou présentant un défaut de rectitude.
        /// </summary>
        BarDeformed = 1,

        /// <summary>
        /// Barre rayée, choquée ou corrodée.
        /// </summary>
        BarDamaged = 2,

        /// <summary>
        /// Défauts plus nombreux que les deux zones contournables par la saisie.
        /// </summary>
        TooManyDefects = 3,

        /// <summary>
        /// Couleur ou finition non conforme à l’article attendu ; écart relevant plus
        /// probablement d’une erreur amont (étiquetage, rangement, préparation) que d’un
        /// défaut de matière.
        /// </summary>
        ColorNotCompliant = 4,

        /// <summary>
        /// Profil ne correspondant pas à la référence attendue ; écart relevant plus
        /// probablement d’une erreur amont (étiquetage, rangement, préparation) que d’un
        /// défaut de matière.
        /// </summary>
        ReferenceNotCompliant = 5,

        // Valeurs 6 à 9 libres, réservées à la famille Qualité.

        // --- Famille Écart de stock (10 à 19) : stock applicatif inexact ; chute seule ---

        /// <summary>
        /// Chute absente de l’emplacement enregistré.
        /// </summary>
        ScrapNotFound = 10,

        /// <summary>
        /// Chute déjà consommée ailleurs.
        /// </summary>
        ScrapAlreadyConsumed = 11,

        /// <summary>
        /// Longueur réelle de la chute inférieure à la longueur enregistrée.
        /// </summary>
        ScrapLengthMismatch = 12,

        // Valeurs 13 à 19 libres, réservées à la famille Écart de stock.

        // --- Famille Système (90) : motif posé par l’application ; jamais sélectionnable ---

        /// <summary>
        /// Barre acceptée avec défauts, sur laquelle la recomposition du plan ne place aucune
        /// pièce ; motif posé par l’application, jamais proposé à l’opérateur.
        /// </summary>
        BarUnproductiveAfterDefects = 90
    }
}