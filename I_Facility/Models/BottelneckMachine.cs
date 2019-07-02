using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class BottelneckMachine
    {
        public tblbottelneck BottelNeck { get; set; }
        public IEnumerable<tblbottelneck> BottelNeckList { get; set; }
    }
}