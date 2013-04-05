using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using ATPlatforms.PerformanceInsights;
using ATPlatforms.PerformanceInsights.Examples;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;

namespace ATPlatforms.PerformanceInsights.App
{
    class Program
    {
        private static void Main(string[] args)
        {
            
            var config = new PreformanceCounterConfig();
            config.Specs.Add(new CounterSpec()
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            });
            config.RefreshPeriod = 100;
            Console.WriteLine("Starting observable source...");
            using (var source = new PerformanceCounterObservable(config))
            {
                Console.WriteLine("Started observable source.");
                using (var server = Server.Create("burrito"))
                {
                    var application = server.CreateApplication("PerformanceMonitor");
       
                    var stream = source.ToPointStream(application,
                                                      e =>
                                                      PointEvent.CreateInsert(DateTime.Now, e),
                                                      AdvanceTimeSettings.StrictlyIncreasingStartTime,
                                                      "Observable Stream");

                    var query = from e in stream
                                select e;
                   // Console.ReadLine();
                    Console.WriteLine("Starting query...");
                    application.DisplayPointResults<CounterReading>(query);
                    //using (query.ToObservable().Subscribe(Console.WriteLine))
                    //{
                    //    Console.WriteLine("Started query.");
                    //    Console.ReadLine();
                    //    Console.WriteLine("Stopping query...");
                    //}
                    Console.WriteLine("Stopped query.");
                }

                Console.ReadLine();
                Console.WriteLine("Stopping observable source...");
                source.OnCompleted();
            }
            Console.WriteLine("Stopped observable source.");

          
        }


    }
}
