using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;


namespace RHEVENT.Models.RH
{
    public class FonctionsUsers
    {
        [key]
        public int Id { get; set; }

        //[Required]
        //[Display(Name = "Code")]
        //[StringLength(50)]
        //public string Code { get; set; }

        [StringLength(100)]
        [Display(Name = "Fonction")]
        public string Fonction { get; set; }

    }
}