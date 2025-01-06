namespace NetworkPool
{
    static class Utils
    {
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            var enu = values.GetEnumerator();
            while (enu.MoveNext())
            {
                action(enu.Current);
            }
        }

        public static void Clear(this byte[] bytes) => bytes.ForEach(x => x = 0);
    }
}
