using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public abstract class IGenerator2D<T, D> where T : IEnumerable<D>
    {
        protected GameObject m_container;
        protected T m_data;
        protected Func<D, float> m_x_accessor;
        protected Func<D, float> m_y_accessor;

        protected Func<D, float> m_width_accessor;
        protected Func<D, float> m_height_accessor;
        protected Func<D, bool> m_filter;
        protected PrimitiveShape2D m_shape = PrimitiveShape2D.CIRCLE;
        protected bool m_dirty = true;

        public IGenerator2D<T, D> Datum(T data)
        {
            m_data = data;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> X(Func<D, float> accessor)
        {
            m_x_accessor = accessor;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> Y(Func<D, float> accessor)
        {
            m_y_accessor = accessor;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> Width(Func<D, float> accessor)
        {
            m_width_accessor = accessor;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> Height(Func<D, float> accessor)
        {
            m_height_accessor = accessor;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> SetShape(PrimitiveShape2D shape)
        {
            m_shape = shape;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> Filter(Func<D, bool> filter)
        {
            m_filter = filter;
            m_dirty = true;
            return this;
        }

        public IGenerator2D<T, D> Attach(GameObject game_object)
        {
            m_container.transform.SetParent(game_object.transform, worldPositionStays: false);
            m_container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            return this;
        }

        /// <summary>
        ///     Tries to regenerate the primitive(s) if the internal dirty flag is set. Should be called initially or
        ///     inside a GameObject's Update callback.
        /// </summary>
        /// <returns>the 2D generator this method was called on</returns>
        public abstract IGenerator2D<T, D> Update();


        /// <summary>
        ///     Forces the regeneration of the primitive(s) even if the internal dirty flag is not set. This is useful
        ///     for instance when one of the accessors' (e.g, X, Y, height, etc.) return value has changed but not the
        ///     accessor itself.
        /// </summary>
        /// <returns>the 2D generator this method was called on</returns>
        public IGenerator2D<T, D> ForceUpdate()
        {
            m_dirty = true;
            Update();
            return this;
        }

        public abstract IGenerator2D<T, D> SetMaterial(Material mat);

        public abstract IGenerator2D<T, D> SetColor(Color color);

        public abstract IGenerator2D<T, D> SetStrokeWidth(float stroke_width);
    }
}