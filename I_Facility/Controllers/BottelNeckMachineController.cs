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
    public class BottelNeckMachineController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        // GET: BottelNeckMachine
        public ActionResult Index()
        {

            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.plantID = new SelectList(db.tblplants.Where(p => p.IsDeleted == 0), "PlantID", "PlantName").ToList();
            ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopID", "ShopName").ToList();

            ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellId", "CellName").ToList();
            ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0), "MachineID", "MachineDisplayName").ToList();
            ViewBag.pid = new SelectList(db.tblcellparts.Where(d => d.IsDeleted == 0), "cpID", "partNo").ToList();
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            //using (i_facilityEntities1 db = new i_facilityEntities1())
            //{
                BottelneckMachine pms = new BottelneckMachine();
                tblbottelneck mp = new tblbottelneck();
                pms.BottelNeck = mp;
                pms.BottelNeckList = db.tblbottelnecks.Where(m => m.IsDeleted == 0).ToList();
                return View(pms);
            //}

        }

        [HttpPost]
        public ActionResult Create(BottelneckMachine tblpc, int plantID = 0, int CellID = 0, int ShopID = 0, int MachineID = 0, int Pid = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.plantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblpc.BottelNeck.plantID);
            ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", tblpc.BottelNeck.ShopID);
            ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", tblpc.BottelNeck.CellID);
            ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(p => p.IsDeleted == 0), "MachineID", "MachineDisplayName", tblpc.BottelNeck.MachineID);
            ViewBag.pid = new SelectList(db.tblcellparts.Where(d => d.IsDeleted == 0), "cpID", "partNo", tblpc.BottelNeck.cpid);

            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            string machineName = tblpc.BottelNeck.ToString();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var doesThisPlantExist = db.tblbottelnecks.Where(m => m.IsDeleted == 0 && m.plantID == plantID && m.CellID == CellID && m.ShopID == ShopID && m.MachineName == machineName).ToList();
                if (doesThisPlantExist.Count == 0)
                {


                    string plantname = db.tblplants.Where(m => m.PlantID == plantID).FirstOrDefault().PlantName;
                    string shopname = db.tblshops.Where(m => m.ShopID == ShopID).FirstOrDefault().ShopName;
                    string cellname = db.tblcells.Where(m => m.CellID == CellID).FirstOrDefault().CellName;
                    string machinename = db.tblmachinedetails.Where(m => m.MachineID == MachineID).FirstOrDefault().MachineName;
                    var noOfParts = db.tblcellparts.Where(m => m.cpID == Pid).FirstOrDefault().partNo;
                    tblpc.BottelNeck.CreatedBy = 1;
                    tblpc.BottelNeck.CreatedOn = DateTime.Now;
                    tblpc.BottelNeck.IsDeleted = 0;
                    tblpc.BottelNeck.ShopID = ShopID;
                    tblpc.BottelNeck.plantID = plantID;
                    tblpc.BottelNeck.CellID = CellID;
                    tblpc.BottelNeck.MachineID = MachineID;
                    tblpc.BottelNeck.cpid = Pid;
                    tblpc.BottelNeck.PlantName = plantname;
                    tblpc.BottelNeck.ShopName = shopname;
                    tblpc.BottelNeck.CellName = cellname;
                    tblpc.BottelNeck.MachineName = machinename;
                    //tblpc.BottelNeck.PartNo = noOfParts;
                    db.tblbottelnecks.Add(tblpc.BottelNeck);
                    db.SaveChanges();

                    // Your code...
                    // Could also be before try if you know the exception occurs in SaveChanges
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.plantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblpc.BottelNeck.plantID);
                    ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", tblpc.BottelNeck.ShopID);
                    ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", tblpc.BottelNeck.CellID);
                    ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(p => p.IsDeleted == 0), "MachineID", "MachineDisplayName", tblpc.BottelNeck.MachineID);
                    ViewBag.pid = new SelectList(db.tblcellparts.Where(d => d.IsDeleted == 0), "cpID", "partNo", tblpc.BottelNeck.cpid);
                    Session["Error"] = "machine Name" + " " + tblpc.BottelNeck.MachineName + " already Exists.";
                    return View(tblpc);
                }
            }

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
                tblbottelneck tblpm = db.tblbottelnecks.Find(id);
                if (tblpm == null)
                {
                    return HttpNotFound();
                }

                ViewBag.plantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblpm.plantID).ToList();
                ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", tblpm.ShopID).ToList();
                ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", tblpm.CellID).ToList();
                ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(p => p.IsDeleted == 0), "MachineID", "MachineDisplayName", tblpm.MachineID).ToList();
                ViewBag.pid = new SelectList(db.tblcellparts.Where(d => d.IsDeleted == 0), "cpID", "partNo", tblpm.cpid);
                return View(tblpm);
            }
        }
        [HttpPost]
        public ActionResult Edit(BottelneckMachine tblmp, int plantID = 0, int CellID = 0, int ShopID = 0, int MachineID = 0, int Pid = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.plantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblmp.BottelNeck.plantID).ToList();
            ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", tblmp.BottelNeck.ShopID).ToList();
            ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", tblmp.BottelNeck.CellID).ToList();
            ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(p => p.IsDeleted == 0), "MachineID", "MachineDisplayName", tblmp.BottelNeck.MachineID).ToList();
            ViewBag.pid = new SelectList(db.tblcellparts.Where(d => d.IsDeleted == 0), "cpID", "partNo", tblmp.BottelNeck.cpid).ToList();
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserID"]);
            string MachineName = Convert.ToString(tblmp.BottelNeck.MachineName);
            int BId = tblmp.BottelNeck.Bid;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var doesThisPlantExist = db.tblbottelnecks.Where(m => m.IsDeleted == 0 && m.plantID == plantID && m.ShopID == ShopID && m.CellID == CellID && m.MachineName == MachineName && m.Bid != BId).ToList();
                if (doesThisPlantExist.Count == 0)
                {
                    string plantname = db.tblplants.Where(m => m.PlantID == plantID).FirstOrDefault().PlantName;
                    string shopname = db.tblshops.Where(m => m.ShopID == ShopID).FirstOrDefault().ShopName;
                    string cellname = db.tblcells.Where(m => m.CellID == CellID).FirstOrDefault().CellName;
                    string machinename = db.tblmachinedetails.Where(m => m.MachineID == MachineID).FirstOrDefault().MachineName;
                    int noOfParts = int.Parse(db.tblcellparts.Where(m => m.cpID == Pid).FirstOrDefault().partNo);
                    var pms = db.tblbottelnecks.Find(BId);
                    pms.CellName = cellname;
                    pms.MachineName = machinename;
                    pms.PlantName = plantname;
                    pms.ShopName = shopname;
                    //pms.PartNo = noOfParts;
                    pms.plantID = plantID;
                    pms.ShopID = ShopID;
                    pms.CellID = CellID;
                    pms.MachineID = MachineID;
                    pms.cpid = Pid;
                    pms.ModifiedBy = 1;
                    pms.ModifiedOn = DateTime.Now;
                    db.Entry(pms).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.plantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblmp.BottelNeck.plantID);
                    ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", tblmp.BottelNeck.ShopID);
                    ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", tblmp.BottelNeck.CellID);
                    ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(p => p.IsDeleted == 0), "MachineID", "MachineDisplayName", tblmp.BottelNeck.MachineID);
                    ViewBag.Pid = new SelectList(db.tblcellparts.Where(d => d.IsDeleted == 0), "cpID", "partNo", tblmp.BottelNeck.cpid);
                    Session["Error"] = "Machine Name" + " " + tblmp.BottelNeck.MachineName + " " + "already Exists.";
                    return View(tblmp);
                }
            }
        }

        public ActionResult BottelnecksCategoryById(int Id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            var Data = db.tblbottelnecks.Where(m => m.Bid == Id).Select(m => new { PlantId = m.plantID, DepartmentId = m.ShopID, cellID = m.CellID, machineid = m.MachineID, pid = m.cpid }).ToList();
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
                tblbottelneck tblpm = db.tblbottelnecks.Find(id);
                tblpm.IsDeleted = 1;
                tblpm.ModifiedBy = UserID;
                tblpm.ModifiedOn = DateTime.Now;
                db.Entry(tblpm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        public JsonResult FetchShop(int PID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var ShopData = (from row in db.tblshops
                                where row.IsDeleted == 0 && row.PlantID == PID
                                select new { Value = row.ShopID, Text = row.Shopdisplayname }).ToList();
                return Json(ShopData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FetchCell(int PID, int SID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var CellData = (from row in db.tblcells
                                where row.IsDeleted == 0 && row.PlantID == PID && row.ShopID == SID
                                select new { Value = row.CellID, Text = row.CelldisplayName }).ToList();
                return Json(CellData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FetchMachine(int PID, int SID, int CID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var MachineData = (from row in db.tblmachinedetails
                                   where row.IsDeleted == 0 && row.PlantID == PID && row.ShopID == SID && row.CellID == CID
                                   select new { Value = row.MachineID, Text = row.MachineDisplayName }).ToList();
                return Json(MachineData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FetchPartNo( int CID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var PartData = (from row in db.tblcellparts
                                where row.IsDeleted == 0 && row.CellID == CID
                                select new { Value = row.cpID, Text = row.partNo }).ToList();
                return Json(PartData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult FetchCellByCid(int CID)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var CellData = (from row in db.tblcells
                                where row.IsDeleted == 0 && row.CellID == CID
                                select new { Value = row.CellID, Text = row.CelldisplayName }
                                );
                return Json(CellData, JsonRequestBehavior.AllowGet);
            }
        }
    }
}