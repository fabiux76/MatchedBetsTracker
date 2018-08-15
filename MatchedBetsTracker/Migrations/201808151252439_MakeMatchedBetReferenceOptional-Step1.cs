namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeMatchedBetReferenceOptionalStep1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "MatchedBetId", "dbo.MatchedBets");
            DropIndex("dbo.Bets", new[] { "MatchedBetId" });
            DropColumn("dbo.Bets", "MatchedBetId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bets", "MatchedBetId", c => c.Int(nullable: false));
            CreateIndex("dbo.Bets", "MatchedBetId");
            AddForeignKey("dbo.Bets", "MatchedBetId", "dbo.MatchedBets", "Id", cascadeDelete: true);
        }
    }
}
