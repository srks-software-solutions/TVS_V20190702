using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class ModulesModel
    {
        public tblmodule Modules { get; set; }

        public IEnumerable<tblmodule> ModuleList { get; set; }
    }
}