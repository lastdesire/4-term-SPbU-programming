﻿using MPI;

namespace Sort;

public static class Sort
{
    public static int Main(string[] args)
    {
        using (var _ = new MPI.Environment(ref args))
        {
            var comm = Communicator.world;
            var rank = comm.Rank;
            var size = comm.Size;
            var list = new List<int>();
            
            switch (rank)
            {
                case 0 when 2 != args.Length:
                    Console.WriteLine("Expected paths to input and output files as arguments.");
                    return -1;
                case 0:
                {
                    list = ListReader.GetIntListFromFile(args[0]);

                    if (ListReader.WithError)
                    {
                        Console.WriteLine($"Error trying read data from file with path {args[0]}");
                        comm.Scatter(Enumerable.Repeat(0, size).ToArray(), 0); 
                        return -1;
                    }

                    comm.Scatter(Enumerable.Repeat(1, size).ToArray(), 0);
                    break;
                }
                default:
                {
                    var scattered = comm.Scatter<int>(0);
                    if (scattered == 0)
                    {
                        return -1;
                    }

                    break;
                }
            }

            List<int> scatteredList;
            if (0 == rank)
            {
                var arrayList = new List<int>[size];
                var savedListSize = list.Count;
                
                for (var i = 0; i < size - 1; i++)
                {
                    arrayList[i] = list.Take(savedListSize / size).ToList();
                    list.RemoveRange(0, savedListSize / size);
                }

                arrayList[size - 1] = list;
                scatteredList = comm.Scatter(arrayList.ToArray(), 0);
            }
            else
            {
                scatteredList = comm.Scatter<List<int>>(0);
            }

            for (var i = 1; i <= size; i++)
            {
                if ((i + rank + 1) % 2 == 0 )
                {
                    if (rank + 1 != size)
                    {
                        comm.Send(scatteredList, rank + 1, 0);
                        scatteredList = comm.Receive<List<int>>(rank + 1, 0);
                    }
                }
                else if (rank + 1 != 1)
                {
                    var combinedList  = comm.Receive<List<int>>(rank - 1, 0).Concat(scatteredList).ToList();
                    combinedList.Sort();
                    var listToSend = combinedList.Take(combinedList.Count / 2).ToList();
                    comm.Send(listToSend, rank - 1, 0);
                    combinedList.RemoveRange(0, combinedList.Count / 2);
                    scatteredList = combinedList;
                }

            }

            var result = new List<int>();
            result = result.Concat(scatteredList).ToList();
            if (rank == 0)
            {
                for (var i = 1; i < size; i++)
                {
                    result = result.Concat(comm.Receive<List<int>>(i, 0)).ToList();
                }

                foreach (var item in result)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                comm.Send(scatteredList, 0, 0);
            }
            
            return 0;
        }
    }
}
