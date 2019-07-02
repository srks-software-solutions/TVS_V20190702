
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using Newtonsoft.Json;
using I_Facility.Models;

namespace UnitWorksCCS.Controllers
{
    public class MConnextDashboardController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        //GET: Gentella
        public ActionResult Index()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                
                return RedirectToAction("Login", "Login", null);
            }
            else
                ViewBag.Time = DateTime.Now.ToShortTimeString();
            return View();
        }

        public ActionResult MachineDetails()
        {
            
            return View();
        }

        public string GetMachine()
        {
            
            List<GentellaModel> gentelladetList = new List<GentellaModel>();
            List<MachineUTF> MachineUTFList = new List<MachineUTF>();
            List<DataProviderAxisBySpindleLoad> SpindleLoadList = new List<DataProviderAxisBySpindleLoad>();
            string res = "";
            DateTime nowdate = DateTime.Now;
            string correctedDate = nowdate.ToString("yyyy-MM-dd");
           // string correctedDate = "2018-08-23";
            if (nowdate.Hour < 7 && nowdate.Hour > 0)
            {
                correctedDate = nowdate.AddDays(-1).ToString("yyyy-MM-dd");
            }
            DateTime correctdate = Convert.ToDateTime(correctedDate);
            var machinedet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 ).OrderByDescending(m => m.InsertedOn).ToList();

            //var machinedet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 ).OrderByDescending(m => m.InsertedOn).ToList();

            if (machinedet != null)
            {
                machinedet = machinedet.OrderBy(m => m.MachineID).ToList();
                foreach (var machine in machinedet)
                {
                    int machineID = machine.MachineID;
                    double machineIdlemin = Convert.ToDouble(machine.MachineIdleMin);
                    double machineidletime = TimeSpan.FromMinutes(machineIdlemin).TotalSeconds;


                    TimeSpan machineIdlesec = TimeSpan.Parse(machineidletime.ToString());

                    var tblparam = db.parameters_master.Where(m => m.MachineID == machine.MachineID).Select(m => new { m.OperatingTime, m.PowerOnTime, m.SetupTime, m.PartsTotal, m.CuttingTime }).OrderByDescending(m => m.OperatingTime).FirstOrDefault();
                    var tblmode = db.tbllivemodes.Where(m => m.IsDeleted == 0 && m.IsCompleted == 0 && m.MachineID == machineID).FirstOrDefault();
                    var machinmodes = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && m.IsDeleted == 0).Select(m => new { m.MacMode, m.ColorCode, m.DurationInSec }).ToList();
                    double idletimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec));
                    double RunningTimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec));
                    double Breakdowntimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "RED").ToList().Sum(m => m.DurationInSec));
                    double TotaltimeTaken = idletimeinsec + RunningTimeinsec + Breakdowntimeinsec;
                    if (tblparam != null)
                    {
                        TimeSpan PoweronTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.PowerOnTime));
                        double UTF = Convert.ToInt32(TotaltimeTaken) / Convert.ToInt32(PoweronTime.TotalSeconds) * 100;
                    }

                    #region MachineDetails
                    GentellaModel gentelldet = new GentellaModel();
                    gentelldet.MachineName = machine.MachineDisplayName;
                    gentelldet.MachineID = machine.MachineID;
                    gentelldet.CurrentStatus = tblmode.MacMode;
                    gentelldet.Color = tblmode.ColorCode;
                    //gentelldet.Utilization = UTF.ToString();

                    string idletimeinseconds = TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"ss");
                    gentelldet.IdleTime = TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"hh\:mm\:ss");

                    //VirtualHMI objvirtual = new VirtualHMI(machine.IPAddress, machine.MachineName);
                    double CycleTime = 0;
                    short exeprogramnum = 0;
                    ushort h;
                    //objvirtual.UTFValuesforMachine(out CycleTime, out exeprogramnum, out h);
                    TimeSpan tm1 = TimeSpan.FromMilliseconds(CycleTime);
                    gentelldet.CycleTime = tm1.ToString();
                    gentelldet.ExeProgramName = exeprogramnum.ToString();
                    if (idletimeinsec < machineidletime)
                    {
                        gentelldet.MinorLossesTime = idletimeinsec.ToString();
                    }
                    else
                    {
                        gentelldet.MinorLossesTime = "0";
                    }
                    if (tblparam != null)
                    {
                        gentelldet.PowerOnTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.PowerOnTime)).ToString(@"hh\:mm\:ss");
                        gentelldet.RunningTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.OperatingTime)).ToString(@"hh\:mm\:ss");
                        gentelldet.CuttingTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.CuttingTime)).ToString(@"hh\:mm\:ss");
                        gentelldet.PartsCount = tblparam.PartsTotal;

                    }

                    #endregion

                    #region MachindetailsChart
                    MachineUTF objmachineutf = new MachineUTF();
                    objmachineutf.RunningTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
                    objmachineutf.IdleTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
                    objmachineutf.BreakDownTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "RED").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
                    objmachineutf.PowerOffTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "BLUE").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));

                    objmachineutf.MachineName = machine.MachineDisplayName;

                    MachineUTFList.Add(objmachineutf);
                    gentelldet.MachineUTFs = MachineUTFList;
                    #endregion

                    #region Graph

                    var machinedet1 = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                    var spindleload = db.tbl_axisdetails2.Where(m => m.IsDeleted == 0 && m.MachineID == machinedet1.MachineID).Select(a => new { a.StartTime, a.SpindleLoad }).Take(30).ToList();
                    foreach (var load in spindleload)
                    {

                        DataProviderAxisBySpindleLoad objload = new DataProviderAxisBySpindleLoad();
                        objload.Time = load.StartTime?.ToString(@"hh\:mm\:ss");
                        objload.value = load.SpindleLoad;
                        SpindleLoadList.Add(objload);

                    }
                    gentelldet.Spindleloads = SpindleLoadList.OrderBy(m => m.Time).ToList();
                    #endregion



                    gentelladetList.Add(gentelldet);

                }
            }

            res = JsonConvert.SerializeObject(gentelladetList);

            return res;
        }

        //public string GetMachine()
        //{
        //    List<GentellaModel> gentelladetList = new List<GentellaModel>();
        //    List<MachineUTF> MachineUTFList = new List<MachineUTF>();
        //    List<DataProviderAxisBySpindleLoad> SpindleLoadList = new List<DataProviderAxisBySpindleLoad>();
        //    string res = "";
        //    DateTime nowdate = DateTime.Now;
        //    string correctedDate = nowdate.ToString("yyyy-MM-dd");
        //    string correctedDate = "2018-05-16";
        //    if (nowdate.Hour < 7 && nowdate.Hour > 0)
        //    {
        //        correctedDate = nowdate.AddDays(-1).ToString("yyyy-MM-dd");
        //    }
        //    DateTime correctdate = Convert.ToDateTime(correctedDate);
        //    var machinedet = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).Take(5).ToList();
        //    if (machinedet != null)
        //    {
        //        machinedet = machinedet.OrderBy(m => m.MachineID).ToList();
        //        foreach (var machine in machinedet)
        //        {
        //            int machineID = machine.MachineID;
        //            double machineIdlemin = Convert.ToDouble(machine.MachineIdleMin);
        //            double machineidletime = TimeSpan.FromMinutes(machineIdlemin).TotalSeconds;


        //            TimeSpan machineIdlesec = TimeSpan.Parse(machineidletime.ToString());

        //            var tblparam = db.parameters_master.Where(m => m.MachineID == machine.MachineID).Select(m => new { m.OperatingTime, m.PowerOnTime, m.SetupTime, m.PartsTotal, m.CuttingTime }).OrderByDescending(m => m.OperatingTime).FirstOrDefault();
        //            var tblmode = db.tblmodes.Where(m => m.IsDeleted == 0 && m.IsCompleted == 0 && m.MachineID == machineID).FirstOrDefault();
        //            var machinmodes = db.tblmodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && m.IsDeleted == 0).Select(m => new { m.MacMode, m.ColorCode, m.DurationInSec }).ToList();
        //            double idletimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec));
        //            double RunningTimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec));
        //            double Breakdowntimeinsec = Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "RED").ToList().Sum(m => m.DurationInSec));
        //            double TotaltimeTaken = idletimeinsec + RunningTimeinsec + Breakdowntimeinsec;
        //            TimeSpan PoweronTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.PowerOnTime));
        //            double UTF = Convert.ToInt32(TotaltimeTaken) / Convert.ToInt32(PoweronTime.TotalSeconds) * 100;

        //            #region MachineDetails
        //            GentellaModel gentelldet = new GentellaModel();
        //            gentelldet.MachineName = machine.MachineDisplayName;
        //            gentelldet.MachineID = machine.MachineID;
        //            gentelldet.CurrentStatus = tblmode.MacMode;
        //            gentelldet.Color = tblmode.ColorCode;
        //            gentelldet.Utilization = UTF.ToString();

        //            string idletimeinseconds = TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"ss");
        //            gentelldet.IdleTime = TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"hh\:mm\:ss");

        //            VirtualHMI objvirtual = new VirtualHMI(machine.IPAddress, machine.MachineName);
        //            double CycleTime = 0;
        //            short exeprogramnum = 0;
        //            ushort h;
        //            objvirtual.UTFValuesforMachine(out CycleTime, out exeprogramnum, out h);
        //            TimeSpan tm1 = TimeSpan.FromMilliseconds(CycleTime);
        //            gentelldet.CycleTime = tm1.ToString();
        //            gentelldet.ExeProgramName = exeprogramnum.ToString();
        //            if (idletimeinsec < machineidletime)
        //            {
        //                gentelldet.MinorLossesTime = idletimeinsec.ToString();
        //            }
        //            else
        //            {
        //                gentelldet.MinorLossesTime = "0";
        //            }
        //            if (tblparam != null)
        //            {
        //                gentelldet.PowerOnTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.PowerOnTime)).ToString(@"hh\:mm\:ss");
        //                gentelldet.RunningTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.OperatingTime)).ToString(@"hh\:mm\:ss");
        //                gentelldet.CuttingTime = TimeSpan.FromMinutes(Convert.ToDouble(tblparam.CuttingTime)).ToString(@"hh\:mm\:ss");
        //                gentelldet.PartsCount = tblparam.PartsTotal;

        //            }

        //            #endregion

        //            #region MachindetailsChart
        //            MachineUTF objmachineutf = new MachineUTF();
        //            objmachineutf.RunningTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));

        //            objmachineutf.IdleTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
        //            objmachineutf.BreakDownTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "RED").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
        //            objmachineutf.PowerOffTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "BLUE").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));

        //            objmachineutf.MachineName = machine.MachineDisplayName;

        //            MachineUTFList.Add(objmachineutf);
        //            gentelldet.MachineUTFs = MachineUTFList;
        //            #endregion

        //            #region Graph

        //            var machinedet1 = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
        //            var spindleload = db.tbl_axisdetails2.Where(m => m.IsDeleted == 0 && m.MachineID == machinedet1.MachineID).Select(a => new { a.StartTime, a.SpindleLoad }).Take(30).ToList();
        //            foreach (var load in spindleload)
        //            {

        //                DataProviderAxisBySpindleLoad objload = new DataProviderAxisBySpindleLoad();
        //                objload.Time = load.StartTime?.ToString(@"hh\:mm\:ss");
        //                objload.value = load.SpindleLoad;
        //                SpindleLoadList.Add(objload);

        //            }
        //            gentelldet.Spindleloads = SpindleLoadList.OrderBy(m => m.Time).ToList();
        //            #endregion



        //            gentelladetList.Add(gentelldet);

        //        }
        //    }

        //    res = JsonConvert.SerializeObject(gentelladetList);

        //    return res;
        //}

        public string GetAxisDetails()
        {

            string res = "";
            DateTime nowdate = DateTime.Now;
            string correctedDate = nowdate.ToString("yyyy-MM-dd");
            //string correctedDate = "2018-10-08";
            if (nowdate.Hour < 7 && nowdate.Hour > 0)
            {
                correctedDate = nowdate.AddDays(-1).ToString("yyyy-MM-dd");
            }

            DateTime correctdate = Convert.ToDateTime(correctedDate);
            List<MachineUTF> MachineUTFList = new List<MachineUTF>();
            var machinedet = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).ToList();
            machinedet = machinedet.OrderBy(m => m.MachineID).ToList();
            var c = 0;
            foreach (var machine in machinedet)
            {

                int machineID = machine.MachineID;

                var machinmodes = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && m.IsDeleted == 0).Select(m => new { m.MacMode, m.ColorCode, m.DurationInSec }).ToList();
                MachineUTF objmachineutf = new MachineUTF();
                objmachineutf.RunningTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "GREEN").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));

                objmachineutf.IdleTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "YELLOW").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
                objmachineutf.BreakDownTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "RED").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));
                objmachineutf.PowerOffTime = Convert.ToInt32(TimeSpan.FromSeconds(Convert.ToDouble(machinmodes.Where(m => m.ColorCode == "BLUE").ToList().Sum(m => m.DurationInSec))).ToString(@"hh"));

                objmachineutf.MachineName = machine.MachineDisplayName;

                MachineUTFList.Add(objmachineutf);


            }
            res = JsonConvert.SerializeObject(MachineUTFList);
            return res;
        }

        public string GetSpindleLoad()
        {
            string res = "";
             DateTime nowdate = DateTime.Now;
            string correctedDate = nowdate.ToString("yyyy-MM-23");
            //string correctedDate = "2018-10-08";
            if (nowdate.Hour < 7 && nowdate.Hour > 0)
            {
                correctedDate = nowdate.AddDays(-1).ToString("yyyy-MM-dd");
            }

            DateTime correctdate = Convert.ToDateTime(correctedDate);
            List<DataProviderAxisBySpindleLoad> SpindleLoadList = new List<DataProviderAxisBySpindleLoad>();
            var machinedet = db.tblmachinedetails.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
            // machinedet = machinedet.OrderBy(m => m.MachineID).ToList();
            //foreach (var machine in machinedet)
            //{
            var spindleload = db.tbl_axisdetails2.Where(m => m.IsDeleted == 0 && m.MachineID == machinedet.MachineID).Select(a => new { a.StartTime, a.SpindleLoad }).Take(30).ToList();
            var c = 0;
            //spindleload.OrderBy(m => m.SpindleLoad);
            foreach (var load in spindleload)
            {
                if (c < 30)
                {
                    DataProviderAxisBySpindleLoad objload = new DataProviderAxisBySpindleLoad();
                    objload.Time = load.StartTime?.ToString(@"hh\:mm\:ss");
                    objload.value = load.SpindleLoad;
                    SpindleLoadList.Add(objload);
                }
                c = c + 1;
            }
            //}

            res = JsonConvert.SerializeObject(SpindleLoadList.OrderBy(m => m.Time));
            return res;
        }
    }
}