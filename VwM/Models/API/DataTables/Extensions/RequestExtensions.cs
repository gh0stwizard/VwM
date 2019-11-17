using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Driver;
using ExpressionBuilder.Common;
using ExpressionBuilder.Generics;
using ExpressionBuilder.Operations;
using VwM.Extensions;
using VwM.Helpers;
using VwM.Database.Filters;
using VwM.Database.Collections;

namespace VwM.Models.API.DataTables.Extensions
{
    public static class RequestExtensions
    {
        public static async Task<Response> GetResponseAsync<T>(this Request request, CollectionBase<T> service, IMapper mapper)
            where T : class
        {
            var response = new Response() { Draw = request.Draw };
            var filter = request.GetQueryExpression<T>();
            var order = request.GetOrderExpression<T>();
            var pagination = mapper.Map<Pagination>(request);
            var items = await service.GetListAsync(filter, order, pagination);
            var total = await service.CountAsync();

            response.Filtered = filter.Statements.SelectMany(a => a).Any() ? items.Count : (int)total;
            response.Total = (int)total;
            response.Data = items;

            return response;
        }


        public static Filter<T> GetQueryExpression<T>(this Request request)
            where T : class
        {
            var filter = new Filter<T>();
            var group = filter.Group;

            foreach (var c in request.Columns.Where(a => a.Searchable))
            {
                if (string.IsNullOrEmpty(c.Data))
                    continue;

                var search = c.Search.Value;

                if (string.IsNullOrEmpty(search))
                    search = request.Search.Value;

                if (string.IsNullOrWhiteSpace(search))
                    continue;

                var column = c.Data.ToUpperFirst();
                var type = ObjectHelper.GetNestedPropertyType<T>(column)
                        ?? ObjectHelper.GetNestedFieldType<T>(column);

                if (type == null)
                    continue;

                type = Nullable.GetUnderlyingType(type) ?? type;

                if (type == typeof(bool))
                {
                    var @bool = search.ParseBool();

                    if (@bool.HasValue)
                        group.By(column, Operation.EqualTo, @bool, Connector.Or);
                }
                else if (type == typeof(int))
                {
                    if (int.TryParse(search, out int @int))
                        group.By(column, Operation.EqualTo, @int, Connector.Or);
                }
                else if (type.IsGenericType)
                {
                    var genericType = type.GetGenericTypeDefinition();
                    if (genericType == typeof(IList<>) || genericType == typeof(List<>))
                    {
                        var itemType = type.GetGenericArguments()[0];
                        var value = Convert.ChangeType(search, itemType);
                        group.By(column + "[]", Operation.StartsWith, search, Connector.Or);
                    }
                }
                else
                {
                    var value = Convert.ChangeType(search, type);
                    group.By(column, Operation.StartsWith, value, Connector.Or);
                }
            }

            return filter;
        }


        public static Ordering GetOrderExpression<T>(this Request request)
            where T : class
        {
            var ordering = new Ordering();

            if (request.Order.Count == 0)
                return ordering;

            request.Order.ForEach(o =>
            {
                var colName = request.Columns[o.Column].Data.ToUpperFirst();

                if (!string.IsNullOrEmpty(colName))
                    ordering.Add(new Tuple<string, string>(colName, o.Direction));
            });

            return ordering;
        }
    }
}
