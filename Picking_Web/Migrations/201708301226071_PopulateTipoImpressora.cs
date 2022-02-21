namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTipoImpressora : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO TipoImpressoras (Id, Descricao) VALUES (1,'HP Laser - Etiqueta Amarela') ");
            Sql("INSERT INTO TipoImpressoras (Id, Descricao) VALUES (2,'HP Laser - Etiqueta Branca') ");
        }
        
        public override void Down()
        {
        }
    }
}
