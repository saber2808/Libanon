namespace Libanon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialVer6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Book", "OTP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Book", "OTP");
        }
    }
}
