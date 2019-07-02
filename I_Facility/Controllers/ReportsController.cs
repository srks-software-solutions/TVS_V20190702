using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.IO;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data.SqlClient;
using I_Facility.ServerModel;
using I_Facility;
using MySql.Data.MySqlClient;
using I_Facility.Models;
using System.Data.Entity;

namespace I_Facility.Controllers
{
    public class ReportsController : Controller
    {
        i_facilityEntities1 Serverdb = new i_facilityEntities1();

        // GET: Reports
        public ActionResult MasterPartsReport()
        {
            return View();
        }

        public ActionResult Utilization_ABGraph()
        {

            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            return View();
        }

        //[HttpPost]
        //public ActionResult Utilization_ABGraph(int PlantID, String FromDate, String ToDate, int ShopID = 0, int CellID = 0, int MachineID = 0)
        //{
        //    ReportsCalcClass.UtilizationReport UR = new ReportsCalcClass.UtilizationReport();
        //    UR.CalculateUtilization(PlantID, ShopID, CellID, MachineID, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate));

        //    var getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

        //    if (MachineID != 0)
        //    {
        //        getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
        //    }
        //    else if (CellID != 0)
        //    {
        //        getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
        //    }
        //    else if (ShopID != 0)
        //    {
        //        getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
        //    }


        //    int dateDifference = Convert.ToDateTime(ToDate).Subtract(Convert.ToDateTime(FromDate)).Days;

        //    FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\UtilizationReport_Graph.xlsx");

        //    ExcelPackage templatep = new ExcelPackage(templateFile);
        //    ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
        //    ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];

        //    String FileDir = @"C:\I_ShopFloorReports\ReportsList\" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
        //    bool exists = System.IO.Directory.Exists(FileDir);
        //    if (!exists)
        //        System.IO.Directory.CreateDirectory(FileDir);

        //    FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "UtilizationReport_Graph" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
        //    if (newFile.Exists)
        //    {
        //        try
        //        {
        //            newFile.Delete();  // ensures we create a new workbook
        //            newFile = new FileInfo(System.IO.Path.Combine(FileDir, "UtilizationReport_Graph" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx"));
        //        }
        //        catch
        //        {
        //            TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
        //            //return View();
        //        }
        //    }
        //    //Using the File for generation and populating it
        //    ExcelPackage p = null;
        //    p = new ExcelPackage(newFile);
        //    ExcelWorksheet worksheet = null;
        //    ExcelWorksheet worksheetGraph = null;
        //    ExcelWorksheet workSheetGraphData = null;

        //    //Creating the WorkSheet for populating
        //    try
        //    {
        //        worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy"), Templatews);
        //        worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
        //        workSheetGraphData = p.Workbook.Worksheets.Add("GraphData", TemplateGraph);
        //    }
        //    catch { }

        //    if (worksheet == null)
        //    {
        //        worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy") + "1", Templatews);
        //        worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
        //        workSheetGraphData = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "GraphData", TemplateGraph);
        //    }
        //    int sheetcount = p.Workbook.Worksheets.Count;
        //    p.Workbook.Worksheets.MoveToStart(sheetcount);
        //    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        //    int StartRow = 3;
        //    int SlNo = 1;
        //    for (int i = 0; i <= dateDifference; i++)
        //    {
        //        DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
        //        //string corrdate = QueryDate.ToString("yyyy-MM-dd");
        //        foreach (var Machine in getMachineList)
        //        {
        //            var GetUtilList = Serverdb.tbl_utilreport.Where(m => m.MachineID == Machine.MachineID && m.CorrectedDate == QueryDate.Date).ToList();
        //            foreach (var MacRow in GetUtilList)
        //            {
        //                worksheet.Cells["A" + StartRow].Value = SlNo++;
        //                worksheet.Cells["B" + StartRow].Value = QueryDate.Date.ToString("dd-MM-yyyy");
        //                worksheet.Cells["C" + StartRow].Value = MacRow.tblmachinedetail.tblplant.PlantDisplayName;
        //                worksheet.Cells["D" + StartRow].Value = MacRow.tblmachinedetail.tblshop.Shopdisplayname;
        //                worksheet.Cells["E" + StartRow].Value = MacRow.tblmachinedetail.tblcell.CelldisplayName;
        //                worksheet.Cells["F" + StartRow].Value = MacRow.tblmachinedetail.MachineDisplayName;
        //                worksheet.Cells["G" + StartRow].Value = MacRow.TotalTime;
        //                worksheet.Cells["H" + StartRow].Value = MacRow.OperatingTime;
        //                worksheet.Cells["I" + StartRow].Value = MacRow.SetupTime;
        //                worksheet.Cells["J" + StartRow].Value = (MacRow.MinorLossTime - MacRow.SetupMinorTime);
        //                worksheet.Cells["K" + StartRow].Value = MacRow.LossTime;
        //                worksheet.Cells["L" + StartRow].Value = MacRow.BDTime;
        //                worksheet.Cells["M" + StartRow].Value = MacRow.PowerOffTime;
        //                worksheet.Cells["N" + StartRow].Value = MacRow.UtilPercent + " %";
        //                StartRow++;
        //            }
        //        }
        //    }

        //    int rowCount = 2 + dateDifference;
        //    //getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

        //    int oldcolumn = 2;
        //    int height = 20;
        //    foreach (var Machine1 in getMachineList)
        //    {
        //        int currentCOlumn = oldcolumn;
        //        string macName = Machine1.MachineDisplayName;
        //        int StartRow1 = 2;
        //        for (int i = 0; i <= dateDifference; i++)
        //        {
        //            DateTime QueryDate1 = Convert.ToDateTime(FromDate).AddDays(i);
        //            //string corrdate = QueryDate1.ToString("yyyy-MM-dd");
        //            var GetUtilList = Serverdb.tbl_utilreport.Where(m => m.MachineID == Machine1.MachineID && m.CorrectedDate == QueryDate1.Date).ToList();
        //            foreach (var MacRow in GetUtilList)
        //            {
        //                string ColEntry = ExcelColumnFromNumber(currentCOlumn);
        //                workSheetGraphData.Cells[ColEntry + "" + StartRow1].Value = QueryDate1.Date.ToString("dd-MM-yyyy");
        //                ColEntry = ExcelColumnFromNumber(currentCOlumn + 1);
        //                workSheetGraphData.Cells[ColEntry + "" + StartRow1].Value = MacRow.tblmachinedetail.MachineDisplayName;
        //                ColEntry = ExcelColumnFromNumber(currentCOlumn + 2);
        //                workSheetGraphData.Cells[ColEntry + "" + StartRow1].Value = MacRow.OperatingTime;
        //                StartRow1++;
        //            }

        //        }
        //        if (StartRow1 > 2)
        //        {
        //            oldcolumn = currentCOlumn + 3;
        //            var chartIDAndUnID = (ExcelBarChart)worksheetGraph.Drawings.AddChart("Graph-" + macName, eChartType.ColumnStacked);

        //            chartIDAndUnID.SetSize((40 * rowCount), 350);

        //            chartIDAndUnID.SetPosition(height, 20);
        //            height = height + 400;

        //            chartIDAndUnID.Title.Text = "Graph - " + macName;
        //            chartIDAndUnID.Style = eChartStyle.Style18;
        //            chartIDAndUnID.Legend.Position = eLegendPosition.Bottom;
        //            //chartIDAndUnID.Legend.Remove();
        //            chartIDAndUnID.YAxis.MaxValue = 24;
        //            chartIDAndUnID.YAxis.MinValue = 0;
        //            chartIDAndUnID.YAxis.MajorUnit = 4;

        //            chartIDAndUnID.Locked = false;
        //            chartIDAndUnID.PlotArea.Border.Width = 0;
        //            chartIDAndUnID.YAxis.MinorTickMark = eAxisTickMark.None;
        //            chartIDAndUnID.DataLabel.ShowValue = true;
        //            chartIDAndUnID.DisplayBlanksAs = eDisplayBlanksAs.Gap;
        //            string ColEntry1 = ExcelColumnFromNumber(currentCOlumn);
        //            ExcelRange dateWork = workSheetGraphData.Cells[ColEntry1 + "2:" + ColEntry1 + rowCount];
        //            ColEntry1 = ExcelColumnFromNumber(currentCOlumn + 2);
        //            ExcelRange hoursWork = workSheetGraphData.Cells[ColEntry1 + "2:" + ColEntry1 + rowCount];
        //            workSheetGraphData.Hidden = eWorkSheetHidden.Hidden;
        //            var hours = (ExcelChartSerie)(chartIDAndUnID.Series.Add(hoursWork, dateWork));
        //            hours.Header = "Operating Time (Hours)";
        //            //Get reference to the worksheet xml for proper namespace
        //            var chartXml = chartIDAndUnID.ChartXml;
        //            var nsuri = chartXml.DocumentElement.NamespaceURI;
        //            var nsm = new XmlNamespaceManager(chartXml.NameTable);
        //            nsm.AddNamespace("c", nsuri);

        //            //XY Scatter plots have 2 value axis and no category
        //            var valAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:valAx", nsm);
        //            if (valAxisNodes != null && valAxisNodes.Count > 0)
        //                foreach (XmlNode valAxisNode in valAxisNodes)
        //                {
        //                    var major = valAxisNode.SelectSingleNode("c:majorGridlines", nsm);
        //                    if (major != null)
        //                        valAxisNode.RemoveChild(major);

        //                    var minor = valAxisNode.SelectSingleNode("c:minorGridlines", nsm);
        //                    if (minor != null)
        //                        valAxisNode.RemoveChild(minor);
        //                }

        //            //Other charts can have a category axis
        //            var catAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:catAx", nsm);
        //            if (catAxisNodes != null && catAxisNodes.Count > 0)
        //                foreach (XmlNode catAxisNode in catAxisNodes)
        //                {
        //                    var major = catAxisNode.SelectSingleNode("c:majorGridlines", nsm);
        //                    if (major != null)
        //                        catAxisNode.RemoveChild(major);

        //                    var minor = catAxisNode.SelectSingleNode("c:minorGridlines", nsm);
        //                    if (minor != null)
        //                        catAxisNode.RemoveChild(minor);
        //                }
        //        }
        //    }
        //    p.Workbook.Worksheets.MoveToStart(3);
        //    p.Save();

        //    //Downloding Excel
        //    string path1 = System.IO.Path.Combine(FileDir, "UtilizationReport_Graph" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx");
        //    DownloadUtilReport(path1, "UtilizationReport_Graph", ToDate);

        //    ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantID);
        //    ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName", ShopID);
        //    ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName", CellID);
        //    ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName", MachineID);

        //    return View();
        //}

        [HttpPost]
        public ActionResult Utilization_ABGraph(int PlantID, String FromDate, String ToDate, int ShopID = 0, int CellID = 0, int MachineID = 0)
        {
            ReportsCalcClass.UtilizationReport UR = new ReportsCalcClass.UtilizationReport();
            UR.CalculateUtilization(PlantID, ShopID, CellID, MachineID, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate));

            var getMachineList = new List<tblmachinedetail>();
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();
            }

            if (MachineID != 0)
            {
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
                }

            }
            else if (CellID != 0)
            {
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
                }

            }
            else if (ShopID != 0)
            {
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
                }

            }


            int dateDifference = Convert.ToDateTime(ToDate).Subtract(Convert.ToDateTime(FromDate)).Days;

            FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\UtilizationReport_Graph.xlsx");

            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
            ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];

            String FileDir = @"C:\I_ShopFloorReports\ReportsList\" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "UtilizationReport_Graph" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "UtilizationReport_Graph" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    //return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;
            ExcelWorksheet worksheetGraph = null;
            ExcelWorksheet workSheetGraphData = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy"), Templatews);
                worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
                workSheetGraphData = p.Workbook.Worksheets.Add("GraphData", TemplateGraph);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy") + "1", Templatews);
                worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
                workSheetGraphData = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "GraphData", TemplateGraph);
            }
            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            int StartRow = 3;
            int SlNo = 1;

            for (int i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                //string corrdate = QueryDate.ToString("yyyy-MM-dd");
                foreach (var Machine in getMachineList)
                {

                    var GetUtilList = new List<tbl_utilreport>();
                    var Plant = new List<tblplant>();
                    var shopList = new List<tblshop>();
                    var cellList = new List<tblcell>();
                    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                    {
                        GetUtilList = Serverdb.tbl_utilreport.Where(m => m.MachineID == Machine.MachineID && m.CorrectedDate == QueryDate.Date).ToList();
                        Plant = Serverdb.tblplants.Where(m => m.IsDeleted == 0).ToList();
                        shopList = Serverdb.tblshops.Where(m => m.IsDeleted == 0).ToList();
                        cellList = Serverdb.tblcells.Where(m => m.IsDeleted == 0).ToList();
                    }
                    var plantname = Plant.Where(m => m.PlantID == Machine.PlantID).FirstOrDefault();
                    var shopname = shopList.Where(m => m.ShopID == Machine.ShopID).FirstOrDefault();
                    var celldet = cellList.Where(m => m.CellID == Machine.CellID).FirstOrDefault();
                    foreach (var MacRow in GetUtilList)
                    {
                        worksheet.Cells["A" + StartRow].Value = SlNo++;
                        worksheet.Cells["B" + StartRow].Value = QueryDate.Date.ToString("dd-MM-yyyy");
                        worksheet.Cells["C" + StartRow].Value = plantname.PlantDisplayName;
                        worksheet.Cells["D" + StartRow].Value = shopname.Shopdisplayname;
                        worksheet.Cells["E" + StartRow].Value = celldet.CelldisplayName;
                        worksheet.Cells["F" + StartRow].Value = Machine.MachineDisplayName;
                        worksheet.Cells["G" + StartRow].Value = MacRow.TotalTime;
                        worksheet.Cells["H" + StartRow].Value = MacRow.OperatingTime;
                        worksheet.Cells["I" + StartRow].Value = MacRow.SetupTime;
                        worksheet.Cells["J" + StartRow].Value = (MacRow.MinorLossTime - MacRow.SetupMinorTime);
                        worksheet.Cells["K" + StartRow].Value = MacRow.LossTime;
                        worksheet.Cells["L" + StartRow].Value = MacRow.BDTime;
                        worksheet.Cells["M" + StartRow].Value = MacRow.PowerOffTime;
                        worksheet.Cells["N" + StartRow].Value = MacRow.UtilPercent + " %";
                        StartRow++;
                    }
                }
            }

            int rowCount = 2 + dateDifference;
            //getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

            int oldcolumn = 2;
            int height = 20;
            foreach (var Machine1 in getMachineList)
            {
                int currentCOlumn = oldcolumn;
                string macName = Machine1.MachineDisplayName;
                int StartRow1 = 2;
                for (int i = 0; i <= dateDifference; i++)
                {
                    DateTime QueryDate1 = Convert.ToDateTime(FromDate).AddDays(i);

                    var GetUtilList = new List<tbl_utilreport>();
                    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                    {
                        GetUtilList = Serverdb.tbl_utilreport.Where(m => m.MachineID == Machine1.MachineID && m.CorrectedDate == QueryDate1.Date).ToList();
                    }
                    foreach (var MacRow in GetUtilList)
                    {

                        string ColEntry = ExcelColumnFromNumber(currentCOlumn);
                        workSheetGraphData.Cells[ColEntry + "" + StartRow1].Value = QueryDate1.Date.ToString("dd-MM-yyyy");
                        ColEntry = ExcelColumnFromNumber(currentCOlumn + 1);
                        workSheetGraphData.Cells[ColEntry + "" + StartRow1].Value = Machine1.MachineDisplayName;
                        ColEntry = ExcelColumnFromNumber(currentCOlumn + 2);
                        workSheetGraphData.Cells[ColEntry + "" + StartRow1].Value = MacRow.OperatingTime;
                        StartRow1++;
                    }

                }
                if (StartRow1 > 2)
                {
                    oldcolumn = currentCOlumn + 3;
                    var chartIDAndUnID = (ExcelBarChart)worksheetGraph.Drawings.AddChart("Graph-" + macName, eChartType.ColumnStacked);

                    chartIDAndUnID.SetSize((40 * rowCount), 350);

                    chartIDAndUnID.SetPosition(height, 20);
                    height = height + 400;

                    chartIDAndUnID.Title.Text = "Graph - " + macName;
                    chartIDAndUnID.Style = eChartStyle.Style18;
                    chartIDAndUnID.Legend.Position = eLegendPosition.Bottom;
                    //chartIDAndUnID.Legend.Remove();
                    chartIDAndUnID.YAxis.MaxValue = 24;
                    chartIDAndUnID.YAxis.MinValue = 0;
                    chartIDAndUnID.YAxis.MajorUnit = 4;

                    chartIDAndUnID.Locked = false;
                    chartIDAndUnID.PlotArea.Border.Width = 0;
                    chartIDAndUnID.YAxis.MinorTickMark = eAxisTickMark.None;
                    chartIDAndUnID.DataLabel.ShowValue = true;
                    chartIDAndUnID.DisplayBlanksAs = eDisplayBlanksAs.Gap;
                    string ColEntry1 = ExcelColumnFromNumber(currentCOlumn);
                    ExcelRange dateWork = workSheetGraphData.Cells[ColEntry1 + "2:" + ColEntry1 + rowCount];
                    ColEntry1 = ExcelColumnFromNumber(currentCOlumn + 2);
                    ExcelRange hoursWork = workSheetGraphData.Cells[ColEntry1 + "2:" + ColEntry1 + rowCount];
                    workSheetGraphData.Hidden = eWorkSheetHidden.Hidden;
                    var hours = (ExcelChartSerie)(chartIDAndUnID.Series.Add(hoursWork, dateWork));
                    hours.Header = "Operating Time (Hours)";
                    //Get reference to the worksheet xml for proper namespace
                    var chartXml = chartIDAndUnID.ChartXml;
                    var nsuri = chartXml.DocumentElement.NamespaceURI;
                    var nsm = new XmlNamespaceManager(chartXml.NameTable);
                    nsm.AddNamespace("c", nsuri);

                    //XY Scatter plots have 2 value axis and no category
                    var valAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:valAx", nsm);
                    if (valAxisNodes != null && valAxisNodes.Count > 0)
                        foreach (XmlNode valAxisNode in valAxisNodes)
                        {
                            var major = valAxisNode.SelectSingleNode("c:majorGridlines", nsm);
                            if (major != null)
                                valAxisNode.RemoveChild(major);

                            var minor = valAxisNode.SelectSingleNode("c:minorGridlines", nsm);
                            if (minor != null)
                                valAxisNode.RemoveChild(minor);
                        }

                    //Other charts can have a category axis
                    var catAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:catAx", nsm);
                    if (catAxisNodes != null && catAxisNodes.Count > 0)
                        foreach (XmlNode catAxisNode in catAxisNodes)
                        {
                            var major = catAxisNode.SelectSingleNode("c:majorGridlines", nsm);
                            if (major != null)
                                catAxisNode.RemoveChild(major);

                            var minor = catAxisNode.SelectSingleNode("c:minorGridlines", nsm);
                            if (minor != null)
                                catAxisNode.RemoveChild(minor);
                        }
                }
            }
            p.Workbook.Worksheets.MoveToStart(3);
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "UtilizationReport_Graph" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx");
            DownloadUtilReport(path1, "UtilizationReport_Graph", ToDate);
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {


                ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantID);
                ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName", ShopID);
                ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName", CellID);
                ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName", MachineID);
            }


            return View();
        }

        public void DownloadUtilReport(String FilePath, String FileName, String ToDate)
        {
            System.IO.FileInfo file1 = new System.IO.FileInfo(FilePath);
            string Outgoingfile = FileName + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx";
            if (file1.Exists)
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                Response.AddHeader("Content-Length", file1.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.WriteFile(file1.FullName);
                Response.Flush();
                Response.Close();
                Response.End();
            }
        }

        public ActionResult ManMachineTicket()
        {
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            return View();
        }

        [HttpPost]
        public ActionResult ManMachineTicket(int PlantID, String FromDate, String ToDate, int ShopID = 0, int CellID = 0, int MachineID = 0)
        {
            ReportsCalcClass.ProdDetAndon UR = new ReportsCalcClass.ProdDetAndon();
            var getMachineList = new List<tblmachinedetail>();
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();
            }

            if (MachineID != 0)
            {
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
                }

            }
            else if (CellID != 0)
            {
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
                }

            }
            else if (ShopID != 0)
            {
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
                }

            }

            int dateDifference = Convert.ToDateTime(ToDate).Subtract(Convert.ToDateTime(FromDate)).Days;

            FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\ManMachineTicket.xlsx");

            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
            //ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];

            String FileDir = @"C:\I_FacilityReports\ReportsList\" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ManMachineTicket" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ManMachineTicket" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    //return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;
            //ExcelWorksheet worksheetGraph = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy"), Templatews);
                //worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy") + "1", Templatews);
                //worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
            }
            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            int StartRow = 4;
            int SlNo = 1;

            int Startcolumn = 18;
            String ColNam = ExcelColumnFromNumber(Startcolumn);
            var GetMainLossList = new List<tbllossescode>();
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
            }
            foreach (var LossRow in GetMainLossList)
            {
                ColNam = ExcelColumnFromNumber(Startcolumn);
                worksheet.Cells[ColNam + "3"].Value = LossRow.LossCode;
                Startcolumn++;
            }

            for (int i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                string corrdate = QueryDate.ToString("yyyy-MM-dd");
                foreach (var Machine in getMachineList)
                {
                    UR.insertManMacProd(Machine.MachineID, QueryDate.Date);
                    var GetUtilList = new List<tbl_prodmanmachine>();
                    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                    {
                        GetUtilList = Serverdb.tbl_prodmanmachine.Where(m => m.MachineID == Machine.MachineID && m.CorrectedDate == corrdate).ToList();
                    }

                    foreach (var MacRow in GetUtilList)
                    {
                        int MacStartcolumn = 18;
                        worksheet.Cells["A" + StartRow].Value = SlNo++;
                        worksheet.Cells["B" + StartRow].Value = MacRow.tblmachinedetail.MachineDisplayName;
                        worksheet.Cells["C" + StartRow].Value = MacRow.tblmachinedetail.MachineDisplayName;
                        worksheet.Cells["D" + StartRow].Value = MacRow.tblworkorderentry.OperatorID;
                        worksheet.Cells["E" + StartRow].Value = MacRow.tblworkorderentry.Prod_Order_No;
                        worksheet.Cells["F" + StartRow].Value = MacRow.tblworkorderentry.OperationNo;
                        worksheet.Cells["G" + StartRow].Value = QueryDate.Date.ToString("dd-MM-yyyy");
                        worksheet.Cells["H" + StartRow].Value = MacRow.tblworkorderentry.ShiftID;
                        worksheet.Cells["I" + StartRow].Value = MacRow.tblworkorderentry.WOStart.ToString("hh:mm tt");
                        worksheet.Cells["J" + StartRow].Value = Convert.ToDateTime(MacRow.tblworkorderentry.WOEnd).ToString("hh:mm tt");
                        worksheet.Cells["K" + StartRow].Value = MacRow.tblworkorderentry.Yield_Qty;
                        worksheet.Cells["L" + StartRow].Value = MacRow.tblworkorderentry.ScrapQty;
                        worksheet.Cells["M" + StartRow].Value = MacRow.tblworkorderentry.Total_Qty;
                        worksheet.Cells["N" + StartRow].Value = MacRow.TotalSetup;
                        worksheet.Cells["O" + StartRow].Value = MacRow.TotalOperatingTime;
                        worksheet.Cells["P" + StartRow].Value = 0;
                        worksheet.Cells["Q" + StartRow].Value = MacRow.TotalMinorLoss - MacRow.TotalSetupMinorLoss;
                        //var getWoLossList = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == MacRow.WOID).ToList();

                        foreach (var LossRow in GetMainLossList)
                        {
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {
                                var getWoLossList1 = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == MacRow.WOID && m.LossID == LossRow.LossCodeID).FirstOrDefault();
                                String ColEntry = ExcelColumnFromNumber(MacStartcolumn);
                                if (getWoLossList1 != null)
                                    worksheet.Cells[ColEntry + "" + StartRow].Value = getWoLossList1.LossDuration;
                                else
                                    worksheet.Cells[ColEntry + "" + StartRow].Value = 0;
                                MacStartcolumn++;
                            }

                        }

                        //foreach (var LossRow in getWoLossList)
                        //{
                        //    int LossIndex = GetMainLossList.IndexOf(Serverdb.tbllossescodes.Find(LossRow.LossID));
                        //    String ColEntry = ExcelColumnFromNumber(MacStartcolumn + LossIndex);
                        //    worksheet.Cells[ColEntry + "" + StartRow].Value = LossRow.LossDuration;
                        //}
                        StartRow++;
                    }
                }
            }

            //worksheet.View.ShowGridLines = false;
            //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "ManMachineTicket" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx");
            DownloadUtilReport(path1, "ManMachineTicket", ToDate);
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantID);
                ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName", ShopID);
                ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName", CellID);
                ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName", MachineID);
            }

            return View();
        }

        public static string ExcelColumnFromNumber(int column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            return columnString;
        }

        public ActionResult OEEReport()
        {
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            return View();
        }

        //[HttpPost]
        //public ActionResult OEEReport(int PlantID, String FromDate, String ToDate, int ShopID = 0, int CellID = 0, int MachineID = 0)
        //{
        //    ReportsCalcClass.ProdDetAndon UR = new ReportsCalcClass.ProdDetAndon();

        //    var getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

        //    if (MachineID != 0)
        //    {
        //        getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
        //    }
        //    else if (CellID != 0)
        //    {
        //        getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
        //    }
        //    else if (ShopID != 0)
        //    {
        //        getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
        //    }

        //    int dateDifference = Convert.ToDateTime(ToDate).Subtract(Convert.ToDateTime(FromDate)).Days;

        //    FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\OEE_Report.xlsx");

        //    ExcelPackage templatep = new ExcelPackage(templateFile);
        //    ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
        //    ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];

        //    String FileDir = @"C:\I_ShopFloorReports\ReportsList\" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
        //    bool exists = System.IO.Directory.Exists(FileDir);
        //    if (!exists)
        //        System.IO.Directory.CreateDirectory(FileDir);

        //    FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "OEE_Report" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
        //    if (newFile.Exists)
        //    {
        //        try
        //        {
        //            newFile.Delete();  // ensures we create a new workbook
        //            newFile = new FileInfo(System.IO.Path.Combine(FileDir, "OEE_Report" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx"));
        //        }
        //        catch
        //        {
        //            TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
        //            //return View();
        //        }
        //    }
        //    //Using the File for generation and populating it
        //    ExcelPackage p = null;
        //    p = new ExcelPackage(newFile);
        //    ExcelWorksheet worksheet = null;
        //    ExcelWorksheet worksheetGraph = null;

        //    //Creating the WorkSheet for populating
        //    try
        //    {
        //        worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy"), Templatews);
        //        worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
        //    }
        //    catch { }

        //    if (worksheet == null)
        //    {
        //        worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy") + "1", Templatews);
        //        worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
        //    }
        //    else if (worksheetGraph == null)
        //    {
        //        worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
        //    }
        //    int sheetcount = p.Workbook.Worksheets.Count;
        //    p.Workbook.Worksheets.MoveToStart(sheetcount);
        //    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        //    int StartRow = 2;
        //    int SlNo = 1;
        //    int MachineCount = getMachineList.Count;
        //    int Startcolumn = 12;
        //    String ColNam = ExcelColumnFromNumber(Startcolumn);
        //    var GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
        //    foreach (var LossRow in GetMainLossList)
        //    {
        //        ColNam = ExcelColumnFromNumber(Startcolumn);
        //        worksheet.Cells[ColNam + "1"].Value = LossRow.LossCode;
        //        Startcolumn++;
        //    }

        //    //Tabular sheet Data Population
        //    for (int i = 0; i <= dateDifference; i++)
        //    {
        //        DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
        //        string corrdate = QueryDate.ToString("yyyy-MM-dd");
        //        foreach (var Machine in getMachineList)
        //        {
        //            UR.insertManMacProd(Machine.MachineID, QueryDate.Date);
        //            var GetUtilList = Serverdb.tbl_prodmanmachine.Where(m => m.MachineID == Machine.MachineID && m.CorrectedDate == corrdate).ToList();
        //            foreach (var MacRow in GetUtilList)
        //            {
        //                int MacStartcolumn = 12;
        //                worksheet.Cells["A" + StartRow].Value = MacRow.tblmachinedetail.MachineName;
        //                worksheet.Cells["B" + StartRow].Value = MacRow.tblmachinedetail.MachineName;
        //                worksheet.Cells["C" + StartRow].Value = MacRow.tblworkorderentry.Prod_Order_No;
        //                worksheet.Cells["D" + StartRow].Value = MacRow.tblworkorderentry.FGCode;
        //                worksheet.Cells["E" + StartRow].Value = MacRow.tblworkorderentry.ProdOrderQty;
        //                worksheet.Cells["F" + StartRow].Value = MacRow.tblworkorderentry.OperationNo;
        //                worksheet.Cells["G" + StartRow].Value = QueryDate.Date.ToString("dd-MM-yyyy");
        //                worksheet.Cells["H" + StartRow].Value = MacRow.TotalOperatingTime;
        //                worksheet.Cells["I" + StartRow].Value = MacRow.tblworkorderentry.Yield_Qty;
        //                worksheet.Cells["J" + StartRow].Value = MacRow.tblworkorderentry.ScrapQty;
        //                worksheet.Cells["K" + StartRow].Value = MacRow.TotalSetup;
        //                int TotalQty = MacRow.tblworkorderentry.Yield_Qty + MacRow.tblworkorderentry.ScrapQty;
        //                if (TotalQty == 0)
        //                    TotalQty = 1;
        //                worksheet.Cells["K1"].Value = "Setup Time";
        //                worksheet.Cells["L1"].Value = "Rejections";
        //                worksheet.Cells["L" + StartRow].Value = (MacRow.TotalOperatingTime / TotalQty) * MacRow.tblworkorderentry.ScrapQty;
        //                long MacTotalLoss = 0;
        //                foreach (var LossRow in GetMainLossList)
        //                {
        //                    var getWoLossList1 = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == MacRow.WOID && m.LossID == LossRow.LossCodeID).FirstOrDefault();
        //                    String ColEntry = ExcelColumnFromNumber(MacStartcolumn);
        //                    if (getWoLossList1 != null)
        //                    {
        //                        worksheet.Cells[ColEntry + "" + StartRow].Value = getWoLossList1.LossDuration;
        //                        MacTotalLoss += getWoLossList1.LossDuration;
        //                    }
        //                    else
        //                        worksheet.Cells[ColEntry + "" + StartRow].Value = 0;
        //                    MacStartcolumn++;
        //                }
        //                String ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "No Power";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = MacRow.TotalPowerLoss;
        //                MacStartcolumn++;

        //                ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "Total Part";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = MacRow.tblworkorderentry.Total_Qty;
        //                MacStartcolumn++;

        //                ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "Load / Unload";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = MacRow.TotalMinorLoss - MacRow.TotalSetupMinorLoss;
        //                MacStartcolumn++;

        //                ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "Shift";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = MacRow.tblworkorderentry.ShiftID;
        //                MacStartcolumn++;

        //                ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "Operator ID";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = MacRow.tblworkorderentry.OperatorID;
        //                MacStartcolumn++;

        //                ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "Total OEE Loss";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = MacTotalLoss;
        //                MacStartcolumn++;

        //                decimal OEEPercent = (decimal)Math.Round((double)(MacRow.UtilPercent / 100) * (double)(MacRow.PerformancePerCent / 100) * (double)(MacRow.QualityPercent / 100) * 100, 2);

        //                ColEntry1 = ExcelColumnFromNumber(MacStartcolumn);
        //                worksheet.Cells[ColEntry1 + "1"].Value = "% of OEE";
        //                worksheet.Cells[ColEntry1 + "" + StartRow].Value = OEEPercent;
        //                MacStartcolumn++;
        //                StartRow++;
        //            }
        //        }
        //    }

        //    DataTable LossTbl = new DataTable();
        //    LossTbl.Columns.Add("LossID", typeof(int));
        //    LossTbl.Columns.Add("LossDuration", typeof(int));
        //    LossTbl.Columns.Add("LossTarget", typeof(string));
        //    LossTbl.Columns.Add("LossName", typeof(string));
        //    LossTbl.Columns.Add("LossActual", typeof(string));

        //    //Graph Sheet Population
        //    //Start Date and End Date
        //    worksheetGraph.Cells["C6"].Value = Convert.ToDateTime(FromDate).ToString("dd-MM-yyyy");
        //    worksheetGraph.Cells["E6"].Value = Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy");
        //    int GetHolidays = getsundays(Convert.ToDateTime(ToDate), Convert.ToDateTime(FromDate));
        //    int WorkingDays = dateDifference - GetHolidays + 1;
        //    //Working Days
        //    worksheetGraph.Cells["E5"].Value = WorkingDays;
        //    //Planned Production Time
        //    worksheetGraph.Cells["E10"].Value = WorkingDays * 24 * MachineCount;
        //    double TotalOperatingTime = 0;
        //    double TotalDownTime = 0;
        //    double TotalAcceptedQty = 0;
        //    double TotalRejectedQty = 0;
        //    double TotalPerformanceFactor = 0;
        //    int StartGrpah1 = 48;
        //    for (int i = 0; i <= dateDifference; i++)
        //    {
        //        double DayOperatingTime = 0;
        //        double DayDownTime = 0;
        //        double DayAcceptedQty = 0;
        //        double DayRejectedQty = 0;
        //        double DayPerformanceFactor = 0;
        //        DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
        //        string corrdate = QueryDate.ToString("yyyy-MM-dd");
        //        foreach (var MachRow in getMachineList)
        //        {
        //            if (MachineID == 0)
        //            {
        //                worksheetGraph.Cells["C4"].Value = MachRow.tblcell.CelldisplayName;
        //                worksheetGraph.Cells["C5"].Value = "AS DIVISION";
        //            }
        //            else
        //            {
        //                worksheetGraph.Cells["C4"].Value = MachRow.tblcell.CelldisplayName;
        //                worksheetGraph.Cells["C5"].Value = MachRow.MachineDisplayName;
        //            }
        //            var GetUtilList = Serverdb.tbl_prodmanmachine.Where(m => m.MachineID == MachRow.MachineID && m.CorrectedDate == corrdate).ToList();
        //            foreach (var ProdRow in GetUtilList)
        //            {
        //                //Total Values
        //                TotalOperatingTime += (double)ProdRow.TotalOperatingTime;
        //                TotalDownTime += (double)ProdRow.TotalLoss + (double)ProdRow.TotalMinorLoss + (double)ProdRow.TotalSetup;
        //                TotalAcceptedQty += ProdRow.tblworkorderentry.Yield_Qty;
        //                TotalRejectedQty += ProdRow.tblworkorderentry.ScrapQty;
        //                TotalPerformanceFactor += ProdRow.PerfromaceFactor;
        //                //Day Values
        //                DayOperatingTime += (double)ProdRow.TotalOperatingTime;
        //                DayDownTime += (double)ProdRow.TotalLoss + (double)ProdRow.TotalMinorLoss;
        //                DayAcceptedQty += ProdRow.tblworkorderentry.Yield_Qty;
        //                DayRejectedQty += ProdRow.tblworkorderentry.ScrapQty;
        //                DayPerformanceFactor += ProdRow.PerfromaceFactor;
        //            }
        //            var GetLossList = Serverdb.tbl_prodorderlosses.Where(m => m.MachineID == MachRow.MachineID && m.CorrectedDate == QueryDate.Date).ToList();

        //            foreach (var LossRow in GetLossList)
        //            {
        //                var getrow = (from DataRow row in LossTbl.Rows where row.Field<int>("LossID") == LossRow.LossID select row["LossID"]).FirstOrDefault();
        //                if (getrow == null)
        //                {
        //                    var GetLossTargetPercent = "1%";
        //                    String GetLossName = null;
        //                    var GetLossTarget = Serverdb.tbllossescodes.Where(m => m.LossCodeID == LossRow.LossID).FirstOrDefault();
        //                    if (GetLossTarget != null)
        //                    {
        //                        GetLossTargetPercent = GetLossTarget.TargetPercent.ToString();
        //                        GetLossName = GetLossTarget.LossCode;
        //                    }

        //                    LossTbl.Rows.Add(LossRow.LossID, LossRow.LossDuration, GetLossTargetPercent, GetLossName);
        //                }
        //                else
        //                {
        //                    foreach (DataRow GetRow in LossTbl.Rows)
        //                    {
        //                        if (Convert.ToInt32(GetRow["LossID"]) == LossRow.LossID)
        //                        {
        //                            long LossDura = Convert.ToInt32(GetRow["LossDuration"]);
        //                            LossDura += LossRow.LossDuration;
        //                            GetRow["LossDuration"] = LossDura;
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //        int TotQty = (int)(DayAcceptedQty + DayRejectedQty);
        //        if (TotQty == 0)
        //            TotQty = 1;

        //        double DayOpTime = DayOperatingTime;
        //        if (DayOpTime == 0)
        //            DayOpTime = 1;

        //        decimal DayAvailPercent = (decimal)Math.Round(DayOperatingTime / (24 * MachineCount), 2);
        //        decimal DayPerformancePercent = (decimal)Math.Round(DayPerformanceFactor / DayOpTime, 2);
        //        decimal DayQualityPercent = (decimal)Math.Round((DayAcceptedQty / (TotQty)), 2);
        //        decimal DayOEEPercent = (decimal)Math.Round((double)(DayAvailPercent) * (double)(DayPerformancePercent) * (double)(DayQualityPercent), 2);

        //        worksheetGraph.Cells["B" + StartGrpah1].Value = QueryDate.ToString("dd-MM-yyyy");
        //        worksheetGraph.Cells["C" + StartGrpah1].Value = 0.85;
        //        worksheetGraph.Cells["D" + StartGrpah1].Value = DayOEEPercent;

        //        StartGrpah1++;
        //    }
        //    worksheetGraph.Cells["E11"].Value = (double)Math.Round(TotalOperatingTime / 60, 2);
        //    worksheetGraph.Cells["E12"].Value = (double)Math.Round(TotalDownTime / 60, 2);
        //    worksheetGraph.Cells["E13"].Value = TotalAcceptedQty;
        //    worksheetGraph.Cells["E14"].Value = TotalRejectedQty;


        //    decimal TotalAvailPercent = 0, TotalPerformancePercent = 0, TotalQualityPercent = 0, TotalOEEPercent = 0;
        //    if (TotalOperatingTime != 0)
        //    {
        //        TotalAvailPercent = (decimal)Math.Round(TotalOperatingTime / (WorkingDays * 24 * 60 * MachineCount), 2);
        //        TotalPerformancePercent = (decimal)Math.Round(TotalPerformanceFactor / TotalOperatingTime, 2);
        //        TotalQualityPercent = (decimal)Math.Round((TotalAcceptedQty / (TotalAcceptedQty + TotalRejectedQty)), 2);
        //        TotalOEEPercent = (decimal)Math.Round((double)(TotalAvailPercent) * (double)(TotalPerformancePercent) * (double)(TotalQualityPercent), 2);
        //    }
        //    worksheetGraph.Cells["E20"].Value = TotalAvailPercent;
        //    worksheetGraph.Cells["E21"].Value = TotalPerformancePercent;
        //    worksheetGraph.Cells["E22"].Value = TotalQualityPercent;
        //    worksheetGraph.Cells["E23"].Value = TotalOEEPercent;
        //    worksheetGraph.Cells["G5"].Value = TotalOEEPercent;
        //    worksheetGraph.View.ShowGridLines = false;

        //    DateTime fromDate = Convert.ToDateTime(FromDate);
        //    DateTime toDate = Convert.ToDateTime(ToDate);
        //    var top3ContrubutingFactors = (from dbItem in Serverdb.tbl_prodorderlosses
        //                                   where dbItem.CorrectedDate >= fromDate.Date && dbItem.CorrectedDate <= toDate.Date
        //                                   group dbItem by dbItem.LossID into x
        //                                   select new
        //                                   {
        //                                       LossId = x.Key,
        //                                       LossDuration = Serverdb.tbl_prodorderlosses.Where(m => m.LossID == x.Key).Select(m => m.LossDuration).Sum()
        //                                   }).ToList();
        //    var item = top3ContrubutingFactors.OrderByDescending(m => m.LossDuration).Take(3).ToList();
        //    int lossXccelNo = 29;
        //    foreach (var GetRow in item)
        //    {
        //        string lossCode = Serverdb.tbllossescodes.Where(m => m.LossCodeID == GetRow.LossId).Select(m => m.LossCode).FirstOrDefault();
        //        decimal lossPercentage = (decimal)Math.Round(((GetRow.LossDuration) / TotalDownTime), 2);
        //        decimal lossDurationInHours = (decimal)Math.Round((GetRow.LossDuration / 60.00), 2);
        //        worksheetGraph.Cells["L" + lossXccelNo].Value = lossCode;
        //        worksheetGraph.Cells["N" + lossXccelNo].Value = lossPercentage;
        //        worksheetGraph.Cells["O" + lossXccelNo].Value = lossDurationInHours;
        //        lossXccelNo++;
        //    }

        //    int grphData = 5;
        //    decimal CumulativePercentage = 0;
        //    foreach (var data in top3ContrubutingFactors)
        //    {
        //        var dbLoss = Serverdb.tbllossescodes.Where(m => m.LossCodeID == data.LossId).FirstOrDefault();
        //        string lossCode = dbLoss.LossCode;
        //        decimal Target = dbLoss.TargetPercent;
        //        decimal actualPercentage = (decimal)Math.Round(((data.LossDuration) / TotalDownTime), 2);
        //        CumulativePercentage = CumulativePercentage + actualPercentage;
        //        worksheetGraph.Cells["K" + grphData].Value = lossCode;
        //        worksheetGraph.Cells["L" + grphData].Value = Target;
        //        worksheetGraph.Cells["M" + grphData].Value = actualPercentage;
        //        worksheetGraph.Cells["N" + grphData].Value = CumulativePercentage;
        //        grphData++;

        //    }

        //    //var chartIDAndUnID = (ExcelBarChart)worksheetGraph.Drawings.AddChart("Testing", eChartType.ColumnClustered);

        //    //chartIDAndUnID.SetSize((350), 550);

        //    //chartIDAndUnID.SetPosition(50, 60);

        //    //chartIDAndUnID.Title.Text = "AB Graph ";
        //    //chartIDAndUnID.Style = eChartStyle.Style18;
        //    //chartIDAndUnID.Legend.Position = eLegendPosition.Bottom;
        //    ////chartIDAndUnID.Legend.Remove();
        //    //chartIDAndUnID.YAxis.MaxValue = 100;
        //    //chartIDAndUnID.YAxis.MinValue = 0;
        //    //chartIDAndUnID.YAxis.MajorUnit = 5;

        //    //chartIDAndUnID.Locked = false;
        //    //chartIDAndUnID.PlotArea.Border.Width = 0;
        //    //chartIDAndUnID.YAxis.MinorTickMark = eAxisTickMark.None;
        //    //chartIDAndUnID.DataLabel.ShowValue = true;
        //    //chartIDAndUnID.DisplayBlanksAs = eDisplayBlanksAs.Gap;


        //    //ExcelRange dateWork = worksheetGraph.Cells["K33:" + lossXccelNo];
        //    //ExcelRange hoursWork = worksheetGraph.Cells["N33:" + lossXccelNo];
        //    //var hours = (ExcelChartSerie)(chartIDAndUnID.Series.Add(hoursWork, dateWork));
        //    //hours.Header = "Operating Time (Hours)";
        //    ////Get reference to the worksheet xml for proper namespace
        //    //var chartXml = chartIDAndUnID.ChartXml;
        //    //var nsuri = chartXml.DocumentElement.NamespaceURI;
        //    //var nsm = new XmlNamespaceManager(chartXml.NameTable);
        //    //nsm.AddNamespace("c", nsuri);

        //    ////XY Scatter plots have 2 value axis and no category
        //    //var valAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:valAx", nsm);
        //    //if (valAxisNodes != null && valAxisNodes.Count > 0)
        //    //    foreach (XmlNode valAxisNode in valAxisNodes)
        //    //    {
        //    //        var major = valAxisNode.SelectSingleNode("c:majorGridlines", nsm);
        //    //        if (major != null)
        //    //            valAxisNode.RemoveChild(major);

        //    //        var minor = valAxisNode.SelectSingleNode("c:minorGridlines", nsm);
        //    //        if (minor != null)
        //    //            valAxisNode.RemoveChild(minor);
        //    //    }

        //    ////Other charts can have a category axis
        //    //var catAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:catAx", nsm);
        //    //if (catAxisNodes != null && catAxisNodes.Count > 0)
        //    //    foreach (XmlNode catAxisNode in catAxisNodes)
        //    //    {
        //    //        var major = catAxisNode.SelectSingleNode("c:majorGridlines", nsm);
        //    //        if (major != null)
        //    //            catAxisNode.RemoveChild(major);

        //    //        var minor = catAxisNode.SelectSingleNode("c:minorGridlines", nsm);
        //    //        if (minor != null)
        //    //            catAxisNode.RemoveChild(minor);
        //    //    }
        //    //worksheetGraph.View["L29"]
        //    //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //    p.Save();

        //    //Downloding Excel
        //    string path1 = System.IO.Path.Combine(FileDir, "OEE_Report" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx");
        //    DownloadUtilReport(path1, "OEE_Report", ToDate);

        //    ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantID);
        //    ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID), "ShopID", "ShopName", ShopID);
        //    ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID && m.ShopID == ShopID), "CellID", "CellName", CellID);
        //    ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID && m.ShopID == ShopID && m.CellID == CellID), "MachineID", "MachineDisplayName", MachineID);
        //    return View();
        //}



        [HttpPost]
        public ActionResult OEEReport(int PlantID, String FromDate, String ToDate, int ShopID = 0, int CellID = 0)
        {
            ReportsCalcClass.ProdDetAndon UR = new ReportsCalcClass.ProdDetAndon();
            var getCellList = new List<tblcell>();
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                getCellList = Serverdb.tblcells.Where(m => m.IsDeleted == 0).ToList();

            }
            // if (CellID != 0)
            //{
            //    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            //    {
            //        getCellList = Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            //    }

            //}
            //else if (ShopID != 0)
            //{
            //    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            //    {
            //        getCellList = Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
            //    }

            //}

            int cellcount = getCellList.Count();
            int dateDifference = Convert.ToDateTime(ToDate).Subtract(Convert.ToDateTime(FromDate)).Days;

            FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\OEE_Report.xlsx");

            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
            ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];

            String FileDir = @"C:\I_ShopFloorReports\ReportsList\" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "OEE_Report" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "OEE_Report" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    //return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;
            ExcelWorksheet worksheetGraph = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy"), Templatews);
                worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy") + "1", Templatews);
                worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
            }
            else if (worksheetGraph == null)
            {
                worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "Graph", TemplateGraph);
            }
            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            int StartRow = 3;
            int SlNo = 1;
            int i = 0;
            //int MachineCount = getCellList.Count;
            int Startcolumn = 12;
            String ColNam = ExcelColumnFromNumber(Startcolumn);
            //var GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
            //foreach (var LossRow in GetMainLossList)
            //{
            //    ColNam = ExcelColumnFromNumber(Startcolumn);
            //    worksheet.Cells[ColNam + "1"].Value = LossRow.LossCode;
            //    Startcolumn++;
            //}
            var plantnameList = new List<tblplant>();
            var cellname = new tblcell();
            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
            {
                plantnameList = Serverdb.tblplants.Where(m => m.IsDeleted == 0).ToList();
            }
            //Tabular sheet Data Population
            for (i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                string corrdate = QueryDate.ToString("yyyy-MM-dd");

                //UR.insertOEE(CellID, QueryDate.Date);
                //foreach (var cell in getCellList)
                //{
                //    CellID = cell.CellID;
                OEE(CellID, QueryDate.Date);
                //}
                var GetUtilList = new List<tbloeedetail>();
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    //if (getCellList.Count == 1)
                    GetUtilList = Serverdb.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == corrdate).ToList();
                    //else if (ShopID != 0 && getCellList.Count > 1)
                    //{                      
                    //        GetUtilList = Serverdb.tbloeedetails.Where(m => m.tblcell.tblshop.ShopID==ShopID && m.CorrectedDate == corrdate).ToList();                      
                    //}
                }
                foreach (var MacRow in GetUtilList)
                {
                    int MacStartcolumn = 12;
                    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                    {
                        cellname = Serverdb.tblcells.Where(m => m.CellID == CellID).FirstOrDefault();
                    }
                    var plantname = plantnameList.Where(m => m.PlantID == cellname.PlantID).FirstOrDefault();
                    worksheet.Cells["A" + StartRow].Value = cellname.CelldisplayName;
                    worksheet.Cells["B" + StartRow].Value = QueryDate.Date.ToString("dd-MM-yyyy");
                    worksheet.Cells["C" + StartRow].Value = MacRow.TotalQuantity;
                    worksheet.Cells["D" + StartRow].Value = MacRow.scrapqty;
                    worksheet.Cells["E" + StartRow].Value = Math.Round(MacRow.OperatingTime, 2);
                    worksheet.Cells["F" + StartRow].Value = Math.Round(MacRow.LossTime, 2);
                    worksheet.Cells["G" + StartRow].Value = Math.Round(MacRow.PlannedBrkDurationinMin);
                    worksheet.Cells["H" + StartRow].Value = Math.Round(MacRow.PowerOffTime, 2);
                    worksheet.Cells["I" + StartRow].Value = Math.Round(MacRow.PlannedBrkDurationinMin, 2);
                    worksheet.Cells["J" + StartRow].Value = Math.Round(MacRow.MinorLossTime, 2);
                    worksheet.Cells["K" + StartRow].Value = Math.Round(MacRow.LoadingUnLoadingWithProd, 2);
                    worksheet.Cells["L" + StartRow].Value = Math.Round(MacRow.TotalTime, 2);
                    worksheet.Cells["M" + StartRow].Value = MacRow.Availability;


                    worksheet.Cells["N" + StartRow].Value = MacRow.performance;
                    worksheet.Cells["O" + StartRow].Value = MacRow.Quality;
                    worksheet.Cells["P" + StartRow].Value = MacRow.OEEPercent;

                    //worksheet.Cells["A" + StartRow].Value = cellname;
                    //worksheet.Cells["B" + StartRow].Value = MacRow.OperatingTime;
                    //worksheet.Cells["C" + StartRow].Value = MacRow.TotalTime;
                    //worksheet.Cells["D" + StartRow].Value = MacRow.BDTime;
                    //worksheet.Cells["E" + StartRow].Value = MacRow.Availability;
                    //worksheet.Cells["j" + StartRow].Value = QueryDate.Date.ToString("dd-MM-yyyy");
                    //worksheet.Cells["G" + StartRow].Value = MacRow.performance;
                    //worksheet.Cells["I" + StartRow].Value = MacRow.LossTime;
                    //worksheet.Cells["F" + StartRow].Value = MacRow.Quality;
                    //worksheet.Cells["K" + StartRow].Value = MacRow.TotalQuantity;
                    //worksheet.Cells["H" + StartRow].Value = MacRow.scrapqty;
                    //worksheet.Cells["L" + StartRow].Value = MacRow.OEEPercent;
                    StartRow++;
                }
            }

            DataTable LossTbl = new DataTable();
            LossTbl.Columns.Add("LossID", typeof(int));
            LossTbl.Columns.Add("LossDuration", typeof(int));
            LossTbl.Columns.Add("LossTarget", typeof(string));
            LossTbl.Columns.Add("LossName", typeof(string));
            LossTbl.Columns.Add("LossActual", typeof(string));

            //Graph Sheet Population
            //Start Date and End Date
            worksheetGraph.Cells["C6"].Value = Convert.ToDateTime(FromDate).ToString("dd-MM-yyyy");
            worksheetGraph.Cells["E6"].Value = Convert.ToDateTime(ToDate).ToString("dd-MM-yyyy");
            int GetHolidays = getsundays(Convert.ToDateTime(ToDate), Convert.ToDateTime(FromDate));
            int WorkingDays = dateDifference - GetHolidays + 1;
            //Working Days
            worksheetGraph.Cells["E5"].Value = WorkingDays;
            //Planned Production Time
            worksheetGraph.Cells["E10"].Value = WorkingDays * 24;
            double TotalOperatingTime = 0;
            double TotalDownTime = 0;
            double TotalAcceptedQty = 0;
            double TotalRejectedQty = 0;
            double TotalPerformanceFactor = 0;
            double totalProduction = 0;
            double TotalTime = 0;
            double TotalOpWithMinorStoppage = 0;
            double totalAvailability = 0;
            double TotalPlannedCycleTime = 0;
            double AvgTotalPerformance = 0;
            double AVgTotalAvailability = 0;
            double AvgTotalQuality = 0;


            int StartGrpah1 = 48;
            for (i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                string corrdate = QueryDate.ToString("yyyy-MM-dd");
                double DayOperatingTime = 0;
                double DayDownTime = 0;
                double DayAcceptedQty = 0;
                double DayRejectedQty = 0;
                double DayPerformanceFactor = 0;
                double DayTotalTime = 0;
                double DayOee = 0;
                double DayUtilFactor = 0;
                double DayTotalProduction = 0;
                double DayOpWithMinorStoppage = 0;
                double AvailabilityDay = 0;
                double dayplannedcycletime = 0;
                double plannedCycleTimeInMin = 0;


                //foreach (var MachRow in getMachineList)
                //{
                if (CellID == 0)
                {

                    worksheetGraph.Cells["C4"].Value = cellname.CelldisplayName;
                    worksheetGraph.Cells["C5"].Value = "AS DIVISION";
                }
                else
                {

                    worksheetGraph.Cells["C4"].Value = cellname.CelldisplayName;
                    //worksheetGraph.Cells["C5"].Value = MachRow.MachineDisplayName;
                }
                var GetList = new List<tbloeedetail>();
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    GetList = Serverdb.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == corrdate).ToList();
                }
                foreach (var ProdRow in GetList)
                {
                    //Total Values
                    TotalOperatingTime += (double)ProdRow.OperatingTime;
                    TotalDownTime += (double)ProdRow.LossTime;
                    TotalAcceptedQty += ProdRow.TotalQuantity;
                    TotalRejectedQty += (double)ProdRow.scrapqty;
                    TotalPerformanceFactor += (double)ProdRow.PerformanceFactor;
                    TotalTime += (double)ProdRow.TotalTime;
                    totalProduction += (double)ProdRow.TotalProduction;
                    TotalOpWithMinorStoppage += (double)ProdRow.OPwithMinorStoppage;
                    totalAvailability += (double)ProdRow.AvailabilityInMin;
                    TotalPlannedCycleTime = (double)ProdRow.PlannedCycleTimeInMin;
                    //Day Values

                    dayplannedcycletime = (double)ProdRow.PlannedCycleTimeInMin;
                    DayOpWithMinorStoppage = (double)ProdRow.OPwithMinorStoppage;
                    AvailabilityDay = (double)ProdRow.AvailabilityInMin;
                    DayOperatingTime += (double)ProdRow.OperatingTime;
                    DayDownTime += (double)ProdRow.LossTime;
                    DayAcceptedQty = ProdRow.TotalQuantity;
                    DayRejectedQty = (double)ProdRow.scrapqty;
                    DayPerformanceFactor = (double)ProdRow.PerformanceFactor; // UtilFactor
                    DayTotalTime += (double)ProdRow.TotalTime;
                    DayOee = (double)ProdRow.OEEPercent;
                    DayTotalProduction = (double)ProdRow.TotalProduction;
                    //  DayUtilFactor=(dub)
                }
                //    var GetLossList = Serverdb.tbl_prodorderlosses.Where(m => m.MachineID == MachRow.MachineID && m.CorrectedDate == QueryDate.Date).ToList();

                //    foreach (var LossRow in GetLossList)
                //    {
                //        var getrow = (from DataRow row in LossTbl.Rows where row.Field<int>("LossID") == LossRow.LossID select row["LossID"]).FirstOrDefault();
                //        if (getrow == null)
                //        {
                //            var GetLossTargetPercent = "1%";
                //            String GetLossName = null;
                //            var GetLossTarget = Serverdb.tbllossescodes.Where(m => m.LossCodeID == LossRow.LossID).FirstOrDefault();
                //            if (GetLossTarget != null)
                //            {
                //                GetLossTargetPercent = GetLossTarget.TargetPercent.ToString();
                //                GetLossName = GetLossTarget.LossCode;
                //            }

                //            LossTbl.Rows.Add(LossRow.LossID, LossRow.LossDuration, GetLossTargetPercent, GetLossName);
                //        }
                //        else
                //        {
                //            foreach (DataRow GetRow in LossTbl.Rows)
                //            {
                //                if (Convert.ToInt32(GetRow["LossID"]) == LossRow.LossID)
                //                {
                //                    long LossDura = Convert.ToInt32(GetRow["LossDuration"]);
                //                    LossDura += LossRow.LossDuration;
                //                    GetRow["LossDuration"] = LossDura;
                //                }
                //            }

                //        }
                //    }
                //}

                //var macid = Serverdb.tblmachinedetails.Where(m => m.CellID == CellID && m.IsDeleted == 0).Select(m=>m.MachineID).ToList();

                //foreach(var items in macid)
                //{
                //    var scrap = Serverdb.tblworkorderentries.Where(m => m.MachineID == items).FirstOrDefault();
                //    {
                //        var scrapqty1 = Serverdb.tblrejectqties.Where(m => m.WOID == scrap.HMIID && m.CorrectedDate == corrdate).Select(m=>m.RejectQty).ToList();
                //        int reject = 0;
                //        foreach(int r1 in scrapqty1)
                //        {
                //            reject = reject + r1;
                //        }

                //    }
                //}
                int TotQty = (int)(DayAcceptedQty + DayRejectedQty);
                if (TotQty == 0)
                    TotQty = 1;

                double DayOpTime = DayOperatingTime;
                if (DayOpTime == 0)
                    DayOpTime = 1;

                //decimal DayAvailPercent = (decimal)Math.Round(DayOperatingTime / (24 * MachineCount), 2);
                decimal DayAvailPercent = (decimal)Math.Round(DayOpWithMinorStoppage / AvailabilityDay, 2);
                decimal DayPerformancePercent = (decimal)Math.Round((dayplannedcycletime * DayAcceptedQty) / DayOpWithMinorStoppage, 2);
                decimal DayQualityPercent = (decimal)Math.Round((DayAcceptedQty / (TotQty)), 2);
                decimal DayOEEPercent = (decimal)Math.Round((double)(DayAvailPercent) * (double)(DayPerformancePercent) * (double)(DayQualityPercent), 2);

                AVgTotalAvailability += (double)DayAvailPercent;
                AvgTotalPerformance += (double)DayPerformancePercent;
                AvgTotalQuality += (double)DayQualityPercent;

                worksheetGraph.Cells["B" + StartGrpah1].Value = QueryDate.ToString("dd-MM-yyyy");
                worksheetGraph.Cells["C" + StartGrpah1].Value = 0.85;
                worksheetGraph.Cells["D" + StartGrpah1].Value = DayOEEPercent;

                StartGrpah1++;
            }
            // }
            worksheetGraph.Cells["E11"].Value = (double)Math.Round(TotalOperatingTime / 60, 2);
            worksheetGraph.Cells["E12"].Value = (double)Math.Round(TotalDownTime / 60, 2);
            worksheetGraph.Cells["E13"].Value = TotalAcceptedQty;
            worksheetGraph.Cells["E14"].Value = TotalRejectedQty;


            decimal TotalAvailPercent = 0, TotalPerformancePercent = 0, TotalQualityPercent = 0, TotalOEEPercent = 0;
            if (TotalOperatingTime != 0)
            {
                //TotalAvailPercent = (decimal)Math.Round(TotalOperatingTime / (WorkingDays * 24 * 60 * MachineCount), 2);

                //Previous Calculations
                TotalAvailPercent = (decimal)Math.Round(TotalOpWithMinorStoppage / totalAvailability, 2);
                TotalPerformancePercent = (decimal)Math.Round((TotalPlannedCycleTime * TotalAcceptedQty) / TotalOpWithMinorStoppage, 2);
                TotalQualityPercent = (decimal)Math.Round((TotalAcceptedQty / (TotalAcceptedQty + TotalRejectedQty)), 2);
                TotalOEEPercent = (decimal)((double)(TotalAvailPercent) * (double)(TotalPerformancePercent) * (double)(TotalQualityPercent));

                int numofdays = dateDifference + 1;
                //New Calculations
                //TotalAvailPercent = (decimal)Math.Round(AVgTotalAvailability / numofdays, 2);
                //TotalPerformancePercent = (decimal)Math.Round(AvgTotalPerformance / numofdays, 2);
                //TotalQualityPercent = (decimal)Math.Round((AvgTotalQuality/ numofdays), 2);
                //TotalOEEPercent = (decimal)((double)(TotalAvailPercent) * (double)(TotalPerformancePercent) * (double)(TotalQualityPercent));
            }
            worksheetGraph.Cells["E20"].Value = TotalAvailPercent;
            worksheetGraph.Cells["E21"].Value = TotalPerformancePercent;
            worksheetGraph.Cells["E22"].Value = TotalQualityPercent;
            worksheetGraph.Cells["E23"].Value = TotalOEEPercent;
            worksheetGraph.Cells["G5"].Value = TotalOEEPercent;
            worksheetGraph.View.ShowGridLines = false;

            DateTime fromDate = Convert.ToDateTime(FromDate);
            DateTime toDate = Convert.ToDateTime(ToDate);
            var top3ContrubutingFactors = (from dbItem in Serverdb.tbl_prodorderlosses
                                           where dbItem.CorrectedDate >= fromDate.Date && dbItem.CorrectedDate <= toDate.Date
                                           group dbItem by dbItem.LossID into x
                                           select new
                                           {
                                               LossId = x.Key,
                                               LossDuration = Serverdb.tbl_prodorderlosses.Where(m => m.LossID == x.Key).Select(m => m.LossDuration).Sum()
                                           }).ToList();
            var item = top3ContrubutingFactors.OrderByDescending(m => m.LossDuration).Take(3).ToList();
            int lossXccelNo = 29;
            foreach (var GetRow in item)
            {
                string lossCode = Serverdb.tbllossescodes.Where(m => m.LossCodeID == GetRow.LossId).Select(m => m.LossCode).FirstOrDefault();
                decimal lossPercentage = (decimal)Math.Round(((GetRow.LossDuration) / TotalDownTime), 2);
                decimal lossDurationInHours = (decimal)Math.Round((GetRow.LossDuration / 60.00), 2);
                worksheetGraph.Cells["L" + lossXccelNo].Value = lossCode;
                worksheetGraph.Cells["N" + lossXccelNo].Value = lossPercentage;
                worksheetGraph.Cells["O" + lossXccelNo].Value = lossDurationInHours;
                lossXccelNo++;
            }

            int grphData = 5;
            decimal CumulativePercentage = 0;
            foreach (var data in top3ContrubutingFactors)
            {
                var dbLoss = Serverdb.tbllossescodes.Where(m => m.LossCodeID == data.LossId).FirstOrDefault();
                string lossCode = dbLoss.LossCode;
                decimal Target = dbLoss.TargetPercent;
                decimal actualPercentage = (decimal)Math.Round(((data.LossDuration) / TotalDownTime), 2);
                CumulativePercentage = CumulativePercentage + actualPercentage;
                worksheetGraph.Cells["K" + grphData].Value = lossCode;
                worksheetGraph.Cells["L" + grphData].Value = Target;
                worksheetGraph.Cells["M" + grphData].Value = actualPercentage;
                worksheetGraph.Cells["N" + grphData].Value = CumulativePercentage;
                grphData++;

            }
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "OEE_Report" + Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd") + ".xlsx");
            DownloadUtilReport(path1, "OEE_Report", ToDate);

            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantID);
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID), "ShopID", "ShopName", ShopID);
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID && m.ShopID == ShopID), "CellID", "CellName", CellID);
            //ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == PlantID && m.ShopID == ShopID && m.CellID == CellID), "MachineID", "MachineDisplayName", MachineID);



            //  }
            return View();
        }

        public void OEE(int CellID, DateTime QueryDate)
        {
            string correctdate = QueryDate.ToString("yyyy-MM-dd");
            DateTime correctedDate = Convert.ToDateTime(correctdate);
            decimal OperatingTime = 0;
            decimal LossTime = 0;
            decimal MinorLossTime = 0;
            decimal MntTime = 0;
            decimal SetupTime = 0;
            double AvailabilityPercentage = 0;
            double PerformancePercentage = 0;
            double QualityPercentage = 0;
            double OEEPercentage = 0;
            //decimal SetupMinorTime = 0;
            decimal PowerOffTime = 0;
            decimal PowerONTime = 0;
            //decimal Utilization = 0;
            decimal DayOEEPercent = 0;
            //int PerformanceFactor = 0;
            //decimal Quality = 0;
            int TotlaQty = 0;
            int YieldQty = 0;
            int BottleNeckYieldQty = 0;
            //decimal IdealCycleTimeVal = 2;
            decimal plannedCycleTime = 0;
            decimal LoadingTime = 0;
            decimal UnloadingTime = 0;

            double plannedBrkDurationinMin = 0;
            decimal LoadingUnloadingWithProd = 0;
            decimal LoadingUnloadingwithProdBottleNeck = 0;
            int minorstoppage = 0;
            decimal TotalProductoin = 0;
            decimal Availability;
            int rejQty = 0;
            int reject = 0;
            //  string plantName = row.tblplant.PlantName;
            var machineslist = new List<tblmachinedetail>();
            var bottleneckmachines = new tblbottelneck();
            var scrap = new tblworkorderentry();
            var scrapqty1 = new List<tblrejectqty>();
            var cellpartDet = new tblcellpart();
            var partsDet = new tblpart();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                scrap = db.tblworkorderentries.Where(m => m.CellID == CellID && m.CorrectedDate == correctdate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
                if (scrap != null)
                {
                    partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == scrap.PartNo).FirstOrDefault();
                    if (partsDet != null)
                        bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == partsDet.FGCode && m.CellID == scrap.CellID).FirstOrDefault();
                }
                else
                {
                    cellpartDet = db.tblcellparts.Where(m => m.CellID == CellID && m.IsDefault == 1 && m.IsDeleted == 0).FirstOrDefault();
                    if (cellpartDet != null)
                        bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == cellpartDet.partNo && m.CellID == cellpartDet.CellID).FirstOrDefault();
                    string Operationnum = bottleneckmachines.tblmachinedetail.OperationNumber.ToString();
                    partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

                }


            }
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                // Get Machines               
                machineslist = db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.MachineID == bottleneckmachines.MachineID).OrderBy(m => m.MachineID).ToList();
            }
            foreach (var machine in machineslist)
            {
                Machines machines = new Machines();
                int machineID = machine.MachineID;
                // Mode details
                minorstoppage = Convert.ToInt32(machine.MachineIdleMin) * 60; // in sec
                var GetModeDurations = new List<tblmode>();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    GetModeDurations = db.tblmodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 1).ToList();
                }
                OperatingTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "PROD").ToList().Sum(m => m.DurationInSec));
                PowerOffTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWEROFF").ToList().Sum(m => m.DurationInSec));
                MntTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "MNT").ToList().Sum(m => m.DurationInSec));
                MinorLossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec < minorstoppage).ToList().Sum(m => m.DurationInSec));
                LossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec > minorstoppage).ToList().Sum(m => m.DurationInSec));
                PowerONTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWERON").ToList().Sum(m => m.DurationInSec));
                OperatingTime = Math.Round((OperatingTime / 60), 2);
                PowerOffTime = (PowerOffTime / 60);
                MntTime = (MntTime / 60);
                MinorLossTime = (MinorLossTime / 60);
                LossTime = (LossTime / 60);
                PowerONTime = (PowerONTime / 60);
                var plannedbrks = Serverdb.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
                foreach (var row in plannedbrks)
                {
                    plannedBrkDurationinMin += Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalMinutes;
                }
                foreach (var ModeRow in GetModeDurations)
                {
                    if (ModeRow.ModeType == "SETUP")
                    {
                        try
                        {
                            SetupTime += (decimal)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
                            //SetupMinorTime += (decimal)(db.tblSetupMaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60.00);
                        }
                        catch { }
                    }
                }
                //var GetModeDurationsRunning = new List<tbllivemode>();
                //using (i_facilityEntities1 db = new i_facilityEntities1())
                //{
                //    GetModeDurationsRunning = db.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 0).ToList();
                //}
                //foreach (var ModeRow in GetModeDurationsRunning)
                //{
                //    String ColorCode = ModeRow.ColorCode;
                //    DateTime StartTime = (DateTime)ModeRow.StartTime;
                //    decimal Duration = (decimal)System.DateTime.Now.Subtract(StartTime).TotalMinutes;
                //    if (ColorCode == "YELLOW")
                //    {
                //        LossTime += Duration;
                //    }
                //    else if (ColorCode == "GREEN")
                //    {
                //        OperatingTime += Duration;
                //    }
                //    else if (ColorCode == "RED")
                //    {
                //        MntTime += Duration;
                //    }
                //    else if (ColorCode == "BLUE")
                //    {
                //        PowerOffTime += Duration;
                //    }
                //}
                LoadingTime += (decimal)partsDet.StdLoadingTime;
                UnloadingTime += (decimal)partsDet.StdUnLoadingTime;

                //using (i_facilityEntities1 db = new i_facilityEntities1())
                //{
                //    scrap = db.tblworkorderentries.Where(m => m.MachineID == machine.MachineID && m.tblmachinedetail.IsLastMachine == 1).FirstOrDefault();
                //    string operationnum =Convert.ToString( machine.OperationNumber);
                //    partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == bottleneckmachines.PartNo && m.OperationNo == operationnum).FirstOrDefault();
                //}
                if (scrap != null)
                {
                    using (i_facilityEntities1 db = new i_facilityEntities1())
                    {
                        scrapqty1 = db.tblrejectqties.Where(m => m.WOID == scrap.HMIID && m.CorrectedDate == correctdate).ToList();
                    }

                    foreach (var r1 in scrapqty1)
                    {
                        reject = reject + Convert.ToInt32(r1.RejectQty);
                    }

                }
                plannedCycleTime = Convert.ToDecimal(partsDet.IdealCycleTime);
            }
            int bottleneckMachineID = bottleneckmachines.MachineID;
            TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty, bottleneckMachineID);
            //Actual = YieldQty;
            if (YieldQty == 0)
                YieldQty = 1;
            LoadingUnloadingWithProd = ((LoadingTime + UnloadingTime) * YieldQty) / 60;
            LoadingUnloadingwithProdBottleNeck = ((LoadingTime + UnloadingTime) * BottleNeckYieldQty) / 60;
            MinorLossTime = MinorLossTime - LoadingUnloadingWithProd;
            decimal OPwithMinorStoppage = (OperatingTime + LoadingUnloadingWithProd + MinorLossTime);
            decimal utilFactor = Math.Round((LoadingUnloadingWithProd + OperatingTime), 2);
            decimal IdleTime = LossTime;
            decimal BDTime = MntTime;
            // int TotalTime = Convert.ToInt32(PowerONTime) + Convert.ToInt32(OperatingTime) + Convert.ToInt32(IdleTime) + Convert.ToInt32(BDTime) + Convert.ToInt32(PowerOffTime);
            int TotalTime = 24 * 60;

            if (TotalTime == 0)
            {
                TotalTime = 1;
            }
            if (TotlaQty == 0)
                TotlaQty = 1;
            decimal plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
            var StdCycleTimeinMin = Convert.ToDecimal(plannedCycleTimeInMin);
            var LoadunloadTimeinMin = ((int)LoadingTime + (int)UnloadingTime) / 60;
            if (StdCycleTimeinMin < 1)
                StdCycleTimeinMin = 1;
            var Targetdec = ((decimal)TotalTime / (StdCycleTimeinMin + LoadunloadTimeinMin));
            // Target = Convert.ToInt32(Targetdec);
            if (TotalTime > 360)
                Availability = Math.Round((TotalTime - (decimal)plannedBrkDurationinMin), 2);
            else
                Availability = TotalTime;
            if (OPwithMinorStoppage == 0)
                OPwithMinorStoppage = 1;
            decimal TotalTimeWithPlannedBrk = Availability;
            decimal AvailabilityPercent = Math.Round((OPwithMinorStoppage / TotalTimeWithPlannedBrk), 2) * 100;  // From BottleNeckMachine
            if (AvailabilityPercent > 100)
                AvailabilityPercent = 100;
            decimal PerformanceBottelNeck = Math.Round(((plannedCycleTimeInMin * YieldQty) / OPwithMinorStoppage), 2) * 100;
            decimal performanceFactor = (plannedCycleTimeInMin * YieldQty);
            decimal QualityLastMachine = Math.Round((decimal)((YieldQty - reject) / YieldQty), 2) * 100;            // From LastMachine
            DayOEEPercent = (decimal)Math.Round((double)(AvailabilityPercent / 100) * (double)(PerformanceBottelNeck / 100) * (double)(QualityLastMachine / 100), 2) * 100;
            //decimal availabilityDenominator = Math.Round((plannedCycleTimeInMin + LoadingUnloadingWithProd), 2);

            //TotalProductoin = Math.Round((Availability / availabilityDenominator) * 100, 2);
            //decimal performance = Math.Round((utilFactor / TotalProductoin) * 100, 2);
            //decimal performanceFactor = Math.Round((utilFactor));

            //decimal quality = Math.Round((decimal)(YieldQty / (YieldQty + rejQty)) * 100, 2);

            //Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(utilFactor) / Convert.ToDouble(TotalTime)) * 100));

            //DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(performance / 100) * (double)(quality / 100), 2) * 100;
            if (AvailabilityPercent == 0)
            {
                QualityLastMachine = 0;
                PerformanceBottelNeck = 0;
                DayOEEPercent = 0;
            }
            AvailabilityPercentage = (double)AvailabilityPercent;
            QualityPercentage = (double)QualityLastMachine;
            PerformancePercentage = (double)PerformanceBottelNeck;
            OEEPercentage = (double)DayOEEPercent;

            DateTime curr = DateTime.Now;
            string current = curr.ToString("yyyy-MM-dd");
            var doesthisexists = new List<tbloeedetail>();
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                doesthisexists = db.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == correctdate).ToList();
            }
            if (doesthisexists.Count == 0)
            {

                tbloeedetail obj = new tbloeedetail();
                obj.Availability = (decimal)AvailabilityPercentage;
                obj.BDTime = BDTime;
                obj.CellId = CellID;
                obj.CorrectedDate = correctdate;
                obj.LossTime = IdleTime;
                obj.OEEPercent = (decimal)OEEPercentage;
                obj.OperatingTime = OperatingTime;
                obj.performance = (decimal)PerformancePercentage;
                obj.PerformanceFactor = performanceFactor;
                obj.Quality = (decimal)QualityPercentage;
                obj.TotalTime = TotalTime;
                obj.TotalQuantity = YieldQty;
                obj.scrapqty = reject;
                obj.TotalProduction = TotalProductoin;
                obj.AvailabilityInMin = Availability;
                obj.PowerOffTime = PowerOffTime;
                obj.OPwithMinorStoppage = OPwithMinorStoppage;
                obj.MinorLossTime = MinorLossTime;
                obj.PlannedCycleTimeInMin = plannedCycleTimeInMin;
                obj.PlannedBrkDurationinMin = (decimal)plannedBrkDurationinMin;
                obj.LastmachineYieldQty = YieldQty;
                obj.LoadingUnLoadingWithProd = LoadingUnloadingWithProd;
                obj.BottleNeckMachineYieldQtry = BottleNeckYieldQty;
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    db.tbloeedetails.Add(obj);
                    db.SaveChanges();
                }

            }
            else if (current == correctdate)
            {
                var Data = new tbloeedetail();
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    Data = Serverdb.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == current).FirstOrDefault();
                }
                Data.Availability = (decimal)AvailabilityPercentage;
                Data.BDTime = BDTime;
                Data.CellId = CellID;
                Data.CorrectedDate = correctdate;
                Data.LossTime = IdleTime;
                Data.OEEPercent = (decimal)OEEPercentage;
                Data.OperatingTime = OperatingTime;
                Data.performance = (decimal)PerformancePercentage;
                Data.PerformanceFactor = performanceFactor;
                Data.Quality = (decimal)QualityPercentage;
                Data.TotalTime = TotalTime;
                Data.TotalQuantity = YieldQty;
                Data.scrapqty = reject;
                Data.TotalProduction = TotalProductoin;
                Data.AvailabilityInMin = Availability;
                Data.PowerOffTime = PowerOffTime;
                Data.OPwithMinorStoppage = OPwithMinorStoppage;
                Data.MinorLossTime = MinorLossTime;
                Data.PlannedCycleTimeInMin = plannedCycleTimeInMin;
                Data.PlannedBrkDurationinMin = (decimal)plannedBrkDurationinMin;
                Data.LastmachineYieldQty = YieldQty;
                Data.BottleNeckMachineYieldQtry = BottleNeckYieldQty;
                Data.LoadingUnLoadingWithProd = LoadingUnloadingWithProd;
                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                {
                    Serverdb.Entry(Data).State = EntityState.Modified;
                    Serverdb.SaveChanges();
                }
            }

        }



        #region Previous OEE Calculations
        //public void OEE(int CellID, DateTime QueryDate)
        //{

        //    DateTime correctedDate = Convert.ToDateTime(QueryDate);
        //    string correctdate = correctedDate.ToString("yyyy-MM-dd");
        //    int GetHour = System.DateTime.Now.Hour;
        //    DateTime StartModeTime = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd " + GetHour + ":00:00"));
        //    double AvailabilityPercentage = 0;
        //    double PerformancePercentage = 0;
        //    double QualityPercentage = 0;
        //    double OEEPercentage = 0;

        //    decimal OperatingTime = 0;
        //    decimal LossTime = 0;
        //    decimal MinorLossTime = 0;
        //    decimal MntTime = 0;
        //    decimal SetupTime = 0;
        //    decimal SetupMinorTime = 0;
        //    decimal PowerOffTime = 0;
        //    decimal PowerONTime = 0;
        //    decimal Utilization = 0;
        //    decimal DayOEEPercent = 0;
        //    int PerformanceFactor = 0;
        //    decimal Quality = 0;
        //    int TotlaQty = 0;
        //    int YieldQty = 0;
        //    int BottleNeckYieldQty = 0;
        //    decimal IdealCycleTimeVal = 2;
        //    decimal plannedCycleTime = 0;
        //    decimal LoadingTime = 0;
        //    decimal UnloadingTime = 0;

        //    double plannedBrkDurationinMin = 0;
        //    decimal LoadingUnloadingWithProd = 0;
        //    decimal LoadingUnloadingwithProdBottleNeck = 0;
        //    int minorstoppage = 0;
        //    decimal TotalProductoin = 0;
        //    decimal Availability;
        //    int rejQty = 0;
        //    int reject = 0;
        //    //  string plantName = row.tblplant.PlantName;
        //    var machineslist = new List<tblmachinedetail>();
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        // Get Machines
        //        machineslist = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsBottelNeck == 1).OrderBy(m => m.MachineID).ToList();
        //    }
        //    foreach (var machine in machineslist)
        //    {
        //        Machines machines = new Machines();
        //        int machineID = machine.MachineID;
        //        //int machineID = machineslist.MachineID;
        //        // Mode details
        //        minorstoppage = Convert.ToInt32(machine.MachineIdleMin) * 60; // in sec
        //        var GetModeDurations = Serverdb.tblmodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 1).ToList();
        //        OperatingTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "PROD").ToList().Sum(m => m.DurationInSec));
        //        PowerOffTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWEROFF").ToList().Sum(m => m.DurationInSec));
        //        MntTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "MNT").ToList().Sum(m => m.DurationInSec));
        //        MinorLossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec < minorstoppage).ToList().Sum(m => m.DurationInSec));
        //        LossTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "IDLE" && m.DurationInSec > minorstoppage).ToList().Sum(m => m.DurationInSec));
        //        PowerONTime = Convert.ToDecimal(GetModeDurations.Where(m => m.ModeType == "POWERON").ToList().Sum(m => m.DurationInSec));
        //        OperatingTime = Math.Round((OperatingTime / 60), 2);
        //        PowerOffTime = (PowerOffTime / 60);
        //        MntTime = (MntTime / 60);
        //        MinorLossTime = (MinorLossTime / 60);
        //        LossTime = (LossTime / 60);
        //        PowerONTime = (PowerONTime / 60);


        //        var plannedbrks = Serverdb.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
        //        foreach (var row in plannedbrks)
        //        {
        //            plannedBrkDurationinMin += Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.EndTime).Subtract(Convert.ToDateTime(correctedDate.Date.ToString("yyyy-MM-dd") + " " + row.StartTime)).TotalMinutes;
        //        }
        //        foreach (var ModeRow in GetModeDurations)
        //        {
        //            if (ModeRow.ModeType == "SETUP")
        //            {
        //                try
        //                {
        //                    SetupTime += (decimal)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
        //                    //SetupMinorTime += (decimal)(db.tblSetupMaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60.00);
        //                }
        //                catch { }
        //            }

        //        }

        //        //var GetModeDurationsRunning = Serverdb.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 0).ToList();
        //        //foreach (var ModeRow in GetModeDurationsRunning)
        //        //{
        //        //    String ColorCode = ModeRow.ColorCode;
        //        //    DateTime StartTime = (DateTime)ModeRow.StartTime;
        //        //    decimal Duration = (decimal)System.DateTime.Now.Subtract(StartTime).TotalMinutes;
        //        //    if (ColorCode == "YELLOW")
        //        //    {
        //        //        LossTime += Duration;
        //        //    }
        //        //    else if (ColorCode == "GREEN")
        //        //    {
        //        //        OperatingTime += Duration;
        //        //    }
        //        //    else if (ColorCode == "RED")
        //        //    {
        //        //        MntTime += Duration;
        //        //    }
        //        //    else if (ColorCode == "BLUE")
        //        //    {
        //        //        PowerOffTime += Duration;
        //        //    }
        //        //}


        //        LoadingTime += (decimal)machine.StdLoadingTime;
        //        UnloadingTime += (decimal)machine.StdUnLoadingTime;

        //        //var macid = Serverdb.tblmachinedetails.Where(m => m.CellID == CellID && m.IsDeleted == 0).Select(m => m.MachineID).ToList();

        //        //foreach (var items in macid)
        //        //{
        //        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //        {
        //            var scrap = Serverdb.tblworkorderentries.Where(m => m.MachineID == machine.MachineID && m.tblmachinedetail.IsLastMachine == 1).FirstOrDefault();
        //            if (scrap != null)
        //            {
        //                var scrapqty1 = new List<int?>();
        //                using (i_facilityEntities1 db = new i_facilityEntities1())
        //                {
        //                    scrapqty1 = db.tblrejectqties.Where(m => m.WOID == scrap.HMIID && m.CorrectedDate == correctdate).Select(m => m.RejectQty).ToList();
        //                }

        //                foreach (int r1 in scrapqty1)
        //                {
        //                    reject = reject + r1;
        //                }
        //            }
        //        }

        //        //  }

        //        plannedCycleTime += Convert.ToDecimal(machine.PlannedCycleTimeInSec);
        //        var prodplanchine = new List<tblworkorderentry>();
        //        using (i_facilityEntities1 db = new i_facilityEntities1())
        //        {
        //            prodplanchine = db.tblworkorderentries.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && (m.IsFinished == 1 || m.IsHold == 1)).ToList();
        //        }
        //        foreach (var ProdRow in prodplanchine)
        //        {
        //            rejQty += ProdRow.ScrapQty;
        //            YieldQty -= ProdRow.ScrapQty;
        //        }
        //    }
        //    TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty);
        //    if (YieldQty == 0)
        //        YieldQty = 1;
        //    LoadingUnloadingWithProd = ((LoadingTime + UnloadingTime) / 60) * YieldQty;
        //    LoadingUnloadingwithProdBottleNeck = ((LoadingTime + UnloadingTime) / 60) * BottleNeckYieldQty;


        //    decimal OPwithMinorStoppage = (OperatingTime + MinorLossTime);

        //    decimal utilFactor = Math.Round((LoadingUnloadingWithProd + OperatingTime), 2);

        //    decimal IdleTime = LossTime + MinorLossTime;
        //    decimal BDTime = MntTime;

        //    int TotalTime = 24 * 60;
        //    if (TotalTime == 0)
        //    {
        //        TotalTime = 1;
        //    }
        //    if (TotlaQty == 0)
        //        TotlaQty = 1;

        //    decimal plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
        //    Availability = Math.Round((TotalTime - (decimal)plannedBrkDurationinMin), 2);

        //    decimal TotalTimeWithPlannedBrk = Availability;

        //    decimal AvailabilityPercent = Math.Round((OPwithMinorStoppage / TotalTimeWithPlannedBrk), 2) * 100;  // From BottleNeckMachine
        //    decimal PerformanceBottelNeck = Math.Round(((plannedCycleTimeInMin * YieldQty) / OPwithMinorStoppage), 2) * 100;
        //    decimal performanceFactor = (plannedCycleTime * YieldQty);
        //    decimal QualityLastMachine = Math.Round((decimal)((YieldQty - reject) / YieldQty), 2) * 100;            // From LastMachine
        //    DayOEEPercent = (decimal)Math.Round((double)(AvailabilityPercent / 100) * (double)(PerformanceBottelNeck / 100) * (double)(QualityLastMachine / 100), 2) * 100;

        //    //decimal availabilityDenominator = Math.Round((plannedCycleTimeInMin + LoadingUnloadingWithProd), 2);

        //    //TotalProductoin = Math.Round((Availability / availabilityDenominator) * 100, 2);
        //    //decimal performance = Math.Round((utilFactor / TotalProductoin) * 100, 2);
        //    //decimal performanceFactor = Math.Round((utilFactor));

        //    //decimal quality = Math.Round((decimal)(YieldQty / (YieldQty + rejQty)) * 100, 2);

        //    //Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(utilFactor) / Convert.ToDouble(TotalTime)) * 100));

        //    //DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(performance / 100) * (double)(quality / 100), 2) * 100;

        //    AvailabilityPercentage = (double)AvailabilityPercent;
        //    QualityPercentage = (double)QualityLastMachine;
        //    PerformancePercentage = (double)PerformanceBottelNeck;
        //    OEEPercentage = (double)DayOEEPercent;


        //    DateTime curr = DateTime.Now;
        //    string current = curr.ToString("yyyy-MM-dd");
        //    var doesthisexists = new List<tbloeedetail>();
        //    using (i_facilityEntities1 db = new i_facilityEntities1())
        //    {
        //        doesthisexists = db.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == correctdate).ToList();
        //    }
        //    if (doesthisexists.Count == 0)
        //    {

        //        tbloeedetail obj = new tbloeedetail();
        //        obj.Availability = (decimal)AvailabilityPercentage;
        //        obj.BDTime = BDTime;
        //        obj.CellId = CellID;
        //        obj.CorrectedDate = correctdate;
        //        obj.LossTime = IdleTime;
        //        obj.OEEPercent = (decimal)OEEPercentage;
        //        obj.OperatingTime = OperatingTime;
        //        obj.performance = (decimal)PerformancePercentage;
        //        obj.PerformanceFactor = performanceFactor;
        //        obj.Quality = (decimal)QualityPercentage;
        //        obj.TotalTime = TotalTime;
        //        obj.TotalQuantity = YieldQty;
        //        obj.scrapqty = reject;
        //        obj.TotalProduction = TotalProductoin;
        //        obj.AvailabilityInMin = Availability;
        //        obj.PowerOffTime = PowerOffTime;
        //        obj.OPwithMinorStoppage = OPwithMinorStoppage;
        //        obj.MinorLossTime = MinorLossTime;
        //        obj.PlannedCycleTimeInMin = plannedCycleTimeInMin;
        //        obj.PlannedBrkDurationinMin = (decimal)plannedBrkDurationinMin;
        //        obj.LastmachineYieldQty = YieldQty;
        //        obj.LoadingUnLoadingWithProd = LoadingUnloadingWithProd;
        //        obj.BottleNeckMachineYieldQtry = BottleNeckYieldQty;
        //        using (i_facilityEntities1 db = new i_facilityEntities1())
        //        {
        //            db.tbloeedetails.Add(obj);
        //            db.SaveChanges();
        //        }

        //    }
        //    else if (current == correctdate)
        //    {
        //        var Data = new tbloeedetail();
        //        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //        {
        //            Data = Serverdb.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == current).FirstOrDefault();
        //        }
        //        Data.Availability = (decimal)AvailabilityPercentage;
        //        Data.BDTime = BDTime;
        //        Data.CellId = CellID;
        //        Data.CorrectedDate = correctdate;
        //        Data.LossTime = IdleTime;
        //        Data.OEEPercent = (decimal)OEEPercentage;
        //        Data.OperatingTime = OperatingTime;
        //        Data.performance = (decimal)PerformancePercentage;
        //        Data.PerformanceFactor = performanceFactor;
        //        Data.Quality = (decimal)QualityPercentage;
        //        Data.TotalTime = TotalTime;
        //        Data.TotalQuantity = YieldQty;
        //        Data.scrapqty = reject;
        //        Data.TotalProduction = TotalProductoin;
        //        Data.AvailabilityInMin = Availability;
        //        Data.PowerOffTime = PowerOffTime;
        //        Data.OPwithMinorStoppage = OPwithMinorStoppage;
        //        Data.MinorLossTime = MinorLossTime;
        //        Data.PlannedCycleTimeInMin = plannedCycleTimeInMin;
        //        Data.PlannedBrkDurationinMin = (decimal)plannedBrkDurationinMin;
        //        Data.LastmachineYieldQty = YieldQty;
        //        Data.BottleNeckMachineYieldQtry = BottleNeckYieldQty;
        //        Data.LoadingUnLoadingWithProd = LoadingUnloadingWithProd;
        //        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //        {
        //            Serverdb.Entry(Data).State = EntityState.Modified;
        //            Serverdb.SaveChanges();
        //        }
        //    }

        //}


        private int GetQuantiy(int CellID, DateTime CorrectedDate, out int YieldQty, out int BottleNeckYieldQty)
        {
            int TotalQty = 0;
            YieldQty = 0;
            //BottleNeckTotalQty = 0;
            BottleNeckYieldQty = 0;
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");

            var bottleneckmachine = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsBottelNeck == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            var lastmachine = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsLastMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            var firtstmachine = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsFirstMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            int firstmachineId = 0;
            int lstmachineId = 0;
            int bottleneckMachineID = 0;
            if (firtstmachine != null)
                firstmachineId = firtstmachine.MachineID;
            if (lastmachine != null)
                lstmachineId = lastmachine.MachineID;
            if (bottleneckmachine != null)
                bottleneckMachineID = bottleneckmachine.MachineID;


            YieldQty = GetPartscount(lstmachineId, bottleneckMachineID, CorrectedDate, out BottleNeckYieldQty);

            return TotalQty;

        }
        #endregion





        #region Previous Code
        //public void OEE(int CellID, DateTime QueryDate)
        //{

        //    DateTime correctedDate = Convert.ToDateTime(QueryDate);
        //    string correctdate = correctedDate.ToString("yyyy-MM-dd");
        //    int GetHour = System.DateTime.Now.Hour;
        //    DateTime StartModeTime = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd " + GetHour + ":00:00"));
        //    double AvailabilityPercentage = 0;
        //    double PerformancePercentage = 0;
        //    double QualityPercentage = 0;
        //    double OEEPercentage = 0;

        //    decimal OperatingTime = 0;
        //    decimal LossTime = 0;
        //    decimal MinorLossTime = 0;
        //    decimal MntTime = 0;
        //    decimal SetupTime = 0;
        //    decimal SetupMinorTime = 0;
        //    decimal PowerOffTime = 0;
        //    decimal PowerONTime = 0;
        //    decimal Utilization = 0;
        //    decimal DayOEEPercent = 0;
        //    int PerformanceFactor = 0;
        //    decimal Quality = 0;
        //    int TotlaQty = 0;
        //    int YieldQty = 0;
        //    int BottleNeckYieldQty = 0;
        //    decimal IdealCycleTimeVal = 2;
        //    decimal plannedCycleTime = 0;
        //    decimal LoadingTime = 0;
        //    decimal UnloadingTime = 0;

        //    decimal LoadingUnloadingWithProd = 0;
        //    decimal TotalProductoin = 0;
        //    decimal Availability;
        //    int rejQty = 0;
        //    int reject = 0;
        //    //  string plantName = row.tblplant.PlantName;

        //    // Get Machines
        //    var machineslist = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID /*&& m.IsBottelNeck == 1*/).OrderBy(m => m.MachineID).ToList();
        //    foreach (var machine in machineslist)
        //    {
        //        Machines machines = new Machines();
        //        int machineID = machine.MachineID;
        //        //int machineID = machineslist.MachineID;
        //        // Mode details
        //        var GetModeDurations = Serverdb.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 1 && m.ModeTypeEnd == 1).ToList();
        //        foreach (var ModeRow in GetModeDurations)
        //        {
        //            //GetCorrectedDate = ModeRow.CorrectedDate;
        //            if (ModeRow.ModeType == "PROD")
        //            {
        //                OperatingTime += Math.Round((decimal)(ModeRow.DurationInSec / 60.00), 2);
        //            }
        //            else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec > 600)
        //            {
        //                LossTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //                decimal LossDuration = (decimal)(ModeRow.DurationInSec / 60.00);
        //                //if (ModeRow.LossCodeID != null)
        //                // insertProdlosses(ProdRow.HMIID, (int)ModeRow.LossCodeID, LossDuration, CorrectedDate, MachineID);
        //            }
        //            else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec < 600)
        //            {
        //                MinorLossTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //            }
        //            else if (ModeRow.ModeType == "MNT")
        //            {
        //                MntTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //            }
        //            else if (ModeRow.ModeType == "POWEROFF")
        //            {
        //                PowerOffTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //            }
        //            else if (ModeRow.ModeType == "SETUP")
        //            {
        //                try
        //                {
        //                    SetupTime += (decimal)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
        //                    //SetupMinorTime += (decimal)(db.tblSetupMaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60.00);
        //                }
        //                catch { }
        //            }
        //            else if (ModeRow.ModeType == "POWERON")
        //            {
        //                PowerONTime += (decimal)(ModeRow.DurationInSec / 60.00);
        //            }
        //        }

        //        var GetModeDurationsRunning = Serverdb.tbllivemodes.Where(m => m.MachineID == machineID && m.CorrectedDate == correctedDate.Date && m.IsCompleted == 0).ToList();
        //        foreach (var ModeRow in GetModeDurationsRunning)
        //        {
        //            String ColorCode = ModeRow.ColorCode;
        //            DateTime StartTime = (DateTime)ModeRow.StartTime;
        //            decimal Duration = (decimal)System.DateTime.Now.Subtract(StartTime).TotalMinutes;
        //            if (ColorCode == "YELLOW")
        //            {
        //                LossTime += Duration;
        //            }
        //            else if (ColorCode == "GREEN")
        //            {
        //                OperatingTime += Duration;
        //            }
        //            else if (ColorCode == "RED")
        //            {
        //                MntTime += Duration;
        //            }
        //            else if (ColorCode == "BLUE")
        //            {
        //                PowerOffTime += Duration;
        //            }
        //        }


        //        LoadingTime += (decimal)machine.StdLoadingTime;
        //        UnloadingTime += (decimal)machine.StdUnLoadingTime;

        //        //var macid = Serverdb.tblmachinedetails.Where(m => m.CellID == CellID && m.IsDeleted == 0).Select(m => m.MachineID).ToList();

        //        //foreach (var items in macid)
        //        //{
        //        var scrap = Serverdb.tblworkorderentries.Where(m => m.MachineID == machine.MachineID && m.tblmachinedetail.IsLastMachine == 1).FirstOrDefault();
        //        if (scrap != null)
        //        {
        //            var scrapqty1 = Serverdb.tblrejectqties.Where(m => m.WOID == scrap.HMIID && m.CorrectedDate == correctdate).Select(m => m.RejectQty).ToList();

        //            foreach (int r1 in scrapqty1)
        //            {
        //                reject = reject + r1;
        //            }

        //        }
        //        //  }

        //        plannedCycleTime += Convert.ToDecimal(machine.PlannedCycleTimeInSec);

        //        var prodplanchine = Serverdb.tblworkorderentries.Where(m => m.MachineID == machineID && m.CorrectedDate == correctdate && (m.IsFinished == 1 || m.IsHold == 1)).ToList();
        //        foreach (var ProdRow in prodplanchine)
        //        {
        //            rejQty += ProdRow.ScrapQty;
        //            YieldQty -= ProdRow.ScrapQty;
        //        }
        //    }
        //    TotlaQty = GetQuantiy(CellID, correctedDate, out YieldQty, out BottleNeckYieldQty);
        //    if (YieldQty == 0)
        //        YieldQty = 1;
        //    LoadingUnloadingWithProd = ((LoadingTime + UnloadingTime) / 60) * YieldQty;
        //    decimal utilFactor = Math.Round((LoadingUnloadingWithProd + OperatingTime), 2);

        //    decimal IdleTime = LossTime + MinorLossTime;
        //    decimal BDTime = MntTime;

        //    int TotalTime = Convert.ToInt32(PowerONTime) + Convert.ToInt32(OperatingTime) + Convert.ToInt32(IdleTime) + Convert.ToInt32(BDTime) + Convert.ToInt32(PowerOffTime);
        //    if (TotalTime == 0)
        //    {
        //        TotalTime = 1;
        //    }
        //    if (TotlaQty == 0)
        //        TotlaQty = 1;

        //    decimal plannedCycleTimeInMin = Math.Round((plannedCycleTime / 60), 2);
        //    Availability = Math.Round((TotalTime - PowerOffTime), 2);
        //    decimal availabilityDenominator = Math.Round((plannedCycleTimeInMin + LoadingUnloadingWithProd), 2);

        //    TotalProductoin = Math.Round((Availability / availabilityDenominator) * 100, 2);
        //    decimal performance = Math.Round((utilFactor / TotalProductoin) * 100, 2);
        //    decimal performanceFactor = Math.Round((utilFactor));

        //    decimal quality = Math.Round((decimal)(YieldQty / (YieldQty + rejQty)) * 100, 2);

        //    Utilization = Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Convert.ToDouble(utilFactor) / Convert.ToDouble(TotalTime)) * 100));

        //    DayOEEPercent = (decimal)Math.Round((double)(Utilization / 100) * (double)(performance / 100) * (double)(quality / 100), 2) * 100;

        //    AvailabilityPercentage = (double)Utilization;
        //    QualityPercentage = (double)quality;
        //    PerformancePercentage = (double)performance;
        //    OEEPercentage = (double)DayOEEPercent;

        //    #region Previous calculations
        //    DateTime curr = DateTime.Now;
        //    string current = curr.ToString("yyyy-MM-dd");

        //    var doesthisexists = Serverdb.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == correctdate).ToList();
        //    if (doesthisexists.Count == 0)
        //    {

        //        tbloeedetail obj = new tbloeedetail();
        //        obj.Availability = (decimal)AvailabilityPercentage;
        //        obj.BDTime = BDTime;
        //        obj.CellId = CellID;
        //        obj.CorrectedDate = correctdate;
        //        obj.LossTime = IdleTime;
        //        obj.OEEPercent = (decimal)OEEPercentage;
        //        obj.OperatingTime = OperatingTime;
        //        obj.performance = (decimal)PerformancePercentage;
        //        obj.PerformanceFactor = performanceFactor;
        //        obj.Quality = (decimal)QualityPercentage;
        //        obj.TotalTime = TotalTime;
        //        obj.TotalQuantity = YieldQty;
        //        obj.scrapqty = reject;
        //        obj.TotalProduction = TotalProductoin;
        //        obj.AvailabilityInMin = Availability;
        //        obj.PowerOffTime = PowerOffTime;
        //        Serverdb.tbloeedetails.Add(obj);
        //        Serverdb.SaveChanges();

        //    }
        //    else if (current == correctdate)
        //    {
        //        var Data = Serverdb.tbloeedetails.Where(m => m.CellId == CellID && m.CorrectedDate == current).FirstOrDefault();
        //        Data.Availability = (decimal)AvailabilityPercentage;
        //        Data.BDTime = BDTime;
        //        Data.CellId = CellID;
        //        Data.CorrectedDate = correctdate;
        //        Data.LossTime = IdleTime;
        //        Data.OEEPercent = (decimal)OEEPercentage;
        //        Data.OperatingTime = OperatingTime;
        //        Data.performance = (decimal)PerformancePercentage;
        //        Data.PerformanceFactor = performanceFactor;
        //        Data.Quality = (decimal)QualityPercentage;
        //        Data.TotalTime = TotalTime;
        //        Data.TotalQuantity = YieldQty;
        //        Data.scrapqty = reject;
        //        Data.TotalProduction = TotalProductoin;
        //        Data.AvailabilityInMin = Availability;
        //        Data.PowerOffTime = PowerOffTime;
        //        Serverdb.Entry(Data).State = EntityState.Modified;
        //        Serverdb.SaveChanges();
        //    }

        //}
        #endregion

        private int GetPartscount(int LastMachineID, int BottleNeckMachineId, DateTime CorrectedDate, out int BottleNeckYieldQty)
        {
            int TotalPartsCount = 0;
            BottleNeckYieldQty = 0;

            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");
            string StartTime = Correcteddate + " 07:15:00";
            string EndTime = NxtCorrecteddate + " 07:15:00";
            var parts_cuttingslist = new List<tblpartscountandcutting>();
            var partslist = new List<tblpartscountandcutting>();
            DateTime St = Convert.ToDateTime(StartTime);
            DateTime Et = Convert.ToDateTime(EndTime);
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                parts_cuttingslist = db.tblpartscountandcuttings.Where(m => m.MachineID == LastMachineID && m.CorrectedDate == CorrectedDate.Date && m.StartTime >= St && m.EndTime <= Et).ToList();
            }
            foreach (var row in parts_cuttingslist)
            {
                TotalPartsCount += row.PartCount;
            }
            using (i_facilityEntities1 db = new i_facilityEntities1())
            {
                partslist = db.tblpartscountandcuttings.Where(m => m.MachineID == BottleNeckMachineId && m.CorrectedDate == CorrectedDate.Date && m.StartTime >= St && m.EndTime <= Et).ToList();
            }
            foreach (var row in partslist)
            {
                BottleNeckYieldQty += row.PartCount;
            }

            return TotalPartsCount;
        }


        private int GetQuantiy(int CellID, DateTime CorrectedDate, out int YieldQty, out int BottleNeckYieldQty, int bottleneckMachineID/*, out int BottleNeckTotalQty*/)
        {
            int TotalQty = 0;
            YieldQty = 0;
            //BottleNeckTotalQty = 0;
            BottleNeckYieldQty = 0;
            string Correcteddate = CorrectedDate.ToString("yyyy-MM-dd");
            string NxtCorrecteddate = CorrectedDate.AddDays(1).ToString("yyyy-MM-dd");

            var bottleneckmachine = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsBottelNeck == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            var lastmachine = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsLastMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            var firtstmachine = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.IsNormalWC == 0 && m.CellID == CellID && m.IsFirstMachine == 1).OrderBy(m => m.MachineID).FirstOrDefault();
            int firstmachineId = 0;
            int lstmachineId = 0;
            // int bottleneckMachineID = bottleneckMachineID;
            if (firtstmachine != null)
                firstmachineId = firtstmachine.MachineID;
            if (lastmachine != null)
                lstmachineId = lastmachine.MachineID;
            if (bottleneckmachine != null)
                bottleneckMachineID = bottleneckmachine.MachineID;


            YieldQty = GetPartscount(lstmachineId, bottleneckMachineID, CorrectedDate, out BottleNeckYieldQty);

            //string StartTime = Correcteddate + " 07:15:00";
            //string EndTime = NxtCorrecteddate + " 07:15:00";

            //DateTime St = Convert.ToDateTime(StartTime);
            //DateTime Et = Convert.ToDateTime(EndTime);

            //// Based on 1st Machine
            //var parametermasterlist = Serverdb.parameters_master.Where(m => m.MachineID == firstmachineId && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            //var TopRow = parametermasterlist.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            //var LastRow = parametermasterlist.OrderBy(m => m.ParameterID).FirstOrDefault();


            //// Based on Last Machine
            //var parametermasterlistLast = Serverdb.parameters_master.Where(m => m.MachineID == lstmachineId && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            //var TopRowLast = parametermasterlistLast.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            //var RowLast = parametermasterlistLast.OrderBy(m => m.ParameterID).FirstOrDefault();

            //// Based on Last Machine
            //var parametermasterlistBottleNeck = Serverdb.parameters_master.Where(m => m.MachineID == bottleneckMachineID && m.CorrectedDate == CorrectedDate.Date && m.InsertedOn >= St && m.InsertedOn <= Et).ToList();
            //var TopRowBottleNeck = parametermasterlistBottleNeck.OrderByDescending(m => m.ParameterID).FirstOrDefault();
            //var RowLastBottleNeck = parametermasterlistBottleNeck.OrderBy(m => m.ParameterID).FirstOrDefault();


            //if (TopRowLast != null && RowLast != null)
            //    YieldQty = Convert.ToInt32(TopRowLast.PartsTotal - RowLast.PartsTotal);

            //if (TopRow != null && LastRow != null)
            //    TotalQty = Convert.ToInt32(TopRow.PartsTotal - LastRow.PartsTotal);

            //if (TopRowBottleNeck != null && RowLastBottleNeck != null)
            //    BottleNeckYieldQty = Convert.ToInt32(TopRowBottleNeck.PartsTotal - RowLastBottleNeck.PartsTotal);

            return TotalQty;

        }


        public int getsundays(DateTime fdate, DateTime sdate)
        {
            //TimeSpan ts = fdate - sdate;
            //var sundays = ((ts.TotalDays / 7) + (sdate.DayOfWeek == DayOfWeek.Sunday || fdate.DayOfWeek == DayOfWeek.Sunday || fdate.DayOfWeek > sdate.DayOfWeek ? 1 : 0));

            //sundays = Math.Round(sundays - .5, MidpointRounding.AwayFromZero);

            //return (int)sundays;
            int sundayCount = 0;

            for (DateTime dt = sdate; dt < fdate; dt = dt.AddDays(1.0))
            {
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    sundayCount++;
                }
            }

            return sundayCount;
        }

        public ActionResult CycleTime()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];

            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");

            return View();
        }

        [HttpPost]
        public ActionResult CycleTime(string PlantID, string TimeType, DateTime FromDate, DateTime ToDate, string PartsList, string ShopID = null, string CellID = null, string WorkCenterID = null)
        {
            #region old
            //if (report.Shift == "--Select Shift--")
            //{
            //    report.Shift = "No Use";
            //}
            //if (report.ShopNo == null)
            //{
            //    report.ShopNo = "No Use";
            //}
            //if (report.WorkCenter == null)
            //{
            //    report.WorkCenter = "No Use";
            //}
            #endregion
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];

            CycleTimeReportExcel(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"), PartsList, PlantID.ToString(), Convert.ToString(ShopID), Convert.ToString(CellID), Convert.ToString(WorkCenterID));
            //UtilizationReportExcel(report.FromDate.ToString(), report.ToDate.ToString(), report.ShopNo.ToString(), report.WorkCenter.ToString(), TimeType);
            int p = Convert.ToInt32(PlantID);
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");

            return View();
        }

        // Part Learning  
        public void CycleTimeReportExcel(string StartDate, string EndDate, string PartsList, string PlantID, string ShopID = null, string CellID = null, string WorkCenterID = null)
        {
            #region Excel and Stuff

            DateTime frda = DateTime.Now;
            if (string.IsNullOrEmpty(StartDate) == true)
            {
                StartDate = DateTime.Now.Date.ToString();
            }
            if (string.IsNullOrEmpty(EndDate) == true)
            {
                EndDate = StartDate;
            }

            DateTime frmDate = Convert.ToDateTime(StartDate);
            DateTime toDate = Convert.ToDateTime(EndDate);

            double TotalDay = toDate.Subtract(frmDate).TotalDays;

            FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\PartLearning.xlsx");
            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
            ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];
            ExcelWorksheet workSheetGraphData = templatep.Workbook.Worksheets[3];

            String FileDir = @"C:\I_FacilityReports\ReportsList\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "CycleTime" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "CycleTime" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //" to " + toda.ToString("yyyy-MM-dd") + 
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    //return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;
            ExcelWorksheet worksheetGraph = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                workSheetGraphData = p.Workbook.Worksheets.Add("GraphData", workSheetGraphData);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), TemplateGraph);
                workSheetGraphData = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "GraphData", workSheetGraphData);

            }

            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            #endregion

            #region MacCount & LowestLevel
            string lowestLevel = null;
            int MacCount = 0;
            int plantId = 0, shopId = 0, cellId = 0, wcId = 0;
            if (string.IsNullOrEmpty(WorkCenterID))
            {
                if (string.IsNullOrEmpty(CellID))
                {
                    if (string.IsNullOrEmpty(ShopID))
                    {
                        if (string.IsNullOrEmpty(PlantID))
                        {
                            //donothing
                        }
                        else
                        {
                            lowestLevel = "Plant";
                            plantId = Convert.ToInt32(PlantID);
                            MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == plantId).ToList().Count();
                        }
                    }
                    else
                    {
                        lowestLevel = "Shop";
                        shopId = Convert.ToInt32(ShopID);
                        MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == shopId).ToList().Count();
                    }
                }
                else
                {
                    lowestLevel = "Cell";
                    cellId = Convert.ToInt32(CellID);
                    MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellId).ToList().Count();
                }
            }
            else
            {
                lowestLevel = "WorkCentre";
                wcId = Convert.ToInt32(WorkCenterID);
                MacCount = 1;
            }

            #endregion

            #region Get Machines List
            DataTable machin = new DataTable();
            DateTime endDateTime = Convert.ToDateTime(toDate.AddDays(1).ToString("yyyy-MM-dd") + " " + new TimeSpan(6, 0, 0));
            string startDateTime = frmDate.ToString("yyyy-MM-dd");
            using (MsqlConnection mc = new MsqlConnection())
            {
                mc.open();
                String query1 = null;
                if (lowestLevel == "Plant")
                {
                    query1 = " SELECT  distinct MachineID FROM  i_facility.tblmachinedetails WHERE PlantID = " + PlantID + "  and IsNormalWC = 0  and ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                else if (lowestLevel == "Shop")
                {
                    query1 = " SELECT * FROM  i_facility.tblmachinedetails WHERE ShopID = " + ShopID + "  and IsNormalWC = 0   and  ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                else if (lowestLevel == "Cell")
                {
                    query1 = " SELECT * FROM  i_facility.tblmachinedetails WHERE CellID = " + CellID + "  and IsNormalWC = 0  and   ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                else if (lowestLevel == "WorkCentre")
                {
                    query1 = "SELECT * FROM  i_facility.tblmachinedetails WHERE MachineID = " + WorkCenterID + "  and IsNormalWC = 0 and((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                MySqlDataAdapter da1 = new MySqlDataAdapter(query1, mc.sqlConnection);
                da1.Fill(machin);
                mc.close();
            }
            #endregion
            List<int> MachineIdList = new List<int>();
            foreach (DataRow intItem in machin.Rows)
            {
                MachineIdList.Add(Convert.ToInt32(intItem["MachineID"].ToString()));
            }
            DateTime UsedDateForExcel = Convert.ToDateTime(frmDate);
            //For each Date ...... for all Machines.
            var Col = 'B';
            int Row = 5; // Gap to Insert OverAll data. DataStartRow + MachinesCount + 2(1 for HighestLevel & another for Gap).
            int Sno = 1;
            string finalLossCol = null;
            string existingPartNo = PartsList;

            //DataTable for Consolidated Data 


            string correctedDate = UsedDateForExcel.ToString("yyyy-MM-dd");
            PartSearchCreate obj = new PartSearchCreate();
            obj.StartTime = Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd 07:00:00"));
            obj.EndTime = Convert.ToDateTime(Convert.ToDateTime(EndDate).AddDays(1).ToString("yyyy-MM-dd 07:00:00"));
            obj.MachineId = MachineIdList;
            obj.FG_code = existingPartNo;
           
            obj.CellID = cellId;
            int BottleneckMachineid = 0;
           

            List<CycleTiemDataGraph> cycleTimeList = new List<CycleTiemDataGraph>();
            for (int i = 0; i < TotalDay + 1; i++)
            {
                DateTime QueryDate = frmDate.AddDays(i);
                string corrdate = QueryDate.ToString("yyyy-MM-dd");
                obj.correctedDate = corrdate;
                PushDataToTblPartLearingReport(obj, out BottleneckMachineid);
                int StartingRowForToday = Row;
                //string dateforMachine = UsedDateForExcel.ToString("yyyy-MM-dd");
                //DateTime QueryDate = frmDate.AddDays(i);
                
                if (BottleneckMachineid!=0)                //foreach (var macId in MachineIdList)
                {
                    int macId = BottleneckMachineid;
                    //1) Get distinct partno,WoNo,Opno which are JF
                    //2) Get sum of green, settingTime, etc and push into excel
                    DataTable PartData = new DataTable();
                    using (MsqlConnection mc = new MsqlConnection())
                    {
                        mc.open();
                        //String query = "  select * FROM [i_facility].[i_facility].tblpartlearningreport where HMIID in  (SELECT HMIID FROM[i_facility].[i_facility].tblpartlearningreport where Part_No = '" + existingPartNo + "' and CorrectedDate = '" + QueryDate.ToString("yyyy-MM-dd") + "'); ";

                        //String query = "  select * FROM i_facility.tblpartlearningreport where HMIID in  (SELECT HMIID FROM i_facility.tblworkorderentry where CorrectedDate = '" + QueryDate.ToString("yyyy-MM-dd") + "' and MachineID = " + macId + " ); ";
                        String query = "  select * FROM i_facility.tblpartlearningreport where HMIID in  (SELECT HMIID FROM i_facility.tblworkorderentry where CorrectedDate = '" + corrdate + "' and MachineID = " + macId + " ); ";
                        //var search = Serverdb.tblworkorderentries.Where(m => m.CorrectedDate == corrdate && m.MachineID == macId).Select(m => m.HMIID).ToList();
                        //String query = Serverdb.tblpartlearningreports.Where(m => search.Contains((int)m.HMIID)).ToString();

                        if (obj.FG_code != null && obj.FG_code != "")
                        {
                            //query = "  select * FROM i_facility.tblpartlearningreport where HMIID in  (SELECT HMIID FROM i_facility.tblworkorderentry where PartNo = '" + existingPartNo + "' and CorrectedDate = '" + QueryDate.ToString("yyyy-MM-dd") + "' and MachineID = " + macId + " ); ";
                            query = "  select * FROM i_facility.tblpartlearningreport where HMIID in  (SELECT HMIID FROM i_facility.tblworkorderentry where FGCode = '" + existingPartNo + "' and CorrectedDate = '" + corrdate + "' and MachineID = " + macId + " ); ";

                            //var sear = Serverdb.tblworkorderentries.Where(m => m.FGCode == existingPartNo && m.CorrectedDate == corrdate && m.MachineID == macId).Select(m => m.HMIID).ToList();
                            //query = Serverdb.tblpartlearningreports.Where(m => sear.Contains((int)m.HMIID)).ToString();
                        }
                        else
                        {
                            query = " select * FROM i_facility.tblpartlearningreport where CorrectedDate = '" + corrdate + "' and MachineID = " + BottleneckMachineid + " ; ";
                        }



                        MySqlDataAdapter da = new MySqlDataAdapter(query, mc.sqlConnection);
                        da.Fill(PartData);
                        mc.close();
                    }
                    for (int j = 0; j < PartData.Rows.Count; j++)
                    {
                        int MachineID = Convert.ToInt32(PartData.Rows[j][1]); //MachineID
                        List<string> HierarchyData = GetHierarchyData(MachineID);

                        worksheet.Cells["B" + Row].Value = Sno++;
                        //worksheet.Cells["C" + Row].Value = HierarchyData[0];//Plant
                        //worksheet.Cells["D" + Row].Value = HierarchyData[1];//Shop
                        //worksheet.Cells["E" + Row].Value = HierarchyData[2];//Cell
                        worksheet.Cells["C" + Row].Value = HierarchyData[2];//Cell Display Name
                        string WorkOrderNo = Convert.ToString(PartData.Rows[j][4]);//WorkOrderNo
                        worksheet.Cells["D" + Row].Value = Convert.ToDateTime(Convert.ToString(PartData.Rows[j][3])).ToString("dd-MM-yyyy");//completed Date
                        worksheet.Cells["E" + Row].Value = HierarchyData[2];// BottleNeck Mac Display Name
                        worksheet.Cells["F" + Row].Value = PartData.Rows[j][5];//FG Code
                        string OpNo = Convert.ToString(PartData.Rows[j][6]);//OpNo
                        worksheet.Cells["G" + Row].Value = OpNo;
                        string TargetQty = Convert.ToString(PartData.Rows[j][7]);//TargetQty
                        int TargetQtyCalc = Convert.ToInt32(PartData.Rows[j][9]) + Convert.ToInt32(PartData.Rows[j][10]);//Yield Qty
                        if (TargetQtyCalc == 0)
                        {
                            TargetQtyCalc = 1;
                        }
                        worksheet.Cells["H" + Row].Value = TargetQty;
                        worksheet.Cells["I" + Row].Value = Convert.ToString(PartData.Rows[j][9]);//Yield Qty
                        worksheet.Cells["J" + Row].Value = Convert.ToInt32(PartData.Rows[j][10]); //Scrap Qty
                        double StdCycTime = Convert.ToDouble(PartData.Rows[j][18]);
                        double StdMinorLoss = Convert.ToDouble(PartData.Rows[j][21]);
                        double stdCycletimeinMin = StdCycTime / 60;
                        worksheet.Cells["K" + Row].Value = Math.Round(stdCycletimeinMin);//Std Cycle Time
                        worksheet.Cells["L" + Row].Value = StdMinorLoss; //Std Minor Loss
                        worksheet.Cells["M" + Row].Value =Math.Round( stdCycletimeinMin + StdMinorLoss); //Total Std Time

                        //worksheet.Cells["N" + Row].Value = Convert.ToInt32(PartData.Rows[j][22]); //Total Std Minor Loss
                        //worksheet.Cells["N" + Row].Value = Convert.ToInt32(PartData.Rows[j][11]); //Setting Time
                        //worksheet.Cells["O" + Row].Value = Convert.ToInt32(PartData.Rows[j][12]);//Idle

                        //worksheet.Cells["Q" + Row].Value = Convert.ToInt32(PartData.Rows[j][14]); //Blue
                        int HMIID = 0;
                        if (!string.IsNullOrEmpty(PartData.Rows[j][2].ToString()))
                        {
                             HMIID = Convert.ToInt32(PartData.Rows[j][2]);//Hmmid
                        }
                      
                        DataTable dt1 = new DataTable();
                        using (MsqlConnection mc = new MsqlConnection())
                        {
                            mc.open();
                            String qry = "SELECT WOStart,WOEnd FROM i_facility.tblworkorderentry where HMIID = '" + HMIID + "'";
                            //String qry = Serverdb.tblworkorderentries.Where(m => m.HMIID == HMIID).Select(m => new { m.WOStart, m.WOEnd }).ToString();
                            MySqlDataAdapter da = new MySqlDataAdapter(qry, mc.sqlConnection);
                            da.Fill(dt1);
                            mc.close();
                        }
                        int tbCount = dt1.Rows.Count;
                        int ActualCuttingTime = 0;
                        if (tbCount > 0)
                        {
                            string startDate = (dt1.Rows[0][0]).ToString();
                            string endDate = (dt1.Rows[0][1]).ToString();

                            DataTable dt2 = new DataTable();
                            using (MsqlConnection mc = new MsqlConnection())
                            {
                                mc.open();
                                String qry = "SELECT SUM(TIMESTAMPDIFF(MINUTE,StartTime,EndTime)) as diff FROM i_facility.tblmode where MachineID = " + MachineID + "  and StartTime>= '" + startDate + "' and EndTime<= '" + endDate + "' and MacMode = 'PROD'";
                                //String qry = from mode in Serverdb.tblmodes.Where(m=>m.MachineID == MachineID && m.StartTime == startDate && m.EndTime == endDate && m.MacMode == PROD)
                                MySqlDataAdapter da = new MySqlDataAdapter(qry, mc.sqlConnection);
                                da.Fill(dt2);
                                mc.close();
                            }
                            try
                            {
                                ActualCuttingTime = Convert.ToInt32(dt2.Rows[0][0])+ Convert.ToInt32(PartData.Rows[j][13]);
                            }
                            catch
                            {
                                ActualCuttingTime = 0;
                            }
                        }
                        else
                        {
                            DataTable dt2 = new DataTable();
                            using (MsqlConnection mc = new MsqlConnection())
                            {
                                mc.open();
                                String qry = "SELECT SUM(TIMESTAMPDIFF(MINUTE,StartTime,EndTime)) as diff FROM i_facility.tblmode where MachineID = " + MachineID + "  and correctedDate= '" + correctedDate + "' and MacMode = 'PROD'";
                                //String qry = from mode in Serverdb.tblmodes.Where(m=>m.MachineID == MachineID && m.StartTime == startDate && m.EndTime == endDate && m.MacMode == PROD)
                                MySqlDataAdapter da = new MySqlDataAdapter(qry, mc.sqlConnection);
                                da.Fill(dt2);
                                mc.close();
                            }
                            try
                            {
                                ActualCuttingTime = Convert.ToInt32(dt2.Rows[0][0]);
                            }
                            catch
                            {
                                ActualCuttingTime = 0;
                            }
                        }
                        worksheet.Cells["N" + Row].Value = ActualCuttingTime;
                        worksheet.Cells["O" + Row].Value = Convert.ToInt32(PartData.Rows[j][13]);//Minor Loss
                        worksheet.Cells["P" + Row].Value = ActualCuttingTime + Convert.ToInt32(PartData.Rows[j][13]);//Actual Total Operating Time
                        worksheet.Cells["Q" + Row].Value = Convert.ToInt32(PartData.Rows[j][17]);//Average Cuttng Time
                        worksheet.Cells["R" + Row].Value =Math.Round( Convert.ToDecimal(PartData.Rows[j][13]) / TargetQtyCalc,2);//Average Minor Loss
                        worksheet.Cells["S" + Row].Value = Convert.ToInt32(PartData.Rows[j][17]) + (Convert.ToInt32(PartData.Rows[j][13]) / TargetQtyCalc);//Average Total Operating Time
                        worksheet.Cells["T" + Row].Value = Math.Round( stdCycletimeinMin + StdMinorLoss - (Convert.ToInt32(PartData.Rows[j][17]) + (Convert.ToInt32(PartData.Rows[j][13]) / TargetQtyCalc)),2);//Cycle Time Delta
                        int CyCtimeDelta = (int)(stdCycletimeinMin + StdMinorLoss - (Convert.ToInt32(PartData.Rows[j][17]) + (Convert.ToInt32(PartData.Rows[j][13]) / TargetQtyCalc)));
                        setcellcolor(worksheet, CyCtimeDelta, "T" + Row.ToString());
                        worksheet.Cells["U" + Row].Value = Math.Round(((Convert.ToInt32(PartData.Rows[j][17]) + (Convert.ToInt32(PartData.Rows[j][13]) / TargetQtyCalc)) / ((StdCycTime + StdMinorLoss))) * 100, 0) / 100 * 100;//Cycle Time Delta %
                        double CycDel = Math.Round(((Convert.ToInt32(PartData.Rows[j][17]) + (Convert.ToInt32(PartData.Rows[j][13]) / TargetQtyCalc)) / ((StdCycTime + StdMinorLoss))) * 100, 0) - 100;
                        //double CycDel = Math.Round(((Convert.ToInt32(PartData.Rows[j][17]) + (Convert.ToInt32(PartData.Rows[j][13]) / TargetQtyCalc)) / ((StdCycTime + StdMinorLoss))) * 100, 0) / 100 *100;
                        settextcolor(worksheet, CycDel, "U" + Row.ToString());

                        string modelRange = "B" + Row + ":U" + Row + "";
                        var modelTable = worksheet.Cells[modelRange];
                        modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        CycleTiemDataGraph itemCycleTime = new CycleTiemDataGraph();
                        string fgcodOpno = PartData.Rows[j][5] + "-" + OpNo;
                        itemCycleTime.fgcodOpno = fgcodOpno;
                        itemCycleTime.YieldQty = Convert.ToInt32(PartData.Rows[j][9]);
                        itemCycleTime.ScrapQty = Convert.ToInt32(PartData.Rows[j][10]);
                        itemCycleTime.TotalStdTime = Math.Round(stdCycletimeinMin + StdMinorLoss,2);
                        itemCycleTime.ActualTotalOperatingTime = ActualCuttingTime + Convert.ToInt32(PartData.Rows[j][13]);
                        cycleTimeList.Add(itemCycleTime);
                        Row++;
                    }
                }

                UsedDateForExcel = UsedDateForExcel.AddDays(+1);
            }


            #region//graph data
            int RowGraph = 5;

            int intalColumn = 2;
            var iListItem = cycleTimeList.OrderBy(m => m.fgcodOpno);
            var uniqPart_NoOpNo = cycleTimeList.Select(m => m.fgcodOpno).Distinct();

            foreach (var fgItem in uniqPart_NoOpNo)
            {
                int ActRow = 1;
                string Part_NoOverAll = "";
                int totalYelidAndScrapQty = 0, TotalActualTotalOperatingTime = 0;
                double TotalTotalStdTime = 0;
                foreach (var item in iListItem)
                {
                    if (fgItem == item.fgcodOpno)
                    {
                        Part_NoOverAll = item.fgcodOpno;
                        totalYelidAndScrapQty = totalYelidAndScrapQty + item.YieldQty + item.ScrapQty;
                        TotalTotalStdTime = item.TotalStdTime;
                        TotalActualTotalOperatingTime = TotalActualTotalOperatingTime + item.ActualTotalOperatingTime;

                    }

                }
                if (totalYelidAndScrapQty == 0)
                    totalYelidAndScrapQty = 1;
                decimal diff = (Convert.ToDecimal(TotalActualTotalOperatingTime) / Convert.ToDecimal(totalYelidAndScrapQty));
                // string dcdff = diff.ToString("0.##");
                int dfrnc = Convert.ToInt32(Math.Round(diff));
                workSheetGraphData.Cells["A" + RowGraph].Value = Part_NoOverAll;//FG Code
                workSheetGraphData.Cells["B" + RowGraph].Value = totalYelidAndScrapQty;//Yield Qty+Scrap Qty
                workSheetGraphData.Cells["C" + RowGraph].Value = TotalTotalStdTime; //Total Std Time
                workSheetGraphData.Cells["D" + RowGraph].Value = TotalActualTotalOperatingTime;//Actual Total Operating Time
                workSheetGraphData.Cells["E" + RowGraph].Value = Convert.ToDecimal(dfrnc);//Actual Total Op Time = Cum of Actual Total Operating Time/Cum(yeild qty+scrap qty)


                var coluName = ExcelColumnFromNumber(intalColumn);
                workSheetGraphData.Cells[coluName + ActRow].Value = Part_NoOverAll;//FG Code
                ActRow++;
                workSheetGraphData.Cells[coluName + ActRow].Value = TotalTotalStdTime; //Total Std Time
                ActRow++;
                workSheetGraphData.Cells[coluName + ActRow].Value = Convert.ToDecimal(dfrnc);//Actual Total Op Time = Cum of Actual Total Operating Time/Cum(yeild qty+scrap qty)

                RowGraph++;
                intalColumn++;
            }

            for (int i = intalColumn; i <= 104; i++)
            {
                workSheetGraphData.Column(i).Hidden = true;
            }

            workSheetGraphData.Hidden = OfficeOpenXml.eWorkSheetHidden.VeryHidden;

            #endregion

            #region Save and Download

            //Hide Values
            //Color ColorHexWhite = System.Drawing.Color.White;
            //worksheetGraph.Cells["A1:Z50"].Style.Font.Color.SetColor(ColorHexWhite);

            //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            //worksheetGraph.View.ShowGridLines = false;
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "CycleTime" + frda.ToString("yyyy-MM-dd") + ".xlsx");
            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
            string Outgoingfile = "CycleTime" + frda.ToString("yyyy-MM-dd") + ".xlsx";
            if (file1.Exists)
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                Response.AddHeader("Content-Length", file1.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.WriteFile(file1.FullName);
                Response.Flush();
                Response.Close();
            }
            #endregion
        }

        #region Previous Code
        //public void PushDataToTblPartLearingReport(PartSearchCreate obj)
        //{
        //    //(obj.FG_code != null || obj.FG_code != "") && 
        //    if ((obj.StartTime != null) && (obj.EndTime != null) && (obj.MachineId != null))
        //    {
        //        foreach (var macId in obj.MachineId)
        //        {
        //            var getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();

        //            //String query = "  select * FROM [i_facility].[i_facility].[tblpartlearningreport] where HMIID in  (SELECT HMIID FROM [i_facility].[i_facility].[tblworkorderentry] where CorrectedDate >= '" + obj.StartTime.ToString("yyyy-MM-dd") + "'  and CorrectedDate <= '" + obj.EndTime.ToString("yyyy-MM-dd") + "' and MachineID = " + macId + " ); ";

        //            if (obj.FG_code != null && obj.FG_code != "")
        //            {
        //                getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.FGCode == obj.FG_code && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();

        //                //query = "  select * FROM [i_facility].[i_facility].[tblpartlearningreport] where HMIID in  (SELECT HMIID FROM [i_facility].[i_facility].[tblworkorderentry] where Part_No = '" + obj.FG_code + "' and CorrectedDate >= '" + obj.StartTime.ToString("yyyy-MM-dd") + "'  and CorrectedDate <= '" + obj.EndTime.ToString("yyyy-MM-dd") + "' and MachineID = " + macId + " ); ";
        //            }
        //            int count = getWorkOrderIds.Count();
        //            if (count > 0)
        //            {
        //                //DataTable PartData = new DataTable();
        //                //using (MsqlConnection mc = new MsqlConnection())
        //                //{
        //                //    mc.open();
        //                //    MySqlDataAdapter da = new MySqlDataAdapter(query, mc.sqlConnection);
        //                //    da.Fill(PartData);
        //                //    mc.close();
        //                //}
        //                //int countPartData = PartData.Rows.Count;
        //                //if (countPartData == 0)
        //                {
        //                    foreach (var item in getWorkOrderIds)
        //                    {
        //                        var GetDataPre = Serverdb.tblpartlearningreports.Where(m => m.HMIID == item.HMIID).ToList();
        //                        if (GetDataPre.Count == 0)
        //                        {
        //                            int OperatingTime = 0;
        //                            int LossTime = 0;
        //                            int MinorLossTime = 0;
        //                            int MntTime = 0;
        //                            int SetupTime = 0;
        //                            int SetupMinorTime = 0;
        //                            int PowerOffTime = 0;
        //                            long idle = 0;
        //                            decimal loadAndUnload = 0;
        //                            int rejections = 0;
        //                            DateTime ProdStartTime = item.WOStart;
        //                            DateTime ProdEndtime = DateTime.Now;
        //                            try
        //                            {
        //                                if (item.WOEnd.HasValue)
        //                                {
        //                                    ProdEndtime = Convert.ToDateTime(item.WOEnd);
        //                                }
        //                            }
        //                            catch { }

        //                            #region Logic to get the Mode Durations between a Production Order which are completed
        //                            var GetModeDurations = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).ToList();
        //                            foreach (var ModeRow in GetModeDurations)
        //                            {
        //                                if (ModeRow.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec > 600)
        //                                {
        //                                    LossTime += (int)(ModeRow.DurationInSec / 60);
        //                                    int LossDuration = (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec < 600)
        //                                {
        //                                    MinorLossTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "SETUP")
        //                                {
        //                                    try
        //                                    {
        //                                        SetupTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.DurationInSec).First() / 60);
        //                                        SetupMinorTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60);
        //                                    }
        //                                    catch { }
        //                                }
        //                            }
        //                            #endregion

        //                            #region Logic to get the Mode Duration Which Was started before this Production and Ended during this Production
        //                            var GetEndModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime < ProdStartTime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
        //                            if (GetEndModeDuration != null)
        //                            {
        //                                if (GetEndModeDuration.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "IDLE")
        //                                {
        //                                    LossTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                    int LossDuration = (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                    //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                            }
        //                            #endregion

        //                            #region Logic to get the Mode Duration Which Was Started during the Production and Ended after the Production
        //                            var GetStartModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime >= ProdStartTime && m.EndTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
        //                            if (GetStartModeDuration != null)
        //                            {
        //                                if (GetStartModeDuration.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "IDLE")
        //                                {
        //                                    LossTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                    int LossDuration = (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                    //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                            }
        //                            #endregion

        //                            int TotlaQty = item.Total_Qty;
        //                            if (TotlaQty == 0)
        //                                TotlaQty = 1;
        //                            int GetOptime = OperatingTime;
        //                            if (GetOptime == 0)
        //                            {
        //                                GetOptime = 1;
        //                            }
        //                            decimal IdealCycleTimeVal = 0;
        //                            var IdealCycTime = Serverdb.tblparts.Where(m => m.FGCode == item.FGCode && m.OperationNo == item.OperationNo).FirstOrDefault();
        //                            if (IdealCycTime != null)
        //                                IdealCycleTimeVal = IdealCycTime.IdealCycleTime;
        //                            double TotalTime = ProdEndtime.Subtract(ProdStartTime).TotalMinutes;
        //                            decimal UtilPercent = (decimal)Math.Round(OperatingTime / TotalTime * 100, 2);
        //                            decimal Quality = (decimal)Math.Round((double)item.Yield_Qty / TotlaQty * 100, 2);
        //                            decimal Performance = (decimal)Math.Round((double)IdealCycleTimeVal * (double)item.Total_Qty / GetOptime * 100, 2);
        //                            int PerformanceFactor = (int)IdealCycleTimeVal * item.Total_Qty;
        //                            //tbl_prodmanmachine PRA = new tbl_prodmanmachine();
        //                            //PRA.MachineID = macId;
        //                            //PRA.WOID = item.HMIID;
        //                            ////PRA.CorrectedDate = CorrectedDate.Date;
        //                            //PRA.TotalLoss = LossTime;
        //                            //PRA.TotalOperatingTime = OperatingTime;
        //                            //PRA.TotalSetup = SetupTime + SetupMinorTime;
        //                            //PRA.TotalMinorLoss = MinorLossTime - SetupMinorTime;
        //                            //PRA.TotalSetupMinorLoss = SetupMinorTime;
        //                            //PRA.TotalPowerLoss = PowerOffTime;
        //                            //PRA.UtilPercent = UtilPercent;
        //                            //PRA.QualityPercent = Quality;
        //                            //PRA.PerformancePerCent = Performance;
        //                            //PRA.PerfromaceFactor = PerformanceFactor;
        //                            //PRA.InsertedOn = DateTime.Now;
        //                            loadAndUnload = MinorLossTime;
        //                            int TotalQty = item.Yield_Qty + item.ScrapQty;
        //                            if (TotalQty == 0)
        //                                TotalQty = 1;
        //                            rejections = Convert.ToInt32((OperatingTime / TotalQty) * item.ScrapQty);

        //                            var GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
        //                            foreach (var LossRow in GetMainLossList)
        //                            {
        //                                var getWoLossList1 = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == item.HMIID && m.LossID == LossRow.LossCodeID).FirstOrDefault();
        //                                if (getWoLossList1 == null)
        //                                {
        //                                    idle = idle + 0;
        //                                }
        //                                else
        //                                {
        //                                    idle = idle + getWoLossList1.LossDuration;
        //                                }
        //                                if (LossRow.LossCode == "LOAD / UNLOAD")
        //                                {
        //                                    if (getWoLossList1 == null)
        //                                    {
        //                                        loadAndUnload = loadAndUnload + 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        loadAndUnload = loadAndUnload + getWoLossList1.LossDuration;
        //                                    }
        //                                }
        //                            }
        //                            var dbParts = Serverdb.tblparts.Where(m => m.FGCode == item.FGCode && m.OperationNo == item.OperationNo).FirstOrDefault();
        //                            decimal idealctime = 0;
        //                            decimal? stdmloss = 0;
        //                            if (dbParts != null)
        //                            {
        //                                idealctime = dbParts.IdealCycleTime;
        //                                if (dbParts.StdMinorLoss != null)
        //                                {
        //                                    stdmloss = (decimal)dbParts.StdMinorLoss;
        //                                }
        //                                else
        //                                {
        //                                    stdmloss = 0;
        //                                }
        //                            }
        //                            tblpartlearningreport partLearning = new tblpartlearningreport();
        //                            partLearning.MachineID = macId;
        //                            partLearning.HMIID = item.HMIID;
        //                            //partLearning.CorrectedDate = item.CorrectedDate.ToString("yyyy-MM-dd");
        //                            partLearning.CorrectedDate = item.CorrectedDate;
        //                            partLearning.WorkOrderNo = item.Prod_Order_No;
        //                            partLearning.FGCode = item.FGCode;
        //                            partLearning.OpNo = item.OperationNo;
        //                            partLearning.TargetQty = item.ProdOrderQty;
        //                            partLearning.TotalQty = item.Total_Qty;
        //                            partLearning.YieldQty = item.Yield_Qty;
        //                            partLearning.ScrapQty = item.ScrapQty;
        //                            partLearning.SettingTime = SetupTime + SetupMinorTime;
        //                            partLearning.Idle = idle;
        //                            partLearning.MinorLoss = loadAndUnload;
        //                            partLearning.PowerOff = PowerOffTime;
        //                            partLearning.TotalNCCuttingTime = OperatingTime;
        //                            try
        //                            {
        //                                partLearning.AvgCuttingTime = OperatingTime / item.Total_Qty;
        //                            }
        //                            catch
        //                            {
        //                                partLearning.AvgCuttingTime = 0;
        //                            }

        //                            partLearning.StdCycleTime = idealctime;
        //                            partLearning.TotalStdCycleTime = idealctime * item.Total_Qty;
        //                            partLearning.StdMinorLoss = stdmloss;
        //                            partLearning.TotalStdMinorLoss = stdmloss * item.Total_Qty;
        //                            partLearning.InsertedOn = DateTime.Now;
        //                            partLearning.StartTime = obj.StartTime;
        //                            partLearning.EndTime = obj.EndTime;
        //                            Serverdb.tblpartlearningreports.Add(partLearning);
        //                            Serverdb.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //}
        #endregion



        public void PushDataToTblPartLearingReport( PartSearchCreate obj,out int bottleneckmachineid)
        {

            bottleneckmachineid = 0;
            //(obj.FG_code != null || obj.FG_code != "") && 
            if ((obj.StartTime != null) && (obj.EndTime != null) && (obj.MachineId != null))
            {
                //if(obj.MachineId==) (var macId in obj.MachineId)            //foreach (var macId in obj.MachineId)
                //{
                var machineslist = new List<tblmachinedetail>();
                var bottleneckmachines = new tblbottelneck();
                var workorder = new tblworkorderentry();
                var scrapqty1 = new List<tblrejectqty>();
                var cellpartDet = new tblcellpart();
                var partsDet = new tblpart();
                using (i_facilityEntities1 db = new i_facilityEntities1())
                {
                    workorder = db.tblworkorderentries.Where(m => m.CellID == obj.CellID && m.CorrectedDate == obj.correctedDate).OrderByDescending(m => m.HMIID).FirstOrDefault();  //workorder entry
                    if (workorder != null)
                    {
                        partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == workorder.PartNo).FirstOrDefault();
                        if (partsDet != null)
                            bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == partsDet.FGCode && m.CellID == workorder.CellID).FirstOrDefault();
                    }
                    else
                    {
                        cellpartDet = db.tblcellparts.Where(m => m.CellID == obj.CellID && m.IsDefault == 1 && m.IsDeleted == 0).FirstOrDefault();
                        if (cellpartDet != null)
                            bottleneckmachines = db.tblbottelnecks.Where(m => m.PartNo == cellpartDet.partNo && m.CellID == cellpartDet.CellID).FirstOrDefault();
                        string Operationnum = bottleneckmachines.tblmachinedetail.OperationNumber.ToString();
                        partsDet = db.tblparts.Where(m => m.IsDeleted == 0 && m.FGCode == cellpartDet.partNo && m.OperationNo == Operationnum).FirstOrDefault();

                    }

                    bottleneckmachineid = bottleneckmachines.MachineID;
                }


                var getWorkOrderIds = new List<tblworkorderentry>();
                //using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                //{
                //    getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();
                //}
                //if (obj.FG_code != null && obj.FG_code != "")
                //{
                //    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                //    {
                //        getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.FGCode == obj.FG_code && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();
                //    }
                //}
                int count = getWorkOrderIds.Count();
                if (partsDet != null)
                {
                    if (partsDet != null)// foreach (var item in getWorkOrderIds)
                    {
                        var GetDataPre = new List<tblpartlearningreport>();
                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                        {
                            if (workorder != null)
                                GetDataPre = Serverdb.tblpartlearningreports.Where(m => m.HMIID == workorder.HMIID).ToList();
                            else
                                GetDataPre = Serverdb.tblpartlearningreports.Where(m => m.MachineID == bottleneckmachines.MachineID && m.CorrectedDate == obj.correctedDate).ToList();
                        }
                        if (GetDataPre.Count == 0)
                        {
                            int OperatingTime = 0;
                            int LossTime = 0;
                            int MinorLossTime = 0;
                            int MntTime = 0;
                            int SetupTime = 0;
                            int SetupMinorTime = 0;
                            int PowerOffTime = 0;
                            long idle = 0;
                            decimal loadAndUnload = 0;
                            int rejections = 0;
                            var GetModeDurations = new List<tblmode>();
                            if (workorder != null)
                            {
                                DateTime ProdStartTime = workorder.WOStart;
                                DateTime ProdEndtime = DateTime.Now;

                                try
                                {
                                    if (workorder.WOEnd.HasValue)
                                    {
                                        ProdEndtime = Convert.ToDateTime(workorder.WOEnd);
                                    }
                                }

                                catch { }


                                #region Logic to get the Mode Durations between a Production Order which are completed

                                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                                {
                                    GetModeDurations = Serverdb.tblmodes.Where(m => m.MachineID == bottleneckmachines.MachineID && m.StartTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).ToList();
                                }
                            }
                            else if (cellpartDet != null)
                            {
                                DateTime corrctdate = Convert.ToDateTime(obj.correctedDate);
                                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())

                                {
                                    GetModeDurations = Serverdb.tblmodes.Where(m => m.MachineID == bottleneckmachines.MachineID && m.IsCompleted == 1 && m.ModeTypeEnd == 1 && m.CorrectedDate == corrctdate.Date).ToList();
                                }
                            }
                            foreach (var ModeRow in GetModeDurations)
                            {
                                if (ModeRow.ModeType == "PROD")
                                {
                                    OperatingTime += (int)(ModeRow.DurationInSec / 60);
                                }
                                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec > 600)
                                {
                                    LossTime += (int)(ModeRow.DurationInSec / 60);
                                    int LossDuration = (int)(ModeRow.DurationInSec / 60);
                                }
                                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec < 600)
                                {
                                    MinorLossTime += (int)(ModeRow.DurationInSec / 60);
                                }
                                else if (ModeRow.ModeType == "MNT")
                                {
                                    MntTime += (int)(ModeRow.DurationInSec / 60);
                                }
                                else if (ModeRow.ModeType == "POWEROFF")
                                {
                                    PowerOffTime += (int)(ModeRow.DurationInSec / 60);
                                }
                                else if (ModeRow.ModeType == "SETUP")
                                {
                                    try
                                    {
                                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                                        {
                                            SetupTime += (int)Convert.ToDateTime(ModeRow.LossCodeEnteredTime).Subtract(Convert.ToDateTime(ModeRow.StartTime)).TotalMinutes;
                                            //SetupTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.DurationInSec).First() / 60);
                                            //SetupMinorTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60);
                                        }

                                    }
                                    catch { }
                                }
                            }
                            #endregion

                            #region Logic to get the Mode Duration Which Was started before this Production and Ended during this Production

                            var GetEndModeDuration = new tblmode();
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {
                                if (workorder != null)
                                {
                                    DateTime ProdStartTime = workorder.WOStart;
                                    DateTime ProdEndtime = DateTime.Now;
                                    GetEndModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == bottleneckmachines.MachineID && m.StartTime < ProdStartTime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();


                                    if (GetEndModeDuration != null)
                                    {
                                        if (GetEndModeDuration.ModeType == "PROD")
                                        {
                                            OperatingTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
                                        }
                                        else if (GetEndModeDuration.ModeType == "IDLE")
                                        {
                                            LossTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
                                            int LossDuration = (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
                                            //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
                                        }
                                        else if (GetEndModeDuration.ModeType == "MNT")
                                        {
                                            MntTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
                                        }
                                        else if (GetEndModeDuration.ModeType == "POWEROFF")
                                        {
                                            PowerOffTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region Logic to get the Mode Duration Which Was Started during the Production and Ended after the Production
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {
                                if (workorder != null)
                                {
                                    DateTime ProdStartTime = workorder.WOStart;
                                    DateTime ProdEndtime = DateTime.Now;
                                    var GetStartModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == bottleneckmachines.MachineID && m.StartTime >= ProdStartTime && m.EndTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
                                    if (GetStartModeDuration != null)
                                    {
                                        if (GetStartModeDuration.ModeType == "PROD")
                                        {
                                            OperatingTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
                                        }
                                        else if (GetStartModeDuration.ModeType == "IDLE")
                                        {
                                            LossTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
                                            int LossDuration = (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
                                            //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
                                        }
                                        else if (GetStartModeDuration.ModeType == "MNT")
                                        {
                                            MntTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
                                        }
                                        else if (GetStartModeDuration.ModeType == "POWEROFF")
                                        {
                                            PowerOffTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
                                        }
                                    }
                                }
                            }

                            #endregion
                            int YieldQty = 0;
                            int BottleNeckYieldQty = 0;
                            int reject = 0;
                            if (workorder != null)
                            {
                                using (i_facilityEntities1 db = new i_facilityEntities1())
                                {
                                    scrapqty1 = db.tblrejectqties.Where(m => m.WOID == workorder.HMIID && m.CorrectedDate == obj.correctedDate).ToList();
                                }

                                foreach (var r1 in scrapqty1)
                                {
                                    reject = reject + Convert.ToInt32(r1.RejectQty);
                                }

                            }
                            int TotlaQty = GetQuantiy(obj.CellID, Convert.ToDateTime(obj.correctedDate), out YieldQty, out BottleNeckYieldQty, bottleneckmachines.MachineID);
                            TotlaQty = YieldQty;
                            if (TotlaQty == 0)
                                TotlaQty = 1;
                            int GetOptime = OperatingTime;
                            if (GetOptime == 0)
                            {
                                GetOptime = 1;
                            }
                            decimal IdealCycleTimeVal = 0;
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {

                                var IdealCycTime = partsDet.IdealCycleTime;
                                if (IdealCycTime != null)
                                    IdealCycleTimeVal = (decimal)(IdealCycTime / 60);
                            }
                            double plannedBrkDurationinMin = 0;
                            var plannedbrks = Serverdb.tblplannedbreaks.Where(m => m.IsDeleted == 0).ToList();
                            foreach (var row in plannedbrks)
                            {
                                plannedBrkDurationinMin += Convert.ToDateTime(obj.correctedDate + " " + row.EndTime).Subtract(Convert.ToDateTime(obj.correctedDate + " " + row.StartTime)).TotalMinutes;
                            }
                            double TotalTime = 1440;
                            TotalTime = TotalTime - plannedBrkDurationinMin;
                            decimal Quality = 0;
                            decimal Performance = 0;
                            int PerformanceFactor = 0;
                           
                            loadAndUnload = ((partsDet.StdLoadingTime + partsDet.StdUnLoadingTime) * YieldQty) / 60;
                            double opwithloadunload =(double) OperatingTime +(double)loadAndUnload;
                            if (workorder != null)
                            {
                                DateTime ProdStartTime = workorder.WOStart;
                                DateTime ProdEndtime = DateTime.Now;
                                TotalTime = ProdEndtime.Subtract(ProdStartTime).TotalMinutes;
                                Quality = (decimal)Math.Round((double)(YieldQty - reject) / TotlaQty * 100, 2);
                                Performance = (decimal)Math.Round((double)IdealCycleTimeVal * (double)YieldQty / GetOptime * 100, 2);
                                PerformanceFactor = (int)IdealCycleTimeVal * YieldQty;

                            }
                            // OperatingTime = OperatingTime / 60;
                            decimal UtilPercent = (decimal)Math.Round(opwithloadunload / TotalTime * 100, 2);
                            Quality = (decimal)Math.Round((double)(YieldQty - reject) / TotlaQty * 100, 2);
                            Performance = (decimal)Math.Round(((double)IdealCycleTimeVal * (double)YieldQty / opwithloadunload )* 100, 2);
                            PerformanceFactor = (int)IdealCycleTimeVal * YieldQty;

                            loadAndUnload = MinorLossTime;
                            int TotalQty = YieldQty + reject;
                            if (TotalQty == 0)
                                TotalQty = 1;
                            rejections = Convert.ToInt32((OperatingTime / TotalQty) * reject);
                            var GetMainLossList = new List<tbllossescode>();
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {
                                GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
                            }
                            foreach (var LossRow in GetMainLossList)
                            {
                                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                                {
                                    if (workorder != null)
                                    {
                                        var getWoLossList1 = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == workorder.HMIID && m.LossID == LossRow.LossCodeID).FirstOrDefault();
                                        if (getWoLossList1 == null)
                                        {
                                            idle = idle + 0;
                                        }
                                        else
                                        {
                                            idle = idle + getWoLossList1.LossDuration;
                                        }
                                        if (LossRow.LossCode == "LOAD / UNLOAD")
                                        {
                                            if (getWoLossList1 == null)
                                            {
                                                loadAndUnload = loadAndUnload + 0;
                                            }
                                            else
                                            {
                                                loadAndUnload = loadAndUnload + getWoLossList1.LossDuration;
                                            }
                                        }
                                    }
                                }

                            }
                            loadAndUnload = ((partsDet.StdLoadingTime + partsDet.StdUnLoadingTime) * YieldQty) / 60;
                            var Targetdec = ((decimal)TotalTime / (IdealCycleTimeVal + ((partsDet.StdLoadingTime + partsDet.StdUnLoadingTime)/60)));
                           int TargetQty = Convert.ToInt32(Targetdec);
                            decimal idealctime = 0;
                            decimal? stdmloss = 0;
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {
                                // var dbParts = Serverdb.tblparts.Where(m => m.FGCode == item.FGCode && m.OperationNo == item.OperationNo).FirstOrDefault();

                                idealctime = IdealCycleTimeVal;
                                if (partsDet != null)
                                {
                                    idealctime = partsDet.IdealCycleTime;


                                    //if (dbParts.StdMinorLoss != null)
                                    //{
                                    //    stdmloss = (decimal)dbParts.StdMinorLoss;
                                    //}
                                    //else
                                    //{
                                    //    stdmloss = 0;
                                    //}
                                }
                            }

                            tblpartlearningreport partLearning = new tblpartlearningreport();
                            partLearning.MachineID = bottleneckmachines.MachineID;
                            if (workorder != null)
                            {
                                partLearning.HMIID = workorder.HMIID;
                                partLearning.WorkOrderNo = workorder.Prod_Order_No;

                            }
                            //partLearning.CorrectedDate = item.CorrectedDate.ToString("yyyy-MM-dd");
                            partLearning.CorrectedDate = obj.correctedDate;
                            partLearning.TargetQty = TargetQty;

                            partLearning.FGCode = partsDet.FGCode;
                            partLearning.OpNo = partsDet.OperationNo;

                            partLearning.TotalQty = TotalQty;
                            partLearning.YieldQty = YieldQty;
                            partLearning.ScrapQty = reject;
                            partLearning.SettingTime = SetupTime + SetupMinorTime;
                            partLearning.Idle = idle;
                            partLearning.MinorLoss = loadAndUnload;
                            partLearning.PowerOff = PowerOffTime;
                            partLearning.TotalNCCuttingTime =(decimal) opwithloadunload;
                            try
                            {
                                partLearning.AvgCuttingTime = (decimal)opwithloadunload / TotalQty;
                            }
                            catch
                            {
                                partLearning.AvgCuttingTime = 0;
                            }

                            partLearning.StdCycleTime = idealctime;
                            partLearning.TotalStdCycleTime = idealctime * TotalQty;
                            partLearning.StdMinorLoss = stdmloss;
                            partLearning.TotalStdMinorLoss = stdmloss * TotalQty;
                            partLearning.InsertedOn = DateTime.Now;
                            partLearning.StartTime = obj.StartTime;
                            partLearning.EndTime = obj.EndTime;
                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
                            {
                                Serverdb.tblpartlearningreports.Add(partLearning);
                                Serverdb.SaveChanges();
                            }

                        }
                    }
                }

            }

        }

        #region Previous CycleTimeCalculations
        //public void PushDataToTblPartLearingReport(PartSearchCreate obj)
        //{
        //    //(obj.FG_code != null || obj.FG_code != "") && 
        //    if ((obj.StartTime != null) && (obj.EndTime != null) && (obj.MachineId != null))
        //    {
        //        foreach (var macId in obj.MachineId)
        //        {
        //            var getWorkOrderIds = new List<tblworkorderentry>();
        //            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //            {
        //                getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();
        //            }
        //            if (obj.FG_code != null && obj.FG_code != "")
        //            {
        //                using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                {
        //                    getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.FGCode == obj.FG_code && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();
        //                }
        //            }
        //            int count = getWorkOrderIds.Count();
        //            if (count > 0)
        //            {
        //                foreach (var item in getWorkOrderIds)
        //                {
        //                    var GetDataPre = new List<tblpartlearningreport>();
        //                    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                    {
        //                        GetDataPre = Serverdb.tblpartlearningreports.Where(m => m.HMIID == item.HMIID).ToList();
        //                    }
        //                    if (GetDataPre.Count == 0)
        //                    {
        //                        int OperatingTime = 0;
        //                        int LossTime = 0;
        //                        int MinorLossTime = 0;
        //                        int MntTime = 0;
        //                        int SetupTime = 0;
        //                        int SetupMinorTime = 0;
        //                        int PowerOffTime = 0;
        //                        long idle = 0;
        //                        decimal loadAndUnload = 0;
        //                        int rejections = 0;
        //                        DateTime ProdStartTime = item.WOStart;
        //                        DateTime ProdEndtime = DateTime.Now;
        //                        try
        //                        {
        //                            if (item.WOEnd.HasValue)
        //                            {
        //                                ProdEndtime = Convert.ToDateTime(item.WOEnd);
        //                            }
        //                        }
        //                        catch { }

        //                        #region Logic to get the Mode Durations between a Production Order which are completed
        //                        var GetModeDurations = new List<tblmode>();
        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            GetModeDurations = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).ToList();
        //                        }
        //                        foreach (var ModeRow in GetModeDurations)
        //                        {
        //                            if (ModeRow.ModeType == "PROD")
        //                            {
        //                                OperatingTime += (int)(ModeRow.DurationInSec / 60);
        //                            }
        //                            else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec > 600)
        //                            {
        //                                LossTime += (int)(ModeRow.DurationInSec / 60);
        //                                int LossDuration = (int)(ModeRow.DurationInSec / 60);
        //                            }
        //                            else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec < 600)
        //                            {
        //                                MinorLossTime += (int)(ModeRow.DurationInSec / 60);
        //                            }
        //                            else if (ModeRow.ModeType == "MNT")
        //                            {
        //                                MntTime += (int)(ModeRow.DurationInSec / 60);
        //                            }
        //                            else if (ModeRow.ModeType == "POWEROFF")
        //                            {
        //                                PowerOffTime += (int)(ModeRow.DurationInSec / 60);
        //                            }
        //                            else if (ModeRow.ModeType == "SETUP")
        //                            {
        //                                try
        //                                {
        //                                    using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                                    {
        //                                        SetupTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.DurationInSec).First() / 60);
        //                                        SetupMinorTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60);
        //                                    }

        //                                }
        //                                catch { }
        //                            }
        //                        }
        //                        #endregion

        //                        #region Logic to get the Mode Duration Which Was started before this Production and Ended during this Production


        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            var GetEndModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime < ProdStartTime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
        //                            if (GetEndModeDuration != null)
        //                            {
        //                                if (GetEndModeDuration.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "IDLE")
        //                                {
        //                                    LossTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                    int LossDuration = (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                    //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                            }
        //                        }

        //                        #endregion

        //                        #region Logic to get the Mode Duration Which Was Started during the Production and Ended after the Production
        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            var GetStartModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime >= ProdStartTime && m.EndTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
        //                            if (GetStartModeDuration != null)
        //                            {
        //                                if (GetStartModeDuration.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "IDLE")
        //                                {
        //                                    LossTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                    int LossDuration = (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                    //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                            }
        //                        }

        //                        #endregion

        //                        int TotlaQty = item.Total_Qty;
        //                        if (TotlaQty == 0)
        //                            TotlaQty = 1;
        //                        int GetOptime = OperatingTime;
        //                        if (GetOptime == 0)
        //                        {
        //                            GetOptime = 1;
        //                        }
        //                        decimal IdealCycleTimeVal = 0;
        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            var IdealCycTime = Serverdb.tblmachinedetails.Where(m => m.MachineID == item.MachineID).FirstOrDefault();
        //                            if (IdealCycTime != null)
        //                                IdealCycleTimeVal = (decimal)(IdealCycTime.PlannedCycleTimeInSec / 60);
        //                        }

        //                        double TotalTime = ProdEndtime.Subtract(ProdStartTime).TotalMinutes;
        //                        decimal UtilPercent = (decimal)Math.Round(OperatingTime / TotalTime * 100, 2);
        //                        decimal Quality = (decimal)Math.Round((double)item.Yield_Qty / TotlaQty * 100, 2);
        //                        decimal Performance = (decimal)Math.Round((double)IdealCycleTimeVal * (double)item.Total_Qty / GetOptime * 100, 2);
        //                        int PerformanceFactor = (int)IdealCycleTimeVal * item.Total_Qty;

        //                        loadAndUnload = MinorLossTime;
        //                        int TotalQty = item.Yield_Qty + item.ScrapQty;
        //                        if (TotalQty == 0)
        //                            TotalQty = 1;
        //                        rejections = Convert.ToInt32((OperatingTime / TotalQty) * item.ScrapQty);
        //                        var GetMainLossList = new List<tbllossescode>();
        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
        //                        }
        //                        foreach (var LossRow in GetMainLossList)
        //                        {
        //                            using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                            {

        //                                var getWoLossList1 = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == item.HMIID && m.LossID == LossRow.LossCodeID).FirstOrDefault();
        //                                if (getWoLossList1 == null)
        //                                {
        //                                    idle = idle + 0;
        //                                }
        //                                else
        //                                {
        //                                    idle = idle + getWoLossList1.LossDuration;
        //                                }
        //                                if (LossRow.LossCode == "LOAD / UNLOAD")
        //                                {
        //                                    if (getWoLossList1 == null)
        //                                    {
        //                                        loadAndUnload = loadAndUnload + 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        loadAndUnload = loadAndUnload + getWoLossList1.LossDuration;
        //                                    }
        //                                }
        //                            }

        //                        }
        //                        decimal idealctime = 0;
        //                        decimal? stdmloss = 0;
        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            var dbParts = Serverdb.tblparts.Where(m => m.FGCode == item.FGCode && m.OperationNo == item.OperationNo).FirstOrDefault();

        //                            idealctime = IdealCycleTimeVal;
        //                            if (dbParts != null)
        //                            {
        //                                idealctime = dbParts.IdealCycleTime;


        //                                //if (dbParts.StdMinorLoss != null)
        //                                //{
        //                                //    stdmloss = (decimal)dbParts.StdMinorLoss;
        //                                //}
        //                                //else
        //                                //{
        //                                //    stdmloss = 0;
        //                                //}
        //                            }
        //                        }

        //                        tblpartlearningreport partLearning = new tblpartlearningreport();
        //                        partLearning.MachineID = macId;
        //                        partLearning.HMIID = item.HMIID;
        //                        //partLearning.CorrectedDate = item.CorrectedDate.ToString("yyyy-MM-dd");
        //                        partLearning.CorrectedDate = item.CorrectedDate;
        //                        partLearning.WorkOrderNo = item.Prod_Order_No;
        //                        partLearning.FGCode = item.FGCode;
        //                        partLearning.OpNo = item.OperationNo;
        //                        partLearning.TargetQty = item.ProdOrderQty;
        //                        partLearning.TotalQty = item.Total_Qty;
        //                        partLearning.YieldQty = item.Yield_Qty;
        //                        partLearning.ScrapQty = item.ScrapQty;
        //                        partLearning.SettingTime = SetupTime + SetupMinorTime;
        //                        partLearning.Idle = idle;
        //                        partLearning.MinorLoss = loadAndUnload;
        //                        partLearning.PowerOff = PowerOffTime;
        //                        partLearning.TotalNCCuttingTime = OperatingTime;
        //                        try
        //                        {
        //                            partLearning.AvgCuttingTime = OperatingTime / item.Total_Qty;
        //                        }
        //                        catch
        //                        {
        //                            partLearning.AvgCuttingTime = 0;
        //                        }

        //                        partLearning.StdCycleTime = idealctime;
        //                        partLearning.TotalStdCycleTime = idealctime * item.Total_Qty;
        //                        partLearning.StdMinorLoss = stdmloss;
        //                        partLearning.TotalStdMinorLoss = stdmloss * item.Total_Qty;
        //                        partLearning.InsertedOn = DateTime.Now;
        //                        partLearning.StartTime = obj.StartTime;
        //                        partLearning.EndTime = obj.EndTime;
        //                        using (i_facilityEntities1 Serverdb = new i_facilityEntities1())
        //                        {
        //                            Serverdb.tblpartlearningreports.Add(partLearning);
        //                            Serverdb.SaveChanges();
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //    }

        //}
        #endregion

        //public void PushDataToTblPartLearingReport(PartSearchCreate obj)
        //{
        //    //(obj.FG_code != null || obj.FG_code != "") && 
        //    if ((obj.StartTime != null) && (obj.EndTime != null) && (obj.MachineId != null))
        //    {
        //        foreach (var macId in obj.MachineId)
        //        {
        //            var getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();

        //            //String query = "  select * FROM [i_facility].[i_facility].[tblpartlearningreport] where HMIID in  (SELECT HMIID FROM [i_facility].[i_facility].[tblworkorderentry] where CorrectedDate >= '" + obj.StartTime.ToString("yyyy-MM-dd") + "'  and CorrectedDate <= '" + obj.EndTime.ToString("yyyy-MM-dd") + "' and MachineID = " + macId + " ); ";

        //            if (obj.FG_code != null && obj.FG_code != "")
        //            {
        //                getWorkOrderIds = Serverdb.tblworkorderentries.Where(m => m.MachineID == macId && m.FGCode == obj.FG_code && m.IsFinished == 1).Where(m => m.WOStart >= obj.StartTime && m.WOEnd <= obj.EndTime).ToList();

        //                //query = "  select * FROM [i_facility].[i_facility].[tblpartlearningreport] where HMIID in  (SELECT HMIID FROM [i_facility].[i_facility].[tblworkorderentry] where Part_No = '" + obj.FG_code + "' and CorrectedDate >= '" + obj.StartTime.ToString("yyyy-MM-dd") + "'  and CorrectedDate <= '" + obj.EndTime.ToString("yyyy-MM-dd") + "' and MachineID = " + macId + " ); ";
        //            }
        //            int count = getWorkOrderIds.Count();
        //            if (count > 0)
        //            {
        //                //DataTable PartData = new DataTable();
        //                //using (MsqlConnection mc = new MsqlConnection())
        //                //{
        //                //    mc.open();
        //                //    MySqlDataAdapter da = new MySqlDataAdapter(query, mc.sqlConnection);
        //                //    da.Fill(PartData);
        //                //    mc.close();
        //                //}
        //                //int countPartData = PartData.Rows.Count;
        //                //if (countPartData == 0)
        //                {
        //                    foreach (var item in getWorkOrderIds)
        //                    {
        //                        var GetDataPre = Serverdb.tblpartlearningreports.Where(m => m.HMIID == item.HMIID).ToList();
        //                        if (GetDataPre.Count == 0)
        //                        {
        //                            int OperatingTime = 0;
        //                            int LossTime = 0;
        //                            int MinorLossTime = 0;
        //                            int MntTime = 0;
        //                            int SetupTime = 0;
        //                            int SetupMinorTime = 0;
        //                            int PowerOffTime = 0;
        //                            long idle = 0;
        //                            decimal loadAndUnload = 0;
        //                            int rejections = 0;
        //                            DateTime ProdStartTime = item.WOStart;
        //                            DateTime ProdEndtime = DateTime.Now;
        //                            try
        //                            {
        //                                if (item.WOEnd.HasValue)
        //                                {
        //                                    ProdEndtime = Convert.ToDateTime(item.WOEnd);
        //                                }
        //                            }
        //                            catch { }

        //                            #region Logic to get the Mode Durations between a Production Order which are completed
        //                            var GetModeDurations = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).ToList();
        //                            foreach (var ModeRow in GetModeDurations)
        //                            {
        //                                if (ModeRow.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec > 600)
        //                                {
        //                                    LossTime += (int)(ModeRow.DurationInSec / 60);
        //                                    int LossDuration = (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "IDLE" && ModeRow.DurationInSec < 600)
        //                                {
        //                                    MinorLossTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(ModeRow.DurationInSec / 60);
        //                                }
        //                                else if (ModeRow.ModeType == "SETUP")
        //                                {
        //                                    try
        //                                    {
        //                                        SetupTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.DurationInSec).First() / 60);
        //                                        SetupMinorTime += (int)(Serverdb.tblsetupmaints.Where(m => m.ModeID == ModeRow.ModeID).Select(m => m.MinorLossTime).First() / 60);
        //                                    }
        //                                    catch { }
        //                                }
        //                            }
        //                            #endregion

        //                            #region Logic to get the Mode Duration Which Was started before this Production and Ended during this Production
        //                            var GetEndModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime < ProdStartTime && m.EndTime > ProdStartTime && m.EndTime < ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
        //                            if (GetEndModeDuration != null)
        //                            {
        //                                if (GetEndModeDuration.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "IDLE")
        //                                {
        //                                    LossTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                    int LossDuration = (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                    //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetEndModeDuration.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(Convert.ToDateTime(GetEndModeDuration.EndTime).Subtract(Convert.ToDateTime(ProdStartTime)).TotalSeconds / 60);
        //                                }
        //                            }
        //                            #endregion

        //                            #region Logic to get the Mode Duration Which Was Started during the Production and Ended after the Production
        //                            var GetStartModeDuration = Serverdb.tblmodes.Where(m => m.MachineID == macId && m.StartTime >= ProdStartTime && m.EndTime >= ProdStartTime && m.StartTime < ProdEndtime && m.EndTime > ProdEndtime && m.IsCompleted == 1 && m.ModeTypeEnd == 1).FirstOrDefault();
        //                            if (GetStartModeDuration != null)
        //                            {
        //                                if (GetStartModeDuration.ModeType == "PROD")
        //                                {
        //                                    OperatingTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "IDLE")
        //                                {
        //                                    LossTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                    int LossDuration = (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                    //insertProdlosses(WOID, LossID, LossDuration, CorrectedDate);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "MNT")
        //                                {
        //                                    MntTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                                else if (GetStartModeDuration.ModeType == "POWEROFF")
        //                                {
        //                                    PowerOffTime += (int)(Convert.ToDateTime(ProdEndtime).Subtract(Convert.ToDateTime(GetStartModeDuration.StartTime)).TotalSeconds / 60);
        //                                }
        //                            }
        //                            #endregion

        //                            int TotlaQty = item.Total_Qty;
        //                            if (TotlaQty == 0)
        //                                TotlaQty = 1;
        //                            int GetOptime = OperatingTime;
        //                            if (GetOptime == 0)
        //                            {
        //                                GetOptime = 1;
        //                            }
        //                            decimal IdealCycleTimeVal = 0;
        //                            //var IdealCycTime = Serverdb.tblparts.Where(m => m.FGCode == item.FGCode && m.OperationNo == item.OperationNo).FirstOrDefault();
        //                            var IdealCycTime = Serverdb.tblmachinedetails.Where(m => m.MachineID == item.MachineID).FirstOrDefault();
        //                            if (IdealCycTime != null)
        //                                IdealCycleTimeVal = (decimal)(IdealCycTime.PlannedCycleTimeInSec / 60);
        //                            double TotalTime = ProdEndtime.Subtract(ProdStartTime).TotalMinutes;
        //                            decimal UtilPercent = (decimal)Math.Round(OperatingTime / TotalTime * 100, 2);
        //                            decimal Quality = (decimal)Math.Round((double)item.Yield_Qty / TotlaQty * 100, 2);
        //                            decimal Performance = (decimal)Math.Round((double)IdealCycleTimeVal * (double)item.Total_Qty / GetOptime * 100, 2);
        //                            int PerformanceFactor = (int)IdealCycleTimeVal * item.Total_Qty;
        //                            //tbl_prodmanmachine PRA = new tbl_prodmanmachine();
        //                            //PRA.MachineID = macId;
        //                            //PRA.WOID = item.HMIID;
        //                            ////PRA.CorrectedDate = CorrectedDate.Date;
        //                            //PRA.TotalLoss = LossTime;
        //                            //PRA.TotalOperatingTime = OperatingTime;
        //                            //PRA.TotalSetup = SetupTime + SetupMinorTime;
        //                            //PRA.TotalMinorLoss = MinorLossTime - SetupMinorTime;
        //                            //PRA.TotalSetupMinorLoss = SetupMinorTime;
        //                            //PRA.TotalPowerLoss = PowerOffTime;
        //                            //PRA.UtilPercent = UtilPercent;
        //                            //PRA.QualityPercent = Quality;
        //                            //PRA.PerformancePerCent = Performance;
        //                            //PRA.PerfromaceFactor = PerformanceFactor;
        //                            //PRA.InsertedOn = DateTime.Now;
        //                            loadAndUnload = MinorLossTime;
        //                            int TotalQty = item.Yield_Qty + item.ScrapQty;
        //                            if (TotalQty == 0)
        //                                TotalQty = 1;
        //                            rejections = Convert.ToInt32((OperatingTime / TotalQty) * item.ScrapQty);

        //                            var GetMainLossList = Serverdb.tbllossescodes.Where(m => m.LossCodesLevel == 1 && m.IsDeleted == 0 && m.MessageType != "SETUP").OrderBy(m => m.LossCodeID).ToList();
        //                            foreach (var LossRow in GetMainLossList)
        //                            {
        //                                var getWoLossList1 = Serverdb.tbl_prodorderlosses.Where(m => m.WOID == item.HMIID && m.LossID == LossRow.LossCodeID).FirstOrDefault();
        //                                if (getWoLossList1 == null)
        //                                {
        //                                    idle = idle + 0;
        //                                }
        //                                else
        //                                {
        //                                    idle = idle + getWoLossList1.LossDuration;
        //                                }
        //                                if (LossRow.LossCode == "LOAD / UNLOAD")
        //                                {
        //                                    if (getWoLossList1 == null)
        //                                    {
        //                                        loadAndUnload = loadAndUnload + 0;
        //                                    }
        //                                    else
        //                                    {
        //                                        loadAndUnload = loadAndUnload + getWoLossList1.LossDuration;
        //                                    }
        //                                }
        //                            }
        //                            var dbParts = Serverdb.tblparts.Where(m => m.FGCode == item.FGCode && m.OperationNo == item.OperationNo).FirstOrDefault();
        //                            decimal idealctime = 0;
        //                            decimal? stdmloss = 0;
        //                            idealctime = IdealCycleTimeVal;
        //                            if (dbParts != null)
        //                            {
        //                                idealctime = dbParts.IdealCycleTime;

        //                                if (dbParts.StdMinorLoss != null)
        //                                {
        //                                    stdmloss = (decimal)dbParts.StdMinorLoss;
        //                                }
        //                                else
        //                                {
        //                                    stdmloss = 0;
        //                                }
        //                            }
        //                            tblpartlearningreport partLearning = new tblpartlearningreport();
        //                            partLearning.MachineID = macId;
        //                            partLearning.HMIID = item.HMIID;
        //                            //partLearning.CorrectedDate = item.CorrectedDate.ToString("yyyy-MM-dd");
        //                            partLearning.CorrectedDate = item.CorrectedDate;
        //                            partLearning.WorkOrderNo = item.Prod_Order_No;
        //                            partLearning.FGCode = item.FGCode;
        //                            partLearning.OpNo = item.OperationNo;
        //                            partLearning.TargetQty = item.ProdOrderQty;
        //                            partLearning.TotalQty = item.Total_Qty;
        //                            partLearning.YieldQty = item.Yield_Qty;
        //                            partLearning.ScrapQty = item.ScrapQty;
        //                            partLearning.SettingTime = SetupTime + SetupMinorTime;
        //                            partLearning.Idle = idle;
        //                            partLearning.MinorLoss = loadAndUnload;
        //                            partLearning.PowerOff = PowerOffTime;
        //                            partLearning.TotalNCCuttingTime = OperatingTime;
        //                            try
        //                            {
        //                                partLearning.AvgCuttingTime = OperatingTime / item.Total_Qty;
        //                            }
        //                            catch
        //                            {
        //                                partLearning.AvgCuttingTime = 0;
        //                            }

        //                            partLearning.StdCycleTime = idealctime;
        //                            partLearning.TotalStdCycleTime = idealctime * item.Total_Qty;
        //                            partLearning.StdMinorLoss = stdmloss;
        //                            partLearning.TotalStdMinorLoss = stdmloss * item.Total_Qty;
        //                            partLearning.InsertedOn = DateTime.Now;
        //                            partLearning.StartTime = obj.StartTime;
        //                            partLearning.EndTime = obj.EndTime;
        //                            Serverdb.tblpartlearningreports.Add(partLearning);
        //                            Serverdb.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //}


        public void setcellcolor(ExcelWorksheet ws, int value, String cell)
        {
            try
            {
                ws.Cells[cell].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (value < 0)
                {
                    ws.Cells[cell].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                }
                else if (value >= 0)
                {
                    ws.Cells[cell].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                }
            }
            catch { }
        }

        public void settextcolor(ExcelWorksheet ws, double value, String cell)
        {
            try
            {
                ws.Cells[cell].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                if (value > 0)
                {
                    ws.Cells[cell].Style.Font.Color.SetColor(Color.DarkRed);
                }
                else if (value <= 0)
                {
                    ws.Cells[cell].Style.Font.Color.SetColor(Color.Green);
                }
            }
            catch { }
        }

        List<string> GetHierarchyData(int MachineID)
        {
            List<string> HierarchyData = new List<string>();
            //1st get PlantName or -
            //2nd get ShopName or -
            //3rd get CellName or -
            //4th get MachineName.

            using (i_facilityEntities1 dbMac = new i_facilityEntities1())
            {
                var machineData = dbMac.tblmachinedetails.Where(m => m.MachineID == MachineID).FirstOrDefault();
                int PlantID = Convert.ToInt32(machineData.PlantID);
                string name = "-";
                name = dbMac.tblplants.Where(m => m.PlantID == PlantID).Select(m => m.PlantName).FirstOrDefault();
                HierarchyData.Add(name);

                string ShopIDString = Convert.ToString(machineData.ShopID);
                int value;
                if (int.TryParse(ShopIDString, out value))
                {
                    name = dbMac.tblshops.Where(m => m.ShopID == value).Select(m => m.ShopName).FirstOrDefault();
                    HierarchyData.Add(name.ToString());
                }
                else
                {
                    HierarchyData.Add("-");
                }

                string CellIDString = Convert.ToString(machineData.CellID);
                if (int.TryParse(CellIDString, out value))
                {
                    name = dbMac.tblcells.Where(m => m.CellID == value).Select(m => m.CellName).FirstOrDefault();
                    HierarchyData.Add(name.ToString());
                }
                else
                {
                    HierarchyData.Add("-");
                }
                // HierarchyData.Add(Convert.ToString(machineData.MachineName));
                HierarchyData.Add(Convert.ToString(machineData.MachineDisplayName));
            }
            return HierarchyData;
        }

        //code to remove major GridLines
        public void RemoveGridLines(ref ExcelChart chartName)
        {
            var chartXml = chartName.ChartXml;
            var nsuri = chartXml.DocumentElement.NamespaceURI;
            var nsm = new XmlNamespaceManager(chartXml.NameTable);
            nsm.AddNamespace("c", nsuri);

            //XY Scatter plots have 2 value axis and no category
            var valAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:valAx", nsm);
            if (valAxisNodes != null && valAxisNodes.Count > 0)
                foreach (XmlNode valAxisNode in valAxisNodes)
                {
                    var major = valAxisNode.SelectSingleNode("c:majorGridlines", nsm);
                    if (major != null)
                        valAxisNode.RemoveChild(major);

                    var minor = valAxisNode.SelectSingleNode("c:minorGridlines", nsm);
                    if (minor != null)
                        valAxisNode.RemoveChild(minor);
                }

            //Other charts can have a category axis
            var catAxisNodes = chartXml.SelectNodes("c:chartSpace/c:chart/c:plotArea/c:catAx", nsm);
            if (catAxisNodes != null && catAxisNodes.Count > 0)
                foreach (XmlNode catAxisNode in catAxisNodes)
                {
                    var major = catAxisNode.SelectSingleNode("c:majorGridlines", nsm);
                    if (major != null)
                        catAxisNode.RemoveChild(major);

                    var minor = catAxisNode.SelectSingleNode("c:minorGridlines", nsm);
                    if (minor != null)
                        catAxisNode.RemoveChild(minor);
                }
        }

        public ActionResult ToolLife()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];

            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");

            return View();
        }

        [HttpPost]
        public ActionResult ToolLife(string PlantID, string ShopID, string CellID, string WorkCenterID, DateTime FromDate, DateTime ToDate, string PrtCode = null)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];
            //DateTime FromDate = DateTime.Now;
            //DateTime ToDate = DateTime.Now;
            //ToolLifeReportExcel(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"), PlantID.ToString(), Convert.ToString(ShopID), Convert.ToString(CellID), Convert.ToString(WorkCenterID), PrtCode);
            ToolLifeReportExcel(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"), PlantID.ToString(), Convert.ToString(ShopID), Convert.ToInt32(CellID), Convert.ToString(WorkCenterID), PrtCode);
            //String RetStatus = ToolLifeReportExcel(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"), PlantID.ToString(), Convert.ToString(ShopID), Convert.ToString(CellID), Convert.ToString(WorkCenterID), PrtCode, OpNo, Part_No, PrtCode);
            int p = Convert.ToInt32(PlantID);
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            // TempData["ToolLifeStatus"] = RetStatus;
            return View();
        }

        //public String ToolLifeReportExcel(string StartDate, string EndDate, string PlantID, string ShopID, string CellID, string WorkCenterID, string PrtCode = null)
        //{
        //    String RetStatus = "Success";

        //    #region Excel and Stuff

        //    DateTime frda = DateTime.Now;
        //    if (string.IsNullOrEmpty(StartDate) == true)
        //    {
        //        StartDate = DateTime.Now.Date.ToString();
        //    }
        //    if (string.IsNullOrEmpty(EndDate) == true)
        //    {
        //        EndDate = StartDate;
        //    }

        //    DateTime frmDate = Convert.ToDateTime(StartDate);
        //    DateTime toDate = Convert.ToDateTime(EndDate);

        //    double TotalDay = toDate.Subtract(frmDate).TotalDays;

        //    FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\ToolLifeMonitoringSheet.xlsx");
        //    ExcelPackage templatep = new ExcelPackage(templateFile);
        //    ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
        //    //ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];

        //    String FileDir = @"C:\I_ShopFloorReports\ReportsList\" + System.DateTime.Now.ToString("yyyy-MM-dd");
        //    bool exists = System.IO.Directory.Exists(FileDir);
        //    if (!exists)
        //        System.IO.Directory.CreateDirectory(FileDir);

        //    FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
        //    if (newFile.Exists)
        //    {
        //        try
        //        {
        //            newFile.Delete();  // ensures we create a new workbook
        //            newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //" to " + toda.ToString("yyyy-MM-dd") + 
        //        }
        //        catch
        //        {
        //            RetStatus = "Excel with same date is already open, please close it and try to generate!!!!";
        //            //return View();
        //        }
        //    }
        //    //Using the File for generation and populating it
        //    ExcelPackage p = null;
        //    p = new ExcelPackage(newFile);
        //    ExcelWorksheet worksheet = null;
        //    //ExcelWorksheet worksheetGraph = null;

        //    //Creating the WorkSheet for populating
        //    try
        //    {
        //        worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
        //        //worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
        //    }
        //    catch { }

        //    if (worksheet == null)
        //    {
        //        worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
        //        //worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), TemplateGraph);
        //    }
        //    int sheetcount = p.Workbook.Worksheets.Count;
        //    p.Workbook.Worksheets.MoveToStart(sheetcount);
        //    worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //    worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //    #endregion

        //    #region MacCount & LowestLevel
        //    string lowestLevel = null;
        //    int MacCount = 0;
        //    int plantId = 0, shopId = 0, cellId = 0, wcId = 0;
        //    if (string.IsNullOrEmpty(WorkCenterID))
        //    {
        //        if (string.IsNullOrEmpty(CellID))
        //        {
        //            if (string.IsNullOrEmpty(ShopID))
        //            {
        //                if (string.IsNullOrEmpty(PlantID))
        //                {
        //                    //donothing
        //                }
        //                else
        //                {
        //                    lowestLevel = "Plant";
        //                    plantId = Convert.ToInt32(PlantID);
        //                    MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == plantId).ToList().Count();
        //                }
        //            }
        //            else
        //            {
        //                lowestLevel = "Shop";
        //                shopId = Convert.ToInt32(ShopID);
        //                MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == shopId).ToList().Count();
        //            }
        //        }
        //        else
        //        {
        //            lowestLevel = "Cell";
        //            cellId = Convert.ToInt32(CellID);
        //            MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellId).ToList().Count();
        //        }
        //    }
        //    else
        //    {
        //        lowestLevel = "WorkCentre";
        //        wcId = Convert.ToInt32(WorkCenterID);
        //        MacCount = 1;
        //    }

        //    #endregion

        //    #region Get Machines List
        //    DataTable machin = new DataTable();
        //    DateTime endDateTime = Convert.ToDateTime(toDate.AddDays(1).ToString("yyyy-MM-dd") + " " + new TimeSpan(6, 0, 0));
        //    string startDateTime = frmDate.ToString("yyyy-MM-dd");
        //    using (MsqlConnection mc = new MsqlConnection())
        //    {
        //        mc.open();
        //        String query1 = null;
        //        if (lowestLevel == "Plant")
        //        {
        //            query1 = " SELECT  distinct MachineID FROM  i_facility.tblmachinedetails WHERE PlantID = " + PlantID + "  and IsNormalWC = 0  and ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
        //        }
        //        else if (lowestLevel == "Shop")
        //        {
        //            query1 = " SELECT * FROM  i_facility.i_facility.tblmachinedetails WHERE ShopID = " + ShopID + "  and IsNormalWC = 0   and  ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
        //        }
        //        else if (lowestLevel == "Cell")
        //        {
        //            query1 = " SELECT * FROM  i_facility.tblmachinedetails WHERE CellID = " + CellID + "  and IsNormalWC = 0  and   ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
        //        }
        //        else if (lowestLevel == "WorkCentre")
        //        {
        //            query1 = "SELECT * FROM  i_facility.tblmachinedetails WHERE MachineID = " + WorkCenterID + "  and IsNormalWC = 0 and((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
        //        }
        //        MySqlDataAdapter da1 = new MySqlDataAdapter(query1, mc.sqlConnection);
        //        da1.Fill(machin);
        //        mc.close();
        //    }
        //    #endregion
        //    List<int> MachineIdList = new List<int>();
        //    foreach (DataRow intItem in machin.Rows)
        //    {
        //        MachineIdList.Add(Convert.ToInt32(intItem["MachineID"].ToString()));
        //    }
        //    DateTime UsedDateForExcel = Convert.ToDateTime(frmDate);
        //    //For each Date ...... for all Machines.
        //    var Col = 'B';
        //    int Row = 9; // Gap to Insert OverAll data. DataStartRow + MachinesCount + 2(1 for HighestLevel & another for Gap).
        //    int Sno = 1;
        //    string finalLossCol = null;
        //    //string existingPartNo = PartsList;

        //    //DataTable for Consolidated Data 

        //    string correctedDate = UsedDateForExcel.ToString("yyyy-MM-dd");

        //        int StartingRowForToday = Row;
        //    //DateTime QueryDate = frmDate.AddDays(i);
        //    DataTable toolData = new DataTable();
        //    using (MsqlConnection mc = new MsqlConnection())
        //    {
        //        mc.open();
        //        //String query = "SELECT wrk.*,tblop.HMIID,tblop.IsReset,tblop.ResetReason,tblop.toollifecounter FROM i_facility.tbltoollifeoperator tblop " +
        //        //    "left outer join i_facility.tblworkorderentry wrk on  tblop.HMIID=wrk.HMIID where wrk.PartNo='" + Part_No +
        //        //    "' and wrk.OperationNo='" + opNo + "' and wrk.MachineID= " + WorkCenterID + " and tblop.ToolCTCode = '" + PrtCode + "' ";
        //        String query = "SELECT wrk.*,tblop.HMIID,tblop.IsReset,tblop.ResetReason,tblop.toollifecounter FROM i_facility.tbltoollifeoperator tblop " +
        //            "left outer join i_facility.tblworkorderentry wrk on tblop.HMIID=wrk.HMIID where wrk.MachineID= " + WorkCenterID + " and tblop.ToolCTCode = '" + PrtCode + "' ";
        //        //query = (from tblop in Serverdb.tbltoollifeoperators
        //        //               join workorder in Serverdb.tblworkorderentries on tblop.HMIID equals workorder.HMIID
        //        //               where workorder.PartNo == Part_No && workorder.OperationNo == opNo && workorder.MachineID == wcId && tblop.ToolCTCode == PrtCode
        //        //               select new { workorder,tblop.HMIID,tblop.IsReset,tblop.ResetReason,tblop.toollifecounter }).ToString();
        //        MySqlDataAdapter da = new MySqlDataAdapter(query, mc.sqlConnection);
        //        da.Fill(toolData);
        //        mc.close();
        //    }
        //    if (toolData.Rows.Count > 0)
        //    {
        //        //string PrtCode = Serverdb.tblstdtoollives.Where(m => m.Part_No == existingPartNo && m.OperationNo == opNo).Select(m => m.PrtCode).FirstOrDefault();


        //        //var Part_NoDet = Serverdb.tblworkorderentries.Where(m => m.FGCode.Trim() == Part_No && m.OperationNo == opNo).FirstOrDefault();
        //        //string drawingNo = Serverdb.tblparts.Where(m => m.FGCode == Part_No.Trim() && m.OperationNo == opNo).Select(m => m.DrawingNo).FirstOrDefault();
        //        int? macId = Convert.ToInt32(WorkCenterID);
        //        string macName = Serverdb.tblmachinedetails.Where(m => m.MachineID == macId).Select(m => m.MachineDisplayName).FirstOrDefault();
        //        //int? stdToolLife = Serverdb.tblstdtoollives.Where(m => m.Part_No == Part_No.Trim() && m.OperationNo == opNo && m.PrtCode == PrtCode).Select(m => m.StdToolLife).FirstOrDefault();
        //        int? stdToolLife = Serverdb.tblstdtoollives.Where(m => m.PrtCode == PrtCode).Select(m => m.StdToolLife).FirstOrDefault();
        //        worksheet.Cells["C4"].Value = PrtCode;
        //      //  worksheet.Cells["C6"].Value = Part_No;
        //        //worksheet.Cells["G6"].Value = Part_NoDet.ProdOrderQty;
        //        //worksheet.Cells["C8"].Value = Part_No.Trim();
        //        //worksheet.Cells["G8"].Value = drawingNo;
        //       worksheet.Cells["H4"].Value = stdToolLife;
        //        //worksheet.Cells["L4"].Value = opNo;
        //        worksheet.Cells["L6"].Value = macName;
        //        int CumulativeValue = 0;
        //        for (int j = 0; j < toolData.Rows.Count; j++)
        //        {
        //            int MachineID = Convert.ToInt32(toolData.Rows[j][1]); //MachineID

        //            string CorrectedDate = Convert.ToString(toolData.Rows[j][14]);//CorrectedDate
        //            DateTime CorrectedDate1 = Convert.ToDateTime(CorrectedDate);
        //            correctedDate = CorrectedDate1.Date.ToString("dd-MM-yyyy");
        //            string shift = Convert.ToString(toolData.Rows[j][5]);//shift

        //            int isreset = Convert.ToInt32(toolData.Rows[j][29]);

        //            string ResetReason = Convert.ToString(toolData.Rows[j][31]);//ResetReason
        //            CumulativeValue += Convert.ToInt32(toolData.Rows[j][32]);
        //            worksheet.Cells["B" + Row].Value = correctedDate;
        //            worksheet.Cells["B" + Row].Style.Numberformat.Format = "yyyy-MM-dd";
        //            worksheet.Cells["C" + Row].Value = toolData.Rows[j][7].ToString();
        //            worksheet.Cells["D" + Row].Value = shift;
        //            worksheet.Cells["E" + Row].Value = Convert.ToInt32(toolData.Rows[j][32]);
        //            worksheet.Cells["H" + Row].Value = CumulativeValue;
        //            if (isreset == 0)
        //            {
        //                worksheet.Cells["K" + Row].Value = "NA";
        //            }
        //            else
        //            {
        //                worksheet.Cells["K" + Row].Value = ResetReason;
        //            }

        //            //string modelRange = "B" + Row + ":M" + Row + "";
        //            worksheet.Cells["E" + Row + ":G" + Row + ""].Merge = true;
        //            worksheet.Cells["H" + Row + ":J" + Row + ""].Merge = true;
        //            worksheet.Cells["K" + Row + ":N" + Row + ""].Merge = true;
        //            //var modelTable = worksheet.Cells[modelRange];
        //            //modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //            //modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            //modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //            //modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            Row++;
        //        }

        //        #region Save and Download

        //        p.Save();

        //        //Downloding Excel
        //        string path1 = System.IO.Path.Combine(FileDir, "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx");
        //        System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
        //        string Outgoingfile = "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx";
        //        if (file1.Exists)
        //        {
        //            Response.Clear();
        //            Response.ClearContent();
        //            Response.ClearHeaders();
        //            Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
        //            Response.AddHeader("Content-Length", file1.Length.ToString());
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.WriteFile(file1.FullName);
        //            Response.Flush();
        //            Response.Close();
        //        }
        //        #endregion
        //    }
        //    else
        //    {
        //        TempData["RetStatus"] = "No data to Generate the report";
        //    }

        //    return RetStatus;
        //}

        //public String ToolLifeReportExcel(string StartDate, string EndDate, string PlantID, string ShopID, string CellID, string WorkCenterID, string PrtCode = null)
        public String ToolLifeReportExcel(string StartDate, string EndDate, string PlantID, string ShopID, int CellID, string WorkCenterID, string PrtCode = null)
        {
            String RetStatus = "Success";

            #region Excel and Stuff
            DateTime frda = DateTime.Now;
            if (string.IsNullOrEmpty(StartDate) == true)
            {
                StartDate = DateTime.Now.Date.ToString();
            }
            if (string.IsNullOrEmpty(EndDate) == true)
            {
                EndDate = StartDate;
            }

            DateTime frmDate = Convert.ToDateTime(StartDate);
            DateTime toDate = Convert.ToDateTime(EndDate);

            double TotalDay = toDate.Subtract(frmDate).TotalDays;

            FileInfo templateFile = new FileInfo(@"C:\I_ShopFloorReports\MainTemplate\ToolLifeMonitoringSheet.xlsx");
            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];
            //ExcelWorksheet TemplateGraph = templatep.Workbook.Worksheets[2];
            // ExcelWorksheet workSheetGraphData = templatep.Workbook.Worksheets[3];

            String FileDir = @"C:\I_FacilityReports\ReportsList\" + System.DateTime.Now.ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx")); //" to " + toda.ToString("yyyy-MM-dd") + 
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    //return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;
            //ExcelWorksheet worksheetGraph = null;

            //Creating the WorkSheet for populating
            try
            {
                //worksheetGraph = p.Workbook.Worksheets.Add("Graphs", TemplateGraph);
                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                //workSheetGraphData = p.Workbook.Worksheets.Add("GraphData", workSheetGraphData);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), Templatews);
                //worksheetGraph = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy"), TemplateGraph);
                //workSheetGraphData = p.Workbook.Worksheets.Add(System.DateTime.Now.ToString("dd-MM-yyyy") + "GraphData", workSheetGraphData);

            }
            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            #endregion

            #region MacCount & LowestLevel
            string lowestLevel = null;
            int MacCount = 0;
            int plantId = 0, shopId = 0, cellId = 0, wcId = 0;
            if (string.IsNullOrEmpty(WorkCenterID))
            {
                if (string.IsNullOrEmpty(Convert.ToString(CellID)))
                {
                    if (string.IsNullOrEmpty(ShopID))
                    {
                        if (string.IsNullOrEmpty(PlantID))
                        {
                            //donothing
                        }
                        else
                        {
                            lowestLevel = "Plant";
                            plantId = Convert.ToInt32(PlantID);
                            MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == plantId).ToList().Count();
                        }
                    }
                    else
                    {
                        lowestLevel = "Shop";
                        shopId = Convert.ToInt32(ShopID);
                        MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == shopId).ToList().Count();
                    }
                }
                else
                {
                    lowestLevel = "Cell";
                    cellId = Convert.ToInt32(CellID);
                    MacCount = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == cellId).ToList().Count();
                }
            }
            else
            {
                lowestLevel = "WorkCentre";
                wcId = Convert.ToInt32(WorkCenterID);
                MacCount = 1;
            }
            #region Get Machines List
            DataTable machin = new DataTable();
            DateTime endDateTime = Convert.ToDateTime(toDate.AddDays(1).ToString("yyyy-MM-dd") + " " + new TimeSpan(6, 0, 0));
            string startDateTime = frmDate.ToString("yyyy-MM-dd");
            using (MsqlConnection mc = new MsqlConnection())
            {
                mc.open();
                String query1 = null;
                if (lowestLevel == "Plant")
                {
                    query1 = " SELECT  distinct MachineID FROM  i_facility.tblmachinedetails WHERE PlantID = " + PlantID + "  and IsNormalWC = 0  and ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                else if (lowestLevel == "Shop")
                {
                    query1 = " SELECT * FROM  i_facility.tblmachinedetails WHERE ShopID = " + ShopID + "  and IsNormalWC = 0   and  ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                else if (lowestLevel == "Cell")
                {
                    query1 = " SELECT * FROM  i_facility.tblmachinedetails WHERE CellID = " + CellID + "  and IsNormalWC = 0  and   ((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                else if (lowestLevel == "WorkCentre")
                {
                    query1 = "SELECT * FROM  i_facility.tblmachinedetails WHERE MachineID = " + WorkCenterID + "  and IsNormalWC = 0 and((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and IsDeleted = 0) or (CASE IsDeleted WHEN 1 THEN  CASE WHEN((InsertedOn <= '" + endDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "') and  (DeletedDate >= '" + startDateTime + "'))  THEN 1 ELSE 0 END END = 1)); ";
                }
                MySqlDataAdapter da1 = new MySqlDataAdapter(query1, mc.sqlConnection);
                da1.Fill(machin);
                mc.close();
            }
            #endregion
            List<int> MachineIdList = new List<int>();
            foreach (DataRow intItem in machin.Rows)
            {
                MachineIdList.Add(Convert.ToInt32(intItem["MachineID"].ToString()));
            }
            DateTime UsedDateForExcel = Convert.ToDateTime(frmDate);
            //For each Date ...... for all Machines.
            var Col = 'B';
            int Row = 11; // Gap to Insert OverAll data. DataStartRow + MachinesCount + 2(1 for HighestLevel & another for Gap).
            int Sno = 1;
            string finalLossCol = null;

            //DataTable for Consolidated Data 


            string correctedDate = UsedDateForExcel.ToString("yyyy-MM-dd");
            PartSearchCreate obj = new PartSearchCreate();
            obj.StartTime = Convert.ToDateTime(Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd 07:00:00"));
            obj.EndTime = Convert.ToDateTime(Convert.ToDateTime(EndDate).AddDays(1).ToString("yyyy-MM-dd 07:00:00"));
            obj.MachineId = MachineIdList;
            obj.correctedDate = correctedDate;
            int BottelneckMachineid = 0;
            PushDataToTblPartLearingReport(obj,out BottelneckMachineid);
            //List<CycleTiemDataGraph> cycleTimeList = new List<CycleTiemDataGraph>();
            for (int i = 0; i < TotalDay + 1; i++)
            {
                DataTable toolData = new DataTable();
                int StartingRowForToday = Row;
                //string dateforMachine = UsedDateForExcel.ToString("yyyy-MM-dd");
                DateTime QueryDate = frmDate.AddDays(i);
                foreach (var macId in MachineIdList)
                {
                    //1) Get distinct partno,WoNo,Opno which are JF
                    //2) Get sum of green, settingTime, etc and push into excel

                    using (MsqlConnection mc = new MsqlConnection())
                    {
                        mc.open();
                        String query = "SELECT wrk.*,tblop.HMIID,tblop.IsReset,tblop.ResetReason,tblop.toollifecounter FROM i_facility.tbltoollifeoperator tblop " + "left outer join i_facility.tblworkorderentry wrk on tblop.HMIID=wrk.HMIID where wrk.CorrectedDate = '" + startDateTime + "' and wrk.MachineID= " + WorkCenterID + " and tblop.ToolCTCode = '" + PrtCode + "' ";
                        MySqlDataAdapter da = new MySqlDataAdapter(query, mc.sqlConnection);
                        da.Fill(toolData);
                        mc.close();
                    }
                }
                if (toolData.Rows.Count > 0)
                {
                    var Part_NoDet = Serverdb.tblworkorderentries.Where(m => m.CorrectedDate == correctedDate).FirstOrDefault();
                    string drawingNo = Serverdb.tblparts.Where(m => m.OperationNo == Part_NoDet.OperationNo).Select(m => m.DrawingNo).FirstOrDefault();
                    int? macId = Convert.ToInt32(WorkCenterID);
                    string macName = Serverdb.tblmachinedetails.Where(m => m.MachineID == macId).Select(m => m.MachineDisplayName).FirstOrDefault();
                    int? stdToolLife = Serverdb.tblstdtoollives.Where(m => m.PrtCode == PrtCode).Select(m => m.StdToolLife).FirstOrDefault();
                    var partno = Serverdb.tblcellparts.Where(m => m.CellID == CellID).Select(m => m.partNo).FirstOrDefault();
                    worksheet.Cells["C4"].Value = PrtCode;
                    worksheet.Cells["L4"].Value = Part_NoDet.OperationNo;
                    worksheet.Cells["C8"].Value = partno;
                    worksheet.Cells["G6"].Value = Part_NoDet.ProdOrderQty;
                    worksheet.Cells["G8"].Value = drawingNo;
                    worksheet.Cells["H4"].Value = stdToolLife;
                    worksheet.Cells["L6"].Value = macName;
                    int CumulativeValue = 0;
                    for (int j = 0; j < toolData.Rows.Count; j++)
                    {
                        int MachineID = Convert.ToInt32(toolData.Rows[j][1]); //MachineID

                        string CorrectedDate = Convert.ToString(toolData.Rows[j][14]);//CorrectedDate
                        DateTime CorrectedDate1 = Convert.ToDateTime(CorrectedDate);
                        correctedDate = CorrectedDate1.Date.ToString("dd-MM-yyyy");
                        string shift = Convert.ToString(toolData.Rows[j][5]);//shift
                        int isreset = Convert.ToInt32(toolData.Rows[j][34]);

                        string ResetReason = Convert.ToString(toolData.Rows[j][35]);//ResetReason
                        CumulativeValue += Convert.ToInt32(toolData.Rows[j][36]);
                        worksheet.Cells["B" + Row].Value = correctedDate;
                        worksheet.Cells["B" + Row].Style.Numberformat.Format = "yyyy-MM-dd";
                        //worksheet.Cells["C" + Row].Value = toolData.Rows[j][7].ToString();
                        worksheet.Cells["C" + Row].Value = shift;
                        worksheet.Cells["E" + Row].Value = Convert.ToInt32(toolData.Rows[j][36]);
                        worksheet.Cells["H" + Row].Value = CumulativeValue;
                        if (isreset == 0)
                        {
                            worksheet.Cells["K" + Row].Value = "NA";
                        }
                        else
                        {
                            worksheet.Cells["K" + Row].Value = ResetReason;
                        }

                        //string modelRange = "B" + Row + ":M" + Row + "";
                        worksheet.Cells["E" + Row + ":G" + Row + ""].Merge = true;
                        worksheet.Cells["H" + Row + ":J" + Row + ""].Merge = true;
                        worksheet.Cells["K" + Row + ":N" + Row + ""].Merge = true;
                        //var modelTable = worksheet.Cells[modelRange];
                        //modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        //modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        //modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        //modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Row++;
                    }

                    #region Save and Download

                    p.Save();

                    //Downloding Excel
                    string path1 = System.IO.Path.Combine(FileDir, "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx");
                    System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
                    string Outgoingfile = "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx";
                    if (file1.Exists)
                    {
                        Response.Clear();
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                        Response.AddHeader("Content-Length", file1.Length.ToString());
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.WriteFile(file1.FullName);
                        Response.Flush();
                        Response.Close();
                    }
                    #endregion
                }
                else
                {
                    TempData["RetStatus"] = "No data to Generate the report";
                }


            }
            return RetStatus;
        }
    }


    public class PartSearchCreate
    {
        public List<int> MachineId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string FG_code { get; set; }
        public string correctedDate { get; set; }
        public int CellID { get; set; }
        public int BottleNeckMachineID { get; set; }

    }

    public class CycleTiemDataGraph
    {
        public string fgcodOpno { get; set; }
        public int YieldQty { get; set; }
        public int ScrapQty { get; set; }
        public double TotalStdTime { get; set; }
        public int ActualTotalOperatingTime { get; set; }
    }
    #endregion
}

