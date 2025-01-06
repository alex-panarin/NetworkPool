namespace NetworkPool
{
    internal partial class Program
    {
        static async Task Main(string[] args)
        {
            using var server = new TcpServer(new SessionProcessor());

            var task = Task.Run(() =>
            {
                Console.WriteLine("Type \"stop\" to interrupt the job :)");
                while (true)
                {
                    var input = Console.ReadLine();
                    if (input == "stop")
                    {
                        server.Stop();
                        Console.WriteLine("Server stoped...");
                        break;
                    }
                }
            });

            await server.Start();
            await task;
        }
    }
}
