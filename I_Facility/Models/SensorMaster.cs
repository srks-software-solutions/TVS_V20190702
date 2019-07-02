using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class SensorMaster
    {
        public tblsensormaster sensormaster { get; set; }
        public IEnumerable<tblsensormaster> sensormasterList { get; set; }
    }
}