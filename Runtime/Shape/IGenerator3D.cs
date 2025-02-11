using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public abstract class IGenerator3D<T, D> where T : IEnumerable<D>
    {
        protected GameObject m_container;
        protected T m_data;
        protected Func<D, float> m_x_accessor;
        protected Func<D, float> m_y_accessor;
        protected Func<D, float> m_z_accessor;
        protected Func<D, bool> m_filter;

        public IGenerator3D<T, D> Datum(T data)
        {
            m_data = data;
            return this;
        }

        public IGenerator3D<T, D> X(Func<D, float> accessor)
        {
            m_x_accessor = accessor;
            return this;
        }

        public IGenerator3D<T, D> Y(Func<D, float> accessor)
        {
            m_y_accessor = accessor;
            return this;
        }

        public IGenerator3D<T, D> Z(Func<D, float> accessor)
        {
            m_z_accessor = accessor;
            return this;
        }

        public IGenerator3D<T, D> Filter(Func<D, bool> filter)
        {
            m_filter = filter;
            return this;
        }

        public IGenerator3D<T, D> Attach(GameObject game_object)
        {
            m_container.transform.SetParent(game_object.transform, worldPositionStays: false);
            m_container.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            return this;
        }

        public abstract IGenerator3D<T, D> Generate();

        public abstract IGenerator3D<T, D> SetMaterial(Material mat);

        public abstract IGenerator3D<T, D> SetColor(Color color);

        public abstract IGenerator3D<T, D> SetStrokeWidth(float stroke_width);
    }
}