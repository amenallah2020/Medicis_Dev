namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1122 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.DA_Fournisseurs", "Ix_Raison");
            DropIndex("dbo.DA_TypesActions", "Ix_TypeAction");
            CreateTable(
                "dbo.DA_CodesArticlesSage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_Materiels_Dem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Réference = c.String(),
                        Code = c.String(nullable: false),
                        Désignation = c.String(nullable: false),
                        Date_Recp_Souh = c.DateTime(storeType: "date"),
                        Type = c.String(nullable: false),
                        PlafondBudget = c.String(),
                        Fournisseur = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_Produits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Désignation = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_ProduitsDem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Réference = c.String(),
                        Laboratoire = c.String(nullable: false),
                        Pourcentage = c.Single(nullable: false),
                        Code = c.String(),
                        Montant = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_WorkflowTypAch",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_type = c.Int(nullable: false),
                        Num = c.Int(nullable: false),
                        Intervenant = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dictionnaires",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        table = c.String(nullable: false),
                        champ = c.String(nullable: false),
                        valeur = c.String(nullable: false),
                        signification = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FonctionsUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fonction = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MotifsRejets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MotifRejet = c.String(nullable: false, maxLength: 100),
                        Conséquense = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.MotifRejet, unique: true, name: "Ix_MotifRejet");
            
            AddColumn("dbo.DA_Budget", "Article", c => c.String(nullable: false));
            AddColumn("dbo.DA_Budget", "Description", c => c.String());
            AddColumn("dbo.DA_Budget", "Date_Recp_Souh", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.DA_Budget", "Date_Recp", c => c.String());
            AddColumn("dbo.DA_Budget", "Type", c => c.String());
            AddColumn("dbo.DA_Budget", "PlafondBudget", c => c.String());
            AddColumn("dbo.DA_Demande", "Labo", c => c.String(nullable: false));
            AddColumn("dbo.DA_Demande", "TypeAchat", c => c.String(nullable: false));
            AddColumn("dbo.DA_Demande", "TypeAction", c => c.String(nullable: false));
            AddColumn("dbo.DA_Demande", "Statut", c => c.String());
            AddColumn("dbo.DA_Demande", "etat_prochain", c => c.String());
            AddColumn("dbo.DA_Demande", "Validee", c => c.String());
            AddColumn("dbo.DA_Demande", "AvecSans", c => c.String());
            AddColumn("dbo.DA_Demande", "Lieu", c => c.String());
            AddColumn("dbo.DA_Demande", "matrsign", c => c.String());
            AddColumn("dbo.DA_Demande", "MotifRejet", c => c.String());
            AddColumn("dbo.DA_Labo", "Code", c => c.String(nullable: false));
            AddColumn("dbo.DA_Materiels", "Type", c => c.String(nullable: false));
            AddColumn("dbo.DA_Materiels", "PlafondBudget", c => c.String());
            AddColumn("dbo.DA_TypesAchats", "Code", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.DA_TypesAchats", "Workflow", c => c.String());
            AddColumn("dbo.DA_TypesAchats", "typworkflow_Id", c => c.Int());
            AddColumn("dbo.DA_TypesActions", "TypeActhat", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.DA_Budget", "PrixUnitaire", c => c.Single(nullable: false));
            AlterColumn("dbo.DA_Budget", "Quantité", c => c.Int(nullable: false));
            AlterColumn("dbo.DA_Budget", "Total", c => c.Single(nullable: false));
            AlterColumn("dbo.DA_Demande", "Date_demande", c => c.DateTime(storeType: "date"));
            AlterColumn("dbo.DA_Fournisseurs", "Code", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.DA_Fournisseurs", "Raison", c => c.String());
            AlterColumn("dbo.DA_TypesActions", "TypeAction", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.DA_TypesAchats", "typworkflow_Id");
            AddForeignKey("dbo.DA_TypesAchats", "typworkflow_Id", "dbo.DA_WorkflowTypAch", "Id");
            DropColumn("dbo.DA_Budget", "BudgetElementaire");
            DropColumn("dbo.DA_Budget", "AL");
            DropColumn("dbo.DA_Demande", "Cibles");
            DropColumn("dbo.DA_Fournisseurs", "Adresse");
            DropColumn("dbo.DA_Fournisseurs", "Tel");
            DropColumn("dbo.DA_Fournisseurs", "Mobile");
            DropColumn("dbo.DA_Materiels", "Version");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DA_Materiels", "Version", c => c.String(nullable: false));
            AddColumn("dbo.DA_Fournisseurs", "Mobile", c => c.String());
            AddColumn("dbo.DA_Fournisseurs", "Tel", c => c.String());
            AddColumn("dbo.DA_Fournisseurs", "Adresse", c => c.String());
            AddColumn("dbo.DA_Demande", "Cibles", c => c.String(nullable: false));
            AddColumn("dbo.DA_Budget", "AL", c => c.String());
            AddColumn("dbo.DA_Budget", "BudgetElementaire", c => c.Single(nullable: false));
            DropForeignKey("dbo.DA_TypesAchats", "typworkflow_Id", "dbo.DA_WorkflowTypAch");
            DropIndex("dbo.MotifsRejets", "Ix_MotifRejet");
            DropIndex("dbo.DA_TypesAchats", new[] { "typworkflow_Id" });
            AlterColumn("dbo.DA_TypesActions", "TypeAction", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.DA_Fournisseurs", "Raison", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.DA_Fournisseurs", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.DA_Demande", "Date_demande", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.DA_Budget", "Total", c => c.String());
            AlterColumn("dbo.DA_Budget", "Quantité", c => c.String());
            AlterColumn("dbo.DA_Budget", "PrixUnitaire", c => c.String());
            DropColumn("dbo.DA_TypesActions", "TypeActhat");
            DropColumn("dbo.DA_TypesAchats", "typworkflow_Id");
            DropColumn("dbo.DA_TypesAchats", "Workflow");
            DropColumn("dbo.DA_TypesAchats", "Code");
            DropColumn("dbo.DA_Materiels", "PlafondBudget");
            DropColumn("dbo.DA_Materiels", "Type");
            DropColumn("dbo.DA_Labo", "Code");
            DropColumn("dbo.DA_Demande", "MotifRejet");
            DropColumn("dbo.DA_Demande", "matrsign");
            DropColumn("dbo.DA_Demande", "Lieu");
            DropColumn("dbo.DA_Demande", "AvecSans");
            DropColumn("dbo.DA_Demande", "Validee");
            DropColumn("dbo.DA_Demande", "etat_prochain");
            DropColumn("dbo.DA_Demande", "Statut");
            DropColumn("dbo.DA_Demande", "TypeAction");
            DropColumn("dbo.DA_Demande", "TypeAchat");
            DropColumn("dbo.DA_Demande", "Labo");
            DropColumn("dbo.DA_Budget", "PlafondBudget");
            DropColumn("dbo.DA_Budget", "Type");
            DropColumn("dbo.DA_Budget", "Date_Recp");
            DropColumn("dbo.DA_Budget", "Date_Recp_Souh");
            DropColumn("dbo.DA_Budget", "Description");
            DropColumn("dbo.DA_Budget", "Article");
            DropTable("dbo.MotifsRejets");
            DropTable("dbo.FonctionsUsers");
            DropTable("dbo.Dictionnaires");
            DropTable("dbo.DA_WorkflowTypAch");
            DropTable("dbo.DA_ProduitsDem");
            DropTable("dbo.DA_Produits");
            DropTable("dbo.DA_Materiels_Dem");
            DropTable("dbo.DA_CodesArticlesSage");
            CreateIndex("dbo.DA_TypesActions", "TypeAction", unique: true, name: "Ix_TypeAction");
            CreateIndex("dbo.DA_Fournisseurs", "Raison", unique: true, name: "Ix_Raison");
        }
    }
}
