using System;
using System.Collections.Generic;

namespace JessyProject.Models;

public partial class Ohm
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Num_contrat { get; set; } = null!;

    public string? Vendeur { get; set; }

    public string? Status { get; set; }

    public string? Client { get; set; }

    public string? Equipe { get; set; }
}
