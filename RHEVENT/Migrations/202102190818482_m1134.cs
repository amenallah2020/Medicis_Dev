namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1134 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_DeadLlineEvalUsr", "Deadline", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_DeadLlineEvalUsr", "Deadline");
        }
    }
}
