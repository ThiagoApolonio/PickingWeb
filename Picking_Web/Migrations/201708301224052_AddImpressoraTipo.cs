namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImpressoraTipo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TipoImpressoras",
                c => new
                    {
                        Id = c.Byte(nullable: false),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Impressoras", "TipoImpressoraId", c => c.Byte(nullable: false));
            CreateIndex("dbo.Impressoras", "TipoImpressoraId");
            AddForeignKey("dbo.Impressoras", "TipoImpressoraId", "dbo.TipoImpressoras", "Id", cascadeDelete: true);
            DropColumn("dbo.Impressoras", "Tipo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Impressoras", "Tipo", c => c.String());
            DropForeignKey("dbo.Impressoras", "TipoImpressoraId", "dbo.TipoImpressoras");
            DropIndex("dbo.Impressoras", new[] { "TipoImpressoraId" });
            DropColumn("dbo.Impressoras", "TipoImpressoraId");
            DropTable("dbo.TipoImpressoras");
        }
    }
}
