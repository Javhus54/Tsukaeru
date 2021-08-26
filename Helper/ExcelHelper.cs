using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tsukaeru
{
     public static class ExcelHelper
    {
        private static ExcelWorksheet ReadFromExcel(string fileName, int sheetNo)
        {
            string path = Constants.INPUT_DIRECOTRY + fileName;
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
        private static string ReadCellFromExcel(string fileName, int sheetNo, int row, int column, bool header)
        {
            var worksheet = ReadFromExcel(fileName, sheetNo);
            string result = worksheet.Cells[row + (header ? 1 : 0), column].Value.ToString();
            return result;
        }
        public static List<string> ReadAllRows(string fileName, int sheetNo, int column, bool header = true)
        {
            List<string> result = new List<string>();
            var startRow = header ? 2 : 1;
            var worksheet = ReadFromExcel(fileName, sheetNo);
            var rowCount = worksheet.Dimension.End.Row;
            for (var rowNo = startRow; rowNo <= rowCount; rowNo++)
                result.Add(worksheet.Cells[rowNo, column].Value.ToString());
            return result;
        }
        public static string[,] ReadAllData(string fileName, int sheetNo, bool header = true)
        {
            var worksheet = ReadFromExcel(fileName, sheetNo);
            int columnCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;
            string[,] result = new string[rowCount + 2, columnCount + 2];
            int startRow = header ? 2 : 1;
            for (int row = startRow; row <= rowCount; row++)
                for (int column = 1; column <= columnCount; column++)
                    result[row, column] = worksheet.Cells[row, column].Value.ToString();
            return result;
        }
        public static bool WriteAllData(string filename, int sheetNo, string[,] data, int columnCount, int rowCount, string header = null)
        {
            int flag = 1;
            string filePath = Constants.OUTPUT_DIRECTORY;
            if (File.Exists(filePath + filename))
            {
                string[] fileName = filename.Split('.');
                int i;
                string reName;
                for (reName = $"{fileName[0]}_{DateTime.Now:ddMMMyyyy}.{fileName[1]}", i = 0; File.Exists(filePath + reName); i++, reName = $"{fileName[0]}_{DateTime.Now:ddMMMyyyy}_{i}.{fileName[1]}") ;
                File.Move(filePath + filename, destFileName: filePath + reName);
                LogHelper.Log(LogHelper.LEVEL.INFO, null, $"Succesfull rename{filePath + filename} TO {filePath + reName}");
                filename = reName;
            }
            string path = filePath + filename;
            Stream stream = File.Create(path);
            ExcelPackage excel = new ExcelPackage();
            ExcelWorkbook workbook = excel.Workbook;
            ExcelWorksheet worksheet = workbook.Worksheets.Add(sheetNo.ToString());
            if (header != null)
            {
                worksheet.Cells[1, 1].Value = header;
                flag = 2;
            }
            for (int row = 0; row < rowCount; row++)
                for (int column = 0; column < columnCount; column++)
                    worksheet.Cells[row + flag, column + 1].Value = data[row, column];
            excel.SaveAs(stream);
            stream.Flush();
            stream.Close();
            return true;
        }
    }
}
