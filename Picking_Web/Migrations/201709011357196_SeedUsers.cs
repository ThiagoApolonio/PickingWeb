namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [Ativo], [Licenciado], [Operador]) VALUES (N'be95bb48-83cc-4856-b64f-cb4c187d90b1', N'admin@update.com', 0, N'AEnfM9XkoKzE2d9xCkieSHAjgPcEKeUFFTQ98az06srDIPkjkkJ5ArWE+zKCm6+iDQ==', N'7bc65194-22e1-4a59-8230-b5bb7f5def18', NULL, 0, 0, NULL, 1, 0, N'Administrador', 1, 1, 0)
                INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [Ativo], [Licenciado], [Operador]) VALUES (N'82ff043b-a0d4-4a9a-8d46-4c39b71b8244', N'convidado@update.com', 0, N'AL8My3hl57HfIgbx8AKbBgOHa/1ajGQUFBOKiJcYDfn4BSbrEkMlyJSNvoBQyhAsiQ==', N'd051a298-f162-4535-b793-300493c0baf2', NULL, 0, 0, NULL, 1, 0, N'Convidado', 1, 1, 0)

                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'2f1b03f3-76c2-459e-8331-e2fe737030ab', N'PodeConferirCodigoBarras')
                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'88314d77-1fba-4086-b2c5-bc0602638936', N'PodeGerenciarAdmin')
                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'ee30e7c0-e42e-4e34-8dc4-6a5158e70023', N'PodeGerenciarEtiqueta')
                INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'e5c92efd-3df8-4095-bc00-79ee917b9a06', N'PodeGerenciarListaPicking')

                INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'be95bb48-83cc-4856-b64f-cb4c187d90b1', N'2f1b03f3-76c2-459e-8331-e2fe737030ab')
                INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'be95bb48-83cc-4856-b64f-cb4c187d90b1', N'88314d77-1fba-4086-b2c5-bc0602638936')
                INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'be95bb48-83cc-4856-b64f-cb4c187d90b1', N'ee30e7c0-e42e-4e34-8dc4-6a5158e70023')
                INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'be95bb48-83cc-4856-b64f-cb4c187d90b1', N'e5c92efd-3df8-4095-bc00-79ee917b9a06')

            ");
        }
        
        public override void Down()
        {
        }
    }
}
