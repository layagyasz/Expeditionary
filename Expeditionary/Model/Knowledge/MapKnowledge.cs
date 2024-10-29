using Expeditionary.Hexagons;
using Expeditionary.Model.Combat;
using Expeditionary.Model.Mapping;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Knowledge
{
    public class MapKnowledge
    {
        private readonly TileKnowledge[,] _tiles;

        public MapKnowledge(TileKnowledge[,] tiles) 
        {
            _tiles = tiles;
        }

        public static MapKnowledge Create(Vector2i size, bool isDiscovered)
        {
            var tiles = new TileKnowledge[size.X, size.Y];
            for (int i=0; i<size.X; ++i)
            {
                for (int j=0; j<size.Y; ++j)
                {
                    tiles[i, j] = new(isDiscovered, VisibilityCounter: 0);
                }
            }
            return new MapKnowledge(tiles);
        }

        public TileKnowledge Get(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            if (coord.X < 0 || coord.X >= _tiles.GetLength(0) || coord.Y < 0 || coord.Y >= _tiles.GetLength(1))
            {
                return new(IsDiscovered: false, VisibilityCounter: 0);
            }
            return _tiles[coord.X, coord.Y];
        }

        public List<Vector3i> Move(Map map, IAsset asset, Pathing.Path path)
        {
            var initialSightField = Sighting.GetSightField(map, asset, path.Origin).Select(x => x.Target).ToHashSet();
            var stepSightFields =
                path.Steps.Take(path.Steps.Count - 1)
                    .SelectMany(x => Sighting.GetSightField(map, asset, x))
                    .Select(x => x.Target)
                    .ToHashSet();
            var finalSightField =
                Sighting.GetSightField(map, asset, path.Destination).Select(x => x.Target).ToHashSet();
            stepSightFields.ExceptWith(initialSightField);
            stepSightFields.ExceptWith(finalSightField);

            var result = new List<Vector3i>();
            foreach (var hex in initialSightField)
            {
                if (Decrement(hex))
                {
                    result.Add(hex);
                }
            }
            foreach (var hex in stepSightFields)
            {
                if (Discover(hex))
                {
                    result.Add(hex);
                }
            }
            foreach (var hex in finalSightField)
            {
                if (Increment(hex))
                {
                    result.Add(hex);
                }
            }
            return result;
        }

        public List<Vector3i> Place(Map map, IAsset asset, Vector3i position)
        {
            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(map, asset, position))
            {
                if (Increment(los.Target))
                {
                    result.Add(los.Target);
                }
            }
            return result;
        }

        public List<Vector3i> Remove(Map map, IAsset asset)
        {
            var result = new List<Vector3i>();
            foreach (var los in Sighting.GetSightField(map, asset, asset.Position))
            {
                if (Decrement(los.Target))
                {
                    result.Add(los.Target);
                }
            }
            return result;
        }

        private bool Discover(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            var current = _tiles[coord.X, coord.Y].IsDiscovered;
            _tiles[coord.X, coord.Y].IsDiscovered = true;
            return !current;
        }

        private bool Decrement(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            return --_tiles[coord.X, coord.Y].VisibilityCounter == 0;
        }

        private bool Increment(Vector3i hex)
        {
            var coord = Cubic.HexagonalOffset.Instance.Project(hex);
            _tiles[coord.X, coord.Y].IsDiscovered = true;
            return ++_tiles[coord.X, coord.Y].VisibilityCounter > 0;
        }
    }
}
