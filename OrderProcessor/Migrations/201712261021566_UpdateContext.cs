namespace OrderProcessor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateContext : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "CreatedDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "CreatedDate", c => c.String());
        }
    }
}
