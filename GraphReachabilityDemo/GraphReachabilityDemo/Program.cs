using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphReachabilityDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            var graph = new Graph();

            //download more graphs from https://snap.stanford.edu/data/#citnets
            graph.Load("ExampleGraphs\\CA-GrQc.txt");

            var sw = Stopwatch.StartNew();
            var randomizedNumbers25 = graph.GenerateNumberOfReachableNodes(ReachableNodeMethod.RandomizedApproach, 100);
            sw.Stop();
            var timeRandomized = sw.Elapsed;
            Console.WriteLine("25 iterations:{0}", timeRandomized);
            
            Stopwatch sw1 = Stopwatch.StartNew();
            var actualResults = graph.GenerateNumberOfReachableNodes(ReachableNodeMethod.BreadthFirstSearch);
            sw1.Stop();
            var timeBFS = sw1.Elapsed;

            Console.WriteLine("25 iterations. Random:{0}, BFS:{1}", timeRandomized, timeBFS);

            double error25 = 0;
            double maxerror25 = 0;
            for (int i = 0; i < randomizedNumbers25.Length; i++)
            {
                error25 += Math.Pow((randomizedNumbers25[i] - actualResults[i])/actualResults[i], 2);
                maxerror25 = Math.Max(maxerror25, randomizedNumbers25[i] - actualResults[i]);
            }
            error25 = error25 / randomizedNumbers25.Length;
            Console.WriteLine("25 iterations error: {0}", error25);
        }
    }
}
