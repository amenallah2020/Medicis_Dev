using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class DonneesBase
    {
        [Key]
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
    }
}