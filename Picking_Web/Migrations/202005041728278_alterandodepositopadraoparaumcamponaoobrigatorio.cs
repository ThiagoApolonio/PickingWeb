namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterandodepositopadraoparaumcamponaoobrigatorio : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Empresas", "DepoPadrao", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Empresas", "DepoPadrao", c => c.String(nullable: false));
        }
    }
}
