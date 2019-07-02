using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using I_Facility.ServerModel;
using I_Facility.Models;

namespace I_Facility.Controllers
{
    public class OperatorEntryListController : Controller
    {
        private i_facilityEntities1 db = new i_facilityEntities1();

        //public void getmachinedet()
        //{
        //    List<OP> opobj = new List<OP>();
        //    var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.CellID).ToList();
        //    foreach (var item in celldet)
        //    {
        //        OP obj = new OP();
        //        var cellid = item.CellID;
        //        obj.cellid = cellid;
        //        var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
        //        List<mc> m1 = new List<mc>();
        //        foreach (var mitem in machdet)
        //        {
        //            mc mobj = new mc();
        //            mobj.machid = mitem.MachineID;
        //            m1.Add(mobj);
        //        }
        //        obj.machdet = m1;
        //        opobj.Add(obj);
        //    }
        //    Session["MachineDetails"] = opobj;
        //}


        // GET: OperatorEntryList
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoredataStartfun(int OperatorID, string OperatorNo, int ProductionOrderNo, string PartNo, int ProductionOrderQty, int ProcessQty, string Shift, int YieldQty, int ScrapQty, int TotalQty)
        {
            tbloperatorentry optobj = new tbloperatorentry();
            optobj.CorrectedDate = DateTime.Now;
            optobj.CreatedBy = 1;
            optobj.CreatedOn = DateTime.Now;
            optobj.isDeleted = 0;
            optobj.ModifiedBy = 1;
            optobj.ModifiedOn = DateTime.Now;  
            optobj.OperationNo = OperatorNo;
            //optobj.OPID = OperatorID.ToString();
            optobj.PartNo = PartNo;
            optobj.ProcessQty = ProcessQty;
            optobj.ProducationOrderNo = ProductionOrderNo;
            optobj.ProductionOrderQty = ProductionOrderQty;
            optobj.ScrapQty = ScrapQty;
            optobj.Shift = Shift;
            optobj.Total = TotalQty;
            optobj.YieldQty = YieldQty;
            db.tbloperatorentries.Add(optobj);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return View("Index");
        }
        #region V changed

        public int MachineCheck(int machineid)
        {
            var machdet = db.tbllivemodes.Where(m => m.IsDeleted == 0 && m.MachineID == machineid && m.StartIdle == 1 && m.IsCompleted == 0).OrderByDescending(m => m.MachineID).FirstOrDefault();
            if (machdet != null)
            {
                return machdet.MachineID;
            }
            return 0;
        }

        public ActionResult IDLEPopupCell(FormCollection form, int LossSelect = 0, int value = 0)
        {
            //if(value==0)
            //{
            //    getmachinedet();
            //}
            var ob = Session["MachineDetails"];
            var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.CellID).ToList();
            foreach (var item in celldet)
            {
                var cellid = item.CellID;
                var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
                foreach (var mitem in machdet)
                {
                    var mid = mitem.MachineID;
                    GetMode GM = new GetMode();
                    //String IPAddress = GM.GetIPAddressofTabSystem();
                    //if (mid == value)
                    //{

                    //}
                    //else
                    //{
                    /* MachineCheck(mitem.MachineID)*/
                    //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    var MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0 && m.MachineID != value).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).First();
                    bool flag = GM.CheckIdleEntry(MachineID);
                    if (flag == true)
                    {
                        Session["MachineID"] = MachineID;

                        var prvmode = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ModeType == "SETUP" && m.ModeTypeEnd == 0).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                        if (prvmode != null)
                        {
                            ViewBag.SetUpStarted = "1";
                            ViewBag.MachineMode = "Setting";
                        }

                        var prvmodeMaint = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ModeType == "MNT" && m.ModeTypeEnd == 0).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                        if (prvmodeMaint != null)
                        {
                            ViewBag.MNTStarted = "1";
                            ViewBag.MachineMode = "MNT";
                        }

                        var tblLossCodes = db.tbllossescodes.Where(m => m.MessageType == "IDLE").ToList();
                        ViewBag.lossCodes = tblLossCodes;

                        if (LossSelect == 0)//first time ,show level 1
                        {
                            int lossCodeID = tblLossCodes.Find(a => a.MessageType == "IDLE").LossCodeID;
                            ViewBag.lossCodeID = lossCodeID;
                            ViewBag.level = 1;
                        }
                        else if (tblLossCodes.Where(m => m.LossCodesLevel1ID == LossSelect).ToList().Count > 0)// show level 2
                        {
                            int lossCodeID = LossSelect;
                            ViewBag.lossCodeID = lossCodeID;
                            ViewBag.level = 2;
                        }
                        else //show level 3
                        {
                            int lossCodeID = LossSelect;
                            ViewBag.lossCodeID = lossCodeID;
                            ViewBag.level = 3;
                        }

                        #region Update tblOperatorHeader 

                        //get the latest row according to the MachineID and CorrectedDate ->  there is no CorrectedDate column 
                        var operatorHeader = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                        if (operatorHeader != null)
                        {
                            operatorHeader.MachineMode = "IDLE";
                            operatorHeader.ModifiedOn = DateTime.Now;
                            operatorHeader.ModifiedBy = 1;// get from session once these screens are integrated....

                            db.Entry(operatorHeader).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        #region Lock the Machine
                        //var MacDet = db.tblmachinedetails.Find(MachineID);

                        //if (MacDet.MachineModelType == 1)
                        //{
                        //    AddFanucMachineWithConn AC = new AddFanucMachineWithConn(MacDet.IPAddress);
                        //    //AC.setmachinelock(true, (ushort)MacDet.MachineLockBit, (ushort)MacDet.MachineIdleBit, (ushort)MacDet.MachineUnlockBit);
                        //}

                        #endregion

                        var HeaderDet = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).SingleOrDefault();
                        var HeaderDet1 = db.tblmachinedetails.Where(m => m.MachineID == MachineID).SingleOrDefault();
                        if (HeaderDet != null)
                        {
                            ViewBag.MachineMode = HeaderDet.MachineMode;
                            ViewBag.MachineName = HeaderDet1.tblcell.CellName;
                            ViewBag.TabStatus = HeaderDet.TabConnecStatus;
                            ViewBag.ServerStatus = HeaderDet.ServerConnecStatus;
                            ViewBag.PageName = "IDLE";
                            ViewBag.Shift = HeaderDet.Shift;

                            return View(new { value = MachineID });
                            //}
                        }
                    }
                    else
                    {

                    }
                }
            }
            return RedirectToAction("DashboardProduction");
        }

        public JsonResult CheckIdle(int value = 0)
        {
            // db.Database.CommandTimeout = 180;
            GetMode GM = new GetMode();
            int Data = 0;

            //String IPAddress = GM.GetIPAddressofTabSystem();
            var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.CellID).ToList();
            foreach (var item in celldet)
            {
                var cellid = item.CellID;
                //int MachineID = _ServerContext.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
                foreach (var mitem in machdet)
                {
                    //String IPAddress = GM.GetIPAddressofTabSystem();
                    var mid = mitem.MachineID;
                    //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    int MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).First();
                    Session["MachineID"] = MachineID;

                    GM.UpdateOperatorHeader(MachineID);
                    var toolCounter = db.tbltoollifeoperators.Where(m => m.toollifecounter == m.StandardToolLife).Where(m => m.IsCompleted == false && m.IsReset == 0 && m.IsDeleted == 0).ToList();

                    bool IdleStatus = GM.CheckIdleEntry(MachineID);
                    if (IdleStatus == true)
                    {
                        if (IdleStatus)
                            Data = 1;
                        int toolcount = toolCounter.Count();
                        if (Data == 1 && toolcount == 0)
                        {
                            Data = 1;
                        }
                        else if (Data == 1 && toolcount > 0)
                        {
                            Data = 1;
                        }
                        else if (Data != 1 && toolcount > 0)
                        {
                            Data = 2;
                        }
                        else
                        {
                            Data = 0;
                        }
                        return Json(Data, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                    }
                }
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshData()
        {
            List<string> retValList = new List<string>();
            GetMode GM = new GetMode();
            var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.CellID).ToList();
            foreach (var item in celldet)
            {
                var cellid = item.CellID;
                var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
                if (machdet.Count == 1)
                {
                    foreach (var mitem in machdet)
                    {
                        //String IPAddress = GM.GetIPAddressofTabSystem();
                        var mid = mitem.MachineID;
                        //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                        int MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).FirstOrDefault();
                        Session["MachineID"] = MachineID;
                        GM.UpdateOperatorHeader(MachineID);
                        retValList = new List<string>();
                        var HeaderDet = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).SingleOrDefault();
                        var HeaderDet1 = db.tblmachinedetails.Where(m => m.MachineID == MachineID && m.CellID == cellid).SingleOrDefault();
                        if (HeaderDet != null)
                        {
                            retValList.Add(HeaderDet1.tblcell.CellName); //0
                            retValList.Add(HeaderDet.TabConnecStatus); //1
                            retValList.Add(HeaderDet.ServerConnecStatus); //2
                            retValList.Add(HeaderDet.MachineMode); //3
                            retValList.Add(HeaderDet.Shift); //4
                            retValList.Add(MachineID.ToString()); //5
                                                                  //retValList.Add(IPAddress); //6
                            retValList.Add(MachineID.ToString());
                            //string errormessage = Session["setuperror"].ToString();
                        }
                        var result = new { RetVal = retValList };
                        return Json(result, JsonRequestBehavior.AllowGet);

                    }
                }
                //else if(machdet.Count > 1)
                //{
                //    foreach (var mitem in machdet)
                //    {
                //        //String IPAddress = GM.GetIPAddressofTabSystem();
                //        var mid = mitem.MachineID;
                //        //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                //        int MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).First();
                //        Session["MachineID"] = MachineID;
                //        GM.UpdateOperatorHeader(MachineID);
                //        retValList = new List<string>();
                //        var HeaderDet = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).SingleOrDefault();
                //        var HeaderDet1 = db.tblmachinedetails.Where(m => m.MachineID == MachineID && m.CellID == cellid).SingleOrDefault();
                //        retValList.Add(HeaderDet1.tblcell.CellName); //0
                //        retValList.Add(HeaderDet.TabConnecStatus); //1
                //        retValList.Add(HeaderDet.ServerConnecStatus); //2
                //        retValList.Add(HeaderDet.MachineMode); //3
                //        retValList.Add(HeaderDet.Shift); //4
                //        retValList.Add(MachineID.ToString()); //5
                //                                              //retValList.Add(IPAddress); //6
                //        retValList.Add(MachineID.ToString());
                //        //string errormessage = Session["setuperror"].ToString();

                //        var result = new { RetVal = retValList };
                //        return Json(result, JsonRequestBehavior.AllowGet);
                //    }
                //}
            }
            var ret = new { RetVal = retValList };
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DashboardProduction()
        //{
        //    GetMode GM = new GetMode();
        //    var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
        //    foreach (var item in celldet)
        //    {
        //        var cellid = item.CellID;
        //        //int MachineID = _ServerContext.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
        //        var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
        //        foreach (var mitem in machdet)
        //        {
        //            // TempData["toaster_success"] = "pinging messages to server";

        //            //String IPAddress = GM.GetIPAddressofTabSystem();

        //            //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
        //            var mid = mitem.MachineID;
        //            //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
        //            int MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).First();

        //            Session["MachineID"] = MachineID;
        //            //int MachineID = Convert.ToInt32(Session["MachineID"]);
        //            var HeaderDet = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).SingleOrDefault();
        //            var HeaderDet1 = db.tblmachinedetails.Where(m => m.MachineID == MachineID && m.CellID == cellid).SingleOrDefault();
        //            if (HeaderDet != null)
        //            {
        //                ViewBag.MachineMode = HeaderDet.MachineMode;
        //                ViewBag.MachineName = HeaderDet1.tblcell.CellName;
        //                ViewBag.TabStatus = HeaderDet.TabConnecStatus;
        //                ViewBag.ServerStatus = HeaderDet.ServerConnecStatus;
        //                ViewBag.PageName = "Dashboard";
        //                ViewBag.Shift = HeaderDet.Shift;
        //            }
        //            DateTime correctedDate = DateTime.Now;
        //            tbldaytiming StartTime = db.tbldaytimings.Where(m => m.IsDeleted == 0).SingleOrDefault();
        //            TimeSpan Start = StartTime.StartTime;
        //            if (Start.Hours <= DateTime.Now.Hour)
        //            {
        //                correctedDate = DateTime.Now.Date;
        //            }
        //            else
        //            {
        //                correctedDate = DateTime.Now.AddDays(-1).Date;
        //            }

        //            var prvmode = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ModeType == "SETUP" && m.ModeTypeEnd == 0)
        //                    .OrderByDescending(m => m.InsertedOn).FirstOrDefault();
        //            if (prvmode != null)
        //            {
        //                ViewBag.SetUpStarted = "1";
        //                ViewBag.MachineMode = "Setting";
        //            }

        //            var prvmodeMaint = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ModeType == "MNT" && m.ModeTypeEnd == 0)
        //                    .OrderByDescending(m => m.InsertedOn).FirstOrDefault();
        //            if (prvmodeMaint != null)
        //            {
        //                ViewBag.MNTStarted = "1";
        //                ViewBag.MachineMode = "MNT";
        //            }

        //            var GetDashboardData = db.tbloperatordashboards.Where(m => /*m.MachineID == MachineID && */m.CorrectedDate == correctedDate).OrderByDescending(m => m.InsertedOn).ToList();
        //            //var machname = db.tblmachinedetails.Find(MachineID);
        //            tbloperatordashboard TOD = new tbloperatordashboard();
        //            OperatorDashboard OD = new OperatorDashboard();
        //            //OD.machinename = machname.MachineName;
        //            OD.OpDashboardList = GetDashboardData;
        //            OD.OpDashboard = TOD;
        //            return View(OD);
        //        }
        //    }
        //    return View();
        //}

        //Get DashboardProduction by Cell
        public ActionResult DashboardProduction(int id=0)
        {
            //GetMode GM = new GetMode();
            //var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
            //foreach (var item in celldet)
            //{
                var cellid = id;
                //int MachineID = _ServerContext.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
                foreach (var mitem in machdet)
                {
                    // TempData["toaster_success"] = "pinging messages to server";

                    //String IPAddress = GM.GetIPAddressofTabSystem();

                    //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    var mid = mitem.MachineID;
                    //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    int MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).First();

                    Session["MachineID"] = MachineID;
                    //int MachineID = Convert.ToInt32(Session["MachineID"]);
                    var HeaderDet = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).SingleOrDefault();
                    var HeaderDet1 = db.tblmachinedetails.Where(m => m.MachineID == MachineID && m.CellID == cellid).SingleOrDefault();
                    if (HeaderDet != null)
                    {
                        ViewBag.MachineMode = HeaderDet.MachineMode;
                        ViewBag.MachineName = HeaderDet1.tblcell.CellName;
                        ViewBag.TabStatus = HeaderDet.TabConnecStatus;
                        ViewBag.ServerStatus = HeaderDet.ServerConnecStatus;
                        ViewBag.PageName = "Dashboard";
                        ViewBag.Shift = HeaderDet.Shift;
                    }
                    DateTime correctedDate = DateTime.Now;
                    tbldaytiming StartTime = db.tbldaytimings.Where(m => m.IsDeleted == 0).SingleOrDefault();
                    TimeSpan Start = StartTime.StartTime;
                    if (Start.Hours <= DateTime.Now.Hour)
                    {
                        correctedDate = DateTime.Now.Date;
                    }
                    else
                    {
                        correctedDate = DateTime.Now.AddDays(-1).Date;
                    }

                    var prvmode = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ModeType == "SETUP" && m.ModeTypeEnd == 0)
                            .OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                    if (prvmode != null)
                    {
                        ViewBag.SetUpStarted = "1";
                        ViewBag.MachineMode = "Setting";
                    }

                    var prvmodeMaint = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ModeType == "MNT" && m.ModeTypeEnd == 0)
                            .OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                    if (prvmodeMaint != null)
                    {
                        ViewBag.MNTStarted = "1";
                        ViewBag.MachineMode = "MNT";
                    }

                    var GetDashboardData = db.tbloperatordashboards.Where(m => /*m.MachineID == MachineID && */m.CorrectedDate == correctedDate).OrderByDescending(m => m.InsertedOn).ToList();
                    //var machname = db.tblmachinedetails.Find(MachineID);
                    tbloperatordashboard TOD = new tbloperatordashboard();
                    OperatorDashboard OD = new OperatorDashboard();
                    //OD.machinename = machname.MachineName;
                    OD.OpDashboardList = GetDashboardData;
                    OD.OpDashboard = TOD;
                    return View(OD);
                }
            //}
            return View();
        }
        [HttpPost]
        public string ServerPing()
        {
            string Status = "Disconnected";
            GetMode GM = new GetMode();
            //Ping ping = new Ping();
            //String TabIPAddress = GM.GetIPAddressofTabSystem();
            var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderBy(m => m.CellID).ToList();
            foreach (var item in celldet)
            {
                var cellid = item.CellID;
                //int MachineID = _ServerContext.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
                foreach (var mitem in machdet)
                {
                    //String IPAddress = GM.GetIPAddressofTabSystem();
                    var mid = mitem.MachineID;
                    var MachineDetails = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).FirstOrDefault();

                    //try
                    //{
                    //    //PingReply pingresult = ping.Send(MachineDetails.IPAddress);
                    //    if (pingresult.Status.ToString() == "Success")
                    //    {
                    //        Status = "Connected";
                    //    }
                    //}
                    //catch
                    //{
                    //    Status = "Disconnected";
                    //}
                    return Status = "Connected";
                }
            }
            return Status = "Connected";
        }

        public ContentResult lastNodeIdleCheck(int id, int lev)
        {
            var tblLossCodes = db.tbllossescodes.ToList();

            if (lev == 1)
            {
                if (tblLossCodes.Find(level => level.LossCodesLevel == 2 && level.LossCodesLevel1ID == id) == null) { return Content("true/" + id); }
                else
                {
                    return Content("false/" + id);
                }
            }
            else
            {
                if (tblLossCodes.Find(level => level.LossCodesLevel == 3 && level.LossCodesLevel2ID == id) == null) { return Content("true/" + id); }

                return Content("false/" + id);
            }
        }

        public ActionResult SaveIdle(int LossSelect = 0)
        {
            //request came from level 2 and was a last node .Level 3  code will come as parameter.
            if (LossSelect == 0)
            {
                if (Request.QueryString["selectLoss"] != null) // Ideally not null, if null go to hell :) 
                    LossSelect = Convert.ToInt32(Request.QueryString["selectLoss"]);
            }

            #region Update TblMode

            GetMode GM = new GetMode();
            //String IPAddress = GM.GetIPAddressofTabSystem();
            var celldet = db.tblcells.Where(m => m.IsDeleted == 0).OrderByDescending(m => m.CellID).ToList();
            foreach (var item in celldet)
            {
                var cellid = item.CellID;
                //int MachineID = _ServerContext.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                var machdet = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellid).OrderByDescending(m => m.MachineID).ToList();
                foreach (var mitem in machdet)
                {
                    // TempData["toaster_success"] = "pinging messages to server";

                    //String IPAddress = GM.GetIPAddressofTabSystem();

                    //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    var mid = mitem.MachineID;
                    //int MachineID = db.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    int MachineID = db.tblmachinedetails.Where(m => m.MachineID == mid && m.IsDeleted == 0).OrderByDescending(m => m.MachineID).Select(m => m.MachineID).First();

                    //int machineID = _ServerContext.tblmachinedetails.Where(m => m.TabIPAddress == IPAddress && m.IsDeleted == 0).Select(m => m.MachineID).First();
                    bool flag = GM.CheckIdleEntry(MachineID);
                    if (flag == true)
                    {
                        Session["MachineID"] = MachineID;
                        DateTime correctedDate = DateTime.Now;
                        tbldaytiming StartTime = db.tbldaytimings.Where(m => m.IsDeleted == 0).SingleOrDefault();
                        TimeSpan Start = StartTime.StartTime;
                        if (Start.Hours <= DateTime.Now.Hour)
                        {
                            correctedDate = DateTime.Now;
                        }
                        else
                        {
                            correctedDate = DateTime.Now.AddDays(-1);
                        }
                        int durationinsec = 0;
                        //var correctedDate = "2017-11-17";   // Hard coding for time being
                        string colorCode = "YELLOW";
                        //Update TblMode with the Loss Code
                        var mode = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ColorCode == colorCode && m.IsCompleted == 0 && m.StartIdle == 1).OrderByDescending(m => m.ModeID).FirstOrDefault();
                        DateTime ModeStartTime = DateTime.Now;
                        if (mode != null)
                        {
                            ModeStartTime = (DateTime)mode.StartTime;
                            durationinsec = Convert.ToInt32(DateTime.Now.Subtract(ModeStartTime).TotalSeconds);
                            mode.LossCodeID = LossSelect;
                            mode.ModeType = "IDLE";
                            mode.LossCodeEnteredTime = DateTime.Now;
                            mode.LossCodeEnteredBy = "";
                            mode.ModeTypeEnd = 1;
                            mode.IsCompleted = 1;
                            mode.StartIdle = 0;
                            mode.EndTime = DateTime.Now;
                            mode.DurationInSec = durationinsec;
                            mode.ModifiedOn = DateTime.Now; // doing now for testing purpose
                            mode.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                            db.Entry(mode).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();


                            //tblmode tm = new tblmode();
                            //tm.BreakdownID = mode.BreakdownID;
                            //tm.ColorCode = mode.ColorCode;
                            //tm.CorrectedDate = correctedDate;
                            //tm.InsertedBy = Convert.ToInt32(Session["UserID"]);
                            //tm.InsertedOn = DateTime.Now;
                            //tm.IsCompleted = 0;
                            //tm.IsDeleted = 0;
                            //tm.LossCodeID = null;
                            //tm.MachineID = MachineID;
                            //tm.MacMode = mode.MacMode;
                            //tm.ModeType = "IDLE";
                            //tm.ModeTypeEnd = 0;
                            //tm.StartIdle = 0;
                            //tm.StartTime = tm.InsertedOn;
                            ////tm.ServerModeID = ServerNewTM.ModeID;
                            //// tm.Sync = 0;  Ashok
                            //db.tblmodes.Add(tm);
                            //db.SaveChanges();

                            mode = db.tbllivemodes.Where(m => m.MachineID == MachineID && m.ColorCode == colorCode && m.IsCompleted == 0)
                                        .OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                        }
                        else
                        {

                        }
                        #endregion

                        #region UnLock the Machine
                        //var MacDet = _ServerContext.tblmachinedetails.Find(machineID);
                        //if (MacDet.MachineModelType == 1)
                        //{
                        //    AddFanucMachineWithConn AC = new AddFanucMachineWithConn(MacDet.IPAddress);
                        //    AC.SetMachineUnlock((ushort)MacDet.MachineUnlockBit, (ushort)MacDet.MachineLockBit);
                        //}
                        #endregion

                        #region Insert to Operator Dashboard

                        //Pre requsite before insertion ???
                        var losscode = db.tbllossescodes.Find(LossSelect);
                        tbloperatordashboard operatorDashboard = new tbloperatordashboard();
                        DateTime CorrectedDateToDate = Convert.ToDateTime(correctedDate);
                        Random OperatorDashboardID = new Random();
                        int machineid = Convert.ToInt32(Session["MachineID"]);
                        //operatorDashboard.OperatorDashboardID = OperatorDashboardID.Next(1, 9999999);  //remove this line once Identity is setup
                        operatorDashboard.MachineID = Convert.ToInt32(Session["MachineID"]);
                        operatorDashboard.CorrectedDate = CorrectedDateToDate;
                        operatorDashboard.SlNo = db.tbloperatordashboards.Where(m => m.CorrectedDate == CorrectedDateToDate).Where(m => m.MachineID == machineid).ToList().Count + 1; //  @Pavan , I'm not sure what you meant here..  --Count++ from the tbloperatordashboard for the Machine and CorrectedDate
                        operatorDashboard.MessageCode = "IDLE";
                        operatorDashboard.MessageDescription = "Machine in IDLE Mode" + " " + "-" + " " + losscode.LossCode;
                        operatorDashboard.MessageStartTime = ModeStartTime;
                        operatorDashboard.MessageEndTime = DateTime.Now;
                        operatorDashboard.TotalDurationinMin = Convert.ToInt32(DateTime.Now.Subtract(ModeStartTime).TotalMinutes);
                        operatorDashboard.InsertedOn = DateTime.Now;
                        operatorDashboard.InsertedBy = Convert.ToInt32(Session["UserID"]);
                        //operatorDashboard.ModifiedOn = DateTime.Now;
                        //operatorDashboard.ModifiedBy = 1;  // Session["UserID"]
                        operatorDashboard.IsDeleted = 0;

                        db.tbloperatordashboards.Add(operatorDashboard);
                        db.SaveChanges();

                        #endregion

                        #region Update tblOperatorHeader 

                        //get the latest row according to the MachineID and CorrectedDate ->  there is no CorrectedDate column 
                        var operatorHeader = db.tbloperatorheaders.Where(m => m.MachineID == MachineID).OrderByDescending(m => m.InsertedOn).FirstOrDefault();
                        if (operatorHeader != null)
                        {
                            operatorHeader.MachineMode = "Production";
                            operatorHeader.ModifiedOn = DateTime.Now;
                            operatorHeader.ModifiedBy = 1;// get from session once these screens are integrated....

                            db.Entry(operatorHeader).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        return RedirectToAction("DashboardProduction", new { value = MachineID });
                    }
                    else
                    {

                    }
                }
            }
            return RedirectToAction("DashboardProduction");
        }
        #endregion
    }
}
