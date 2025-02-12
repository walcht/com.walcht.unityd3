using System;
using TMPro;
using UnityEngine;

namespace UnityD3
{
    public class AxisBottom<InputType> : Axis<InputType> where InputType : unmanaged, IFormattable
    {
        /// <summary>
        ///     Constructs a new bottom-oriented axis generator for the given scale with empty tick arguments. In this
        ///     orientation, ticks are drawn below the horizontal domain path.
        /// </summary>
        /// <param name="scale">
        ///     scale to use for this axis. The scale is used for determining the extents of the main
        ///     axis line and for generating its ticks
        /// </param>
        /// <param name="tick_count">Number of ticks to generate for this axis</param>
        public AxisBottom(IScaleContinuous<InputType, float> scale, int tick_count = 6) : base()
        {
            m_scale = scale;
            m_axis_container = new GameObject("axis_bottom", new Type[] { typeof(LineRenderer) });
            m_axis_line = m_axis_container.GetComponent<LineRenderer>();
            SetTickCount(tick_count);

            // register callbacks to scale change events
            m_scale.DomainChanged += OnScaleDomainChange;
            m_scale.RangeChanged += OnScaleRangeChange;
        }

        public override Axis<InputType> SetTickFontSize(float font_size)
        {
            foreach (TextMeshPro t in m_tick_texts)
            {
                t.fontSize = font_size;
                t.ForceMeshUpdate();
                t.transform.localPosition = new Vector3(
                    t.transform.localPosition.x,
                    -m_tick_size - t.renderedHeight / 2,
                    t.transform.localPosition.z
                );
            }
            if (m_tick_texts.Length > 0) m_tick_fontsize = m_tick_texts[0].fontSize;
            return this;
        }

        public override Axis<InputType> SetTickSize(float tick_size)
        {
            m_tick_size = tick_size;
            foreach (LineRenderer l in m_tick_lines) l.SetPosition(1, new Vector3(0.0f, -m_tick_size, 0.0f));
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
                lr.gameObject.transform.localPosition = new Vector3(0.0f, m_axis_stroke_width / 2, 0.0f);
            return this;
        }

        public override Axis<InputType> Update()
        {
            if (!m_dirty) return this;
            m_dirty = false;

            ConstructMainLine(new(1.0f, 0.0f, 0.0f));
            ConstructTickContainers(new(1.0f, 0.0f, 0.0f));
            ConstructTickLines(new(0.0f, 1.0f, 0.0f));
            ConstructTickTexts(new(0.0f, 1.0f, 0.0f));

            return this;
        }
    }
}