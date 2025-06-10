using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpticienMvcApp;
using System.Globalization;

namespace OpticienMvcApp.Controllers
{
    public class ProduitController : Controller
    {
        private OPTICIENEntities db = new OPTICIENEntities();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            // Définir la culture française pour la validation des nombres
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        }

        // GET: Produit
        public ActionResult Index()
        {
            var produits = db.Produit
                .Include(p => p.CategorieProduit)
                .Include(p => p.DetailMonture)
                .Include(p => p.DetailVerre)
                .OrderBy(p => p.Reference)
                .ToList();

            return View(produits);
        }

        // GET: Produit/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produit produit = db.Produit
                .Include(p => p.CategorieProduit)
                .Include(p => p.DetailMonture)
                .Include(p => p.DetailVerre)
                .FirstOrDefault(p => p.ID == id);

            if (produit == null)
            {
                return HttpNotFound();
            }
            return View(produit);
        }

        // GET: Produit/Create
        public ActionResult Create()
        {
            ViewBag.CategorieID = new SelectList(db.CategorieProduit, "ID", "Nom");
            return View();
        }

        // POST: Produit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Reference,Designation,CategorieID,PrixUnitaire,TauxTVA,Fabricant,Actif,DetailMonture,DetailVerre")] Produit produit)
        {
            if (ModelState.IsValid)
            {
                // Vérifier l'unicité de la référence
                if (db.Produit.Any(p => p.Reference == produit.Reference))
                {
                    ModelState.AddModelError("Reference", "Un produit avec cette référence existe déjà.");
                    ViewBag.CategorieID = new SelectList(db.CategorieProduit, "ID", "Nom", produit.CategorieID);
                    return View(produit);
                }

                // Gérer les détails selon la catégorie
                if (produit.CategorieID == 1) // Monture
                {
                    // Vérifier les champs obligatoires
                    if (string.IsNullOrEmpty(produit.DetailMonture?.Marque) ||
                        string.IsNullOrEmpty(produit.DetailMonture?.Couleur) ||
                        string.IsNullOrEmpty(produit.DetailMonture?.Taille))
                    {
                        ModelState.AddModelError("", "Les champs Marque, Couleur et Taille sont obligatoires pour une monture.");
                        ViewBag.CategorieID = new SelectList(db.CategorieProduit, "ID", "Nom", produit.CategorieID);
                        return View(produit);
                    }
                }

                db.Produit.Add(produit);
                db.SaveChanges();

                // Mettre à jour les ProduitID des détails après la sauvegarde
                if (produit.DetailMonture != null)
                {
                    produit.DetailMonture.ProduitID = produit.ID;
                }
                if (produit.DetailVerre != null)
                {
                    produit.DetailVerre.ProduitID = produit.ID;
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategorieID = new SelectList(db.CategorieProduit, "ID", "Nom", produit.CategorieID);
            return View(produit);
        }

        // GET: Produit/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Charger le produit avec ses relations
            Produit produit = db.Produit
                .Include(p => p.CategorieProduit)
                .Include(p => p.DetailMonture)
                .Include(p => p.DetailVerre)
                .FirstOrDefault(p => p.ID == id);

            if (produit == null)
            {
                return HttpNotFound();
            }

            // Initialiser les détails si nécessaire
            if (produit.CategorieID == 1 && produit.DetailMonture == null)
            {
                produit.DetailMonture = new DetailMonture();
            }
            else if (produit.CategorieID == 2 && produit.DetailVerre == null)
            {
                produit.DetailVerre = new DetailVerre();
            }

            // Charger la liste des catégories et la matérialiser immédiatement
            var categories = db.CategorieProduit
                .OrderBy(c => c.Nom)
                .Select(c => new { c.ID, c.Nom })
                .ToList();

            // Créer le SelectList avec la valeur sélectionnée
            ViewBag.CategorieID = new SelectList(categories, "ID", "Nom", produit.CategorieID);

            return View(produit);
        }

        // POST: Produit/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Reference,Designation,CategorieID,Fabricant,Actif,DetailMonture,DetailVerre")] Produit produit)
        {
            // Réinitialiser les erreurs de validation pour les champs décimaux
            ModelState.Remove("PrixUnitaire");
            ModelState.Remove("TauxTVA");

            // Convertir les valeurs en décimales
            string prixUnitaireStr = Request.Form["PrixUnitaire"]?.Replace(',', '.');
            string tauxTVAStr = Request.Form["TauxTVA"]?.Replace(',', '.');

            decimal prixUnitaire;
            decimal tauxTVA;

            if (decimal.TryParse(prixUnitaireStr, NumberStyles.Any, CultureInfo.InvariantCulture, out prixUnitaire))
            {
                produit.PrixUnitaire = prixUnitaire;
            }
            else
            {
                ModelState.AddModelError("PrixUnitaire", "Le prix unitaire doit être un nombre valide.");
            }

            if (decimal.TryParse(tauxTVAStr, NumberStyles.Any, CultureInfo.InvariantCulture, out tauxTVA))
            {
                produit.TauxTVA = tauxTVA;
            }
            else
            {
                ModelState.AddModelError("TauxTVA", "Le taux de TVA doit être un nombre valide.");
            }

            if (ModelState.IsValid)
            {
                var existingProduit = db.Produit
                    .Include(p => p.DetailMonture)
                    .Include(p => p.DetailVerre)
                    .FirstOrDefault(p => p.ID == produit.ID);

                if (existingProduit == null)
                {
                    return HttpNotFound();
                }

                // Mise à jour des propriétés de base
                existingProduit.Reference = produit.Reference;
                existingProduit.Designation = produit.Designation;
                existingProduit.CategorieID = produit.CategorieID;
                existingProduit.PrixUnitaire = prixUnitaire;
                existingProduit.TauxTVA = tauxTVA;
                existingProduit.Fabricant = produit.Fabricant;
                existingProduit.Actif = produit.Actif;

                // Gérer les détails de monture
                if (produit.DetailMonture != null)
                {
                    if (existingProduit.DetailMonture == null)
                    {
                        existingProduit.DetailMonture = new DetailMonture { ProduitID = existingProduit.ID };
                    }
                    existingProduit.DetailMonture.Marque = produit.DetailMonture.Marque;
                    existingProduit.DetailMonture.Modele = produit.DetailMonture.Modele;
                    existingProduit.DetailMonture.Couleur = produit.DetailMonture.Couleur;
                    existingProduit.DetailMonture.Taille = produit.DetailMonture.Taille;
                    existingProduit.DetailMonture.Materiau = produit.DetailMonture.Materiau;
                    existingProduit.DetailMonture.Genre = produit.DetailMonture.Genre;
                }
                else if (existingProduit.DetailMonture != null)
                {
                    db.DetailMonture.Remove(existingProduit.DetailMonture);
                }

                // Gérer les détails de verre
                if (produit.DetailVerre != null)
                {
                    if (existingProduit.DetailVerre == null)
                    {
                        existingProduit.DetailVerre = new DetailVerre { ProduitID = existingProduit.ID };
                    }
                    existingProduit.DetailVerre.TypeTraitement = produit.DetailVerre.TypeTraitement;
                    existingProduit.DetailVerre.Indice = produit.DetailVerre.Indice;
                    existingProduit.DetailVerre.DiametrePrecal = produit.DetailVerre.DiametrePrecal;
                }
                else if (existingProduit.DetailVerre != null)
                {
                    db.DetailVerre.Remove(existingProduit.DetailVerre);
                }

                db.Entry(existingProduit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategorieID = new SelectList(db.CategorieProduit, "ID", "Nom", produit.CategorieID);
            return View(produit);
        }

        // GET: Produit/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Produit produit = db.Produit
                .Include(p => p.CategorieProduit)
                .Include(p => p.DetailMonture)
                .Include(p => p.DetailVerre)
                .FirstOrDefault(p => p.ID == id);

            if (produit == null)
            {
                return HttpNotFound();
            }

            return View(produit);
        }

        // POST: Produit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Produit produit = db.Produit
                .Include(p => p.DetailMonture)
                .Include(p => p.DetailVerre)
                .FirstOrDefault(p => p.ID == id);

            if (produit == null)
            {
                return HttpNotFound();
            }

            // Supprimer d'abord les détails
            if (produit.DetailMonture != null)
            {
                db.DetailMonture.Remove(produit.DetailMonture);
            }
            if (produit.DetailVerre != null)
            {
                db.DetailVerre.Remove(produit.DetailVerre);
            }

            // Puis supprimer le produit
            db.Produit.Remove(produit);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}