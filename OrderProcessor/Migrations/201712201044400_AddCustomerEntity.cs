namespace OrderProcessor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FullName = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        CreatedDate = c.String(),
                        NextExpectedVersion = c.Long(nullable: false),
                        LogPosition = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Orders", "CustomerId");
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropTable("dbo.Customers");
        }
    }
}
