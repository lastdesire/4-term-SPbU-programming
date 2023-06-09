﻿namespace StudentsExamsLib.Hashing
{
    public abstract class PhasedCuckooHashSet<T> : IHashSet<T>
    {
        // list is semi-full
        protected const int Threshold = 50;
        // list is full
        protected const int ListSize = 55;
        // steps to relocate
        protected const int Limit = 60;

        volatile protected int Capacity;
        volatile protected List<T>[,] Table;
        private IEqualityComparer<T> _comparer;
        
        public PhasedCuckooHashSet(int size, IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
            Capacity = size;
            Table = new List<T>[2, Capacity];

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Capacity; j++)
                {
                    Table[i, j] = new List<T>(ListSize);
                }
            }
        }

        protected int Hash0(T i)
        {
            return _comparer.GetHashCode(i!);
        }

        protected int Hash1(T i)
        {
            return _comparer.GetHashCode(i!) - 1;
        }

        public bool Contains(T x)
        {
            Acquire(x);
            try
            {
                var set0 = Table[0, Hash0(x) % Capacity];
                if (set0.Contains(x, _comparer))
                {
                    return true;
                }
                else
                {
                    var set1 = Table[1, Hash1(x) % Capacity];
                    if (set1.Contains(x, _comparer))
                    {
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                Release(x);
            }
        }

        protected abstract void Acquire(T x);
        protected abstract void Release(T x);
        protected abstract void Resize();

        public bool Remove(T x)
        {
            Acquire(x);
            try
            {
                var set0 = Table[0, Hash0(x) % Capacity];
                if (set0.Contains(x, _comparer))
                {
                    var i = set0.FindIndex(y => _comparer.Equals(x, y));

                    if (i == -1)
                    {
                        return false;
                    }

                    set0.RemoveAt(i);
                    return true;
                }
                else
                {
                    var set1 = Table[1, Hash1(x) % Capacity];
                    if (set1.Contains(x, _comparer))
                    {
                        var i = set1.FindIndex(y => _comparer.Equals(x, y));

                        if (i == -1)
                        {
                            return false;
                        }

                        set1.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            finally
            {
                Release(x);
            }
        }

        public bool Add(T x)
        {
            Acquire(x);
            int h0 = Hash0(x) % Capacity, h1 = Hash1(x) % Capacity;
            int i = -1, h = -1;
            var mustResize = false;
            try
            {
                if (Contains(x)) return false;
                List<T> set0 = Table[0, h0];
                List<T> set1 = Table[1, h1];
                if (set0.Count < Threshold)
                {
                    set0.Add(x); return true;
                }
                else if (set1.Count < Threshold)
                {
                    set1.Add(x); return true;
                }
                else if (set0.Count < ListSize)
                {
                    set0.Add(x); i = 0; h = h0;
                }
                else if (set1.Count < ListSize)
                {
                    set1.Add(x); i = 1; h = h1;
                }
                else
                {
                    mustResize = true;
                }
            }
            finally
            {
                Release(x);
            }
            if (mustResize)
            {
                Resize(); Add(x);
            }
            else if (!Relocate(i, h))
            {
                Resize();
            }
            return true; // x must have been present
        }

        protected bool Relocate(int i, int hi)
        {
            var hj = 0;
            var j = 1 - i;
            for (var round = 0; round < Limit; round++)
            {
                var iSet = Table[i, hi];
                var y = iSet[0];
                switch (i)
                {
                    case 0: hj = Hash1(y) % Capacity; break;
                    case 1: hj = Hash0(y) % Capacity; break;
                }
                Acquire(y);
                var jSet = Table[j, hj];
                try
                {
                    if (iSet.Remove(y))
                    {
                        if (jSet.Count < Threshold)
                        {
                            jSet.Add(y);
                            return true;
                        }
                        else if (jSet.Count < ListSize)
                        {
                            jSet.Add(y);
                            i = 1 - i;
                            hi = hj;
                            j = 1 - j;
                        }
                        else
                        {
                            iSet.Add(y);
                            return false;
                        }
                    }
                    else if (iSet.Count >= Threshold)
                    {
                        continue;
                    }
                    else
                    {
                        return true;
                    }
                }
                finally
                {
                    Release(y);
                }
            }
            return false;
        }
    }
}
