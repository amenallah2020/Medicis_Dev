namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class da4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DA_Budget", "PourRecep", c => c.String());
            AddColumn("dbo.DA_Budget", "QteRecue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DA_Budget", "QteRecue");
            DropColumn("dbo.DA_Budget", "PourRecep");
        }
    }
}
