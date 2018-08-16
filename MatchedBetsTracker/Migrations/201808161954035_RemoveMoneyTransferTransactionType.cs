namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMoneyTransferTransactionType : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM TransactionTypes WHERE Id = 1");
        }
        
        public override void Down()
        {
        }
    }
}
