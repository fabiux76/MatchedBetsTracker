namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSportEventHelperId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SportEvents", "BetEventIdMigration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SportEvents", "BetEventIdMigration");
        }
    }
}
