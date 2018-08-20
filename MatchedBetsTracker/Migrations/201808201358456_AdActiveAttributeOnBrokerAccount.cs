namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdActiveAttributeOnBrokerAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BrokerAccounts", "Active", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BrokerAccounts", "Active");
        }
    }
}
