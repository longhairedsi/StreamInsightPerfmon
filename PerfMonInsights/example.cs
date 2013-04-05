//namespace HelloWorld
//{
//    using System;
//    using System.ServiceModel;
//    using System.Collections.Generic;
//    using System.Reactive;
//    using System.Reactive.Concurrency;
//    using System.Reactive.Linq;
//    using Microsoft.ComplexEventProcessing;
//    using Microsoft.ComplexEventProcessing.Linq;
//    using System.Globalization;
//    using trade = Acme.Trade.Business.Entities;

//    static class Program
//    {

//        // Global variables
//        private static IEnumerable<trade.TradeCaptureReportEnrichment> _tcrEnumerableSource;
//        private static IQStreamable<trade.TradeCaptureReportEnrichment> inputStream;

//        static void Main(string[] args)
//        {

//            // The StreamInsight engine is a server that can be embedded (in-memory) or remote (e.g. the Azure Service).
//            // We first use Server.Create to create a server instance and return a handle to that instance.
//            using (Server server = Server.Create("STATISTICS"))
//            {
//                Application application = server.CreateApplication("RealTimeStatistics");

//                inputStream = CreateStream(application);

//                // First query output
//                ConfirmedTradeCaptureReports(application, inputStream);

//            }

//        }

//        private static IObservable<trade.TradeCaptureReportEnrichment> LiveTradeCaptureReports()
//        {

//            // Reference to class that connects to WCF to get the IEnumerable source
//            TradeCaptureReportSource tcrSource = new TradeCaptureReportSource();

//            // Create the CEP Stream
//            _tcrEnumerableSource = tcrSource.GetTradeCaptureReportEvents();

//            return ToObservableInterval(_tcrEnumerableSource, TimeSpan.FromMilliseconds(100), Scheduler.ThreadPool);
//        }

//        private static IObservable<T> ToObservableInterval<T>(IEnumerable<T> source, TimeSpan period, IScheduler scheduler)
//        {
//            return Observable.Using(
//                () => source.GetEnumerator(),
//                it => Observable.Generate(
//                    default(object),
//                    _ => it.MoveNext(),
//                    _ => _,
//                    _ =>
//                    {
//                        return it.Current;
//                    },
//                    _ => period, scheduler));
//        }

//        static IQStreamable<trade.TradeCaptureReportEnrichment> CreateStream(Application application)
//        {
//            uint cti = 1;

//            DateTime startTime = DateTime.Parse("2013-01-03");
//            var ats = new AdvanceTimeSettings(new AdvanceTimeGenerationSettings((uint)cti, TimeSpan.FromTicks(-1)), null, AdvanceTimePolicy.Drop);

//            return application.DefineObservable(() => LiveTradeCaptureReports()).ToPointStreamable(
//                r => PointEvent<trade.TradeCaptureReportEnrichment>.CreateInsert(DateTime.UtcNow, r), ats).Deploy("wcfTCRSource");

//        }

//        static void ConsoleWritePoint<TPayload>(PointEvent<TPayload> e)
//        {
//            if (e.EventKind == EventKind.Insert)
//                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "INSERT <{0}> {1}", e.StartTime.DateTime, e.Payload.ToString()));
//            else
//                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "CTI    <{0}>", e.StartTime.DateTime));
//        }

//        static void ConfirmedTradeCaptureReports(Application app, IQStreamable<trade.TradeCaptureReportEnrichment> inputStream)
//        {
//            var confirmedTCR = from tcr in inputStream
//                               group tcr by new { tcr.Symbol, tcr.OBTradeSubTypeName } into TradeTypeByInstrument
//                               from window in TradeTypeByInstrument.HoppingWindow(TimeSpan.FromSeconds(20), // Window Size
//                                                                                    TimeSpan.FromSeconds(10)) // Hop Size
//                               select new 
//                               {
//                                   Symbol = TradeTypeByInstrument.Key,
//                                   Volume = window.Sum (e => e.ExecutedSize),
//                                   Value = window.Sum(e => e.TradeValue),
//                                   Trades = window.Count()
//                               };            
//            app.DisplayPointResults(confirmedTCR);

//        }

//        static void DisplayPointResults<TPayload>(this Application app, IQStreamable<TPayload> resultStream)
//        {
//            var consoleObserver = app.DefineObserver(() => Observer.Create<PointEvent<TPayload>>(ConsoleWritePoint));

//            var binding = resultStream.Bind(consoleObserver);
//            using (binding.Run("ServerProcess"))
//            {
//                Console.WriteLine("***Hit Return to exit after viewing query output***");
//                Console.WriteLine();
//                Console.ReadLine();
//            }
//        }
//    }