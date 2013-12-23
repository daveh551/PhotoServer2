namespace PhotoServer2.App_Archicture.Services.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeSequenceNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Photos", "Sequence", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Photos", "Sequence", c => c.Int(nullable: false));
        }
    }
}
