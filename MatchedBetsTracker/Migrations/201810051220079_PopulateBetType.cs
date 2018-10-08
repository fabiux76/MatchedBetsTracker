namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateBetType : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE Bets
                  SET BetType = 0
                  WHERE IsLay = 0");

            Sql(@"UPDATE Bets
                  SET BetType = 1
                  WHERE IsLay = 1");
        }
        
        public override void Down()
        {
        }
    }
}
