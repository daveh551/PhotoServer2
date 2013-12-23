namespace PhotoServer2.App_Archicture.Services.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEXIFColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "FStop", c => c.String());
            AddColumn("dbo.Photos", "ShutterSpeed", c => c.String());
            AddColumn("dbo.Photos", "ISOSpeed", c => c.Short());
            AddColumn("dbo.Photos", "FocalLength", c => c.Short());
            AddColumn("dbo.Photos", "PhotographerInitials", c => c.String());
            AddColumn("dbo.Photos", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Photos", "CreatedAt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "CreatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.Photos", "CreatedDate");
            DropColumn("dbo.Photos", "PhotographerInitials");
            DropColumn("dbo.Photos", "FocalLength");
            DropColumn("dbo.Photos", "ISOSpeed");
            DropColumn("dbo.Photos", "ShutterSpeed");
            DropColumn("dbo.Photos", "FStop");
        }
    }
}
