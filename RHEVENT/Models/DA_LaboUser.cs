using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
namespace RHEVENT.Models
{
    public class DA_LaboUser
    {
        [key]
        public int Id { get; set; }
        
        [Display(Name = "Matricule")]
        [Required]
        [StringLength(10)]
        public string Matricule { get; set; }

        
        [Display(Name = "Laboratoire")]
        [StringLength(100)]
        public string Laboratoire { get; set; }

        [Display(Name = "Etat")]
        public int Etat { get; set; }

        //public List<DA_LaboUser> listeLabo { get; set; }

    }
}