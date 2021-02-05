namespace RHEVENT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m2 : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.MotifsRejets", name: "Ix_Motif", newName: "Ix_MotifRejet");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MotifsRejets", name: "Ix_MotifRejet", newName: "Ix_Motif");
        }
    }
}
