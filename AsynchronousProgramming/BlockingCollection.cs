using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace AsynchronousProgramming
{
    public class BlockingCollectionDemo
    {
        ConcurrentQueue<string?> _requestQueue;
        BlockingCollection<string?> _collection;
        public BlockingCollectionDemo()
        {
            _requestQueue = new ConcurrentQueue<string?>();
            _collection = new BlockingCollection<string?>(_requestQueue, 3);
        }
        public void Demo()
        {
            // 2. Start the requests queue monitoring thread
            Thread monitoringThread = new Thread(MonitorQueue);
            monitoringThread.Start();

            // 1. Enqueue the requests
            Console.WriteLine("Server is running. Type 'exit' to stop.");
            while (true)
            {
                string? input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                {
                    _collection.CompleteAdding();
                    break;
                }

                _collection.Add(input);

                Console.WriteLine($"Enqueued: {input}; queue size: {_collection.Count}");
            }
        }

        void MonitorQueue()
        {

            foreach (var request in _collection.GetConsumingEnumerable())
            {
                if (_collection.IsCompleted) break;

                Thread processingThread = new Thread(() => ProcessInput(request));
                processingThread.Start();

                Thread.Sleep(2000);
            }
        }

        // 3. Processing the requests
        void ProcessInput(string? input)
        {
            // Simulate processing time
            Thread.Sleep(2000);
            Console.WriteLine($"Processed input: {input}");
        }

    }
}
