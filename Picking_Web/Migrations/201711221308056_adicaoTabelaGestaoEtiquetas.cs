namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adicaoTabelaGestaoEtiquetas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GestaoEtiquetas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpresaId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        NomeImpressoraEtiqueta = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Empresas", t => t.EmpresaId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EmpresaId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GestaoEtiquetas", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GestaoEtiquetas", "EmpresaId", "dbo.Empresas");
            DropIndex("dbo.GestaoEtiquetas", new[] { "UserId" });
            DropIndex("dbo.GestaoEtiquetas", new[] { "EmpresaId" });
            DropTable("dbo.GestaoEtiquetas");
        }
    }
}
