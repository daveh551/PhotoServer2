namespace PhotoServer2.App_Archicture.Services.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreateAtAndCreatedByColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.Photos", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "CreatedBy");
            DropColumn("dbo.Photos", "CreatedAt");
        }
    }
}
