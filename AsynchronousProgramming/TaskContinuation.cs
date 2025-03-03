using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AsynchronousProgramming
{
    class TaskContinuationDemo
    {

        public void Demo()
        {
            var task = Task.Run(() =>
            {
                int sum = 0;
                for (int i = 1; i <= 100; i++)
                {
                    Task.Delay(100);
                    sum += 1;
                }
                return sum;
            });

            //task.Wait();
            // Getting the task result is blocking:
            var result = task.Result;

            Console.WriteLine($"The result is: {task.Result}");
            Console.ReadLine();
        }

        public void GetPokemon()
        {
            using var client = new HttpClient();
            var task = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon");
            var result = task.Result;

            Console.WriteLine(result);
        }

        public void GetPokemonContinueWith()
        {
            using var client = new HttpClient();
            var task = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon");
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

        public void GetPokemonContinueWithChain()
        {
            using var client = new HttpClient();
            var taskPokemonListJson = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon");
            var taskGetFirstPokemonUrl = taskPokemonListJson.ContinueWith(t =>
            {
                var result = t.Result;
                var doc = JsonDocument.Parse(result);
                JsonElement root = doc.RootElement;
                JsonElement results = root.GetProperty("results");
                JsonElement firstPokemon = results[0];
                return firstPokemon.GetProperty("url").ToString();
            });

            var taskGetFirstPokemonDetailsJson = taskGetFirstPokemonUrl.ContinueWith(t =>
            {
                var result = t.Result;
                return client.GetStringAsync(result);
            }).Unwrap(); // This is needed so that we revert to the original task

            taskGetFirstPokemonDetailsJson.ContinueWith(t =>
            {
                var result = t.Result;
                var doc = JsonDocument.Parse(result);
                JsonElement root = doc.RootElement;
                Console.WriteLine($"Name: {root.GetProperty("name").ToString()}");
                Console.WriteLine($"Weight: {root.GetProperty("weight").ToString()}");
                Console.WriteLine($"Height: {root.GetProperty("height")}");
            });
            Console.WriteLine("This is the end of the program.");
            Console.ReadLine();
        }
    }
}
