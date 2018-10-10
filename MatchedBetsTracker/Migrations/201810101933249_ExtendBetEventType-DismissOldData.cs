namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendBetEventTypeDismissOldData : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BetEvents", "IsLay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BetEvents", "IsLay", c => c.Boolean(nullable: false));
        }
    }
}
