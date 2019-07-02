using System;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using I_Facility.ServerModel;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace I_Facility.ReportsCalcClass
{
    public class UtilizationReport
    {
        i_facilityEntities1 Serverdb = new i_facilityEntities1();

        public UtilizationReport()
        {

        }

        public void CalculateUtilization(int PlantID, int ShopID, int CellID, int MachineID, DateTime FromDate, DateTime Enddate)
        {
            var getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

            if (MachineID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
            }
            else if (CellID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            }
            else if (ShopID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
            }

            int dateDifference = Enddate.Subtract(FromDate).Days;
            MsqlConnection mc = new MsqlConnection();
            for (int i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = FromDate.AddDays(i);

                foreach (var Machine in getMachineList)
                {
                    var GetUtilList = Serverdb.tbl_utilreport.Where(m => m.MachineID == Machine.MachineID && m.CorrectedDate == QueryDate.Date).ToList();
                    if (GetUtilList.Count == 0)
                    {
                        string correctedDate = QueryDate.ToString("yyyy-MM-dd");
                        var machineslist = new List<tblmachinedetail>();
                        var bottleneckmachines = new tblbottelneck();
                        var scrap = new tblworkorderentry();
                        var scrapqty1 = new List<tblrejectqty>();
                        var cellpartDet = new tblcellpart();
                        var partsDet = new tblpart();
                        using (i_facilityEntities1 db = new i_facilityEntities1())
                        {
                            scrap = db.tblworkorderentries.Where(m => m.CellID == CellID && m.CorrectedDate == correctedDate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
                            if (scrap != null)
                            {
                                partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == scrap.PartNo).FirstOrDefault();
                                if (partsDet != null)
                                    bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == partsDet.FGCode && m.CellID == scrap.CellID).FirstOrDefault();
                            }
                            else
                            {
                                cellpartDet = db.tblcellparts.Where(m => m.CellID == CellID && m.IsDefault == 1 && m.IsDeleted == 0).FirstOrDefault();
                                if (cellpartDet != null)
                                    bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == cellpartDet.partNo && m.CellID == cellpartDet.CellID).FirstOrDefault();
                                string Operationnum = Machine.OperationNumber.ToString();
                                partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

                            }


                        }
                        int bottelneckmachineid = bottleneckmachines.MachineID;
                        calculateUtilization(QueryDate.Date, Machine.MachineID, (decimal)partsDet.StdLoadingTime, (decimal)partsDet.StdUnLoadingTime, bottelneckmachineid);
                        //using (MySqlCommand cmd = new MySqlCommand("i_facility.SP_UtilData", mc.sqlConnection))
                        //{
                        //    cmd.CommandType = CommandType.StoredProcedure;
                        //    cmd.Parameters.AddWithValue("@Cdate", QueryDate.Date);
                        //    cmd.Parameters.AddWithValue("@MachineID", Machine.MachineID);
                        //    mc.open();
                        //    cmd.ExecuteNonQuery();
                        //    mc.close();
                        //}
                        //Serverdb.Database.
                        //Serverdb.SP_UtilData(QueryDate.Date, Machine.MachineID);
                    }
                }
            }

        }

        private void calculateUtilization(DateTime correctedDate, int machineID, decimal LoadingTime, decimal UnloadingTime, int bottleneckmachineid)
        {
            var modedataList = new List<tblmode>();
            var plannedbrks = new List<tblplannedbreak>();
            decimal TotalTime = 24, OperatingTime = 0, SetupTime = 0, SetupMinorTime = 0, MinorLossTime = 0, LossTime = 0, BDTime = 0, PowerOffTime = 0, UtilPercent = 0, TotalTimeDiff = 0;
            double plannedBrkDurationinHrs = 0;
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                modedataList = Serverdb.tblmodes.Where(m => m.CorrectedDate == correctedDate.Date && m.MachineID == machineID && m.IsCompleted == 1).ToList();
            }
            OperatingTime = Convert.ToDecimal(modedataList.Where(m => m.ModeType == "PROD").ToList().Sum(m => m.DurationInSec));
            PowerOffTime = Convert.ToDecimal(modedataList.Where(m => m.ModeType == "POWEROFF").ToList().Sum(m => m.DurationInSec));
            BDTime = Convert.ToDecimal(modedataList.Where(m => m.ModeType == "MNT").ToList().Sum(m => m.DurationInSec));
            MinorLossTime = Convert.ToDecimal(modedataList.Where(m => m.ModeType == "IDLE" && m.DurationInSec < 600).ToList().Sum(m => m.DurationInSec));
            LossTime = Convert.ToDecimal(modedataList.Where(m => m.ModeType == "IDLE" && m.DurationInSec > 600).ToList().Sum(m => m.DurationInSec));
            OperatingTime = Math.Round((OperatingTime / 3600), 2);
            PowerOffTime = Math.Round((PowerOffTime / 3600), 2);
            BDTime = Math.Round((BDTime / 3600), 2);
            MinorLossTime = Math.Round((MinorLossTime / 3600), 2);
            LossTime = Math.Round((LossTime / 3600), 2);
            double diff = 0;
            var modedataforsetup = modedataList.Where(m => m.CorrectedDate == correctedDate.Date && m.MachineID == machineID && m.IsCompleted == 1 && m.ModeTypeEnd == 1 && m.ModeType == "SETUP").ToList();
            foreach (var row in modedataforsetup)
            {
                DateTime LossEndTime = Convert.ToDateTime(row.LossCodeEnteredTime);
                DateTime StartTime = Convert.ToDateTime(row.StartTime);
                diff += LossEndTime.Subtract(StartTime).TotalSeconds;
            }
            SetupTime = Math.Round((decimal)(diff / 3600), 2);
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                plannedbrks = Serverdb.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
            }
            foreach (var row in plannedbrks)
            {
                plannedBrkDurationinHrs += Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalHours;
            }
            int PartsCount = GetPartscount(machineID, correctedDate);
            if (LoadingTime == 0)
                LoadingTime = 6;
            if (UnloadingTime == 0)
                UnloadingTime = 6;
            decimal LoadtimewithProd = Math.Round((decimal)((LoadingTime + UnloadingTime) / 3600) * PartsCount, 2);
            decimal opwithLoadUnload = OperatingTime + LoadtimewithProd;
            decimal Availability = Math.Round((TotalTime - (decimal)plannedBrkDurationinHrs), 2);
            UtilPercent = Math.Round((opwithLoadUnload / Availability) * 100, 2);
            BDTime = Convert.ToDecimal(plannedBrkDurationinHrs);
            TotalTime = OperatingTime + PowerOffTime + BDTime + MinorLossTime + LossTime + SetupTime;
            if (TotalTime > 24)
            {
                TotalTimeDiff = TotalTime - 24;
                TotalTime = 24;
                PowerOffTime -= TotalTimeDiff;
                if (PowerOffTime < 0)
                    PowerOffTime = 0;
            }
            TotalTime = 24;
            MinorLossTime = MinorLossTime - LoadtimewithProd;
            tbl_utilreport rep = new tbl_utilreport();
            rep.TotalTime = TotalTime;
            rep.SetupTime = SetupTime;
            rep.OperatingTime = opwithLoadUnload;
            rep.PowerOffTime = PowerOffTime;
            rep.LossTime = LossTime;
            rep.MachineID = machineID;
            rep.CorrectedDate = correctedDate.Date;
            rep.MinorLossTime = MinorLossTime;
            rep.SetupMinorTime = SetupMinorTime;
            rep.BDTime = BDTime;
            rep.UtilPercent = UtilPercent;
            rep.InsertedOn = DateTime.Now;
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                Serverdb.tbl_utilreport.Add(rep);
                Serverdb.SaveChanges();
            }
        }
        private int GetPartscount(int MachineID, DateTime CorrectedDate)
        {
            int TotalPartsCount = 0;
            var parts_cuttingslist = new List<tblpartscountandcutting>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                parts_cuttingslist = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date).ToList();
            }
            foreach (var row in parts_cuttingslist)
            {
                TotalPartsCount += row.PartCount;
            }

            return TotalPartsCount;
        }



    }
}