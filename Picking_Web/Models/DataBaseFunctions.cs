using Picking_Web.Contexto;
using Picking_Web.Helpers;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Picking_Web.Models
{
    
    public class DataBaseFunctions
    {
 
        public DataBaseFunctions(SAPbobsCOM.Company oCompany)
        {
            this.oCompany = oCompany;
        }


        #region :: DECLARAÇÕES

        // public SAPbouiCOM.Application SBO_Application;
        public SAPbobsCOM.Company oCompany;

        private SAPbobsCOM.Recordset sboRecordSet;
        private string strQuery;

        private String MsgErroDB;
        private int CodErroDB;

        public SAPbobsCOM.UserFieldsMD objUserFieldsMD;
        public SAPbobsCOM.UserTablesMD objUserTablesMD;
        public SAPbobsCOM.UserObjectsMD objUserObjectMD;
        private SAPbobsCOM.CompanyService compsrv { get; set; }
        private SAPbobsCOM.GeneralService service { get; set; }
        private SAPbobsCOM.GeneralData data { get; set; }
        private SAPbobsCOM.GeneralDataParams dataparams { get; set; }

        #endregion


        #region :: Enumeração de Componentes

        public enum TipoCampo
        {
            tALPHA = 0,
            tMEMO = 1,
            tNUMBER = 2,
            tDATE = 3,
            tBOOL = 4,
            tPrice = 5,
            tPercent = 6,
            tSum = 7,
            tTime = 8
        }

        public enum TipoTabela
        {
            tUSER = 0,
            tSBO = 1
        }

        public enum TipoObjeto
        {
            tNOOBJECT = 0,
            tMASTERDATA = 1,
            tMASTERLINE = 2,
            tDOCUMENT = 3,
            tDOCUMENT_Line = 4
        }

        #endregion


        #region :: Documentação

        /// <summary>
        /// Funções universais para manipulação de campos. Com este recurso otimizamos tempo na criação de propriedades de usuários e do sistema SAP Businss one
        /// Todos os campos, criados, removidos ou atualizados utilizarão as funções listadas abaixo.
        /// </summary>
        /// <param name ="cTable">Tabela de usuário </param>
        /// <param name ="cDescription">Descrição de tabela ou campo de usuário</param>
        /// <param name ="cTipo">Tipo do Objeto do sistema (SAP ou Usuário)</param>
        /// <param name ="TipoTabela">Tipo de tabela do sistema (SAP ou Usuário)</param>
        /// <returns></returns>

        #endregion


        #region :: Criação de Funções

        //Verifica a existencia de tabela de usuário 
        public bool fExiteTabela(string cTable)
        {
            sboRecordSet = (Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = "Select Count(*) from OUTB where TableName = '" + cTable + "'";
            sboRecordSet.DoQuery(strQuery);
            DataBaseFunctions zero;
            

            if ((int)sboRecordSet.Fields.Item(0).Value <= 0)
            {
                sboRecordSet = null;
                strQuery = null;
                return false;
            }
            else
            {
                strQuery = null;
                return true;
            }
        }

        public bool fExiteTabelaUDO_OPRD(string cTable0, string cTable1, string cTable2, string cTable3, string cTable4, string cTable5, string cTable6)
        {
            sboRecordSet = (Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = "Select Count(*) from OUTB where TableName in('" + cTable0 + "','" + cTable1 + "','" + cTable2 + "','" + cTable3 + "','" + cTable4 + "','" + cTable5 + "','" + cTable6 + "')";
            sboRecordSet.DoQuery(strQuery);

            if ((int)sboRecordSet.Fields.Item(0).Value <= 0)
            {
                sboRecordSet = null;
                strQuery = null;
                return false;
            }
            else
            {
                strQuery = null;
                return true;
            }
        }


        //Verifica se UDO Existe
        public bool fExiteUDO(string cTable)
        {
            sboRecordSet = (Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = "Select Count(*) from OUDO where TableName = '" + cTable + "'";
            sboRecordSet.DoQuery(strQuery);

            if ((int)sboRecordSet.Fields.Item(0).Value <= 0)
            {
                sboRecordSet = null;
                strQuery = null;
                return false;
            }
            else
            {
                strQuery = null;
                return true;
            }
        }

        //Excluir tabela de usuário
        public bool fExcluirTabela(string cTable)
        {
            GC.Collect();
            //SBO_Application.StatusBar.SetText("Removendo tabela [" + cTable + "]", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            SAPbobsCOM.UserTablesMD objUserTablesMD;
            objUserTablesMD = (UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
            objUserTablesMD.TableName = cTable;
            objUserTablesMD.GetByKey(cTable);

            CodErroDB = objUserTablesMD.Remove();
            if (CodErroDB != 0)
            {
                oCompany.GetLastError(out CodErroDB, out string MsgErroDB);
                //SBO_Application.MessageBox("Erro ao remover tabela [" + cTable + "]. Motivo: " + MsgErroDB + "....");
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserTablesMD);
                objUserTablesMD = null;
                return false;
            }
            else
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserTablesMD);
                objUserTablesMD = null;
                return true;
            }

        }

        //Criar tabela de usuário

        private void AddUserTable(string Name, string Description, SAPbobsCOM.BoUTBTableType Type)
        {
            ////****************************************//**********************************
            // The UserTablesMD represents a meta-data object which allows us
            // to add\remove tables, change a table name etc.
            ////*********************//*********************//**********************************
            GC.Collect();
            SAPbobsCOM.UserTablesMD oUserTablesMD = null;
            ////*********************//*********************//**********************************
            // In any meta-data operation there should be no other object "alive"
            // but the meta-data object, otherwise the operation will fail.
            // This restriction is intended to prevent a collisions
            ////*********************//*********************//**********************************
            // the meta-data object needs to be initialized with a
            // regular UserTables object
            oUserTablesMD = ((SAPbobsCOM.UserTablesMD)(oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables)));
            ////*********************//*****************************
            // when adding user tables or fields to the SBO DB
            // use a prefix identifying your partner name space
            // this will prevent collisions between different
            // partners add-ons
            // SAP's name space prefix is "BE_"
            ////*********************//*****************************		
            // set the table parameters
            oUserTablesMD.TableName = Name;
            oUserTablesMD.TableDescription = Description;
            oUserTablesMD.TableType = Type;
            // Add the table
            // This action add an empty table with 2 default fields
            // 'Code' and 'Name' which serve as the key
            // in order to add your own User Fields
            // see the AddUserFields.frm in this project
            // a privat, user defined, key may be added
            // see AddPrivateKey.frm in this project
            CodErroDB = oUserTablesMD.Add();
            // check for errors in the process
            if (CodErroDB != 0)
            {
                if (CodErroDB == -1)
                {
                }
                else
                {
                    oCompany.GetLastError(out CodErroDB, out MsgErroDB);
                    // SBO_Application.StatusBar.SetText("Erro ao criar Tabela [" + Name + "] --> [" + Description + "] - " + MsgErroDB + "!", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
            else
            {
                // SBO_Application.StatusBar.SetText("Tabela [" + Name + "] --> [" + Description + "] - Criada com Sucesso!", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            oUserTablesMD = null;
            GC.Collect(); // Release the handle to the table
        }

        public object zero()
        {
            return 0;
        }
        //Verificar existencia de campos de usuário
        public bool fExiteCampo(string cTable, string cField)
        {

            sboRecordSet = (Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = string.Format("Select Count(*) From CUFD (NOLOCK) Where TableID='{0}' and AliasID='{1}'", cTable, cField);
            sboRecordSet.DoQuery(strQuery);
            sboRecordSet.MoveFirst();
        

            if ((int)sboRecordSet.Fields.Item(0).Value <= 0)
            {
                sboRecordSet = null;
                return false;
            }
            else
            {
                sboRecordSet = null;
                return true;
            }

        }

        //Verificar se campo existe dentro de tabela
        public int fFieldId(string cTabela, string cCampo)
        {
            sboRecordSet =(Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = " SELECT FieldID FROM CUFD (NOLOCK) ";
            strQuery += " Where TableID='" + cTabela + "' and ";
            strQuery += " AliasID='" + cCampo + "'";

            sboRecordSet.DoQuery(strQuery);
            sboRecordSet.MoveFirst();

            if ((int)sboRecordSet.Fields.Item(0).Value >= 0)
            {
                return (int)sboRecordSet.Fields.Item(0).Value;
            }
            else
            {
                sboRecordSet = null;
                return -1;
            }
        }

        // Verifica se exite valor valido atribuido a campo de usuario
        public bool fExisteValorValido(string cTabela, int cCampoID, string Valor)
        {
            sboRecordSet = (Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = "Select Count(*) From UFD1 (NOLOCK) ";
            strQuery += "Where TableID='" + cTabela + "' and ";
            strQuery += "      FieldID='" + cCampoID.ToString() + "' and";
            strQuery += "      FldValue='" + Valor + "'";
            sboRecordSet.DoQuery(strQuery);
            sboRecordSet.MoveFirst();

            if ((int)sboRecordSet.Fields.Item(0).Value <= 0)
            {
                sboRecordSet = null;
                return false;
            }
            else
            {
                sboRecordSet = null;
                return true;
            }



        }

        //Criação de campos de usuários em tabela de usuário ou em tabela do Sistema SAP
        public void fCriaCampo(string cTabela, string cCampo, string cDescricao, TipoCampo cTipo, short nsize, TipoTabela cTipoTabela)
        {
            string cTabelaAux;

            cTabelaAux = "";
            //TipoTabela: 0=user, 1=SAPB1
            if (cTipoTabela == 0)
            {
                cTabelaAux = "@" + cTabela;
            }
            else if (cTipoTabela == TipoTabela.tSBO)
            {
                cTabelaAux = cTabela;
            }

            if (!fExiteCampo(cTabelaAux, cCampo))
            {

                GC.Collect();
                objUserFieldsMD = (UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                objUserFieldsMD.TableName = cTabela;
                objUserFieldsMD.Name = cCampo;
                objUserFieldsMD.Description = cDescricao;

                if (cTipo == TipoCampo.tPrice)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Float;
                    objUserFieldsMD.SubType = SAPbobsCOM.BoFldSubTypes.st_Price;
                }
                else if (cTipo == TipoCampo.tPercent)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Float;
                    objUserFieldsMD.SubType = SAPbobsCOM.BoFldSubTypes.st_Percentage;
                }
                else if (cTipo == TipoCampo.tSum)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Float;
                    objUserFieldsMD.SubType = SAPbobsCOM.BoFldSubTypes.st_Sum;
                }
                else if (cTipo == TipoCampo.tALPHA)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Alpha;
                }
                else if (cTipo == TipoCampo.tDATE)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Date;
                }
                else if (cTipo == TipoCampo.tTime)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Date;
                    objUserFieldsMD.SubType = SAPbobsCOM.BoFldSubTypes.st_Time;
                }
                else if (cTipo == TipoCampo.tNUMBER)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Numeric;
                }
                else if (cTipo == TipoCampo.tMEMO)
                {
                    objUserFieldsMD.Type = SAPbobsCOM.BoFieldTypes.db_Memo;
                }
                objUserFieldsMD.EditSize = nsize;
                // Adding the Field to the Table
                CodErroDB = objUserFieldsMD.Add();

                // Check for errors
                if (CodErroDB != 0)
                {
                    oCompany.GetLastError(out int CodErroDB, out string MsgErroDB);
                    // SBO_Application.MessageBox("Erro na Criação do Campo: " + cCampo + " " + CodErroDB.ToString() + " - " + MsgErroDB);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldsMD);
                    objUserFieldsMD = null;

                }
                else
                {
                    // SBO_Application.StatusBar.SetText("Campo: [" + cCampo + "] da tabela [" + cTabela + "] criado com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldsMD);
                    objUserFieldsMD = null;

                }

            }
            else
            {

            }

        }

        //Remoção de campos de usuários em tabelas de usuário ou em tabela do Sistema SAP
        public bool fExcluiCampo(string cTabela, string cCampo, TipoTabela cTipoTabela)
        {
            string cTabelaAux = null;
            //TipoTabela: 0=user, 1=SAPB1
            if (cTipoTabela == TipoTabela.tUSER)
            {
                cTabelaAux = "@" + cTabela;
            }
            else if (cTipoTabela == TipoTabela.tSBO)
            {
                cTabelaAux = cTabela;
            }
            try
            {
                int FieldId = fFieldId(cTabelaAux, cCampo);
                SAPbobsCOM.UserFieldsMD oUserFieldsMD;

                GC.Collect();
                oUserFieldsMD =(UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                if (oUserFieldsMD.GetByKey(cTabelaAux, FieldId) == false)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldsMD);
                    return false;
                }
                else
                {
                    //removendo campo de tabela
                    CodErroDB = oUserFieldsMD.Remove();
                    if (CodErroDB != 0)
                    {
                        oCompany.GetLastError(out int CodErroDB, out string MsgErroDB);
                        // SBO_Application.StatusBar.SetText("Campo: [" + cCampo + "] da tabela [" + cTabela + "] com erro em sua remoção! Codigo: " + CodErroDB.ToString() + " - Mensagem: " + MsgErroDB, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldsMD);
                        return false;
                    }
                    else
                    {
                        // SBO_Application.StatusBar.SetText("Campo: [" + cCampo + "] da tabela [" + cTabela + "] excluído com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldsMD);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                // SBO_Application.MessageBox("Erro de sistema: " + ex.Message);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldsMD);
                return false;
            }

        }

        // Seta valor com defalut em campo de usuário
        public bool fExisteValorDefaultSet(string cTabela, string cCampoID, string Valor)
        {
            sboRecordSet = (Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            strQuery = "Select Count(*) From CUFD (NOLOCK) ";
            strQuery += "Where TableID='" + cTabela + "' and ";
            strQuery += "      FieldID='" + cCampoID + "' and";
            strQuery += "      dflt='" + Valor + "'";
            sboRecordSet.DoQuery(strQuery);
            sboRecordSet.MoveFirst();

            if ((int)(sboRecordSet.Fields.Item(0).Value) <= 0)
            {
                sboRecordSet = null;
                return false;
            }
            else
            {
                sboRecordSet = null;
                return true;
            }
        }

        //Adiciona valor valido a campo de usuário
        public void fAdicionaValorValido(string cTabela, string cCampo, string Valor, string Descricao, TipoTabela cTipoTabela)
        {
            //TipoTabela: 0=user, 1=SAPB1
            string cTabelaAux = null;
            if (cTipoTabela == TipoTabela.tUSER)
            {
                cTabelaAux = "@" + cTabela;
            }
            else
            {
                cTabelaAux = cTabela;
            }
            bool valorExiste = false;
            int campoID = fFieldId(cTabelaAux, cCampo);

            if (fExisteValorValido(cTabelaAux, campoID, Valor))
            {
                valorExiste = true;

            }
            else
            {
                GC.Collect();
                SAPbobsCOM.UserFieldsMD objUserFieldsMD;
                objUserFieldsMD = (UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                objUserFieldsMD.GetByKey(cTabelaAux, campoID);
                SAPbobsCOM.ValidValuesMD oValidValues;
                oValidValues = objUserFieldsMD.ValidValues;
                if (!valorExiste)
                {
                    if (oValidValues.Value != "")
                    {
                        oValidValues.Add();
                        oValidValues.SetCurrentLine(oValidValues.Count - 1);
                        oValidValues.Value = Valor;
                        oValidValues.Description = Descricao;
                        CodErroDB = objUserFieldsMD.Update();
                    }
                    else
                    {
                        oValidValues.SetCurrentLine(oValidValues.Count - 1);
                        oValidValues.Value = Valor;
                        oValidValues.Description = Descricao;
                        CodErroDB = objUserFieldsMD.Update();
                    }

                    if (CodErroDB != 0)
                    {
                        oCompany.GetLastError(out int CodErroDB, out string MsgErroDB);
                        // SBO_Application.StatusBar.SetText("Valor válido para o campo: [" + cCampo + "] da tabela [" + cTabela + "] reportado com erro! " + CodErroDB + " - " + MsgErroDB, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                    }
                    else
                    {
                        // SBO_Application.StatusBar.SetText("Valor válido para o campo: [" + cCampo + "] da tabela [" + cTabela + "] criado com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                    }
                }
                else
                {
                    CodErroDB = objUserFieldsMD.Update();
                    if (CodErroDB != 0)
                    {
                        oCompany.GetLastError(out int CodErroDB, out string MsgErroDB);
                        // SBO_Application.StatusBar.SetText("Valor válido para o campo: [" + cCampo + "] da tabela [" + cTabela + "] reportado com erro! " + CodErroDB.ToString() + " - " + MsgErroDB, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                    }
                    else
                    {
                        // SBO_Application.StatusBar.SetText("Valor válido para o campo: [" + cCampo + "] da tabela [" + cTabela + "] atualziado com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                    }
                }
            }



        }

        //Aplica valor padrão a campo de usuário
        public bool fSetaValorPadrao(string cTabela, string cCampo, string Valor, TipoTabela cTipoTabela)
        {
            string cTabelaAux = null;
            //TipoTabela: 0=user, 1=SAPB1

            if (cTipoTabela == TipoTabela.tUSER)
            {
                cTabelaAux = "@" + cTabela;
            }
            else
            {
                cTabelaAux = cTabela;
            }

            bool valorExiste = false;
            int campoID = fFieldId(cTabelaAux, cCampo);

            if (fExisteValorValido(cTabelaAux, campoID, Valor))
            {
                valorExiste = true;
            }

            //se existe esse valor válido
            if (valorExiste && (fExisteValorDefaultSet(cTabelaAux, campoID.ToString(), Valor)) == false)
            {
                GC.Collect();
                SAPbobsCOM.UserFieldsMD objUserFieldsMD;
                objUserFieldsMD = (UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                if (objUserFieldsMD.GetByKey(cTabelaAux, campoID))
                    objUserFieldsMD.DefaultValue = Valor;
                CodErroDB = objUserFieldsMD.Update();
                if (CodErroDB != 0)
                {
                    objUserFieldsMD = null;
                    oCompany.GetLastError(out int CodErroDB, out string MsgErroDB);
                    // SBO_Application.StatusBar.SetText("Valor padrão para o campo: [" + cCampo + "] da tabela [" + cTabela + "] reportado com erro! " + CodErroDB.ToString() + " - " + MsgErroDB, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }
                else
                {
                    objUserFieldsMD = null;
                    // SBO_Application.StatusBar.SetText("Valor padrão para o campo: [" + cCampo + "] da tabela [" + cTabela + "] setado com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    return true;
                }

            }
            else
            {
                return false;
            }



        }

        //Aplica campo como obrigatorio
        public bool fSetaCampoObrigatorio(string cTabela, string cCampo, TipoTabela cTipoTabela)
        {
            string cTabelaAux = null;
            //'TipoTabela: 0=user, 1=SAPB1

            if (cTipoTabela == TipoTabela.tUSER)
            {
                cTabelaAux = "@" + cTabela;
            }
            else
            {
                cTabelaAux = cTabela;
            }

            int campoID = fFieldId(cTabelaAux, cCampo);
            SAPbobsCOM.UserFieldsMD objUserFieldsMD;
            GC.Collect();
            objUserFieldsMD = (UserFieldsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
            if (objUserFieldsMD.GetByKey(cTabelaAux, campoID))
            {
                objUserFieldsMD.Mandatory = SAPbobsCOM.BoYesNoEnum.tYES;
                CodErroDB = objUserFieldsMD.Update();
            }

            if (CodErroDB != 0)
            {
                oCompany.GetLastError(out int CodErroDB, out string MsgErroDB);
                // SBO_Application.StatusBar.SetText("Obrigatoriedade para o campo: [" + cCampo + "] da tabela [" + cTabela + "] reportado com sucesso! " + CodErroDB.ToString() + " - " + MsgErroDB, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
            else
            {
                // SBO_Application.StatusBar.SetText("Obrigatoriedade para o campo: [" + cCampo + "] da tabela [" + cTabela + "] setada com sucesso!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                return true;
            }
        }


        //registra tabela 
        public void Register_UDO_Config()
        {
            try
            {
                objUserObjectMD = (UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                objUserTablesMD = (UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserTablesMD);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserObjectMD);
                GC.Collect();

                //=================================================================================================

                objUserObjectMD =(UserObjectsMD)oCompany.GetBusinessObject(BoObjectTypes.oUserObjectsMD);
                objUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.Code = "UPD_PKL";
                objUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.Name = "UPD_PKL";
                objUserObjectMD.ObjectType = SAPbobsCOM.BoUDOObjType.boud_Document;
                objUserObjectMD.TableName = "UPD_PKL4";

                #region Tabela Cabeçalho UPD_PKL4

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "DocEntry";
                objUserObjectMD.FormColumns.FormColumnDescription = "DocEntry";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.FindColumns.ColumnAlias = "DocEntry";
                objUserObjectMD.FormColumns.Add();
                ////*********************



                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_PkEntry";
                objUserObjectMD.FormColumns.FormColumnDescription = "Lista Picking";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_PkEntry";
                objUserObjectMD.FormColumns.Add();
                ////*********************
                


                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_OrderEntry";
                objUserObjectMD.FormColumns.FormColumnDescription = "Pedido Base";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_OrderEntry";
                objUserObjectMD.FormColumns.Add();
                ////*********************



               

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_ItemCode";
                objUserObjectMD.FormColumns.FormColumnDescription = "Item";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_ItemCode";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_Dscription";
                objUserObjectMD.FormColumns.FormColumnDescription = "Descricao";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_Dscription";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_QtdPedido";
                objUserObjectMD.FormColumns.FormColumnDescription = "Qtd Solicitada";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_QtdPedido";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_Deposito";
                objUserObjectMD.FormColumns.FormColumnDescription = "Depósito";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_Deposito";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_NumLote";
                objUserObjectMD.FormColumns.FormColumnDescription = "Número Lote";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_NumLote";
                objUserObjectMD.FormColumns.Add();
                ////********************* 

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_OrderLine";
                objUserObjectMD.FormColumns.FormColumnDescription = "Número Linha";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_OrderLine";
                objUserObjectMD.FormColumns.Add();
                ////********************* 
                
                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_QtdLote";
                objUserObjectMD.FormColumns.FormColumnDescription = "Qtd Alocada(Lote)";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_QtdLote";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_QtdAlocada";
                objUserObjectMD.FormColumns.FormColumnDescription = "Qtd Alocada(Picking)";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_QtdAlocada";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                objUserObjectMD.FormColumns.FormColumnAlias = "U_QtdPicking";
                objUserObjectMD.FormColumns.FormColumnDescription = "Qtd Efetuda (Picking)";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_QtdPicking";
                objUserObjectMD.FormColumns.Add();



                int enhancedFormColumnNumber = 1;
                int enhancedFormChildNumber = 1;

                #endregion



                CodErroDB = objUserObjectMD.Add();

                if (CodErroDB != 0)
                {
                    oCompany.GetLastError(out CodErroDB, out MsgErroDB);
                    //Program._application.StatusBar.SetText("Erro " + MsgErroDB + " em Criação de Objeto " + objUserObjectMD.Name,
                    // BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);

                }
                else
                {
                    // Program._application.StatusBar.SetText("Objeto " + objUserObjectMD.Name + " criado com sucesso!",
                    //BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
                }


                objUserObjectMD = null;
                GC.Collect();
            }
            catch (Exception e)
            {

            }

        }
        public void Register_UDO_Lote()
        {
            try
            {
                objUserObjectMD = (UserObjectsMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                objUserTablesMD = (UserTablesMD)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserTablesMD);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserObjectMD);
                GC.Collect();

                //=================================================================================================

                objUserObjectMD = (UserObjectsMD)oCompany.GetBusinessObject(BoObjectTypes.oUserObjectsMD);
                objUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.Code = "UPD_Lote";
                objUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.Name = "UPD_Lote";
                objUserObjectMD.ObjectType = SAPbobsCOM.BoUDOObjType.boud_Document;
                objUserObjectMD.TableName = "UPD_Lote";

                #region Tabela Cabeçalho UPD_Lote

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "DocEntry";
                objUserObjectMD.FormColumns.FormColumnDescription = "DocEntry";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tNO;
                objUserObjectMD.FindColumns.ColumnAlias = "DocEntry";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_ItemCode";
                objUserObjectMD.FormColumns.FormColumnDescription = "Codigo Item";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_ItemCode";
                objUserObjectMD.FormColumns.Add();
                ////********************* 

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_NumLote";
                objUserObjectMD.FormColumns.FormColumnDescription = "Número Lote";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_NumLote";
                objUserObjectMD.FormColumns.Add();
                ////********************* 

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_DigControle";
                objUserObjectMD.FormColumns.FormColumnDescription = "Digito de Controle)";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_DigControle";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_Status";
                objUserObjectMD.FormColumns.FormColumnDescription = "Disponibilidade Lote";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_Status";
                objUserObjectMD.FormColumns.Add();
                ////*********************

                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_CodigoProduto";
                objUserObjectMD.FormColumns.FormColumnDescription = "Codigo do Produto";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_CodigoProduto";
                objUserObjectMD.FormColumns.Add();
                ////*********************
                ///
                ////*********************
                objUserObjectMD.FormColumns.FormColumnAlias = "U_PkEntry";
                objUserObjectMD.FormColumns.FormColumnDescription = "Lista Picking";
                objUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                objUserObjectMD.FindColumns.ColumnAlias = "U_PkEntry";
                objUserObjectMD.FormColumns.Add();
                ////*********************


                int enhancedFormColumnNumber = 1;
                int enhancedFormChildNumber = 1;

                #endregion



                CodErroDB = objUserObjectMD.Add();

                if (CodErroDB != 0)
                {
                    oCompany.GetLastError(out CodErroDB, out MsgErroDB);
                    //Program._application.StatusBar.SetText("Erro " + MsgErroDB + " em Criação de Objeto " + objUserObjectMD.Name,
                    // BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Error);

                }
                else
                {
                    // Program._application.StatusBar.SetText("Objeto " + objUserObjectMD.Name + " criado com sucesso!",
                    //BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Success);
                }


                objUserObjectMD = null;
                GC.Collect();
            }
            catch (Exception e)
            {

            }

        }


        #endregion


        #region :: Manipulações de Campos

        public void ManipulaCamposPicking()
        {

            #region :: Variaveis Default

            string oTable = null;
            string oField = null;
            string oDescription = null;
            string oFieldValue1 = null;
            string oFieldValue2 = null;
            string oFieldValue3 = null;
            string oFieldValue4 = null;
            string oFieldValue5 = null;
            string oFieldValue6 = null;
            string oFieldValue7 = null;
            string oFieldValue8 = null;
            string oFieldValue9 = null;

            string oFieldDesc1 = null;
            string oFieldDesc2 = null;
            string oFieldDesc3 = null;
            string oFieldDesc4 = null;
            string oFieldDesc5 = null;
            string oFieldDesc6 = null;
            string oFieldDesc7 = null;
            string oFieldDesc8 = null;
            string oFieldDesc9 = null;

            short nsize = 0;

            #endregion

            #region :: Configuração de Lista Picking - CAMPO: PRIORIDADE

            oTable = "OPKL";
            oField = ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"].Replace("U_", "");
            oDescription = "UPD: Prioridade";

            oFieldValue1 = "S";
            oFieldValue2 = "N";

            oFieldDesc1 = "Sim";
            oFieldDesc2 = "Não";
            nsize = 1;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue1, oFieldDesc1, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue2, oFieldDesc2, TipoTabela.tSBO);
                fSetaValorPadrao(oTable, oField, oFieldValue2, TipoTabela.tSBO);
            }

            #endregion

            #region :: Configuração de Lista Picking - CAMPO: STATUS PICKING
            oTable = "OPKL";
            oField = ConfigurationManager.AppSettings["CAMPO_STATUS_PICKING"].Replace("U_", "");
            oDescription = "UPD: Situação Picking";

            oFieldValue1 = "AS";
            oFieldDesc1 = "Aguardando Separação";

            oFieldValue2 = "ES";
            oFieldDesc2 = "Em Separação";

            oFieldValue3 = "AC";
            oFieldDesc3 = "Aguardando Conferência";

            oFieldValue4 = "EC";
            oFieldDesc4 = "Em Conferência";

            oFieldValue5 = "AP";
            oFieldDesc5 = "Aguardando Pesos e Rótulos";

            oFieldValue6 = "EP";
            oFieldDesc6 = "Em Pesos e Rótulos";

            oFieldValue7 = "PE";
            oFieldDesc7 = "Picking Efetuado";

            oFieldValue8 = "SP";
            oFieldDesc8 = "Separação Pendente";

            nsize = 3;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue1, oFieldDesc1, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue2, oFieldDesc2, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue3, oFieldDesc3, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue4, oFieldDesc4, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue5, oFieldDesc5, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue6, oFieldDesc6, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue7, oFieldDesc7, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue8, oFieldDesc8, TipoTabela.tSBO);
                fSetaValorPadrao(oTable, oField, oFieldValue2, TipoTabela.tSBO);
            }

            #endregion

            #region :: Configuração de Lista Picking - CAMPO: EMBALAGEM

            oTable = "OPKL";
            oField = "UPD_TRANSP";
            oDescription = "UPD: Transportadora";

            nsize = 20;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
            }

            oTable = "OPKL";
            oField = "UPD_QEMBAL";
            oDescription = "UPD: Qtd Embalagens";

            nsize = 9;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tSBO);
            }

            oTable = "OPKL";
            oField = "UPD_PBRUTO";
            oDescription = "UPD: Peso Bruto(Kg)";

            nsize = 30;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tSum, nsize, TipoTabela.tSBO);
            }
            #endregion

            #region :: Configuração de Pedido de Venda - CAMPO: PRIORIDADE

            oTable = "ORDR";
            oField = ConfigurationManager.AppSettings["CAMPO_PRIORIDADE"].Replace("U_", "");
            oDescription = "UPD: Prioridade";

            oFieldValue1 = "S";
            oFieldValue2 = "N";

            oFieldDesc1 = "Sim";
            oFieldDesc2 = "Não";
            nsize = 1;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue1, oFieldDesc1, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue2, oFieldDesc2, TipoTabela.tSBO);
                fSetaValorPadrao(oTable, oField, oFieldValue2, TipoTabela.tSBO);
            }

            #endregion

            #region :: Configuração de Pedido de Venda - CAMPO: STATUS PICKING

            oField = ConfigurationManager.AppSettings["CAMPO_STATUS_PICKING"].Replace("U_", "");
            oDescription = "UPD: Situação Picking";

            oFieldValue1 = "AS";
            oFieldDesc1 = "Aguardando Separação";

            oFieldValue2 = "ES";
            oFieldDesc2 = "Em Separação";

            oFieldValue9 = "SC";
            oFieldDesc9 = "Separação Concluída";

            oFieldValue3 = "AC";
            oFieldDesc3 = "Aguardando Conferência";

            oFieldValue4 = "EC";
            oFieldDesc4 = "Em Conferência";

            oFieldValue5 = "AP";
            oFieldDesc5 = "Aguardando Pesos e Rótulos";

            oFieldValue6 = "EP";
            oFieldDesc6 = "Em Pesos e Rótulos";

            oFieldValue7 = "PE";
            oFieldDesc7 = "Picking Efetuado";

            oFieldValue8 = "SP";
            oFieldDesc8 = "Separação Pendente";

            nsize = 3;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue1, oFieldDesc1, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue2, oFieldDesc2, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue9, oFieldDesc9, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue3, oFieldDesc3, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue4, oFieldDesc4, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue5, oFieldDesc5, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue6, oFieldDesc6, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue7, oFieldDesc7, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue8, oFieldDesc8, TipoTabela.tSBO);
                fSetaValorPadrao(oTable, oField, oFieldValue1, TipoTabela.tSBO);
            }

            #endregion


            #region :: Configuração de Grupo de Fornecedor - CAMPO: É transportador

            oTable = "OCRG";
            oField = "U_UPD_PCK_TRANSP";
            oDescription = "Transportador?";

            oFieldValue1 = "S";
            oFieldValue2 = "N";

            oFieldDesc1 = "Sim";
            oFieldDesc2 = "Não";
            nsize = 1;

            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue1, oFieldDesc1, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, oFieldValue2, oFieldDesc2, TipoTabela.tSBO);
                fSetaValorPadrao(oTable, oField, oFieldValue2, TipoTabela.tSBO);
            }

            #endregion


            #region :: Criação da Tabela UPD_PCK_ETAPA

            // tabela que armazena o andamento de cada etapa do processo do picking
            oTable = "UPD_PCK_ETAPA";
            oDescription = "UPD: Etapa Picking Pedido";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);


                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "status";
                oDescription = "Status";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "date";
                oDescription = "Data";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tDATE, nsize, TipoTabela.tUSER);
            }



            #endregion


            #region :: Criação da Tabela UPD_PCK_CANCEL

            // tabela que armazena só os pickings que foram cancelados
            oTable = "UPD_PCK_CANCEL";
            oDescription = "UPD: Picking Pedido Cancelado";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);

                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "date";
                oDescription = "Data";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tDATE, nsize, TipoTabela.tUSER);
            }



            #endregion


            #region :: Criação da Tabela UPD_PCK_LOCAL

            // tabela que armazena os locais físicos do andamento do picking
            oTable = "UPD_PCK_LOCAL";
            oDescription = "UPD: Local Fisico";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);


                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "status";
                oDescription = "Status";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "local";
                oDescription = "Local Físico";
                nsize = 253;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tMEMO, nsize, TipoTabela.tUSER);
            }



            #endregion


            #region :: Criação da Tabela UPD_PCK_CONF_PARCIAL

            // tabela que armazena os operadores no andamento do picking
            oTable = "UPD_PCK_CONFPARCIAL";
            oDescription = "UPD: Conf Parcial";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);


                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "itemcode";
                oDescription = "ItemCode";
                nsize = 30;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "numserie";
                oDescription = "Numero Serie";
                nsize = 50;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "codbarras";
                oDescription = "Código Barras";
                nsize = 100;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "codfabric";
                oDescription = "Código Fabricante";
                nsize = 100;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "numlote";
                oDescription = "Numero Lote";
                nsize = 50;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "qtd";
                oDescription = "Quantidade";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tPrice, nsize, TipoTabela.tUSER);

                oField = "qtdTotal";
                oDescription = "QtdTotal";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tPrice, nsize, TipoTabela.tUSER);
            }



            #endregion


            #region :: Criação da Tabela UPD_PCK_CONF_LOG

            // tabela que armazena os operadores no andamento do picking
            oTable = "UPD_PCK_CONFLOG";
            oDescription = "UPD: Conf Log";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);

                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "operador";
                oDescription = "Operador";
                nsize = 50;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "itemcode";
                oDescription = "ItemCode";
                nsize = 30;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "numLote";
                oDescription = "Numero Lote";
                nsize = 50;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "data";
                oDescription = "Data Hora";
                nsize = 30;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tDATE, nsize, TipoTabela.tUSER);

                oField = "msg";
                oDescription = "Mensagem";
                nsize = 30;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tMEMO, nsize, TipoTabela.tUSER);

            }


            #endregion


            #region :: Criação da Tabela UPD_PCK_VOLUMES

            // tabela que armazena os volumes
            oTable = "UPD_PCK_VOLUMES";
            oDescription = "UPD: Operadores";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);


                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "volume";
                oDescription = "Volume";
                nsize = 100;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "peso";
                oDescription = "Peso";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tPrice, nsize, TipoTabela.tUSER);

                oField = "indice";
                oDescription = "Indice";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);
            }



            #endregion


            #region :: Criação da Tabela UPD_PCK_OPERADOR

            // tabela que armazena os operadores no andamento do picking
            oTable = "UPD_PCK_OPERADOR";
            oDescription = "UPD: Operadores";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_NoObject);


                oField = "docentry";
                oDescription = "Pedido de Venda";
                nsize = 9;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "status";
                oDescription = "Status";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "operador";
                oDescription = "Operador";
                nsize = 50;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);
            }



            #endregion

            #region :: Criação de campos na Tabela de Itens

            oTable = "OITM";
            oField = "UPD_PCK_ARMAZ";
            oDescription = "Ambiente Impressão";
            nsize = 10;
            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
            }

            #endregion

            #region :: Criação de campos na Tabela de Itens

            oTable = "OCRD";
            oField = "UPD_PCK_TRSP";
            oDescription = "Listar em Transportadoras";
            nsize = 1;
            if (!fExiteCampo(oTable, oField))
            {
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, "S", "Sim", TipoTabela.tSBO);
                fAdicionaValorValido(oTable, oField, "N", "Nao", TipoTabela.tSBO);
                fSetaValorPadrao(oTable, oField, "N", TipoTabela.tSBO);

            }

            #endregion



            #region :: Criação da Tabela UPD_PKL4

            // tabela que armazena os Dados da Etapa de Separação

            oTable = "UPD_PKL4";
            oDescription = "UPD: Etapa Separação Picking";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_Document);


                oField = "PkEntry";
                oDescription = "Lista Picking";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "OrderEntry";
                oDescription = "Pedido Base";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);

                oField = "ItemCode";
                oDescription = "Item";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "Dscription";
                oDescription = "Descricao";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "QtdPedido";
                oDescription = "Qtd Solicitada";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "Deposito";
                oDescription = "Depósito";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "NumLote";
                oDescription = "Número Lote";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "OrderLine";
                oDescription = "Número linha";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "QtdLote";
                oDescription = "Qtd Alocada(Lote)";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tSum, nsize, TipoTabela.tUSER);

                oField = "QtdAlocada";
                oDescription = "Qtd Alocada(Picking)";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tSum, nsize, TipoTabela.tUSER);

                oField = "QtdPicking";
                oDescription = "Qtd Efetuda (Picking)";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tSum, nsize, TipoTabela.tUSER);



            }

            #region Regitrando UDO PKL4

            oTable = "UPD_PKL4";
            if (!fExiteUDO(oTable))
            {
                Register_UDO_Config();
            }

            #endregion



            #endregion

            #region :: Criação da Tabela UPD_Lote

            // tabela que armazena os Dados da Etapa de Separação

            oTable = "UPD_LOTE";
            oDescription = "UPD: Controle de Lote";
            if (!fExiteTabela(oTable))
            {
                AddUserTable(oTable, oDescription, BoUTBTableType.bott_Document);

                oField = "ItemCode";
                oDescription = "Codigo Item";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "NumLote";
                oDescription = "Número Lote";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "DigControle";
                oDescription = "Digito de Controle";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tSum, nsize, TipoTabela.tUSER);

                oField = "CodigoProduto";
                oDescription = "Codigo do Produto";
                nsize = 254;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "Status";
                oDescription = "Disponibilidade Lote";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tALPHA, nsize, TipoTabela.tUSER);

                oField = "PkEntry";
                oDescription = "Lista Picking";
                nsize = 10;
                fCriaCampo(oTable, oField, oDescription, TipoCampo.tNUMBER, nsize, TipoTabela.tUSER);



            }

            #region Regitrando UDO Lote

            oTable = "UPD_LOTE";
            if (!fExiteUDO(oTable))
            {
                Register_UDO_Lote();
            }

            #endregion



            #endregion
        }

        #endregion

        #region :: Adiciona Inclusão Tabela PKL4

        public bool AdcPKL4(List<ItensPedido> picking, string connectionString)
        {
            try
            {
                compsrv = oCompany.GetCompanyService();
                service = compsrv.GetGeneralService("UPD_PKL");
                data = (SAPbobsCOM.GeneralData)service.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                foreach (var item in picking)
                {
                    if (item.check)
                    {
                        var AbsEntryPK = oCompany.GetNewObjectKey();
                        data.SetProperty("U_PkEntry", oCompany.GetNewObjectKey());

                        var OrderEntry = GetOrderBase(connectionString, AbsEntryPK);
                        data.SetProperty("U_OrderEntry", OrderEntry);
                        data.SetProperty("U_ItemCode", item.item);
                        data.SetProperty("U_QtdPedido", item.qtdSolicitada);
                        data.SetProperty("U_Deposito", item.deposito);
                        data.SetProperty("U_NumLote", item.numLote);
                        data.SetProperty("U_OrderLine", item.LineNum);
                        data.SetProperty("U_QtdLote", item.qtdAlocada);
                        var QtdAlocadaPk = Convert.ToDouble(item.qtdAlocadaPk) + Convert.ToDouble(item.qtdPicking);
                        data.SetProperty("U_QtdAlocada", QtdAlocadaPk);
                        data.SetProperty("U_QtdPicking", item.qtdPicking);
                        var ad = service.Add(data);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public bool AdcPKL4PkCompleto(List<ItensPedido> picking, string connectionString)
        {
            try
            {
                compsrv = oCompany.GetCompanyService();
                service = compsrv.GetGeneralService("UPD_PKL");
                data = (SAPbobsCOM.GeneralData)service.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                foreach (var item in picking)
                {
                    var qtdAlocadaLote = Convert.ToDouble(item.qtdAlocada);
                    var qtdAlocadaPK = Convert.ToDouble(item.qtdAlocadaPk);
                    var qtdSolicitada = qtdAlocadaLote - qtdAlocadaPK;
                    if ((qtdSolicitada) > 0)
                    {
                        var AbsEntryPK = oCompany.GetNewObjectKey();
                        data.SetProperty("U_PkEntry", oCompany.GetNewObjectKey());

                        var OrderEntry = GetOrderBase(connectionString, AbsEntryPK);
                        data.SetProperty("U_OrderEntry", OrderEntry);

                        data.SetProperty("U_ItemCode", item.item);
                        data.SetProperty("U_QtdPedido", item.qtdSolicitada);
                        data.SetProperty("U_Deposito", item.deposito);
                        data.SetProperty("U_NumLote", item.numLote);
                        data.SetProperty("U_OrderLine", item.LineNum);
                        data.SetProperty("U_QtdLote", item.qtdAlocada);
                        data.SetProperty("U_QtdAlocada", item.qtdAlocada);
                        data.SetProperty("U_QtdPicking", qtdSolicitada);
                        var ad = service.Add(data);
                    }
                }
                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);

            }

        }

        public string GetOrderBase(string connectionString, string AbsEntryPK)
        {
            var OrderEntry = "";
            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@" select OrderEntry from pkl1 
                                                  where AbsEntry = '{0}'
                                                  group by OrderEntry", AbsEntryPK);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        OrderEntry = reader["OrderEntry"].ToString();


                    }
                    return OrderEntry;
                }
            }
        }
        #endregion

        #region :: Atualiza status lista picking PKL4

        public void AtualizaStatus(int NumPicking, string connectionString)
        {
            compsrv = oCompany.GetCompanyService();
            service = compsrv.GetGeneralService("UPD_PKL");
            data = (SAPbobsCOM.GeneralData)service.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                if (conn2.State == ConnectionState.Closed)
                {
                    conn2.Open();
                }
                using (SqlCommand command = new SqlCommand("", conn2))
                {
                    string sql = string.Format(@" Select DocEntry from [@UPD_PKL4] where U_PkEntry = '{0}'", NumPicking);

                    command.CommandText = sql;
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var DocEntry = reader["DocEntry"];
                        dataparams = (SAPbobsCOM.GeneralDataParams)service.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                        dataparams.SetProperty("DocEntry", DocEntry);
                        service.Close(dataparams);

                    }
                }
            }





        }
        #endregion





        #region :: Adiciona Inclusão Tabela UPD_LOTE

        public string AdcControleLote(ItensRecebimento ItemRecebido)
        {
            try
            {
                compsrv = oCompany.GetCompanyService();
                service = compsrv.GetGeneralService("UPD_Lote");
                data = (SAPbobsCOM.GeneralData)service.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);

                data.SetProperty("U_ItemCode", ItemRecebido.ItemCode);
                data.SetProperty("U_NumLote", ItemRecebido.NumLote);
                data.SetProperty("U_DigControle", ItemRecebido.Digito);
                data.SetProperty("U_Status", "Y");
                data.SetProperty("U_CodigoProduto", ItemRecebido.CodigoItem);

                var ad = service.Add(data);
                string docentry = ad.GetProperty("DocEntry").ToString();
                return docentry;

            }
            catch (Exception e)
            {
                throw new Exception("Erro ao Adicionar Chave sequencia de lote!" +e.Message);
            }

        }
        #endregion
    }
}