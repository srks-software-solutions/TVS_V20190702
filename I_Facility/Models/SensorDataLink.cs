using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class SensorDataLink
    {
        public tblsensordatalink sensordatalink { get; set; }
        public IEnumerable<tblsensordatalink> sensordataList { get; set; }
    }
}