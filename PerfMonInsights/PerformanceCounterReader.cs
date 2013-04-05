using System;

using System.Timers;
using System.Collections;
using System.Diagnostics;

namespace ATPlatforms.PerformanceInsights
{
	public class PerformanceCounterReader
	{
        private PerformanceCounter Counter { get; set; }

        public CounterSpec Spec { get; set; }

	    public PerformanceCounterReader(CounterSpec spec)
	    {
            if (spec.MachineName != null)
	        {
	            Counter = new PerformanceCounter(spec.CategoryName, spec.CounterName, spec.InstanceName, spec.MachineName);
	        }
	        else
	        {
	            Counter = new PerformanceCounter(spec.CategoryName, spec.CounterName, spec.InstanceName);
	        }
	        Spec = spec;
	    }

	    public long RawValue()
	    {
            return Counter.RawValue;
	    }

        public float NextReading()
        {
            return Counter.NextValue();
        }
	}
}
