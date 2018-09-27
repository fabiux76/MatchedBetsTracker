namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchedBetOwnerv2Populate : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE MatchedBets
                  SET UserAccountId = (SELECT Id
                                 FROM UserAccounts
                                 WHERE Name = 'Fabio')");
        }
        
        public override void Down()
        {
        }
    }
}
