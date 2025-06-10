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
            var clients = db.Client.ToList();          // Charge en mémoire
            var ordonnances = db.Ordonnance.ToList(); // Charge en mémoire

            ViewBag.ClientID = new SelectList(clients, "ID", "Nom");
            ViewBag.OrdID = new SelectList(ordonnances, "ID", "DateDePrescription");
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
    public ActionResult Create ([Bind(Include = "NumeroOp,DateDeVente,DateLivrisonPrevu,Remarque,ClientID,OrdID,VendeurID,Statut")]
    OperationVente operationVente)
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
    db.OperationVente.Add(operationVente);
    db.SaveChanges();
    // Rediriger vers Details pour pouvoir ajouter lignes/paiements
    return RedirectToAction("Details", new { id = operationVente.ID });
}
            }
        }
        // Si invalide, recharger les DropDownLists
        using (var dbRefresh = new OPTICIENEntities())
{
    ViewBag.ClientID = new SelectList(dbRefresh.Client, "ID", "Nom", operationVente.ClientID);
    ViewBag.OrdID = new SelectList(dbRefresh.Ordonnance, "ID", "DateDePrescription", operationVente.OrdID);
    ViewBag.VendeurID = new SelectList(dbRefresh.Vendeur, "ID", "Nom");
        }
return View(operationVente);
    }

    // GET: OperationVente/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        using (var db = new OPTICIENEntities())
        {
            var operationVente = db.OperationVente
                                   .Include(o => o.Client)
                                   .Include(o => o.Ordonnance)
                                   .Include(o => o.Vendeur)
                                   .FirstOrDefault(op => op.ID == id);

            if (operationVente == null)
                return HttpNotFound();

            ViewBag.ClientID = new SelectList(db.Client.ToList(), "ID", "Nom", operationVente.ClientID);
            ViewBag.OrdID = new SelectList(db.Ordonnance.ToList(), "ID", "DateDePrescription", operationVente.OrdID);
            ViewBag.VendeurID = new SelectList(db.Vendeur.ToList(), "ID", "Nom", operationVente.VendeurID);

            return View(operationVente);
        }
    }





    // POST: OperationVente/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "ID,NumeroOp,DateDeVente,DateLivrisonPrevu,Remarque,ClientID,OrdID,VendeurID,Statut")] OperationVente operationVente)
    {
        using (var db = new OPTICIENEntities())
        {
            if (ModelState.IsValid)
            {
                // Vérifier l’unicité du numéro d’opération
                if (db.OperationVente.Any(o => o.NumeroOp == operationVente.NumeroOp && o.ID != operationVente.ID))
                {
                    ModelState.AddModelError("NumeroOp", "Un autre numéro d'opération existe déjà.");
                }
                else
                {
                    try
                    {
                        db.Entry(operationVente).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = operationVente.ID });
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        ModelState.AddModelError("", "Conflit de mise à jour. Veuillez réessayer.");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                    }
                }
            }

            // Si non valide ou erreur, recharger les listes
            ViewBag.ClientID = new SelectList(db.Client.ToList(), "ID", "Nom", operationVente.ClientID);
            ViewBag.OrdID = new SelectList(db.Ordonnance.ToList(), "ID", "DateDePrescription", operationVente.OrdID);
            ViewBag.VendeurID = new SelectList(db.Vendeur.ToList(), "ID", "Nom", operationVente.VendeurID);

            return View(operationVente);
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
}