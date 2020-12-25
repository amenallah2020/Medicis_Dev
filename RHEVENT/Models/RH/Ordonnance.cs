using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace RHEVENT.Models
{
    public class Ordonnance
    {
        [Key]
        public int Id { get; set; }
        public string Ref_cmd { get; set; }
        [Display(Name = "Demandeur")]
        public string Nom_prenom { get; set; }
        [Display(Name = "Matricule")]
        public string Matricule { get; set; }
        [Display(Name = "Service")]
        public string Service { get; set; }
        public string lien { get; set; }
    }
}