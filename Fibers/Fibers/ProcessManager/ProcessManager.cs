using Fibers.FibersLib;

namespace Fibers.ProcessManager;

public static class ProcessManager
{
    private static bool _withPriority = false;
    
    private static readonly Random Rng = new Random();

    private static int _currFiberIndex = 0;
    
    private static List<Fiber> _fibers = new List<Fiber>();

    private static List<Fiber> _finishedFibers = new List<Fiber>();

    // Key: Id of fiber; value: index of initial list _fibers; Usage: for beautiful display.
    private static Dictionary<uint, int> _dictionaryFiber = new Dictionary<uint, int>();

    private static List<int> _fibersPriorityNumbers = new List<int>();
    private static int _fibersPrioritySum = 0;

    public static void Run(List<Process> processes, bool withPriority)
    {
        _withPriority = withPriority;
        
        if (_withPriority)
        {
            Console.WriteLine("Running with priority...");
            processes = processes.OrderByDescending(process => process.Priority).ToList();
        }
        else
        {
            Console.WriteLine("Running without priority...");
        }

        var i = 0;
        foreach (var process in processes)
        {
            var newFiber = new Fiber(process.Run);
            _fibers.Add(newFiber);
            _dictionaryFiber.Add(newFiber.Id, i);
            
            if (_withPriority)
            {
                _fibersPriorityNumbers.Add(process.Priority);
                _fibersPrioritySum += process.Priority;
            }
            
            i++;
        }
        
        Fiber.Switch(_fibers[0].Id);
        
        foreach (var fiber in _finishedFibers)
        {
            Fiber.Delete(fiber.Id);
            _dictionaryFiber.Clear();
        }
    }
    
    public static void Switch(bool fiberFinished)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(new string(' ',_dictionaryFiber[_fibers[_currFiberIndex].Id] * 8) + $"///{_dictionaryFiber[_fibers[_currFiberIndex].Id]}///");
        Console.ForegroundColor = ConsoleColor.White;

        Console.Write($"Switch from [{_fibers[_currFiberIndex].Id}] ");
        if (fiberFinished)
        {
            _finishedFibers.Add(_fibers[_currFiberIndex]);
            _fibers.RemoveAt(_currFiberIndex);
            if (_withPriority)
            {
                _fibersPrioritySum -= _fibersPriorityNumbers[_currFiberIndex];
                _fibersPriorityNumbers.RemoveAt(_currFiberIndex);
            }
        }
        
        if (_fibers.Count == 0)
        {
            Console.WriteLine("to primary fiber.");
            Fiber.Switch(Fiber.PrimaryId);
        }

        Thread.Sleep(100);
        var nextFiberIndex = 0;
        
        // Just choose random.
        if (!_withPriority)
        {
            nextFiberIndex = Rng.Next(_fibers.Count);
        }
        // Choose one of the random numbers in the set k_1 ... k_1 (k_1 times), ..., k_i ... k_i (k_i times), ..., k_n ... k_n (k_n times).
        else
        {
            var randomLessThanPrioritySum = Rng.Next(_fibersPrioritySum) + 1;
            var sum = 0;
            for (var k = 0; k < _fibersPriorityNumbers.Count; k++)
            {
                sum += _fibersPriorityNumbers[k];
                if (sum >= randomLessThanPrioritySum)
                {
                    nextFiberIndex = k;
                    break;
                }
            }
        }
        
        Console.WriteLine($"to [{_fibers[nextFiberIndex].Id}].");

        _currFiberIndex = nextFiberIndex;
        Fiber.Switch(_fibers[_currFiberIndex].Id);
    }
}