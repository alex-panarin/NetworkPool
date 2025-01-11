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
        public async Task<bool> ProcessRead(Session session)
        {
            await session.ReadAsync();

            var buffer = session.GetLastData();
            if (buffer.HasClosed)
            {
                //Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Remove connection {session.Id} ===");
                session.Dispose();
                return false;
            }
            if (buffer.IsNotEmpty)
            {
                //Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Read: {session.GetLastValue()} ===");
                session.State = JobState.Write; // Need Answer
            }
            return session.State != JobState.Close;
        }

        public async Task<bool> ProcessWrite(Session session)
        {
            var value = session.GetLastValue();
            //Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Write: {value} ===");
            await session.WriteAsync($"Echo: {value}");
            session.State = JobState.Read;

            return session.State == JobState.Read;
        }
    }
}
