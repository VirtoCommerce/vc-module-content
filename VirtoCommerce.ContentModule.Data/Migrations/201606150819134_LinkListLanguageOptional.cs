namespace VirtoCommerce.ContentModule.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkListLanguageOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContentMenuLinkList", "Language", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ContentMenuLinkList", "Language", c => c.String(nullable: false));
        }
    }
}
