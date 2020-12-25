using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Enumeration
    {
        public enum Service
        {
            [Display(Name = "Information Tehnology")] IT,
            [Display(Name = "Controle Qualité")] CQ,
            [Display(Name = "Assurance Qualité")] AQ,
            [Display(Name = "Ressources Humaines")] RH,
            [Display(Name = "Achat")] ACHAT,
            [Display(Name = "Affaire Regelementaire")] AR,
            [Display(Name = "Controle de gestion")] CG,
            [Display(Name = "Commercial")] COM,
            [Display(Name = "Direction Administrative & Financière")] DAF,
            [Display(Name = "Marketing")] MAR,
            [Display(Name = "Maintenance")] MAINT,
            [Display(Name = "Planification")] PLANIF,
            [Display(Name = "Production")] PROD,
            [Display(Name = "Audit")] AUDIT,

            [Display(Name = "Administration el Fejja")] ADM_FJ,
            [Display(Name = "Production el Fejja")] PROD_FJ,
            [Display(Name = "Logistique el Fejja")] LOGIS_FJ,
            [Display(Name = "Planification el Fejja")] PLANIF_FJ,
        }
       
    }
}