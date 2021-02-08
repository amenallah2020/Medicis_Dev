using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RHEVENT.Models
{
    public class DA_Demande : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        public string Réference { get; set; }

        public string Demandeur { get; set; }

        public string Gamme { get; set; }

        [Display(Name = "Objet de la manifestation")]
        public string Objet { get; set; }

        [Required]
        [Display(Name = "Laboratoire")]
        public string Labo { get; set; }

        [Required]
        [Display(Name = "Type d'achat")]
        public string TypeAchat { get; set; }

        [Required]
        [Display(Name = "Type d'Action")]
        public string TypeAction { get; set; }

        [Display(Name = "Commentaire")]
        public string Argumentaires { get; set; }

        [Display(Name = "Budget demandé")]
        public float Budget { get; set; }
        public string Statut { get; set; }
        public string etat_prochain { get; set; }
        public string Validee { get; set; }

        //[Required(ErrorMessage = "Veuillez choisir Si cette action est avec PM ou non")]
        public string AvecSans { get; set; }

        ////[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date demande")]
        public DateTime? Date_demande { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Réception Souhaitée")]
        public DateTime? Date_reception { get; set; }

        [Required]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Date Action")]
        public DateTime? Date_action { get; set; }

        [Display(Name = "Etat demande")]
        public string Etat { get; set; }

        [Display(Name = "Lieu de l'action")]
        public string Lieu { get; set; }

        [Display(Name = "signataire")]
        public string matrsign { get; set; }

        
        [Display(Name = "Motif de Rejet")]
        public string MotifRejet { get; set; }

        [NotMapped]
        public List<DA_Labo> listesLabo { get; set; }

        [NotMapped]
        public List<MotifsRejet> listeMotifs { get; set; }


        [NotMapped]
        public List<DA_TypesAchats> listesachats { get; set; }

        [NotMapped]
        public List<DA_TypesActions> listesactions { get; set; }

        [NotMapped]
        public List<DA_Demande> listesDemandes { get; set; }

        [NotMapped]
        public List<DA_Materiels_Dem> listemateriels{ get; set; }

        [NotMapped]
        public List<DA_Budget> listesbudget { get; set; }

        [NotMapped]
        public List<DA_ProduitsDem> listeproduits { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> Laboratoires { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> TypesAchats { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> TypesActions { get; set; }


        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = Date_action > Date_reception;
            if (validateName != true)
            {
                ValidationResult errorMessage = new ValidationResult
                ("La date d'action doit etre superieure à celle de reception.", new[] { "Date_action" });
                validationResult.Add(errorMessage);
                return validationResult;
            }
            
            else
            {
                return validationResult;
            }


        }
    }
 
}