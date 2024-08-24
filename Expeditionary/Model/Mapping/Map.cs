using OpenTK.Mathematics;

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

        public Edge GetEdge(Vector2i offset)
        {
            return _edges[offset.X + 1, offset.Y + 1];
        }

        public Tile GetTile(Vector2i offset)
        {
            return _tiles[offset.X, offset.Y];
        }

        public void Set(Vector2i offset, Tile tile)
        {
            _tiles[offset.X, offset.Y] = tile;
        }

        public void Set(Vector2i offset, Edge edge)
        {
            _edges[offset.X + 1, offset.Y + 1] = edge;
        }
    }
}
