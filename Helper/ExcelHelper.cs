using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tsukaeru
{
    public class ExcelHelper
    {
        private ExcelWorksheet ReadFromExcel(string fileName, int sheetNo)
        {
            string path = @"C:\temp\" + fileName;
            if (File.Exists(path))
            {
                var file = new FileInfo(path);
                ExcelPackage excel = new ExcelPackage(file);
                ExcelWorkbook workbook = excel.Workbook;
                ExcelWorksheet worksheet = workbook.Worksheets[sheetNo];
                return worksheet;
            }
            else
            {
                Console.WriteLine("File Not Found");
                return null;
            }
        }
        private string ReadCellFromExcel(string fileName, int sheetNo, int row, int column, bool header)
        {
            var worksheet = ReadFromExcel(fileName, sheetNo);
            string result = worksheet.Cells[row + (header ? 1 : 0), column].Value.ToString();
            return result;
        }
        public List<string> ReadAllRows(string fileName,int sheetNo, int column, bool header = true)
        {
            List<string> result = new List<string>();
            var startRow = header ? 2 : 1;
            var worksheet = ReadFromExcel(fileName, sheetNo);
            var rowCount = worksheet.Dimension.End.Row;
            for(var rowNo = startRow; rowNo<= rowCount; rowNo++)
                result.Add(worksheet.Cells[rowNo,column].Value.ToString());
            return result;
        }
        public string[,] ReadAllData(string fileName,int sheetNo,bool header = true)
        {
            var worksheet = ReadFromExcel(fileName, sheetNo);
            int columnCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;
            string[,] result = new string[rowCount+2,columnCount+2];
            int startRow = header ? 2 : 1;
            for (int row = startRow; row <= rowCount; row++)
                for (int column = 1; column <= columnCount; column++)
                    result[row,column] = worksheet.Cells[row, column].Value.ToString();
            return result;
        }
        public bool WriteAllData(string fileName,int sheetNo, string[,] data,int columnCount, int rowCount, string header = null, bool newFile = true)
        {
            int flag = 1;
            string path = @"C:\temp\" + fileName;
            if (File.Exists(path) && newFile)
                File.Delete(path);
            Stream stream = File.Create(path);
            var file = new FileInfo(path);
            ExcelPackage excel = new ExcelPackage();
            ExcelWorkbook workbook = excel.Workbook;
            ExcelWorksheet worksheet = workbook.Worksheets.Add(sheetNo.ToString());
            if (header != null)
            {
                worksheet.Cells[1, 1].Value = header;
                flag = 2;
            }
            for(int row = 0; row < rowCount; row++)
                for(int column = 0; column < columnCount; column++)
                    worksheet.Cells[row + flag, column + 1].Value = data[row, column];
            excel.SaveAs(stream);
            stream.Flush();
            stream.Close();
            return true;
        }
    }
}
