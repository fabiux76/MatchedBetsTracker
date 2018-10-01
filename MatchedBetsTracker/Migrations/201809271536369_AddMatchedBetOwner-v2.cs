namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchedBetOwnerv2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.MatchedBets", name: "UserAccount_Id", newName: "UserAccountId");
            RenameIndex(table: "dbo.MatchedBets", name: "IX_UserAccount_Id", newName: "IX_UserAccountId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.MatchedBets", name: "IX_UserAccountId", newName: "IX_UserAccount_Id");
            RenameColumn(table: "dbo.MatchedBets", name: "UserAccountId", newName: "UserAccount_Id");
        }
    }
}
