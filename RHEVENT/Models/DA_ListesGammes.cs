using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_ListesGammes : IValidatableObject
    {
        [key]
        public int Id { get; set; }

        
        [Display(Name = "Désignation Gamme")]
        [Required]
        [Index("Ix_Gamme", Order = 1, IsUnique = true)]
        [StringLength(50)]
        public string Gamme { get; set; }



        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = db.DA_ListesGammes.FirstOrDefault(x => x.Gamme == Gamme && x.Id != Id);
            if (validateName != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Cette gamme existe déja.", new[] { "Gamme" });
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