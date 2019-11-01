using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using OfficeOpenXml;

namespace PTR
{
    public class ExcelLib
    {

        public ExcelLib()
        {
       
        }

            
        private Color GetColour(string str)
        {
            if (!string.IsNullOrEmpty(str) && str != "0")
            {
                System.Windows.Media.SolidColorBrush sb = StaticCollections.ColorDictionary[str];
                Color myColor = Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B);
                return myColor;
            }
            else
                return Color.White;
        }

        private Color GetColourFromDesc(string str)
        {
            foreach (Models.ActivityStatusCodesModel a in StaticCollections.ActivityStatusCodes)
                if (a.PlaybookDescription == str)                
                    return GetColour(a.GOM.ID.ToString());
                
            return Color.White;
        }

        public void MakeSalesFunnelReport(DataTable dt, DataTable projectcountdata)
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

                ws.Cells[1,1].Value = "Projects Value";
                //format first row
                for (int i = 0; i < summarydatacolumncount; i++)
                {
                    if (DateTime.TryParse(dt.Columns[i].ColumnName, out newdate))
                    {
                        ws.Cells[firstrowsummarydata, i + 1].Value = newdate.ToString("MMM-yyyy");
                        ws.Column(i + 1).Width = 10;
                    }
                    else
                        ws.Cells[firstrowsummarydata, i + 1].Value = dt.Columns[i].ColumnName;
                    ws.Cells[firstrowsummarydata, i + 1].Style.HorizontalAlignment= OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                //load data
                for (int i = 0; i < summarydatarowcount; i++)                
                    for (int j = 0; j < summarydatacolumncount; j++)
                        ws.Cells[i + firstdatarowsummarydata, j + 1].Value = dt.Rows[i][j];
                
                //colour rows according to status 
                for (int p = firstdatarowsummarydata; p <= lastdatarowsummarydata; p++)
                {
                    var result = ws.Cells["A" + p.ToString()].Value;
                    var newrange = ws.Cells[p, 1, p, summarydatacolumncount];
                    newrange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    newrange.Style.Fill.BackgroundColor.SetColor(GetColour(result.ToString()));
                }

                //Format top row 
                var rangeheader = ws.Cells[firstrowsummarydata, 1, firstrowsummarydata, summarydatacolumncount];
                rangeheader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rangeheader.Style.Font.Color.SetColor(Color.Black);
                rangeheader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Row(firstrowsummarydata).Height = 30;

                //Format grid
                ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, summarydatacolumncount].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, summarydatacolumncount].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, summarydatacolumncount].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, summarydatacolumncount].Style.Border.Top.Color.SetColor(System.Drawing.Color.LightGray);
                               
                //Format numerical data cells
                for (int i = 2; i < summarydatacolumncount + 1; i++)
                {
                    var currencyrange = ws.Cells[firstdatarowsummarydata, i, lastdatarowsummarydata, i];
                    currencyrange.Style.Numberformat.Format = "$ #,##0";

                    var datasalesrange = ws.Cells[firstdatarowsummarydata, i, lastdatarowsummarydata, i];
                    datasalesrange.Style.HorizontalAlignment= OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    ws.Column(i).AutoFit();
                }

                //Format Status column
                var statusrange = ws.Cells[firstrowsummarydata, 1, lastdatarowsummarydata, 1];
                statusrange.Style.HorizontalAlignment= OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                statusrange.Style.Numberformat.Format = "0";
                ws.Column(1).Width = 8;

                //==========================================================================================================
                //ADD PROJECTCOUNT TABLE
                int firstrowprojectcount = 16;
                int firstdatarowprojectcount = firstrowprojectcount + 1;
                int lastdatarowprojectcount = firstrowprojectcount + projectcountdata.Rows.Count;
                int columncountprojectcount = projectcountdata.Columns.Count;

                ws.Cells["A" + (firstrowprojectcount - 1).ToString()].Value = "Number of Projects";

                for (int i = 0; i < columncountprojectcount; i++)
                {
                    if (DateTime.TryParse(projectcountdata.Columns[i].ColumnName, out newdate))
                        ws.Cells[firstrowprojectcount, i + 1].Value = newdate.ToString("MMM-yyyy");
                    else
                        ws.Cells[firstrowprojectcount, i + 1].Value = projectcountdata.Columns[i].ColumnName;

                    ws.Cells[firstrowprojectcount, i + 1].Style.HorizontalAlignment= OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }
                //load data
                for (int i = 0; i < projectcountdata.Rows.Count; i++)                
                    for (int j = 0; j < columncountprojectcount; j++)
                        ws.Cells[i + firstdatarowprojectcount, j + 1].Value = projectcountdata.Rows[i][j];
                
                //colour rows according to status                         
                for (int p = firstdatarowprojectcount; p <= lastdatarowprojectcount; p++)
                {
                    var result = ws.Cells["A" + p.ToString()].Value;  //<<<============parse to int
                    var newrange = ws.Cells[p, 1, p, columncountprojectcount];
                    newrange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    newrange.Style.Fill.BackgroundColor.SetColor(GetColour(result.ToString()));
                }
                //Format top row 
                rangeheader = ws.Cells[firstrowprojectcount, 1, firstrowprojectcount, columncountprojectcount];
                rangeheader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AliceBlue);
                rangeheader.Style.Font.Color.SetColor(System.Drawing.Color.Black);

                ws.Row(firstrowprojectcount).Height = 30;

                //Format grid
                ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, columncountprojectcount].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, columncountprojectcount].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, columncountprojectcount].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, columncountprojectcount].Style.Border.Top.Color.SetColor(System.Drawing.Color.LightGray);

                //Format Status column
                var projectcountstatusrange = ws.Cells[firstrowprojectcount, 1, lastdatarowprojectcount, 1];
                projectcountstatusrange.Style.HorizontalAlignment= OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                projectcountstatusrange.Style.Numberformat.Format = "0";
                
                try
                {
                    string filename = SaveAs(dt.TableName.ToString());
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
             
        public void MakeGenericReport(DataTable dt)
        {
            using (var xl = new ExcelPackage())
            {
                var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());
                int dtcolumncount = dt.Columns.Count;

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;
                    ws.Cells[1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                    for (int j = 0; j < dt.Columns.Count; j++)
                        ws.Cells[i + 2, j + 1].Value = dt.Rows[i][j];

                int firstrow = 1;
                int firstdatarow = firstrow + 1;
                int lastdatarow = dt.Rows.Count + 1;

                //Format top row
                var rangeheader = ws.Cells[1, 1, 1, dtcolumncount];
                rangeheader.Style.Font.Bold = true;
                rangeheader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AliceBlue);
                rangeheader.Style.Font.Color.SetColor(System.Drawing.Color.Black);

                rangeheader.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                rangeheader.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Style.Border.Top.Color.SetColor(System.Drawing.Color.LightGray);
                ws.Row(1).Height = 30;
                
                //Format status column
                int statuscolumn = 0;
                for (int col = 0; col < dt.Columns.Count; col++)
                    if (ws.Cells[1, col + 1].Value.ToString() == "Status")
                        statuscolumn = col+1;

                if (statuscolumn != 0)
                {
                    ws.Column(statuscolumn).Width = 15;
                    for (int p = firstdatarow; p <= lastdatarow; p++)
                    {
                        var cell = ws.Cells[p, statuscolumn].Value;
                        ws.Cells[p, statuscolumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[p, statuscolumn].Style.Fill.BackgroundColor.SetColor(GetColour(cell.ToString()));                             
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dtcolumncount; i++)
                    {
                        ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;

                        if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                            ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                        if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))                        
                            if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                                ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            else
                            if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                                ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            else
                                ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        
                        ws.Column(i + 1).AutoFit();
                    }
                }
                try
                {
                    string filename = SaveAs(dt.TableName.ToString());
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


        public void MakeProjectReport(DataSet ds)
        {
            using (var xl = new ExcelPackage())
            {
                DataTable dt = ds.Tables["Project Report"];

                var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());
                int dtcolumncount = dt.Columns.Count;

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;
                    ws.Cells[1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                    for (int j = 0; j < dt.Columns.Count; j++)
                        ws.Cells[i + 2, j + 1].Value = dt.Rows[i][j];

                int firstrow = 1;
                int firstdatarow = firstrow + 1;
                int lastdatarow = dt.Rows.Count + 1;

                //Format top row
                var rangeheader = ws.Cells[1, 1, 1, dtcolumncount];
                rangeheader.Style.Font.Bold = true;
                rangeheader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rangeheader.Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                rangeheader.Style.Font.Color.SetColor(Color.Black);

                rangeheader.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Style.Border.Right.Color.SetColor(Color.LightGray);
                rangeheader.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                rangeheader.Style.Border.Top.Color.SetColor(Color.LightGray);
                ws.Row(1).Height = 30;                               

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dtcolumncount; i++)
                    {
                        ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;

                        if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                            ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                        if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))                        
                            if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                                ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            else
                            if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                                ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            else
                                ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        
                        ws.Column(i + 1).AutoFit();
                    }
                }

                //make activities table

                DataTable dtactivities = ds.Tables["Activities"];
                int dtactcolumncount = dtactivities.Columns.Count;
                int captionrow = 4;
                int startrow = captionrow + 1;

                for (int i = 0; i < dtactivities.Columns.Count; i++)
                {
                    ws.Cells[startrow, i + 1].Value = dtactivities.Columns[i].Caption;
                    ws.Cells[startrow, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                for (int i = 0; i < dtactivities.Rows.Count; i++)
                    for (int j = 0; j < dtactivities.Columns.Count; j++)
                        ws.Cells[i + startrow, j + 1].Value = dtactivities.Rows[i][j] ?? string.Empty;
                //Format top row
                var actrangeheader = ws.Cells[captionrow, 1, captionrow, dtactcolumncount];
                actrangeheader.Style.Font.Bold = true;
                actrangeheader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                actrangeheader.Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                actrangeheader.Style.Font.Color.SetColor(Color.Black);
                ws.Row(captionrow).Height = 30;

                if (dtactivities.Rows.Count > 0)
                {
                    for (int i = 0; i < dtactcolumncount; i++)
                    {
                        ws.Cells[captionrow, i + 1].Value = dtactivities.Columns[i].Caption;
                        ws.Cells[captionrow, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        if (dtactivities.Columns[i].ExtendedProperties.ContainsKey("Format"))
                            ws.Cells[startrow, i + 1, startrow + dtactivities.Rows.Count, i + 1].Style.Numberformat.Format = dtactivities.Columns[i].ExtendedProperties["Format"].ToString();

                        if (dtactivities.Columns[i].ExtendedProperties.ContainsKey("Alignment"))                        
                            if (dtactivities.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                                ws.Cells[startrow, i + 1, startrow + dtactivities.Rows.Count, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            else
                            if (dtactivities.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                                ws.Cells[startrow, i + 1, startrow + dtactivities.Rows.Count, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            else
                                ws.Cells[startrow, i + 1, startrow + dtactivities.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        
                    }
                }
                
                //Format status column
                int statuscolumn = 2;
                for (int p = startrow; p < startrow + dtactivities.Rows.Count; p++)
                {
                    var cell = ws.Cells[p, statuscolumn].Value;
                    if (!string.IsNullOrEmpty(cell.ToString()))
                    {
                        ws.Cells[p, statuscolumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[p, statuscolumn].Style.Fill.BackgroundColor.SetColor(GetColourFromDesc(cell.ToString()));
                    }
                }
                ws.Column(statuscolumn).AutoFit();               
                ws.Column(statuscolumn + 1).Width = 40;
                var x = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                ws.Cells[captionrow, 1, captionrow + dtactivities.Rows.Count, dtactcolumncount].Style.Border.BorderAround(x);
                
                string projectid = " ID " + dt.Rows[0][0].ToString();

                try
                {
                    string filename = SaveAs(dt.TableName.ToString() + projectid );
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


        private void MakeMasterReportSheet(ExcelPackage xl, DataTable dt)
        {
            var ws = xl.Workbook.Worksheets.Add(dt.TableName.ToString());

            int dtcolumncount = dt.Columns.Count;
            int salesfunnelstatuscol = 0;
            if (dt.Rows.Count > 0)
            {
                //Load data
                for (int i = 0; i < dt.Rows.Count; i++)
                    for (int j = 0; j < dtcolumncount; j++)
                        ws.Cells[i + 2, j + 1].Value = dt.Rows[i][j];

                for (int i = 0; i < dtcolumncount; i++)
                {
                    ws.Cells[1, i + 1].Value = dt.Columns[i].Caption;

                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                        ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.Numberformat.Format = dt.Columns[i].ExtendedProperties["Format"].ToString();

                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))                    
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                            ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        else
                        if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                            ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        else
                            ws.Cells[2, i + 1, dt.Rows.Count + 1, i + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    
                    ws.Column(i + 1).AutoFit();
                    if (dt.Columns[i].ExtendedProperties.Contains("FieldType"))
                        if ((int)dt.Columns[i].ExtendedProperties["FieldType"] == 99)
                        {
                            ws.Cells[1, i + 1].Value = dt.Columns[i].Caption + " Comments";
                            ws.Column(i + 1).Style.WrapText = true;
                            ws.Column(i + 1).AutoFit(20, 20);
                        }
                                        
                    if (dt.Columns[i].ColumnName == "SalesFunnelStage")
                        salesfunnelstatuscol = i + 1;
                }          
             }

            if (salesfunnelstatuscol > 0)
            {
                for (int p = 2; p <= dt.Rows.Count + 1; p++)
                {
                    var cell = ws.Cells[p, salesfunnelstatuscol].Value;
                    ws.Cells[p, salesfunnelstatuscol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[p, salesfunnelstatuscol].Style.Fill.BackgroundColor.SetColor(GetColourFromDesc(cell.ToString()));                        
                }
            }

            //Format top row
            var rangeheader = ws.Cells[1, 1, 1, dtcolumncount];
            rangeheader.Style.Font.Bold = true;
            rangeheader.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            rangeheader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.AliceBlue);
            rangeheader.Style.Font.Color.SetColor(System.Drawing.Color.Black);
            rangeheader.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            ws.Row(1).Height = 30;
            rangeheader.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            rangeheader.Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
            rangeheader.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            rangeheader.Style.Border.Top.Color.SetColor(System.Drawing.Color.LightGray);
        }

        public void MakeMasterProjectReport(DataTable salesfunnel)
        {
            using (var xl = new ExcelPackage())
            {
                MakeMasterReportSheet(xl, salesfunnel);
                try
                {
                    string filename = SaveAs(salesfunnel.TableName.ToString());
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

        public void MakeMasterReport(DataTable salesfunnel, DataTable newbusiness)
        {
            using (var xl = new ExcelPackage())
            {                
                MakeMasterReportSheet(xl, salesfunnel);
                MakeMasterReportSheet(xl, newbusiness);

                try
                {
                    string filename = SaveAs("MasterProjectList");
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


        private string SaveAs(string filename)
        {
            IMessageBoxService dlg = new MessageBoxService();
            Window owner;
            owner = Application.Current.Windows[0];
            string result = dlg.SaveFileDlg("Select File Name to Save As", "Excel Files(*.xlsx)| *.xlsx", filename, owner);
            dlg = null;
            return result;
        }

    }
}
