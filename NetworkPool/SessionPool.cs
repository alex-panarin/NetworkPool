namespace NetworkPool
{
    internal class SessionPool
        : JobPool<Session>
    {
        private readonly IProcessor<Session> _processor;

        public SessionPool(IProcessor<Session> processor)
            : base()
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }
        protected override async Task<bool> DoRead(Session session)
        {
            await session.ReadAsync();

            return await _processor.ProcessRead(session); 
        }
        protected override async Task<bool> DoWrite(Session session)
        {
            return await _processor.ProcessWrite(session); 
        }
    }
}

