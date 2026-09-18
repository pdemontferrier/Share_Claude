using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class DecoupeBarre
{
    public int Id { get; set; }

    public int IdDecoupeLot { get; set; }

    public int IdArticleInterne { get; set; }

    public int IdStock { get; set; }

    public decimal? LongueurBarre { get; set; }

    public decimal? LongueurChuteMini { get; set; }

    public int Categorie1 { get; set; }

    public int Categorie2 { get; set; }

    public int Categorie3 { get; set; }

    public string? Categorie4 { get; set; }

    public int OrdreTri { get; set; }

    public double? DernierPrixMm { get; set; }

    public DateTime? DernierPrixDateMaj { get; set; }

    public string? ApproOrigine { get; set; }

    public string? ApproCodeBarre { get; set; }

    public bool ApproAllocation { get; set; }

    public bool ApproRupture { get; set; }

    public int ApproZonePriorite { get; set; }

    public string? ApproZoneDesignation { get; set; }

    public int ApproAdressePriorite { get; set; }

    public string? ApproAdresseDesignation { get; set; }

    public string? ApproConteneur { get; set; }

    public string? ApproTypeConteneur { get; set; }

    public string? ApproTypeContenant { get; set; }

    public int ApproEmplacement { get; set; }

    public string? ApproEmplacementDesignation { get; set; }

    public int ApproIdChariot { get; set; }

    public string? ApproChariotDesignation { get; set; }

    public bool ApproSortieFaite { get; set; }

    public bool ApproSortieForce { get; set; }

    public bool ApproSortieSupp { get; set; }

    public bool ApproInactif { get; set; }

    public DateTime? ApproDateDebut { get; set; }

    public DateTime? ApproDateFin { get; set; }

    public string? ApproUtilisateurPoste { get; set; }

    public int ApproUtilisateurErp { get; set; }

    public string? ApproPosteMachineId { get; set; }

    public string? ApproPosteMachineIp { get; set; }

    public int DecoupeNombre { get; set; }

    public decimal? DecoupeLongueurReste { get; set; }

    public int DecoupeLongueurChuteFinale { get; set; }

    public bool DecoupeGestionChutes { get; set; }

    public string? DecoupeTypeReste { get; set; }

    public string DecoupeEmpSc { get; set; } = null!;

    public int DecoupeIdEmpSc { get; set; }

    public string? DecoupeCodeBarreChute { get; set; }

    public bool DecoupeFaite { get; set; }

    public string? DecoupeCommentaires { get; set; }

    public DateTime? DecoupeDateDebut { get; set; }

    public DateTime? DecoupeDateFin { get; set; }

    public string? DecoupeUtilisateurPoste { get; set; }

    public int DecoupeUtilisateurErp { get; set; }

    public string? DecoupePosteMachineId { get; set; }

    public string? DecoupePosteMachineIp { get; set; }
}
