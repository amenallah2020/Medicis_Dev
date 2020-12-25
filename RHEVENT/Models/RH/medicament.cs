using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class medicament
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Code")]
        public string code_medicament { get; set; }
        [Display(Name = "Désignation")]

        public string Code_PCT { get; set; }
        public string designation_medicament { get; set; }
        [Display(Name = "Validation PRT")]
        public validation validation_prt { get; set; }
        [Display(Name = "Image")]
        public string lien_image { get; set;}
        public string list_medicaments_commander { get; set; }

    }
    public enum validation
    {   RIEN,
        NON,
        OUI
    }
    

  
}