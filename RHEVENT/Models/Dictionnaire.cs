using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class Dictionnaire
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string table { get; set; }
        [Required]
        public string champ { get; set; }
        [Required]
        public string valeur { get; set; }
        [Required]
        public string signification { get; set; }

    }
}