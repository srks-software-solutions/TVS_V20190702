using System;
using I_Facility.ServerModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.Models;
using Newtonsoft.Json;
using I_Facility.ServerModelLive;
namespace I_Facility.Controllers
{

    public class MachineHealthController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();

        i_facility_liveEntities livedb = new i_facility_liveEntities();
        // GET: MachineHealth
        public ActionResult Index()
        {
            var sensormachine = db.tblmachinesensors.Where(m => m.IsDeleted == 0).Select(m => m.MachineId).Distinct().ToList();
            foreach (var sensormachinedet in sensormachine)
            {
                var plant = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == sensormachinedet).Select(m => m.PlantID).ToList();
                foreach (var plantdet in plant)
                {
                    ViewData["Plant"] = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0 && m.PlantID == plantdet), "PlantID", "PlantName");
                }
                ViewData["Shop"] = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopID", "ShopName");
                ViewData["Cell"] = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName");
                ViewData["Machine"] = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0), "MachineID", "MachineName");
                ViewData["Parameter"] = new SelectList(db.tblsensormasters.Where(m => m.IsDeleted == 0), "SMID", "SensorDesc");
            }
            return View();

        }
        public string FetchShop(int PID)
        {
            string res = "";
            var sensormachine = db.tblmachinesensors.Where(m => m.IsDeleted == 0).Select(m => m.MachineId).Distinct().ToList();
            foreach (var sensormachinedet in sensormachine)
            {
                var shop = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == sensormachinedet).Select(m => m.ShopID).ToList();
                foreach (var shopdet in shop)
                {
                    var shopdetails = (from s in db.tblshops where s.PlantID == PID && s.IsDeleted == 0 && s.ShopID == shopdet select new { Value = s.ShopID, Text = s.Shopdisplayname }).ToList();
                    res = JsonConvert.SerializeObject(shopdetails);
                }
                
            }
            return res;
        }
        public string Fetchcell(int SID)
        {
            string res = "";
            var sensormachine = db.tblmachinesensors.Where(m => m.IsDeleted == 0).Select(m => m.MachineId).Distinct().ToList();
            foreach (var sensormachinedet in sensormachine)
            {
                var cell = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == sensormachinedet).Select(m => m.CellID).ToList();
                foreach (var celldet in cell)
                {
                    var celldetails = (from s in db.tblcells where s.ShopID == SID && s.IsDeleted == 0 && s.CellID == celldet select new { Value = s.CellID, Text = s.CelldisplayName }).ToList();
                    res = JsonConvert.SerializeObject(celldetails);
                }
            }
            return res;
        }
        public string FetchMachine(int CID)
        {
            string res = "";
            var sensormachine = db.tblmachinesensors.Where(m => m.IsDeleted == 0).Select(m => m.MachineId).Distinct().ToList();
            foreach (var sensormachinedet in sensormachine)
            {
                tblmachinedetail obj = new tblmachinedetail();
                var machinedet = (from m in db.tblmachinedetails where m.IsDeleted == 0 && m.CellID == CID && m.MachineID == sensormachinedet select new { Value = m.MachineID, Text = m.MachineDisplayName }).ToList();
                res = JsonConvert.SerializeObject(machinedet);
            }
            return res;
            
        }
        public string Fetchsensor(int MID)
        {
            string res = "";
            List<cbmparametermodel> cbmlist = new List<cbmparametermodel>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var sensordata = (from sensor in db.tblmachinesensors
                                  where sensor.MachineId == MID
                                  select sensor.Sid).ToList();
                foreach(var sensordatarow in sensordata )
                {
                   
                    var sensormasterdata = (from sensormaster in db.tblsensormasters
                                            where sensormaster.Sid == sensordatarow
                                            select new
                                            {
                                                Value = sensormaster.SMID,
                                                Text = sensormaster.SensorDesc
                                            }).ToList();
                    
                    foreach(var item in sensormasterdata)
                    {
                        cbmparametermodel obj = new cbmparametermodel();
                        obj.Value = item.Value;
                        obj.Text = item.Text;
                        cbmlist.Add(obj);

                    }
                   

                }
                res = JsonConvert.SerializeObject(cbmlist);
                return res;
            }
        }
        public string GetParameters(int id)
        {

            string res = " ";
            // int count = 3;
           string correctedDate = GetCorrectedDate();
           // string correctedDate = "2019-03-20";
            List<MachineHealth> machineHealthList = new List<MachineHealth>();
            MachineHealth objmahchinehealth = new MachineHealth();
            var sensormasterrow = db.tblsensormasters.Where(m => m.SMID == id).Select(m => new { m.Sid, m.MemoryAddress, m.parametertypeid }).FirstOrDefault();

            List<Health> cbmList = new List<Health>();
            //foreach (var sensormasterrow in sensormasterdata)
            //{
            var memadd = Convert.ToInt32(sensormasterrow.MemoryAddress);

            var sensordatalinkdata = db.tblsensordatalinks.Where(m => m.ParameterTypeID == sensormasterrow.parametertypeid).FirstOrDefault();
            objmahchinehealth.LSL = sensordatalinkdata.LSL;
            objmahchinehealth.USL = sensordatalinkdata.USL;
            var data1 = livedb.tbl_livecbmparameters.Where(m => m.SensorGroupID == sensormasterrow.Sid && m.MemoryAddress == memadd && m.CorrectedDate == correctedDate).OrderBy(m => m.cbmpID).ToList().Take(30).ToList();
            foreach (var row in data1)
            {
                Health objvalue = new Health();
                objvalue.value = row.Values;
                objvalue.Time = row.CreatedOn?.ToString(@"hh\:mm\:ss");
                cbmList.Add(objvalue);
            }

            objmahchinehealth.MachineHealthdet = cbmList;

            res = JsonConvert.SerializeObject(objmahchinehealth);

            return res;
        }
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
        
    }

}