using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_DiffEval
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nom groupe")]
        public string Code { get; set; }

        [Display(Name = "Matricule utilisateur")]
        public string Matricule_Usr { get; set; }

        public string Utilisateur { get; set; }

       

        //[Display(Name = "Etat groupe")]
        //public string Etat_Grp { get; set; }

        //public string Code_Formation { get; set; }

        //public bool IsSelected { get; set; }


    }
}