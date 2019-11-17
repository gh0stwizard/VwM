using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace VwM.Database.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ConnectTimeout { get; set; }
        public int ServerSelectionTimeout { get; set; }
        public string ApplicationName { get; } = "VwM";


        public MongoClientSettings GetMongoClientSettings()
        {
            var settings = new MongoClientSettings()
            {
                ApplicationName = ApplicationName,
                Server = new MongoServerAddress(Host, Port)
            };

            if (!string.IsNullOrEmpty(UserName))
            {
                settings.Credential =
                    MongoCredential.CreateCredential(DatabaseName, UserName, Password);
            }

            if (ConnectTimeout >= 1)
                settings.ConnectTimeout = TimeSpan.FromSeconds(ConnectTimeout);

            if (ServerSelectionTimeout >= 1)
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(ServerSelectionTimeout);

            return settings;
        }
    }
}
