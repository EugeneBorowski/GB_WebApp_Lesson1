using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GB_WebApp_Lesson1
{
    internal class Program
    {
        private static readonly CancellationTokenSource cts = new();
        private static readonly HttpClient client = new();
        private static readonly string typicodeURL = "https://jsonplaceholder.typicode.com/posts/";
        private static readonly int startId = 4;
        private static readonly int endId = 13;
        private static readonly string resultFile = "result.txt";
        private static readonly object lockObject = new();

        static async Task Main(string[] args)
        {
            try
            {
                await TypicodeCall();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                cts.Dispose();
            }
        }

        static async Task TypicodeCall()
        {
            var tasks = new List<Task>();

            for (int i = startId; i <= endId; i++)
            {
                tasks.Add(GetResponce(i));
            }

            await Task.WhenAll(tasks);
        }

        static async Task GetResponce(int postId)
        {
            var response = await client.GetAsync(typicodeURL + postId, cts.Token);
            var content = await response.Content.ReadAsStringAsync(cts.Token);
            //Console.WriteLine(content);
            var str = JsonSerializer.Deserialize<Responce>(content);
            Console.WriteLine(MakeString(str));
            var writestring = MakeString(str);
            WriteTask(writestring);
        }

        static void WriteTask(string content)
        {
            lock (lockObject)
            {
                using var file = new StreamWriter(resultFile, true);
                file.WriteAsync(content);
            }
        }

        static string MakeString(Responce responce)
        {
            return responce.userId + "\n" + responce.id + "\n" + responce.title + "\n" + responce.body + "\n" + "\n";
        }
    }
}
