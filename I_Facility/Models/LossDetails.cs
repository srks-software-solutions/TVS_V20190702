using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_Facility.Models
{
   public class LossDetails
    {
        public int CellID { get; set; }
        public int? LossID { get; set; }
        public string LossCodeDescription { get; set; }
        public DateTime LossStartTime { get; set; }
        public DateTime LossEndTime { get; set; }
        public double DurationinMin { get; set; }
    }
}
