namespace PhotoServer2.App_Archicture.Services.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeRaceIdNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photos", "RaceId", "dbo.Races");
            DropIndex("dbo.Photos", new[] { "RaceId" });
            AlterColumn("dbo.Photos", "RaceId", c => c.Int());
            CreateIndex("dbo.Photos", "RaceId");
            AddForeignKey("dbo.Photos", "RaceId", "dbo.Races", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "RaceId", "dbo.Races");
            DropIndex("dbo.Photos", new[] { "RaceId" });
            AlterColumn("dbo.Photos", "RaceId", c => c.Int(nullable: false));
            CreateIndex("dbo.Photos", "RaceId");
            AddForeignKey("dbo.Photos", "RaceId", "dbo.Races", "Id", cascadeDelete: true);
        }
    }
}
