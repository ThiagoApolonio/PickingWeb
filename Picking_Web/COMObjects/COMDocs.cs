using Picking_Web.Helpers;
using SAPbobsCOM;
using System;

namespace Picking_Web.COMObjects
{
    public class COMDocs : IDisposable
    {
        private Documents _documents;
        public Documents Documents { get { return _documents; } }

        public COMDocs(Company company, BoObjectTypes type)
        {
            _documents =(Documents)company.GetBusinessObject(type);
        }

        public void Dispose()
        {
            SAPHelper.ReleaseObjectFromMemory(_documents);
            _documents = null;
        }
    }
}