﻿using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace I_Facility.ServerModel
{
    public class ProductionLossModel
    {
        public tbllossescode ProductionLoss { get; set; }
        public IEnumerable<tbllossescode> ProductionLossList { get; set; }
    }
}