using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ATPlatforms.PerformanceInsights.Examples
{
    public class RandomSubject : IObservable<int>, IDisposable
    {
        private bool _done;
        private readonly List<IObserver<int>> _observers;
        private readonly Random _random;
        private readonly object _sync;
        private readonly Timer _timer;
        private readonly int _timerPeriod;

        /// <summary>
        /// Random observable subject. It produces an integer in regular time periods.
        /// </summary>
        /// <param name="timerPeriod">Timer period (in milliseconds)</param>
        public RandomSubject(int timerPeriod)
        {
            _done = false;
            _observers = new List<IObserver<int>>();
            _random = new Random();
            _sync = new object();
            _timer = new Timer(EmitRandomValue);
            _timerPeriod = timerPeriod;
            Schedule();
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            lock (_sync)
            {
                _observers.Add(observer);
            }
            return new Subscription(this, observer);
        }

        public void OnNext(int value)
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

        void IDisposable.Dispose()
        {
            _timer.Dispose();
        }

        private void Schedule()
        {
            lock (_sync)
            {
                if (!_done)
                {
                    _timer.Change(_timerPeriod, Timeout.Infinite);
                }
            }
        }

        private void EmitRandomValue(object _)
        {
            var value = (int)(_random.NextDouble() * 100);
            Console.WriteLine("[Observable]\t" + value);
            OnNext(value);
            Schedule();
        }

        private sealed class Subscription : IDisposable
        {
            private readonly RandomSubject _subject;
            private IObserver<int> _observer;

            public Subscription(RandomSubject subject, IObserver<int> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                IObserver<int> observer = _observer;
                if (null != observer)
                {
                    lock (_subject._sync)
                    {
                        _subject._observers.Remove(observer);
                    }
                    _observer = null;
                }
            }
        }
    }
}
