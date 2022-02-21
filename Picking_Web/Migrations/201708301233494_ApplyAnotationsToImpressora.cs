namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplyAnotationsToImpressora : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Impressoras", "Descricao", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.TipoImpressoras", "Descricao", c => c.String(nullable: false, maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TipoImpressoras", "Descricao", c => c.String());
            AlterColumn("dbo.Impressoras", "Descricao", c => c.String());
        }
    }
}
