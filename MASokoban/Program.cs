using System;
using MASokoban.Planning;
using MASokoban.Entities;
using MASokoban.Environment;
using MASokoban.MAvis;
using MASokoban.ConflictBasedSolver;
using System.Collections.Generic;
using MASokoban.Heuristics;

namespace MASokoban
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // ----------- USER INPUT -------------

            bool isDebug = false;
            
            string path = @"C:\Users\Count\source\repos\ImprovedMAC\MASokoban\Levels\MADouble.lvl";

            // ------------------------------------

            var initialStates = LevelReader.readLevelFromFile(path);
            List<(WorldState, List<(EntityLocation, int[,], float priority)>)> hMatrix = BFS.PrecalcH(initialStates);
            var testCBS = new CBS(hMatrix);
            var testSolution = testCBS.findSolution();

            if (isDebug == false)
            {
                List<string> lines = PrintSolution.printSolutionToFile(testSolution);

                int counter = 0;
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                    string result = Console.ReadLine();

                    counter++;
                    if (!result.Contains("false")) continue;
                    else throw new Exception(line + result + "counter = " + counter);

                }
            }
        }
    }
}
