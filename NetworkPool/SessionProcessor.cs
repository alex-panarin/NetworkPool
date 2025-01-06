namespace NetworkPool
{
    internal interface IProcessor<TArg>
    {
        void ProcessRead(TArg arg);
        void ProcessWrite(TArg arg);
    }
    internal class SessionProcessor
        : IProcessor<Session>
    {
        public void ProcessRead(Session session)
        {
            session.State = JobState.Write; // Need Answer
            Console.WriteLine($"=== Thread: {Environment.CurrentManagedThreadId} => Data: {session.GetLastValue()} ===");
        }

        public void ProcessWrite(Session session)
        {
            session.State = JobState.Read;
        }
    }
}
