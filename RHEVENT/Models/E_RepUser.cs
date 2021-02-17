using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_RepUser
    {
        [Key]
        public int Id { get; set; }

        public string Code_eval { get; set; }

        public string MatUser { get; set; }

        public string CodeQcm { get; set; }

        public string Question { get; set; }

        public string Reponse { get; set; }

        public int NumQ { get; set; }

        public int Ordre { get; set; }


        [Display(Name = "Date création")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Creation { get; set; }
          

    }
}