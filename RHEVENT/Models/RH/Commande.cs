using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Commande
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Réference commande")]
        public string Ref_cmd { get; set; }

        [Display(Name = "Demandeur")]
        public string Nom_prenom { get; set; }
        [Display(Name = "Matricule")]
        public string Matricule { get; set; }
        [Display(Name = "Service")]
        public string Service { get; set; }
        [Display(Name = "Etat commande")]
        public Etat_commande_medicament Etat_commande_medicament { get; set; }
        [Display(Name = "Date commande")]
        public DateTime Date_commande { get; set; }

    }
    public enum Etat_commande_medicament
    {
        Demande_en_cours,
        Demande_livrée,
        Demande_partiellement_livrée
    }
}