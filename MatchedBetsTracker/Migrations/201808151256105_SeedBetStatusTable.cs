namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedBetStatusTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO BetStatus(Id, Description) VALUES (1, 'Open')");
            Sql("INSERT INTO BetStatus(Id, Description) VALUES (2, 'Won')");
            Sql("INSERT INTO BetStatus(Id, Description) VALUES (3, 'Loss')");
        }
        
        public override void Down()
        {
        }
    }
}
