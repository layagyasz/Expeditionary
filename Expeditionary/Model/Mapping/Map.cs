﻿using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class Map
    {
        public Vector2i Size => new(Width, Height);
        public int ElevationLevels { get; }
        public int Width => _tiles.GetLength(0);
        public int Height => _tiles.GetLength(1);

        private readonly Tile[,] _tiles;
        private readonly Edge[,] _edges;

        public Map(Vector2i size, int elevationLevels)
        {
            _tiles = new Tile[size.X, size.Y];
            _edges = new Edge[2 * size.X + 1, 2 * size.Y + 1];
            ElevationLevels = elevationLevels;


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

        public bool ContainsTile(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= Width || y >= Height);
        }

        public bool ContainsTile(Vector2i offset)
        {
            return ContainsTile(offset.X, offset.Y);
        }

        public IEnumerable<Vector3i> GetTiles()
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j <  Height; ++j)
                {
                    yield return Cubic.HexagonalOffset.Instance.Wrap(new(i, j));
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
            return ContainsTile(x, y) ? _tiles[x, y] : null;
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
