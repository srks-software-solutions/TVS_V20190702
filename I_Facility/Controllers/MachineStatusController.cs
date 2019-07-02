using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using I_Facility.Models;
using System.IO;


namespace I_Facility.Controllers
{
    public class MachineStatusController : Controller
    {
        // GET: /AllMachineStatus/
        private i_facilityEntities1 db = new i_facilityEntities1();
        public ActionResult Index()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }

            Session["CellId"] = 1;
            Session["colordata"] = null;
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                //calculating Corrected Date
                TimeSpan currentHourMint = new TimeSpan(06, 59, 59);
                TimeSpan RealCurrntHour = System.DateTime.Now.TimeOfDay;
                string CorrectedDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                DateTime correctedDate = DateTime.Now.Date;
                string PrvCorrectedDate = correctedDate.AddDays(-1).Date.ToString("yyyy-MM-dd");
                if (RealCurrntHour < currentHourMint)
                {
                    CorrectedDate = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
                    correctedDate = DateTime.Now.AddDays(-1).Date;
                    PrvCorrectedDate = correctedDate.AddDays(-1).Date.ToString("yyyy-MM-dd");
                }

                // getting all machine details and their count.
                var macData = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0);
                int mc = macData.Count();
                ViewBag.macCount = mc;

                //int[] macid = new int[mc];
                //int macidlooper = 0;
                //foreach (var v in macData)
                //{
                //    macid[macidlooper++] = v.MachineID;
                //}
                //Session["macid"] = macid;
                //ViewBag.macCount = mc;

                //int[,] maindata = new int[mc, 10];
                //int[,] maindata = new int[mc, 6];
                // write a raw query to get sum of powerOff, Operating, Idle, BreakDown, PlannedMaintenance. 
                List<Models.MachineStatus> objmachstatUpdate = new List<Models.MachineStatus>();
                List<MachineStatusCell> objcell = new List<MachineStatusCell>();

                //using (MsqlConnection mc1 = new MsqlConnection())
                //{
                var celldet = db.tblcells.Where(m => m.IsDeleted == 0).ToList();
                List<Models.MachineStatus> objmachstat = new List<Models.MachineStatus>();

                foreach (var cell in celldet)
                {
                    double CellplannedBrkDurationinMin = 0;
                    MachineStatusCell objCeldet = new MachineStatusCell();

                    using (MsqlConnection mc1 = new MsqlConnection())
                    {
                        mc1.open();
                        MySqlCommand cmd1 = new MySqlCommand("SELECT MachineID,sum(MachineOffTime) as op,sum(OperatingTime)as o,sum(IdleTime) as it,sum(BreakdownTime)as bt FROM i_facility.tblmimics where CorrectedDate='" + CorrectedDate + "'and MachineID IN (select distinct(MachineID) from i_facility.tblmachinedetails where IsDeleted = 0 and IsNormalWC = 0) group by MachineID", mc1.sqlConnection);
                        MySqlDataReader datareader = cmd1.ExecuteReader();

                        // int maindatalooper1 = 0;
                        var scrap = new tblworkorderentry();
                        var scrapqty1 = new List<tblrejectqty>();
                        var cellpartDet = new tblcellpart();
                        var partsDet = new tblpart();
                        var bottleneckmachines = new tblbottelneck();
                        using (i_facilityEntities1 db1 = new i_facilityEntities1())
                        {
                            scrap = db1.tblworkorderentries.Where(m => m.CellID == cell.CellID && m.CorrectedDate == CorrectedDate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
                            if (scrap != null)
                            {
                                //int PartID = Convert.ToInt32(scrap.FGCode);
                                partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == scrap.PartNo).FirstOrDefault();
                                if (partsDet != null)
                                    bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == partsDet.FGCode && m.CellID == scrap.CellID).FirstOrDefault();
                            }
                            else
                            {
                                cellpartDet = db.tblcellparts.Where(m => m.CellID == cell.CellID && m.IsDefault == 1 && m.IsDeleted == 0).FirstOrDefault();
                                if (cellpartDet != null)
                                    bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == cellpartDet.partNo && m.CellID == cellpartDet.CellID).FirstOrDefault();
                                string Operationnum = bottleneckmachines.tblmachinedetail.OperationNumber.ToString();
                                partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

                            }
                        }

                        int MachineoffCell = 0, OpertTimeCell = 0, IdleTimeCell = 0, BDTimeCell = 0, TotalTimeCell = 0;
                        int PrevMachineoffCell = 0, PrevOpertTimeCell = 0, PrevIdleTimeCell = 0, PrevBDTimeCell = 0, PrevTotalTimeCell = 0, utilizationCell = 0;
                        int Loadtime = 0, unloadtime = 0, LoadtimeCell = 0, unloadtimeCell = 0;
                        while (datareader.Read())
                        {
                            MachineStatus macstatus = new MachineStatus();
                            //int maindatalooper2 = 0;     
                            int bottleneckmachine = 0;
                            int MachineId = datareader.GetInt32(0);
                            macstatus.MachineID = datareader.GetInt32(0);
                            if (bottleneckmachines != null)
                            {
                                 bottleneckmachine = bottleneckmachines.MachineID;
                            }
                            int Cellid = cell.CellID;
                            var iscell = db.tblmachinedetails.Where(m => m.tblcell.CellID == Cellid && m.MachineID == MachineId).FirstOrDefault();
                            var iscellBottleneck = db.tblmachinedetails.Where(m => m.tblcell.CellID == Cellid && m.MachineID == bottleneckmachine).FirstOrDefault();
                           
                           
                            if (iscell != null)
                            {
                                int MachineOffTime = datareader.GetInt32(1);
                                int OpertTime = datareader.GetInt32(2);
                                int IdleTime = datareader.GetInt32(3);
                                int BDTime = datareader.GetInt32(4);
                                int TotalTime = MachineOffTime + OpertTime + IdleTime + BDTime;
                                if (TotalTime == 0)
                                {
                                    TotalTime = 1;
                                }

                                if (iscellBottleneck != null)
                                {
                                    MachineoffCell = MachineOffTime;
                                    OpertTimeCell = OpertTime;
                                    IdleTimeCell = IdleTime;
                                    BDTimeCell = BDTime;
                                }
                                //using (i_facilityEntities1 db1 = new i_facilityEntities1())
                                //{
                                    var machinedet = db.tblmachinedetails.Where(m => m.MachineID == MachineId).FirstOrDefault();
                                    var cellname = db.tblmachinedetails.Where(m => m.MachineID == MachineId).Select(m => m.tblcell.CelldisplayName).FirstOrDefault();

                                    macstatus.CellName = cellname;
                                    macstatus.machinedet = machinedet;
                                //}
                                Loadtime = Convert.ToInt32(partsDet.StdLoadingTime);
                                unloadtime = Convert.ToInt32(partsDet.StdUnLoadingTime);
                               
                                int PartsCount = 0;
                                GetParts_Cutting(MachineId, correctedDate, out PartsCount);
                              
                                decimal LoadtimeWithProd = Math.Round(((decimal)(Loadtime + unloadtime) / 60), 2) * PartsCount;
                                double opwithProduction = OpertTime + (double)LoadtimeWithProd;
                                double plannedBrkDurationinMin = 0;
                                var plannedbrks = db.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
                                foreach (var row in plannedbrks)
                                {
                                    plannedBrkDurationinMin += Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalMinutes;
                                }
                                CellplannedBrkDurationinMin = plannedBrkDurationinMin;
                                double Availability = TotalTime;
                                if (TotalTime > 360)
                                     Availability = TotalTime - plannedBrkDurationinMin;
                                int UtilPer = Convert.ToInt32(Convert.ToDouble((Convert.ToDouble(Convert.ToDouble(opwithProduction)) / Convert.ToDouble(Availability)) * 100));
                                if (UtilPer > 100)
                                    UtilPer = 100;

                                //int UtilPer = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OpertTime) / Convert.ToDouble(TotalTime)) * 100));

                               
                                macstatus.MachineOffTime = datareader.GetInt32(1);
                                macstatus.OperatingTime = datareader.GetInt32(2);
                                macstatus.IdleTime = datareader.GetInt32(3);
                                macstatus.BreakdownTime = datareader.GetInt32(4); ;
                                macstatus.Utilization = UtilPer;
                                macstatus.TotalTime = TotalTime;
                                
                                //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(0);
                                //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(1);
                                //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(2);
                                //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(3);
                                //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(4);
                                //maindata[maindatalooper1, maindatalooper2++] = UtilPer;
                                //maindata[maindatalooper1, maindatalooper2++] = TotalTime;
                                //maindatalooper1++;
                                objmachstat.Add(macstatus);
                            }
                        }
                        
                        LoadtimeCell = Convert.ToInt32(partsDet.StdLoadingTime);
                        unloadtimeCell = Convert.ToInt32(partsDet.StdUnLoadingTime);
                         int partscount = 0;
                        GetParts_Cutting(bottleneckmachines.MachineID, correctedDate, out partscount);
                        decimal LoadwithProdcell = Math.Round(((decimal)(LoadtimeCell + unloadtimeCell) / 60), 2) * partscount;
                        //  double LoadwithProdcell = (double)(LoadtimeCell + unloadtimeCell) * partscount;
                        double opwithLoadUnloadProd = OpertTimeCell + (double)LoadwithProdcell;
                        TotalTimeCell = Convert.ToInt32(MachineoffCell + OpertTimeCell + IdleTimeCell + BDTimeCell);
                        double AvailabilityCell = (TotalTimeCell);
                        if (TotalTimeCell > 360)
                            AvailabilityCell = (TotalTimeCell - CellplannedBrkDurationinMin);
                        if (TotalTimeCell == 0)
                            TotalTimeCell = 1;
                        if (AvailabilityCell == 0)
                            AvailabilityCell = 1;
                        utilizationCell = Convert.ToInt32(Convert.ToDouble((Convert.ToDouble(Convert.ToDouble(opwithLoadUnloadProd) / Convert.ToDouble(AvailabilityCell))) * 100));
                        if (utilizationCell > 100)
                            utilizationCell = 100;
                        else if (utilizationCell < 0)
                            utilizationCell = 0;
                        objCeldet.CellName = cell.CellName;
                        objCeldet.CellUtlization = utilizationCell;
                        objcell.Add(objCeldet);
                        datareader.Close();
                        mc1.close();
                    }
                }
                Session["CellUtilization"] = objcell;

                using (MsqlConnection mc1 = new MsqlConnection())
                {
                    mc1.open();
                    MySqlCommand Prvcmd1 = new MySqlCommand("SELECT MachineID,sum(MachineOffTime) as op,sum(OperatingTime)as o,sum(IdleTime) as it,sum(BreakdownTime)as bt FROM i_facility.tblmimics where CorrectedDate='" + PrvCorrectedDate + "'and MachineID IN (select distinct(MachineID) from i_facility.tblmachinedetails where IsDeleted = 0 and IsNormalWC = 0) group by MachineID", mc1.sqlConnection);
                    MySqlDataReader Prvdatareader = Prvcmd1.ExecuteReader();

                    //  int Prvmaindatalooper1 = 0;
                    while (Prvdatareader.Read())
                    {
                        int MachineOffTime = Prvdatareader.GetInt32(1);
                        int OpertTime = Prvdatareader.GetInt32(2);
                        int IdleTime = Prvdatareader.GetInt32(3);
                        int BDTime = Prvdatareader.GetInt32(4);
                        int TotalTime = MachineOffTime + OpertTime + IdleTime + BDTime;
                        if (TotalTime == 0)
                        {
                            TotalTime = 1;
                        }
                        else if (TotalTime > 1440)
                            TotalTime = 1440;
                        int UtilPer = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OpertTime) / Convert.ToDouble(TotalTime)) * 100));
                        foreach (var row in objmachstat)
                        {
                            if (row.MachineID == Prvdatareader.GetInt32(0))
                            {
                                row.PrevOperatingTime = OpertTime;
                                row.PrevTotalTime = TotalTime;
                                row.PrevUtilization = UtilPer;
                                objmachstatUpdate.Add(row);

                            }

                        }

                        //maindata[Prvmaindatalooper1, 7] = UtilPer;
                        //maindata[Prvmaindatalooper1, 8] = OpertTime;
                        //maindata[Prvmaindatalooper1, 9] = TotalTime;
                        //Prvmaindatalooper1++;
                    }
                    Prvdatareader.Close();

                    mc1.close();
                }
                //}

                // Session["colordata"] = maindata;
                objmachstatUpdate = objmachstatUpdate.OrderBy(m => m.CellName).ToList();
                Session["MachineStauts"] = objmachstatUpdate.OrderBy(m => m.CellName).ToList();

                //Get Modes for All Machines for Today
                List<tbllivemode> tblModeDT = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.DurationInSec >= 60 && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 1).OrderBy(m => m.MachineID).ThenBy(m => m.StartTime).ToList();
                List<tbllivemode> tblModeDTCurr = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 0).OrderBy(m => m.MachineID).ThenByDescending(m => m.ModeID).ToList();
                //Get Latest Mode for each machine and Update the DurationInSec Column
                List<tbllivemode> CurrentModesOfAllMachines = (from row in tblModeDT
                                                               where row.IsCompleted == 0
                                                               orderby row.ModeID descending
                                                               select row).ToList().OrderByDescending(m => m.ModeID).ToList();
                int PrvMachineID = 0;

                List<tbllivemode> tblModeDTloss = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.DurationInSec >= 60 && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 1 && m.LossCodeID!=null).OrderBy(m => m.MachineID).ThenBy(m => m.StartTime).ToList();


               
                foreach(var row in tblModeDTloss)
                {
                    var lossrow = db.tbllossescodes.Find(row.LossCodeID);
                    row.tbllossescode = lossrow;
                }

                foreach (var row in tblModeDTCurr)
                {
                    if (PrvMachineID != row.MachineID)
                    {
                        DateTime startDateTime = Convert.ToDateTime(row.StartTime);
                        int DurInSec = Convert.ToInt32(DateTime.Now.Subtract(startDateTime).TotalSeconds);
                        //row.DurationInSec = Convert.ToInt32( DateTime.Now.Subtract(startDateTime).TotalSeconds );
                        int ModeID = row.ModeID;
                        row.DurationInSec = DurInSec;
                        
                        tblModeDT.Add(row);
                        //foreach (var tom in tblModeDT.Where(w => w.ModeID == ModeID))
                        //{

                        //}
                        PrvMachineID = row.MachineID;
                    }
                    if (row.ModeType == "SETUP")
                    {
                        DateTime StartTime = Convert.ToDateTime(row.StartTime);
                        DateTime EndTime = DateTime.Now;
                        try
                        {
                            EndTime = Convert.ToDateTime(row.LossCodeEnteredTime);
                        }
                        catch { }
                        int DurInSec = Convert.ToInt32(EndTime.Subtract(StartTime).TotalSeconds);
                        int ModeID = row.ModeID;
                        row.DurationInSec = DurInSec;
                        tblModeDT.Add(row);
                    }
                }
                tblModeDT = tblModeDT.Where(m => m.DurationInSec >= 60).ToList();
                //Update DurationInSec to Minutes
                foreach (var MainRow in tblModeDT.Where(m => m.DurationInSec > 0))
                {
                    int GetDur = (int)MainRow.DurationInSec / 60;
                    if (MainRow.ModeType == "SETUP")
                    {
                        GetDur = (int)Convert.ToDateTime(MainRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(MainRow.StartTime)).TotalSeconds / 60;
                    }
                    if (GetDur < 1)
                    {
                        GetDur = 0;
                    }
                    MainRow.DurationInSec = GetDur;
                };
                List<string> ShopNames = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0).OrderBy(m => m.tblmachinedetail.tblcell.CelldisplayName).Select(m => m.tblmachinedetail.tblcell.CelldisplayName).Distinct().ToList();
                //  List<int> ShopID = db.tblmodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0).Select(m => m.tblmachinedetail.tblcell.CellID).Distinct().ToList();
                ViewBag.DistinctShops = ShopNames.OrderBy(m => m).ToList();
                // ViewBag.DistinctShopID = ShopID;

                //List<int> macCountshopwise = new List<int>();
                //foreach(int shop in ShopID)
                //{
                //    int MacCount = db.tblmachinedetails.Where(m => m.tblcell.CellID == shop && m.IsDeleted == 0 && m.IsNormalWC == 0).ToList().Count;
                //    macCountshopwise.Add(MacCount);
                //}
                // ViewBag.machinecountbyshop = macCountshopwise;
                //ViewBag.machinecountbyshopCount = macCountshopwise.Count;

                return View(tblModeDT.ToList());
            }

        }

        private int GetParts_Cutting(int MachineID, DateTime CorrectedDate, out int TotalPartsCount)
        {
            int CuttingTime = 0;
            TotalPartsCount = 0;
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
            string StartTime = Correcteddate + " 06:00:00";
            string EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime St = Convert.ToDateTime(StartTime);
            DateTime Et = Convert.ToDateTime(EndTime);
            var parametermasterlist = db.parameters_master.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();

            if (TopRow != null && LastRow != null)
            {
                CuttingTime = Convert.ToInt32(TopRow.CuttingTime) - Convert.ToInt32(LastRow.CuttingTime);
                TotalPartsCount = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);
            }
            return CuttingTime;
        }

        private int GetParts_Cutting_Cell(int CellId, DateTime CorrectedDate, out int TotalPartsCount)
        {
            int CuttingTime = 0;
            TotalPartsCount = 0;
            int MachineID = 0;
            var lastmachine = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellId && m.IsLastMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
            string StartTime = Correcteddate + " 06:00:00";
            string EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime St = Convert.ToDateTime(StartTime);
            DateTime Et = Convert.ToDateTime(EndTime);
            if (lastmachine != null)
                MachineID = lastmachine.MachineID;

            var parametermasterlist = db.parameters_master.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();

            if (TopRow != null && LastRow != null)
            {
                CuttingTime = Convert.ToInt32(TopRow.CuttingTime) - Convert.ToInt32(LastRow.CuttingTime);
                TotalPartsCount = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);
            }
            return CuttingTime;
        }

        public ActionResult Index1()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }

            Session["CellId"] = 1;
            Session["colordata"] = null;
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];

            //calculating Corrected Date
            TimeSpan currentHourMint = new TimeSpan(06, 59, 59);
            TimeSpan RealCurrntHour = System.DateTime.Now.TimeOfDay;
            string CorrectedDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            DateTime correctedDate = DateTime.Now.Date;
            string PrvCorrectedDate = correctedDate.AddDays(-1).Date.ToString("yyyy-MM-dd");
            if (RealCurrntHour < currentHourMint)
            {
                CorrectedDate = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
                correctedDate = DateTime.Now.AddDays(-1).Date;
                PrvCorrectedDate = correctedDate.AddDays(-1).Date.ToString("yyyy-MM-dd");
            }

            // getting all machine details and their count.
            var macData = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0);
            int mc = macData.Count();
            ViewBag.macCount = mc;

            //int[] macid = new int[mc];
            //int macidlooper = 0;
            //foreach (var v in macData)
            //{
            //    macid[macidlooper++] = v.MachineID;
            //}
            //Session["macid"] = macid;
            //ViewBag.macCount = mc;

            //int[,] maindata = new int[mc, 10];
            //int[,] maindata = new int[mc, 6];
            // write a raw query to get sum of powerOff, Operating, Idle, BreakDown, PlannedMaintenance. 
            List<Models.MachineStatus> objmachstatUpdate = new List<Models.MachineStatus>();
            using (MsqlConnection mc1 = new MsqlConnection())
            {
                mc1.open();
                MySqlCommand cmd1 = new MySqlCommand("SELECT MachineID,sum(MachineOffTime) as op,sum(OperatingTime)as o,sum(IdleTime) as it,sum(BreakdownTime)as bt FROM i_facility.tblmimics where CorrectedDate='" + CorrectedDate + "'and MachineID IN (select distinct(MachineID) from i_facility.tblmachinedetails where IsDeleted = 0 and IsNormalWC = 0) group by MachineID", mc1.sqlConnection);
                MySqlDataReader datareader = cmd1.ExecuteReader();
                // int maindatalooper1 = 0;
                List<Models.MachineStatus> objmachstat = new List<Models.MachineStatus>();
                int MachineoffCell = 0, OpertTimeCell = 0, IdleTimeCell = 0, BDTimeCell = 0, TotalTimeCell = 0;
                while (datareader.Read())
                {
                    MachineStatus macstatus = new MachineStatus();
                    //int maindatalooper2 = 0;     
                    int MachineId = datareader.GetInt32(0);
                    macstatus.MachineID = datareader.GetInt32(0);

                    int MachineOffTime = datareader.GetInt32(1);
                    int OpertTime = datareader.GetInt32(2);
                    int IdleTime = datareader.GetInt32(3);
                    int BDTime = datareader.GetInt32(4);
                    int TotalTime = MachineOffTime + OpertTime + IdleTime + BDTime;
                    if (TotalTime == 0)
                    {
                        TotalTime = 1;
                    }
                    int UtilPer = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OpertTime) / Convert.ToDouble(TotalTime)) * 100));

                    var cellname = db.tblmachinedetails.Where(m => m.MachineID == macstatus.MachineID).Select(m => m.tblcell.CelldisplayName).FirstOrDefault();
                    var machinedet = db.tblmachinedetails.Where(m => m.MachineID == macstatus.MachineID).FirstOrDefault();
                    macstatus.CellName = cellname;
                    macstatus.MachineOffTime = datareader.GetInt32(1);
                    macstatus.OperatingTime = datareader.GetInt32(2);
                    macstatus.IdleTime = datareader.GetInt32(3);
                    macstatus.BreakdownTime = datareader.GetInt32(4); ;
                    macstatus.Utilization = UtilPer;
                    macstatus.TotalTime = TotalTime;
                    macstatus.machinedet = machinedet;
                    //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(0);
                    //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(1);
                    //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(2);
                    //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(3);
                    //maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(4);
                    //maindata[maindatalooper1, maindatalooper2++] = UtilPer;
                    //maindata[maindatalooper1, maindatalooper2++] = TotalTime;
                    //maindatalooper1++;
                    objmachstat.Add(macstatus);

                }
                datareader.Close();

                MySqlCommand Prvcmd1 = new MySqlCommand("SELECT MachineID,sum(MachineOffTime) as op,sum(OperatingTime)as o,sum(IdleTime) as it,sum(BreakdownTime)as bt FROM i_facility.tblmimics where CorrectedDate='" + PrvCorrectedDate + "'and MachineID IN (select distinct(MachineID) from i_facility.tblmachinedetails where IsDeleted = 0 and IsNormalWC = 0) group by MachineID", mc1.sqlConnection);
                MySqlDataReader Prvdatareader = Prvcmd1.ExecuteReader();
                //  int Prvmaindatalooper1 = 0;
                while (Prvdatareader.Read())
                {
                    int MachineOffTime = Prvdatareader.GetInt32(1);
                    int OpertTime = Prvdatareader.GetInt32(2);
                    int IdleTime = Prvdatareader.GetInt32(3);
                    int BDTime = Prvdatareader.GetInt32(4);
                    int TotalTime = MachineOffTime + OpertTime + IdleTime + BDTime;
                    if (TotalTime == 0)
                    {
                        TotalTime = 1;
                    }
                    int UtilPer = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OpertTime) / Convert.ToDouble(TotalTime)) * 100));
                    foreach (var row in objmachstat)
                    {
                        if (row.MachineID == Prvdatareader.GetInt32(0))
                        {
                            row.PrevOperatingTime = OpertTime;
                            row.PrevTotalTime = TotalTime;
                            row.PrevUtilization = UtilPer;
                            objmachstatUpdate.Add(row);

                        }

                    }

                    //maindata[Prvmaindatalooper1, 7] = UtilPer;
                    //maindata[Prvmaindatalooper1, 8] = OpertTime;
                    //maindata[Prvmaindatalooper1, 9] = TotalTime;
                    //Prvmaindatalooper1++;
                }
                Prvdatareader.Close();
                mc1.close();

            }
            // Session["colordata"] = maindata;
            objmachstatUpdate = objmachstatUpdate.OrderBy(m => m.CellName).ToList();
            Session["MachineStauts"] = objmachstatUpdate.OrderBy(m => m.CellName).ToList();

            //Get Modes for All Machines for Today
            List<tbllivemode> tblModeDT = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 1).OrderBy(m => m.MachineID).ThenBy(m => m.StartTime).ToList();
            List<tbllivemode> tblModeDTCurr = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 0).OrderBy(m => m.MachineID).ThenByDescending(m => m.ModeID).ToList();
            //Get Latest Mode for each machine and Update the DurationInSec Column
            List<tbllivemode> CurrentModesOfAllMachines = (from row in tblModeDT
                                                           where row.IsCompleted == 0
                                                           orderby row.ModeID descending
                                                           select row).ToList().OrderByDescending(m => m.ModeID).ToList();
            int PrvMachineID = 0;
            foreach (var row in tblModeDTCurr)
            {
                if (PrvMachineID != row.MachineID)
                {
                    DateTime startDateTime = Convert.ToDateTime(row.StartTime);
                    int DurInSec = Convert.ToInt32(DateTime.Now.Subtract(startDateTime).TotalSeconds);
                    //row.DurationInSec = Convert.ToInt32( DateTime.Now.Subtract(startDateTime).TotalSeconds );
                    int ModeID = row.ModeID;
                    row.DurationInSec = DurInSec;
                    tblModeDT.Add(row);
                    //foreach (var tom in tblModeDT.Where(w => w.ModeID == ModeID))
                    //{

                    //}
                    PrvMachineID = row.MachineID;
                }
                if (row.ModeType == "SETUP")
                {
                    DateTime StartTime = Convert.ToDateTime(row.StartTime);
                    DateTime EndTime = DateTime.Now;
                    try
                    {
                        EndTime = Convert.ToDateTime(row.LossCodeEnteredTime);
                    }
                    catch { }
                    int DurInSec = Convert.ToInt32(EndTime.Subtract(StartTime).TotalSeconds);
                    int ModeID = row.ModeID;
                    row.DurationInSec = DurInSec;
                    tblModeDT.Add(row);
                }
            }
            //Update DurationInSec to Minutes
            foreach (var MainRow in tblModeDT.Where(m => m.DurationInSec > 0))
            {
                int GetDur = (int)MainRow.DurationInSec / 60;
                if (MainRow.ModeType == "SETUP")
                {
                    GetDur = (int)Convert.ToDateTime(MainRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(MainRow.StartTime)).TotalSeconds / 60;
                }
                if (GetDur < 1)
                {
                    GetDur = 0;
                }
                MainRow.DurationInSec = GetDur;
            };
            List<string> ShopNames = db.tbllivemodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0).OrderBy(m => m.tblmachinedetail.tblcell.CelldisplayName).Select(m => m.tblmachinedetail.tblcell.CelldisplayName).Distinct().ToList();
            //  List<int> ShopID = db.tblmodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0).Select(m => m.tblmachinedetail.tblcell.CellID).Distinct().ToList();
            ViewBag.DistinctShops = ShopNames.OrderBy(m => m).ToList();
            // ViewBag.DistinctShopID = ShopID;

            //List<int> macCountshopwise = new List<int>();
            //foreach(int shop in ShopID)
            //{
            //    int MacCount = db.tblmachinedetails.Where(m => m.tblcell.CellID == shop && m.IsDeleted == 0 && m.IsNormalWC == 0).ToList().Count;
            //    macCountshopwise.Add(MacCount);
            //}
            // ViewBag.machinecountbyshop = macCountshopwise;
            //ViewBag.machinecountbyshopCount = macCountshopwise.Count;
            return View(tblModeDT.ToList());
        }


        //public ActionResult Index()
        //{
        //    if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
        //    {
        //        return RedirectToAction("Login", "Login", null);
        //    }

        //    Session["CellId"] = 1;
        //    Session["colordata"] = null;
        //    ViewBag.Logout = Session["Username"];
        //    ViewBag.roleid = Session["RoleID"];

        //    //calculating Corrected Date
        //    TimeSpan currentHourMint = new TimeSpan(06, 59, 59);
        //    TimeSpan RealCurrntHour = System.DateTime.Now.TimeOfDay;
        //    string CorrectedDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
        //    DateTime correctedDate = DateTime.Now.Date;
        //    string PrvCorrectedDate = correctedDate.AddDays(-1).Date.ToString("yyyy-MM-dd");
        //    if (RealCurrntHour < currentHourMint)
        //    {
        //        CorrectedDate = DateTime.Now.AddDays(-1).Date.ToString("yyyy-MM-dd");
        //        correctedDate = DateTime.Now.AddDays(-1).Date;
        //        PrvCorrectedDate = correctedDate.AddDays(-1).Date.ToString("yyyy-MM-dd");
        //    }

        //    // getting all machine details and their count.
        //    var macData = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0);
        //    int mc = macData.Count();
        //    ViewBag.macCount = mc;

        //    int[] macid = new int[mc];
        //    int macidlooper = 0;
        //    foreach (var v in macData)
        //    {
        //        macid[macidlooper++] = v.MachineID;
        //    }
        //    Session["macid"] = macid;
        //    ViewBag.macCount = mc;

        //    int[,] maindata = new int[mc, 10];
        //    //int[,] maindata = new int[mc, 6];
        //    // write a raw query to get sum of powerOff, Operating, Idle, BreakDown, PlannedMaintenance. 

        //    using (MsqlConnection mc1 = new MsqlConnection())
        //    {
        //        mc1.open();
        //        MySqlCommand cmd1 = new MySqlCommand("SELECT MachineID,sum(MachineOffTime) as op,sum(OperatingTime)as o,sum(IdleTime) as it,sum(BreakdownTime)as bt FROM i_facility.tblmimics where CorrectedDate='" + CorrectedDate + "'and MachineID IN (select distinct(MachineID) from i_facility.tblmachinedetails where IsDeleted = 0 and IsNormalWC = 0) group by MachineID", mc1.sqlConnection);
        //        MySqlDataReader datareader = cmd1.ExecuteReader();
        //        int maindatalooper1 = 0;

        //        while (datareader.Read())
        //        {
        //            int maindatalooper2 = 0;
        //            int MachineOffTime = datareader.GetInt32(1);
        //            int OpertTime = datareader.GetInt32(2);
        //            int IdleTime = datareader.GetInt32(3);
        //            int BDTime = datareader.GetInt32(4);
        //            int TotalTime = MachineOffTime + OpertTime + IdleTime + BDTime;
        //            if (TotalTime == 0)
        //            {
        //                TotalTime = 1;
        //            }
        //            int UtilPer = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OpertTime) / Convert.ToDouble(TotalTime)) * 100));
        //            maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(0);
        //            maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(1);
        //            maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(2);
        //            maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(3);
        //            maindata[maindatalooper1, maindatalooper2++] = datareader.GetInt32(4);
        //            maindata[maindatalooper1, maindatalooper2++] = UtilPer;
        //            maindata[maindatalooper1, maindatalooper2++] = TotalTime;
        //            maindatalooper1++;
        //        }
        //        datareader.Close();
        //        MySqlCommand Prvcmd1 = new MySqlCommand("SELECT MachineID,sum(MachineOffTime) as op,sum(OperatingTime)as o,sum(IdleTime) as it,sum(BreakdownTime)as bt FROM i_facility.tblmimics where CorrectedDate='" + CorrectedDate + "'and MachineID IN (select distinct(MachineID) from i_facility.tblmachinedetails where IsDeleted = 0 and IsNormalWC = 0) group by MachineID", mc1.sqlConnection);
        //        MySqlDataReader Prvdatareader = Prvcmd1.ExecuteReader();
        //        int Prvmaindatalooper1 = 0;
        //        while (Prvdatareader.Read())
        //        {
        //            int MachineOffTime = Prvdatareader.GetInt32(1);
        //            int OpertTime = Prvdatareader.GetInt32(2);
        //            int IdleTime = Prvdatareader.GetInt32(3);
        //            int BDTime = Prvdatareader.GetInt32(4);
        //            int TotalTime = MachineOffTime + OpertTime + IdleTime + BDTime;
        //            if (TotalTime == 0)
        //            {
        //                TotalTime = 1;
        //            }
        //            int UtilPer = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OpertTime) / Convert.ToDouble(TotalTime)) * 100));
        //            maindata[Prvmaindatalooper1, 7] = UtilPer;
        //            maindata[Prvmaindatalooper1, 8] = OpertTime;
        //            maindata[Prvmaindatalooper1, 9] = TotalTime;
        //            Prvmaindatalooper1++;
        //        }
        //        Prvdatareader.Close();
        //        mc1.close();
        //    }
        //    Session["colordata"] = maindata;

        //    //Get Modes for All Machines for Today
        //    List<tblmode> tblModeDT = db.tblmodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 1).OrderBy(m => m.MachineID).ThenBy(m => m.StartTime).ToList();
        //    List<tblmode> tblModeDTCurr = db.tblmodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0 && m.IsCompleted == 0).OrderBy(m => m.MachineID).ThenByDescending(m => m.ModeID).ToList();
        //    //Get Latest Mode for each machine and Update the DurationInSec Column
        //    List<tblmode> CurrentModesOfAllMachines = (from row in tblModeDT
        //                                               where row.IsCompleted == 0
        //                                               orderby row.ModeID descending
        //                                               select row).ToList().OrderByDescending(m => m.ModeID).ToList();
        //    int PrvMachineID = 0;
        //    foreach (var row in tblModeDTCurr)
        //    {
        //        if (PrvMachineID != row.MachineID)
        //        {
        //            DateTime startDateTime = Convert.ToDateTime(row.StartTime);
        //            int DurInSec = Convert.ToInt32(DateTime.Now.Subtract(startDateTime).TotalSeconds);
        //            //row.DurationInSec = Convert.ToInt32( DateTime.Now.Subtract(startDateTime).TotalSeconds );
        //            int ModeID = row.ModeID;
        //            row.DurationInSec = DurInSec;
        //            tblModeDT.Add(row);
        //            //foreach (var tom in tblModeDT.Where(w => w.ModeID == ModeID))
        //            //{

        //            //}
        //            PrvMachineID = row.MachineID;
        //        }
        //        if (row.ModeType == "SETUP")
        //        {
        //            DateTime StartTime = Convert.ToDateTime(row.StartTime);
        //            DateTime EndTime = DateTime.Now;
        //            try
        //            {
        //                EndTime = Convert.ToDateTime(row.LossCodeEnteredTime);
        //            }
        //            catch { }
        //            int DurInSec = Convert.ToInt32(EndTime.Subtract(StartTime).TotalSeconds);
        //            int ModeID = row.ModeID;
        //            row.DurationInSec = DurInSec;
        //            tblModeDT.Add(row);
        //        }
        //    }
        //    //Update DurationInSec to Minutes
        //    foreach (var MainRow in tblModeDT.Where(m => m.DurationInSec > 0))
        //    {
        //        int GetDur = (int)MainRow.DurationInSec / 60;
        //        if (MainRow.ModeType == "SETUP")
        //        {
        //            GetDur = (int)Convert.ToDateTime(MainRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(MainRow.StartTime)).TotalSeconds / 60;
        //        }
        //        if (GetDur < 1)
        //        {
        //            GetDur = 0;
        //        }
        //        MainRow.DurationInSec = GetDur;
        //    };
        //    List<string> ShopNames = db.tblmodes.Where(m => m.CorrectedDate == correctedDate && m.tblmachinedetail.IsDeleted == 0 && m.tblmachinedetail.IsNormalWC == 0).Select(m => m.tblmachinedetail.tblcell.CelldisplayName).Distinct().ToList();
        //    ViewBag.DistinctShops = ShopNames;
        //    return View(tblModeDT.OrderBy(m => m.MachineID).ThenBy(m => m.StartTime).ToList());
        //}
    }
}
