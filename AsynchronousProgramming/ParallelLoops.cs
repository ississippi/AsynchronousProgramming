using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    class ParallelLoops
    {
        public void Demo()
        {
            //int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int[] array = Enumerable.Range(0, 100).ToArray();

            int sum = 0;
            object lockSum = new object();

            Parallel.For(0, array.Length, i =>
            {
                lock (lockSum)
                {
                    sum += array[i];
                }
            });
            Console.WriteLine($"The sum of For() is {sum}");
            sum = 0;
            Parallel.ForEach(array, item =>
            {
                lock (lockSum)
                {
                    sum += item;
                    Console.WriteLine($"Current task id: {Task.CurrentId}");
                }
            });

            Console.WriteLine($"The sum of ForEach() is {sum}");

            Console.ReadLine();
        }

        public void DemoParallelInvoke()
        {
            Parallel.Invoke(() =>
            {
                Console.WriteLine("I am one.");
            },
            () =>
            {
                Console.WriteLine("I am two.");
            },
            () =>
            {
                Console.WriteLine("I am three.");
            });


            Console.ReadLine();
        }

        public void DemoParallelExceptions()
        {
            int[] array = Enumerable.Range(0, 100).ToArray();

            int sum = 0;
            object lockSum = new object();


            try
            {
                Parallel.For(0, array.Length, (i, state) =>
                {
                    lock (lockSum)
                    {
                        if (!state.IsExceptional)
                        {
                            if (i == 65)
                                throw new InvalidOperationException("This is on purpose.");

                            sum += array[i];
                            Console.WriteLine($"Current task id: {Task.CurrentId}; Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}");
                        }
                    }
                });


            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"The sum is {sum}");

            Console.ReadLine();
        }

        public void DemoStop()
        {
            int[] array = Enumerable.Range(0, 100).ToArray();

            int sum = 0;
            object lockSum = new object();


            try
            {
                Parallel.For(0, array.Length, (i, state) =>
                {
                    lock (lockSum)
                    {
                        if (!state.IsStopped)
                        {
                            if (i == 65)
                                state.Stop();

                            sum += array[i];
                            Console.WriteLine($"Current task id: {Task.CurrentId}; Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}");
                        }
                    }
                });


            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"The sum is {sum}");

            Console.ReadLine();
        }

        public void DemoBreak()
        {
            int[] array = Enumerable.Range(0, 100).ToArray();

            int sum = 0;
            object lockSum = new object();


            try
            {
                Parallel.For(0, array.Length, (i, state) =>
                {
                    lock (lockSum)
                    {
                        if (state.ShouldExitCurrentIteration && state.LowestBreakIteration < i)
                            return;


                        if (i == 65)
                            state.Break();

                        sum += array[i];
                        Console.WriteLine($"Current task id: {Task.CurrentId}; Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}");

                    }
                });
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"The sum is {sum}");
            Console.ReadLine();
        }
        public void DemoCancel()
        {
            using var cts = new CancellationTokenSource();
            var token = cts.Token;

            var task = Task.Run(() => { WorkCancel(cts); }, token); Console.WriteLine("To cancel, press 'c'");
            var input = Console.ReadLine();
            if (input == "c")
            {
                cts.Cancel();
            }


            task.Wait();
            Console.WriteLine($"Task status is: {task.Status}");
            Console.ReadLine();
        }
        void WorkCancel(CancellationTokenSource cts)
        {
            Console.WriteLine("Started doing the work.");

            var options = new ParallelOptions { CancellationToken = cts.Token };

            try
            {
                Parallel.For(0, 100000, options, i =>
                {
                    Console.WriteLine($"{DateTime.Now}");
                    Thread.SpinWait(30000000);
                });
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.ToString());
            }


            Console.WriteLine("Work is done.");

        }

        public void DemoThreadLocalStorage()
        {
            int[] array = Enumerable.Range(1, 1000000).ToArray();

            long sum = 0;
            object lockSum = new object();
            DateTime beginTime = DateTime.Now;
            Parallel.For(0, array.Length, 
                () => 0,
                (i, state, tls) =>
                {
                    tls += array[i];
                    return tls;
                },
                tls =>
                {
                    lock (lockSum)
                    {
                        sum += tls;
                        Console.WriteLine($"The task id: {Task.CurrentId}");
                    }
                }
            );

            // for simple operations such as getting the sum of this array, or for a very huge number
            // of iterations in a loop, a simeple for-loop is much faster than a Parallel loop.
            //for(int i = 0; i < array.Length; i++)
            //{
            //    sum += array[i];
            //}

            DateTime endTime = DateTime.Now;

            Console.WriteLine($"The sum is {sum}");
            Console.WriteLine($"Timespent: {(endTime - beginTime).TotalMilliseconds}");
            Console.ReadLine();
        }
    }
}
