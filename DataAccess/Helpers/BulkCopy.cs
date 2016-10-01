using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    public class SynchronizationHelpers : IDisposable
    {
        private static SqlConnection _sqlConnection;
        private static string _connectionString;
        public static string GetConnectionString(string univId = "")
        {
            string connectionString = "";
            if (univId == "")
                connectionString = ConfigurationManager.ConnectionStrings["Sql2k5ConnectionString"].ConnectionString;
            else
                connectionString = ConfigurationManager.ConnectionStrings["userDataDH_" + univId].ConnectionString;
            return connectionString;
        }
        public static string GetDbName(string isTN, string univId)
        {
            // isTN = 1 - data tốt nghiệp
            // isTN = 0 - data đang học
            string giatri = "";
            if (isTN == "1") { giatri = "userDataTN_" + univId; }
            else { giatri = "userDataDH_" + univId; }

            string connectionString = ConfigurationManager.ConnectionStrings[giatri].ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
        public static string GetDbPortalName(string isTN)
        {
            string giatri = "";
            if (isTN == "1") { giatri = "ConnectionString_Backup_TN"; }
            else { giatri = "ConnectionString_Backup"; }

            string connectionString = ConfigurationManager.ConnectionStrings[giatri].ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
        public static SqlConnection CreateConnection(string univId)
        {
            try
            {
                //_connectionString = "";//Lấy theo univId ở đây
                _connectionString = GetConnectionString(univId);
                if (_sqlConnection != null)
                {//Đóng kết nối
                    _sqlConnection.Close();
                }
                if (_connectionString != null && _sqlConnection == null)
                {
                    _sqlConnection = new SqlConnection(_connectionString);
                }

                if (_sqlConnection.State == ConnectionState.Closed)
                {
                    _sqlConnection.Open();
                }
            }
            catch (SqlException)
            {
            }
            return _sqlConnection;
        }
        public static void CloseConnection()
        {
            try
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    CloseConnection();
                }
            }
            catch (SqlException)
            {
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_sqlConnection != null)
                {
                    if (_sqlConnection.State == ConnectionState.Open)
                    {
                        CloseConnection();
                    }

                    _sqlConnection.Dispose();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public DataTable RetrieveData(SqlCommand sqlCommand, string univId)
        {
            _sqlConnection = CreateConnection(univId);
            sqlCommand.Connection = _sqlConnection;
            var dataAdapter = new SqlDataAdapter(sqlCommand);
            var table = new DataTable { Locale = CultureInfo.CurrentCulture };
            try
            {
                dataAdapter.Fill(table);
            }
            catch (SqlException)
            {
                return null;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return table;
        }
        public static int ExecuteNonQuery(string sql, string univId)
        {
            _sqlConnection = CreateConnection(univId);
            SqlCommand sqlCommand = new SqlCommand(sql, _sqlConnection);
            try
            {
                return sqlCommand.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                _sqlConnection.Close();
            }
            return -1;
        }

        /// <summary>
        /// Chạy lệnh sql đầy đủ (execution full client-side SQL scripts)
        /// </summary>
        /// <param name="script"></param>
        /// <param name="dbName"></param>
        public static void ExecuteSqlScript(string script, string dbName)
        {
            try
            {
                _sqlConnection = CreateConnection("");
                _sqlConnection.ChangeDatabase(dbName);
                var serverConnection = new Microsoft.SqlServer.Management.Common.ServerConnection(_sqlConnection);
                var server = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
                server.ConnectionContext.ExecuteNonQuery(script);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlConnection.Close();
            }
        }
        /// <summary>
        /// Xóa và tạo database tạm để lấy dữ liệu về
        /// </summary>
        /// <param name="isTN"></param>
        /// <param name="univId"></param>
        /// <returns></returns>
        public static string CreateTemporaryDatabase(string isTN, string univId)
        {
            string dbTmp = ConfigurationManager.AppSettings["DataTmpLDL"];
            string dbName = dbTmp + GetDbName(isTN, univId);
            try
            {
                //Xóa database tạm nếu có
                var cmd = String.Format(@"
                    USE [Master]; 
                    ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE {0};"
                    , dbName);
                ExecuteNonQuery(cmd, "");
                //Tạo database tạm
                var cmd1 = string.Format(@"
                    USE [Master];
                    IF DB_ID (N'{0}') IS NULL CREATE DATABASE {0};"
                    , dbName);
                ExecuteNonQuery(cmd1, "");
                return dbName;
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Generate lệnh tạo bảng
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string ScriptingCreateTable(string connectionString, List<string> tablesName)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                var srvConn = new ServerConnection
                {
                    ServerInstance = builder.DataSource,
                    LoginSecure = false,
                    Login = builder.UserID,
                    Password = builder.Password
                };
                var server = new Microsoft.SqlServer.Management.Smo.Server(srvConn);
                string dbName = builder.InitialCatalog;
                var databse = server.Databases[dbName];
                var sb = new StringBuilder();
                var scripter = new Scripter(server);
                scripter.Options.ScriptDrops = false;
                scripter.Options.WithDependencies = false;
                scripter.Options.IncludeHeaders = false;
                scripter.Options.AllowSystemObjects = false;
                scripter.Options.ScriptOwner = false;
                scripter.Options.Triggers = false;
                scripter.Options.FullTextIndexes = false;
                scripter.Options.NonClusteredIndexes = false;
                scripter.Options.NoCollation = true;
                scripter.Options.Bindings = false;
                scripter.Options.SchemaQualify = true;
                scripter.Options.IncludeIfNotExists = true;
                var smoObjects = new Urn[1];
                foreach (string table in tablesName)
                {
                    Table t = databse.Tables[table];
                    smoObjects[0] = t.Urn;
                    if (t.IsSystemObject == false)
                    {
                        StringCollection sc = scripter.Script(smoObjects);
                        foreach (var st in sc)
                        {
                            sb.Append(st + " ");
                        }
                    }
                }
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Generate lệnh tạo bảng
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static TableScriptAndColumnMapping ScriptingCreateTableAndColumnMapping(string connectionString, string tableName, bool map = true, bool list = true)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                var srvConn = new ServerConnection
                {
                    ServerInstance = builder.DataSource,
                    LoginSecure = false,
                    Login = builder.UserID,
                    Password = builder.Password
                };
                var server = new Microsoft.SqlServer.Management.Smo.Server(srvConn);
                string dbName = builder.InitialCatalog;
                var databse = server.Databases[dbName];
                var sb = new StringBuilder();
                var scripter = new Scripter(server);
                scripter.Options.ScriptDrops = false;
                scripter.Options.WithDependencies = false;
                scripter.Options.IncludeHeaders = false;
                scripter.Options.AllowSystemObjects = false;
                scripter.Options.ScriptOwner = false;
                scripter.Options.Triggers = false;
                scripter.Options.FullTextIndexes = false;
                scripter.Options.NonClusteredIndexes = false;
                scripter.Options.NoCollation = true;
                scripter.Options.Bindings = false;
                scripter.Options.SchemaQualify = true;
                scripter.Options.IncludeIfNotExists = true;
                var smoObjects = new Urn[1];
                var columnMapping = new List<string>();
                var listColumn = new StringBuilder();
                Table t = databse.Tables[tableName];
                smoObjects[0] = t.Urn;
                if (t.IsSystemObject == false)
                {
                    StringCollection sc = scripter.Script(smoObjects);
                    foreach (var st in sc)
                    {
                        sb.Append(st + " ");
                    }
                    if (map || list)
                    {
                        foreach (Column column in t.Columns)
                        {
                            if (map)
                                columnMapping.Add(column.Name + "," + column.Name);
                            if (list)
                                listColumn.Append(column.Name + ",");
                        }
                    }
                }
                TableScriptAndColumnMapping tableScriptAndColumnMapping = new TableScriptAndColumnMapping()
                {
                    TableScript = sb.ToString(),
                    ColumnMapping = columnMapping,
                    ListColumn = listColumn.ToString()
                };
                return tableScriptAndColumnMapping;
            }
            catch
            {
                return new TableScriptAndColumnMapping();
            }
        }
        #region BulkCopy
        /// <summary>
        /// SqlBulkCopy từ linq query, có sử dụng columnMapper
        /// Dữ liệu từ LinqQuery sẽ được convert về DataTable nên chạy rất chậm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="columnMapping"></param>
        public static void BulkInsertAll<T>(IEnumerable<T> entities, string dbName, string tableName, List<string> columnMapping)
        {
            try
            {
                _sqlConnection = CreateConnection("");
                _sqlConnection.ChangeDatabase(dbName);
                entities = entities.ToArray();
                Type t = typeof(T);
                var bulkCopy = new SqlBulkCopy(_sqlConnection)
                {
                    BulkCopyTimeout = 2147483647,
                    DestinationTableName = tableName
                };
                var properties = t.GetProperties().Where(EventTypeFilter).ToArray();
                var table = new DataTable();
                foreach (var property in properties)
                {
                    Type propertyType = property.PropertyType;
                    if (propertyType.IsGenericType &&
                        propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propertyType = Nullable.GetUnderlyingType(propertyType);
                    }

                    table.Columns.Add(new DataColumn(property.Name, propertyType));
                }
                foreach (var entity in entities)
                {
                    table.Rows.Add(properties.Select(
                      property => GetPropertyValue(
                      property.GetValue(entity, null))).ToArray());
                }
                foreach (var mapping in columnMapping)
                {
                    var split = mapping.Split(new[] { ',' });
                    bulkCopy.ColumnMappings.Add(split.First(), split.Last());
                }
                bulkCopy.BatchSize = 1000;
                bulkCopy.WriteToServer(table);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlConnection.Close();
            }
        }
        private static bool EventTypeFilter(System.Reflection.PropertyInfo p)
        {
            var attribute = Attribute.GetCustomAttribute(p,
                typeof(AssociationAttribute)) as AssociationAttribute;
            if (attribute == null) return true;
            if (attribute.IsForeignKey == false) return true;
            return false;
        }
        private static object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }
        /// <summary>
        /// SqlBulkCopy từ sql data reader, nên dùng hàm vì không phải convert dữ liệu
        /// trước khi insert, chạy cực nhanh
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        public static void BulkCopy(SqlDataReader reader, string dbName, string tableName)
        {
            try
            {
                _sqlConnection = CreateConnection("");
                _sqlConnection.ChangeDatabase(dbName);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_sqlConnection))
                {
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.BulkCopyTimeout = 2147483647;
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlConnection.Close();
            }
        }
        /// <summary>
        /// Insert dữ liệu từ DataTable vào bảng trong SQL, sử dụng SqlBulkCopy
        /// Trước khi dùng hàm phải convert dữ liệu về dạng dataTable => chạy rất chậm
        /// </summary>
        /// <param name="table"></param>
        /// <param name="univId"></param>
        /// <param name="tableName"></param>
        /// <param name="dbName"></param>
        public static void SqlBulkCopy(DataTable table, string tableName, string dbName)
        {
            try
            {
                _sqlConnection = CreateConnection("");
                _sqlConnection.ChangeDatabase(dbName);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_sqlConnection))
                {
                    bulkCopy.BulkCopyTimeout = 2147483647;
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.DestinationTableName = "dbo." + tableName;
                    try
                    {
                        bulkCopy.WriteToServer(table);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _sqlConnection.Close();
            }
        }
        #endregion
    }
    public class TableScriptAndColumnMapping
    {
        public string TableScript { get; set; }
        public List<string> ColumnMapping { get; set; }
        public string ListColumn { get; set; }
    }
}
