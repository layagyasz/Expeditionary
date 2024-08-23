using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class Map
    {
        public int Width => _tiles.GetLength(0);
        public int Height => _tiles.GetLength(1);

        private readonly Tile[,] _tiles;

        private Map(Tile[,] tiles)
        {
            _tiles = tiles;
        }

        public Tile Get(Vector2i offset)
        {
            return _tiles[offset.X, offset.Y];
        }

        public class Builder
        {
            private readonly Tile[,] _tiles;

            public Builder(Vector2i size)
            {
                _tiles = new Tile[size.X, size.Y];
            }

            public Builder Set(Vector2i offset, Tile tile)
            {
                _tiles[offset.X, offset.Y] = tile;
                return this;
            }

            public Map Build()
            {
                return new Map(_tiles);
            }
        }
    }
}
