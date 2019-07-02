using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using I_Facility.ServerModel;
namespace I_Facility.Models
{
    public class MachineStatus
    {
        public int MachineID { get; set; }
        public int MachineOffTime { get; set; }
        public int OperatingTime { get; set; }
        public int IdleTime { get; set; }
        public int BreakdownTime { get; set; }
        public int Utilization { get; set; }
        public int TotalTime { get; set; }
        public int PrevUtilization { get; set; }
        public int PrevOperatingTime { get; set; }
        public int PrevTotalTime { get; set; }
        public string CellName { get; set; }
        public int CellUtilization { get; set; }

        public virtual tblmachinedetail machinedet { get; set; }
    }

    public class MachineStatusCell {
        public string CellName { get; set; }
        public int CellUtlization { get; set; }
    }
}