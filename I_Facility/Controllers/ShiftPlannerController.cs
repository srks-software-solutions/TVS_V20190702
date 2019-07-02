using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
//using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace I_Facility.Controllers
{

    //[CustomFilters]
    public class ShiftPlannerController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        GetShift sd = new GetShift();
        // GET: ShiftPlanner
        public ActionResult Index()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserId"]);
            ShiftPlannerModel pa = new ShiftPlannerModel();
            tblshiftplanner mp = new tblshiftplanner();
            ViewBag.ShiftMethod = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName");
            ViewBag.SelectedDropDown = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineName");
            ViewBag.Shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == mp.PlantID), "ShopID", "ShopName", mp.ShopID);
            ViewBag.Cell = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == mp.CellID), "CellID", "CellName", mp.CellID);
            ViewBag.WCID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == mp.CellID), "MachineID", "MachineDisplayName", mp.MachineID);
            pa.ShiftPlanner = mp;
            DateTime onlyDate = DateTime.Now.Date;
            pa.ShiftPlannerList = db.tblshiftplanners.Where(m => m.IsDeleted == 0 && m.EndDate > onlyDate).OrderBy(m => m.ShiftPlannerID).ToList();
            return View(pa);
        }

        public JsonResult PlanOverLapChecking(string PlantID, string ShopID, string CellID, string WorkCenterID, string startdatestring, string enddatestring)
        {

            List<int> DoesThisPlanOverlapUpwards = new List<int>(), DoesThisPlanOverlapDownwards = new List<int>(), DoesThisPlanOverlapAll = new List<int>();
            #region
            if (!String.IsNullOrEmpty(ShopID))
            {
                if (!String.IsNullOrEmpty(CellID))
                {
                    if (!String.IsNullOrEmpty(WorkCenterID))
                    {
                        int wcid = Convert.ToInt32(WorkCenterID);
                        DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachine(startdatestring, enddatestring, wcid);
                        DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachineDownwards(startdatestring, enddatestring, wcid);
                    }
                    else
                    {
                        int cellid = Convert.ToInt32(CellID);
                        DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachinecategory(startdatestring, enddatestring, cellid);
                        DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachinecategoryDownwards(startdatestring, enddatestring, cellid);
                    }
                }
                else
                {
                    int shopid = Convert.ToInt32(ShopID);
                    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForDepartment(startdatestring, enddatestring, shopid);
                    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerFordeptDownwards(startdatestring, enddatestring, shopid);
                }
            }
            else
            {
                int plantid = Convert.ToInt32(PlantID);
                DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForPlant(startdatestring, enddatestring, plantid);
                DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForPlantDownwards(startdatestring, enddatestring, plantid);
            }
            #endregion

            //move all id's into one list.
            DoesThisPlanOverlapAll.AddRange(DoesThisPlanOverlapUpwards);
            DoesThisPlanOverlapAll.AddRange(DoesThisPlanOverlapDownwards);

            if (DoesThisPlanOverlapAll.Count == 0) //plan doesn't ovelap. So commit.
            {
                return Json("No", JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["toaster_error"] = "Shift Planner exists for this Duration";

                //string OLPD = "<div><p><span>Planner Name</span><span>StartDate</span><span>End Date</span></p></div>";
                var results = db.tblshiftplanners.Where(m => m.IsDeleted == 0).Where(x => DoesThisPlanOverlapAll.Contains(x.ShiftPlannerID));

                string OLPD = "<div  style='font-size:.75vw'>";
                foreach (var row in results)
                {
                    int planId = row.ShiftPlannerID;
                    bool tick = sd.IsThisPlanInAction(planId);

                    OLPD += "<p><span>Shift_Planner Name : " + row.ShiftPlannerName + "</span></p>";
                    if (tick)
                    {
                        OLPD += "<span></br>This Plan is In Action</br></span>";
                    }
                    OLPD += "<span> Start Date : " + row.StartDate.ToString("yyyy-MM-dd") + "</span></p>";
                    OLPD += "<span>End Date : " + row.EndDate.ToString("yyyy-MM-dd") + "</span></p>";
                }
                OLPD += "</div>";

                ViewBag.OverLappingPlanDetails = OLPD;
                Session["Overlapping details"] = OLPD;
                return Json(ViewBag.OverLappingPlanDetails, JsonRequestBehavior.AllowGet);
            }
            //return Json(result, JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        public ActionResult Create(ShiftPlannerModel tsp, string MachineID, int ShiftMethod = 0, int SelectedDropDown = 0, string method = null, int shiftOverrideConfirm = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            //string WorkCenterID = MachineID;
            string PlantID = tsp.ShiftPlanner.PlantID.ToString();
            string ShopID = tsp.ShiftPlanner.ShopID.ToString();
            string CellID = tsp.ShiftPlanner.CellID.ToString();
            string WorkCenterID = tsp.ShiftPlanner.MachineID.ToString();
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserId"]);
            ViewBag.radiobutton = method;

            //validate plan overlapping
            List<int> DoesThisPlanOverlapUpwards = new List<int>(), DoesThisPlanOverlapDownwards = new List<int>(), DoesThisPlanOverlapAll = new List<int>();
            string startdatestring = tsp.ShiftPlanner.StartDate.ToString("yyyy-MM-dd");
            string enddatestring = tsp.ShiftPlanner.EndDate.ToString("yyyy-MM-dd");
            string oldEndDate = tsp.ShiftPlanner.StartDate.AddDays(-1).ToString("yyyy-MM-dd");
            int FactorID = SelectedDropDown;
            tsp.ShiftPlanner.ShiftMethodID = ShiftMethod;

            #region
            if (!String.IsNullOrEmpty(ShopID))
            {
                if (!String.IsNullOrEmpty(CellID))
                {
                    if (!String.IsNullOrEmpty(WorkCenterID))
                    {
                        int wcid = Convert.ToInt32(WorkCenterID);
                        DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachine(startdatestring, enddatestring, wcid);
                        DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachineDownwards(startdatestring, enddatestring, wcid);
                    }
                    else
                    {
                        int cellid = Convert.ToInt32(CellID);
                        DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachinecategory(startdatestring, enddatestring, cellid);
                        DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachinecategoryDownwards(startdatestring, enddatestring, cellid);
                    }
                }
                else
                {
                    int shopid = Convert.ToInt32(ShopID);
                    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForDepartment(startdatestring, enddatestring, shopid);
                    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerFordeptDownwards(startdatestring, enddatestring, shopid);
                }
            }
            else
            {
                int plantid = Convert.ToInt32(PlantID);
                DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForPlant(startdatestring, enddatestring, plantid);
                DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForPlantDownwards(startdatestring, enddatestring, plantid);
            }
            #endregion

            #region OLD
            //if (method == "Plant")
            //{
            //    tsp.PlantID = SelectedDropDown;
            //    ViewBag.SelectedDropDown = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", SelectedDropDown);
            //    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForPlant(startdatestring, enddatestring, FactorID);
            //    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForPlantDownwards(startdatestring, enddatestring, FactorID);
            //}
            //else if (method == "Shop")
            //{
            //    tsp.ShopID = SelectedDropDown;
            //    ViewBag.SelectedDropDown = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopID", "ShopName", SelectedDropDown);
            //    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForShop(startdatestring, enddatestring, FactorID);
            //    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForShopDownwards(startdatestring, enddatestring, FactorID);
            //}
            //else if (method == "Cell")
            //{
            //    tsp.CellID = SelectedDropDown;
            //    ViewBag.SelectedDropDown = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", SelectedDropDown);
            //    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForCell(startdatestring, enddatestring, FactorID);
            //    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForCellDownwards(startdatestring, enddatestring, FactorID);
            //}
            //else if (method == "Machine")
            //{
            //    tsp.MachineID = SelectedDropDown;
            //    ViewBag.SelectedDropDown = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0), "MachineID", "MachineDispName", SelectedDropDown);
            //    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachine(startdatestring, enddatestring, FactorID);
            //    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachineDownwards(startdatestring, enddatestring, FactorID);
            //}
            #endregion

            //move all id's into one list.
            DoesThisPlanOverlapAll.AddRange(DoesThisPlanOverlapUpwards);
            DoesThisPlanOverlapAll.AddRange(DoesThisPlanOverlapDownwards);

            if (DoesThisPlanOverlapAll.Count == 0) //plan doesn't ovelap. So commit.//New ShiftPlanner will be added
            {
                tsp.ShiftPlanner.StartDate = Convert.ToDateTime(startdatestring).Date;
                tsp.ShiftPlanner.EndDate = Convert.ToDateTime(enddatestring).Date;
                tsp.ShiftPlanner.CreatedBy = UserID;
                tsp.ShiftPlanner.CreatedOn = DateTime.Now;
                tsp.ShiftPlanner.IsDeleted = 0;

                db.tblshiftplanners.Add(tsp.ShiftPlanner);
                db.SaveChanges();
            }
            else
            {

                //get details of ovelapping plans and send for confirmation, If confirmed(shiftOverrideConfirm == 1) commit.
                if (shiftOverrideConfirm == 1)
                {
                    tsp.ShiftPlanner.StartDate = Convert.ToDateTime(startdatestring).Date;
                    tsp.ShiftPlanner.EndDate = Convert.ToDateTime(enddatestring).Date;
                    tsp.ShiftPlanner.CreatedBy = UserID;
                    tsp.ShiftPlanner.CreatedOn = DateTime.Now;
                    tsp.ShiftPlanner.IsDeleted = 0;

                    db.tblshiftplanners.Add(tsp.ShiftPlanner);
                    db.SaveChanges();

                    // Old shift plan has to end -- update ENdate //Remove old plan
                    var results = db.tblshiftplanners.Where(m => m.IsDeleted == 0).Where(x => DoesThisPlanOverlapAll.Contains(x.ShiftPlannerID));

                    foreach (var row in results)
                    {
                        int id = row.ShiftPlannerID;
                        var item = db.tblshiftplanners.Find(id);

                        bool tick = sd.IsThisPlanInAction(id);
                        if (tick)
                        {
                            item.PlanStoppedDate = Convert.ToDateTime(row.EndDate);
                            item.EndDate = Convert.ToDateTime(oldEndDate);
                            item.IsPlanRemoved = 0;
                            item.IsPlanStopped = 1;
                            item.IsDeleted = 0;
                        }
                        else
                        {
                            item.PlanStoppedDate = Convert.ToDateTime(oldEndDate);
                            item.IsPlanStopped = 0;
                            item.IsPlanRemoved = 1;
                            item.IsDeleted = 1;
                        }
                        //db.Entry(row).State = EntityState.Modified;

                    }
                    db.SaveChanges();
                }
                else
                {
                    TempData["toaster_error"] = "Shift Planner exists for this Duration";

                    //string OLPD = "<div><p><span>Planner Name</span><span>StartDate</span><span>End Date</span></p></div>";
                    var results = db.tblshiftplanners.Where(m => m.IsDeleted == 0).Where(x => DoesThisPlanOverlapAll.Contains(x.ShiftPlannerID));

                    string OLPD = "<div  style='font-size:.75vw'>";
                    foreach (var row in results)
                    {
                        int planId = row.ShiftPlannerID;
                        bool tick = sd.IsThisPlanInAction(planId);

                        OLPD += "<p><span>Shift_Planner Name : " + row.ShiftPlannerName + "</span></p>";
                        if (tick)
                        {
                            OLPD += "<span></br>This Plan is In Action</br></span>";
                        }
                        OLPD += "<span> Start Date : " + row.StartDate.ToString("yyyy-MM-dd") + "</span></p>";
                        OLPD += "<span>End Date : " + row.EndDate.ToString("yyyy-MM-dd") + "</span></p>";
                    }
                    OLPD += "</div>";

                    ViewBag.OverLappingPlanDetails = OLPD;
                    ViewBag.ShiftMethod = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName", tsp.ShiftPlanner.ShiftMethodID);
                    ViewBag.PlantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tsp.ShiftPlanner.PlantID);
                    ViewBag.DepartmentID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == tsp.ShiftPlanner.PlantID), "ShopID", "ShopName", tsp.ShiftPlanner.ShopID);
                    ViewBag.MachineCategoryID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == tsp.ShiftPlanner.CellID), "CellID", "MachineCategory", tsp.ShiftPlanner.CellID);
                    if (tsp.ShiftPlanner.CellID != null || tsp.ShiftPlanner.CellID != 0)
                    {
                        ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == tsp.ShiftPlanner.CellID), "MachineID", "MachineName", tsp.ShiftPlanner.MachineID);
                    }
                    else
                    {
                        ViewBag.MachineID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == tsp.ShiftPlanner.ShopID), "MachineID", "MachineInvNo", tsp.ShiftPlanner.MachineID);
                    }

                    return View("Index");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(ShiftPlannerModel tsp, string PlantID, string ShopID, string CellID, string SPStartDate, string MachineID, int ShiftMethod = 0, int SelectedDropDown = 0, string method = null, int EditshiftOverrideConfirm = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserId"]);
            ViewBag.radiobutton = method;

            #region ActiveLog Code
            // ActiveLogStorage Obj = new ActiveLogStorage();
            // Obj.SaveActiveLog(Action, Controller, Username, UserID, CompleteModificationdetail);
            #endregion

            //validate plan overlapping
            List<int> DoesThisPlanOverlapUpwards = new List<int>(), DoesThisPlanOverlapDownwards = new List<int>(), DoesThisPlanOverlapAll = new List<int>();
            string startdatestring = tsp.ShiftPlanner.StartDate.ToString("yyyy-MM-dd");
            string oldEndDate = tsp.ShiftPlanner.StartDate.AddDays(-1).ToString("yyyy-MM-dd");
            string enddatestring = tsp.ShiftPlanner.EndDate.ToString("yyyy-MM-dd");

            int FactorID = SelectedDropDown;
            tsp.ShiftPlanner.ShiftMethodID = ShiftMethod;

            int ShiftPlannerID = tsp.ShiftPlanner.ShiftPlannerID;

            //New Code: 2016-10-01
            #region
            if (!String.IsNullOrEmpty(ShopID))
            {
                if (!String.IsNullOrEmpty(CellID))
                {
                    if (!String.IsNullOrEmpty(MachineID))
                    {
                        int wcid = Convert.ToInt32(MachineID);
                        DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachine(startdatestring, enddatestring, wcid, ShiftPlannerID);
                        DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachineDownwards(startdatestring, enddatestring, wcid, ShiftPlannerID);
                    }
                    else
                    {
                        int cellid = Convert.ToInt32(CellID);
                        DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForMachinecategory(startdatestring, enddatestring, cellid, ShiftPlannerID);
                        DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForMachinecategoryDownwards(startdatestring, enddatestring, cellid, ShiftPlannerID);
                    }
                }
                else
                {
                    int shopid = Convert.ToInt32(ShopID);
                    DoesThisPlanOverlapUpwards = Plan_OverlapCheckerFordeptDownwards(startdatestring, enddatestring, shopid, ShiftPlannerID);
                    DoesThisPlanOverlapDownwards = Plan_OverlapCheckerFordeptDownwards(startdatestring, enddatestring, shopid, ShiftPlannerID);
                }
            }
            else
            {
                int plantid = Convert.ToInt32(PlantID);
                DoesThisPlanOverlapUpwards = Plan_OverlapCheckerForPlant(startdatestring, enddatestring, plantid, ShiftPlannerID);
                DoesThisPlanOverlapDownwards = Plan_OverlapCheckerForPlantDownwards(startdatestring, enddatestring, plantid, ShiftPlannerID);
            }
            #endregion



            DoesThisPlanOverlapAll.AddRange(DoesThisPlanOverlapUpwards);
            DoesThisPlanOverlapAll.AddRange(DoesThisPlanOverlapDownwards);

            if (DoesThisPlanOverlapAll.Count == 0) //plan doesn't ovelap. So commit.
            {
                tsp.ShiftPlanner.StartDate = Convert.ToDateTime(startdatestring).Date;
                tsp.ShiftPlanner.EndDate = Convert.ToDateTime(enddatestring).Date;
                //tsp.ShiftPlanner.CreatedBy = UserID;
                //tsp.ShiftPlanner.CreatedOn = DateTime.Now;
                tsp.ShiftPlanner.ModifiedBy = UserID;
                tsp.ShiftPlanner.ModifiedOn = DateTime.Now;
                tsp.ShiftPlanner.IsDeleted = 0;
                db.Entry(tsp.ShiftPlanner).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //get details of ovelapping plans and send for confirmation, If confirmed(EditshiftOverrideConfirm == 1) commit.
                if (EditshiftOverrideConfirm == 1)
                {
                    tsp.ShiftPlanner.StartDate = Convert.ToDateTime(startdatestring).Date;
                    tsp.ShiftPlanner.EndDate = Convert.ToDateTime(enddatestring).Date;
                    //tsp.ShiftPlanner.CreatedBy= UserID;
                    //tsp.ShiftPlanner.CreatedOn= DateTime.Now;
                    tsp.ShiftPlanner.ModifiedBy = UserID;
                    tsp.ShiftPlanner.ModifiedOn = DateTime.Now;
                    tsp.ShiftPlanner.IsDeleted = 0;
                    db.Entry(tsp).State = EntityState.Modified;
                    db.SaveChanges();
                    //now remove the old plans.
                    var results = db.tblshiftplanners.Where(m => m.IsDeleted == 0).Where(x => DoesThisPlanOverlapAll.Contains(x.ShiftPlannerID));

                    foreach (var row in results)
                    {
                        int id = row.ShiftPlannerID;
                        bool tick = sd.IsThisPlanInAction(id);
                        if (tick)
                        {
                            row.PlanStoppedDate = Convert.ToDateTime(row.EndDate);
                            row.EndDate = Convert.ToDateTime(oldEndDate);
                            row.IsPlanStopped = 1;
                            row.IsDeleted = 0;
                        }
                        else
                        {
                            row.PlanStoppedDate = Convert.ToDateTime(oldEndDate);
                            row.IsPlanRemoved = 1;
                            row.IsDeleted = 1;
                        }
                        db.Entry(row).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    TempData["toaster_error"] = "Shift Planner exists for this Duration";
                    //string OLPD = "<div><p><span>Planner Name</span><span>StartDate</span><span>End Date</span></p></div>";
                    var results = db.tblshiftplanners.Where(m => m.IsDeleted == 0).Where(x => DoesThisPlanOverlapAll.Contains(x.ShiftPlannerID));
                    string OLPD = "<div style='font-size:.75vw'>";
                    foreach (var row in results)
                    {
                        int planId = row.ShiftPlannerID;
                        bool tick = sd.IsThisPlanInAction(planId);

                        OLPD += "<p><span>Shift_Planner Name : " + row.ShiftPlannerName + "</span></p>";
                        if (tick)
                        {
                            OLPD += "<span></br>This Plan is In Action</br></span>";
                        }
                        OLPD += "</p><span> Start Date : " + row.StartDate.ToString("yyyy-MM-dd") + "</span></p>";
                        OLPD += "</p><span>End Date : " + row.EndDate.ToString("yyyy-MM-dd") + "</span></p>";
                    }
                    OLPD += "</div>";
                    ViewBag.OverLappingPlanDetails = OLPD;
                    ViewBag.ShiftMethod = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName", tsp.ShiftPlanner.ShiftMethodID);
                    ViewBag.PlantID = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tsp.ShiftPlanner.PlantID);
                    ViewBag.ShopID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == tsp.ShiftPlanner.PlantID), "ShopID", "ShopName", tsp.ShiftPlanner.ShopID);
                    ViewBag.CellID = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.ShopID == tsp.ShiftPlanner.ShopID), "CellID", "CellName", tsp.ShiftPlanner.CellID);
                    if (tsp.ShiftPlanner.CellID != null || tsp.ShiftPlanner.CellID != 0)
                    {
                        ViewBag.WCID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == tsp.ShiftPlanner.CellID), "MachineID", "MachineDisplayName", tsp.ShiftPlanner.MachineID);
                    }
                    else
                    {
                        ViewBag.WCID = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == tsp.ShiftPlanner.ShopID), "MachineID", "MachineInvNo", tsp.ShiftPlanner.MachineID);
                    }


                    return View(tsp);
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
            String Username = Session["Username"].ToString();
            int UserID1 = id;
            //ViewBag.IsConfigMenu = 0;

            //start Logging
            int UserID = Convert.ToInt32(Session["UserId"]);
            bool tick = sd.IsThisPlanInAction(id);

            if (!tick)
            {
                tblshiftplanner tblmc = db.tblshiftplanners.Find(id);
                tblmc.IsDeleted = 1;
                tblmc.ModifiedBy = UserID;
                tblmc.ModifiedOn = DateTime.Now;
                db.Entry(tblmc).State = EntityState.Modified;
                db.SaveChanges();

            }
            else
            {
                TempData["toaster_error"] = "Opps! This plan is in Action . Cannot Delete.";
            }
            return RedirectToAction("Index");
        }

        public List<int> Plan_OverlapCheckerForPlant(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>();
            int PlantID = FactorID;
            DataTable dataHolder = new DataTable();
            MsqlConnection mc = new MsqlConnection();
            mc.open();
            String sql = null;
            if (ShiftPlannerID != 0)
            {
                sql = "SELECT ShiftPlannerID FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "')OR (StartDate <='" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND PlantID =" + PlantID + " AND ShopID is null AND CellID is null AND MachineID is null  and IsDeleted = 0 and ShiftPlannerID != " + ShiftPlannerID + "  ORDER BY ShiftPlannerID ASC";
            }
            else
            {
                sql = "SELECT ShiftPlannerID FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "')OR (StartDate <='" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND PlantID =" + PlantID + " AND ShopID is null AND CellID is null AND MachineID is null  and IsDeleted = 0 ORDER BY ShiftPlannerID ASC";
            }
            MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
            da.Fill(dataHolder);
            mc.close();
            for (int i = 0; i < dataHolder.Rows.Count; i++)
            {
                overlappingPlanId.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
            }
            return overlappingPlanId;
        }
        public List<int> Plan_OverlapCheckerForDepartment(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>();
            int ShopId = FactorID;

            //1st check if its Plant has a Plan.
            //so get its plantid.
            var plantdetails = db.tblshops.Where(m => m.IsDeleted == 0 && m.ShopID == ShopId).FirstOrDefault();
            int plantId = Convert.ToInt32(plantdetails.PlantID);
            overlappingPlanId = Plan_OverlapCheckerForPlant(startdatestring, enddatestring, plantId, ShiftPlannerID);

            if (overlappingPlanId.Count == 0)
            {
                DataTable dataHolder = new DataTable();
                MsqlConnection mc = new MsqlConnection();
                mc.open();
                String sql = null;
                if (ShiftPlannerID != 0)
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "')OR( StartDate <='" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND ShopID =" + ShopId + " AND CellID is null AND MachineID is null   and IsDeleted = 0 and ShiftPlannerID != " + ShiftPlannerID + "  ORDER BY ShiftPlannerID ASC";
                }
                else
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "')OR( StartDate <='" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND ShopID =" + ShopId + " AND CellID is null AND MachineID is null  and IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
                }
                MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
                da.Fill(dataHolder);
                mc.close();

                for (int i = 0; i < dataHolder.Rows.Count; i++)
                {
                    overlappingPlanId.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
                }
            }
            return overlappingPlanId;
        }
        public List<int> Plan_OverlapCheckerForMachinecategory(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>();
            int CELLID = FactorID;
            DataTable dataHolder = new DataTable();
            //1st check if its Shop has a Plan.
            //so get its shopid.
            var MachinecategoryDetails = db.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == CELLID).FirstOrDefault();
            int DeptId = Convert.ToInt32(MachinecategoryDetails.ShopID);
            overlappingPlanId = Plan_OverlapCheckerForDepartment(startdatestring, enddatestring, DeptId, ShiftPlannerID);

            if (overlappingPlanId.Count == 0)
            {
                MsqlConnection mc = new MsqlConnection();
                mc.open();
                String sql = null;
                if (ShiftPlannerID != 0)
                {
                    sql = "SELECT * FROM i_facility.tblShiftPlanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR ( StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND CellID =" + CELLID + " AND MachineID is null  and IsDeleted = 0  and ShiftPlannerID != " + ShiftPlannerID + "  ORDER BY ShiftPlannerID ASC";
                }
                else
                {
                    sql = "SELECT * FROM i_facility.tblShiftPlanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR ( StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND CellID =" + CELLID + " AND MachineID is null  and  IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
                }
                MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
                da.Fill(dataHolder);
                mc.close();

                for (int i = 0; i < dataHolder.Rows.Count; i++)
                {
                    overlappingPlanId.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
                }

            }
            return overlappingPlanId;
        }
        public List<int> Plan_OverlapCheckerForMachine(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>(), overlappingPlanId1 = new List<int>(), overlappingPlanId2 = new List<int>();
            int MachineID = FactorID;
            DataTable dataHolder = new DataTable();

            //1st check if it has a Cell else go for Shop

            var machinedetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).FirstOrDefault();
            if (machinedetails.CellID.HasValue)
            {
                int MachinecategoryId = Convert.ToInt32(machinedetails.CellID);
                overlappingPlanId = Plan_OverlapCheckerForMachinecategory(startdatestring, enddatestring, MachinecategoryId, ShiftPlannerID);
            }
            else
            {
                int shopId = Convert.ToInt32(machinedetails.ShopID);
                overlappingPlanId1 = Plan_OverlapCheckerForDepartment(startdatestring, enddatestring, shopId, ShiftPlannerID);
            }

            //move all id's into one list.
            overlappingPlanId2.AddRange(overlappingPlanId);
            overlappingPlanId2.AddRange(overlappingPlanId1);

            if (overlappingPlanId2.Count == 0)
            {
                MsqlConnection mc = new MsqlConnection();
                mc.open();
                String sql = null;
                if (ShiftPlannerID != 0)
                {
                    sql = "SELECT * FROM i_facility. tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND MachineID =" + MachineID + "  and IsDeleted = 0 and ShiftPlannerID != " + ShiftPlannerID + "  ORDER BY ShiftPlannerID ASC";
                }
                else
                {
                    sql = "SELECT * FROM i_facility. tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND MachineID =" + MachineID + "  and IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
                }
                MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
                da.Fill(dataHolder);
                mc.close();

                for (int i = 0; i < dataHolder.Rows.Count; i++)
                {
                    overlappingPlanId2.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
                }
            }
            return overlappingPlanId2;
        }


        public List<int> Plan_OverlapCheckerForPlantDownwards(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>();
            int PlantID = FactorID;
            DataTable dataHolder = new DataTable();


            //1st check if its shop has a Plan.
            //so get its shopid.
            var ShopDetails = db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID).ToList();
            foreach (var Deptrow in ShopDetails)
            {
                int ShopID = Deptrow.ShopID;
                overlappingPlanId = Plan_OverlapCheckerFordeptDownwards(startdatestring, enddatestring, ShopID, ShiftPlannerID);
                if (overlappingPlanId.Count > 0)
                {
                    break;
                }
            }

            if (overlappingPlanId.Count == 0)
            {
                MsqlConnection mc = new MsqlConnection();
                mc.open();
                String sql = null;

                if (ShiftPlannerID != 0)
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND PlantID =" + PlantID + "   and IsDeleted = 0 and ShiftPlannerID != " + ShiftPlannerID + " ORDER BY ShiftPlannerID ASC";
                }
                else
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND PlantID =" + PlantID + "   and IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
                }
                MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
                da.Fill(dataHolder);
                mc.close();

                for (int i = 0; i < dataHolder.Rows.Count; i++)
                {
                    overlappingPlanId.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
                }
            }
            return overlappingPlanId;
        }

        public List<int> Plan_OverlapCheckerFordeptDownwards(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>(), overlappingPlanId1 = new List<int>(), overlappingPlanId2 = new List<int>();
            int ShopId = FactorID;

            //1st check if its Cells has a Plan.
            //so get its cellid.
            var CellDetails = db.tblcells.Where(m => m.IsDeleted == 0 && m.ShopID == ShopId).ToList();
            foreach (var Machinerow in CellDetails)
            {
                int CELLID = Convert.ToInt32(Machinerow.CellID);
                overlappingPlanId = Plan_OverlapCheckerForMachinecategoryDownwards(startdatestring, enddatestring, CELLID, ShiftPlannerID);
                if (overlappingPlanId.Count > 0)
                {
                    break;
                }
            }

            var machinedetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopId).ToList();
            foreach (var machinerow in machinedetails)
            {
                int machineId = machinerow.MachineID;
                overlappingPlanId1 = Plan_OverlapCheckerForMachineDownwards(startdatestring, enddatestring, machineId, ShiftPlannerID);
                if (overlappingPlanId1.Count > 0)
                {
                    break;
                }
            }

            //move all id's into one list.
            overlappingPlanId2.AddRange(overlappingPlanId);
            overlappingPlanId2.AddRange(overlappingPlanId1);

            if (overlappingPlanId2.Count == 0)
            {
                DataTable dataHolder = new DataTable();
                MsqlConnection mc = new MsqlConnection();
                mc.open();
                String sql = null;
                if (ShiftPlannerID != 0)
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE (( StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND ShopID =" + ShopId + "  and IsDeleted = 0  and ShiftPlannerID != " + ShiftPlannerID + " ORDER BY ShiftPlannerID ASC";
                }
                else
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE (( StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND ShopID =" + ShopId + "  and IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
                }
                MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
                da.Fill(dataHolder);
                mc.close();
                for (int i = 0; i < dataHolder.Rows.Count; i++)
                {
                    overlappingPlanId2.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
                }
            }
            return overlappingPlanId2;
        }
        public List<int> Plan_OverlapCheckerForMachinecategoryDownwards(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>();
            int CellId = FactorID;
            DataTable dataHolder = new DataTable();
            //1st check if its machines has a Plan.
            //so get its machineids.
            var machinedetails = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellId).ToList();
            foreach (var machinerow in machinedetails)
            {
                int machineId = machinerow.MachineID;
                overlappingPlanId = Plan_OverlapCheckerForMachineDownwards(startdatestring, enddatestring, machineId, ShiftPlannerID);
                if (overlappingPlanId.Count > 0)
                {
                    break;
                }
            }

            if (overlappingPlanId.Count == 0)
            {
                MsqlConnection mc = new MsqlConnection();
                mc.open();
                String sql = null;
                if (ShiftPlannerID != 0)
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND CellID =" + CellId + "  and IsDeleted = 0  and ShiftPlannerID != " + ShiftPlannerID + " ORDER BY ShiftPlannerID ASC";
                }
                else
                {
                    sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND CellID =" + CellId + "  and IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
                }
                MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
                da.Fill(dataHolder);
                mc.close();
                for (int i = 0; i < dataHolder.Rows.Count; i++)
                {
                    overlappingPlanId.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
                }

            }
            return overlappingPlanId;
        }
        public List<int> Plan_OverlapCheckerForMachineDownwards(string startdatestring, string enddatestring, int FactorID, int ShiftPlannerID = 0)
        {
            List<int> overlappingPlanId = new List<int>();
            int MachineID = FactorID;
            DataTable dataHolder = new DataTable();

            MsqlConnection mc = new MsqlConnection();
            mc.open();
            String sql = null;
            if (ShiftPlannerID != 0)
            {
                sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND MachineID =" + MachineID + "  and IsDeleted = 0  and ShiftPlannerID != " + ShiftPlannerID + " ORDER BY ShiftPlannerID ASC";
            }
            else
            {
                sql = "SELECT * FROM i_facility.tblshiftplanner WHERE ((StartDate <='" + startdatestring + "' AND EndDate >='" + startdatestring + "') OR (StartDate <'" + enddatestring + "' AND EndDate >='" + enddatestring + "')) AND MachineID =" + MachineID + "  and IsDeleted = 0  ORDER BY ShiftPlannerID ASC";
            }
            MySqlDataAdapter da = new MySqlDataAdapter(sql, mc.sqlConnection);
            da.Fill(dataHolder);
            mc.close();

            for (int i = 0; i < dataHolder.Rows.Count; i++)
            {
                overlappingPlanId.Add(Convert.ToInt32(dataHolder.Rows[i][0]));
            }
            return overlappingPlanId;
        }


        public JsonResult CheckIfThisPlanIsInAction(int id)
        {
            //if nothing == 0 you will let him edit .
            int nothing = 1;
            GetShift gs = new GetShift();
            nothing = gs.IsThisPlanInAction(id) == true ? 1 : 0;
            return Json(nothing, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShop(int PlantID)
        {
            var ShopData = (from row in db.tblshops
                            where row.IsDeleted == 0 && row.PlantID == PlantID
                            select new { Value = row.ShopID, Text = row.Shopdisplayname });
            return Json(ShopData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCell(int ShopID)
        {
            var CellData = (from row in db.tblcells
                            where row.IsDeleted == 0 && row.ShopID == ShopID
                            select new { Value = row.CellID, Text = row.CelldisplayName });

            return Json(CellData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWC_Cell(int CellID)
        {
            var MachineData = (from row in db.tblmachinedetails
                               where row.IsDeleted == 0 && row.CellID == CellID
                               select new { Value = row.MachineID, Text = row.MachineDisplayName });
            return Json(MachineData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWC_Shop(int ShopID)
        {
            var MachineData = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID && m.CellID.Equals(null)), "MachineID", "MachineDisplayName");
            return Json(MachineData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWC_Cell_MWC(int CellID)
        {
            var MachineData = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID), "MachineID", "MachineDisplayName");
            return Json(MachineData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWC_Shop_MWC(int ShopID)
        {
            var MachineData = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID && m.CellID.Equals(null)), "MachineID", "MachineDisplayName");
            return Json(MachineData, JsonRequestBehavior.AllowGet);
        }


        //By Using Json call binding data to model popup window
        public JsonResult GetShiftPlannerById(int Id)
        {
            ShiftPlan sp = new ShiftPlan();
            var Data = db.tblshiftplanners.Where(m => m.ShiftPlannerID == Id).FirstOrDefault();
            sp.SPlannerName = Data.ShiftPlannerName;
            sp.SPlannerDesc = Data.ShiftPlannerDesc;
            sp.Sethod = Convert.ToInt32(Data.ShiftMethodID);
            sp.plantid = Convert.ToInt32(Data.PlantID);
            sp.department = Convert.ToInt32(Data.ShopID);
            sp.machinecate = Convert.ToInt32(Data.CellID);
            sp.machineid = Convert.ToInt32(Data.MachineID);
            sp.startdate = Data.StartDate.ToString("yyyy-MM-dd");
            sp.enddate = Data.EndDate.ToString("yyyy-MM-dd");
            return Json(sp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string ShiftPlannerNameDuplicateCheck(string shiftPlannerName = "")
        {
            string status = "notok";
            var doesThisPlannerNameNameExist = db.tblshiftplanners.Where(m => m.IsDeleted == 0 && m.ShiftPlannerName == shiftPlannerName).ToList();
            if (doesThisPlannerNameNameExist.Count == 0)
            {
                status = "ok";
            }
            else
            {
                status = "notok";
            }
            return status;
        }

        [HttpPost]
        public string ShiftPlannerNameDuplicateCheckEdit(int ShiftPlannerID = 0, string shiftPlannerName = "")
        {
            string status = "notok";
            if (ShiftPlannerID != 0)
            {
                var doesThisShiftMethodNameExist = db.tblshiftplanners.Where(m => m.IsDeleted == 0 && m.ShiftPlannerName == shiftPlannerName).ToList();
                if (doesThisShiftMethodNameExist.Count == 0)
                {
                    status = "ok";
                }
                else
                {
                    var checkforId = db.tblshiftplanners.Where(m => m.IsDeleted == 0 && m.ShiftPlannerName == shiftPlannerName && m.ShiftPlannerID == ShiftPlannerID).ToList();
                    if (checkforId.Count == 0)
                    {
                        status = "notok";
                    }
                    else
                    {
                        status = "ok";
                    }

                }
            }
            return status;
        }

    }
}