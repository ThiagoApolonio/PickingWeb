using Picking_Web.COMObjects;
using Picking_Web.Contexto;
using Picking_Web.Helpers;
using Picking_Web.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace Picking_Web.Controllers.API {
    public class CodigoBarrasController : ApiController {
        #region :: Propriedades e Construtor

        private ApplicationDbContext _context;
        public static List<ItensLoteCode> itens_ControleLote = new List<ItensLoteCode>();
        public CodigoBarrasController() {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing) {
            _context.Dispose();
        }

        #endregion


        #region :: API's

        //  [System.Web.Http.HttpGet]
        //  public IHttpActionResult GetItensPedido(int empresa_id, string numdoc) {
        //      try {
        //          if (String.IsNullOrEmpty(numdoc) || numdoc == "-1") {
        //              return Ok(new List<string>() { });
        //          }

        //          Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

        //          if (empresa == null) {
        //              return NotFound();
        //          }

        //          string campo_usuario_status = ConfigurationManager.AppSettings["CAMPO_STATUS_PICKING"];

        //          string connectionString = DBHelper.GetConnectionString(empresa);
        //          using (SqlConnection conn = new SqlConnection(connectionString)) {
        //              if (conn.State == ConnectionState.Closed) {
        //                  conn.Open();
        //              }

        //              string sql =
        //                  $@"SELECT 
        //	AbsEntry 
        //FROM OPKL tb0 
        //WHERE tb0.AbsEntry = {numdoc}
        //                  AND tb0.Status = 'R'
        //                  AND tb0.U_UPD_PCK_STATUS = 'EC'
        //                  ";

        //              bool pedido_existe = false;
        //              using (SqlCommand command = new SqlCommand("", conn)) {
        //                  command.CommandText = sql;
        //                  SqlDataReader reader = command.ExecuteReader();
        //                  if (reader.Read()) {
        //                      pedido_existe = NumberHelper.GetFromDBToInt(reader["AbsEntry"]) > 0;
        //                  }
        //              }

        //              if (pedido_existe) {
        //                  List<object> res = new List<object>() { };

        //                  using (SqlConnection conn2 = new SqlConnection(connectionString)) {
        //                      if (conn2.State == ConnectionState.Closed) {
        //                          conn2.Open();
        //                      }
        //                      using (SqlCommand command = new SqlCommand("", conn2)) {
        //                          int pedido_objType = (int)SAPbobsCOM.BoObjectTypes.oOrders;

        //                          sql = string.Format(
        //                              $@"
        //			SELECT 
        //                              T1.ItemCode
        //                              , T2.ItemName
        //                              , T2.LocCode
        //                              , '       ' as numSerie
        //                              , ISNULL(T4.MnfSerial,  '     ') as CodBarras
        //                              , ISNULL(T4.DistNumber, '     ') as numLote
        //                              , ISNULL(T4.LotNumber,'     ') as CodFabricante
        //                              , 0 as qtd
        //                              , T1.RelQtty as qtdTotal
        //                              FROM PKL1 T0
        //                              INNER JOIN PKL2 T1 on T0.AbsEntry = T1.AbsEntry and T0.PickEntry = T1.PickEntry
        //                              INNER JOIN OITL T2 on T1.ItemCode = T2.ItemCode
        //                              INNER JOIN OBTN T4 on T1.SnBEntry = T4.AbsEntry
        //                              WHERE 1 = 1
        //                              AND T0.AbsEntry = {numdoc}
        //                              AND T0.BaseObject = 17
        //                              AND T1.ManagedBy = 10000044
        //                              and T0.PickStatus <> 'C' 
        //                              GROUP BY T1.ItemCode , T2.ItemName , T2.LocCode, T4.MnfSerial,  T4.DistNumber, T4.LotNumber, T1.RelQtty
        //                              UNION ALL
        //                              SELECT 
        //                              T1.ItemCode
        //                              , T2.ItemName
        //                              , T2.LocCode
        //                              , ISNULL(tb2.DistNumber, '     ') as numSerie
        //                              , ISNULL(tb2.MnfSerial,  '     ') as CodBarras
        //                              , ISNULL(tb2.LotNumber,'     ') as CodFabricante
        //                              , '       ' as numLote
        //                              , 0 as qtd
        //                              , T1.RelQtty as qtdTotal
        //                              FROM PKL1 T0
        //                              INNER JOIN PKL2 T1 on T0.AbsEntry = T1.AbsEntry and T0.PickEntry = T1.PickEntry
        //                              INNER JOIN OITL T2 on T1.ItemCode = T2.ItemCode
        //                              INNER JOIN OSRN tb2 ON (tb2.AbsEntry = T1.AbsEntry)
        //                              WHERE 1 = 1
        //                              AND T0.AbsEntry = {numdoc}
        //                              AND T0.BaseObject = 17
        //                              AND T1.ManagedBy = 10000045
        //                              GROUP BY T1.ItemCode , T2.ItemName , T2.LocCode, tb2.MnfSerial,  tb2.DistNumber, tb2.LotNumber, T1.RelQtty");
        //                          command.CommandText = sql;
        //                          SqlDataReader reader = command.ExecuteReader();
        //                          int i = 0;
        //                          while (reader.Read()) {
        //                              res.Add(new {
        //                                  index = i,
        //                                  Item = reader["ItemCode"],
        //                                  Descricao = reader["ItemName"],
        //                                  Deposito = reader["LocCode"],
        //                                  numSerie = reader["numSerie"],
        //                                  CodigoBarras = reader["codBarras"],
        //                                  CodigoFabricante = reader["CodFabricante"],
        //                                  numLote = reader["numLote"],
        //                                  Quantidade = reader["qtd"],
        //                                  QuantidadeTotal = reader["qtdTotal"],
        //                              });
        //                              i++;
        //                          }
        //                      }
        //                  }
        //                  return Ok(res);
        //              } else {
        //                  return BadRequest("Não é possível realizar a conferência para esta Lista de Picking");
        //              }
        //          }
        //      } catch (Exception e) {
        //          return BadRequest(e.Message);
        //      }
        //  }
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetItensPedido(int empresa_id, string numdoc)
        {
            try
            {
                if (String.IsNullOrEmpty(numdoc) || numdoc == "-1")
                {
                    return Ok(new List<string>() { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null)
                {
                    return NotFound();
                }

                string campo_usuario_status = ConfigurationManager.AppSettings["CAMPO_STATUS_PICKING"];

                string connectionString = DBHelper.GetConnectionString(empresa);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string sql =
                        $@"SELECT 
							AbsEntry 
						FROM OPKL tb0 
						WHERE tb0.AbsEntry = {numdoc}
                        AND tb0.Status = 'R'
                        AND tb0.U_UPD_PCK_STATUS = 'EC'
                        ";

                    bool pedido_existe = false;
                    using (SqlCommand command = new SqlCommand("", conn))
                    {
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            pedido_existe = NumberHelper.GetFromDBToInt(reader["AbsEntry"]) > 0;
                        }
                    }

                    if (pedido_existe)
                    {
                        List<object> res = new List<object>() { };

                        using (SqlConnection conn2 = new SqlConnection(connectionString))
                        {
                            if (conn2.State == ConnectionState.Closed)
                            {
                                conn2.Open();
                            }
                            using (SqlCommand command = new SqlCommand("", conn2))
                            {
                                int pedido_objType = (int)SAPbobsCOM.BoObjectTypes.oOrders;

                                sql = string.Format(
                                    $@"
                                        
                                        select t1.ItemCode 
                                        , T2.ItemName
	                                    , T2.LocCode
	                                    , '       ' as numSerie
	                                    , ISNULL(T4.MnfSerial,  '     ') as CodBarras
	                                    , ISNULL(T4.DistNumber, '     ') as numLote
	                                    , ISNULL(T4.LotNumber,'     ') as CodFabricante  
	                                    , 0 as qtd
	                                    ,T10.U_QtdPicking as qtdTotal
                                        ,t0.OrderLine 
	                                    from PKL1 t0 
                                    inner join RDR1  t1 on t0.OrderEntry = t1.DocEntry and t0.OrderLine = t1.LineNum
                                    inner join OITL t2 on T2.ApplyEntry = T1.DocEntry and T1.ItemCode = T2.ItemCode 
                                    inner join ITL1 t3 on t2.LogEntry = t3.LogEntry
                                    INNER JOIN OBTN T4 on T3.SysNumber = T4.SysNumber and t4.AbsEntry = t3.MdAbsEntry
									inner Join [@UPD_PKL4] T10 on T0.AbsEntry = T10.U_PkEntry 
									and T4.DistNumber = T10.U_NumLote
									and t2.ItemCode = t10.U_ItemCode 
									and t1.LineNum = t10.U_OrderLine
                                    where 1 = 1
                                    AND T0.AbsEntry = {numdoc}
                                    AND T0.BaseObject = 17
                                    and T0.PickStatus <> 'C'
                                    GROUP BY T1.ItemCode, T2.ItemName, T2.LocCode, T4.MnfSerial, T4.DistNumber, T4.LotNumber, T10.U_QtdPicking,t0.OrderLine
                                    UNION ALL
                                    select t1.ItemCode
                                        , T2.ItemName
                                        , T2.LocCode
                                        , ISNULL(t4.DistNumber, '     ') as numSerie
                                        , ISNULL(t4.MnfSerial, '     ') as CodBarras
                                        , ISNULL(t4.LotNumber, '     ') as CodFabricante
                                        , '       ' as numLote
                                        , 0 as qtd
                                        , T10.U_QtdPicking as qtdTotal
                                        ,t0.OrderLine 
                                        from PKL1 t0
                                    inner
                                        join RDR1 t1 on t0.OrderEntry = t1.DocEntry and t0.OrderLine = t1.LineNum
                                    inner join OITL t2 on T2.ApplyEntry = T1.DocEntry and T1.ItemCode = T2.ItemCode
                                    inner join ITL1 t3 on t2.LogEntry = t3.LogEntry
                                    INNER JOIN OSRN T4 on T3.SysNumber = T4.SysNumber and t4.AbsEntry = t3.MdAbsEntry
									inner Join [@UPD_PKL4] T10 on T0.AbsEntry = T10.U_PkEntry 
									and T4.DistNumber = T10.U_NumLote
									and t2.ItemCode = t10.U_ItemCode 
									and t0.OrderLine = t10.U_OrderLine
                                    where 1 = 1
                                    AND T0.AbsEntry = {numdoc}
                                    AND T0.BaseObject = 17
                                    and T0.PickStatus <> 'C'
                                    GROUP BY T1.ItemCode, T2.ItemName, T2.LocCode, T4.MnfSerial, T4.DistNumber, T4.LotNumber,T10.U_QtdPicking,t0.OrderLine
                                    order by t0.OrderLine");
                                command.CommandText = sql;
                                SqlDataReader reader = command.ExecuteReader();
                                int i = 0;
                                while (reader.Read())
                                {
                                    res.Add(new
                                    {
                                        index = i,
                                        Item = reader["ItemCode"],
                                        Descricao = reader["ItemName"],
                                        Deposito = reader["LocCode"],
                                        numSerie = reader["numSerie"],
                                        CodigoBarras = reader["codBarras"],
                                        CodigoFabricante = reader["CodFabricante"],
                                        numLote = reader["numLote"],
                                        lineNum = reader["OrderLine"],
                                        Quantidade = reader["qtd"],
                                        QuantidadeTotal = reader["qtdTotal"],
                                    });
                                    i++;
                                }
                            }
                        }
                        return Ok(res);
                    }
                    else
                    {
                        return BadRequest("Não é possível realizar a conferência para esta Lista de Picking");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult ConferenciaParcial() {
            try {
                string numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string operador = HttpContext.Current.Request.Form.Get("operador");
                int empresa_id = NumberHelper.StringToInt(str_empresa_id);

                Empresa empresa = _context.Empresa.Single(e => e.Id == empresa_id);
                if (empresa == null) {
                    return NotFound();
                }

                // sempre deleta a última conferência quando for fazer a conferência.
                DeletarConferenciaParcial(empresa, numdoc);

                string nome_tabela = "[@UPD_PCK_CONFPARCIAL]";
                string insert = GlobalHelper.SQLDeclareTableID(nome_tabela);

                // 1 é a quantidade de colunas.
                for (int i = 0; i < ((HttpContext.Current.Request.Form.Count - 3) / 10); i++) {
                    string itemcode = HttpContext.Current.Request.Form.Get("data[" + i + "][item]");
                    string numSerie = HttpContext.Current.Request.Form.Get("data[" + i + "][numSerie]");
                    string numLote = HttpContext.Current.Request.Form.Get("data[" + i + "][numLote]");
                    string codigoBarras = HttpContext.Current.Request.Form.Get("data[" + i + "][codigoBarras]");
                    string qtd = HttpContext.Current.Request.Form.Get("data[" + i + "][qtd]");
                    string qtdTotal = HttpContext.Current.Request.Form.Get("data[" + i + "][quantidadeTotal]");

                    insert +=
                        $@"INSERT INTO {nome_tabela}
						(Code, Name, U_docentry, U_itemcode, U_numserie, U_numlote, U_codbarras, U_qtd, U_qtdTotal)
						VALUES
						(@table_id + " + i + ",@table_id + " + i + ",'" + numdoc + "','" + itemcode + "','" + numSerie + "','" + numLote + "','" + codigoBarras + "'," + qtd + "," + qtdTotal + ");";
                }

                // salva na tabela que contabiliza os minutos
                string connectionString = DBHelper.GetConnectionString(empresa);
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = insert;
                        command.ExecuteNonQuery();
                    }
                }

                GlobalHelper.InsereNovoOperadorPickingDoPedido(empresa, Int32.Parse(numdoc), operador, "CP");
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult ResetConferencia() {
            try {
                string numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                int empresa_id = NumberHelper.StringToInt(str_empresa_id);

                Empresa empresa = _context.Empresa.Single(e => e.Id == empresa_id);
                if (empresa == null) {
                    return NotFound();
                }

                DeletarConferenciaParcial(empresa, numdoc);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult SalvarLog() {
            try {
                string numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string operador = HttpContext.Current.Request.Form.Get("operador");
                string item = HttpContext.Current.Request.Form.Get("item");
                string lote = HttpContext.Current.Request.Form.Get("lote");
                string msg = HttpContext.Current.Request.Unvalidated.Form.Get("msg");

                msg = Regex.Replace(msg, "<.*?>", string.Empty);

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);

                Empresa empresa = _context.Empresa.Single(e => e.Id == empresa_id);
                if (empresa == null) {
                    return NotFound();
                }


                string nome_tabela = "[@UPD_PCK_CONFLOG]";
                string insert =
                        $@"{GlobalHelper.SQLDeclareTableID(nome_tabela)}

							INSERT INTO {nome_tabela}
							    (Code, name, U_docentry, U_operador, U_itemcode, U_numLote, U_data, U_msg)
							VALUES
							    (@table_id, @table_id, " + numdoc + ", '" + operador + "', '" + item + "','" + lote + "', GETDATE(), '" + msg + "' );";

                // salva na tabela que contabiliza os minutos
                string connectionString = DBHelper.GetConnectionString(empresa);
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = insert;
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult DetalheEncerramentoConferencia(int empresa_id) {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null) {
                return NotFound();
            }

            var operadores = GlobalHelper.GetOperadores(_context, empresa.Id, Privilegios.PodeConferirCodigoBarrasID);

            object obj = new {
                operadores = operadores,
            };
            return Ok(obj);
        }

        //[System.Web.Http.HttpPost]
        //public IHttpActionResult EncerrarConferencia() {
        //    try {
        //        string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
        //        string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
        //        string str_operador = HttpContext.Current.Request.Form.Get("operador");
        //        string local = HttpContext.Current.Request.Form.Get("local");
        //        int empresa_id = NumberHelper.StringToInt(str_empresa_id);
        //        int numPicking = NumberHelper.StringToInt(str_numdoc);

        //        Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
        //        if (empresa == null) {
        //            return NotFound();
        //        }

        //        List<ItensConferencia> itens_web = new List<ItensConferencia>();
        //        ItensConferencia itensConf = new ItensConferencia();
        //        // 1 é a quantidade de colunas.
        //        for (int i = 0; i < ((HttpContext.Current.Request.Form.Count - 3) / 10); i++) {
        //            itensConf.ItemCode = HttpContext.Current.Request.Form.Get("data[" + i + "][item]");
        //            itensConf.Descricao = HttpContext.Current.Request.Form.Get("data[" + i + "][descricao]");
        //            itensConf.Deposito = HttpContext.Current.Request.Form.Get("data[" + i + "][deposito]");
        //            itensConf.NumSerie = HttpContext.Current.Request.Form.Get("data[" + i + "][numSerie]");
        //            itensConf.NumLote = HttpContext.Current.Request.Form.Get("data[" + i + "][numLote]");
        //            itensConf.CodigoBarras = HttpContext.Current.Request.Form.Get("data[" + i + "][codigoBarras]");
        //            itensConf.QtdVerificada = double.Parse(HttpContext.Current.Request.Form.Get("data[" + i + "][qtd]"));
        //            itensConf.QtdSolicitada = double.Parse(HttpContext.Current.Request.Form.Get("data[" + i + "][quantidadeTotal]"));

        //            itens_web.Add(itensConf);
        //            itensConf = new ItensConferencia();
        //        }

        //        string connectionString = DBHelper.GetConnectionString(empresa);
        //        if (numPicking > 0) {

        //            //using (var companyCOM = new COMCompany(empresa))
        //            //{
        //            //    Company oCompany = companyCOM.Company;
        //            new COMCompany(empresa, true);
        //            var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
        //            Company oCompany = com.company;
        //            using (var pickListCOM = new COMPickList(oCompany)) {
        //                var oPickList = pickListCOM._pickList;
        //                if (oPickList.GetByKey(numPicking)) {
        //                    var indexItem = 0;
        //                    var lstItensFil = itens_web.GroupBy(x => x.ItemCode);
        //                    foreach (var lsItem in lstItensFil) {
        //                        var i = 0;
        //                        foreach (var item in lsItem) {

        //                            if (i > 0) {
        //                                oPickList.Lines.BatchNumbers.Add();
        //                                oPickList.Lines.BinAllocations.Add();
        //                            }

        //                            oPickList.Lines.SetCurrentLine(indexItem);
        //                            oPickList.Lines.PickedQuantity = item.QtdVerificada;

        //                            oPickList.Lines.BatchNumbers.SetCurrentLine(i);
        //                            oPickList.Lines.BatchNumbers.BatchNumber = item.NumLote;
        //                            oPickList.Lines.BatchNumbers.Quantity = item.QtdVerificada;
        //                            oPickList.Lines.BatchNumbers.BaseLineNumber = indexItem;

        //                            oPickList.Lines.BinAllocations.SetCurrentLine(i);
        //                            oPickList.Lines.BinAllocations.BinAbsEntry = GlobalHelper.GetBinAbsEntry(empresa, item.Deposito, item.ItemCode);
        //                            oPickList.Lines.BinAllocations.Quantity = item.QtdVerificada;
        //                            oPickList.Lines.BinAllocations.BaseLineNumber = indexItem;
        //                            oPickList.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
        //                            i++;

        //                        }
        //                        indexItem++;

        //                    }

        //                    var res = oPickList.Update();
        //                    if (res != 0) {
        //                        oCompany.GetLastError(out res, out string msg);
        //                        return BadRequest(msg);
        //                    } else {
        //                        string novo_status = "AP";
        //                        string erro_final = GlobalHelper.AtualizarEtapaPickingListaPicking(_context, novo_status, numPicking);
        //                        if (String.IsNullOrEmpty(erro_final)) {
        //                            return Ok();
        //                        } else {
        //                            return BadRequest(erro_final);
        //                        }
        //                    }
        //                } else {
        //                    return BadRequest("Lista de Picking " + numPicking + " não encontrada.");
        //                }
        //            }
        //            //}

        //        } else {
        //            return BadRequest("Lista de Picking não encontrada para o pedido " + numPicking);
        //        }
        //    } catch (Exception e) {
        //        return BadRequest(e.Message);
        //    }
        //}


        [System.Web.Http.HttpPost]
        public IHttpActionResult EncerrarConferencia()
        {
            try
            {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                string str_Row = HttpContext.Current.Request.Form.Get("countRow");
                string str_operador = HttpContext.Current.Request.Form.Get("operador");
                string local = HttpContext.Current.Request.Form.Get("local");
                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numPicking = NumberHelper.StringToInt(str_numdoc);
                int RowCount = NumberHelper.StringToInt(str_Row);

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null)
                {
                    return NotFound();
                }
                

                List<ItensConferencia> itens_web = new List<ItensConferencia>();
                ItensConferencia itensConf = new ItensConferencia();
                // 10 é a quantidade de colunas.
                for (int i = 0; i < RowCount; i++)
                {
                    itensConf.ItemCode = HttpContext.Current.Request.Form.Get("data[" + i + "][item]");
                    itensConf.Descricao = HttpContext.Current.Request.Form.Get("data[" + i + "][descricao]");
                    itensConf.Deposito = HttpContext.Current.Request.Form.Get("data[" + i + "][deposito]");
                    itensConf.NumSerie = HttpContext.Current.Request.Form.Get("data[" + i + "][numSerie]");
                    itensConf.NumLote = HttpContext.Current.Request.Form.Get("data[" + i + "][numLote]");
                    itensConf.OrderLine = HttpContext.Current.Request.Form.Get("data[" + i + "][lineNum]");
                    itensConf.CodigoBarras = HttpContext.Current.Request.Form.Get("data[" + i + "][codigoBarras]");
                    itensConf.QtdVerificada = double.Parse(HttpContext.Current.Request.Form.Get("data[" + i + "][qtd]"));
                    itensConf.QtdSolicitada = double.Parse(HttpContext.Current.Request.Form.Get("data[" + i + "][quantidadeTotal]"));

                    itens_web.Add(itensConf);
                    itensConf = new ItensConferencia();
                }

                string connectionString = DBHelper.GetConnectionString(empresa);
                if (numPicking > 0)
                {

                    //using (var companyCOM = new COMCompany(empresa))
                    //{
                    //    Company oCompany = companyCOM.Company;
                    new COMCompany(empresa, true);
                    var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
                    Company oCompany = com.company;
                    using (var pickListCOM = new COMPickList(oCompany))
                    {
                        var oPickList = pickListCOM._pickList;
                        if (oPickList.GetByKey(numPicking))
                        {
                            var indexItem = 0;
                            var lstItensFil = itens_web.GroupBy(x => x.OrderLine)/*.Select(y => new {Qtd = y.Sum(qtd => qtd.QtdVerificada)})*/; 
                            foreach (var lsItem in lstItensFil)
                            {
                                var i = 0;
                                foreach (var item in lsItem)
                                {
                                    //mimi

                                    if (i > 0)
                                    {
                                        oPickList.Lines.BatchNumbers.Add();
                                        //oPickList.Lines.BinAllocations.Add();
                                    }
                                    oPickList.Lines.SetCurrentLine(indexItem);
                                    //var aux = lsItem.Where(obj => obj.ItemCode == item.ItemCode).Sum(obj => obj.QtdVerificada);
                                    //oPickList.Lines.PickedQuantity = lsItem.Where(obj => obj.ItemCode == item.ItemCode).Sum(obj => obj.QtdVerificada);
                                    // oPickList.Lines.PickedQuantity = lsItem.Where(obj => obj.OrderLine == item.OrderLine).Sum(obj => obj.QtdVerificada);
                                    oPickList.Lines.PickedQuantity = item.QtdSolicitada;

                                    oPickList.Lines.BatchNumbers.SetCurrentLine(i);
                                    oPickList.Lines.BatchNumbers.BatchNumber = item.NumLote;
                                    //oPickList.Lines.BatchNumbers.Quantity = item.QtdVerificada;
                                    oPickList.Lines.BatchNumbers.BaseLineNumber = indexItem;

                                    //oPickList.Lines.BinAllocations.SetCurrentLine(i);
                                    //oPickList.Lines.BinAllocations.BinAbsEntry = GlobalHelper.GetBinAbsEntry(empresa, item.Deposito, item.ItemCode);
                                    //oPickList.Lines.BinAllocations.Quantity = item.QtdVerificada;
                                    //oPickList.Lines.BinAllocations.BaseLineNumber = indexItem;
                                    //oPickList.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                                    i++;

                                }
                                indexItem++;

                            }

                            var res = oPickList.Update();
                            if (res != 0)
                            {
                                oCompany.GetLastError(out res, out string msg);
                                return BadRequest(msg);
                            }
                            else
                            {
                                string novo_status = "AP";
                                string erro_final = GlobalHelper.AtualizarEtapaPickingListaPicking(_context, novo_status, numPicking);
                                if (String.IsNullOrEmpty(erro_final))
                                {
                                    if (VerificaControleLote(empresa.Id)) {
                                        
                                           AtualizaPKL4ControleLote(connectionString, numPicking,"N");
                                           //IncluiCodigoControleLote(connectionString, numPicking);
                                     
                                    }
                                    return Ok();
                                    
                                }
                                else
                                {
                                    return BadRequest(erro_final);
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("Lista de Picking " + numPicking + " não encontrada.");
                        }
                    }
                    //}

                }
                else
                {
                    return BadRequest("Lista de Picking não encontrada para o pedido " + numPicking);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
         
        [System.Web.Http.HttpPost]
        public IHttpActionResult VerificaLote(string codigo_barras,int empresa_id, string Codigo_barrasOri)
        {
            var aux1 = itens_ControleLote.Count;

            try
            {

                // Separando os componentes do código de barras de acordo com os caracteres
                string[] separacao = Codigo_barrasOri.Split(new char[] { ' ', '-' });
                string _codigoItem = "";
                string _codLote = "";
                string _DV = "";
                // verifica se etiqueta está no modelo antigo ou novo
                if (separacao.Length == 3)  //modelo novo (itemCode + CodLote + DV)
                {
                    _codigoItem = separacao[0];
                    _codLote = separacao[1];
                    _DV = separacao[2];

                }
                else if (separacao.Length == 2)    //modelo antigo (CodLote + DV)
                {
                    _codLote = separacao[0];
                    _DV = separacao[1];
                }
                else if (separacao.Length == 1)
                {
                    _codLote = separacao[0];

                }
                else
                {
                    return BadRequest("Código de barras lido está em formato diferente do esperado");
                }
                string CodLote_and_D = $"{_codLote}-{_DV}";

                string countRow = HttpContext.Current.Request.Form.Get("countRow");
                int RowCount = NumberHelper.StringToInt(countRow);
                var Codigo_De_Barras = "";
                var encontrou = false;
                var itemdisponivel = "";
                
                var empresaControlaLote = VerificaControleLote(empresa_id);
                var i = 0;
                if (empresaControlaLote)
                {
                    Codigo_De_Barras = Codigo_barrasOri.ToUpper();
                }
                else
                {
                    Codigo_De_Barras = codigo_barras.ToUpper();
                }

                var aux = HttpContext.Current.Request.Form.Count;
                for (i = 0; i < RowCount; i++)
                {

                    var NumSerie = HttpContext.Current.Request.Form.Get("data[" + i + "][numSerie]").ToUpper();
                    var NumLote = HttpContext.Current.Request.Form.Get("data[" + i + "][numLote]").ToUpper();
                    var CodigoFabricante = HttpContext.Current.Request.Form.Get("data[" + i + "][codigoFabricante]").ToUpper();
                    var CodigoDeBarras = HttpContext.Current.Request.Form.Get("data[" + i + "][codigoBarras]").ToUpper();
                    var quantidade = Convert.ToDouble(HttpContext.Current.Request.Form.Get("data[" + i + "][qtd]").ToUpper());
                    var quantidadeTotal = Convert.ToDouble(HttpContext.Current.Request.Form.Get("data[" + i + "][quantidadeTotal]").ToUpper());
                    var ItemCode = HttpContext.Current.Request.Form.Get("data[" + i + "][item]");

                    if (NumSerie == Codigo_De_Barras
                        || NumLote == Codigo_De_Barras
                        || CodigoDeBarras == Codigo_De_Barras
                        || CodigoFabricante == Codigo_De_Barras)
                    {
                        if (quantidade < quantidadeTotal)
                        {
                            encontrou = true;
                            if (empresaControlaLote) {
                                if (VerificaDisponibilidade(empresa_id, codigo_barras, ItemCode).Equals("Y"))
                                {
                                    ItensLoteCode itensConf = new ItensLoteCode();
                                    itensConf.ItemCode = HttpContext.Current.Request.Form.Get("data[" + i + "][item]");
                                    itensConf.Descricao = HttpContext.Current.Request.Form.Get("data[" + i + "][descricao]");
                                    itensConf.NumLote = HttpContext.Current.Request.Form.Get("data[" + i + "][numLote]");
                                    itensConf.LoteCode = codigo_barras;

                                    if (itens_ControleLote.Count > 0)
                                    {
                                        foreach (var item in itens_ControleLote)
                                        {

                                            if (itensConf.ItemCode == item.ItemCode && itensConf.LoteCode == item.LoteCode)
                                            {
                                                itemdisponivel = "C";
                                                break;
                                            }
                                            else
                                            {
                                                itemdisponivel = "";
                                            }
                                        }
                                        if (itemdisponivel != "C")
                                        {
                                            itemdisponivel = "Y";
                                            itens_ControleLote.Add(itensConf);
                                            break;
                                        }

                                    }
                                    else
                                    {
                                        itemdisponivel = "Y";
                                        itens_ControleLote.Add(itensConf);
                                        break;
                                    }
                                }
                                else if (VerificaDisponibilidade(empresa_id, codigo_barras, ItemCode).Equals("N"))
                                {
                                    itemdisponivel = "N";
                                }
                                else
                                {
                                    itemdisponivel = "S/N";
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                if (encontrou && empresaControlaLote)
                {

                    if (itemdisponivel.Equals("Y"))
                    {
                        return Ok(i);

                    }
                    else  if (itemdisponivel.Equals("C"))
                    {
                        return BadRequest("O item já foi Conferido nesta sessão!");

                    }
                    else if (itemdisponivel.Equals("N"))
                    {
                        return BadRequest("O Item  não está liberado para saída");
                    }
                    else
                    {
                        return BadRequest("O Item  não foi recebido pelo Picking");
                    }
                }
                else if (encontrou && !empresaControlaLote)
                {
                    return Ok(i);
                }
                else
                {
                    return BadRequest("O Lote " + Codigo_De_Barras + " não foi encontrado para este pedido");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        #endregion


        #region :: Funções auxiliares

        public void DeletarConferenciaParcial(Empresa empresa, string numdoc) {
            string delete =
                    @"DELETE FROM [@UPD_PCK_CONFPARCIAL] WHERE U_docentry = " + numdoc;

            // salva na tabela que contabiliza os minutos
            string connectionString = DBHelper.GetConnectionString(empresa);
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                if (conn.State == ConnectionState.Closed) {
                    conn.Open();
                }

                using (SqlCommand command = new SqlCommand("", conn)) {
                    command.CommandText = delete;
                    command.ExecuteNonQuery();
                }
            }
        }
        [System.Web.Http.HttpPost]
        public void LimpaListaLoteConferencia()
        {
            if (itens_ControleLote.Count > 0) {
                itens_ControleLote.Clear();
            }
        }
        public bool VerificaControleLote(int empresa_id)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);

            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();

                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@" select ContadorLote from Picking_Web..Empresas where id = '{0}'", empresa_id);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var Controle = reader["ContadorLote"].ToString();
                        if (Controle.Equals("True"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return false;
                }

            }

        }

        public string VerificaDisponibilidade(int empresa_id,  string CodeItemLote, string ItemCode)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            var Controle = "";
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();

                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@"select U_Status from [@UPD_LOTE] where U_CodigoProduto = '{0}' and U_ItemCode = '{1}'", CodeItemLote, ItemCode);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Controle = reader["U_Status"].ToString();
                       
                            return Controle;
                       
                    }
                    return Controle;
                }

            }
        }
        public string GetDocEntry(int empresa_id, string CodeItemLote)
        {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            string connectionString = DBHelper.GetConnectionString(empresa);
            var Controle = "";
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();

                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@"select U_Status from [@UPD_LOTE] where U_CodigoProduto = '{0}'", CodeItemLote);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Controle = reader["U_Status"].ToString();

                        return Controle;

                    }
                    return Controle;
                }

            }
        }
        #endregion
        #region :: Atualiza status  Controle Lote
        public void AtualizaPKL4ControleLote(string connectionString,int PkEntry, string Disponibilidade)
        {

            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                foreach (var item in itens_ControleLote)
                {
                    using (SqlCommand command = new SqlCommand("", conn2))
                    {
                        string sql = string.Format(@"update [@UPD_Lote] set U_Status = '{0}', U_PkEntry = '{1}' where U_CodigoProduto = '{2}' and U_ItemCode = '{3}'", Disponibilidade, PkEntry, item.LoteCode,item.ItemCode);

                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();


                    }
                }
            }
        }
        #endregion
    }
}