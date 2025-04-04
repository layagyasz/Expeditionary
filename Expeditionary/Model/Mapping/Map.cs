﻿using Cardamom.Collections;
using Expeditionary.Hexagons;
using OpenTK.Mathematics;

namespace Expeditionary.Model.Mapping
{
    public class Map : DenseHexGrid<Tile>
    {
        public int ElevationLevels { get; }

        private readonly Edge?[,] _edges;

        private MultiMap<MapTag, Vector3i>? _areas;

        private Map(Vector2i size, int elevationLevels) : base(size)
        {
            _edges = new Edge[2 * size.X + 1, 2 * size.Y + 1];
            ElevationLevels = elevationLevels;
        }

        public static Map Create(Vector2i size, int elevationLevels)
        {
            var map = new Map(size, elevationLevels);
            for (int i = 0; i < size.X; ++i)
            {
                for (int j = 0; j < size.Y; ++j)
                {
                    map.Set(i, j, new());
                }
            }
            for (int i = 0; i < 2 * size.X + 1; ++i)
            {
                for (int j = 0; j < 2 * size.Y + 1; ++j)
                {
                    map.SetEdge(i, j, new());
                }
            }
            return map;
        }

        public IEnumerable<Vector3i> GetArea(MapTag tag)
        {
            if (_areas == null)
            {
                PopulateAreas();
            }
            if (_areas!.TryGetValue(tag, out IEnumerable<Vector3i> hexes))
            {
                return hexes;
            }
            return Enumerable.Empty<Vector3i>();
        }

        public int GetAxisSize(MapDirection direction)
        {
            int size = 0;
            if (direction.HasFlag(MapDirection.North) || direction.HasFlag(MapDirection.South))
            {
                size = Math.Max(size, Size.Y); 
            }
            if (direction.HasFlag(MapDirection.East) || direction.HasFlag(MapDirection.West))
            {
                size = Math.Max(size, Size.X);
            }
            return size;
        }

        public IEnumerable<Vector3i> GetBorder(MapDirection direction)
        {
            if (direction == MapDirection.North)
            {
                for (Vector2i offset = new(); offset.X < Size.X && offset.Y < Size.Y; offset += new Vector2i(1, 0))
                {
                    yield return Cubic.HexagonalOffset.Instance.Wrap(offset);
                }
            }
            else if (direction == MapDirection.East)
            {
                for (
                    Vector2i offset = new(Size.X - 1, 0);
                    offset.X < Size.X && offset.Y < Size.Y;
                    offset += new Vector2i(0, 1))
                {
                    yield return Cubic.HexagonalOffset.Instance.Wrap(offset);
                }
            }
            else if (direction == MapDirection.South)
            {
                for (
                    Vector2i offset = new(0, Size.Y - 1); 
                    offset.X < Size.X && offset.Y < Size.Y; 
                    offset += new Vector2i(1, 0))
                {
                    yield return Cubic.HexagonalOffset.Instance.Wrap(offset);
                }
            }
            else
            {
                for (Vector2i offset = new(); offset.X < Size.X && offset.Y < Size.Y; offset += new Vector2i(0, 1))
                {
                    yield return Cubic.HexagonalOffset.Instance.Wrap(offset);
                }
            }

        }

        public Edge? GetEdge(Vector3i edge)
        {
            var offset = Cubic.HexagonalOffset.Instance.Project(edge) + new Vector2i(1, 1);
            if (offset.X < 0 || offset.Y < 0 || offset.X >= _edges.GetLength(0) || offset.Y >= _edges.GetLength(1))
            {
                return null;
            }
            return _edges[offset.X, offset.Y];
        }

        public void SetEdge(int x, int y, Edge? value)
        {
            if (x < 0 || y < 0 || x >= _edges.GetLength(0) || y >= _edges.GetLength(1))
            {
                throw new IndexOutOfRangeException();
            }
            _edges[x, y] = value;
        }

        public void SetEdge(Vector2i offset, Edge? value)
        {
            SetEdge(offset.X, offset.Y, value);
        }

        public void SetEdge(Vector3i edge, Edge? value)
        {
            SetEdge(Cubic.HexagonalOffset.Instance.Project(edge) + new Vector2i(1, 1), value);
        }

        private void PopulateAreas()
        {
            _areas = new();
            foreach (var hex in Range())
            {
                var tile = Get(hex)!;
                foreach (var t in tile.Tags)
                {
                    _areas.Add(t, hex);
                }
            }
        }
    }
}
