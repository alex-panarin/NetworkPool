namespace NetworkPool
{
    internal class SessionPool
        : JobPool<Session>
    {
        private readonly IProcessor<Session> _processor;

        public SessionPool(IProcessor<Session> processor)
            : base()
        {
            _processor = processor;
        }
        protected override async Task<bool> DoRead(Session session)
        {
            var buffer = await session.ReadAsync();

            if (buffer.HasClosed)
            {
                Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Remove connection {session.Id} ===");
                session.Dispose();
                return false;
            }

            if (buffer.IsNotEmpty)
            {
                _processor.ProcessRead(session);
            }
            return session.State == JobState.Write;
        }
        protected override async Task<bool> DoWrite(Session session)
        {
            _processor?.ProcessWrite(session);
            await session.WriteAsync($"Echo: {session.GetLastValue()}");

            return session.State == JobState.Read;
        }
    }
}

