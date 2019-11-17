using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VwM.Database.Filters;

namespace VwM.Database.Extensions
{
    public static class MongoQueryableExtensions
    {
        #region OrderBy, ThenBy
        public static IOrderedMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> source, string property, bool ascending)
        {
            return ApplyOrder(source, property, ascending ? "OrderBy" : "OrderByDescending");
        }
        public static IOrderedMongoQueryable<T> OrderBy<T>(this IMongoQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }
        public static IOrderedMongoQueryable<T> OrderByDescending<T>(this IMongoQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }
        public static IOrderedMongoQueryable<T> ThenBy<T>(this IOrderedMongoQueryable<T> source, string property, bool ascending)
        {
            return ApplyOrder(source, property, ascending ? "ThenBy" : "ThenByDescending");
        }
        public static IOrderedMongoQueryable<T> ThenBy<T>(this IOrderedMongoQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }
        public static IOrderedMongoQueryable<T> ThenByDescending<T>(this IOrderedMongoQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }
        static IOrderedMongoQueryable<T> ApplyOrder<T>(IMongoQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedMongoQueryable<T>)result;
        }
        #endregion


        #region Paginate
        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> query, int skip = 0, int take = 0)
        {
            if (skip > 0)
                query = query.Skip(skip);

            if (take > 0)
                query = query.Take(take);

            return query;
        }

        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> query, Pagination pagination)
        {
            if (pagination == null)
                return query;

            return Paginate(query, pagination.Skip, pagination.Take);
        }
        #endregion


        #region Sort
        public static IOrderedMongoQueryable<T> Sort<T>(
        this IMongoQueryable<T> query,
        List<Tuple<string, string>> order)
        {
            var orderedQuery = (IOrderedMongoQueryable<T>)query;

            if (order == null || order.Count == 0)
                return orderedQuery;

            var unordered = true;

            foreach (var o in order)
            {
                var column = o.Item1;
                var asc = o.Item2 == "asc";

                if (unordered)
                {
                    orderedQuery = orderedQuery.OrderBy(column, asc);
                    unordered = false;
                }
                else
                {
                    orderedQuery = orderedQuery.ThenBy(column, asc);
                }
            }

            return orderedQuery;
        }
        #endregion
    }
}
