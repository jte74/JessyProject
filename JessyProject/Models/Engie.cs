using System;
using System.Collections.Generic;

namespace JessyProject.Models;

public partial class Engie
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? NumContrat { get; set; }

    public string? Vendeur { get; set; }

    public string? Status { get; set; }

    public string? Client { get; set; }

    public string? Type { get; set; }

    public string? Equipe { get; set; }
}
