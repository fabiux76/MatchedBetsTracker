namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeBrokerAccountValidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BrokerAccounts", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BrokerAccounts", "Name", c => c.String());
        }
    }
}
