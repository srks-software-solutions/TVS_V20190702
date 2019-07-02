using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class SensorGroup
    {
        public int MachineID { get; set; }
        public int sid { get; set; }
        public string machineName { get; set; }
        public string SensorName { get; set; }
        public string sensorDesc { get; set; }

        public List<SensorGroup> sensorGroupsList { get; set; }
            
    }
    public class sensormodel
    {
        public tblsensorgroup sensorgroup { get; set; }
        public List<tblsensorgroup> sensorList { get; set; }
    }
}