using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATPlatforms.PerformanceInsights
{
    public class CounterSpec
    {
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
	    public string InstanceName { get; set; }
	    public string MachineName { get; set; }
    }
}
