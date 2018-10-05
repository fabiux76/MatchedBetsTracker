namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOldFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Bets", "BetType", c => c.Int(nullable: false));
            DropColumn("dbo.Bets", "EventDescription");
            DropColumn("dbo.Bets", "EventDate");
            DropColumn("dbo.Bets", "IsLay");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bets", "IsLay", c => c.Boolean(nullable: false));
            AddColumn("dbo.Bets", "EventDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Bets", "EventDescription", c => c.String());
            AlterColumn("dbo.Bets", "BetType", c => c.Int());
        }
    }
}
