namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m7745 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.E_ResultQCM", "Reponse1User", c => c.String());
            AddColumn("dbo.E_ResultQCM", "Reponse2User", c => c.String());
            AddColumn("dbo.E_ResultQCM", "Reponse3User", c => c.String());
            AddColumn("dbo.E_ResultQCM", "Reponse4User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse1User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse2User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse3User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse4User", c => c.String());
            AddColumn("dbo.E_ResultQCMParSlide", "Reponse1User", c => c.String());
            AddColumn("dbo.E_ResultQCMParSlide", "Reponse2User", c => c.String());
            AddColumn("dbo.E_ResultQCMParSlide", "Reponse3User", c => c.String());
            AddColumn("dbo.E_ResultQCMParSlide", "Reponse4User", c => c.String());
            DropColumn("dbo.E_ResultQCM", "ReponseUser");
            DropColumn("dbo.E_ResultQCM_Historiq", "ReponseUser");
            DropColumn("dbo.E_ResultQCMParSlide", "ReponseUser");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_ResultQCMParSlide", "ReponseUser", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "ReponseUser", c => c.String());
            AddColumn("dbo.E_ResultQCM", "ReponseUser", c => c.String());
            DropColumn("dbo.E_ResultQCMParSlide", "Reponse4User");
            DropColumn("dbo.E_ResultQCMParSlide", "Reponse3User");
            DropColumn("dbo.E_ResultQCMParSlide", "Reponse2User");
            DropColumn("dbo.E_ResultQCMParSlide", "Reponse1User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse4User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse3User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse2User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse1User");
            DropColumn("dbo.E_ResultQCM", "Reponse4User");
            DropColumn("dbo.E_ResultQCM", "Reponse3User");
            DropColumn("dbo.E_ResultQCM", "Reponse2User");
            DropColumn("dbo.E_ResultQCM", "Reponse1User");
        }
    }
}
