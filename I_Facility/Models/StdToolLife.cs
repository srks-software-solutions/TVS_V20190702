using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    public class StdToolLife
    {
        public tblstdtoollife tblStdToolLife { get; set; }
        public IEnumerable<tblstdtoollife> tblStdToolLifeList { get; set; }
    }
}