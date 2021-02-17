namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m774 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_RepUser", "Ordre", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_RepUser", "Ordre");
        }
    }
}
