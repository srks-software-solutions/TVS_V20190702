using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.Models
{
    public class operatorgetqty
    {
        public int PartNumber { get; set; }
        public int ShiftID { get; set; }
        public string[] Operatorname { get; set; }
    }
}