namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2251 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.E_Formation", "Etat_Diffusion");
            DropColumn("dbo.E_GrpByUsr", "Etat_Grp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_GrpByUsr", "Etat_Grp", c => c.String());
            AddColumn("dbo.E_Formation", "Etat_Diffusion", c => c.String());
        }
    }
}
