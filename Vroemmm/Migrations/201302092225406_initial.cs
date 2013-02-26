namespace Vroemmm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.StoredCredentials",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        AccessToken = c.String(),
                        RefreshToken = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StoredCredentials");
            DropTable("dbo.UserProfile");
        }
    }
}
