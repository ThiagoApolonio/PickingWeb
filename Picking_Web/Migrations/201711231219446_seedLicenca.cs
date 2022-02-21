namespace Picking_Web.Migrations
{
    using Picking_Web.Helpers;
    using System.Data.Entity.Migrations;

    public partial class seedLicenca : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Licencas (QuantidadeLicencas) VALUES ('" + EncryptionHelper.Base64Encode("5") + "')");
        }

        public override void Down()
        {
        }
    }
}
