namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIPPortaField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GestaoEtiquetas", "IP", c => c.String());
            AddColumn("dbo.GestaoEtiquetas", "Porta", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GestaoEtiquetas", "Porta");
            DropColumn("dbo.GestaoEtiquetas", "IP");
        }
    }
}
