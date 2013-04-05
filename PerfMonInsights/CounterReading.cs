using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATPlatforms.PerformanceInsights
{
    public class CounterReading : CounterSpec
    {
        public float Value { get; set; }
        public long AggregateWindowLength { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
