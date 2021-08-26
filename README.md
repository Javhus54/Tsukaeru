# Tsukaeru
This is a Selenium C# Helper

//
private static void AddColumnHeaderRow(DataTable dt, Row row, SheetData sheetData)
        {
            // Add the column names which need to be exported as String instead of numbers even through they have only numbers
            // parsing any value as number will export that as a excel numeric value which may trim zeros in the beginning
            List<string> columnsExportAsString = new List<string>() { "ZipCode" };

 

            // We have to loop through the columns and get column names to create header row in excel
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var cell = new Cell { DataType = CellValues.InlineString, StyleIndex = 1 };
                var inlineCell = new InlineString();
                var cellText = new Text { Text = dt.Columns[i].ColumnName };
                inlineCell.AppendChild(cellText);
                cell.AppendChild(inlineCell);

 

                row.AppendChild(cell);

 

                //check if the column value need to be exported as string
                dt.Columns[i].ExtendedProperties["IsString"] = "false";
                columnsExportAsString.ForEach(x =>
                {
                    if (dt.Columns[i].ColumnName.ToLowerInvariant().Contains(x.ToLowerInvariant()))
                        dt.Columns[i].ExtendedProperties["IsString"] = "true";
                });
            }

 

            //Append new row to the sheet data
            sheetData.AppendChild(row);
        }

 

        private static void AddDataRows(DataTable dt, uint rowIndex, SheetData sheetData)
        {
            // Loop through all the data rows, create and append excel rows to data sheet
            for (var r = 0; r < dt.Rows.Count; r++)
            {
                var row = new Row { RowIndex = ++rowIndex };

 

                for (var c = 0; c < dt.Columns.Count; c++)
                {
                    var value = dt.Rows[r][c];
                    double doubleTemp; int intTemp;

 

                    if (dt.Columns[c].ExtendedProperties["IsString"].ToString() != "true" &&
                        (value is int || value is short || value is double
                         || int.TryParse(value.ToString(), out intTemp) || double.TryParse(value.ToString(), out doubleTemp)))
                    {
                        row.Append(new Cell() { CellValue = new CellValue(value.ToString()), DataType = CellValues.String });
                    }
                    else if (value.GetType() == typeof(DateTime))
                    {
                        row.Append(new Cell() { CellValue = new CellValue(((DateTime)value).ToOADate().ToString(CultureInfo.InvariantCulture)), StyleIndex = UInt32Value.FromUInt32(3) });
                    }
                    else
                    {
                        var cell = new Cell { DataType = CellValues.InlineString };
                        var istring = new InlineString();
                        var t = new Text { Text = dt.Rows[r][c].ToString() };
                        istring.AppendChild(t);
                        cell.AppendChild(istring);
                        row.AppendChild(cell);
                        //row.AppendChild(new Cell() { CellValue = new CellValue((string)value), DataType = CellValues.String });
                    }
                }

 

                //Append each data row to sheet data
                sheetData.AppendChild(row);
            }
        }
