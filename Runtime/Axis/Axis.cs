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

        protected float m_axis_stroke_width = 0.0175f;
        protected float m_tick_stroke_width = 0.0175f;

        protected int m_tick_count = 0;
        protected GameObject[] m_tick_containers = new GameObject[0];
        protected LineRenderer[] m_tick_lines = new LineRenderer[0];
        protected TextMeshPro[] m_tick_texts = new TextMeshPro[0];
        protected Material m_tick_mat;

        /// <summary>
        ///     Number of ticks to generate for this axis.
        /// </summary>
        /// <param name="count">To disable ticks, set this to a value stricly less than 2</param>
        /// <returns>the axis on which this method was called</returns>
        public abstract Axis<InputType> SetTickCount(int count);

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

        protected abstract void ConstructMainLine();

        protected void OnScaleDomainChange(InputType x0, InputType x1)
        {
            ConstructMainLine();
            SetTickCount(m_tick_count);
        }

        protected void OnScaleRangeChange(float y0, float y1)
        {
            ConstructMainLine();
            SetTickCount(m_tick_count);
        }

    }

}
