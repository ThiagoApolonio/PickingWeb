namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemocaodoCampoNomeUserAccount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Nome");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Nome", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
