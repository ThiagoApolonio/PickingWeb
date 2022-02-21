using Picking_Web.COMObjects;
using Picking_Web.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Picking_Web.Helpers {
    public static class SAPHelper {
        public static List<UsuarioSAP> GetUsuariosSap(ApplicationDbContext _context, int empresa_id) {
            List<UsuarioSAP> users = new List<UsuarioSAP>();
            try {
                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                using (SqlConnection connection = new SqlConnection(DBHelper.GetConnectionString(empresa))) {
                    if (connection.State == ConnectionState.Closed) {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand("", connection)) {
                        command.CommandText = "SELECT USERID, U_NAME FROM OUSR WHERE GROUPS <> 99 ORDER BY U_NAME ASC";
                        //command.Parameters.AddWithValue("@CardCode", "BF0000000000004");
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read()) {
                            int userid = Convert.ToInt32(reader["USERID"]);
                            string name = reader["U_NAME"].ToString();

                            users.Add(new UsuarioSAP() {
                                Id = userid,
                                Nome = name
                            });
                        }
                    }
                }
            } catch (Exception e) {
            }
            return users;
        }
        public static List<DepositosSap> GetDepositoSap(ApplicationDbContext _context, int empresa_id)
        {
            List<DepositosSap> deposito = new List<DepositosSap>();
            try
            {
                Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
                using (SqlConnection connection = new SqlConnection(DBHelper.GetConnectionString(empresa)))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        command.CommandText = "select WhsCode,  WhsName from OWHS where Inactive = 'N'";
                        //command.Parameters.AddWithValue("@CardCode", "BF0000000000004");
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string DepId = reader["WhsCode"].ToString();
                            string DepDesc = reader["WhsName"].ToString();

                            deposito.Add(new DepositosSap()
                            {
                                Id = DepId,
                                Descricao = DepDesc
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return deposito;
        }

        public static List<ItemSAP> GetItensSap(ApplicationDbContext _context, int empresa_id) {
            Empresa empresa = _context.Empresa.Single(i => i.Id == empresa_id);
            List<ItemSAP> itens = new List<ItemSAP>() { };
            try {
                using (SqlConnection connection = new SqlConnection(DBHelper.GetConnectionString(empresa))) {
                    if (connection.State == ConnectionState.Closed) {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand("", connection)) {
                        command.CommandText = "SELECT ItemCode, ItemName FROM OITM WHERE PrchseItem = 'Y' AND InvntItem = 'Y' AND frozenFor = 'N' AND OnHand > 0 ORDER BY ItemCode";
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read()) {
                            itens.Add(new ItemSAP() {
                                ItemCode = reader["ItemCode"].ToString(),
                                ItemName = reader["ItemName"].ToString(),
                            });
                        }
                    }
                }
            } catch (Exception e) {
            }
            return itens;
        }

        public static SAPbobsCOM.Company CriarCompany(string server, int serverType, string companydb, string username, string password, BoSuppLangs lang = BoSuppLangs.ln_Portuguese_Br) {
            SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
           

            BoDataServerTypes oServerType = (BoDataServerTypes)serverType;
            oCompany.DbServerType =(BoDataServerTypes)oServerType;
            oCompany.LicenseServer =(string)null;
            oCompany.Server =(string) server;
            oCompany.CompanyDB = companydb;
            oCompany.UserName = username;
            oCompany.Password = password;
            
            //oCompany.language = lang;

            var err = oCompany.Connect();
            if (err != 0) {
               
                    //Conexão Falhou
                    oCompany.GetLastError(out err,out string errorMessage);//Utilizando o metodo GetLastError Da CompanyClass Conseguimos Capturar o erro exato gerado ao tentar se conectar com A DI API sap
            
                return new Company();
            } else {
                return oCompany;
            }
        }

        public static void CriarCompanies(Empresa empresa) {
            Companie companie = null;
            SAPbobsCOM.Company comp = null;
            if (COMCompany.companies.Count > 0) {
                foreach (var item in COMCompany.companies)
                {
                    var teste = item.company.CompanyDB; 
                }
           
                companie = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
                if (companie != null)
                    comp = companie.company;
            }

            if (comp != null) {
                if (comp.Connected) {
                    string companyTime = comp.GetCompanyTime();
                    var time = DateTime.ParseExact(companyTime, "HH:mm", CultureInfo.InvariantCulture);
                    var ret = time - companie.CompanyFirsTime;
                    if (ret.TotalMinutes > 30) {
                        COMCompany.companies.Remove(companie);
                        comp.Disconnect();
                        comp = null;
                        comp = CriarCompany(empresa.InstanciaBanco, Convert.ToInt32(empresa.TipoBanco), empresa.NomeBanco, empresa.UsuarioSap, empresa.SenhaSap);
                        COMCompany.companies.Add(new Companie() { CompanyFirsTime = DateTime.Now, company = comp });
                    }
                } else {
                    COMCompany.companies.Remove(companie);
                    comp = null;
                    comp = CriarCompany(empresa.InstanciaBanco, Convert.ToInt32(empresa.TipoBanco), empresa.NomeBanco, empresa.UsuarioSap, empresa.SenhaSap);
                    COMCompany.companies.Add(new Companie() { CompanyFirsTime = DateTime.Now, company = comp });
                }
            } else {
                comp = CriarCompany(empresa.InstanciaBanco, Convert.ToInt32(empresa.TipoBanco), empresa.NomeBanco, empresa.UsuarioSap, empresa.SenhaSap);
                COMCompany.companies.Add(new Companie() { CompanyFirsTime = DateTime.Now, company = comp });

            }
        }

        public static SAPbobsCOM.Company CriarCompany(string server, string serverType, string companydb, string username,
            string password, BoSuppLangs lang = BoSuppLangs.ln_Portuguese_Br) {
            return CriarCompany(server, Convert.ToInt32(serverType), companydb, username, password, lang);
        }

        public static SAPbobsCOM.Company CriarCompany(Empresa empresa, BoSuppLangs lang = BoSuppLangs.ln_Portuguese_Br) {
            return CriarCompany(empresa.InstanciaBanco, empresa.TipoBanco, empresa.NomeBanco, empresa.UsuarioSap, empresa.SenhaSap);
        }

        public static SAPbobsCOM.Company GetCompany(Empresa empresa) {
            return CriarCompany(empresa.InstanciaBanco, empresa.TipoBanco, empresa.NomeBanco, empresa.UsuarioSap, empresa.SenhaSap);
        }

        #region Helpers Picking Parcial

        public static int GetOrderRowItem(Empresa empresa, string item, int docEntry, int empresa_id) {
            int row = 0;
            try {
                string connectionString = DBHelper.GetConnectionString(empresa);
                using (SqlConnection conn2 = new SqlConnection(connectionString)) {
                    if (conn2.State == ConnectionState.Closed) {
                        conn2.Open();
                    }
                    using (SqlCommand command = new SqlCommand("", conn2)) {
                        string sql = string.Format(
                            $@"select top 1 LineNum 
                        from RDR1 T0
                        inner join ORDR T1 on T0.DocEntry = T1.DocEntry
                        where T1.DocEntry = {docEntry} 
                        and T0.ItemCode = '{item}'");
                        command.CommandText = sql;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read()) {
                            row = int.Parse(reader["LineNum"].ToString());
                        }
                    }
                }
            } catch (Exception ex) {
            }
            return row;
        }

        public static void CancelarListaDePicking(Empresa empresa, int numPicking) {
            try {

                string connectionString = DBHelper.GetConnectionString(empresa);

                new COMCompany(empresa, true);
                var com = COMCompany.companies.Where(x => x.company.CompanyDB == empresa.NomeBanco).FirstOrDefault();
                Company oCompany = com.company;
                using (var pickListCOM = new COMPickList(oCompany)) {
                    var oPickLists = pickListCOM._pickList;
                    if (oPickLists.GetByKey(numPicking)) {
                        if (oPickLists.Close() != 0) {

                        } else {

                        }
                    }
                }


            } catch (Exception e) {

            }
        }

        #endregion

        public static void ReleaseObjectFromMemory(object o) {
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(o);
            if (o != null) {
                o = null;
            }
            GC.Collect();
        }
    }
}