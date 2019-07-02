using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class AlarmHMI
    {
        public string AxisName { set; get; }
        public string AlarmNo { set; get; }
        public string AlarmDesc { set; get; }
        public string OccurredOn { set; get; }

        public IEnumerable<AlarmHMI> AlarmList { get; set; }
    }
}