using Cardamom.Collections;
using Cardamom.Graphing;
using DelaunayTriangulator;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Hexagons;
using MathNet.Numerics.Statistics;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public static class TransportGenerator
    {
        public record struct StructurePredicate(int Level, bool IsCore);

        public class Parameters
        {
            public EdgeType Type { get; set; }
            public int Level { get; set; }
            public EnumMap<StructureType, StructurePredicate> SupportedStructures { get; set; } = new();
            public float MaximumCost { get; set; } = 1000f;
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
                    neighbors.Select(
                        neighbor => new DefaultGraphEdge(
                            this, neighbor, Geometry.GetCubicDistance(Hex, neighbor.Hex))));
            }
        }

        private static readonly Movement s_Movement =
            new(
                restriction: new(0, 5, 5),
                roughness: new(0, 25, 5),
                slope: new(0, 25, 5),
                softness: new(0, 25, 5),
                waterDepth: new(0, 50, 5));

        public static void Generate(IEnumerable<Parameters> parameters, ISet<Vector3i> cores, IList<Vector3i> nodes, Map map, Random random)
        {
            foreach (var p in parameters)
            {
                var wrappers = 
                    nodes
                        .Where(hex => IsSupported(map.Get(hex)!, cores.Contains(hex), p))
                        .Select(hex => new Node(hex))
                        .ToList();

                var voronoiVerts =
                    wrappers
                        .Select(node => Cubic.Cartesian.Instance.Project(node.Hex))
                        .Select(position => new Vertex(position.X, position.Y))
                        .ToList();
                if (voronoiVerts.Count < 2)
                {
                    continue;
                }
                var voronoiTris = VoronoiGrapher.GetTriangulation(voronoiVerts);
                var graph = VoronoiGrapher.GetNeighbors(voronoiVerts, voronoiTris);

                for (int i=0; i<wrappers.Count; ++i)
                {
                    wrappers[i].AddNeighbors(
                        graph.Neighbors[i]
                            .Where(wrapperId => wrapperId >= 0).Select(wrapperId => wrappers[wrapperId]));
                }
                var mst = MinimalSpanningTree.Compute(wrappers).ToHashSet();
                foreach (var edge in mst)
                {
                    AddEdge(((Node)edge.Start).Hex, ((Node)edge.End).Hex, p, map);
                }

                (double mean, double stdDev) =
                    wrappers.SelectMany(node => node.GetEdges()).Select(edge => edge.Cost).MeanStandardDeviation();
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
            var path = Pathing.GetShortestPath(map, origin, destination, s_Movement, TileConsiderations.None)!;
            if (path.Cost> parameters.MaximumCost)
            {
                return;
            }
            for (int i=0; i<path.Steps.Count - 1; ++i)
            {
                var e = map.GetEdge(Geometry.GetEdge(path.Steps[i], path.Steps[i + 1]));
                if (e != null)
                {
                    e.Levels[parameters.Type] = parameters.Level;
                }
            }
        }

        private static bool IsSupported(Tile tile, bool isCore, Parameters parameters)
        {
            var predicate = parameters.SupportedStructures[tile.Structure.Type];
            return predicate.Level > 0 && predicate.Level <= tile.Structure.Level && (!predicate.IsCore || isCore);
        }

        private static double GetDensity(double density, double droppoff, double deviations)
        {
            return 2 * density / (1 + Math.Exp(droppoff * deviations));
        }
    }
}
