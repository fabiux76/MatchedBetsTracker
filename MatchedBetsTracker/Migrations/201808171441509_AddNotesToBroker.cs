namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotesToBroker : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BrokerAccounts", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BrokerAccounts", "Notes");
        }
    }
}
