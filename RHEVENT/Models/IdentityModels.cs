using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;


using System.Web.WebPages.Html;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace RHEVENT.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez qu'authenticationType doit correspondre à l'élément défini dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter les revendications personnalisées de l’utilisateur ici
            return userIdentity;
        }


        [Required(ErrorMessage = "Nom requis")]
        [Display(Name = "Nom")]
        //  [RegularExpression(@"^[a-zA-Z]{3,30}$",ErrorMessage ="Nom invalide")]
        public string nom { get; set; }

        [Required(ErrorMessage = "Prénom requis")]
        [Display(Name = "Prénom")]
        //    [RegularExpression(@"^[a-zA-Z]{3,30}$", ErrorMessage = "Prénom invalide")]
        public string prenom { get; set; }

        public string Etat { get; set; }

        public string Statut { get; set; }

        [Required(ErrorMessage = "matricule requis")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Matricule invalide")]
        [Display(Name = "Matricule")]
        public string matricule { get; set; }


        [Required(ErrorMessage = "Téléphone requis")]
        [Display(Name = "Téléphone")]
        [RegularExpression(@"^[0-9]{8}$",
          ErrorMessage = "Numéro de télephone invalide.")]
        [DataType(DataType.PhoneNumber)]
        public int telephone { get; set; }

        [Required(ErrorMessage = "Fonction requis")]
        [Display(Name = "Fonction")]
        public string fonction { get; set; }

        //Le site doit être seulement majuscule

        [Display(Name = "Site")]
        public string site { get; set; }

        //Le service doit être seulement majuscule

        [Display(Name = "Service")]
        public string service { get; set; }
        [Required(ErrorMessage = "Entrer la date de naissance ")]
        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime date_naissance { get; set; }

        [Required(ErrorMessage = "Entrer la date de recrutement")]
        [Display(Name = "Date de recrutement")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime date_recrutement { get; set; }

        [Display(Name = "Signataire")]
        public string signataire { get; set; }

        public IEnumerable<System.Web.Mvc.SelectListItem> signataires { get; set; }

       

        public IEnumerable<System.Web.Mvc.SelectListItem> fonctions { get; set; }

        public Role RoleName { get; set; }


        public float Solde_Conge { get; set; }

        public string Dernier_maj_solde_conge { get; set; }


        public string NomPrenom { get; set; }

        public static explicit operator ApplicationUser(List<ApplicationUser> v)
        {
            throw new NotImplementedException();
        }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
























    public enum Role
    {
        Admin,
        Chargé_RH,
        Employer,
        Superieur_Hiéarchique,
        Directeur,
        Chargé_personnel_RH
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Attestation> Attestations { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.ChangePasswordViewModelbyAdmin> ChangePasswordViewModelbyAdmins { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Autorisation> Autorisations { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Image> Images { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Conge> Conges { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Event> Events { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Email> Emails { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Calendrier_Direction> Calendrier_Directions { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Heure_Superieur> Heure_Superieur { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.medicament> medicaments { get; set; }
        public System.Data.Entity.DbSet<RHEVENT.Models.Commande> Commandes { get; set; }
        public System.Data.Entity.DbSet<RHEVENT.Models.CommandeL> Commandels { get; set; }
        public System.Data.Entity.DbSet<RHEVENT.ViewModels.MesHeuresSup> MesHeuresSup { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.ViewModels.RoleViewModel> RoleViewModels { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_Formation> E_Formation { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_GrpByUsr> E_GrpByUsr { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_listeUsr> E_listeUsr { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ListFormationDiffus> e_ListFormationDiffus { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ListEvaluationDiffus> e_ListEvaluationDiffus { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Fournisseurs> DA_Fournisseurs { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Demande> DA_Demande { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Budget> DA_Budget { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Materiels> DA_Materiels { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Labo> DA_Labo { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_TypesAchats> DA_TypesAchats { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_TypesActions> DA_TypesActions { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_ListesGammes> DA_ListesGammes { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.MesDemAjout> MesDemAjouts { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Materiels_Dem> DA_Materiels_Dem { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_Produits> DA_Produits { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_ProduitsDem> DA_ProduitsDem { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_WorkflowTypAch> DA_WorkflowTypAch { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.RH.FonctionsUsers> FonctionsUsers { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.Dictionnaire> Dictionnaires { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_CodesArticlesSage> DA_CodesArticlesSage { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.MotifsRejet> MotifsRejets { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_listImg> E_listImg { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_Evaluation> E_Evaluation { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_QCM> E_QCM { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_RepUser> E_RepUser { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ResultQCM> E_ResultQCM { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_SlideUsr> E_SlideUsr { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ResultFormation> E_ResultFormation { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ResultQCM_Historiq> E_ResultQCM_Historiq { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ListEvaluationDiffus_Historiq> E_ListEvaluationDiffus_Historiq { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ResultFormation_Historiq> E_ResultFormation_Historiq { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_ResultQCMParSlide> E_ResultQCMParSlide { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.E_DeadLlineEvalUsr> E_DeadLlineEvalUsr { get; set; }


        public System.Data.Entity.DbSet<RHEVENT.Models.Serveur> Serveur { get; set; }

        public System.Data.Entity.DbSet<RHEVENT.Models.DA_LaboUser> DA_LaboUser { get; set; }
        public System.Data.Entity.DbSet<RHEVENT.Models.DA_DemUsersTraitees> DA_DemUsersTraitees { get; set; }
    }


    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore) : base(roleStore) { }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var applicationRoleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
            return applicationRoleManager;
        }
    }

}