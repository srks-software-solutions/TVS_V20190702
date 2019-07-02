using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    public class MachineHierarchy
    {
        public int PlantID
        {
            get;
            set;
        }
        public int ShopID
        {
            get;
            set;
        }
        public int CellID
        {
            get;
            set;
        }
        public int WorkCenterID
        {
            get;
            set;
        }
    }
}