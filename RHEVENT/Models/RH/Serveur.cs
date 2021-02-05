using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Serveur
    {
        [Key]
        public int Id { get; set; }
       
        public string Serv { get; set; }
       
         
    }
    
}