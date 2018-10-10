namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendBetEventType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BetEvents", "BetEventType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BetEvents", "BetEventType");
        }
    }
}
