﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace RHEVENT.Models
{
    public class DA_TypesActions : IValidatableObject
    {
        [key]
        public int Id { get; set; }


        [Display(Name = "Type d'action")]
        [Required]
        //[Index("Ix_TypeAction", Order = 1, IsUnique = true)]
        [StringLength(100)]
        public string TypeAction { get; set; }

        [Required]
        [Display(Name = "Type d'achat")]
        [StringLength(100)]
        public string TypeActhat { get; set; }

        


        [NotMapped]
        public List<DA_TypesAchats> listesachats { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ValidationResult> validationResult = new List<ValidationResult>();
            var validateName = db.DA_TypesActions.FirstOrDefault(x => x.TypeAction == TypeAction && x.TypeActhat == TypeActhat && x.Id != Id);
            if (validateName != null)
            {
                ValidationResult errorMessage = new ValidationResult
                ("Ce type d'action existe déja.", new[] { "TypeAction" });
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