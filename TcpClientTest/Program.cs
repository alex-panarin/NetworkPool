using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpClientTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<Task> tasks = 
                new List<Task>(Enumerable.Range(0, 500)
                    .Select(i =>
                    {
                        return Task.Factory.StartNew(async () =>
                         {
                             try
                             {
                                 using TcpClient tcpClient = new TcpClient("127.0.0.1", 9999);

                                 await tcpClient.Client.SendAsync(Encoding.UTF8.GetBytes($" {i} Hello"));

                                 var bytes = new byte[1024];
                                 var number = tcpClient.Client.Receive(bytes);

                                 Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, number));
                             }
                             catch (Exception ex)
                             {
                                 Console.WriteLine($"Error === {ex.Message}");
                             }
                         });
                    })
                );

            Task.WhenAll(tasks).Wait();
            sw.Stop();
            Console.WriteLine($"Result: {sw.ElapsedMilliseconds} ms");

            Console.ReadKey();
        }
    }
}
