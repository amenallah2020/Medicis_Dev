namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m225 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.E_listeUsr", "Code_formt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_listeUsr", "Code_formt", c => c.String());
        }
    }
}
