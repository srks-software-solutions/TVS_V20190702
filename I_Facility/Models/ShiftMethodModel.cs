using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class ShiftMethodModel
    {
        public tblshiftmethod ShiftMethod { get; set; }

        public IEnumerable<tblshiftmethod> ShiftMethodList { get; set; }

    }
}