using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_WorkflowTypAch
    {
        [key]
        public int Id { get; set; }

        public int Id_type { get; set; }

        [Display(Name = "Ordre")]
        public int Num { get; set; }

        //fonction
        [Display(Name = "Intervenant")]
        [StringLength(100)]
        public string Intervenant { get; set; }

       
    }
}