using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RHEVENT.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Mémoriser ce navigateur ?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Matricule",Prompt ="matricule")]
        public string matricule { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe",Prompt ="mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Mémoriser le mot de passe ?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel 
    {
        [Required(ErrorMessage = "Champ obligatoire")]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]
        [Display(Name = "Nom utilisateur")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 5)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }
        
        // [DataType(DataType.Password)]
        // [Display(Name = "Confirmer le mot de passe ")]
        // [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        //   public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]  
        [Display(Name = "Nom")]
        public string nom { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]
        [Display(Name = "Prénom")]
        public string prenom { get; set; }

        [Required(ErrorMessage = "matricule requis")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Matricule invalide")]
        [Display(Name = "Matricule")]
        public string matricule { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]
        [Display(Name = "Téléphone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]{8}$",
          ErrorMessage = "Numéro de télephone invalide.")]
        public int telephone { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]
        [Display(Name = "Fonction")]
        public string fonction { get; set; }


        [Display(Name = "Signataire")]
        public string signataire { get; set; }

        //Le site doit être seulement majuscule
        [Required(ErrorMessage = "Champ obligatoire")]
        [Display(Name = "Site")]
        public string site { get; set; }

        //Le service doit être seulement majuscule
        [Required(ErrorMessage = "Champ obligatoire")]
        [Display(Name = "Service")]
        public string service { get; set; }


          [Required(ErrorMessage = "Champ obligatoire")]
          [DataType(DataType.Date)]
          [Display(Name = "Date de recrutement")]
          public DateTime date_recrutement { get; set; }

        [Required(ErrorMessage = "Champ obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "date de naissance")]
        public DateTime date_naissance { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> signataires { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> fonctions { get; set; }


        public Role RoleName { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Le nouveau mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
    public class ChangePasswordViewModelbyAdmin
    {
        public string Id { get; set;}
        public string Password { get; set; }
    }




    public class ChangePasswordViewModelByUser
    {


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Ancien mot de passe")]
        public string AncienPassword { get; set;}

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Le nouveau mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

    }
}
