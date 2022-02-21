using System;

namespace Picking_Web.Helpers
{
    public static class DateHelper
    {

        #region :: Tratamento de Data

        /// <summary>
        /// Retorna a string de como deve ser tratado o campo para a consulta
        /// </summary>
        /// <param name="nome_campo_data">nome da coluna na tabela</param>
        public static string ConvertToSelectAsDate(string nome_campo_data)
        {
            return "CONVERT(DATE, " + nome_campo_data + ", 103)";
        }


        /// <summary>
        /// Retorna a string de como deve ser tratado o campo para uma comparação com outro
        /// </summary>
        /// <param name="nome_campo_data">nome da coluna na tabela</param>
        public static string DateFieldToCompare(string nome_campo_data)
        {
            return "CONVERT(DATE, " + nome_campo_data + ")";
        }

        /// <summary>
        /// Retorna a string de como deve ser tratado o campo para a consulta
        /// </summary>
        /// <param name="nome_campo_data">nome da coluna na tabela</param>
        public static string ConvertToSelectAsDatetime(string nome_campo_data)
        {
            return "CONVERT(DATETIME, " + nome_campo_data + ", 103)";
        }

        /// <summary>
        /// Retorna um string conforme o que o SQL Reader retorna
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string GetFromDB(object datetime)
        {
            return datetime.ToString();
        }

        /// <summary>
        /// Retorna um datetime conforme o que o SQL Reader retorna
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetFromDB(string datetime)
        {
            return DateTime.Parse(datetime);
        }

        /// <summary>
        /// Recebe um DateTime e converte para String PT-BR
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ConvertToPTBR(DateTime datetime)
        {
            return datetime.ToString("dd/MM/yy");
        }

        #endregion


        #region :: Tratamento de Hora

        /// <summary>
        /// Retorna a string de como deve ser tratado o campo para a consulta
        /// </summary>
        /// <param name="nome_campo_data">nome da coluna na tabela</param>
        public static string ConvertToSelectAsTime(string nome_campo_data)
        {
            return "CONVERT(VARCHAR(5), " + nome_campo_data + ", 108)";
        }

        /// <summary>
        /// pega o campo de hora (bugado) do sap e faz com que ele se transforme em um campo varchar para quem tá montando a consulta.
        /// </summary>
        /// <param name="nome_campo_hora"></param>
        /// <param name="nome_alias"></param>
        /// <returns></returns>
        public static string ConvertToSelectTimeFieldAsString(string nome_campo_hora, string nome_alias = "")
        {
            string sql =
                "CASE " +
                "   WHEN LEN(" + nome_campo_hora + ") = 3 " +
                "       THEN '0' + LEFT(" + nome_campo_hora + ", 1) + ':' + RIGHT(" + nome_campo_hora + ", 2) " +
                "       ELSE LEFT(" + nome_campo_hora + ",2) +':' + RIGHT(" + nome_campo_hora + ", 2) " +
                "END ";

            if (!String.IsNullOrEmpty(nome_alias))
            {
                sql += "AS " + nome_alias;
            }

            return sql;
        }

        #endregion
    }
}