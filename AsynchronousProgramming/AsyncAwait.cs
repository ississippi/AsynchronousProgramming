using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    class AsyncAwait
    {
        public void Demo()
        {
            OutputFirstPokemon();

            Console.WriteLine("This is the end of the program.");

            Console.ReadLine();
        }

        public async Task DemoAwait()
        {
            Console.WriteLine($"1. Main thread id:{Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("Starting to do work.");
            
            var data = await FetchDataAsync();
            Console.WriteLine($"Result from async method is {data}");
            Console.WriteLine($"2. Thread id:{Thread.CurrentThread.ManagedThreadId}");

            Console.ReadLine();
        }

        async void OutputFirstPokemon()
        {
            using var client = new HttpClient();
            var taskGetPokemonList = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon");

            var response = await taskGetPokemonList;

            var doc = JsonDocument.Parse(response);
            JsonElement root = doc.RootElement;
            JsonElement results = root.GetProperty("results");
            JsonElement firstPokemon = results[0];

            Console.WriteLine($"First pokemon name: {firstPokemon.GetProperty("name")}");
            Console.WriteLine($"First pokemon url: {firstPokemon.GetProperty("url")}");
        }

        static async Task<string> FetchDataAsync()
        {
            Console.WriteLine($"3. Thread id:{Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(2000);

            Console.WriteLine($"4. Thread id:{Thread.CurrentThread.ManagedThreadId}");

            return "Complex data";
        }
    }
}
