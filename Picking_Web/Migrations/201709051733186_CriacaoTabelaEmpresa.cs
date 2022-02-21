namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoTabelaEmpresa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Empresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(),
                        Ativo = c.Boolean(nullable: false),
                        Licenciado = c.Boolean(nullable: false),
                        LicencaSap = c.String(),
                        PortaSap = c.String(),
                        UsuarioSap = c.String(),
                        SenhaSap = c.String(),
                        TipoBanco = c.String(),
                        InstanciaBanco = c.String(),
                        NomeBanco = c.String(),
                        UsuarioBanco = c.String(),
                        SenhaBanco = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Empresas");
        }
    }
}
