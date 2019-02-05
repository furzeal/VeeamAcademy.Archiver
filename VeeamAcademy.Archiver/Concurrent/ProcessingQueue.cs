using System;
using System.Collections.Generic;
using System.Threading;

namespace VeeamAcademy.Archiver.Concurrent
{
    public sealed class ProcessingQueue<T> : IDisposable where T : class
    {
        private readonly Action<T> _action;
        private readonly object _locker = new object();
        private readonly Thread[] _workers;
        private readonly Queue<T> _queue = new Queue<T>();

        public ProcessingQueue(int workerCount, Action<T> action)
        {
            _action = action;
            _workers = new Thread[workerCount];

            // Create and start a separate thread for each worker
            for (var i = 0; i < workerCount; i++)
            {
                (_workers[i] = new Thread(Consume)).Start();
                _workers[i].Name = "Worker #" + (i + 1);
            }
        }

        public void EnqueueProductForProcessing(T product)
        {
            lock (_locker)
            {
                _queue.Enqueue(product);
                Monitor.PulseAll(_locker);
            }
        }

        void Consume()
        {
            while (true)
            {
                T product;
                lock (_locker)
                {
                    while (_queue.Count == 0) Monitor.Wait(_locker);
                    product = _queue.Dequeue();
                }

                if (product == null)
                    return; // This signals our exit

                _action.Invoke(product);
            }
        }

        public void Dispose()
        {
            // Enqueue one null task per worker to make each exit.
            foreach (var worker in _workers)
                EnqueueProductForProcessing(null);
            foreach (var worker in _workers)
                worker.Join();
        }
    }
}