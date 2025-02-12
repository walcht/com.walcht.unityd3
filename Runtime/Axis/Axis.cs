using System;
using TMPro;
using UnityEngine;

namespace UnityD3
{
    /// <summary>
    ///     Abstract base class for axis implementations to inherit from
    /// </summary>
    /// <typeparam name="InputType">axis' domain's input type. An axis' range (i.e, output) type is float</typeparam>
    public abstract class Axis<InputType> where InputType : unmanaged, IFormattable
    {
        protected IScaleContinuous<InputType, float> m_scale;
        protected GameObject m_axis_container;
        protected LineRenderer m_axis_line;

        protected float m_tick_size = 0.125f;
        protected float m_tick_fontsize = 2;
        protected Color m_tick_text_color = Color.black;

        protected float m_axis_stroke_width = 0.0175f;
        protected float m_tick_stroke_width = 0.0175f;

        protected int m_tick_count = 0;
        protected GameObject[] m_tick_containers = new GameObject[0];
        protected LineRenderer[] m_tick_lines = new LineRenderer[0];
        protected TextMeshPro[] m_tick_texts = new TextMeshPro[0];
        protected Material m_tick_mat;

        protected bool m_dirty = true;

        /// <summary>
        ///     Number of ticks to generate for this axis. If number of ticks provided hasn't changed this method simply
        ///     returns and does nothing.
        /// </summary>
        /// <param name="count">To disable ticks, set this to a value stricly less than 2</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> SetTickCount(int count)
        {
            int _count = m_scale.Ticks(count).Length;
            if (m_tick_count == _count) return this;
            m_dirty = true;
            m_tick_count = _count;
            return this;
        }

        /// <summary>
        ///     Gets the number of ticks generated for this axis.
        /// </summary>
        /// <returns>number of ticks</returns>
        public int GetTickCount() => m_tick_count;

        /// <summary>
        ///     Sets the text font size for all the ticks associated with this axis.
        /// </summary>
        /// <param name="font_size">font size. This is simply supplied to TMP's fontsize field</param>
        /// <returns>the axis on which this method was called</returns>
        public abstract Axis<InputType> SetTickFontSize(float font_size);

        /// <summary>
        ///     Sets the tick size (length) for all the ticks associated with this axis.
        /// </summary>
        /// <param name="tick_size">tick size (length) in Unity units. Initially set by default to 0.125f</param>
        /// <returns>the axis on which this method was called</returns>
        public abstract Axis<InputType> SetTickSize(float tick_size);

        /// <summary>
        ///     Sets the stroke width (line width) of this axis. Set to 0 to hide the main axis' line.
        /// </summary>
        /// <param name="stroke_width">stroke width in Unity units. Initially set by default to 0.0175f</param>
        /// <returns>the axis on which this method was called</returns>
        public abstract Axis<InputType> SetAxisStrokeWidth(float stroke_width);


        /// <summary>
        ///     
        /// </summary>
        /// <param name="val"></param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> SetTickLookAtCamera(bool val)
        {
            if (val)
                foreach (TextMeshPro t in m_tick_texts)
                {
                    t.gameObject.AddComponent<LookAtCamera>();
                }
            else
                foreach (TextMeshPro t in m_tick_texts)
                {
                    GameObject.Destroy(t.gameObject.GetComponent<LookAtCamera>());
                }
            return this;
        }

        /// <summary>
        ///     Rotates the axis around its pivot's X axis
        /// </summary>
        /// <param name="rotation">rotation in degrees</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> RotateAroundX(float rotation)
        {
            m_axis_container.transform.localRotation = Quaternion.Euler(
                rotation,
                m_axis_container.transform.localRotation.y,
                m_axis_container.transform.localRotation.z
            );
            return this;
        }

        /// <summary>
        ///     Rotates the axis around its pivot's Y axis
        /// </summary>
        /// <param name="rotation">rotation in degrees</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> RotateAroundY(float rotation)
        {
            m_axis_container.transform.localRotation = Quaternion.Euler(
                m_axis_container.transform.localRotation.x,
                rotation,
                m_axis_container.transform.localRotation.z
            );
            return this;
        }

        /// <summary>
        ///     Rotates the axis around its pivot's Z axis
        /// </summary>
        /// <param name="rotation">rotation in degrees</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> RotateAroundZ(float rotation)
        {
            m_axis_container.transform.localRotation = Quaternion.Euler(
                m_axis_container.transform.localRotation.x,
                m_axis_container.transform.localRotation.y,
                rotation
            );
            return this;
        }

        /// <summary>
        ///     Attaches this axis (i.e., its GameObject) to the provided GameObject
        /// </summary>
        /// <param name="game_object">GameObject that this axis will be attached to</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> Attach(GameObject game_object)
        {
            m_axis_container.transform.SetParent(game_object.transform, worldPositionStays: false);
            m_axis_container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            return this;
        }

        /// <summary>
        ///     Sets the Material for this axis' main line renderer. A new Material is created out of the provided
        ///     Material.
        /// </summary>
        /// <param name="mat">Material to be cloned and set to this axis' main line renderer.</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> SetAxisMaterial(Material mat)
        {
            m_axis_line.material = new Material(mat);
            return this;
        }


        /// <summary>
        ///     Sets the shared Material for this axis' ticks' line renderers. A new Material is created out of the
        ///     provided Material.
        /// </summary>
        /// <param name="mat">
        ///     Material to be cloned and set as shared Material to all this axis' tick line
        ///     renderers
        /// </param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> SetTickMaterial(Material mat)
        {

            Material copied_mat = new(mat);
            foreach (LineRenderer l in m_tick_lines)
            {
                l.sharedMaterial = copied_mat;
            }
            m_tick_mat = copied_mat;
            return this;
        }

        /// <summary>
        ///     Sets the stroke width (line width) of all this axis' tick lines. Set to 0 to hide all the tick.
        /// </summary>
        /// <param name="stroke_width">stroke width in Unity units. Initially set by default to 0.0175f</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> SetTickStrokeWidth(float stroke_width)
        {
            foreach (LineRenderer l in m_tick_lines) l.startWidth = (l.endWidth = stroke_width);
            if (m_tick_lines.Length > 0)
                m_tick_stroke_width = m_tick_lines[0].endWidth;
            return this;
        }

        /// <summary>
        ///     Sets the text color for all of this axis' tick texts. Set alpha component to 0 to hide all the ticks.
        /// </summary>
        /// <param name="color">color to be set to all this axis' tick texts</param>
        /// <returns>the axis on which this method was called</returns>
        public Axis<InputType> SetTickTextColor(Color color)
        {
            foreach (TextMeshPro t in m_tick_texts) t.color = color;
            if (m_tick_texts.Length > 0) m_tick_text_color = m_tick_texts[0].color;
            return this;
        }

        /// <summary>
        ///     Gets the Material that was assigned to this axis' main line renderer
        /// </summary>
        /// <returns>axis' main line renderer Material</returns>
        public Material GetAxisMaterial() => m_axis_line.material;

        /// <summary>
        ///     Gets the shared Material that was assigned to all this axis' tick line renderers
        /// </summary>
        /// <returns>axis' tick line renderes shared Material</returns>
        public Material GetTickMaterial() => m_tick_mat;

        /// <summary>
        ///     Tries to regenerate the axis if dirty flag is set. Should be called initially or inside a GameObject's
        ///     Update().
        /// </summary>
        /// <returns>the axis on which this method was called</returns>
        public abstract Axis<InputType> Update();

        protected void OnScaleDomainChange(InputType x0, InputType x1) => m_dirty = true;

        protected void OnScaleRangeChange(float y0, float y1) => m_dirty = true;

        protected void ConstructMainLine(Vector3 axis /* e.g., x => (1.0f, 0.0f, 0.0f) */)
        {
            m_axis_line.useWorldSpace = false;
            m_axis_line.positionCount = 2;
            Vector3[] positions = new Vector3[2];
            m_scale.Domain(out InputType x0, out InputType x1);
            float _x0 = m_scale.F(x0);
            float _x1 = m_scale.F(x1);
            positions[0] = Vector3.Scale(new(_x0, _x0, _x0), axis);
            positions[1] = Vector3.Scale(new(_x1, _x1, _x1), axis);
            m_axis_line.SetPositions(positions);
            m_axis_line.startWidth = (m_axis_line.endWidth = m_axis_stroke_width);
            m_axis_stroke_width = m_axis_line.endWidth;
        }

        protected void ConstructTickContainers(Vector3 axis /* e.g., x => (1.0f, 0.0f, 0.0f) */)
        {
            // clear/destroy previous ticks
            foreach (GameObject go in m_tick_containers) GameObject.Destroy(go);

            // get tick positions
            InputType[] ticks = m_scale.Ticks(m_tick_count);

            // construct tick containers GameObjects
            m_tick_containers = new GameObject[m_tick_count];
            for (int i = 0; i < m_tick_count; ++i)
            {
                float val = m_scale.F(ticks[i]);
                GameObject tick_container = new($"tick_{i}");
                tick_container.transform.parent = m_axis_container.transform;
                tick_container.transform.localPosition = Vector3.Scale(new(val, val, val), axis);
                m_tick_containers[i] = tick_container;
            }
        }

        protected void ConstructTickLines(Vector3 axis /* e.g., x => (1.0f, 0.0f, 0.0f) */)
        {
            // construct tick lines
            m_tick_lines = new LineRenderer[m_tick_count];
            for (int i = 0; i < m_tick_count; ++i)
            {
                float val = m_axis_stroke_width / 2;
                GameObject go = new("tick_line", new Type[] { typeof(LineRenderer) });
                go.transform.SetParent(m_tick_containers[i].transform, worldPositionStays: false);
                LineRenderer tick_line = go.GetComponent<LineRenderer>();
                tick_line.useWorldSpace = false;
                tick_line.positionCount = 2;
                tick_line.SetPositions(new Vector3[] {
                    Vector3.Scale(new Vector3(val, val, val), axis),
                    Vector3.Scale(new(-m_tick_size, -m_tick_size, -m_tick_size), axis)
                });
                tick_line.startWidth = (tick_line.endWidth = m_tick_stroke_width);
                m_tick_stroke_width = tick_line.endWidth;
                tick_line.sharedMaterial = m_tick_mat;
                m_tick_lines[i] = tick_line;
            }
        }

        protected void ConstructTickTexts(Vector3 axis /* e.g., x => (1.0f, 0.0f, 0.0f) */)
        {
            // construct tick texts
            m_tick_texts = new TextMeshPro[m_tick_count];
            InputType[] ticks = m_scale.Ticks(m_tick_count);
            for (int i = 0; i < m_tick_count; ++i)
            {
                GameObject go = new("tick_text", new Type[] { typeof(TMPro.TextMeshPro) });
                go.transform.SetParent(m_tick_containers[i].transform, worldPositionStays: false);
                TMPro.TextMeshPro tick_text = go.GetComponent<TextMeshPro>();
                tick_text.isTextObjectScaleStatic = true;
                tick_text.autoSizeTextContainer = true;
                tick_text.text = m_scale.TickText(ticks[i]);
                tick_text.fontSize = m_tick_fontsize;
                tick_text.color = m_tick_text_color;
                tick_text.ForceMeshUpdate();
                float val = -(m_tick_size + (axis.y * tick_text.renderedHeight / 2)
                    + (axis.x * tick_text.renderedWidth / 2));
                tick_text.transform.localPosition = Vector3.Scale(new Vector3(val, val, val), axis);
                m_tick_texts[i] = tick_text;
            }

            // re-assign because the actual font size that was set might be different
            // from what it was initially set to
            if (m_tick_texts.Length > 0) m_tick_fontsize = m_tick_texts[0].fontSize;
        }

    }

}
