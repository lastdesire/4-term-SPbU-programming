namespace StudentsExamsLib.Hashing
{
    public class StripedCuckooHashSet<T> : PhasedCuckooHashSet<T>
    {
        Mutex[,] _locks;

        public StripedCuckooHashSet(int capacity, IEqualityComparer<T> comparer) : base(capacity, comparer)
        {
            _locks = new Mutex[2, capacity];
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < capacity; j++)
                {
                    _locks[i, j] = new Mutex();
                }
            }
        }


        protected override void Acquire(T x)
        {
            _locks[0, Hash0(x) % _locks.GetLength(1)].WaitOne();
            _locks[1, Hash1(x) % _locks.GetLength(1)].WaitOne();
        }

        protected override void Release(T x)
        {
            _locks[0, Hash0(x) % _locks.GetLength(1)].ReleaseMutex();
            _locks[1, Hash1(x) % _locks.GetLength(1)].ReleaseMutex();
        }

        protected override void Resize()
        {
            var oldCapacity = Capacity;
            for (var i = 0; i < oldCapacity; i++)
            {
                _locks[0, i].WaitOne();
            }

            try
            {
                if (Capacity != oldCapacity)
                {
                    return;
                }
                var oldTable = Table;
                Capacity = 2 * Capacity;
                Table = new List<T>[2, Capacity];

                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < Capacity; j++)
                    {
                        Table[i, j] = new List<T>(ListSize);
                    }
                }

                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < Capacity; j++)
                    {
                        foreach (var z in oldTable[i, j])
                        {
                            Add(z);
                        }
                    }
                }
            }
            finally
            {
                for (var i = 0; i < oldCapacity; i++)
                {
                    _locks[0, i].ReleaseMutex();
                }
            }
        }
    }
}
