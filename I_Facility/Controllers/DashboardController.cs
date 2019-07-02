using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using I_Facility;
using I_Facility.Models;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Data;
using System.Dynamic;
using System.Drawing;
using static I_Facility.Models.TargetVsActal;

namespace I_Facility.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        i_facilityEntities1 db = new i_facilityEntities1();

        public ActionResult Dashboard()
        {
            Session["Errors"] = "";
            return View();
        }

        public ActionResult NewDashboard2911()
        {

            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }

            return View();
        }

        //public string GetCorrectedDate()
        //{
        //    DateTime nowDate = DateTime.Now;
        //    string correctedDate = DateTime.Now.ToString("yyyy-MM-05");
        //    if (nowDate.Hour < 7 && nowDate.Hour > 0)
        //    {
        //        correctedDate = nowDate.AddDays(-1).ToString("yyyy-MM-dd");
        //    }
        //    return correctedDate;
        //}      

        private string GetCorrectedDate()
        {
            DateTime correctedDate = DateTime.Now;
            var daytimings = db.tbldaytimings.Where(m => m.IsDeleted == 0).FirstOrDefault();
            if (daytimings != null)
            {
                DateTime Start = Convert.ToDateTime(correctedDate.ToString("yyyy-MM-dd") + " " + daytimings.StartTime);


                //DateTime Start = Convert.ToDateTime(dtMode.Rows[0][0].ToString());
                if (Start <= DateTime.Now)
                {
                    correctedDate = DateTime.Now.Date;
                }
                else
                {
                    correctedDate = DateTime.Now.AddDays(-1).Date;
                }
            }
            string correctedDateformat = correctedDate.ToString("yyyy-MM-dd");
            return correctedDateformat;
        }

        public string MachineDashboard()
        {

            string correctedDate = GetCorrectedDate();  // get CorrectedDate
            string res = "";                      // string correctedDate = "2018-08-23";

            DateTime correctedDate1 = Convert.ToDateTime(correctedDate);
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {

                int c = 0;
                List<GetMachines> AllMachinesList = new List<GetMachines>();
                List<MachineConnectivityStatusModel> machineModel = new List<MachineConnectivityStatusModel>();

                var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
                foreach (var row in celldet)
                {
                    MachineConnectivityStatusModel machinedet = new MachineConnectivityStatusModel();
                    GetMachines machinesdata = new GetMachines();

                    List<Machines> machineList = new List<Machines>();
                    machinesdata.cellName = row.CelldisplayName;
                    machinesdata.plantName = row.tblplant.PlantDisplayName;
                    machinesdata.shopName = row.tblshop.Shopdisplayname;

                    //Previous
                    //machinedet.cellName = row.CellName;
                    //machinedet.plantName = row.tblplant.PlantDisplayName;
                    //machinedet.shopName = row.tblshop.Shopdisplayname;

                    var machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == row.CellID).ToList();
                    foreach (var machine in machineslist)
                    {
                        Machines machines = new Machines();
                        int machineID = machine.MachineID;
                        var modetails = db.tbllivemodes.Where(m => m.MachineID == machineID && m.IsDeleted == 0 && m.IsCompleted == 0 && m.CorrectedDate == correctedDate1.Date).OrderByDescending(m => m.ModeID).FirstOrDefault();
                        if (modetails != null)
                        {
                            machines.Color = modetails.ColorCode;
                            machines.CurrentStatus = modetails.MacMode;
                        }
                        machines.MachineName = machine.MachineName;
                        machines.MachineID = machine.MachineID;
                        machines.Time = DateTime.Now.ToShortTimeString();
                        machineList.Add(machines);

                    }
                    ///  machinedet.machines = machineList;
                    machinesdata.machines = machineList;
                    if (c == 0)
                    {
                        machineModel = MachineConnectivityDet();
                        machinesdata.machineModel = machineModel;
                        c = c + 1;
                    }



                    AllMachinesList.Add(machinesdata);
                    //machineModel.Add(machinedet);
                }
                AllMachinesList = AllMachinesList.OrderBy(m => m.cellName).ToList();
                res = JsonConvert.SerializeObject(AllMachinesList);
            }
            return res;
        }

        #region Commented

        //public string MachineConnectivity(int MID, string Status)
        //{
        //    List<MachineConnectivityStatusModel> model = new List<MachineConnectivityStatusModel>();
        //    string res = "";
        //    DateTime nowDate = DateTime.Now;
        //    //string correctedDate = "2017-11-18";
        //    string correctedDate = DateTime.Now.ToString("yyyy-MM-dd");
        //    if (nowDate.Hour < 7 && nowDate.Hour > 0)
        //    {
        //        correctedDate = nowDate.AddDays(-1).ToString("yyyy-MM-dd");
        //    }
        //    DateTime correctDate = Convert.ToDateTime(correctedDate);
        //    var machineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MID).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
        //    if (machineDetails != null)
        //    {
        //        var cuttingAndPartsData = db.tbllivemodes.Where(m => m.MachineID == MID && m.CorrectedDate == correctDate && m.IsDeleted == 0).Select(m => new { m.ModeID, m.CuttingDuration, m.TotalPartsCount, m.ColorCode, m.MacMode }).OrderByDescending(m => m.ModeID).FirstOrDefault();
        //        var PowerTime = db.tbllivemodes.Where(m => m.MachineID == MID && m.IsDeleted == 0 && m.CorrectedDate == correctDate).Sum(m => m.DurationInSec);
        //        int Cuttingtime = 0, totalparts = 0;
        //        string ColorCode = "", MacMode = "";
        //        if (cuttingAndPartsData != null)
        //        {
        //            Cuttingtime =Convert.ToInt32( cuttingAndPartsData.CuttingDuration);
        //            totalparts = Convert.ToInt32(cuttingAndPartsData.TotalPartsCount);
        //            ColorCode = cuttingAndPartsData.ColorCode;
        //            MacMode = cuttingAndPartsData.MacMode;
        //        }

        //        Cuttingtime = GetParts_Cutting(MID, correctDate, out totalparts);
        //        var machinmodes = db.tbllivemodes.Where(m => m.MachineID == MID && m.CorrectedDate == correctDate && m.IsDeleted == 0).Select(m => new { m.MacMode, m.ColorCode, m.DurationInSec }).ToList();
        //        MachineConnectivityStatusModel databind = new MachineConnectivityStatusModel();
        //        databind.MachineName = machineDetails.MachineDisplayName;
        //        databind.MachineID = machineDetails.MachineID;
        //        double IdleTime = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec));

        //        double running = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec));
        //        VirtualHMI objvirtual = new VirtualHMI(machineDetails.IPAddress, machineDetails.MachineName);
        //        double CycleTime = 0;
        //        short exeprogramnum = 0;
        //        ushort h;
        //        int AxisCount = 32;
        //        List<string> retValList = new List<string>();
        //        List<AxisDetails> AxisDetailsList = new List<AxisDetails>();
        //        objvirtual.VirtualDispRefersh(AxisCount, out retValList, out AxisDetailsList);
        //        string programnum = retValList[6].ToString();
        //        objvirtual.UTFValuesforMachine(out CycleTime, out exeprogramnum, out h);
        //        TimeSpan tmrunning = TimeSpan.FromSeconds(running);
        //        TimeSpan tmIdle = TimeSpan.FromSeconds(IdleTime);
        //        TimeSpan tm1 = TimeSpan.FromSeconds(CycleTime);
        //        TimeSpan tm2 = TimeSpan.FromSeconds(Convert.ToDouble(PowerTime));
        //        TimeSpan tm3 = TimeSpan.FromSeconds(Convert.ToDouble(Cuttingtime));
        //        databind.RunningTime = tmrunning.ToString(@"hh\:mm\:ss");
        //        databind.IdleTime = tmIdle.ToString(@"hh\:mm\:ss");
        //        databind.CycleTime = tm1.ToString(@"hh\:mm\:ss");
        //        databind.ExeProgramName = programnum.ToString();
        //        databind.Color = ColorCode;
        //        databind.CurrentStatus = MacMode;
        //        databind.PowerOnTime = tm2.ToString(@"hh\:mm\:ss");
        //        databind.CuttingTime = tm3.ToString(@"hh\:mm\:ss");
        //        databind.PartsCount = totalparts;
        //        model.Add(databind);
        //    }
        //    res = JsonConvert.SerializeObject(model);
        //    return res;
        //}
        #endregion
        public List<MachineConnectivityStatusModel> MachineConnectivityDet()
        {
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            List<MachineConnectivityStatusModel> model = new List<MachineConnectivityStatusModel>();
            string res = "";
            DateTime nowDate = DateTime.Now;
            string correctedDate = GetCorrectedDate();
            DateTime correctDate = Convert.ToDateTime(correctedDate);
            var machineDetailsList = new List<tblmachinedetail>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                machineDetailsList = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).ToList();
            }
            foreach (var machineDetails in machineDetailsList)
            {
                int MID = machineDetails.MachineID;
                var livemodeData = new List<tbllivemode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    livemodeData = db.tbllivemodes.Where(m => m.MachineID == MID && m.CorrectedDate == correctDate && m.IsDeleted == 0).ToList();
                }

                //var cuttingAndPartsData = db.tbllivemodes.Where(m => m.MachineID == MID && m.CorrectedDate == correctDate && m.IsDeleted == 0).Select(m => new { m.ModeID, m.CuttingDuration, m.TotalPartsCount, m.ColorCode, m.MacMode }).OrderByDescending(m => m.ModeID).FirstOrDefault();
                //var PowerTime = db.tbllivemodes.Where(m => m.MachineID == MID && m.IsDeleted == 0 && m.CorrectedDate == correctDate).Sum(m => m.DurationInSec);
                var cuttingAndPartsData = livemodeData.Select(m => new { m.ModeID, m.CuttingDuration, m.TotalPartsCount, m.ColorCode, m.MacMode }).OrderByDescending(m => m.ModeID).FirstOrDefault();
                var PowerTime = livemodeData.Sum(m => m.DurationInSec);
                int Cuttingtime = 0, totalparts = 0;
                string ColorCode = "", MacMode = "";
                if (cuttingAndPartsData != null)
                {
                    Cuttingtime = Convert.ToInt32(cuttingAndPartsData.CuttingDuration);
                    totalparts = Convert.ToInt32(cuttingAndPartsData.TotalPartsCount);
                    ColorCode = cuttingAndPartsData.ColorCode;
                    MacMode = cuttingAndPartsData.MacMode;
                }

                Cuttingtime = GetParts_Cutting(MID, correctDate, out totalparts);
                var machinmodes = livemodeData.Select(m => new { m.MacMode, m.ColorCode, m.DurationInSec }).ToList();
                MachineConnectivityStatusModel databind = new MachineConnectivityStatusModel();
                databind.MachineName = machineDetails.MachineDisplayName;
                databind.MachineID = machineDetails.MachineID;
                double IdleTime = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec));

                double running = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec));
                VirtualHMI objvirtual = new VirtualHMI(machineDetails.IPAddress, machineDetails.MachineName);
                double CycleTime = 0;
                short exeprogramnum = 0;
                ushort h;
                int AxisCount = 32;
                List<string> retValList = new List<string>();
                List<AxisDetails> AxisDetailsList = new List<AxisDetails>();
                objvirtual.VirtualDispRefersh(AxisCount, out retValList, out AxisDetailsList);
                string programnum = retValList[6].ToString();
                objvirtual.UTFValuesforMachine(out CycleTime, out exeprogramnum, out h);
                TimeSpan tmrunning = TimeSpan.FromSeconds(running);
                TimeSpan tmIdle = TimeSpan.FromSeconds(IdleTime);
                TimeSpan tm1 = TimeSpan.FromMinutes(CycleTime);
                TimeSpan tm2 = TimeSpan.FromSeconds(Convert.ToDouble(PowerTime));
                TimeSpan tm3 = TimeSpan.FromMinutes(Convert.ToDouble(Cuttingtime));
                databind.RunningTime = tmrunning.ToString(@"hh\:mm\:ss");
                databind.IdleTime = tmIdle.ToString(@"hh\:mm\:ss");
                databind.CycleTime = tm1.ToString(@"hh\:mm\:ss");
                databind.ExeProgramName = programnum.ToString();
                databind.Color = ColorCode;
                databind.CurrentStatus = MacMode;
                databind.PowerOnTime = tm2.ToString(@"hh\:mm\:ss");
                databind.CuttingTime = tm3.ToString(@"hh\:mm\:ss");
                databind.PartsCount = totalparts;
                if (running == 0)
                    running = 1;
                running = (running / 60);
                databind.CuttingRatio = Math.Round(Convert.ToDecimal(((double)Cuttingtime / running)) * 100, 2).ToString();
                model.Add(databind);
            }
            res = JsonConvert.SerializeObject(model);

            // return res;
            return model;
            //}
        }

        public ActionResult MConnectivity2()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }

            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];
            string correctedDate = DateTime.Now.ToString("yyyy-MM-dd");
            string correctdate = correctedDate;
            DateTime correctedDate1 = Convert.ToDateTime(correctdate);

            //ViewModel model = new ViewModel();
            //model.MachineUtilizationModels = MachineUtilization();
            ////Alarms();
            //model.AlarmLists = GetAlarms();
            return View();
        }

        // Utilization
        public List<MachineUtilizationModel> MachineUtilization()
        {
            List<MachineUtilizationModel> machineUtilizationList = new List<MachineUtilizationModel>();
            var machinedetails = new List<tblmachinedetail>();
            var celldet = new List<tblcell>();
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            //string correctedDate = "2017-11-18";
            string correctedDate = GetCorrectedDate();
            DateTime correctdate = Convert.ToDateTime(correctedDate);
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                celldet = db.tblcells.Where(m => m.IsDeleted == 0).ToList();
                machinedetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).ToList();
            }

            if (machinedetails != null)
            {
                machinedetails = machinedetails.OrderBy(m => m.MachineID).ToList();
                foreach (var machine in machinedetails)
                {
                    var cellName = celldet.Where(m => m.CellID == machine.CellID).FirstOrDefault();
                    var scrap = new tblworkorderentry();
                    var scrapqty1 = new List<tblrejectqty>();
                    var cellpartDet = new tblcellpart();
                    var partsDet = new tblpart();
                    var bottleneckmachines = new tblbottelneck();
                    using (i_facilityEntities1 db1 = new i_facilityEntities1())
                    {
                        scrap = db1.tblworkorderentries.Where(m => m.CellID == cellName.CellID && m.CorrectedDate == correctedDate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
                        if (scrap != null)
                        {
                            partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == scrap.PartNo).FirstOrDefault();
                            if (partsDet != null)
                                bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == partsDet.FGCode && m.CellID == scrap.CellID).FirstOrDefault();
                        }
                        else
                        {
                            cellpartDet = db.tblcellparts.Where(m => m.CellID == cellName.CellID && m.IsDefault == 1 && m.IsDeleted == 0).FirstOrDefault();
                            if (cellpartDet != null)
                                bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == cellpartDet.partNo && m.CellID == cellpartDet.CellID).FirstOrDefault();
                            string Operationnum = bottleneckmachines.tblmachinedetail.OperationNumber.ToString();
                            partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

                        }
                    }

                    MachineUtilizationModel mum = new MachineUtilizationModel();
                    int machineID = machine.MachineID;
                    string machineName = machine.MachineDisplayName;
                    var tblmode = db.tbllivemodes.Where(m => m.IsDeleted == 0 && m.IsCompleted == 0 && m.MachineID == machineID && m.CorrectedDate == correctdate).FirstOrDefault();
                    var machinmodes = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && m.IsDeleted == 0).Select(m => new { m.MacMode, m.ColorCode, m.DurationInSec }).ToList();
                    double RunningTimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec));
                    var StartTime = db.tbldaytimings.Where(d => d.IsDeleted == 0).Select(d => d.StartTime).FirstOrDefault();
                    TimeSpan correctedTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    TimeSpan TotalTimeTaken = correctedTime - StartTime;
                    int PartsCount = 0;
                    GetParts_Cutting(machine.MachineID, correctdate, out PartsCount);
                    double LoadtimewithProd = (double)(partsDet.StdLoadingTime + partsDet.StdUnLoadingTime) * PartsCount;
                    double plannedBrkDurationinMin = 0;
                    double opwithProduction = RunningTimeinsec + LoadtimewithProd;
                    var plannedbrks = db.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
                    foreach (var row in plannedbrks)
                    {
                        plannedBrkDurationinMin += Convert.ToDateTime(correctdate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctdate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalSeconds;
                    }
                    mum.CellName = cellName.CelldisplayName;
                    double totaltimetaken = Convert.ToDouble(TotalTimeTaken.TotalSeconds);
                    double Availability = totaltimetaken;
                    if (totaltimetaken > 360)
                        Availability = totaltimetaken - plannedBrkDurationinMin;
                    //  double Availability = totaltimetaken - plannedBrkDurationinMin;
                    double MachineUtilization = ((opwithProduction) / Availability) * 100;
                    if (MachineUtilization > 100)
                        MachineUtilization = 100;
                    MachineUtilization = Math.Round((Double)MachineUtilization, 2);
                    mum.MachineName = machine.MachineDisplayName;
                    mum.MachineUtiization = MachineUtilization;
                    mum.CurrentTime = correctedTime;
                    mum.cellid = cellName.CellID;
                    machineUtilizationList.Add(mum);
                }
            }

            return machineUtilizationList;
        }


        public string GetMachineUtilization()
        {
            string res = "";
            ViewModel model = new ViewModel();
            model.MachineUtilizationModels = MachineUtilization();
            //Alarms();
            model.AlarmLists = GetAlarms();
            res = JsonConvert.SerializeObject(model);
            return res;

        }
        // OEE Chart
        public string OEEs()
        {
            List<OEEModel> model = new List<OEEModel>();
            string result = "";
            string OEEOP = "";
            string cellName = "";
            string[] backgroundcolr;
            string[] borderColor;
            //string correctedDate = "2017-11-18";
            string correctedDate = GetCorrectedDate();
            DateTime correctdate = Convert.ToDateTime(correctedDate);
            //var plantDetails = db.tblplants.Where(p => p.IsDeleted == 0).ToList();
            //if (plantDetails.Count > 0)
            //{
            //foreach (var plant in plantDetails)
            //{
            //var plantid = plant.PlantID;
            //var plantname = plant.PlantDisplayName;
            //var shopDetails = db.tblshops.Where(s => s.IsDeleted == 0 && s.PlantID == plantid).ToList();
            //if (shopDetails.Count > 0)
            //{
            //    foreach (var shop in shopDetails)
            //    {

            //var shopId = shop.ShopID;

            var cellDetails = new List<tblcell>();
            var shopdet = new List<tblshop>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                cellDetails = db.tblcells.Where(c => c.IsDeleted == 0).ToList();

                shopdet = db.tblshops.Where(m => m.IsDeleted == 0).ToList();
            }
            int count = 0;
            if (cellDetails.Count > 0)
            {
                foreach (var cell in cellDetails)
                {

                    Color color = GetRandomColour();
                    string val = "rgba(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + "," + color.A.ToString() + ")";
                    count = count + 1;
                    borderColor = new string[] { val, val, val, val };
                    backgroundcolr = new string[] { val, val, val, val };
                    var shop = shopdet.Where(m => m.ShopID == cell.ShopID).FirstOrDefault();

                    var cellid = cell.CellID;
                    cellName = shop.Shopdisplayname + " - " + cell.CelldisplayName;
                    double AvailabilityPercentage = 0;
                    double PerformancePercentage = 0;
                    double QualityPercentage = 0;
                    double OEEPercentage = 0;
                    int Actual = 0;
                    int Target = 0;

                    OEE(cellid, out AvailabilityPercentage, out PerformancePercentage, out QualityPercentage, out OEEPercentage, out Actual, out Target); // GET OEE

                    OEEModel OEEListData = new OEEModel();
                    OEEListData.CellName = cellName;
                    OEEListData.CellID = cellid;
                    OEEListData.Target = Target;
                    OEEListData.Actual = Actual;
                    double[] objdata = new double[] { AvailabilityPercentage, PerformancePercentage, QualityPercentage, OEEPercentage };

                    OEEListData.backgroundColor = backgroundcolr;
                    OEEListData.borderColor = borderColor;
                    OEEListData.data = objdata;
                    model.Add(OEEListData);

                }
                OEEOP = JsonConvert.SerializeObject(model);
                result = OEEOP;
            }
            //}

            //        }
            //    }
            //}
            //}
            return result;
        }

        private static readonly Random rand = new Random();

        private Color GetRandomColour()
        {

            return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
        // Get OEE based on Cell

        #region Previous OEE Calculations
        //public void OEE(int CellID, out double AvailabilityPercentage, out double PerformancePercentage, out double QualityPercentage, out double OEEPercentage)
        //{
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        string correctdate = GetCorrectedDate();
        //        DateTime correctedDate = Convert.ToDateTime(correctdate);
        //        int GetHour = System.DateTime.Now.Hour;
        //        DateTime StartModeTime = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd " + GetHour + ":00:00"));
        //        AvailabilityPercentage = 0;
        //        PerformancePercentage = 0;
        //        QualityPercentage = 0;
        //        OEEPercentage = 0;

        //        //var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
        //        //foreach (var row in celldet)
        //        //{
        //        decimal OperatingTime = 0;
        //        decimal LossTime = 0;
        //        decimal MinorLossTime = 0;
        //        decimal MntTime = 0;
        //        decimal SetupTime = 0;
        //        decimal SetupMinorTime = 0;
        //        decimal PowerOffTime = 0;
        //        decimal PowerONTime = 0;
        //        decimal Utilization = 0;
        //        decimal DayOEEPercent = 0;
        //        int PerformanceFactor = 0;
        //        decimal Quality = 0;
        //        int TotlaQty = 0;
        //        int YieldQty = 0;
        //        int BottleNeckYieldQty = 0;
        //        decimal IdealCycleTimeVal = 2;
        //        decimal plannedCycleTime = 0;
        //        decimal LoadingTime = 0;
        //        decimal UnloadingTime = 0;

        //        decimal LoadingUnloadingWithProd = 0;
        //        decimal TotalProductoin = 0;
        //        decimal Availability;
        //        int rejQty = 0;
        //        //  string plantName = row.tblplant.PlantName;

        //        // Get Machines
        //        var machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID /*&& m.IsBottelNeck == 1*/).OrderBy(m => m.MachineID).ToList();
        //        foreach (var machine in machineslist)
        //        {

        //            Machines machines = new Machines();
        //            int machineID = machine.MachineID;
        //            //int machineID = machineslist.MachineID;
        //            // Mode details
        //            var GetModeDurations = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1).ToList();
        //            foreach (var ModeRow in GetModeDurations)
        //            {
        //                //GetCorrectedDate = ModeRow.CorrectedDate;
        //                if (ModeRow.ModeType == "PROD")
        //                {
        //                    OperatingTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                }
        //                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec > 600)
        //                {
        //                    LossTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                    decimal LossDuration = (decimal)(ModeRow.DurationInSec / 60.00);
        //                    //if (ModeRow.LossCodeID != null)
        //                    // insertProdlosses(ProdRow.HMIID, (int)ModeRow.LossCodeID, LossDuration, CorrectedDate, MachineID);
        //                }
        //                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec < 600)
        //                {
        //                    MinorLossTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                }
        //                else if (ModeRow.ModeType == "MNT")
        //                {
        //                    MntTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                }
        //                else if (ModeRow.ModeType == "POWEROFF")
        //                {
        //                    PowerOffTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                }
        //                else if (ModeRow.ModeType == "SETUP")
        //                {
        //                    try
        //                    {
        //                        SetupTime += (decimal)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
        //                        //SetupMinorTime += (decimal)(db.tblSetupMaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60.00);
        //                    }
        //                    catch { }
        //                }
        //                else if (ModeRow.ModeType == "POWERON")
        //                {
        //                    PowerONTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                }
        //            }

        //            var GetModeDurationsRunning = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 0).ToList();
        //            foreach (var ModeRow in GetModeDurationsRunning)
        //            {
        //                String ColorCode = ModeRow.ColorCode;
        //                DateTime StartTime = (DateTime)ModeRow.StartTime;
        //                decimal Duration = (decimal)System.DateTime.Now.Subtract(StartTime).TotalMinutes;
        //                if (ColorCode == "YELLOW")
        //                {
        //                    LossTime += Duration;
        //                }
        //                else if (ColorCode == "GREEN")
        //                {
        //                    OperatingTime += Duration;
        //                }
        //                else if (ColorCode == "RED")
        //                {
        //                    MntTime += Duration;
        //                }
        //                else if (ColorCode == "BLUE")
        //                {
        //                    PowerOffTime += Duration;
        //                }
        //            }


        //            LoadingTime += (decimal)machine.StdLoadingTime;
        //            UnloadingTime += (decimal)machine.StdUnLoadingTime;


        //            plannedCycleTime += Convert.ToDecimal(machine.PlannedCycleTimeInSec);

        //            var prodplanchine = db.tblworkorderentries.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && (m.IsFinished == 1 || m.IsHold == 1) && m.tblmachinedetail.IsLastMachine == 0).ToList();
        //            foreach (var ProdRow in prodplanchine)
        //            {

        //                var scrapqty1 = db.tblrejectqties.Where(m => m.WOID == ProdRow.HMIID && m.CorrectedDate == correctdate).Select(m => m.RejectQty).ToList();

        //                foreach (int r1 in scrapqty1)
        //                {
        //                    rejQty += r1;
        //                }
        //                //TotlaQty += ProdRow.Total_Qty;
        //                //if (TotlaQty == 0)
        //                //    TotlaQty = 1;
        //                //rejQty += ProdRow.ScrapQty;
        //                //YieldQty -= rejQty;
        //                //YieldQty -= ProdRow.ScrapQty;

        //                //var IdealCycTime = db.tblparts.Where(m => m.FGCode == ProdRow.FGCode && m.OperationNo == ProdRow.OperationNo).FirstOrDefault();
        //                //if (IdealCycTime != null)
        //                //    IdealCycleTimeVal += ((IdealCycTime.IdealCycleTime));
        //                // YieldQty += ProdRow.Yield_Qty;


        //            }


        //        }
        //        TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty);
        //        if (YieldQty == 0)
        //            YieldQty = 1;
        //        LoadingUnloadingWithProd = ((LoadingTime + UnloadingTime) / 60) * YieldQty;
        //        decimal utilFactor = Math.Round((LoadingUnloadingWithProd + OperatingTime), 2);

        //        decimal IdleTime = LossTime + MinorLossTime;
        //        decimal BDTime = MntTime;

        //        int TotalTime = Convert.ToInt32(PowerONTime) + Convert.ToInt32(OperatingTime) + Convert.ToInt32(IdleTime) + Convert.ToInt32(BDTime) + Convert.ToInt32(PowerOffTime);
        //        if (TotalTime == 0)
        //        {
        //            TotalTime = 1;
        //        }
        //        if (TotlaQty == 0)
        //            TotlaQty = 1;

        //        decimal plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
        //        Availability = Math.Round((TotalTime - PowerOffTime), 2);
        //        decimal availabilityDenominator = Math.Round((plannedCycleTimeInMin + LoadingUnloadingWithProd), 2);
        //        if (availabilityDenominator == 0)
        //            availabilityDenominator = 1;
        //        TotalProductoin = Math.Round((Availability / availabilityDenominator) * 100, 2);
        //        decimal performance = Math.Round((utilFactor / TotalProductoin) * 100, 2);

        //        decimal quality = Math.Round((decimal)(YieldQty / (YieldQty + rejQty)) * 100, 2);

        //        Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(utilFactor) / Convert.ToDouble(TotalTime)) * 100));

        //        DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(performance / 100) * (double)(quality / 100), 2) * 100;

        //        AvailabilityPercentage = (double)Utilization;
        //        QualityPercentage = (double)quality;
        //        PerformancePercentage = (double)performance;
        //        OEEPercentage = (double)DayOEEPercent;

        //        #region Previous calculations
        //        //Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(OperatingTime) / Convert.ToDouble(TotalTime)) * 100));


        //        //if (plannedCycleTimeInMin == 0)
        //        //    plannedCycleTimeInMin = (decimal)0.1;
        //        //if (OperatingTime == 0)
        //        //    OperatingTime = 1;
        //        //double TotalTime1 = Convert.ToDouble(PowerONTime) + Convert.ToDouble(OperatingTime) + Convert.ToDouble(IdleTime) + Convert.ToDouble(BDTime) + Convert.ToDouble(PowerOffTime);
        //        //Quality = (decimal)Math.Round((double)YieldQty / TotlaQty * 100, 2);
        //        ////decimal Performance = Math.Round( (((1 - (OperatingTime - plannedCycleTimeInMin)) / plannedCycleTimeInMin)) * 100,2);

        //        //decimal Performance = (decimal)Math.Round((((double)plannedCycleTimeInMin * (double)BottleNeckYieldQty) / (double)OperatingTime) * 100, 2);
        //        //PerformanceFactor = (int)IdealCycleTimeVal * BottleNeckYieldQty;
        //        //if (Performance > 100 || Performance == 0)
        //        //{
        //        //    Performance = 100;
        //        //}
        //        //if (Quality == 0 || Quality > 100)
        //        //{
        //        //    Quality = 100;
        //        //}
        //        //DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(Performance / 100) * (double)(Quality / 100), 2) * 100;

        //        //AvailabilityPercentage = (double)Utilization;
        //        //QualityPercentage = (double)Quality;
        //        //PerformancePercentage = (double)Performance;
        //        //OEEPercentage = (double)DayOEEPercent;
        //        #endregion
        //        //}
        //    }
        //}

        #endregion

        public void OEE(int CellID, out double AvailabilityPercentage, out double PerformancePercentage, out double QualityPercentage, out double OEEPercentage, out int Actual, out int Target)
        {
            string correctdate = GetCorrectedDate();
            DateTime correctedDate = Convert.ToDateTime(correctdate);
            decimal OperatingTime = 0;
            decimal LossTime = 0;
            decimal MinorLossTime = 0;
            decimal MntTime = 0;
            decimal SetupTime = 0;
            Actual = 0;
            Target = 0;
            //decimal SetupMinorTime = 0;
            decimal PowerOffTime = 0;
            decimal PowerONTime = 0;
            //decimal Utilization = 0;
            decimal DayOEEPercent = 0;
            //int PerformanceFactor = 0;
            //decimal Quality = 0;
            int TotlaQty = 0;
            int YieldQty = 0;
            int BottleNeckYieldQty = 0;
            //decimal IdealCycleTimeVal = 2;
            decimal plannedCycleTime = 0;
            decimal LoadingTime = 0;
            decimal UnloadingTime = 0;

            double plannedBrkDurationinMin = 0;
            decimal LoadingUnloadingWithProd = 0;
            decimal LoadingUnloadingwithProdBottleNeck = 0;
            int minorstoppage = 0;
            //decimal TotalProductoin = 0;
            decimal Availability;
            int rejQty = 0;
            int reject = 0;
            //  string plantName = row.tblplant.PlantName;
            var machineslist = new List<tblmachinedetail>();
            var bottleneckmachines = new tblbottelneck();
            var scrap = new tblworkorderentry();
            var scrapqty1 = new List<tblrejectqty>();
            var cellpartDet = new tblcellpart();
            var partsDet = new tblpart();

            decimal OPwithMinorStoppage = 0;
            int TotalTime = 0;
            decimal plannedCycleTimeInMin = 0;
            decimal PlanedwithActual = 0;
            int Actualandrej = 0;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                scrap = db.tblworkorderentries.Where(m => m.CellID == CellID && m.CorrectedDate == correctdate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
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
                    string Operationnum = bottleneckmachines.tblmachinedetail.OperationNumber.ToString();
                    partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

                }


            }
            var celldet = new tblcell();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                celldet = db.tblcells.Find(CellID);
            }
            if (celldet.Nobottleneck == 1)
            {
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.tblcell.Nobottleneck == 1).ToList();

                }
            }
            else
            {
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    // Get Machines               
                    machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.MachineID == bottleneckmachines.MachineID).OrderBy(m => m.MachineID).ToList();
                }
            }



            foreach (var machine in machineslist)
            {
                Machines machines = new Machines();
                int machineID = machine.MachineID;
                // Mode details
                minorstoppage = Convert.ToInt32(machine.MachineIdleMin) * 60; // in sec
                var GetModeDurations = new List<tbllivemode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    GetModeDurations = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 1).ToList();
                }
                OperatingTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "PROD").ToList().Sum(m => m.DurationInSec));
                PowerOffTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWEROFF").ToList().Sum(m => m.DurationInSec));
                MntTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "MNT").ToList().Sum(m => m.DurationInSec));
                MinorLossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec < minorstoppage).ToList().Sum(m => m.DurationInSec));
                LossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec > minorstoppage).ToList().Sum(m => m.DurationInSec));
                PowerONTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWERON").ToList().Sum(m => m.DurationInSec));
                OperatingTime = Math.Round((OperatingTime / 60), 2);
                PowerOffTime = (PowerOffTime / 60);
                MntTime = (MntTime / 60);
                MinorLossTime = (MinorLossTime / 60);
                LossTime = (LossTime / 60);
                PowerONTime = (PowerONTime / 60);
                var plannedbrks = db.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
                foreach (var row in plannedbrks)
                {
                    plannedBrkDurationinMin += Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalMinutes;
                }
                foreach (var ModeRow in GetModeDurations)
                {
                    if (ModeRow.ModeType == "SETUP")
                    {
                        try
                        {
                            SetupTime += (decimal)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
                            //SetupMinorTime += (decimal)(db.tblSetupMaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60.00);
                        }
                        catch { }
                    }
                }
                var GetModeDurationsRunning = new List<tbllivemode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    GetModeDurationsRunning = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 0).ToList();
                }
                foreach (var ModeRow in GetModeDurationsRunning)
                {
                    String ColorCode = ModeRow.ColorCode;
                    DateTime StartTime = (DateTime)ModeRow.StartTime;
                    decimal Duration = (decimal)System.DateTime.Now.Subtract(StartTime).TotalMinutes;
                    if (ColorCode == "YELLOW")
                    {
                        LossTime += Duration;
                    }
                    else if (ColorCode == "GREEN")
                    {
                        OperatingTime += Duration;
                    }
                    else if (ColorCode == "RED")
                    {
                        MntTime += Duration;
                    }
                    else if (ColorCode == "BLUE")
                    {
                        PowerOffTime += Duration;
                    }
                }
                LoadingTime = (decimal)partsDet.StdLoadingTime;
                UnloadingTime = (decimal)partsDet.StdUnLoadingTime;

                //using (i_facilityEntities1 db = new i_facilityEntities1())
                //{
                //    scrap = db.tblworkorderentries.Where(m => m.MachineID == machine.MachineID && m.tblmachinedetail.IsLastMachine == 1).FirstOrDefault();
                //    string operationnum =Convert.ToString( machine.OperationNumber);
                //    partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == bottleneckmachines.PartNo && m.OperationNo == operationnum).FirstOrDefault();
                //}
                if (scrap != null)
                {
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        scrapqty1 = db.tblrejectqties.Where(m => m.WOID == scrap.HMIID && m.CorrectedDate == correctdate).ToList();
                    }

                    foreach (var r1 in scrapqty1)
                    {
                        reject = reject + Convert.ToInt32(r1.RejectQty);
                    }

                }
                plannedCycleTime = Convert.ToDecimal(partsDet.IdealCycleTime);


                int bottleneckMachineID = machineID;
                if (celldet.Nobottleneck == 0)
                {
                    TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty, bottleneckMachineID);
                }
                else
                {
                    TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, bottleneckMachineID);
                }
               
                Actual += YieldQty;
                if (YieldQty == 0)
                    YieldQty = 1;
                LoadingUnloadingWithProd += ((LoadingTime + UnloadingTime) * YieldQty) / 60;
                LoadingUnloadingwithProdBottleNeck = ((LoadingTime + UnloadingTime) * BottleNeckYieldQty) / 60;
                MinorLossTime = MinorLossTime - LoadingUnloadingWithProd;
                OPwithMinorStoppage += (OperatingTime + LoadingUnloadingWithProd + MinorLossTime);
                TotalTime += Convert.ToInt32(PowerONTime) + Convert.ToInt32(OperatingTime) + Convert.ToInt32(LossTime) + Convert.ToInt32(MntTime) + Convert.ToInt32(PowerOffTime);
                plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
                var StdCycleTimeinMin = Convert.ToDecimal(plannedCycleTimeInMin);
                var LoadunloadTimeinMin = ((int)LoadingTime + (int)UnloadingTime) / 60;
                if (StdCycleTimeinMin < 1)
                    StdCycleTimeinMin = 1;
                var Targetdec = ((decimal)TotalTime / (StdCycleTimeinMin + LoadunloadTimeinMin));
                Target += Convert.ToInt32(Targetdec);
                PlanedwithActual +=(plannedCycleTimeInMin*YieldQty);
                Actualandrej += YieldQty - reject;
            }
            //int bottleneckMachineID = bottleneckmachines.MachineID;
            //TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty, bottleneckMachineID);
            //Actual = YieldQty;
            //if (YieldQty == 0)
            //    YieldQty = 1;
            //LoadingUnloadingWithProd = ((LoadingTime + UnloadingTime) * YieldQty) / 60;
            //LoadingUnloadingwithProdBottleNeck = ((LoadingTime + UnloadingTime) * BottleNeckYieldQty) / 60;
            //MinorLossTime = MinorLossTime - LoadingUnloadingWithProd;
            //decimal OPwithMinorStoppage = (OperatingTime + LoadingUnloadingWithProd + MinorLossTime);
            decimal utilFactor = Math.Round((LoadingUnloadingWithProd + OperatingTime), 2);
            decimal IdleTime = LossTime;
            decimal BDTime = MntTime;
            //TotalTime = Convert.ToInt32(PowerONTime) + Convert.ToInt32(OperatingTime) + Convert.ToInt32(IdleTime) + Convert.ToInt32(BDTime) + Convert.ToInt32(PowerOffTime);
            //int TotalTime = 24 * 60;

            if (TotalTime == 0)
            {
                TotalTime = 1;
            }
            if (TotlaQty == 0)
                TotlaQty = 1;
            //decimal plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
            //var StdCycleTimeinMin = Convert.ToDecimal(plannedCycleTimeInMin);
            //var LoadunloadTimeinMin = ((int)LoadingTime + (int)UnloadingTime) / 60;
            //if (StdCycleTimeinMin < 1)
            //    StdCycleTimeinMin = 1;
            //var Targetdec = ((decimal)TotalTime / (StdCycleTimeinMin + LoadunloadTimeinMin));
            //Target = Convert.ToInt32(Targetdec);
            if (TotalTime > 360)
                Availability = Math.Round((TotalTime - (decimal)plannedBrkDurationinMin), 2);
            else
                Availability = TotalTime;
            if (OPwithMinorStoppage == 0)
                OPwithMinorStoppage = 1;
            decimal TotalTimeWithPlannedBrk = Availability;
            decimal AvailabilityPercent = Math.Round((OPwithMinorStoppage / TotalTimeWithPlannedBrk), 2) * 100;  // From BottleNeckMachine
            if (AvailabilityPercent > 100)
                AvailabilityPercent = 100;
            decimal PerformanceBottelNeck = 0;
            decimal performanceFactor = 0;
            decimal QualityLastMachine = 0;
            if (celldet.Nobottleneck == 1)
            {
                PerformanceBottelNeck = Math.Round(((PlanedwithActual) / OPwithMinorStoppage), 2) * 100;
                performanceFactor = PlanedwithActual;
                QualityLastMachine = Math.Round((decimal)((Actualandrej) / Actual), 2) * 100;
            }
            else
            {
                PerformanceBottelNeck = Math.Round(((plannedCycleTimeInMin * YieldQty) / OPwithMinorStoppage), 2) * 100;
                performanceFactor = (plannedCycleTime * YieldQty);
                QualityLastMachine = Math.Round((decimal)((YieldQty - reject) / YieldQty), 2) * 100;            // From LastMachine
            }
            DayOEEPercent = (decimal)Math.Round((double)(AvailabilityPercent / 100) * (double)(PerformanceBottelNeck / 100) * (double)(QualityLastMachine / 100), 2) * 100;
            //decimal availabilityDenominator = Math.Round((plannedCycleTimeInMin + LoadingUnloadingWithProd), 2);

            //TotalProductoin = Math.Round((Availability / availabilityDenominator) * 100, 2);
            //decimal performance = Math.Round((utilFactor / TotalProductoin) * 100, 2);
            //decimal performanceFactor = Math.Round((utilFactor));

            //decimal quality = Math.Round((decimal)(YieldQty / (YieldQty + rejQty)) * 100, 2);

            //Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(utilFactor) / Convert.ToDouble(TotalTime)) * 100));

            //DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(performance / 100) * (double)(quality / 100), 2) * 100;
            if (AvailabilityPercent == 0)
            {
                QualityLastMachine = 0;
                PerformanceBottelNeck = 0;
                DayOEEPercent = 0;
            }
            AvailabilityPercentage = (double)AvailabilityPercent;
            QualityPercentage = (double)QualityLastMachine;
            PerformancePercentage = (double)PerformanceBottelNeck;
            OEEPercentage = (double)DayOEEPercent;
        }

        #region Previous 2019-06-11
        //public void OEE(int CellID, out double AvailabilityPercentage, out double PerformancePercentage, out double QualityPercentage, out double OEEPercentage, out int Actual, out int Target)
        //{
        //    string correctdate = GetCorrectedDate();
        //    DateTime correctedDate = Convert.ToDateTime(correctdate);
        //    decimal OperatingTime = 0;
        //    decimal LossTime = 0;
        //    decimal MinorLossTime = 0;
        //    decimal MntTime = 0;
        //    decimal SetupTime = 0;
        //    Actual = 0;
        //    Target = 0;
        //    //decimal SetupMinorTime = 0;
        //    decimal PowerOffTime = 0;
        //    decimal PowerONTime = 0;
        //    //decimal Utilization = 0;
        //    decimal DayOEEPercent = 0;
        //    //int PerformanceFactor = 0;
        //    //decimal Quality = 0;
        //    int TotlaQty = 0;
        //    int YieldQty = 0;
        //    int BottleNeckYieldQty = 0;
        //    //decimal IdealCycleTimeVal = 2;
        //    decimal plannedCycleTime = 0;
        //    decimal LoadingTime = 0;
        //    decimal UnloadingTime = 0;

        //    double plannedBrkDurationinMin = 0;
        //    decimal LoadingUnloadingWithProd = 0;
        //    decimal LoadingUnloadingwithProdBottleNeck = 0;
        //    int minorstoppage = 0;
        //    //decimal TotalProductoin = 0;
        //    decimal Availability;
        //    int rejQty = 0;
        //    int reject = 0;
        //    //  string plantName = row.tblplant.PlantName;
        //    var machineslist = new List<tblmachinedetail>();
        //    var bottleneckmachines = new tblbottelneck();
        //    var scrap = new tblworkorderentry();
        //    var scrapqty1 = new List<tblrejectqty>();
        //    var cellpartDet = new tblcellpart();
        //    var partsDet = new tblpart();
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        scrap = db.tblworkorderentries.Where(m => m.CellID == CellID && m.CorrectedDate == correctdate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
        //        if (scrap != null)
        //        {
        //            partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == scrap.PartNo).FirstOrDefault();
        //            if (partsDet != null)
        //                bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == partsDet.FGCode && m.CellID == scrap.CellID).FirstOrDefault();
        //        }
        //        else
        //        {
        //            cellpartDet = db.tblcellparts.Where(m => m.CellID == CellID && m.IsDefault == 1 && m.IsDeleted == 0).FirstOrDefault();
        //            if (cellpartDet != null)
        //                bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == cellpartDet.partNo && m.CellID == cellpartDet.CellID).FirstOrDefault();
        //            string Operationnum = bottleneckmachines.tblmachinedetail.OperationNumber.ToString();
        //            partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

        //        }


        //    }
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        // Get Machines               
        //        machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.MachineID == bottleneckmachines.MachineID).OrderBy(m => m.MachineID).ToList();
        //    }
        //    foreach (var machine in machineslist)
        //    {
        //        Machines machines = new Machines();
        //        int machineID = machine.MachineID;
        //        // Mode details
        //        minorstoppage = Convert.ToInt32(machine.MachineIdleMin) * 60; // in sec
        //        var GetModeDurations = new List<tbllivemode>();
        //        using (i_facilityEntities1 db = new i_facilityEntities1())
        //        {
        //            GetModeDurations = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 1).ToList();
        //        }
        //        OperatingTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "PROD").ToList().Sum(m => m.DurationInSec));
        //        PowerOffTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWEROFF").ToList().Sum(m => m.DurationInSec));
        //        MntTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "MNT").ToList().Sum(m => m.DurationInSec));
        //        MinorLossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec < minorstoppage).ToList().Sum(m => m.DurationInSec));
        //        LossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec > minorstoppage).ToList().Sum(m => m.DurationInSec));
        //        PowerONTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWERON").ToList().Sum(m => m.DurationInSec));
        //        OperatingTime = Math.Round((OperatingTime / 60), 2);
        //        PowerOffTime = (PowerOffTime / 60);
        //        MntTime = (MntTime / 60);
        //        MinorLossTime = (MinorLossTime / 60);
        //        LossTime = (LossTime / 60);
        //        PowerONTime = (PowerONTime / 60);
        //        var plannedbrks = db.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
        //        foreach (var row in plannedbrks)
        //        {
        //            plannedBrkDurationinMin += Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalMinutes;
        //        }
        //        foreach (var ModeRow in GetModeDurations)
        //        {
        //            if (ModeRow.ModeType == "SETUP")
        //            {
        //                try
        //                {
        //                    SetupTime += (decimal)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
        //                    //SetupMinorTime += (decimal)(db.tblSetupMaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60.00);
        //                }
        //                catch { }
        //            }
        //        }
        //        var GetModeDurationsRunning = new List<tbllivemode>();
        //        using (i_facilityEntities1 db = new i_facilityEntities1())
        //        {
        //            GetModeDurationsRunning = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 0).ToList();
        //        }
        //        foreach (var ModeRow in GetModeDurationsRunning)
        //        {
        //            String ColorCode = ModeRow.ColorCode;
        //            DateTime StartTime = (DateTime)ModeRow.StartTime;
        //            decimal Duration = (decimal)System.DateTime.Now.Subtract(StartTime).TotalMinutes;
        //            if (ColorCode == "YELLOW")
        //            {
        //                LossTime += Duration;
        //            }
        //            else if (ColorCode == "GREEN")
        //            {
        //                OperatingTime += Duration;
        //            }
        //            else if (ColorCode == "RED")
        //            {
        //                MntTime += Duration;
        //            }
        //            else if (ColorCode == "BLUE")
        //            {
        //                PowerOffTime += Duration;
        //            }
        //        }
        //        LoadingTime += (decimal)partsDet.StdLoadingTime;
        //        UnloadingTime += (decimal)partsDet.StdUnLoadingTime;

        //        //using (i_facilityEntities1 db = new i_facilityEntities1())
        //        //{
        //        //    scrap = db.tblworkorderentries.Where(m => m.MachineID == machine.MachineID && m.tblmachinedetail.IsLastMachine == 1).FirstOrDefault();
        //        //    string operationnum =Convert.ToString( machine.OperationNumber);
        //        //    partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == bottleneckmachines.PartNo && m.OperationNo == operationnum).FirstOrDefault();
        //        //}
        //        if (scrap != null)
        //        {
        //            using (i_facilityEntities1 db = new i_facilityEntities1())
        //            {
        //                scrapqty1 = db.tblrejectqties.Where(m => m.WOID == scrap.HMIID && m.CorrectedDate == correctdate).ToList();
        //            }

        //            foreach (var r1 in scrapqty1)
        //            {
        //                reject = reject + Convert.ToInt32(r1.RejectQty);
        //            }

        //        }
        //        plannedCycleTime = Convert.ToDecimal(partsDet.IdealCycleTime);
        //    }
        //    int bottleneckMachineID = bottleneckmachines.MachineID;
        //    TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty, bottleneckMachineID);
        //    Actual = YieldQty;
        //    if (YieldQty == 0)
        //        YieldQty = 1;
        //    LoadingUnloadingWithProd = ((LoadingTime + UnloadingTime) * YieldQty) / 60;
        //    LoadingUnloadingwithProdBottleNeck = ((LoadingTime + UnloadingTime) * BottleNeckYieldQty) / 60;
        //    MinorLossTime = MinorLossTime - LoadingUnloadingWithProd;
        //    decimal OPwithMinorStoppage = (OperatingTime + LoadingUnloadingWithProd + MinorLossTime);
        //    decimal utilFactor = Math.Round((LoadingUnloadingWithProd + OperatingTime), 2);
        //    decimal IdleTime = LossTime;
        //    decimal BDTime = MntTime;
        //    int TotalTime = Convert.ToInt32(PowerONTime) + Convert.ToInt32(OperatingTime) + Convert.ToInt32(IdleTime) + Convert.ToInt32(BDTime) + Convert.ToInt32(PowerOffTime);
        //    //int TotalTime = 24 * 60;

        //    if (TotalTime == 0)
        //    {
        //        TotalTime = 1;
        //    }
        //    if (TotlaQty == 0)
        //        TotlaQty = 1;
        //    decimal plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
        //    var StdCycleTimeinMin = Convert.ToDecimal(plannedCycleTimeInMin);
        //    var LoadunloadTimeinMin = ((int)LoadingTime + (int)UnloadingTime) / 60;
        //    if (StdCycleTimeinMin < 1)
        //        StdCycleTimeinMin = 1;
        //    var Targetdec = ((decimal)TotalTime / (StdCycleTimeinMin + LoadunloadTimeinMin));
        //    Target = Convert.ToInt32(Targetdec);
        //    if (TotalTime > 360)
        //        Availability = Math.Round((TotalTime - (decimal)plannedBrkDurationinMin), 2);
        //    else
        //        Availability = TotalTime;
        //    if (OPwithMinorStoppage == 0)
        //        OPwithMinorStoppage = 1;
        //    decimal TotalTimeWithPlannedBrk = Availability;
        //    decimal AvailabilityPercent = Math.Round((OPwithMinorStoppage / TotalTimeWithPlannedBrk), 2) * 100;  // From BottleNeckMachine
        //    if (AvailabilityPercent > 100)
        //        AvailabilityPercent = 100;
        //    decimal PerformanceBottelNeck = Math.Round(((plannedCycleTimeInMin * YieldQty) / OPwithMinorStoppage), 2) * 100;
        //    decimal performanceFactor = (plannedCycleTime * YieldQty);
        //    decimal QualityLastMachine = Math.Round((decimal)((YieldQty - reject) / YieldQty), 2) * 100;            // From LastMachine
        //    DayOEEPercent = (decimal)Math.Round((double)(AvailabilityPercent / 100) * (double)(PerformanceBottelNeck / 100) * (double)(QualityLastMachine / 100), 2) * 100;
        //    //decimal availabilityDenominator = Math.Round((plannedCycleTimeInMin + LoadingUnloadingWithProd), 2);

        //    //TotalProductoin = Math.Round((Availability / availabilityDenominator) * 100, 2);
        //    //decimal performance = Math.Round((utilFactor / TotalProductoin) * 100, 2);
        //    //decimal performanceFactor = Math.Round((utilFactor));

        //    //decimal quality = Math.Round((decimal)(YieldQty / (YieldQty + rejQty)) * 100, 2);

        //    //Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(utilFactor) / Convert.ToDouble(TotalTime)) * 100));

        //    //DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(performance / 100) * (double)(quality / 100), 2) * 100;
        //    if(AvailabilityPercent == 0)
        //    {
        //        QualityLastMachine = 0;
        //        PerformanceBottelNeck = 0;
        //        DayOEEPercent = 0;
        //    }
        //    AvailabilityPercentage = (double)AvailabilityPercent;
        //    QualityPercentage = (double)QualityLastMachine;
        //    PerformancePercentage = (double)PerformanceBottelNeck;
        //    OEEPercentage = (double)DayOEEPercent;
        //}

        //private int GetQuantiy(int CellID, DateTime CorrectedDate, out int YieldQty, out int BottleNeckYieldQty, int bottlneckMachineID/*, out int BottleNeckTotalQty*/)
        //{
        //    int TotalQty = 0;
        //    var machineDet = new List<tblmachinedetail>();
        //    var starttime = new tbldaytiming();
        //    var parametermasterlistAll = new List<parameters_master>();
        //    var parametermasterlist = new List<parameters_master>();
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID).ToList();
        //    }
        //    YieldQty = 0;
        //    //BottleNeckTotalQty = 0;
        //    BottleNeckYieldQty = 0;
        //    string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
        //    string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
        //    var bottleneckmachine = machineDet.Where(m => m.IsBottelNeck == 1).OrderBy(m => m.MachineID).FirstOrDefault();
        //    var lastmachine = machineDet.Where(m => m.IsLastMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
        //    var firtstmachine = machineDet.Where(m => m.IsFirstMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
        //    int firstmachineId = 0;
        //    int lstmachineId = 0;
        //    int bottleneckMachineID = 0;
        //    if (firtstmachine != null)
        //        firstmachineId = firtstmachine.MachineID;
        //    if (lastmachine != null)
        //        lstmachineId = lastmachine.MachineID;
        //    if (bottleneckmachine != null)
        //        bottleneckMachineID = bottleneckmachine.MachineID;
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        starttime = db.tbldaytimings.Where(m => m.IsDeleted == 0).FirstOrDefault(); //.Select(m => m.StartTime)
        //    }

        //    string StartTime = Correcteddate + " 07:15:00";
        //    string EndTime = NxtCorrecteddate + " 07:15:00";

        //    DateTime St = Convert.ToDateTime(StartTime);
        //    DateTime Et = Convert.ToDateTime(EndTime);

        //    // Based on 1st Machine
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        parametermasterlistAll = db.parameters_master.Where(m => m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
        //    }
        //    parametermasterlist = parametermasterlistAll.Where(m => m.MachineID == firstmachineId && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
        //    var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
        //    var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();


        //    // Based on Last Machine
        //    var parametermasterlistLast = parametermasterlistAll.Where(m => m.MachineID == lstmachineId && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
        //    var TopRowLast = parametermasterlistLast.OrderByDescending(m => m.ParameterID).FirstOrDefault();
        //    var RowLast = parametermasterlistLast.OrderBy(m => m.ParameterID).FirstOrDefault();

        //    // Based on Last Machine
        //    var parametermasterlistBottleNeck = parametermasterlistAll.Where(m => m.MachineID == bottlneckMachineID && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
        //    var TopRowBottleNeck = parametermasterlistBottleNeck.OrderByDescending(m => m.ParameterID).FirstOrDefault();
        //    var RowLastBottleNeck = parametermasterlistBottleNeck.OrderBy(m => m.ParameterID).FirstOrDefault();

        //    var celldet = new tblcell();

        //    var machineids = new List<int>();
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        machineids = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.tblcell.Nobottleneck == 1).Select(m => m.MachineID).ToList(); //m.IsBottelNeck == 1
        //        celldet = db.tblcells.Find(CellID);
        //    }

        //    if (TopRowLast != null && RowLast != null)
        //        YieldQty = Convert.ToInt32(TopRowLast.PartsTotal - RowLast.PartsTotal);

        //    if (TopRow != null && LastRow != null)
        //        TotalQty = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);

        //    if (TopRowBottleNeck != null && RowLastBottleNeck != null)
        //        BottleNeckYieldQty = Convert.ToInt32(TopRowBottleNeck.PartsTotal - RowLastBottleNeck.PartsTotal);
        //    //added by ashok
        //    #region New code
        //    //if (celldet.Nobottleneck == 0)
        //    //{
        //    //    if (TopRowLast != null && RowLast != null)
        //    //        YieldQty = Convert.ToInt32(TopRowLast.PartsTotal - RowLast.PartsTotal);

        //    //    if (TopRow != null && LastRow != null)
        //    //        TotalQty = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);

        //    //    if (TopRowBottleNeck != null && RowLastBottleNeck != null)
        //    //        BottleNeckYieldQty = Convert.ToInt32(TopRowBottleNeck.PartsTotal - RowLastBottleNeck.PartsTotal);
        //    //}
        //    //else
        //    //{
        //    //    foreach(var machine in machineids)
        //    //    {
        //    //        parametermasterlist = parametermasterlistAll.Where(m => m.MachineID == machine && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
        //    //        var Toprow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
        //    //        var lastrow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();
        //    //        YieldQty += Convert.ToInt32(Toprow.PartsTotal - lastrow.PartsTotal);
        //    //    }
        //    //}
        //    #endregion
        //    //}
        //    return TotalQty;

        //}
        #endregion
        private int GetQuantiy(int CellID, DateTime CorrectedDate, out int YieldQty, out int BottleNeckYieldQty, int bottlneckMachineID)
        {
            int TotalQty = 0;
            var machineDet = new List<tblmachinedetail>();
            var starttime = new tbldaytiming();
            var parametermasterlistAll = new List<parameters_master>();
            var parametermasterlist = new List<parameters_master>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID).ToList();
            }
            YieldQty = 0;
            //BottleNeckTotalQty = 0;
            BottleNeckYieldQty = 0;
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
            var bottleneckmachine = machineDet.Where(m => m.IsBottelNeck == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            var lastmachine = machineDet.Where(m => m.IsLastMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            var firtstmachine = machineDet.Where(m => m.IsFirstMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            int firstmachineId = 0;
            int lstmachineId = 0;
            int bottleneckMachineID = 0;
            if (firtstmachine != null)
                firstmachineId = firtstmachine.MachineID;
            if (lastmachine != null)
                lstmachineId = lastmachine.MachineID;
            if (bottleneckmachine != null)
                bottleneckMachineID = bottleneckmachine.MachineID;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                starttime = db.tbldaytimings.Where(m => m.IsDeleted == 0).FirstOrDefault(); //.Select(m => m.StartTime)
            }

            string StartTime = Correcteddate + " 07:15:00";
            string EndTime = NxtCorrecteddate + " 07:15:00";

            DateTime St = Convert.ToDateTime(StartTime);
            DateTime Et = Convert.ToDateTime(EndTime);

            // Based on 1st Machine
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                parametermasterlistAll = db.parameters_master.Where(m => m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            }
            parametermasterlist = parametermasterlistAll.Where(m => m.MachineID == firstmachineId && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();


            // Based on Last Machine
            var parametermasterlistLast = parametermasterlistAll.Where(m => m.MachineID == lstmachineId && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            var TopRowLast = parametermasterlistLast.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var RowLast = parametermasterlistLast.OrderBy(m => m.ParameterID).FirstOrDefault();

            // Based on Last Machine
            var parametermasterlistBottleNeck = parametermasterlistAll.Where(m => m.MachineID == bottlneckMachineID && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            var TopRowBottleNeck = parametermasterlistBottleNeck.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var RowLastBottleNeck = parametermasterlistBottleNeck.OrderBy(m => m.ParameterID).FirstOrDefault();

            var celldet = new tblcell();

            var machineids = new List<int>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                machineids = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.tblcell.Nobottleneck == 1).Select(m => m.MachineID).ToList(); //m.IsBottelNeck == 1
                celldet = db.tblcells.Find(CellID);
            }

            if (TopRowLast != null && RowLast != null)
                YieldQty = Convert.ToInt32(TopRowLast.PartsTotal - RowLast.PartsTotal);

            if (TopRow != null && LastRow != null)
                TotalQty = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);

            if (TopRowBottleNeck != null && RowLastBottleNeck != null)
                BottleNeckYieldQty = Convert.ToInt32(TopRowBottleNeck.PartsTotal - RowLastBottleNeck.PartsTotal);
            //added by ashok
            #region New code
            //if (celldet.Nobottleneck == 0)
            //{
            //    if (TopRowLast != null && RowLast != null)
            //        YieldQty = Convert.ToInt32(TopRowLast.PartsTotal - RowLast.PartsTotal);

            //    if (TopRow != null && LastRow != null)
            //        TotalQty = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);

            //    if (TopRowBottleNeck != null && RowLastBottleNeck != null)
            //        BottleNeckYieldQty = Convert.ToInt32(TopRowBottleNeck.PartsTotal - RowLastBottleNeck.PartsTotal);
            //}
            //else
            //{
            //    foreach (var machine in machineids)
            //    {
            //        parametermasterlist = parametermasterlistAll.Where(m => m.MachineID == machine && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            //        var Toprow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            //        var lastrow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();
            //        YieldQty += Convert.ToInt32(Toprow.PartsTotal - lastrow.PartsTotal);
            //    }
            //}
            #endregion
            //}
            return TotalQty;

        }

        private int GetQuantiy(int CellID, DateTime CorrectedDate, out int YieldQty, int nobottleneckmachinedid)
        {
            int TotalQty = 0;
            var machineDet = new List<tblmachinedetail>();
            var starttime = new tbldaytiming();
            var parametermasterlistAll = new List<parameters_master>();
            var parametermasterlist = new List<parameters_master>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID).ToList();
            }
            YieldQty = 0;
            //BottleNeckTotalQty = 0;
            
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
            //var bottleneckmachine = machineDet.Where(m => m.IsBottelNeck == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            //var lastmachine = machineDet.Where(m => m.IsLastMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            //var firtstmachine = machineDet.Where(m => m.IsFirstMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            //int firstmachineId = 0;
            //int lstmachineId = 0;
            //int bottleneckMachineID = 0;
            //if (firtstmachine != null)
            //    firstmachineId = firtstmachine.MachineID;
            //if (lastmachine != null)
            //    lstmachineId = lastmachine.MachineID;
            //if (bottleneckmachine != null)
            //    bottleneckMachineID = bottleneckmachine.MachineID;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                starttime = db.tbldaytimings.Where(m => m.IsDeleted == 0).FirstOrDefault(); //.Select(m => m.StartTime)
            }

            string StartTime = Correcteddate + " 07:15:00";
            string EndTime = NxtCorrecteddate + " 07:15:00";

            DateTime St = Convert.ToDateTime(StartTime);
            DateTime Et = Convert.ToDateTime(EndTime);

            // Based on 1st Machine
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                parametermasterlistAll = db.parameters_master.Where(m => m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            }
            parametermasterlist = parametermasterlistAll.Where(m => m.MachineID == nobottleneckmachinedid && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();


            if (TopRow != null && LastRow != null)
                YieldQty = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);

           
            //}
            return TotalQty;

        }




        private int GetParts_Cutting(int MachineID, DateTime CorrectedDate, out int TotalPartsCount)
        {
            int CuttingTime = 0;
            TotalPartsCount = 0;
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
            string StartTime = Correcteddate + " 07:15:00";
            string EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime St = Convert.ToDateTime(StartTime);
            DateTime Et = Convert.ToDateTime(EndTime);
            var parametermasterlist = new List<parameters_master>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                parametermasterlist = db.parameters_master.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            }
            var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();
            if (TopRow != null && LastRow != null)
            {
                CuttingTime = Convert.ToInt32(TopRow.CuttingTime) - Convert.ToInt32(LastRow.CuttingTime);
                TotalPartsCount = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);
            }
            return CuttingTime;
        }

        //public string TargetActuals()
        //{

        //    DateTime nowdate = DateTime.Now;
        //    string chartop = "";
        //    string actualtargetresult = "";
        //    //string correctedDate = "2019-01-27";
        //    string correctedDate = GetCorrectedDate();

        //    DateTime correctdate = Convert.ToDateTime(correctedDate);
        //    List<TargetActualList> TargetActualList = new List<TargetActualList>();

        //    var plantDetails = db.tblplants.Where(p => p.IsDeleted == 0).ToList();
        //    if (plantDetails.Count > 0)
        //    {
        //        foreach (var plant in plantDetails)
        //        {
        //            var plantid = plant.PlantID;
        //            var plantname = plant.PlantName;
        //            var shopDetails = db.tblshops.Where(s => s.IsDeleted == 0 && s.PlantID == plantid).ToList();
        //            if (shopDetails.Count > 0)
        //            {
        //                foreach (var shop in shopDetails)
        //                {
        //                    var shopId = shop.ShopID;
        //                    var cellDetails = db.tblcells.Where(c => c.IsDeleted == 0 && c.ShopID == shopId && c.PlantID == plantid).ToList();
        //                    if (cellDetails != null)
        //                    {
        //                        foreach (var cell in cellDetails)
        //                        {
        //                            var cellid = cell.CellID;
        //                            var machineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid && m.ShopID == shopId && m.PlantID == plantid).OrderByDescending(m => m.InsertedOn).ToList();
        //                            if (machineDetails != null)
        //                            {
        //                                foreach (var machine in machineDetails)
        //                                {
        //                                    var machineId = machine.MachineID;
        //                                    var machineName = machine.MachineDisplayName;
        //                                    var partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == machineId && m.CorrectedDate == correctdate && m.Isdeleted == 0).Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime }).OrderBy(m => m.StartTime).FirstOrDefault();
        //                                    if (partDetails != null)
        //                                    {
        //                                        TargetActualList TAL = new TargetActualList();
        //                                        TAL.Actual = partDetails.PartCount;
        //                                        TAL.Target = partDetails.TargetQuantity;
        //                                        TAL.MachineName = machineName;
        //                                        TargetActualList.Add(TAL);
        //                                    }
        //                                    chartop = JsonConvert.SerializeObject(TargetActualList);
        //                                    actualtargetresult = chartop;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    return actualtargetresult;
        //}

        #region Commented
        //public string ContributingFactor()
        //{
        //    string res = "";
        //    //string correctedDate = nowdate.ToString("yyyy-MM-dd");
        //    string correctedDate = DateTime.Now.ToString("yyyy-MM-dd");
        //    string correctdate = correctedDate;
        //    List<ContributingFactors> ContributingFactorsList = new List<ContributingFactors>();
        //    var shopDetails = db.tblshops.Where(s => s.IsDeleted == 0).ToList();

        //    if (shopDetails != null)
        //    {
        //        foreach (var shop in shopDetails)
        //        {
        //            var shopId = shop.ShopID;
        //            var cellDetails = db.tblcells.Where(c => c.IsDeleted == 0 && c.ShopID == shopId).ToList();
        //            if (cellDetails != null)
        //            {
        //                foreach (var cell in cellDetails)
        //                {
        //                    var cellid = cell.CellID;
        //                    var machineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid && m.ShopID == shopId).OrderByDescending(m => m.InsertedOn).ToList();
        //                    if (machineDetails.Count > 0)
        //                    {
        //                        foreach (var machine in machineDetails)
        //                        {
        //                            var duration = "";
        //                            int lossmessageId = 0;
        //                            var machineId = machine.MachineID;
        //                            var factor = "";
        //                            TimeSpan TotalTimeTaken = new TimeSpan();
        //                            var lossData = db.tbllossofentries.Where(l => l.MachineID == machineId && l.CorrectedDate == correctdate).Select(l => new { l.MessageCodeID, l.StartDateTime, l.EndDateTime }).FirstOrDefault();
        //                            if (lossData != null)
        //                            {
        //                                lossmessageId = lossData.MessageCodeID;
        //                                duration = Convert.ToString(lossData.StartDateTime - lossData.EndDateTime);
        //                                TotalTimeTaken = TimeSpan.Parse(duration);

        //                            }
        //                            var losscodedesc = db.tbllossescodes.Where(s => s.LossCodeID == lossmessageId).Select(s => new { s.LossCodeDesc }).FirstOrDefault();
        //                            if (losscodedesc != null)
        //                            {
        //                                factor = losscodedesc.LossCodeDesc;
        //                            }

        //                            double totaltimetaken = Convert.ToDouble(TotalTimeTaken.TotalHours);
        //                            var lossPercentage = (totaltimetaken / 24) * 100;
        //                            ContributingFactors cf = new ContributingFactors();
        //                            cf.LossCodeDescription = factor;
        //                            cf.LossPercent = Convert.ToDecimal(lossPercentage);
        //                            cf.LossDurationInHours = Convert.ToDecimal(totaltimetaken);
        //                            ContributingFactorsList.Add(cf);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    res = JsonConvert.SerializeObject(ContributingFactorsList);
        //    return res;
        //}
        #endregion

        // Get Top 3 Losses  
        public string ContributingFactor()
        {
            List<TopContributingFactors> contfacList = new List<TopContributingFactors>();
            string res = "";
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            List<ContributingFactors> ContributingFactorsList = new List<ContributingFactors>();
            string[] backgroundcolr;
            string[] borderColor;
            List<ContributingFactors> ContributingFactorsListDist = new List<ContributingFactors>();
            List<LossDetails> objLossDistinct = new List<LossDetails>();
            List<LossDetails> objLoss = new List<LossDetails>();

            string correctedDate = GetCorrectedDate();
            DateTime correctedDate1 = Convert.ToDateTime(correctedDate);
            var celldet = new List<tblcell>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
            }
            int count = 0;
            foreach (var cell in celldet)
            {
                Color color = GetRandomColour();
                string val = "rgba(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + "," + color.A.ToString() + ")";
                count = count + 1;
                borderColor = new string[] { val, val, val, val };
                backgroundcolr = new string[] { val, val, val, val };
                var getmodes = new List<tbllivemode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    getmodes = db.tbllivemodes.Where(m => m.tblmachinedetail.tblcell.CellID == cell.CellID && m.CorrectedDate == correctedDate1.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1 && (m.LossCodeID != null || m.BreakdownID != null)).OrderBy(m => new { m.ModeID, m.StartTime }).ToList();
                }
                var TotalLossDuration = getmodes.Where(m => m.ModeType == "IDLE").Sum(m => m.DurationInSec).ToString();
                double TotalLossDura = Convert.ToDouble(TotalLossDuration);
                TopContributingFactors contf = new TopContributingFactors();
                foreach (var row in getmodes)
                {
                    if ((row.LossCodeID != null && row.LossCodeID != 0) || (row.BreakdownID != null && row.BreakdownID != 0))
                    {
                        LossDetails loss = new LossDetails();
                        if ((row.LossCodeID != null && row.LossCodeID != 0))
                        {
                            loss.LossID = row.LossCodeID;
                            loss.LossCodeDescription = row.tbllossescode.LossCode;

                        }
                        else if (row.BreakdownID != null && row.BreakdownID != 0)
                        {
                            loss.LossID = row.BreakdownID;
                            loss.LossCodeDescription = row.tblbreakdown.tbllossescode.LossCode.ToString();
                        }
                        loss.LossStartTime = Convert.ToDateTime(row.StartTime);
                        loss.LossEndTime = Convert.ToDateTime(row.EndTime);
                        double diff = loss.LossEndTime.Subtract(loss.LossStartTime).TotalMinutes;
                        loss.DurationinMin = diff;
                        loss.CellID = cell.CellID;
                        objLoss.Add(loss);
                    }
                }
                var idledistinct = objLoss.Where(m => m.CellID == cell.CellID).Select(m => new { m.LossCodeDescription, m.LossID }).Distinct().ToList();
                foreach (var row2 in idledistinct)
                {
                    ContributingFactors conf = new ContributingFactors();
                    LossDetails det = new LossDetails();
                    double Totalduration = 0;
                    var lossrow = objLoss.Where(m => m.LossCodeDescription == row2.LossCodeDescription).OrderByDescending(m => m.DurationinMin).ToList();
                    foreach (var loss in lossrow)
                    {
                        if (row2.LossID == loss.LossID)
                        {
                            Totalduration += loss.DurationinMin;
                            det = loss;
                            conf.LossCodeDescription = det.LossCodeDescription;
                        }
                    }
                    det.DurationinMin = Totalduration;
                    Double TotalTimeTaken = TimeSpan.FromSeconds(TotalLossDura).TotalMinutes;
                    //double totaltimetaken = Convert.ToDouble(TotalTimeTaken);
                    double lossduratin = TimeSpan.FromSeconds(Totalduration).TotalHours;
                    var lossPercentage = (Totalduration / TotalTimeTaken) * 100;
                    if (TotalTimeTaken == 0)
                        lossPercentage = 0;
                    if (lossPercentage > 100)
                        lossPercentage = 100;
                    conf.cellid = cell.CellID;
                    contf.CellName = cell.tblshop.Shopdisplayname + " - " + cell.CelldisplayName;
                    conf.LossPercent = Convert.ToDecimal(lossPercentage);
                    conf.LossDurationInHours = Convert.ToDecimal(lossduratin);
                    ContributingFactorsList.Add(conf);
                    objLossDistinct.Add(det);
                }
                var contributingdistinct = ContributingFactorsList.Where(m => m.cellid == cell.CellID).Select(m => new { m.LossCodeDescription }).Distinct().ToList();
                foreach (var con in contributingdistinct)
                {
                    var row = ContributingFactorsList.Where(m => m.LossCodeDescription == con.LossCodeDescription && m.cellid == cell.CellID).OrderByDescending(m => m.LossDurationInHours).FirstOrDefault();
                    if (con.LossCodeDescription == row.LossCodeDescription)
                    {
                        ContributingFactorsListDist.Add(row);
                    }

                }
                ContributingFactorsListDist = ContributingFactorsListDist.Where(m => m.cellid == cell.CellID).OrderByDescending(m => m.LossPercent).Take(3).ToList();
                double[] data = new double[3];
                string[] LossNames = new string[3];
                if (ContributingFactorsListDist.Count > 0)
                {
                    for (int i = 0; i < ContributingFactorsListDist.Count; i++)
                    {
                        LossNames[i] = ContributingFactorsListDist[i].LossCodeDescription;
                        data[i] = Convert.ToDouble(ContributingFactorsListDist[i].LossPercent);
                    }
                }
                else
                {
                    for (int i = 0; i < LossNames.Length; i++)
                    {
                        LossNames[i] = "";
                        data[i] = 0;
                    }
                }
                contf.backgroundColor = backgroundcolr;
                contf.borderColor = borderColor;
                contf.data = data;
                contf.LossName = LossNames;
                contfacList.Add(contf);
            }

            //}
            res = JsonConvert.SerializeObject(contfacList);
            //}
            return res;
        }

        public List<AlarmList> Alarms()
        {
            DateTime nowdate = DateTime.Now;
            string correctedDate = GetCorrectedDate();
            string correctdate = correctedDate;
            List<AlarmList> AlarmList = new List<AlarmList>();

            //var plantDetails = db.tblplants.Where(p => p.IsDeleted == 0).ToList();
            //if (plantDetails.Count > 0)
            //{
            //    foreach (var plant in plantDetails)
            //    {
            //        var plantid = plant.PlantID;
            //        var plantname = plant.PlantName;
            //        var shopDetails = db.tblshops.Where(s => s.IsDeleted == 0 && s.PlantID == plantid).ToList();
            //        if (shopDetails.Count > 0)
            //        {
            //            foreach (var shop in shopDetails)
            //            {
            //var shopId = shop.ShopID;
            var cellDetails = new List<tblcell>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                cellDetails = db.tblcells.Where(c => c.IsDeleted == 0).ToList(); //&& c.ShopID == shopId && c.PlantID == plantid
            }
            if (cellDetails != null)
            {
                foreach (var cell in cellDetails)
                {
                    var cellid = cell.CellID;
                    var machineDetails = new List<tblmachinedetail>();
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        machineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.InsertedOn).ToList(); //&& m.ShopID == shopId && m.PlantID == plantid
                    }
                    if (machineDetails != null)
                    {
                        foreach (var machine in machineDetails)
                        {
                            DataTable dt = new DataTable();
                            var machineId = machine.MachineID;
                            string machineName = machine.MachineDisplayName;
                            string alarmMasterDetails = "";
                            using (MsqlConnection con = new MsqlConnection())
                            {
                                con.open();
                                var alarmsdetails = db.tblpriorityalarms.Where(a => a.IsDeleted == 0).Select(a => new { a.AlarmNumber });
                                alarmMasterDetails = "SELECT MachineID,AlarmNo,AlarmMessage,Axis_Num,AlarmDateTime FROM i_facility.alarm_history_master  where AlarmNo in(" + alarmsdetails + ") && CorrectedDate = '" + correctdate + "' && MachineID=" + machineId + " group by AlarmNo order by AlarmDateTime desc;";
                                MySqlDataAdapter msda = new MySqlDataAdapter(alarmMasterDetails, con.sqlConnection);
                                msda.Fill(dt);
                                for (int p = 0; p < dt.Rows.Count; p++)
                                {
                                    AlarmList al = new AlarmList();
                                    al.MachineID = Convert.ToInt32(dt.Rows[p]["MachineID"]);
                                    //al.MachineName = dt.Rows[p][machineName].ToString();
                                    al.AlarmNumber = dt.Rows[p]["AlarmNo"].ToString();
                                    al.AlarmMessage = dt.Rows[p]["AlarmMessage"].ToString();
                                    al.AxisNumber = dt.Rows[p]["Axis_Num"].ToString();
                                    al.AlarmDateTime = Convert.ToDateTime(dt.Rows[p]["AlarmDateTime"]);
                                    AlarmList.Add(al);
                                }
                            }
                        }
                    }
                }
                //                }
                //            }
                //        }
                //    }
            }
            return AlarmList;
        }

        // Alarm Details
        public List<AlarmList> GetAlarms()
        {
            string res = "";
            List<AlarmList> AlarmList = new List<AlarmList>();
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            //string correctedDate = "2018-08-23";
            string correctedDate = GetCorrectedDate();
            string correctdate = correctedDate;
            DateTime CorrectedDate = Convert.ToDateTime(correctedDate);
            var celldet = new List<tblcell>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
            }
            foreach (var row in celldet)
            {
                var machineslist = new List<tblmachinedetail>();
                var alaramhistory = new List<alarm_history_master>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == row.CellID).ToList();
                }
                foreach (var machine in machineslist)
                {
                    var machineId = machine.MachineID;
                    string machineName = machine.MachineDisplayName;
                    var alarmsdetails = db.tblpriorityalarms.Where(a => a.IsDeleted == 0).OrderBy(m => m.AlarmID).ToList();
                    foreach (var alarm in alarmsdetails)
                    {
                        using (i_facilityEntities1 db = new i_facilityEntities1())
                        {
                            alaramhistory = db.alarm_history_master.Where(m => m.AlarmNo == alarm.AlarmNumber && m.MachineID == machineId && m.CorrectedDate == CorrectedDate.Date).OrderByDescending(m => m.AlarmNo).ToList();
                        }
                        foreach (var alh in alaramhistory)
                        {
                            AlarmList al = new AlarmList();
                            al.MachineID = alh.MachineID;
                            //al.MachineName = dt.Rows[p][machineName].ToString();
                            al.AlarmNumber = alh.AlarmNo.ToString();
                            al.AlarmMessage = alh.AlarmMessage.ToString();
                            al.AxisNumber = alh.Axis_Num.ToString();
                            al.AlarmDateTime = Convert.ToDateTime(alh.AlarmDateTime);
                            AlarmList.Add(al);
                        }
                    }
                }

            }
            res = JsonConvert.SerializeObject(AlarmList);
            //}
            return AlarmList;
        }

        public string TargetAcualsDet(int cellid)
        {
            string res = "";
            res = GetTarget_Actual(cellid);
            return res;
        }


        //public string GetTarget_Actual()
        //{
        //    string res = "";
        //    string[] backgroundcolr;
        //    string[] borderColor;
        //    DateTime nowdate = DateTime.Now;
        //    string correctedDate = DateTime.Now.ToString("yyyy-MM-dd");
        //    if (nowdate.Hour < 7 && nowdate.Hour > 0)
        //    {
        //        correctedDate = nowdate.AddDays(-1).ToString("yyyy-MM-dd");
        //    }
        //    string correctdate = correctedDate;
        //    DateTime CorrectedDate = Convert.ToDateTime(correctedDate);
        //    List<TargetActualListDet> TargetList = new List<TargetActualListDet>();
        //    List<TargetActualList> TargetActualList = new List<TargetActualList>();
        //    var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
        //    int count = 0;
        //    foreach (var row in celldet)
        //    {
        //        count = count + 1;
        //        backgroundcolr = new string[] { "rgba(254, 99, 131, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 87, 1)", "rgba(75, 192, 192, 1)" };
        //        borderColor = new string[] { "rgba(254, 99, 131, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)" };

        //        if (count == 1)
        //        {
        //            backgroundcolr = new string[] { "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)" };
        //            borderColor = new string[] { "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)" };

        //        }
        //        else if (count > 1)
        //        {
        //            backgroundcolr = new string[] { "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)" };
        //            borderColor = new string[] { "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)" };
        //        }
        //        TargetActualListDet TAL = new TargetActualListDet();
        //        TAL.CellName = row.CellName;


        //        var machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == row.CellID && m.IsBottelNeck == 1).FirstOrDefault();
        //        if (machineDet != null)
        //        {
        //            int MachineID = machineDet.MachineID;
        //            var partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0).Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime }).OrderBy(m => m.StartTime).ToList();
        //            int[] data = new int[partDetails.Count];
        //            List<int> Target = new List<int>();
        //            List<int> Actual = new List<int>();


        //            string[] Lables = new string[partDetails.Count];
        //            for (int i = 0; i < partDetails.Count; i++)
        //            {
        //              Target.Add( partDetails[i].TargetQuantity);
        //                Actual.Add(partDetails[i].PartCount);
        //                Lables[i] = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");

        //            }


        //            TAL.backgroundColor = backgroundcolr;
        //            TAL.borderColor = borderColor;
        //            TAL.Timings = Lables;

        //            TAL.Target = Target;
        //            TAL.Actual = Actual;
        //            TargetList.Add(TAL);
        //        }


        //    }
        //    res = JsonConvert.SerializeObject(TargetList);
        //    return res;
        //}


        public string GetTarget_Actual(int cellid)
        {
            string res = "";
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            string[] backgroundcolr;
            string[] borderColor;

            string correctedDate = GetCorrectedDate();
            string correctdate = correctedDate;
            DateTime CorrectedDate = Convert.ToDateTime(correctedDate);
            List<TargetActualListDet> TargetList = new List<TargetActualListDet>();
            List<TargetActualList> TargetActualList = new List<TargetActualList>();
            var celldet = new tblcell();
            var machineDet = new tblmachinedetail();
            var partDetails = new List<tblpartscountandcutting>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {

                celldet = db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderBy(m => m.CellID).FirstOrDefault();
            }
            int count = 0;
            if (celldet != null)
            {
                count = count + 1;
                backgroundcolr = new string[] { "rgba(254, 99, 131, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 87, 1)", "rgba(75, 192, 192, 1)" };
                borderColor = new string[] { "rgba(254, 99, 131, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)" };

                if (count == 1)
                {
                    backgroundcolr = new string[] { "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)" };
                    borderColor = new string[] { "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)" };

                }
                else if (count > 1)
                {
                    backgroundcolr = new string[] { "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)" };
                    borderColor = new string[] { "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)" };
                }
                TargetActualListDet TAL = new TargetActualListDet();
                TAL.CellName = celldet.CellName;

                string StartTime = CorrectedDate.ToString("yyyy-MM-dd") + " 07:00:00";
                string EndTime = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime St = Convert.ToDateTime(StartTime);
                DateTime Et = Convert.ToDateTime(EndTime);
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == celldet.CellID && m.IsBottelNeck == 1).FirstOrDefault();
                }
                if (machineDet != null)
                {
                    int MachineID = machineDet.MachineID;
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime >= St && m.EndTime <= Et).OrderBy(m => m.StartTime).ToList(); //.Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime })
                    }
                    int[] data = new int[partDetails.Count];
                    List<int> Target = new List<int>();
                    List<int> Actual = new List<int>();
                    string[] Lables = new string[partDetails.Count];
                    for (int i = 0; i < partDetails.Count; i++)
                    {
                        Target.Add(partDetails[i].TargetQuantity);
                        Actual.Add(partDetails[i].PartCount);
                        Lables[i] = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                    }
                    TAL.backgroundColor = backgroundcolr;
                    TAL.borderColor = borderColor;
                    TAL.Timings = Lables;
                    TAL.Target = Target;
                    TAL.Actual = Actual;
                    TargetList.Add(TAL);
                }
            }
            res = JsonConvert.SerializeObject(TargetList);
            //}
            return res;
        }

        public string GetTarget_Actual_Line(int cellid)
        {
            string res = "";
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            string[] backgroundcolr;
            string[] borderColor;

            string correctedDate = GetCorrectedDate();
            string correctdate = correctedDate;
            DateTime CorrectedDate = Convert.ToDateTime(correctedDate);
            List<TargetActualListDet> TargetList = new List<TargetActualListDet>();
            List<TargetActualList> TargetActualList = new List<TargetActualList>();
            var celldet = new tblcell();
            var machineDet = new tblmachinedetail();
            var partDetails = new List<tblpartscountandcutting>();
            List<ViewData> finalData = new List<ViewData>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {

                celldet = db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderBy(m => m.CellID).FirstOrDefault();
            }
            int count = 0;
            if (celldet != null)
            {
                count = count + 1;
                backgroundcolr = new string[] { "rgba(254, 99, 131, 1)", "rgba(54, 162, 235, 1)", "rgba(255, 206, 87, 1)", "rgba(75, 192, 192, 1)" };
                borderColor = new string[] { "rgba(254, 99, 131, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)" };

                if (count == 1)
                {
                    backgroundcolr = new string[] { "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)" };
                    borderColor = new string[] { "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)", "rgba(254, 99, 131, 1)" };

                }
                else if (count > 1)
                {
                    backgroundcolr = new string[] { "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)", "rgba(54, 162, 235, 1)" };
                    borderColor = new string[] { "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)", "rgba(75, 192, 192, 1)" };
                }
                ViewData obj = new ViewData();
                TargetActualListDet TAL = new TargetActualListDet();
                TAL.CellName = celldet.CellName;
                obj.name = celldet.CellName;
                obj.type = "line";
                obj.showInLegend = "true";
                string StartTime = CorrectedDate.ToString("yyyy-MM-dd") + " 07:00:00";
                string EndTime = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime St = Convert.ToDateTime(StartTime);
                DateTime Et = Convert.ToDateTime(EndTime);
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == celldet.CellID && m.IsBottelNeck == 1).FirstOrDefault();
                }
                if (machineDet != null)
                {
                    int MachineID = machineDet.MachineID;
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime >= St && m.EndTime <= Et).OrderBy(m => m.StartTime).ToList(); //.Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime })
                    }
                    int[] data = new int[partDetails.Count];
                    List<int> Target = new List<int>();
                    List<int> Actual = new List<int>();
                    List<dataPoints> ActualData = new List<dataPoints>();
                    List<dataPoints> TargetData = new List<dataPoints>();
                    string[] Lables = new string[partDetails.Count];
                    for (int i = 0; i < partDetails.Count; i++)
                    {
                        dataPoints obj1 = new dataPoints();

                        if (i == 0)
                        {
                            dataPoints obj2 = new dataPoints();
                            obj2.label = "Target";
                            obj2.markerColor = "red";
                            obj2.markerType = "triangle";
                            obj2.indexLabel = "Target";
                            obj2.y = partDetails[i].TargetQuantity;
                            ActualData.Add(obj2);

                        }
                        obj1.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                        obj1.y = partDetails[i].PartCount;
                        obj1.indexLabel = "Actual";
                        //obj2.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                        //obj2.y = partDetails[i].TargetQuantity;
                        ActualData.Add(obj1);
                        //TargetData.Add(obj2);
                        //Target.Add(partDetails[i].TargetQuantity);
                        Actual.Add(partDetails[i].PartCount);
                        Lables[i] = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                    }
                    obj.dataPoints = ActualData;
                    // obj.dataPointsTarget = TargetData;
                    TAL.backgroundColor = backgroundcolr;
                    TAL.borderColor = borderColor;
                    TAL.Timings = Lables;
                    TAL.Target = Target;
                    TAL.Actual = Actual;
                    TargetList.Add(TAL);
                    finalData.Add(obj);
                }
            }
            res = JsonConvert.SerializeObject(finalData);
            //}
            return res;
        }

        public string GetTarget_Actual_Data(int cellid)
        {
            string res = "";
            List<ChartDataVal> ListData = new List<ChartDataVal>();
            ChartDataVal objListData = new ChartDataVal();
            objListData.type = "column";
            objListData.name = "Target";
            objListData.showInLegend = "true";
            objListData.indexLabel = "{y}";
            List<PivotalData> objList = new List<PivotalData>();
            List<PivotalData> objListTarget = new List<PivotalData>();
            string correctedDate = GetCorrectedDate();
            string correctdate = correctedDate;
            DateTime CorrectedDate = Convert.ToDateTime(correctedDate);
            var celldet = new tblcell();
            var machineDet = new tblmachinedetail();
            var partDetails = new List<tblpartscountandcutting>();
            List<ViewData> finalData = new List<ViewData>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {

                celldet = db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderBy(m => m.CellID).FirstOrDefault();
            }
            if (celldet != null)
            {
                string StartTime = CorrectedDate.ToString("yyyy-MM-dd") + " 07:15:00";
                string EndTime = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime St = Convert.ToDateTime(StartTime);
                DateTime Et = Convert.ToDateTime(EndTime);
                if (celldet.Nobottleneck == 0)
                {
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == celldet.CellID && m.IsLastMachine == 1).FirstOrDefault(); //m.IsBottelNeck == 1
                    }
                    if (machineDet != null)
                    {
                        int MachineID = machineDet.MachineID;
                        using (i_facilityEntities1 db = new i_facilityEntities1())
                        {
                            partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime >= St && m.EndTime <= Et).OrderBy(m => m.StartTime).ToList(); //.Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime })
                        }
                        for (int i = 0; i < partDetails.Count; i++)
                        {
                            PivotalData obj = new PivotalData();
                            PivotalData Tobj = new PivotalData();
                            obj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                            obj.y = partDetails[i].TargetQuantity;
                            objList.Add(obj);
                            Tobj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                            Tobj.y = partDetails[i].PartCount;
                            objListTarget.Add(Tobj);
                        }
                    }
                }
                else
                {
                    var machineids = new List<int>();
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        machineids = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == celldet.CellID && m.tblcell.Nobottleneck == 1).Select(m => m.MachineID).ToList(); //m.IsBottelNeck == 1
                    }

                    if (machineids.Count > 0)
                    {
                        var SumTarget = 0;
                        var SumActual = 0;
                        int MachineID = machineids[0];
                        using (i_facilityEntities1 db = new i_facilityEntities1())
                        {
                            partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime >= St && m.EndTime <= Et).OrderBy(m => m.StartTime).ToList(); //.Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime })
                        }
                        for (int i = 0; i < partDetails.Count; i++)
                        {
                            PivotalData obj = new PivotalData();
                            PivotalData Tobj = new PivotalData();
                            DateTime st1 = partDetails[i].StartTime;
                            DateTime Et1 = partDetails[i].EndTime;
                            using (i_facilityEntities1 db = new i_facilityEntities1())
                            {
                                SumTarget = db.tblpartscountandcuttings.Where(m => machineids.Contains(m.MachineID) && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime == st1 && m.EndTime == Et1).Sum(m => m.TargetQuantity);
                                SumActual = db.tblpartscountandcuttings.Where(m => machineids.Contains(m.MachineID) && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime == st1 && m.EndTime == Et1).Sum(m => m.PartCount);
                            }

                            obj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                            obj.y = SumTarget;
                            objList.Add(obj);
                            Tobj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
                            Tobj.y = SumActual;
                            objListTarget.Add(Tobj);
                        }
                    }
                }
            }
            objListData.dataPoints = objList;
            objListData.dataPointsTarget = objListTarget;
            ListData.Add(objListData);
            res = JsonConvert.SerializeObject(ListData);
            return res;
        }


        public string ContributingFactorLosses()
        {
            List<TopContributingFactors> contfacList = new List<TopContributingFactors>();
            string res = "";
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            List<ContributingFactors> ContributingFactorsList = new List<ContributingFactors>();
            string[] backgroundcolr;
            string[] borderColor;
            List<ContributingFactors> ContributingFactorsListDist = new List<ContributingFactors>();
            List<LossDetails> objLossDistinct = new List<LossDetails>();
            List<LossDetails> objLoss = new List<LossDetails>();

            string correctedDate = GetCorrectedDate();
            DateTime correctedDate1 = Convert.ToDateTime(correctedDate);
            var celldet = new List<tblcell>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
            }
            int count = 0;
            foreach (var cell in celldet)
            {
                Color color = GetRandomColour();
                string val = "rgba(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + "," + color.A.ToString() + ")";
                count = count + 1;
                borderColor = new string[] { val, val, val, val };
                backgroundcolr = new string[] { val, val, val, val };
                var getmodes = new List<tbllivemode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    getmodes = db.tbllivemodes.Where(m => m.tblmachinedetail.tblcell.CellID == cell.CellID && m.tblmachinedetail.IsLastMachine == 1 && m.CorrectedDate == correctedDate1.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1 && (m.LossCodeID != null || m.BreakdownID != null)).OrderBy(m => new { m.ModeID, m.StartTime }).ToList();
                    if (getmodes.Count == 0)
                    {
                        getmodes = getmodes = db.tbllivemodes.Where(m => m.tblmachinedetail.tblcell.CellID == cell.CellID && m.tblmachinedetail.IsLastMachine == 1 && m.CorrectedDate == correctedDate1.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1).OrderBy(m => new { m.ModeID, m.StartTime }).ToList(); //&& (m.LossCodeID != null || m.BreakdownID != null)
                    }
                }
                var TotalLossDuration = getmodes.Where(m => m.ModeType == "IDLE").Sum(m => m.DurationInSec).ToString();
                double TotalLossDura = Convert.ToDouble(TotalLossDuration);
                
                TopContributingFactors contf = new TopContributingFactors();
                foreach (var row in getmodes)
                {
                    if ((row.LossCodeID != null && row.LossCodeID != 0) || (row.BreakdownID != null && row.BreakdownID != 0))
                    {
                        LossDetails loss = new LossDetails();
                        if ((row.LossCodeID != null && row.LossCodeID != 0))
                        {
                            loss.LossID = row.LossCodeID;
                            var losscode = db.tbllossescodes.Where(m => m.LossCodeID == loss.LossID).Select(m => m.LossCode).FirstOrDefault();
                            //loss.LossCodeDescription = row.tbllossescode.LossCode;
                            loss.LossCodeDescription = losscode;

                        }
                        else if (row.BreakdownID != null && row.BreakdownID != 0)
                        {
                            loss.LossID = row.BreakdownID;
                            loss.LossCodeDescription = row.tblbreakdown.tbllossescode.LossCode.ToString();
                        }
                        loss.LossStartTime = Convert.ToDateTime(row.StartTime);
                        loss.LossEndTime = Convert.ToDateTime(row.EndTime);
                        double diff = loss.LossEndTime.Subtract(loss.LossStartTime).TotalMinutes;
                        loss.DurationinMin = diff;
                        loss.CellID = cell.CellID;
                        objLoss.Add(loss);
                    }
                    else
                    {
                        LossDetails loss = new LossDetails();
                        loss.LossID = 0;
                        loss.LossCodeDescription = "NO CODE";
                        loss.DurationinMin = TimeSpan.FromSeconds(TotalLossDura).TotalMinutes;
                        loss.CellID = cell.CellID;
                        objLoss.Add(loss);
                        TotalLossDuration = getmodes.Sum(m => m.DurationInSec).ToString();
                        TotalLossDura = Convert.ToDouble(TotalLossDuration);
                        break;
                    }
                }
                var idledistinct = objLoss.Where(m => m.CellID == cell.CellID).Select(m => new { m.LossCodeDescription, m.LossID }).Distinct().ToList();
                foreach (var row2 in idledistinct)
                {
                    ContributingFactors conf = new ContributingFactors();
                    LossDetails det = new LossDetails();
                    double Totalduration = 0;
                    var lossrow = objLoss.Where(m => m.LossCodeDescription == row2.LossCodeDescription && m.CellID == cell.CellID).OrderByDescending(m => m.DurationinMin).ToList();
                    foreach (var loss in lossrow)
                    {
                        if (row2.LossID == loss.LossID)
                        {
                            Totalduration += loss.DurationinMin;
                            det = loss;
                            conf.LossCodeDescription = det.LossCodeDescription;
                        }
                    }
                    det.DurationinMin = Totalduration;
                    if (TotalLossDura == 0)
                    {
                        TotalLossDura = 120;
                    }
                    Double TotalTimeTaken = TimeSpan.FromMinutes(TotalLossDura).TotalHours;
                    //double totaltimetaken = Convert.ToDouble(TotalTimeTaken);
                    double lossduratin = TimeSpan.FromMinutes(Totalduration).TotalHours;
                    // var lossPercentage = (Totalduration / TotalTimeTaken) * 100;
                    var lossPercentage = Convert.ToDouble(lossduratin);
                    if (TotalTimeTaken == 0)
                        lossPercentage = 0;
                    if (lossPercentage > 24)
                        lossPercentage = 24;
                    conf.cellid = cell.CellID;
                    contf.CellName = /*cell.tblshop.Shopdisplayname + " - " +*/ cell.CelldisplayName;
                    conf.LossPercent = Convert.ToDecimal(lossPercentage);
                    conf.LossDurationInHours = Convert.ToDecimal(lossduratin);
                    ContributingFactorsList.Add(conf);
                    objLossDistinct.Add(det);
                }
                var contributingdistinct = ContributingFactorsList.Where(m => m.cellid == cell.CellID).Select(m => new { m.LossCodeDescription }).Distinct().ToList();
                foreach (var con in contributingdistinct)
                {
                    var row = ContributingFactorsList.Where(m => m.LossCodeDescription == con.LossCodeDescription && m.cellid == cell.CellID).OrderByDescending(m => m.LossDurationInHours).FirstOrDefault();
                    if (con.LossCodeDescription == row.LossCodeDescription)
                    {
                        ContributingFactorsListDist.Add(row);
                    }

                }
                ContributingFactorsListDist = ContributingFactorsListDist.Where(m => m.cellid == cell.CellID).OrderByDescending(m => m.LossPercent).Take(3).ToList();
                double[] data = new double[3];
                string[] LossNames = new string[3];
                ContributingFactorsListDist = ContributingFactorsListDist.Where(m => m.cellid == cell.CellID).OrderByDescending(m => m.LossPercent).Take(3).ToList();
                int namecount = 0;
                namecount = ContributingFactorsListDist.Where(m => m.LossCodeDescription != null).ToList().Count;
                int j = 0;
                if (ContributingFactorsListDist.Count > 0 && ContributingFactorsListDist.Count == 3)
                {
                    for (int i = 0; i < ContributingFactorsListDist.Count; i++)
                    {
                        LossNames[i] = ContributingFactorsListDist[i].LossCodeDescription;
                        data[i] = Convert.ToDouble(ContributingFactorsListDist[i].LossPercent);
                    }
                }
                else
                {
                    for (int i = 0; i < ContributingFactorsListDist.Count; i++)
                    {

                        LossNames[i] = ContributingFactorsListDist[i].LossCodeDescription;
                        data[i] = Convert.ToDouble(ContributingFactorsListDist[i].LossPercent);
                        j = i + 1;
                    }
                    for (int i = j; i <= (3 - namecount); i++)
                    {
                        if (i != 3)
                        {
                            LossNames[i] = "";
                            data[i] = 0;
                        }
                    }
                }
                contf.backgroundColor = backgroundcolr;
                contf.borderColor = borderColor;
                contf.data = data;
                contf.LossName = LossNames;
                contf.indexLabel = "{y}";
                contfacList.Add(contf);
            }

            //}
            res = JsonConvert.SerializeObject(contfacList);
            //}
            return res;
        }

        public string ContributingFactorLossesByCell(int cellid)
        {
            List<TopContributingFactors> contfacList = new List<TopContributingFactors>();
            string res = "";
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
            List<ContributingFactors> ContributingFactorsList = new List<ContributingFactors>();
            string[] backgroundcolr;
            string[] borderColor;
            List<ContributingFactors> ContributingFactorsListDist = new List<ContributingFactors>();
            List<LossDetails> objLossDistinct = new List<LossDetails>();
            List<LossDetails> objLoss = new List<LossDetails>();
            List<ChartDataVal> ListData = new List<ChartDataVal>();
            List<PivotalData> objList = new List<PivotalData>();
            ChartDataVal objListData = new ChartDataVal();
            objListData.type = "column";

            objListData.showInLegend = "true";
            objListData.indexLabel = "{y}";
            string correctedDate = GetCorrectedDate();
            DateTime correctedDate1 = Convert.ToDateTime(correctedDate);
            var celldet = new List<tblcell>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                celldet = db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderBy(m => m.CellID).ToList();
            }
            int count = 0;
            foreach (var cell in celldet)
            {
                Color color = GetRandomColour();
                string val = "rgba(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + "," + color.A.ToString() + ")";
                count = count + 1;
                borderColor = new string[] { val, val, val, val };
                backgroundcolr = new string[] { val, val, val, val };
                var getmodes = new List<tbllivemode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    getmodes = db.tbllivemodes.Where(m => m.tblmachinedetail.tblcell.CellID == cell.CellID && m.tblmachinedetail.IsLastMachine == 1 && m.CorrectedDate == correctedDate1.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1 && (m.LossCodeID != null || m.BreakdownID != null)).OrderBy(m => new { m.ModeID, m.StartTime }).ToList();
                    if (getmodes.Count == 0)
                    {
                        getmodes = db.tbllivemodes.Where(m => m.tblmachinedetail.tblcell.CellID == cell.CellID && m.tblmachinedetail.IsLastMachine == 1 && m.CorrectedDate == correctedDate1.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1).OrderBy(m => new { m.ModeID, m.StartTime }).ToList(); //&& (m.LossCodeID != null || m.BreakdownID != null)
                    }
                }
                var TotalLossDuration = getmodes.Where(m => m.ModeType == "IDLE").Sum(m => m.DurationInSec).ToString();
                double TotalLossDura = Convert.ToDouble(TotalLossDuration);
                TopContributingFactors contf = new TopContributingFactors();
                foreach (var row in getmodes)
                {
                    if ((row.LossCodeID != null && row.LossCodeID != 0) || (row.BreakdownID != null && row.BreakdownID != 0))
                    {
                        LossDetails loss = new LossDetails();
                        if ((row.LossCodeID != null && row.LossCodeID != 0))
                        {
                            loss.LossID = row.LossCodeID;
                            var losscode = db.tbllossescodes.Where(m => m.LossCodeID == loss.LossID).Select(m => m.LossCode).FirstOrDefault();
                            //loss.LossCodeDescription = row.tbllossescode.LossCode;
                            loss.LossCodeDescription = losscode;

                        }
                        else if (row.BreakdownID != null && row.BreakdownID != 0)
                        {
                            loss.LossID = row.BreakdownID;
                            loss.LossCodeDescription = row.tblbreakdown.tbllossescode.LossCode.ToString();
                        }
                        loss.LossStartTime = Convert.ToDateTime(row.StartTime);
                        loss.LossEndTime = Convert.ToDateTime(row.EndTime);
                        double diff = loss.LossEndTime.Subtract(loss.LossStartTime).TotalMinutes;
                        loss.DurationinMin = diff;
                        loss.CellID = cell.CellID;
                        objLoss.Add(loss);
                    }
                    else
                    {
                        LossDetails loss = new LossDetails();
                        loss.LossID = 0;
                        loss.LossCodeDescription = "NO CODE";
                        loss.DurationinMin = TimeSpan.FromSeconds(TotalLossDura).TotalMinutes;
                        loss.CellID = cell.CellID;
                        objLoss.Add(loss);
                        TotalLossDuration = getmodes.Sum(m => m.DurationInSec).ToString();
                        TotalLossDura = Convert.ToDouble(TotalLossDuration);
                        break;
                    }
                }
                var idledistinct = objLoss.Where(m => m.CellID == cell.CellID).Select(m => new { m.LossCodeDescription, m.LossID }).Distinct().ToList();
                foreach (var row2 in idledistinct)
                {
                    ContributingFactors conf = new ContributingFactors();
                    LossDetails det = new LossDetails();
                    double Totalduration = 0;
                    var lossrow = objLoss.Where(m => m.LossCodeDescription == row2.LossCodeDescription && m.CellID == cell.CellID).OrderByDescending(m => m.DurationinMin).ToList();
                    foreach (var loss in lossrow)
                    {
                        if (row2.LossID == loss.LossID)
                        {
                            Totalduration += loss.DurationinMin;
                            det = loss;
                            conf.LossCodeDescription = det.LossCodeDescription;
                        }
                    }
                    det.DurationinMin = Totalduration;

                    Double TotalTimeTaken = TimeSpan.FromMinutes(TotalLossDura).TotalHours;
                    //double totaltimetaken = Convert.ToDouble(TotalTimeTaken);
                    double lossduratin = TimeSpan.FromMinutes(Totalduration).TotalHours;
                    // var lossPercentage = (Totalduration / TotalTimeTaken) * 100;
                    var lossPercentage = Convert.ToDouble(lossduratin);
                   
                    if (TotalTimeTaken == 0)
                        lossPercentage = 0;
                    if (lossPercentage > 24)
                        lossPercentage = 24;
                    conf.cellid = cell.CellID;
                    contf.CellName = /*cell.tblshop.Shopdisplayname + " - " +*/ cell.CelldisplayName;
                    conf.LossPercent = Convert.ToDecimal(lossPercentage);
                    conf.LossDurationInHours = Convert.ToDecimal(lossduratin);
                    ContributingFactorsList.Add(conf);
                    objLossDistinct.Add(det);
                }
                var contributingdistinct = ContributingFactorsList.Where(m => m.cellid == cell.CellID).Select(m => new { m.LossCodeDescription }).Distinct().ToList();
                foreach (var con in contributingdistinct)
                {

                    var row = ContributingFactorsList.Where(m => m.LossCodeDescription == con.LossCodeDescription && m.cellid == cell.CellID).OrderByDescending(m => m.LossDurationInHours).FirstOrDefault();
                    if (con.LossCodeDescription == row.LossCodeDescription)
                    {
                        ContributingFactorsListDist.Add(row);
                    }

                }
                ContributingFactorsListDist = ContributingFactorsListDist.Where(m => m.cellid == cell.CellID).OrderByDescending(m => m.LossPercent).Take(3).ToList();
                double[] data = new double[3];
                string[] LossNames = new string[3];
                ContributingFactorsListDist = ContributingFactorsListDist.Where(m => m.cellid == cell.CellID).OrderByDescending(m => m.LossPercent).Take(3).ToList();
                int namecount = 0;
                namecount = ContributingFactorsListDist.Where(m => m.LossCodeDescription != null).ToList().Count;
                int j = 0;
                if (ContributingFactorsListDist.Count > 0 && ContributingFactorsListDist.Count == 3)
                {
                    for (int i = 0; i < ContributingFactorsListDist.Count; i++)
                    {
                        PivotalData obj = new PivotalData();
                        obj.label = ContributingFactorsListDist[i].LossCodeDescription;
                        obj.y = Convert.ToInt32(ContributingFactorsListDist[i].LossPercent);
                        objList.Add(obj);
                        LossNames[i] = ContributingFactorsListDist[i].LossCodeDescription;
                        data[i] = Convert.ToDouble(ContributingFactorsListDist[i].LossPercent);
                    }
                }
                else
                {

                    for (int i = 0; i < ContributingFactorsListDist.Count; i++)
                    {
                        PivotalData obj = new PivotalData();
                        obj.label = ContributingFactorsListDist[i].LossCodeDescription;
                        var lossduration = Math.Round(Convert.ToDecimal(ContributingFactorsListDist[i].LossPercent), 2);
                        obj.y = Convert.ToInt32(lossduration);
                        objListData.name = ContributingFactorsListDist[i].LossCodeDescription;
                        objList.Add(obj);
                        LossNames[i] = ContributingFactorsListDist[i].LossCodeDescription;
                        data[i] = Convert.ToDouble(ContributingFactorsListDist[i].LossPercent);
                        j = i + 1;
                    }
                    for (int i = j; i <= (3 - namecount); i++)
                    {
                        if (i != 3)
                        {
                            LossNames[i] = "";
                            data[i] = 0;
                        }
                    }
                }
                contf.backgroundColor = backgroundcolr;
                contf.borderColor = borderColor;
                contf.data = data;
                contf.LossName = LossNames;
                contf.indexLabel = "{y}";
                contfacList.Add(contf);
                objListData.dataPoints = objList;

                //objListData.dataPointsTarget = objListTarget;
                ListData.Add(objListData);

            }

            res = JsonConvert.SerializeObject(ListData);
            //}
            //res = JsonConvert.SerializeObject(contfacList);
            //}
            return res;
        }


        //    private List<PivotalData> getpartsdetais(tblcell celldet,DateTime CorrectedDate)
        //    {
        //        var machinedetList= new List<tblmachinedetail>();
        //        var partDetails = new List<tblpartscountandcutting>();
        //        List<PivotalData> objList = new List<PivotalData>();
        //        List<PivotalData> objListTarget = new List<PivotalData>();
        //        string StartTime = CorrectedDate.ToString("yyyy-MM-dd") + " 07:15:00";
        //        string EndTime = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
        //        DateTime St = Convert.ToDateTime(StartTime);
        //        DateTime Et = Convert.ToDateTime(EndTime);
        //        if (celldet.Nobottleneck == 0)
        //        {

        //            var machineDet = new tblmachinedetail();
        //            using (i_facilityEntities1 db = new i_facilityEntities1())
        //            {
        //                machineDet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == celldet.CellID && m.IsLastMachine == 1).FirstOrDefault(); //m.IsBottelNeck == 1
        //            }
        //            if (machineDet != null)
        //            {
        //                int MachineID = machineDet.MachineID;
        //                using (i_facilityEntities1 db = new i_facilityEntities1())
        //                {
        //                    partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime >= St && m.EndTime <= Et).OrderBy(m => m.StartTime).ToList(); //.Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime })
        //                }
        //                for (int i = 0; i < partDetails.Count; i++)
        //                {
        //                    PivotalData obj = new PivotalData();
        //                    PivotalData Tobj = new PivotalData();
        //                    obj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
        //                    obj.y = partDetails[i].TargetQuantity;
        //                    objList.Add(obj);
        //                    Tobj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
        //                    Tobj.y = partDetails[i].PartCount;
        //                    objListTarget.Add(Tobj);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var machineids = new List<int>();
        //            using (i_facilityEntities1 db = new i_facilityEntities1())
        //            {
        //                machineids = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == celldet.CellID && m.tblcell.Nobottleneck==1).Select(m=>m.MachineID).ToList(); //m.IsBottelNeck == 1
        //            }

        //           if (machineids.Count>0)
        //            {
        //                var SumTarget = 0;
        //                var SumActual = 0;
        //                int MachineID = machineids[0];
        //                using (i_facilityEntities1 db = new i_facilityEntities1())
        //                {
        //                    partDetails = db.tblpartscountandcuttings.Where(m => m.MachineID == MachineID && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime >= St && m.EndTime <= Et).OrderBy(m => m.StartTime).ToList(); //.Select(m => new { m.PartCount, m.TargetQuantity, m.StartTime, m.EndTime })
        //                }
        //                for (int i = 0; i < partDetails.Count; i++)
        //                {
        //                    PivotalData obj = new PivotalData();
        //                    PivotalData Tobj = new PivotalData();
        //                    DateTime st1 = partDetails[i].StartTime;
        //                    DateTime Et1 = partDetails[i].StartTime;
        //                    using (i_facilityEntities1 db = new i_facilityEntities1())
        //                    {
        //                         SumTarget= db.tblpartscountandcuttings.Where(m => machineids.Contains(m.MachineID) && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime == st1 && m.EndTime == Et1).Sum(m => m.TargetQuantity);
        //                         SumActual = db.tblpartscountandcuttings.Where(m => machineids.Contains( m.MachineID ) && m.CorrectedDate == CorrectedDate.Date && m.Isdeleted == 0 && m.StartTime == st1 && m.EndTime == Et1).Sum(m => m.PartCount);
        //                    }

        //                    obj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
        //                    obj.y = SumTarget;
        //                    objList.Add(obj);
        //                    Tobj.label = partDetails[i].StartTime.ToString("HH:mm") + " - " + partDetails[i].EndTime.ToString("HH:mm");
        //                    Tobj.y = SumActual;
        //                    objListTarget.Add(Tobj);
        //                }
        //            }
        //    }

    }
}