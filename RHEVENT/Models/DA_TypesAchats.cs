using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

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


        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = db.DA_TypesAchats.FirstOrDefault(x => x.TypeAchat == TypeAchat && x.Id != Id);
            if (validateName != null)
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