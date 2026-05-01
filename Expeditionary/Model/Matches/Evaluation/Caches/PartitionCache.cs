using Expeditionary.Hexagons;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Matches.Evaluation.Caches
{
    public class PartitionCache
    {
        private class PartitionCacheLayer
        {
            private readonly Map _map;
            private readonly Movement.Hindrance _maxHindrance;
            private readonly DenseHexGrid<byte> _partition;

            private byte _partitionId = 0;

            internal PartitionCacheLayer(Map map, Movement.Hindrance maxHindrance)
            {
                _map = map;
                _maxHindrance = maxHindrance;
                _partition = new(map.Size);
            }

            public byte ComputePartition(Vector3i hex)
            {
                FindPartition(hex, ++_partitionId);
                return _partitionId;
            }

            public byte GetPartition(Vector3i hex)
            {
                return _partition.Get(hex);
            }

            private void FindPartition(Vector3i hex, byte partitionId)
            {
                Queue<Vector3i> open = new();
                open.Enqueue(hex);
                HashSet<Vector3i> closed = new();
                while (open.TryDequeue(out var current))
                {
                    if (closed.Contains(current))
                    {
                        continue;
                    }
                    closed.Add(current);
                    if (_partition.Get(current) != 0)
                    {
                        throw new InvalidProgramException();
                    }
                    _partition.Set(current, partitionId);
                    var tile = _map.Get(current)!;
                    foreach (var neighbor in Geometry.GetNeighbors(current))
                    {
                        var neighborTile = _map.Get(neighbor);
                        if (neighborTile == null)
                        {
                            continue;
                        }
                        var hindrance = 
                            Movement.GetHindrance(
                                tile, neighborTile, _map.GetEdge(Geometry.GetEdge(current, neighbor))!);
                        if (!Movement.IsOver(hindrance, _maxHindrance))
                        {
                            open.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        private readonly Map _map;

        private readonly Dictionary<Movement.Hindrance, PartitionCacheLayer> _layers = new();

        public PartitionCache(Map map)
        {
            _map = map;
        }

        public bool IsReachable(Vector3i left, Vector3i right, Movement.Hindrance maxHindrance)
        {
            var layer = GetLayer(maxHindrance);
            var leftPartition = layer.GetPartition(left);
            var rightPartition = layer.GetPartition(right);
            if (leftPartition == 0 && rightPartition == 0)
            {
                leftPartition = layer.ComputePartition(left);
                rightPartition = layer.GetPartition(right);
            }
            return leftPartition == rightPartition;
        }

        private PartitionCacheLayer GetLayer(Movement.Hindrance key)
        {
            if (_layers.TryGetValue(key, out var layer))
            {
                return layer;
            }
            layer = new(_map, key);
            _layers.Add(key, layer);
            return layer;
        }
    }
}
