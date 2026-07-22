using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class DecoupeDetail
{
    public int Id { get; set; }

    public int IdImport { get; set; }

    public int IdPrimaire { get; set; }

    public int IdCommandeClient { get; set; }

    public int IdDecoupeLot { get; set; }

    public int IdArticleInterne { get; set; }

    public int IdDecoupeBarre { get; set; }

    public int IdArticleCompose { get; set; }

    public string? NumProjet { get; set; }

    public string? NomProjet { get; set; }

    public string? Structure { get; set; }

    public string? Position { get; set; }

    public ulong? NumLigne { get; set; }

    public int IndiceDecoupe { get; set; }

    public string? Reference { get; set; }

    public string? Couleur { get; set; }

    public string? Designation { get; set; }

    public decimal? LongueurBarre { get; set; }

    public decimal? LongueurChuteMini { get; set; }

    public int Quantite { get; set; }

    public decimal? LongueurOptim { get; set; }

    public decimal? LongueurDecoupe { get; set; }

    public decimal? Inclinaison1 { get; set; }

    public decimal? Pivot1 { get; set; }

    public decimal? Pivot2 { get; set; }

    public decimal? Inclinaison2 { get; set; }

    public string? ReferenceVue { get; set; }

    public string? RefCommentaires { get; set; }

    public string? Commentaires { get; set; }

    public bool Inactif { get; set; }

    public bool ValidationLigne { get; set; }

    public string? TypeHistorique { get; set; }

    public string? Source { get; set; }

    public string? MessageElumatec { get; set; }

    public int Categorie1 { get; set; }

    public int Categorie2 { get; set; }

    public int Categorie3 { get; set; }

    public string? Categorie4 { get; set; }

    public int? OrdreTri { get; set; }

    public string? AdvUtilisateurPoste { get; set; }

    public int AdvUtilisateurErp { get; set; }

    public string? AdvPosteMachineId { get; set; }

    public string? AdvPosteMachineIp { get; set; }

    public bool ApproComposeInactif { get; set; }

    public bool ApproComposant { get; set; }

    public bool ApproCompoChute { get; set; }

    public bool ApproCompoNeuf { get; set; }

    public bool ApproOptimBarreChute { get; set; }

    public bool ApproOptimBarreNeuve { get; set; }

    public int DecoupeBarreIndex { get; set; }

    public decimal? DecoupeLongueurReste { get; set; }

    public bool DecoupeAFaire { get; set; }

    public bool DecoupeFaite { get; set; }

    public string? DecoupeCommentaires { get; set; }

    public DateTime? DecoupeDateDebut { get; set; }

    public DateTime? DecoupeDateFin { get; set; }

    public string? DecoupeUtilisateurPoste { get; set; }

    public int DecoupeUtilisateurErp { get; set; }

    public string? DecoupePosteMachineId { get; set; }

    public string? DecoupePosteMachineIp { get; set; }
}
