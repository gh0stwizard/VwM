using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using VwM.Database.Models;

namespace VwM.Database.Server
{
    public class DatabaseStatus
    {
        private readonly ILogger<DatabaseStatus> _logger;
        private DateTime NextUpdate = DateTime.MinValue;
        private const int PingRefreshInterval = 10;

        protected IMongoDatabase Database { get; set; }
        public bool Connected { get; private set; }


        public DatabaseStatus(
            ILogger<DatabaseStatus> logger,
            IDatabaseSettings dbSettings)
        {
            _logger = logger;
            var settings = dbSettings.GetMongoClientSettings();
            var client = new MongoClient(settings);
            Database = client.GetDatabase(dbSettings.DatabaseName);
        }


        public async ValueTask<bool> PingAsync()
        {
            if (NextUpdate >= DateTime.UtcNow)
                return Connected;

            try
            {
                var result = await Database.RunCommandAsync<BsonDocument>("{ping:1}");
                Connected = result.GetValue("ok", false).ToBoolean();
            }
            catch (TimeoutException)
            {
                Connected = false;
            }
            catch (Exception e)
            {
                Connected = false;

                //if (!(e is TimeoutException || e is EndOfStreamException))
                _logger.LogError(e, "PingAsync Unhandeled Exception.");
            }
            finally
            {
                NextUpdate = DateTime.UtcNow.AddSeconds(PingRefreshInterval);
            }

            return Connected;
        }
    }
}
