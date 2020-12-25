using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Attestation
    {
        [Key]
        public int Id { get; set; }

      
        [Display(Name ="Attestaion demandée")]
        public AttestationDemandee Intitule { get; set;}
        
        public string titre_attestation { get; set; }
        public string UserId { get; set;}
  
        public string UserName { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name ="Date Demande")]
        public DateTime ? Datetime { get; set;} 

        public string Approbateur_demande { get; set; }

        [Display(Name ="Etat demande")]
        public etat_demande etat_demande { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name ="Commentaire")]
        public string commentaire { get; set; }

        [Display(Name ="Nom et prénom")]
        public string nom_prenom { get; set; }

        [Display(Name = "Service")]
        public string service { get; set; }
        public string Approbateur_RH { get; set; }
    }
    public enum etat_demande
    {
        Demande_en_cours,
        Demande_validée,
        Demande_refusée
    }

    public enum AttestationDemandee
    {
     Stage,
     Travail,
     Salaire,
     Titularisation,
     [Description("Non bénéfice de prêt")]
     Non_benefice_de_pret,
     [Description("Fiche de paie")]
     Fiche_de_paie,
     [Description("Certeficat d'impôt")]
     Certficat_impot, 
     [Description("Titre de congé")]
     Titre_de_conge,
     [Description("Ordre de mission")]
     Ordre_de_mission
    }
}