using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Nécessaire pour Include() et EntityState
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net; // Nécessaire pour HttpStatusCodeResult et HttpStatusCode
using System.Web; // Nécessaire pour HttpNotFound()
using System.Web.Mvc; // Nécessaire pour Controller, ActionResult, ViewBag, SelectList, ModelState, RedirectToAction
using OpticienMvcApp;



// Assurez-vous que ce namespace correspond exactement au vôtre

public class OperationVenteController : Controller
{
    // GET: OperationVente
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities())
        {
            // VUE Index affiche Client.Nom, Ordonnance.DateDePrescription, Ordonnance.Medecin.Nom
            // On doit inclure Client, Ordonnance, et Medecin VIA Ordonnance
            var operations = db.OperationVente
                               .Include(o => o.Client)
                               .Include(o => o.Ordonnance.Medecin)
                               .Include(o => o.Vendeur)
                               .ToList();
            return View(operations);
        }
    }

    // GET: OperationVente/Details/5
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // VUE Details affiche Client, Ordonnance (et son Medecin), LignOpVente (et leurs Produits), PAIEMENT
            OperationVente operationVente = db.OperationVente
                                               .Include(o => o.Client)
                                               .Include(o => o.Ordonnance.Medecin) // Inclure Medecin via Ordonnance
                                               .Include(o => o.LignOpVente.Select(li => li.Produit)) // Inclure Lignes ET les Produits dans les lignes
                                               .Include(o => o.PAIEMENT) // Inclure les Paiements
                                               .Include(o => o.Vendeur)
                                               .FirstOrDefault(o => o.ID == id);

            if (operationVente == null)
            {
                return HttpNotFound();
            }
            return View(operationVente);
        }
    }

    // GET: OperationVente/Create
    public ActionResult Create()
    {
        using (var db = new OPTICIENEntities())
        {
            var clients = db.Client.ToList();
            var ordonnances = db.Ordonnance.ToList();
            var produits = db.Produit.Where(p => p.Actif).ToList();

            ViewBag.ClientID = new SelectList(clients, "ID", "Nom");
            ViewBag.OrdID = new SelectList(ordonnances, "ID", "DateDePrescription");
            ViewBag.Produits = new SelectList(produits, "ID", "Designation");
            ViewBag.VendeurID = db.Vendeur
                .ToList()
                .Select(v => new SelectListItem
                {
                    Value = v.ID.ToString(),
                    Text = v.Nom + " " + v.Prenom
                }).ToList();
        }
        return View();
    }

    // POST: OperationVente/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "NumeroOp,DateDeVente,DateLivrisonPrevu,Remarque,ClientID,OrdID,VendeurID,Statut")] OperationVente operationVente, List<LignOpVente> LignesVente)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                // Valider NumeroOp unique
                if (db.OperationVente.Any(o => o.NumeroOp == operationVente.NumeroOp))
                {
                    ModelState.AddModelError("NumeroOp", "Ce numéro d'opération existe déjà.");
                }
                else
                {
                    // Ajouter l'opération de vente
                    db.OperationVente.Add(operationVente);
                    db.SaveChanges(); // Sauvegarder pour obtenir l'ID

                    // Ajouter les lignes de vente
                    if (LignesVente != null && LignesVente.Any())
                    {
                        foreach (var ligne in LignesVente)
                        {
                            ligne.OperationVenteID = operationVente.ID;
                            db.LignOpVente.Add(ligne);
                        }
                        db.SaveChanges();
                    }

                    return RedirectToAction("Details", new { id = operationVente.ID });
                }
            }
        }

        // Si invalide, recharger les DropDownLists
        using (var dbRefresh = new OPTICIENEntities())
        {
            var clients = dbRefresh.Client.ToList();
            var ordonnances = dbRefresh.Ordonnance.ToList();
            var produits = dbRefresh.Produit.Where(p => p.Actif).ToList();

            ViewBag.ClientID = new SelectList(clients, "ID", "Nom", operationVente.ClientID);
            ViewBag.OrdID = new SelectList(ordonnances, "ID", "DateDePrescription", operationVente.OrdID);
            ViewBag.Produits = new SelectList(produits, "ID", "Designation");
            ViewBag.VendeurID = new SelectList(dbRefresh.Vendeur.ToList(), "ID", "Nom");
        }
        return View(operationVente);
    }

    // GET: OperationVente/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        var db = new OPTICIENEntities();
        try
        {
            var operationVente = db.OperationVente
                .Include(o => o.LignOpVente)
                .Include(o => o.LignOpVente.Select(l => l.Produit))
                .FirstOrDefault(o => o.ID == id);

            if (operationVente == null)
            {
                return HttpNotFound();
            }

            // Forcer le chargement des valeurs décimales
            foreach (var ligne in operationVente.LignOpVente)
            {
                // Forcer le chargement des propriétés décimales
                var prixUnitaire = db.LignOpVente
                    .Where(l => l.ID == ligne.ID)
                    .Select(l => l.PrixUnitaireVendu)
                    .FirstOrDefault();
                
                var remise = db.LignOpVente
                    .Where(l => l.ID == ligne.ID)
                    .Select(l => l.Remise)
                    .FirstOrDefault();

                ligne.PrixUnitaireVendu = prixUnitaire;
                ligne.Remise = remise;
            }

            // Charger les listes déroulantes
            var clients = db.Client.ToList();
            var ordonnances = db.Ordonnance.ToList();
            var produits = db.Produit.Where(p => p.Actif).ToList();

            ViewBag.ClientID = new SelectList(clients, "ID", "Nom", operationVente.ClientID);
            ViewBag.OrdID = new SelectList(ordonnances, "ID", "DateDePrescription", operationVente.OrdID);
            ViewBag.Produits = new SelectList(produits, "ID", "Designation");
            ViewBag.VendeurID = new SelectList(db.Vendeur.ToList(), "ID", "Nom", operationVente.VendeurID);

            return View(operationVente);
        }
        catch (Exception)
        {
            db.Dispose();
            throw;
        }
    }

    // POST: OperationVente/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "ID,NumeroOp,Statut,DateDeVente,DateLivrisonPrevu,ClientID,OrdID,VendeurID,Remarque")] OperationVente operationVente, List<LignOpVente> LignesVente)
    {
        var db = new OPTICIENEntities();
        try
        {
            // Vérifier si le modèle est valide
            if (!ModelState.IsValid)
            {
                LoadDropDownLists(db);
                return View(operationVente);
            }

            // Vérifier si le numéro d'opération est unique
            var existingOp = db.OperationVente.FirstOrDefault(o => o.NumeroOp == operationVente.NumeroOp && o.ID != operationVente.ID);
            if (existingOp != null)
            {
                ModelState.AddModelError("NumeroOp", "Ce numéro d'opération existe déjà.");
                LoadDropDownLists(db);
                return View(operationVente);
            }

            // Récupérer l'opération de vente existante
            var opVente = db.OperationVente
                .Include(o => o.LignOpVente)
                .FirstOrDefault(o => o.ID == operationVente.ID);

            if (opVente == null)
            {
                return HttpNotFound();
            }

            // Mettre à jour les propriétés de base
            opVente.NumeroOp = operationVente.NumeroOp;
            opVente.Statut = operationVente.Statut;
            opVente.DateDeVente = operationVente.DateDeVente;
            opVente.DateLivrisonPrevu = operationVente.DateLivrisonPrevu;
            opVente.ClientID = operationVente.ClientID;
            opVente.OrdID = operationVente.OrdID;
            opVente.VendeurID = operationVente.VendeurID;
            opVente.Remarque = operationVente.Remarque;

            // Supprimer les anciennes lignes de vente
            var oldLignes = opVente.LignOpVente.ToList();
            foreach (var ligne in oldLignes)
            {
                db.LignOpVente.Remove(ligne);
            }

            // Ajouter les nouvelles lignes de vente
            if (LignesVente != null)
            {
                foreach (var ligne in LignesVente)
                {
                    if (ligne.ProduitID > 0) // Ne pas ajouter les lignes sans produit
                    {
                        var newLigne = new LignOpVente
                        {
                            OperationVenteID = opVente.ID,
                            ProduitID = ligne.ProduitID,
                            Quantite = ligne.Quantite,
                            PrixUnitaireVendu = ligne.PrixUnitaireVendu,
                            Remise = ligne.Remise,
                            Ligne_OD_SPH = ligne.Ligne_OD_SPH,
                            Ligne_OD_CYL = ligne.Ligne_OD_CYL,
                            Ligne_OD_AXE = ligne.Ligne_OD_AXE,
                            Ligne_OD_ADD = ligne.Ligne_OD_ADD,
                            Ligne_OG_SPH = ligne.Ligne_OG_SPH,
                            Ligne_OG_CYL = ligne.Ligne_OG_CYL,
                            Ligne_OG_AXE = ligne.Ligne_OG_AXE,
                            Ligne_OG_ADD = ligne.Ligne_OG_ADD
                        };
                        db.LignOpVente.Add(newLigne);
                    }
                }
            }

            try
            {
                db.SaveChanges();
                return RedirectToAction("Details", new { id = opVente.ID });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError("", "Conflit de mise à jour. Veuillez réessayer.");
                LoadDropDownLists(db);
                return View(operationVente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                LoadDropDownLists(db);
                return View(operationVente);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Une erreur est survenue : " + ex.Message);
            LoadDropDownLists(db);
            return View(operationVente);
        }
        finally
        {
            db.Dispose();
        }
    }

    // GET: OperationVente/Delete/5
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // VUE Delete affiche Client, Ordonnance (et son Medecin)
            OperationVente operationVente = db.OperationVente
                                               .Include(o => o.Client)
                                               .Include(o => o.Ordonnance.Medecin)
                                               .Include(o => o.Vendeur)
                                               .FirstOrDefault(o => o.ID == id);

            if (operationVente == null)
            {
                return HttpNotFound();
            }
            return View(operationVente);
        }
    }

    // POST: OperationVente/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        using (var db = new OPTICIENEntities())
        {
            OperationVente operationVente = db.OperationVente.Find(id);
            if (operationVente == null)
            {
                return RedirectToAction("Index");
            }

            // Relations LignOpVente et PAIEMENT sont ON DELETE CASCADE.
            // La suppression de l'opération supprime automatiquement lignes et paiements.
            try
            {
                db.OperationVente.Remove(operationVente); // Marquer pour suppression
                db.SaveChanges(); // Exécute DELETE + Cascades
            }
            catch (Exception ex) // Gérer d'autres erreurs imprévues
            {
                System.Diagnostics.Trace.TraceError("Erreur imprévue lors de la suppression OperationVente ID {0}: {1}", id, ex.ToString());
                TempData["ErrorMessage"] = "Erreur lors de la suppression de l'opération de vente.";
                return RedirectToAction("Index"); // Rediriger avec message d'erreur
            }

            return RedirectToAction("Index"); // Rediriger vers la liste après succès
        }
    }

    private void LoadDropDownLists(OPTICIENEntities db)
    {
        var clients = db.Client.ToList();
        var ordonnances = db.Ordonnance.ToList();
        var produits = db.Produit.Where(p => p.Actif).ToList();

        ViewBag.ClientID = new SelectList(clients, "ID", "Nom");
        ViewBag.OrdID = new SelectList(ordonnances, "ID", "DateDePrescription");
        ViewBag.Produits = new SelectList(produits, "ID", "Designation");
        ViewBag.VendeurID = new SelectList(db.Vendeur.ToList(), "ID", "Nom");
    }
}