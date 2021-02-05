using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_Evaluation
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Code")]
        public string Code_Eval { get; set; }

        [Display(Name = "Code formation")]
        public string CodeForm { get; set; }

        [Display(Name = "Etat")]
        public string Etat_Eval { get; set; }

        [Display(Name = "Date création")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Creation { get; set; }

        [Display(Name = "Date modification")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Modif { get; set; }

        //public string Etat_Diffusion { get; set; }

        public string Matricule_Formateur { get; set; }

        [Display(Name = "Objet")]
        public string Objet_Eval { get; set; }

        [Display(Name = "Pourcentage de validation")]
        [Required]
        [RegularExpression("^[0-9]*${8}", ErrorMessage = "Pourcentage de validation invalide")]
        public int Pourc_Valid { get; set; }

        [Display(Name = "Durée")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true) ]
        [DataType(DataType.Time)]
        [Required]
        public TimeSpan Duree_Eval { get; set; }

        public string EtatDiff { get; set; }




        //public bool IsSelected { get; set; }

    }
}