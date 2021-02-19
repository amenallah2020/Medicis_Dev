using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_DeadLlineEvalUsr
    {
        [Key]
        public int Id { get; set; }

        public string Code_eval { get; set; }

        public string MatUser { get; set; }
          
        public DateTime   Deadline { get; set; }
          

    }
}