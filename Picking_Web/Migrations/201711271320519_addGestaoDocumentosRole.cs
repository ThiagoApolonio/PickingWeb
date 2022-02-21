using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace Picking_Web.Migrations
{
    public class _201711271320519_addGestaoDocumentosRole: DbMigration
    {
        public override void Up()
        {
            Sql(@"
                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'be95bb48-83cc-4856-b64f-79ee917b9a0b', N'PodeGerenciarDocumentos')
            ");
        }

        public override void Down()
        {
        }
    }
}