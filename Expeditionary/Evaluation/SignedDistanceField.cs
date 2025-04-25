using Cardamom.Collections;
using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using Expeditionary.Model.Mapping.Regions;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation
{
    public class SignedDistanceField
    {
        private record class Node
        {
            public Vector3i Hex { get; }
            public bool IsInternal { get; }
            public int Distance { get; set; }
            public bool Open { get; set; }
            public bool Closed { get; set; }

            public Node(Vector3i hex, bool isInternal, int distance, bool open)
            {
                Hex = hex;
                IsInternal = isInternal;
                Distance = distance;
                Open = open;
            }
        }

        public int MaxInternalDistance { get; }
        public int MaxExternalDistance { get; }

        private readonly Dictionary<Vector3i, int> _distances;

        private SignedDistanceField(
            int maxInternalDistance, int maxExternalDistance, Dictionary<Vector3i, int> distances)
        {
            MaxInternalDistance = maxInternalDistance;
            MaxExternalDistance = maxExternalDistance;
            _distances = distances;
        }

        public static SignedDistanceField FromPathField(IEnumerable<Pathing.PathOption> pathField, int frontier)
        {
            return new(
                -frontier,
                pathField.Max(x => (int)Math.Ceiling(x.Cost)) - frontier, 
                pathField.ToDictionary(x => x.Destination, x => (int)Math.Ceiling(x.Cost) - frontier));
        }

        public static SignedDistanceField FromRegion(Map map, IMapRegion region, int maxDistance, MapDirection facing)
        {
            var range = region.Range(map).ToHashSet();
            var open = new Heap<Node, float>();
            var nodes = new Dictionary<Vector3i, Node>();
            foreach (var hex in range.Where(x => IsFacing(x, facing, range, map)))
            {
                var node = new Node(hex, true, 0, true);
                open.Push(node, 0);
                nodes.Add(hex, node);
            }

            int maxInternalDistance = 0;
            while (open.Count > 0)
            {
                var current = open.Pop();
                current.Closed = true;
                var distance = current.Distance + 1;
                foreach (var neighbor in Geometry.GetNeighbors(current.Hex))
                {
                    var neighborTile = map.Get(neighbor);
                    if (neighborTile == null)
                    {
                        continue;
                    }

                    if (!nodes.TryGetValue(neighbor, out var neighborNode))
                    {
                        neighborNode = new Node(neighbor, range.Contains(neighbor), int.MaxValue, false);
                        nodes.Add(neighbor, neighborNode);
                    }

                    if (neighborNode.Closed)
                    {
                        continue;
                    }

                    if (distance < neighborNode.Distance)
                    {
                        if (neighborNode.Open)
                        {
                            open.Remove(neighborNode);
                        }
                        else
                        {
                            neighborNode.Open = true;
                        }
                        neighborNode.Distance = distance;
                        if (neighborNode.IsInternal)
                        {
                            maxInternalDistance = Math.Max(maxInternalDistance, neighborNode.Distance);
                        }
                        if (neighborNode.IsInternal || distance < maxDistance)
                        {
                            open.Push(neighborNode, distance);
                        }
                    }
                }
            }

            return new(
                maxInternalDistance,
                maxDistance,
                nodes.ToDictionary(x => x.Key, x => x.Value.IsInternal ? -x.Value.Distance : x.Value.Distance));
        }

        public int Get(Vector3i hex)
        {
            if (_distances.TryGetValue(hex, out var distance))
            {
                return distance;
            }
            return MaxExternalDistance;
        }

        private static bool IsFacing(Vector3i hex, MapDirection direction, ISet<Vector3i> range, Map map)
        {
            return MapDirectionUtils.GetNeighborsInDirection(hex, direction)
                .Any(y => map.Contains(y) && !range.Contains(y));
        }
    }
}
