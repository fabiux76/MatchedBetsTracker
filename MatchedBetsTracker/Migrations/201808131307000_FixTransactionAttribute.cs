namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixTransactionAttribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "Amount", c => c.Double(nullable: false));
            DropColumn("dbo.Transactions", "Amoount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "Amoount", c => c.Double(nullable: false));
            DropColumn("dbo.Transactions", "Amount");
        }
    }
}
