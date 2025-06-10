using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Nécessaire pour Include() et EntityState
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpticienMvcApp;
using System.Globalization;

// Assurez-vous que ce namspace correspond exactement au vôtre

public class PaiementController : Controller
{
    // GET: Paiement
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities())
        {
            // VUE Index affiche OperationVente.NumeroOp, donc on doit inclure OperationVente
            var paiements = db.PAIEMENT.Include(p => p.OperationVente).ToList();
            return View(paiements);
        }
    }

    // GET: Paiement/Details/5
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // VUE Details affiche OperationVente.NumeroOp, donc on doit inclure OperationVente
            PAIEMENT paiement = db.PAIEMENT.Include(p => p.OperationVente)
                                         .FirstOrDefault(p => p.ID == id); // Utilise FirstOrDefault avec Include

            if (paiement == null)
            {
                return HttpNotFound();
            }
            return View(paiement);
        }
    }

    // GET: Paiement/Create
    // Peut recevoir operationVenteId pour présélectionner
    public ActionResult Create(int? operationVenteId)
    {
        using (var db = new OPTICIENEntities())
        {
            var operations = db.OperationVente.ToList(); // Chargement en mémoire

            ViewBag.OpVenteID = new SelectList(operations, "ID", "NumeroOp", operationVenteId);
        }
        return View();
    }


    // POST: Paiement/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create ([Bind(Include = "DatePaiement,ModeDePaiement,MontantPaye,OpVenteID,ReferencePaiement")]
    PAIEMENT paiement)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                db.PAIEMENT.Add(paiement);
                db.SaveChanges();
                // Rediriger vers Details de l'OperationVente associée
                return RedirectToAction("Details", "OperationVente", new { id = paiement.OpVenteID
});
            }
        }
        // Si invalide, recharger la liste d'opérations pour le DropDownList
        using (var dbRefresh = new OPTICIENEntities()) { ViewBag.OpVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", paiement.OpVenteID); }
return View(paiement);
    }

    // GET: Paiement/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // Charger paiement avec sa relation OperationVente et Client (si besoin)
            PAIEMENT paiement = db.PAIEMENT
                .Include(p => p.OperationVente)
                .Include(p => p.OperationVente.Client)  // si tu accèdes à Client dans la vue
                .SingleOrDefault(p => p.ID == id);

            if (paiement == null)
            {
                return HttpNotFound();
            }

            var operations = db.OperationVente.ToList();

            ViewBag.OpVenteID = new SelectList(operations, "ID", "NumeroOp", paiement.OpVenteID);

            return View(paiement);
        }
    }


    [HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "ID,DatePaiement,ModeDePaiement,OpVenteID,ReferencePaiement")] PAIEMENT paiement)
{
    ModelState.Remove("MontantPaye");
    string montantPayeStr = Request.Form["MontantPaye"]?.Replace(',', '.');
    decimal montantPaye;

    if (decimal.TryParse(montantPayeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out montantPaye))
    {
        paiement.MontantPaye = montantPaye;
    }
    else
    {
        ModelState.AddModelError("MontantPaye", "Le montant payé doit être un nombre valide.");
    }

    if (ModelState.IsValid)
    {
        using (var db = new OPTICIENEntities())
        {
            try
            {
                db.Entry(paiement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "OperationVente", new { id = paiement.OpVenteID });
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Le paiement a été modifié ou supprimé par un autre utilisateur. Veuillez réessayer.");
                using (var dbRefresh = new OPTICIENEntities()) 
                { 
                    ViewBag.OpVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", paiement.OpVenteID); 
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                using (var dbRefresh = new OPTICIENEntities()) 
                { 
                    ViewBag.OpVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", paiement.OpVenteID); 
                }
            }
        }
    }
    using (var dbRefresh = new OPTICIENEntities()) 
    { 
        ViewBag.OpVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", paiement.OpVenteID); 
    }
    return View(paiement);
}
    // GET: Paiement/Delete/5
    public ActionResult Delete(int? id)
{
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    using (var db = new OPTICIENEntities())
    {
        // VUE Delete affiche OperationVente.NumeroOp, donc on doit inclure OperationVente
        PAIEMENT paiement = db.PAIEMENT.Include(p => p.OperationVente)
                                     .FirstOrDefault(p => p.ID == id);

        if (paiement == null)
        {
            return HttpNotFound();
        }
        return View(paiement);
    }
}

// POST: Paiement/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public ActionResult DeleteConfirmed(int id)
{
    using (var db = new OPTICIENEntities())
    {
        PAIEMENT paiement = db.PAIEMENT.Find(id);
        if (paiement == null)
        {
            return RedirectToAction("Index");
        }

        // Stocker OpVenteID avant suppression pour redirection
        int operationVenteId = paiement.OpVenteID;

        // Relation Paiement -> OperationVente est ON DELETE CASCADE. Pas d'erreur FK ici.
        try
        {
            db.PAIEMENT.Remove(paiement);
            db.SaveChanges();
        }
        catch (Exception ex) // Gérer d'autres erreurs imprévues
        {
            System.Diagnostics.Trace.TraceError("Erreur imprévue lors de la suppression Paiement ID {0}: {1}", id, ex.ToString());
            TempData["ErrorMessage"] = "Erreur lors de la suppression du paiement.";
            return RedirectToAction("Details", "OperationVente", new { id = operationVenteId }); // Rediriger vers détails opération
        }

        // Rediriger vers Details de l'OperationVente associée après succès
        return RedirectToAction("Details", "OperationVente", new { id = operationVenteId });
    }
}
}