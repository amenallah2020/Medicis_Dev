using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Numéro image")]
        public string image_id { get; set;}
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Display(Name ="Titre      ")]
        public string titre { get; set; }
        [Display(Name ="Description")]
        public string text { get; set; }
        public string lien { get; set; }
        [Display(Name ="Date téléchargement")]
        public DateTime? date_upload { get; set; }
        public string suppresseur { get; set;}
        public DateTime? date_suppression { get; set; }
        [Display(Name ="Type")]
        public Type_Photo type_photo { get; set; }
    }
    public enum Type_Photo
    {
        Ecran_Gauche,
        Ecran_Droite,
        Gallerie
    }
}