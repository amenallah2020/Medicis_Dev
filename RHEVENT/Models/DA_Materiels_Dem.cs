using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_Materiels_Dem
    {
        [key]
        public int Id { get; set; }

        public string Réference { get; set; }

        [Display(Name = "Code Sage")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "Désignation")]
        [Required]
        public string Désignation { get; set; }

        //[Display(Name = "Version")]
        //[Required]
        //public string Version { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Réception Souhaitée")]
        public DateTime? Date_Recp_Souh { get; set; }

        [Display(Name = "Type")]
        [Required]
        public string Type { get; set; }


        [Display(Name = "Plafond Budget")]
        [Required]
        public string PlafondBudget { get; set; }


        [Required]
        public string Fournisseur { get; set; }

        [NotMapped]
        public List<DA_Fournisseurs> listesfournisseurs { get; set; }

        [NotMapped]
        public List<DA_Materiels> listesmateriels { get; set; }

        [NotMapped]
        public List<DA_Materiels_Dem> listesmaterielsdem { get; set; }
    }
}