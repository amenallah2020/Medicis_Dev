using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_Produits
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Code Produit")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Désignation")]
        public string Désignation { get; set; }

        [Required]
        [Display(Name = "Laboratoire")]
        public string Laboratoire { get; set; }
    }
}