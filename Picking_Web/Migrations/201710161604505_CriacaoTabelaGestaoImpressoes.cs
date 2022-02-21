namespace Picking_Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoTabelaGestaoImpressoes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GestaoImpressoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpresaId = c.Int(nullable: false),
                        LugarImpressaoId = c.Int(nullable: false),
                        NomeImpressora = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Empresas", t => t.EmpresaId, cascadeDelete: true)
                .Index(t => t.EmpresaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GestaoImpressoes", "EmpresaId", "dbo.Empresas");
            DropIndex("dbo.GestaoImpressoes", new[] { "EmpresaId" });
            DropTable("dbo.GestaoImpressoes");
        }
    }
}
