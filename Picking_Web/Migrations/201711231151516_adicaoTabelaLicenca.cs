namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adicaoTabelaLicenca : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Licencas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuantidadeLicencas = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Licencas");
        }
    }
}
