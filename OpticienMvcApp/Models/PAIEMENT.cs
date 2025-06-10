using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpticienMvcApp.Models
{
    [MetadataType(typeof(PAIEMENTMetadata))]
    public partial class PAIEMENT
    {
    }

    public class PAIEMENTMetadata
    {
        [Required(ErrorMessage = "Le champ ID est requis.")]
        public int ID { get; set; }

        [Required(ErrorMessage = "La date de paiement est requise.")]
        [Display(Name = "Date de Paiement")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DatePaiement { get; set; }

        [Required(ErrorMessage = "Le mode de paiement est requis.")]
        [Display(Name = "Mode de Paiement")]
        public string ModeDePaiement { get; set; }

        [Required(ErrorMessage = "Le montant payé est requis.")]
        [Display(Name = "Montant Payé")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0.")]
        public decimal MontantPaye { get; set; }

        [Required(ErrorMessage = "L'opération de vente est requise.")]
        [Display(Name = "Opération de Vente")]
        public int OpVenteID { get; set; }

        [Display(Name = "Référence de Paiement")]
        public string ReferencePaiement { get; set; }
    }
} 