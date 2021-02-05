namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m6 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DA_Budget", "Date_Recp_Souh", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DA_Budget", "Date_Recp_Souh", c => c.DateTime(storeType: "date"));
        }
    }
}
