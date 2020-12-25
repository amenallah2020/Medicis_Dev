using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Email
    {
        [Key]
        public int Id { get; set;}   
        public string Destinataire { get; set; }
        public string Email_Destinataire {get;set;}
        public string Sujet {get; set;}
        public string Message { get; set;}
        public string Current_User_Event { get; set; }
        public DateTime Date_email { get; set; }
        public string Etat_Envoi { get; set; } 
      
    }
}