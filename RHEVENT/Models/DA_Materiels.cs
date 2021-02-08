using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class DA_Materiels
    {
        [key]
        public int Id { get; set; }

        [Display(Name = "Code Sage")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "Désignation")]
        [Required]
        public string Désignation { get; set; }

        /*type : materiel ou bien service*/
        [Display(Name = "Type")]
        [Required]
        public string Type { get; set; }

        
        [Display(Name = "Plafond Budget")]
        public string PlafondBudget { get; set; }

        //[Display(Name = "Version")]
        //[Required]
        //public string Version { get; set; }
    }
}