using JessyProject.Data;
using JessyProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JessyProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClassementController : ControllerBase
    {
        private readonly ILogger<ClassementController> _logger;

        public ClassementController(ILogger<ClassementController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetClassement")]
        public IEnumerable<ClassementIndividuel> GetClassementSeul()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClassementDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=ClassementDB;Username=postgres;Password=admin;Search Path=c2e");


            using (var context = new ClassementDbContext(optionsBuilder.Options))
            {
                var classementsEngie = context.Engies.ToList();
                var classementBouygues = context.Bouygues.ToList();
                var classementsOhm = context.Ohms.ToList()
                    .Where(x => x.Status != null && (x.Status.StartsWith("Accepted") ||
                                                     x.Status.StartsWith("effective") ||
                                                     x.Status.StartsWith("sendToMkt")));

                var all = new List<ClassementIndividuel>();

                foreach (var classement in classementBouygues)
                {
                    var point = 0;

                    if (classement.Produite.StartsWith("Bbox"))
                    {
                        point = 15;

                        var contract = new ClassementIndividuel()
                        {
                            Nom = classement.Vendeur,
                            Points = point
                        };

                        all.Add(contract);
                    }
                }

                foreach (var classement in classementsEngie)
                {

                    var point = 0;

                    if (classement.Type.TrimEnd() == "DUAL")
                    {
                        point = 10;
                    }

                    if (classement.Type.TrimEnd() == "ASSU")
                    {
                        point = 5;
                    }

                    if (classement.Type.TrimEnd() == "GAZ")
                    {
                        point = 5;
                    }

                    if (classement.Type.TrimEnd() == "ELEC")
                    {
                        point = 5;
                    }

                    var contract = new ClassementIndividuel()
                    {
                        Nom = classement.Vendeur,
                        Points = point
                    };

                    all.Add(contract);
                }

                foreach (var classement in classementsOhm)
                {
                    if (classement.Vendeur.Contains("CHETIH") &&
                        classement.Date > new DateTime(2025, 3, 1) &&
                        (classement.Status.TrimEnd() == "signed" || classement.Status.TrimEnd() == "effective" || classement.Status.TrimEnd() == "sendToMkt"))
                    {
                        var contrat = new ClassementIndividuel()
                        {
                            Nom = classement.Vendeur,
                            Points = 5,
                            Equipe = classement.Equipe
                        };

                        all.Add(contrat);
                    }
                }

                var resultats = all
                    .GroupBy(v => new { v.Nom }) // Grouper par Nom et Prénom
                    .Select(g => new ClassementIndividuel()
                    {
                        Nom = g.Key.Nom.TrimEnd(),
                        Points = g.Sum(v => v.Points),
                        TotalContrats = g.Count()
                    })
                    .OrderByDescending(x => x.Points)
                    .ToList();


                return resultats;
            }
        }

        [HttpGet("GetClassementEquipe")]
        public IEnumerable<ClassementEquipe> GetClassementEquipe()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClassementDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=ClassementDB;Username=postgres;Password=admin;Search Path=c2e");

            var resultats = new List<ClassementEquipe>();

            var classementsIndividuel = getClassementsIndividuels();

            using (var context = new ClassementDbContext(optionsBuilder.Options))
            {
                    resultats = classementsIndividuel
                    .Where(v => v.Equipe != null) // Optionnel : filtre les éléments sans équipe
                    .GroupBy(individu => individu.Equipe)
                    .Select(groupe => new ClassementEquipe
                    {
                        Equipe = groupe.Key, // Le nom de l'équipe vient de la clé de regroupement
                        Points = groupe.Sum(individu => individu.Points) // Somme des points
                    })
                    .OrderByDescending(e => e.Points) // Tri par points décroissants
                    .ToList();
            }

            return resultats;
        }


        private List<ClassementIndividuel> getClassementsIndividuels()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClassementDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=ClassementDB;Username=postgres;Password=admin;Search Path=c2e");


            using (var context = new ClassementDbContext(optionsBuilder.Options))
            {
                var classementsEngie = context.Engies.ToList();
                var classementBouygues = context.Bouygues.ToList();
                var classementsOhm = context.Ohms.ToList()
                    .Where(x => x.Status != null && (x.Status.StartsWith("Accepted") ||
                                                     x.Status.StartsWith("effective") ||
                                                     x.Status.StartsWith("sendToMkt")));

                var all = new List<ClassementIndividuel>();

                foreach (var classement in classementBouygues)
                {
                    var point = 0;

                    if (classement.Produite.StartsWith("Bbox"))
                    {
                        point = 15;

                        var contract = new ClassementIndividuel()
                        {
                            Nom = classement.Vendeur,
                            Points = point,
                            Equipe = classement.Equipe
                        };

                        all.Add(contract);
                    }
                }

                foreach (var classement in classementsEngie)
                {

                    var point = 0;

                    if (classement.Type.TrimEnd() == "DUAL")
                    {
                        point = 10;
                    }

                    if (classement.Type.TrimEnd() == "ASSU")
                    {
                        point = 5;
                    }

                    if (classement.Type.TrimEnd() == "GAZ")
                    {
                        point = 5;
                    }

                    if (classement.Type.TrimEnd() == "ELEC")
                    {
                        point = 5;
                    }

                    var contract = new ClassementIndividuel()
                    {
                        Nom = classement.Vendeur,
                        Points = point,
                        Equipe = classement.Equipe
                    };

                    all.Add(contract);
                }

                foreach (var classement in classementsOhm)
                {
                    if (classement.Vendeur.Contains("CHETIH") && 
                        classement.Date > new DateTime(2025, 3, 1) && 
                        (classement.Status.TrimEnd() == "signed" || classement.Status.TrimEnd() == "effective" || classement.Status.TrimEnd() == "sendToMkt"))
                    {
                        var contrat = new ClassementIndividuel()
                        {
                            Nom = classement.Vendeur,
                            Points = 5,
                            Equipe = classement.Equipe
                        };

                        all.Add(contrat);
                    }
                }

                var resultats = all
                    .GroupBy(v => new { v.Nom, v.Equipe }) // Grouper par Nom et Prénom
                    .Select(g => new ClassementIndividuel()
                    {
                        Nom = g.Key.Nom.TrimEnd(),
                        Points = g.Sum(v => v.Points),
                        TotalContrats = g.Count(),
                        Equipe = g.Key.Equipe
                    })
                    .OrderByDescending(x => x.Points)
                    .ToList();


                return resultats;
            }
        }
    }
}
