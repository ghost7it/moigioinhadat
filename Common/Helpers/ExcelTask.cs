using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataTable = System.Data.DataTable;
namespace Common
{
    //NamMV: Xam lại ở Project Traning Tool
    public class ExcelTask
    {
        [DllImport("user32.dll")]
        public static extern bool EndTask(IntPtr hwnd, bool shutdown, bool force);

        /// <summary>
        /// Collects the header.
        /// </summary>
        /// <param name="excelFilePath">The excel file path.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns>System.String[][].</returns>
        public static string[] CollectHeader(string excelFilePath, string sheetName)
        {
            var oleDbConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";", excelFilePath);

            var oleDbConnection = new OleDbConnection(oleDbConnectionString);
            var oleDbCommand = new OleDbCommand(string.Format("SELECT * FROM [{0}$]", sheetName), oleDbConnection);
            var dataAdapter = new OleDbDataAdapter();
            var dataTable = new DataTable();

            try
            {
                dataAdapter.SelectCommand = oleDbCommand;
                dataAdapter.Fill(dataTable);

                var listHeaders = new string[dataTable.Columns.Count + 1];
                listHeaders[0] = "";
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    if (dataTable.Columns[i].Caption != ("F" + (i + 1)))
                    {
                        listHeaders[i + 1] = dataTable.Columns[i].Caption;
                    }
                }

                var goodNumber = listHeaders.Length;
                while ((listHeaders[goodNumber - 1] == null) || (listHeaders[goodNumber - 1] == ""))
                {
                    goodNumber--;
                    if (goodNumber < 1)
                    {
                        break;
                    }
                }

                var result = new string[goodNumber];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = listHeaders[i];
                }
                oleDbConnection.Close();
                return result;
            }
            catch (FormatException formatException)
            {
            }
            catch (IndexOutOfRangeException indexOutOfRangeException)
            {
            }
            catch (OleDbException oleDbException)
            {
            }
            catch (SqlException sqlException)
            {
            }
            catch (SqlTypeException sqlTypeException)
            {
            }
            finally
            {
                oleDbConnection.Close();
            }
            return null;
        }

        /// <summary>
        /// Collects the name of the sheet.
        /// </summary>
        /// <param name="excelFilePath">The excel file path.</param>
        /// <returns>System.String[][].</returns>
        public static string[] CollectSheetName(string excelFilePath)
        {
            var sbConnection = new OleDbConnectionStringBuilder();
            var strExtendedProperties = String.Empty;
            sbConnection.DataSource = excelFilePath;
            if (Path.GetExtension(excelFilePath).Equals(".xls"))//for 97-03 Excel file
            {
                sbConnection.Provider = "Microsoft.Jet.OLEDB.4.0";
                strExtendedProperties = "Excel 8.0;HDR=Yes;IMEX=1";//HDR=ColumnHeader,IMEX=InterMixed
            }
            else if (Path.GetExtension(excelFilePath).Equals(".xlsx"))  //for 2007 Excel file
            {
                sbConnection.Provider = "Microsoft.ACE.OLEDB.12.0";
                strExtendedProperties = "Excel 12.0;HDR=Yes;IMEX=1";
            }
            sbConnection.Add("Extended Properties", strExtendedProperties);
            var conn = new OleDbConnection(sbConnection.ToString());

            DataTable dtSheet;
            try
            {
                conn.Open();
                dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            }
            catch (Exception e)
            {
                return null;
            }

            var listSheet = new List<string>();
            if (dtSheet.Rows.Count <= 0) return null;
            foreach (DataRow drSheet in dtSheet.Rows)
            {
                //checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                if (drSheet["TABLE_NAME"].ToString().Contains("$"))
                {
                    var tableName = drSheet["TABLE_NAME"].ToString().Replace("\'", "");
                    var tableNamePart = tableName.Split('$');
                    if (String.IsNullOrEmpty(tableNamePart[1]))
                    {
                        listSheet.Add(tableNamePart[0]);
                    }
                }
            }

            return listSheet.ToArray();
        }

        /// <summary>
        /// Đọc Excel File theo cách tạo Application
        /// LuanBH
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static DataTable ReadExcelToDataTable(string excelFilePath, string sheetName)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataTable dtreturn = new DataTable();
            Application oExcel;
            Workbook oWorkBook;
            Worksheet oWorkSheet = null;

            oExcel = new Application();

            oWorkBook = oExcel.Workbooks.Open(excelFilePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            foreach (Worksheet sheet in oWorkBook.Worksheets)
            {
                if (sheet.Name.Equals(sheetName))
                {
                    oWorkSheet = sheet;
                }
            }

            int r;
            int c;
            int intRows;
            int intCols;

            Range excelCell = oWorkSheet.UsedRange;
            Object[,] values = (Object[,])excelCell.Value2;
            intRows = values.GetLength(0);
            List<int> dateColumns = new List<int>();

            if (intRows != 0)
            {
                intCols = values.GetLength(1);
                if (intCols != 0)
                {
                    for (c = 1; c <= intCols; c++)
                    {
                        if (values[1, c] != null)
                        {
                            if (dt.Columns.IndexOf("ErrorMessage") != -1 && values[1, c].ToString().Equals("ErrorMessage"))
                            {
                                values[1, c] = "ErrorMessage - Duplicate";
                            }
                            if (values[1, c].ToString().Contains("Date"))
                            {
                                dateColumns.Add(c);
                            }
                            dt.Columns.Add(new DataColumn((String)values[1, c]));
                        }
                    }
                    for (r = 2; r <= intRows; r++)
                    {
                        dr = dt.NewRow();
                        for (c = 1; c <= dt.Columns.Count; c++)
                        {
                            if (dateColumns.IndexOf(c) != -1)
                            {
                                if (values[r, c] != null)
                                {
                                    try
                                    {
                                        var dateDouble = Double.Parse(values[r, c].ToString());
                                        var date = DateTime.FromOADate(dateDouble).ToString("dd-MMM-yy");
                                        dr[c - 1] = date;

                                    }
                                    catch (Exception)
                                    {
                                        dr[c - 1] = values[r, c];
                                    }
                                }
                                else
                                {
                                    dr[c - 1] = values[r, c];
                                }
                            }
                            else
                            {
                                dr[c - 1] = values[r, c];
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }

            dtreturn = dt;
            oWorkBook.Close(Missing.Value, Missing.Value, Missing.Value);
            oExcel.Quit();
            return dtreturn;
        }

        /// <summary>
        /// Retrieves the excel table in the sheet in excel file.
        /// </summary>
        /// <param name="excelFilePath">The excel file path.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <returns>DataTable.</returns>
        public static DataTable RetrieveExcelTable(string excelFilePath, string sheetName)
        {
            var oleDbConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=Yes;IMEX=1'", excelFilePath);
            //var oleDbConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", excelFilePath);
            var oleDbConnection = new OleDbConnection(oleDbConnectionString);
            var oleDbCommand = new OleDbCommand(string.Format("SELECT * FROM [{0}$]", sheetName), oleDbConnection);
            var dataAdapter = new OleDbDataAdapter();
            var dataTable = new DataTable();

            // VietNA
            /*oleDbCommand.Connection.Open();
            var dataReader = oleDbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            var table = new DataTable();
            table.Columns.Add("EmployeeID", typeof(string));
            table.Columns.Add("AccountID", typeof(string));
            table.Columns.Add("Full Name", typeof(string));
            table.Columns.Add("Branch", typeof(string));
            table.Columns.Add("Parrent Department", typeof(string));
            table.Columns.Add("Child Department", typeof(string));
            table.Columns.Add("Course Category", typeof(string));
            table.Columns.Add("Course Year", typeof(string));
            table.Columns.Add("Course Name", typeof(string));
            table.Columns.Add("Course Code", typeof(string));
            table.Columns.Add("Course End Date", typeof(string));
            table.Columns.Add("Fee", typeof(string));
            table.Columns.Add("Time commit", typeof(string));
            table.Columns.Add("Refun", typeof(string));
            table.Columns.Add("Trained Duration (hrs)", typeof(string));
            table.Columns.Add("Pass Coverage", typeof(string));
            table.Columns.Add("Final Result", typeof(string));
            table.Columns.Add("Notes", typeof(string));
            table.Columns.Add("F19", typeof(string));

            int tableCount = 0;
            DataRow newRow;
            while (dataReader.Read())
            {
                newRow = table.NewRow();

                newRow[0] = dataReader[0];
                newRow[1] = dataReader[1];
                newRow[2] = dataReader[2];
                newRow[3] = dataReader[3];
                newRow[4] = dataReader[4];
                newRow[5] = dataReader[5];
                newRow[6] = dataReader[6];
                newRow[7] = dataReader[7];
                newRow[8] = dataReader[8];
                newRow[9] = dataReader[9];
                newRow[10] = dataReader[10];
                newRow[11] = dataReader[11];
                newRow[12] = dataReader[12];
                newRow[13] = dataReader[13];
                newRow[14] = dataReader[14];
                newRow[15] = dataReader[15];
                newRow[16] = dataReader[16];
                newRow[17] = dataReader[17];
                newRow[18] = dataReader[18];

                table.Rows.Add(newRow);
                tableCount++;
            }*/
            // VietNA

            //DataTable result = new DataTable();
            try
            {
                dataAdapter.SelectCommand = oleDbCommand;
                dataAdapter.Fill(dataTable);

                if (dataTable.Columns.Contains("ErrorMessage"))
                {
                    dataTable.Columns.Remove("ErrorMessage");
                }
                //result = dataTable.Rows.Cast<DataRow>()
                //        .Where(row => !row.ItemArray.All(field => field is System.DBNull))
                //        .CopyToDataTable();

                //DataTable result = new DataTable();

                //foreach (DataRow dataRow in dataTable.Rows)
                //{
                //    bool check = false;
                //    string valuesarr = "";
                //    foreach (DataColumn column in dataTable.Columns)
                //    {
                //        if (!column.ColumnName.Equals("ErrorMessage"))
                //        {
                //            valuesarr += dataRow[column];
                //        }
                //    }
                //    if (String.IsNullOrEmpty(valuesarr.Trim()))
                //    {
                //        dataTable.Rows.Remove(dataRow);
                //    }
                //}

                oleDbConnection.Close();
            }
            catch (FormatException formatException)
            {
            }
            catch (IndexOutOfRangeException indexOutOfRangeException)
            {
            }
            catch (OleDbException oleDbException)
            {
            }
            catch (SqlException sqlException)
            {
            }
            catch (SqlTypeException sqlTypeException)
            {
            }
            oleDbConnection.Close();
            return dataTable;
        }

        /// <summary>
        /// Updates to origin excel.
        /// </summary>
        /// <param name="excelFilePath">The excel file path.</param>
        /// <param name="sheetName">Name of the sheet.</param>
        /// <param name="errorDictionary">The error dictionary.</param>
        public static void UpdateToOriginExcel(string excelFilePath, string sheetName, Dictionary<int, string> errorDictionary)
        {
            if ((errorDictionary == null) || (errorDictionary.Count < 1)) { return; }

            var xlApp = new Application();
            var workBooksTemp = xlApp.Workbooks;
            var xlWorkbook = workBooksTemp.Open(excelFilePath);

            _Worksheet xlWorksheet = xlWorkbook.Sheets[sheetName];
            var xlRange = xlWorksheet.UsedRange;

            // Retrieve list header in the sheet
            var listHeaders = CollectHeader(excelFilePath, sheetName);

            // Retrieve position of ErrorMessage column
            var errorColumn = listHeaders.Length - 1;

            // When ErrorMessage column is not exist, increase index of the ErrorMessage column
            if (!listHeaders.Contains("ErrorMessage"))
            {
                errorColumn++;
            }

            // Delete old error
            var rang = (Range)xlRange.Columns[errorColumn];
            rang.Value2 = null;

            // Wire header of the Error column
            Range rangeTemp = xlRange.Cells[1, errorColumn];
            rangeTemp.Value2 = "ErrorMessage";

            // Format header cell in excel file
            Range templateHeader = xlRange.Cells[1, errorColumn - 1];
            rangeTemp.ColumnWidth = 60;
            rangeTemp.Font.Bold = templateHeader.Font.Bold;
            rangeTemp.Borders.LineStyle = templateHeader.Borders.LineStyle;
            rangeTemp.HorizontalAlignment = templateHeader.HorizontalAlignment;
            rangeTemp.Interior.ColorIndex = templateHeader.Interior.ColorIndex;

            // Update error from dictionary to excel file
            foreach (var item in errorDictionary)
            {
                rangeTemp = xlRange.Cells[item.Key, errorColumn];
                rangeTemp.Value2 = item.Value;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(rangeTemp);
            Marshal.FinalReleaseComObject(xlRange);
            Marshal.FinalReleaseComObject(xlWorksheet);
            Marshal.FinalReleaseComObject(workBooksTemp);

            xlWorkbook.Save();
            xlWorkbook.Close(false, Missing.Value, Missing.Value);

            Marshal.FinalReleaseComObject(xlWorkbook);

            xlApp.Application.Quit();
            xlApp.Quit();
            EndTask((IntPtr)xlApp.Hwnd, true, true);
            Marshal.FinalReleaseComObject(xlApp);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Ghi lỗi cho Import Language Score
        /// LuanBH
        /// </summary>
        /// <param name="excelFilePath"></param>
        /// <param name="sheets"></param>
        /// <param name="errorDictionary"></param>
        public static void WriteErrorLanguageScore(string excelFilePath, string sheets, Dictionary<string, string> errorDictionary)
        {
            if ((errorDictionary == null) || (errorDictionary.Count < 1)) { return; }

            var xlApp = new Application();
            var workBooksTemp = xlApp.Workbooks;
            var xlWorkbook = workBooksTemp.Open(excelFilePath);
            var sheetList = sheets.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries).Select(sheet => sheet.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries)[0]).ToList();

            foreach (var sheetName in sheetList)
            {
                _Worksheet xlWorksheet = xlWorkbook.Sheets[sheetName];
                var xlRange = xlWorksheet.UsedRange;

                // Retrieve list header in the sheet
                var listHeaders = CollectHeader(excelFilePath, sheetName);

                // Retrieve position of ErrorMessage column
                var errorColumn = listHeaders.Length - 1;
                Range rangeTemp = xlRange.Cells[1, errorColumn];

                // When ErrorMessage column is not exist, create title for the ErrorMessage column
                if (!listHeaders.Contains("ErrorMessage"))
                {
                    errorColumn++;
                    rangeTemp = xlRange.Cells[1, errorColumn];
                    rangeTemp.Value2 = "ErrorMessage";

                    // Format header cell in excel file
                    Range templateHeader = xlRange.Cells[1, errorColumn - 1];
                    rangeTemp.ColumnWidth = 60;
                    rangeTemp.Font.Bold = templateHeader.Font.Bold;
                    rangeTemp.Borders.LineStyle = templateHeader.Borders.LineStyle;
                    rangeTemp.HorizontalAlignment = templateHeader.HorizontalAlignment;
                    rangeTemp.Interior.ColorIndex = templateHeader.Interior.ColorIndex;
                }

                // Update error from dictionary to excel file
                foreach (var item in errorDictionary)
                {
                    if (sheetName.Equals(item.Key.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries)[0]))
                    {
                        int index = Convert.ToInt32(item.Key.Split(new[] { "&&" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                        rangeTemp = xlRange.Cells[index, errorColumn];
                        rangeTemp.Value2 = item.Value;
                    }
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(workBooksTemp);

            xlWorkbook.Save();
            xlWorkbook.Close(false, Missing.Value, Missing.Value);

            Marshal.FinalReleaseComObject(xlWorkbook);

            xlApp.Application.Quit();
            xlApp.Quit();
            EndTask((IntPtr)xlApp.Hwnd, true, true);
            Marshal.FinalReleaseComObject(xlApp);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Exports data form dataSource to excel.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public static string ExportToExcel(DataTable dataSource, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 1;

            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 43, rowIndex);
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataSource, rowIndex);

            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

            return "Success";
        }

        /// <summary>
        /// Export cho Language Score
        /// </summary>
        /// <param name="listTable"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ExportToExcel(List<DataTable> listTable, string filePath)
        {
            if ((listTable == null) || (listTable.Count < 1)) return string.Empty;
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = listTable.Count;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            DataTable dataSource;
            var rowIndex = 1;

            for (var k = 0; k < listTable.Count; k++)
            {
                dataSource = listTable[k];
                var oSheet = (Worksheet)oSheets.Item[k + 1];
                oSheet.Activate();

                if (!string.IsNullOrEmpty(dataSource.TableName))
                {
                    oSheet.Name = dataSource.TableName;
                }

                //WriteTitle(oSheet, dataTable, rowIndex, "Ngoi noi day");
                WriteTitleDefault(oSheet, dataSource, 43, rowIndex);

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataSource, rowIndex + 1);

            }
            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

            return "Success";
        }

        /// <summary>
        /// Export cho Language Score
        /// </summary>
        /// <param name="listTable"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ExportToExcel(string[] sheetNames, List<DataTable> listTable, string filePath)
        {
            if ((listTable == null) || (listTable.Count < 1)) return string.Empty;
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = listTable.Count;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            DataTable dataSource;
            var rowIndex = 1;

            for (var k = 0; k < listTable.Count; k++)
            {
                dataSource = listTable[k];
                var oSheet = (Worksheet)oSheets.Item[k + 1];
                oSheet.Name = sheetNames[k];
                oSheet.Activate();

                if (!string.IsNullOrEmpty(dataSource.TableName))
                {
                    oSheet.Name = dataSource.TableName;
                }

                //WriteTitle(oSheet, dataTable, rowIndex, "Ngoi noi day");
                WriteTitleDefault(oSheet, dataSource, 43, rowIndex);

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataSource, rowIndex + 1);

            }
            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

            return "Success";
        }

        /// <summary>
        /// Splits the data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="field">The field.</param>
        /// <returns>DataTable.</returns>
        public static DataTable SplitDataSource(DataTable dataSource, string field)
        {
            // When the field is null or empty then return
            if (string.IsNullOrEmpty(field))
            {
                return dataSource;
            }

            // check the field is exits in DataSource columns
            var isColumnDataSource = false;
            foreach (var column in dataSource.Columns)
            {
                if (field == column.ToString())
                {
                    isColumnDataSource = true;
                    break;
                }
            }

            // When the field is not exits in list columns of dataSource then return
            if (!isColumnDataSource)
            {
                return dataSource;
            }

            var ordinal = dataSource.Columns[field].Ordinal;
            var columnNumber = 1;

            // Prosess once row
            for (var i = 0; i < dataSource.Rows.Count; i++)
            {
                var parrentField = dataSource.Rows[i][field].ToString();
                var listValues = parrentField.Split(">>".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                while (columnNumber < listValues.Length)
                {
                    // Extend a new column
                    var newColumn = new DataColumn(field + " " + columnNumber, typeof(string));
                    dataSource.Columns.Add(newColumn);
                    dataSource.Columns[newColumn.ColumnName].SetOrdinal(ordinal + columnNumber);
                    columnNumber++;
                }

                for (var j = 0; j < listValues.Length; j++)
                {
                    // Set data for column
                    dataSource.Rows[i][ordinal + j] = listValues[j];
                }
            }

            // Change the name of ordinal field
            dataSource.Columns[field].ColumnName += " 0";

            return dataSource;
        }

        public static DataTable SplitName(DataTable dataSource, string field)
        {
            // When the field is null or empty then return
            if (string.IsNullOrEmpty(field))
            {
                return dataSource;
            }

            // check the field is exits in DataSource columns
            var isColumnDataSource = false;
            foreach (var column in dataSource.Columns)
            {
                if (field == column.ToString())
                {
                    isColumnDataSource = true;
                    break;
                }
            }

            // When the field is not exits in list columns of dataSource then return
            if (!isColumnDataSource)
            {
                return dataSource;
            }

            var ordinal = dataSource.Columns[field].Ordinal;
            var columnNumber = 1;

            // Prosess once row
            for (var i = 0; i < dataSource.Rows.Count; i++)
            {
                var parrentField = dataSource.Rows[i][field].ToString();
                var listValues = parrentField.Split(">>".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                while (columnNumber < listValues.Length)
                {
                    // Extend a new column
                    var newColumn = new DataColumn(field + " " + columnNumber, typeof(string));
                    dataSource.Columns.Add(newColumn);
                    dataSource.Columns[newColumn.ColumnName].SetOrdinal(ordinal + columnNumber);
                    columnNumber++;
                }

                for (var j = 0; j < listValues.Length - 1; j++)
                {
                    // Set data for column
                    dataSource.Rows[i][ordinal + j + 1] = listValues[j];
                }
                dataSource.Rows[i][ordinal] = listValues[listValues.Length - 1];
            }
            return dataSource;
        }

        /// <summary>
        /// Finds the left like of compareValue in the source array.
        /// </summary>
        /// <param name="compareValue">The compare value.</param>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public static string FindLeftLike(string compareValue, string[] source)
        {
            foreach (var s in source)
            {
                // When the length of compareValue larger than item of source then cancel
                if ((s == null) || (s.Length < compareValue.Length))
                {
                    continue;
                }

                // Compare the compareValue with the first string of source
                if (compareValue.ToLower() == s.Substring(0, compareValue.Length).ToLower())
                {
                    return s;
                }
            }

            // When th find is not found then return empty
            return "";
        }

        //===========Report Coverage===============
        public static void ReportCoverage(DataTable groupTable, DataTable branchTable, DataTable fsoTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 4;

            // For the groupTable
            DataTable dataSource = groupTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                Header(oSheet, rowIndex, true, "By Parent Department");
                rowIndex++;

                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 15, rowIndex);
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataSource, rowIndex);

                // Merge column
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // For the branchTabe
            dataSource = branchTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                Header(oSheet, rowIndex, false, "By Branch");
                rowIndex++;

                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 15, rowIndex);
                rowIndex++;

                // Write data to the sheet
                WriteData(oSheet, dataSource, rowIndex);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // For the fsoTable
            dataSource = fsoTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                Header(oSheet, rowIndex, false, "By Fsoft");
                rowIndex++;

                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 15, rowIndex);
                rowIndex++;

                // Write data to the sheet
                WriteData(oSheet, dataSource, rowIndex);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }
        }


        // Report Certificate Point
        public static void ReportCertificatePoint(DataTable groupTable, DataTable branchTable, DataTable fsoTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            oSheet.Name = "Certificate Point";
            var rowIndex = 4;

            // For the groupTable
            DataTable dataSource = groupTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                Header(oSheet, rowIndex, true, "Certificate Point By FSU");
                rowIndex++;

                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 15, rowIndex);
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataSource, rowIndex);

                // Merge column
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // For the branchTabe
            dataSource = branchTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                Header(oSheet, rowIndex, false, "Certificate Point By Branch");
                rowIndex++;

                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 15, rowIndex);
                rowIndex++;

                // Write data to the sheet
                WriteData(oSheet, dataSource, rowIndex);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // For the fsoTable
            dataSource = fsoTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                Header(oSheet, rowIndex, false, "Certificate Point By Fsoft");
                rowIndex++;

                // Format the title of the columns
                WriteTitleDefault(oSheet, dataSource, 15, rowIndex);
                rowIndex++;

                // Write data to the sheet
                WriteData(oSheet, dataSource, rowIndex);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }
        }

        private static void Header(Worksheet oSheet, int rowIndex, bool isGroup, string reportName)
        {
            // REPORT NAM: Merge three cells at the first by horizontal
            var mergeRange = Merge(oSheet, rowIndex - 1, 1, rowIndex - 1, 3);
            mergeRange.Value2 = reportName;
            mergeRange.ColumnWidth = 20;
            mergeRange.Font.Size = "12";
            mergeRange.Interior.ColorIndex = 6;
        }

        //=========== Report =============
        public static void ReportToExcel(DataTable groupTable, DataTable branchTable, DataTable fsoTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            DataTable dataSource;
            var rowIndex = 4;

            // Test color-----------------------
            //Range range;
            //for (int i = 0; i < 50; i++)
            //{
            //    range = Merger(oSheet, rowIndex - 2, i + 1, rowIndex - 2, i + 1);
            //    range.Interior.ColorIndex = i;
            //    range.Value2 = i;
            //}
            // End test color--------------------

            // For the groupTable
            dataSource = groupTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                WriteHeader(oSheet, rowIndex, true, "Báo cáo theo Group");
                rowIndex++;

                // Format the title of the columns
                WriteTitleReport1(oSheet, dataSource, rowIndex, true);
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataSource, rowIndex);

                // Merge column
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // For the branchTabe
            dataSource = branchTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                WriteHeader(oSheet, rowIndex, false, "Báo cáo theo Branch");
                rowIndex++;

                // Format the title of the columns
                WriteTitleReport1(oSheet, dataSource, rowIndex, false);
                rowIndex++;

                // Write data to the sheet
                WriteData(oSheet, dataSource, rowIndex);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // For the fsoTable
            dataSource = fsoTable;
            if ((dataSource != null) && (dataSource.Rows.Count > 0))
            {
                // Format the Header of the columns
                WriteHeader(oSheet, rowIndex, false, "Báo cáo theo FSO");
                rowIndex++;

                // Format the title of the columns
                WriteTitleReport1(oSheet, dataSource, rowIndex, false);
                rowIndex++;

                // Write data to the sheet
                WriteData(oSheet, dataSource, rowIndex);
                rowIndex = rowIndex + dataSource.Rows.Count + 3;
            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }
        }

        public static void Report4ToExcel(DataTable dataTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 4;

            if ((dataTable != null) && (dataTable.Rows.Count > 0))
            {
                // Format the title of the columns
                //WriteTitleGeneral(oSheet, dataTable, rowIndex);
                WriteTitle(oSheet, dataTable, rowIndex, "Monthly Training Report");
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataTable, rowIndex);

                // Merge data in the column 1
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count);
            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

        }

        public static void ReportTrainingContributionToExcel(DataTable dtBu, string filePath, DataTable dtBranch, DataTable dtFsoft)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;


            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 6;

            // show title of report table
            var titleExcel = (Range)oSheet.Cells[2, 1];
            titleExcel.Value2 = "TRAINING CONTRIBUTION";
            titleExcel.ColumnWidth = 20;
            titleExcel.Font.Bold = true;
            titleExcel.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            titleExcel.Font.Name = "Calibri";
            titleExcel.Font.Size = "20";
            //

            //----------------------------------show data report for table 1
            if ((dtBu != null) && (dtBu.Rows.Count > 0))
            {
                // show title of table 1
                var cell = (Range)oSheet.Cells[rowIndex - 2, 1];
                cell.Value2 = "Báo cáo theo Brach và Group";
                cell.ColumnWidth = 13.5;
                cell.Font.Bold = true;
                cell.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                cell.Font.Name = "Calibri";
                cell.Font.Size = "17";
                //

                // Format the title of the columns
                WriteTitleDefault(oSheet, dtBu, 42, rowIndex);
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dtBu, rowIndex);

                // merge all cell have same value in column 1
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count);
            }

            //----------------------------------show data report for table 2
            if (dtBranch != null && dtBranch.Rows.Count > 0)
            {
                rowIndex = dtBu.Rows.Count + 10;

                // show title of table 2
                var cell2 = (Range)oSheet.Cells[rowIndex - 2, 1];
                cell2.Value2 = "Báo cáo theo Branch";
                cell2.ColumnWidth = 13.5;
                cell2.Font.Bold = true;
                cell2.HorizontalAlignment = XlHAlign.xlHAlignLeft;
                cell2.Font.Name = "Calibri";
                cell2.Font.Size = "17";
                //

                WriteTitleDefault(oSheet, dtBranch, 42, rowIndex);
                rowIndex++;

                //show data table 3
                WriteData(oSheet, dtBranch, rowIndex);
            }

            //----------------------------------show data report for table 3
            if (dtFsoft != null && dtFsoft.Rows.Count > 0)
            {
                rowIndex = dtBu.Rows.Count + dtBranch.Rows.Count + 11;

                var rowFsoft = (Range)oSheet.Rows[rowIndex + 1];
                rowFsoft.EntireRow.ColumnWidth = 20;
                rowFsoft.EntireRow.Font.Bold = true;
                rowFsoft.EntireRow.Font.Size = "20";
                rowIndex++;

                WriteData(oSheet, dtFsoft, rowIndex);
            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }
        }

        public static void Create_Currency_Template(DataTable dataTable, string filePath)
        {
            var xlApp = new Application();
            var workBooksTemp = xlApp.Workbooks;
            var xlWorkbook = workBooksTemp.Open(filePath);

            Worksheet xlWorksheet = xlWorkbook.Sheets["Currency"];
            var xlRange = xlWorksheet.UsedRange;
            var rowIndex = 4;

            if ((dataTable != null) && (dataTable.Rows.Count > 0))
            {
                //Next row
                rowIndex++;

                // Write data to the sheet
                WriteData(xlWorksheet, dataTable, rowIndex);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(xlRange);
            Marshal.FinalReleaseComObject(xlWorksheet);
            Marshal.FinalReleaseComObject(workBooksTemp);

            xlWorkbook.Save();
            xlWorkbook.Close(false, Missing.Value, Missing.Value);

            Marshal.FinalReleaseComObject(xlWorkbook);

            xlApp.Application.Quit();
            xlApp.Quit();
            EndTask((IntPtr)xlApp.Hwnd, true, true);
            Marshal.FinalReleaseComObject(xlApp);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void Clear_Currency_Template(string filePath)
        {
            var xlApp = new Application();
            var workBooksTemp = xlApp.Workbooks;
            var xlWorkbook = workBooksTemp.Open(filePath);

            Worksheet xlWorksheet = xlWorkbook.Sheets["Currency"];
            var xlRange = xlWorksheet.UsedRange;

            xlRange.Value2 = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.FinalReleaseComObject(xlRange);
            Marshal.FinalReleaseComObject(xlWorksheet);
            Marshal.FinalReleaseComObject(workBooksTemp);

            xlWorkbook.Save();
            xlWorkbook.Close(false, Missing.Value, Missing.Value);

            Marshal.FinalReleaseComObject(xlWorkbook);

            xlApp.Application.Quit();
            xlApp.Quit();
            EndTask((IntPtr)xlApp.Hwnd, true, true);
            Marshal.FinalReleaseComObject(xlApp);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static void Report5ToExcel(DataTable dataTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 4;

            if ((dataTable != null) && (dataTable.Rows.Count > 0))
            {
                //Next row
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataTable, rowIndex);

                // Write data to the sheet
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count - 1);

                //Merge row
                Merge(oSheet, rowIndex, 2, rowIndex + 1, 2); //Row departmentName
                Merge(oSheet, rowIndex, dataTable.Columns.Count, rowIndex + 1, dataTable.Columns.Count);
                MergeRow(oSheet, rowIndex, 3, dataTable.Columns.Count - 3, 37, XlHAlign.xlHAlignCenter); //Row columnName

                //Set color
                for (int i = 3; i < dataTable.Columns.Count; i++)
                {
                    SetCellColor(oSheet, rowIndex + 1, i, 15);
                }

                rowIndex = rowIndex + rangeData.Rows.Count; //Row sum
                Merge(oSheet, rowIndex - 1, 1, rowIndex - 1, 2);
            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

        }

        public static void Report6ToExcel(List<DataTable> listData, DataTable headerTable, string filePath)
        {
            if (listData == null || listData.Count < 1 || headerTable == null)
            {
                // Data input not correct
                return;
            }
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = listData.Count;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 4;
            DataTable dataSource;

            for (int k = 1; k < listData.Count + 1; k++)
            {
                oSheet = (Worksheet)oSheets.Item[k];
                dataSource = listData[k - 1];
                if ((dataSource != null) && (dataSource.Rows.Count > 0))
                {
                    oSheet.Activate();
                    //Next row
                    rowIndex = 4;

                    // Write title
                    WriteData(oSheet, headerTable, rowIndex);

                    // Write header
                    MergeRow(oSheet, rowIndex, 1, dataSource.Columns.Count, 37, XlHAlign.xlHAlignCenter); //Row columnName
                    rowIndex += 2;

                    // Write data to the sheet
                    var rangeData = WriteData(oSheet, dataSource, rowIndex);

                    // Write data to the sheet
                    MergeColum(oSheet, 1, rowIndex, rangeData.Rows.Count);
                    MergeColum(oSheet, 2, rowIndex, rangeData.Rows.Count);

                    //Merge row
                    //Merge(oSheet, rowIndex, 2, rowIndex + 1, 2); //Row departmentName
                    //Merge(oSheet, rowIndex, dataSource.Columns.Count, rowIndex + 1, dataSource.Columns.Count);


                    //Set color
                    for (int i = 3; i < dataSource.Columns.Count + 1; i++)
                    {
                        SetCellColor(oSheet, rowIndex - 1, i, 15);
                    }

                    //rowIndex = rowIndex + rangeData.Rows.Count; //Row sum
                    //Merge(oSheet, rowIndex - 1, 1, rowIndex - 1, 2);
                }
            }



            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

        }

        public static void Report7ToExcel(DataTable dataTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 4;

            if ((dataTable != null) && (dataTable.Rows.Count > 0))
            {

                // Write title
                WriteTitleDefault(oSheet, dataTable, 37, rowIndex);

                //Next row
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataTable, rowIndex);

                // Write data to the sheet
                MergeColum(oSheet, 1, rangeData.Row, rangeData.Rows.Count);
                MergeColum(oSheet, 2, rangeData.Row, rangeData.Rows.Count);

            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

        }

        public static void Report8ToExcel(DataTable dataTable, string filePath)
        {
            var oExcel = new Application { Visible = true, DisplayAlerts = false };
            oExcel.Application.SheetsInNewWorkbook = 1;

            var oBook = oExcel.Workbooks.Add(Type.Missing);
            var oSheets = oBook.Worksheets;
            var oSheet = (Worksheet)oSheets.Item[1];
            var rowIndex = 4;

            if ((dataTable != null) && (dataTable.Rows.Count > 0))
            {

                Header(oSheet, rowIndex, true, "Training Duration");

                // Write title
                WriteTitleDefault(oSheet, dataTable, 37, rowIndex);

                //Next row
                rowIndex++;

                // Write data to the sheet
                var rangeData = WriteData(oSheet, dataTable, rowIndex);

            }

            // Check SaveAs function to notified that: Can't access to filePath
            try
            {
                oBook.SaveAs(filePath, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, false, false,
                    XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch (Exception)
            {
                // Notified that: Can't access to filePath
                throw;
            }

        }

        private static void WriteTitleDefault(Worksheet oSheet, DataTable dataSource, int colorIndex, int rowIndex)
        {
            // Set the name of the columns by the ColumnNames of DataTable
            Range cell;
            var i = 1;
            foreach (DataColumn column in dataSource.Columns)
            {
                cell = (Range)oSheet.Cells[rowIndex, i];
                var columns = (Range)oSheet.Columns[i];
                if (column.ColumnName.IndexOf("Date") != -1)
                {
                    columns.NumberFormat = "DD/MMM/YYYY";
                }
                cell.Value2 = column.ColumnName;
                cell.ColumnWidth = 13.5;
                cell.Font.Bold = true;
                cell.Borders.LineStyle = Constants.xlSolid;
                cell.Interior.ColorIndex = colorIndex;

                cell.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                cell.Font.Name = "Calibri";
                cell.Font.Size = "10";
                i++;
            }
        }

        private static void WriteHeader(Worksheet oSheet, int rowIndex, bool isGroup, string reportName)
        {
            // REPORT NAM: Merge three cells at the first by horizontal
            var mergeRange = Merge(oSheet, rowIndex - 1, 1, rowIndex - 1, 3);
            mergeRange.Value2 = reportName;
            mergeRange.ColumnWidth = 13.5;
            mergeRange.Font.Size = "12";
            mergeRange.Interior.ColorIndex = 0;

            // The first
            mergeRange = Merge(oSheet, rowIndex, 1, rowIndex + 1, 1);
            mergeRange.Value2 = "Index by Branch";
            mergeRange.Font.Size = "9";
            mergeRange.ColumnWidth = 20;
            mergeRange.Interior.ColorIndex = 0;

            var permanentStaffIndex = 2;
            if (isGroup)
            {
                // The three
                mergeRange = Merge(oSheet, rowIndex, 2, rowIndex + 1, 2);
                mergeRange.Value2 = "Index by Group";
                mergeRange.ColumnWidth = 20;
                mergeRange.RowHeight = 30;
                mergeRange.Interior.ColorIndex = 0;

                permanentStaffIndex = 3;
            }

            // The next
            mergeRange = Merge(oSheet, rowIndex, permanentStaffIndex, rowIndex, permanentStaffIndex);
            mergeRange.Value2 = "Permanent staff";
            mergeRange.Interior.ColorIndex = 34;
            mergeRange.RowHeight = 30;

            // Group three cells first
            mergeRange = Merge(oSheet, rowIndex, permanentStaffIndex + 1, rowIndex, permanentStaffIndex + 3);
            mergeRange.Value2 = "";
            mergeRange.Interior.ColorIndex = 34;

            // Group three cells second
            mergeRange = Merge(oSheet, rowIndex, permanentStaffIndex + 4, rowIndex, permanentStaffIndex + 6);
            mergeRange.Value2 = "Sum with weight";
            mergeRange.Interior.ColorIndex = 43;

            // Group three cells second
            mergeRange = Merge(oSheet, rowIndex, permanentStaffIndex + 7, rowIndex, permanentStaffIndex + 9);
            mergeRange.Value2 = "Staff with weight";
            mergeRange.Interior.ColorIndex = 41;

            // Group three cells second
            mergeRange = Merge(oSheet, rowIndex, permanentStaffIndex + 10, rowIndex, permanentStaffIndex + 12);
            mergeRange.Value2 = "Index";
            mergeRange.Interior.ColorIndex = 6;

            // Group three cells second
            mergeRange = Merge(oSheet, rowIndex, permanentStaffIndex + 13, rowIndex, permanentStaffIndex + 13);
            mergeRange.Value2 = "Index";
            mergeRange.Interior.ColorIndex = 3;
        }

        private static void WriteTitleReport1(Worksheet oSheet, DataTable dataSource, int rowIndex, bool isGroup)
        {
            WriteTitleDefault(oSheet, dataSource, 43, rowIndex);

            var permanentStaffIndex = 2;
            if (isGroup)
            {
                permanentStaffIndex = 3;
            }
            // Set color for cell
            SetCellColor(oSheet, rowIndex, permanentStaffIndex, 6);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 1, 6);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 2, 6);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 3, 6);

            // Set color for cell
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 4, 4);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 5, 4);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 6, 4);

            // Set color for cell
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 7, 10);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 8, 10);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 9, 10);

            // Set color for cell
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 10, 37);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 11, 37);
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 12, 37);

            // Set color for cell
            SetCellColor(oSheet, rowIndex, permanentStaffIndex + 13, 15);
        }

        private static void WriteTitle(Worksheet oSheet, DataTable dataSource, int rowIndex, string reportName)
        {
            // REPORT NAM: Merge three cells at the first by horizontal
            var mergeRange = Merge(oSheet, rowIndex - 1, 1, rowIndex - 1, 3);
            mergeRange.Value2 = reportName;
            mergeRange.ColumnWidth = 13.5;
            mergeRange.Font.Size = "12";
            mergeRange.Interior.ColorIndex = 0;

            WriteTitleDefault(oSheet, dataSource, 43, rowIndex);
        }

        private static void SetCellColor(Worksheet oSheet, int rowIndex, int columIndex, int colorIndex)
        {
            var cell = (Range)oSheet.Cells[rowIndex, columIndex];
            cell.Interior.ColorIndex = colorIndex;
        }

        private static Range WriteData(Worksheet oSheet, DataTable dataSource, int rowIndex)
        {
            // Set the name of Worksheet by the name of DataTable
            if (!string.IsNullOrEmpty(dataSource.TableName))
            {
                oSheet.Name = dataSource.TableName;
            }

            // Forward data from dataSource to sourceArray
            var sourceArray = new object[dataSource.Rows.Count, dataSource.Columns.Count];

            for (var i = 0; i < dataSource.Rows.Count; i++)
            {
                var row = dataSource.Rows[i];
                for (var j = 0; j < dataSource.Columns.Count; j++)
                {
                    // Extend two character into first of columnName
                    var colulumnName = "12" + dataSource.Columns[j].ColumnName;
                    var lastCharacters = colulumnName.Substring(colulumnName.Length - 2, 2);
                    if (lastCharacters.Equals("Id"))
                    {
                        sourceArray[i, j] = "'" + row[j];
                    }
                    else
                    {
                        sourceArray[i, j] = row[j];
                    }
                }
            }

            // Create a range fit the sourceArray
            int rowFirst = rowIndex;
            const int columnFirst = 1;

            var rowLast = rowFirst + dataSource.Rows.Count - 1;
            var columnLast = dataSource.Columns.Count;

            var cellFirst = (Range)oSheet.Cells[rowFirst, columnFirst];
            var cellLast = (Range)oSheet.Cells[rowLast, columnLast];
            var rangeData = oSheet.Range[cellFirst, cellLast];

            // Export data from sourceArray to the range in excel file
            rangeData.Value2 = sourceArray;
            rangeData.Borders.LineStyle = Constants.xlSolid;

            return rangeData;
        }

        private static void MergeColum(Worksheet oSheet, int columnPosition, int startRow, int rowNumber)
        {
            Range cellCurrent;

            int i = startRow;
            var cellFirst = oSheet.Cells[i, columnPosition];
            int cellFirstIndex = i;
            int cellLastIndex = cellFirstIndex;
            while (i < startRow + rowNumber)
            {
                cellCurrent = oSheet.Cells[i, columnPosition];
                var value = cellFirst.Value2;
                if (cellCurrent.Value2 != cellFirst.Value2)
                {
                    Merge(oSheet, cellFirstIndex, columnPosition, cellLastIndex, columnPosition);

                    cellFirst = cellCurrent;
                    cellFirstIndex = i;

                }
                cellLastIndex = i;
                i++;
            }
            Merge(oSheet, cellFirstIndex, columnPosition, cellLastIndex, columnPosition);
        }

        private static void MergeRow(Worksheet oSheet, int rowPosition, int startColumn, int columNumber, int indexColor, XlHAlign Align)
        {
            Range cellCurrent;

            int i = startColumn;
            var cellFirst = oSheet.Cells[rowPosition, i];
            int cellFirstIndex = i;
            int cellLastIndex = cellFirstIndex;
            Range mergeRange;
            Range cellDown;
            while (i < startColumn + columNumber)
            {
                cellCurrent = oSheet.Cells[rowPosition, i];
                cellDown = oSheet.Cells[rowPosition + 1, i];
                if (String.IsNullOrEmpty(cellCurrent.Value2))
                {

                    //Merge parent
                    mergeRange = oSheet.Range[cellCurrent, cellDown];
                    mergeRange.Merge();

                    //Merge group
                    mergeRange = Merge(oSheet, rowPosition, cellFirstIndex, rowPosition, cellLastIndex);
                    mergeRange.HorizontalAlignment = Align;
                    mergeRange.VerticalAlignment = Align;
                    mergeRange.Interior.ColorIndex = indexColor;
                    cellFirst = oSheet.Cells[rowPosition, i];
                    cellFirstIndex = i;
                }
                else
                {
                    if (cellFirst.Value2 != null)
                    {
                        if (cellCurrent.Value2 != cellFirst.Value2)
                        {
                            //Merge group
                            mergeRange = Merge(oSheet, rowPosition, cellFirstIndex, rowPosition, cellLastIndex);
                            mergeRange.HorizontalAlignment = Align;
                            mergeRange.VerticalAlignment = Align;
                            mergeRange.Interior.ColorIndex = indexColor;
                            cellFirst = cellCurrent;
                            cellFirstIndex = i;
                        }
                    }
                }
                cellLastIndex = i;
                i++;
            }
            mergeRange = Merge(oSheet, rowPosition, cellFirstIndex, rowPosition, cellLastIndex);
            mergeRange.HorizontalAlignment = Align;
            mergeRange.VerticalAlignment = Align;
            mergeRange.Interior.ColorIndex = indexColor;
        }

        private static Range Merge(Worksheet oSheet, int firstRow, int firstColumn, int lastRow, int lastColumn)
        {
            var startRange = (Range)oSheet.Cells[firstRow, firstColumn];
            var endRange = (Range)oSheet.Cells[lastRow, lastColumn];
            var mergeRange = oSheet.Range[startRange, endRange];
            mergeRange.Merge();

            mergeRange.ColumnWidth = 15;
            mergeRange.Font.Bold = true;
            mergeRange.Borders.LineStyle = Constants.xlSolid;
            //mergeRange.Interior.ColorIndex = 15;
            mergeRange.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            mergeRange.VerticalAlignment = XlVAlign.xlVAlignCenter;
            mergeRange.Font.Name = "Tahoma";
            mergeRange.Font.Size = "9";

            return mergeRange;
        }


    }
}
