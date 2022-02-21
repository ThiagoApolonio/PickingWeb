namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsUserAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Ativo", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Licenciado", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Operador", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Operador");
            DropColumn("dbo.AspNetUsers", "Licenciado");
            DropColumn("dbo.AspNetUsers", "Ativo");
        }
    }
}
