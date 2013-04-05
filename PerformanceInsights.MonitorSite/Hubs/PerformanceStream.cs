using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using ATPlatforms.PerformanceInsights;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;

namespace PerformanceInsights.MonitorSite.Hubs
{
    public class PerformanceStream
    {
        private readonly static Lazy<PerformanceStream> _instance = new Lazy<PerformanceStream>(() => new PerformanceStream());
        private readonly static object _streamStateLock = new object();
        // private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();
        // private readonly double _rangePercent = .002; //stock can go up or down by a percentage of this factor on each change
        //private readonly int _updateInterval = 250; //ms
        // This is used as an singleton instance so we'll never both disposing the timer
        private PerformanceCounterObservable _source;
        private Server _server;
        private readonly object _updateStockPricesLock = new object();
        // private bool _updatingStockPrices = false;
        //private readonly Random _updateOrNotRandom = new Random();
        private StreamState _streamState = StreamState.Closed;
        private readonly Lazy<IHubConnectionContext> _clientsInstance = new Lazy<IHubConnectionContext>(() => GlobalHost.ConnectionManager.GetHubContext<PreformanceStreamHub>().Clients);

        private PerformanceStream()
        {
          //  LoadDefaultStocks();
        }

        public static PerformanceStream Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext Clients
        {
            get { return _clientsInstance.Value; }
        }

        public void StartMonitoring(PreformanceCounterConfig config)
        {

            if (_streamState != StreamState.Open || _streamState != StreamState.Opening)
            {
                lock (_streamStateLock)
                {
                    if (_streamState != StreamState.Open || _streamState != StreamState.Opening)
                    {
                        _streamState = StreamState.Opening;
                        //_timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);
                        _source = new PerformanceCounterObservable(config);
                        _server = Server.Create("burrito");
                        var application = _server.CreateApplication("PerformanceMonitor");
                        var stream = _source.ToPointStream(application,
                                       e =>
                                       PointEvent.CreateInsert(DateTime.Now, e),
                                       AdvanceTimeSettings.StrictlyIncreasingStartTime,
                                       "Observable Stream");

                        //Realtime query 
                        var realTimeQuery = from e in stream
                                            select e;
                        //                    //select FillCounterReading(e, 0);

                        realTimeQuery.ToPointObservable().Subscribe(BroadcastCounterReadings);

                      /*  var fiveSecondAverage = PartitionedMovingAverage(stream,5);
                        fiveSecondAverage.ToPointObservable().Subscribe(BroadcastCounterReadings);

                        var thirtySecondAverage = PartitionedMovingAverage(stream, 30);
                        thirtySecondAverage.ToPointObservable().Subscribe(BroadcastCounterReadings);*/

                        _streamState = StreamState.Open;

                        //BroadcastMarketStateChange(MarketState.Open);
                    }
                }
            }
        }

        
        private CepStream<CounterReading> PartitionedMovingAverage(CepStream<CounterReading> inputStream, int windowSeconds)
        {
            var partitionedSlidingWindow = from e in inputStream.AlterEventDuration(e => e.EndTime - e.StartTime + TimeSpan.FromSeconds(1))
                                           group e by new {e.CounterName,e.MachineName,e.CategoryName,e.InstanceName} into perCounter
                                           from win in perCounter.SnapshotWindow()
                                           select new CounterReading()
                                            {
                                                AggregateWindowLength = windowSeconds,
                                                CounterName = perCounter.Key.CategoryName,
                                                CategoryName = perCounter.Key.CategoryName,
                                                InstanceName = perCounter.Key.InstanceName,
                                                MachineName = perCounter.Key.MachineName,
                                                Value = win.Sum(e => e.Value)/win.Count(),
                                
                                           };
            return partitionedSlidingWindow;
            /*var query = from e in partitionedSlidingWindow.ToPointEventStream()
                        select new TollAverage
                        {
                            TollId = e.TollId,
                            AverageToll = e.TollAmount / e.VehicleCount
                        };*/
        }

        private static CounterReading FillCounterReading(CounterReading source, long aggregateWindow)
        {
            return new CounterReading()
            {
                AggregateWindowLength = aggregateWindow,
                CounterName = source.CategoryName,
                CategoryName = source.CategoryName,
                InstanceName = source.InstanceName,
                MachineName = source.MachineName,
                Value = source.Value
            };
        }
        private void BroadcastCounterReadings(PointEvent<CounterReading> e)
        {
            if (e.EventKind == EventKind.Insert)
                Clients.All.updatePreformanceReading(e);
        }

        public void CloseStream()
        {

            if (_streamState == StreamState.Open || _streamState == StreamState.Opening)
            {
                lock (_streamStateLock)
                {
                    if (_streamState == StreamState.Open || _streamState == StreamState.Opening)
                    {
                        _streamState = StreamState.Closing;

                        if (_source != null)
                        {
                            _source.Dispose();
                        }

                        if (_server != null)
                        {
                            _server.Dispose();
                        }
                        _streamState = StreamState.Closed;
                       // BroadcastMarketStateChange(MarketState.Closed);
                    }
                }
            }
        }


    }
}
