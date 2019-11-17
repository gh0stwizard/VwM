using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using VwM.Database.Server;
using VwM.Extensions;

namespace VwM.Hubs
{
    public class HomeHub : BaseHub
    {
        private readonly ILogger<HomeHub> _logger;
        private readonly DatabaseStatus _dbStatus;


        public HomeHub(ILogger<HomeHub> logger, DatabaseStatus serverStatus)
        {
            _logger = logger;
            _dbStatus = serverStatus;
        }


        public async Task GetDbStatus(string id)
        {
            var status = await _dbStatus.PingAsync();
            await Clients.Client(id).SendAsync("DbStatus", status);
        }
    }
}
