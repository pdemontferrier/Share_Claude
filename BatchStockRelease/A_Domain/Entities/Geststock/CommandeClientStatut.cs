using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CommandeClientStatut
{
    public int Id { get; set; }

    public int? Valeur { get; set; }

    public string? Nom { get; set; }

    public int? IdStatutAx { get; set; }

    public sbyte? Extraction { get; set; }

    public sbyte MailClient { get; set; }

    public string? MailTexte { get; set; }

    public string? Couleur { get; set; }

    public short? FinirTacheOuverte { get; set; }

    public sbyte? FinirReservation { get; set; }

    public sbyte? VerifImport { get; set; }

    public sbyte? Pointage { get; set; }

    public sbyte? RecalculMarge { get; set; }

    public int? Affichage { get; set; }

    public sbyte? ResetPlanif { get; set; }

    public int Confirmable { get; set; }

    public bool? CaPrevisionnelReste { get; set; }

    public sbyte? TabCaFacture { get; set; }

    public sbyte? TabCaFacturable { get; set; }

    public sbyte? TabCaPreviAttente { get; set; }

    public sbyte? TabCaPreviConsolide { get; set; }

    public sbyte? TabIgnore { get; set; }
}
