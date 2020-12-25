namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attestations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Intitule = c.Int(nullable: false),
                        titre_attestation = c.String(),
                        UserId = c.String(),
                        UserName = c.String(),
                        Datetime = c.DateTime(),
                        Approbateur_demande = c.String(),
                        etat_demande = c.Int(nullable: false),
                        commentaire = c.String(),
                        nom_prenom = c.String(),
                        service = c.String(),
                        Approbateur_RH = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Autorisations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserName = c.String(),
                        matricule = c.String(),
                        Fonction = c.String(),
                        service = c.String(),
                        superieur_hierarchique = c.String(),
                        charge_ressource_humaine = c.String(),
                        Date_emission_demande = c.DateTime(nullable: false),
                        jour_autorisation = c.DateTime(nullable: false),
                        heure_sortie = c.String(nullable: false),
                        heure_entree = c.String(nullable: false),
                        commentaire_superieur_hiearchique = c.String(),
                        commentaire_rh = c.String(),
                        acceptation_superieur = c.Int(nullable: false),
                        acceptation_ressource = c.Int(nullable: false),
                        nom_prenom = c.String(),
                        Approbateur_RH = c.String(),
                        site = c.String(),
                        Date_validation_superieur = c.DateTime(nullable: false),
                        Solde_Conge = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Calendrier_Direction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        matricule = c.String(),
                        nom_prenom = c.String(),
                        service = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChangePasswordViewModelbyAdmins",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CommandeLs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ref_cmd = c.String(),
                        Designation_medicament = c.String(),
                        Code_medicament = c.String(),
                        Quantite_Commandee = c.Int(nullable: false),
                        Quantite_acceptee = c.Int(nullable: false),
                        Nom_prenom = c.String(),
                        Matricule = c.String(),
                        Service = c.String(),
                        Validation_prt = c.Int(nullable: false),
                        Decision_prt = c.Int(nullable: false),
                        Decision_primaire = c.Int(nullable: false),
                        Approbateur_PRT = c.String(),
                        Approbateur_primaire = c.String(),
                        Commentaire_prt = c.String(),
                        Commentaire_primaire = c.String(),
                        Date_creation = c.DateTime(nullable: false),
                        list_lignes_valider = c.String(),
                        etat = c.Int(nullable: false),
                        Date_validation = c.DateTime(nullable: false),
                        user_validation = c.String(),
                        CommandeL_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CommandeLs", t => t.CommandeL_Id)
                .Index(t => t.CommandeL_Id);
            
            CreateTable(
                "dbo.Commandes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ref_cmd = c.String(),
                        Nom_prenom = c.String(),
                        Matricule = c.String(),
                        Service = c.String(),
                        Etat_commande_medicament = c.Int(nullable: false),
                        Date_commande = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Conges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserName = c.String(),
                        matricule = c.String(),
                        Fonction = c.String(),
                        service = c.String(),
                        superieur_hierarchique = c.String(),
                        charge_ressource_humaine = c.String(),
                        Date_emission_demande = c.DateTime(nullable: false),
                        jour_debut = c.DateTime(nullable: false),
                        jour_fin = c.DateTime(nullable: false),
                        heure_sortie = c.String(nullable: false),
                        heure_entree = c.String(nullable: false),
                        commentaire_superieur_hiearchique = c.String(),
                        commentaire_rh = c.String(),
                        acceptation_superieur = c.Int(nullable: false),
                        acceptation_ressource = c.Int(nullable: false),
                        nom_prenom = c.String(),
                        Approbateur_RH = c.String(),
                        site = c.String(),
                        Solde_Conge = c.Single(nullable: false),
                        Date_validation_superieur = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Destinataire = c.String(),
                        Email_Destinataire = c.String(),
                        Sujet = c.String(),
                        Message = c.String(),
                        Current_User_Event = c.String(),
                        Date_email = c.DateTime(nullable: false),
                        Etat_Envoi = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Start = c.String(),
                        End = c.String(),
                        Color = c.String(),
                        TextColor = c.String(),
                        Current_User = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Heure_Superieur",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        matricule_superieur = c.String(),
                        matricule_employer = c.String(),
                        nom_prenom_superieur = c.String(),
                        nom_prenom_employer = c.String(nullable: false),
                        Service = c.String(),
                        jour_planing = c.DateTime(nullable: false),
                        Commentaire = c.String(),
                        date_creation = c.DateTime(),
                        Approbation_Heure_Sup = c.Int(nullable: false),
                        list_employers = c.String(),
                        approbateur = c.String(),
                        date_approbation = c.DateTime(),
                        date_debut_prevu = c.String(),
                        date_fin_prevu = c.String(),
                        date_debut_pointage = c.String(),
                        date_fin_pointage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        image_id = c.String(),
                        UserId = c.String(),
                        UserName = c.String(),
                        titre = c.String(),
                        text = c.String(),
                        lien = c.String(),
                        date_upload = c.DateTime(),
                        suppresseur = c.String(),
                        date_suppression = c.DateTime(),
                        type_photo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.medicaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        code_medicament = c.String(),
                        Code_PCT = c.String(),
                        designation_medicament = c.String(),
                        validation_prt = c.Int(nullable: false),
                        lien_image = c.String(),
                        list_medicaments_commander = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MesHeuresSups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        matricule = c.String(),
                        nom_prenom = c.String(),
                        Total_HN = c.String(),
                        Total_HD = c.String(),
                        date = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.RoleViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        nom = c.String(nullable: false),
                        prenom = c.String(nullable: false),
                        matricule = c.String(nullable: false),
                        telephone = c.Int(nullable: false),
                        fonction = c.String(nullable: false),
                        site = c.String(),
                        service = c.String(),
                        date_naissance = c.DateTime(nullable: false),
                        date_recrutement = c.DateTime(nullable: false),
                        signataire = c.String(),
                        RoleName = c.Int(nullable: false),
                        Solde_Conge = c.Single(nullable: false),
                        Dernier_maj_solde_conge = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CommandeLs", "CommandeL_Id", "dbo.CommandeLs");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.CommandeLs", new[] { "CommandeL_Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.RoleViewModels");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.MesHeuresSups");
            DropTable("dbo.medicaments");
            DropTable("dbo.Images");
            DropTable("dbo.Heure_Superieur");
            DropTable("dbo.Events");
            DropTable("dbo.Emails");
            DropTable("dbo.Conges");
            DropTable("dbo.Commandes");
            DropTable("dbo.CommandeLs");
            DropTable("dbo.ChangePasswordViewModelbyAdmins");
            DropTable("dbo.Calendrier_Direction");
            DropTable("dbo.Autorisations");
            DropTable("dbo.Attestations");
        }
    }
}
