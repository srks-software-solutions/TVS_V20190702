using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
   
    public class CellsModel
    {
        public tblcell Cells { get; set; }
        public IEnumerable<tblcell> cellslist { get; set; }
        public tblcellpart cellpart { get; set; }
        public List<tblcellpart> CellpartList { get; set; }
    }
   
}