namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m445 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_Formation", "ImageName", c => c.String());
            AddColumn("dbo.E_Formation", "NumDiapo", c => c.Int(nullable: false));
            AddColumn("dbo.E_Formation", "Chemin", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_Formation", "Chemin");
            DropColumn("dbo.E_Formation", "NumDiapo");
            DropColumn("dbo.E_Formation", "ImageName");
        }
    }
}
