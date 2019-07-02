using I_Facility.ServerModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace I_Facility.Controllers
{
    public class cellpartController : Controller
    {
        i_facilityEntities1 db = new i_facilityEntities1();
        // GET: cellpart
        public ActionResult Index()
        {

            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            cellpartdetailsmodel pa = new cellpartdetailsmodel();
            tblcell mp = new tblcell();
            pa.cell = mp;
            pa.celllist = db.tblcells.Where(m => m.IsDeleted == 0).ToList();
            return View(pa);
        }
        [HttpGet]
        public ActionResult Createcell()
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
        public ActionResult Createcell(cellpartdetailsmodel tblp, int shop = 0, int Plant = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            int UserID = 1; //Convert.ToInt32(Session["UserId"]);
            //Cell name validation
            string cellname = tblp.cell.CellName;
            var PlantId = tblp.cell.PlantID;
            var count = tblp.cell.NoOfModel;
            var doesThisShopExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == Plant && m.ShopID == shop && m.CellName == cellname && m.CellDesc == tblp.cell.CellDesc && m.CelldisplayName == tblp.cell.CelldisplayName).ToList();
            if (doesThisShopExists.Count == 0)
            {
                tblp.cell.CreatedBy = UserID;
                tblp.cell.CreatedOn = DateTime.Now;
                tblp.cell.ShopID = shop;
                tblp.cell.PlantID = Plant;
                tblp.cell.IsDeleted = 0;
                db.tblcells.Add(tblp.cell);
                db.SaveChanges();
                var cellid = tblp.cell.CellID;
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
                TempData["Message"] = "This Cell is already exists for this plant/Shop.";
                // Session["Error"] = "Machine Name" + " " + tblp.cell + " " + " already exists for this Plant/Department.";
                return View(tblp);

            }
        }
        public ActionResult CreatePart(int id)
        {
            List<tblcellpart> cpList = new List<tblcellpart>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                //var data = db.tblcells.Where(m => m.CellID == id).ToList();
                var det = db.tblcells.Where(m => m.CellID == id).Select(m => m.CellName).FirstOrDefault();
                var data = db.tblcells.Where(m => m.CellID == id).Select(m => m.CellDesc).FirstOrDefault();
                var data1 = db.tblcells.Where(m => m.CellID == id).Select(m => m.CelldisplayName).FirstOrDefault();
                var data2 = db.tblcells.Where(m => m.CellID == id).Select(m => m.NoOfModel).FirstOrDefault();
                ViewBag.item = det;
                ViewBag.item1 = data;
                ViewBag.item2 = data1;
                ViewBag.item3 = data2;

                var items = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == id).ToList();
                foreach (var item in items)
                {
                    tblcellpart tblcp = new tblcellpart();
                    tblcp.CellID = item.CellID;
                    tblcp.PartDescription = item.PartDescription;
                    tblcp.partNo = item.partNo;
                    cpList.Add(tblcp);
                }
                cellpartdetailsmodel VM = new cellpartdetailsmodel();
                VM.cellpartliat = items;
                return View(VM);
            }
        }
        [HttpPost]
        public ActionResult CreatePart(cellpartdetailsmodel tblpm)
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
                var test = tblpm.cellpartliat[0].CellID;

                var data = (from s in db.tblcellparts where s.CellID == test && s.IsDeleted == 0 select s.cpID).ToList();
                int i = 0;
                foreach (var row in data)
                {

                    var partExistedornot = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.cpID == row).ToList();
                    partExistedornot[0].partNo = tblpm.cellpartliat[i].partNo;
                    partExistedornot[0].PartDescription = tblpm.cellpartliat[i].PartDescription;
                    db.SaveChanges();
                    if (i < data.Count())
                    {
                        i++;
                    }
                }
                return RedirectToAction("Index");

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
                tblcell tblmc = db.tblcells.Find(id);
                if (tblmc == null)
                {
                    return HttpNotFound();
                }
                else

                {
                    int plantid = Convert.ToInt32(tblmc.PlantID);
                    ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblmc.PlantID).ToList();
                    ViewBag.shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plantid), "ShopId", "ShopName", tblmc.ShopID).ToList();
                    List<cellpartdetailsmodel> obj2 = new List<cellpartdetailsmodel>();
                    cellpartdetailsmodel obj = new cellpartdetailsmodel();
                    obj.cell = tblmc;

                    obj2.Add(obj);
                    var tblcp = db.tblcellparts.Where(m => m.CellID == id && m.IsDeleted == 0).ToList();
                    foreach (var item in tblcp)
                    {
                        cellpartdetailsmodel obj1 = new cellpartdetailsmodel();

                        var temp = db.tblcellparts.Where(m => m.cpID == item.cpID && m.IsDeleted == 0).FirstOrDefault();
                        obj1.cellpart = temp;



                        obj2.Add(obj1);
                    }
                    return View(obj2);
                }
            }
        }

        //[HttpPost]
        public string Edit(int plant = 0, int cellid = 0, int shop = 0, string cellname = "", string catdesc = "", string catdispname = "", int Noofmodel = 0)
        {
            string res = "";
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {

                ViewBag.Logout = Session["Username"].ToString().ToUpper();
                ViewBag.roleid = Session["RoleID"];
                ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", plant).ToList();
                ViewBag.shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plant), "ShopId", "ShopName", shop).ToList();
                string Username = Session["Username"].ToString();
                int UserID = Convert.ToInt32(Session["UserID"]);
                var noofmodel = (from s in db.tblcells where (s.CellID == cellid) select s.NoOfModel).FirstOrDefault();
                if (noofmodel == null)
                {
                    noofmodel = 1;
                }
                var count = (int)noofmodel;

                var doesThisExists = db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == plant && m.ShopID == shop && m.CellName == cellname && m.CellID != cellid).ToList();
                if (doesThisExists.Count == 0)
                {
                    var celldata = db.tblcells.Find(cellid);
                    if (celldata == null)
                    {

                    }
                    else
                    {
                        celldata.PlantID = plant;
                        celldata.ShopID = shop;
                        celldata.CellName = cellname;
                        celldata.CellDesc = catdesc;
                        celldata.CelldisplayName = catdispname;
                        celldata.NoOfModel = Noofmodel;
                        db.Entry(celldata).State = EntityState.Modified;
                        db.SaveChanges();
                        res = "Success";

                        int modelCount = (int)celldata.NoOfModel;
                        if (count < modelCount)
                        {
                            for (var i = (int)noofmodel; i < modelCount; i++)
                            {
                                tblcellpart cp = new tblcellpart();
                                cp.CellID = cellid;
                                cp.CreatedBy = 1;
                                cp.CreatedOn = DateTime.Now;
                                cp.IsDeleted = 0;
                                db.tblcellparts.Add(cp);
                                db.SaveChanges();
                                res = "saved";
                            }
                            //return RedirectToAction("EditPart", new { Id = cellid });
                        }
                        else if (count > modelCount)
                        {
                            //TempData["Success"] = "Record Modified Successfully!!!!";
                            Session["Message"] = "Select which Part You want to delete....";
                            res = "delete";
                            // return RedirectToAction("EditPart", new { Id = cellid });
                        }
                        else if (count == modelCount)
                        {
                            res = "equal";
                            //return RedirectToAction("EditPart");
                        }

                        //}
                    }
                }


                return res;
            }
            //var celldat = db.tblcells.Where(m => m.CellID == cellid && m.IsDeleted == 0).FirstOrDefault();
            //var partdata = db.tblcellparts.Where(m => m.CellID == cellid && m.IsDeleted == 0).ToList();
            //List<cellpartdetailsmodel> obj = new List<cellpartdetailsmodel>();
            //cellpartdetailsmodel tblc = new cellpartdetailsmodel();
            //tblc.cellpartliat = partdata;
            //cellpartdetailsmodel tblcp = new cellpartdetailsmodel();
            //tblcp.cell = celldat;
            //obj.Add(tblcp);
            //obj.Add(tblc);
            //return View(tblcp);

        }

        [HttpGet]
        public ActionResult EditPart(int Id)
        {
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                tblcell tblmc = db.tblcells.Find(Id);
                var partdata = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == Id).ToList();
                List<cellpartdetailsmodel> obj = new List<cellpartdetailsmodel>();
                cellpartdetailsmodel tblc = new cellpartdetailsmodel();
                tblc.cell = tblmc;
                obj.Add(tblc);
                int plantid = Convert.ToInt32(tblmc.PlantID);
                ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", tblmc.PlantID).ToList();
                ViewBag.shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == plantid), "ShopId", "ShopName", tblmc.ShopID).ToList();
                foreach (var item in partdata)
                {
                    cellpartdetailsmodel obj1 = new cellpartdetailsmodel();
                    var temp = db.tblcellparts.Where(m => m.cpID == item.cpID && m.IsDeleted == 0).FirstOrDefault();
                    obj1.cellpart = temp;
                    obj.Add(obj1);
                }
                //TempData["message"] = "Select which Part You want to delete....";
                return View(obj);
            }
        }

        public string UpdateData(int id, string partno)
        {
            string res = "";
            var cpid = db.tblcellparts.Where(m => m.cpID == id && m.IsDeleted == 0).FirstOrDefault();
            cpid.partNo = partno;
            db.Entry(cpid).State = EntityState.Modified;
            db.SaveChanges();
            res = "Success";
            return res;
        }
        public string UpdateData1(int id, string partdesc)
        {
            string res = "";
            var cpid = db.tblcellparts.Where(m => m.cpID == id && m.IsDeleted == 0).FirstOrDefault();
            cpid.PartDescription = partdesc;
            db.Entry(cpid).State = EntityState.Modified;
            db.SaveChanges();
            res = "Success";
            return res;
        }
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
            //Getting all the details of that particular id in cell table
            tblcell tblmc = db.tblcells.Find(id);

            //Getting all the details of that particular id in cellpart table
            var cellpart = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == id).ToList();
            foreach (var row in cellpart)
            {
                //deleting the perticular row of that id in both tblcell and tblcellpart table
                row.IsDeleted = 1;
                db.Entry(row).State = EntityState.Modified;
                db.SaveChanges();
            }
            //otherwise delete the row only present in tblcell table
            tblmc.IsDeleted = 1;
            db.Entry(tblmc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //public ActionResult DeleteParts(int id=0)
        //{
        //    if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
        //    {
        //        return RedirectToAction("Login", "Login", null);
        //    }
        //    ViewBag.Logout = Session["Username"].ToString().ToUpper();
        //    ViewBag.roleid = Session["RoleID"];
        //    String Username = Session["Username"].ToString();
        //    int UserID1 = id;
        //    //tblcellpart tblcp = db.tblcellparts.Find(id);
        //    var tblcp = db.tblcellparts.Where(m => m.cpID == id).FirstOrDefault();
        //    var cellid = tblcp.CellID;
        //    tblcp.IsDeleted = 1;
        //    tblcp.ModifiedBy = UserID1;
        //    tblcp.ModifiedOn = DateTime.Now;
        //    db.Entry(tblcp).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("EditPart", new { id = cellid });
        //}
        public string DeleteParts(int id)
        {
            string res = "";
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            int UserID1 = id;
            //tblcellpart tblcp = db.tblcellparts.Find(id);
            var tblcp = db.tblcellparts.Where(m => m.cpID == id).FirstOrDefault();
            var cellid = tblcp.CellID;
            tblcp.IsDeleted = 1;
            tblcp.ModifiedBy = UserID1;
            tblcp.ModifiedOn = DateTime.Now;
            db.Entry(tblcp).State = EntityState.Modified;
            db.SaveChanges();
            res = "success";
            return res;
            //return RedirectToAction("EditPart", new { id = cellid });
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
        public string PartNumDuplicateCheck(int cellid = 0, string Partno = " ")
        {
            string status = "notok";
            var doesThisPartExists = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == cellid && m.partNo == Partno).ToList();
            if (doesThisPartExists.Count == 0)
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
        public string PartNumDuplicateCheckEdit(int cellid = 0, string PartNo = "", int Editcpid = 0)
        {
            string status = "notok";
            if (Editcpid != 0)
            {
                var doesThispartnoExist = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.CellID == cellid && m.partNo == PartNo).ToList();
                if (doesThispartnoExist.Count == 0)
                {
                    status = "ok";
                }
                else
                {
                    var checkforId = db.tblcellparts.Where(m => m.IsDeleted == 0 && m.partNo == PartNo && m.cpID == Editcpid).ToList();
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