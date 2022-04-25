namespace Libanon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialVer7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Book", "WasBorrowed", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Book", "WasBorrowed");
        }
    }
}
