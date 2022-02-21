using Picking_Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Picking_Web.Helpers {
    /// <summary>
    /// Helpers globais do site
    /// </summary>
    public static class GlobalHelper {
        #region :: Definições de Impressão

        public static int LugarParaImprimirGeracaoListaPicking = 1;
        public static int LugarParaImprimirEtiquetagemPesagem = 3;
        public static int LugarParaImprimirImpressoraAmbiente = 4;
        public static int LugarParaImprimirImpressoraGeladeira = 5;

        /// <summary>
        /// definição de quais lugares
        /// </summary>
        public static Dictionary<int, string> LugaresParaImpressao = new Dictionary<int, string>() {
            { GlobalHelper.LugarParaImprimirGeracaoListaPicking, "Geração Lista de Picking" },
            { GlobalHelper.LugarParaImprimirEtiquetagemPesagem, "Etiquetagem / Pesagem" },
            { GlobalHelper.LugarParaImprimirImpressoraAmbiente, "Impressora Ambiente" },
            { GlobalHelper.LugarParaImprimirImpressoraGeladeira, "Impressora Geladeira" },
        };

        public static string RelatoriosPath = HostingEnvironment.MapPath("~/Relatorio");

        #endregion


        #region :: Métodos Auxiliares

        /// <summary>
        /// Retorna os operadores do contexto recebido
        /// </summary>
        /// <param name="_context"></param>
        /// <returns></returns>
        public static object GetOperadores(ApplicationDbContext _context, int empresa_id, string roleid) {
            return _context.Users.Select(x => new { x.Id, x.UserName, x.Operador, x.Ativo, x.Licenciado, x.UsuarioSAPId, x.EmpresaId, x.Roles })
                .Where(x => x.Operador == true && x.Ativo == true && x.Licenciado == true && x.EmpresaId == empresa_id && x.Roles.Select(y => y.RoleId).Contains(roleid))
                .ToList()
                .OrderBy(x => x.UserName);
        }

        /// <summary>
        /// Retorna os operadores do contexto recebido
        /// </summary>
        /// <param name="_context"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<ApplicationUser> GetOperadoresEnumerable(ApplicationDbContext _context, int empresa_id, string roleid) {
            return _context.Users
                .Where(x => x.Operador == true && x.Ativo == true && x.Licenciado == true && x.EmpresaId == empresa_id && x.Roles.Select(y => y.RoleId).Contains(roleid))
                .ToList()
                .OrderBy(x => x.UserName);
        }

        /// <summary>
        /// atualiza o campo de usuário status do pedido de venda
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        public static bool AtualizaCampoUsuarioStatusPedidoVenda(Empresa empresa, int numdoc, string status) {
            int affected_rows = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // muda status do picking
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string sql =
                        string.Format(
                            @"UPDATE ORDR " +
                            @"SET U_UPD_PCK_STATUS = '" + status + "' WHERE DocEntry = " + numdoc
                        );
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }
            return affected_rows > 0;
        }

        /// <summary>
        /// Adiciona uma etapa do pedido na realização do picking
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool InsereNovaEtapaPickingDoPedido(Empresa empresa, int numdoc, string status) {
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string nome_tabela = "[@UPD_PCK_ETAPA]";
                    string sql =
                        $@" {GlobalHelper.SQLDeclareTableID(nome_tabela)}

                                INSERT INTO {nome_tabela}
                                    (Code, name, U_docentry, U_status, U_date)
                                VALUES
                                    (@table_id, @table_id, " + numdoc + ", '" + status + "', GETDATE() );";
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return true;
        }

        /// <summary>
        /// Adiciona uma etapa do pedido na realização do picking
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool RemoveNovaEtapaPickingDoPedido(Empresa empresa, int numdoc, string status) {
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string nome_tabela = "[@UPD_PCK_ETAPA]";
                    string sql =
                        $@" DELETE FROM {nome_tabela} WHERE U_status = '{status}' AND U_docentry = {numdoc};";
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return true;
        }

        /// <summary>
        /// Adiciona uma etapa do pedido na realização do picking
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool InserePedidoCancelado(Empresa empresa, int numdoc) {
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string nome_tabela = "[@UPD_PCK_CANCEL]";
                    string sql =
                        $@" {GlobalHelper.SQLDeclareTableID(nome_tabela)}

                                INSERT INTO {nome_tabela}
                                    (Code, name, U_docentry, U_date)
                                VALUES
                                    (@table_id, @table_id, {numdoc}, GETDATE() );";
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return true;
        }

        /// <summary>
        /// Adiciona um operador do picking ao pedido
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="operador_id"></param>
        /// <returns></returns>
        public static bool InsereNovoOperadorPickingDoPedido(Empresa empresa, int numdoc, string operador_id, string status) {
            if (String.IsNullOrEmpty(operador_id)) {
                return true;
            }

            int affected_rows = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string nome_tabela = "[@UPD_PCK_OPERADOR]";
                    string sql =
                        $@"{SQLDeclareTableID(nome_tabela)}

                            INSERT INTO {nome_tabela}
                                (Code, name, U_docentry, U_status, U_operador)
                            VALUES
                                (@table_id, @table_id, " + numdoc + ", '" + status + "', '" + operador_id + "' );";
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return affected_rows > 0;
        }

        /// <summary>
        /// Adiciona um local fisico ao picking ao pedido
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="local"></param>
        /// <returns></returns>
        public static bool InsereNovoLocalFisicoPickingDoPedido(Empresa empresa, int numdoc, string local, string status) {
            if (String.IsNullOrEmpty(local)) {
                return true;
            }

            int affected_rows = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }
                    string nome_tabela = "[@UPD_PCK_LOCAL]";
                    string sql =
                        $@" {SQLDeclareTableID(nome_tabela)}

                            INSERT INTO {nome_tabela}
                                (Code, name, U_docentry, U_status, U_local)
                            VALUES
                                (@table_id, @table_id, " + numdoc + ",'" + status + "', '" + local + "' );";
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return affected_rows > 0;
        }

        /// <summary>
        /// ESTE MÉTODO DEVE SER UTILIZADO ANTES DE ATUALIZAR O CAMPO DE USUÁRIOS DE STATUS DO PEDIDO DE VENDA!!!
        /// ELE PEGA COM BASE NO CAMPO DE USUÁRIO PARA DESCOBRIR QUAL O ÚLTIMA ETAPA E ATUALIZAR A DATA FINAL
        /// Atualiza a DATA FINAL Status de uma ETAPA de picking 
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool EncerraEtapaPickingDoPedido(Empresa empresa, int numdoc) {
            int affected_rows = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string sql =
                        string.Format(
                            @"UPDATE [@UPD_PCK_ETAPA] SET U_end_date = GETDATE()
                            WHERE 1 = 1
                            AND U_end_date IS NULL
                            AND U_docentry = " + numdoc +
                            " AND U_status = (SELECT U_UPD_PCK_STATUS FROM ORDR WHERE DocEntry = " + numdoc + ")"
                        );
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return affected_rows > 0;
        }

        /// <summary>
        /// Faz a gestão de toda a atualização e Atualiza etapa do picking
        /// </summary>
        /// <param name="novo_status">muda o status do pedido e adiciona uma nova etapa para ele</param>
        /// <returns>retorna a msg de erro. se retornar vazio é pq não teve erro</returns>
        public static string AtualizarEtapaPicking(ApplicationDbContext _context, string novo_status) {
            try {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string str_operador = HttpContext.Current.Request.Form.Get("operador");
                string local = HttpContext.Current.Request.Form.Get("local");

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null) {
                    return "Empresa não encontrada";
                }

                return AtualizarEtapaPicking(empresa, numdoc, str_operador, local, novo_status);
            } catch (Exception e) {
                return e.Message;
            }
        }

        /// <summary>
        /// Faz a gestão de toda a atualização e Atualiza etapa do picking
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="operador_id">se receber -1 é porque não tem.</param>
        /// <param name="local"></param>
        /// <param name="novo_status">muda o status do pedido e adiciona uma nova etapa para ele</param>
        /// <returns>retorna a msg de erro. se retornar vazio é pq não teve erro</returns>
        public static string AtualizarEtapaPicking(Empresa empresa, int numdoc, string operador_id, string local, string novo_status) {
            try {
                if (AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, novo_status)) {
                    if (InsereNovaEtapaPickingDoPedido(empresa, numdoc, novo_status)) {
                        if (InsereNovoOperadorPickingDoPedido(empresa, numdoc, operador_id, novo_status)) {
                            if (!String.IsNullOrEmpty(local)) {
                                if (InsereNovoLocalFisicoPickingDoPedido(empresa, numdoc, local, novo_status)) {
                                    return "";
                                } else {
                                    return "Erro ao inserir local no Pedido.";
                                }
                            } else {
                                return "";
                            }
                        } else {
                            return "Erro ao inserir operador no Pedido.";
                        }
                    } else {
                        return "Erro ao criar nova etapa do Pedido.";
                    }
                } else {
                    return "Erro ao atualizar situação do pedido de venda.";
                }
            } catch (Exception e) {
                return e.Message;
            }
        }

        /// <summary>
        /// Faz a gestão de toda a atualização e Atualiza etapa do picking
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="operador_id">se receber -1 é porque não tem.</param>
        /// <param name="local"></param>
        /// <param name="status_atual">muda o status do pedido e adiciona uma nova etapa para ele</param>
        /// <returns>retorna a msg de erro. se retornar vazio é pq não teve erro</returns>
        public static string RemoverEtapaPicking(Empresa empresa, int numdoc, string status_atual, string status_antigo) {
            try {
                if (AtualizaCampoUsuarioStatusPedidoVenda(empresa, numdoc, status_antigo)) {
                    if (RemoveNovaEtapaPickingDoPedido(empresa, numdoc, status_atual)) {
                        return "";
                    } else {
                        return "Erro ao criar nova etapa do Pedido.";
                    }
                } else {
                    return "Erro ao atualizar situação do pedido de venda.";
                }
            } catch (Exception e) {
                return e.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="numdoc"></param>
        /// <returns></returns>
        public static int RetornaAbsEntryListaPickingDoPedido(string connectionString, int numdoc) {
            int numPicking = 0;
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                if (conn.State == ConnectionState.Closed) {
                    conn.Open();
                }

                string sql = "SELECT DISTINCT AbsEntry FROM PKL1 WHERE PickStatus <> 'C' AND BaseObject = " + ((int)SAPbobsCOM.BoObjectTypes.oOrders) + "  AND OrderEntry = " + numdoc;
                using (SqlCommand command = new SqlCommand("", conn)) {
                    command.CommandText = sql;
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            numPicking = NumberHelper.GetFromDBToInt(reader["AbsEntry"]);
                        }
                    }
                }
            }
            return numPicking;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="filepath"></param>
        public static void criarArquivoTXT(string text, string filepath = @"c:\tmp\") {
            if (!Directory.Exists(filepath)) {
                Directory.CreateDirectory(filepath);
            }

            filepath += @"log.txt";

            File.WriteAllText(filepath, text);
        }

        public static int RetornaQuantidadeLicencasUtilizando(ApplicationDbContext _context) {
            return _context.Users
            .Where(u => u.Licenciado)
            .Count();
        }

        public static int RetornaQuantidadeLicencasUtilizandoTirandoEleMesmo(ApplicationDbContext _context, string userID) {
            return _context.Users
            .Where(u => u.Licenciado && u.Id != userID)
            .Count();
        }

        public static int RetornaQuantidadeTotalLicencas(ApplicationDbContext _context) {
            var licenca = _context.Licenca.ToList();

            return Int32.Parse(EncryptionHelper.Base64Decode(licenca[0].QuantidadeLicencas));
        }

        public static int RetornaQuantidadeLicencasDisponiveis(ApplicationDbContext _context) {
            return (RetornaQuantidadeTotalLicencas(_context) - RetornaQuantidadeLicencasUtilizando(_context));
        }

        public static bool PodeAdicionarNovaLicenca(ApplicationDbContext _context, int qtd_licensas) {
            return (RetornaQuantidadeLicencasDisponiveis(_context) - qtd_licensas) >= 0;
        }

        public static bool PodeAtualizarUsuarioLicenca(ApplicationDbContext _context, string userID, bool mais_uma) {
            int usando_mais = mais_uma ? 1 : 0;
            return (RetornaQuantidadeTotalLicencas(_context) - (RetornaQuantidadeLicencasUtilizandoTirandoEleMesmo(_context, userID) + usando_mais)) >= 0;
        }

        public static string SQLDeclareTableID(string nome_tabela) {
            return $@"DECLARE @table_id INT;
					IF(SELECT COUNT(*) FROM {nome_tabela}) > 0
						SELECT @table_id = (SELECT TOP (1) (CAST(Code as INT) + 1) FROM {nome_tabela} ORDER BY CAST(Code as INT) DESC) 
					ELSE
						SELECT @table_id = '1'; ";
        }

        public static List<ApplicationUser> GetUsuarios(ApplicationDbContext _context, int empresa_id) {
            return _context.Users.OrderBy(x => x.UserName).Where(x => x.EmpresaId == empresa_id).ToList();
        }

        #endregion

        #region Métodos Auxiliares - Lista Picking

        /// <summary>
        /// Faz a gestão de toda a atualização e Atualiza etapa do picking
        /// </summary>
        /// <param name="novo_status">muda o status do pedido e adiciona uma nova etapa para ele</param>
        /// <returns>retorna a msg de erro. se retornar vazio é pq não teve erro</returns>
        public static string AtualizarEtapaPickingListaPicking(ApplicationDbContext _context, string novo_status, int numpk = 0) {
            try {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                if (numpk == 0)
                    numpk = int.Parse(HttpContext.Current.Request.Form.Get("numpk"));

                string str_operador = HttpContext.Current.Request.Form.Get("operador");
                string local = HttpContext.Current.Request.Form.Get("local");

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null) {
                    return "Empresa não encontrada";
                }

                var att = AttCampoSituacaoPickingListaPicking(empresa, numpk, novo_status);
                if (att) {
                    return "";
                } else {
                    return "Falha ao realizar update no status da lista de picking.";
                }
            } catch (Exception e) {
                return e.Message;
            }
        }

        /// <summary>
        /// atualiza o campo de usuário status da lista de picking
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        public static bool AttCampoSituacaoPickingListaPicking(Empresa empresa, int numdoc, string status) {
            int affected_rows = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // muda status do picking
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string sql =
                        string.Format(
                            @"UPDATE OPKL " +
                            @"SET U_UPD_PCK_STATUS = '" + status + "' WHERE AbsEntry = " + numdoc
                        );
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }
            return affected_rows > 0;
        }

        /// <summary>
        /// atualiza os campos de usuário Transportadora, Qtd Embalagens e Peso Bruto
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        /// <param name="status"></param>
        public static bool AttCamposEmbalagemListaPicking(Empresa empresa, int numpk, string transp, int qtdEmb, double pesoBruto, string status) {
            int affected_rows = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // muda status do picking
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string sql =
                        string.Format(
                            @"UPDATE OPKL SET U_UPD_TRANSP = '" + transp + "', U_UPD_QEMBAL = '" + qtdEmb + "', U_UPD_PBRUTO = '" + pesoBruto + "', U_UPD_PCK_STATUS = '" + status + "' WHERE AbsEntry = " + numpk
                        );
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }
            return affected_rows > 0;
        }

        public static int GetBinAbsEntry(Empresa empresa, string deposito, string item) {
            int BinAbs = 0;
            string connectionString = DBHelper.GetConnectionString(empresa);
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                if (conn.State == ConnectionState.Closed) {
                    conn.Open();
                }

                string sql = string.Format("select BinAbs from OBBQ where WhsCode = '"+ deposito +"' and ItemCode = '"+ item +"'");
                using (SqlCommand command = new SqlCommand("", conn)) {
                    command.CommandText = sql;
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            BinAbs = int.Parse(reader["BinAbs"].ToString());
                        }
                    }
                }
            }
            return BinAbs;
        }

        #endregion
    }
}