using System;
using UnityEngine;

namespace UnityD3
{
    public class ScaleLinearFloatFloat : IScaleContinuous<float, float>
    {
        /// <summary>
        ///     Constructs a new linear scale with the specified float domain [x0, x1] and float range [y0, y1]
        /// </summary>
        /// <param name="x0">domain's left extremety</param>
        /// <param name="x1">domain's right extremety</param>
        /// <param name="y0">range's left extremety</param>
        /// <param name="y1">range's right extremety</param>
        public ScaleLinearFloatFloat(float x0, float x1, float y0, float y1)
        {
            this.Domain(x0, x1)
                .Range(y0, y1);
            m_format = "N2";
        }

        public override float F(float x) => Mathf.Lerp(m_y0, m_y1, Mathf.Clamp01((x - m_x0) / (m_x1 - m_x0)));

        public override float I(float y) => Mathf.Lerp(m_x0, m_x1, Mathf.Clamp01((y - m_y0) / (m_y1 - m_y0)));

        public override float[] Ticks(int count)
        {
            if (count <= 1) return new float[] { };
            float[] ticks = new float[count];
            float step = Mathf.Abs(m_x1 - m_x0) / (ticks.Length - 1);
            for (int i = 0; i < ticks.Length; ++i)
            {
                ticks[i] = m_x0 + i * step;
            }
            return ticks;
        }
    }

    public class ScaleLinearIntFloat : IScaleContinuous<int, float>
    {
        /// <summary>
        ///     Constructs a new linear scale with the specified integer domain [x0, x1] and float range [y0, y1]
        /// </summary>
        /// <param name="x0">domain's left extremety</param>
        /// <param name="x1">domain's right extremety</param>
        /// <param name="y0">range's left extremety</param>
        /// <param name="y1">range's right extremety</param>
        public ScaleLinearIntFloat(int x0, int x1, float y0, float y1)
        {
            this.Domain(x0, x1)
                .Range(y0, y1);
            m_format = "N0";
        }

        public override float F(int x) => Mathf.Lerp(m_y0, m_y1, Mathf.Clamp01((x - m_x0) / (float)(m_x1 - m_x0)));

        public override int I(float y) => (int)Mathf.Lerp(m_x0, m_x1, Mathf.Clamp01((y - m_y0) / (m_y1 - m_y0)));

        public override int[] Ticks(int count)
        {
            if (count <= 1) return new int[] { };
            // walk down until we find a suitable count that divides the domain
            while ((Mathf.Abs(m_x1 - m_x0) % (count - 1)) != 0) --count;
            int[] ticks = new int[count];
            int step = Mathf.RoundToInt(Mathf.Abs(m_x1 - m_x0) / (ticks.Length - 1));
            for (int i = 0; i < ticks.Length; ++i)
            {
                ticks[i] = m_x0 + i * step;
            }
            return ticks;
        }
    }


    /// <summary>
    ///     Time scales are a variant of linear scales that have a temporal domain. Time scales implement ticks based
    ///     on calendar intervals, taking the pain out of generating axes for temporal domains.
    /// </summary>
    public class ScaleLinearTimeFloat : IScaleContinuous<DateTime, float>
    {
        private double m_input_diff;

        /// <summary>
        ///     Constructs a new linear scale with the specified DateTime domain [x0, x1] and float range [y0, y1]
        /// </summary>
        /// <param name="x0">domain's left extremety</param>
        /// <param name="x1">domain's right extremety</param>
        /// <param name="y0">range's left extremety</param>
        /// <param name="y1">range's right extremety</param>
        public ScaleLinearTimeFloat(DateTime x0, DateTime x1, float y0, float y1)
        {
            this.Domain(x0, x1)
                .Range(y0, y1);
            m_input_diff = (m_x1 - m_x0).TotalMilliseconds;
        }

        public override IScaleContinuous<DateTime, float> Domain(DateTime x0, DateTime x1)
        {
            m_x0 = x0;
            m_x1 = x1;
            m_input_diff = (m_x1 - m_x0).TotalMilliseconds;
            return this;
        }

        public override float F(DateTime x)
        {
            // get difference to
            double diff = (x - m_x0).TotalMilliseconds;
            return Mathf.Lerp(m_y0, m_y1, Mathf.Clamp01((float)(diff / m_input_diff)));
        }

        public override DateTime I(float y)
        {
            throw new NotImplementedException();
        }

        public override DateTime[] Ticks(int count)
        {
            if (count <= 1) return new DateTime[] { };
            DateTime[] ticks = new DateTime[count];
            double step = m_input_diff / (ticks.Length - 1);
            for (int i = 0; i < ticks.Length; ++i)
            {
                ticks[i] = m_x0 + TimeSpan.FromMilliseconds(step * i);
            }
            return ticks;
        }
    }
}