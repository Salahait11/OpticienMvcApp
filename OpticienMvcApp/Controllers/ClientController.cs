using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Peut être utile, ou si vous ajoutez des .Include plus tard
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpticienMvcApp;

// Assurez-vous que ce namespace correspond exactement au vôtre

public class ClientController : Controller
{
    // GET: Client
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities())
        {
            // Pas de relations affichées dans la vue Index, donc pas besoin de .Include()
            var clients = db.Client.ToList();
            return View(clients);
        }
    }

    // GET: Client/Details/5
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // Pas de relations affichées dans la vue Details, donc pas besoin de .Include()
            Client client = db.Client.Find(id); // Find cherche par clé primaire, pas besoin de FirstOrDefault ici

            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
    }

    // GET: Client/Create
    public ActionResult Create()
    {
        // Pas de listes déroulantes ou de relations à charger pour la création d'un client simple
        return View();
    }

    // POST: Client/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Nom,Prenom,DateDeNaissance,Adresse,NumeroDeTelephone,Email")] Client client)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                db.Client.Add(client);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        // Si invalide, retourne la vue avec le modèle (pour réafficher les erreurs)
        return View(client);
    }

    // GET: Client/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            // Pas de listes déroulantes ou de relations à charger pour la modification d'un client simple
            return View(client);
        }
    }

    // POST: Client/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "ID,Nom,Prenom,DateDeNaissance,Adresse,NumeroDeTelephone,Email")] Client client)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                // Marquer l'objet client comme modifié
                db.Entry(client).State = EntityState.Modified;
                try
                {
                    db.SaveChanges(); // Exécute l'UPDATE
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException) // Gérer les conflits de concurrence si nécessaire
                {
                    ModelState.AddModelError("", "Le client a été modifié ou supprimé par un autre utilisateur. Veuillez réessayer.");
                    // Retourner la vue Edit avec le modèle et l'erreur
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                    // Retourner la vue Edit avec le modèle et l'erreur
                }
            }
        }
        // Si invalide, retourne la vue avec le modèle (pour réafficher les données et erreurs)
        return View(client);
    }

    // GET: Client/Delete/5
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            // Pas de relations affichées dans la vue Delete, pas besoin de .Include()
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }
    }

    // POST: Client/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
        using (var db = new OPTICIENEntities())
        {
            // Rechercher le client à supprimer
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return RedirectToAction("Index"); // Déjà supprimé ou n'existe pas
            }

            // Gérer l'erreur si des OpérationVente sont liées (ON DELETE NO ACTION)
            try
            {
                db.Client.Remove(client); // Marquer pour suppression
                db.SaveChanges(); // Exécute DELETE
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex) // Exception pour les erreurs de base de données
            {
                // Erreur de clé étrangère probable
                System.Diagnostics.Trace.TraceError("Erreur de suppression Client ID {0}: {1}", id, ex.ToString());
                TempData["ErrorMessage"] = "Impossible de supprimer ce client car il a des opérations de vente associées.";
                return RedirectToAction("Index"); // Rediriger vers l'index avec message d'erreur
            }
            catch (Exception ex) // Gérer d'autres erreurs imprévues
            {
                System.Diagnostics.Trace.TraceError("Erreur imprévue lors de la suppression Client ID {0}: {1}", id, ex.ToString());
                TempData["ErrorMessage"] = "Erreur lors de la suppression du client.";
                return RedirectToAction("Index"); // Rediriger vers l'index avec message d'erreur
            }


            return RedirectToAction("Index"); // Rediriger vers la liste après succès
        }
    }
}