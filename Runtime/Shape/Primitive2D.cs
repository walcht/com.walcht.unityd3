using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public enum PrimitiveShape2D
    {
        RECT,
        CIRCLE,
        DOT
    }

    public class Primitive2D<T, D> : IGenerator2D<T, D> where T : IEnumerable<D>
    {
        private List<GameObject> m_primitives = new();
        private Material m_material;

        public Primitive2D(T data, Func<D, float> x_accessor, Func<D, float> y_accessor, Func<D, float> width_accessor,
            Func<D, float> height_accessor, PrimitiveShape2D shape, Func<D, bool> filter = null)
        {
            m_container = new GameObject("primitive");

            m_data = data;
            m_x_accessor = x_accessor;
            m_y_accessor = y_accessor;
            m_width_accessor = width_accessor;
            m_height_accessor = height_accessor;
            m_shape = shape;
            if (filter == null) m_filter = (d) => true;
            else m_filter = filter;
            Update();
        }

        public override IGenerator2D<T, D> Update()
        {
            foreach (GameObject go in m_primitives) GameObject.Destroy(go);
            m_primitives.Clear();
            string _name = m_shape switch
            {
                PrimitiveShape2D.RECT => "rect",
                PrimitiveShape2D.CIRCLE => "circle",
                _ => throw new NotImplementedException(),
            };
            int counter = 0;
            Mesh mesh = Utils.GeneratePrimitiveMesh(m_shape);
            foreach (D d in m_data)
            {
                if (!m_filter(d)) continue;
                GameObject go = new($"{_name}_{counter}", new Type[] {
                    typeof(MeshFilter),
                    typeof(MeshRenderer)
                });
                go.transform.SetParent(m_container.transform, worldPositionStays: false);
                go.GetComponent<MeshFilter>().sharedMesh = mesh;
                m_primitives.Add(go);
                go.transform.localPosition = new Vector3(
                    m_x_accessor(d),
                    m_y_accessor(d),
                    0.0f
                );
                go.transform.localScale = new Vector3(
                    m_width_accessor(d),
                    m_height_accessor(d),
                    1.0f
                );
                ++counter;
            }
            return this;
        }

        public override IGenerator2D<T, D> SetMaterial(Material mat)
        {
            m_material = new Material(mat);
            foreach (GameObject go in m_primitives)
            {
                go.GetComponent<MeshRenderer>().sharedMaterial = m_material;
            }
            return this;
        }

        public override IGenerator2D<T, D> SetColor(Color color)
        {
            m_material.color = color;
            return this;
        }

        public override IGenerator2D<T, D> SetStrokeWidth(float _) => this;
    }
}