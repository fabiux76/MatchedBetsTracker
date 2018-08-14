namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestoreBetReferenceInTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "BetId", c => c.Int());
            CreateIndex("dbo.Transactions", "BetId");
            AddForeignKey("dbo.Transactions", "BetId", "dbo.Bets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "BetId", "dbo.Bets");
            DropIndex("dbo.Transactions", new[] { "BetId" });
            DropColumn("dbo.Transactions", "BetId");
        }
    }
}
