﻿using I_Facility.ServerModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace I_Facility.Controllers
{
    public class SensorDataLinkController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        // GET: SensorDataLink
        public ActionResult Index()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                SensorDataLink pa = new SensorDataLink();
                tblsensordatalink mp = new tblsensordatalink();
                pa.sensordatalink = mp;
                pa.sensordataList = db.tblsensordatalinks.Where(m => m.IsDeleted == 0).ToList();
                return View(pa);
            }
        }
        [HttpGet]
        public ActionResult CreateSensorDataLink()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            ViewBag.FrequencyUnit = new SelectList(db.tblunits.Where(m => m.IsDeleted == 0), "U_id", "Unit").ToList();
            ViewBag.Unit = new SelectList(db.tblunits.Where(m => m.IsDeleted == 0), "U_id", "Unit").ToList();
            ViewBag.Axis = new SelectList(db.tbl_axisdet.Where(m => m.IsDeleted == 0), "AxisDetID", "AxisID").ToList();
            ViewBag.SensorGroupName = new SelectList(db.tblsensorgroups.Where(m => m.IsDeleted == 0), "SID", "SensorGroupName").ToList();

            return View();
        }
       
        public string InsertData(string ParameterName , string ParameterDesc, int IsAxis, int IsSensor, decimal LSL , decimal USL ,int LogFrequency,int axisid, int logfreqid,int unit,string element,string subelement,string Deterioration, int IsCycle)
        {
            string res = "";
            var doesThisPlantExist = db.tblsensordatalinks.Where(m => m.IsDeleted == 0 && m.ParameterName == ParameterName && m.AxisID != axisid).ToList();
            if (doesThisPlantExist.Count == 0)
            {
                
                tblsensordatalink tblpc = new tblsensordatalink();
                tblpc.CreatedBy = 1;
                tblpc.CreatedOn = DateTime.Now;
                tblpc.IsDeleted = 0;
                tblpc.ParameterName = ParameterName;
                tblpc.ParameterDesc = ParameterDesc;
                tblpc.IsCycle = IsCycle;
                tblpc.IsAxis = IsAxis;
                tblpc.IsSensor = IsSensor;
                tblpc.Element = element;
                tblpc.Unit = unit;
                tblpc.SubElement = subelement;
                tblpc.Deterioration = Deterioration;
                tblpc.LSL = LSL;
               tblpc.LogFreqUnitID = logfreqid;
                tblpc.USL = USL;
                tblpc.LogFrequency = LogFrequency;
                tblpc.AxisID = axisid;
                db.tblsensordatalinks.Add(tblpc);
                db.SaveChanges();
                res = "success";
            }
            return res;
        }
        //Get Method
        public ActionResult EditSensorDataLink(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                tblsensordatalink tblmc = db.tblsensordatalinks.Find(id);
                if (tblmc == null)
                {
                    return HttpNotFound();
                }
                ViewBag.FrequencyUnit = new SelectList(db.tblunits.Where(m => m.IsDeleted == 0), "U_id", "Unit", tblmc.LogFreqUnitID).ToList();
                ViewBag.Unit = new SelectList(db.tblunits.Where(m => m.IsDeleted == 0), "U_id", "Unit", tblmc.LogFreqUnitID).ToList();
                ViewBag.Axis = new SelectList(db.tbl_axisdet.Where(m => m.IsDeleted == 0), "AxisDetID", "AxisID", tblmc.AxisID).ToList();
                SensorDataLink sd = new SensorDataLink();
                sd.sensordatalink = tblmc;
                return View(sd);
            }
        }
        [HttpPost]
        public ActionResult EditSensorDataLink(SensorDataLink tblmc)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserID"]);
            string parameterName = tblmc.sensordatalink.ParameterName.ToString();
            var parameterid = tblmc.sensordatalink.ParameterTypeID;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var doesThissensorExist = db.tblsensordatalinks.Where(m => m.IsDeleted == 0 && m.ParameterName == parameterName && m.ParameterDesc== tblmc.sensordatalink.ParameterDesc && m.LSL == tblmc.sensordatalink.LSL && m.LogFrequency == tblmc.sensordatalink.LogFrequency && m.IsAxis == tblmc.sensordatalink.IsAxis && m.IsSensor == tblmc.sensordatalink.IsSensor && m.USL == tblmc.sensordatalink.USL &&  m.LogFrequency == tblmc.sensordatalink.LogFrequency && m.ParameterTypeID != parameterid && m.Element == tblmc.sensordatalink.Element && m.SubElement == tblmc.sensordatalink.SubElement&& m.Deterioration == tblmc.sensordatalink.Deterioration).ToList();
                if (doesThissensorExist.Count == 0)
                {
                    var sensor = db.tblsensordatalinks.Find(tblmc.sensordatalink.ParameterTypeID);
                    sensor.ParameterName = tblmc.sensordatalink.ParameterName;
                    sensor.ParameterDesc = tblmc.sensordatalink.ParameterDesc;
                    sensor.IsCycle = tblmc.sensordatalink.IsCycle;
                    sensor.LSL = tblmc.sensordatalink.LSL;
                    sensor.USL = tblmc.sensordatalink.USL;
                    sensor.IsAxis = tblmc.sensordatalink.IsAxis;
                    sensor.IsSensor = tblmc.sensordatalink.IsSensor;
                    sensor.LogFrequency = tblmc.sensordatalink.LogFrequency;
                    sensor.LogFreqUnitID = tblmc.sensordatalink.LogFreqUnitID;
                    sensor.AxisID = tblmc.sensordatalink.AxisID;
                    sensor.Element = tblmc.sensordatalink.Element;
                    sensor.SubElement = tblmc.sensordatalink.SubElement;
                    sensor.Deterioration = tblmc.sensordatalink.Deterioration;
                    sensor.ModifiedBy = ViewBag.roleid;
                    sensor.ModifiedOn = DateTime.Now;
                    db.Entry(sensor).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "parameter Name already exists";
                    return View(tblmc);
                }
            }
        }
        //Getting all the values stored in database
        public JsonResult GetSensorById(int Id)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var Data = db.tblsensordatalinks.Where(m => m.ParameterTypeID == Id).Select(m => new { paramname = m.ParameterName, paramdesc = m.ParameterDesc, isaxis = m.IsAxis, issensor = m.IsSensor, lsl= m.LSL, usl = m.USL, logfreq = m.LogFrequency, element = m.Element, subelement = m.SubElement, deterioration = m.Deterioration});
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Delete(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID1 = id;
            int UserID = Convert.ToInt32(Session["UserId"]);
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var tblpm = db.tblsensordatalinks.Where(m => m.ParameterTypeID == id).FirstOrDefault();
                tblpm.IsDeleted = 1;
                tblpm.ModifiedBy = UserID;
                tblpm.ModifiedOn = DateTime.Now;
                db.Entry(tblpm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }
        //Checking whether the parameter Name already exists or not
        [HttpPost]
        public string parameternameDuplicateCheck(string parameterName = " ",int axisid = 0)
        {
            string status = "notok";
            var doesThisExist = db.tblsensordatalinks.Where(m => m.IsDeleted == 0 && m.ParameterName == parameterName && m.AxisID != axisid).ToList();
            if (doesThisExist.Count == 0)
            {
                status = "ok";
            }
            else
            {
                status = "notok";
            }
            return status;

        }
    }
}