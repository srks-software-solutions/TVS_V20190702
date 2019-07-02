using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class PlantModel
    {
        public tblplant Plant { get; set; }

        public IEnumerable<tblplant> PlantList { get; set; }
    }
}