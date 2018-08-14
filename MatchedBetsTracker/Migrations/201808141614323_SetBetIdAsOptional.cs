namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetBetIdAsOptional : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "BetId", "dbo.Bets");
            DropIndex("dbo.Transactions", new[] { "BetId" });
            AlterColumn("dbo.Transactions", "BetId", c => c.Int());
            CreateIndex("dbo.Transactions", "BetId");
            AddForeignKey("dbo.Transactions", "BetId", "dbo.Bets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "BetId", "dbo.Bets");
            DropIndex("dbo.Transactions", new[] { "BetId" });
            AlterColumn("dbo.Transactions", "BetId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "BetId");
            AddForeignKey("dbo.Transactions", "BetId", "dbo.Bets", "Id", cascadeDelete: true);
        }
    }
}
