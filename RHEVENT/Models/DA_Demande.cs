using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class DA_Demande
    {
        [Key]
        public int Id { get; set; }

        public string Réference { get; set; }

        public string Demandeur { get; set; }

        public string Gamme { get; set; }

        [Display(Name = "Objet de la manifestation")]
        public string Objet { get; set; }

        [Required]
        [Display(Name = "Cibles")]
        public string Cibles { get; set; }

        [Display(Name = "Argumentaires")]
        public string Argumentaires { get; set; }

        [Display(Name = "Budget demandé")]
        public float Budget { get; set; }

        [Required]
        ////[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date demande")]
        public DateTime? Date_demande { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Réception")]
        public DateTime? Date_reception { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Action")]
        public DateTime? Date_action { get; set; }

        [Display(Name = "Etat demande")]
        public string Etat { get; set; }

        [NotMapped]
        public List<DA_ListesGammes> listesGammes { get; set; }
    }
 
}