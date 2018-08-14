namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveBetReferenceInTransaction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "BetId", "dbo.Bets");
            DropIndex("dbo.Transactions", new[] { "BetId" });
            DropColumn("dbo.Transactions", "BetId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "BetId", c => c.Int());
            CreateIndex("dbo.Transactions", "BetId");
            AddForeignKey("dbo.Transactions", "BetId", "dbo.Bets", "Id");
        }
    }
}
