using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkPool
{
    record SessionBuffer
    {
        public int Size = -1;
        public byte[]? Data;
        public bool HasClosed => Size == 0;
        public bool IsNotEmpty => Size > 0;
        public string GetString() => IsNotEmpty && Data != null
            ? Encoding.UTF8.GetString(Data, 0, Size)
            : string.Empty;
        public void Clear() => Data?.Clear();
    }
    internal class Session
        : IJobState
        , IDisposable
    {
        public Session(Socket socket)
        {
            Socket = socket;
        }
        protected Socket Socket;
        public EndPoint? Address => Socket?.RemoteEndPoint;
        public string? Id => Address?.ToString();

        public JobState State { get; internal set; }

        protected SessionBuffer buffer = new SessionBuffer { Data = new byte[ushort.MaxValue] };
        private bool _disposed = false;
        internal async Task<SessionBuffer> ReadAsync() => await Task.FromResult(Read());
        internal Task WriteAsync(string val)  
        {
            Write(val);
            return Task.CompletedTask;
        }
        public SessionBuffer Read()
        {
            buffer.Size = -1;
            lock (Socket)
            {
                try
                {
                    Socket.Blocking = false;
                    buffer.Size = Socket.Receive(buffer.Data !, SocketFlags.None);
                }
                catch (SocketException x)
                {
                    if (x.SocketErrorCode != SocketError.WouldBlock)
                        throw;

                }
                finally
                {
                    Socket.Blocking = true;
                }
                return buffer;
            }
        }
        public void Write(string val)
        {
            lock (Socket)
            {
                try
                {
                    var bytes = Encoding.UTF8.GetBytes(val);
                    buffer.Clear();
                    Buffer.BlockCopy(bytes, 0, buffer.Data !, 0, bytes.Length);
                    Socket.Send(buffer.Data !, bytes.Length, SocketFlags.None);
                }
                catch (SocketException)
                {
                    throw;
                }
                finally
                {
                }
            }
        }
        public override string ToString() => $"Session: {Id}";
        public string GetLastValue() => buffer.GetString();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
            _disposed = true;

            if (Socket?.Connected == true)
                Socket?.Close();

            if(disposing)
                Socket?.Dispose();
        }
        ~Session () => Dispose(false);
    }
}
