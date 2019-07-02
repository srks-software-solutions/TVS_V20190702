using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class cellpartdetailsmodel
    {
        public tblcell cell { get; set; }
        public IEnumerable<tblcell> celllist { get; set; }
        public tblcellpart cellpart { get; set; }
        public List<tblcellpart> cellpartliat { get; set; }
    }
}