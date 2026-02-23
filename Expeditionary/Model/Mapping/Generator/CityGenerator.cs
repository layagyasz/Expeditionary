using Cardamom.Collections;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public static class CityGenerator
    {
        public record class Parameters
        {
            public float LiquidAffinity { get; set; }
            public List<LayerParameters> Layers { get; set; } = new();
        }

        public record class LayerParameters
        {
            public int CoreCount { get; set; }
            public float CoreDensity { get; set; }
            public float CandidateDensity { get; set; }
            public ISampler Size { get; set; } = new NormalSampler(20, 10);
            public StructureType Type { get; set; }
            public int Level { get; set; } = 1;
            public List<MapTag> Tags { get; set; } = new();
            public Vector3i Center { get; set; }
            public Quadratic DistancePenalty { get; set; }
            public Quadratic SprawlPenalty { get; set; } = new(0f, 0.5f, 0f);
            public Quadratic SlopePenalty { get; set; } = new(0f, 1f, 0f);
            public Quadratic ElevationPenalty { get; set; } = new(0f, 0.2f, 0f);
            public Quadratic CoastPenalty { get; set; } = new(0f, -2f, 2);
            public Quadratic RiverPenalty { get; set; } = new(0f, -1f, 1);
            public Quadratic SandPenalty { get; set; } = new(0f, 1f, 0f);
            public Quadratic ClayPenalty { get; set; }
            public Quadratic SiltPenalty { get; set; }
            public Quadratic HeatPenalty { get; set; }
            public Quadratic MoisturePenalty { get; set; }
        }

        private class Core
        {
            public LayerParameters Parameters { get; }
            public int Max { get; }
            public int Count { get; set; }

            internal Core(LayerParameters parameters, int max) 
            { 
                Parameters = parameters;
                Max = max;
            }
        }

        private class Node
        {
            public Vector3i Hex { get; }
            public bool Open { get; set; }
            public bool Closed { get; set; }
            public float Distance { get; set; }
            public float Cost { get; set; }
            public Core? Core { get; set; }

            internal Node(Vector3i hex)
            {
                Hex = hex;
                Distance = float.MaxValue;
                Cost = float.MaxValue;
            }
        }

        public static HashSet<Vector3i> Generate(Parameters parameters, Map map, Random random)
        {
            var closed = new HashSet<Vector3i>();
            var open = new Heap<Node, float>();
            var nodes = new Dictionary<Vector3i, Node>();
            var cores = new HashSet<Vector3i>();

            int availableTiles = map.Range().Select(map.Get).Count(x => !x!.Terrain.IsLiquid);
            foreach (var param in parameters.Layers)
            {
                var costDict = 
                    map.Range()
                        .Where(x => !closed.Contains(x))
                        .ToDictionary(
                            x => x,
                            x => GetCost(x, map, param, parameters.LiquidAffinity) + 0.2f * random.NextSingle());
                int numCandidates = (int)(param.CandidateDensity * availableTiles);
                var candidates = costDict.OrderBy(x => x.Value).Take(numCandidates).ToList();
                candidates.Shuffle(random);

                int numCores = param.CoreCount == 0 ? (int)(param.CoreDensity * availableTiles) : param.CoreCount;
                foreach (var candidate in candidates.Take(numCores))
                {
                    closed.Add(candidate.Key);
                    var core = new Core(param, (int)Math.Max(1, param.Size.Generate(random)));
                    var node =
                        new Node(candidate.Key)
                        {
                            Core = core,
                            Open = true,
                            Distance = 0,
                            Cost = candidate.Value
                        };
                    nodes.Add(candidate.Key, node);
                    open.Push(node, 0);
                    cores.Add(candidate.Key);
                }
            }

            while (open.Count > 0)
            {
                var current = open.Pop();
                current.Open = false;
                current.Closed = true;
                if (current.Core == null)
                {
                    continue;
                }
                if (current.Core.Count >= current.Core.Max)
                {
                    current.Core = null;
                    continue;
                }
                current.Core.Count++;
                var p = current.Core.Parameters;
                foreach (var neighbor in Geometry.GetNeighbors(current.Hex))
                {

                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = new(neighbor);
                        nodes.Add(neighbor, neighborNode);
                    }

                    if (neighborNode.Closed)
                    {
                        continue;
                    }

                    var distance = current.Distance + 1;
                    var cost = 
                        p.SprawlPenalty.Evaluate(distance) + GetCost(neighbor, map, p, parameters.LiquidAffinity);
                    if (cost == float.MaxValue)
                    {
                        continue;
                    }

                    if (cost < neighborNode.Cost)
                    {
                        if (neighborNode.Open)
                        {
                            open.Remove(neighborNode);
                        }
                        else
                        {
                            neighborNode.Open = true;
                        }
                        neighborNode.Core = current.Core;
                        neighborNode.Distance = distance;
                        neighborNode.Cost = cost;
                        open.Push(neighborNode, cost);
                    }
                }
            }

            foreach (var node in  nodes.Values)
            {
                if (node.Closed && node.Core != null)
                {
                    var tile = map.Get(node.Hex)!;
                    tile.Structure =
                        new()
                        {
                            Type = node.Core.Parameters.Type,
                            Level = node.Core.Parameters.Level
                        };
                    tile.Terrain.Foliage = null;
                    tile.Tags.AddRange(node.Core.Parameters.Tags);
                }
            }

            return cores;
        }

        private static float GetCost(Vector3i hex, Map map, LayerParameters parameters, float liquidAffinity)
        {
            var tile = map.Get(hex);
            if (tile == null || tile.Terrain.IsLiquid)
            {
                return float.MaxValue;
            }
            bool neighborsWater =
                Geometry.GetNeighbors(hex).Select(map.Get).Where(x => x != null).Any(x => x!.Terrain.IsLiquid);
            bool neighborsRiver =
                Geometry.GetEdges(hex)
                    .Select(map.GetEdge).Where(x => x != null).Any(x => x!.Levels.ContainsKey(EdgeType.River));
            return parameters.DistancePenalty.Evaluate(Geometry.GetCubicDistance(hex, parameters.Center))
                + parameters.SlopePenalty.Evaluate(tile.Slope)
                + parameters.ElevationPenalty.Evaluate(tile.Elevation)
                + liquidAffinity * parameters.CoastPenalty.Evaluate(neighborsWater ? 1 : 0)
                + liquidAffinity * parameters.RiverPenalty.Evaluate(neighborsRiver ? 1 : 0)
                + parameters.SandPenalty.Evaluate(tile.Terrain.Soil.HasValue ? tile.Terrain.Soil.Value.X : 0)
                + parameters.ClayPenalty.Evaluate(tile.Terrain.Soil.HasValue ? tile.Terrain.Soil.Value.Y : 0)
                + parameters.SiltPenalty.Evaluate(tile.Terrain.Soil.HasValue ? tile.Terrain.Soil.Value.Z : 0)
                + parameters.HeatPenalty.Evaluate(tile.Heat)
                + parameters.MoisturePenalty.Evaluate(tile.Moisture);
        }
    }
}
