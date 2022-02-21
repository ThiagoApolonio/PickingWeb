using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Picking_Web.COMObjects {
    public class Companie {
        public DateTime CompanyFirsTime { get; set; }
        public SAPbobsCOM.Company company;
    }
}