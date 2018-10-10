namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendBetEventTypeMigrateData : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE BetEvents 
                  SET BetEventType = 1
                  WHERE IsLay = 0");

            Sql(@"UPDATE BetEvents 
                  SET BetEventType = 3
                  WHERE IsLay = 1");
        }
        
        public override void Down()
        {
        }
    }
}
