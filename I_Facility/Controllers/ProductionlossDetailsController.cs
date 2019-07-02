using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
using System.Data;

namespace I_Facility.Controllers
{
    public class ProductionlossDetailsController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        // GET: ProductionlossDetails
        public ActionResult Index()
        {

            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.UserName = Session["Username"];
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            ProductionLossModel pa = new ProductionLossModel();
            tbllossescode pe = new tbllossescode();
            pa.ProductionLoss = pe;
            pa.ProductionLossList = db.tbllossescodes.Where(m => m.IsDeleted == 0).ToList();
            ViewBag.LossCode = new SelectList(db.tbllossescodes.ToList().Where(p => p.IsDeleted == 0), "LosscodeID", "MessageType").ToList();
            ViewData["Level1"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode").ToList();
            ViewData["Level2"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode").ToList();//Dont take Any Loss to view, as we will display data based on Department and Category.
            return View(pa);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            string Username = Session["Username"].ToString();
            ViewBag.LossCode = new SelectList(db.tbllossescodes.ToList().Where(p => p.IsDeleted == 0), "LosscodeID", "MessageType").ToList();
            ViewData["Level1"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode").ToList();
            ViewData["Level2"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode").ToList();

            return View();
        }
        [HttpPost]
        public ActionResult Create(ProductionLossModel objLossCode, int Level1 = 0, int Level2 = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            int UserID = Convert.ToInt32(Session["UserId"]);
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            objLossCode.ProductionLoss.CreatedBy = UserID;
            objLossCode.ProductionLoss.CreatedOn = DateTime.Now;
            objLossCode.ProductionLoss.IsDeleted = 0;
            objLossCode.ProductionLoss.ServerTabCheck = 1;
            objLossCode.ProductionLoss.ServerTabFlagSync = 0;
            //Check duplicate entry
            var losscodelevel = objLossCode.ProductionLoss.LossCodesLevel;
            var losscode = objLossCode.ProductionLoss.LossCode;
            if (losscodelevel == 1)
            {
                var DuplicateEntry = db.tbllossescodes.Where(m => m.LossCode == losscode && m.IsDeleted == 0).ToList();
                //if (DuplicateEntry != null)
                if(DuplicateEntry.Count == 0)

                {
                    db.tbllossescodes.Add(objLossCode.ProductionLoss);
                    db.SaveChanges();
                    RedirectToAction("Index");
                }
                else
                {
                    ViewData["Level1"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode").ToList();
                    ViewData["Level2"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode").ToList();
                    TempData["Message"] = "Losscode already exists";
                    return View();
                }
            }
            else if (losscodelevel == 2)
            {

                objLossCode.ProductionLoss.LossCodesLevel1ID = Level1;

                var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == objLossCode.ProductionLoss.LossCode && m.LossCodesLevel1ID == objLossCode.ProductionLoss.LossCodesLevel1ID).ToList();
                if (duplosscode.Count == 0)
                {
                    db.tbllossescodes.Add(objLossCode.ProductionLoss);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Level1"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode").ToList();
                    ViewData["Level2"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode").ToList();
                    TempData["Message"] = "Loss code already Exist.";
                    return View(objLossCode);
                }
            }
            else if (losscodelevel == 3)
            {
                objLossCode.ProductionLoss.LossCodesLevel1ID = Level1;
                objLossCode.ProductionLoss.LossCodesLevel2ID = Level2;
                var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == objLossCode.ProductionLoss.LossCode && m.LossCodesLevel1ID == objLossCode.ProductionLoss.LossCodesLevel1ID && m.LossCodesLevel2ID == objLossCode.ProductionLoss.LossCodesLevel2ID).ToList();
                if (duplosscode.Count == 0)
                {
                    db.tbllossescodes.Add(objLossCode.ProductionLoss);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Level1"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode").ToList();
                    ViewData["Level2"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode").ToList();
                    TempData["Message"] = "Loss code already Exist.";
                    return View(objLossCode);
                }
            }
            return RedirectToAction("Index");
        }
        public JsonResult FetchLevel1(string Data)
        {
            var results = (from row in db.tbllossescodes
                           where row.IsDeleted == 0 && row.MessageType == Data
                           select new { Value = row.LossCodeID, Text = row.LossCode });
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchLevel2(string selectedID, int LevelId)
        {
            var results = (from row in db.tbllossescodes
                           where row.IsDeleted == 0 && row.MessageType == selectedID && row.LossCodesLevel1ID == LevelId
                           select new { Value = row.LossCodeID, Text = row.LossCode });
            return Json(results, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchLevel2LosscodesEdit(int LevelId)
        {
            var results = (from row in db.tbllossescodes
                           where row.IsDeleted == 0 && row.LossCodeID == LevelId
                           select new { Value = row.LossCodeID, Text = row.LossCode });
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchLevel1CodesForedit(int PID)
        {
            var LevelResult = (from row in db.tbllossescodes
                               where row.IsDeleted == 0 && row.LossCodeID == PID
                               select new { Value = row.LossCodeID, Text = row.LossCode });
            return Json(LevelResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductionLossById(int PRDRLOID)
        {
            var Data = db.tbllossescodes.Where(m => m.LossCodeID == PRDRLOID).Select(m => new { Lcode = m.LossCode, Lcodedesc = m.LossCodeDesc, messagetype = m.MessageType, LocodeLevel1id = m.LossCodesLevel1ID, LocdeLevel2Id = m.LossCodesLevel2ID, Lcodelevel = m.LossCodesLevel, Contributesto = m.ContributeTo, LossCodeID = m.LossCodeID }).ToList();
            return Json(Data, JsonRequestBehavior.AllowGet);
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
                tbllossescode tblmc = db.tbllossescodes.Find(id);
                if (tblmc == null)
                {
                    return HttpNotFound();
                }
                ViewData["Level1"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode", tblmc.LossCodesLevel1ID).ToList();
                ViewData["Level2"] = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode",tblmc.LossCodesLevel2ID).ToList();
                ProductionLossModel sd = new ProductionLossModel();
                sd.ProductionLoss = tblmc;
                return View(sd);
            }
        }
        [HttpPost]
        public ActionResult Edit(ProductionLossModel tlc, int Level1 = 0, int Level2 = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            int losscodeID = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == tlc.ProductionLoss.LossCodesLevel).Select(m => m.LossCodeID).FirstOrDefault();
            int UserID = Convert.ToInt32(Session["UserId"]);
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            if (Level1 == 0 && Level2 == 0)
            {
                tlc.ProductionLoss.LossCodesLevel = 1;
            }
            else if (Level1 != 0 && Level2 == 0)
            {
                tlc.ProductionLoss.LossCodesLevel = 2;
                tlc.ProductionLoss.LossCodesLevel1ID = Level1;
            }
            else if (Level1 != 0 && Level2 != 0)
            {
                tlc.ProductionLoss.LossCodesLevel = 3;
                tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                tlc.ProductionLoss.LossCodesLevel2ID = Level2;
            }


            tlc.ProductionLoss.ModifiedBy = UserID;
            tlc.ProductionLoss.ModifiedOn = DateTime.Now;
            tlc.ProductionLoss.ServerTabFlagSync = 1;
            tlc.ProductionLoss.ServerTabCheck = 2;

            if (Convert.ToInt16(tlc.ProductionLoss.LossCodesLevel) == 1)
            {
                var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == tlc.ProductionLoss.LossCode && m.LossCodeID != losscodeID && m.LossCodeDesc == tlc.ProductionLoss.LossCodeDesc && m.MessageType == tlc.ProductionLoss.MessageType && m.ContributeTo == tlc.ProductionLoss.ContributeTo).ToList();
                if (duplosscode.Count == 0)
                {

                    db.Entry(tlc.ProductionLoss).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Loss code already Exist.";
                    ViewData["Level1"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0), "LossCodesLevel1ID", "LossCode", tlc.ProductionLoss.LossCodesLevel1ID);
                    ViewData["Level2"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0), "LossCodesLevel2ID", "LossCode", tlc.ProductionLoss.LossCodesLevel2ID);
                    ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                    return View(tlc);
                }
            }

            if (Convert.ToInt16(tlc.ProductionLoss.LossCodesLevel) == 2)
            {
                tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == tlc.ProductionLoss.LossCode && m.LossCodeID != tlc.ProductionLoss.LossCodeID && m.LossCodesLevel1ID == tlc.ProductionLoss.LossCodesLevel1ID).ToList();
                if (duplosscode.Count == 0)
                {
                    db.Entry(tlc.ProductionLoss).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Loss code already Exist.";
                    ViewData["Level1"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0), "LossCodesLevel1ID", "LossCode", tlc.ProductionLoss.LossCodesLevel1ID);
                    ViewData["Level2"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0), "LossCodesLevel2ID", "LossCode", tlc.ProductionLoss.LossCodesLevel2ID);
                    ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;
                    ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                    return View(tlc);
                }
            }
            if (Convert.ToInt16(tlc.ProductionLoss.LossCodesLevel) == 3)
            {
                tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                tlc.ProductionLoss.LossCodesLevel2ID = Level2;
                var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == tlc.ProductionLoss.LossCode && m.LossCodeID != tlc.ProductionLoss.LossCodeID && m.LossCodesLevel1ID == tlc.ProductionLoss.LossCodesLevel1ID && m.LossCodesLevel2ID == tlc.ProductionLoss.LossCodesLevel2ID).ToList();
                if (duplosscode.Count == 0)
                {
                    db.Entry(tlc.ProductionLoss).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "Loss code already Exist.";
                    ViewData["Level1"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0), "LossCodesLevel1ID", "LossCode", tlc.ProductionLoss.LossCodesLevel1ID);
                    ViewData["Level2"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0), "LossCodesLevel2ID", "LossCode", tlc.ProductionLoss.LossCodesLevel2ID);
                    ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;
                    ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                    return View(tlc);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];

            var details = db.tbllossescodes.Find(id);
            int lossLevel = Convert.ToInt32(details.LossCodesLevel);
            int UserID = Convert.ToInt32(Session["UserId"]);
            if (lossLevel == 1)
            {
                var item = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel1ID == id).ToList();
                foreach (var it in item)
                {
                    it.ServerTabFlagSync = 1;
                    it.ServerTabCheck = 2;
                    it.IsDeleted = 1;
                    it.ModifiedBy = UserID;
                    it.ModifiedOn = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            else if (lossLevel == 2)
            {
                var item = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel2ID == id).ToList();
                foreach (var it in item)
                {
                    it.ServerTabFlagSync = 1;
                    it.ServerTabCheck = 2;
                    it.IsDeleted = 1;
                    it.ModifiedBy = UserID;
                    it.ModifiedOn = System.DateTime.Now;
                    db.SaveChanges();
                }
            }


            int UserID1 = id;
            tbllossescode tlc = db.tbllossescodes.Find(id);
            tlc.ServerTabFlagSync = 1;
            tlc.ServerTabCheck = 2;
            tlc.IsDeleted = 1;
            tlc.ModifiedBy = UserID;
            tlc.ModifiedOn = System.DateTime.Now;
            //start Logging

            String Username = Session["Username"].ToString();
            db.Entry(tlc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public string Level2Check(int level1)
        {
            string status = "notok";
            //var doesThisPlantExist = db.tblplants.Where(m => m.IsDeleted == 0 && m.PlantName == plantName).ToList();
            if (level1 == 2)
            {
                status = "First Enter LossCodeLevel1 Details";
            }
            else
            {
                status = "Enter LossCodeLevel2 Details";
            }
            return status;
        }
        [HttpPost]
        public string Level3Check(int level1,int level2)
        {
            string status = "notok";
            var doesThisExist = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel1ID == level1 && m.LossCodesLevel2ID == level2).ToList();
            if (doesThisExist.Count > 0)
            {
                status = "First Enter LossCodeLevel1 and LossCodeLevel2 Details";
            }
            else
            {
                status = "Enter LossCodeLevel3 Details";
            }
            return status;
        }
  

    }
}

