//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpticienMvcApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class DetailVerre
    {
        public int ProduitID { get; set; }
        public string TypeTraitement { get; set; }
        public Nullable<decimal> Indice { get; set; }
        public Nullable<decimal> DiametrePrecal { get; set; }
    
        public virtual Produit Produit { get; set; }
    }
}
