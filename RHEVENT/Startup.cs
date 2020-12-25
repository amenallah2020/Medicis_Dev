using System;
using Microsoft.Owin;
using Owin;
using RHEVENT.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

[assembly: OwinStartupAttribute(typeof(RHEVENT.Startup))]
namespace RHEVENT
{
    public partial class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();

        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("PRT"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "PRT";
                roleManager.Create(role);
            }



            /*     if (!roleManager.RoleExists("Admin"))
                 {
                     var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     role.Name = "Admin";
                     roleManager.Create(role);
                 }
                 if (!roleManager.RoleExists("Employer"))
                 {
                     var role_employee = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     role_employee.Name = "Employer";
                     roleManager.Create(role_employee);
                 }

                 if (!roleManager.RoleExists("Chargé_RH"))
                 {
                     var role_charge_rh = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     role_charge_rh.Name = "Chargé_RH";
                     roleManager.Create(role_charge_rh);
                 }
                 if (!roleManager.RoleExists("Chargé_personnel_RH"))
                 {
                     var Chargé_personnel_RH = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     Chargé_personnel_RH.Name = "Chargé_personnel_RH";
                     roleManager.Create(Chargé_personnel_RH);
                 }

                 if (!roleManager.RoleExists("Supérieur_Hiérarchique"))
                 {
                     var role_superieur_hiearchique = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     role_superieur_hiearchique.Name = "Superieur_Hiéarchique";
                     roleManager.Create(role_superieur_hiearchique);
                 }

                 if (!roleManager.RoleExists("Directeur"))
                 {
                     var role_directeur = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     role_directeur.Name = "Directeur";
                     roleManager.Create(role_directeur);
                 }
                 if (!roleManager.RoleExists("SoldeCongésService"))
                 {
                     var role_directeur = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                     role_directeur.Name = "SoldeCongésService";
                     roleManager.Create(role_directeur);
                 }
                 */
        }
        public void ConfigureServices(IServiceCollection services)
        {
          
        }

        }
}
