namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1232 : DbMigration
    {
        public override void Up()
        {
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
                        ReponseUser = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.E_ResultQCMParSlide");
        }
    }
}
