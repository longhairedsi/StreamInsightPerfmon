using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ATPlatforms.PerformanceInsights;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace PerformanceInsights.MonitorSite.Hubs
{
   [HubName("preformanceStreamHub")]
    public class PreformanceStreamHub : Hub
    {
        private readonly PerformanceStream _performanceStream;

        public PreformanceStreamHub() : this(PerformanceStream.Instance) { }

        public PreformanceStreamHub(PerformanceStream stockTicker)
        {
            _performanceStream = stockTicker;
        }

        public void StartPerformanceStream(PreformanceCounterConfig config)
        {
            _performanceStream.StartMonitoring(config);
        }

        public void StopPerformanceStream()
        {
            _performanceStream.CloseStream();
        }
        /* do something with these if I need to
        public override Task OnConnected()
        {
            return Clients.All.joined(Context.ConnectionId, DateTime.Now.ToString());
        }
        */

       
        public override Task OnDisconnected()
        {
            _performanceStream.CloseStream();
            return Clients.All.leave(Context.ConnectionId, DateTime.Now.ToString());
        }
       /*
        public override Task OnReconnected()
        {
            return Clients.All.rejoined(Context.ConnectionId, DateTime.Now.ToString());
        }
         */
    }
}