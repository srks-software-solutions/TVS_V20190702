using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using I_Facility.ServerModel;
using System.Data.Entity;
using System.Data;
using System.Data.Entity.Core;

namespace I_Facility.Controllers
{
    public class CellsController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();

        //Gettimg Machine category list
        public ActionResult CellsList()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.plant = new SelectList(db.tblplants.Where(p => p.IsDeleted == 0), "PlantID", "PlantName").ToList();
            ViewBag.shop = new SelectList(db.tblshops.Where(d => d.IsDeleted == 0), "ShopId", "ShopName").ToList();
            CellsModel pa = new CellsModel();
            tblcell mp = new tblcell();
            pa.Cells = mp;
            pa.cellslist = db.tblcells.Where(m => m.IsDeleted == 0).ToList();

            return View(pa);

        }

        [HttpGet]
        public ActionResult CreateCells()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.plant = new SelectList(db.tblplants.Where(p => p.IsDeleted == 0), "PlantID", "PlantName").ToList();
            ViewBag.shop = new SelectList(db.tblshops.Where(d => d.IsDeleted == 0), "ShopId", "ShopName").ToList();
            return View();

        }

        [HttpPost]
        public ActionResult CreateCells(CellsModel tblp, int shop = 0, int Plant = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            int UserID = 1; //Convert.ToInt32(Session["UserId"]);
            //Cell name validation
            string cellname = tblp.Cells.ToString();
            var PlantId = tblp.Cells.PlantID;
            var count = tblp.Cells.NoOfModel;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var doesThisShopExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == Plant && m.ShopID == shop && m.CellName == cellname).ToList();
                if (doesThisShopExists.Count == 0)
                {
                    tblp.Cells.CreatedBy = UserID;
                    tblp.Cells.CreatedOn = DateTime.Now;
                    tblp.Cells.ShopID = shop;
                    tblp.Cells.PlantID = Plant;
                    tblp.Cells.IsDeleted = 0;
                    db.tblcells.Add(tblp.Cells);
                    db.SaveChanges();
                    var cellid = tblp.Cells.CellID;

                    for (var i = 0; i < count; i++)
                    {
                        tblcellpart cp = new tblcellpart();
                        cp.CellID = cellid;
                        cp.CreatedBy = 1;
                        cp.CreatedOn = DateTime.Now;
                        cp.IsDeleted = 0;
                        db.tblcellparts.Add(cp);
                        db.SaveChanges();
                    }
                    return RedirectToAction("CreatePart", (new { Id = cellid }));
                }
                else
                {
                    ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", Plant);
                    ViewBag.Shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", shop);
                    Session["Error"] = "Machine Name" + " " + tblp.Cells + " " + " already exists for this Plant/Department.";
                    return View(tblp);

                }

            }
        }
        //getPartDetails
        public ActionResult CreatePart(int Id)
        {
            List<tblcellpart> cpList = new List<tblcellpart>();

            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var data = db.tblcells.Where(m => m.CellID == Id).ToList();
                ViewBag.item = data;
                var items = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == Id).ToList();
                foreach (var item in items)
                {
                    tblcellpart tblcp = new tblcellpart();
                    tblcp.CellID = item.CellID;
                    tblcp.PartDescription = item.PartDescription;
                    tblcp.partNo = item.partNo;
                    cpList.Add(tblcp);
                }
                CellPartModel VM = new CellPartModel();
                VM.cellPartList = items;
                return View(VM);
            }
        }
        [HttpPost]
        public ActionResult CreatePart(CellPartModel tblpm)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            int UserID = 1;

            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var test = tblpm.cellPartList[0].CellID;

                var data = (from s in db.tblcellparts where s.CellID == test select s.cpID).ToList();
                int i = 0;
                foreach (var row in data)
                {

                    var partExistedornot = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.cpID == row).ToList();
                    partExistedornot[0].partNo = tblpm.cellPartList[i].partNo;
                    partExistedornot[0].PartDescription = tblpm.cellPartList[i].PartDescription;
                    db.SaveChanges();
                    if (i < data.Count())
                    {
                        i++;
                    }
                }
                return RedirectToAction("cellsList");

            }
            // return View();
        }

        [HttpGet]
        public ActionResult EditCellDetails(int Id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                tblcell tblmc = db.tblcells.Find(Id);
                //var celldata = db.tblcells.Where(m => m.CellID == Id).ToList();
                //List<tblcellpart> cpList = new List<tblcellpart>();
                //var partdata = db.tblcellparts.Where(m => m.CellID == Id).ToList();
                //foreach(var row in partdata)
                //{
                //    tblcellpart tblcp = new tblcellpart();
                //    tblcp.CellID = row.CellID;
                //    tblcp.PartDescription = row.PartDescription;
                //    tblcp.partNo = row.partNo;
                //    cpList.Add(tblcp);
                //}
                if (tblmc == null)
                {
                    return HttpNotFound();
                }
                int plantid = Convert.ToInt32(tblmc.PlantID);
                ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblmc.PlantID).ToList();
                ViewBag.DepartmentID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plantid), "ShopId", "ShopName", tblmc.ShopID).ToList();
                //CellpartEdit CE = new CellpartEdit();
                //CE.CellpartList = partdata;
                //CE.cellslist = celldata;
                return View(tblmc);
            }
        }

        [HttpPost]
        public ActionResult EditCellDetails(CellsModel objcell, int shop = 0, int Plant = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserID"]);
            //Cell name validation
            string cellname = objcell.Cells.CellName.ToString();
            int cellid = objcell.Cells.CellID;
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var doesThisdeptExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == Plant && m.ShopID == shop && m.CellName == cellname && m.CellID != cellid).ToList();
                if (doesThisdeptExists.Count == 0)
                {
                    var objCell = db.tblcells.Find(objcell.Cells.CellID);

                    objCell.PlantID = Plant;
                    objCell.ShopID = shop;
                    objCell.CellName = objcell.Cells.CellName;
                    objCell.CellDesc = objcell.Cells.CellDesc;
                    objCell.CelldisplayName = objcell.Cells.CelldisplayName;
                    objCell.NoOfModel = objcell.Cells.NoOfModel;


                    db.Entry(objCell).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("CellsList");
                }
                else
                {
                    ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", objcell.Cells.PlantID).ToList();
                    ViewBag.DepartmentID = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "DepartmentID", "DepartmentDisplayName", objcell.Cells.ShopID).ToList();
                    Session["Error"] = "Machine Name" + " " + objcell.Cells + " " + "already exists for this Department.";
                    return View(objcell);
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
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                tblcell tblmc = db.tblcells.Find(id);
                //tblcellpart tblcp = db.tblcellparts.Find(id);
                //var tblmc = db.tblcells.Where(s => s.CellID == id).FirstOrDefault();

                int i = 0;
                if (tblmc == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    int plantid = Convert.ToInt32(tblmc.PlantID);
                    ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblmc.PlantID).ToList();
                    ViewBag.shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plantid), "ShopId", "ShopName", tblmc.ShopID).ToList();
                    var count = tblmc.NoOfModel;
                    ViewBag.item = count;
                    List<CellpartEdit> obj2 = new List<CellpartEdit>();
                    CellpartEdit obj = new CellpartEdit();
                    obj.cell = tblmc;

                    obj2.Add(obj);
                    var tblcp = db.tblcellparts.Where(m => m.CellID == id).ToList();
                    if (tblcp.Count == 0)
                    {
                        for (i = 0; i < count; i++)
                        {
                            tblcellpart cp = new tblcellpart();
                            cp.CellID = id;
                            cp.CreatedBy = 1;
                            cp.CreatedOn = DateTime.Now;
                            cp.IsDeleted = 0;
                            db.tblcellparts.Add(cp);
                            db.SaveChanges();
                        }
                        return RedirectToAction("CreatePart", new { Id = id });
                    }
                    else
                    {
                        foreach (var item in tblcp)
                        {
                            CellpartEdit obj1 = new CellpartEdit();

                            var temp = db.tblcellparts.Where(m => m.cpID == item.cpID).FirstOrDefault();
                            obj1.cellpart = temp;



                            obj2.Add(obj1);
                        }
                    }
                    // return View(tblmc);
                    return View(obj2);

                    //return View("Edit", obj1);
                }
            }
        }
        [HttpPost]
        public ActionResult Edit(cellpartDetails tblcp, int plant = 0, int shop = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID = Convert.ToInt32(Session["UserID"]);
            //int cellid = (from s in db.tblcells where s.PlantID == plant select s.CellID).FirstOrDefault();
            //string cellname = (from s in db.tblcells where s.PlantID == plant select s.CellName).FirstOrDefault();
            string cellname = tblcp.CellName.ToString();
            int cellid = tblcp.CellID;
            var NoOfModel = (from s in db.tblcells where (s.CellID == cellid) select s.NoOfModel).FirstOrDefault();
            var count = (int)NoOfModel;

            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                var doesThisdeptExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == plant && m.ShopID == shop && m.CellName == cellname && m.CellID != cellid).ToList();
                if (doesThisdeptExists.Count == 0)
                {
                    var celldata = db.tblcells.Find(cellid);
                    celldata.PlantID = plant;
                    celldata.ShopID = shop;
                    celldata.CellName = tblcp.CellName;
                    celldata.CellDesc = tblcp.CellDesc;
                    celldata.CelldisplayName = tblcp.CellDisplayname;
                    celldata.NoOfModel = tblcp.NoofModel;
                    db.Entry(celldata).State = EntityState.Modified;
                    db.SaveChanges();
                    int modelCount = (int)celldata.NoOfModel;
                    if (count < modelCount)
                    {
                        for (var i = (int)NoOfModel; i < modelCount; i++)
                        {
                            tblcellpart cp = new tblcellpart();
                            cp.CellID = cellid;
                            cp.CreatedBy = 1;
                            cp.CreatedOn = DateTime.Now;
                            cp.IsDeleted = 0;
                            db.tblcellparts.Add(cp);
                            db.SaveChanges();
                        }
                        return RedirectToAction("createPart");
                    }

                    else if (count > modelCount)
                    {

                        TempData["message"] = "Select which Part You want to delete";
                        return View();
                    }
                    else
                    {
                        if (count == modelCount)
                        {
                            return View();
                        }
                    }

                }

                // return RedirectToAction("CellsList");
            }

            int partno = tblcp.PartNo;
            var cpid = tblcp.CpID;
            var partexistornot = db.tblcellparts.Where(m => m.IsDeleted == 0 && Convert.ToInt32(m.partNo) == partno && m.cpID != cpid).ToList();
            if (partexistornot.Count == 0)
            {
                var partdata = db.tblcellparts.Find(cellid);
                partdata.partNo = partdata.partNo;
                partdata.PartDescription = partdata.PartDescription;
                db.SaveChanges();
            }

            else
            {
                ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblcp.PlantID).ToList();
                ViewBag.shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plant), "ShopId", "ShopName", tblcp.ShopID).ToList();
                Session["Error"] ="already exists for this shop.";
                return View("Edit", tblcp);
            }


            return RedirectToAction("CellsList");
        }

        //public ActionResult Edit(int plant = 0, int cpid = 0, int cellid = 0, int shop = 0, string cellname = "", string catdesc = "", string catdispname = "", int NoOfModel = 0, int partno = 0, string partdescription = "")
        //{
        //    if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
        //    {
        //        return RedirectToAction("Login", "Login", null);
        //    }
        //    ViewBag.Logout = Session["Username"].ToString().ToUpper();
        //    ViewBag.roleid = Session["RoleID"];
        //    String Username = Session["Username"].ToString();
        //    int UserID = Convert.ToInt32(Session["UserID"]);
        //    //string cellname = tblcp.CellName.ToString();
        //    //int cellid = tblcp.CellID;
        //    var NoOfModel = (from s in db.tblcells where (s.CellID == cellid) select s.NoOfModel).FirstOrDefault();
        //    var count = (int)NoOfModel;

        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        var doesThisdeptExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == plant && m.ShopID == shop && m.CellName == cellname && m.CellID != cellid).ToList();
        //        if (doesThisdeptExists.Count == 0)
        //        {
        //            var celldata = db.tblcells.Find(cellid);
        //            celldata.PlantID = plant;
        //            celldata.ShopID = shop;
        //            celldata.CellName = cellname;
        //            celldata.CellDesc = catdesc;
        //            celldata.CelldisplayName = catdispname;
        //            celldata.NoOfModel = NoOfModel;
        //            db.Entry(celldata).State = EntityState.Modified;
        //            db.SaveChanges();
        //            int modelCount = (int)celldata.NoOfModel;
        //            if (count < modelCount)
        //            {
        //                for (var i = (int)NoOfModel; i < modelCount; i++)
        //                {
        //                    tblcellpart cp = new tblcellpart();
        //                    cp.CellID = cellid;
        //                    cp.CreatedBy = 1;
        //                    cp.CreatedOn = DateTime.Now;
        //                    cp.IsDeleted = 0;
        //                    db.tblcellparts.Add(cp);
        //                    db.SaveChanges();
        //                }
        //                return RedirectToAction("createPart");
        //            }

        //            else if (count > modelCount)
        //            {

        //                TempData["message"] = "Select which Part You want to delete";
        //                return View();
        //            }
        //            else
        //            {
        //                if (count == modelCount)
        //                {
        //                    return View();
        //                }
        //            }

        //        }

        //        // return RedirectToAction("CellsList");
        //    }

        //    ////int partno = tblcp.PartNo;
        //    //var cpid = tblcp.CpID;
        //    var partexistornot = db.tblcellparts.Where(m => m.IsDeleted == 0 && Convert.ToInt32(m.partNo) == partno && m.cpID != cpid).ToList();
        //    if (partexistornot.Count == 0)
        //    {
        //        var partdata = db.tblcellparts.Find(cellid);
        //        partdata.partNo = partdata.partNo;
        //        partdata.PartDescription = partdata.PartDescription;
        //        db.SaveChanges();
        //    }

        //    else
        //    {
        //        ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", plant).ToList();
        //        ViewBag.shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plant), "ShopId", "ShopName", shop).ToList();
        //        Session["Error"] = "already exists for this shop.";
        //        return View("Edit");
        //    }


        //    return RedirectToAction("CellsList");
        //}

        //delete Cell and Part Details
        public ActionResult DeleteCells(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID1 = id;
            tblcell tblmc = db.tblcells.Find(id);

            //Getting all the cellid of both tblcell and tblcellpart
            var cellpart = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == id).ToList();
            foreach (var row in cellpart)
            {
                //deleting the row of perticular id in both tblcell and tblcellpart table
                row.IsDeleted = 1;
                db.Entry(row).State = EntityState.Modified;
                db.SaveChanges();
            }
            //otherwise delete the row only present in tblcell table
            tblmc.IsDeleted = 1;
            db.Entry(tblmc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("CellsList");
        }

        public ActionResult DeleteParts(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID1 = id;
            //tblcellpart tblcp = db.tblcellparts.Find(id);
            var tblcp = db.tblcellparts.Where(m => m.CellID == id).FirstOrDefault();
            tblcp.IsDeleted = 1;
            tblcp.ModifiedBy = UserID1;
            tblcp.ModifiedOn = DateTime.Now;
            db.Entry(tblcp).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Edit");
        }

        public ActionResult CellCategoryById(int Id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }

            var Data = db.tblcells.Where(m => m.CellID == Id).Select(m => new { PlantId = m.PlantID, DepartmentId = m.ShopID, machinecategory = m.CellName, catdesc = m.CellDesc, catdeisplay = m.CelldisplayName, NoOfModel = m.NoOfModel });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PartById(int id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            var Data = db.tblcellparts.Where(m => m.CellID == id).Select(m => new { partno = m.partNo, partdesc = m.PartDescription });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FetchDept(int PID)
        {

            var DeptData = (from row in db.tblshops
                            where row.IsDeleted == 0 && row.PlantID == PID
                            select new { Value = row.ShopID, Text = row.Shopdisplayname }
                                ).ToList();
            return Json(DeptData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public string CellNameDuplicateCheck(int plantID = 0, int shopId = 0, string cellName = "")
        {
            string status = "notok";
            var doesThisCellExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == plantID && m.ShopID == shopId && m.CellName == cellName).ToList();
            if (doesThisCellExists.Count == 0)
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
        public string CellNameDuplicateCheckEdit(int plantID = 0, int shopId = 0, string cellName = "", int editCellID = 0)
        {
            string status = "notok";
            var doesThisCellExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == plantID && m.ShopID == shopId && m.CellName == cellName).ToList();
            if (doesThisCellExists.Count == 0)
            {
                status = "ok";
            }
            else
            {
                var checkforId = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == plantID && m.ShopID == shopId && m.CellID == editCellID && m.CellName == cellName).ToList();//checks for that cellid
                if (checkforId.Count != 0)
                {
                    status = "ok";
                }
                else
                {
                    status = "notok";
                }
            }
            return status;
        }

        [HttpPost]
        public string ChildNodeCheck(int id = 0)
        {
            string status = "";
            var macChild = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == id).ToList();
            if (macChild.Count == 0)
            {
                status = "";
            }
            else
            {
                status = "The Cell is having dependent machines, Do you want to continue(If Yes every machine having this cell will be deleted)";
            }
            return status;
        }
    }
}