using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
using System.Data;
using I_Facility.Models;
using Newtonsoft.Json;


namespace I_Facility.Controllers
{
    public class PreventiveMaintainanceSchedulingController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        public ActionResult Index()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            PreventiveMaintainanceScheduling pm = new PreventiveMaintainanceScheduling();

            pm.primitivemaintainancescheduling = db.tblprimitivemaintainanceschedulings.Where(m => m.IsDeleted == 0 && m.Month!=null && m.Week!=null).ToList();

            return View(pm);
        }

        // Get Plant Details
        public string GetPlant()
        {
            string res = "";

            var plant = db.tblplants.Where(p => p.IsDeleted == 0).Select(m => new { m.PlantID, m.PlantName }).ToList();
            res = JsonConvert.SerializeObject(plant);
            return res;
        }

        // Get Shop Details
        public string GetShop(int PlantID)
        {
            string res = "";

            var shop = db.tblshops.Where(p => p.IsDeleted == 0 && p.PlantID == PlantID).Select(m => new { m.ShopID, m.ShopName }).ToList();
            res = JsonConvert.SerializeObject(shop);
            return res;
        }

        //Get Cell Details
        public string GetCell(int ShopID)
        {
            string res = "";

            var cell = db.tblcells.Where(p => p.IsDeleted == 0 && p.ShopID == ShopID).Select(m => new { m.CellID, m.CellName }).ToList();
            res = JsonConvert.SerializeObject(cell);
            return res;
        }

        public ActionResult Create()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.display = 0;
            return View();
        }

        // Get Month Details
        public string GetMonth()
        {
            string res = "";

            var Month = db.monthdatas.Where(p => p.Isdeleted == 0).Select(m => new { m.MonthID, m.Text }).ToList();
            res = JsonConvert.SerializeObject(Month);
            return res;
        }
        //Get Week Details
        public string GetWeek()
        {
            string res = "";

            var Week = db.weekdatas.Where(p => p.Isdeleted == 0).Select(m => new { m.WeekID, m.value }).ToList();
            res = JsonConvert.SerializeObject(Week);
            return res;
        }

        // Get PMS Details based on CELL
        public string GetDetails(int CellID)
        {
            string res = "";
            List<PMSData> pm = new List<PMSData>();
            var MachineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            int count = MachineDetails.Count();
            int i = 1;
            foreach (var item in MachineDetails)
            {

                int macId = Convert.ToInt32(item.MachineID);
                var macName = item.MachineDisplayName;
                List<PMSData> pmsList = new List<PMSData>();
                var doesmachineIDpresent = db.tblprimitivemaintainanceschedulings.Where(m => m.IsDeleted == 0 && m.MachineID == macId).ToList();
                if (doesmachineIDpresent.Count == 0)
                {
                    tblprimitivemaintainancescheduling cp = new tblprimitivemaintainancescheduling();
                    var CellDet = db.tblcells.Where(m => m.CellID == CellID).FirstOrDefault();

                    cp.CellID = CellDet.CellID;
                    cp.plantName = CellDet.tblplant.PlantName;
                    cp.shopname = CellDet.tblshop.ShopName;
                    cp.MachineID = macId;
                    cp.PlantID = CellDet.tblplant.PlantID;
                    cp.ShopID = CellDet.tblshop.ShopID;
                    cp.MachineName = macName;
                    cp.CellName = CellDet.CellName;
                    cp.CreatedBy = 1;
                    cp.CreatedOn = DateTime.Now;
                    cp.IsDeleted = 0;
                    db.tblprimitivemaintainanceschedulings.Add(cp);
                    db.SaveChanges();
                    res = "Saved";
                }
            }
            MachineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            count = MachineDetails.Count();
            ViewBag.item = count;
            i = 1;

            foreach (var item in MachineDetails)
            {
                PMSData obj = new PMSData();
                obj.MachineName = item.MachineDisplayName;
                obj.MachineID = item.MachineID;

                var data = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == item.MachineID && m.IsDeleted == 0).ToList();

                List<PmsDetails> objpmsdet = new List<PmsDetails>();
                foreach (var row in data)
                {
                    PmsDetails pmsdet = new PmsDetails();
                    pmsdet.MachineID = row.MachineID;
                    pmsdet.pmid = row.pmid;
                    pmsdet.MachineName = row.MachineName;
                    pmsdet.month = Convert.ToInt32(row.Month);
                    pmsdet.week = Convert.ToInt32(row.Week);
                    objpmsdet.Add(pmsdet);
                }
                obj.pmsdetailsList = objpmsdet;
                pm.Add(obj);
            }
            res = JsonConvert.SerializeObject(pm);
            return res;
        }



        //public ActionResult Create(int cell = 0, int id = 0)

        //{
        //    i_facilityEntities1 db = new i_facilityEntities1();
        //    if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
        //    {
        //        return RedirectToAction("Login", "Login", null);
        //    }
        //    ViewBag.plant = new SelectList(db.tblplants.Where(p => p.IsDeleted == 0), "PlantID", "PlantName").ToList();

        //    ViewBag.shop = new SelectList(db.tblshops.Where(d => d.IsDeleted == 0), "ShopId", "ShopName").ToList();
        //    ViewBag.cell = new SelectList(db.tblcells.Where(p => p.IsDeleted == 0), "cellID", "cellName").ToList();
        //    ViewBag.Logout = Session["Username"].ToString().ToUpper();
        //    ViewBag.roleid = Session["RoleID"];
        //    String Username = Session["Username"].ToString();
        //    ViewBag.display = 0;
        //    var year = DateTime.Now.Year;
        //    ViewBag.Month = new SelectList(db.monthdatas.Where(p => p.Isdeleted == 0), "MonthID", "Text").ToList();
        //    ViewBag.Week = new SelectList(db.weekdatas.Where(p => p.Isdeleted == 0), "WeekID", "Value").ToList();
        //    ViewBag.Year = year;
        //    List<PMSData> pm = new List<PMSData>();
        //    if (cell != 0)
        //    {

        //        using (i_facilityEntities1 db = new i_facilityEntities1())
        //        {
        //            int plantid = Convert.ToInt32(db.tblcells.Where(m => m.CellID == cell).Select(m => m.PlantID).FirstOrDefault());
        //            string plantname = Convert.ToString(db.tblplants.Where(m => m.PlantID == plantid).Select(m => m.PlantName).FirstOrDefault());
        //            int shopid = Convert.ToInt32(db.tblcells.Where(m => m.CellID == cell).Select(m => m.ShopID).FirstOrDefault());
        //            string shopname = Convert.ToString(db.tblshops.Where(m => m.ShopID == shopid).Select(m => m.ShopName).FirstOrDefault());
        //            var cellname = (from s in db.tblcells where s.CellID == cell select s.CellName).FirstOrDefault();

        //            int machineid = (from s in db.tblmachinedetails where s.IsDeleted == 0 && s.CellID == cell select s.MachineID).FirstOrDefault();
        //            if (machineid == 0)
        //            {
        //                TempData["Message"] = "The selected Cell does not have any machines for this plant and shop,please select another cell";
        //                return View();
        //            }
        //            else
        //            {
        //                var machinename = (from s in db.tblmachinedetails where s.MachineID == machineid select s.MachineName).FirstOrDefault();
        //                var pmsmachinedetails = (from s in db.tblprimitivemaintainanceschedulings where s.IsDeleted == 0 && s.CellID == cell select s.MachineID).ToList();
        //                var MachineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cell).ToList();
        //                int count = MachineDetails.Count();
        //                int i = 1;
        //                foreach (var item in MachineDetails)
        //                {

        //                    int macId = Convert.ToInt32(item.MachineID);
        //                    var macName = item.MachineName;
        //                    List<PMSData> pmsList = new List<PMSData>();
        //                    var doesmachineIDpresent = db.tblprimitivemaintainanceschedulings.Where(m => m.IsDeleted == 0 && m.MachineID == macId).ToList();
        //                    if (doesmachineIDpresent.Count == 0)
        //                    {
        //                        tblprimitivemaintainancescheduling cp = new tblprimitivemaintainancescheduling();

        //                        cp.CellID = cell;
        //                        cp.plantName = plantname;
        //                        cp.shopname = shopname;
        //                        cp.MachineID = macId;
        //                        cp.PlantID = plantid;
        //                        cp.ShopID = shopid;
        //                        cp.MachineName = macName;
        //                        cp.CellName = cellname;
        //                        cp.CreatedBy = 1;
        //                        cp.CreatedOn = DateTime.Now;
        //                        cp.IsDeleted = 0;
        //                        db.tblprimitivemaintainanceschedulings.Add(cp);
        //                        db.SaveChanges();
        //                        if (i == count)
        //                        {
        //                            return View("Index");
        //                        }
        //                        i++;
        //                    }
        //                }

        //                MachineDetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cell).ToList();
        //                count = MachineDetails.Count();
        //                ViewBag.item = count;
        //                i = 1;

        //                foreach (var item in MachineDetails)
        //                {

        //                    PMSData obj = new PMSData();
        //                    obj.MachineName = item.MachineName;
        //                    obj.MachineID = item.MachineID;

        //                    var data = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == item.MachineID && m.IsDeleted == 0).ToList();

        //                    List<PmsDetails> objpmsdet = new List<PmsDetails>();
        //                    foreach (var row in data)
        //                    {
        //                        PmsDetails pmsdet = new PmsDetails();
        //                        pmsdet.MachineID = row.MachineID;
        //                        pmsdet.pmid = row.pmid;
        //                        pmsdet.MachineName = row.MachineName;
        //                        pmsdet.month = Convert.ToInt32(row.Month);
        //                        pmsdet.week = Convert.ToInt32(row.Week);
        //                        pmsdet.selectMonth = new SelectList(db.monthdatas.Where(p => p.Isdeleted == 0), "MonthID", "Text", row.MachineID);
        //                        pmsdet.selectweek = new SelectList(db.weekdatas.Where(p => p.Isdeleted == 0), "WeekID", "Value");
        //                        objpmsdet.Add(pmsdet);
        //                    }
        //                    obj.pmsdetailsList = objpmsdet;
        //                    pm.Add(obj);
        //                }
        //                var details = (from s in db.tblprimitivemaintainanceschedulings where s.CellID == cell select s.plantName).FirstOrDefault();
        //                var dat = (from s in db.tblprimitivemaintainanceschedulings where s.CellID == cell select s.CellName).FirstOrDefault();
        //                var dat1 = (from s in db.tblprimitivemaintainanceschedulings where s.CellID == cell select s.shopname).FirstOrDefault();
        //                ViewBag.item = dat.ToString();
        //                ViewBag.item1 = dat1.ToString();
        //                ViewBag.item2 = details.ToString();
        //                ViewBag.display = 1;
        //            }
        //        }
        //        return View(pm);
        //    }
        //}

        //Inserting the Data into database when user click on + button
        public string InsertData(int id, int monthValue, int weekvalue)
        {
            string res = "";

            //var prevenid = db.tblprimitivemaintainanceschedulings.Where(m => m.IsDeleted == 0).Select(m => m.MachineID).ToList();
            //foreach (var items in prevenid)
            //{
            //    var mac1id = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == items && m.IsDeleted == 0).ToList();

            //    foreach (var item in mac1id)
            //    {
            //        if (item.MachineID == id)
            //        {
            //            //if (mac1id.Count > 1)
            //            //{

            //            var tblpm = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == id && m.IsDeleted == 0).FirstOrDefault();
            //            tblprimitivemaintainancescheduling cp1 = new tblprimitivemaintainancescheduling();
            //            cp1.CellID = tblpm.CellID;
            //            cp1.plantName = tblpm.plantName;
            //            cp1.shopname = tblpm.shopname;
            //            cp1.MachineID = tblpm.MachineID;
            //            cp1.PlantID = tblpm.PlantID;
            //            cp1.ShopID = tblpm.ShopID;
            //            cp1.MachineName = tblpm.MachineName;
            //            cp1.CellName = tblpm.CellName;
            //            cp1.CreatedBy = 1;
            //            cp1.CreatedOn = DateTime.Now;
            //            cp1.IsDeleted = 0;
            //            cp1.Month = monthValue;
            //            cp1.Week = weekvalue;
            //            db.tblprimitivemaintainanceschedulings.Add(cp1);
            //            db.SaveChanges();
            //            break;
            //            res = "Success";
            //            return res;
                        
            //        }
            //        else //if (item.pmid == id)
            //        {

            //        }
            //    }
            //}
                var macid = db.tblprimitivemaintainanceschedulings.Where(m => m.pmid == id && m.IsDeleted == 0).FirstOrDefault();
                var tblpm1 = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == macid.MachineID && m.IsDeleted == 0).FirstOrDefault();
                macid.Month = monthValue;
                macid.Week = weekvalue;
                db.Entry(macid).State = EntityState.Modified;
                db.SaveChanges();
                tblprimitivemaintainancescheduling cp = new tblprimitivemaintainancescheduling();
                cp.CellID = tblpm1.CellID;
            cp.plantName = tblpm1.plantName;
            cp.shopname = tblpm1.shopname;
            cp.MachineID = tblpm1.MachineID;
                cp.PlantID = tblpm1.PlantID;
                cp.ShopID = tblpm1.ShopID;
            cp.MachineName = tblpm1.MachineName;
            cp.CellName = tblpm1.CellName;
            cp.CreatedBy = 1;
                cp.CreatedOn = DateTime.Now;
                cp.IsDeleted = 0;
                cp.Month = null;
                cp.Week = null;
                db.tblprimitivemaintainanceschedulings.Add(cp);
                db.SaveChanges();
                res = "Success";
                return res;
        }

        public string InsertData1(/*int pmid,*/int macid, int monthValue, int weekvalue)
        {
            string res = "";
            //var macid1 = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == macid && m.IsDeleted == 0 && m.pmid == pmid).FirstOrDefault();
            var tblpm1 = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == macid && m.IsDeleted == 0).FirstOrDefault();
            //macid1.Month = monthValue;
            //macid1.Week = weekvalue;
            //db.Entry(macid1).State = EntityState.Modified;
            //db.SaveChanges();
            tblprimitivemaintainancescheduling cp = new tblprimitivemaintainancescheduling();
            cp.CellID = tblpm1.CellID;
            cp.plantName = tblpm1.plantName;
            cp.shopname = tblpm1.shopname;
            cp.MachineID = tblpm1.MachineID;
            cp.PlantID = tblpm1.PlantID;
            cp.ShopID = tblpm1.ShopID;
            cp.MachineName = tblpm1.MachineName;
            cp.CellName = tblpm1.CellName;
            cp.CreatedBy = 1;
            cp.CreatedOn = DateTime.Now;
            cp.IsDeleted = 0;
            cp.Month = monthValue;
            cp.Week = weekvalue;
            db.tblprimitivemaintainanceschedulings.Add(cp);
            db.SaveChanges();
            res = "Success";
            return res;
        }

        //Saving all the Data into database without click on + button
        public string Save(int id, int mon, int wee)
        {
            string res = "";
            var macid = db.tblmachinedetails.Where(m => m.IsDeleted == 0).Select(m => m.MachineID).ToList();
            foreach (var items in macid)
            {
                var mac1id = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == items && m.IsDeleted == 0).ToList();

                foreach (var item in mac1id)
                {
                    if (item.MachineID == id)
                    {
                        var tblpm = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == id && m.IsDeleted == 0).FirstOrDefault();
                        tblprimitivemaintainancescheduling cp = new tblprimitivemaintainancescheduling();
                        cp.CellID = tblpm.CellID;
                        cp.plantName = tblpm.plantName;
                        cp.shopname = tblpm.shopname;
                        cp.MachineID = tblpm.MachineID;
                        cp.PlantID = tblpm.PlantID;
                        cp.ShopID = tblpm.ShopID;
                        cp.MachineName = tblpm.MachineName;
                        cp.CellName = tblpm.CellName;
                        cp.CreatedBy = 1;
                        cp.CreatedOn = DateTime.Now;
                        cp.IsDeleted = 0;
                        cp.Month = mon;
                        cp.Week = wee;
                        db.tblprimitivemaintainanceschedulings.Add(cp);
                        db.SaveChanges();
                        res = "Success";
                        return res;
                    }
                    else if (item.pmid == id)
                    {
                        var pmid = db.tblprimitivemaintainanceschedulings.Where(m => m.pmid == id && m.IsDeleted == 0).FirstOrDefault();
                        var tblpm = db.tblprimitivemaintainanceschedulings.Where(m => m.MachineID == pmid.MachineID && m.IsDeleted == 0).FirstOrDefault();
                        pmid.Month = mon;
                        pmid.Week = wee;
                        db.Entry(pmid).State = EntityState.Modified;
                        db.SaveChanges();
                        tblprimitivemaintainancescheduling cp = new tblprimitivemaintainancescheduling();
                        cp.CellID = tblpm.CellID;
                        cp.plantName = tblpm.plantName;
                        cp.shopname = tblpm.shopname;
                        cp.MachineID = tblpm.MachineID;
                        cp.PlantID = tblpm.PlantID;
                        cp.ShopID = tblpm.ShopID;
                        cp.MachineName = tblpm.MachineName;
                        cp.CellName = tblpm.CellName;
                        cp.CreatedBy = 1;
                        cp.CreatedOn = DateTime.Now;
                        cp.IsDeleted = 0;
                        cp.Month = null;
                        cp.Week = null;
                        db.tblprimitivemaintainanceschedulings.Add(cp);
                        db.SaveChanges();
                        res = "Success";
                    }
                }
            }
            return res;
        }
        //Deleting the record
        public ActionResult Delete(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            int UserID = Convert.ToInt32(Session["UserId"]);
            String Username = Session["Username"].ToString();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                tblprimitivemaintainancescheduling tblpm = db.tblprimitivemaintainanceschedulings.Find(id);
                tblpm.IsDeleted = 1;
                tblpm.ModifiedBy = UserID;
                tblpm.ModifiedOn = DateTime.Now;
                db.Entry(tblpm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        //Deleting the record when user click on - button
        public string DeleteData(int Id)
        {
            string res = "";
            var tblpm = db.tblprimitivemaintainanceschedulings.Where(m => m.pmid == Id && m.IsDeleted == 0).FirstOrDefault();
            tblpm.IsDeleted = 1;
            tblpm.ModifiedBy = 1;
            tblpm.ModifiedOn = DateTime.Now;
            db.Entry(tblpm).State = EntityState.Modified;
            db.SaveChanges();
            res = "Success";
            return res;
        }

        //Updating the Month value
        public string UpdateData(int pmid, int monthvalue)
        {
            string res = "";
            var tblpm = db.tblprimitivemaintainanceschedulings.Where(m => m.pmid == pmid && m.IsDeleted == 0).FirstOrDefault();
            tblpm.Month = monthvalue;
            db.Entry(tblpm).State = EntityState.Modified;
            db.SaveChanges();
            res = "Success";
            return res;
        }

        //Updating the Week Value
        public string UpdateData1(int pmid, int weekvalue)
        {
            string res = "";

            var tblpm = db.tblprimitivemaintainanceschedulings.Where(m => m.pmid == pmid && m.IsDeleted == 0).FirstOrDefault();
            tblpm.Week = weekvalue;
            db.Entry(tblpm).State = EntityState.Modified;
            db.SaveChanges();
            res = "Success";
            return res;
        }
        //Checking whether the cell Name is present in Machine Table or not
        [HttpPost]
        public string cellnameDuplicateCheck(int cellid)
        {
            string status = "notok";
            var doesThisExist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).ToList();
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