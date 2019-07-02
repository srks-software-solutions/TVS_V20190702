using I_Facility.Models;
using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class RoleModuleModel
    {
        public tblrolemodulelink RoleModule { get; set; }

        public List<tblrolemodulelink> RoleModuleList { get; set; }
    }
}