namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSportEvent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SportEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventDescription = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        Happened = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BetEvents", "IsLay", c => c.Boolean(nullable: false));
            AddColumn("dbo.BetEvents", "SportEventId", c => c.Int());
            CreateIndex("dbo.BetEvents", "SportEventId");
            AddForeignKey("dbo.BetEvents", "SportEventId", "dbo.SportEvents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BetEvents", "SportEventId", "dbo.SportEvents");
            DropIndex("dbo.BetEvents", new[] { "SportEventId" });
            DropColumn("dbo.BetEvents", "SportEventId");
            DropColumn("dbo.BetEvents", "IsLay");
            DropTable("dbo.SportEvents");
        }
    }
}
