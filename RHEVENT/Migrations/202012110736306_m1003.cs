namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1003 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_ListFormationDiffus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mat_usr = c.String(),
                        Nom_usr = c.String(),
                        Code_grp = c.String(),
                        Code_formt = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.E_listeUsr", "SelectUsr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.E_listeUsr", "SelectUsr");
            DropTable("dbo.E_ListFormationDiffus");
        }
    }
}
