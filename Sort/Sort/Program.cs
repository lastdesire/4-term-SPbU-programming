using MPI;

namespace Sort;

public static class Sort
{
    public static int Main(string[] args)
    {
        using (var env = new MPI.Environment(ref args))
        {
            var comm = Communicator.world;
            var rank = comm.Rank;
            
            if (0 == rank && 2 != args.Length)
            {
                Console.WriteLine("Expected paths to input and output files as arguments.");
                return -1;
            }
            
            var size = comm.Size;

            if (0 == rank)
            {
                var list = ListReader.GetIntListFromFile(args[0]);

                if (ListReader.WithError)
                {
                    Console.WriteLine($"Error trying read data from file with path {args[0]}");
                    return -1;
                }
                else
                {
                    foreach (var item in list)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
         
            return 0;
        }
    }
}
