using System.Collections.Generic;
using System.Linq;
using IFCConverter.Geometry.Boundary;

namespace IFCConverter.Geometry.Algorithms
{
    public static class BoundaryLoopBuilder
    {
        public static List<BoundaryLoop> Build(IEnumerable<Edge> boundaryEdges)
        {
            List<Edge> edges = boundaryEdges.ToList();
            if (edges.Count == 0)
                return new List<BoundaryLoop>();

            Dictionary<int, List<int>> adjacency = BuildAdjacency(edges);
            HashSet<Edge> unusedEdges = new HashSet<Edge>(edges);
            List<BoundaryLoop> loops = new List<BoundaryLoop>();

            while (unusedEdges.Count > 0)
            {
                Edge startEdge = unusedEdges.First();
                List<int> vertices = BuildLoop(startEdge, adjacency, unusedEdges);
                loops.Add(new BoundaryLoop(vertices));
            }

            return loops;
        }

        private static Dictionary<int, List<int>> BuildAdjacency(IEnumerable<Edge> edges)
        {
            Dictionary<int, List<int>> adjacency = new Dictionary<int, List<int>>();
            foreach (Edge edge in edges)
            {
                AddNeighbour(adjacency, edge.A, edge.B);
                AddNeighbour(adjacency, edge.B, edge.A);
            }

            return adjacency;
        }

        private static void AddNeighbour(Dictionary<int, List<int>> adjacency, int vertex, int neighbour)
        {
            if (!adjacency.TryGetValue(vertex, out List<int> neighbours))
            {
                neighbours = new List<int>();
                adjacency.Add(vertex, neighbours);
            }
            
            neighbours.Add(neighbour);
        }

        private static List<int> BuildLoop(Edge startEdge, Dictionary<int, List<int>> adjacency, HashSet<Edge> unusedEdges)
        {
            List<int> loop = new List<int>();

            int startVertex = startEdge.A;
            int previousVertex = -1;
            int currentVertex = startVertex;

            while (true)
            {
                loop.Add(currentVertex);

                List<int> neighbours = adjacency[currentVertex];
                int nextVertex = FindNextVertex(currentVertex, previousVertex, neighbours, unusedEdges);
                
                if (nextVertex == -1)
                    break;

                unusedEdges.Remove(new Edge(currentVertex, nextVertex));

                previousVertex = currentVertex;
                currentVertex = nextVertex;
                
                if (currentVertex == startVertex)
                    break;
            }

            return loop;
        }

        private static int FindNextVertex(int currentVertex, int previousVertex, List<int> neighbours, HashSet<Edge> unusedEdges)
        {
            foreach (int neighbour in neighbours)
            {
                if (neighbour == previousVertex)
                    continue;

                Edge edge = new Edge(currentVertex, neighbour);
                if (unusedEdges.Contains(edge))
                    return neighbour;
            }

            return -1;
        }
    }
}