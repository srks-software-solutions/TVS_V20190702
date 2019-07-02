using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class SensorMachine
    {
        public int MachineID { get; set; }
        public int sid { get; set; }
        public string machineName { get; set; }
        public string SensorName { get; set; }

        public List<SensorMachine> sensorGroupsList { get; set; }
    }
    public class sensormachinemodel
    {
        public tblmachinesensor machinesensor { get; set; }
        public List<tblmachinesensor> machinesensorList { get; set; }
    }
}