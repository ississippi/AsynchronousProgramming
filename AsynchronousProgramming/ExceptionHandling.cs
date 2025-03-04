using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    class ExceptionHandling
    {
        public void Demo()
        {
            // 1. Exceptions in Tasks are hidden. 
            using var client = new HttpClient();
            var task = client.GetStringAsync("https://pokeapi123.co/api/v2/pokemon");
            task.ContinueWith(t =>
            {
                var result = t.Result;
                var doc = JsonDocument.Parse(result);
                JsonElement root = doc.RootElement;
                JsonElement results = root.GetProperty("results");
                JsonElement firstPokemon = results[0];

                Console.WriteLine($"First pokemon name: {firstPokemon.GetProperty("name")}");
                Console.WriteLine($"First pokemon url: {firstPokemon.GetProperty("url")}");
            });

            Console.WriteLine("This is the end of the program.");
            Console.ReadLine();
        }

        public void DemoExceptionHandling()
        {
            var tasks = new[]
            {
                Task.Run(() => throw new InvalidOperationException("Invalid operation exception")),
                Task.Run(() => throw new ArgumentNullException("Argument null exception")),
                Task.Run(() => throw new Exception("General exception"))
            };

            Task.WhenAll(tasks).ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    foreach (var ex in t.Exception.InnerExceptions)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                Console.WriteLine("Press enter key to exit.");
                Console.ReadLine();
            });
        }

        public void DemoExceptionThrow()
        {
            var tasks = new[]
            {
                Task.Run(() => throw new InvalidOperationException("Invalid operation exception")),
                Task.Run(() => throw new ArgumentNullException("Argument null exception")),
                Task.Run(() => throw new Exception("General exception"))
            };

            var t = Task.WhenAll(tasks);

            t.Wait();  // this will actually trigger throwing of the exception
            // using the await keyword will do the same.

            Console.WriteLine("Press enter key to exit.");
            Console.ReadLine();
        }
    }
}
