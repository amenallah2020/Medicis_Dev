﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_Formation
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        [Display(Name = "Code évaluation")]
        public string CodeEval { get; set; }

        public string Objet { get; set; }

        public string Etat_Formation { get; set; }

        [Display(Name = "Date création")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Creation { get; set; }

        //public string Etat_Diffusion { get; set; }

        public string Matricule_Formateur { get; set; }

        public string ImageName { get; set; }

        public int NumDiapo { get; set; }

        public string Chemin { get; set; }

        public string EtatDiff { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        //[Display(Name = "Date limite")]
        //public DateTime? Deadline { get; set; }

        public string EtatF { get; set; }

        //[Display(Name = " ")]
        //public string Eval { get; set; }


        //public string Groupe { get; set; }

        //public string Utilisateur { get; set; }

        //public bool IsSelected { get; set; }

    }
}