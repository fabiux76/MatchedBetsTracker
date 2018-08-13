namespace MatchedBetsTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefToBrokerAccountInTransactions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "TransactionType_Id", "dbo.TransactionTypes");
            DropIndex("dbo.Transactions", new[] { "TransactionType_Id" });
            RenameColumn(table: "dbo.Transactions", name: "TransactionType_Id", newName: "TransactionTypeId");
            AddColumn("dbo.Transactions", "BrokerAccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.Transactions", "TransactionTypeId", c => c.Byte(nullable: false));
            CreateIndex("dbo.Transactions", "TransactionTypeId");
            CreateIndex("dbo.Transactions", "BrokerAccountId");
            AddForeignKey("dbo.Transactions", "BrokerAccountId", "dbo.BrokerAccounts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Transactions", "TransactionTypeId", "dbo.TransactionTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "TransactionTypeId", "dbo.TransactionTypes");
            DropForeignKey("dbo.Transactions", "BrokerAccountId", "dbo.BrokerAccounts");
            DropIndex("dbo.Transactions", new[] { "BrokerAccountId" });
            DropIndex("dbo.Transactions", new[] { "TransactionTypeId" });
            AlterColumn("dbo.Transactions", "TransactionTypeId", c => c.Byte());
            DropColumn("dbo.Transactions", "BrokerAccountId");
            RenameColumn(table: "dbo.Transactions", name: "TransactionTypeId", newName: "TransactionType_Id");
            CreateIndex("dbo.Transactions", "TransactionType_Id");
            AddForeignKey("dbo.Transactions", "TransactionType_Id", "dbo.TransactionTypes", "Id");
        }
    }
}
