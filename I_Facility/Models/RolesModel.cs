using I_Facility.Models;
using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class RolesModel
    {
        public tblrole Role { get; set; }

        public IEnumerable<tblrole> RoleList { get; set; }
    }
}