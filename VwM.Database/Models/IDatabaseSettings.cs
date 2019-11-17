using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace VwM.Database.Models
{
    public interface IDatabaseSettings
    {
        string Host { get; set; }
        int Port { get; set; }
        string DatabaseName { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        int ConnectTimeout { get; set; }
        int ServerSelectionTimeout { get; set; }
        string ApplicationName { get; }


        MongoClientSettings GetMongoClientSettings();
    }
}
