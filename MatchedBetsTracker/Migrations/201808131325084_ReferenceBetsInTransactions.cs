namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceBetsInTransactions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "Status_Id", "dbo.BetStatus");
            DropIndex("dbo.Bets", new[] { "Status_Id" });
            RenameColumn(table: "dbo.Bets", name: "Status_Id", newName: "BetStatusId");
            AddColumn("dbo.Transactions", "BetId", c => c.Int(nullable: false));
            AlterColumn("dbo.Bets", "BetStatusId", c => c.Byte(nullable: false));
            CreateIndex("dbo.Bets", "BetStatusId");
            CreateIndex("dbo.Transactions", "BetId");
            AddForeignKey("dbo.Transactions", "BetId", "dbo.Bets", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Bets", "BetStatusId", "dbo.BetStatus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "BetStatusId", "dbo.BetStatus");
            DropForeignKey("dbo.Transactions", "BetId", "dbo.Bets");
            DropIndex("dbo.Transactions", new[] { "BetId" });
            DropIndex("dbo.Bets", new[] { "BetStatusId" });
            AlterColumn("dbo.Bets", "BetStatusId", c => c.Byte());
            DropColumn("dbo.Transactions", "BetId");
            RenameColumn(table: "dbo.Bets", name: "BetStatusId", newName: "Status_Id");
            CreateIndex("dbo.Bets", "Status_Id");
            AddForeignKey("dbo.Bets", "Status_Id", "dbo.BetStatus", "Id");
        }
    }
}
