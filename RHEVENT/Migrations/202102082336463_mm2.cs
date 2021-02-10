namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mm2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DA_Labo", "Adresse");
            DropColumn("dbo.DA_Labo", "Tel");
            DropColumn("dbo.DA_Labo", "Mobile");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DA_Labo", "Mobile", c => c.String());
            AddColumn("dbo.DA_Labo", "Tel", c => c.String());
            AddColumn("dbo.DA_Labo", "Adresse", c => c.String());
        }
    }
}
