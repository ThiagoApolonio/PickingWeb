namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCampoTimerEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Empresas", "Timer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Empresas", "Timer");
        }
    }
}
