using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    public class OperatorDashboard
    {
        public tbloperatordashboard OpDashboard { get; set; }

        public IEnumerable<tbloperatordashboard> OpDashboardList { get; set; }

        public string machinename { get; set; }
    }
}