using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    //public class graph
    //{
    //    public string balloonText { get; set; }
    //    public int fillAlphas { get; set; }
    //    public string id { get; set; }
    //    public string title { get; set; }
    //    public string type { get; set; }
    //    public string valueField { get; set; }
    //}


    public class Health
    {
        public string Time { get; set; }
        public int value { get; set; }
    }

    public class MachineHealth {
        public decimal? LSL { get; set; }
        public decimal? USL { get; set; }
        public List<Health> MachineHealthdet{get;set;}
    }
}