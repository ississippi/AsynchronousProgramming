using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace AsynchronousProgramming
{
    class ConcurrentStackDemo
    {
        public void Demo()
        {
            var stack = new ConcurrentStack<int>();

            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            stack.TryPop(out var result);

            Console.WriteLine($"result: {result}");

        }
    }
}
