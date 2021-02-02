namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m44 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DA_Budget",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Réference = c.String(),
                        BudgetElementaire = c.Single(nullable: false),
                        PrixUnitaire = c.String(),
                        Quantité = c.String(),
                        Total = c.String(),
                        Fournisseur = c.String(),
                        AL = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_Demande",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Réference = c.String(),
                        Demandeur = c.String(),
                        Gamme = c.String(),
                        Objet = c.String(),
                        Cibles = c.String(nullable: false),
                        Argumentaires = c.String(),
                        Budget = c.Single(nullable: false),
                        Date_demande = c.DateTime(nullable: false, storeType: "date"),
                        Date_reception = c.DateTime(nullable: false, storeType: "date"),
                        Date_action = c.DateTime(nullable: false, storeType: "date"),
                        Etat = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_Fournisseurs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Raison = c.String(nullable: false, maxLength: 50),
                        Adresse = c.String(),
                        Tel = c.String(),
                        Mobile = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Raison, unique: true, name: "Ix_Raison");
            
            CreateTable(
                "dbo.DA_Labo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Laboratoire = c.String(nullable: false),
                        Adresse = c.String(),
                        Tel = c.String(),
                        Mobile = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_ListesGammes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Gamme = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Gamme, unique: true, name: "Ix_Gamme");
            
            CreateTable(
                "dbo.DA_Materiels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Désignation = c.String(nullable: false),
                        Version = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_TypesAchats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeAchat = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.TypeAchat, unique: true, name: "Ix_TypeAchat");
            
            CreateTable(
                "dbo.DA_TypesActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeAction = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.TypeAction, unique: true, name: "Ix_TypeAction");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.DA_TypesActions", "Ix_TypeAction");
            DropIndex("dbo.DA_TypesAchats", "Ix_TypeAchat");
            DropIndex("dbo.DA_ListesGammes", "Ix_Gamme");
            DropIndex("dbo.DA_Fournisseurs", "Ix_Raison");
            DropTable("dbo.DA_TypesActions");
            DropTable("dbo.DA_TypesAchats");
            DropTable("dbo.DA_Materiels");
            DropTable("dbo.DA_ListesGammes");
            DropTable("dbo.DA_Labo");
            DropTable("dbo.DA_Fournisseurs");
            DropTable("dbo.DA_Demande");
            DropTable("dbo.DA_Budget");
        }
    }
}
