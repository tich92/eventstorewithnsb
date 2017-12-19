namespace OrderProcessor.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddEventStoreMetaData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "NextExpectedVersion", c => c.Long(nullable: false));
            AddColumn("dbo.OrderItems", "LogPosition", c => c.Long(nullable: false));
            AddColumn("dbo.Orders", "NextExpectedVersion", c => c.Long(nullable: false));
            AddColumn("dbo.Orders", "LogPosition", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "LogPosition");
            DropColumn("dbo.Orders", "NextExpectedVersion");
            DropColumn("dbo.OrderItems", "LogPosition");
            DropColumn("dbo.OrderItems", "NextExpectedVersion");
        }
    }
}
