//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace I_Facility.ServerModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class hist_parameters_master
    {
        public int ParameterID { get; set; }
        public string SetupTime { get; set; }
        public string OperatingTime { get; set; }
        public string PowerOnTime { get; set; }
        public Nullable<double> PartsCount { get; set; }
        public Nullable<System.DateTime> InsertedOn { get; set; }
        public Nullable<int> MachineID { get; set; }
        public Nullable<int> Shift { get; set; }
        public Nullable<System.DateTime> CorrectedDate { get; set; }
        public string AutoCutTime { get; set; }
        public string Total_CutTime { get; set; }
        public Nullable<int> PartsTotal { get; set; }
        public string CuttingTime { get; set; }
        public int AutoMode { get; set; }
    
        public virtual tblmachinedetail tblmachinedetail { get; set; }
    }
}