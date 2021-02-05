using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using RHEVENT.Models.RH;

namespace RHEVENT.Models
{
    public class DA_TypesAchats : IValidatableObject
    {
        [key]
        public int Id { get; set; }


        [Display(Name = "Type d'achat")]
        [Required]
        [Index("Ix_TypeAchat", Order = 1, IsUnique = true)]
        [StringLength(50)]
        public string TypeAchat { get; set; }

        [Required]
        [Display(Name = "Code")]
        [StringLength(50)]
        public string Code { get; set; }

        public DA_WorkflowTypAch typworkflow { get; set; }

        [Display(Name = "Workflow")]
        public string Workflow { get; set; }

        //public DA_WorkflowTypAch type_workflow { get; set; }

        [NotMapped]
        public List<DA_WorkflowTypAch> listesworkflow { get; set; }

        [NotMapped]
        public List<FonctionsUsers> listesintervenant { get; set; }

        [NotMapped]
        public IEnumerable<FonctionsUsers> FonctionsCollection { get; set; }

        [NotMapped]
        public string[] SelectedIntervenantsArray { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = db.DA_TypesAchats.FirstOrDefault(x => x.TypeAchat == TypeAchat && x.Id != Id);
            var validateName1 = db.DA_TypesAchats.FirstOrDefault(x => x.Code == Code && x.Id != Id);
            if (validateName1 != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Ce code est déja utilisé pour un autre type.", new[] { "Code" });
                validationResult.Add(errorMessage);
                return validationResult;
            }
            else if(validateName != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Ce type d'achat existe déja.", new[] { "TypeAchat" });
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