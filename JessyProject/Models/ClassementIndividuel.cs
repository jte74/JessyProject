namespace JessyProject.Models
{
    public class ClassementIndividuel
    {
        public int Id { get; set; }

        public string Nom { get; set; } = null!;

        public int TotalContrats { get; set; }

        public int Points { get; set; }

        public string? Equipe { get; set; }
    }
}
