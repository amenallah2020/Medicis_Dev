using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static RHEVENT.Models.Enumeration;

namespace RHEVENT.Models
{
    public class Heure_Superieur
    {
        [Key]
        public int Id { get; set; }

        public string matricule_superieur { get; set; }
        [Display(Name = "Matricule employer")]

        public string matricule_employer { get; set; }

        public string nom_prenom_superieur { get; set; }

        [Required(ErrorMessage = "Nom et prénom sont requis")]
        [Display(Name = "Nom & prénom")]
        public string nom_prenom_employer { get; set; }

        public string Service { get; set; }

        [Required(ErrorMessage = "Jour du planing est requis")]
        [Display(Name = "Jour du planing")]
        public DateTime? jour_planing { get; set; }

        public string Commentaire { get; set; }

        public DateTime? date_creation { get; set; }

        [Display(Name = "Approbation RH")]
        public Approbation_Heures_Superieurs Approbation_Heure_Sup { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> employers { get; set; }

        public string  list_employers { get; set; }

        public string approbateur { get; set;}

        public DateTime? date_approbation { get; set; }
        //   public List<CheckBoxViewModel> CheckEmployerModel_list { get; set; }

        [Display(Name = "Heure début prévue")]
        public string date_debut_prevu { get; set; }
        [Display(Name = "Heure fin prévue")]
        public string date_fin_prevu { get; set; }
        [Display(Name = "Heure début pointage")]
        public string date_debut_pointage { get; set; }
        [Display(Name = "Heure fin pointage")]
        public string date_fin_pointage { get; set; }        
    }
    public enum Approbation_Heures_Superieurs
    {
        Demande_en_cours,
        Demande_validée,
        Demande_refusée
    }
}