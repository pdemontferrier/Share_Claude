using DG244Cutting.A_Domain.Common.Enums;

namespace DG244Cutting.A_Domain.DTOs.Business
{
    /// <summary>
    /// Objet de transport du résultat d'une invocation du moteur d'optimisation de
    /// découpe : issue, contenant retenu, contenu ordonné et qualification du résidu.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter, sans perte et sans ambiguïté, le résultat d'une
    /// invocation du moteur d'optimisation de découpe, calcul pur et sans effet de
    /// bord dont le contrat est exposé par <c>IS_CuttingOptimizer</c> et
    /// l'implémentation portée par <c>SR_CuttingOptimizer</c>. Le même type est
    /// retourné par ses deux méthodes, <c>Optimize</c> et <c>OptimizeWithDefects</c>,
    /// et consommé respectivement par les UseCases <c>UC_BarOptimization</c> et
    /// <c>UC_BarValidation</c>, qui en tirent la création ou la mise à jour de la barre
    /// de production. Le type matérialise ainsi la frontière entre le calcul et
    /// l'orchestration qui écrit en base ; il ne franchit pas la frontière de
    /// présentation. La matière transportée relève de quatre natures : un
    /// discriminant d'issue, l'identification du contenant retenu (origine, chute
    /// source, longueur), le contenu sous forme de liste ordonnée d'identifiants de
    /// découpes, et la qualification du résidu (longueur physique, chute réutilisable
    /// ou déchet).</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// <para>Invariants : seule <see cref="Outcome"/> gouverne la lecture du
    /// résultat ; sur toute issue autre que
    /// <see cref="En_CuttingOptimizationOutcome.Success"/>, le contenant et le contenu
    /// sont sans objet, <see cref="PieceIds"/> est vide et les valeurs 0, false et
    /// null portées par les autres propriétés ne revêtent aucune signification. Sur
    /// une issue <see cref="En_CuttingOptimizationOutcome.Success"/>,
    /// <see cref="PieceIds"/> compte au moins un élément. Le rang d'un identifiant
    /// dans <see cref="PieceIds"/>, compté à partir de 1, est sa position de coupe
    /// dans la barre. Le nombre de découpes de la barre de production
    /// (<c>ProductionBar.CutPieceCount</c>) se déduit du cardinal de
    /// <see cref="PieceIds"/> et n'est pas porté par le type. Le respect de ces
    /// invariants incombe au producteur ; le type n'en vérifie aucun.</para>
    /// </remarks>
    public class DTO_CuttingOptimizationResult
    {
        /// <summary>Issue de l'invocation du moteur d'optimisation, seule propriété gouvernant la lecture du résultat ; la valeur par défaut du type, <see cref="En_CuttingOptimizationOutcome.Undetermined"/>, signale une issue non valorisée et n'est jamais interprétée comme un succès.</summary>
        public En_CuttingOptimizationOutcome Outcome { get; set; }

        /// <summary>Origine du contenant retenu : true = barre neuve, false = chute du stock ; sur la variante <c>OptimizeWithDefects</c>, recopie de l'origine du contenant reçu en entrée ; hors succès, sans objet et valant false.</summary>
        public bool IsNewBar { get; set; }

        /// <summary>Identifiant de la chute source du contenant retenu, null pour une barre neuve ; sur la variante <c>OptimizeWithDefects</c>, recopie de la chute source du contenant reçu en entrée ; hors succès, sans objet et valant null.</summary>
        public int? IdSourceScrap { get; set; }

        /// <summary>Longueur retenue du contenant, en millimètres ; sur la variante <c>OptimizeWithDefects</c>, recopie de la longueur du contenant reçu en entrée ; hors succès, sans objet et valant 0.</summary>
        public int BarLength { get; set; }

        /// <summary>Identifiants des découpes retenues, dans l'ordre de coupe, le rang de chaque identifiant (compté à partir de 1) valant position de coupe dans la barre ; sur un succès, au moins un élément ; hors succès, liste vide.</summary>
        public List<int> PieceIds { get; set; } = [];

        /// <summary>Longueur physique du résidu de la barre après découpe, en millimètres, la valeur zéro étant admise sur un succès ; hors succès, sans objet et valant 0.</summary>
        public int ResidueLength { get; set; }

        /// <summary>Qualification du résidu : true = déchet, false = chute réutilisable ; hors succès, sans objet et valant false.</summary>
        public bool ResidueIsScrap { get; set; }
    }
}