using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_Labo
    {
        [key]
        public int Id { get; set; }
        [Display(Name = "Laboratoire")]
        [Required]
        public string Laboratoire { get; set; }

        public string Adresse { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }
    }
}