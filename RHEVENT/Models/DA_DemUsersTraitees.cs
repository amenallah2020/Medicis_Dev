using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_DemUsersTraitees
    {
        [key]
        public int Id { get; set; }

        
        public string Matricule { get; set; }

        public string Reference { get; set; }
    }
}