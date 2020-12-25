using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class CommandeL
    {
        [Key]
        public int Id { get; set; }
        public string Ref_cmd { get; set; }
        [Display(Name = "Désignation médicament")]
        public string Designation_medicament { get; set; }
        [Display(Name = "Code médicament")]

        public string Code_medicament { get; set; }
    
        [Display(Name = "Quantité commandée")]
        public int Quantite_Commandee { get; set; }
        [Display (Name ="Quantité acceptée")]
      
        public int Quantite_acceptee { get; set; }
        [Display(Name ="Demandeur")]
        public string Nom_prenom { get; set; }
        [Display(Name ="Matricule")]
        public string Matricule { get; set; }
        [Display(Name = "Service")]
        public string Service { get; set; }
        [Display(Name = "Validation PRT")]
        public validation Validation_prt { get; set; }
        [Display(Name = "Décision PRT")]
        public Decision_PRT Decision_prt { get; set; }
        [Display(Name = "Décision ")]
        public validation Decision_primaire { get; set; }
        public string Approbateur_PRT { get; set; }
       
        public string Approbateur_primaire { get; set; }
        [Display(Name = "Commentaire PRT")]
        public string Commentaire_prt { get; set; }
        [Display(Name = "Commentaire")] 
        public string Commentaire_primaire { get; set; }
        public DateTime Date_creation { get; set; }



        public List<CommandeL> list_afficher { get; set; }
        public string list_lignes_valider { get; set; }

        public  Etat_ligne etat { get; set; }

        public DateTime Date_validation { get; set; }

        public string user_validation { get; set; }

    }
    public enum validation_PRT
    {
        NON_APPLICABLE,
        NON,
        OUI

    }

    public enum Decision_PRT
    {
        RIEN,
        NON,
        OUI
    }
  

    public enum Etat_ligne
    {
        en_cours,
        accepte
    }

}