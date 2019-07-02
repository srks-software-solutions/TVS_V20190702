using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class MachineModel
    {
        public tblmachinedetail Machine { get; set; }
        public IEnumerable<tblmachinedetail> MachineList { get; set; }
    }
}