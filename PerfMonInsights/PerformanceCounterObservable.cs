using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ATPlatforms.PerformanceInsights
{
    public class PerformanceCounterObservable : IObservable<CounterReading>, IDisposable
    {
        private List<IObserver<CounterReading>> _observers;
        private bool _done;
        private readonly object _sync;
        private readonly Timer _timer;


        private List<PerformanceCounterReader> readers { get; set; }

        private PreformanceCounterConfig _config;

        public PerformanceCounterObservable(PreformanceCounterConfig config)
        {
            _done = false;
            _sync = new object();
            _timer = new Timer(Monitor);
            _config = config;
            _observers = new List<IObserver<CounterReading>>();
            if (config.Specs!=null)
                AddCounters(config.Specs);
            Schedule();
        }

        public void AddCounters(List<CounterSpec> specs)
        {
            if(readers == null)
                readers = new List<PerformanceCounterReader>();

            if (specs == null)
                throw new ArgumentNullException("specs cannot be null");

            foreach (var spec in specs)
            {
                readers.Add(new PerformanceCounterReader(spec));

            }
        }

        public void AddCounter(CounterSpec spec)
        {
            _config.Specs.Add(spec);
            readers.Add(new PerformanceCounterReader(spec));

        }

        private void Schedule()
        {
            lock (_sync)
            {
                if (!_done)
                {
                    _timer.Change(_config.RefreshPeriod, Timeout.Infinite);
                }
            }
        }

        public void OnNext(CounterReading value)
        {
            lock (_sync)
            {
                if (!_done)
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnNext(value);
                    }
                }
            }
        }

        private bool _updatingCounters;
        private object _updatingCountersLock = new object();

        public void Monitor(object _){

            IEnumerable<CounterReading> items;
            items = from r in readers
                    select new CounterReading()
                    {
                        CategoryName = r.Spec.CategoryName,
                        CounterName = r.Spec.CounterName,
                        InstanceName = r.Spec.InstanceName,
                        MachineName = r.Spec.MachineName,
                        Value = r.NextReading()
                    };

            foreach (var item in items)
            {
                OnNext(item);
            }
      
            Schedule();
        }

        public IDisposable Subscribe(IObserver<CounterReading> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<CounterReading>> _observers;
            private IObserver<CounterReading> _observer;

            public Unsubscriber(List<IObserver<CounterReading>> observers, IObserver<CounterReading> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        public void OnError(Exception e)
        {
            lock (_sync)
            {
                foreach (var observer in _observers)
                {
                    observer.OnError(e);
                }
                _done = true;
            }
        }

        public void OnCompleted()
        {
            lock (_sync)
            {
                foreach (var observer in _observers)
                {
                    observer.OnCompleted();
                }
                _done = true;
            }
        }

        public void Dispose()
        {
            _done = true;
            _timer.Dispose();
        }
    }
}
