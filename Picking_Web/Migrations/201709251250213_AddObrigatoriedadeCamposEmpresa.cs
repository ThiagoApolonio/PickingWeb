namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddObrigatoriedadeCamposEmpresa : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Empresas", "Nome", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "PortaSap", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "UsuarioSap", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "SenhaSap", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "TipoBanco", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "InstanciaBanco", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "NomeBanco", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "UsuarioBanco", c => c.String(nullable: false));
            AlterColumn("dbo.Empresas", "SenhaBanco", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Empresas", "SenhaBanco", c => c.String());
            AlterColumn("dbo.Empresas", "UsuarioBanco", c => c.String());
            AlterColumn("dbo.Empresas", "NomeBanco", c => c.String());
            AlterColumn("dbo.Empresas", "InstanciaBanco", c => c.String());
            AlterColumn("dbo.Empresas", "TipoBanco", c => c.String());
            AlterColumn("dbo.Empresas", "SenhaSap", c => c.String());
            AlterColumn("dbo.Empresas", "UsuarioSap", c => c.String());
            AlterColumn("dbo.Empresas", "PortaSap", c => c.String());
            AlterColumn("dbo.Empresas", "Nome", c => c.String());
        }
    }
}
