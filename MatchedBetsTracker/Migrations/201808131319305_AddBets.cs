namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventDescription = c.String(),
                        BetDescription = c.String(),
                        BetDate = c.DateTime(nullable: false),
                        EventDate = c.DateTime(nullable: false),
                        Validated = c.Boolean(nullable: false),
                        IsLay = c.Boolean(nullable: false),
                        Quote = c.Double(nullable: false),
                        BetAmount = c.Double(nullable: false),
                        Responsability = c.Double(nullable: false),
                        ProfitLoss = c.Double(nullable: false),
                        MatchedBetId = c.Int(nullable: false),
                        Status_Id = c.Byte(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BetStatus", t => t.Status_Id)
                .Index(t => t.Status_Id);
            
            CreateTable(
                "dbo.BetStatus",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bets", "Status_Id", "dbo.BetStatus");
            DropIndex("dbo.Bets", new[] { "Status_Id" });
            DropTable("dbo.BetStatus");
            DropTable("dbo.Bets");
        }
    }
}
