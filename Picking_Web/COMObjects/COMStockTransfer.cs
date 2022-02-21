using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.COMObjects
{
    public class COMStockTransfer: IDisposable
    {

        public SAPbobsCOM.StockTransfer Transfer { get; set; }

        public COMStockTransfer(SAPbobsCOM.Company company)
        {
            Transfer = (SAPbobsCOM.StockTransfer)company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(Transfer);
            Transfer = null;
            GC.Collect();
        }
    }
}