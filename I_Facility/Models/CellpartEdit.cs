using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class CellpartEdit
    {
        public tblcell cell { get; set; }
        public List<tblcell> cellslist { get; set; }
        public tblcellpart cellpart { get; set; }
        public List<tblcellpart> CellpartList { get; set; }
        
    }
    public class cellpartDetails
    {
        public int CellID { get; set; }
        public int CpID { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public int Isdeleted { get; set; }
        public int NoofModel { get; set; }
        public int PartNo { get; set; }
        public string CellName { get; set; }
        public string CellDesc { get; set; }
        public string CellDisplayname { get; set; }
        public string partDesc { get; set; }
        public int PlantID { get; set; }
        public int ShopID { get; set; }
    }
}