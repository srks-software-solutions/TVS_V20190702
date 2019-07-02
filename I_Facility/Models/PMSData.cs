using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace I_Facility.ServerModel
{
    public class PMSData
    {
     
        public int MachineID { get; set; }
     
        public string MachineName { get; set; }

        
        
        public List<PmsDetails> pmsdetailsList { get; set; }
       
    }
    public class PmsDetails
    {
        public int pmid { get; set; }
        public int month { get; set; }
        public int week { get; set; }
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public int Year { get; set; }
        public SelectList selectMonth { get; set; }

        public SelectList selectweek { get; set; }
    }

    
}