using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace PTR
{
    public class CreateExcelFile : IDisposable
    {
          
        /// <summary>
        /// Report all status totals for each Activity Status by month
        /// </summary>
        /// <param name="dt"></param>
        public void zCreateExcelSummaryReport(DataTable dt, DataTable projectcountdata)
        {
            XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled);
            workbook.Worksheets.Add("TotalData");

            var worksheet = workbook.Worksheet("TotalData");
            DateTime _newdate;

            int _summarydatacolumncount = dt.Columns.Count;
            int _summarydatarowcount = dt.Rows.Count;
            int _firstrowsummarydata = 2;
            int _firstdatarowsummarydata = _firstrowsummarydata + 1;
            int _lastdatarowsummarydata = _summarydatarowcount + _firstrowsummarydata;
            worksheet.Cell("A" + (_firstrowsummarydata - 1).ToString()).Value = "Projects Value";
            //format first row
            for (int i = 0; i < _summarydatacolumncount; i++)
            {
                if (DateTime.TryParse(dt.Columns[i].ColumnName, out _newdate))
                {
                    worksheet.Cell(_firstrowsummarydata, (i + 1)).Value = "'" + _newdate.ToString("MMM-yyyy");
                    worksheet.Column(i + 1).Width = 10;
                }
                else
                    worksheet.Cell(_firstrowsummarydata, (i + 1)).Value = dt.Columns[i].ColumnName;
                worksheet.Cell(_firstrowsummarydata, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            //load data
            for (int i = 0; i < _summarydatarowcount; i++)
            {
                for (int j = 0; j < _summarydatacolumncount; j++)
                    worksheet.Cell(i + _firstdatarowsummarydata, j + 1).Value = dt.Rows[i][j];                
            }

            XLColor _xlcolor;
            Collection<Models.ActivityStatusCodesModel> _statuscodes = StaticCollections.ActivityStatusCodes;// DatabaseQueries.GetActivityStatusCodes();
            Dictionary<string, XLColor> dictionary = new Dictionary<string, XLColor>();
            foreach (Models.ActivityStatusCodesModel ac in _statuscodes)
            {
                dictionary.Add(ac.GOM.Name, XLColor.FromHtml(ac.Colour));
            }
            //colour rows according to status 
            for (int p = _firstdatarowsummarydata; p <= _lastdatarowsummarydata; p++)
            {
                var result = worksheet.Cell("A" + p.ToString()).Value;
                if(!string.IsNullOrEmpty(result.ToString()))
                {
                    var newrange = worksheet.Range(p, 1, p, _summarydatacolumncount);
                    _xlcolor = dictionary[result.ToString()];
                    newrange.Style.Fill.BackgroundColor = _xlcolor;
                }
                else
                {
                    var _Color = XLColor.White;
                    var newrange = worksheet.Range(p, 1, p, _summarydatacolumncount);
                    newrange.Style.Fill.BackgroundColor = _Color;
                }
            }
            //Format top row 
            var range = worksheet.Range(_firstrowsummarydata, 1, _firstrowsummarydata, _summarydatacolumncount);
            range.Style.Fill.BackgroundColor = XLColor.Blue;
            range.Style.Font.FontColor = XLColor.White;
            worksheet.Row(_firstrowsummarydata).Height = 30;

            //Format numerical data cells
            for (int i = 2; i < _summarydatacolumncount + 1; i++)
            {
                    var currencyrange = worksheet.Range(_firstdatarowsummarydata, i, _lastdatarowsummarydata, i);
                    currencyrange.Style.NumberFormat.Format = "$ #,##0";

                    var datasalesrange = worksheet.Range(_firstdatarowsummarydata, i, _lastdatarowsummarydata, i);
                    datasalesrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }
            //Format Status column
            var statusrange = worksheet.Range(_firstrowsummarydata, 1, _lastdatarowsummarydata, 1);
            statusrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column(1).Width = 8;

            //==========================================================================================================
            //ADD PROJECTCOUNT TABLE
            int _firstrowprojectcount = 16;
            int _firstdatarowprojectcount = _firstrowprojectcount+1;
            int _lastdatarowprojectcount = _firstrowprojectcount + projectcountdata.Rows.Count;
            int _columncountprojectcount = projectcountdata.Columns.Count;

            worksheet.Cell("A" + (_firstrowprojectcount - 1).ToString()).Value = "Number of Projects";
            
            for (int i = 0; i < _columncountprojectcount; i++)
            {
                if (DateTime.TryParse(projectcountdata.Columns[i].ColumnName, out _newdate))
                    worksheet.Cell(_firstrowprojectcount, (i + 1)).Value = "'" + _newdate.ToString("MMM-yyyy");
                else
                    worksheet.Cell(_firstrowprojectcount, (i + 1)).Value = projectcountdata.Columns[i].ColumnName;

                worksheet.Cell(_firstrowprojectcount, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            //load data
            for (int i = 0; i < projectcountdata.Rows.Count; i++)
            {
                for (int j = 0; j < _columncountprojectcount; j++)
                     worksheet.Cell(i + _firstdatarowprojectcount, j + 1).Value = projectcountdata.Rows[i][j];                
            }
            
            //colour rows according to status                         
            for (int p = _firstdatarowprojectcount; p <= _lastdatarowprojectcount; p++)
            {
                var result = worksheet.Cell("A" + p.ToString()).Value;
                if (!string.IsNullOrEmpty(result.ToString()))
                {
                    var newrange = worksheet.Range(p, 1, p, _columncountprojectcount);
                    _xlcolor = dictionary[result.ToString()];
                    newrange.Style.Fill.BackgroundColor = _xlcolor;
                }
                else
                {
                    var _Color = XLColor.White;
                    var newrange = worksheet.Range(p, 1, p, _columncountprojectcount);
                    newrange.Style.Fill.BackgroundColor = _Color;
                }
            }
            //Format top row 
            range = worksheet.Range(_firstrowprojectcount, 1, _firstrowprojectcount, _columncountprojectcount);
            range.Style.Fill.BackgroundColor = XLColor.Blue;
            range.Style.Font.FontColor = XLColor.White;
            worksheet.Row(_firstrowprojectcount).Height = 30;
            //Format Status column
            var projectcountstatusrange = worksheet.Range(_firstrowprojectcount, 1, _lastdatarowprojectcount, 1);
            projectcountstatusrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            SaveWorkbook(ref workbook);
       
        }

        /// <summary>
        /// List of Projects meeting specific status and selection criteris
        /// </summary>
        /// <param name="dt"></param>
        public void zCreateStatusReport(DataTable dt)
        {
            XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled);
            workbook.Worksheets.Add("StatusReportData");
            var worksheet = workbook.Worksheet("StatusReportData");

            for (int i = 0; i < dt.Columns.Count; i++)
            {                
                    worksheet.Cell(1, (i + 1)).Value = dt.Columns[i].Caption;
                    worksheet.Cell(1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;                
            }

            for (int i = 0; i < dt.Rows.Count; i++)                
                for (int j = 0; j < dt.Columns.Count; j++)
                    worksheet.Cell(i + 2, j + 1).Value = dt.Rows[i][j];

            XLColor _xlcolor;
            //Collection<Models.ActivityStatusCodesModel> _statuscodes = StaticCollections.ActivityStatusCodes;
            Dictionary<string, XLColor> dictionary = new Dictionary<string, XLColor>();
            foreach (Models.ActivityStatusCodesModel ac in StaticCollections.ActivityStatusCodes)            
                dictionary.Add(ac.GOM.Name, XLColor.FromHtml(ac.Colour));
            
            var range = worksheet.RangeUsed();
            int firstrow = range.FirstRowUsed().RowNumber();
            int firstdatarow = firstrow + 1;
            int lastdatarow = range.RowCount() + 1;

            //Format top row
            range.FirstRowUsed().Style.Fill.BackgroundColor = XLColor.Blue;
            range.FirstRowUsed().Style.Font.FontColor = XLColor.White;
            worksheet.Rows(firstrow, firstrow).Height = 30;

            //Format data range background
            var datarange = worksheet.Range(firstdatarow, 1, range.RowCount(), 11);
            datarange.Style.Fill.BackgroundColor = XLColor.AliceBlue;

            //Format status column
            int _statuscolumn = 0;
            for (int col = 0; col < range.ColumnCount(); col++)
                if (worksheet.Cell(GetExcelColumnName(col) + "1").Value.ToString() == "Status")
                    _statuscolumn = col;

            string _statuscol = GetExcelColumnName(_statuscolumn);
            if (_statuscolumn != 0)
            {
                worksheet.Column(_statuscol).Width = 15;
                for (int p = firstdatarow; p <= lastdatarow; p++)
                {
                    var _cell = worksheet.Cell(_statuscol + p.ToString());
                    if (!string.IsNullOrEmpty(_cell.Value.ToString()))
                    {
                        _xlcolor = dictionary[worksheet.Cell(_statuscol + p.ToString()).Value.ToString()];
                        worksheet.Cell(p, _statuscolumn + 1).Style.Fill.BackgroundColor = _xlcolor;
                    }
                }
            }
            var statusrange = worksheet.Range(firstdatarow, _statuscolumn + 1, range.RowCount(), _statuscolumn + 1);
            statusrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //Format ID, Status, FirstMonth columns - set alignment
            var idrange = worksheet.Range(firstrow, 1, range.RowCount(), 1);
            idrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column(1).Width = 10;

            //format division
            worksheet.Column(2).Width = 15;
            
            //Format Project column
            worksheet.Column(3).Width = 45;

            //Format Customer column
            worksheet.Column(4).Width = 35;

            //Format currency data cells
            var currencyrange = worksheet.Range(firstdatarow, 5, range.RowCount(), 5);
            currencyrange.Style.NumberFormat.Format = "$ #,##0";
            currencyrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Column(5).Width = 15;
            worksheet.Cell(firstrow, 5).Value = "Targeted Sales";

            //Format number of months column
            worksheet.Column(7).Width = 18;
            worksheet.Column(7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var nummonthrange = worksheet.Range(firstdatarow, 7, range.RowCount(), 7);
            nummonthrange.Style.NumberFormat.Format = "0";

            worksheet.Column(8).Width = 25;
            worksheet.Column(8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var firstmonthrange = worksheet.Range(firstdatarow, 8, range.RowCount(), 8);
            firstmonthrange.Style.NumberFormat.Format = "MMM-yy";

            worksheet.Column(9).Width = 15;
            worksheet.Column(9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Column(11).Width = 15;
            worksheet.Column(11).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            string _culturecodecol = GetExcelColumnName(9);
            worksheet.Column(_culturecodecol).Delete();

            SaveWorkbook(ref workbook);
 
        }

        /// <summary>
        /// Similar to CreateStatusReport but with no Status information
        /// </summary>
        /// <param name="_projects"></param>
        public void zCreateProjectReport(FullyObservableCollection<Models.ProjectReportSummary> _projects)
        {
            ClosedXML.Excel.XLWorkbook workbook;
            workbook = new XLWorkbook(XLEventTracking.Disabled);            
            workbook.Worksheets.Add("ProjectListReport");
            var worksheet = workbook.Worksheet("ProjectListReport");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Status";
            worksheet.Cell(1, 3).Value = "Project";
            worksheet.Cell(1, 4).Value = "Customer";
            worksheet.Cell(1, 5).Value = "Targeted Sales";
            worksheet.Cell(1, 6).Value = "Associate";

            for (int i = 0; i < _projects.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = _projects[i].ID;
                worksheet.Cell(i + 2, 2).Value = _projects[i].ProjectStatus;
                worksheet.Cell(i + 2, 3).Value = _projects[i].ProjectName;
                worksheet.Cell(i + 2, 4).Value = _projects[i].Customer;
                worksheet.Cell(i + 2, 5).Value = _projects[i].EstimatedAnnualSales;
                worksheet.Cell(i + 2, 6).Value = _projects[i].Associate;
            }

            var rangeheader = worksheet.Range(1, 1, 1, 6);
            rangeheader.Style.Fill.BackgroundColor = XLColor.Blue;
            rangeheader.Style.Font.FontColor = XLColor.White;
            rangeheader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //Set data range background
            var datarange = worksheet.Range(2, 1, _projects.Count+1, 6);
            datarange.Style.Fill.BackgroundColor = XLColor.AliceBlue;

            worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //Format currency data cells
            var currencyrange = worksheet.Range(2, 5, _projects.Count, 5);
            currencyrange.Style.NumberFormat.Format = "$ #,##0";
            currencyrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Column(5).Width = 15;

            var textrange = worksheet.Range(2, 2, _projects.Count, 4);
            textrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Column(2).Width = 25;
            worksheet.Columns("3:4").Width = 25;

            var assocrange = worksheet.Range(2, 6, _projects.Count, 6);
            assocrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Column(6).Width = 25;

            SaveWorkbook(ref workbook);

        }          
                       
        public void zCreateProjectSummaryReport(DataTable dt)
        {
            XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled);
            workbook.Worksheets.Add(dt.TableName.ToString());

            var worksheet = workbook.Worksheet(dt.TableName.ToString());
            int dtcolumncount = dt.Columns.Count;

            for (int i = 0; i < dtcolumncount; i++)
            {
                worksheet.Cell(1, (i + 1)).Value = dt.Columns[i].Caption;
                  
                if (dt.Columns[i].DataType.Equals(typeof(decimal)))
                {
                    worksheet.Range(2, i+1, dt.Rows.Count + 1, i+1).Style.NumberFormat.Format = "$ #,##0";
                    worksheet.Range(2, i+1, dt.Rows.Count + 1, i+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                if (dt.Columns[i].DataType.Equals(typeof(int)))                
                    worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                else
                    if (dt.Columns[i].DataType.Equals(typeof(bool)))                                   
                        worksheet.Range(2, i+1, dt.Rows.Count + 1, i+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;                    
                    else                    
                        worksheet.Range(2, i+1, dt.Rows.Count + 1, i+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    
            }

            for (int i = 0; i < dt.Rows.Count; i++)            
                for (int j = 0; j < dtcolumncount; j++)
                    worksheet.Cell(i + 2, j + 1).Value = dt.Rows[i][j];
            
            string _newcolor = string.Empty;

            var range = worksheet.Range(1, 1, dt.Rows.Count, dtcolumncount);
            int firstrow = range.FirstRow().RowNumber();
            int firstdatarow = firstrow + 1;
            int lastdatarow = range.RowCount() + 1;

            //Format data range background
            var datarange = worksheet.Range(firstdatarow, 1, lastdatarow, dtcolumncount);
            datarange.Style.Fill.BackgroundColor = XLColor.AliceBlue;

            //Format top row
            var rangeheader = worksheet.Range(1, 1, 1, range.ColumnCount());
            rangeheader.Style.Fill.BackgroundColor = XLColor.Blue;
            rangeheader.Style.Font.FontColor = XLColor.White;
            rangeheader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Row(1).Height = 30;

            string _colname = string.Empty;
            string _prevcolname = string.Empty;
            string _total = string.Empty;

            //Format Status column
            var statusrange = worksheet.Range(firstrow, 1, lastdatarow, 1);
            statusrange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column(1).Width = 8;

            worksheet.Columns().AdjustToContents();
            SaveWorkbook(ref workbook);
     
        }

        public void CreatePlaybookReportSheet(ref XLWorkbook workbook, DataTable dt, Dictionary<string, string> _highlightedprojectstatuses = null)
        {
            //workbook.Worksheets.Add(dt.TableName.ToString());
            //var worksheet = workbook.Worksheet(dt.TableName.ToString());

            IXLWorksheet worksheet = workbook.Worksheets.Add(dt.TableName.ToString());

            int dtcolumncount = dt.Columns.Count;
            int _salesfunnelstatuscol = 0;
            for (int i = 0; i < dtcolumncount; i++)
            {
                worksheet.Cell(1, (i + 1)).Value = dt.Columns[i].Caption;

                if (dt.Columns[i].DataType.Equals(typeof(decimal)))                
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))                                           
                        worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.NumberFormat.Format = ConvertStringFormatToXL(dt.Columns[i].ExtendedProperties["Format"].ToString());                    
                
                if (dt.Columns[i].DataType.Equals(typeof(DateTime)))                
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Format"))                    
                        worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.NumberFormat.Format = "dd MMM yyyy";// ConvertStringFormatToXL(dt.Columns[i].ExtendedProperties["Format"].ToString());                                                    

                if (dt.Columns[i].ExtendedProperties.ContainsKey("Alignment"))
                {
                    if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                        worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    else
                    if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                        worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    else
                    if (dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Center")
                        worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                if (dt.Columns[i].ExtendedProperties.Contains("FieldType"))
                    if ((int)dt.Columns[i].ExtendedProperties["FieldType"] == 99)
                    {
                        worksheet.Cell(1, (i + 1)).Value =  dt.Columns[i].Caption + " Comments";
                        worksheet.Column(i + 1).Style.Alignment.WrapText = true;
                    }

                if (_highlightedprojectstatuses != null && dt.Rows.Count > 0 && dt.Columns[i].ColumnName == "SalesFunnelStage")
                    _salesfunnelstatuscol = i;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dtcolumncount; j++)
                    worksheet.Cell(i + 2, j + 1).Value = dt.Rows[i][j];
                
                if (_highlightedprojectstatuses != null && _salesfunnelstatuscol > 0)                                    
                    if (_highlightedprojectstatuses.ContainsKey(dt.Rows[i][_salesfunnelstatuscol].ToString()))                 
                        worksheet.Rows(i+2,i+2).Style.Fill.BackgroundColor = XLColor.FromHtml(StaticCollections.ColorDictionary[_highlightedprojectstatuses[dt.Rows[i][15].ToString()]].ToString());
                                
            }

            //Format top row
            var rangeheader = worksheet.Range(1, 1, 1, dtcolumncount);
            rangeheader.Style.Font.Bold = true;
            worksheet.Row(1).Height = 30;
            worksheet.Columns().AdjustToContents();

            //worksheet.Columns(1, dtcolumncount).AdjustToContents(1, dt.Rows.Count + 1);
        }

        public void zCreateEvaluationPlansReport( DataTable dt) {
            ClosedXML.Excel.XLWorkbook workbook = new XLWorkbook(ClosedXML.Excel.XLEventTracking.Disabled);
            // workbook.Worksheets.Add(dt.TableName.ToString());
            // var worksheet = workbook.Worksheet(dt.TableName.ToString());
          

            IXLWorksheet worksheet = workbook.Worksheets.Add(dt.TableName.ToString());

            int dtcolumncount = dt.Columns.Count;

            for (int i = 0; i < dtcolumncount; i++)
            {
                worksheet.Cell(1, (i + 1)).Value = dt.Columns[i].Caption;
                worksheet.Range(2, i + 1, dt.Rows.Count + 1, i + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
                for (int j = 0; j < dtcolumncount; j++)
                    worksheet.Cell(i + 2, j + 1).Value = dt.Rows[i][j];
           
            //Format top row
            var rangeheader = worksheet.Range(1, 1, 1, dtcolumncount);
            rangeheader.Style.Font.Bold = true;
            worksheet.Row(1).Height = 30;

            worksheet.Columns(1,dtcolumncount).AdjustToContents(1, dt.Rows.Count+1);

            SaveWorkbook(ref workbook);

            workbook.Dispose();
        }


        private string ConvertStringFormatToXL(string _strformat)
        {
            string _newformat = "0";
            //split string into first char and following number
            string _firstchar = string.Empty;
            int _numberdecimals =0;
            string _decimalpart = string.Empty;
            bool isnumber = false;

            if (!string.IsNullOrEmpty(_strformat))
            {
                _strformat = _strformat.TrimStart(' ');
                _strformat = _strformat.TrimEnd(' ');

                if (_strformat.Length > 0 && _strformat.Length < 3)
                {
                    _firstchar = _strformat[0].ToString();

                    if (_strformat.Length == 2)
                        isnumber = int.TryParse(_strformat[1].ToString(), out _numberdecimals);
                    if (_strformat.Length == 1 || isnumber)
                    {
                        if (_numberdecimals > 0)
                            _decimalpart = "." + new String('0', _numberdecimals);
                        switch (_firstchar.ToUpper())
                        {
                            case "P":
                                _newformat = "0" + _decimalpart + "%";
                                break;

                            case "N":
                                _newformat = "#,##0" + _decimalpart;
                                break;

                            case "C":
                                _newformat = "$#,##0" + _decimalpart;
                                break;

                            default:
                                _newformat = "0";
                                break;
                        }
                    }
                }
            }            
            return _newformat;
        }
          
        //public void CreatePlaybookReport(DataTable _salesfunnel, DataTable _newbusiness, DataTable _businessatrisk, DataTable _lostbusiness, Dictionary<string, string> _selectedprojectstatuses = null)
        //{
        //    XLWorkbook workbook;
        //    workbook = new XLWorkbook(XLEventTracking.Disabled);
        //    CreatePlaybookReportSheet(ref workbook, _salesfunnel, _selectedprojectstatuses);
        //    CreatePlaybookReportSheet(ref workbook, _newbusiness);
        //    SaveWorkbook(ref workbook);
        //    workbook.Dispose();
        //}

        public void zCreatePlaybookReport( DataTable _salesfunnel, DataTable _newbusiness, Dictionary<string, string> _selectedprojectstatuses = null)
        {
            ClosedXML.Excel.XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled);
            CreatePlaybookReportSheet(ref workbook, _salesfunnel, _selectedprojectstatuses);
            CreatePlaybookReportSheet(ref workbook, _newbusiness);                                 
            SaveWorkbook(ref workbook);
            workbook.Dispose();
        }

        public void zCreatePlaybookSFReport(DataTable _salesfunnel, Dictionary<string, string> _selectedprojectstatuses = null)
        {
            XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled);
            CreatePlaybookReportSheet(ref workbook, _salesfunnel, _selectedprojectstatuses);
            SaveWorkbook(ref workbook);
            workbook.Dispose();
        }
       
        private void SaveWorkbook(ref XLWorkbook _workbook)
        {
            try
            {
                IMessageBoxService dlg = new MessageBoxService();
                Window owner;
                owner = Application.Current.Windows[0];
                string result = dlg.SaveFileDlg("Select File Name to Save As", "Excel Files(*.xlsx)| *.xlsx", owner);
                if (!string.IsNullOrEmpty(result))
                {
                    _workbook.SaveAs(result);
                    Process.Start(result);
                    _workbook.Dispose();
                }
                dlg = null;
            }
            catch (Exception e)
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("The file is already open.\nPlease close or select a different file name\n" + e.Message, "File already open", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Exclamation);
                msg = null;
            }
            
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            //  Convert a zero-based column index into an Excel column reference  (A, B, C.. Y, Y, AA, AB, AC... AY, AZ, B1, B2..)
            //
            //  eg  GetExcelColumnName(0) should return "A"
            //      GetExcelColumnName(1) should return "B"
            //      GetExcelColumnName(25) should return "Z"
            //      GetExcelColumnName(26) should return "AA"
            //      GetExcelColumnName(27) should return "AB"
            //      ..etc..
            //
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            char firstChar = (char)('A' + (columnIndex / 26) - 1);
            char secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (this != null)
                {
                    this.Dispose();
                    //this = null;
                }
            }
        }
    }

}

