namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m113 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.E_DeadLlineEvalUsr", "Deadline");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_DeadLlineEvalUsr", "Deadline", c => c.String());
        }
    }
}
