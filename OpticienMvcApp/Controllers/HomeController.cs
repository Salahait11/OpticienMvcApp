using System.Linq;
using System.Web.Mvc;
using System;
// Ajoutez les using nécessaires
using System.Data.Entity; // Pour les Includes si besoin
using OpticienMvcApp; // Assurez-vous que ce namespace est correct

namespace OpticienMvcApp.Controllers 
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Vous pouvez ajouter ici du code pour récupérer des données à afficher sur la page d'accueil
            // par exemple, le nombre total de clients, les 5 dernières ventes, etc.

            using (var db = new OPTICIENEntities()) // Utilisez votre contexte EF
            {
                // Exemple : Compter le nombre de clients
                ViewBag.TotalClients = db.Client.Count();

                // Exemple : Récupérer les 5 dernières opérations de vente
                // Inclure le client pour afficher son nom dans la vue
                var lastSales = db.OperationVente
                                   .Include(o => o.Client)
                                   .OrderByDescending(o => o.DateDeVente) // Trier par date la plus récente
                                   .Take(5) // Prendre les 5 premières
                                   .ToList();

                ViewBag.LastSales = lastSales;

                // Exemple : Compter les produits actifs
                ViewBag.ActiveProducts = db.Produit.Count(p => p.Actif);

                // Exemple : Compter les ordonnances valides (non expirées)
                ViewBag.ValidOrdonnances = db.Ordonnance.Count(o => !o.DateExpiration.HasValue || o.DateExpiration.Value >= DateTime.Today);
            }


            return View(); // Retourne la vue Views/Home/Index.cshtml
        }

        public ActionResult About()
        {
            ViewBag.Message = "Votre page de description de l'application.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Votre page de contact.";
            return View();
        }
    }
}