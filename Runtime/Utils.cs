using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public static class Utils
    {
        public static void Extent<Record>(IEnumerable<Record> data, Func<Record, float> accessor,
            out float min, out float max)
        {
            float _min = float.MaxValue;
            float _max = float.MinValue;
            foreach (Record d in data)
            {
                float v = accessor(d);
                _min = Mathf.Min(_min, v);
                _max = Mathf.Max(_max, v);
            }
            min = _min;
            max = _max;
        }

        public static void Extent<Record>(IEnumerable<Record> data, Func<Record, int> accessor,
            out int min, out int max)
        {
            int _min = int.MaxValue;
            int _max = int.MinValue;
            foreach (Record d in data)
            {
                int v = accessor(d);
                _min = Mathf.Min(_min, v);
                _max = Mathf.Max(_max, v);
            }
            min = _min;
            max = _max;
        }


        public static void Extent<Record>(IEnumerable<Record> data, Func<Record, DateTime> accessor,
            out DateTime min, out DateTime max)
        {
            DateTime _min = DateTime.MaxValue;
            DateTime _max = DateTime.MinValue;

            foreach (Record d in data)
            {
                DateTime v = accessor(d);
                if (DateTime.Compare(v, _min) < 0) _min = v;
                if (DateTime.Compare(v, _max) > 0) _max = v;
            }
            min = _min;
            max = _max;
        }

        public static Mesh GeneratePrimitiveMesh(PrimitiveShape2D shape)
        {
            Mesh mesh = new();
            switch (shape)
            {
                case PrimitiveShape2D.RECT:
                    {
                        // quad
                        mesh.vertices = new Vector3[] {
                        new (-0.5f, -0.5f, 0.0f),  // 0
                        new (-0.5f, 0.5f, 0.0f),   // 1
                        new (0.5f, 0.5f, 0.0f),    // 2
                        new (0.5f, -0.5f, 0.0f)    // 3
                    };
                        mesh.triangles = new int[] {
                        0, 1, 3, 3, 1, 2
                    };
                        break;
                    }
                case PrimitiveShape2D.CIRCLE:
                    {
                        int n = 10;
                        float radius = 1.0f;
                        List<Vector3> vertices = new(n);
                        for (int i = 0; i < n; ++i)
                        {
                            vertices.Add(new(
                                radius * Mathf.Cos(((2 * Mathf.PI) / n) * i),
                                radius * Mathf.Sin(((2 * Mathf.PI) / n) * i),
                                0.0f
                            ));
                        }
                        List<int> indices = new((n - 2) * 3);
                        for (int i = 0; i < (n - 2); ++i)
                        {
                            indices.Add(0);
                            indices.Add(i + 1);
                            indices.Add(i + 2);
                        }
                        List<Vector3> normals = new(n);
                        for (int i = 0; i < n; ++i)
                        {
                            normals.Add(-Vector3.forward);
                        }
                        mesh.vertices = vertices.ToArray();
                        mesh.triangles = indices.ToArray();
                        mesh.normals = normals.ToArray();

                        break;
                    }
                    ;
                default:
                    throw new NotImplementedException();

            }
            return mesh;
        }

        public static Mesh GeneratePrimitiveMesh(PrimitiveShape3D shape)
        {
            Mesh mesh = new();
            switch (shape)
            {
                case PrimitiveShape3D.CUBE:
                    {
                        // cube
                        mesh.vertices = new Vector3[] {
                            new (0.50f, -0.50f, 0.50f),
                            new (-0.50f, -0.50f, 0.50f),
                            new (0.50f, 0.50f, 0.50f),
                            new (-0.50f, 0.50f, 0.50f),
                            new (0.50f, 0.50f, -0.50f),
                            new (-0.50f, 0.50f, -0.50f),
                            new (0.50f, -0.50f, -0.50f),
                            new (-0.50f, -0.50f, -0.50f),
                            new (0.50f, 0.50f, 0.50f),
                            new (-0.50f, 0.50f, 0.50f),
                            new (0.50f, 0.50f, -0.50f),
                            new (-0.50f, 0.50f, -0.50f),
                            new (0.50f, -0.50f, -0.50f),
                            new (0.50f, -0.50f, 0.50f),
                            new (-0.50f, -0.50f, 0.50f),
                            new (-0.50f, -0.50f, -0.50f),
                            new (-0.50f, -0.50f, 0.50f),
                            new (-0.50f, 0.50f, 0.50f),
                            new (-0.50f, 0.50f, -0.50f),
                            new (-0.50f, -0.50f, -0.50f),
                            new (0.50f, -0.50f, -0.50f),
                            new (0.50f, 0.50f, -0.50f),
                            new (0.50f, 0.50f, 0.50f),
                            new (0.50f, -0.50f, 0.50f),
                        };

                        mesh.triangles = new int[] {
                            0, 2, 3, 0, 3, 1,
                            8, 4, 5, 8, 5, 9,
                            10, 6, 7, 10, 7, 11,
                            12, 13, 14, 12, 14, 15,
                            16, 17, 18, 16, 18, 19,
                            20, 21, 22, 20, 22, 23
                        };

                        mesh.normals = new Vector3[] {
                            new (0.00f, 0.00f, 1.00f),
                            new (0.00f, 0.00f, 1.00f),
                            new (0.00f, 0.00f, 1.00f),
                            new (0.00f, 0.00f, 1.00f),
                            new (0.00f, 1.00f, 0.00f),
                            new (0.00f, 1.00f, 0.00f),
                            new (0.00f, 0.00f, -1.00f),
                            new (0.00f, 0.00f, -1.00f),
                            new (0.00f, 1.00f, 0.00f),
                            new (0.00f, 1.00f, 0.00f),
                            new (0.00f, 0.00f, -1.00f),
                            new (0.00f, 0.00f, -1.00f),
                            new (0.00f, -1.00f, 0.00f),
                            new (0.00f, -1.00f, 0.00f),
                            new (0.00f, -1.00f, 0.00f),
                            new (0.00f, -1.00f, 0.00f),
                            new (-1.00f, 0.00f, 0.00f),
                            new (-1.00f, 0.00f, 0.00f),
                            new (-1.00f, 0.00f, 0.00f),
                            new (-1.00f, 0.00f, 0.00f),
                            new (1.00f, 0.00f, 0.00f),
                            new (1.00f, 0.00f, 0.00f),
                            new (1.00f, 0.00f, 0.00f),
                            new (1.00f, 0.00f, 0.00f)
                        };
                        break;
                    }
                default:
                    throw new NotImplementedException();

            }
            return mesh;
        }
    }
}