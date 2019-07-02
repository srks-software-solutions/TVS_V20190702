using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class MultiWorkOrderModel
    {
        public tblmultipleworkorder Multiworkorder { get; set; }
        public IEnumerable<tblmultipleworkorder> MultiOwrkOrderList { get; set; }
    }
}