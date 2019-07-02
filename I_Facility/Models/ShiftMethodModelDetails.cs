using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    public class ShiftMethodModelDetails
    {
        public int ShiftMethodID { get; set; }
        public string ShiftMethodName { get; set; }
        public string ShiftMethodDesc { get; set; }
        public int NoOfShifts { get; set; }
        public string ShiftDetailsName { get; set; }
        public string ShiftDetailsDesc { get; set; }
        public TimeSpan? ShiftStartTime { get; set; }
        public TimeSpan? ShiftEndTime { get; set; }
        public int? NextDay { get; set; }
        public int? IsGShift { get; set; }
    }
}