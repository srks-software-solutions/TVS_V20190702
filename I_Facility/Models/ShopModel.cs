using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class ShopModel
    {

        public tblshop Shops { get; set; }
        public IEnumerable<tblshop> Shopslist {get;set;}
    }
}