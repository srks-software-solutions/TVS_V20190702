using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    public class GentellaModel
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string PowerOnTime { get; set; }
        public string RunningTime { get; set; }
        public string CuttingTime { get; set; }
        public string CycleTime { get; set; }
        public string CurrentStatus { get; set; }
        public string ExeProgramName { get; set; }
        public int? PartsCount { get; set; }
        public string Color { get; set; }
        public string IdleTime { get; set; }
        public string Utilization { get; set; }
        public string MinorLossesTime { get; set; }

        public List<MachineUTF> MachineUTFs { get; set; }

        public List<DataProviderAxisBySpindleLoad> Spindleloads { get; set; }
    }
    public class MachineUTF
    {
        public string MachineName { get; set; }
        public int RunningTime { get; set; }
        public int IdleTime { get; set; }
        public int PowerOffTime { get; set; }
        public int BreakDownTime { get; set; }
        //public string colorIdleTime { get; set; }
        //public string colorRunningTime { get; set; }
        //public string colorBreakDownTime { get; set; }
        //public string colorPowerOffTime { get; set; }



    }



    public class graph
    {
        public string balloonText { get; set; }
        public int fillAlphas { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string valueField { get; set; }
    }

    public class DataProviderAxisBySpindleLoad
    {
        public string Time { get; set; }
        public Decimal? value { get; set; }


    }
}