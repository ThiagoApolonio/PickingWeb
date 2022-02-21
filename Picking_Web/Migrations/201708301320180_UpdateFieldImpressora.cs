namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFieldImpressora : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Impressoras", "IP", c => c.String());
            DropColumn("dbo.Impressoras", "Char");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Impressoras", "Char", c => c.String());
            DropColumn("dbo.Impressoras", "IP");
        }
    }
}
