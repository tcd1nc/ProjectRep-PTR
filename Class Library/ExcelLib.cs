using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using OfficeOpenXml;
using PTR.Models;

namespace PTR
{
    public class ExcelLib
    {
        bool colouriseplaybookreport = true;

        private Dictionary<string, System.Windows.Media.SolidColorBrush> ColorDictionary { get; set; }
        public ExcelLib()
        { }

        private Color GetColourFromString(string col)
        {
            if (!string.IsNullOrEmpty(col))
            {
                System.Windows.Media.SolidColorBrush sb = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(col));
                return Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B);                 
            }
            else
                return Color.White;
        }
        
        private void FormatColumns(DataTable dt, ref ExcelWorksheet ws, int firstrow, Collection<int> excludedcols)
        {
            try
            {
                int dtcolumncount = dt.Columns.Count;
                int dtrowcount = dt.Rows.Count;
                int colctr = 1;
                for (int i = 0; i < dtcolumncount; i++)
                {
                    if (!excludedcols.Contains(i))
                    {
                        if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                            ws.Column(colctr).Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                        if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))
                            if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                                ws.Column(colctr).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            else
                            if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                                ws.Column(colctr).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            else
                                ws.Column(colctr).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        ws.Cells[firstrow, colctr].Value = dt.Columns[i].Caption;
                        ws.Cells[firstrow, colctr].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        ws.Cells[firstrow, colctr].Style.Numberformat.Format = "";
                        ws.Column(colctr).AutoFit();
                        colctr++;
                    }
                }
                var rangeheader = ws.Cells[firstrow, 1, firstrow, dtcolumncount - excludedcols.Count].Style;
                rangeheader.Font.Bold = true;
                rangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rangeheader.Font.Color.SetColor(Color.Black);
                rangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                rangeheader.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Border.Right.Color.SetColor(Color.LightGray);
                rangeheader.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Border.Top.Color.SetColor(Color.LightGray);
                ws.Row(1).Height = 30;
            }
            catch {

            }
        }

        private void FormatSecondTable(DataTable dt, ref ExcelWorksheet ws, int firstrow, int excludecol)
        {
            try
            {
                int dtcolumncount = dt.Columns.Count;
                int dtrowcount = dt.Rows.Count;
                if (dtrowcount > 0)
                {
                    int colctr = 1;
                    for (int i = 0; i < dtcolumncount; i++)
                    {
                        if (i != excludecol)
                        {
                            if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                                ws.Cells[firstrow + 1, colctr, firstrow + dtrowcount, colctr].Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                            if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))
                                if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                                    ws.Cells[firstrow + 1, colctr, firstrow + dtrowcount, colctr].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                                else
                                if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                                    ws.Cells[firstrow + 1, colctr, firstrow + dtrowcount, colctr].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                                else
                                    ws.Cells[firstrow + 1, colctr, firstrow + dtrowcount, colctr].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                            ws.Cells[firstrow, colctr].Value = dt.Columns[i].Caption;
                            ws.Cells[firstrow, colctr].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            ws.Cells[firstrow, colctr].Style.Numberformat.Format = "";
                            ws.Column(colctr).AutoFit();
                            colctr++;
                        }
                    }
                }
            }
            catch {
            }
        }
        
        private void FormatCustomTable(DataTable dt, ref ExcelWorksheet ws, int firstrow)
        {
            try
            {
                int dtcolumncount = dt.Columns.Count;
                int dtrowcount = dt.Rows.Count;                
                for (int i = 0; i < dtcolumncount; i++)
                {                   
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                        ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                            ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        else
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                            ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        else
                            ws.Cells[firstrow + 1, i + 1, firstrow + dtrowcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ws.Cells[firstrow, i + 1].Value = dt.Columns[i].Caption;
                    ws.Cells[firstrow, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[firstrow, i + 1].Style.Numberformat.Format = "";
                    ws.Column(i + 1).AutoFit();                                           
                }
                var rangeheader = ws.Cells[firstrow, 1, firstrow, dtcolumncount].Style;
                rangeheader.Font.Bold = true;
                rangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rangeheader.Font.Color.SetColor(Color.Black);
                rangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                rangeheader.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Border.Right.Color.SetColor(Color.LightGray);
                rangeheader.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Border.Top.Color.SetColor(Color.LightGray);
                ws.Row(1).Height = 30;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in FormatCustomTable", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
        public void MakeSalesPipelineReport(Window winref, DataTable dt, DataTable projectcountdata)
        {
            try
            {
                using (var xl = new ExcelPackage())
                {
                    var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());
                    int dtcolumncount = dt.Columns.Count;

                    DateTime newdate;

                    int summarydatacolumncount = dt.Columns.Count;
                    int summarydatarowcount = dt.Rows.Count;
                    int firstrowsummarydata = 2;
                    int firstdatarowsummarydata = firstrowsummarydata + 1;
                    int lastdatarowsummarydata = summarydatarowcount + firstrowsummarydata;

                    int colourcolumn = 0;
                    ws.Cells[1, 1].Value = dt.TableName;// "Projects Value";
                    //format first row
                    for (int i = 0; i < summarydatacolumncount; i++)
                    {
                        if (DateTime.TryParse(dt.Columns[i].ColumnName, out newdate))
                        {
                            ws.Cells[firstrowsummarydata, i + 1].Value = newdate.ToString("MMM yyyy");
                            ws.Column(i + 1).Width = 10;
                        }
                        else
                            ws.Cells[firstrowsummarydata, i + 1].Value = dt.Columns[i].ColumnName;
                        ws.Cells[firstrowsummarydata, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        if (dt.Columns[i].ColumnName == "StatusColour")
                            colourcolumn = i + 1;

                    }
                    //load data
                    for (int i = 0; i < summarydatarowcount; i++)
                        for (int j = 0; j < summarydatacolumncount; j++)
                            ws.Cells[i + firstdatarowsummarydata, j + 1].Value = dt.Rows[i][j];

                    //colour rows according to status 
                    for (int p = firstdatarowsummarydata; p <= lastdatarowsummarydata; p++)
                    {
                        if (colourcolumn > 0)
                        {
                            var result = ws.Cells["A" + p.ToString()].Value;
                            var newrange = ws.Cells[p, 1, p, summarydatacolumncount];
                            newrange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newrange.Style.Fill.BackgroundColor.SetColor(GetColourFromString(ws.Cells[p, colourcolumn].Value.ToString()));
                        }
                    }

                    //Format top row 
                    var rangeheader = ws.Cells[firstrowsummarydata, 1, firstrowsummarydata, summarydatacolumncount].Style;
                    rangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    rangeheader.Font.Color.SetColor(Color.Black);
                    rangeheader.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    rangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    
                    ws.Row(firstrowsummarydata).Height = 30;

                    //Format grid
                    var gridrowrange = ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, summarydatacolumncount].Style.Border;
                    gridrowrange.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    gridrowrange.Right.Color.SetColor(System.Drawing.Color.LightGray);
                    gridrowrange.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    gridrowrange.Top.Color.SetColor(System.Drawing.Color.LightGray);

                    //Format numerical data cells
                    for (int i = 2; i < summarydatacolumncount + 1; i++)
                    {
                        var currencyrange = ws.Cells[firstdatarowsummarydata, i, lastdatarowsummarydata, i];
                        currencyrange.Style.Numberformat.Format = "$ #,##0";

                        var datasalesrange = ws.Cells[firstdatarowsummarydata, i, lastdatarowsummarydata, i];
                        datasalesrange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        ws.Column(i).AutoFit();
                    }

                    //Format Status column
                    var statusrange = ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, 1];
                    statusrange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    ws.Column(1).AutoFit();

                    //==========================================================================================================
                    //ADD PROJECTCOUNT TABLE
                    int firstrowprojectcount = 16;
                    int firstdatarowprojectcount = firstrowprojectcount + 1;
                    int lastdatarowprojectcount = firstrowprojectcount + projectcountdata.Rows.Count;
                    int columncountprojectcount = projectcountdata.Columns.Count;

                    ws.Cells["A" + (firstrowprojectcount - 1).ToString()].Value = projectcountdata.TableName;// "Number of Projects";

                    for (int i = 0; i < columncountprojectcount; i++)
                    {
                        if (DateTime.TryParse(projectcountdata.Columns[i].ColumnName, out newdate))
                            ws.Cells[firstrowprojectcount, i + 1].Value = newdate.ToString("MMM yyyy");
                        else
                            ws.Cells[firstrowprojectcount, i + 1].Value = projectcountdata.Columns[i].ColumnName;

                        ws.Cells[firstrowprojectcount, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }
                    //load data
                    for (int i = 0; i < projectcountdata.Rows.Count; i++)
                        for (int j = 0; j < columncountprojectcount; j++)
                            ws.Cells[i + firstdatarowprojectcount, j + 1].Value = projectcountdata.Rows[i][j];

                    //colour rows according to status                         
                    for (int p = firstdatarowprojectcount; p <= lastdatarowprojectcount; p++)
                    {
                        if (colourcolumn > 0)
                        {
                            var result = ws.Cells["A" + p.ToString()].Value;
                            var newrange = ws.Cells[p, 1, p, columncountprojectcount];
                            newrange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            newrange.Style.Fill.BackgroundColor.SetColor(GetColourFromString(ws.Cells[p, colourcolumn].Value.ToString()));
                        }
                    }
                    //Format top row 
                    rangeheader = ws.Cells[firstrowprojectcount, 1, firstrowprojectcount, columncountprojectcount].Style;
                    rangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    rangeheader.Font.Color.SetColor(Color.Black);
                    rangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    ws.Row(firstrowprojectcount).Height = 30;

                    //Format grid
                    var projcountrange = ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, columncountprojectcount].Style.Border;
                    projcountrange.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    projcountrange.Right.Color.SetColor(Color.LightGray);
                    projcountrange.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    projcountrange.Top.Color.SetColor(Color.LightGray);

                    //Format Status column
                    var projectcountstatusrange = ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, 1];
                    projectcountstatusrange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                
                    //remove colour column
                    if (colourcolumn > 0)
                        ws.DeleteColumn(colourcolumn);
                  
                    try
                    {
                        string filename = SaveAs(winref, Constants.SalesPipelineReportName);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch(Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeSalesPipelineReport", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
        public void MakeCustomReport(Window winref, DataSet ds, CustomReportModel reportdetails)
        {
            try
            {
                using (var xl = new ExcelPackage())
                {
                    ExcelWorksheet ws;
                    ws = xl.Workbook.Worksheets.Add(reportdetails.Name);

                    int firstrow = 1;
                    int firstdatarow = firstrow + 1;
                    int lastdatarow = firstrow;
                    int dtcolumncount = 0;
                    int dtrowcount = 0;
                    int tblctr = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (!reportdetails.CombineTables)
                        {
                            if (tblctr == 0)
                                ws.Name = dt.TableName;
                            else
                                ws = xl.Workbook.Worksheets.Add(dt.TableName);
                        }

                        tblctr++;
                        dtcolumncount = dt.Columns.Count;
                        dtrowcount = dt.Rows.Count;

                        for (int i = 0; i < dtrowcount; i++)
                            for (int j = 0; j < dtcolumncount; j++)
                                ws.Cells[firstrow + i + 1, j + 1].Value = dt.Rows[i][j];

                        FormatCustomTable(dt, ref ws, firstrow);

                        if (reportdetails.CombineTables)
                        {
                            firstrow = dtrowcount + firstrow + 2; //2 rows between tables
                            firstdatarow = firstrow + 1;
                        }
                    }

                    try
                    {
                        string filename = SaveAs(winref, reportdetails.Name);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }               
            }
            catch 
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeCustomReport","Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
        public void MakeGenericReport(Window winref,  DataTable dt)
        {
            try
            {
                using (var xl = new ExcelPackage())
                {
                    var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());
                    int dtcolumncount = dt.Columns.Count;
                    int dtrowcount = dt.Rows.Count;
                                      
                    int firstrow = 1;
                    int firstdatarow = firstrow + 1;
                    int lastdatarow = dtrowcount + 1;
                    
                    int statuscolumn = -1;
                    int colourcolumn = -1;
                    int projecttypecolourcol = -1;
                    Collection<int> excludedcols = new Collection<int>();
                    
                    for (int col = 0; col < dtcolumncount; col++)
                    {
                        if (dt.Columns[col].ColumnName == "StatusColour")
                        {
                            colourcolumn = col;
                            excludedcols.Add(col);
                        }
                        else
                        if (dt.Columns[col].ColumnName == "ProjectTypeColour")
                        {
                            projecttypecolourcol = col;
                            excludedcols.Add(col);                           
                        }
                        else
                        {
                            if (dt.Columns[col].ColumnName == "SalesFunnelStage" || dt.Columns[col].ColumnName == "Status")
                            {
                                statuscolumn = col + 1;
                                if (colourcolumn != -1)
                                    statuscolumn--;
                                if (projecttypecolourcol != -1)
                                    statuscolumn--;

                            }                                                     
                        }
                    }
                                    
                    FormatColumns(dt, ref ws, 1, excludedcols);

                    int colctr = 1;
                    for (int i = 0; i < dtrowcount; i++)
                    {
                        colctr = 1;
                        for (int j = 0; j < dtcolumncount; j++)
                            if (!excludedcols.Contains(j))
                            {
                                if (projecttypecolourcol != -1)
                                {
                                    var cell = ws.Cells[i + 2, colctr].Style.Fill;
                                    cell.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    cell.BackgroundColor.SetColor(GetColourFromString(dt.Rows[i][projecttypecolourcol].ToString()));
                                    ws.Cells[i + 2, colctr].Value = dt.Rows[i][j];
                                }
                                else
                                    ws.Cells[i + 2, colctr].Value = dt.Rows[i][j];
                                colctr++;
                            }
                    }

                    if (statuscolumn > -1 && colourcolumn > -1)
                    {                        
                        for (int p = firstdatarow; p <= lastdatarow; p++)
                        {
                            var cell = ws.Cells[p, statuscolumn].Style.Fill;
                            cell.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                                                   
                            cell.BackgroundColor.SetColor(GetColourFromString(dt.Rows[p - firstdatarow][colourcolumn].ToString()));
                        }
                    }
                 
                    for(int g = 1; g < colctr; g++)
                        ws.Column(g).AutoFit();

                    try
                    {
                        string filename = SaveAs(winref, dt.TableName.ToString());
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeGenericReport", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
        public void MakeProjectReport(Window winref, DataSet ds)
        {
            try
            {
                using (var xl = new ExcelPackage())
                {
                    DataTable dt = ds.Tables[Constants.SingleProjectReport];

                    var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());
                    int dtcolumncount = dt.Columns.Count;
                    int dtrowcount = dt.Rows.Count;

                    for (int i = 0; i < dtrowcount; i++)
                        for (int j = 0; j < dtcolumncount; j++)
                            ws.Cells[i + 2, j + 1].Value = dt.Rows[i][j];

                    int firstrow = 1;
                    int firstdatarow = firstrow + 1;
                    int lastdatarow = dt.Rows.Count + 1;
                    
                    FormatColumns(dt, ref ws, 1, new Collection<int>());                                     

                    //make activities table                                       
                    DataTable dtactivities = ds.Tables[Constants.SingleProjectReportActivities];
                    if (dtactivities.Columns.Count > 0 && dtactivities.Rows.Count > 0)
                    {
                        int dtactcolumncount = dtactivities.Columns.Count;
                        int captionrow = 4;
                        int startrow = captionrow + 1;
                        int colourcolumnindex = 0;

                        for (int i = 0; i < dtactivities.Columns.Count; i++)
                        {
                            if (dtactivities.Columns[i].Caption == "StatusColour")
                                colourcolumnindex = i;
                            else
                            {
                                ws.Cells[startrow, i + 1].Value = dtactivities.Columns[i].Caption;
                                ws.Cells[startrow, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            }                            
                        }

                        for (int i = 0; i < dtactivities.Rows.Count; i++)
                            for (int j = 0; j < dtactivities.Columns.Count; j++)
                                if(j != colourcolumnindex)
                                    ws.Cells[i + startrow, j + 1].Value = dtactivities.Rows[i][j] ?? string.Empty;

                        //Format top row
                        var actrangeheader = ws.Cells[captionrow, 1, captionrow, dtactcolumncount-1].Style;
                        actrangeheader.Font.Bold = true;
                        actrangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        actrangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                        actrangeheader.Font.Color.SetColor(Color.Black);
                        actrangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        ws.Row(captionrow).Height = 30;

                        FormatSecondTable(dtactivities, ref ws, startrow - 1, colourcolumnindex);

                        //Format status column
                        int statuscolumn = 2;
                        for (int p = startrow; p < startrow + dtactivities.Rows.Count; p++)
                        {
                            var cell = ws.Cells[p, statuscolumn].Value;
                            if (!string.IsNullOrEmpty(cell.ToString()))
                            {
                                ws.Cells[p, statuscolumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                ws.Cells[p, statuscolumn].Style.Fill.BackgroundColor.SetColor(GetColourFromString(dtactivities.Rows[p-startrow][colourcolumnindex].ToString()));
                            }
                            //Format status month rows
                            ws.Cells[p, 1].Style.Numberformat.Format = "MMM-yyy";
                            ws.Cells[p, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        ws.Column(statuscolumn).AutoFit();
                        ws.Column(statuscolumn + 1).Width = 40;
                        ws.Cells[captionrow, 1, captionrow + dtactivities.Rows.Count, dtactcolumncount-1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
                    }                                                                           
                    string projectid = " ID " + dt.Rows[0][0].ToString();
                    try
                    {
                        string filename = SaveAs(winref, dt.TableName.ToString() + projectid);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeProjectReport", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }
        
        private void MakeMasterReportSheet(ExcelPackage xl, DataTable dt)
        {
            try
            {
                var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());

                int dtcolumncount = dt.Columns.Count;
                int dtrowcount = dt.Rows.Count;
                int salesfunnelstatuscol = 0;
                int colourcol = 0;
                int projecttypecolourcol = 0;               
              
                //Load data
                for (int i = 0; i < dtrowcount; i++)
                    for (int j = 0; j < dtcolumncount; j++)
                        ws.Cells[i + 2, j + 1].Value = dt.Rows[i][j];

                for (int i = 0; i < dtcolumncount; i++)
                {                        
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                        ws.Column(i + 1).Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();
                        
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                            ws.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        else
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                            ws.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        else
                            ws.Column(i + 1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;

                    ws.Column(i + 1).AutoFit();
                    if (dt.Columns[i].ExtendedProperties.Contains("FieldType"))
                    {
                        if ((int)dt.Columns[i].ExtendedProperties["FieldType"] == (int)ReportFieldType.PlaybookComments)
                        {
                            ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;
                            ws.Column(i + 1).Style.WrapText = true;
                            ws.Column(i + 1).AutoFit(20, 20);
                        }                         
                    }

                    if (dt.Columns[i].ColumnName == "SalesFunnelStage")
                        salesfunnelstatuscol = i + 1;

                    if (dt.Columns[i].ColumnName == "StatusColour")
                        colourcol = i + 1;
                        
                    if (dt.Columns[i].ColumnName == "ProjectTypeColour")
                        projecttypecolourcol = i + 1;
                }

                if (colouriseplaybookreport)
                {
                    for (int p = 0; p < dtrowcount; p++)
                    {
                        if (projecttypecolourcol > 0)
                        {
                            var rowrange = ws.Cells[p + 2, 1, dtrowcount + 1, dtcolumncount].Style.Fill;
                            rowrange.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            rowrange.BackgroundColor.SetColor(GetColourFromString(ws.Cells[p + 2, projecttypecolourcol].Value.ToString()));
                        }

                        if (colourcol > 0)
                        {
                            if (salesfunnelstatuscol > 0)
                            {
                                var cell = ws.Cells[p + 2, salesfunnelstatuscol].Style.Fill;
                                cell.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.BackgroundColor.SetColor(GetColourFromString(ws.Cells[p + 2, colourcol].Value.ToString()));
                            }
                        }
                    }
                }


                if (colourcol > 0)
                {
                    ws.DeleteColumn(colourcol);
                    dtcolumncount--;
                }

                string projectTypeColourCaption = string.Empty;
                if (dt.Columns.Contains("ProjectTypeColour"))
                {
                    int idx = dt.Columns.IndexOf("ProjectTypeColour");
                    projectTypeColourCaption = dt.Columns[idx].Caption;
                }

                for (int j = 0; j < dtcolumncount; j++)                                    
                    if (ws.Cells[1, j + 1].Value.ToString() == projectTypeColourCaption)
                    {
                        ws.DeleteColumn(j + 1);
                        dtcolumncount--;
                        break;
                    }
                
                //Format top row
                var rangeheader = ws.Cells[1, 1, 1, dtcolumncount].Style;                
                rangeheader.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                rangeheader.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                if (colouriseplaybookreport)
                {
                    rangeheader.Font.Bold = true;
                    rangeheader.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rangeheader.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    rangeheader.Font.Color.SetColor(Color.Black);                                
                    rangeheader.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    rangeheader.Border.Right.Color.SetColor(Color.LightGray);
                    rangeheader.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    rangeheader.Border.Top.Color.SetColor(Color.LightGray);
                }  
                ws.Row(1).Height = 30;

                for(int n = 0; n < dtrowcount; n++)
                {
                    ws.Row(n + 2).Height = 15;                    
                }
            }
            catch
            {

            }
        }

        public void MakePlaybookReport(Window winref, DataTable salesfunnel,  bool ColourisePlaybookReport)
        {
            try
            {
                colouriseplaybookreport = ColourisePlaybookReport;
                using (var xl = new ExcelPackage())
                {
                    MakeMasterReportSheet(xl, salesfunnel);
                    //MakeMasterReportSheet(xl, newbusiness);
                    try
                    {
                        string filename = SaveAs(winref, Constants.PlayBookReportName);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeMasterProjectReport", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        public void MakeMasterReport(Window winref, DataTable salesfunnel,  bool ColourisePlaybookReport)
        {
            try
            {
                colouriseplaybookreport = ColourisePlaybookReport;
                using (var xl = new ExcelPackage())
                {
                    MakeMasterReportSheet(xl, salesfunnel);
                    //MakeMasterReportSheet(xl, newbusiness);
                    try
                    {
                        string filename = SaveAs(winref, Constants.MasterProjectReportName);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            xl.SaveAs(new System.IO.FileInfo(filename));
                            Process.Start(filename);
                        }
                    }
                    catch (Exception e)
                    {
                        IMessageBoxService msg = new MessageBoxService();
                        msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                        msg = null;
                    }
                }
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("An unexpected error has occurred in MakeMasterReport", "Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        private string SaveAs(Window winref, string filename)
        {
            IMessageBoxService dlg = new MessageBoxService();
            //Window owner;
            if(winref == null)
                winref = Application.Current.Windows[0];
            string result = dlg.SaveFileDlg("Select File Name to Save As", "Excel Files(*.xlsx)| *.xlsx", filename, winref);
            dlg = null;
            return result;
        }

    }
}
