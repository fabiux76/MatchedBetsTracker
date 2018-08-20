namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCorrectionTransactionType : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TransactionTypes(Id, TransactionDescription) VALUES (8, 'Correction')");
        }
        
        public override void Down()
        {
        }
    }
}
