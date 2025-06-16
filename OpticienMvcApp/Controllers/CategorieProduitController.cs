using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpticienMvcApp;

public class CategorieProduitController : Controller
{
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities()) // Utilisez le nom exact de votre contexte EF
        {
            // Récupérer la liste de toutes les catégories
            var categories = db.CategorieProduit.ToList();
            // Passer la liste à la Vue Index (Views/CategorieProduit/Index.cshtml)
            return View(categories);
        }
    }

    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            CategorieProduit categorieProduit = db.CategorieProduit.Find(id);
            if (categorieProduit == null)
            {
                return HttpNotFound();
            }
            return View(categorieProduit);
        }
    }

    // GET: CategorieProduit/Create
    public ActionResult Create()
    {
        return View();
    }

    // POST: CategorieProduit/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "ID,Nom")] CategorieProduit categorieProduit)
    {
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                try
                {
                    // Vérifier si une catégorie avec ce nom existe déjà (Nom est UNIQUE)
                    if (db.CategorieProduit.Any(c => c.Nom == categorieProduit.Nom))
                    {
                        ModelState.AddModelError("Nom", "Une catégorie avec ce nom existe déjà.");
                    }
                    else
                    {
                        // Vérifier la longueur du nom
                        if (string.IsNullOrWhiteSpace(categorieProduit.Nom))
                        {
                            ModelState.AddModelError("Nom", "Le nom de la catégorie est obligatoire.");
                        }
                        else if (categorieProduit.Nom.Length > 50)
                        {
                            ModelState.AddModelError("Nom", "Le nom de la catégorie ne peut pas dépasser 50 caractères.");
                        }
                        else
                        {
                            db.CategorieProduit.Add(categorieProduit);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }
        }

        return View(categorieProduit);
    }

    // GET: CategorieProduit/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            CategorieProduit categorieProduit = db.CategorieProduit.Find(id);
            if (categorieProduit == null)
            {
                return HttpNotFound();
            }
            return View(categorieProduit); // Passer la catégorie à la vue Edit (Views/CategorieProduit/Edit.cshtml)
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Edit")] // Important si vous utilisez Bind ou UpdateModel
    public ActionResult Edit_Post([Bind(Include = "ID,Nom")] CategorieProduit categorieProduit) // Nommé différemment pour éviter conflit avec GET Edit
    {


        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities())
            {
                // Vérifier si une autre catégorie (différente de celle qu'on modifie)
                // a déjà ce nom unique
                if (db.CategorieProduit.Any(c => c.Nom == categorieProduit.Nom && c.ID != categorieProduit.ID))
                {
                    ModelState.AddModelError("Nom", "Une autre catégorie avec ce nom existe déjà.");
                }
                else
                {
                    db.Entry(categorieProduit).State = System.Data.Entity.EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                    {
                        // Gérer les conflits de concurrence si plusieurs utilisateurs modifient la même catégorie
                        ModelState.AddModelError("", "La catégorie a été modifiée ou supprimée par un autre utilisateur pendant que vous la modifiiez. Veuillez réessayer.");
                        // Ici, il faudrait recharger la catégorie depuis la base et repasser à la vue
                        // Simplifié pour l'exemple
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                    }
                }
            }
        }
        // Si invalide, nom déjà existant, ou erreur de sauvegarde
        return View(categorieProduit); // Retourne la vue Edit avec le modèle et les erreurs
    }



    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
        }
        using (var db = new OPTICIENEntities())
        {
            CategorieProduit categorieProduit = db.CategorieProduit.Find(id);
            if (categorieProduit == null)
            {
                return HttpNotFound();
            }
            return View(categorieProduit); // Passe la catégorie à la vue Delete (Views/CategorieProduit/Delete.cshtml)
        }
    }

   
    [HttpPost, ActionName("Delete")] // L'attribut ActionName permet d'appeler cette méthode via /CategorieProduit/Delete (POST)
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id) // L'ID est reçu du formulaire de confirmation
    {
        using (var db = new OPTICIENEntities())
        {
            CategorieProduit categorieProduit = db.CategorieProduit.Find(id); // Charger l'objet à supprimer
            if (categorieProduit == null)
            {
                return RedirectToAction("Index");
            }

            db.CategorieProduit.Remove(categorieProduit); // Marquer pour suppression
            try
            {
                db.SaveChanges(); // Exécuter la suppression (DELETE)
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
         
                TempData["ErrorMessage"] = "Impossible de supprimer cette catégorie car des produits y sont associés.";
                return RedirectToAction("Index"); // Rediriger vers la liste avec le message
            }


            return RedirectToAction("Index"); // Rediriger vers la liste après suppression réussie
        }
    }
}