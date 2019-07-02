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
    public class ProductionLossController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();

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
            ViewBag.LossCodeLevel1ID = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode").ToList();
            ViewBag.LossCodeLevel2ID = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode").ToList();//Dont take Any Loss to view, as we will display data based on Department and Category.
            return View(pa);
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
            var losscode = objLossCode.ProductionLoss.LossCode;
            var DuplicateEntry = db.tbllossescodes.Where(m => m.LossCode == losscode && m.IsDeleted == 0 && m.MessageType == objLossCode.ProductionLoss.MessageType && m.LossCodesLevel == objLossCode.ProductionLoss.LossCodesLevel && m.LossCodesLevel1ID == objLossCode.ProductionLoss.LossCodesLevel1ID && m.LossCodesLevel2ID == objLossCode.ProductionLoss.LossCodesLevel2ID).ToList();
            if (DuplicateEntry.Count == 0)

            {
                db.tbllossescodes.Add(objLossCode.ProductionLoss);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                Session["Error"] = "Losscode already exists";
            }
            return View();
        }



        public JsonResult FetchLevel2Losscodes(int LevelId)
        {
            var results = (from row in db.tbllossescodes
                           where row.IsDeleted == 0 && row.LossCodesLevel1ID == LevelId
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

        public JsonResult GetBreakdowns(string BREAKDOWN)
        {
            var ShopData = (from row in db.tbllossescodes
                            where row.IsDeleted == 0 && row.MessageType == "BREAKDOWN" && row.LossCodesLevel == 1
                            select new { Value = row.LossCodeID, Text = row.LossCode });
            return Json(ShopData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBreakdownsLevel2(string BREAKDOWN)
        {
            var ShopData = (from row in db.tbllossescodes
                            where row.IsDeleted == 0 && row.MessageType == "BREAKDOWN" && row.LossCodesLevel == 2
                            select new { Value = row.LossCodeID, Text = row.LossCode });
            return Json(ShopData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Edit(ProductionLossModel tlc, int Level1 = 0, int Level2 = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            int UserID = Convert.ToInt32(Session["UserId"]);
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var losscode = tlc.ProductionLoss.LossCode;
                var DuplicateEntry = db.tbllossescodes.Where(m => m.LossCode == losscode && m.IsDeleted == 0 && m.MessageType == tlc.ProductionLoss.MessageType && m.LossCodesLevel == tlc.ProductionLoss.LossCodesLevel && m.LossCodesLevel1ID == tlc.ProductionLoss.LossCodesLevel1ID && m.LossCodesLevel2ID == tlc.ProductionLoss.LossCodesLevel2ID && m.LossCodeID != tlc.ProductionLoss.LossCodeID).ToList();
                if (DuplicateEntry.Count == 0)
                {
                    var losscodes = db.tbllossescodes.Find(tlc.ProductionLoss.LossCodeID);

                    losscodes.LossCode = tlc.ProductionLoss.LossCode;
                    losscodes.LossCodeDesc = tlc.ProductionLoss.LossCodeDesc;
                    losscodes.MessageType = tlc.ProductionLoss.MessageType;
                    losscodes.ContributeTo = tlc.ProductionLoss.ContributeTo;
                    losscodes.LossCodesLevel = tlc.ProductionLoss.LossCodesLevel;
                    losscodes.LossCodesLevel1ID = tlc.ProductionLoss.LossCodesLevel1ID;
                    losscodes.LossCodesLevel2ID = tlc.ProductionLoss.LossCodesLevel2ID;
                    losscodes.ModifiedBy = UserID;
                    losscodes.ModifiedOn = DateTime.Now;
                    db.Entry(losscodes).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
                //{
                //    return RedirectToAction("Login", "Login", null);
                //}
                //int UserID = Convert.ToInt32(Session["UserId"]);
                //ViewBag.Logout = Session["Username"].ToString().ToUpper();
                //ViewBag.roleid = Session["RoleID"];
                //if (Level1 == 0 && Level2 == 0)
                //{
                //    tlc.ProductionLoss.LossCodesLevel = 1;
                //}
                //else if (Level1 != 0 && Level2 == 0)
                //{
                //    tlc.ProductionLoss.LossCodesLevel = 2;
                //    tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                //}
                //else if (Level1 != 0 && Level2 != 0)
                //{
                //    tlc.ProductionLoss.LossCodesLevel = 3;
                //    tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                //    tlc.ProductionLoss.LossCodesLevel2ID = Level2;
                //}


                //tlc.ProductionLoss.ModifiedBy = UserID;
                //tlc.ProductionLoss.ModifiedOn = DateTime.Now;
                //tlc.ProductionLoss.ServerTabFlagSync = 1;
                //tlc.ProductionLoss.ServerTabCheck = 2;

                //if (Convert.ToInt16(tlc.ProductionLoss.LossCodesLevel) == 1)
                //{
                //    var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == tlc.ProductionLoss.LossCode && m.LossCodeID != tlc.ProductionLoss.LossCodeID).ToList();
                //    if (duplosscode.Count == 0)
                //    {

                //        db.Entry(tlc.ProductionLoss).State = EntityState.Modified;
                //        db.SaveChanges();
                //        return RedirectToAction("Index");
                //    }
                //    else
                //    {
                //        Session["Error"] = "Loss code already Exist.";
                //        ViewData["Level1"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == 1 && m.LossCode != "999" && m.MessageType != "PM" && m.MessageType != "BREAKDOWN"), "LossCodeID", "LossCodeDesc", tlc.ProductionLoss.LossCodesLevel1ID);
                //        ViewData["Level2"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == 2 && m.MessageType != "PM" && m.MessageType != "BREAKDOWN"), "LossCodeID", "LossCodeDesc", tlc.ProductionLoss.LossCodesLevel2ID);
                //        ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                //        return View(tlc);
                //    }
                //}

                //if (Convert.ToInt16(tlc.ProductionLoss.LossCodesLevel) == 2)
                //{
                //    tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                //    var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == tlc.ProductionLoss.LossCode && m.LossCodeID != tlc.ProductionLoss.LossCodeID && m.LossCodesLevel1ID == tlc.ProductionLoss.LossCodesLevel1ID).ToList();
                //    if (duplosscode.Count == 0)
                //    {
                //        db.Entry(tlc.ProductionLoss).State = EntityState.Modified;
                //        db.SaveChanges();
                //        return RedirectToAction("Index");
                //    }
                //    else
                //    {
                //        Session["Error"] = "Loss code already Exist.";
                //        ViewData["Level1"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == 1 && m.LossCode != "999" && m.MessageType != "PM" && m.MessageType != "BREAKDOWN"), "LossCodeID", "LossCodeDesc", tlc.ProductionLoss.LossCodesLevel1ID);
                //        ViewData["Level2"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == 2 && m.MessageType != "PM" && m.MessageType != "BREAKDOWN"), "LossCodeID", "LossCodeDesc", tlc.ProductionLoss.LossCodesLevel2ID);
                //        ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                //        return View(tlc);
                //    }
                //}
                //if (Convert.ToInt16(tlc.ProductionLoss.LossCodesLevel) == 3)
                //{
                //    tlc.ProductionLoss.LossCodesLevel1ID = Level1;
                //    tlc.ProductionLoss.LossCodesLevel2ID = Level2;
                //    var duplosscode = db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCode == tlc.ProductionLoss.LossCode && m.LossCodeID != tlc.ProductionLoss.LossCodeID && m.LossCodesLevel1ID == tlc.ProductionLoss.LossCodesLevel1ID && m.LossCodesLevel2ID == tlc.ProductionLoss.LossCodesLevel2ID).ToList();
                //    if (duplosscode.Count == 0)
                //    {
                //        db.Entry(tlc.ProductionLoss).State = EntityState.Modified;
                //        db.SaveChanges();
                //        return RedirectToAction("Index");
                //    }
                //    else
                //    {
                //        Session["Error"] = "Loss code already Exist.";
                //        ViewData["Level1"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == 1 && m.LossCode != "999" && m.MessageType != "PM" && m.MessageType != "BREAKDOWN"), "LossCodeID", "LossCodeDesc", tlc.ProductionLoss.LossCodesLevel1ID);
                //        ViewData["Level2"] = new SelectList(db.tbllossescodes.Where(m => m.IsDeleted == 0 && m.LossCodesLevel == 2 && m.MessageType != "PM" && m.MessageType != "BREAKDOWN"), "LossCodeID", "LossCodeDesc", tlc.ProductionLoss.LossCodesLevel2ID);
                //        ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                //        return View(tlc);
                //    }
                //}
                //return RedirectToAction("Index");
                else
                {
                    Session["Error"] = "Loss code already Exist.";
                    ViewBag.LossCodeLevel1ID = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel1ID", "LossCode", tlc.ProductionLoss.LossCode).ToList();
                    ViewBag.LossCodeLevel2ID = new SelectList(db.tbllossescodes.ToList().Where(d => d.IsDeleted == 0), "LossCodesLevel2ID", "LossCode", tlc.ProductionLoss.LossCode).ToList();
                    ViewBag.radioselected = tlc.ProductionLoss.LossCodesLevel;

                    return View(tlc);


                }

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
            //ViewBag.IsConfigMenu = 0;
            tbllossescode tlc = db.tbllossescodes.Find(id);
            tlc.ServerTabFlagSync = 1;
            tlc.ServerTabCheck = 2;
            tlc.IsDeleted = 1;
            tlc.ModifiedBy = UserID;
            tlc.ModifiedOn = System.DateTime.Now;
            //start Logging

            String Username = Session["Username"].ToString();
            //string CompleteModificationdetail = "Deleted Parts/Item";
            //ActiveLogStorage Obj = new ActiveLogStorage();
            //Obj.SaveActiveLog(Action, Controller, Username, UserID, CompleteModificationdetail);
            //End
            db.Entry(tlc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}