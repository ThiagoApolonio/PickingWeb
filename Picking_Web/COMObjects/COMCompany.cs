using Picking_Web.Helpers;
using Picking_Web.Models;
using SAPbobsCOM;
using System;
using System.Collections.Generic;

namespace Picking_Web.COMObjects
{
    public class COMCompany : IDisposable
    {
        private Company _company;
        public Company Company { get { return _company; } }
        public static DateTime CompanyFirsTime { get; set; }
        public static List<Companie> companies = new List<Companie>();

        public COMCompany(Empresa empresa)
        {
            _company = SAPHelper.CriarCompany(empresa);
        }

        public COMCompany(Empresa empresa, bool com) {
            SAPHelper.CriarCompanies(empresa);
        }

        public void Dispose()
        {
            //if (_company.Connected)
            //{
            //    _company.Disconnect();
            //}
            //SAPHelper.ReleaseObjectFromMemory(_company);
        }
    }
}