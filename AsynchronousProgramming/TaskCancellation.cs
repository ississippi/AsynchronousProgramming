using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    public class TaskCancellationDemo
    {
        CancellationTokenSource _cts;
        CancellationToken _token;

        public TaskCancellationDemo()
        {
            _cts = new CancellationTokenSource();
            _token = _cts.Token;
        }

        public void Demo()
        {

            var task = Task.Run(Work, _token);

            Console.WriteLine("To cancel, press 'c'");
            var input = Console.ReadLine();
            if (input == "c")
            {
                _cts.Cancel();
            }


            task.Wait();
            Console.WriteLine($"Task status is: {task.Status}");
            Console.ReadLine();
        }

        void Work()
        {
            Console.WriteLine("Started doing the work.");

            for (int i = 0; i < 100000; i++)
            {
                Console.WriteLine($"{DateTime.Now}");

                if (_token.IsCancellationRequested)
                {
                    Console.WriteLine($"User requested cancellation at iteration: {i}");
                    //break;
                    //throw new OperationCanceledException();
                    _token.ThrowIfCancellationRequested();
                }

                Thread.SpinWait(30000000);
            }

            Console.WriteLine("Work is done.");

        }

        public void DemoCancelAfter()
        {
            using CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(10);

            using var client = new HttpClient();
            var taskPokemonListJson = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon", source.Token);

            Console.WriteLine(taskPokemonListJson.Result);
        }
    }
}
