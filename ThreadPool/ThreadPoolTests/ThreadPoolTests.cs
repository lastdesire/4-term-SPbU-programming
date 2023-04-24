using NUnit.Framework;
using ThreadPool;

namespace ThreadPoolTests;

public class ThreadPoolTests
{
    private volatile int _x = 0;
    private Random _rnd = new Random();
    
    [Test]
    public void ThreadPoolTest()
    {
        _x = 0;
        var numberOfThreads = (uint)_rnd.Next(1, 5);
        var numberOfTasks = _rnd.Next(1, 100);
        
        
        var threadPool = new ThreadPool.ThreadPool(numberOfThreads);
        
        for (var i = 0; i < numberOfTasks; i++)
        {
            threadPool.Enqueue(() => { Interlocked.Increment(ref _x); });
        }
        
        Thread.Sleep(1000);
        
        threadPool.Dispose();

        Assert.That(_x, Is.EqualTo(numberOfTasks));
    }
}
