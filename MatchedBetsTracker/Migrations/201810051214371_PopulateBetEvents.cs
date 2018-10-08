namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateBetEvents : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO BetEvents(BetId, BetStatusId, Quote, EventDescription, EventDate) 
                  SELECT Id, BetStatusId, Quote, EventDescription, EventDate
                  FROM Bets");
        }
        
        public override void Down()
        {
        }
    }
}
