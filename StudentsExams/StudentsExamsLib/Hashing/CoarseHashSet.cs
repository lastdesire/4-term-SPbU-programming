namespace StudentsExamsLib.Hashing
{
    public class CoarseHashSet<T> : BaseHashSet<T>
    {
        public Mutex Lock = new Mutex();
        public CoarseHashSet(int capacity, IEqualityComparer<T> comparer) : base(capacity, comparer)
        {
        }

        protected override bool PolicyDemandsResize
        {
            get
            {
                return SetSize / Table.Length > 4;
            }
        }
        protected override void Acquire(T x)
        {
            Lock.WaitOne();
        }

        protected override void Release(T x)
        {
            Lock.ReleaseMutex();
        }

        protected override void Resize()
        {
            var oldCapacity = Table.Length;
            Lock.WaitOne();
            try
            {
                if (oldCapacity != Table.Length)
                {
                    return; // someone beat us to it
                }
                var newCapacity = 2 * oldCapacity;
                var oldTable = Table;
                Table = new List<T>[newCapacity];
                for (var i = 0; i < newCapacity; i++)
                    Table[i] = new List<T>();
                foreach (var bucket in oldTable)
                {
                    foreach (var x in bucket)
                    {
                        Table[x!.GetHashCode() % Table.Length].Add(x);
                    }
                }
            }
            finally
            {
                Lock.ReleaseMutex();
            }
        }
    }
}
