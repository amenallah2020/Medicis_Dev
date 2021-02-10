namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mm1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DA_Produits", "Laboratoire", c => c.String(nullable: false));
            AlterColumn("dbo.DA_Produits", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.DA_Produits", "Désignation", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DA_Produits", "Désignation", c => c.String());
            AlterColumn("dbo.DA_Produits", "Code", c => c.String());
            DropColumn("dbo.DA_Produits", "Laboratoire");
        }
    }
}
