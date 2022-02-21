namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleManual : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                
                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'c70ed6b4-fa18-7d39-7d62-f26a3b39db0d', N'PodeGerenciarBaixa')
                
                INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'be95bb48-83cc-4856-b64f-cb4c187d90b1', N'c70ed6b4-fa18-7d39-7d62-f26a3b39db0d')
  

            ");
        }
        
        public override void Down()
        {
        }
    }
}
