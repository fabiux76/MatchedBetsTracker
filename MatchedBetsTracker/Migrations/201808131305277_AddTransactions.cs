namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amoount = c.Double(nullable: false),
                        Validated = c.Boolean(nullable: false),
                        TransactionType_Id = c.Byte(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransactionTypes", t => t.TransactionType_Id)
                .Index(t => t.TransactionType_Id);
            
            CreateTable(
                "dbo.TransactionTypes",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "TransactionType_Id", "dbo.TransactionTypes");
            DropIndex("dbo.Transactions", new[] { "TransactionType_Id" });
            DropTable("dbo.TransactionTypes");
            DropTable("dbo.Transactions");
        }
    }
}
