using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformanceInsights.MonitorSite.Hubs
{
    public enum StreamState
    {
        Open,
        Opening,
        Closing,
        Closed
    }
}