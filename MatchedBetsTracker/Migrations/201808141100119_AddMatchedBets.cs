namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchedBets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchedBets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Bets", "MatchedBetId");
            AddForeignKey("dbo.Bets", "MatchedBetId", "dbo.MatchedBets", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "MatchedBetId", "dbo.MatchedBets");
            DropIndex("dbo.Bets", new[] { "MatchedBetId" });
            DropTable("dbo.MatchedBets");
        }
    }
}
