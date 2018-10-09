namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSportEventClean : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BetEvents", "SportEventId", "dbo.SportEvents");
            DropIndex("dbo.BetEvents", new[] { "SportEventId" });
            AlterColumn("dbo.BetEvents", "SportEventId", c => c.Int(nullable: false));
            CreateIndex("dbo.BetEvents", "SportEventId");
            AddForeignKey("dbo.BetEvents", "SportEventId", "dbo.SportEvents", "Id", cascadeDelete: true);
            DropColumn("dbo.BetEvents", "EventDescription");
            DropColumn("dbo.BetEvents", "EventDate");
            DropColumn("dbo.SportEvents", "BetEventIdMigration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SportEvents", "BetEventIdMigration", c => c.Int(nullable: false));
            AddColumn("dbo.BetEvents", "EventDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.BetEvents", "EventDescription", c => c.String());
            DropForeignKey("dbo.BetEvents", "SportEventId", "dbo.SportEvents");
            DropIndex("dbo.BetEvents", new[] { "SportEventId" });
            AlterColumn("dbo.BetEvents", "SportEventId", c => c.Int());
            CreateIndex("dbo.BetEvents", "SportEventId");
            AddForeignKey("dbo.BetEvents", "SportEventId", "dbo.SportEvents", "Id");
        }
    }
}
