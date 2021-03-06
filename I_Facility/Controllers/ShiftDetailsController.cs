﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
using System.Data;

namespace I_Facility.Controllers
{

    public class ShiftDetailsController : Controller
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
            String Username = Session["Username"].ToString();
            var shiftdetails = db.tblshiftdetails.Where(m => m.IsDeleted == 0).OrderBy(m => m.ShiftDetailsID).ToList();
            return View(shiftdetails.ToList());
            //return View();
        }

        public ActionResult Create()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            ViewBag.ShiftMethod1 = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName");
            return View();
        }
        [HttpPost]
        public ActionResult Create(IEnumerable<tblshiftdetail> tblp, string Nextday, int ShiftMethod1 = 0, int IsGShift = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            string error = "";
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            #region//ActiveLog Code
            int UserID = Convert.ToInt32(Session["UserId"]);
            //check if there's a entry of this shiftMethod in tblshiftdetails
            var shiftmethodCheck = db.tblshiftdetails.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == ShiftMethod1).ToList();
            if (shiftmethodCheck.Count > 0)
            {
                error = "ShiftDetails for this ShiftMethod Exists.";
                TempData["toaster_error"] = error;
                ViewBag.ShiftMethod1 = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName");
                return View();

            }

            var shiftmethodiddata = db.tblshiftmethods.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == ShiftMethod1).SingleOrDefault();
            int? noofshifts = shiftmethodiddata.NoOfShifts;
            int rowscount = 0;

            //to check if names are duplicate
            List<string> shiftdetailsnames = new List<string>();
            foreach (var shift in tblp)
            {
                if (shift.ShiftDetailsName != null)
                {
                    shiftdetailsnames.Add(shift.ShiftDetailsName);
                }
            }
            // for current shiftdetails.
            if (shiftdetailsnames.Distinct().Count() != shiftdetailsnames.Count())
            {
                //Console.WriteLine("List contains duplicate values.");
                error = "Shift Names Cannot be Same.";
                TempData["toaster_error"] = error;
                ViewBag.ShiftMethod1 = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName");
                return View();
            }

            try
            {
                foreach (var shift in tblp)
                {
                    if (rowscount < noofshifts)
                    {
                        // calculate duration
                        int duration = 0;
                        string starttimestring = "2016-06-02" + " " + shift.ShiftStartTime;
                        DateTime starttimedatetime = Convert.ToDateTime(starttimestring);
                        string endtimestring = null;

                        TimeSpan tsStart = (System.TimeSpan)shift.ShiftStartTime;
                        TimeSpan tsEnd = (System.TimeSpan)shift.ShiftEndTime;

                        int result = TimeSpan.Compare(tsStart, tsEnd);
                        if (result < 0)
                        {
                            endtimestring = "2016-06-02" + " " + shift.ShiftEndTime;
                        }
                        else if (result > 0)
                        {
                            endtimestring = "2016-06-03" + " " + shift.ShiftEndTime;
                            shift.NextDay = 1;
                        }
                        DateTime endtimedatetime = Convert.ToDateTime(endtimestring);
                        TimeSpan ts = endtimedatetime.Subtract(starttimedatetime);
                        duration = Convert.ToInt32(ts.TotalMinutes);
                        int UserId = (Convert.ToInt32(Session["UserId"]));
                        //create new object/row
                        tblshiftdetail tsd = new tblshiftdetail();
                        tsd.CreatedBy = UserId;
                        tsd.CreatedOn = DateTime.Now;
                        tsd.Duration = duration;
                        tsd.IsDeleted = 0;
                        tsd.IsGShift = 0;
                        tsd.NextDay = shift.NextDay;
                        tsd.ShiftMethodID = ShiftMethod1;
                        tsd.ShiftDetailsDesc = shift.ShiftDetailsDesc;
                        tsd.ShiftDetailsName = shift.ShiftDetailsName;
                        tsd.ShiftEndTime = shift.ShiftEndTime;
                        tsd.IsShiftDetailsEdited = 0;
                        tsd.ShiftStartTime = shift.ShiftStartTime;
                        db.tblshiftdetails.Add(tsd);
                        db.SaveChanges();
                    }
                    rowscount++;
                }
            }
            catch (Exception e)
            {
                error = "Shift Name already exists for this ShiftMethod.";
                TempData["toaster_error"] = error;

                using (i_facilityEntities1 db1 = new i_facilityEntities1())
                {
                    var todeletedata = db1.tblshiftdetails.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == ShiftMethod1).ToList();
                    foreach (var row in todeletedata)
                    {
                        row.IsDeleted = 1;
                        db.Entry(row).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                ViewBag.ShiftMethod1 = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName");
                return View();
            }

            ViewBag.ShiftMethod = new SelectList(db.tblshiftdetails.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName");
            return RedirectToAction("Index");
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
            ViewBag.ID = id;

            //get all the shiftsdetails in the shift method
            tblshiftdetail tblmc = db.tblshiftdetails.Find(id);
            List<tblshiftdetail> tsd = null;
            if (tblmc == null)
            {
                return HttpNotFound();
            }
            else
            {
                int shiftmethodid = Convert.ToInt32(tblmc.ShiftMethodID);
                tsd = db.tblshiftdetails.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == shiftmethodid).ToList();
            }
            ViewBag.ShiftMethod = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName", tblmc.ShiftMethodID);
            ViewBag.NextDay = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "NextDay", "NextDay", tblmc.NextDay);
            ViewBag.id = id;

            //ViewBag.unit = new SelectList(db.tblunits.Where(m => m.IsDeleted == 0), "U_ID", "Unit", tblpart.UnitDesc);
            return View(tsd);
        }
        //Update

        [HttpPost]
        public ActionResult Edit(IEnumerable<tblshiftdetail> tblp, int ShiftMethod = 0, int hdnishift = 0)
        {
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserID"]);

            var shiftmethodiddata = db.tblshiftmethods.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == ShiftMethod).SingleOrDefault();
            int? noofshifts = shiftmethodiddata.NoOfShifts;
            int rowscount = 0;

            //insert isedited and other details into old rows and insert the new rows.
            var shiftDetailsData = db.tblshiftdetails.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == ShiftMethod).ToList();
            //check if shift method is in use or was used and now its being modified.
            ShiftDetails sd = new ShiftDetails();
            int shiftmethodid = Convert.ToInt32(ShiftMethod);
            bool tick = Convert.ToBoolean(1); 
            try
            {
                foreach (var shift in tblp)
                {
                    if (rowscount < noofshifts)
                    {
                        using (i_facilityEntities1 db3 = new i_facilityEntities1())
                        {
                            // calculate duration
                            int duration = 0;
                            string starttimestring = "2016-06-02" + " " + shift.ShiftStartTime;
                            DateTime starttimedatetime = Convert.ToDateTime(starttimestring);
                            string endtimestring = null;

                            TimeSpan tsStart = (System.TimeSpan)shift.ShiftStartTime;
                            TimeSpan tsEnd = (System.TimeSpan)shift.ShiftEndTime;

                            int result = TimeSpan.Compare(tsStart, tsEnd);
                            if (result < 0)
                            {
                                endtimestring = "2016-06-02" + " " + shift.ShiftEndTime;
                            }
                            else if (result > 0)
                            {
                                endtimestring = "2016-06-03" + " " + shift.ShiftEndTime;
                                shift.NextDay = 1;
                            }
                            DateTime endtimedatetime = Convert.ToDateTime(endtimestring);
                            TimeSpan ts = endtimedatetime.Subtract(starttimedatetime);
                            duration = Convert.ToInt32(ts.TotalMinutes);

                            if (tick)
                            {
                                //create new object/row
                                int shiftid = shift.ShiftDetailsID;
                                int oldcreatedby = 0;
                                DateTime oldcreatedon = DateTime.Now;
                                using (i_facilityEntities1 db1 = new i_facilityEntities1())
                                {
                                    var getShiftId = db1.tblshiftdetails.Where(m => m.IsDeleted == 0 && m.ShiftDetailsID == shiftid).SingleOrDefault();
                                    getShiftId.IsShiftDetailsEdited = 1;
                                    getShiftId.IsDeleted = 1;
                                    getShiftId.ShiftMethodID = ShiftMethod;
                                    getShiftId.ShiftDetailsEditedDate = DateTime.Now;

                                    oldcreatedon = Convert.ToDateTime(getShiftId.CreatedOn);
                                    oldcreatedby = Convert.ToInt32(getShiftId.CreatedBy);
                                    ViewBag.ShiftMethod = new SelectList(db.tblshiftdetails.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftDetailsName", shift.ShiftMethodID);

                                    db1.Entry(getShiftId).State = EntityState.Modified;
                                    db1.SaveChanges();
                                }
                                tblshiftdetail tsd = new tblshiftdetail();
                                tsd.Duration = duration;
                                tsd.IsDeleted = 0;
                                tsd.CreatedBy = oldcreatedby;
                                tsd.CreatedOn = oldcreatedon;
                                tsd.ModifiedBy = UserID;
                                tsd.ModifiedOn = DateTime.Now;
                                tsd.IsDeleted = 0;
                                tsd.NextDay = shift.NextDay;
                                tsd.ShiftMethodID = ShiftMethod;
                                tsd.ShiftDetailsDesc = shift.ShiftDetailsDesc;
                                tsd.ShiftDetailsName = shift.ShiftDetailsName;
                                tsd.ShiftEndTime = shift.ShiftEndTime;
                                tsd.ShiftStartTime = shift.ShiftStartTime;
                                db.tblshiftdetails.Add(tsd);
                                db.SaveChanges();
                            }
                            else
                            {
                                //create new object/row
                                shift.ModifiedBy = UserID;
                                shift.ModifiedOn = DateTime.Now;
                                shift.Duration = duration;
                                shift.IsDeleted = 0;
                                shift.ShiftMethodID = ShiftMethod;

                                //db3.Entry(shift).State = EntityState.Modified;
                                db3.SaveChanges();
                            }
                        }
                    }
                    rowscount++;
                }

            }
            catch (Exception e)
            {

                TempData["toaster_error"] = "Please check with the data";
                ViewBag.ShiftMethod = new SelectList(db.tblshiftmethods.Where(m => m.IsDeleted == 0), "ShiftMethodID", "ShiftMethodName", ShiftMethod);
                return View(tblp);
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
            int UserID = Convert.ToInt32(Session["UserId"]);

            tblshiftdetail tblmc = db.tblshiftdetails.Find(id);
            tblmc.IsDeleted = 1;
            tblmc.ModifiedBy = UserID;
            tblmc.ModifiedOn = DateTime.Now;
            db.Entry(tblmc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetShifts(int shiftsCount)
        {
            int shifts = 0;
            var NumberOfShifts = db.tblshiftmethods.Where(m => m.IsDeleted == 0 && m.ShiftMethodID == shiftsCount).Take(1).ToList();
            shifts = Convert.ToInt32(NumberOfShifts[0].NoOfShifts);
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}