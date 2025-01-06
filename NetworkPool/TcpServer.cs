using System.Net.Sockets;

namespace NetworkPool
{
    internal class TcpServer
        : SessionPool
    {
        readonly TcpListener _listener;
        
        public TcpServer(IProcessor<Session> processor, int port = 9999)
            : base(processor)
        {
            _listener = TcpListener.Create(port);
        }

        public Task Start()
        {
            var task = Task.Factory.StartNew((l) =>
            {
                var listner = (TcpListener)l!;
                _listener.Start();
                Console.WriteLine("Server running ...");
                try
                {
                    while (true)
                    {
                        var session = new Session(_listener.AcceptSocket());
                        session.State = JobState.Read; // Need to read session data
                        AddJob(session);

                        Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Add new connection {session.Id} ===");
                    }
                }
                catch (SocketException x)
                {
                    if(x.SocketErrorCode == SocketError.OperationAborted
                        || x.SocketErrorCode == SocketError.Interrupted)
                    {
                        Console.WriteLine("Operation terminated by user");
                    }
                }
            }
            , _listener
            , TaskCreationOptions.LongRunning);

            Join();
            return task;
        }

        public void Stop()
        {
            Close();
            _listener.Stop();
        }
        public override void Dispose()
        {
            Stop();
            _listener.Dispose();
            base.Dispose();
        }
    }        
}

