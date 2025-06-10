using System;
using System.Collections.Generic;
using System.Data; // Peut être utile
using System.Data.Entity; // Nécessaire pour Include(), bien que non utilisé directement dans ce contrôleur simple
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net; // Nécessaire pour HttpStatusCodeResult
using System.Web;
using System.Web.Mvc;
// Assurez-vous que ce namespace correspond exactement à celui de vos classes EF
using OpticienMvcApp;


    public class MedecinController : Controller
    {
    // GET: Medecin
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities()) // Utilisez le nom exact de votre contexte EF
        {
            // Récupérer tous les médecins
            var medecins = db.Medecin.ToList();

            // Passer la liste des médecins à la vue Index (Views/Medecin/Index.cshtml)
            return View(medecins);
        }
    }
    public ActionResult Details(int? id) // 'int?' permet de gérer le cas où l'ID est manquant
    {
        // Vérifier si un ID a été fourni
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Erreur 400 si ID null
        }

        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher le médecin par son ID
            Medecin medecin = db.Medecin.Find(id);

            // Vérifier si le médecin a été trouvé
            if (medecin == null)
            {
                return HttpNotFound(); // Erreur 404 si médecin non trouvé
            }

            // Passer le médecin trouvé à la vue Details
            return View(medecin);
        }
    }
    // GET: Medecin/Create
    // Affiche le formulaire pour créer un nouveau médecin
    public ActionResult Create()
    {
        return View(); // Retourne la vue Create (Views/Medecin/Create.cshtml)
    }
    [HttpPost] // Indique que cette action répond aux requêtes POST
    [ValidateAntiForgeryToken] // Protection contre les attaques CSRF
    // [Bind] spécifie les propriétés autorisées. N'incluez PAS l'ID.
    public ActionResult Create([Bind(Include = "Nom,Prenom,Gsm,NumeroRPPS")] Medecin medecin)
    {
        // Vérifier si les données reçues sont valides
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities()) // Utilise le contexte EF
            {
                // Optionnel: Ajouter une validation pour le numéro RPPS s'il doit être unique
                // if (!string.IsNullOrEmpty(medecin.NumeroRPPS) && db.Medecin.Any(m => m.NumeroRPPS == medecin.NumeroRPPS))
                // {
                //      ModelState.AddModelError("NumeroRPPS", "Un médecin avec ce numéro RPPS existe déjà.");
                // }
                // else
                // {
                // Ajouter le nouveau médecin au contexte EF
                db.Medecin.Add(medecin);
                // Enregistrer les modifications dans la base de données (exécute INSERT)
                db.SaveChanges();
                // Rediriger vers la page Index (la liste des médecins)
                return RedirectToAction("Index");
                // }
            }
        }

        // Si le modèle n'est pas valide (ou si RPPS existait), retourner la vue Create avec le modèle
        return View(medecin); // Retourne la vue Create avec l'objet 'medecin'
    }
    public ActionResult Edit(int? id) // Reçoit l'ID du médecin à modifier
    {
        // Vérifier si l'ID est fourni
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Erreur 400 si ID null
        }

        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher le médecin par son ID
            Medecin medecin = db.Medecin.Find(id);

            // Vérifier si le médecin a été trouvé
            if (medecin == null)
            {
                return HttpNotFound(); // Erreur 404 si médecin non trouvé
            }

            // Passer le médecin trouvé à la vue Edit
            return View(medecin);
        }
    }

    // POST: Medecin/Edit/5
    // Traite les données soumises par le formulaire de modification et met à jour le médecin
    [HttpPost] // Indique que cette action répond aux requêtes POST
    [ValidateAntiForgeryToken] // Protection CSRF
    // [Bind] inclut l'ID cette fois, car nous avons besoin de l'ID pour savoir quel médecin mettre à jour
    public ActionResult Edit([Bind(Include = "ID,Nom,Prenom,Gsm,NumeroRPPS")] Medecin medecin)
    {
        // Vérifier si les données soumises sont valides
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities()) // Utilise le contexte EF
            {
                // Optionnel: Vérifier si un autre médecin (différent de celui qu'on modifie)
                // a déjà ce numéro RPPS
                // if (!string.IsNullOrEmpty(medecin.NumeroRPPS) && db.Medecin.Any(m => m.NumeroRPPS == medecin.NumeroRPPS && m.ID != medecin.ID))
                // {
                //      ModelState.AddModelError("NumeroRPPS", "Un autre médecin avec ce numéro RPPS existe déjà.");
                // }
                // else
                // {
                try
                {
                    // Marquer l'objet 'medecin' comme modifié dans le contexte EF
                    db.Entry(medecin).State = EntityState.Modified;
                    // Enregistrer les modifications dans la base de données (exécute UPDATE)
                    db.SaveChanges();
                    // Rediriger vers la page Index
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Gérer les conflits de concurrence
                    ModelState.AddModelError("", "Le médecin a été modifié ou supprimé par un autre utilisateur. Veuillez réessayer.");
                    // L'objet medecin contient toujours les valeurs soumises par l'utilisateur.
                }
                catch (Exception ex)
                {
                    // Gérer d'autres erreurs potentielles lors de la sauvegarde
                    ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                    // L'objet medecin contient toujours les valeurs soumises par l'utilisateur.
                }
                // }
            }
        }

        // Si le modèle n'est pas valide (ou si RPPS existait, ou s'il y a eu une erreur de sauvegarde),
        // retourner la vue Edit
        return View(medecin); // Retourne la vue Edit avec l'objet 'medecin'
    }
    public ActionResult Delete(int? id) // Reçoit l'ID du médecin à supprimer
    {
        // Vérifier si l'ID est fourni
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Erreur 400 si ID null
        }

        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher le médecin par son ID
            Medecin medecin = db.Medecin.Find(id);

            // Vérifier si le médecin a été trouvé
            if (medecin == null)
            {
                return HttpNotFound(); // Erreur 404 si médecin non trouvé
            }

            // Passer le médecin trouvé à la vue Delete (confirmation)
            return View(medecin);
        }
    }

    // POST: Medecin/Delete/5
    // Exécute la suppression après confirmation
    [HttpPost, ActionName("Delete")] // Indique POST. ActionName("Delete") mappe cette méthode à l'URL /Medecin/Delete
    [ValidateAntiForgeryToken] // Protection CSRF
    public ActionResult DeleteConfirmed(int id) // Reçoit l'ID du médecin à supprimer
    {
        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher le médecin à supprimer
            Medecin medecin = db.Medecin.Find(id);

            // Vérifier si le médecin existe toujours
            if (medecin == null)
            {
                return RedirectToAction("Index"); // Si déjà supprimé, on redirige
            }

            // Note: La base de données gère la relation avec Ordonnance avec ON DELETE SET NULL.
            // La suppression du médecin ne causera PAS une erreur de clé étrangère.
            // Les Ordonnances liées auront leur MedecinID mis à NULL automatiquement par la base.

            try
            {
                db.Medecin.Remove(medecin); // Marquer l'objet comme à supprimer
                db.SaveChanges(); // Exécuter la commande DELETE dans la base
            }
            catch (Exception ex)
            {
                // Gérer d'autres exceptions imprévues lors de la suppression
                System.Diagnostics.Trace.TraceError("Erreur de suppression Medecin ID {0}: {1}", id, ex.ToString());
                // Ajouter un message d'erreur (même si ON DELETE SET NULL empêche l'erreur FK classique)
                TempData["ErrorMessage"] = "Erreur lors de la suppression du médecin.";
                return RedirectToAction("Index"); // Rediriger vers la liste avec un message
            }

            // Rediriger vers la page Index après une suppression réussie
            return RedirectToAction("Index");
        }
    }
}
