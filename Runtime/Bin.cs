using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public struct Bin
    {
        public Bin(float x, float interval, int count = 0)
        {
            this.x = x;
            this.interval = interval;
            this.count = count;
        }
        public float x;
        public float interval;
        public int count;
    }

    public class Bin<T, D> where T : IEnumerable<D>
    {
        private int m_threshold = 10;
        private float m_domain_min;
        private float m_domain_max;
        private Func<D, float> m_accessor = null;
        private T m_data = default(T);
        private float m_interval;

        public Bin<T, D> Domain(float min, float max)
        {
            m_domain_min = min;
            m_domain_max = max;
            m_interval = (m_domain_max - m_domain_min) / m_threshold;
            return this;
        }

        public Bin<T, D> Threshold(int threshold)
        {
            m_threshold = threshold;
            m_interval = (m_domain_max - m_domain_min) / m_threshold;
            return this;
        }

        public Bin<T, D> Value(Func<D, float> accessor)
        {
            m_accessor = accessor;
            return this;
        }

        public Bin<T, D> Datum(T data)
        {
            m_data = data;
            return this;
        }

        public Bin<T, D> Generate(out Bin[] data)
        {
            if (m_accessor == null)
            {
                throw new ArgumentNullException("accessor is null!");
            }
            // initialize the bins
            data = new Bin[m_threshold];
            for (int i = 0; i < m_threshold; ++i)
            {
                // x should be in the middle of the interval
                data[i] = new Bin(
                    x: m_domain_min + (m_interval / 2) + i * m_interval,
                    interval: m_interval
                );
            }

            // bin the given data into the previously created bins
            foreach (D d in m_data)
            {
                float v = m_accessor(d);
                if ((v < m_domain_min) || (v >= m_domain_max)) continue;
                int i = Mathf.FloorToInt((v - m_domain_min) / m_interval);
                ++data[i].count;
            }
            return this;
        }

        public float GetWidth() => m_interval;

    }

}