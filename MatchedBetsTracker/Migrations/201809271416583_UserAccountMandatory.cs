namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAccountMandatory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bets", "UserAccountId", "dbo.UserAccounts");
            DropForeignKey("dbo.BrokerAccounts", "OwnerId", "dbo.UserAccounts");
            DropForeignKey("dbo.Transactions", "UserAccountId", "dbo.UserAccounts");
            DropIndex("dbo.Bets", new[] { "UserAccountId" });
            DropIndex("dbo.BrokerAccounts", new[] { "OwnerId" });
            DropIndex("dbo.Transactions", new[] { "UserAccountId" });
            AlterColumn("dbo.Bets", "UserAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.BrokerAccounts", "OwnerId", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "UserAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.Bets", "UserAccountId");
            CreateIndex("dbo.BrokerAccounts", "OwnerId");
            CreateIndex("dbo.Transactions", "UserAccountId");
            AddForeignKey("dbo.Bets", "UserAccountId", "dbo.UserAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.BrokerAccounts", "OwnerId", "dbo.UserAccounts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Transactions", "UserAccountId", "dbo.UserAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "UserAccountId", "dbo.UserAccounts");
            DropForeignKey("dbo.BrokerAccounts", "OwnerId", "dbo.UserAccounts");
            DropForeignKey("dbo.Bets", "UserAccountId", "dbo.UserAccounts");
            DropIndex("dbo.Transactions", new[] { "UserAccountId" });
            DropIndex("dbo.BrokerAccounts", new[] { "OwnerId" });
            DropIndex("dbo.Bets", new[] { "UserAccountId" });
            AlterColumn("dbo.Transactions", "UserAccountId", c => c.Int());
            AlterColumn("dbo.BrokerAccounts", "OwnerId", c => c.Int());
            AlterColumn("dbo.Bets", "UserAccountId", c => c.Int());
            CreateIndex("dbo.Transactions", "UserAccountId");
            CreateIndex("dbo.BrokerAccounts", "OwnerId");
            CreateIndex("dbo.Bets", "UserAccountId");
            AddForeignKey("dbo.Transactions", "UserAccountId", "dbo.UserAccounts", "Id");
            AddForeignKey("dbo.BrokerAccounts", "OwnerId", "dbo.UserAccounts", "Id");
            AddForeignKey("dbo.Bets", "UserAccountId", "dbo.UserAccounts", "Id");
        }
    }
}
