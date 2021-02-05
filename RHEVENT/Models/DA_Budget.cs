using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_Budget
    {
        [Key]
        public int Id { get; set; }

        public string Réference { get; set; }

        [Required]
        [Display(Name = "Article")]
        public string Article { get; set; }

        [Display(Name = "Commentaire")]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Prix Unitaire")]
        public float PrixUnitaire { get; set; }

        [Required]
        public int Quantité { get; set; }

        public float Total { get; set; }

        public string Fournisseur { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Réception Souhaitée")]
        public DateTime? Date_Recp_Souh { get; set; }

        //[DataType(DataType.Date)]
        //[Column(TypeName = "date")]
        [Display(Name = "Date Réception")]
        public string Date_Recp { get; set; }

        [Display(Name = "Type")]
        //[Required]
        public string Type { get; set; }


        //[Display(Name = "Plafond Budget")]
        //[Required]
        //public string PlafondBudget { get; set; }

        //public string AL { get; set; }
        [NotMapped]
        public List<DA_Budget> listesbudget { get; set; }

        [NotMapped]
        public List<DA_Fournisseurs> listesfournisseurs { get; set; }

        [NotMapped]
        public List<DA_Materiels> listeMateriels { get; set; }

        [NotMapped]
        public List<DA_Materiels> listeServices { get; set; }

    }
}