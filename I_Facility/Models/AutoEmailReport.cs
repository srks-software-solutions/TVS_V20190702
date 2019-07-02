using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class AutoEmailReport
    {
        public tbl_autoreportsetting Autoemailreport { get; set; }
        public IEnumerable<tbl_autoreportsetting> Autoreportemaillist { get; set; }
    }
}