using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public class Line2D<T, D> : IGenerator2D<T, D> where T : IEnumerable<D>
    {
        private LineRenderer m_line_renderer;
        private float m_stroke_width = 0.0175f;

        public Line2D(T data, Func<D, float> x_accessor, Func<D, float> y_accessor,
            Func<D, bool> filter = null)
        {
            m_container = new GameObject("line", new Type[] { typeof(LineRenderer) });
            m_line_renderer = m_container.GetComponent<LineRenderer>();
            m_line_renderer.useWorldSpace = false;

            m_data = data;
            m_x_accessor = x_accessor;
            m_y_accessor = y_accessor;
            if (filter == null) m_filter = (d) => false;
            else m_filter = filter;

            // default properties
            SetStrokeWidth(m_stroke_width);
        }

        public override IGenerator2D<T, D> Update()
        {
            if (m_dirty)
            {
                m_dirty = false;
                List<Vector3> positions = new();
                // construct the line
                foreach (D d in m_data)
                {
                    if (m_filter(d)) continue;
                    positions.Add(new Vector3(
                        m_x_accessor(d),
                        m_y_accessor(d),
                        0.0f
                    ));
                }
                m_line_renderer.positionCount = positions.Count;
                m_line_renderer.SetPositions(positions.ToArray());
            }
            return this;
        }

        public override IGenerator2D<T, D> SetStrokeWidth(float stroke_width)
        {
            m_line_renderer.startWidth = stroke_width;
            m_line_renderer.endWidth = stroke_width;
            m_stroke_width = m_line_renderer.endWidth;
            return this;
        }

        public override IGenerator2D<T, D> SetMaterial(Material mat)
        {
            m_line_renderer.material = new Material(mat);
            return this;
        }

        public override IGenerator2D<T, D> SetColor(Color color)
        {
            m_line_renderer.material.color = color;
            return this;
        }
    }
}