using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_ResultFormation
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Formation")]
        public string Code_Formation { get; set; }

        [Display(Name = "Objet formation")]
        public string ObjForm { get; set; }

        public string MatUser { get; set; }
 
        [Display(Name = "Utilisateur")]
        public string Usr { get; set; }

        [Display(Name = "Date formation")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateTerm { get; set; }

        [Display(Name = "Date limite")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DeadLine { get; set; }

        [Display(Name = "Résultat")]
        public string Resultat { get; set; }


        [Display(Name = "Etat")]
        public string Etat { get; set; }



    }
}