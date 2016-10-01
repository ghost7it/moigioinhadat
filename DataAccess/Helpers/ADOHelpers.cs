using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace DataAccess.Helpers
{
    /// <summary>
    /// Author: Le Viet Nam
    /// </summary>
    public class ADOHelpers
    {
        #region return DataTable SQL for SQLserver
        //Query returns DataTable
        public static DataTable GetSQLDataTable(string connectionString, string sql, List<SqlParameter> parameters)
        {
            var conn = new SqlConnection(connectionString);
            var da = new SqlDataAdapter();
            var myCommand = conn.CreateCommand();
            myCommand.CommandText = sql;
            foreach (var p in parameters)
                myCommand.Parameters.Add(p);
            da.SelectCommand = myCommand;
            var dt = new DataTable();
            conn.Open();
            da.Fill(dt);
            conn.Close();
            return dt;
        }
        public static DataTable GetSQLDataTable(string connectionString, string sql)
        {
            DataTable dt;
            using (var conn = new SqlConnection(connectionString))
            {
                var da = new SqlDataAdapter();
                var myCommand = conn.CreateCommand();
                myCommand.CommandText = sql;
                da.SelectCommand = myCommand;
                dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();
            }
            return dt;
        }
        #endregion

        #region return DataSet SQL for SQLserver

        //Query returns dataset
        public static DataSet GetSQLDataSet(string connectionString, string sql, List<SqlParameter> parameters)
        {
            DataSet ds;
            using (var conn = new SqlConnection(connectionString))
            {
                var da = new SqlDataAdapter();
                var myCommand = conn.CreateCommand();
                myCommand.CommandText = sql;
                foreach (var p in parameters)
                    myCommand.Parameters.Add(p);
                da.SelectCommand = myCommand;
                ds = new DataSet();
                conn.Open();
                da.Fill(ds);
                conn.Close();
            }
            return ds;
        }

        public static DataSet GetSQLDataSet(string connectionString, List<string> sqls, List<SqlParameter> parameters)
        {
            DataSet ds;
            using (var conn = new SqlConnection(connectionString))
            {
                var da = new SqlDataAdapter();
                var myCommand = conn.CreateCommand();
                ds = new DataSet();
                conn.Open();
                foreach (var sql in sqls)
                {
                    myCommand.CommandText = sql;
                    foreach (SqlParameter p in parameters)
                        myCommand.Parameters.Add(p);
                    da.SelectCommand = myCommand;

                    da.Fill(ds);
                }
                conn.Close();
            }
            return ds;
        }

        public static DataSet GetSQLDataSet(string connectionString, string sql)
        {
            DataSet ds;
            using (var conn = new SqlConnection(connectionString))
            {
                var da = new SqlDataAdapter();
                SqlCommand myCommand = conn.CreateCommand();
                myCommand.CommandText = sql;
                da.SelectCommand = myCommand;
                ds = new DataSet();
                conn.Open();
                da.Fill(ds);
                conn.Close();
            }
            return ds;
        }

        public static DataSet GetSQLDataSet(string connectionString, List<string> sqls)
        {
            DataSet ds;
            using (var conn = new SqlConnection(connectionString))
            {
                var da = new SqlDataAdapter();
                var myCommand = conn.CreateCommand();
                ds = new DataSet();
                conn.Open();
                foreach (string sql in sqls)
                {
                    myCommand.CommandText = sql;
                    da.SelectCommand = myCommand;
                    da.Fill(ds);
                }
                conn.Close();
            }
            return ds;
        }

        #endregion

        #region ExecuteNonQuery SQLserver

        //Non query
        public static int ExecuteNonQuerySQL(string connectionString, string sql)
        {
            int result;
            using (var conn = new SqlConnection(connectionString))
            {
                var myCommand = new SqlCommand(sql, conn);
                myCommand.Connection.Open();
                result = myCommand.ExecuteNonQuery();
                conn.Close();
            }
            return result;
        }

        public static int ExecuteNonQuerySQL(string connectionString, string sql, List<SqlParameter> parameters)
        {
            int result;
            using (var conn = new SqlConnection(connectionString))
            {
                var myCommand = new SqlCommand(sql, conn);

                foreach (var p in parameters)
                    myCommand.Parameters.Add(p);

                myCommand.Connection.Open();
                result = myCommand.ExecuteNonQuery();
                conn.Close();
            }
            return result;
        }

        #endregion

        #region ExecuteScalar SQLserver

        //Return a single value
        public static object ExecuteScalarSQL(string connectionString, string sql)
        {
            object result;
            using (var conn = new SqlConnection(connectionString))
            {
                var myCommand = new SqlCommand(sql, conn);
                myCommand.Connection.Open();
                result = myCommand.ExecuteScalar();
                conn.Close();
            }
            return result;
        }

        public static object ExecuteScalarSQL(string connectionString, string sql, List<SqlParameter> _parameters)
        {
            object result;
            using (var conn = new SqlConnection(connectionString))
            {
                var myCommand = new SqlCommand(sql, conn);

                foreach (var p in _parameters)
                    myCommand.Parameters.Add(p);
                myCommand.Connection.Open();
                result = myCommand.ExecuteScalar();
                conn.Close();
            }
            return result;
        }

        #endregion
    }
}
