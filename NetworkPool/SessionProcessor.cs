using System;

namespace NetworkPool
{
    internal interface IProcessor<TArg>
    {
        Task<bool> ProcessRead(TArg arg);
        Task<bool> ProcessWrite(TArg arg);
    }
    internal class SessionProcessor
        : IProcessor<Session>
    {
        public Task<bool> ProcessRead(Session session)
        {
            var buffer = session.GetLastData();
            if (buffer.HasClosed)
            {
                Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Remove connection {session.Id} ===");
                session.Dispose();
                return Task.FromResult(false);
            }
            if (buffer.IsNotEmpty)
            {
                session.State = JobState.Write; // Need Answer
            }
            
            Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Data: {session.GetLastValue()} ===");

            return Task.FromResult(session.State == JobState.Write);
        }

        public async Task<bool> ProcessWrite(Session session)
        {
            await session.WriteAsync($"Echo: {session.GetLastValue()}");
            session.State = JobState.Read;

            return session.State == JobState.Read;
        }
    }
}
