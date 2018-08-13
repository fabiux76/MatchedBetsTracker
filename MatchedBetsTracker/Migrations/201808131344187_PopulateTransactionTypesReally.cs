namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTransactionTypesReally : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TransactionTypes(Id, TransactionDescription) VALUES (1, 'MoneyTransfer')");
            Sql("INSERT INTO TransactionTypes(Id, TransactionDescription) VALUES (2, 'OpenBet')");
            Sql("INSERT INTO TransactionTypes(Id, TransactionDescription) VALUES (3, 'CreditBet')");
            Sql("INSERT INTO TransactionTypes(Id, TransactionDescription) VALUES (4, 'CreditBonus')");
            Sql("INSERT INTO TransactionTypes(Id, TransactionDescription) VALUES (5, 'ExpireBonus')");
        }
        
        public override void Down()
        {
        }
    }
}
