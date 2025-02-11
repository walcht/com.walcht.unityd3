using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityD3
{
    public struct Bin2D
    {
        public Bin2D(float x, float y, float interval_x, float interval_y, int count = 0)
        {
            this.x = x;
            this.y = y;
            this.interval_x = interval_x;
            this.interval_y = interval_y;
            this.count = count;
        }
        public float x;
        public float y;
        public float interval_x;
        public float interval_y;
        public int count;
    }

    public class Bin2D<T, D> where T : IEnumerable<D>
    {
        private Vector2 m_x_domain;
        private Vector2 m_y_domain;
        private Func<D, float> m_x_accessor = null;
        private Func<D, float> m_y_accessor = null;
        private T m_data = default(T);
        private int m_nbr_bins_x = 10;
        private int m_nbr_bins_y = 10;
        private float m_interval_x;
        private float m_interval_y;

        public Bin2D<T, D> DomainX(float min, float max)
        {
            m_x_domain = new Vector2(min, max);
            m_interval_x = (m_x_domain.y - m_x_domain.x) / m_nbr_bins_x;
            return this;
        }

        public Bin2D<T, D> DomainY(float min, float max)
        {
            m_y_domain = new Vector2(min, max);
            m_interval_x = (m_y_domain.y - m_y_domain.x) / m_nbr_bins_y;
            return this;
        }

        public Bin2D<T, D> NbrBinsX(int count)
        {
            m_nbr_bins_x = count;
            m_interval_x = (m_x_domain.y - m_x_domain.x) / m_nbr_bins_x;
            return this;
        }

        public Bin2D<T, D> NbrBinsY(int count)
        {
            m_nbr_bins_y = count;
            m_interval_y = (m_y_domain.y - m_y_domain.x) / m_nbr_bins_y;
            return this;
        }

        public Bin2D<T, D> X(Func<D, float> accessor)
        {
            m_x_accessor = accessor;
            return this;
        }

        public Bin2D<T, D> Y(Func<D, float> accessor)
        {
            m_y_accessor = accessor;
            return this;
        }

        public Bin2D<T, D> Datum(T data)
        {
            m_data = data;
            return this;
        }

        public Bin2D<T, D> Generate(out Bin2D[] data)
        {
            if ((m_x_accessor == null) || (m_y_accessor == null))
            {
                throw new ArgumentNullException("accessor is null!");
            }
            // initialize the bins
            int total_nbr_bins = m_nbr_bins_x * m_nbr_bins_y;
            data = new Bin2D[total_nbr_bins];
            for (int j = 0; j < m_nbr_bins_y; ++j)
            {
                for (int i = 0; i < m_nbr_bins_x; ++i)
                {
                    // row-wise layout
                    data[j * m_nbr_bins_x + i] = new Bin2D(
                        x: m_x_domain.x + (m_interval_x / 2) + i * m_interval_x,
                        y: m_y_domain.x + (m_interval_y / 2) + j * m_interval_y,
                        interval_x: m_interval_x,
                        interval_y: m_interval_y
                    );
                }
            }

            // bin the given data into the previously created bins
            foreach (D d in m_data)
            {
                float vx = m_x_accessor(d);
                float vy = m_y_accessor(d);
                if ((vx < m_x_domain.x) || (vx >= m_x_domain.y)
                    || (vy < m_y_domain.x) || (vy >= m_y_domain.y)) continue;
                int bin_idx = Mathf.FloorToInt((vy - m_y_domain.x) / m_interval_y) * m_nbr_bins_x
                    + Mathf.FloorToInt((vx - m_x_domain.x) / m_interval_x);
                ++data[bin_idx].count;
            }
            return this;
        }

        public float GetWidthX() => m_interval_x;
        public float GetWidthY() => m_interval_y;

    }

}