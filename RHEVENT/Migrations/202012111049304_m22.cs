namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_GrpByUsr", "IsSelected", c => c.Boolean(nullable: false));
            DropColumn("dbo.E_GrpByUsr", "NomPrenom_Usr");
            DropColumn("dbo.E_GrpByUsr", "Mat_usr_by_grp");
            DropColumn("dbo.E_GrpByUsr", "Nom_usr_by_grp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_GrpByUsr", "Nom_usr_by_grp", c => c.String());
            AddColumn("dbo.E_GrpByUsr", "Mat_usr_by_grp", c => c.String());
            AddColumn("dbo.E_GrpByUsr", "NomPrenom_Usr", c => c.String());
            DropColumn("dbo.E_GrpByUsr", "IsSelected");
        }
    }
}
