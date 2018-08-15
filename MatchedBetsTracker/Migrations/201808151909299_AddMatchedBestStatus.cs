namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMatchedBestStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MatchedBets", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MatchedBets", "Status");
        }
    }
}
