using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    class DivideAndConquer
    {
        int[] _array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public void Demo()
        {

            var startTime = DateTime.Now;

            var numOfTasks = 4;
            var segmentLength = _array.Length / numOfTasks;


            Task<int>[] tasks = new Task<int>[numOfTasks];
            tasks[0] = Task.Run(() => { return SumSegment(0, segmentLength); });
            tasks[1] = Task.Run(() => { return SumSegment(segmentLength, 2 * segmentLength); });
            tasks[2] = Task.Run(() => { return SumSegment(2 * segmentLength, 3 * segmentLength); });
            tasks[3] = Task.Run(() => { return SumSegment(3 * segmentLength, _array.Length); });

            //Console.WriteLine($"The sum is {tasks[0].Result + tasks[1].Result + tasks[2].Result + tasks[3].Result}.");
            //Console.WriteLine($"The sum is {tasks.Sum(t => t.Result)}");

            Task.WhenAll(tasks).ContinueWith(t =>
            {
                Console.WriteLine($"The summary is {t.Result.Sum()}");
            });

            Console.ReadLine();
        }

        int SumSegment(int start, int end)
        {
            int segmentSum = 0;
            for (int i = start; i < end; i++)
            {
                Thread.Sleep(100);
                segmentSum += _array[i];
            }

            return segmentSum;
        }

    }
}
