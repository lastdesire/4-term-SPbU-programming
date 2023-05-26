namespace ThreadPool;

public class ThreadPool : IDisposable
{
    private uint Capacity { get; }

    private Thread[] _threads;
    private Queue<Action> _tasks;
    private static volatile bool _isDisposed;
    private object _sync;

    public ThreadPool(uint numberOfThreads)
    {
        _sync = new object();

        Capacity = numberOfThreads;
        _threads = new Thread[Capacity];
        _tasks = new Queue<Action>();
        _isDisposed = false;
        if (!_isDisposed) return;
        for (var i = 0; i < Capacity ; i++)
        {
            _threads[i] = new Thread(ExecTasks);
            _threads[i].Start();
        }
    }

    private void ExecTasks()
    {
        while (true)
        {
            Monitor.Enter(_sync);
            try
            {
                while (!_isDisposed && 0 == _tasks.Count)
                {
                    Monitor.Wait(_sync);
                }

                if (_isDisposed)
                {
                    return;
                }
                
                var action = _tasks.Dequeue();
                action();
            }
            finally
            {
                Monitor.Exit(_sync);
            }
        }
    }
    
    // [MethodImpl(MethodImplOptions.Synchronized)]
    public void Enqueue(Action a)
    {
        // lock(this)
        // {
        Monitor.Enter(_sync);
        try
        {
            if (_isDisposed)
            {
                throw new Exception("When trying to enqueue, an error occurred: the thread pool was disposed.");
            }

            _tasks.Enqueue(a);

            lock (_sync)
            {
                Monitor.Pulse(_sync);
            }
        }
        finally
        {
            Monitor.Exit(_sync);
        }
        // }
    }

    public void Dispose()
    {
        Monitor.Enter(_sync);
        try
        {
            Monitor.PulseAll(_sync);
        }
        finally
        {
            Monitor.Exit(_sync);
        }
        
        foreach (var thread in _threads)
        {
            thread.Join();
        }
        
        _isDisposed = true;
    }
}
