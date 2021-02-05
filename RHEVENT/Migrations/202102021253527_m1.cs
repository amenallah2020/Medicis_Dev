namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MotifsRejets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MotifRejet = c.String(nullable: false, maxLength: 100),
                        Conséquense = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.MotifRejet, unique: true, name: "Ix_Motif");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.MotifsRejets", "Ix_Motif");
            DropTable("dbo.MotifsRejets");
        }
    }
}
