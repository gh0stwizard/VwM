using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using VwM.Database.Extensions;
using VwM.Database.Filters;
using VwM.Database.Models;

namespace VwM.Database.Collections
{
    public class CollectionBase<T>
        where T : class
    {
        protected IMongoCollection<T> Collection { get; set; }


        public CollectionBase(IDatabaseSettings dbSettings)
        {
            var settings = dbSettings.GetMongoClientSettings();
            var client = new MongoClient(settings);
            var db = client.GetDatabase(dbSettings.DatabaseName);

            Collection = db.GetCollection<T>(typeof(T).Name.ToLower());
        }


        public IMongoQueryable<T> Query(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null)
                return Collection.AsQueryable();

            return Collection.AsQueryable().Where(filter);
        }


        public Task<long> CountAsync(Expression<Func<T, bool>> filter = null) =>
            filter == null
            ? Collection.CountDocumentsAsync(f => true)
            : Collection.CountDocumentsAsync(filter);


        public Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
            => Query(filter).FirstOrDefaultAsync();


        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> filter = null)
            => Query(filter).ToListAsync();

        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> filter, Ordering order, Pagination pagination)
            => Query(filter).Sort(order).Paginate(pagination).ToListAsync();


        public Task<DeleteResult> DeleteOneAsync(Expression<Func<T, bool>> filter)
            => Collection.DeleteOneAsync<T>(filter);


        public Task<T> FindOneAndUpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
            => Collection.FindOneAndUpdateAsync(filter, update);


        public Task InsertOneAsync(T document) => Collection.InsertOneAsync(document);


        public Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
            => Collection.UpdateOneAsync(filter, update);

        public Task UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
            => Collection.UpdateOneAsync(filter, update);


        public Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<T, bool>> filter, T document)
            => Collection.ReplaceOneAsync(filter, document);
    }
}
