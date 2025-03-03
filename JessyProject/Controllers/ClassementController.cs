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
        private readonly ClassementDbContext _context;

        public ClassementController(ILogger<ClassementController> logger, ClassementDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("GetClassement")]
        public IEnumerable<ClassementIndividuel> GetClassementSeul()
        {
            var classementsEngie = _context.Engies.ToList();
            var classementBouygues = _context.Bouygues.ToList();
            var classementsOhm = _context.Ohms.ToList()
                .Where(x => x.Status != null && (x.Status.StartsWith("Accepted") ||
                                                 x.Status.StartsWith("effective") ||
                                                 x.Status.StartsWith("sendToMkt")));

            var all = new List<ClassementIndividuel>();

            foreach (var classement in classementBouygues)
            {
                var point = 0;

                if (classement.Produite.StartsWith("Bbox") && (classement.Status.StartsWith("Active") || classement.Status.StartsWith("Vente validée")))
                {
                    point = 15;

                    var contract = new ClassementIndividuel()
                    {
                        Nom = classement.Vendeur,
                        Points = point
                    };

                    all.Add(contract);
                }

                if (classement.Produite.StartsWith("Forfait-Bouygues") && (classement.Status.StartsWith("Active") || classement.Status.StartsWith("Vente validée")))
                {
                    point = 5;

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

            var dateReference = new DateTime(2025, 3, 3, 0, 0, 0, DateTimeKind.Utc);
            foreach (var classement in classementsOhm)
            {
                var test = classement.Date.ToUniversalTime();

                if (classement.Vendeur.Contains("CHETIH") && (test.Year == 2025 && test.Month >= 3) &&
                    (classement.Status.TrimEnd() == "signed" || 
                    classement.Status.TrimEnd() == "effective" || 
                    classement.Status.TrimEnd() == "sendToMkt" || 
                    classement.Status.TrimEnd() == "waiting-prepay-vad" || 
                    classement.Status.TrimEnd() == "waiting-prepay-gas-vad" ))
                {
                    var contrat = new ClassementIndividuel()
                    {
                        Nom = classement.Vendeur,
                        Points = 5
                    };

                    all.Add(contrat);
                }
            }

            var resultats = all
                .GroupBy(v => new { v.Nom }) // Grouper par Nom et Pr�nom
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

        [HttpGet("GetClassementEquipe")]
        public IEnumerable<ClassementEquipe> GetClassementEquipe()
        {
            var resultats = new List<ClassementEquipe>();

            var classementsIndividuel = getClassementsIndividuels();

            resultats = classementsIndividuel
            .Where(v => v.Equipe != null) // Optionnel : filtre les elements sans �quipe
            .GroupBy(individu => individu.Equipe)
            .Select(groupe => new ClassementEquipe
            {
                Equipe = groupe.Key, // Le nom de l'equipe vient de la cle de regroupement
                Points = groupe.Sum(individu => individu.Points) // Somme des points
            })
            .OrderByDescending(e => e.Points) // Tri par points decroissants
            .ToList();

            return resultats;
        }


        private List<ClassementIndividuel> getClassementsIndividuels()
        {
            var classementsEngie = _context.Engies.ToList();
            var classementBouygues = _context.Bouygues.ToList();
            var classementsOhm = _context.Ohms.ToList()
                .Where(x => x.Status != null && (x.Status.StartsWith("Accepted") ||
                                                 x.Status.StartsWith("effective") ||
                                                 x.Status.StartsWith("sendToMkt")));

            var all = new List<ClassementIndividuel>();

            foreach (var classement in classementBouygues)
            {
                var point = 0;

                if (classement.Produite.StartsWith("Bbox") && (classement.Status.StartsWith("Active") || classement.Status.StartsWith("Vente validée")))
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

                if (classement.Produite.StartsWith("Forfait-Bouygues") && (classement.Status.StartsWith("Active") || classement.Status.StartsWith("Vente validée")))
                {
                    point = 5;

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
                var test = classement.Date.ToUniversalTime();

                if (classement.Vendeur.Contains("CHETIH") && (test.Year == 2025 && test.Month >= 3) &&
                    (classement.Status.TrimEnd() == "signed" ||
                     classement.Status.TrimEnd() == "effective" ||
                     classement.Status.TrimEnd() == "sendToMkt" ||
                     classement.Status.TrimEnd() == "waiting-prepay-vad" ||
                     classement.Status.TrimEnd() == "waiting-prepay-gas-vad"))
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
                .GroupBy(v => new { v.Nom, v.Equipe }) // Grouper par Nom et Pr�nom
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

