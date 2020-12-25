namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m223 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.E_ListFormationDiffus", "DateDiffus", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.E_ListFormationDiffus", "DateDiffus", c => c.String());
        }
    }
}
