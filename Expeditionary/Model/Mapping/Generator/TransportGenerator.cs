using Cardamom.Graphing;
using DelaunayTriangulator;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public static class TransportGenerator
    {
        private class Node : IGraphNode
        {
            public Vector3i Hex { get; }

            private readonly List<IGraphEdge> _edges = new();

            public Node(Vector3i hex)
            {
                Hex = hex;
            }

            public IEnumerable<IGraphEdge> GetEdges()
            {
                return _edges;
            }

            public void AddNeighbors(IEnumerable<Node> neighbors)
            {
                _edges.AddRange(
                    neighbors.Select(x => new DefaultGraphEdge(this, x, Geometry.GetCubicDistance(Hex, x.Hex))));
            }
        }

        private static readonly Movement s_Movement =
            new(
                roughness: new(new(1, 0), new(1, 5), new(1, 5)), 
                softness: new(new(1, 0), new(1, 10), new(1, 5)),
                waterDepth: new(new(1, 0), new(1, 50), new(1, 5)));

        public static void Generate(List<Vector3i> nodes, Map map, Random random)
        {
            var voronoiVerts =
                nodes.Select(x => Cubic.Cartesian.Instance.Project(x)).Select(x => new Vertex(x.X, x.Y)).ToList();
            var voronoiTris = VoronoiGrapher.GetTriangulation(voronoiVerts);
            var graph = VoronoiGrapher.GetNeighbors(voronoiVerts, voronoiTris);

            var wrappers = nodes.Select(x => new Node(x)).ToList();
            for (int i=0; i<wrappers.Count; ++i)
            {
                wrappers[i].AddNeighbors(graph.Neighbors[i].Where(x => x >= 0).Select(x => wrappers[x]));
            }
            var mst = MinimalSpanningTree.Compute(wrappers);
            foreach (var edge in mst)
            {
                var path = Pathing.GetShortestPath(((Node)edge.Start).Hex, ((Node)edge.End).Hex, s_Movement, map);
                while (path.Steps.TryPop(out var step))
                {
                    if (path.Steps.TryPeek(out var next))
                    {
                        var e = map.GetEdge(Geometry.GetEdge(step, next));
                        if (e != null)
                        {
                            e.Levels[EdgeType.Road] = 1;
                        }
                    }
                }
            }

            // TODO -- Add random extra roads based on density
        }
    }
}
