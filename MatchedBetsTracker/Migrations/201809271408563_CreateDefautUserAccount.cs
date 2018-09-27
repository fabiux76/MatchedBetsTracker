namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDefautUserAccount : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO UserAccounts(Name) VALUES('Fabio')");

            Sql(@"UPDATE BrokerAccounts
                  SET OwnerID = (SELECT Id
                                 FROM UserAccounts
                                 WHERE Name = 'Fabio')");

            Sql(@"UPDATE Transactions
                  SET UserAccountId = (SELECT Id
                                        FROM UserAccounts
                                        WHERE Name = 'Fabio')");

            Sql(@"UPDATE Bets
                  SET UserAccountId = (SELECT Id
                                        FROM UserAccounts
                                        WHERE Name = 'Fabio')");
        }
        
        public override void Down()
        {
        }
    }
}
