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
            public float CrossoverMultiplier = 5f;
        }

        private class Node : IGraphNode
        {
            public StructureType Type { get; }
            public Vector3i Hex { get; }

            private readonly List<IGraphEdge> _edges = new();

            public Node(StructureType type, Vector3i hex)
            {
                Type = type;
                Hex = hex;
            }

            public IEnumerable<IGraphEdge> GetEdges()
            {
                return _edges;
            }

            public void AddEdge(IGraphEdge edge)
            {
                _edges.Add(edge);
            }
        }

        private static readonly Movement s_Movement =
            new(
                restriction: new(0, 5, 5),
                roughness: new(0, 25, 5),
                slope: new(0, 25, 5),
                softness: new(0, 25, 5),
                waterDepth: new(0, 50, 5));

        public static void Generate(
            IEnumerable<Parameters> parameters, ISet<Vector3i> cores, IList<Vector3i> nodes, Map map, Random random)
        {
            foreach (var p in parameters)
            {
                var wrappers =
                    nodes
                        .Select(hex => (tile: map.Get(hex)!, hex))
                        .Where(tile => IsSupported(tile.tile, cores.Contains(tile.hex), p))
                        .Select(tile => new Node(tile.tile.Structure.Type, tile.hex))
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
                    var node = wrappers[i];
                    foreach (var neighborId in graph.Neighbors[i])
                    {
                        if (neighborId < 0)
                        {
                            continue;
                        }
                        var neighbor = wrappers[neighborId];
                        node.AddEdge(
                            new DefaultGraphEdge(
                                node,
                                neighbor,
                                (node.Type == neighbor.Type ? 1 : p.CrossoverMultiplier) 
                                    * Geometry.GetCubicDistance(node.Hex, neighbor.Hex)));
                    }
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
            if (path.Cost > parameters.MaximumCost)
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
