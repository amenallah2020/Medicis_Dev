using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;


namespace RHEVENT.Models
{
    public class MotifsRejet : IValidatableObject
    {
        [key]
        public int Id { get; set; }


        [Display(Name = "Motif de rejet")]
        [Required]
        [Index("Ix_MotifRejet", Order = 1, IsUnique = true)]
        [StringLength(100)]
        public string MotifRejet { get; set; }


        [Required]
        [Display(Name = "Conséquense")]
        [StringLength(100)]
        public string Conséquense { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = db.MotifsRejets.FirstOrDefault(x => x.MotifRejet == MotifRejet && x.Id != Id);
            //var validateName = db.DA_TypesAchats.FirstOrDefault(x => x.TypeAchat == MotifRejet && x.Id != Id);

            if (validateName != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Ce motif de rejet existe déja.", new[] { "MotifRejet" });
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