namespace ThreadPool;

public static class Program
{
    public static int Main(string[] _)
    {
        var rnd = new Random();

        var numberOfThreads = (uint)rnd.Next(1, 5);
        var numberOfTasks = rnd.Next(1, 100);

        Console.WriteLine($"The number of threads: {numberOfThreads}.");
        Console.WriteLine($"The number of tasks: {numberOfTasks}.");
        
        var threadPool = new ThreadPool(numberOfThreads);

        for (var i = 0; i < numberOfTasks; i++)
        {
            threadPool.Enqueue(Task.Exec);
        }
        
        Thread.Sleep(1000);
        
        threadPool.Dispose();

        Console.WriteLine("Done.");

        return 0;
    }
}
