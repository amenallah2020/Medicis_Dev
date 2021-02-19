namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m112 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_DeadLlineEvalUsr",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code_eval = c.String(),
                        MatUser = c.String(),
                        Deadline = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.E_DeadLlineEvalUsr");
        }
    }
}
