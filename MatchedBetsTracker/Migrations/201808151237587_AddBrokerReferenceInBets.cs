namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBrokerReferenceInBets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bets", "BrokerAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.Bets", "BrokerAccountId");
            AddForeignKey("dbo.Bets", "BrokerAccountId", "dbo.BrokerAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "BrokerAccountId", "dbo.BrokerAccounts");
            DropIndex("dbo.Bets", new[] { "BrokerAccountId" });
            DropColumn("dbo.Bets", "BrokerAccountId");
        }
    }
}
