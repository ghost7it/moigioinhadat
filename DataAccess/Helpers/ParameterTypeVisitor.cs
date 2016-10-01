using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    //NamMV
    //Cách dùng:
    //Expression<Func<PortalContext.DSDK, bool>> exp;
    //var visitor = new ParameterTypeVisitor<DSDK, PortalContext.DSDK>(whereCondition);
    //exp = visitor.Convert();
    public class ParameterTypeVisitor<TFrom, TTo> : ExpressionVisitor
    {

        private Dictionary<string, ParameterExpression> convertedParameters;
        private Expression<Func<TFrom, bool>> expression;

        public ParameterTypeVisitor(Expression<Func<TFrom, bool>> expresionToConvert)
        {
            //for each parameter in the original expression creates a new parameter with the same name but with changed type 
            convertedParameters = expresionToConvert.Parameters
                .ToDictionary(
                    x => x.Name,
                    x => Expression.Parameter(typeof(TTo), x.Name)
                );

            expression = expresionToConvert;
        }

        public Expression<Func<TTo, bool>> Convert()
        {
            return (Expression<Func<TTo, bool>>)Visit(expression);
        }

        //handles Properties and Fields accessors 
        protected override Expression VisitMember(MemberExpression node)
        {
            //we want to replace only the nodes of type TFrom
            //so we can handle expressions of the form x=> x.Property.SubProperty
            //in the expression x=> x.Property1 == 6 && x.Property2 == 3
            //this replaces         ^^^^^^^^^^^         ^^^^^^^^^^^            
            if (node.Member.DeclaringType == typeof(TFrom))
            {
                //gets the memberinfo from type TTo that matches the member of type TFrom
                var memeberInfo = typeof(TTo).GetMember(node.Member.Name).First();

                //this will actually call the VisitParameter method in this class
                var newExp = Visit(node.Expression);
                return Expression.MakeMemberAccess(newExp, memeberInfo);
            }
            else
            {
                return base.VisitMember(node);
            }
        }

        // this will be called where ever we have a reference to a parameter in the expression
        // for ex. in the expression x=> x.Property1 == 6 && x.Property2 == 3
        // this will be called twice     ^                   ^
        protected override Expression VisitParameter(ParameterExpression node)
        {
            var newParameter = convertedParameters[node.Name];
            return newParameter;
        }

        //this will be the first Visit method to be called
        //since we're converting LamdaExpressions
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            //visit the body of the lambda, this will Traverse the ExpressionTree 
            //and recursively replace parts of the expresion we for witch we have matching Visit methods 
            var newExp = Visit(node.Body);

            //this will create the new expression            
            return Expression.Lambda(newExp, convertedParameters.Select(x => x.Value));
        }
    }
}
