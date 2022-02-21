namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdicionaDepositoPadrãoNaEmpresa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Empresas", "DepoPadrao", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Empresas", "DepoPadrao");
        }
    }
}
