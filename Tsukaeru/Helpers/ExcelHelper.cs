using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsukaeru.Helpers
{
    public class ExcelCellWithType
    {
        public string Value { get; set; }
        public UInt32Value ExcelCellFormat { get; set; }
        public bool IsDateTimeType { get; set; }
    }
    public static class ExcelHelper
    {
        public static DataTable ReadExcelSheet(string fileName, string pathDirectory = "Input", bool header = true)
        {
            string path = "";
            if (pathDirectory == "Download")
                path = Defaults.DOWNLOADS_DIRECTORY;
            else
                path = Defaults.INPUT_DIRECOTRY;
            List<string> headers = new List<string>();
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(path + fileName, false))
            {
                //Read the first Sheets 
                Sheet sheet = spreadsheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (spreadsheet.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                int counter = 0;
                foreach (Row row in rows)
                {
                    counter++;
                    //Read the first row as header
                    if (counter == 1)
                    {
                        var j = 1;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            //var colunmName = header ? GetCellValue(spreadsheet, cell) : "Field" + j++;
                            var colunmName = header ? ReadExcelCell(cell, spreadsheet.WorkbookPart).Value : "Field" + j++;
                            //Console.WriteLine(colunmName);
                            if (headers.Contains(colunmName))
                            {
                                int columnCount = 2;
                                while (headers.Contains(colunmName + columnCount))
                                {
                                    columnCount++;
                                }
                                headers.Add(colunmName + columnCount);
                                dataTable.Columns.Add(colunmName + columnCount);
                            }
                            else
                            {
                                headers.Add(colunmName);
                                dataTable.Columns.Add(colunmName);
                            }
                        }
                    }
                    else
                    {
                        dataTable.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            //dataTable.Rows[dataTable.Rows.Count - 1][i] = GetCellValue(spreadsheet, cell);
                            dataTable.Rows[dataTable.Rows.Count - 1][i] = ReadExcelCell(cell, spreadsheet.WorkbookPart).Value;
                            i++;
                        }
                    }
                }

            }
            return dataTable;
        }
        public static DataTable ReadExcelSheet2(string fileName, string pathDirectory = "Input", bool header = true)
        {
            string path = "";
            if (pathDirectory == "Download")
                path = Defaults.DOWNLOADS_DIRECTORY;
            else
                path = Defaults.INPUT_DIRECOTRY;
            List<string> headers = new List<string>();
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(path + fileName, false))
            {
                //Read the first Sheets 
                Sheet sheet = spreadsheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                Worksheet worksheet = (spreadsheet.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                int maxColumnCount = -1;
                foreach (Row row in rows)
                {
                    int currentRowCount = 0;
                    foreach (Cell cell in row.Descendants<Cell>())
                        currentRowCount++;
                    maxColumnCount = (currentRowCount > maxColumnCount) ? currentRowCount : maxColumnCount;
                }
                for (int i = 0; i < maxColumnCount; i++)
                {
                    dataTable.Columns.Add("colunm NO:" + i);
                }
                foreach (Row row in rows)
                {
                    dataTable.Rows.Add();
                    int i = 0;
                    foreach (Cell cell in row.Descendants<Cell>())
                    {
                        dataTable.Rows[dataTable.Rows.Count - 1][i] = ReadExcelCell(cell, spreadsheet.WorkbookPart).Value;
                        i++;
                    }
                }

            }
            return dataTable;
        }
        public static DataTable ReadExcelSheet(string fileName, int sheetNo, string pathDirectory = "Input", bool header = true)
        {
            string path = "";
            if (pathDirectory == "Download")
                path = Defaults.DOWNLOADS_DIRECTORY;
            else
                path = Defaults.INPUT_DIRECOTRY;
            List<string> headers = new List<string>();
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(path + fileName, false))
            {
                //Read the first Sheets

                //create the object for workbook part  
                WorkbookPart workbookPart = spreadsheet.WorkbookPart;

                //statement to get the count of the worksheet  
                int worksheetcount = workbookPart.Workbook.Sheets.Count();

                //statement to get the sheet object  
                Sheet mysheet = (Sheet)workbookPart.Workbook.Sheets.ChildElements.ElementAt(sheetNo);

                //statement to get the worksheet object by using the sheet id  
                Worksheet worksheet = ((WorksheetPart)workbookPart.GetPartById(mysheet.Id)).Worksheet;

                //statement to get the sheetdata which contains the rows and cell in table  
                //SheetData rows = (SheetData)Worksheet.ChildElements.GetItem(sheetNo);

                //Sheet sheet = spreadsheet.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                //Worksheet worksheet = (spreadsheet.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                int counter = 0;
                foreach (Row row in rows)
                {
                    counter++;
                    //Read the first row as header
                    if (counter == 1)
                    {
                        var j = 1;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            //var colunmName = header ? GetCellValue(spreadsheet, cell) : "Field" + j++;
                            var colunmName = header ? ReadExcelCell(cell, spreadsheet.WorkbookPart).Value : "Field" + j++;
                            //Console.WriteLine(colunmName);
                            if (headers.Contains(colunmName))
                            {
                                int columnCount = 2;
                                while (headers.Contains(colunmName + columnCount))
                                {
                                    columnCount++;
                                }
                                headers.Add(colunmName + columnCount);
                                dataTable.Columns.Add(colunmName + columnCount);
                            }
                            else
                            {
                                headers.Add(colunmName);
                                dataTable.Columns.Add(colunmName);
                            }
                        }
                    }
                    else
                    {
                        dataTable.Rows.Add();
                        int i = 0;
                        foreach (Cell cell in row.Descendants<Cell>())
                        {
                            //dataTable.Rows[dataTable.Rows.Count - 1][i] = GetCellValue(spreadsheet, cell);
                            dataTable.Rows[dataTable.Rows.Count - 1][i] = ReadExcelCell(cell, spreadsheet.WorkbookPart).Value;
                            i++;
                        }
                    }
                }

            }
            return dataTable;
        }
        public static string GetExcelSheetName(string fileName, int sheetNo, string pathDirectory = "Input", bool header = true)
        {
            string path;
            if (pathDirectory == "Download")
                path = Defaults.DOWNLOADS_DIRECTORY;
            else
                path = Defaults.INPUT_DIRECOTRY;
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(path + fileName, false))
            {
                //Read the first Sheets

                //create the object for workbook part  
                WorkbookPart workbookPart = spreadsheet.WorkbookPart;

                //statement to get the count of the worksheet  
                int worksheetcount = workbookPart.Workbook.Sheets.Count();

                //statement to get the sheet object  
                Sheet mysheet = (Sheet)workbookPart.Workbook.Sheets.ChildElements.ElementAt(sheetNo);

                //statement to get the worksheet object by using the sheet id  
                Worksheet worksheet = ((WorksheetPart)workbookPart.GetPartById(mysheet.Id)).Worksheet;
                return mysheet.Name;
                //return worksheet.LocalName;
            }
        }
        public static string GetCellValue(SpreadsheetDocument spreadsheet, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return spreadsheet.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.ElementAt(int.Parse(value)).InnerText;
            }
            return value;
        }
        public static ExcelCellWithType ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = (cellValue == null) ? cell.InnerText : cellValue.Text;
            if (cell.DataType?.Value == CellValues.SharedString)
            {
                text = workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;
            }

            var cellText = (text ?? string.Empty).Trim();

            var cellWithType = new ExcelCellWithType();

            if (cell.StyleIndex != null)
            {
                var cellFormat = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements[
                    int.Parse(cell.StyleIndex.InnerText)] as CellFormat;

                if (cellFormat != null)
                {
                    cellWithType.ExcelCellFormat = cellFormat.NumberFormatId;
                    if (cellFormat.NumberFormatId != null)
                    {
                        var dateFormat = GetDateTimeFormat(cellFormat.NumberFormatId);
                        if (!string.IsNullOrEmpty(dateFormat))
                        {
                            cellWithType.IsDateTimeType = true;

                            if (!string.IsNullOrEmpty(cellText))
                            {
                                if (double.TryParse(cellText, out var cellDouble))
                                {
                                    var theDate = DateTime.FromOADate(cellDouble);
                                    cellText = theDate.ToString(dateFormat);
                                }
                            }
                        }
                    }
                }
            }

            cellWithType.Value = cellText;

            return cellWithType;
        }
        public static void CreateExcelFile(DataSet ds, string fileName, string path = "Output")
        {
            string finalName = fileName;
            if (path == "Output")
            {
                path = Defaults.OUTPUT_DIRECTORY;
                string[] temp = fileName.Split('.');
                string fileExtension = temp[1];
                fileName = temp[0];
                int fileCount = 1;
                string fileIdentifier = DateTime.Now.ToString("ddMMMyyy");
                while (File.Exists(path + fileName + fileIdentifier + '_' + fileCount + '.' + fileExtension))
                {
                    fileCount++;
                }
                finalName = path + fileName + fileIdentifier + '_' + fileCount + '.' + fileExtension;
            }
            else
            {
                //path = Defaults.SUMMARYOUTPUT_DIRECTORY;
                if (File.Exists(path + fileName + ".xlsx"))
                {
                    int fileCount = 1;
                    string fileIdentifier = DateTime.Now.ToString("ddMMMyyy");
                    while (File.Exists(Defaults.TEMPORARY_DIRECTORY + fileName + fileIdentifier + '_' + fileCount + ".xlsx"))
                    {
                        fileCount++;
                    }
                    string finalTempName = Defaults.TEMPORARY_DIRECTORY + fileName + fileIdentifier + '_' + fileCount + ".xlsx";
                    File.Move(path + fileName + ".xlsx", finalTempName);
                }
                finalName = path + fileName + ".xlsx";
            }
            using (var workbook = SpreadsheetDocument.Create(finalName, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();

                workbook.WorkbookPart.Workbook = new Workbook();

                workbook.WorkbookPart.Workbook.Sheets = new Sheets();
                //DataSet ds = new DataSet();
                //ds.Tables.Add(table);
                foreach (DataTable dataTable in ds.Tables)
                {

                    var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    sheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

                    uint sheetId = 1;
                    if (sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId =
                            sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = dataTable.TableName };
                    sheets.Append(sheet);

                    Row headerRow = new Row();
                    List<String> columns = new List<string>();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        columns.Add(column.ColumnName);
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in dataTable.Rows)
                    {
                        Row newRow = new Row();
                        foreach (string col in columns)
                        {
                            Cell cell = new Cell();
                            var value = dsrow[col];
                            int result;
                            if (Int32.TryParse(value.ToString(), out result))
                            {
                                cell.DataType = CellValues.Number;
                                cell.CellValue = new CellValue(result);
                            }
                            else
                            {
                                cell.DataType = CellValues.String;
                                cell.CellValue = new CellValue(value.ToString());
                            }
                            newRow.AppendChild(cell);
                        }
                        sheetData.AppendChild(newRow);
                    }
                }
            }
        }
        public static readonly Dictionary<uint, string> DateFormatDictionary = new Dictionary<uint, string>()
        {
            [14] = "dd/MM/yyyy",
            [15] = "d-MMM-yy",
            [16] = "d-MMM",
            [17] = "MMM-yy",
            [18] = "h:mm AM/PM",
            [19] = "h:mm:ss AM/PM",
            [20] = "h:mm",
            [21] = "h:mm:ss",
            [22] = "M/d/yy h:mm",
            [30] = "M/d/yy",
            [34] = "yyyy-MM-dd",
            [45] = "mm:ss",
            [46] = "[h]:mm:ss",
            [47] = "mmss.0",
            [51] = "MM-dd",
            [52] = "yyyy-MM-dd",
            [53] = "yyyy-MM-dd",
            [55] = "yyyy-MM-dd",
            [56] = "yyyy-MM-dd",
            [58] = "MM-dd",
            [165] = "M/d/yy",
            [166] = "dd MMMM yyyy",
            [167] = "dd/MM/yyyy",
            [168] = "dd/MM/yy",
            [169] = "d.M.yy",
            [170] = "yyyy-MM-dd",
            [171] = "dd MMMM yyyy",
            [172] = "d MMMM yyyy",
            [173] = "M/d",
            [174] = "M/d/yy",
            [175] = "MM/dd/yy",
            [176] = "d-MMM",
            [177] = "d-MMM-yy",
            [178] = "dd-MMM-yy",
            [179] = "MMM-yy",
            [180] = "MMMM-yy",
            [181] = "MMMM d, yyyy",
            [182] = "M/d/yy hh:mm t",
            [183] = "M/d/y HH:mm",
            [184] = "MMM",
            [185] = "MMM-dd",
            [186] = "M/d/yyyy",
            [187] = "d-MMM-yyyy"
        };

        public static string GetDateTimeFormat(UInt32Value numberFormatId)
        {
            return DateFormatDictionary.ContainsKey(numberFormatId) ? DateFormatDictionary[numberFormatId] : string.Empty;
        }
    }
}