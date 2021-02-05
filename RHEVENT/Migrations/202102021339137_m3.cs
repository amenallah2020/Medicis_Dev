namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DA_Demande", "MotifRejet", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DA_Demande", "MotifRejet");
        }
    }
}
