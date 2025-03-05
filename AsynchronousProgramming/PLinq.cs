using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    class PLinq
    {
        public void Demo()
        {
            var items = Enumerable.Range(1, 200);

            var evenNumbers = items.AsParallel().Where(x =>
            {
                Console.WriteLine($"Processing number {x}; Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                return (x % 2 == 0);
            });

            Console.WriteLine();
            //Console.WriteLine($"There are {evenNumbers.Count()} even numbers in the collection.");

            //foreach (var item in evenNumbers)
            //{
            //    Console.WriteLine($"{item}: Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            //}

            evenNumbers.ForAll(item =>
            {
                Console.WriteLine($"{item}: Thread Id: {Thread.CurrentThread.ManagedThreadId}");
            });

        }

        public void DemoExceptionHandling()
        {
            var items = Enumerable.Range(1, 20);

            ParallelQuery<int> evenNumbers = null;


            evenNumbers = items.AsParallel().WithMergeOptions(ParallelMergeOptions.FullyBuffered).Where(x =>
            {
                Console.WriteLine($"Processing number {x}; Thread Id: {Thread.CurrentThread.ManagedThreadId}");

                if (x == 5) throw new InvalidOperationException("This is intentional 5");

                if (x == 20) throw new ArgumentNullException("This is intentional 20");

                return (x % 2 == 0);
            });


            Console.WriteLine();

            try
            {
                evenNumbers.ForAll(item =>
                {
                    Console.WriteLine($"{item}: Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                });
            }
            catch (AggregateException ex)
            {
                ex.Handle(x =>
                {
                    Console.WriteLine(x.Message);
                    return true;
                });
            }

        }

        public void DemoCancellation()
        {
            var items = Enumerable.Range(1, 20);

            ParallelQuery<int> evenNumbers = null;
            var cts = new CancellationTokenSource();

            evenNumbers = items.AsParallel()
                .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                .WithCancellation(cts.Token)
                .Where(x =>
            {
                Console.WriteLine($"Processing number {x}; Thread Id: {Thread.CurrentThread.ManagedThreadId}");

                return (x % 2 == 0);
            });


            Console.WriteLine();

            try
            {
                evenNumbers.ForAll(item =>
                {

                    if (item > 8) cts.Cancel();

                    Console.WriteLine($"{item}: Thread Id: {Thread.CurrentThread.ManagedThreadId}");
                });
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (AggregateException ex)
            {
                ex.Handle(x =>
                {
                    Console.WriteLine(x.Message);
                    return true;
                });
            }

        }

    }
}
