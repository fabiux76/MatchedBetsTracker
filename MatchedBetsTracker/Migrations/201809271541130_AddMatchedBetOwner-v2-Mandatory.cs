namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchedBetOwnerv2Mandatory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MatchedBets", "UserAccountId", "dbo.UserAccounts");
            DropIndex("dbo.MatchedBets", new[] { "UserAccountId" });
            AlterColumn("dbo.MatchedBets", "UserAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.MatchedBets", "UserAccountId");
            AddForeignKey("dbo.MatchedBets", "UserAccountId", "dbo.UserAccounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MatchedBets", "UserAccountId", "dbo.UserAccounts");
            DropIndex("dbo.MatchedBets", new[] { "UserAccountId" });
            AlterColumn("dbo.MatchedBets", "UserAccountId", c => c.Int());
            CreateIndex("dbo.MatchedBets", "UserAccountId");
            AddForeignKey("dbo.MatchedBets", "UserAccountId", "dbo.UserAccounts", "Id");
        }
    }
}
