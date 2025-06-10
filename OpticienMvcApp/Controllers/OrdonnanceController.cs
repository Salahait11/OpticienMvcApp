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


// Assurez-vous que ce namespace correspond exactement au vôtre

public class OrdonnanceController : Controller
{
    // GET: Ordonnance
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities())
        {
            // VUE Index affiche Medecin.Nom, donc on doit inclure Medecin
            var ordonnances = db.Ordonnance.Include(o => o.Medecin).ToList();
            return View(ordonnances);
        }
    }

    // GET: Ordonnance/Details/5
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // VUE Details affiche Medecin.Nom, donc on doit inclure Medecin
            Ordonnance ordonnance = db.Ordonnance.Include(o => o.Medecin)
                                               .FirstOrDefault(o => o.ID == id); // Utilise FirstOrDefault avec Include

            if (ordonnance == null)
            {
                return HttpNotFound();
            }
            return View(ordonnance);
        }
    }

    // GET: Ordonnance/Create
    public ActionResult Create()
    {
        using (var db = new OPTICIENEntities())
        {
            var medecins = db.Medecin.ToList();  // Force le chargement des données en mémoire
            ViewBag.MedecinID = new SelectList(medecins, "ID", "Nom");
            return View();
        }
    }

    // POST: Ordonnance/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "DateDePrescription,DateExpiration,OD_VL_SPH,OD_VL_CYL,OD_VL_AXE,OD_VL_ADD,OD_VL_EP,OD_VL_H,OD_VL_DAIM,OG_VL_SPH,OG_VL_CYL,OG_VL_AXE,OG_VL_ADD,OG_VL_EP,OG_VL_H,OG_VL_DAIM,OD_VP_SPH,OD_VP_CYL,OD_VP_AXE,OD_VP_ADD,OD_VP_EP,OD_VP_H,OD_VP_DAIM,OG_VP_SPH,OG_VP_CYL,OG_VP_AXE,OG_VP_ADD,OG_VP_EP,OG_VP_H,OG_VP_DAIM,MedecinID")] Ordonnance ordonnance)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                db.Ordonnance.Add(ordonnance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        using (var db = new OPTICIENEntities())
        {
            ViewBag.MedecinID = new SelectList(db.Medecin, "ID", "Nom", ordonnance.MedecinID);
        }
        return View(ordonnance);
    }


    // GET: Ordonnance/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        using (var db = new OPTICIENEntities())
        {
            Ordonnance ordonnance = db.Ordonnance.Find(id);
            if (ordonnance == null)
            {
                return HttpNotFound();
            }

            var medecins = db.Medecin.ToList(); // Charge en mémoire avant la fermeture
            ViewBag.MedecinID = new SelectList(medecins, "ID", "Nom", ordonnance.MedecinID);

            return View(ordonnance);
        }
    }


    // POST: Ordonnance/Edit/5
    [HttpPost]
[ValidateAntiForgeryToken]
    public ActionResult Edit ([Bind(Include = "ID,DateDePrescription,DateExpiration,OD_VL_SPH,OD_VL_CYL,OD_VL_AXE,OD_VL_ADD,OD_VL_EP,OD_VL_H,OD_VL_DAIM,OG_VL_SPH,OG_VL_CYL,OG_VL_AXE,OG_VL_ADD,OG_VL_EP,OG_VL_H,OG_VL_DAIM,OD_VP_SPH,OD_VP_CYL,OD_VP_AXE,OD_VP_ADD,OD_VP_EP,OD_VP_H,OD_VP_DAIM,OG_VP_SPH,OG_VP_CYL,OG_VP_AXE,OG_VP_ADD,OG_VP_EP,OG_VP_H,OG_VP_DAIM,MedecinID")] Ordonnance ordonnance)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
{
    try
    {
        db.Entry(ordonnance).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
    }
    catch (DbUpdateConcurrencyException)
    {
        ModelState.AddModelError("", "L'ordonnance a été modifiée ou supprimée par un autre utilisateur. Veuillez réessayer.");
        // Recharger la liste du médecin pour le DropDownList et retourner la vue
        using (var dbRefresh = new OPTICIENEntities()) { ViewBag.MedecinID = new SelectList(dbRefresh.Medecin, "ID", "Nom", ordonnance.MedecinID); }
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
        // Recharger la liste du médecin pour le DropDownList et retourner la vue
        using (var dbRefresh = new OPTICIENEntities()) { ViewBag.MedecinID = new SelectList(dbRefresh.Medecin, "ID", "Nom", ordonnance.MedecinID); }
    }
}
        }
         // Si invalide, recharger la liste du médecin pour le DropDownList et retourner la vue
        using (var dbRefresh = new OPTICIENEntities()) { ViewBag.MedecinID = new SelectList(dbRefresh.Medecin, "ID", "Nom", ordonnance.MedecinID); }
return View(ordonnance);
    }

    // GET: Ordonnance/Delete/5
    public ActionResult Delete(int? id)
{
    if (id == null)
    {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    }
    using (var db = new OPTICIENEntities())
    {
        // VUE Delete affiche Medecin.Nom, donc on doit inclure Medecin
        Ordonnance ordonnance = db.Ordonnance.Include(o => o.Medecin)
                                            .FirstOrDefault(o => o.ID == id);

        if (ordonnance == null)
        {
            return HttpNotFound();
        }
        return View(ordonnance);
    }
}

// POST: Ordonnance/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public ActionResult DeleteConfirmed(int id)
{
    using (var db = new OPTICIENEntities())
    {
        Ordonnance ordonnance = db.Ordonnance.Find(id);
        if (ordonnance == null)
        {
            return RedirectToAction("Index");
        }

        // Note: Relation OperationVente -> Ordonnance est ON DELETE SET NULL. Pas d'erreur FK ici.
        try
        {
            db.Ordonnance.Remove(ordonnance);
            db.SaveChanges();
        }
        catch (Exception ex) // Gérer d'autres erreurs imprévues
        {
            System.Diagnostics.Trace.TraceError("Erreur imprévue lors de la suppression Ordonnance ID {0}: {1}", id, ex.ToString());
            TempData["ErrorMessage"] = "Erreur lors de la suppression de l'ordonnance.";
            return RedirectToAction("Index"); // Rediriger avec message d'erreur
        }


        return RedirectToAction("Index");
    }
}
}