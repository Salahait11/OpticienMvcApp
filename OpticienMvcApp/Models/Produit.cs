using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Globalization;

namespace OpticienMvcApp.Models
{
    [MetadataType(typeof(ProduitMetadata))]
    public partial class Produit
    {
        public DetailMonture DetailMonture { get; set; }
        public DetailVerre DetailVerre { get; set; }
    }

    public class ProduitMetadata
    {
        [Required(ErrorMessage = "Le champ ID est requis.")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Le champ Référence est requis.")]
        [Display(Name = "Référence")]
        public string Reference { get; set; }

        [Required(ErrorMessage = "Le champ Désignation est requis.")]
        [Display(Name = "Désignation")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Le champ Catégorie est requis.")]
        [Display(Name = "Catégorie")]
        public int CategorieID { get; set; }

        [Display(Name = "Fabricant")]
        public string Fabricant { get; set; }

        [Required(ErrorMessage = "Le champ Prix Unitaire est requis.")]
        [Display(Name = "Prix Unitaire")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être un nombre positif.")]
        public decimal PrixUnitaire { get; set; }

        [Required(ErrorMessage = "Le champ Taux TVA est requis.")]
        [Display(Name = "Taux TVA")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Range(0, 100, ErrorMessage = "Le taux de TVA doit être compris entre 0 et 100.")]
        public decimal TauxTVA { get; set; }

        [Required(ErrorMessage = "Le champ Actif est requis.")]
        [Display(Name = "Actif")]
        public bool Actif { get; set; }

        public virtual CategorieProduit CategorieProduit { get; set; }
    }
} 