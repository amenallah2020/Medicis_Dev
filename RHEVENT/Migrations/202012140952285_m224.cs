namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m224 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.E_GrpByUsr", "Code_Formation");
            DropColumn("dbo.E_GrpByUsr", "IsSelected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_GrpByUsr", "IsSelected", c => c.Boolean(nullable: false));
            AddColumn("dbo.E_GrpByUsr", "Code_Formation", c => c.String());
        }
    }
}
