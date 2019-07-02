using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class PartsManagement
    {

        public tblpart MasterParts { get; set; }
        public IEnumerable<tblpart> MasterPartsList { get; set; }
    }
}