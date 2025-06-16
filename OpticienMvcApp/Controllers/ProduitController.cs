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
            try
            {
                // Log des données reçues
                System.Diagnostics.Debug.WriteLine("=== DÉBUT CRÉATION PRODUIT ===");
                System.Diagnostics.Debug.WriteLine($"Catégorie: {produit.CategorieID}");
                System.Diagnostics.Debug.WriteLine($"Référence: {produit.Reference}");
                System.Diagnostics.Debug.WriteLine($"Désignation: {produit.Designation}");
                System.Diagnostics.Debug.WriteLine($"Prix: {produit.PrixUnitaire}");
                System.Diagnostics.Debug.WriteLine($"TVA: {produit.TauxTVA}");
                System.Diagnostics.Debug.WriteLine($"Fabricant: {produit.Fabricant}");
                System.Diagnostics.Debug.WriteLine($"Actif: {produit.Actif}");

                // Vérification des champs obligatoires
                if (string.IsNullOrWhiteSpace(produit.Reference))
                {
                    ModelState.AddModelError("Reference", "La référence est obligatoire");
                    System.Diagnostics.Debug.WriteLine("Erreur: Référence manquante");
                }
                if (string.IsNullOrWhiteSpace(produit.Designation))
                {
                    ModelState.AddModelError("Designation", "La désignation est obligatoire");
                    System.Diagnostics.Debug.WriteLine("Erreur: Désignation manquante");
                }
                if (produit.CategorieID == 0)
                {
                    ModelState.AddModelError("CategorieID", "La catégorie est obligatoire");
                    System.Diagnostics.Debug.WriteLine("Erreur: Catégorie non sélectionnée");
                }
                if (produit.PrixUnitaire <= 0)
                {
                    ModelState.AddModelError("PrixUnitaire", "Le prix unitaire doit être supérieur à 0");
                    System.Diagnostics.Debug.WriteLine("Erreur: Prix unitaire invalide");
                }
                if (produit.TauxTVA < 0)
                {
                    ModelState.AddModelError("TauxTVA", "Le taux de TVA ne peut pas être négatif");
                    System.Diagnostics.Debug.WriteLine("Erreur: Taux TVA négatif");
                }

                // Vérification des détails de monture si c'est une monture
                var categorie = db.CategorieProduit.Find(produit.CategorieID);
                if (categorie != null && categorie.Nom.ToLower() == "monture")
                {
                    if (produit.DetailMonture == null)
                    {
                        produit.DetailMonture = new DetailMonture();
                    }

                    // Initialiser les champs avec des valeurs par défaut
                    if (string.IsNullOrEmpty(produit.DetailMonture.Genre))
                    {
                        produit.DetailMonture.Genre = "U"; // U pour Unisexe par défaut
                    }
                    if (string.IsNullOrEmpty(produit.DetailMonture.Materiau))
                    {
                        produit.DetailMonture.Materiau = "Métal"; // Valeur par défaut
                    }
                    if (string.IsNullOrEmpty(produit.DetailMonture.Modele))
                    {
                        produit.DetailMonture.Modele = "Standard"; // Valeur par défaut
                    }

                    if (string.IsNullOrWhiteSpace(produit.DetailMonture.Marque))
                    {
                        ModelState.AddModelError("DetailMonture.Marque", "La marque est obligatoire pour une monture");
                        System.Diagnostics.Debug.WriteLine("Erreur: Marque manquante pour la monture");
                    }
                    if (string.IsNullOrWhiteSpace(produit.DetailMonture.Couleur))
                    {
                        ModelState.AddModelError("DetailMonture.Couleur", "La couleur est obligatoire pour une monture");
                        System.Diagnostics.Debug.WriteLine("Erreur: Couleur manquante pour la monture");
                    }
                    if (string.IsNullOrWhiteSpace(produit.DetailMonture.Taille))
                    {
                        ModelState.AddModelError("DetailMonture.Taille", "La taille est obligatoire pour une monture");
                        System.Diagnostics.Debug.WriteLine("Erreur: Taille manquante pour la monture");
                    }

                    // Vérifier la longueur du champ Genre
                    if (produit.DetailMonture.Genre != null && produit.DetailMonture.Genre.Length > 1)
                    {
                        ModelState.AddModelError("DetailMonture.Genre", "Le genre doit être un seul caractère (H, F ou U)");
                        System.Diagnostics.Debug.WriteLine("Erreur: Genre trop long");
                    }
                }
                else
                {
                    // Pour les autres catégories, on supprime les erreurs de validation liées à DetailMonture
                    ModelState.Remove("DetailMonture.Marque");
                    ModelState.Remove("DetailMonture.Couleur");
                    ModelState.Remove("DetailMonture.Taille");
                    ModelState.Remove("DetailMonture.Genre");
                    ModelState.Remove("DetailMonture.Materiau");
                    ModelState.Remove("DetailMonture.Modele");
                    // On met DetailMonture à null pour éviter la validation
                    produit.DetailMonture = null;
                }

                if (ModelState.IsValid)
                {
                    // Vérifier l'unicité de la référence
                    if (db.Produit.Any(p => p.Reference == produit.Reference))
                    {
                        ModelState.AddModelError("Reference", "Un produit avec cette référence existe déjà.");
                        System.Diagnostics.Debug.WriteLine("Erreur: Référence déjà existante");
                        ViewBag.CategorieID = new SelectList(db.CategorieProduit, "ID", "Nom", produit.CategorieID);
                        return View(produit);
                    }

                    try
                    {
                        // Sauvegarder d'abord le produit
                        db.Produit.Add(produit);
                        db.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"Produit créé avec ID: {produit.ID}");

                        // Mettre à jour les détails après avoir obtenu l'ID du produit
                        if (categorie != null && categorie.Nom.ToLower() == "monture")
                        {
                            if (produit.DetailMonture != null)
                            {
                                // Vérifier si les détails existent déjà
                                var existingDetails = db.DetailMonture.FirstOrDefault(d => d.ProduitID == produit.ID);
                                if (existingDetails == null)
                                {
                                    // Créer un nouvel objet DetailMonture
                                    var detailMonture = new DetailMonture
                                    {
                                        ProduitID = produit.ID,
                                        Marque = produit.DetailMonture.Marque,
                                        Couleur = produit.DetailMonture.Couleur,
                                        Taille = produit.DetailMonture.Taille,
                                        Genre = produit.DetailMonture.Genre,
                                        Materiau = produit.DetailMonture.Materiau,
                                        Modele = produit.DetailMonture.Modele
                                    };
                                    db.DetailMonture.Add(detailMonture);
                                    System.Diagnostics.Debug.WriteLine("Détails de monture ajoutés");
                                    db.SaveChanges();
                                }
                                else
                                {
                                    // Mettre à jour les détails existants
                                    existingDetails.Marque = produit.DetailMonture.Marque;
                                    existingDetails.Couleur = produit.DetailMonture.Couleur;
                                    existingDetails.Taille = produit.DetailMonture.Taille;
                                    existingDetails.Genre = produit.DetailMonture.Genre;
                                    existingDetails.Materiau = produit.DetailMonture.Materiau;
                                    existingDetails.Modele = produit.DetailMonture.Modele;
                                    System.Diagnostics.Debug.WriteLine("Détails de monture mis à jour");
                                    db.SaveChanges();
                                }
                            }
                        }
                        else if (categorie != null && categorie.Nom.ToLower() == "verre" && produit.DetailVerre != null)
                        {
                            produit.DetailVerre.ProduitID = produit.ID;
                            db.DetailVerre.Add(produit.DetailVerre);
                            System.Diagnostics.Debug.WriteLine("Détails de verre ajoutés");
                            db.SaveChanges();
                        }
                        else
                        {
                            // Pour les autres catégories, on ne fait rien avec les détails
                            System.Diagnostics.Debug.WriteLine("Aucun détail à ajouter pour cette catégorie");
                        }

                        System.Diagnostics.Debug.WriteLine("Sauvegarde finale réussie");
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erreur lors de la sauvegarde: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                        throw;
                    }
                }
                else
                {
                    // Log des erreurs de validation
                    System.Diagnostics.Debug.WriteLine("=== ERREURS DE VALIDATION ===");
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erreur: {error.ErrorMessage}");
                        }
                    }
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                System.Diagnostics.Debug.WriteLine("=== ERREUR DE VALIDATION ENTITY FRAMEWORK ===");
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine($"Entité: {validationErrors.Entry.Entity.GetType().Name}");
                    System.Diagnostics.Debug.WriteLine($"État: {validationErrors.Entry.State}");
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Propriété: {validationError.PropertyName}");
                        System.Diagnostics.Debug.WriteLine($"Erreur: {validationError.ErrorMessage}");
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine("=== ERREUR DE MISE À JOUR BASE DE DONNÉES ===");
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Message: {innerException.Message}");
                    ModelState.AddModelError("", "Erreur de mise à jour : " + innerException.Message);
                    innerException = innerException.InnerException;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== ERREUR INATTENDUE ===");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Une erreur est survenue lors de la création du produit : " + ex.Message);
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