﻿using Cardamom.Graphics;
using Cardamom.Mathematics;
using Expeditionary.Coordinates;
using Expeditionary.Model.Mapping;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Expeditionary.View
{
    public class MapViewFactory
    {
        private static readonly Axial2i[] s_Neighbors =
        {
            new(-1, 1),
            new(-1, 0),
            new(0, -1)
        };

        private readonly MapViewParameters _parameters;
        private readonly TerrainTextureLibrary _tileBaseLibrary;
        private readonly RenderShader _tileBaseShader;

        public MapViewFactory(
            MapViewParameters parameters, TerrainTextureLibrary tileBaseLibrary, RenderShader tileBaseShader)
        {
            _parameters = parameters;
            _tileBaseLibrary = tileBaseLibrary;
            _tileBaseShader = tileBaseShader;
        }

        public MapView Create(Map map, TerrainViewParameters parameters, int seed)
        {
            TerrainTextureLibrary.Option[] options = _tileBaseLibrary.Query().ToArray();
            var random = new Random(seed);
            Vertex3[] vertices = new Vertex3[18 * (map.Width - 1) * (map.Height - 1)];
            var xRange = new IntInterval(0, map.Width - 1);
            var yRange = new IntInterval(0, map.Height - 1);
            int v = 0;
            for (int x = 1; x < map.Width; ++x)
            {
                for (int y = 0; y < map.Height; ++y)
                {
                    var center = map.Get(new(x, y));
                    var centerAxial = Offset2i.ToAxial(new(x, y));
                    var centerPos = Axial2i.ToCartesian(centerAxial);
                    for (int i = 0; i < 2; ++i)
                    {
                        var leftAxial = centerAxial + s_Neighbors[i];
                        var leftOffset = Axial2i.ToOffset(leftAxial);
                        if (!xRange.Contains(leftOffset.X) || !yRange.Contains(leftOffset.Y))
                        {
                            continue;
                        }
                        var rightAxial = centerAxial + s_Neighbors[i + 1];
                        var rightOffset = Axial2i.ToOffset(rightAxial);
                        if (!xRange.Contains(rightOffset.X) || !yRange.Contains(rightOffset.Y))
                        {
                            continue;
                        }

                        var left = map.Get(leftOffset);
                        var right = map.Get(rightOffset);
                        var selected = options[random.Next(options.Length)];
                        for (int j = 0; j < 3; ++j)
                        {
                            Tile tile;
                            if (j == 0)
                            {
                                tile = center;
                            }
                            else if (j == 1)
                            {
                                tile = left;
                            }
                            else
                            {
                                tile = right;
                            }
                            var e = (int)(tile.Elevation * _parameters.ElevationLevel) 
                                / (_parameters.ElevationLevel - 1f);
                            var color = parameters.StoneParameters!.Colors[tile.Terrain!.Stone];
                            color = (Color4)Color4.ToHsl(color);
                            color.B = 
                                Math.Min(
                                    1, 
                                    Math.Max(
                                        color.B + _parameters.ElevationGradient.Minimum + e * 
                                            (_parameters.ElevationGradient.Maximum -
                                                _parameters.ElevationGradient.Minimum), 
                                        0));
                            color = Color4.FromHsl((Vector4)color);
                            vertices[v++] = new(ToVector3(centerPos), color, selected.TexCoords[j][0]);
                            vertices[v++] = 
                                new(ToVector3(Axial2i.ToCartesian(leftAxial)), color, selected.TexCoords[j][1]);
                            vertices[v++] =
                                new(ToVector3(Axial2i.ToCartesian(rightAxial)), color, selected.TexCoords[j][2]);
                        }
                    }
                }
            }
            return new MapView(
                new VertexBuffer<Vertex3>(vertices, PrimitiveType.Triangles),
                _tileBaseLibrary.GetTexture(), 
                _tileBaseShader);
        }

        private static Vector3 ToVector3(Vector2 x)
        {
            return new(x.X, 0, x.Y);
        }
    }
}