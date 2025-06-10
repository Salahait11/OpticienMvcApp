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




public class LignOpVenteController : Controller
{
    // GET: LignOpVente
    // Affiche la liste de toutes les lignes de vente (peut être très long)
    // Note: Dans une vraie application, il est plus courant de filtrer par OperationVente
    public ActionResult Index()
    {
        using (var db = new OPTICIENEntities()) // Utilisez le nom exact de votre contexte EF
        {
            // Récupérer toutes les lignes de vente et inclure l'opération de vente et le produit associés
            var lignes = db.LignOpVente.Include(l => l.OperationVente).Include(l => l.Produit).ToList();

            // Passer la liste des lignes (avec opération et produit) à la vue Index
            return View(lignes);
        }
    }

    // GET: LignOpVente/Details/5
    // Affiche les détails d'une ligne de vente spécifique
    public ActionResult Details(int? id) // 'int?' pour gérer l'ID manquant
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Retourne une erreur 400 si l'ID est null
        }

        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher la ligne par son ID et inclure l'opération de vente et le produit
            LignOpVente lignOpVente = db.LignOpVente.Include(l => l.OperationVente).Include(l => l.Produit)
                                                .FirstOrDefault(l => l.ID == id); // Utilise FirstOrDefault pour rechercher par condition

            // Vérifier si la ligne a été trouvée
            if (lignOpVente == null)
            {
                return HttpNotFound(); // Retourne une erreur 404 si la ligne n'existe pas
            }

            // Passer la ligne trouvée (avec opération et produit inclus) à la vue Details
            return View(lignOpVente);
        }
    }

    // GET: LignOpVente/Create
    // Affiche le formulaire pour créer une nouvelle ligne de vente.
    // Peut recevoir un OperationVenteId pour lier la ligne à une opération existante.
    public ActionResult Create(int? operationVenteId)
    {
        using (var db = new OPTICIENEntities())
        {
            // Charger en mémoire AVANT la fermeture du contexte
            var operations = db.OperationVente.ToList();
            var produitsActifs = db.Produit.Where(p => p.Actif).ToList();

            ViewBag.OperationVenteID = new SelectList(operations, "ID", "NumeroOp", operationVenteId);
            ViewBag.ProduitID = new SelectList(produitsActifs, "ID", "Designation");

            if (operationVenteId.HasValue)
            {
                ViewBag.ParentOperationVenteId = operationVenteId.Value;
            }

            return View();
        }
    }

    // POST: LignOpVente/Create
    // Traite les données soumises par le formulaire de création
    [HttpPost] // Cette action répond aux requêtes POST
    [ValidateAntiForgeryToken] // Protection CSRF
    // [Bind] spécifie les propriétés autorisées. N'incluez PAS l'ID.
    // Inclure les champs spécifiques de la ligne et les clés étrangères.
    public ActionResult Create([Bind(Include = "OperationVenteID,ProduitID,Quantite,PrixUnitaireVendu,Remise,Ligne_OD_SPH,Ligne_OD_CYL,Ligne_OD_AXE,Ligne_OD_ADD,Ligne_OG_SPH,Ligne_OG_CYL,Ligne_OG_AXE,Ligne_OG_ADD,Ligne_EP_VL,Ligne_H_Prog")] LignOpVente lignOpVente)
    {
        // Note: PrixUnitaireVendu et Remise ont des valeurs par défaut dans la base. Quantite > 0.

        // Vérifier si les données reçues sont valides
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities()) // Utilise le contexte EF
            {
                // Vous pourriez ajouter une logique métier ici, par exemple:
                // - Vérifier que ProduitID et OperationVenteID existent (normalement la DropDownList s'en assure)
                // - Vérifier que PrixUnitaireVendu est <= PrixUnitaire du produit (règle métier)
                // - Calculer le prix HT de la ligne ici si vous en avez besoin avant sauvegarde
                // - Vérifier la Quantite > 0 (déjà dans la base, mais double validation est bien)

                // Ajouter la nouvelle ligne de vente au contexte EF
                db.LignOpVente.Add(lignOpVente);
                // Enregistrer les modifications dans la base de données (exécute INSERT)
                db.SaveChanges();

                // Rediriger l'utilisateur, idéalement vers la page Details de l'OperationVente parente
                return RedirectToAction("Details", "OperationVente", new { id = lignOpVente.OperationVenteID });
                // Alternative: return RedirectToAction("Index"); pour retourner à la liste de toutes les lignes
            }
        }

        // Si le modèle n'est pas valide, recharger les listes déroulantes
        using (var db = new OPTICIENEntities())
        {
            // Re-crée les SelectList, en présélectionnant les valeurs choisies par l'utilisateur
            ViewBag.OperationVenteID = new SelectList(db.OperationVente, "ID", "NumeroOp", lignOpVente.OperationVenteID);
            ViewBag.ProduitID = new SelectList(db.Produit.Where(p => p.Actif), "ID", "Designation", lignOpVente.ProduitID);
        }

        // Retourne la vue Create avec l'objet 'lignOpVente' (qui contient les données soumises et les erreurs)
        return View(lignOpVente);
    }

    // GET: LignOpVente/Edit/5
    // Affiche le formulaire pour modifier une ligne de vente existante
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        using (var db = new OPTICIENEntities())
        {
            // Recherche de la ligne avec ses relations
            LignOpVente lignOpVente = db.LignOpVente
                .Include(l => l.OperationVente)
                .Include(l => l.Produit)
                .FirstOrDefault(l => l.ID == id);

            if (lignOpVente == null)
            {
                return HttpNotFound();
            }

            // Charger en mémoire les données nécessaires AVANT fermeture du contexte
            var operations = db.OperationVente.ToList();
            var produitsActifs = db.Produit.Where(p => p.Actif).ToList();

            // Initialiser les listes déroulantes avec les valeurs présélectionnées
            ViewBag.OperationVenteID = new SelectList(operations, "ID", "NumeroOp", lignOpVente.OperationVenteID);
            ViewBag.ProduitID = new SelectList(produitsActifs, "ID", "Designation", lignOpVente.ProduitID);

            // Stocker l'ID de l'opération parente pour le lien de retour
            ViewBag.ParentOperationVenteId = lignOpVente.OperationVenteID;

            return View(lignOpVente);
        }
    }


  
    [HttpPost] // Cette action répond aux requêtes POST
    [ValidateAntiForgeryToken] // Protection CSRF
    // [Bind] inclut l'ID et les propriétés autorisées, y compris les clés étrangères
    public ActionResult Edit([Bind(Include = "ID,OperationVenteID,ProduitID,Quantite,PrixUnitaireVendu,Remise,Ligne_OD_SPH,Ligne_OD_CYL,Ligne_OD_AXE,Ligne_OD_ADD,Ligne_OG_SPH,Ligne_OG_CYL,Ligne_OG_AXE,Ligne_OG_ADD,Ligne_EP_VL,Ligne_H_Prog")] LignOpVente lignOpVente)
    {
        // Vérifier si les données soumises sont valides
        if (ModelState.IsValid)
        {
            using (var db = new OPTICIENEntities()) // Utilise le contexte EF
            {
                // Vous pourriez ajouter ici des validations métier comme pour le Create
                try
                {
                    // Marquer l'objet 'lignOpVente' comme modifié
                    db.Entry(lignOpVente).State = EntityState.Modified;
                    // Enregistrer les modifications (UPDATE)
                    db.SaveChanges();
                    // Rediriger vers Details de l'OperationVente parente
                    return RedirectToAction("Details", "OperationVente", new { id = lignOpVente.OperationVenteID });
                    // Alternative: return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "La ligne de vente a été modifiée ou supprimée par un autre utilisateur. Veuillez réessayer.");
                    // Recharger les listes DropDownList et retourner la vue
                    using (var dbRefresh = new OPTICIENEntities())
                    {
                        ViewBag.OperationVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", lignOpVente.OperationVenteID);
                        ViewBag.ProduitID = new SelectList(dbRefresh.Produit.Where(p => p.Actif), "ID", "Designation", lignOpVente.ProduitID);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la sauvegarde : " + ex.Message);
                    // Recharger les listes DropDownList et retourner la vue
                    using (var dbRefresh = new OPTICIENEntities())
                    {
                        ViewBag.OperationVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", lignOpVente.OperationVenteID);
                        ViewBag.ProduitID = new SelectList(dbRefresh.Produit.Where(p => p.Actif), "ID", "Designation", lignOpVente.ProduitID);
                    }
                }
            }
        }

        // Si le modèle n'est pas valide, recharger les listes DropDownList et retourner la vue Edit
        using (var dbRefresh = new OPTICIENEntities())
        {
            ViewBag.OperationVenteID = new SelectList(dbRefresh.OperationVente, "ID", "NumeroOp", lignOpVente.OperationVenteID);
            ViewBag.ProduitID = new SelectList(dbRefresh.Produit.Where(p => p.Actif), "ID", "Designation", lignOpVente.ProduitID);
        }
        // Stocker l'ID de l'opération parente pour le lien de retour
        ViewBag.ParentOperationVenteId = lignOpVente.OperationVenteID;
        return View(lignOpVente); // Retourne la vue Edit avec le modèle et les erreurs
    }

    // GET: LignOpVente/Delete/5
    // Affiche la page de confirmation de suppression
    public ActionResult Delete(int? id) // Reçoit l'ID de la ligne à supprimer
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // Erreur 400 si ID null
        }

        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher la ligne par son ID et inclure l'opération de vente et le produit pour l'afficher dans la confirmation
            LignOpVente lignOpVente = db.LignOpVente.Include(l => l.OperationVente).Include(l => l.Produit)
                                                .FirstOrDefault(l => l.ID == id);

            // Vérifier si la ligne a été trouvée
            if (lignOpVente == null)
            {
                return HttpNotFound(); // Erreur 404 si ligne non trouvée
            }

            // Stocker l'ID de l'opération parente pour le lien de retour après suppression
            ViewBag.ParentOperationVenteId = lignOpVente.OperationVenteID;


            // Passer la ligne trouvée (avec relations incluses) à la vue Delete (confirmation)
            return View(lignOpVente);
        }
    }

    // POST: LignOpVente/Delete/5
    // Exécute la suppression après confirmation
    [HttpPost, ActionName("Delete")] // Mappe cette méthode à l'URL /LignOpVente/Delete (POST)
    [ValidateAntiForgeryToken] // Protection CSRF
    public ActionResult DeleteConfirmed(int id) // Reçoit l'ID de la ligne à supprimer
    {
        using (var db = new OPTICIENEntities()) // Utilise le contexte EF
        {
            // Rechercher la ligne à supprimer
            LignOpVente lignOpVente = db.LignOpVente.Find(id);

            // Vérifier si la ligne existe toujours
            if (lignOpVente == null)
            {
                // Si déjà supprimé ou n'existe pas, rediriger vers l'index ou la page parente
                // Stocker l'ID de l'opération parente avant de tenter de trouver la ligne
                // (nécessiterait de le passer en paramètre ou via formulaire)
                // Simplifions: On redirige vers l'index des opérations de vente si on ne peut pas trouver la ligne parente
                return RedirectToAction("Index", "OperationVente");
            }

            // Stocker l'ID de l'opération parente AVANT la suppression pour pouvoir y rediriger
            int operationVenteId = lignOpVente.OperationVenteID;

            // Note: La suppression d'une ligne de vente n'a pas de contraintes ON DELETE NO ACTION vers d'autres tables.
            // La suppression de l'OperationVente parente déclenchera CASCADE pour les lignes (voir OperationVenteController).

            try
            {
                db.LignOpVente.Remove(lignOpVente); // Marquer l'objet comme à supprimer
                db.SaveChanges(); // Exécuter la commande DELETE dans la base
            }
            catch (Exception ex) // Gérer d'autres exceptions imprévues
            {
                System.Diagnostics.Trace.TraceError("Erreur imprévue lors de la suppression LignOpVente ID {0}: {1}", id, ex.ToString()); // Optionnel: logguer l'erreur
                // Ajouter un message d'erreur à TempData pour l'afficher sur la page de l'Opération Vente
                TempData["ErrorMessage"] = "Erreur lors de la suppression de la ligne de vente.";
                // Rediriger vers la page Details de l'OperationVente parente
                return RedirectToAction("Details", "OperationVente", new { id = operationVenteId });
            }

            // Rediriger vers la page Details de l'OperationVente parente après une suppression réussie
            return RedirectToAction("Details", "OperationVente", new { id = operationVenteId });
            // Alternative: return RedirectToAction("Index"); pour rediriger vers la liste de toutes les lignes
        }
    }
}