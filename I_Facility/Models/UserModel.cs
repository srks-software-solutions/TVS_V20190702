using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class UserModel
    {
        public tbluser Users { get; set; }

        public IEnumerable<tbluser> UsersList { get; set; }
    }
}