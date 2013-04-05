using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;

namespace ATPlatforms.PerformanceInsights
{
    public class PerformanceCounterFactory : ITypedInputAdapterFactory<PreformanceCounterConfig>
    {
        public InputAdapterBase Create<TPayload>(PreformanceCounterConfig config, EventShape eventShape)
        {
            // Only support the point event model
            if (eventShape == EventShape.Point)
               // return new PerformanceCounterAdapter(config);
                return default(InputAdapterBase);
            else
                return default(InputAdapterBase);
        }

        public void Dispose()
        {
        }
    }
}
