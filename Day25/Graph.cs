using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace Day25
{
    public record Graph
    {
        private int _idCounter = 0;
        private Dictionary<string, int> stringToId = new();
        private Dictionary<int, string> idToString = new();
        private int[,] adjacencyMatrix;
        private int[,] _adjacencyMatrixResetTarget;

        private HashSet<int> contractedNodes = new HashSet<int>();

        public int NrOfNodes => adjacencyMatrix.GetLength(0);
        public int GetLargestValueInMatrix() // used after contracting the graph to determine the minimum cut
        {
            int largestValue = -1;
            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
                {
                    if (adjacencyMatrix[i, j] > largestValue) largestValue = adjacencyMatrix[i, j];
                }
            }
            return largestValue;
        }

        public int[] Nodes => stringToId.Values.Except(contractedNodes).ToArray();

        public Graph(string[] lines)
        {
            HashSet<string> allNodeNames = new HashSet<string>();
            Dictionary<string, string[]> nodeToConnection = new Dictionary<string, string[]>();

            // Create all nodes
            for (int i = 0; i < lines.Length; i++)
            {
                List<string> nodesInRow = new List<string>();
                string[] split1 = lines[i].Split(": ");
                string[] destinations = split1[1].Split(' ');

                // Add all nodes named in the data row to a list
                // for example "node1: connection1 connection2 connection3" is 4 nodes
                nodesInRow.Add(split1[0]);
                nodesInRow.AddRange(destinations);
                nodeToConnection.Add(split1[0], destinations);

                // If a node in the row is not added to the hashset, add it
                foreach (string node in nodesInRow)
                    if (!allNodeNames.Contains(node))
                    {
                        allNodeNames.Add(node);
                        stringToId.Add(node, _idCounter);
                        idToString.Add(_idCounter, node);
                        _idCounter++;
                    }
            }

            // Create all nodes
            List<Node> allNodes = new List<Node>();
            allNodes = allNodeNames.Select(x => new Node(stringToId[x], x)).ToList();

            // Create the adjacency matrix
            adjacencyMatrix = new int[allNodes.Count, allNodes.Count];

            // Add all existing connections to the adjacency matrix
            foreach (var node in nodeToConnection)
            {
                int id1 = stringToId[node.Key];
                foreach (var connection in node.Value)
                {
                    int id2 = stringToId[connection];
                    adjacencyMatrix[id1, id2] += 1;
                    adjacencyMatrix[id2, id1] += 1; // symmetric, because we want connection from and to
                }
            }

            // We copy the values from the adjacency matrix to the "reset target", which is the state we return to when we Reset() the graph.
            _adjacencyMatrixResetTarget = adjacencyMatrix;
            _adjacencyMatrixResetTarget = new int[_adjacencyMatrixResetTarget.GetLength(0), _adjacencyMatrixResetTarget.GetLength(1)];
            for (int i = 0; i < _adjacencyMatrixResetTarget.GetLength(0); i++)
            {
                for (int j = 0; j < _adjacencyMatrixResetTarget.GetLength(1); j++)
                {
                    _adjacencyMatrixResetTarget[i, j] = adjacencyMatrix[i, j];
                }
            }
        }

        // Restores the adjacency matrix and other tracked features (such as contractions and history) to the default
        public void Reset()
        {
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                {
                    adjacencyMatrix[i, j] = _adjacencyMatrixResetTarget[i, j];
                }
            }
            contractedNodes.Clear();
            contractedHistory.Clear();
        }

        // Randomly selects two nodes in adjacency and merges them into one
        public bool ContractRandom()
        {
            int nodeNr = NrOfNodes;
            if (contractedNodes.Count + 2 == nodeNr) return true; // return isComplete

            Random random = new();

            // Selects a random node. The randomness is biased toward nodes with many connections.
            int node1 = GetWeightedRandomNode();

            // Slects a random connection to a given node. The randomness is biased toward the most connections
            int node2 = GetWeightedRandomEdge(node1);

            Contract(node1, node2);

            return false; // return isComplete


            // Credit to Chris Waters on using weighted randomness.
            // Without weighted, it just takes way too long (let it run for 3 hours, compared to 13 minutes with weights)
            // https://github.com/chriswaters78/AdventOfCode2023/blob/main/2023_21/Program.cs#L53
            int GetWeightedRandomNode()
            {
                int node = -1;

                int[,] nodeWeights = new int[nodeNr, 2];
                int allTotalEdges = 0;
                for (int i = 0; i < nodeNr; i++) // column index
                {
                    int columnEdges = 0;
                    for (int j = 0; j < nodeNr; j++) // row index  //i+1
                    {
                        columnEdges += adjacencyMatrix[i, j];
                    }

                    allTotalEdges += columnEdges; // count all edges existing
                    nodeWeights[i, 0] = i;
                    nodeWeights[i, 1] = allTotalEdges;
                }

                int re1 = random.Next(allTotalEdges);
                for (int i = 0; i < nodeNr; i++)
                {
                    if (re1 < nodeWeights[i, 1])
                    {
                        node = nodeWeights[i, 0];
                        break;
                    }
                }

                return node;
            }
            int GetWeightedRandomEdge(int node)
            {
                int nodeResult = -1;

                int[,] nodeEdgeWeights = new int[nodeNr, 2];
                int nodeTotalEdges = 0;
                for (int i = 0; i < nodeNr; i++)
                {
                    nodeTotalEdges += adjacencyMatrix[i, node];

                    nodeEdgeWeights[i, 0] = i;
                    nodeEdgeWeights[i, 1] = nodeTotalEdges;
                }

                int re2 = random.Next(nodeTotalEdges);
                for (int i = 0; i < nodeNr; i++)
                {
                    if (re2 < nodeEdgeWeights[i, 1])
                    {
                        nodeResult = nodeEdgeWeights[i, 0];
                        break;
                    }
                }

                return nodeResult;
            }
        }

        Dictionary<int, List<int>> contractedHistory = new();
        public Dictionary<int, List<int>> ContractedHistory => contractedHistory;

        // Contract two nodes
        public void Contract(int node1, int node2)
        {
            if (contractedNodes.Contains(node1) || contractedNodes.Contains(node2)) throw new IndexOutOfRangeException();

            // contractedNodes contains all nodes that have been "eliminated" due to contraction.
            // When contracting, node2 is merged into node1, meaning that node2 effectively disappears.
            // the hashset contractedNodes contains all "eliminated" nodes.

            // contractedHistory describes what nodes are currently grouped together.
            // When you only have two nodes left after contracting multiple times, you
            // can see what groupings remain with contractedHistory[0] and contractedHistory[1]

            // If node1 isn't already a group, we have to create one
            if (!contractedHistory.ContainsKey(node1))
                contractedHistory.Add(node1, [node1]); // node1 should be part of its own group

            // If node2 also is a grouped node, we have to add its grouped values to node1 as well.
            if (contractedHistory.ContainsKey(node2))
            {
                contractedHistory[node1].AddRange(contractedHistory[node2]);
                contractedHistory.Remove(node2); // and remove node2 group afterward
            }
            else contractedHistory[node1].Add(node2);

            // We merge node2 into node1
            adjacencyMatrix[node1, node2] = 0;
            adjacencyMatrix[node2, node1] = 0;

            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++) //column or row
            {
                adjacencyMatrix[node1, i] += adjacencyMatrix[node2, i]; // merge Node2 into Node1
                adjacencyMatrix[node2, i] = 0;
                adjacencyMatrix[i, node1] += adjacencyMatrix[i, node2]; // merge Node2 into Node1
                adjacencyMatrix[i, node2] = 0;
            }

            contractedNodes.Add(node2); // only node 2 is disappearing
        }


        // Prints the adjacency map to the console window.
        // Don't run this if the graph is big (over 20 vertices) as
        // it will exceed the buffer size of the console window.
        public void WriteAdjacencyMap(int x = -1, int y = -1)
        {
            if (x == -1) x = Console.CursorLeft;
            if (y == -1) y = Console.CursorTop;
            Console.SetCursorPosition(x, y);

            string firstLine = "    |" + string.Join('\0', Enumerable.Range(0, adjacencyMatrix.GetLength(0)).Select(x => x.ToString().PadRight(4) + "|").ToArray());
            Console.WriteLine(firstLine);
            string horizontalLine = string.Join('\0', Enumerable.Repeat("----+", firstLine.Length / 5 - 2).ToArray());
            Console.WriteLine(horizontalLine);


            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(j.ToString().PadRight(4));
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("|");
                for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
                {
                    int value = adjacencyMatrix[i, j];
                    if (i == j) Console.ForegroundColor = ConsoleColor.Black;
                    else if (value > 10) Console.ForegroundColor = ConsoleColor.Red;
                    else if (value > 7) Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (value > 4) Console.ForegroundColor = ConsoleColor.Yellow;
                    else if (value > 0) Console.ForegroundColor = ConsoleColor.Green;
                    else Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(adjacencyMatrix[i, j].ToString().PadRight(4));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("|");
                }
                Console.WriteLine("\n" + horizontalLine);
            }
        }

        public string IDtoString(int id) => idToString.TryGetValue(id, out string v) ? v : "";
        public int StringtoID(string nodeName) => stringToId.TryGetValue(nodeName, out int v) ? v : -1;
    }

    public record Node(int ID, string Name);
}


//namespace Day25OBSOLETE
//{
//    record Graph
//    {
//        public HashSet<Node> nodesHashset;
//        public List<Node> nodesList;
//        public Dictionary<string, Node> stringToNode;

//        public Graph(HashSet<Node> nodes)
//        {
//            this.nodesHashset = nodes;
//            nodesList = [.. nodes];
            
//            stringToNode = new();
//            foreach (var node in nodes) stringToNode.Add(node.Name, node);
//        }

//        public Graph(Graph fromOtherGraph)
//        {
//            nodesHashset = new();
//            foreach (var item in fromOtherGraph.nodesList)
//            {
//                nodesHashset.Add(new Node(item));
//            }

//            nodesList = nodesHashset.ToList();
//            stringToNode = new();

//            foreach (var item in nodesList) stringToNode.Add(item.Name, item);

//            this.nodesHashset = [..fromOtherGraph.nodesHashset];
//            nodesList = [..fromOtherGraph.nodesList];

//            stringToNode = new();
//            foreach (var node in nodesList) stringToNode.Add(node.Name, node);
//        }

//        public Node AddOrChangeNode(string name, string[] destinations)
//        {
//            Node nodeReference;
//            if (stringToNode.ContainsKey(name))
//            {
//                nodeReference = stringToNode[name];
//                nodeReference.Edges.Clear();
//            }
//            else
//            {
//                nodeReference = new(name, []);
//                nodesHashset.Add(nodeReference);
//                nodesList.Add(nodeReference);
//                stringToNode.Add(name, nodeReference);
//            }

//            // Repopulate destination collection
//            foreach (var dest in destinations)
//            {
//                Node destinationReference;
//                if (stringToNode.ContainsKey(dest)) destinationReference = stringToNode[dest];
//                else
//                {
//                    destinationReference = new(dest, []);
//                    nodesHashset.Add(destinationReference);
//                    nodesList.Add(destinationReference);
//                    stringToNode.Add(dest, destinationReference);
//                }
//                nodeReference.Edges.Add(destinationReference.Name);
//                destinationReference.Edges.Add(nodeReference.Name);
//            }

//            return nodeReference;
//        }
        
//        /*public Node AddOrChangeNode(string name, List<string> destinations)
//        {
//            Node nodeReference;
//            if (stringToNode.ContainsKey(name))
//            {
//                nodeReference = stringToNode[name];
//                nodeReference.Edges.Clear();
//            }
//            else
//            {
//                nodeReference = new(name, []);
//                nodesHashset.Add(nodeReference);
//                nodesList.Add(nodeReference);
//                stringToNode.Add(name, nodeReference);
//            }

//            //nodeReference.Edges.Clear();
//            nodeReference.Edges.AddRange(destinations);
//            foreach (var dest in destinations.Select(AsNode)) dest.Edges.Add(nodeReference.Name);

//            return nodeReference;
//        }*/

//        void RemoveNode(Node n)
//        {
//            nodesHashset.Remove(n);
//            nodesList.Remove(n);
//            stringToNode.Remove(n.Name);
//        }

//        Node AsNode(string nodeName) => stringToNode[nodeName];

//        // returns TRUE if complete
//        public bool Contract(out int nrOfNodesAfterContract)
//        {
//            Random random = new();

//            int randomIndex1 = random.Next(0, nodesHashset.Count - 1);
//            Node firstNode = nodesList[randomIndex1];

//            int randomIndex2 = -1;
//            randomIndex2 = random.Next(0, firstNode.Edges.Count - 1);
//            Node secondNode = AsNode(firstNode.Edges[randomIndex2]);
//            if (firstNode.Edges.Count - 1 < 0)
//                throw new UnreachableException();

//            Con(firstNode, secondNode);

//            nrOfNodesAfterContract = nodesHashset.Count;
//            if (nrOfNodesAfterContract <= 2) return true;
//            else return false;

//            void Con(Node node1, Node node2)
//            {
//                //Console.WriteLine($"Contracted {node1} and {node2}");
//                //Console.WriteLine("   N1: " + string.Join(", ", node1.Edges.OrderBy(x => x.Name)));
//                //Console.WriteLine("   N2: " + string.Join(", ", node2.Edges.OrderBy(x => x.Name)));

//                List<string> combinedEdges = [.. node1.Edges];
//                combinedEdges.AddRange(node2.Edges);
//                //combinedEdges = [..combinedEdges.Except(new Node[] { node1, node2 })];
//                combinedEdges.RemoveAll(n => n == node1.Name || n == node2.Name);

//                string newName = node2.Name + node1.Name;

//                // Create a new node with the same connections as the two other nodes
//                //Node newNode = new(newName, combinedEdges);
//                Node newNode = AddOrChangeNode(newName, [..combinedEdges]); // Creates a new node connected to the connections of node1 and node2, without node1 and node2

//                // Modify all connections to node1 and node2 to instead be connected to the new node
//                for (int i = 0; i < node1.Edges.Count; i++)
//                {
//                    string node1_edgeI = node1.Edges[i];
//                    for (int j = 0; j < node1_edgeI.Edges.Count; j++) // look at the edges of each connected node to newNode. If contains old reference, update to newNode
//                    {
//                        var node1_edgeI_edgeJ = AsNode(node1_edgeI.Edges[j]);
//                        if (node1_edgeI_edgeJ == node1 ||
//                            node1_edgeI_edgeJ == node2)
//                        {
//                            //Console.WriteLine($"  Adjusted edge {n1_edge.Edges[j]} of node {n1_edge} to {newNode}");
//                            node1_edgeI_edgeJ = newNode;
//                        }
//                    }
//                }
//                for (int i = 0; i < node2.Edges.Count; i++)
//                {
//                    var node2_edgeI = AsNode(node2.Edges[i]);
//                    for (int j = 0; j < node2_edgeI.Edges.Count; j++) // look at the edges of each connected node to newNode. If contains old reference, update to newNode
//                    {
//                        var node2_edgeI_edgeJ = AsNode(node2_edgeI.Edges[j]);
//                        if (node2_edgeI_edgeJ == node1 ||
//                            node2_edgeI_edgeJ == node2)
//                        {
//                            //Console.WriteLine($"  Adjusted edge {n2_edge.Edges[j]} of node {n2_edge} to {newNode}");
//                            node2_edgeI_edgeJ = newNode;
//                        }
//                    }

//                }

//                // Remove old nodes
//                RemoveNode(node1);
//                RemoveNode(node2);

//                //Console.WriteLine("   ->  " + string.Join(", ", newNode.Edges.OrderBy(x => x.Name)) + "\n");
//            }
//        }

//        public override string? ToString()
//        {
//            return nodesHashset.Count.ToString();
//        }


//    }


//    public class Node
//    {
//        public Node(string name, List<string> edges)
//        {
//            Name = name;
//            Edges = edges;
//        }
//        public Node(Node fromOtherNode)
//        {
//            Name = fromOtherNode.Name;
//            Edges = [.. fromOtherNode.Edges];
//        }

//        public string Name { get; private set; }
//        public List<string> Edges { get; init; }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(Name);
//        }

//        public override string? ToString()
//        {
//            return Name;
//        }

//        public override bool Equals(object? obj)
//        {
//            return obj is Node node &&
//                   Name == node.Name;
//        }

//        public static bool operator ==(Node n1, Node n2)
//        {
//            return n1.Name == n2.Name;
//        }
//        public static bool operator !=(Node n1, Node n2)
//        {
//            return n1.Name != n2.Name;
//        }
//    }
//}
