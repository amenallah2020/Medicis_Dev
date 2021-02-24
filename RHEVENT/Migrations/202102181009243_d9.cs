namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class d9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DA_Demande", "MatriculeDem", c => c.String());
            AlterColumn("dbo.DA_Budget", "PrixUnitaire", c => c.String(nullable: false));
            AlterColumn("dbo.DA_Budget", "Total", c => c.String());
            AlterColumn("dbo.DA_Demande", "Budget", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DA_Demande", "Budget", c => c.Single(nullable: false));
            AlterColumn("dbo.DA_Budget", "Total", c => c.Single(nullable: false));
            AlterColumn("dbo.DA_Budget", "PrixUnitaire", c => c.Single(nullable: false));
            DropColumn("dbo.DA_Demande", "MatriculeDem");
        }
    }
}
