using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_listImg
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string chemin { get; set; }

         

    }
}