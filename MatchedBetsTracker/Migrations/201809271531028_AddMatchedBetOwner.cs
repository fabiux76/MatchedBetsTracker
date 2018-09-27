namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchedBetOwner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MatchedBets", "UserAccount_Id", c => c.Int());
            CreateIndex("dbo.MatchedBets", "UserAccount_Id");
            AddForeignKey("dbo.MatchedBets", "UserAccount_Id", "dbo.UserAccounts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MatchedBets", "UserAccount_Id", "dbo.UserAccounts");
            DropIndex("dbo.MatchedBets", new[] { "UserAccount_Id" });
            DropColumn("dbo.MatchedBets", "UserAccount_Id");
        }
    }
}
