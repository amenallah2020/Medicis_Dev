using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class E_listeUsr
    {
        [key]
        public int Id { get; set; }

        [DisplayName("Matricule utilisateur")]
        public string Mat_usr { get; set; }

        [DisplayName("Nom utilisateur")]
        public string Nom_usr { get; set; }

        [DisplayName("Groupe")]
        public string Code_grp { get; set; }

        //[DisplayName("Code formation")]
        //public string Code_formt { get; set; }

        [DisplayName("Utilisateur")]
        public string SelectUsr { get; set; }
    }
}