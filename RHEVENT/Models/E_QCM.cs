using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_QCM
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Code")]
        public string Code_QCM { get; set; }

        public string Code_EvalByQCM { get; set; }

        public int NumQ { get; set; }

        [Display(Name = "Date création")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Creation { get; set; }

        [Display(Name = "Date Modification")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Modif { get; set; }

        [Required]
        public string Question { get; set; }

        [Display(Name = "Réponse 1 ")]
        [Required]
        public string Reponse1 { get; set; }

        [Display(Name = "")]
        public string EtatRep1 { get; set; }

        [Display(Name = "Réponse 2 ")]
        [Required]
        public string Reponse2 { get; set; }

        [Display(Name = "")]
        public string EtatRep2 { get; set; }

        [Display(Name = "Réponse 3 ")]
        public string Reponse3 { get; set; }

        [Display(Name = "")]
        public string EtatRep3 { get; set; }

        [Display(Name = "Réponse 4 ")]
        public string Reponse4 { get; set; }

        [Display(Name = "")]
        public string EtatRep4 { get; set; }

        [Display(Name = "Coefficient")]
        public int Coeff { get; set; }

       

        //public bool IsSelected { get; set; }

    }
}