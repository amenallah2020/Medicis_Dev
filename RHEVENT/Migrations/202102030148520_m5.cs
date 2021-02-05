namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DA_Budget", "Date_Recp", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DA_Budget", "Date_Recp", c => c.DateTime(storeType: "date"));
        }
    }
}
