using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace RHEVENT.Models
{
    public class DA_ProduitsDem
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }

        public string Réference { get; set; }

        [Required]
        public string Laboratoire { get; set; }

        [Required]
        public float Pourcentage { get; set; }

        [Display(Name = "Code Produit")]
        public string Code { get; set; }

        [Display(Name = "Montant")]
        public string Montant { get; set; }

        [NotMapped]
        public IEnumerable<DA_Produits> ProduitsCollection { get; set; }

        [NotMapped]
        public List<DA_Labo> listesLaboratoire { get; set; }

        [NotMapped]
        public List<DA_ProduitsDem> listesProduitsDem{ get; set; }

        [NotMapped]
        public string[] SelectedCodeArray { get; set; }
    }
}