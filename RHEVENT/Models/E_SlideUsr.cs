using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_SlideUsr
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Code formation")]
        public string Code_Formation { get; set; }
         
        public int numSlide { get; set; }

        public int TotalSlide { get; set; }

        public string NameSlide { get; set; }

        public string MatUsr { get; set; }
         
        [Display(Name = "Date création")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Date_Creation { get; set; }

           

    }
}