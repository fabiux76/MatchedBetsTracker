namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSportEventMigrateData : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO SportEvents(EventDescription, EventDate, BetEventIdMigration) 
                  SELECT EventDescription, EventDate, Id
                  FROM BetEvents");

            Sql(@"UPDATE BE
                    SET BE.SportEventId = SE.Id
                    FROM BetEvents BE,
                         SportEvents SE
                    WHERE BE.Id = SE.BetEventIdMigration");

            Sql(@"UPDATE BE
                    SET BE.IsLay = B.BetType
                    FROM BetEvents BE,
                            Bets B
                    WHERE BE.BetId = B.Id
                    ");

            Sql(@"UPDATE SE
                    SET SE.Happened = 0
                    FROM SportEvents SE,
		                    BetEvents BE,
		                    Bets B
                    WHERE SE.Id = BE.Id AND
                            BE.BetId = B.Id AND
                            B.BetStatusId != 1 AND
		                    NOT ((B.BetStatusId = 2 AND B.BetType = 0) OR (B.BetStatusId = 3 AND B.BetType = 1))
                    ");

            Sql(@"UPDATE SE
                    SET SE.Happened = 1
                    FROM SportEvents SE,
		                    BetEvents BE,
		                    Bets B
                    WHERE SE.Id = BE.Id AND
                            BE.BetId = B.Id AND
                            B.BetStatusId != 1 AND
		                    ((B.BetStatusId = 2 AND B.BetType = 0) OR (B.BetStatusId = 3 AND B.BetType = 1))
                    ");

        }

        public override void Down()
        {
        }
    }
}
