using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace DataAccess.Helpers
{
    public class SqlHelpers : IDisposable
    {
        #region Property
        public static string ErrorMessage { get; set; }
        protected static bool ThrowError { set; get; }
        private static SqlConnection _sqlConnection;
        private readonly string _connectionString;
        #endregion
        public SqlHelpers()
        {
            // When debuging then set ThrowError properties is true, when packaging the set this properties is false
            ThrowError = false;

            ErrorMessage = string.Empty;
            try
            {
                if (_connectionString == null)
                {
                    //_connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    //_connectionString = ((System.Data.EntityClient.EntityConnection)ModelInstance.Connection).StoreConnection.ConnectionString;
                    string entityConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                    _connectionString = new EntityConnectionStringBuilder(entityConnectionString).ProviderConnectionString;

                }

                if (_sqlConnection == null)
                {
                    _sqlConnection = new SqlConnection(_connectionString);
                }
            }
            catch (NullReferenceException nullReferenceException)
            {
                ErrorMessage = nullReferenceException.Message;
            }
            catch (SqlException sqlException)
            {
                ErrorMessage = sqlException.Message;
            }
            catch
            {
                ErrorMessage = "Some Error";
                throw;
            }
        }
        /// <summary>
        /// CreateConnection
        /// </summary>
        /// <returns> SqlConnection with status Open </returns>
        public SqlConnection CreateConnection()
        {
            try
            {
                if (_connectionString != null && _sqlConnection == null)
                {
                    _sqlConnection = new SqlConnection(_connectionString);
                }

                if (_sqlConnection.State == ConnectionState.Closed)
                {
                    _sqlConnection.Open();
                }
            }
            catch (SqlException sqlException)
            {
                ErrorMessage = sqlException.Message;
            }

            return _sqlConnection;
        }
        /// <summary>
        /// Close connection
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                }
            }
            catch (SqlException sqlException)
            {
                ErrorMessage = sqlException.Message;
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if (_sqlConnection != null)
                {
                    if (_sqlConnection.State == ConnectionState.Open)
                    {
                        _sqlConnection.Close();
                    }

                    _sqlConnection.Dispose();
                }
            }
            // free native resources
        }
        /// <summary>
        /// Dispose
        /// </summary>
        /// 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #region Protected Method
        /// <summary>
        /// Add rows in a specified range in DataTable to match those in data source using the sqlCommand.
        /// Return null when occur error.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>DataTable</returns>
        public static DataTable RetrieveData(SqlCommand sqlCommand)
        {
            sqlCommand.Connection = _sqlConnection;
            var dataAdapter = new SqlDataAdapter(sqlCommand);
            var table = new DataTable { Locale = CultureInfo.CurrentCulture };

            // When is building, need to throw error
            if (ThrowError)
            {
                dataAdapter.Fill(table);
                return table;
            }

            // When is packaging, need catch the error
            try
            {
                dataAdapter.Fill(table);
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return null;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return table;
        }
        /// <summary>
        /// Executes a Transact-SQL statement against the connection and return the number of rows affected.
        /// Return -1 when occur error.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>System.Int32.</returns>
        public int ExecuteNonQuery(SqlCommand sqlCommand)
        {
            sqlCommand.Connection = _sqlConnection;
            // When is building, need to throw error
            if (ThrowError)
            {
                _sqlConnection.Open();
                var result = sqlCommand.ExecuteNonQuery();
                _sqlConnection.Close();
                return result;
            }
            // When is packaging, need catch the error
            try
            {
                _sqlConnection.Open();
                return sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return -1;
        }

        /// <summary>
        /// Executes the query, and return the first column of the first rows in the result set returned by the query.
        /// Additional columns or rows are ignored.
        /// Return -1 when occur error.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>System.Object.</returns>
        public object ExecuteScalar(SqlCommand sqlCommand)
        {
            object result;
            sqlCommand.Connection = _sqlConnection;
            if (ThrowError)
            {
                _sqlConnection.Open();
                result = sqlCommand.ExecuteScalar();
                _sqlConnection.Close();
                return result;
            }
            try
            {
                _sqlConnection.Open();
                result = sqlCommand.ExecuteScalar();
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return null;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return result;
        }
        #endregion
        #region Virtual Method
        /// <summary>
        /// Executes a Transact-SQL statement against the connection to insert specified entity into database 
        /// and return the number of rows affected.
        /// Return -1 when occur error.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>System.Int32.</returns>
        public virtual int Insert(object entity)
        {
            return 0;
        }
        /// <summary>
        /// Executes a Transact-SQL statement against the connection to update specified entity into database 
        /// and return the number of rows affected.
        /// Return -1 when occur error.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>System.Int32.</returns>
        public virtual int Update(object entity)
        {
            return 0;
        }
        /// <summary>
        /// Executes a Transact-SQL statement against the connection to delete record in database is specified  by recordId
        /// and return the number of rows affected.
        /// Return -1 when occur error.
        /// </summary>
        /// <param name="recordId">The record id.</param>
        /// <returns>System.Int32.</returns>
        public virtual int Delete(object recordId)
        {
            return 0;
        }
        /// <summary>
        /// Executes the query, and return the first column of the first rows in the result set returned by the recordName.
        /// Additional columns or rows are ignored.
        /// Return null when occur error.
        /// </summary>
        /// <param name="recordName">Name of the record.</param>
        /// <returns>System.Object.</returns>
        public virtual object RetrieveId(string recordName)
        {
            return null;
        }
        /// <summary>
        /// Add a row in a specified range in DataTable to match those in data source mapped with recordId.
        /// Return null when occur error.
        /// </summary>
        /// <param name="recordId">The record id.</param>
        /// <returns>DataTable.</returns>
        public virtual DataTable RetrieveOne(object recordId)
        {
            return null;
        }
        /// <summary>
        /// Add all rows in a specified range in DataTable to match those in data source using the sqlCommand.
        /// Return null when occur error.
        /// </summary>
        /// <returns>DataTable.</returns>
        public virtual DataTable RetrieveAll()
        {
            return null;
        }
        /// <summary>
        /// Add rows in a specified range in DataTable to match those in data source mapped with the properties of entity.
        /// Return null when occur error.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>DataTable.</returns>
        public virtual DataTable Search(object entity)
        {
            return null;
        }
        #endregion
    }
}
