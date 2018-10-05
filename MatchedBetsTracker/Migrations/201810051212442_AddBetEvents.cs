namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBetEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BetEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventDescription = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        BetId = c.Int(nullable: false),
                        BetStatusId = c.Byte(nullable: false),
                        Quote = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bets", t => t.BetId, cascadeDelete: true)
                .ForeignKey("dbo.BetStatus", t => t.BetStatusId, cascadeDelete: false)
                .Index(t => t.BetId)
                .Index(t => t.BetStatusId);
            
            AddColumn("dbo.Bets", "BetType", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BetEvents", "BetStatusId", "dbo.BetStatus");
            DropForeignKey("dbo.BetEvents", "BetId", "dbo.Bets");
            DropIndex("dbo.BetEvents", new[] { "BetStatusId" });
            DropIndex("dbo.BetEvents", new[] { "BetId" });
            DropColumn("dbo.Bets", "BetType");
            DropTable("dbo.BetEvents");
        }
    }
}
