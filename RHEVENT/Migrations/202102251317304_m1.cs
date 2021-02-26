namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.E_DeadLlineEvalUsr",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code_eval = c.String(),
                        MatUser = c.String(),
                        Deadline = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.E_ResultQCMParSlide",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code_EvalByQCM = c.String(),
                        CodeForm = c.String(),
                        Code_QCM = c.String(),
                        MatUser = c.String(),
                        Usr = c.String(),
                        DateEval = c.DateTime(nullable: false),
                        Score = c.Int(),
                        Resultat = c.String(),
                        ObjEval = c.String(),
                        ObjForm = c.String(),
                        DeadLine = c.DateTime(nullable: false),
                        Question = c.String(),
                        Coeff = c.Int(nullable: false),
                        Reponse1 = c.String(),
                        EtatRep1 = c.String(),
                        Reponse2 = c.String(),
                        EtatRep2 = c.String(),
                        Reponse3 = c.String(),
                        EtatRep3 = c.String(),
                        Reponse4 = c.String(),
                        EtatRep4 = c.String(),
                        Reponse1User = c.String(),
                        Reponse2User = c.String(),
                        Reponse3User = c.String(),
                        Reponse4User = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.E_RepUser", "Ordre", c => c.Int(nullable: false));
            AddColumn("dbo.E_ResultQCM", "Reponse1User", c => c.String());
            AddColumn("dbo.E_ResultQCM", "Reponse2User", c => c.String());
            AddColumn("dbo.E_ResultQCM", "Reponse3User", c => c.String());
            AddColumn("dbo.E_ResultQCM", "Reponse4User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse1User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse2User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse3User", c => c.String());
            AddColumn("dbo.E_ResultQCM_Historiq", "Reponse4User", c => c.String());
            DropColumn("dbo.E_ResultQCM", "ReponseUser");
            DropColumn("dbo.E_ResultQCM_Historiq", "ReponseUser");
        }
        
        public override void Down()
        {
            AddColumn("dbo.E_ResultQCM_Historiq", "ReponseUser", c => c.String());
            AddColumn("dbo.E_ResultQCM", "ReponseUser", c => c.String());
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse4User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse3User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse2User");
            DropColumn("dbo.E_ResultQCM_Historiq", "Reponse1User");
            DropColumn("dbo.E_ResultQCM", "Reponse4User");
            DropColumn("dbo.E_ResultQCM", "Reponse3User");
            DropColumn("dbo.E_ResultQCM", "Reponse2User");
            DropColumn("dbo.E_ResultQCM", "Reponse1User");
            DropColumn("dbo.E_RepUser", "Ordre");
            DropTable("dbo.E_ResultQCMParSlide");
            DropTable("dbo.E_DeadLlineEvalUsr");
        }
    }
}
