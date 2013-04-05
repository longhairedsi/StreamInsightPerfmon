//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;

//using System.Collections;

//using Microsoft.ComplexEventProcessing.Adapters;

//namespace ATPlatforms.PerformanceInsights
//{

//    public class PerformanceCounterAdapter : TypedPointInputAdapter<CounterReading>
//    {
//        #region Private variables

//        private List<PerformanceCounterReader> readers { get; set; }

//        private PreformanceCounterConfig _config;
//        private DateTimeOffset _lastCti = DateTimeOffset.MinValue;
//        #endregion


//        public PerformanceCounterAdapter(PreformanceCounterConfig config)
//        {
//            _config = config;
//            readers = new List<PerformanceCounterReader>();

//            AddCounters(config.Specs);
//        }

//        public void AddCounters(List<CounterSpec> specs)
//        {
//            foreach (var spec in specs)
//            {
//                readers.Add(new PerformanceCounterReader(spec));

//            }
//        }

//        public void AddCounter(CounterSpec spec)
//        {
//            _config.Specs.Add(spec);
//            readers.Add(new PerformanceCounterReader(spec));

//        }

//        public override void Start()
//        {
//            ProduceEvents();
//        }

//        public override void Resume()
//        {
//            ProduceEvents();
//        }

//        private void ProduceEvents()
//        {
//            while (AdapterState != AdapterState.Stopping)
//            {
//                IEnumerable<CounterReading> items;
//                items = from r in readers
//                        select new CounterReading()
//                            {
//                                CategoryName = r.Spec.CategoryName,
//                                CounterName = r.Spec.CounterName,
//                                InstanceName = r.Spec.InstanceName,
//                                MachineName = r.Spec.MachineName,
//                                Value = r.RawValue()
//                            };

                
//                //get all the readings
//                foreach (var item in items)
//                {
//                    var evt = CreateInsertEvent();
//                    evt.StartTime = DateTimeOffset.UtcNow;
//                    evt.Payload = item;
//                    Enqueue(ref evt);
//                    //_previousItems.Add(item.Id);
//                }

//                EnqueueCtiEvent(DateTimeOffset.UtcNow);

//                Thread.Sleep(_config.RefreshPeriod);
//            }

//        }

//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);
//        }
//    }
//}
