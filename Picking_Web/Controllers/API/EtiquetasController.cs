using Picking_Web.COMObjects;
using Picking_Web.Helpers;
using Picking_Web.Models;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAPbobsCOM;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Picking_Web.Controllers.API {
    public class EtiquetasController : ApiController {
        #region :: Propriedades e Construtor

        private ApplicationDbContext _context;
        string peso_balanca = "";

        public EtiquetasController() {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing) {
            _context.Dispose();
        }

        #endregion


        #region :: Funções

        [System.Web.Http.HttpGet]
        public IHttpActionResult GetPedidoData(int empresa_id, string numdoc) {
            try {
                if (numdoc == "-1") {
                    return Ok(new { });
                }

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

                if (empresa == null) {
                    return NotFound();
                }

                bool pedido_existe = false;
                List<double> volumes = new List<double>() { };
                string parceiro = "",
                    docentry = "",
                    endereco = "",
                    status = "",
                    observacoes = "",
                    connectionString = Helpers.DBHelper.GetConnectionString(empresa)
                ;

                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    string sql =
                    $@"SELECT TOP 1
                        tb0.U_UPD_PCK_STATUS as Status,
						tb0.DocEntry,
						tb0.CardName as Parceiro, 
						tb0.Comments as Observacoes, 
						CASE WHEN tb0.PickRmrk IS NULL
							THEN AddrTypeB + ' ' + StreetB + ' ' + StreetNoS + ', ' + BlockB + ' ' + CityB + ' - ' + StateB
							ELSE tb0.PickRmrk 
						END as Endereco
					FROM ORDR tb0
					INNER JOIN RDR12 tb1 ON(tb1.DocEntry = tb0.DocEntry)
					INNER JOIN PKL1 T2 on T2.OrderEntry = tb0.DocEntry 
                    WHERE T2.AbsEntry = {numdoc} 
                        AND (
                                tb0.U_UPD_PCK_STATUS = 'AS'
                                OR
                                tb0.U_UPD_PCK_STATUS = 'SC'
                            )
                    ORDER BY T2.OrderLine
";
                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = sql;
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            if (reader.Read()) {
                                pedido_existe = true;
                                parceiro = reader["Parceiro"].ToString();
                                docentry = reader["DocEntry"].ToString();
                                endereco = reader["Endereco"].ToString();
                                status = reader["Status"].ToString();
                                observacoes = reader["Observacoes"].ToString();
                            }
                        }
                    }
                }

                List<object> transportadoras = new List<object>();
                if (pedido_existe) {
                    using (SqlConnection conn = new SqlConnection(connectionString)) {
                        if (conn.State == ConnectionState.Closed) {
                            conn.Open();
                        }

                        string sql =
                        @"SELECT 
					    CardCode, isnull(Tb0.CardFName,Tb0.CardName) as CardName
						FROM OCRD tb0
						INNER JOIN OCRG tb1 ON (tb1.GroupCode = tb0.GroupCode)
						WHERE 1=1 
						AND tb0.CardType = 'S' 
						AND tb1.U_U_UPD_PCK_TRANSP = 'S' 
						AND isnull(tb0.U_UPD_PCK_TRSP,'N') = 'S'
						ORDER BY isnull(Tb0.CardFName,Tb0.CardName)";
                        using (SqlCommand command = new SqlCommand("", conn)) {
                            command.CommandText = sql;
                            using (SqlDataReader reader = command.ExecuteReader()) {
                                while (reader.Read()) {
                                    transportadoras.Add(new {
                                        CardCode = reader["CardCode"].ToString(),
                                        CardName = reader["CardName"].ToString()
                                    });
                                }
                            }
                        }
                    }


                    using (SqlConnection conn = new SqlConnection(connectionString)) {
                        if (conn.State == ConnectionState.Closed) {
                            conn.Open();
                        }

                        string sql =
                        $@"SELECT U_peso FROM [@UPD_PCK_VOLUMES] WHERE U_docentry = {numdoc} ORDER BY U_indice";
                        using (SqlCommand command = new SqlCommand("", conn)) {
                            command.CommandText = sql;
                            using (SqlDataReader reader = command.ExecuteReader()) {
                                while (reader.Read()) {
                                    volumes.Add(NumberHelper.GetFromDBToDouble(reader["U_peso"]));
                                }
                            }
                        }
                    }
                } else {
                    return BadRequest("Pedido " + numdoc + " não encontrado");
                }

                return Ok(new {
                    DocEntry = docentry,
                    Parceiro = parceiro,
                    Endereco = endereco,
                    Observacoes = observacoes,
                    Transportadoras = transportadoras,
                    Status = status,
                    Volumes = volumes
                });
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult EncerrarPesagem() {
            try {
                string str_empresa_id = HttpContext.Current.Request.Form.Get("empresa_id");
                string str_numdoc = HttpContext.Current.Request.Form.Get("numdoc");
                int numped = int.Parse(HttpContext.Current.Request.Form.Get("numped"));
                string transportadora = HttpContext.Current.Request.Form.Get("transportadora");
                string str_peso_bruto = HttpContext.Current.Request.Form.Get("pesobruto");
                string str_embalador = HttpContext.Current.Request.Form.Get("embalador");
                string local = HttpContext.Current.Request.Form.Get("local");
                string nome_impressora = HttpContext.Current.Request.Form.Get("nome_impressora");
                string str_volumes = HttpContext.Current.Request.Form.Get("dataset_volume[]");

                int empresa_id = NumberHelper.StringToInt(str_empresa_id);
                int numdoc = NumberHelper.StringToInt(str_numdoc);
                double peso_bruto = NumberHelper.StringToDouble(str_peso_bruto);

                string[] volumes = str_volumes.Split(',');
                int qtd_volumes = volumes.Length;

                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                if (empresa == null) {
                    return NotFound();
                }

                //DeletarVolumesCabecalho(empresa, numdoc);
                //DeletarVolumes(empresa, numdoc);


                bool Att = GlobalHelper.AttCamposEmbalagemListaPicking(empresa, numdoc, transportadora, qtd_volumes, peso_bruto, "PE");
                if (Att) {

                    //if (empresa.NomeBanco == "SBO_CENTERLAB_PRD") {
                    //    ImprimirEtiquetaComZebraDesigner(empresa, numped, nome_impressora);
                    //} else {
                    //    ImprimirEtiquetaComCrystal(empresa, numped, nome_impressora);
                    //}

                    return Ok();

                } else {
                    return BadRequest("Falha ao atualizar lista de picking.");
                }


                //if (SalvarVolumes(str_volumes, numdoc, empresa)) {

                //} else {
                //    return BadRequest("Erro. Não foi possível salvar os volumes.");
                //}

                //}
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult Reimprimir(int empresa_id, int numdoc, string nome_impressora) {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null) {
                return NotFound();
            }
            try {
                if (empresa.NomeBanco == "SBO_CENTERLAB_PRD") {
                    ImprimirEtiquetaComZebraDesigner(empresa, numdoc, nome_impressora);
                } else {
                    ImprimirEtiquetaComCrystal(empresa, numdoc, nome_impressora);
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Reset(int empresa_id, int numdoc) {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null) { 
                return NotFound();
            }
            try {
                DeletarVolumes(empresa, numdoc);

                //using (var companyCOM = new COMCompany(empresa))
                //{
                //    Company oCompany = companyCOM.Company;
                new COMCompany(empresa, true);
                var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
                Company oCompany = com.company;
                using (var docCOM = new COMDocs(oCompany, BoObjectTypes.oOrders)) {
                    Documents oDoc = docCOM.Documents;
                    if (oDoc.GetByKey(numdoc)) {
                        oDoc.TaxExtension.Carrier = "";
                        oDoc.TaxExtension.GrossWeight = 0;
                        oDoc.TaxExtension.PackQuantity = 0;

                        var connectionString = Helpers.DBHelper.GetConnectionString(empresa);
                        string sql = $@"SELECT LineNum, FreeChrgBP FROM POR1 tb0 WHERE tb0.DocEntry = {numdoc}";
                        using (SqlConnection conn = new SqlConnection(connectionString)) {
                            if (conn.State == ConnectionState.Closed) {
                                conn.Open();
                            }

                            using (SqlCommand command = new SqlCommand("", conn)) {
                                command.CommandText = sql;
                                using (SqlDataReader reader = command.ExecuteReader()) {
                                    while (reader.Read()) {
                                        var lineNum = Convert.ToInt32(reader["LineNum"].ToString());
                                        var freeOfCharge = reader["FreeChrgBP"].ToString();
                                        for (int i = 0; i < oDoc.Lines.Count; i++) {
                                            if (oDoc.Lines.LineNum == lineNum) {
                                                oDoc.Lines.SetCurrentLine(i);
                                            }
                                        }

                                        if (freeOfCharge == "Y") {
                                            oDoc.Lines.FreeOfChargeBP = BoYesNoEnum.tYES;
                                        }
                                    }
                                }
                            }
                        }

                        if (oDoc.Update() != 0) {
                            return BadRequest(oCompany.GetLastErrorDescription());
                        }
                    } else {
                        return BadRequest("Pedido " + numdoc + " não encontrado");
                    }

                    string novo_status = "PE";
                    string erro_final = GlobalHelper.RemoverEtapaPicking(empresa, numdoc, novo_status, "EP");
                    if (String.IsNullOrEmpty(erro_final)) {
                        return Ok();
                    } else {
                        return BadRequest(erro_final);
                    }
                }
                //}
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult ReimprimirVolume(int empresa_id, int numdoc, string nome_impressora, int index) {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);

            if (empresa == null) {
                return NotFound();
            }
            try {
                if (empresa.NomeBanco == "SBO_CENTERLAB_PRD") {
                    ImprimirEtiquetaComZebraDesigner(empresa, numdoc, nome_impressora, index);
                } else {
                    ImprimirEtiquetaComCrystal(empresa, numdoc, nome_impressora, index);
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult BuscarPesoBalanca(int empresa_id, string ip, int porta) {
            peso_balanca = "";

            if (!String.IsNullOrEmpty(ip) && porta > 0) {
                RetornaPesoBalanca(ip, porta);

                int limite_tentativas = 10,
                    tentativas = 1,
                    default_sleep = 1000;
                while (tentativas <= limite_tentativas) {
                    if (String.IsNullOrEmpty(peso_balanca)) {
                        System.Threading.Thread.Sleep(default_sleep);
                    } else if (peso_balanca == "-9999") {
                        peso_balanca = "";
                    } else {
                        break;
                    }

                    tentativas++;
                }
            }

            return Ok(peso_balanca);
        }


        #endregion


        #region :: Funções Acessórias

        private bool SalvarVolumes(string str_volumes, int numdoc, Empresa empresa) {
            int affected_rows = 0;
            string nome_tabela = "[@UPD_PCK_VOLUMES]";
            string insert = GlobalHelper.SQLDeclareTableID(nome_tabela);

            string[] volumes = str_volumes.Split(',');
            for (int i = 0; i < volumes.Length; i++) {
                insert += $@"INSERT INTO {nome_tabela}
                                (Code, Name, U_docentry, U_volume, U_peso, U_indice)
                            VALUES
                                (@table_id + " + i + ", @table_id + " + i + ", " + numdoc + ", 'Volume " + (i + 1) + "/" + (volumes.Length) + "'  , '" + volumes[i] + "', '" + (i + 1) + "'); ";
            }

            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = insert;
                        affected_rows = command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }

            return affected_rows > 0;
        }

        /// <summary>
        /// manda imprimir etiqueta
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        private void ImprimirEtiquetaComZebraDesigner(Empresa empresa, int numdoc, string printername, int index = 0) {
            string logo_centerlab = "GW2,924,24,272,ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ«þÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿWUõÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþ++ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿWU÷ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþªªúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþª¢ûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUuwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú:z~ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýuuuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúºúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿý}uwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú**~ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿû»»ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþ~ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿêúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþªÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿêªûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUUwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ**~ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúª«þÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUWõÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúª£ûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýU×ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú¯ëúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýU×õÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú*ëÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUW÷ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúª«þÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþªªûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿêªÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþ*zÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿêþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõÿÿÿÿÿÿÿuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿÿÿÕwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ¿ÿÿÿÿÿÿê{ÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUÿÿÿÿÿÿÕuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿêÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕUÿÿÿÿÿÿÕ}ÿÿÿÿÿÿÿÿÿÿÿÿÿû»ÿÿÿÿÿÿêþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕUÿÿÿÿÿÿÕuÿÿÿÿÿÿÿÿÿÿÿÿÿÿïÿÿÿÿÿÿÿêÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUWÿÿÿÿÿÿÕuÿÿÿÿÿÿÿÿÿÿÿÿÿÿ»¿ÿÿÿÿÿÿêzÿÿÿÿÿÿÿÿÿÿÿÿÿýU_ÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿþïÿÿÿÿÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿÿõUÿÿÿÿÿýUUwÿÿÿÿÿÿÿÿÿÿÿÿû»ÿÿÿÿÿúª¢úÿÿÿÿÿÿÿÿÿÿÿÿÿõWÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿîïÿÿÿÿÿúªªúÿÿÿÿÿÿÿÿÿÿÿÿÿÕ_ÿÿÿßÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿû¿ÿÿÿÿÿú**ÿÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿ÷ÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿªÿÿÿÿÿÿúªªþÿÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÕÿýÕÕ÷ÿÿÿÿÿÿÿÿÿÿÿÿ»ÿÿÿÿÿÿÿÿÿþÿÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿ÷ÿÿÿÿwÿÿÿÿÿÿÿÿÿÿÿþ¯ÿÿÿÿÿÿÿÿþûÿÿÿÿÿÿÿÿÿÿÿÿýWÿÿÿÿýÿ×ý}ÿÿÿÿÿÿÿÿÿÿÿÿÿ»ÿÿÿÿÿÿÿ+ú~ÿÿÿÿÿÿÿÿÿÿÿÿýWÿÿÿÿÿÿUõuÿÿÿÿÿÿÿÿÿÿÿú¯ÿÿÿÿÿÿþªêþÿÿÿÿÿÿÿÿÿÿÿÿõ_ÿÿÿÿýÿýUUuÿÿÿÿÿÿÿÿÿÿÿû¿ÿÿÿÿÿÿþª¢úÿÿÿÿÿÿÿÿÿÿÿÿõ_ÿÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿê¿ÿÿÿÿÿÿþªªþÿÿÿÿÿÿÿÿÿÿÿÿõ_ÿÿÿýÿßýUU}ÿÿÿÿÿÿÿÿÿÿÿë¿ÿÿÿþÿÿú>+ûÿÿÿÿÿÿÿÿÿÿÿÿõÿÿÿÿý]W÷ÿÿÿÿÿÿÿÿÿÿÿÿê¿ÿÿÿþ¿ÿú¾¯ûÿÿÿÿÿÿÿÿÿÿÿÿÕÿÿÿý_ßý}_ýÿÿÿÿÿÿÿÿÿÿÿëÿÿÿÿþ¿ÿú¾¿ÿÿÿÿÿÿÿÿÿÿÿÿÿÕwÿ÷_÷ýUUÿÿÿÿÿÿÿÿÿÿÿªÿÿÿÿþ¯ÿúªªþÿÿÿÿÿÿÿÿÿÿÿÿÕÿUßÿý_ßýUUÿÿÿÿÿÿÿÿÿÿÿ«þ+ÿÿþ/ÿú**zÿÿÿÿÿÿÿÿÿÿÿÿUÿUwÿ÷_÷ýUUwÿÿÿÿÿÿÿÿÿÿÿ«þªÿÿþ¯ÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿUÿUUÿõ_ßýUUÿÿÿÿÿÿÿÿÿÿÿÿ«þ¢¿ÿþ¯ÿúª¢ÿÿÿÿÿÿÿÿÿÿÿÿÿUÿUUõ_÷ýUU}ÿÿÿÿÿÿÿÿÿÿÿ«ÿª¯ÿþ¯ÿÿÿÿûÿÿÿÿÿÿÿÿÿÿÿÿUÿUU_õ_ÿÿÿÿ÷ÿÿÿÿÿÿÿÿÿÿÿÿ+ÿê+¿ú/ÿÿÿÿûÿÿÿÿÿÿÿÿÿÿÿÿUÿõUWõ_÷ÿÿÿýÿÿÿÿÿÿÿÿÿÿþ«ÿúªïþ¯ÿÿÿÿþÿÿÿÿÿÿÿÿÿÿÿÿUÿýUUõ_ýý_õÿÿÿÿÿÿÿÿÿÿÿþ«ÿþª»ú¯ÿúººûÿÿÿÿÿÿÿÿÿÿÿÿWÿÿUUU_÷ýUuuÿÿÿÿÿÿÿÿÿÿþ«ÿÿªªª¯ÿúºúÿÿÿÿÿÿÿÿÿÿÿÿÿWÿÿÕUU_ÿýUuuÿÿÿÿÿÿÿÿÿÿÿþ+ÿÿê**/ÿú::zÿÿÿÿÿÿÿÿÿÿÿÿWÿõUU_÷ýUuÿÿÿÿÿÿÿÿÿÿþ«þ¿úªª¯ÿúºúþÿÿÿÿÿÿÿÿÿÿÿÿWý_ýUU_ýýUu}ÿÿÿÿÿÿÿÿÿÿÿþ«þ¯ÿ¢ª¯ÿúª¢ÿÿÿÿÿÿÿÿÿÿÿÿÿWýWÿÕU_÷ýUUwÿÿÿÿÿÿÿÿÿÿÿþ«þ¯ÿêª¯ÿúªªûÿÿÿÿÿÿÿÿÿÿÿÿWýWÿõU_ÿýUU}ÿÿÿÿÿÿÿÿÿÿþ+þ/ÿú*/ÿú**{ÿÿÿÿÿÿÿÿÿÿÿÿWýWÿýU_÷ýUUuÿÿÿÿÿÿÿÿÿÿÿþ«þ¯ÿþª¯ÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿWýWÿÿU_ýýUUuÿÿÿÿÿÿÿÿÿÿÿÿ«þ¯ÿÿê¯ÿúªªúÿÿÿÿÿÿÿÿÿÿÿÿUýWÿÿõ_÷ÿwwÿÿÿÿÿÿÿÿÿÿÿÿ«þ¯ÿÿú¯ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUýWÿÿõ_ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ+þ+ÿÿú/ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUÿWÿÿõ_÷ÿÿõÿÿÿÿÿÿÿÿÿÿÿÿ«þ«ÿÿú¯ÿú¯ÿþÿÿÿÿÿÿÿÿÿÿÿÿUÿUÿÿõ_ÿý_ÿõÿÿÿÿÿÿÿÿÿÿÿ«ÿ£ÿÿê¿ÿú¯ÿúÿÿÿÿÿÿÿÿÿÿÿÿUÿUÿÿÕ_÷ý_ÿ÷ÿÿÿÿÿÿÿÿÿÿÿ«ÿªÿÿê¿ÿú¯ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕÿUÿU_ßýUUuÿÿÿÿÿÿÿÿÿÿÿÿ«ÿª?þ*?ÿú**~ÿÿÿÿÿÿÿÿÿÿÿÿÕU_ýU÷ýUUwÿÿÿÿÿÿÿÿÿÿÿªÿê¯úª¿ÿúªªþÿÿÿÿÿÿÿÿÿÿÿÿÕÕUUUßýUUwÿÿÿÿÿÿÿÿÿÿÿÿëÿêªªªÿÿúªªúÿÿÿÿÿÿÿÿÿÿÿÿÕõUUUwýUUuÿÿÿÿÿÿÿÿÿÿÿÿê¿úªªªÿÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿõõUUUÿ_ýUUuÿÿÿÿÿÿÿÿÿÿÿÿë¿ú**+ÿÿú**~ÿÿÿÿÿÿÿÿÿÿÿÿõ_ýUUWÿýWwuÿÿÿÿÿÿÿÿÿÿÿê¿þªª¯ÿÿú¯ÿÿÿÿÿÿÿÿÿÿÿÿÿÿõ_ÿUU_ýßý_ÿ÷ÿÿÿÿÿÿÿÿÿÿÿÿû¿ÿª¢¿ÿÿú¯ÿúÿÿÿÿÿÿÿÿÿÿÿÿõ_ÿÕUÿý_ÿõÿÿÿÿÿÿÿÿÿÿÿÿú¯ÿêªÿÿÿú¯ÿÿÿÿÿÿÿÿÿÿÿÿÿÿýWÿýWÿýÿÿÿÿõÿÿÿÿÿÿÿÿÿÿÿÿ»ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýWÿÿÿÿ÷ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþ«ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿÕÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿ»ÿÿÿÿÿÿúªªûÿÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿ÷ÿýUUwÿÿÿÿÿÿÿÿÿÿÿÿÿ®ÿÿÿÿÿÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕÿÿÿ_ÿýUUwÿÿÿÿÿÿÿÿÿÿÿÿû¿ÿÿÿÿÿú**~ÿÿÿÿÿÿÿÿÿÿÿÿÿÕ_ÿÿÿwÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿîïÿÿÿÿÿúªªúÿÿÿÿÿÿÿÿÿÿÿÿÿõWÿÿõ_ÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿû»ÿÿÿÿÿÿþ¢ÿÿÿÿÿÿÿÿÿÿÿÿÿÿõUÿÿ÷ÿÿýUÿÿÿÿÿÿÿÿÿÿÿÿÿþïÿÿÿÿÿÿú«ûÿÿÿÿÿÿÿÿÿÿÿÿÿýU_ýUÿÿõW÷ÿÿÿÿÿÿÿÿÿÿÿÿÿÿ»¿ÿÿÿÿÿê/ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUUWWÿÿÕ_õÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿªÿþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕUUUÿÿýUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿû»ÿÿÿÿþ«ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõUUwÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿ¿¿ÿÿÿú**zÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕWÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúª¢ûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ÷ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿý_õÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú»úÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúºúþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUuuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú::zÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúºúþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUuuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúº¢ûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUU}ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú**ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿúªªÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýWÕwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú¯âÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿý_Õuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú¯êþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿý_õ}ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú?ê{ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿý_õwÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú¯êúÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿý_Õuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿú¯êÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýWÕuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþªªþÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿýUUuÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþ**úÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿþª«þÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿUU÷ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿª£þÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÕWõÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿê¯ûÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõ_ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ";
            string logo_gold = "GW46,929,13,268,ÿÿÿÿÿÿð ÿÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿÂ\"#ÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿ€ˆ€ÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿ£/ûÿÿÿÿÿÿÿÿÿÿ××ÿÿÿÿÿÿÿÿÿÿÿ‡ÿÿÿÿÿÿÿÿÿÿÿß×õÿÿÿÿÿÿÿÿÿÿ¯Ãúÿÿÿÿÿÿÿÿÿÿ××ýÿÿÿÿÿÿÿÿÿÿãøÿÿÿÿÿÿÿÿÿÿ×õýÿÿÿÿÿÿÿÿÿÿ§ãúÿÿÿÿÿÿÿÿÿÿ×õõÿÿÿÿÿÿÿÿÿÿèÿÿÿÿÿÿÿÿÿÿ×õUÿÿÿÿÿÿÿÿÿÿóò#ÿÿÿÿÿÿÿÿÿÿÿýUÿÿÿÿÿÿÿÿÿÿÿü‡ÿÿÿÿÿÿÿÿÿÿÿÿ_ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ×ÿ_ÿÿÿÿÿÿÿÿÿÿ§þ'ÿÿÿÿÿÿÿÿÿÿ×ýWÿÿÿÿÿÿÿÿÿÿ‡ð‰ÿÿÿÿÿÿÿÿÿÿ×õUÿÿÿÿÿÿÿÿÿÿ§â#ÿÿÿÿÿÿÿÿÿÿ×ÕUÿÿÿÿÿÿÿÿÿÿ‰èÿÿÿÿÿÿÿÿÿÿÕWõÿÿÿÿÿÿÿÿÿÿ¢'òÿÿÿÿÿÿÿÿÿÿÕWõÿÿÿÿÿÿÿÿÿÿ€ðÿÿÿÿÿÿÿÿÿÿÕ_õÿÿÿÿÿÿÿÿÿÿâãÿÿÿÿÿÿÿÿÿÿýÿõÿÿÿÿÿÿÿÿÿÿÿÿáÿÿÿÿÿÿÿÿÿÿÿÿßÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ¿ÿÿÿÿÿÿÿÿÿÿÿÿ×ÕUßÿÿÿÿÿÿÿÿÿ§¢\"!ÿÿÿÿÿÿÿÿÿ×ÕUUÿÿÿÿÿÿÿÿÿˆ	ÿÿÿÿÿÿÿÿÿ×ÕUUÿÿÿÿÿÿÿÿÿ§‚\"#ÿÿÿÿÿÿÿÿÿÿÿuUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ€ÿÿÿÿÿÿÿÿÿÿÿÿÕUUUÿÿÿÿÿÿÿÿÿ¢\"\"!ÿÿÿÿÿÿÿÿÿÕUUUÿÿÿÿÿÿÿÿÿ€ˆ€‰ÿÿÿÿÿÿÿÿÿÕUUUÿÿÿÿÿÿÿÿÿÿ¢!ÿÿÿÿÿÿÿÿÿÿÿ÷UÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿõU_ÿÿÿÿÿÿÿÿÿÿÂ\"!ÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿˆ	ÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿ¢\"#ÿÿÿÿÿÿÿÿÿÿW_ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ_×ÿÿÿÿÿÿÿÿÿÿÿ?§óÿÿÿÿÿÿÿÿÿÿ_×õÿÿÿÿÿÿÿÿÿÿÏñÿÿÿÿÿÿÿÿÿÿ_×õÿÿÿÿÿÿÿÿÿÿ¯Ãóÿÿÿÿÿÿÿÿÿÿ_Õõÿÿÿÿÿÿÿÿÿÿèáÿÿÿÿÿÿÿÿÿÿ×õUÿÿÿÿÿÿÿÿÿÿ‡â#ÿÿÿÿÿÿÿÿÿÿ÷õWÿÿÿÿÿÿÿÿÿÿÿøÿÿÿÿÿÿÿÿÿÿÿýWÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿâ ;ÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿˆÿÿÿÿÿÿÿÿÿÿÕUUÿÿÿÿÿÿÿÿÿÿ¢\"#ÿÿÿÿÿÿÿÿÿÿUUÿÿÿÿÿÿÿÿÿÿÿûÿÿÿÿÿÿÿÿÿÿWÿÿÿÿÿÿÿÿÿÿÿÿ'ÿÿÿÿÿÿÿÿÿÿÿÿWÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿWÿÿÿÿÿÿÿÿÿÿÿÿ§ÿÿÿÿÿÿÿÿÿÿÿÿ×ÿÿÿÿÿÿÿÿÿÿÿÿËÿÿÿÿÿÿÿÿÿÿÿÿÕÿÿÿÿÿÿÿÿÿÿÿÿòÿÿÿÿÿÿÿÿÿÿÿÿUUÿÿÿÿÿÿÿÿÿÿˆÿÿÿÿÿÿÿÿÿÿUUUÿÿÿÿÿÿÿÿÿÿ¢\"#ÿÿÿÿÿÿÿÿÿÿUUWÿÿÿÿÿÿÿÿÿÿ€ ƒÿÿÿÿÿÿÿÿÿÿÿõUÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ÷ÿÿÿÿÿÿÿÿÿÿÿÿâÿÿÿÿÿÿÿÿÿÿÿÿÕÿÿÿÿÿÿÿÿÿÿÿÿˆÿÿÿÿÿÿÿÿÿÿÿýUÿÿÿÿÿÿÿÿÿÿÿò#ÿÿÿÿÿÿÿÿÿÿUUUÿÿÿÿÿÿÿÿÿàÿÿÿÿÿÿÿÿÕUUUUÿÿÿÿÿÿÿâ\"\"\"\"#ÿÿÿÿÿÿõUUUUUUÿÿÿÿÿÿøˆ€ˆ€ˆ€ÿÿÿÿÿÿõUUUUUUÿÿÿÿÿÿú\"\"\"\"\"#ÿÿÿÿÿÿõUUUUUUÿÿÿÿÿÿø	ÿÿÿÿÿÿõUUUUUUÿÿÿÿÿÿú\"\"\"\"\"#ÿÿÿÿÿÿõUUUUUUÿÿÿÿÿÿøÈ€ƒÿ	ÿÿÿÿÿÿõUõUÿÿÕÿÿÿÿÿÿú#ÿÿÿÿãÿÿÿÿÿÿýUÿÿÿÿõÿÿÿÿÿÿþ	ÿÿÿÿéÿÿÿÿÿÿÿUÿÿÿÿõÿÿÿÿÿÿÿãÿÿÿÿãÿÿÿÿÿÿÿõÿÿÿÿõÿÿÿÿÿÿÿøãÿÿÿÿÿÿÿÿÿÿÿýUÿÿÿÿÿÿÿÿÿÿÿÿ\"ÿÿÿÿÿÿÿÿÿÿÿÕ_ÿÿÿÿÿÿÿÿÿÿÿèÿÿÿÿÿÿÿÿÿÿÿõWÿÿÿÿÿÿÿÿÿÿÿþ#ÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿÿÿÿÿÿÿÿˆ¿ÿÿÿÿÿÿÿÿÿÿÿÕ_ÿÿÿÿÿÿÿÿÿÿÿò'ÿÿÿÿÿÿÿÿÿÿÿýWÿ÷ÿÿÿÿÿÿÿÿÿþÿéÿÿÿÿÿÿÿÿÿÿU_Õÿÿÿÿÿÿÿÿÿÿ¢?ãÿÿÿÿÿÿÿÿÿÿõWUÿÿÿÿÿÿÿÿÿÿð€ÿÿÿÿÿÿÿÿÿÿýUUÿÿÿÿÿÿÿÿÿÿþ\"#ÿÿÿÿÿÿÿÿÿÿÿUUÿÿÿÿÿÿÿÿÿÿÿŠ	ÿÿÿÿÿÿÿÿÿÿÿõUÿÿÿÿÿÿÿÿÿÿÿò#ÿÿÿÿÿÿÿÿÿÿÿýUÿÿÿÿÿÿÿÿÿÿÿü‰ÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿÿÿÿÿÿÿÿÿ#ÿÿÿÿÿÿÿÿÿÿÿÿUÿÿÿÿÿÿÿÿùÿÿÿ‰ÿÿÿÿÿÿÿÿUÿÿÿÕÿÿÿÿÿÿÿø\"ÿÿãÿÿÿÿÿÿÿUÿÿÕÿÿÿÿÿÿøˆ€ŸÿÿùÿÿÿÿÿÿÕUU_ÿÿÿÿÿÿÿÿþ\"\"\"'ÿÿÿÿÿÿÿÿýUUUWÿÿÿÿÿÿÿÿü	ÿÿÿÿÿÿÿÿýUUUUÿÿÿÿÿÿÿÿú\"\"\"\"ÿÿÿÿÿÿÿýUUUUÿÿÿÿÿÿøˆ€ˆ€ŸÿÿðŸÿÿÿõUUUUWÿÿU_ÿÿÿò\"\"\"\"'ÿø\"/ÿÿÿõUUUUWÿUUWÿÿÿø	þÿÿÿõUUUUUÕUUUÿÿÿâ\"\"\"\"#¢\"\"\"ÿÿõUUUUUÕUUUÿÿè€ˆ€ˆˆ€ˆ€ŸÿÿÕUUUUUUUUU_ÿÿâ\"\"\"\"#\"\"\"\"'ÿÿÕUUUUWUUUUWÿÿÈ 	ÿÿÕUUUUWUUUUUÿÿ¢\"\"\"\"\"\"\"\"\"\"ÿUUUUUUUUUUUÿ€ˆ€ˆ€„€ˆ€ˆ€ŸÿUUUUUUUUUUU_ÿ¢\"\"\"\".\"\"\"\"\"ÿÕUUUUUUUUUUÿÈ?ÿõUUUU]UUUUUÿò\"\"\"\":\"\"\"\"\" ? ÿýUUUU]UUUUUÿþ€ˆ€ˆ˜ˆ€ˆ€ˆÿÿUUUUUUUUUUÿÿ\"\"\"\"2\"\"\"\"\"ÿÿÿÕUUUuUUUUUÿÿè(ÿÿÿõUUUuUUUUUÿÿÿò\"\"/ò\"\"\"\"#ÿÿÿýUUUUUUUÿÿÿü€ƒøŒ€ˆ€ˆÿÿÿÿUUWUUUUUÿÿÿÿ£ø\"#\"\"\"\"#ÿÿÿÿßUUUÕUUUWÿÿÿÿüÈÿÿÿÿÕUUUuUUUWÿÿÿÿâ\"\"\"2\"\"\"'ÿÿÿÿÕUUU]UUUWÿÿÿÿˆ€ˆ€Œ€ˆ€ÿÿÿÿÕUUUWUUUÿÿÿÿ¢\"\"\"#\"\"/ÿÿÿÿÿÕUUUUÕUÿÿÿÿÿˆÈÿÿÿÿÿÿUUUUUu_ÿÿÿÿÿÿ\"\"\"\"\"3ÿÿÿÿÿÿÿUUUUU_ÿÿÿÿÿÿþ€ˆ€ˆ€ÿÿÿÿÿÿÿUUUUU_ÿÿÿÿÿÿþ\"\"\"\"\"?ÿÿÿÿÿÿýUUUUU_ÿÿÿÿÿÿü?ÿÿÿÿÿÿýUUUUUÿÿÿÿÿÿþ\"\"\"\"\"?ÿÿÿÿÿÿýUUUUUÿÿÿÿÿÿøˆ€ˆ€ˆÿÿÿÿÿÿýUUUUUÿÿÿÿÿÿú\"\"\"\"\"ÿÿÿÿÿÿõUUUUUÿÿÿÿÿÿðÿÿÿÿÿÿÿõUUUUUÿÿÿÿÿÿÿú\"\"\"\"\"ÿÿÿÿÿÿÿýUUUUUÿÿÿÿÿÿÿþ€ˆ€ˆÿÿÿÿÿÿÿÿUUUUUÿÿÿÿÿÿÿÿ¢\"\"\"!ÿÿÿÿÿÿÿÿÕUUUUÿÿÿÿÿÿÿÿèÿÿÿÿÿÿÿÿõUUUWÿÿÿÿÿÿÿÿþ\"\"\"#ÿÿÿÿÿÿÿÿýUUUWÿÿÿÿÿÿÿÿÿ ˆ€¿ÿÿÿÿÿÿÿÿÿUUUÿÿÿÿÿÿÿÿÿÿâ\"ÿÿÿÿÿÿÿÿÿÿõUÿÿÿÿÿÿÿÿÿÿÿøÿÿÿÿÿÿÿÿÿÿÿõÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ";
            string logo_labshop = "GW64,928,8,266,J07FCP07FE,I03IF8N03IFC,I0JFEN0KF,003KF8L03KFC,007KFCL07KFE,01LFEL0MF,03MFK01MF8,03MF8J03MFC,07MFCJ07MFE,0NFEJ0NFEJ07FX07E,0NFEJ0OFJ07FX07E,1OFI01OFJ07FX07E,1OFI01OF8I07FX07E,3OF8003OF8I07FX07E,3OF8003OFCI07FX07E,7OF8003OFCI07FX07E,7OFC003OFCI07FX07E,7OFC007OFCI07FX07E,:7OFE007OFCI07FX07E,:7OFE007OFCI07FX07EM03E,7PF007OFCI07FX07EL03FFEI01FgO01E,7PF007OFCI07FP03IFJ07E07FE00JFC001FgO01E,7PF803OFCI07FO01JFEI07E1IFC3JFE001FgO01E,7PF803OFCI07FO07KFI07E3IFE7KF001FgO01E,3PF803OF8I07FO0LF8007E7KFE003F801FgO01E,3PFC03OF8I07FN01FFC3FFC007IF1IFC001F801F,3PFC01OF8I07FN03FC003FC007FF803FF8I0FC01F,1PFC01OFJ07FN03F8001FE007FF001FFJ07C01F,1PFE00OFJ07FN07FJ0FE007FEI0FFJ03E01F,0PFE00NFEJ07FN07EJ07E007FCI07FJ03E01F,0QF007MFCJ07FN0FEJ07E007F8I03FJ03E01F01CL0EM0EL01CO018K018,07PF003MFCJ07FN0FCJ07E007F8I03F8L01F0FFCJ0FFEI0787FCI0F0FF8001E01E1FF8I01FF878,03PF801MF8J07FO04J07E007FJ03F8L01F3FFEI03IF80079IFI0F3FFE001E01E3FFEI07FFC78,01PF800MFK07FT07E007FJ01F8L01F7IFI07IFC007BIF800F7IF001E01EJFI0IFE78,00PFC007KFEK07FT07E007FJ01FCL01FFC7F800FE0FE007FC0FC00FF83F801E01EF87F001FC1FF8,003OFE003KF8K07FT0FE007EJ01FF8K01FE01F801F803F007F807E00FF00FC01E01FE01F803F007F8,001PFI0KFL07FS0FFE007EJ01IF8J01FC00FC03F001F807F003E00FE007C01E01FC00F803E003F8,I07OFI03IF8L07FQ07IFE007EJ01JF8I01F8007C03EI0F807E001F00FC003E01E01F8007C07C001F8,J0OF8I07FCM07FO01KFE007EK0KFI01F8007C07CI07C07CI0F00F8003E01E01FI07C07C001F8,J03NFCS07FO07KFE007EK0KFC001FI03C07CI07C07CI0F80F8001E01E01FI07C078I0F8,K03MFES07FN01JFE7E007EK0FC7IF001FI03C07CI03C07CI0F80FI01F01E01FI07C0F8I0F8,L07MF8R07FN03IFE07E007EK0FC07FF801FI03C078I03C078I0780FI01F01E01FI07C0F8I0F8,M0MFCR07FN07FF8007E007EJ01FC00FFC01FI03C078I03E078I0780FI01F01E01EI07C0F8I0F8,M01MFR07FN07F8I07E007EJ01FC001FE01FI03C0F8I03E078I0780FJ0F01E01EI07C0F8I0F8,N03LF8Q07FN0FEJ07E007EJ01F8I07E01FI03C0F8I03E078I0780FJ0F01E01EI07C0F8I078,N01LFEQ07FN0FCJ07E007EJ01F8I03F01FI03C0F8I03E078I0780FJ0F01E01EI07C0F8I078,O07LFCP07FM01FCJ07E007FJ01F8I01F01FI03C0F8I03E078I0780FJ0F01E01EI07C0F8I0F8,O01MFP07FM01FCJ07E007FJ07F8I01F01FI03C0F8I03E078I0780FJ0F01E01EI07C0F8I0F8,P0MFEO07FM01F8J0FE007FJ07FJ01F01FI03C078I03E078I0780FI01F01E01EI07C0F8I0F8,P07MFEN07FM01F8J0FE007F8I07FK0F01FI03C078I03C078I0F80FI01F01E01EI07C0F8I0F8,P03NFCM07FM01FCI01FE007F8I07FJ01F01FI03C07CI07C07CI0F80FI01F01E01EI07C078I0F8,I01FFEI01OFM07FM01FCI03FF007FCI0FEJ01F01FI03C07CI07C07CI0F00F8001E01E01EI07C07C001F8,I0JFCI0OFEL07FN0FEI07FF007FE001FFJ03E01FI03C07CI07C07C001F00F8003E01E01EI07C07C001F8,003KFI07OF8K07MF80FF001IF007FF803FF8I03E01FI03C03EI0F807E001F00FC003E01E01EI07C03E003F8,007KF8003OFCK07MF80FFC07FBF007FFE1IFEI0FC01FI03C03F001F807F003E00FE007C01E01EI07C03F007F8,00LFE003OFEK07MF807KF3F007EMF803FC01FI03C01F803F007F807E00FF00F801E01EI07C01FC1FF8,01MF001PFK07MF803JFC3F007E7IFE7KF801FI03C00FE0FE007FE1FC00FFC7F801E01EI07C00IFEF8,03MF800PF8J07MF801JF81F807E1IF83KF001FI03C007IFC007BIF800F7IF001E01EI07C007FFCF8,07MF800PFCJ07MF8007FFC01F807E0FFE00JFC001FI03C003IF80079FFEI0F3FFC001E01EI07C003FF8F8,0NFC00PFEV03CP0EI03IFI01FI03CI0FFEI0787FCI0F0FFI01E01EI07CI07C0F8,1NFE007OFEhU078L0FgH0F,1NFE007PFhU078L0FgH0F,3OF007PFhU078L0FgH0F,3OF003PF8hT078L0FW078001F,3OF803PF8hT078L0FW07C001F,7OF803PFChT078L0FW07C003E,7OF801PFChT078L0FW03E007E,7OF801PFChT078L0FW03F81FC,7OFC01PFChT078L0FW01JF8,7OFC00PFChT078L0FX0JF,7OFC00PFChT078L0FX03FFC,7OFC00PFCjH03C,7OFC007OFC,:7OF8007OFC,7OF8003OFC,:3OF8003OF8,3OFI01OF8,:1NFEJ0OF,:0NFCJ07MFE,07MFCJ07MFC,07MF8J03MF8,03MFK01MF,01LFEL0LFE,007KFCL07KFC,003KFM01KF8,I0JFCN07IFE,I03IFO01IF,J01F,^FS";

            string nome_banco = empresa.NomeBanco;
            string logo = nome_banco == "SBO_LABSHOPPING_PRD" ? logo_labshop : (nome_banco == "SBO_GOLDANALISA_PROD" ? logo_gold : logo_centerlab);
            string bplname = "";
            string enderecofilial = "";
            string cidadefilial = "";
            string telefonefilial = "";
            string cnpjfilial = "";
            string nomedestinatario = "";
            string nomeendereco = "";
            string nomebairro = "";
            string nomecidade = "";
            string nomeuf = "";
            string numcep = "";
            string nometransportadora = "";
            string connectionString = DBHelper.GetConnectionString(empresa);
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                if (conn.State == ConnectionState.Closed) {
                    conn.Open();
                }

                // adição de complemento
                // 
                string sql =
                $@"SELECT  
	                tb2.BPLName
	                , ISNULL((tb2.AddrType + ' ' + tb2.Street + ' ' + ', ' + tb2.StreetNo + ' - ' + tb2.Block),(select AddrType+' '+Street+' '+', '+StreetNo+' - '+Block from ADM1)) as 'EnderecoFilial'
	                , ISNULL((tb2.City + ', CEP: ' + tb2.ZipCode),(select City+', CEP: '+ZipCode from ADM1)) as 'CidadeFilial'
	                , (select Phone1 from OADM) as TelefoneFilial
                    , ISNULL(('CNPJ: ' + tb2.TaxIdNum),(select 'CNPJ: ' + TaxIdNum from ADM1)) as 'CnpjFilial'
	                , tb0.CardName as NomeDestinatario
	                , ISNULL(tb3.AddrType,isnull(tb1.AddrTypeS,'')) + ' ' + ISNULL(tb3.Street,isnull(tb1.StreetS,'')) + ', ' + ISNULL(tb3.StreetNo, isnull(tb1.StreetNoS,''))+ ', ' + ISNULL(CONVERT(VARCHAR(100),isnull(tb3.Building,'')), CONVERT(VARCHAR(100),isnull(tb1.BuildingS,''))) AS NomeEndereco
	                , ISNULL(tb3.Block, tb1.BlockS) as NomeBairro
	                , ISNULL(tb3.City, tb1.CityS) as NomeCidade
	                , ISNULL(tb3.State, tb1.StateS) as NomeUf
	                , ISNULL(tb3.ZipCode, tb1.ZipCodeS) NumCep
	                , tb4.CardFName as NomeTransportadora
                FROM ORDR tb0
                LEFT JOIN RDR12 tb1 ON (tb1.DocEntry = tb0.DocEntry)
                LEFT JOIN OBPL tb2 ON (tb2.BPLId = tb0.BPLId)
                LEFT JOIN CRD1 tb3 ON (tb3.Address = tb0.PickRmrk AND tb3.CardCode = tb0.CardCode AND tb3.AdresType = 'S')
                LEFT JOIN OCRD tb4 ON (tb4.CardCode = tb1.Carrier)
                WHERE tb0.DocEntry = {numdoc}";
                using (SqlCommand command = new SqlCommand("", conn)) {
                    command.CommandText = sql;
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            bplname = reader["BPLName"].ToString();
                            enderecofilial = reader["EnderecoFilial"].ToString();
                            cidadefilial = reader["CidadeFilial"].ToString();
                            telefonefilial = reader["TelefoneFilial"].ToString();
                            cnpjfilial = reader["CnpjFilial"].ToString();
                            nomedestinatario = reader["NomeDestinatario"].ToString();
                            nomeendereco = reader["NomeEndereco"].ToString();
                            nomebairro = reader["NomeBairro"].ToString();
                            nomecidade = reader["NomeCidade"].ToString();
                            nomeuf = reader["NomeUf"].ToString();
                            numcep = reader["NumCep"].ToString();
                            nometransportadora = reader["NomeTransportadora"].ToString();
                            nomedestinatario = CortaString(nomedestinatario);

                            var limiteCaracteres = 63;
                            if (nomeendereco.Length > limiteCaracteres) {
                                nomeendereco = nomeendereco.Substring(0, 63);
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            using (SqlConnection conn = new SqlConnection(connectionString)) {
                if (conn.State == ConnectionState.Closed) {
                    conn.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn)) {
                    string where_index = index > 0 ? $"AND U_indice = {index}" : "";
                    string sql_volume = $"SELECT U_volume FROM [@UPD_PCK_VOLUMES] WHERE 1 = 1 AND U_docentry = {numdoc} {where_index} ORDER BY U_indice";
                    command.CommandText = sql_volume;
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            string volume = reader["U_volume"].ToString();

                            sb.AppendLine();
                            sb.AppendLine("I8,A,001");
                            sb.AppendLine("Q1199, 024");
                            sb.AppendLine("q831");
                            sb.AppendLine("rN");
                            sb.AppendLine("S4");
                            sb.AppendLine("D7");
                            sb.AppendLine("ZT");
                            sb.AppendLine("JF");
                            sb.AppendLine("OD");
                            sb.AppendLine("R16, 0");
                            sb.AppendLine("f100");
                            sb.AppendLine("N");
                            sb.AppendLine(logo);
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A18,881,3,4,1,1,N,\"{0}\"", bplname));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A103,399,3,4,1,1,N,\"{0}\"", telefonefilial));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A144,589,3,4,1,1,N,\"{0}\"", cnpjfilial));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A103,881,3,4,1,1,N,\"{0}\"", cidadefilial));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A61,881,3,4,1,1,N,\"{0}\"", enderecofilial));

                            sb.AppendLine("LO742,1,2,1195");
                            sb.AppendLine("LO648,3,2,1195");
                            sb.AppendLine("LO594,2,2,1195");
                            sb.AppendLine("LO188,3,2,1195");
                            sb.AppendLine("LO3,926,187,2");

                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A610,877,3,4,1,1,N,\"{0}\"", nometransportadora));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A610,1170,3,4,1,1,N,\"{0}\"", "TRANSPORTADORA:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A671,410,3,4,2,2,N,\"{0}\"", volume));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A675,926,3,4,2,2,N,\"{0}\"", numdoc));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A675,1173,3,4,2,2,N,\"{0}\"", "PEDIDO:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A762,987,3,4,1,1,N,\"{0}\"", "URGENTE: PRODUTO HOSPITALAR PODE SALVAR UMA VIDA"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A442,1045,3,4,1,1,N,\"{0}\"", nomecidade));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A442,474,3,4,1,1,N,\"{0}\"", nomeuf));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A442,533,3,4,1,1,N,\"{0}\"", "UF:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A442,1178,3,4,1,1,N,\"{0}\"", "CIDADE:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A368,1047,3,4,1,1,N,\"{0}\"", nomebairro));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A368,1179,3,4,1,1,N,\"{0}\"", "BAIRRO:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A297,1013,3,4,1,1,N,\"{0}\"", nomeendereco));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A297,1179,3,4,1,1,N,\"{0}\"", "ENDEREÇO:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A224,942,3,4,1,1,N,\"{0}\"", nomedestinatario));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A518,1099,3,4,1,1,N,\"{0}\"", numcep));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A224,1179,3,4,1,1,N,\"{0}\"", "DESTINATÁRIO:"));
                            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "A518,1178,3,4,1,1,N,\"{0}\"", "CEP:"));
                            sb.AppendLine("LO794,4,4,1193");
                            sb.AppendLine("LO2,5,3,1192");
                            sb.AppendLine("LO1,1195,794,2");
                            sb.AppendLine("LO4,2,794,2");
                            sb.AppendLine("P1");
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(sb.ToString())) {
                RawPrinterHelper.SendStringToPrinter(printername, sb.ToString());
            }
        }

        private static string CortaString(string value) {
            if (value.Length > 50) {
                value = value.Substring(0, 50);
            }
            return value;
        }

        /// <summary>
        /// manda imprimir etiqueta
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="numdoc"></param>
        private void ImprimirEtiquetaComCrystal(Empresa empresa, int numdoc, string printername, int index = 0) {
            string ReportPath = GlobalHelper.RelatoriosPath + "/Volumes.rpt";

            if (System.IO.File.Exists(ReportPath)) {

                try {
                    var crReport = new CrystalDecisions.CrystalReports.Engine.ReportDocument();
                    crReport.Load(ReportPath);
                    crReport.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
                    crReport.PrintOptions.PaperOrientation = PaperOrientation.Portrait;

                    PrinterSettings printerSettings = new PrinterSettings();

                    printerSettings.PrinterName = printername;

                    crReport.SetParameterValue("DocKey@", numdoc);
                    crReport.SetParameterValue("DataBase", empresa.NomeBanco);

                    crReport.SetParameterValue("NVolume", index);

                    crReport.DataSourceConnections[0].SetConnection(empresa.InstanciaBanco, empresa.NomeBanco, empresa.UsuarioBanco, empresa.SenhaBanco);
                    crReport.DataSourceConnections[0].IntegratedSecurity = false;
                    crReport.DataSourceConnections[0].SetLogon(empresa.UsuarioBanco, empresa.SenhaBanco);

                    crReport.PrintToPrinter(printerSettings, new PageSettings(), false);

                    crReport.Close();

                } catch (Exception ex) {
                    throw ex;
                } finally {
                    GC.Collect();
                }
            }
        }

        public void DeletarVolumes(Empresa empresa, int numdoc) {
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = "DELETE FROM [@UPD_PCK_VOLUMES] WHERE U_docentry = " + numdoc;
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }
        }

        public void DeletarVolumesCabecalho(Empresa empresa, int numdoc) {
            string connectionString = DBHelper.GetConnectionString(empresa);
            try {
                // salva na tabela que contabiliza os minutos
                using (SqlConnection conn = new SqlConnection(connectionString)) {
                    if (conn.State == ConnectionState.Closed) {
                        conn.Open();
                    }

                    using (SqlCommand command = new SqlCommand("", conn)) {
                        command.CommandText = "DELETE FROM [@UPD_PCK_ETAPA] WHERE U_docentry = " + numdoc + " AND u_status = 'EP'";
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                throw e;
            }
        }

        private void RetornaPesoBalanca(string ip, int port) {
            try {
                SimpleTcpClient client = new SimpleTcpClient();
                client.StringEncoder = Encoding.UTF8;
                client.Connect(ip, port);
                client.Write("conexao_bem_sucedida");
                client.DataReceived += Client_DataReceived;
            } catch (Exception e) { }
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e) {
            peso_balanca = e.MessageString;
            ((SimpleTcpClient)sender).Disconnect();
        }

        #endregion
    }
}