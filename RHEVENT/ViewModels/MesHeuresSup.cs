using RHEVENT.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.ViewModels
{
    public class MesHeuresSup
    {
        [Key]
        public int Id { get; set; }
        public string matricule { get; set;}
        [Display(Name ="Nom et prénom")]
        public string nom_prenom { get; set; }

        [Display(Name ="Total heures normals")]
        public string Total_HN { get; set; }
        [Display(Name ="Total heures doubles")]
        public string Total_HD { get; set; }
        [Display (Name ="Date ")]
        public string date {get; set;}

    }
}