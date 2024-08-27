using Expeditionary.Hexagons;
using OpenTK.Mathematics;
using static Expeditionary.Hexagons.Axial;

namespace Expeditionary.Model.Mapping
{
    public class Map
    {
        public int Width => _tiles.GetLength(0);
        public int Height => _tiles.GetLength(1);

        private readonly Tile[,] _tiles;
        private readonly Edge[,] _edges;

        public Map(Vector2i size)
        {
            _tiles = new Tile[size.X, size.Y];
            _edges = new Edge[2 * size.X + 1, 2 * size.Y + 1];


            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    _tiles[i, j] = new();
                }
            }
            for (int i=0; i < _edges.GetLength(0); ++i)
            {
                for (int j=0; j< _edges.GetLength(1); ++j)
                {
                    _edges[i, j] = new();
                }
            }
        }

        public IEnumerable<Vector3i> GetCorners()
        {
            for (int i=0; i< Width + 2; ++i)
            {
                for (int j=0; j < 2 * Height +2; ++j)
                {
                    yield return Cubic.TriangularOffset.Instance.Wrap(new(i, j));
                }
            }
        }

        public Edge? GetEdge(Vector3i position)
        {
            var offset = Cubic.HexagonalOffset.Instance.Project(position) + new Vector2i(1, 1);
            if (offset.X < 0 || offset.Y < 0 || offset.X >= _edges.GetLength(0) || offset.Y >= _edges.GetLength(1))
            {
                return null;
            }
            return _edges[offset.X, offset.Y];
        }

        public Tile? GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return null;
            }
            return _tiles[x, y];
        }
        public Tile? GetTile(Vector2i offset)
        {
            return GetTile(offset.X, offset.Y);
        }

        public Tile? GetTile(Vector3i position)
        {
            return GetTile(Cubic.HexagonalOffset.Instance.Project(position));
        }
    }
}
