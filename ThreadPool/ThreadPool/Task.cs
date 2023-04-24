namespace ThreadPool;

// Tasks that are performed using a thread pool.
public static class Task
{
    private static Random _rnd = new Random();
    
    public static void Exec()
    {
        Console.WriteLine($"Thread with the index {Environment.CurrentManagedThreadId} has started working on the task.");
        
        var j = _rnd.Next(1, 10000);
        var res = GetSum(j);
        
        Console.WriteLine($"Thread with the index {Environment.CurrentManagedThreadId} has finished working on the task: sum from 1 to {j} := {res}.");
    }

    private static int GetSum(int j)
    {
        var result = 0;
        
        for (var i = 1; i <= j; i++)
        {
            result += i;
        }

        Thread.Sleep(_rnd.Next(1, 100));
        
        return result;
    }
}
