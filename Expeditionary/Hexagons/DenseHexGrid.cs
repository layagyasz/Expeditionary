using OpenTK.Mathematics;

namespace Expeditionary.Hexagons
{
    public class DenseHexGrid<T>
    {
        public Vector2i Size => new(Width, Height);
        public int Width => _values.GetLength(0);
        public int Height => _values.GetLength(1);

        private readonly T?[,] _values;

        public DenseHexGrid(Vector2i size)
        {
            _values = new T[size.X, size.Y];
        }

        public bool Contains(int x, int y)
        {
            return !(x < 0 || y < 0 || x >= Width || y >= Height);
        }

        public bool Contains(Vector2i offset)
        {
            return Contains(offset.X, offset.Y);
        }

        public bool Contains(Vector3i hex)
        {
            return Contains(Cubic.HexagonalOffset.Instance.Project(hex));
        }

        public IEnumerable<Vector3i> Range()
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    yield return Cubic.HexagonalOffset.Instance.Wrap(new(i, j));
                }
            }
        }

        public T? Get(int x, int y)
        {
            return Contains(x, y) ? _values[x, y] : default;
        }

        public T? Get(Vector2i offset)
        {
            return Get(offset.X, offset.Y);
        }

        public T? Get(Vector3i hex)
        {
            return Get(Cubic.HexagonalOffset.Instance.Project(hex));
        }

        public void Set(int x, int y, T? value)
        {
            if (Contains(x, y))
            {
                _values[x, y] = value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public void Set(Vector2i offset, T? value)
        {
            Set(offset.X, offset.Y, value);
        }

        public void Set(Vector3i hex, T? value)
        {
            Set(Cubic.HexagonalOffset.Instance.Project(hex), value);
        }
    }
}
