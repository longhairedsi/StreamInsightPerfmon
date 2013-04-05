using System.Collections.Generic;

namespace ATPlatforms.PerformanceInsights
{
    public class PreformanceCounterConfig
    {
        public List<CounterSpec> Specs { get; set; }
        public int RefreshPeriod { get; set; }

        public PreformanceCounterConfig()
        {
            Specs = new List<CounterSpec>();
        }
    }
}
