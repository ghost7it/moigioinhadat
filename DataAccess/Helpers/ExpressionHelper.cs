using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    public static class ExpressionHelper
    {
        public static Expression<Func<TModel, TToProperty>> Cast<TModel, TFromProperty, TToProperty>(Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));

            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
        }
        public static Expression<Func<TItem, object>> SelectExpression<TItem>(params string[] fieldNames)
        {
            var param = Expression.Parameter(typeof(TItem), "item");
            var fields = fieldNames.Select(x => Expression.Property(param, x)).ToArray();
            var types = fields.Select(x => x.Type).ToArray();
            var type = Type.GetType("System.Tuple`" + fields.Count() + ", mscorlib", true);
            var tuple = type.MakeGenericType(types);
            var ctor = tuple.GetConstructor(types);
            return Expression.Lambda<Func<TItem, object>>(
                Expression.New(ctor, fields),
                param
            );
        }
        public static Expression<Func<TItem, object>> SelectExpression2<TItem>(string[] propertyNames)
        {
            var properties = propertyNames.Select(name => typeof(TItem).GetProperty(name)).ToArray();
            var propertyTypes = properties.Select(p => p.PropertyType).ToArray();
            var tupleTypeDefinition = typeof(Tuple).Assembly.GetType("System.Tuple`" + properties.Length);
            var tupleType = tupleTypeDefinition.MakeGenericType(propertyTypes);
            var constructor = tupleType.GetConstructor(propertyTypes);
            var param = Expression.Parameter(typeof(TItem), "item");
            var body = Expression.New(constructor, properties.Select(p => Expression.Property(param, p)));
            var expr = Expression.Lambda<Func<TItem, object>>(body, param);
            return expr;
        }
    }
}
