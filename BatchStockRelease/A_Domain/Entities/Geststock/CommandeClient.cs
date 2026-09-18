using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CommandeClient
{
    public int Id { get; set; }

    public string NumProjet { get; set; } = null!;

    public string? NumAx { get; set; }

    public string NomProjet { get; set; } = null!;

    public short IdConcession { get; set; }

    public double PrixHt { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateOnly? DateFabrication { get; set; }

    public double RemiseTotale { get; set; }

    public double CoutMateriel { get; set; }

    public double Marge { get; set; }

    public string? Divers { get; set; }

    public string? DetailStructure { get; set; }

    public bool ListePicking { get; set; }

    public int Statut { get; set; }

    public int IdTournee { get; set; }

    public double PrixPrestation { get; set; }

    public double PrixPartCom { get; set; }

    public double PrixTransport { get; set; }

    public int? TypeCommande { get; set; }

    public DateOnly? DateConfirmation { get; set; }

    public DateOnly? DateSignature { get; set; }

    public sbyte? ExportCarnetGarantie { get; set; }

    public int? OrigineSav { get; set; }

    public DateOnly? LivraisonSouhaitee { get; set; }

    public sbyte PrePlanif { get; set; }

    public string? Modele { get; set; }

    public string? Couleur { get; set; }

    public bool ImportCover { get; set; }

    public sbyte? ImportColisage { get; set; }

    public int? IdCommercial { get; set; }

    public int? IdTypeSecondaire { get; set; }

    public int? IdTypeTertiaire { get; set; }

    public sbyte? Reserve { get; set; }

    public DateOnly? DateFacturation { get; set; }

    public int? IdCommandeParent { get; set; }

    public sbyte? Financement { get; set; }

    public sbyte? CommandeUbc { get; set; }

    public DateOnly? DatePremiereConfirme { get; set; }

    public DateOnly? DatePremierDossierFab { get; set; }

    public int? CmdParent { get; set; }

    public DateOnly? DateValidation { get; set; }

    public int? DelaiSupp { get; set; }

    public int Altitude { get; set; }

    public int NumServeur { get; set; }

    public string? DesignationProjet { get; set; }

    public int? EcheanceSemaineFabrication { get; set; }

    public int? IdCouleurRal { get; set; }

    public string? IdCouleurFinition { get; set; }

    public string? IdCouleurRalFinition { get; set; }

    public int NumLancement1 { get; set; }

    public string? LienDossier1 { get; set; }

    public string? FichierConso { get; set; }

    public int NumLancement2 { get; set; }

    public string? LienDossier2 { get; set; }

    public string? FichierDebitComplet { get; set; }

    public bool Updated { get; set; }

    public bool ImportDebitComplet { get; set; }

    public bool ImportControle { get; set; }

    public bool ImportValide { get; set; }

    public bool Annulation { get; set; }

    public bool AffectationLot { get; set; }

    public int IdDecoupeLot { get; set; }

    public string? StatutDecoupeLot { get; set; }

    public string? ModeleCategorie { get; set; }

    public string? ModeleType { get; set; }

    public DateOnly? DateAcces { get; set; }

    public sbyte ServeurArchive { get; set; }
}
