using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_ResultQCM_Historiq
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Evaluation")]
        public string Code_EvalByQCM { get; set; }

        [Display(Name = "Formation")]
        public string CodeForm { get; set; }


        public string Code_QCM { get; set; }


        public string MatUser { get; set; }

        [Display(Name = "Utilisateur")]
        public string Usr { get; set; }

        [Display(Name = "Date évaluation")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateEval { get; set; }

        [DisplayName("Date Histotique")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateHisto { get; set; }

        public int? Score { get; set; }

        [Display(Name = "Résultat")]
        public string Resultat { get; set; }

        [Display(Name = "Objet évaluation")]
        public string ObjEval { get; set; }

        [Display(Name = "Objet formation")]
        public string ObjForm { get; set; }

        [Display(Name = "Date limite")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DeadLine { get; set; }


        public string Question { get; set; }

        [Display(Name = "Coefficient")]
        public int Coeff { get; set; }

        [Display(Name = "Réponse 1 ")]
        public string Reponse1 { get; set; }

        [Display(Name = "")]
        public string EtatRep1 { get; set; }

        [Display(Name = "Réponse 2 ")]
        public string Reponse2 { get; set; }

        [Display(Name = "")]
        public string EtatRep2 { get; set; }

        [Display(Name = "Réponse 3 ")]
        public string Reponse3 { get; set; }

        [Display(Name = "")]
        public string EtatRep3 { get; set; }

        [Display(Name = "Réponse 4 ")]
        public string Reponse4 { get; set; }

        [Display(Name = "")]
        public string EtatRep4 { get; set; }


        public string Reponse1User { get; set; }

        public string Reponse2User { get; set; }

        public string Reponse3User { get; set; }

        public string Reponse4User { get; set; }


    }
}