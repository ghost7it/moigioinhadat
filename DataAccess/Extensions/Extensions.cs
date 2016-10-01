using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Reflection;
namespace DataAccess.Extensions
{
    //NamMV
    //Dùng EF để gọi thủ tục stored proc trong sql
    //Cách sử dụng:
    /*
    public Product GetProduct(int id)
    {
        Product product = null;
 
        using (var context = new NorthwindData())
        {
            product = context.Database.SqlQuerySmart<Product>("Product_GetByID", new
            {
                productId = id
            }).FirstOrDefault();
        } 
        return product;
    }
    */
    public static class Extensions
    {
        public static int ExecuteSqlCommandSmart(this Database self, string storedProcedure, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");

            var arguments = PrepareArguments(storedProcedure, parameters);
            return self.ExecuteSqlCommand(arguments.Item1, arguments.Item2);
        }

        public static IEnumerable<TElement> SqlQuerySmart<TElement>(this Database self, string storedProcedure, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");

            var arguments = PrepareArguments(storedProcedure, parameters);
            return self.SqlQuery<TElement>(arguments.Item1, arguments.Item2);
        }

        public static IEnumerable SqlQuerySmart(this Database self, Type elementType, string storedProcedure, object parameters = null)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            if (string.IsNullOrEmpty(storedProcedure))
                throw new ArgumentException("storedProcedure");

            var arguments = PrepareArguments(storedProcedure, parameters);
            return self.SqlQuery(elementType, arguments.Item1, arguments.Item2);
        }

        private static Tuple<string, object[]> PrepareArguments(string storedProcedure, object parameters)
        {
            var parameterNames = new List<string>();
            var parameterParameters = new List<object>();

            if (parameters != null)
            {
                foreach (PropertyInfo propertyInfo in parameters.GetType().GetProperties())
                {
                    string name = "@" + propertyInfo.Name;
                    object value = propertyInfo.GetValue(parameters, null);

                    parameterNames.Add(name);
                    parameterParameters.Add(new SqlParameter(name, value ?? DBNull.Value));
                }
            }

            if (parameterNames.Count > 0)
                storedProcedure += " " + string.Join(", ", parameterNames);

            return new Tuple<string, object[]>(storedProcedure, parameterParameters.ToArray());
        }
    }
}
