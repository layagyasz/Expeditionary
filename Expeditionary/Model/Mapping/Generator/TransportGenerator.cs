using Cardamom.Collections;
using Cardamom.Graphing;
using DelaunayTriangulator;
using Expeditionary.Hexagons;
using MathNet.Numerics.Statistics;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public static class TransportGenerator
    {
        public class Parameters
        {
            public EdgeType Type { get; set; }
            public int Level { get; set; }
            public EnumMap<StructureType, int> SupportedStructures { get; set; } = new();
            public float MaximumCost { get; set; } = 250f;
            public float Density { get; set; } = 1f;
            public float Dropoff { get; set; } = 2f;
        }

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

        public static void Generate(IEnumerable<Parameters> parameters, List<Vector3i> nodes, Map map, Random random)
        {
            foreach (var p in parameters)
            {
                var wrappers = nodes.Where(x => IsSupported(map.GetTile(x)!, p)).Select(x => new Node(x)).ToList();

                var voronoiVerts =
                    wrappers
                        .Select(x => Cubic.Cartesian.Instance.Project(x.Hex))
                        .Select(x => new Vertex(x.X, x.Y))
                        .ToList();
                var voronoiTris = VoronoiGrapher.GetTriangulation(voronoiVerts);
                var graph = VoronoiGrapher.GetNeighbors(voronoiVerts, voronoiTris);

                for (int i=0; i<wrappers.Count; ++i)
                {
                    wrappers[i].AddNeighbors(graph.Neighbors[i].Where(x => x >= 0).Select(x => wrappers[x]));
                }
                var mst = MinimalSpanningTree.Compute(wrappers).ToHashSet();
                foreach (var edge in mst)
                {
                    AddEdge(((Node)edge.Start).Hex, ((Node)edge.End).Hex, p, map);
                }

                (double mean, double stdDev) =
                    wrappers.SelectMany(x => x.GetEdges()).Select(x => x.Cost).MeanStandardDeviation();
                var closed = new HashSet<Node>();
                foreach (var wrapper in wrappers)
                {
                    foreach (var edge in wrapper.GetEdges())
                    {
                        if (mst.Contains(edge) || closed.Contains(edge.End))
                        {
                            continue;
                        }
                        closed.Add(wrapper);
                        if (random.NextDouble() < GetDensity(p.Density, p.Dropoff, (edge.Cost - mean) / stdDev))
                        {
                            AddEdge(((Node)edge.Start).Hex, ((Node)edge.End).Hex, p, map);
                        }
                    }
                }
            }
        }

        private static void AddEdge(Vector3i origin, Vector3i destination, Parameters parameters, Map map)
        {
            var path = Pathing.GetShortestPath(origin, destination, s_Movement, map);
            if (path.Cost> parameters.MaximumCost)
            {
                return;
            }
            while (path.Steps.TryPop(out var step))
            {
                if (path.Steps.TryPeek(out var next))
                {
                    var e = map.GetEdge(Geometry.GetEdge(step, next));
                    if (e != null)
                    {
                        e.Levels[parameters.Type] = parameters.Level;
                    }
                }
            }
        }

        private static bool IsSupported(Tile tile, Parameters parameters)
        {
            int supportedLevel = parameters.SupportedStructures[tile.Structure.Type];
            return supportedLevel > 0 && supportedLevel <= tile.Structure.Level;
        }

        private static double GetDensity(double density, double droppoff, double deviations)
        {
            return 2 * density / (1 + Math.Exp(droppoff * deviations));
        }
    }
}
