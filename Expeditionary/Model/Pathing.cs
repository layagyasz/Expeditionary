using Cardamom.Collections;
using Expeditionary.Evaluation.Considerations;
using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public static class Pathing
    {
        public record class Path(Vector3i Origin, Vector3i Destination, List<Vector3i> Steps, float Cost);
        public record class PathOption(Vector3i Destination, float Cost);

        private class Node
        {
            public Vector3i Hex { get; }
            public Tile Tile { get; }
            public float ExtraCost { get; }
            public bool Open { get; set; }
            public float Cost { get; set; }
            public bool Closed { get; set; }
            public Node? Parent { get; set; }

            private Node(Vector3i hex, Tile tile, float extraCost, float cost)
            {
                Hex = hex;
                Tile = tile;
                ExtraCost = extraCost;
                Cost = cost;
            }

            internal static Node CreateRoot(Vector3i hex, Tile tile, float cost)
            {
                return new(hex, tile, cost, cost) { Open = true };
            }

            internal static Node Create(Vector3i hex, Tile tile, float extraCost)
            {
                return new(hex, tile, extraCost, float.MaxValue);
            }
        }

        public static IEnumerable<PathOption> GetPathField(
            Map map, 
            Vector3i position,
            Movement movement,
            TileConsideration extraTileCost, 
            float maxCost)
        {
            var nodes = GeneratePaths(map, position, movement, extraTileCost, maxCost, _ => 0, _ => false);
            foreach (var node in nodes.Values)
            {
                if (node.Closed && !(node.Cost > maxCost))
                {
                    yield return new(node.Hex, node.Cost);
                }
            }
        }

        public static Path? GetShortestPath(
            Map map, 
            Vector3i position, 
            Vector3i destination, 
            Movement movement, 
            TileConsideration extraTileCost,
            float maxCost = float.PositiveInfinity)
        {
            var nodes = 
                GeneratePaths(
                    map, 
                    position,
                    movement,
                    extraTileCost,
                    maxCost,
                    x => Geometry.GetCartesianDistance(destination, x),
                    x => x == destination);
            if (nodes.TryGetValue(destination, out var node))
            {
                return BuildPath(node);
            }
            return null;
        }

        private static Dictionary<Vector3i, Node> GeneratePaths(
            Map map,
            Vector3i position,
            Movement movement,
            TileConsideration extraTileCost,
            float maxCost,
            Func<Vector3i, float> heuristic,
            Predicate<Vector3i> exitCondition)
        {
            var open = new Heap<Node, float>();
            var nodes = new Dictionary<Vector3i, Node>();

            var tile = map.Get(position)!;
            var startNode = 
                Node.CreateRoot(position, tile, extraTileCost(position, () => tile, Enumerable.Empty<Edge>));
            nodes.Add(position, startNode);
            open.Push(startNode, startNode.Cost);

            while (open.Count > 0)
            {
                var current = open.Pop();

                if (exitCondition(current.Hex))
                {
                    return nodes;
                }

                current.Open = false;
                current.Closed = true;
                foreach (var neighbor in Geometry.GetNeighbors(current.Hex))
                {
                    var neighborTile = map.Get(neighbor);
                    if (neighborTile == null)
                    {
                        continue;
                    }

                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = 
                            Node.Create(
                                neighbor,
                                neighborTile, 
                                extraTileCost(neighbor, () => neighborTile, Enumerable.Empty<Edge>));
                        nodes.Add(neighbor, neighborNode);
                    }

                    if (neighborNode.Closed)
                    {
                        continue;
                    }

                    var cost = current.Cost
                        + neighborNode.ExtraCost
                        + movement.GetCost(
                            GetHindrance(
                                current.Tile, neighborTile, map.GetEdge(Geometry.GetEdge(current.Hex, neighbor))!));

                    if (cost <= maxCost && cost < neighborNode.Cost)
                    {
                        if (neighborNode.Open)
                        {
                            open.Remove(neighborNode);
                        }
                        else
                        {
                            neighborNode.Open = true;
                        }
                        neighborNode.Cost = cost;
                        neighborNode.Parent = current;
                        open.Push(neighborNode, cost + heuristic(neighbor));
                    }
                }
            }

            return nodes;
        }

        private static Movement.Hindrance GetHindrance(Tile origin, Tile destination, Edge edge)
        {
            if (edge.Levels.ContainsKey(EdgeType.Road))
            {
                return new(Restriction: 0, Roughness: 0, Slope: 0, Softness: 0, WaterDepth: 0);
            }
            if (destination.Terrain.IsLiquid)
            {
                return new(Restriction: 0, Roughness: 0, Slope: 0, Softness: 0, WaterDepth: 5);
            }
            Movement.Hindrance h = destination.Hindrance;
            if (edge.Levels.ContainsKey(EdgeType.River))
            {
                h.WaterDepth = Math.Min(h.WaterDepth + 2, 5);
            }
            if (origin.Elevation != destination.Elevation)
            {
                h.Slope = Math.Abs(origin.Elevation - destination.Elevation);
            }
            h.Restriction = Math.Min(origin.Hindrance.Restriction, destination.Hindrance.Restriction);
            return h;
        }

        private static Path BuildPath(Node destination)
        {
            var current = destination;
            var steps = new List<Vector3i>() { destination.Hex };
            while (current.Parent != null)
            {
                current = current.Parent;
                steps.Insert(0, current.Hex);
            }
            return new(current.Hex, destination.Hex, steps, destination.Cost);
        }
    }
}
