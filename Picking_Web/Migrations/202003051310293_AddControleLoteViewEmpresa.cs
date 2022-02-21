namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddControleLoteViewEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Empresas", "ContadorLote", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Empresas", "ContadorLote");
        }
    }
}
