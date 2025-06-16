using System.Linq;
using System.Web.Mvc;
using System;
// Ajoutez les using nécessaires
using System.Data.Entity; // Pour les Includes si besoin
using OpticienMvcApp; // Assurez-vous que ce namespace est correct
using System.Collections.Generic;

namespace OpticienMvcApp.Controllers 
{
    public class TopProductViewModel
    {
        public string Designation { get; set; }
        public int Quantity { get; set; }
    }

    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (var db = new OPTICIENEntities())
            {
                // Statistiques de base
                ViewBag.TotalClients = db.Client.Count();
                ViewBag.ActiveProducts = db.Produit.Count(p => p.Actif);
                ViewBag.ValidOrdonnances = db.Ordonnance.Count(o => !o.DateExpiration.HasValue || o.DateExpiration.Value >= DateTime.Today);

                // Statistiques financières
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                
                // Chiffre d'affaires du mois
                ViewBag.MonthlyRevenue = db.OperationVente
                    .Where(o => o.DateDeVente.Month == currentMonth && o.DateDeVente.Year == currentYear)
                    .SelectMany(o => o.LignOpVente)
                    .Sum(l => l.PrixUnitaireVendu * l.Quantite);

                // Chiffre d'affaires de l'année
                ViewBag.YearlyRevenue = db.OperationVente
                    .Where(o => o.DateDeVente.Year == currentYear)
                    .SelectMany(o => o.LignOpVente)
                    .Sum(l => l.PrixUnitaireVendu * l.Quantite);

                // Statistiques des ventes par catégorie
                ViewBag.SalesByCategory = db.LignOpVente
                    .Include(l => l.Produit.CategorieProduit)
                    .Where(l => l.OperationVente.DateDeVente.Year == currentYear)
                    .GroupBy(l => l.Produit.CategorieProduit.Nom)
                    .Select(g => new { Category = g.Key, Total = g.Sum(l => l.PrixUnitaireVendu * l.Quantite) })
                    .ToList();

                // Top 5 des produits les plus vendus
                ViewBag.TopProducts = db.LignOpVente
                    .Include(l => l.Produit)
                    .Where(l => l.OperationVente.DateDeVente.Year == currentYear)
                    .GroupBy(l => new { l.Produit.ID, l.Produit.Designation })
                    .Select(g => new TopProductViewModel 
                    { 
                        Designation = g.Key.Designation, 
                        Quantity = g.Sum(l => l.Quantite) 
                    })
                    .OrderByDescending(x => x.Quantity)
                    .Take(5)
                    .ToList();

                // Statistiques des clients
                ViewBag.NewClientsThisMonth = db.Client
                    .Count(c => c.DateDeNaissance.HasValue && 
                           c.DateDeNaissance.Value.Month == currentMonth && 
                           c.DateDeNaissance.Value.Year == currentYear);

                // Dernières ventes avec plus de détails
                var lastSales = db.OperationVente
                    .Include(o => o.Client)
                    .Include(o => o.Vendeur)
                    .Include(o => o.LignOpVente)
                    .OrderByDescending(o => o.DateDeVente)
                    .Take(5)
                    .ToList();

                // Calculer le montant total pour chaque vente
                foreach (var sale in lastSales)
                {
                    sale.MontantTotal = sale.LignOpVente.Sum(l => l.PrixUnitaireVendu * l.Quantite);
                }

                ViewBag.LastSales = lastSales;

                // Taux de conversion (ventes / ordonnances)
                var totalOrdonnances = db.Ordonnance.Count();
                var totalVentes = db.OperationVente.Count();
                ViewBag.ConversionRate = totalOrdonnances > 0 ? (double)totalVentes / totalOrdonnances * 100 : 0;

                // Alertes
                var alertes = new List<string>();
                
                // Alertes pour les produits en stock faible (si la propriété existe)
                var lowStockProducts = db.Produit.Where(p => p.Actif).ToList();
                if (lowStockProducts.Any())
                {
                    alertes.Add($"{lowStockProducts.Count} produits actifs");
                }

                // Alertes pour les ordonnances qui expirent bientôt
                var today = DateTime.Today;
                var nextMonth = today.AddMonths(1);
                var expiringOrdonnances = db.Ordonnance
                    .Where(o => o.DateExpiration.HasValue && 
                           o.DateExpiration.Value >= today && 
                           o.DateExpiration.Value <= nextMonth)
                    .Count();
                if (expiringOrdonnances > 0)
                {
                    alertes.Add($"{expiringOrdonnances} ordonnances expirent dans le mois");
                }

                ViewBag.Alertes = alertes;
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