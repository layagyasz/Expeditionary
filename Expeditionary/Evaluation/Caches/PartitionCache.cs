using Expeditionary.Hexagons;
using Expeditionary.Model;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Evaluation.Caches
{
    public class PartitionCache
    {
        private enum Dimension
        {
            Restriction,
            Roughness,
            Slope,
            Softness,
            WaterDepth
        }

        private class PartitionCacheLayer
        {
            private readonly Map _map;
            private readonly Dimension _dimension;
            private readonly int _limit;
            private readonly DenseHexGrid<byte> _partition;

            private byte _partitionId = 0;

            internal PartitionCacheLayer(Map map, Dimension dimension, int limit)
            {
                _map = map;
                _dimension = dimension;
                _limit = limit;
                _partition = new(map.Size);
            }

            public byte ComputePartition(Vector3i hex)
            {
                FindPartition(hex, ++_partitionId);
                return _partitionId;
            }

            public byte GetOrComputeParition(Vector3i hex)
            {
                var partition = GetPartition(hex);
                if (partition == 0)
                {
                    FindPartition(hex, ++_partitionId);
                    return _partitionId;
                }
                return partition;
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
                        int value = GetDimension(
                            _dimension,
                            Movement.GetHindrance(
                                tile, neighborTile, _map.GetEdge(Geometry.GetEdge(current, neighbor))!));
                        if (value <= _limit)
                        {
                            open.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        private readonly Map _map;

        private readonly PartitionCacheLayer?[] _restriction = new PartitionCacheLayer?[5];
        private readonly PartitionCacheLayer?[] _roughness = new PartitionCacheLayer?[5];
        private readonly PartitionCacheLayer?[] _slope = new PartitionCacheLayer?[5];
        private readonly PartitionCacheLayer?[] _softness = new PartitionCacheLayer?[5];
        private readonly PartitionCacheLayer?[] _waterDepth = new PartitionCacheLayer?[5];

        public PartitionCache(Map map)
        {
            _map = map;
        }

        public bool IsReachable(Vector3i left, Vector3i right, Movement.Hindrance maxHindrance)
        {
            return Enum.GetValues<Dimension>().All(x => IsReachable(left, right, maxHindrance, x));
        }

        private bool IsReachable(Vector3i left, Vector3i right, Movement.Hindrance maxHindrance, Dimension dimension)
        {
            var limit = GetDimension(dimension, maxHindrance);
            if (limit >= 5)
            {
                return true;
            }
            var layer = GetLayer(dimension);
            if (layer[limit] == null)
            {
                layer[limit] = new(_map, dimension, limit);
            }
            var layerLimit = layer[limit]!;
            var leftPartition = layerLimit.GetPartition(left);
            var rightPartition = layerLimit.GetPartition(right);
            if (leftPartition == 0 && rightPartition == 0)
            {
                leftPartition = layerLimit.ComputePartition(left);
                rightPartition = layerLimit.GetPartition(right);
            }
            return leftPartition == rightPartition;
        }

        private PartitionCacheLayer?[] GetLayer(Dimension dimension)
        {
            return dimension switch
            {
                Dimension.Restriction => _restriction,
                Dimension.Roughness => _roughness,
                Dimension.Slope => _slope,
                Dimension.Softness => _softness,
                Dimension.WaterDepth => _waterDepth,
                _ => throw new ArgumentException($"Unsupported dimension {dimension}"),
            };
        }

        private static int GetDimension(Dimension dimension, Movement.Hindrance hindrance)
        {
            return dimension switch
            {
                Dimension.Restriction => hindrance.Restriction,
                Dimension.Roughness => hindrance.Roughness,
                Dimension.Slope => hindrance.Slope,
                Dimension.Softness => hindrance.Softness,
                Dimension.WaterDepth => hindrance.WaterDepth,
                _ => throw new ArgumentException($"Unsupported dimension {dimension}"),
            };
        }
    }
}
