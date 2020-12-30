namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class da19 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MesDemAjouts",
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MesDemAjouts");
        }
    }
}
