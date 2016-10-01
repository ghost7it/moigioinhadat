using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    //NamMV: Lấy cáu này ở Project Tranning Tool
   public class SqlStored : SqlProvider
    {
        private SqlConnection _connection;
        private SqlCommand _sqlCommand;
        private SqlDataAdapter _dataAdapter;

        public DataTable FetchAll()
        {
            var result = new DataTable { Locale = CultureInfo.CurrentCulture };
            try
            {
                _connection = CreateConnection();
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
                _sqlCommand = new SqlCommand("sp_Role_FetchAll")
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = _connection
                };
                _dataAdapter = new SqlDataAdapter(_sqlCommand);
                _dataAdapter.Fill(result);
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return null;
            }
            finally
            {
                _sqlCommand.Dispose();
                _connection.Close();
            }
            return result;
        }

        public DataTable CheckRoleName(string roleName)
        {
            var table = new DataTable { Locale = CultureInfo.CurrentCulture };
            try
            {
                _connection = CreateConnection();
                _sqlCommand = new SqlCommand("sp_Role_CheckRoleName", _connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                _sqlCommand.Parameters.Add(new SqlParameter("@RoleName", string.IsNullOrEmpty(roleName) ? (object)DBNull.Value : roleName));
                _dataAdapter = new SqlDataAdapter(_sqlCommand);
                _dataAdapter.Fill(table);
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return null;
            }
            finally
            {
                _sqlCommand.Dispose();
                _connection.Close();
            }
            return table;
        }

        public int InsertRole(RoleEntity roleEntity)
        {
            var result = 0;

            try
            {
                _connection = CreateConnection();
                _sqlCommand = new SqlCommand("sp_Role_Insert", _connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                _sqlCommand.Parameters.Add(new SqlParameter("@RoleName", roleEntity.RoleName ?? (object)DBNull.Value));
                _sqlCommand.Parameters.Add(new SqlParameter("@Descriptions", roleEntity.Descriptions ?? (object)DBNull.Value));
                result = _sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return result;
            }
            finally
            {
                _sqlCommand.Dispose();
                _connection.Close();
            }
            return result;
        }

        public int UpdateRole(RoleEntity roleEntity)
        {
            var result = 0;
            try
            {
                _connection = CreateConnection();
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                _sqlCommand = new SqlCommand("sp_Role_Update", _connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Connection = _connection
                };
                _sqlCommand.Parameters.Add(new SqlParameter("@RoleId", roleEntity.RoleId));
                _sqlCommand.Parameters.Add(new SqlParameter("@RoleName", roleEntity.RoleName ?? (object)DBNull.Value));
                _sqlCommand.Parameters.Add(new SqlParameter("@Descriptions", roleEntity.Descriptions ?? (object)DBNull.Value));
                result = _sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return result;
            }
            finally
            {
                _sqlCommand.Dispose();
                _connection.Close();
            }
            return result;
        }

        public int DeleteRole(int roleId)
        {
            var result = 0;
            try
            {
                _connection = CreateConnection();
                _sqlCommand = new SqlCommand("sp_Role_Delete", _connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                _sqlCommand.Parameters.Add(new SqlParameter("@RoleId", roleId));
                result = _sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return result;
            }
            finally
            {
                _sqlCommand.Dispose();
                _connection.Close();
            }
            return result;
        }

        public DataTable SearchRole(string roleName)
        {
            var table = new DataTable { Locale = CultureInfo.CurrentCulture };
            try
            {
                _connection = CreateConnection();
                _sqlCommand = new SqlCommand("sp_Role_Search", _connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                _sqlCommand.Parameters.Add(new SqlParameter("@RoleName", string.IsNullOrEmpty(roleName) ? (object)DBNull.Value : roleName));
                _dataAdapter = new SqlDataAdapter(_sqlCommand);
                _dataAdapter.Fill(table);
            }
            catch (SqlException e)
            {
                ErrorMessage = e.Message;
                return null;
            }
            finally
            {
                _sqlCommand.Dispose();
                _connection.Close();
            }
            return table;
        }
    }
   public class RoleEntity : object
   {
       public int RoleId { get; set; }
       public string RoleName { get; set; }
       public string Descriptions { get; set; }
   }
}
