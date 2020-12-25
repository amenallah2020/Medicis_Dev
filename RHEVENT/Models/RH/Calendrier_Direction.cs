using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static RHEVENT.Models.Enumeration;

namespace RHEVENT.Models
{
    public class Calendrier_Direction
    {
        [Key]
        public int Id { get; set;}
        [Display(Name ="Matricule")]
        public string matricule { get; set;}
        [Display(Name = "Nom et prénom")]
        public string nom_prenom { get; set; }
        [Display(Name = "Service")]
        public Service service { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> list_service { get; set; }
    }
}