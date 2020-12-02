using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TinkoffApi.BL.Hubs
{
    public class TinkoffHub : Hub
    {
        public async Task Send(string candle)
        {
            await this.Clients.All.SendAsync("Send", candle);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
