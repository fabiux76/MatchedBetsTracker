namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeMatchedBetReferenceOptionalStep2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "MatchedBetId", c => c.Int());
            CreateIndex("dbo.Bets", "MatchedBetId");
            AddForeignKey("dbo.Bets", "MatchedBetId", "dbo.MatchedBets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "MatchedBetId", "dbo.MatchedBets");
            DropIndex("dbo.Bets", new[] { "MatchedBetId" });
            DropColumn("dbo.Bets", "MatchedBetId");
        }
    }
}
