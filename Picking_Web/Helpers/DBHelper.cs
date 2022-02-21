using Picking_Web.Models;

namespace Picking_Web.Helpers
{
    public class DBHelper
    {
        public static string GetConnectionString(string instancia, string nome_banco, string usuario_banco, string senha_banco)
        {
            return "data source=" + instancia + ";initial catalog=" + nome_banco + ";persist security info=True;user id=" + usuario_banco + ";password=" + senha_banco + ";MultipleActiveResultSets=True;";
        }

        public static string GetConnectionString(Empresa empresa)
        {
            return GetConnectionString(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
        }
    }
}