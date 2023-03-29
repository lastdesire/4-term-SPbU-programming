using Fibers.ProcessManager;

namespace Fibers;

class Program
{
    static int Main(string[] args)
    {
        var processes = new List<Process>();
        for (var i = 0; i < 5; i++)
        {
            processes.Add(new Process());
        }
        ProcessManager.ProcessManager.Run(processes, true); 

        Console.WriteLine("Done.");
        
        return 0;
    }
}