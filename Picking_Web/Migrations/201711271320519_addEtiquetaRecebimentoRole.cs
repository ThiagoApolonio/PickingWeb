namespace Picking_Web.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class addEtiquetaRecebimentoRole : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'a5c92efd-3df8-4095-bc00-79ee917b9a0a', N'PodeImprimirEtiquetaRecebimento')
            ");
        }

        public override void Down()
        {
        }
    }
}
