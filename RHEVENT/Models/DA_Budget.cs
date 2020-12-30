using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class DA_Budget
    {
        [Key]
        public int Id { get; set; }

        public string Réference { get; set; }

        [Display(Name = "Budget Elementaire")]
        public float BudgetElementaire { get; set; }

        [Display(Name = "Prix Unitaire")]
        public string PrixUnitaire { get; set; }

        public string Quantité { get; set; }

        public string Total { get; set; }

        public string Fournisseur { get; set; }

        public string AL { get; set; }

    }
}