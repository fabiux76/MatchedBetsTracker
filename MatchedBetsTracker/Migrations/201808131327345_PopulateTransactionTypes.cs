namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTransactionTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionTypes", "TransactionDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransactionTypes", "TransactionDescription");
        }
    }
}
