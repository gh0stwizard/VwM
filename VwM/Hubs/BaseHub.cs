using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace VwM.Hubs
{
    public class BaseHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.Client(Context.ConnectionId).SendAsync("Registered", Context.ConnectionId);

            return base.OnConnectedAsync();
        }
    }
}
