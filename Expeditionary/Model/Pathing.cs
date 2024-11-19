using Cardamom.Collections;
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
            public bool Open { get; set; }
            public bool Closed { get; set; }
            public float Cost { get; set; }
            public Node? Parent { get; set; }

            internal Node(Vector3i hex, Tile tile)
            {
                Hex = hex;
                Cost = float.MaxValue;
                Tile = tile;
            }
        }

        public static IEnumerable<PathOption> GetPathField(
            Map map, Vector3i position, Movement movement, float maxCost = float.PositiveInfinity)
        {
            var open = new Heap<Node, float>();
            var nodes = new Dictionary<Vector3i, Node>();
            
            var startNode =
                new Node(position, map.GetTile(position)!)
                {
                    Open = true,
                    Cost = 0
                };
            nodes.Add(position, startNode);
            open.Push(startNode, 0);

            while (open.Count > 0)
            {
                var current = open.Pop();
                current.Open = false;
                current.Closed = true;
                foreach (var neighbor in Geometry.GetNeighbors(current.Hex))
                {
                    var neighborTile = map.GetTile(neighbor);
                    if (neighborTile == null)
                    {
                        continue;
                    }

                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = new(neighbor, neighborTile);
                        nodes.Add(neighbor, neighborNode);
                    }
                    
                    if (neighborNode.Closed)
                    {
                        continue;
                    }

                    var cost = current.Cost
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
                        open.Push(neighborNode, cost);
                    }
                }
            }

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
            float maxCost = float.PositiveInfinity)
        {
            var open = new Heap<Node, float>();
            var nodes = new Dictionary<Vector3i, Node>();

            var startNode =
                new Node(position, map.GetTile(position)!)
                {
                    Open = true,
                    Cost = 0
                };
            nodes.Add(position, startNode);
            open.Push(startNode, 0);

            while (open.Count > 0)
            {
                var current = open.Pop();

                if (current.Hex == destination)
                {
                    return BuildPath(current);
                }

                current.Open = false;
                current.Closed = true;
                foreach (var neighbor in Geometry.GetNeighbors(current.Hex))
                {
                    var neighborTile = map.GetTile(neighbor);
                    if (neighborTile == null)
                    {
                        continue;
                    }

                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = new(neighbor, neighborTile);
                        nodes.Add(neighbor, neighborNode);
                    }

                    if (neighborNode.Closed)
                    {
                        continue;
                    }

                    var cost = current.Cost 
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
                        open.Push(neighborNode, cost + Geometry.GetCartesianDistance(destination, neighbor));
                    }
                }
            }

            return null;
        }

        private static Movement.Hindrance GetHindrance(Tile origin, Tile destination, Edge edge)
        {
            if (edge.Levels.ContainsKey(EdgeType.Road))
            {
                return new(restriction: 0, roughness: 0, slope: 0, softness: 0, waterDepth: 0);
            }
            if (destination.Terrain.IsLiquid)
            {
                return new(restriction: 0, roughness: 0, slope: 0, softness: 0, waterDepth: 5);
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
