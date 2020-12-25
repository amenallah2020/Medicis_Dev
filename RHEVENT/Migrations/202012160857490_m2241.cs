namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2241 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_ListFormationDiffus", "Objet", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_ListFormationDiffus", "Objet");
        }
    }
}
