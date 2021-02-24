namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DA_DemUsersTraitees",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Matricule = c.String(),
                        Reference = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DA_LaboUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Matricule = c.String(nullable: false, maxLength: 10),
                        Laboratoire = c.String(maxLength: 100),
                        Etat = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DA_Produits", "Laboratoire", c => c.String(nullable: false));
            AlterColumn("dbo.DA_Labo", "Laboratoire", c => c.String());
            AlterColumn("dbo.DA_Produits", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.DA_Produits", "Désignation", c => c.String(nullable: false));
            DropColumn("dbo.DA_Labo", "Adresse");
            DropColumn("dbo.DA_Labo", "Tel");
            DropColumn("dbo.DA_Labo", "Mobile");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DA_Labo", "Mobile", c => c.String());
            AddColumn("dbo.DA_Labo", "Tel", c => c.String());
            AddColumn("dbo.DA_Labo", "Adresse", c => c.String());
            AlterColumn("dbo.DA_Produits", "Désignation", c => c.String());
            AlterColumn("dbo.DA_Produits", "Code", c => c.String());
            AlterColumn("dbo.DA_Labo", "Laboratoire", c => c.String(nullable: false));
            DropColumn("dbo.DA_Produits", "Laboratoire");
            DropTable("dbo.DA_LaboUser");
            DropTable("dbo.DA_DemUsersTraitees");
        }
    }
}
