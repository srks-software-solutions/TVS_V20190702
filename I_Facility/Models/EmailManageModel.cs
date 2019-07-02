using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class EmailManageModel
    {
        public tblmailid Email { get; set; }

        public IEnumerable<tblmailid> EmailList { get; set; }
    }
}