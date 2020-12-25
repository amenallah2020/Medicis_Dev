using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Conge
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Demandeur")]
        public string UserName { get; set; }
        [Display(Name = "Matricule")]
        public string matricule { get; set; }
        [Display(Name = "Fonction")]
        public string Fonction { get; set; }
        [Display(Name = "Service")]
        public string service { get; set; }

        [Display(Name = "Supérieur hiéarchique")]
        public string superieur_hierarchique { get; set; }
        [Display(Name = "Chargé ressource humaine")]
        public string charge_ressource_humaine { get; set; }

        [Display(Name = "Date demande")]
        public DateTime Date_emission_demande { get; set; }

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

        public string commentaire_superieur_hiearchique { get; set; }

        public string commentaire_rh { get; set; }

        [Display(Name = "acceptation supérieur")]
        public acceptation_superieur_hierarchique acceptation_superieur { get; set; }
        [Display(Name = "acceptation RH")]
        public acceptation_ressource_humaine acceptation_ressource { get; set; }

        [Display(Name = "NOM ET PRENOM")]
        public string nom_prenom { get; set; }
        public string Approbateur_RH {get; set;}
        public string site { get; set; }

        public float Solde_Conge { get; set; }

        public DateTime Date_validation_superieur { get; set; }
    
    }
    public enum Acceptation_superieur_hierarchique
    {
        Demande_en_cours,
        Demande_validée,
        Demande_refusée
    }

    public enum Acceptation_ressource_humaine
    {
        Demande_en_cours,
        Demande_validée,
        Demande_refusée
    }
}