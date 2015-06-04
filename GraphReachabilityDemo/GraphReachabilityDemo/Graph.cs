using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphReachabilityDemo
{

    public enum ReachableNodeMethod
    {
        BreadthFirstSearch,
        RandomizedApproach
    }

    public class Graph
    {
        private static Random r = new Random();
        private int NumNodes;
        //graph[source,dest]
        private bool[][] graph;
        private Dictionary<int, int> nodeIndexToIdMapping;
        public Graph()
        {
            nodeIndexToIdMapping = new Dictionary<int, int>();
        }

        public void Load(string filename)
        {
            int nodesRead = 0;
            using (var reader = new StreamReader(new FileStream(filename, FileMode.Open)))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    if (line.StartsWith("#"))
                    {
                        if (line.Contains("Nodes:") && line.Contains("Edges:"))
                        {
                            var split = line.Split(new char[] { ' ' });
                            NumNodes = int.Parse(split[2]);
                            graph = new bool[NumNodes][];
                            for (int i = 0; i < NumNodes; i++)
                            {
                                graph[i] = new bool[NumNodes];
                            }
                        }
                    }
                    else
                    {
                        var edgeSplit = line.Split(new char[] { '\t' });
                        var source = int.Parse(edgeSplit[0]);
                        if (!nodeIndexToIdMapping.ContainsKey(source))
                        {
                            nodeIndexToIdMapping.Add(source, nodesRead++);
                        }
                        var dest = int.Parse(edgeSplit[1]);
                        if (!nodeIndexToIdMapping.ContainsKey(dest))
                        {
                            nodeIndexToIdMapping.Add(dest, nodesRead++);
                        }

                        var sourceIndex = nodeIndexToIdMapping[source];
                        var destIndex = nodeIndexToIdMapping[dest];
                        graph[sourceIndex][destIndex] = true;
                    }
                }
            }
        }

        public double[] GenerateNumberOfReachableNodes(ReachableNodeMethod reachableNodeMetod, int iterations = 0)
        {
            switch (reachableNodeMetod)
            {
                case ReachableNodeMethod.BreadthFirstSearch:
                    return GenerateBruteForceReachableNumbers();
                    break;
                case ReachableNodeMethod.RandomizedApproach:
                    return GenerateRandomizedReachableNumbers(iterations);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reachableNodeMetod", reachableNodeMetod, null);
            }
        }

        private double[] GenerateRandomizedReachableNumbers(int iterations)
        {
            var iterationResults = new int[NumNodes, iterations];
            var nodeMetadatas = new NodeMetadata[NumNodes];
            var orderedWeights = new List<NodeMetadata>(NumNodes);
            Stack<int> indexesToVisit = new Stack<int>(NumNodes);
            for (var l = 0; l < iterations; l++)
            {
                for (int i = 0; i < nodeMetadatas.Length; i++)
                {
                    var randomDepth = r.NextDouble();
                    nodeMetadatas[i] = new NodeMetadata()
                    {
                        AssignedRandomDepthNumber = randomDepth,
                        MinRandomDepth = randomDepth,
                        NodeIndex = i,
                    };
                    orderedWeights.Add(nodeMetadatas[i]);
                }
                orderedWeights.Sort(new NodeMetadataComparer());

                for (int k = 0; k < orderedWeights.Count; k++)
                {
                    var sourceNode = orderedWeights[k];
                    if (sourceNode.AssignedRandomDepthNumber > sourceNode.MinRandomDepth)
                    {
                        //we can't do anything here, just continue
                        continue;
                    }

                    //traverse the graph backwards
                    indexesToVisit.Clear();
                    indexesToVisit.Push(sourceNode.NodeIndex);
                    int currentNode;
                    var toBeVisited = new bool[NumNodes];
                    toBeVisited[sourceNode.NodeIndex] = true;
                    do
                    {
                        currentNode = indexesToVisit.Pop();
                        for (int j = 0; j < NumNodes; j++)
                        {
                            //reverse indexes
                            if (graph[j][currentNode] && !toBeVisited[j] && nodeMetadatas[j].MinRandomDepth > sourceNode.MinRandomDepth)
                            {
                                toBeVisited[j] = true;
                                nodeMetadatas[j].MinRandomDepth = sourceNode.MinRandomDepth;
                                indexesToVisit.Push(j);
                            }
                        }
                    }
                    while (indexesToVisit.Any());
                }

                for (int i = 0; i < NumNodes; i++)
                {
                    iterationResults[i, l] = (int)Math.Ceiling(1.0d / nodeMetadatas[i].MinRandomDepth) - 1;
                }

                orderedWeights.Clear();
            }

            var results = new double[NumNodes];

            for (int p = 0; p < NumNodes; p++)
            {
                var sortedList = new List<int>();
                for (int q = 0; q < iterations; q++)
                {
                    sortedList.Add(iterationResults[p, q]);
                }
                sortedList.Sort();

                //Because of the way inverse numbers work, we will exclude the top 20% of the largest numbers, and then choose the median
                //often other techniques are used (median for small values, drop top X% and bottom X%, etc)
                sortedList = sortedList.Take((int)(sortedList.Count*0.8)).ToList();
                results[p] = sortedList[sortedList.Count/2];
            }

            return results;

        }

        private double[] GenerateBruteForceReachableNumbers()
        {
            var reachableNodes = new double[NumNodes];

            Stack<int> indexesToVisit = new Stack<int>(NumNodes);
            for (int i = 0; i < NumNodes; i++)
            {
                indexesToVisit = new Stack<int>(NumNodes); //.Clear();
                indexesToVisit.Push(i);
                int currentlyReachableNodes = 0;
                int currentNode;
                var toBeVisited = new bool[NumNodes];
                toBeVisited[i] = true;
                do
                {
                    currentNode = indexesToVisit.Pop();
                    currentlyReachableNodes++;
                    for (int j = 0; j < NumNodes; j++)
                    {
                        if (graph[currentNode][j] && !toBeVisited[j])
                        {
                            toBeVisited[j] = true;
                            indexesToVisit.Push(j);
                        }
                    }
                }
                while (indexesToVisit.Count != 0);
                reachableNodes[i] = currentlyReachableNodes;
            }
            return reachableNodes;
        }
    }
}
