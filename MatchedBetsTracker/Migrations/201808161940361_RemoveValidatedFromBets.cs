namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveValidatedFromBets : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Bets", "Validated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Bets", "Validated", c => c.Boolean(nullable: false));
        }
    }
}
