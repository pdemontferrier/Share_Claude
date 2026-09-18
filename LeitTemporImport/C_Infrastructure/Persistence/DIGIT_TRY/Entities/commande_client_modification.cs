using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

public partial class commande_client_modification
{
    public int id { get; set; }

    public string num_projet { get; set; } = null!;

    public string type_modif { get; set; } = null!;

    public string nouvelle_valeur { get; set; } = null!;

    public DateTime date_modification { get; set; }

    public int user_id { get; set; }
}
