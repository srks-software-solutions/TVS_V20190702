using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
using System.Data;
using I_Facility.Models;
using System.Data.Entity.Validation;

namespace I_Facility.Controllers
{
    public class PmCheckListController : Controller
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
            PmCheckList tblpm = new PmCheckList();
            tblpm.pmchecklistlist = db.tblpmchecklists.Where(m => m.Isdeleted == 0).ToList();

            return View(tblpm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.plant = new SelectList(db.tblplants.ToList().Where(p => p.IsDeleted == 0), "PlantID", "PlantName").ToList();
            ViewBag.shop = new SelectList(db.tblshops.ToList().Where(d => d.IsDeleted == 0), "ShopId", "ShopName").ToList();
            ViewBag.cell = new SelectList(db.tblcells.ToList().Where(d => d.IsDeleted == 0), "CellID", "CellName").ToList();
            ViewBag.TypeOfCheckPoint = new SelectList(db.tblpmcheckpoints.ToList().Where(d => d.Isdeleted == 0), "pmcpID", "TypeOfCheckPoint").ToList();
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            return View();
        }


        public JsonResult InsertData(int Plant, int shop, int cell, string Value, string CheckList, string How, string Frequency, int TypeOfCheckPoint)
        {
            tblpmchecklist tblpc = new tblpmchecklist();
            tblpc.CreatedBy = 1;
            tblpc.CreatedOn = DateTime.Now;
            tblpc.Isdeleted = 0;
            tblpc.PlantID = Plant;
            tblpc.ShopID = shop;
            tblpc.CellID = cell;
            tblpc.How = How;
            tblpc.Frequency = Frequency;
            tblpc.Value = Value;
            tblpc.pmcpID = TypeOfCheckPoint;
            tblpc.CheckList = CheckList;
            db.tblpmchecklists.Add(tblpc);
            db.SaveChanges();

            return Json(tblpc.pmcid, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
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
                var pmd = db.tblpmchecklists.Where(m => m.pmcid == id).Select(m => m.pmcpID).FirstOrDefault();
                var tblpm = db.tblpmchecklists.Where(m => m.pmcid == id).FirstOrDefault();

                var tbl = db.tblpmchecklists.Where(m => m.pmcid == id || m.pmcpID == pmd && m.Isdeleted == 0).ToList();
                if (tbl == null)
                {
                    return HttpNotFound();
                }
                List<PmCheckList> obj = new List<PmCheckList>();
                foreach (var item in tbl)
                {
                    PmCheckList pm1 = new PmCheckList();
                    ViewBag.plant = new SelectList(db.tblplants.ToList().Where(p => p.IsDeleted == 0), "PlantID", "PlantName", item.PlantID).ToList();
                    ViewBag.shop = new SelectList(db.tblshops.ToList().Where(d => d.IsDeleted == 0), "ShopId", "ShopName", item.ShopID).ToList();
                    ViewBag.cell = new SelectList(db.tblcells.ToList().Where(d => d.IsDeleted == 0), "CellID", "CellName", item.CellID).ToList();
                    ViewBag.TypeOfCheckPoint = new SelectList(db.tblpmcheckpoints.ToList().Where(d => d.Isdeleted == 0), "pmcpID", "TypeOfCheckPoint", item.pmcpID).ToList();
                    pm1.pmchecklist = item;
                    obj.Add(pm1);
                }
                return View(obj);
            }
        }
        public string update(int plant, int shop, int cell, string value, string frequency, int pmcpid, int pmcid, string checklist, string How)
        {
            string res = "";
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var tblpc = db.tblpmchecklists.Find(pmcid);

                tblpc.Isdeleted = 0;
                tblpc.PlantID = plant;
                tblpc.ShopID = shop;
                tblpc.CellID = cell;
                tblpc.pmcpID = pmcpid;
                tblpc.How = How;
                tblpc.Value = value;
                tblpc.Frequency = frequency;
                tblpc.CheckList = checklist;
                tblpc.ModifiedBy = 1;
                tblpc.ModifiedOn = DateTime.Now;
                db.Entry(tblpc).State = EntityState.Modified;
                db.SaveChanges();
                res = "Success";
                return res;
            }
        }

        public string update1(int plant, int shop, int cell, string value, string frequency, int pmcpid, int pmcid, string checklist, string How)
        {
            string res = "";
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var tblpc = db.tblpmchecklists.Find(pmcid);

                tblpc.Isdeleted = 0;
                tblpc.PlantID = plant;
                tblpc.ShopID = shop;
                tblpc.CellID = cell;
                tblpc.pmcpID = pmcpid;
                tblpc.Value = value;
                tblpc.How = How;
                tblpc.Frequency = frequency;
                tblpc.CheckList = checklist;
                tblpc.ModifiedBy = 1;
                tblpc.ModifiedOn = DateTime.Now;
                db.Entry(tblpc).State = EntityState.Modified;
                db.SaveChanges();
                res = "Success";
                return res;
            }
        }

        public JsonResult GetCheckListById(int Id)
        {
            var Data = db.tblpmchecklists.Where(m => m.pmcid == Id).Select(m => new { cell = m.CellID, plant = m.PlantID, shop = m.ShopID });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
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
                var tblpm = db.tblpmchecklists.Where(m => m.pmcid == id).FirstOrDefault();
                tblpm.Isdeleted = 1;
                tblpm.ModifiedBy = UserID;
                tblpm.ModifiedOn = DateTime.Now;
                db.Entry(tblpm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        public JsonResult DeleteData(int id = 0)
        {
            var tblpm = db.tblpmchecklists.Where(m => m.pmcid == id).FirstOrDefault();
            tblpm.Isdeleted = 1;
            tblpm.ModifiedBy = 1;
            tblpm.ModifiedOn = DateTime.Now;
            db.Entry(tblpm).State = EntityState.Modified;
            db.SaveChanges();
            return Json(tblpm.pmcid, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchCheckPoint(int CID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var checkPointData = (from row in db.tblpmcheckpoints
                                      where row.Isdeleted == 0 && row.CellID == CID
                                      select new { Value = row.pmcpID, Text = row.TypeofCheckpoint }).ToList();
                return Json(checkPointData, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FetchCell(int SID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var checkPointData = (from row in db.tblcells
                                      where row.IsDeleted == 0 && row.ShopID == SID
                                      select new { Value = row.CellID, Text = row.CellName }).ToList();
                return Json(checkPointData, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult FetchShop(int PID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var checkPointData = (from row in db.tblshops
                                      where row.IsDeleted == 0 && row.PlantID == PID
                                      select new { Value = row.ShopID, Text = row.ShopName }).ToList();
                return Json(checkPointData, JsonRequestBehavior.AllowGet);
            }
        }
    }

}