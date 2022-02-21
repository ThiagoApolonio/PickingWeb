using Picking_Web.Helpers;
using SAPbobsCOM;
using System;

namespace Picking_Web.COMObjects
{
    public class COMPickList : IDisposable
    {
        
        public COMPickList(Company company)
        {
            _pickList = (PickLists)company.GetBusinessObject(BoObjectTypes.oPickLists);
        }

        public PickLists _pickList;
        public void Dispose()
        {
            if (_pickList != null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_pickList);

            _pickList = null;
        }
    }
}