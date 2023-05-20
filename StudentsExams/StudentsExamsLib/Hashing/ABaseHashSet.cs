namespace StudentsExamsLib.Hashing
{
    public abstract class BaseHashSet<T> : IHashSet<T>
    {
        protected List<T>[] Table;
        protected int SetSize;
        private IEqualityComparer<T> _comparer;
        public BaseHashSet(int capacity, IEqualityComparer<T> comparer)
        {
            SetSize = 0;
            _comparer = comparer;
            Table = new List<T>[capacity];
            for (var i = 0; i < capacity; i++)
            {
                Table[i] = new List<T>();
            }
        }

        protected abstract bool PolicyDemandsResize { get; }

        protected abstract void Resize();
        protected abstract void Acquire(T x);
        protected abstract void Release(T x);

        public bool Contains(T x)
        {
            Acquire(x);
            try
            {
                var myBucket = Math.Abs(_comparer.GetHashCode(x!) % Table.Length);
                return Table[myBucket].Contains(x, _comparer);
            }
            finally
            {
                Release(x);
            }
        }
        
        public bool Add(T x)
        {
            var result = false;
            Acquire(x);
            try
            {
                var myBucket = Math.Abs(_comparer.GetHashCode(x!) % Table.Length);
                if (!Table[myBucket].Contains(x, _comparer))
                {
                    Table[myBucket].Add(x);
                    result = true;
                    SetSize++;
                }
            }
            finally
            {
                Release(x);
            }
            if (PolicyDemandsResize)
                Resize();
            return result;
        }
        
        public bool Remove(T x)
        {
            Acquire(x);
            try
            {
                var myBucket = Math.Abs(_comparer.GetHashCode(x!) % Table.Length);
                var set0 = Table[myBucket];
                var i = set0.FindIndex(y => _comparer.Equals(x, y));
                if (i == -1)
                {
                    return false;
                }

                set0.RemoveAt(i);
                SetSize--;
                return true;
            }
            finally
            {
                Release(x);
            }
        }
    }
}
