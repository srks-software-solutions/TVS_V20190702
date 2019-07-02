using I_Facility.Models;
using I_Facility.ServerModel;

namespace MasterPartsList
{
    internal class find : tblmasterparts_st_sw
    {
        private int hdnpid;

        public find(int hdnpid)
        {
            this.hdnpid = hdnpid;
        }
    }
}