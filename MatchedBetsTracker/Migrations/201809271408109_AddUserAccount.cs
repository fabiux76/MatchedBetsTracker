namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Bets", "UserAccountId", c => c.Int());
            AddColumn("dbo.BrokerAccounts", "OwnerId", c => c.Int());
            AddColumn("dbo.Transactions", "UserAccountId", c => c.Int());
            CreateIndex("dbo.Bets", "UserAccountId");
            CreateIndex("dbo.BrokerAccounts", "OwnerId");
            CreateIndex("dbo.Transactions", "UserAccountId");
            AddForeignKey("dbo.BrokerAccounts", "OwnerId", "dbo.UserAccounts", "Id");
            AddForeignKey("dbo.Transactions", "UserAccountId", "dbo.UserAccounts", "Id");
            AddForeignKey("dbo.Bets", "UserAccountId", "dbo.UserAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "UserAccountId", "dbo.UserAccounts");
            DropForeignKey("dbo.Transactions", "UserAccountId", "dbo.UserAccounts");
            DropForeignKey("dbo.BrokerAccounts", "OwnerId", "dbo.UserAccounts");
            DropIndex("dbo.Transactions", new[] { "UserAccountId" });
            DropIndex("dbo.BrokerAccounts", new[] { "OwnerId" });
            DropIndex("dbo.Bets", new[] { "UserAccountId" });
            DropColumn("dbo.Transactions", "UserAccountId");
            DropColumn("dbo.BrokerAccounts", "OwnerId");
            DropColumn("dbo.Bets", "UserAccountId");
            DropTable("dbo.UserAccounts");
        }
    }
}
