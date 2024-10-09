using Cardamom.Collections;
using Expeditionary.Hexagons;
using Expeditionary.Model.Combat.Units;
using OpenTK.Mathematics;

namespace Expeditionary.Model
{
    public static class Pathing
    {
        public record class PathOption(Vector3i Destination, float Cost);

        private class Node
        {
            public Vector3i Hex { get; }
            public bool Open { get; set; }
            public bool Closed { get; set; }
            public float Cost { get; set; }

            internal Node(Vector3i hex)
            {
                Hex = hex;
                Cost = float.MaxValue;
            }
        }

        public static IEnumerable<PathOption> GetPathField(
            Vector3i position, float maxTravel, UnitMovement movement)
        {
            var open = new Heap<Node, float>();
            var nodes = new Dictionary<Vector3i, Node>();
            
            var startNode =
                new Node(position)
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
                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = new(neighbor);
                        nodes.Add(neighbor, neighborNode);
                    }
                    
                    if (neighborNode.Closed)
                    {
                        continue;
                    }

                    // TODO -- read from map
                    var cost = current.Cost + movement.GetCost(0, 0, 0);
                    if (cost > maxTravel)
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
                        neighborNode.Cost = cost;
                        open.Push(neighborNode, cost);
                    }
                }
            }

            foreach (var node in nodes.Values)
            {
                if (node.Closed && !(node.Cost > maxTravel))
                {
                    yield return new(node.Hex, node.Cost);
                }
            }
        }
    }
}
