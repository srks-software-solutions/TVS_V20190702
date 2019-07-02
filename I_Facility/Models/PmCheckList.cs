using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class PmCheckList
    {
        public tblpmchecklist pmchecklist { get; set; }
        public IEnumerable<tblpmchecklist> pmchecklistlist { get; set; }
    }
}