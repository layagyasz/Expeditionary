using Cardamom.Collections;
using Cardamom.Utils.Generators.Samplers;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping.Generator
{
    public class CityGenerator
    {
        public class Parameters
        {
            public int Cores { get; set; }
            public int Candidates { get; set; }
            public ISampler Size { get; set; } = new NormalSampler(20, 10);
            public float BasePenalty { get; set; } = 0.1f;
            public float SlopePenalty { get; set; } = 1;
            public float ElevationPenalty { get; set; } = 1;
            public float NoLiquidPenalty { get; set; } = 2;
            public float NoRiverPenalty { get; set; } = 1;
        }

        private class Core
        {
            public int Max { get; }
            public int Count { get; set; }

            internal Core(int max) 
            { 
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

        public static void Generate(Parameters parameters, Map map, Random random)
        {
            var costDict = map.GetTiles().ToDictionary(x => x, x => GetCost(x, map, parameters));
            var candidates = costDict.OrderBy(x => x.Value).Take(parameters.Candidates).ToList();
            candidates.Shuffle(random);


            Dictionary<Vector3i, Node> nodes = new();
            Heap<Node, float> open = new();
            List<Core> cores = new();
            foreach (var candidate in candidates.Take(parameters.Cores))
            {
                var core = new Core((int)parameters.Size.Generate(random));
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
                foreach (var neighbor in Geometry.GetNeighbors(current.Hex))
                {
                    var distance = current.Distance + parameters.BasePenalty;
                    var cost = distance + GetCost(neighbor, map, parameters);
                    if (cost == float.MaxValue)
                    {
                        continue;
                    }
                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = new(neighbor);
                        nodes.Add(neighbor, neighborNode);
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
                    map.GetTile(node.Hex)!.Structure =
                        new()
                        {
                            Type = StructureType.Residential,
                            Level = 1
                        };
                }
            }
        }

        private static float GetCost(Vector3i hex, Map map, Parameters parameters)
        {
            var tile = map.GetTile(hex);
            if (tile == null || tile.Terrain.IsLiquid)
            {
                return float.MaxValue;
            }
            bool neighborsWater =
                Geometry.GetNeighbors(hex).Select(map.GetTile).Where(x => x != null).Any(x => x!.Terrain.IsLiquid);
            bool neighborsRiver =
                Geometry.GetEdges(hex)
                    .Select(map.GetEdge).Where(x => x != null).Any(x => x!.Type == Edge.EdgeType.River);
            return parameters.SlopePenalty * tile.Slope 
                + parameters.ElevationPenalty * tile.Elevation 
                + (neighborsWater ? 0 : parameters.NoLiquidPenalty) 
                + (neighborsRiver ? 0 : parameters.NoLiquidPenalty);
        }
    }
}
