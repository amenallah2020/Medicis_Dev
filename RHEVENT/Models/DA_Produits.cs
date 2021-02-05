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
        
        [Display(Name = "Code Produit")]
        public string Code { get; set; }

        public string Désignation { get; set; }
        
    }
}