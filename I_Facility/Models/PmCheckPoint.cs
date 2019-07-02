using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class PmCheckPoint
    {
        public tblpmcheckpoint pmCheckPoint { get; set; }
        public IEnumerable<tblpmcheckpoint> pmCheckPointlist { get; set; }
    }
}