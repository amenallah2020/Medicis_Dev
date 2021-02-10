namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mm4 : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DA_DemUsersTraitees");
        }
    }
}
