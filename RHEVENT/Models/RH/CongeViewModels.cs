using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class CongeViewModels
    {  
       [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Jour debut")]
        public DateTime? jour_debut { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Jour fin")]
        public DateTime? jour_fin { get; set; }

        [Required]
        [Display(Name = "Heure de sortie")]
        public string heure_sortie { get; set; }

        [Required]
        [Display(Name = "Heure d'entrée")]
        public string heure_entree { get; set; }

         [Display(Name ="Commentaire")]
        public string commentaire_superieur_hiearchique { get; set; }

        [Display(Name = "NOM ET PRENOM")]
        public string nom_prenom { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> employers { get; set; }
    }
}