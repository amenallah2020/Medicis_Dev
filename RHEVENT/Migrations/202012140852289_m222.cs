namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m222 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_ListFormationDiffus", "DateDiffus", c => c.String());
            AddColumn("dbo.E_ListFormationDiffus", "MatFormateur", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_ListFormationDiffus", "MatFormateur");
            DropColumn("dbo.E_ListFormationDiffus", "DateDiffus");
        }
    }
}
