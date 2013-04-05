using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATPlatforms.PerformanceInsights.Examples
{
    public class Payload
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return "[StreamInsight]\tValue: " + Value.ToString();
        }
    }
}
