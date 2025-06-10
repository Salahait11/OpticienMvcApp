using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpticienMvcApp; // Assurez-vous que c'est le bon namespace pour votre modèle Vendeur
using System.IO; // Nécessaire pour StringWriter

public class VendeurController : Controller
{
    private OPTICIENEntities db = new OPTICIENEntities(); // Votre contexte de base de données

    // GET: Vendeur/CreatePartial
    public ActionResult CreatePartial()
    {
        return PartialView("_CreateVendeurPartial", new Vendeur());
    }

    // POST: Vendeur/CreatePartial
    [HttpPost]
    public ActionResult CreatePartial(Vendeur vendeur)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Validation supplémentaire
                if (string.IsNullOrWhiteSpace(vendeur.Nom))
                {
                    ModelState.AddModelError("Nom", "Le nom est obligatoire");
                    return Json(new { success = false, html = RenderPartialViewToString("_CreateVendeurPartial", vendeur) });
                }

                if (string.IsNullOrWhiteSpace(vendeur.Prenom))
                {
                    ModelState.AddModelError("Prenom", "Le prénom est obligatoire");
                    return Json(new { success = false, html = RenderPartialViewToString("_CreateVendeurPartial", vendeur) });
                }

                // Ajout du vendeur à la base de données
                db.Vendeur.Add(vendeur);
                db.SaveChanges();

                // Retourner le nom complet du vendeur
                string nomComplet = $"{vendeur.Prenom} {vendeur.Nom}";

                return Json(new
                {
                    success = true,
                    vendeurId = vendeur.ID,
                    vendeurName = nomComplet
                });
            }

            return Json(new
            {
                success = false,
                html = RenderPartialViewToString("_CreateVendeurPartial", vendeur)
            });
        }
        catch (Exception ex)
        {
            // Log l'erreur si nécessaire
            return Json(new
            {
                success = false,
                message = "Une erreur est survenue lors de l'enregistrement du vendeur."
            });
        }
    }

    // Méthode utilitaire pour rendre une vue partielle en chaîne de caractères HTML
    private string RenderPartialViewToString(string viewName, object model)
    {
        ViewData.Model = model;

        using (StringWriter sw = new StringWriter())
        {
            ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
            ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
            viewResult.View.Render(viewContext, sw);
            return sw.GetStringBuilder().ToString();
        }
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