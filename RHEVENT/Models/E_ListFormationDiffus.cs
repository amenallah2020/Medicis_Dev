using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_ListFormationDiffus
    {
        [key]
        public int Id { get; set; }

        [DisplayName("Matricule utilisateur")]
        public string Mat_usr { get; set; }

        [DisplayName("Nom utilisateur")]
        public string Nom_usr { get; set; }

        [DisplayName("Code groupe")]
        public string Code_grp { get; set; }

        [DisplayName("Code formation")]
        public string Code_formt { get; set; }

        [DisplayName("Code évaluation")]
        public string Code_eval { get; set; }

        [DisplayName("Date diffusion")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateDiffus { get; set; }
         
        public string MatFormateur { get; set; }

        public string Objet { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Date limite")]
        public DateTime deadline { get; set; }


    }
}