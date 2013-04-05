using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ATPlatforms.PerformanceInsights;
using NUnit.Framework;

namespace ATPlatforms.PerformanceInsights.Tests
{
    [TestFixture]
    public class PrefmonReaderTests : IObserver<CounterReading>
    {

        [Test]
        public void MSMQPerfMonTest()
        {

        }

        [Test]
        public void CPUPerfMonTest()
        {

            CounterSpec spec = new CounterSpec()
                {
                    CategoryName = "Processor",
                    CounterName = "% Processor Time",
                    InstanceName = "_Total"
                };

            PerformanceCounterReader reader = new PerformanceCounterReader(spec)
                {

                };

            long value = reader.RawValue();
            Assert.IsTrue(value > 0);
        }

        private float reading = 0l;
        [Test]
        public void TestMonitorObeservable()
        {
            CounterSpec spec = new CounterSpec()
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };

            PreformanceCounterConfig confg = new PreformanceCounterConfig()
                {
                    RefreshPeriod = 100,
                   
                };

            confg.Specs = new List<CounterSpec>();
            confg.Specs.Add(spec);
            var obeservable = new PerformanceCounterObservable(confg);
            obeservable.Subscribe(this);

            Thread.Sleep(2000);

            obeservable.Dispose();

            Assert.IsTrue(reading>0);

        }

        public void OnNext(CounterReading value)
        {
            reading = value.Value;
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

     
    }
}
