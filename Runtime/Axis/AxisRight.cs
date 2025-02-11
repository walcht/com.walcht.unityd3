using System;
using TMPro;
using UnityEngine;

namespace UnityD3
{
    public class AxisRight<InputType> : Axis<InputType> where InputType : unmanaged, IFormattable
    {

        /// <summary>
        ///     Constructs a new right-oriented axis generator for the given scale. In this orientation, ticks are drawn
        ///     to the right of the vertical domain path.
        /// </summary>
        /// <param name="scale">
        ///     scale to use for this axis. The scale is used for determining the extents of the main axis line and for
        ///     generating its ticks
        /// </param>
        /// <param name="tick_count">Number of ticks to generate for this axis</param>
        public AxisRight(IScaleContinuous<InputType, float> scale, int tick_count = 6) : base()
        {
            m_scale = scale;
            m_axis_container = new GameObject("axis_left", new Type[] { typeof(LineRenderer) });
            m_axis_line = m_axis_container.GetComponent<LineRenderer>();

            // construct the main line
            ConstructMainLine();

            // set default properties
            SetTickCount(tick_count);

            // register callbacks to scale change events
            m_scale.DomainChanged += OnScaleDomainChange;
            m_scale.RangeChanged += OnScaleRangeChange;
        }

        public override Axis<InputType> SetTickCount(int count)
        {
            // clear/destroy previous ticks
            foreach (GameObject go in m_tick_containers) GameObject.Destroy(go);

            // get tick positions
            InputType[] ticks = m_scale.Ticks(count);
            m_tick_count = ticks.Length;

            // construct tick containers GameObjects
            m_tick_containers = new GameObject[m_tick_count];
            for (int i = 0; i < m_tick_count; ++i)
            {
                float y = m_scale.F(ticks[i]);
                GameObject tick_container = new($"tick_{i}");
                tick_container.transform.parent = m_axis_container.transform;
                tick_container.transform.localPosition = new Vector3(0.0f, y, 0.0f);
                m_tick_containers[i] = tick_container;
            }

            // construct tick lines
            m_tick_lines = new LineRenderer[m_tick_count];
            for (int i = 0; i < m_tick_count; ++i)
            {
                GameObject go = new("tick_line", new Type[] { typeof(LineRenderer) });
                go.transform.SetParent(m_tick_containers[i].transform, worldPositionStays: false);
                LineRenderer tick_line = go.GetComponent<LineRenderer>();
                tick_line.useWorldSpace = false;
                tick_line.positionCount = 2;
                tick_line.SetPositions(new Vector3[] {
                    Vector3.zero,
                    new(m_tick_size, 0.0f, 0.0f)
                });
                tick_line.gameObject.transform.localPosition = new Vector3(m_axis_stroke_width / 2, 0.0f, 0.0f);
                tick_line.startWidth = (tick_line.endWidth = m_tick_stroke_width);
                m_tick_stroke_width = tick_line.endWidth;
                m_tick_lines[i] = tick_line;
            }

            // construct tick texts
            m_tick_texts = new TextMeshPro[m_tick_count];
            for (int i = 0; i < m_tick_count; ++i)
            {
                GameObject go = new("tick_text", new Type[] { typeof(TMPro.TextMeshPro) });
                go.transform.SetParent(m_tick_containers[i].transform, worldPositionStays: false);
                TMPro.TextMeshPro tick_text = go.GetComponent<TextMeshPro>();
                tick_text.isTextObjectScaleStatic = true;
                tick_text.autoSizeTextContainer = true;
                tick_text.text = m_scale.TickText(ticks[i]);
                tick_text.fontSize = m_tick_fontsize;
                tick_text.ForceMeshUpdate();
                tick_text.transform.localPosition = new Vector3(
                    m_tick_size + tick_text.renderedWidth / 2,
                    tick_text.transform.localPosition.x,
                    tick_text.transform.localPosition.z
                );
                m_tick_texts[i] = tick_text;
            }

            // re-assign because the actual font size that was set might be different
            // from what it was initially set to
            if (m_tick_texts.Length > 0) m_tick_fontsize = m_tick_texts[0].fontSize;

            return this;
        }

        public override Axis<InputType> SetTickFontSize(float font_size)
        {
            foreach (TextMeshPro t in m_tick_texts)
            {
                t.fontSize = font_size;
                t.ForceMeshUpdate();
                t.transform.localPosition = new Vector3(
                    m_tick_size + t.renderedWidth / 2,
                    t.transform.localPosition.x,
                    t.transform.localPosition.z
                );
            }
            if (m_tick_texts.Length > 0) m_tick_fontsize = m_tick_texts[0].fontSize;
            return this;
        }

        public override Axis<InputType> SetTickSize(float tick_size)
        {
            m_tick_size = tick_size;
            foreach (LineRenderer l in m_tick_lines) l.SetPosition(1, new Vector3(m_tick_size, 0.0f, 0.0f));
            // update tick texts positions
            SetTickFontSize(m_tick_fontsize);
            return this;
        }

        public override Axis<InputType> SetAxisStrokeWidth(float stroke_width)
        {
            m_axis_line.startWidth = stroke_width;
            m_axis_line.endWidth = stroke_width;
            m_axis_stroke_width = m_axis_line.endWidth;
            // whenever the axis' stroke width is modified, the ticks have to be repositioned accordingly
            foreach (LineRenderer lr in m_tick_lines)
                lr.gameObject.transform.localPosition = new Vector3(m_axis_stroke_width / 2, 0.0f, 0.0f);
            return this;
        }

        protected override void ConstructMainLine()
        {
            m_axis_line.useWorldSpace = false;
            m_axis_line.positionCount = 2;
            Vector3[] positions = new Vector3[2];
            m_scale.Domain(out InputType x0, out InputType x1);
            positions[0] = new Vector3(0.0f, m_scale.F(x0), 0.0f);
            positions[1] = new Vector3(0.0f, m_scale.F(x1), 0.0f);
            m_axis_line.SetPositions(positions);
            m_axis_line.startWidth = (m_axis_line.endWidth = m_axis_stroke_width);
            m_axis_stroke_width = m_axis_line.endWidth;
        }
    }
}