namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mm3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DA_Labo", "Laboratoire", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DA_Labo", "Laboratoire", c => c.String(nullable: false));
        }
    }
}
