namespace VirtoCommerce.ContentModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIsActiveColumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ContentMenuLink", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ContentMenuLink", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
