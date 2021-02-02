using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_Fournisseurs: IValidatableObject
    {
        [key]
        public int Id { get; set; }

        [Display (Name ="Code Fournisseur")]
        [Required]
        public string Code { get; set; }

        
        [Display(Name = "Raison Sociale")]
        [Required]
        [Index("Ix_Raison", Order = 1, IsUnique = true)]
        [StringLength(50)]
        public string Raison { get; set; }

        public string Adresse { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = db.DA_Fournisseurs.FirstOrDefault(x => x.Raison == Raison && x.Id != Id);
            if (validateName != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Ce fournisseur existe déja.", new[] { "Raison" });
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