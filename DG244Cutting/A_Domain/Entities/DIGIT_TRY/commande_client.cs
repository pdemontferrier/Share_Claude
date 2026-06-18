using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class commande_client
{
    public int id { get; set; }

    public string num_projet { get; set; } = null!;

    public string? Num_Ax { get; set; }

    public string nom_projet { get; set; } = null!;

    public short id_concession { get; set; }

    public double prix_ht { get; set; }

    public DateTime? date_creation { get; set; }

    public DateOnly? date_fabrication { get; set; }

    public double remise_totale { get; set; }

    public double cout_materiel { get; set; }

    public double marge { get; set; }

    public string? divers { get; set; }

    public string? detail_structure { get; set; }

    public bool liste_picking { get; set; }

    public int statut { get; set; }

    public int id_tournee { get; set; }

    public double prix_prestation { get; set; }

    public double prix_part_com { get; set; }

    public double prix_transport { get; set; }

    public int? type_commande { get; set; }

    public DateOnly? date_confirmation { get; set; }

    public DateOnly? date_signature { get; set; }

    public int? export_carnet_garantie { get; set; }

    public int? origine_sav { get; set; }

    public DateOnly? livraison_souhaitee { get; set; }

    public bool pre_planif { get; set; }

    public string? modele { get; set; }

    public string? couleur { get; set; }

    public bool import_cover { get; set; }

    public int? import_colisage { get; set; }

    public int? id_commercial { get; set; }

    public int? id_type_secondaire { get; set; }

    public int? id_type_tertiaire { get; set; }

    public int? reserve { get; set; }

    public DateOnly? date_facturation { get; set; }

    public int? id_commande_parent { get; set; }

    public int? financement { get; set; }

    public int? commande_ubc { get; set; }

    public DateOnly? date_premiere_confirme { get; set; }

    public DateOnly? date_premier_dossier_fab { get; set; }

    public int? cmd_parent { get; set; }

    public DateOnly? date_validation { get; set; }

    public int? delai_supp { get; set; }

    public int altitude { get; set; }

    public int num_serveur { get; set; }

    public string? designation_projet { get; set; }

    public int? echeance_semaine_fabrication { get; set; }

    public int? id_couleur_ral { get; set; }

    public string? id_couleur_finition { get; set; }

    public string? id_couleur_ral_finition { get; set; }

    public int num_lancement_1 { get; set; }

    public string? lien_dossier_1 { get; set; }

    public string? fichier_Conso { get; set; }

    public int num_lancement_2 { get; set; }

    public string? lien_dossier_2 { get; set; }

    public string? fichier_Debit_Complet { get; set; }

    public bool updated { get; set; }

    public bool import_Debit_Complet { get; set; }

    public bool import_controle { get; set; }

    public bool import_valide { get; set; }

    public bool annulation { get; set; }

    public bool affectation_lot { get; set; }

    public int id_decoupe_lot { get; set; }

    public string statut_decoupe_lot { get; set; } = null!;

    public string? modele_categorie { get; set; }

    public string? modele_type { get; set; }

    public DateOnly? date_acces { get; set; }

    public int serveur_archive { get; set; }

    public virtual ICollection<commande_client_action> commande_client_actions { get; set; } = new List<commande_client_action>();
}
