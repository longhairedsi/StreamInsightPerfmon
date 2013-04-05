using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Text;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;

namespace ATPlatforms.PerformanceInsights
{
    public static class PerformanceCounterConsoleOutput
    {
        public static void DisplayPointResults<TPayload>(this Application app, IQStreamable<TPayload> resultStream)
        {
            // Define observer that formats arriving events as points to the console window.
            var consoleObserver = app.DefineObserver(() => Observer.Create<PointEvent<TPayload>>(ConsoleWritePoint));

            // Bind resultStream stream to consoleObserver.
            var binding = resultStream.Bind(consoleObserver);

            // Run example by creating a process from the binding we built above.
            using (binding.Run("ExampleProcess"))
            {
                Console.WriteLine("***Hit Return to exit after viewing query output***");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        public static void DisplayPointResults<TPayload>(this Application app, CepStream<TPayload> resultStream)
        {
            using (resultStream.ToPointObservable().Subscribe(ConsoleWritePoint))
            {
                Console.WriteLine("Started query.");
                Console.ReadLine();
                Console.WriteLine("Stopping query...");
            }
        }

        public static void ConsoleWritePoint<TPayload>(PointEvent<TPayload> e)
        {
            if (e.EventKind == EventKind.Insert)
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "INSERT <{0}> {1}", e.StartTime.DateTime, e.Payload.ToString()));
            else
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "CTI    <{0}>", e.StartTime.DateTime));
        }

        public static void DisplayIntervalResults<TPayload>(this Application app, IQStreamable<TPayload> resultStream)
        {
            // Define observer that formats arriving events as intervals to the console window.
            var consoleObserver = app.DefineObserver(() => Observer.Create<IntervalEvent<TPayload>>(ConsoleWriteInterval));

            // Bind resultStream stream to consoleObserver.
            var binding = resultStream.Bind(consoleObserver);

            // Run example query by creating a process from the binding we've built above.
            using (binding.Run("ExampleProcess"))
            {
                Console.WriteLine("***Hit Return to exit after viewing query output***");
                Console.WriteLine();
                Console.ReadLine();
            }
        }

        public static void ConsoleWriteInterval<TPayload>(IntervalEvent<TPayload> e)
        {
            if (e.EventKind == EventKind.Insert)
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "INSERT <{0} - {1}> {2}", e.StartTime.DateTime, e.EndTime.DateTime, e.Payload.ToString()));
            else
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "CTI    <{0}>", e.StartTime.DateTime));
        }

    }
}
