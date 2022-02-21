namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedEmpresas : DbMigration
    {
        public override void Up()
        {
            Sql(@"
            SET IDENTITY_INSERT [dbo].[Empresas] ON

            INSERT INTO [dbo].[Empresas] ([Id], [Nome], [Ativo], [Licenciado], [LicencaSap], [PortaSap], [UsuarioSap], [SenhaSap], [TipoBanco], [InstanciaBanco], [NomeBanco], [UsuarioBanco], [SenhaBanco]) 
            VALUES (1, N'DEFAULT', 1, 1, N'192.168.0.1', N'3030', N'manager', N'manager', N'8', N'DELL\SAP', N'SBO_DATABASE_PRD', N'sa', N'senhasa');            
            SET IDENTITY_INSERT [dbo].[Empresas] OFF

            UPDATE [dbo].[AspNetUsers] SET EmpresaId = 1;
            ");
        }
        
        public override void Down()
        {
        }
    }
}
